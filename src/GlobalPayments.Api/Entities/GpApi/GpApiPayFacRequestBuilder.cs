using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities.PayFac;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class GpApiPayFacRequestBuilder<T> where T : class
    {
        private static PayFacBuilder<T> _builder { get; set; }        
        
        internal static GpApiRequest BuildRequest(PayFacBuilder<T> builder, GpApiConnector gateway )
        {
            _builder = builder;
            var merchantUrl = !string.IsNullOrEmpty(gateway.GpApiConfig.MerchantId) ? $"/merchants/{gateway.GpApiConfig.MerchantId}" : string.Empty;
            switch (builder.TransactionType)
            {               
                case TransactionType.Create:
                    if (builder.TransactionModifier == TransactionModifier.Merchant)
                    {
                        if (builder.UserPersonalData == null)
                        {
                            throw new ArgumentException("Merchant data is mandatory!");
                        }
                        var data = BuildCreateMerchantRequest();

                        return new GpApiRequest
                        {
                            Verb = HttpMethod.Post,
                            Endpoint = $"{merchantUrl}/merchants",
                            RequestBody = data.ToString(),
                        };
                    }
                    break;
                case TransactionType.Edit:
                 if (builder.TransactionModifier == TransactionModifier.Merchant) {
                        return new GpApiRequest
                        {
                            Verb = new HttpMethod("PATCH"),
                            Endpoint = $"{merchantUrl}/merchants/{_builder.UserReference.UserId}",
                            RequestBody = BuildEditMerchantRequest().ToString(),
                        };                         
                    }
                    break;
                case TransactionType.Fetch:
                    if (builder.TransactionModifier == TransactionModifier.Merchant)
                    {
                        return new GpApiRequest
                        {
                            Verb = HttpMethod.Get,
                            Endpoint = $"{merchantUrl}/merchants/{_builder.UserReference.UserId}",                            
                        };
                    }
                    break;
               
                default:
                    break;
            }
           
            return null;
        }

        private static JsonDoc SetMerchantInfo()
        {
            if (_builder.UserPersonalData == null) {
                return new JsonDoc();
            }
            var merchantData = _builder.UserPersonalData;
            var data = new JsonDoc()
                        .Set("name", merchantData.UserName)
                        .Set("legal_name", merchantData.LegalName)                        
                        .Set("dba", merchantData.DBA)
                        .Set("merchant_category_code", merchantData.MerchantCategoryCode)
                        .Set("website", merchantData.Website)                        
                        .Set("currency", merchantData.CurrencyCode)
                        .Set("tax_id_reference", merchantData.TaxIdReference)
                        .Set("notification_email", merchantData.NotificationEmail)
                        .Set("status",_builder.UserReference?.UserStatus.ToString() ?? null);
            
            var notifications = new JsonDoc()
               .Set("status_url", merchantData.NotificationStatusUrl);

            if (notifications.HasKeys()) {
                data.Set("notifications", notifications);
            }
            
            return data;
        }

        private static JsonDoc BuildCreateMerchantRequest()
        {
            var merchantData = _builder.UserPersonalData;
            var data = SetMerchantInfo();
            data.Set("description", _builder.Description)
                .Set("type", merchantData.Type.ToString())
                .Set("addresses", SetAddressList())                
                .Set("payment_processing_statistics", SetPaymentStatistics());
            var tier = new JsonDoc()
              .Set("reference", merchantData.Tier);

            data.Set("tier", tier)
                .Set("payment_methods", SetPaymentMethod())
                .Set("persons", SetPersonList())
                .Set("products", _builder.ProductData?.Count > 0 ? SetProductList(_builder.ProductData) : null);

            return data;
        }

        private static JsonDoc SetPaymentStatistics()
        {
            if (_builder.PaymentStatistics == null) {
                return null;
            }

            return new JsonDoc()
                .Set("total_monthly_sales_amount", _builder.PaymentStatistics.TotalMonthlySalesAmount.ToNumericCurrencyString())
                .Set("average_ticket_sales_amount", _builder.PaymentStatistics.AverageTicketSalesAmount.ToNumericCurrencyString())
                .Set("highest_ticket_sales_amount", _builder.PaymentStatistics.HighestTicketSalesAmount.ToNumericCurrencyString());          
        }

        private static List<Dictionary<string, object>> SetPersonList(string type = null)
        {            
            if (_builder.PersonsData?.Count == 0 || _builder.PersonsData == null) {
                return null;
            }
            var personInfo = new List<Dictionary<string, object>>();
            foreach (var person in _builder.PersonsData) {
                var item = new Dictionary<string, object>();
                item.Add("functions", new string[] { person.Functions.ToString() });
                item.Add("first_name", person.FirstName);
                item.Add("middle_name", person.MiddleName);
                item.Add("last_name", person.LastName);
                item.Add("email", person.Email);
                item.Add("date_of_birth", person.DateOfBirth ?? null);
                item.Add("national_id_reference", person.NationalIdReference);
                item.Add("equity_percentage", person.EquityPercentage);
                item.Add("job_title", person.JobTitle);


                if (person.Address != null && type == null)
                {
                    var address = new Dictionary<string, object>();
                    address.Add("line_1", person.Address.StreetAddress1);
                    address.Add("line_2", person.Address.StreetAddress2);
                    address.Add("line_3", person.Address.StreetAddress3);
                    address.Add("city", person.Address.City);
                    address.Add("state", person.Address.State);
                    address.Add("postal_code", person.Address.PostalCode);
                    address.Add("country", person.Address.CountryCode);

                    item.Add("address", address);
                }
                if (person.HomePhone != null) {
                    var contactPhone = new Dictionary<string, object>();
                    contactPhone.Add("country_code", person.HomePhone.CountryCode);
                    contactPhone.Add("subscriber_number", person.HomePhone.Number);
                    
                    item.Add("contact_phone", contactPhone);
                }
                if (person.WorkPhone != null) {
                    var workPhone = new Dictionary<string, object>();
                    workPhone.Add("country_code", person.WorkPhone.CountryCode);
                    workPhone.Add("subscriber_number", person.WorkPhone.Number);
               
                    item.Add("work_phone", workPhone);
                }
                personInfo.Add(item);
            }

            return personInfo;
        }

        private static Dictionary<string, object> SetBankTransferInfo(BankAccountData bankAccountData)
        {
            var data = new Dictionary<string, object>();
            data.Add("account_holder_type", bankAccountData?.AccountOwnershipType);
            data.Add("account_number", bankAccountData?.AccountNumber);
            data.Add("account_type", bankAccountData?.AccountType);

            var bank = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(bankAccountData?.BankName))            
                bank.Add("name", bankAccountData?.BankName);            
            if (!string.IsNullOrEmpty(bankAccountData?.RoutingNumber))            
                bank.Add("code", bankAccountData?.RoutingNumber); //@TODO confirmantion from GP-API team
            
                bank.Add("international_code", ""); //@TODO
            

            var address = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(bankAccountData?.BankAddress?.StreetAddress1))            
                address.Add("line_1", bankAccountData?.BankAddress?.StreetAddress1);            
            if (!string.IsNullOrEmpty(bankAccountData?.BankAddress?.StreetAddress2))
                address.Add("line_2", bankAccountData?.BankAddress?.StreetAddress2);
            if (!string.IsNullOrEmpty(bankAccountData?.BankAddress?.StreetAddress3))
                address.Add("line_3", bankAccountData?.BankAddress?.StreetAddress3);
            if (!string.IsNullOrEmpty(bankAccountData?.BankAddress?.City))
                address.Add("city", bankAccountData?.BankAddress?.City);
            if (!string.IsNullOrEmpty(bankAccountData?.BankAddress?.PostalCode))
                address.Add("postal_code", bankAccountData?.BankAddress?.PostalCode);
            if (!string.IsNullOrEmpty(bankAccountData?.BankAddress?.State))
                address.Add("state", bankAccountData?.BankAddress?.State);
            address.Add("country", bankAccountData?.BankAddress != null ? CountryUtils.GetCountryCodeByCountry(bankAccountData?.BankAddress?.CountryCode) : "");

            bank.Add("address", address);

            data.Add("bank", bank);                

            return data;
        }

        private static Dictionary<string, object> SetCreditCardInfo(CreditCardData creditCardInformation) {
            var item = new Dictionary<string, object>();
            if(!string.IsNullOrEmpty(creditCardInformation?.CardHolderName))
            item.Add("name", creditCardInformation?.CardHolderName);
            item.Add("number", creditCardInformation?.Number);
            item.Add("expiry_month", creditCardInformation?.ExpMonth);
            item.Add("expiry_year", creditCardInformation?.ExpYear);
            return item;
        }

        private static List<Dictionary<string, object>> SetProductList(List<Product> productData) {
            var products = new List<Dictionary<string, object>>();
            
            foreach (var product in productData) {
                var item = new Dictionary<string, object>();
                item.Add("quantity", product.Quantity);
                item.Add("id", product.ProductId);                
                products.Add(item);
            }
            return products;
        }

        private static List<Dictionary<string, object>> SetPaymentMethod() {
            if(_builder.PaymentMethodsFunctions == null) {
                return null;
            }
            var paymentMethods = new List<Dictionary<string, object>>();
            var item1 = new Dictionary<string, object>();
            item1.Add("functions", new string[] { _builder.PaymentMethodsFunctions?[_builder.CreditCardInformation.GetType().Name].ToString()} ?? null);
            item1.Add("card", SetCreditCardInfo(_builder.CreditCardInformation));
            paymentMethods.Add(item1);

            var item2 = new Dictionary<string, object>();
            item2.Add("functions", new string[] { _builder.PaymentMethodsFunctions?[_builder.BankAccountData.GetType().Name].ToString() } ?? null);
            item2.Add("name", _builder.BankAccountData?.AccountHolderName);
            item2.Add("bank_transfer", SetBankTransferInfo(_builder.BankAccountData));
            paymentMethods.Add(item2);

            return paymentMethods;
        }

        private static List<Dictionary<string, object>> SetAddressList()
        {
            if(_builder.UserPersonalData == null) {
                return null;
            }
            var merchantData = _builder.UserPersonalData;
            var addressList = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(merchantData?.UserAddress?.StreetAddress1)) {                
                addressList.Add(AddressType.Business.ToString(), merchantData.UserAddress);               
            }
            if (!string.IsNullOrEmpty(merchantData?.MailingAddress.StreetAddress1)) {                
                addressList.Add(AddressType.Shipping.ToString(), merchantData.MailingAddress);               
            }
            var addresses = new List<Dictionary<string, object>>();
            foreach (KeyValuePair<string, object> address in addressList) {               
                var item = new Dictionary<string, object>();
                var dataAddress = ((Address)address.Value);
                item.Add("functions", new string[] { address.Key });
                item.Add("line_1", dataAddress.StreetAddress1);
                item.Add("line_2", dataAddress.StreetAddress2);
                item.Add("city", dataAddress.City);
                item.Add("postal_code", dataAddress.PostalCode);
                item.Add("state", dataAddress.State);
                item.Add("country", CountryUtils.GetCountryCodeByCountry(dataAddress.CountryCode));                

                addresses.Add(item);               
            }
                        
            return addresses;
        }

        private static JsonDoc BuildEditMerchantRequest()
        {
            var requestBody = SetMerchantInfo();
            requestBody 
                //.Set("description", _builder.Description)
                .Set("status_change_reason", _builder.StatusChangeReason)
                .Set("addresses", SetAddressList() ?? null)
                .Set("persons", SetPersonList("edit"))
                .Set("payment_processing_statistics", SetPaymentStatistics())
                .Set("payment_methods", SetPaymentMethod());
            
                return requestBody;
        }
    }
}
