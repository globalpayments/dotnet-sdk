using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.PayFac;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GlobalPayments.Api.Gateways {
    internal class ProPayConnector : XmlGateway, IPayFacProvider {
        public string CertStr { get; internal set; }
        public string TermID { get; internal set; }
        public string X509CertPath { get; internal set; }
        public string X509Base64String { get; internal set; }
        public bool HasBuiltInMerchantManagementService => false;

        #region Transaction Handling
        public T ProcessPayFac<T>(PayFacBuilder<T> builder) where T : class {
            UpdateGatewaySettings(builder);

            var et = new ElementTree();
            var request = et.Element("XMLRequest");

            // Credentials
            et.SubElement(request, "certStr", CertStr);
            et.SubElement(request, "termid", TermID);
            et.SubElement(request, "class", "partner");

            //Transaction
            var xmlTrans = et.SubElement(request, "XMLTrans");
            et.SubElement(xmlTrans, "transType", MapRequestType(builder));

            // Account Details
            HydrateAccountDetails(et, xmlTrans, builder);

            var response = DoTransaction(et.ToString(request));
            return MapResponse(builder, response) as T;
        }

        private void UpdateGatewaySettings<T>(PayFacBuilder<T> builder) where T : class {
            var certTransactions = new List<TransactionType>()
            {
                TransactionType.Edit,
                TransactionType.ObtainSSOKey,
                TransactionType.UpdateBankAccountOwnership,
                TransactionType.AddFunds,
                TransactionType.AddCardFlashFunds
            };

            if (certTransactions.Contains(builder.TransactionType)) {
                this.Headers["X509Certificate"] = SetX509Certificate();
            }
        }

        private string SetX509Certificate() {
            try {
                if (!string.IsNullOrEmpty(X509CertPath)) {
                    var cert = new X509Certificate2(X509CertPath);
                    return Convert.ToBase64String(cert.Export(X509ContentType.Cert));
                }
                else if (!string.IsNullOrEmpty(X509Base64String)) {
                    return X509Base64String;
                }
                else
                    throw new BuilderException("X509 Certificate was not provided.");
            }
            catch (Exception e) {
                throw new GatewayException("X509 Certificate Error", e);
            }
        }

        public string MapRequestType<T>(PayFacBuilder<T> builder) where T : class {
            switch (builder.TransactionType) {
                case TransactionType.Create:
                    return "01";
                case TransactionType.Edit:
                    return "42";
                case TransactionType.ResetPassword:
                    return "32";
                case TransactionType.RenewAccount:
                    return "39";
                case TransactionType.UpdateBeneficialOwnership:
                    return "44";
                case TransactionType.Deactivate:
                    return "41";
                case TransactionType.UploadDocumentChargeback:
                    return "46";
                case TransactionType.UploadDocument:
                    return "47";
                case TransactionType.ObtainSSOKey:
                    return "300";
                case TransactionType.UpdateBankAccountOwnership:
                    return "210";
                case TransactionType.AddFunds:
                    return "37";
                case TransactionType.SweepFunds:
                    return "38";
                case TransactionType.AddCardFlashFunds:
                    return "209";
                case TransactionType.PushMoneyFlashFunds:
                    return "45";
                case TransactionType.DisburseFunds:
                    return "02";
                case TransactionType.SpendBack:
                    return "11";
                case TransactionType.Reversal:
                    return "43";
                case TransactionType.SplitFunds:
                    return "16";
                case TransactionType.GetAccountDetails:
                    // We are using the Additional TransactionModifier to differentiate between GetAccountDetails and GetAccountDetailsEnhanced
                    if (builder.TransactionModifier == TransactionModifier.Additional)
                        return "19";
                    // If the TransactionModifier isn't "Additional" then it is either "None" or an unsupported value that should be treated as "None"
                    return "13";
                case TransactionType.Balance:
                    return "14";
                case TransactionType.OrderDevice:
                    return "430";
                default:
                    throw new UnsupportedTransactionException();
            }
        }
        #endregion

        #region Response Handling
        public Transaction MapResponse<T>(PayFacBuilder<T> builder, string rawResponse) where T : class {
            var root = new ElementTree(rawResponse).Get("XMLResponse");
            var responseCode = root.GetValue<string>("status");

            if (responseCode != "00") {
                throw new GatewayException($"Unexpected Gateway Response: {responseCode}", responseCode);
            }

            var proPayResponse = PopulateProPayResponse(builder, root);

            var response = new Transaction() {
                PayFacData = proPayResponse,
                ResponseCode = responseCode
            };

            return response;
        }

        private PayFacResponseData PopulateProPayResponse<T>(PayFacBuilder<T> builder, Element root) where T : class 
        {
            if (builder.TransactionType == TransactionType.GetAccountDetails && builder.TransactionModifier == TransactionModifier.Additional) {
                return PopulateResponseWithEnhancedAccountDetails(root);
            }
            else {
                return new PayFacResponseData() {
                    AccountNumber = GetAccountNumberFromResponse(root),
                    RecAccountNum = root.GetValue<string>("recAccntNum"),
                    Password = root.GetValue<string>("password"),
                    Amount = root.GetValue<string>("amount"),
                    TransNum = root.GetValue<string>("transNum"),
                    Pending = root.GetValue<string>("pending"),
                    SecondaryAmount = root.GetValue<string>("secondaryAmount"),
                    SecondaryTransNum = root.GetValue<string>("secondaryTransNum"),
                    SourceEmail = root.GetValue<string>("sourceEmail"),
                    AuthToken = root.GetValue<string>("AuthToken"),
                    BeneficialOwnerDataResults = GetBeneficialOwnerDataResultsFromResponse(root),
                    AccountStatus = root.GetValue<string>("accntStatus"),
                    PhysicalAddress = GetPhysicalAddressFromResponse(root),
                    Affiliation = root.GetValue<string>("affiliation"),
                    APIReady = root.GetValue<string>("apiReady"),
                    CurrencyCode = root.GetValue<string>("currencyCode"),
                    Expiration = root.GetValue<string>("expiration"),
                    SignupDate = root.GetValue<string>("signupDate"),
                    Tier = root.GetValue<string>("tier"),
                    VisaCheckoutMerchantID = root.GetValue<string>("visaCheckoutMerchantId"),
                    CreditCardTransactionLimit = root.GetValue<string>("CreditCardTransactionLimit"),
                    CreditCardMonthLimit = root.GetValue<string>("CreditCardMonthLimit"),
                    ACHPaymentPerTranLimit = root.GetValue<string>("ACHPaymentPerTranLimit"),
                    ACHPaymentMonthLimit = root.GetValue<string>("ACHPaymentMonthLimit"),
                    CreditCardMonthlyVolume = root.GetValue<string>("CreditCardMonthlyVolume"),
                    ACHPaymentMonthlyVolume = root.GetValue<string>("ACHPaymentMonthlyVolume"),
                    ReserveBalance = root.GetValue<string>("ReserveBalance"),
                    MasterPassCheckoutMerchantID = root.GetValue<string>("MasterPassCheckoutMerchantId"),
                    PendingAmount = root.GetValue<string>("pendingAmount"),
                    ReserveAmount = root.GetValue<string>("reserveAmount>"),
                    ACHOut = GetACHOutBalanceInfoFromResponse(root),
                    FlashFunds = GetFlashFundsBalanceInfoFromResponse(root)
                };
            }
        }

        private PayFacResponseData PopulateResponseWithEnhancedAccountDetails(Element root) {
            return new PayFacResponseData() {
                AccountNumber = GetAccountNumberFromResponse(root),
                PersonalData = new UserPersonalData()
                {
                    SourceEmail = root.GetValue<string>("sourceEmail"),
                    FirstName = root.GetValue<string>("firstName"),
                    MiddleInitial = root.GetValue<string>("middleInitial"),
                    LastName = root.GetValue<string>("lastName"),
                    DayPhone = root.GetValue<string>("dayPhone"),
                    EveningPhone = root.GetValue<string>("evenPhone"),
                    ExternalID = root.GetValue<string>("externalId"),
                    Tier = root.GetValue<string>("tier"),
                    CurrencyCode = root.GetValue<string>("currencyCode"),
                    NotificationEmail = root.GetValue<string>("notificationEmail")
                },
                HomeAddress = new Address()
                {
                    StreetAddress1 = root.GetValue<string>("addr"),
                    StreetAddress2 = root.GetValue<string>("aptNum"),
                    City = root.GetValue<string>("city"),
                    State = root.GetValue<string>("state"),
                    PostalCode = root.GetValue<string>("postalCode"),
                    Country = root.GetValue<string>("country")
                },
                MailAddress = new Address()
                {
                    StreetAddress1 = root.GetValue<string>("mailAddr"),
                    StreetAddress2 = root.GetValue<string>("mailApt"),
                    City = root.GetValue<string>("mailCity"),
                    State = root.GetValue<string>("mailState"),
                    PostalCode = root.GetValue<string>("mailPostalCode"),
                    Country = root.GetValue<string>("mailCountry")
                },
                BusinessData = new BusinessData()
                {
                    BusinessLegalName = root.GetValue<string>("businessLegalName"),
                    DoingBusinessAs = root.GetValue<string>("doingBusinessAs"),
                    EmployerIdentificationNumber = root.GetValue<string>("ein"),
                    BusinessAddress = new Address()
                    {
                        StreetAddress1 = root.GetValue<string>("businessAddress"),
                        StreetAddress2 = root.GetValue<string>("businessAddress2"),
                        City = root.GetValue<string>("businessCity"),
                        State = root.GetValue<string>("businessState"),
                        PostalCode = root.GetValue<string>("businessZip")
                    },
                    WebsiteURL = root.GetValue<string>("websiteURL"),
                    AverageTicket = root.GetValue<string>("averageTicket"),
                    HighestTicket = root.GetValue<string>("highestTicket")
                },
                AccountLimits = new AccountPermissions()
                {
                    CreditCardTransactionLimit = root.GetValue<string>("creditCardTransactionLimit"),
                    CreditCardMonthLimit = root.GetValue<string>("creditCardMonthLimit"),
                    ACHPaymentSoftLimitEnabled = root.GetValue<string>("achPaymentSoftLimitEnabled")?.ToUpper() == "Y" ? true : false,
                    ACHPaymentACHOffPercent = root.GetValue<string>("achPaymentAchOffPercent"),
                    SoftLimitEnabled = root.GetValue<string>("softLimitEnabled")?.ToUpper() == "Y" ? true : false,
                    SoftLimitACHOffPercent = root.GetValue<string>("softLimitAchOffPercent")
                },
                ACHPaymentPerTranLimit = root.GetValue<string>("achPaymentPerTranLimit"),
                ACHPaymentMonthLimit = root.GetValue<string>("achPaymentMonthLimit"),
                ACHPaymentMonthlyVolume = root.GetValue<string>("achPaymentMonthlyVolume"),
                CreditCardMonthlyVolume = root.GetValue<string>("creditCardMonthlyVolume"),

                AvailableBalance = root.GetValue<string>("availableBalance"),
                PendingBalance = root.GetValue<string>("pendingBalance"),
                ReserveBalance = root.GetValue<string>("reserveBalance"),
                PrimaryBankAccountData = new BankAccountData()
                {
                    AccountCountryCode = root.GetValue<string>("primaryAccountCountryCode"),
                    AccountType = root.GetValue<string>("primaryAccountType"),
                    AccountOwnershipType = root.GetValue<string>("primaryAccountOwnershipType"),
                    BankName = root.GetValue<string>("primaryBankName"),
                    AccountNumber = root.GetValue<string>("primaryAccountNumberLast4"),
                    RoutingNumber = root.GetValue<string>("primaryRoutingNumber")
                },
                SecondaryBankAccountData = new BankAccountData()
                {
                    AccountCountryCode = root.GetValue<string>("secondaryAccountCountryCode"),
                    AccountType = root.GetValue<string>("secondaryAccountType"),
                    AccountOwnershipType = root.GetValue<string>("secondaryAccountOwnershipType"),
                    BankName = root.GetValue<string>("secondaryBankName"),
                    AccountNumber = root.GetValue<string>("secondaryAccountNumberLast4"),
                    RoutingNumber = root.GetValue<string>("secondaryRoutingNumber")
                },
                GrossBillingInformation = new GrossBillingInformation()
                {
                    GrossSettleBankData = new BankAccountData()
                    {
                        AccountHolderName = root.GetValue<string>("grossSettleAccountHolderName"),
                        AccountNumber = root.GetValue<string>("grossSettleAccountNumberLast4"),
                        RoutingNumber = root.GetValue<string>("grossSettleRoutingNumber"),
                        AccountType = root.GetValue<string>("grossSettleAccountType")
                    },
                    GrossSettleAddress = new Address()
                    {
                        StreetAddress1 = root.GetValue<string>("grossSettleAccountAddress"),
                        City = root.GetValue<string>("grossSettleAccountCity"),
                        State = root.GetValue<string>("grossSettleAccountState"),
                        CountryCode = root.GetValue<string>("grossSettleAccountCountryCode"),
                        PostalCode = root.GetValue<string>("grossSettleAccountZipCode")
                    }
                }
            };
        }

        private string GetAccountNumberFromResponse(Element root) {
            // ProPay API 4.1 (Create an account) has the account number specified in the response as "accntNum"
            // All other methods specify it as "accountNum" in the response
            if (root.Has("accntNum")) {
                return root.GetValue<string>("accntNum");
            }
            else {
                return root.GetValue<string>("accountNum");
            }
        }

        private List<BeneficialOwnerDataResult> GetBeneficialOwnerDataResultsFromResponse(Element root) {
            
            if (root.Has("beneficialOwnerDataResult")) {
                var beneficialOwnerDataResults = new List<BeneficialOwnerDataResult>();
                foreach (Element owner in root.GetAll("Owner"))
                {
                    beneficialOwnerDataResults.Add(new BeneficialOwnerDataResult()
                    {
                        FirstName = owner.GetValue<string>("FirstName"),
                        LastName = owner.GetValue<string>("LastName"),
                        Status = owner.GetValue<string>("Status")
                    });
                }
                return beneficialOwnerDataResults;
            }
            return null;
        }

        private Address GetPhysicalAddressFromResponse(Element root) {
            if (root.Has("addr") ||
                root.Has("city") ||
                root.Has("state") ||
                root.Has("zip"))
            {
                var addr = new Address()
                {
                    StreetAddress1 = root.GetValue<string>("addr"),
                    City = root.GetValue<string>("city"),
                    State = root.GetValue<string>("state"),
                    PostalCode = root.GetValue<string>("zip")
                };
                return addr;
            }
            return null;
        }

        private AccountBalanceResponseData GetACHOutBalanceInfoFromResponse(Element root) {
            if (root.Has("achOut")) {
                return new AccountBalanceResponseData() {
                    Enabled = root.Get("achOut").GetValue<string>("enabled"),
                    LimitRemaining = root.Get("achOut").GetValue<string>("limitRemaining"),
                    TransferFee = root.Get("achOut").GetValue<string>("transferFee"),
                    FeeType = root.Get("achOut").GetValue<string>("feeType"),
                    AccountLastFour = root.Get("achOut").GetValue<string>("accountLastFour")
                };
            }
            return null;
        }

        private AccountBalanceResponseData GetFlashFundsBalanceInfoFromResponse(Element root) {
            if (root.Has("flashFunds")) {
                return new AccountBalanceResponseData() {
                    Enabled = root.Get("flashFunds").GetValue<string>("enabled"),
                    LimitRemaining = root.Get("flashFunds").GetValue<string>("limitRemaining"),
                    TransferFee = root.Get("flashFunds").GetValue<string>("transferFee"),
                    FeeType = root.Get("flashFunds").GetValue<string>("feeType"),
                    AccountLastFour = root.Get("flashFunds").GetValue<string>("accountLastFour")
                };
            }
            return null;
        }
        #endregion

        #region Hydration
        private void HydrateAccountDetails<T>(ElementTree xml, Element xmlTrans, PayFacBuilder<T> builder) where T : class {
            xml.SubElement(xmlTrans, "accountNum", builder.AccountNumber);
            xml.SubElement(xmlTrans, "sourceEmail", builder.SourceEmail);
            xml.SubElement(xmlTrans, "externalId", builder.ExternalID);
            xml.SubElement(xmlTrans, "recAccntNum", builder.ReceivingAccountNumber);
            xml.SubElement(xmlTrans, "amount", builder.Amount);

            xml.SubElement(xmlTrans, "gatewayTransactionId", builder.GatewayTransactionId);
            xml.SubElement(xmlTrans, "globaltransId", builder.GlobaltransId);
            xml.SubElement(xmlTrans, "globalTransSource", builder.GlobalTransSource);
            xml.SubElement(xmlTrans, "cardBrandTransactionId", builder.CardBrandTransactionId);

            if (builder.AllowPending != null)
                xml.SubElement(xmlTrans, "allowPending", builder.AllowPending == true ? "Y" : "N");
            xml.SubElement(xmlTrans, "password", builder.Password);

            if (builder.AccountPermissions != null)
            {
                HydrateAccountPermissions(xml, xmlTrans, builder.AccountPermissions);
            }

            if (builder.UserPersonalData != null) {
                HydrateUserPersonalData(xml, xmlTrans, builder.UserPersonalData);
            }

            if (builder.BusinessData != null) {
                HydrateBusinessData(xml, xmlTrans, builder.BusinessData);
            }

            HydrateBankDetails(xml, xmlTrans, builder);

            if (builder.MailingAddressInformation != null) {
                HydrateMailAddress(xml, xmlTrans, builder.MailingAddressInformation);
            }

            if (builder.ThreatRiskData != null) {
                HydrateThreatRiskData(xml, xmlTrans, builder.ThreatRiskData);
            }

            if (builder.SignificantOwnerData != null) {
                HydrateSignificantOwnerData(xml, xmlTrans, builder.SignificantOwnerData);
            }

            if (!string.IsNullOrEmpty(builder.TimeZone)) {
                var timezoneElement = xml.SubElement(xmlTrans, "TimeZone", builder.TimeZone);
            }

            if (builder.DeviceData != null) {
                HydrateDeviceData(xml, xmlTrans, builder.DeviceData);
            }

            if (builder.BeneficialOwnerData != null) {
                HydrateBeneficialOwnerData(xml, xmlTrans, builder.BeneficialOwnerData);
            }

            if (builder.GrossBillingInformation != null) {
                HydrateGrossBillingData(xml, xmlTrans, builder.GrossBillingInformation);
            }

            if (builder.RenewalAccountData != null) {
                HydrateAccountRenewDetails(xml, xmlTrans, builder.RenewalAccountData);
            }

            if (builder.FlashFundsPaymentCardData != null) {
                HydrateFlashFundsPaymentCardData(xml, xmlTrans, builder.FlashFundsPaymentCardData);
            }

            if (builder.DocumentUploadData != null) {
                HydrateDocumentUploadData(xml, xmlTrans, builder.TransactionType, builder.DocumentUploadData);
            }

            if (builder.SSORequestData != null) {
                HydrateSSORequestData(xml, xmlTrans, builder.SSORequestData);
            }

            if (builder.OrderDevice != null) {
                HydrateOrderDeviceData(xml, xmlTrans, builder.OrderDevice);
            }

            HydrateBankAccountOwnershipData(xml, xmlTrans, builder);

            xml.SubElement(xmlTrans, "ccAmount", builder.CCAmount);
            if (builder.RequireCCRefund != null)
                xml.SubElement(xmlTrans, "requireCCRefund", builder.RequireCCRefund == true ? "Y" : "N");
            xml.SubElement(xmlTrans, "transNum", builder.TransNum);
        }

        private void HydrateUserPersonalData(ElementTree xml, Element xmlTrans, UserPersonalData userPersonalData) {
            xml.SubElement(xmlTrans, "FirstName", userPersonalData.FirstName);
            xml.SubElement(xmlTrans, "mInitial", userPersonalData.MiddleInitial);
            xml.SubElement(xmlTrans, "LastName", userPersonalData.LastName);
            xml.SubElement(xmlTrans, "dob", userPersonalData.DateOfBirth);
            xml.SubElement(xmlTrans, "ssn", userPersonalData.SSN);
            xml.SubElement(xmlTrans, "sourceEmail", userPersonalData.SourceEmail);
            xml.SubElement(xmlTrans, "dayPhone", userPersonalData.DayPhone);
            xml.SubElement(xmlTrans, "evenPhone", userPersonalData.EveningPhone);
            xml.SubElement(xmlTrans, "NotificationEmail", userPersonalData.NotificationEmail);
            xml.SubElement(xmlTrans, "currencyCode", userPersonalData.CurrencyCode);
            xml.SubElement(xmlTrans, "tier", userPersonalData.Tier);
            xml.SubElement(xmlTrans, "externalId", userPersonalData.ExternalID);
            xml.SubElement(xmlTrans, "addr", userPersonalData.UserAddress.StreetAddress1);
            xml.SubElement(xmlTrans, "aptNum", userPersonalData.UserAddress.StreetAddress2);
            xml.SubElement(xmlTrans, "addr3", userPersonalData.UserAddress.StreetAddress3);
            xml.SubElement(xmlTrans, "city", userPersonalData.UserAddress.City);
            xml.SubElement(xmlTrans, "state", userPersonalData.UserAddress.State);
            xml.SubElement(xmlTrans, "zip", userPersonalData.UserAddress.PostalCode);
            xml.SubElement(xmlTrans, "country", userPersonalData.UserAddress.Country);

            xml.SubElement(xmlTrans, "IpSignup", userPersonalData.IpSignup);
            xml.SubElement(xmlTrans, "USCitizen", userPersonalData.USCitizen == true ? "true" : userPersonalData.USCitizen == false ? "false" : "");
            xml.SubElement(xmlTrans, "bOAttestation", userPersonalData.BOAttestation == true ? "true" : userPersonalData.BOAttestation == false ? "false" : "");
            xml.SubElement(xmlTrans, "TermsAcceptanceIP", userPersonalData.TermsAcceptanceIP);
            xml.SubElement(xmlTrans, "TermsAcceptanceTimeStamp", userPersonalData.TermsAcceptanceTimeStamp);
            xml.SubElement(xmlTrans, "TermsVersion", userPersonalData.TermsVersion == null ? "" : userPersonalData.TermsVersion.ToString());
        }

        private void HydrateBusinessData(ElementTree xml, Element xmlTrans, BusinessData businessData) {
            xml.SubElement(xmlTrans, "BusinessLegalName", businessData.BusinessLegalName);
            xml.SubElement(xmlTrans, "DoingBusinessAs", businessData.DoingBusinessAs);
            xml.SubElement(xmlTrans, "EIN", businessData.EmployerIdentificationNumber);
            xml.SubElement(xmlTrans, "MCCCode", businessData.MerchantCategoryCode);
            xml.SubElement(xmlTrans, "WebsiteURL", businessData.WebsiteURL);
            xml.SubElement(xmlTrans, "BusinessDesc", businessData.BusinessDescription);
            xml.SubElement(xmlTrans, "MonthlyBankCardVolume", businessData.MonthlyBankCardVolume);
            xml.SubElement(xmlTrans, "AverageTicket", businessData.AverageTicket);
            xml.SubElement(xmlTrans, "HighestTicket", businessData.HighestTicket);
            xml.SubElement(xmlTrans, "BusinessType", businessData.BusinessType);
            xml.SubElement(xmlTrans, "BusinessAddress", businessData.BusinessAddress.StreetAddress1);
            xml.SubElement(xmlTrans, "BusinessAddress2", businessData.BusinessAddress.StreetAddress2);
            xml.SubElement(xmlTrans, "BusinessCity", businessData.BusinessAddress.City);
            xml.SubElement(xmlTrans, "BusinessCountry", businessData.BusinessAddress.Country);
            xml.SubElement(xmlTrans, "BusinessState", businessData.BusinessAddress.State);
            xml.SubElement(xmlTrans, "BusinessZip", businessData.BusinessAddress.PostalCode);            
        }

        private void HydrateBankDetails<T>(ElementTree xml, Element xmlTrans, PayFacBuilder<T> builder) where T : class {
            if (builder.CreditCardInformation != null) {
                xml.SubElement(xmlTrans, "ccName", builder.CreditCardInformation.CardHolderName);
                xml.SubElement(xmlTrans, "ccNum", builder.CreditCardInformation.Number);
                xml.SubElement(xmlTrans, "expDate", builder.CreditCardInformation.ShortExpiry);
                xml.SubElement(xmlTrans, "CVV2", builder.CreditCardInformation.Cvn);
            }

            if (builder.ACHInformation != null) {
                xml.SubElement(xmlTrans, "PaymentBankAccountNumber", builder.ACHInformation.AccountNumber);
                xml.SubElement(xmlTrans, "PaymentBankRoutingNumber", builder.ACHInformation.RoutingNumber);
                xml.SubElement(xmlTrans, "PaymentBankAccountType", builder.ACHInformation.AccountType);
            }

            if (builder.BankAccountData != null) {
                xml.SubElement(xmlTrans, "AccountCountryCode", builder.BankAccountData.AccountCountryCode);
                xml.SubElement(xmlTrans, "accountName", builder.BankAccountData.AccountName);
                xml.SubElement(xmlTrans, "AccountNumber", builder.BankAccountData.AccountNumber);
                xml.SubElement(xmlTrans, "AccountOwnershipType", builder.BankAccountData.AccountOwnershipType);
                xml.SubElement(xmlTrans, "AccountType", builder.BankAccountData.AccountType);
                xml.SubElement(xmlTrans, "BankName", builder.BankAccountData.BankName);
                xml.SubElement(xmlTrans, "RoutingNumber", builder.BankAccountData.RoutingNumber);
            }

            if (builder.SecondaryBankInformation != null) {
                xml.SubElement(xmlTrans, "SecondaryAccountCountryCode", builder.SecondaryBankInformation.AccountCountryCode);
                xml.SubElement(xmlTrans, "SecondaryAccountName", builder.SecondaryBankInformation.AccountName);
                xml.SubElement(xmlTrans, "SecondaryAccountNumber", builder.SecondaryBankInformation.AccountNumber);
                xml.SubElement(xmlTrans, "SecondaryAccountOwnershipType", builder.SecondaryBankInformation.AccountOwnershipType);
                xml.SubElement(xmlTrans, "SecondaryAccountType", builder.SecondaryBankInformation.AccountType);
                xml.SubElement(xmlTrans, "SecondaryBankName", builder.SecondaryBankInformation.BankName);
                xml.SubElement(xmlTrans, "SecondaryRoutingNumber", builder.SecondaryBankInformation.RoutingNumber);
            }

        }

        private void HydrateMailAddress(ElementTree xml, Element xmlTrans, Address mailingAddressInfo) {
            xml.SubElement(xmlTrans, "mailAddr", mailingAddressInfo.StreetAddress1);
            xml.SubElement(xmlTrans, "mailApt", mailingAddressInfo.StreetAddress2);
            xml.SubElement(xmlTrans, "mailAddr3", mailingAddressInfo.StreetAddress3);
            xml.SubElement(xmlTrans, "mailCity", mailingAddressInfo.City);
            xml.SubElement(xmlTrans, "mailCountry", mailingAddressInfo.Country);
            xml.SubElement(xmlTrans, "mailState", mailingAddressInfo.State);
            xml.SubElement(xmlTrans, "mailZip", mailingAddressInfo.PostalCode);
        }

        private void HydrateThreatRiskData(ElementTree xml, Element xmlTrans, ThreatRiskData threatRiskData) {
            xml.SubElement(xmlTrans, "MerchantSourceip", threatRiskData.MerchantSourceIP);
            xml.SubElement(xmlTrans, "ThreatMetrixPolicy", threatRiskData.ThreatMetrixPolicy);
            xml.SubElement(xmlTrans, "ThreatMetrixSessionid", threatRiskData.ThreatMetrixSessionID);
        }

        private void HydrateSignificantOwnerData(ElementTree xml, Element xmlTrans, SignificantOwnerData significantOwnerData) {
            xml.SubElement(xmlTrans, "AuthorizedSignerFirstName", significantOwnerData.AuthorizedSignerFirstName);
            xml.SubElement(xmlTrans, "AuthorizedSignerLastName", significantOwnerData.AuthorizedSignerLastName);
            xml.SubElement(xmlTrans, "AuthorizedSignerTitle", significantOwnerData.AuthorizedSignerTitle);
            xml.SubElement(xmlTrans, "SignificantOwnerFirstName", significantOwnerData.SignificantOwner.FirstName);
            xml.SubElement(xmlTrans, "SignificantOwnerLastName", significantOwnerData.SignificantOwner.LastName);
            xml.SubElement(xmlTrans, "SignificantOwnerSSN", significantOwnerData.SignificantOwner.SSN);
            xml.SubElement(xmlTrans, "SignificantOwnerDateOfBirth", significantOwnerData.SignificantOwner.DateOfBirth);
            xml.SubElement(xmlTrans, "SignificantOwnerStreetAddress", significantOwnerData.SignificantOwner.OwnerAddress.StreetAddress1);
            xml.SubElement(xmlTrans, "SignificantOwnerCityName", significantOwnerData.SignificantOwner.OwnerAddress.City);
            xml.SubElement(xmlTrans, "SignificantOwnerRegionCode", significantOwnerData.SignificantOwner.OwnerAddress.State);
            xml.SubElement(xmlTrans, "SignificantOwnerPostalCode", significantOwnerData.SignificantOwner.OwnerAddress.PostalCode);
            xml.SubElement(xmlTrans, "SignificantOwnerCountryCode", significantOwnerData.SignificantOwner.OwnerAddress.Country);
            xml.SubElement(xmlTrans, "SignificantOwnerTitle", significantOwnerData.SignificantOwner.Title);
            xml.SubElement(xmlTrans, "SignificantOwnerPercentage", significantOwnerData.SignificantOwner.Percentage);
        }

        private void HydrateDeviceData(ElementTree xml, Element xmlTrans, DeviceData deviceData) {
            var devices = xml.SubElement(xmlTrans, "Devices");
            if (deviceData.Devices.Count > 0) {
                foreach (DeviceInfo deviceInfo in deviceData.Devices) {
                    var device = xml.SubElement(devices, "Device");
                    xml.SubElement(device, "Name", deviceInfo.Name);
                    xml.SubElement(device, "Quantity", deviceInfo.Quantity == null ? 0 : deviceInfo.Quantity);
                    if (deviceInfo.Attributes != null) {
                        if (deviceInfo.Attributes.Count > 0) {
                                var attributes = xml.SubElement(device, "Attributes");
                                foreach (DeviceAttributeInfo attributeInfo in deviceInfo.Attributes) {
                                    var item = xml.SubElement(attributes, "Item");
                                    item.Set("Name", attributeInfo.Name);
                                    item.Set("Value", attributeInfo.Value);
                                }
                        }
                    }
                }
            }
        }

        private void HydrateBeneficialOwnerData(ElementTree xml, Element xmlTrans, BeneficialOwnerData beneficialOwnerData) {
            var ownerDetails = xml.SubElement(xmlTrans, "BeneficialOwnerData");
            xml.SubElement(ownerDetails, "OwnerCount", beneficialOwnerData.OwnersCount);

            if (Convert.ToInt32(beneficialOwnerData.OwnersCount) > 0)
            {
                var ownersList = xml.SubElement(ownerDetails, "Owners");

                foreach (OwnersData ownerInfo in beneficialOwnerData.OwnersList)
                {
                    var newOwner = xml.SubElement(ownersList, "Owner");
                    xml.SubElement(newOwner, "FirstName", ownerInfo.FirstName);
                    xml.SubElement(newOwner, "LastName", ownerInfo.LastName);
                    xml.SubElement(newOwner, "Email", ownerInfo.Email);
                    xml.SubElement(newOwner, "SSN", ownerInfo.SSN);
                    xml.SubElement(newOwner, "DateOfBirth", ownerInfo.DateOfBirth);
                    xml.SubElement(newOwner, "Address", ownerInfo.OwnerAddress.StreetAddress1);
                    xml.SubElement(newOwner, "City", ownerInfo.OwnerAddress.City);
                    xml.SubElement(newOwner, "State", ownerInfo.OwnerAddress.State);
                    xml.SubElement(newOwner, "Zip", ownerInfo.OwnerAddress.PostalCode);
                    xml.SubElement(newOwner, "Country", ownerInfo.OwnerAddress.Country);
                    xml.SubElement(newOwner, "Title", ownerInfo.Title);
                    xml.SubElement(newOwner, "Percentage", ownerInfo.Percentage);
                }
            }
        }

        private void HydrateGrossBillingData(ElementTree xml, Element xmlTrans, GrossBillingInformation grossBillingInformation) {
            xml.SubElement(xmlTrans, "GrossSettleAddress", grossBillingInformation.GrossSettleAddress.StreetAddress1);
            xml.SubElement(xmlTrans, "GrossSettleCity", grossBillingInformation.GrossSettleAddress.City);
            xml.SubElement(xmlTrans, "GrossSettleState", grossBillingInformation.GrossSettleAddress.State);
            xml.SubElement(xmlTrans, "GrossSettleZipCode", grossBillingInformation.GrossSettleAddress.PostalCode);
            xml.SubElement(xmlTrans, "GrossSettleCountry", grossBillingInformation.GrossSettleAddress.Country);
            xml.SubElement(xmlTrans, "GrossSettleCreditCardNumber", grossBillingInformation.GrossSettleCreditCardData.Number);
            xml.SubElement(xmlTrans, "GrossSettleNameOnCard", grossBillingInformation.GrossSettleCreditCardData.CardHolderName);
            xml.SubElement(xmlTrans, "GrossSettleCreditCardExpDate", grossBillingInformation.GrossSettleCreditCardData.ShortExpiry);
            xml.SubElement(xmlTrans, "GrossSettleAccountCountryCode", grossBillingInformation.GrossSettleBankData.AccountCountryCode);
            xml.SubElement(xmlTrans, "GrossSettleAccountHolderName", grossBillingInformation.GrossSettleBankData.AccountHolderName);
            xml.SubElement(xmlTrans, "GrossSettleAccountNumber", grossBillingInformation.GrossSettleBankData.AccountNumber);
            xml.SubElement(xmlTrans, "GrossSettleAccountType", grossBillingInformation.GrossSettleBankData.AccountType);
            xml.SubElement(xmlTrans, "GrossSettleRoutingNumber", grossBillingInformation.GrossSettleBankData.RoutingNumber);
        }

        private void HydrateAccountPermissions(ElementTree xml, Element xmlTrans, AccountPermissions accountPermissions) {
            if (accountPermissions.ACHIn != null)
                xml.SubElement(xmlTrans, "ACHIn", accountPermissions.ACHIn == true ? "Y" : "N");
            if (accountPermissions.ACHOut != null)
                xml.SubElement(xmlTrans, "ACHOut", accountPermissions.ACHOut == true ? "Y" : "N");
            if (accountPermissions.CCProcessing != null)
                xml.SubElement(xmlTrans, "CCProcessing", accountPermissions.CCProcessing == true ? "Y" : "N");
            if (accountPermissions.ProPayIn != null)
                xml.SubElement(xmlTrans, "ProPayIn", accountPermissions.ProPayIn == true ? "Y" : "N");
            if (accountPermissions.ProPayOut != null)
                xml.SubElement(xmlTrans, "ProPayOut", accountPermissions.ProPayOut == true ? "Y" : "N");

            xml.SubElement(xmlTrans, "CreditCardMonthLimit", accountPermissions.CreditCardMonthLimit);
            xml.SubElement(xmlTrans, "CreditCardTransactionLimit", accountPermissions.CreditCardTransactionLimit);
            xml.SubElement(xmlTrans, "MerchantOverallStatus", accountPermissions.MerchantOverallStatus.ToString());

            if (accountPermissions.SoftLimitEnabled != null)
                xml.SubElement(xmlTrans, "SoftLimitEnabled", accountPermissions.SoftLimitEnabled == true ? "Y" : "N");
            if (accountPermissions.ACHPaymentSoftLimitEnabled != null)
                xml.SubElement(xmlTrans, "AchPaymentSoftLimitEnabled", accountPermissions.ACHPaymentSoftLimitEnabled == true ? "Y" : "N");

            xml.SubElement(xmlTrans, "SoftLimitAchOffPercent", accountPermissions.SoftLimitACHOffPercent);
            xml.SubElement(xmlTrans, "AchPaymentAchOffPercent", accountPermissions.ACHPaymentACHOffPercent);
        }

        private void HydrateBankAccountOwnershipData<T>(ElementTree xml, Element xmlTrans, PayFacBuilder<T> builder) where T : class {
            if (builder.PrimaryBankAccountOwner != null || builder.SecondaryBankAccountOwner != null) {
                var ownersDataTag = xml.SubElement(xmlTrans, "BankAccountOwnerData");

                if (builder.PrimaryBankAccountOwner != null) {
                    var primaryOwnerTag = xml.SubElement(ownersDataTag, "PrimaryBankAccountOwner");
                    xml.SubElement(primaryOwnerTag, "FirstName", builder.PrimaryBankAccountOwner.FirstName);
                    xml.SubElement(primaryOwnerTag, "LastName", builder.PrimaryBankAccountOwner.LastName);
                    xml.SubElement(primaryOwnerTag, "Address1", builder.PrimaryBankAccountOwner.OwnerAddress.StreetAddress1);
                    xml.SubElement(primaryOwnerTag, "Address2", builder.PrimaryBankAccountOwner.OwnerAddress.StreetAddress2);
                    xml.SubElement(primaryOwnerTag, "Address3", builder.PrimaryBankAccountOwner.OwnerAddress.StreetAddress3);
                    xml.SubElement(primaryOwnerTag, "City", builder.PrimaryBankAccountOwner.OwnerAddress.City);
                    xml.SubElement(primaryOwnerTag, "StateProvince", builder.PrimaryBankAccountOwner.OwnerAddress.State);
                    xml.SubElement(primaryOwnerTag, "PostalCode", builder.PrimaryBankAccountOwner.OwnerAddress.PostalCode);
                    xml.SubElement(primaryOwnerTag, "Country", builder.PrimaryBankAccountOwner.OwnerAddress.Country);
                    xml.SubElement(primaryOwnerTag, "Phone", builder.PrimaryBankAccountOwner.PhoneNumber);
                }

                if (builder.SecondaryBankAccountOwner != null) {
                    var secondaryOwnerTag = xml.SubElement(ownersDataTag, "SecondaryBankAccountOwner");
                    xml.SubElement(secondaryOwnerTag, "FirstName", builder.SecondaryBankAccountOwner.FirstName);
                    xml.SubElement(secondaryOwnerTag, "LastName", builder.SecondaryBankAccountOwner.LastName);
                    xml.SubElement(secondaryOwnerTag, "Address1", builder.SecondaryBankAccountOwner.OwnerAddress.StreetAddress1);
                    xml.SubElement(secondaryOwnerTag, "Address2", builder.SecondaryBankAccountOwner.OwnerAddress.StreetAddress2);
                    xml.SubElement(secondaryOwnerTag, "Address3", builder.SecondaryBankAccountOwner.OwnerAddress.StreetAddress3);
                    xml.SubElement(secondaryOwnerTag, "City", builder.SecondaryBankAccountOwner.OwnerAddress.City);
                    xml.SubElement(secondaryOwnerTag, "StateProvince", builder.SecondaryBankAccountOwner.OwnerAddress.State);
                    xml.SubElement(secondaryOwnerTag, "PostalCode", builder.SecondaryBankAccountOwner.OwnerAddress.PostalCode);
                    xml.SubElement(secondaryOwnerTag, "Country", builder.SecondaryBankAccountOwner.OwnerAddress.Country);
                    xml.SubElement(secondaryOwnerTag, "Phone", builder.SecondaryBankAccountOwner.PhoneNumber);
                }
            }
        }

        private void HydrateDocumentUploadData(ElementTree xml, Element xmlTrans,TransactionType transType, DocumentUploadData docUploadData) {
            var docNameTag = transType == TransactionType.UploadDocumentChargeback ? "DocumentName" : "documentName";
            var docTypeTag = transType == TransactionType.UploadDocumentChargeback ? "DocType" : "docType";

            xml.SubElement(xmlTrans, docNameTag, docUploadData.DocumentName);
            xml.SubElement(xmlTrans, "TransactionReference", docUploadData.TransactionReference);
            xml.SubElement(xmlTrans, "DocCategory", docUploadData.DocCategory.ToString().ToLower());
            xml.SubElement(xmlTrans, docTypeTag, docUploadData.DocType.ToString().ToLower());
            xml.SubElement(xmlTrans, "Document", docUploadData.Document);
        }

        private void HydrateSSORequestData(ElementTree xml, Element xmlTrans, SSORequestData ssoRequestData) {
            xml.SubElement(xmlTrans, "ReferrerUrl", ssoRequestData.ReferrerURL);
            xml.SubElement(xmlTrans, "IpAddress", ssoRequestData.IPAddress);
            xml.SubElement(xmlTrans, "IpSubnetMask", ssoRequestData.IPSubnetMask);
        }

        private void HydrateOrderDeviceData(ElementTree xml, Element xmlTrans, OrderDevice orderDeviceData ) {
            xml.SubElement(xmlTrans, "accntNum", orderDeviceData.AccountNum.ToString());
            xml.SubElement(xmlTrans, "shipTo", orderDeviceData.ShipTo);
            xml.SubElement(xmlTrans, "shipToContact", orderDeviceData.ShipToContact);
            xml.SubElement(xmlTrans, "shipToAddress", orderDeviceData.ShipToAddress);
            xml.SubElement(xmlTrans, "shipToAddress2", orderDeviceData.ShipToAddress);
            xml.SubElement(xmlTrans, "shipToCity", orderDeviceData.ShipToCity);
            xml.SubElement(xmlTrans, "shipToState", orderDeviceData.ShipToState);
            xml.SubElement(xmlTrans, "shipToZip", orderDeviceData.ShipToZip);
            xml.SubElement(xmlTrans, "shipToPhone", orderDeviceData.ShipToPhone);
            xml.SubElement(xmlTrans, "cardholderName", orderDeviceData.CardholderName);
            xml.SubElement(xmlTrans, "CcNum", orderDeviceData.CcNum);
            xml.SubElement(xmlTrans, "ExpDate", orderDeviceData.ExpDate);
            xml.SubElement(xmlTrans, "CVV2", orderDeviceData.CVV2);
            xml.SubElement(xmlTrans, "billingZip", orderDeviceData.BillingZip);
        }

        private void HydrateAccountRenewDetails(ElementTree xml, Element xmlTrans, RenewAccountData renewalAccountData) {
            xml.SubElement(xmlTrans, "tier", renewalAccountData.Tier);
            xml.SubElement(xmlTrans, "CVV2", renewalAccountData.CreditCard.Cvn);
            xml.SubElement(xmlTrans, "ccNum", renewalAccountData.CreditCard.Number);
            xml.SubElement(xmlTrans, "expDate", renewalAccountData.CreditCard.ShortExpiry);
            xml.SubElement(xmlTrans, "zip", renewalAccountData.ZipCode);
            xml.SubElement(xmlTrans, "PaymentBankAccountNumber", renewalAccountData.PaymentBankAccountNumber);
            xml.SubElement(xmlTrans, "PaymentBankRoutingNumber", renewalAccountData.PaymentBankRoutingNumber);
            xml.SubElement(xmlTrans, "PaymentBankAccountType", renewalAccountData.PaymentBankAccountType);
        }

        private void HydrateFlashFundsPaymentCardData(ElementTree xml, Element xmlTrans, FlashFundsPaymentCardData cardData) {
            xml.SubElement(xmlTrans, "ccNum", cardData.CreditCard.Number);
            xml.SubElement(xmlTrans, "expDate", cardData.CreditCard.ShortExpiry);
            xml.SubElement(xmlTrans, "CVV2", cardData.CreditCard.Cvn);
            xml.SubElement(xmlTrans, "cardholderName", cardData.CreditCard.CardHolderName);
            xml.SubElement(xmlTrans, "addr", cardData.CardholderAddress.StreetAddress1);
            xml.SubElement(xmlTrans, "city", cardData.CardholderAddress.City);
            xml.SubElement(xmlTrans, "state", cardData.CardholderAddress.State);
            xml.SubElement(xmlTrans, "zip", cardData.CardholderAddress.PostalCode);
            xml.SubElement(xmlTrans, "country", cardData.CardholderAddress.Country);
        }

        public T ProcessBoardingUser<T>(PayFacBuilder<T> builder) where T : class
        {
            throw new UnsupportedTransactionException($"Method {this.GetType().GetMethod("ProcessBoardingUser")} not supported");
        }
        #endregion
    }
}
