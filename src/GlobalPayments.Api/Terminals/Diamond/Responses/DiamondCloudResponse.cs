using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Diamond.Entities.Enums;
using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.Terminals.Diamond.Responses {
    public class DiamondCloudResponse : DeviceResponse, ITerminalResponse {
        /// <summary>
        /// For Visa Contactless cards: Visa available offline spending amount For Erzsebet cards: remaining
        /// balance of the Voucher Type used NOTE: should be printed on the customer receipt only, not on a
        /// merchant/control receipt.
        /// </summary>
        public string Aosa { get; set; }

        /// <summary>
        /// Authorization message number – usually equal to transaction number.
        /// </summary>
        public string AuthorizationMessage { get; set; }

        /// <summary>
        /// Cardholder authorization method, possible values enum class AuthorizationMethod
        /// </summary>
        public string AuthorizationMethod { get; set; }

        /// <summary>
        /// Authorization type, possible values enum class AuthorizationType
        /// </summary>
        public string AuthorizationType { get; set; }

        /// <summary>
        /// Brand name of the card – application label(EMV)or cardset name
        /// </summary>
        public string CardBrandName { get; set; }

        /// <summary>
        /// Reader used to read card data. This character depends on the acquirer values in enum class CardSource
        /// </summary>
        public string CardSource { get; set; }

        /// <summary>
        /// Transaction date in format YYYY.MM.DD
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// currencyExchangeRate float Currency exchange rate. Should be set only for DCC transaction.
        /// Uses dot ‘.’ as a separator.
        /// </summary>
        public decimal? CurrencyExchangeRate { get; set; }

        /// <summary>
        /// DCC currency exponent.
        /// </summary>
        public int? DccCurrencyExponent { get; set; }

        /// <summary>
        /// DCC text 1. Should be set only for DCC transaction.
        /// </summary>
        public string DccText1 { get; set; }

        /// <summary>
        /// DCC text 2. Should be set only for DCC transaction.
        /// </summary>
        public string DccText2 { get; set; }

        /// <summary>
        /// Optional descriptive information about intent or android specific error
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Merchant ID
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// Transaction number
        /// </summary>
        public string ClientTransactionId { get; set; }

        /// <summary>
        /// Terminal currency
        /// </summary>
        public string TerminalCurrency { get; set; }

        /// <summary>
        /// Terminal identifier
        /// </summary>
        public string TerminalId { get; set; }

        /// <summary>
        /// Terminal printing indicator (value not equal 0 means that printout has been made by the terminal).
        /// </summary>
        public string TerminalPrintingIndicator { get; set; }

        /// <summary>
        /// Transaction time format hh:mm:ss
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Transaction amount in terminal currency. Should be set only for DCC transaction
        /// </summary>
        public decimal? TransactionAmountInTerminalCurrency { get; set; }

        /// <summary>
        /// Transaction currency. Should be always set. In DCC transaction this currency is selected by user.
        /// </summary>
        public string TransactionCurrency { get; set; }

        /// <summary>
        /// Transaction title
        /// </summary>
        public string TransactionTitle { get; set; }

        /// <summary>
        /// EMV Application Identifier
        /// </summary>
        public string EmvApplicationId { get; set; }//AID

        /// <summary>
        /// TVR for EMV
        /// </summary>
        public string EmvTerminalVerificationResults { get; set; } //TVR

        /// <summary>
        /// TSI for EMV 
        /// </summary>
        public string EmvCardHolderVerificationMethod { get; set; } //TSI

        /// <summary>
        /// EMV Transaction Cryptogram
        /// </summary>
        public string EmvCryptogram { get; set; } //AC

        /// <summary>
        /// EMV card transaction counter
        /// </summary>
        public string EmvCardTransactionCounter { get; set; } //ATC


        public string InvoiceNumber { get; set; }

        public string ResultId { get; set; }
        public string BatchNumber { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public string TransactionId { get; set; }
        public string TerminalRefNumber { get; set; }
        public string Token { get; set; }
        public string SignatureStatus { get; set; }
        public byte[] SignatureData { get; set; }
        public string TransactionType { get; set; }
        public string MaskedCardNumber { get; set; }
        public string EntryMethod { get; set; }
        public string AuthorizationCode { get; set; }
        public string ApprovalCode { get; set; }
        public decimal? TransactionAmount { get; set; }
        public decimal? AmountDue { get; set; }
        public decimal? BalanceAmount { get; set; }
        public string CardHolderName { get; set; }
        public string CardBIN { get; set; }
        public bool CardPresent { get; set; }
        public string ExpirationDate { get; set; }
        public decimal? TipAmount { get; set; }
        public decimal? CashBackAmount { get; set; }
        public string AvsResponseCode { get; set; }
        public string AvsResponseText { get; set; }
        public string CvvResponseCode { get; set; }
        public string CvvResponseText { get; set; }
        public bool TaxExempt { get; set; }
        public string TaxExemptId { get; set; }
        public string TicketNumber { get; set; }
        public string PaymentType { get; set; }
        public string ApplicationPreferredName { get; set; }
        public string ApplicationLabel { get; set; }
        public string ApplicationId { get; set; }
        public ApplicationCryptogramType ApplicationCryptogramType { get; set; }
        public string ApplicationCryptogram { get; set; }
        public string CardHolderVerificationMethod { get; set; }
        public string TerminalVerificationResults { get; set; }
        public decimal? MerchantFee { get; set; }        

        public DiamondCloudResponse(string rawResponse) {
            if (JsonDoc.IsJson(rawResponse)) {
                var jsonParse = JsonDoc.Parse(rawResponse);
                var paymentDetails = jsonParse.Get("PaymentResponse")?.Get("PaymentResponse") ?? (jsonParse.Get("PaymentResponse") ?? null);
                TransactionId = jsonParse.GetValue<string>("CloudTxnId") ?? (jsonParse.GetValue<string>("cloudTxnId") ?? null);
                InvoiceNumber = jsonParse.GetValue<string>("InvoiceId") ?? null;
                ReferenceNumber = jsonParse.GetValue<string>("Device") ?? null;
                TerminalRefNumber = jsonParse.GetValue<string>("PosId") ?? null;
                ResultId = jsonParse.Get("PaymentResponse")?.GetValue<string>("ResultId") ?? null;

                if (paymentDetails != null) {
                    if (string.IsNullOrEmpty(TransactionId)) {
                        TransactionId = jsonParse.GetValue<string>("transactionId ") ?? null;
                    }
                    Status = paymentDetails.GetValue<string>("transactionStatus ") ?? null;
                    ResponseCode = paymentDetails.GetValue<string>("resultCode") ?? null;
                    if (string.IsNullOrEmpty(ResponseCode)) {
                        ResponseCode = paymentDetails.Has("result") ? EnumConverter.FromCharToObject<TransactionResult>(paymentDetails.GetValue<string>("result")).ToString()
                            : null;
                    }
                    ResponseText = paymentDetails.GetValue<string>("hostMessage") ?? null;
                    if (string.IsNullOrEmpty(ResponseText)) {
                        ResponseText = paymentDetails.GetValue<string>("serverMessage") ?? null;
                    }
                    Aosa = paymentDetails.GetValue<string>("aosa") ?? null;
                    Version = paymentDetails.GetValue<string>("applicationVersion") ?? null;
                    AuthorizationCode = paymentDetails.GetValue<string>("authorizationCode") ?? null;
                    AuthorizationMessage = paymentDetails.GetValue<string>("authorizationMessage") ?? null;
                    AuthorizationMethod = paymentDetails.Has("authorizationMethod") ? EnumConverter.FromCharToObject<AuthorizationMethod>(paymentDetails.GetValue<string>("authorizationMethod")).ToString() : null;
                    AuthorizationType = paymentDetails.Has("authorizationType") ? EnumConverter.FromCharToObject<AuthorizationType>(paymentDetails.GetValue<string>("authorizationType")).ToString() : null;
                    CardBrandName = paymentDetails.GetValue<string>("cardBrandName") ?? (paymentDetails.GetValue<string>("cardBrand") ?? null);
                    CardSource = paymentDetails.Has("cardSource") ? EnumConverter.FromCharToObject<CardSource>(paymentDetails.GetValue<string>("cardSource")).ToString() : null;
                    EntryMethod = paymentDetails.GetValue<string>("entryMethod") ?? null;
                    CashBackAmount = paymentDetails.GetValue<string>("cashback").ToDecimal() ?? null;
                    CurrencyExchangeRate = paymentDetails.GetValue<string>("currencyExchangeRate").ToDecimal() ?? null;
                    Date = paymentDetails.GetValue<string>("date") ?? null;
                    DccCurrencyExponent = paymentDetails.GetValue<string>("dccCurrencyExponent").ToInt32() ?? null;
                    DccText1 = paymentDetails.GetValue<string>("dccText1") ?? null;
                    DccText2 = paymentDetails.GetValue<string>("dccText2") ?? null;
                    ErrorMessage = paymentDetails.GetValue<string>("errorMessage") ?? null;
                    MaskedCardNumber = paymentDetails.GetValue<string>("maskedCardNumber") ?? (paymentDetails.GetValue<string>("maskedCard") ?? null);
                    MerchantId = paymentDetails.GetValue<string>("merchantId") ?? null;

                    ClientTransactionId = paymentDetails.GetValue<string>("slipNumber") ?? null;
                    TerminalCurrency = paymentDetails.GetValue<string>("terminalCurrency") ?? null;
                    TerminalId = paymentDetails.GetValue<string>("terminalId") ?? null;
                    TerminalPrintingIndicator = paymentDetails.GetValue<string>("terminalPrintingIndicator") ?? null;
                    Time = paymentDetails.GetValue<string>("time") ?? null;
                    Date = paymentDetails.GetValue<string>("dateTime") ?? null;
                    TipAmount = paymentDetails.GetValue<string>("tipAmount").ToAmount() ?? (paymentDetails.GetValue<string>("tip").ToAmount() ?? null);
                    Token = paymentDetails.GetValue<string>("token") ?? null;
                    TransactionAmount = paymentDetails.GetValue<string>("transactionAmount").ToAmount() ?? (paymentDetails.GetValue<string>("requestAmount").ToAmount() ?? null);
                    TransactionAmountInTerminalCurrency = paymentDetails.GetValue<string>("transactionAmountInTerminalCurrency").ToAmount() ?? null;
                    TransactionCurrency = paymentDetails.GetValue<string>("transactionCurrency") ?? null;
                    TransactionTitle = paymentDetails.GetValue<string>("transactionTitle") ?? null;
                    TransactionType = paymentDetails.Has("type") ? EnumConverter.GetEnumFromValue<DiamondCloudTransactionType>(paymentDetails.GetValue<string>("type")).ToString() : null;

                    EmvCardTransactionCounter = paymentDetails.GetValue<string>("ATC") ?? null;
                    EmvCryptogram = paymentDetails.GetValue<string>("AC") ?? null;
                    EmvApplicationId = paymentDetails.GetValue<string>("AID") ?? null;
                    EmvTerminalVerificationResults = paymentDetails.GetValue<string>("TVR") ?? null;
                    EmvCardHolderVerificationMethod = paymentDetails.GetValue<string>("TSI") ?? null;
                    Token = paymentDetails.GetValue<string>("paymentToken") ?? (paymentDetails.GetValue<string>("token") ?? null);
                    BatchNumber = paymentDetails.GetValue<string>("batchNumber") ?? null;
                }
                
                if (jsonParse.Get("PaymentResponse")?.Has("CloudInfo") ?? false) {
                    Command = jsonParse.Get("PaymentResponse").Get("CloudInfo").GetValue<string>("Command");
                }
                else {
                    Command = jsonParse.Get("CloudInfo")?.GetValue<string>("Command") ?? null;
                }
                DeviceResponseCode = "00";
            }
            else {
                DeviceResponseCode = "00";
                TransactionId = rawResponse;
            }
        }
    }
}
