using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.UPA
{
    internal class TransactionResponse: ITerminalResponse {
        #region Properties
        public decimal? AvailableBalance { get; set; }
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
        public decimal? BaseAmount { get; set; }
        public decimal? CashBackAmount { get; set; }
        public string ReferenceNumber { get; set; }
        public string CardHolderName { get; set; }
        public string RequestId { get; set; }
        #endregion


        public TransactionResponse(JsonDoc root) {
            if (!isGpApiResponse(root)) {
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
            HydrateTransactionData(responseData);
            HydrateEmvData(responseData);
            HydrateDccData(responseData);
            HydrateHeaderData(responseData);
        }
            else {
                RequestId = root.GetValue<string>("id");
                TransactionId = RequestId;
                DeviceResponseText = root.GetValue<string>("status");
                ResponseText = root.Get("action").GetValue<string>("result_code");
                DeviceResponseCode = ResponseText;
            }
        }

        private bool isGpApiResponse(JsonDoc root) {
            return !root.Has("data");
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
            TokenResponseCode = host.GetValue<string>("tokenRspCode");
            TokenResponseMessage = host.GetValue<string>("tokenRspMsg");
            if (host.GetValue("cardBrandTransId") != null)
                CardBrandTransId = host.GetValue<string>("cardBrandTransId");
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
            PaymentType = payment.GetValue<string>("cardType");
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
            ExpirationDate = payment.GetValue<string>("expiryDate");
        }

        protected void HydrateHeaderData(JsonDoc headerData) {
            if (headerData == null) {
                return;
            }
            var acquisitionType = headerData.GetValue<string>("acquisitionType");
            var LuhnCheckPassed = headerData.GetValue<string>("LuhnCheckPassed");
            var dataEncryptionType = headerData.GetValue<string>("dataEncryptionType");
            var EmvTags = headerData.GetValue<string>("EmvTags");
            var expDate = headerData.GetValue<string>("expDate");
            var Cvv = headerData.GetValue<decimal>("Cvv");
            var ScannedData = headerData.GetValue<string>("ScannedData");
            HydratePanData(headerData);
            HydrateTrackData(headerData);
            HydratePinDUKPTData(headerData);
            Hydrate3DesDukptData(headerData);
        }

        protected void HydratePanData(JsonDoc data) {
            var panData = data.Get("Pan");
            if (panData == null) {
                return;
            }
            var clearPAN = panData.GetValue<string>("clearPAN");
            var maskedPAN = panData.GetValue<string>("maskedPAN");
            var encryptedPAN = panData.GetValue<string>("encryptedPAN");
        }

        protected void HydrateTrackData(JsonDoc data) {
            var trackData = data.Get("trackData");
            if (trackData == null) {
                return;
            }
            var clearTrack2 = trackData.GetValue<string>("clearTrack2");
            var maskedTrack2 = trackData.GetValue<string>("maskedTrack2");
            var clearTrack1 = trackData.GetValue<string>("clearTrack1");
            var maskedTrack1 = trackData.GetValue<string>("maskedTrack1");
            var clearTrack3 = trackData.GetValue<string>("clearTrack3");
            var maskedTrack3 = trackData.GetValue<string>("maskedTrack3");
        }

        protected void HydratePinDUKPTData(JsonDoc data) {
            var pinDUKPT = data.Get("PinDUKPT");
            if (pinDUKPT == null) {
                return;
            }
            var pinBlock = pinDUKPT.GetValue<string>("PinBlock");
            var Ksn = pinDUKPT.GetValue<string>("Ksn");
        }

        protected void Hydrate3DesDukptData(JsonDoc data) {
            var desDukpt = data.Get("3DesDukpt");
            if (desDukpt == null) {
                return;
            }
            var encryptedBlob = desDukpt.GetValue<string>("encryptedBlob");
            var Ksn = desDukpt.GetValue<string>("Ksn");
        }

        protected void HydrateTransactionData(JsonDoc data)
        {
            var transaction = data.Get("transaction");
            if (transaction == null)
            {
                return;
            }
            TipAmount = transaction.GetValue<decimal>("tipAmount");
            TransactionAmount = transaction.GetValue<decimal>("totalAmount");
            BaseAmount = transaction.GetValue<decimal>("baseAmount");            
        }

        protected void HydrateEmvData(JsonDoc data) {
            var emv = data.Get("emv");
            if (emv == null) {
                return;
            }
            // Emv4F = emv.Emv4F;
            if (emv.GetValue("50") != null)
                ApplicationLabel = ConvertHEX(emv.GetValue("50").ToString());
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
            if (emv.GetValue("9F06") != null)
                ApplicationId = emv.GetValue("9F06").ToString();
            // Emv9F08 = Convert.ToInt32(emv.Emv9F08);
            // Emv9F0D = emv.Emv9F0D;
            // Emv9F0E = emv.Emv9F0E;
            // Emv9F0F = emv.Emv9F0F;
            // //Emv9F10 = Convert.ToInt32(emv.Emv9F10);
            if (emv.GetValue("9F12") != null)
                ApplicationPreferredName = ConvertHEX(emv.GetValue("9F12").ToString());
            // Emv9F1A = Convert.ToInt32(emv.Emv9F1A);
            // Emv9F1E = emv.Emv9F1E;
            if (emv.GetValue("9F26") != null)
                ApplicationCryptogram = emv.GetValue("9F26").ToString();
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

        protected void HydrateDccData(JsonDoc data) {
            var dcc = data.Get("dcc");
            if (dcc == null)
                return;
            ExchangeRate = dcc.GetValue<decimal>("exchangeRate");
            MarkUp = dcc.GetValue<decimal>("markUp");
            TransactionCurrency = dcc.GetValue<string>("transactionCurrency");
            DccTransactionAmount = dcc.GetValue<string>("transactionAmount");
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

        private string ConvertHEX(string hexString) {
            var retValue = string.Empty;
            if (hexString.Length % 2 != 0) {
                return retValue;
            }

            for (int i = 0; i < hexString.Length - 0; i += 2) {
                retValue += System.Convert.ToChar(System.Convert.ToUInt32(hexString.Substring(i, 2), 16)).ToString();
            }
            return retValue;
        }

        public static TransactionResponse ParseResponse(string rawResponse)
        {
            JsonDoc response = JsonDoc.Parse(rawResponse);
            // TODO: We might have to scope the document down depending on what response we actually get from the message endpoint
            return new TransactionResponse(response);
        }
        public decimal? ExchangeRate { get; set; }
        public decimal? MarkUp { get; set; }
        public string TransactionCurrency { get; set; }
        public string DccTransactionAmount { get; set; }
    }
}

