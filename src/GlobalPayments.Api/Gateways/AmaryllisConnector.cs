using System;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways {
    internal class AmaryllisConnector : XmlGateway, IPaymentGateway {
        public bool SupportsHostedPayments {
            get { return false; }
        }

        #region processing
        public Transaction ProcessAuthorization(AuthorizationBuilder builder) {
            var et = new ElementTree();

            var transaction = et.Element("a:RegularTransaction").Set("xmlns:a", "http://transactions.atpay.net/webservices/ATPayTxWS/");
            var request = et.SubElement(transaction, "a:RegularTransactionRequestPacket")
                .Set("xmlns", "http://schemas.datacontract.org/2004/07/")
                .Set("xmlns:i", "http://www.w3.org/2001/XMLSchema-instance");

            // credentials
            et.SubElement(request, "AMPassword");
            et.SubElement(request, "AMUserName");
            et.SubElement(request, "AMToken");
            et.SubElement(request, "AMServiceExpiryTime").Set("i:nil", "true"); // TODO: Should this have a value?

            // Allow DUp
            // Allow Partial Auth
            et.SubElement(request, "Amount", builder.Amount);
            // Gratuity
            // Convinience Amount
            // Shipping Amount
            // Cashback Amount
            et.SubElement(request, "CurrencyType", builder.Currency);

            // Offline Auth Code

            // Card Holder Information
            // -> Address Billing/Shipping

            // Card Data
            #region ICardData
            if (builder.PaymentMethod is ICardData) {
                var card = builder.PaymentMethod as ICardData;

                et.SubElement(request, "CreditCardExpirationDate", card.ShortExpiry);
                // et.SubElement(request, "CreditCardNameOnCard"); TODO: pull this from the card holder
                // CreditCardType

            }
            #endregion
            #region ITrackData
            else if (builder.PaymentMethod is ITrackData) {
                // CreditCardTrack2Content
                // CreditCardType
            }
            #endregion
            #region Gift
            else if (builder.PaymentMethod is GiftCard) { }
            #endregion
            #region ACH
            else if (builder.PaymentMethod is eCheck) {
                var check = builder.PaymentMethod as eCheck;

                et.SubElement(request, "ACHAccountNumber", check.AccountNumber);
                et.SubElement(request, "ACHAccountType", check.AccountType);
                et.SubElement(request, "ACHCheckNumber", check.CheckNumber);
                et.SubElement(request, "ACHRoutingNumber", check.RoutingNumber);
                et.SubElement(request, "AchEntityType", check.CheckType);
            }
            #endregion
            #region Transaction Reference
            else if (builder.PaymentMethod is TransactionReference) { }
            #endregion
            #region Recurring
            else if (builder.PaymentMethod is RecurringPaymentMethod) { }
            #endregion

            // PIN BLOCK

            // Encryption

            // TOKENIZATION
            // Balance Inquiry
            // CPC Request
            // details
            // ecommerce info
            // dynamic descriptor
            // auto substantiation

            #region UNKNOWN
            // AccessManagementFlag
            // AccessManagementType
            // AccountID
            // AcquirerCompanyId
            // AffiliateProgramInfoItems ("xmlns:b", "http://schemas.datacontract.org/2004/07/IncrediPay.Transactions.Common.Core.Domains")
            // BackOfficeFileId
            // BankAccountNumber
            // BankAddress (comma separated street, city, state, country)
            // BankCode
            // BankIBAN
            // BankName
            // BaseAmount
            // BaseCurrencyType
            // BillingConfigurationInfoItems ("xmlns:b", "http://schemas.datacontract.org/2004/07/Common.Domains")
            // BillingPurpose
            // BranchCode
            // CHBReasonCode
            // CaptureDueTime
            // CorrespondentBank
            // CurrencyConversionFee
            // CurrencyRatesBatchNumber
            /*
            CustomerDateTime i:nil="true" />
            CustomerOriginatedTechnologyType>MsDotNet</a:CustomerOriginatedTechnologyType>
            CustomerOriginatedTxBatchNumber>7585</a:CustomerOriginatedTxBatchNumber>
            CustomerOriginatedTxID>4</a:CustomerOriginatedTxID>
            CustomerTimeZone i:nil="true" />
            DynamicDescriptor>ABC*TestDescriptor*111222111</a:DynamicDescriptor>
            EndUserAccountNumberAtMerchant>ACC123456789KJHGF</a:EndUserAccountNumberAtMerchant>
            EndUserBillingAddressAddressLine>1000 1st Av</a:EndUserBillingAddressAddressLine>
            EndUserBillingAddressAttentionOf>Mrs. E. Johnson</a:EndUserBillingAddressAttentionOf>
            EndUserBillingAddressCity>Middleburg</a:EndUserBillingAddressCity>
            EndUserBillingAddressCountry>USA</a:EndUserBillingAddressCountry>
            EndUserBillingAddressMobileNumber />
            EndUserBillingAddressNumber>15</a:EndUserBillingAddressNumber>
            EndUserBillingAddressPhoneNumber1 />
            EndUserBillingAddressPhoneNumber2 />
            EndUserBillingAddressProvince />
            EndUserBillingAddressRegion />
            EndUserBillingAddressRemarks>urgent shipping</a:EndUserBillingAddressRemarks>
            EndUserBillingAddressSameAsShippingAddress>false</a:EndUserBillingAddressSameAsShippingAddress>
            EndUserBillingAddressState>FL</a:EndUserBillingAddressState>
            EndUserBillingAddressStreet>Roosevelt Avenue</a:EndUserBillingAddressStreet>
            EndUserBillingAddressZipPostalCode>10101</a:EndUserBillingAddressZipPostalCode>
            EndUserBrowserType>IE 6.0</a:EndUserBrowserType>
            EndUserEmailAddress>eleanor@atpay.net</a:EndUserEmailAddress>
            EndUserFirstName>Eleanor</a:EndUserFirstName>
            EndUserID>1</a:EndUserID>
            EndUserIPAddress>212.98.160.42</a:EndUserIPAddress>
            EndUserLastName>Johnson</a:EndUserLastName>
            EndUserShippingAddressAddressLine />
            EndUserShippingAddressAttentionOf />
            EndUserShippingAddressCity>Middleburg</a:EndUserShippingAddressCity>
            EndUserShippingAddressCountry>USA</a:EndUserShippingAddressCountry>
            EndUserShippingAddressMobileNumber />
            EndUserShippingAddressNumber />
            EndUserShippingAddressPhoneNumber1 />
            EndUserShippingAddressPhoneNumber2 />
            EndUserShippingAddressPostalCode>12019</a:EndUserShippingAddressPostalCode>
            EndUserShippingAddressProvince />
            EndUserShippingAddressRegion />
            EndUserShippingAddressRemarks>urgent shipping</a:EndUserShippingAddressRemarks>
            EndUserShippingAddressState>FL</a:EndUserShippingAddressState>
            EndUserShippingAddressStreet />
            EndUserTimeZone>+2</a:EndUserTimeZone>
            ExternalClearingInfo i:nil="true" xmlns:b="http://schemas.datacontract.org/2004/07/IncrediPay.Transactions.Common.Core.Domains" />
            ExternalMerchantProfile i:nil="true" xmlns:b="http://schemas.datacontract.org/2004/07/IncrediPay.Transactions.Common.Core.Domains" />
            FirstInstallmentAmount>0</a:FirstInstallmentAmount>
            FirstInstallmentInterval>0</a:FirstInstallmentInterval>
            InitialPreAuthAmount>0</a:InitialPreAuthAmount>
            InstructionType>Authorization</a:InstructionType>
            IpCommerceServiceConfig i:nil="true" xmlns:b="http://schemas.datacontract.org/2004/07/IncrediPay.Transactions.Common.Core.Domains" />
            LocationID>0</a:LocationID>
            MerchantFreeText>Merchant Free Text</a:MerchantFreeText>
            NumberOfTries>1</a:NumberOfTries>
            OperationType>Debit</a:OperationType>
            OperatorUserName i:nil="true" />
            OriginalRecurringBillingID>0</a:OriginalRecurringBillingID>
            OriginalTransactionID>0</a:OriginalTransactionID>
            PayPalPayerId i:nil="true" />
            PayPalToken i:nil="true" />
            PaymentMethodToken />
            PaymentMethodType>CreditCard</a:PaymentMethodType>
            PaymentPageID i:nil="true" />
            PaymentType>Regular</a:PaymentType>
            PredefinedErrorID>0</a:PredefinedErrorID>
            ProcessingType>Processing</a:ProcessingType>
            ProductID>10000</a:ProductID>
            PurchaseOrderID>0</a:PurchaseOrderID>
            RecurringAmount>1</a:RecurringAmount>
            RecurringBillingAutoRenew>false</a:RecurringBillingAutoRenew>
            RecurringBillingFlag>false</a:RecurringBillingFlag>
            RecurringBillingRenewalFlag>false</a:RecurringBillingRenewalFlag>
            RecurringInstallmentIntervalMethod>Daily</a:RecurringInstallmentIntervalMethod>
            RecurringInstallmentIntervalMethodAdditionalInfo>2</a:RecurringInstallmentIntervalMethodAdditionalInfo>
            RequestType>Regular</a:RequestType>
            ScrubbingFlag>false</a:ScrubbingFlag>
            ServiceExpiryMethod>EndAfterNumberOfOccurrences</a:ServiceExpiryMethod>
            ServiceExpiryMethodAdditionalInfo>3</a:ServiceExpiryMethodAdditionalInfo>
            ServicePurchasedCategory>None</a:ServicePurchasedCategory>
            ServicePurchasedSubCategory>None</a:ServicePurchasedSubCategory>
            ServicePurchasedType>RegularTransaction</a:ServicePurchasedType>
            ServiceRenewalExpiryMethod i:nil="true" />
            ServiceRenewalExpiryMethodAdditionalInfo i:nil="true" />
            ShippingMethodType>Air</a:ShippingMethodType>
            SkinID>4</a:SkinID>
            SourceOfTrx>13</a:SourceOfTrx>
            SubAccountID>1</a:SubAccountID>
            SvsClientUserID>00000000-0000-0000-0000-000000000000</a:SvsClientUserID>
            SvsPayeeAccountNumber>0</a:SvsPayeeAccountNumber>
            SvsPayerAccountNumber>0</a:SvsPayerAccountNumber>
            SvsPaymentMethodID>0</a:SvsPaymentMethodID>
            SwiftCode>CITIUSA</a:SwiftCode>
            TerminalID />
            ThreeDSecureCAVV />
            ThreeDSecureECI />
            ThreeDSecureFlags>NoThreeD</a:ThreeDSecureFlags>
            ThreeDSecureMerchantUrl>http://www.MerchantUrl.com</a:ThreeDSecureMerchantUrl>
            ThreeDSecureTermUrl>http://www.TermUrl.co.il</a:ThreeDSecureTermUrl>
            ThreeDSecureXID />
            TokenizationFlag>Internal</a:TokenizationFlag>
            TransactionCode>Internet</a:TransactionCode>
            TransactionDescription>Transaction Description</a:TransactionDescription>
            TransactionDocument i:nil="true" />
            TxRecurringBillingID>0</a:TxRecurringBillingID>
            UserLevelID>1</a:UserLevelID>
            ValueDate>2006-06-06T00:00:00</a:ValueDate>
            WhiteListIndicator>false</a:WhiteListIndicator>
            */
            #endregion


            // SHIP IT!

            throw new NotImplementedException();
        }
        public Transaction ManageTransaction(ManagementBuilder builder) {
            throw new NotImplementedException();
        }
        public T ProcessReport<T>(ReportBuilder<T> builder) where T : class {
            throw new NotImplementedException();
        }
        public string SerializeRequest(AuthorizationBuilder builder) {
            throw new UnsupportedTransactionException();
        }

        private string BuildEnvelope(ElementTree et, Element transaction) {
            var envelope = et.Element("soap:Envelope");
            var body = et.SubElement(envelope, "soap:Body");

            // transaction
            body.Append(transaction);

            return et.ToString(envelope);
        }

        public bool SupportsOpenBanking() {
            return false;
        }
        #endregion
    }
}
