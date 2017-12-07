using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways {
    internal class PayPlanConnector : RestGateway, IRecurringService {
        private string secretApiKey;
        public string SecretApiKey {
            get { return secretApiKey; }
            set {
                secretApiKey = value;
                var auth = string.Format("Basic {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(value ?? string.Empty)));
                Headers.Add("Authorization", auth);
            }
        }
        public bool SupportsRetrieval { get { return true; } }
        public bool SupportsUpdatePaymentDetails { get { return false; } }

        public PayPlanConnector() { }

        public T ProcessRecurring<T>(RecurringBuilder<T> builder) where T : class {
            var request = new JsonDoc();

            if (builder.TransactionType == TransactionType.Create || builder.TransactionType == TransactionType.Edit) {
                if (builder.Entity is Customer) {
                    BuildCustomer(request, builder.Entity as Customer);
                }
                else if (builder.Entity is RecurringPaymentMethod) {
                    BuildPaymentMethod(request, builder.Entity as RecurringPaymentMethod, builder.TransactionType);
                }
                else if (builder.Entity is Schedule) {
                    BuildSchedule(request, builder.Entity as Schedule, builder.TransactionType);
                }
            }
            else if (builder.TransactionType == TransactionType.Search) {
                foreach (var entry in builder.SearchCriteria) {
                    request.Set(entry.Key, entry.Value);
                }
            }

            var response = DoTransaction(MapMethod(builder.TransactionType), MapUrl(builder), request.ToString());
            return MapResponse<T>(response);
        }

        #region Mappers
        private T MapResponse<T>(string rawResponse) where T : class {
            // this is for DELETE which returns nothing
            if (string.IsNullOrEmpty(rawResponse))
                return null;

            // else do the whole shebang
            var response = JsonDoc.Parse(rawResponse);

            var type = Activator.CreateInstance<T>();
            if (type is Customer) {
                return HydrateCustomer(response) as T;
            }
            else if (type is IEnumerable<Customer>) {
                var customers = new List<Customer>();
                foreach (var customer in response.GetEnumerator("results")) {
                    customers.Add(HydrateCustomer(customer));
                }
                return customers as T;
            }
            else if (type is RecurringPaymentMethod)
                return HydratePaymentMethod(response) as T;
            else if (type is IEnumerable<RecurringPaymentMethod>) {
                var methods = new List<RecurringPaymentMethod>();
                foreach (var method in response.GetEnumerator("results")) {
                    methods.Add(HydratePaymentMethod(method));
                }
                return methods as T;
            }
            if (type is Schedule)
                return HydrateSchedule(response) as T;
            else if (type is IEnumerable<Schedule>) {
                var schedules = new List<Schedule>();
                foreach (var schedule in response.GetEnumerator("results")) {
                    schedules.Add(HydrateSchedule(schedule));
                }
                return schedules as T;
            }
            return type;
        }

        private HttpMethod MapMethod(TransactionType type) {
            switch (type) {
                case TransactionType.Create:
                case TransactionType.Search:
                    return HttpMethod.Post;
                case TransactionType.Edit:
                    return HttpMethod.Put;
                case TransactionType.Delete:
                    return HttpMethod.Delete;
                default:
                    return HttpMethod.Get;
            }
        }

        private string MapUrl<T>(RecurringBuilder<T> builder) where T : class {
            var suffix = string.Empty;
            if (builder.TransactionType == TransactionType.Fetch
                || builder.TransactionType == TransactionType.Delete
                || builder.TransactionType == TransactionType.Edit)
                suffix = "/" + builder.Entity.Key;

            var type = Activator.CreateInstance<T>();
            if (type is Customer || type is IEnumerable<Customer>)
                return string.Format("{0}{1}", (builder.TransactionType == TransactionType.Search) ? "searchCustomers" : "customers", suffix);
            else if (type is RecurringPaymentMethod || type is IEnumerable<RecurringPaymentMethod>) {
                string paymentMethod = string.Empty;
                if (builder.TransactionType == TransactionType.Create)
                    paymentMethod = ((builder.Entity as RecurringPaymentMethod).PaymentMethod is Credit) ? "CreditCard" : "ACH";
                else if (builder.TransactionType == TransactionType.Edit)
                    paymentMethod = (builder.Entity as RecurringPaymentMethod).PaymentType.Replace(" ", "");
                return string.Format("{0}{1}{2}", (builder.TransactionType == TransactionType.Search) ? "searchPaymentMethods" : "paymentMethods", paymentMethod, suffix);
            }
            else if (type is Schedule || type is IEnumerable<Schedule>) {
                return string.Format("{0}{1}", (builder.TransactionType == TransactionType.Search) ? "searchSchedules" : "schedules", suffix);
            }

            throw new UnsupportedTransactionException();
        }
        #endregion

        #region Build Entities
        private JsonDoc BuildCustomer(JsonDoc request, Customer customer) {
            if (customer != null) {
                request.Set("customerIdentifier", customer.Id);
                request.Set("firstName", customer.FirstName);
                request.Set("lastName", customer.LastName);
                request.Set("company", customer.Company);
                request.Set("customerStatus", customer.Status);
                request.Set("primaryEmail", customer.Email);
                request.Set("phoneDay", customer.HomePhone);
                request.Set("phoneEvening", customer.WorkPhone);
                request.Set("phoneMobile", customer.MobilePhone);
                request.Set("fax", customer.Fax);
                request.Set("title", customer.Title);
                request.Set("department", customer.Department);
                BuildAddress(request, customer.Address);
            }
            return request;
        }

        private JsonDoc BuildPaymentMethod(JsonDoc request, RecurringPaymentMethod payment, TransactionType type) {
            if (payment != null) {
                request.Set("preferredPayment", payment.PreferredPayment);
                request.Set("paymentMethodIdentifier", payment.Id);
                request.Set("customerKey", payment.CustomerKey);
                request.Set("nameOnAccount", payment.NameOnAccount);
                BuildAddress(request, payment.Address);

                if (type == TransactionType.Create) {
                    string tokenValue = null;
                    var hasToken = HasToken(payment.PaymentMethod, out tokenValue);
                    JsonDoc paymentInfo = null;
                    if (payment.PaymentMethod is ICardData) {
                        var method = payment.PaymentMethod as ICardData;
                        paymentInfo = request.SubElement(hasToken ? "alternateIdentity" : "card")
                            .Set("type", hasToken ? "SINGLEUSETOKEN" : null)
                            .Set(hasToken ? "token" : "number", hasToken ? tokenValue : method.Number)
                            .Set("expMon", method.ExpMonth)
                            .Set("expYear", method.ExpYear);
                        request.Set("cardVerificationValue", method.Cvn);
                    }
                    else if (payment.PaymentMethod is ITrackData) {
                        var method = payment.PaymentMethod as ITrackData;
                        paymentInfo = request.SubElement("track")
                            .Set("data", method.Value)
                            .Set("dataEntryMode", method.EntryMethod.ToString().ToUpper());
                    }
                    else if (payment.PaymentMethod is eCheck) {
                        var check = payment.PaymentMethod as eCheck;
                        request.Set("achType", check.AccountType.ToInitialCase())
                            .Set("accountType", check.CheckType.ToInitialCase())
                            .Set("telephoneIndicator", (check.SecCode == SecCode.CCD || check.SecCode == SecCode.PPD) ? false : true)
                            .Set("routingNumber", check.RoutingNumber)
                            .Set("accountNumber", check.AccountNumber)
                            .Set("accountHolderYob", check.BirthYear.ToString())
                            .Set("driversLicenseState", check.DriversLicenseState)
                            .Set("driversLicenseNumber", check.DriversLicenseNumber)
                            .Set("socialSecurityNumberLast4", check.SsnLast4);
                        request.Remove("country");
                    }

                    if (payment.PaymentMethod is IEncryptable) {
                        var enc = (payment.PaymentMethod as IEncryptable).EncryptionData;
                        if (enc != null) {
                            paymentInfo.Set("trackNumber", enc.TrackNumber);
                            paymentInfo.Set("key", enc.KTB);
                            paymentInfo.Set("encryptionType", "E3");
                        }
                    }
                }
                else { // EDIT FIELDS
                    request.Remove("customerKey");
                    request.Set("paymentStatus", payment.Status);
                    request.Set("cpcTaxType", payment.TaxType);
                    request.Set("expirationDate", payment.ExpirationDate);
                }
            }
            return request;
        }

        private JsonDoc BuildSchedule(JsonDoc request, Schedule schedule, TransactionType type) {
            Func<string> mapDuration = () => {
                if (schedule.NumberOfPayments != null)
                    return "Limited Number";
                else if (schedule.EndDate != null)
                    return "End Date";
                else return "Ongoing";
            };

            Func<string> mapProcessingDate = () => {
                var frequencies = new List<string> { "Monthly", "Bi-Monthly", "Quarterly", "Semi-Annually" };
                if (frequencies.Contains(schedule.Frequency)) {
                    switch (schedule.PaymentSchedule) {
                        case PaymentSchedule.FirstDayOfTheMonth:
                            return "First";
                        case PaymentSchedule.LastDayOfTheMonth:
                            return "Last";
                        default: {
                                var day = schedule.StartDate.Value.Day;
                                if (day > 28)
                                    return "Last";
                                return day.ToString();
                            }
                    }
                }
                else if (schedule.Frequency == "Semi-Monthly") {
                    if (schedule.PaymentSchedule == PaymentSchedule.LastDayOfTheMonth)
                        return "Last";
                    return "First";
                }
                return null;
            };

            if (schedule != null) {
                request.Set("scheduleIdentifier", schedule.Id);
                request.Set("scheduleName", schedule.Name);
                request.Set("scheduleStatus", schedule.Status);
                request.Set("paymentMethodKey", schedule.PaymentKey);

                BuildAmount(request, "subtotalAmount", schedule.Amount, schedule.Currency, type);
                BuildAmount(request, "taxAmount", schedule.TaxAmount, schedule.Currency, type);

                request.Set("deviceId", schedule.DeviceId);
                request.Set("processingDateInfo", mapProcessingDate());
                BuildDate(request, "endDate", schedule.EndDate, (type == TransactionType.Edit));
                request.Set("reprocessingCount", schedule.ReprocessingCount ?? 3);
                request.Set("emailReceipt", schedule.EmailReceipt.ToString());
                request.Set("emailAdvanceNotice", schedule.EmailNotification ? "Yes" : "No");
                // debt repay ind
                request.Set("invoiceNbr", schedule.InvoiceNumber);
                request.Set("poNumber", schedule.PoNumber);
                request.Set("description", schedule.Description);
                request.Set("numberOfPayments", schedule.NumberOfPayments);

                if (type == TransactionType.Create) {
                    request.Set("customerKey", schedule.CustomerKey);
                    BuildDate(request, "startDate", schedule.StartDate);
                    request.Set("frequency", schedule.Frequency);
                    request.Set("duration", mapDuration());
                }
                else { // Edit Fields
                    if (!schedule.HasStarted) {
                        BuildDate(request, "startDate", schedule.StartDate);
                        request.Set("frequency", schedule.Frequency);
                        request.Set("duration", mapDuration());
                    }
                    else {
                        BuildDate(request, "cancellationDate", schedule.CancellationDate);
                        BuildDate(request, "nextProcressingDate", schedule.NextProcessingDate);
                    }
                }
            }
            return request;
        }

        private JsonDoc BuildDate(JsonDoc request, string name, DateTime? date, bool force = false) {
            if (date.HasValue || force) {
                var value = (date.HasValue) ? date.Value.ToString("MMddyyyy") : null;
                request.Set(name, value, force);
            }
            return request;
        }

        private JsonDoc BuildAmount(JsonDoc request, string name, decimal? amount, string currency, TransactionType type) {
            if (amount.HasValue) {
                var node = request.SubElement(name);
                node.Set("value", amount.ToNumericCurrencyString());
                if (type == TransactionType.Create)
                    node.Set("currency", currency);
            }
            return request;
        }

        private JsonDoc BuildAddress(JsonDoc request, Address address) {
            if (address != null) {
                request.Set("addressLine1", address.StreetAddress1);
                request.Set("addressLine2", address.StreetAddress2);
                request.Set("city", address.City);
                request.Set("country", address.Country);
                request.Set("stateProvince", address.State);
                request.Set("zipPostalCode", address.PostalCode);
            }
            return request;
        }
        #endregion

        #region Hydrate Entities
        private Customer HydrateCustomer(JsonDoc response) {
            var customer = new Customer {
                Key = response.GetValue<string>("customerKey"),
                Id = response.GetValue<string>("customerIdentifier"),
                FirstName = response.GetValue<string>("firstName"),
                LastName = response.GetValue<string>("lastName"),
                Company = response.GetValue<string>("company"),
                Status = response.GetValue<string>("customerStatus"),
                Title = response.GetValue<string>("title"),
                Department = response.GetValue<string>("department"),
                Email = response.GetValue<string>("primaryEmail"),
                HomePhone = response.GetValue<string>("phoneDay"),
                WorkPhone = response.GetValue<string>("phoneEvening"),
                MobilePhone = response.GetValue<string>("phoneMobile"),
                Fax = response.GetValue<string>("fax"),
                Address = new Address {
                    StreetAddress1 = response.GetValue<string>("addressLine1"),
                    StreetAddress2 = response.GetValue<string>("addressLine2"),
                    City = response.GetValue<string>("city"),
                    Province = response.GetValue<string>("stateProvince"),
                    PostalCode = response.GetValue<string>("zipPostalCode"),
                    Country = response.GetValue<string>("country")
                }
            };

            if (response.Has("paymentMethods")) {
                customer.PaymentMethods = new List<RecurringPaymentMethod>();
                foreach (JsonDoc paymentResponse in response.GetEnumerator("paymentMethods")) {
                    var paymentMethod = HydratePaymentMethod(paymentResponse);
                    if (paymentMethod != null)
                        customer.PaymentMethods.Add(paymentMethod);
                }
            }

            return customer;
        }

        private RecurringPaymentMethod HydratePaymentMethod(JsonDoc response) {
            return new RecurringPaymentMethod {
                Key = response.GetValue<string>("paymentMethodKey"),
                PaymentType = response.GetValue<string>("paymentMethodType"),
                PreferredPayment = response.GetValue<bool>("preferredPayment"),
                Status = response.GetValue<string>("paymentStatus"),
                Id = response.GetValue<string>("paymentMethodIdentifier"),
                CustomerKey = response.GetValue<string>("customerKey"),
                NameOnAccount = response.GetValue<string>("nameOnAccount"),
                CommercialIndicator = response.GetValue<string>("cpcInd"),
                TaxType = response.GetValue<string>("cpcTaxType"),
                ExpirationDate = response.GetValue<string>("expirationDate"),
                Address = new Address {
                    StreetAddress1 = response.GetValue<string>("addressLine1"),
                    StreetAddress2 = response.GetValue<string>("addressLine2"),
                    City = response.GetValue<string>("city"),
                    State = response.GetValue<string>("stateProvince"),
                    PostalCode= response.GetValue<string>("zipPostalCode"),
                    Country = response.GetValue<string>("country")
                }
            };
        }

        private Schedule HydrateSchedule(JsonDoc response) {
            var schedule = new Schedule();
            schedule.Key = response.GetValue<string>("scheduleKey");
            schedule.Id = response.GetValue<string>("scheduleIdentifier");
            schedule.CustomerKey = response.GetValue<string>("customerKey");
            schedule.Name = response.GetValue<string>("scheduleName");
            schedule.Status = response.GetValue<string>("scheduleStatus");
            schedule.PaymentKey = response.GetValue<string>("paymentMethodKey");
            if (response.Has("subtotalAmount")) {
                var subtotal = response.Get("subtotalAmount");
                schedule.Amount = subtotal.GetValue("value", AmountConverter);
                schedule.Currency = subtotal.GetValue<string>("currency");
            }
            if (response.Has("taxAmount")) {
                var taxAmount = response.Get("taxAmount");
                schedule.TaxAmount = taxAmount.GetValue("value", AmountConverter);
            }
            schedule.DeviceId = response.GetValue<int>("DeviceId");
            schedule.StartDate = response.GetValue("startDate", DateConverter);
            schedule.PaymentSchedule = response.GetValue("processingDateInfo", (value) => {
                if (value == null) return PaymentSchedule.Dynamic;
                var str = value.ToString();
                if (str == "Last")
                    return PaymentSchedule.LastDayOfTheMonth;
                else if (str == "First")
                    return PaymentSchedule.FirstDayOfTheMonth;
                return PaymentSchedule.Dynamic;
            });
            schedule.Frequency = response.GetValue<string>("frequency");
            schedule.EndDate = response.GetValue("endDate", DateConverter);
            schedule.ReprocessingCount = response.GetValue<int>("reprocessingCount");
            schedule.EmailReceipt = response.GetValue("emailReceipt", EnumConverter<EmailReceipt>);
            schedule.EmailNotification = response.GetValue("emailAdvanceNotice", (value) => {
                if (value == null)
                    return false;
                return value.ToString() == "No" ? false : true;
            });
            // dept repay indicator
            schedule.InvoiceNumber = response.GetValue<string>("invoiceNbr");
            schedule.PoNumber = response.GetValue<string>("poNumber");
            schedule.Description = response.GetValue<string>("Description");
            // statusSetDate
            schedule.NextProcessingDate = response.GetValue<DateTime?>("nextProcessingDate", DateConverter);
            // previousProcessingDate
            // approvedTransactionCount
            // failureCount
            // totalApprovedAmountToDate
            // numberOfPaymentsRemaining
            schedule.CancellationDate = response.GetValue("cancellationDate", DateConverter);
            // creationDate
            // lastChangeDate
            schedule.HasStarted = response.GetValue<bool>("scheduleStarted");
            return schedule;
        }
        #endregion

        private bool HasToken(IPaymentMethod paymentMethod, out string tokenValue) {
            tokenValue = null;

            if (paymentMethod is ITokenizable && !string.IsNullOrEmpty(((ITokenizable)paymentMethod).Token)) {
                tokenValue = ((ITokenizable)paymentMethod).Token;
                return true;
            }
            return false;
        }

        #region Converters
        private DateTime? DateConverter(object value) {
            if(value != null && !string.IsNullOrEmpty(value.ToString()))
                return DateTime.ParseExact(value.ToString(), "MMddyyyy", null);
            return null;
        }
        private T EnumConverter<T>(object value) {
            return (T)Enum.Parse(typeof(T), value.ToString());
        }
        private decimal? AmountConverter(object value) {
            if(value != null)
                return value.ToString().ToAmount();
            return null;
        }
        #endregion

        #region Validations

        #endregion
    }
}
