using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.UPA
{
    internal class TransactionResponse: ITerminalResponse {
        public TransactionResponse(JsonDoc root) {
            var response = root.Get("data");
            if (response == null) {
                return;
            }

            RequestId = response.GetValue<string>("requestId");
            HydrateCmdResult(response);
            var responseData = response.Get("data");
            if (responseData == null) {
                return;
            }
            HydrateHostData(responseData);
            HydratePaymentData(responseData);
            HydrateEmvData(responseData);
        }

        protected void HydrateCmdResult(JsonDoc response) {
            var cmdResult = response.Get("cmdResult");
            if (cmdResult == null) {
                return;
            }
            Status = cmdResult.GetValue<string>("result");
            DeviceResponseCode = cmdResult.GetValue<string>("errorCode");
            DeviceResponseText = cmdResult.GetValue<string>("errorMessage");
        }

        protected void HydrateHostData(JsonDoc data) {
            var host = data.Get("host");
            if (host == null) {
                return;
            }
            TransactionId = host.GetValue<string>("responseId");
            TerminalRefNumber = host.GetValue<string>("tranNo");
            // TransactionDate = host.GetValue<DateTime>("respDateTime");
            // GatewayResponseCode = host.GetValue<string>("gatewayResponseCode");
            // GatewayResponsemessage = host.GetValue<string>("gatewayResponsemessage");
            ResponseCode = NormalizeResponseCode(host.GetValue<string>("responseCode"), host.GetValue<string>("partialApproval"));
            ResponseText = host.GetValue<string>("responseText");
            ApprovalCode = host.GetValue<string>("approvalCode");
            ReferenceNumber = host.GetValue<string>("referenceNumber");
            AvsResponseCode = host.GetValue<string>("avsResultCode");
            CvvResponseCode = host.GetValue<string>("cvvResultCode");
            AvsResponseText = host.GetValue<string>("avsResultText");
            CvvResponseText = host.GetValue<string>("cvvResultText");
            // AdditionalTipAmount = host.GetValue<decimal>("additionalTipAmount");
            // BaseAmount = host.GetValue<decimal>("baseAmount");
            TipAmount = host.GetValue<decimal>("tipAmount");
            // TaxAmount = host.GetValue<decimal>("taxAmount");
            CashBackAmount = host.GetValue<decimal>("cashBackAmount");
            // AuthorizedAmount = host.GetValue<decimal>("authorizedAmount");
            TransactionAmount = host.GetValue<decimal>("totalAmount");
            MerchantFee = host.GetValue<decimal>("surcharge");
            Token = host.GetValue<string>("tokenValue");
            // TxnDescriptor = host.GetValue<string>("txnDescriptor");
            // RecurringDataCode = host.GetValue<string>("recurringDataCode");
            // CavvResultCode = host.GetValue<string>("cavvResultCode");
            // TokenPANLast = host.GetValue<string>("tokenPANLast");
            // PartialApproval = host.GetValue<string>("partialApproval");
            // TraceNumber = host.GetValue<string>("traceNumber");
            // BalanceDue = host.GetValue<decimal>("balanceDue");
            // BaseDue = host.GetValue<decimal>("baseDue");
            // TaxDue = host.GetValue<decimal>("taxDue");
            // TipDue = host.GetValue<decimal>("tipDue");
            AvailableBalance = host.GetValue<decimal>("availableBalance");
            // EmvIssuerResp = host.GetValue<string>("emvIssuerResp");
        }

        protected void HydratePaymentData(JsonDoc data) {
            var payment = data.Get("payment");
            if (payment == null) {
                return;
            }
            CardHolderName = payment.GetValue<string>("cardHolderName");
            // CardType = payment.GetValue<string>("cardType");
            // CardGroup = payment.GetValue<string>("cardGroup");
            // EbtType = payment.GetValue<string>("ebtType");
            EntryMethod = payment.GetValue<string>("cardAcquisition");
            MaskedCardNumber = payment.GetValue<string>("maskedPan");
            // SignatureLine = payment.GetValue<string>("signatureLine");
            // PinVerified = payment.GetValue<string>("pinVerified");
            // QpsQualified = payment.GetValue<string>("qpsQualified");
            // StoreAndForward = payment.GetValue<string>("storeAndForward");
            // ClerkId = payment.GetValue<string>("clerkId");
            // InvoiceNumber = payment.GetValue<string>("invoiceNbr");
            // TrackData = payment.GetValue<string>("trackData");
            // TraceNumber = payment.GetValue<string>("trackNumber");
        }

        protected void HydrateEmvData(JsonDoc data) {
            var emv = data.Get("emv");
            if (emv == null) {
                return;
            }
            // Emv4F = emv.Emv4F;
            // Emv50 = emv.Emv50;
            // Emv5F20 = emv.Emv5F20;
            // Emv5F2A = Convert.ToInt32(emv.Emv5F2A);
            // Emv5F34 = Convert.ToInt32(emv.Emv5F34);
            // Emv82 = emv.Emv82;
            // Emv84 = emv.Emv84;
            // Emv8A = emv.Emv8A;
            // Emv95 = Convert.ToInt64(emv.Emv95);
            // Emv99 = Convert.ToInt32(emv.Emv99);
            // Emv9A = Convert.ToInt32(emv.Emv9A);
            // Emv9B = Convert.ToInt32(emv.Emv9B);
            // Emv9C = Convert.ToInt32(emv.Emv9C);
            // Emv9F02 = Convert.ToInt32(emv.Emv9F02);
            // Emv9F03 = Convert.ToInt32(emv.Emv9F03);
            // Emv9F06 = emv.Emv9F06;
            // Emv9F08 = Convert.ToInt32(emv.Emv9F08);
            // Emv9F0D = emv.Emv9F0D;
            // Emv9F0E = emv.Emv9F0E;
            // Emv9F0F = emv.Emv9F0F;
            // //Emv9F10 = Convert.ToInt32(emv.Emv9F10);
            // Emv9F12 = emv.Emv9F12;
            // Emv9F1A = Convert.ToInt32(emv.Emv9F1A);
            // Emv9F1E = emv.Emv9F1E;
            // Emv9F26 = emv.Emv9F26;
            // Emv9F27 = Convert.ToInt32(emv.Emv9F27);
            // Emv9F33 = emv.Emv9F33;
            // Emv9F34 = emv.Emv9F34;
            // Emv9F35 = Convert.ToInt32(emv.Emv9F35);
            // Emv9F36 = emv.Emv9F36;
            // Emv9F37 = emv.Emv9F37;
            // Emv9F40 = emv.Emv9F40;
            // Emv9F41 = Convert.ToInt32(emv.Emv9F41);
            // TacDefault = emv.TacDefault;
            // TacDenial = emv.TacDenial;
            // TacOnline = emv.TacOnline;
        }

        protected string NormalizeResponseCode(string responseCode, string partialApproval) {
            if (partialApproval == "1") {
                return "10";
            }
            if (responseCode == "0" || responseCode == "85") {
                return "00";
            }
            return responseCode;
        }

        public decimal? AvailableBalance { get; set; }
        public string TransactionId { get; set; }
        public string TerminalRefNumber { get; set; }
        public string Token { get; set; }
        public string SignatureStatus { get; set; }
        public byte[] SignatureData { get; set; }
        public string TransactionType { get; set; }
        public string MaskedCardNumber { get; set; }
        public string EntryMethod { get; set; }
        public string AuthorizationCode { get; set; }
        public decimal? TransactionAmount { get; set; }
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
        public decimal? CashBackAmount { get; set; }
        public string ReferenceNumber { get; set; }
        public string CardHolderName { get; set; }
        public string RequestId { get; set; }
    }
}
