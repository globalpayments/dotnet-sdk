using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Xml;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal sealed class TransactionByOrderIDRequestResponse : BillPayResponseBase<TransactionSummary> {
        public override TransactionSummary Map() {
            var acceptedCodes = new List<string>() { "00", "0" };
            var ResponseCode = response.GetValue<string>("a:ResponseCode");
            var ResponseMessage = GetFirstResponseMessage(response);

            var transaction = response.Get("a:Transaction");

            // Check gateway responses
            if (!acceptedCodes.Contains(ResponseCode)) {
                throw new GatewayException (
                    string.Format("Unexpected Gateway Response: {0} - {1}", ResponseCode, ResponseMessage),
                    ResponseCode,
                    ResponseMessage
                );
            }

            var billTransactionsElement = transaction.Get("a:BillTransactions");
            var authorizationsElement = transaction.Get("a:Authorizations");

            return new TransactionSummary
            {
                Amount = transaction.GetValue<decimal>("a:Amount"),
                Application = transaction.GetValue<string>("a:Application"),
                AuthorizationRecords = PopulateAuthorizationRecordsFromElement(authorizationsElement),
                BillTransactions = PopulateBillTransactionsFromElement(billTransactionsElement),
                FeeAmount = transaction.GetValue<decimal>("a:FeeAmount"),
                MerchantInvoiceNumber = transaction.GetValue<string>("a:MerchantInvoiceNumber"),
                MerchantName = response.GetValue<string>("a:MerchantName"),
                MerchantPONumber = transaction.GetValue<string>("a:MerchantPONumber"),
                MerchantTransactionDescription = transaction.GetValue<string>("a:MerchantTransactionDescription"),
                MerchantTransactionID = transaction.GetValue<string>("a:MerchantTransactionID"),
                NetAmount = transaction.GetValue<decimal>("a:NetAmount"),
                NetFeeAmount = transaction.GetValue<decimal>("a:NetFeeAmount"),
                PayorData = PopulatePayorData(transaction),
                TransactionDate = transaction.GetValue<DateTime>("a:TransactionDate"),
                TransactionId = Convert.ToString(transaction.GetValue<int>("a:TransactionID")),
                TransactionType = transaction.GetValue<string>("a:TransactionType"),
                Username = transaction.GetValue<string>("a:UserName"),
            };
        }

        private Customer PopulatePayorData(Element transactionElement)
        {
            return new Customer
            {
                Address = new Address
                {
                    StreetAddress1 = transactionElement.GetValue<string>("a:PayorAddress"),
                    City = transactionElement.GetValue<string>("a:PayorCity"),
                    Country = transactionElement.GetValue<string>("a:PayorCountry"),
                    PostalCode = transactionElement.GetValue<string>("a:PayorPostalCode"),
                    State = transactionElement.GetValue<string>("a:PayorState")
                },
                Company = transactionElement.GetValue<string>("a:PayorBusinessName"),
                Email = transactionElement.GetValue<string>("a:PayorEmailAddress"),
                FirstName = transactionElement.GetValue<string>("a:PayorFirstName"),
                LastName = transactionElement.GetValue<string>("a:PayorLastName"),
                MiddleName = transactionElement.GetValue<string>("a:PayorMiddleName"),
                WorkPhone = transactionElement.GetValue<string>("a:PayorPhoneNumber"),
            };
        }

        private List<Bill> PopulateBillTransactionsFromElement(Element billTransactionsElement) {
            if (billTransactionsElement.GetElement().ChildNodes.Count > 0) {
                List<Bill> billTransactionsList = new List<Bill>();
                foreach (Element bill in billTransactionsElement.GetAll("a:BillTransactionRecord")) {
                    var newBill = new Bill
                    {
                        BillType = bill.GetValue<string>("a:BillType"),
                        Identifier1 = bill.GetValue<string>("a:ID1"),
                        Identifier2 = bill.GetValue<string>("a:ID2"),
                        Identifier3 = bill.GetValue<string>("a:ID3"),
                        Identifier4 = bill.GetValue<string>("a:ID4"),
                        Amount = bill.GetValue<decimal>("a:AmountToApplyToBill"),
                        Customer = new Customer
                        {
                            Address = new Address
                            {
                                StreetAddress1 = bill.GetValue<string>("a:ObligorAddress"),
                                City = bill.GetValue<string>("a:ObligorCity"),
                                Country = bill.GetValue<string>("a:ObligorCountry"),
                                PostalCode = bill.GetValue<string>("a:ObligorPostalCode"),
                                State = bill.GetValue<string>("a:ObligorState")
                            },
                            Email = bill.GetValue<string>("a:ObligorEmailAddress"),
                            FirstName = bill.GetValue<string>("a:ObligorFirstName"),
                            LastName = bill.GetValue<string>("a:ObligorLastName"),
                            MiddleName = bill.GetValue<string>("a:ObligorMiddleName"),
                            WorkPhone = bill.GetValue<string>("a:ObligorPhoneNumber"),
                        }
                    };
                    billTransactionsList.Add(newBill);
                }
                return billTransactionsList;
            }
            return null;
        }

        private List<AuthorizationRecord> PopulateAuthorizationRecordsFromElement(Element authorizationsElement) {
            if (authorizationsElement.GetElement().ChildNodes.Count > 0) {
                List<AuthorizationRecord> authorizationRecordsList = new List<AuthorizationRecord>();
                foreach (Element record in authorizationsElement.GetAll("a:AuthorizationRecord")) {
                    var authRecord = new AuthorizationRecord
                    {
                        AddToBatchReferenceNumber = record.GetValue<string>("a:AddToBatchReferenceNumber"),
                        Amount = record.GetValue<decimal>("a:Amount"),
                        AuthCode = record.GetValue<string>("a:AuthCode"),
                        AuthorizationType = record.GetValue<string>("a:AuthorizationType"),
                        AvsResultCode = record.GetValue<string>("a:AvsResultCode"),
                        AvsResultText = record.GetValue<string>("a:AvsResultText"),
                        CardEntryMethod = record.GetValue<string>("a:CardEntryMethod"),
                        CvvResultCode = record.GetValue<string>("a:CvvResultCode"),
                        CvvResultText = record.GetValue<string>("a:CvvResultText"),
                        EmvApplicationCryptogram = record.GetValue<string>("a:EmvApplicationCryptogram"),
                        EmvApplicationCryptogramType = record.GetValue<string>("a:EmvApplicationCryptogramType"),
                        EmvApplicationID = record.GetValue<string>("a:EmvApplicationID"),
                        EmvApplicationName = record.GetValue<string>("a:EmvApplicationName"),
                        EmvCardholderVerificationMethod = record.GetValue<string>("a:EmvCardholderVerificationMethod"),
                        EmvIssuerResponse = record.GetValue<string>("a:EmvIssuerResponse"),
                        EmvSignatureRequired = record.GetValue<string>("a:EmvSignatureRequired"),
                        Gateway = record.GetValue<string>("a:Gateway"),
                        GatewayBatchID = record.GetValue<string>("a:GatewayBatchID"),
                        GatewayDescription = record.GetValue<string>("a:GatewayDescription"),
                        MaskedAccountNumber = record.GetValue<string>("a:MaskedAccountNumber"),
                        MaskedRoutingNumber = record.GetValue<string>("a:MaskedRoutingNumber"),
                        PaymentMethod = record.GetValue<string>("a:PaymentMethod"),
                        ReferenceNumber = record.GetValue<string>("a:ReferenceNumber"),
                        RoutingNumber = record.GetValue<string>("a:RoutingNumber"),
                        NetAmount = record.GetValue<decimal>("a:NetAmount"),
                    };
                    // We are taking this approach for the integers because if the value is null, GetValue is failing the cast
                    string refAuthID = record.GetValue<string>("a:ReferenceAuthorizationID");
                    authRecord.ReferenceAuthorizationID = (!string.IsNullOrWhiteSpace(refAuthID)) ? Convert.ToInt32(refAuthID) : (int?)null;

                    string authID = record.GetValue<string>("a:AuthorizationID");
                    authRecord.AuthorizationID = (!string.IsNullOrWhiteSpace(authID)) ? Convert.ToInt32(authID) : (int?)null;

                    string originalAuthID = record.GetValue<string>("a:OriginalAuthorizationID");
                    authRecord.OriginalAuthorizationID = (!string.IsNullOrWhiteSpace(originalAuthID)) ? Convert.ToInt32(originalAuthID) : (int?)null;

                    authorizationRecordsList.Add(authRecord);
                }
                return authorizationRecordsList;
            }
            return null;
        }
    }
}
