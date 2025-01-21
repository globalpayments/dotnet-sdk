using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.UPA;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Terminals.UPA.Responses {
    public class UPAResponseHandler : ITerminalResponse {
        #region Properties
        public decimal? AvailableBalance { get; set; }
        public string ResponseId { get; set; }
        public string TransactionId { get; set; }
        public string TerminalRefNumber { get; set; }
        public string Token { get; set; }
        public string TokenResponseCode { get; set; }
        public string TokenResponseMessage { get; set; }
        public string CardBrandTransId { get; set; }
        public string SignatureStatus { get; set; }
        public byte[] SignatureData { get; set; }
        public string TransactionType { get; set; }
        public string MaskedCardNumber { get; set; }
        public string EntryMethod { get; set; }
        public string AuthorizationCode { get; set; }
        public decimal? TransactionAmount { get; set; }
        public decimal? ExtraChargeTotal { get; set; }
        public decimal? AmountDue { get; set; }
        public decimal? BalanceAmount { get; set; }
        public string CardBIN { get; set; }
        public bool CardPresent { get; set; }
        public string ExpirationDate { get; set; }
        public string AvsResponseCode { get; set; }
        public string AvsResponseText { get; set; }
        public string CvvResponseCode { get; set; }
        public string CvvResponseText { get; set; }
        public bool TaxExempt { get; set; }
        public string TaxExemptId { get; set; }
        public string TicketNumber { get; set; }
        public string BatchSeqNbr { get; set; }
        public string PaymentType { get; set; }
        public string ApplicationPreferredName { get; set; }
        public string ApplicationLabel { get; set; }
        public string ApplicationId { get; set; }
        public ApplicationCryptogramType ApplicationCryptogramType { get; set; }
        public string ApplicationCryptogram { get; set; }
        public string CardHolderVerificationMethod { get; set; }
        public string TerminalVerificationResults { get; set; }
        public decimal? MerchantFee { get; set; }
        public string Status { get; set; }
        public string Command { get; set; }
        public string Version { get; set; }
        public string DeviceResponseCode { get; set; }
        public string DeviceResponseText { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public string ApprovalCode { get; set; }
        public decimal? TipAmount { get; set; }
        public decimal? BaseAmount { get; set; }
        public decimal? CashBackAmount { get; set; }
        public string ReferenceNumber { get; set; }
        public string CardHolderName { get; set; }
        public string DeviceSerialNum { get; set; }
        public string AppVersion { get; set; }
        public string OsVersion { get; set; }
        public string EmvSdkVersion { get; set; }
        public string CTLSSdkVersion { get; set; }
        public string RequestId { get; set; }
        public string ScanData { get; set; }
        public PinDUKPTResponse PinDUKPT { get; set; }
        public int? ButtonPressed { get; set; }
        public string ValueEntered { get; set; }
        public int? PromptMenuSelected { get; set; }
        public string DataEncryptionType { get; set; }
        public string DataString { get; set; }
        public UpaConfigContent ConfigContent { get; set; }
        public string ResponseDateTime { get; set; }
        public string GatewayResponseCode { get; set; }
        public string GatewayResponsemessage { get; set; }
        public decimal? AdditionalTipAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? AuthorizedAmount { get; set; }
        public string CpcInd { get; set; }
        public string Descriptor { get; set; }
        public int? CavvResultCode { get; set; }
        public int? TokenPANLast { get; set; }
        public int? PartialApproval { get; set; }
        public int? TraceNumber { get; set; }
        public int? RecurringDataCode { get; set; }
        public decimal? BaseDue { get; set; }
        public decimal? TaxDue { get; set; }
        public decimal? TipDue { get; set; }
        public string CustomHash { get; set; }
        public string CardGroup { get; set; }
        public string EbtType { get; set; }
        public string SignatureLine { get; set; }
        public string QpsQualified { get; set; }
        public int? StoreAndForward { get; set; }
        public int? ClerkId { get; set; }
        public string InvoiceNumber { get; set; }

        public string Multiplemessage { get; set; }
        public int BatchId { get; set; }
        public string CardType { get; set; }
        public string PinVerified { get; set; }
        public string EmvTags { get; set; }
        public string AcquisitionType { get; set; }
        public string LuhnCheckPassed { get; set; }
        public PANDetails PANDetails { get; set; }
        public TrackData TrackData { get; set; }
        public int? Cvv { get; set; }
        public ThreeDesDukpt ThreeDesDukpt { get; set; }
        public string EmvProcess { get; set; }
        public bool SafTransaction { get; set; }

        //EMV
        public string ApplicationIdentifier { get; set; }
        public string AapplicationLabel { get; set; }
        public string EmvCardholderName { get; set; }
        public string TransactionCurrencyCode { get; set; }
        public string ApplicationPAN { get; set; }
        public string ApplicationAIP { get; set; }
        public string DedicatedDF { get; set; }
        public string AuthorizedResponse { get; set; }
        public string TransactionPIN { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionTSI { get; set; }
        public string AmountAuthorized { get; set; }
        public string OtherAmount { get; set; }
        public string ApplicationICC { get; set; }
        public string ApplicationIAC { get; set; }
        public string IACDenial { get; set; }
        public string IACOnline { get; set; }
        public string IssuerApplicationData { get; set; }
        public string TerminalCountryCode { get; set; }
        public string IFDSerialNumber { get; set; }
        public string TerminalCapabilities { get; set; }
        public string TerminalType { get; set; }
        public string ApplicationTransactionCounter { get; set; }
        public string UnpredictableNumber { get; set; }
        public string AdditionalTerminalCapabilities { get; set; }
        public string TransactionSequenceCounter { get; set; }
        public string TacDefault { get; set; }
        public string TacDenial { get; set; }
        public string TacOnline { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public string EcrId { get; set; }
        #endregion

        const string INVALID_RESPONSE_FORMAT = "The response received is not in the proper format.";
        public void ParseResponse(JsonDoc root) {
            JsonDoc firstNode = IsGpApiResponse(root) ? root.Get("response") : root.Get("data");
            if (firstNode.Get("cmdResult") == null) {
                throw new MessageException(INVALID_RESPONSE_FORMAT);
            }
            CheckResponse(firstNode.Get("cmdResult"));
            JsonDoc response;
            if (!IsGpApiResponse(root)) {
                response = root.Get("data");
                if (response == null) {
                    throw new MessageException(INVALID_RESPONSE_FORMAT);
                }
                Status = firstNode.Get("cmdResult").GetValue<string>("result");
            }
            else {
                Status = root.GetValue<string>("status");
            }

            DeviceResponseText = Status;
            HydrateCmdResult(firstNode);
        }

        protected bool IsGpApiResponse(JsonDoc root) {
            return !root.Has("data");
        }

        private void HydrateCmdResult(JsonDoc response) {
            DeviceResponseCode = (Status == "Success" || Status == "COMPLETE") ? "00" : null;
            Command = response.GetValue<string>("response");
            RequestId = response.GetValue<string>("requestId");
            EcrId = response.GetValue<string>("EcrId");
        }

        private void CheckResponse(JsonDoc cmdResult) {
            if (cmdResult.GetValue<string>("result") == "Failed") {
                var errorCode = cmdResult.GetValue<string>("errorCode");
                var errorMessage = cmdResult.GetValue<string>("errorMessage");
                throw new GatewayException(
                    $"Unexpected Gateway Response: {errorCode} - {errorMessage}",
                    errorCode,
                    errorMessage
                    );
            }
        }
    }
}
