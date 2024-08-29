using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.UPA;
using GlobalPayments.Api.Terminals.UPA.Responses;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.UPA {
    internal class TransactionResponse: UPAResponseHandler, IBatchCloseResponse {

        public TransactionResponse(JsonDoc root) {
            ParseResponse(root);

            var response = root.Get("data");
            if(response == null) {
                return;
            }
            var responseData = response.Get("data");
            if (responseData == null) {
                return;
            }

            if (!string.IsNullOrEmpty(Command)) {
                switch (Command) {
                    case UpaTransType.GetAppInfo:
                        HydrateGetAppInfoData(responseData);
                        break;
                    case UpaTransType.EnterPIN:
                        if (responseData.Has("PinDUKPT")) {
                            var pinDUKPT = responseData.Get("PinDUKPT");
                            PinDUKPT = new PinDUKPTResponse();
                            PinDUKPT.PinBlock = pinDUKPT.GetValue<string>("PinBlock") ?? null;
                            PinDUKPT.Ksn = pinDUKPT.GetValue<string>("Ksn") ?? null;
                        }
                        break;
                    case UpaTransType.Scan:
                        ScanData = responseData.GetValue<string>("scanData") ?? null;
                        break;
                    case UpaTransType.PromptwithOptions:
                        ButtonPressed = responseData.GetValue<int?>("button") ?? null;
                        break;
                    case UpaTransType.PromptMenu:
                        ButtonPressed = responseData.GetValue<int?>("button") ?? null;
                        PromptMenuSelected = responseData.GetValue<int?>("menuSelected") ?? null;
                        break;
                    case UpaTransType.GeneralEntry:
                        ButtonPressed = responseData.GetValue<int?>("button") ?? null;
                        ValueEntered = responseData.GetValue<string>("valueEntered") ?? null;
                        break;
                    case UpaTransType.GetEncryptionType:
                        DataEncryptionType = responseData.GetValue<string>("dataEncryptionType") ?? null;                        
                        break;
                    case UpaTransType.ExecuteUDDataFile:
                        DataString = responseData.GetValue<string>("dataString");
                        break;
                    case UpaTransType.GetConfigContents:
                        HydrateGetConfigData(responseData);
                        break;
                    case UpaTransType.CommunicationCheck:
                    case UpaTransType.GetLastEOD:
                    case UpaTransType.ForceSale:
                        Multiplemessage = responseData.GetValue<string>("multipleMessage");
                        break;
                    case UpaTransType.ProcessCardTransaction:
                        DataEncryptionType = responseData.GetValue<string>("dataEncryptionType") ?? null;
                        AcquisitionType = responseData.GetValue<string>("acquisitionType") ?? null;                        
                        LuhnCheckPassed = responseData.GetValue<string>("LuhnCheckPassed") ?? null;
                        
                        if (responseData.Has("PAN")) {
                            HydratePANData(responseData);
                        }
                        if (responseData.Has("trackData")) {
                            HydrateTrackData(responseData);
                        }
                        EmvTags = responseData.GetValue<string>("EmvTags") ?? null;
                        Cvv = responseData.GetValue<int?>("Cvv") ?? null;
                        ExpirationDate = responseData.GetValue<string>("expDate") ?? null;
                        ScanData = responseData.GetValue<string>("scanData") ?? null;
                        if (responseData.Has("PinDUKPT")) {
                            HydratePinDUKPTData(responseData);
                        }
                        if (responseData.Has("3DesDukpt")) {
                            Hydrate3DesDukptData(responseData);
                        }
                        EmvProcess = responseData.GetValue<string>("EmvProcess") ?? null;
                        break;
                    case UpaTransType.ContinueEmvTransaction:
                    case UpaTransType.ContinueCardTransaction:
                    case UpaTransType.CompleteEMVTransaction:
                        EmvTags = responseData.GetValue<string>("EmvTags");
                        if (responseData.Has("PinDUKPT")) {
                            HydratePinDUKPTData(responseData);
                        }
                        break;
                    default:
                        break;
                }
            }
                
            HydrateHostData(responseData);
            HydratePaymentData(responseData);
            HydrateTransactionData(responseData);
            HydrateEmvData(responseData);
            HydrateDccData(responseData);
            HydrateHeaderData(responseData);           
        }

        private void HydratePANData(JsonDoc responseData) {
            var panData = responseData.Get("PAN");
            if (panData == null) {
                return;
            }
            PANDetails = new PANDetails();
            PANDetails.ClearPAN = panData.GetValue<string>("clearPAN");
            PANDetails.MaskedPAN = panData.GetValue<string>("maskedPan");
            PANDetails.EncryptedPAN = panData.GetValue<string>("encryptedPan");
        }

        private void HydrateGetConfigData(JsonDoc responseData) {
            ConfigContent = new UpaConfigContent();
            ConfigContent.ConfigType = EnumConverter.GetEnumFromValue<TerminalConfigType>(responseData.GetValue<string>("configType"));
            ConfigContent.FileContent = responseData.GetValue<string>("fileContents");
            ConfigContent.Length = responseData.GetValue<int>("length");
        }

        private void HydrateGetAppInfoData(JsonDoc data) {
            DeviceSerialNum = data.GetValue<string>("deviceSerialNum") ?? null;
            AppVersion = data.GetValue<string>("appVersion") ?? null;
            OsVersion = data.GetValue<string>("OsVersion") ?? null;
            EmvSdkVersion = data.GetValue<string>("EmvSdkVersion") ?? null;
            CTLSSdkVersion = data.GetValue<string>("CTLSSdkVersion") ?? null;
        }

        protected void HydrateHostData(JsonDoc data) {
            var host = data.Get("host");
            if (host == null) {
                return;
            }
            decimal defaultDecimal;
            int defaultInt;
            ResponseId = host.GetValue<string>("responseId") ?? null;
            TransactionId = host.GetValue<string>("responseId") ?? null;
            TerminalRefNumber = host.GetValue<string>("tranNo") ?? null;
            ResponseDateTime = host.GetValue<string>("respDateTime") ?? null;
            GatewayResponseCode = host.GetValue<string>("gatewayResponseCode") ?? null;
            GatewayResponsemessage = host.GetValue<string>("gatewayResponseMessage") ?? null;
            ResponseCode = NormalizeResponseCode(host.GetValue<string>("responseCode"), host.GetValue<string>("partialApproval"));
            ResponseText = host.GetValue<string>("responseText") ?? null;
            ApprovalCode = host.GetValue<string>("approvalCode") ?? null;
            ReferenceNumber = host.GetValue<string>("referenceNumber") ?? null;
            AvsResponseCode = host.GetValue<string>("AvsResultCode") ?? null;
            CvvResponseCode = host.GetValue<string>("CvvResultCode") ?? null;
            AvsResponseText = host.GetValue<string>("AvsResultText") ?? null;
            CvvResponseText = host.GetValue<string>("CvvResultText") ?? null;
            AdditionalTipAmount = decimal.TryParse(host.GetValue<string>("additionalTipAmount"), out defaultDecimal) ? defaultDecimal : default(decimal?);
            BaseAmount = decimal.TryParse(host.GetValue<string>("baseAmount"), out defaultDecimal) ? defaultDecimal : default(decimal?);
            TipAmount = decimal.TryParse(host.GetValue<string>("tipAmount"), out defaultDecimal) ? defaultDecimal : default(decimal?);
            TaxAmount = decimal.TryParse(host.GetValue<string>("taxAmount"), out defaultDecimal) ? defaultDecimal: default(decimal?);
            CashBackAmount = decimal.TryParse(host.GetValue<string>("cashBackAmount"), out defaultDecimal) ? defaultDecimal : default(decimal?);
            AuthorizedAmount = decimal.TryParse(host.GetValue<string>("authorizedAmount"), out defaultDecimal) ? defaultDecimal : default(decimal?);
            TransactionAmount = decimal.TryParse(host.GetValue<string>("totalAmount"), out defaultDecimal) ? defaultDecimal : default(decimal?);
            MerchantFee = decimal.TryParse(host.GetValue<string>("surcharge"), out defaultDecimal) ? defaultDecimal : default(decimal?);
            Token = host.GetValue<string>("tokenValue") ?? null;
            TokenResponseCode = host.GetValue<string>("tokenRspCode") ?? null;
            TokenResponseMessage = host.GetValue<string>("tokenRspMsg") ?? null;            
            CardBrandTransId = host.GetValue<string>("cardBrandTransId") ?? null;
            BatchSeqNbr = host.GetValue<string>("batchSeqNbr") ?? null;
            CpcInd = host.GetValue<string>("CpcInd") ?? null;
            Descriptor = host.GetValue<string>("txnDescriptor");
            // RecurringDataCode = host.GetValue<string>("recurringDataCode");
            CavvResultCode = host.GetValue<int?>("CavvResultCode") ?? null;
            TokenPANLast = host.GetValue<int?>("tokenPANLast") ?? null;
            PartialApproval = host.GetValue<int?>("partialApproval") ?? null;
            TraceNumber = host.GetValue<int?>("traceNumber") ?? null;
            BalanceAmount = decimal.TryParse(host.GetValue<string>("balanceDue"), out defaultDecimal) ? defaultDecimal : default(decimal?);
            RecurringDataCode = host.GetValue<int?>("recurringDataCode") ?? null;
            // BalanceDue = host.GetValue<decimal>("balanceDue");
            BaseDue = decimal.TryParse(host.GetValue<string>("baseDue"), out defaultDecimal) ? defaultDecimal : default(decimal?);
            TaxDue = decimal.TryParse(host.GetValue<string>("taxDue"), out defaultDecimal) ? defaultDecimal : default(decimal?);
            TipDue = decimal.TryParse(host.GetValue<string>("tipDue"), out defaultDecimal) ? defaultDecimal : default(decimal?);
            CustomHash = host.GetValue<string>("customHash") ?? null;
            AvailableBalance = decimal.TryParse(host.GetValue<string>("availableBalance"), out defaultDecimal) ? defaultDecimal : default(decimal?);
            // EmvIssuerResp = host.GetValue<string>("emvIssuerResp");
        }

        protected void HydratePaymentData(JsonDoc data) {
            var payment = data.Get("payment");
            if (payment == null) {
                return;
            }
            int defaultInt;
            CardHolderName = payment.GetValue<string>("cardHolderName");
            CardType = payment.GetValue<string>("cardType");
            PaymentType = payment.GetValue<string>("cardType");
            CardGroup = payment.GetValue<string>("cardGroup");
            EbtType = payment.GetValue<string>("ebtType");
            MaskedCardNumber = payment.GetValue<string>("maskedPan");
            SignatureLine = payment.GetValue<string>("signatureLine");
            QpsQualified = payment.GetValue<string>("QpsQualified");
            StoreAndForward = int.TryParse(payment.GetValue<string>("storeAndForward"), out defaultInt) ? defaultInt: default(int?);
            ClerkId = payment.GetValue<int?>("clerkId");
            InvoiceNumber = payment.GetValue<string>("invoiceNbr");
            ExpirationDate = payment.GetValue<string>("expiryDate");
            TransactionType = payment.GetValue<string>("transactionType");            
            EntryMethod = payment.GetValue<string>("cardAcquisition");
            
            PinVerified = payment.GetValue<string>("PinVerified");
            // TrackData = payment.GetValue<string>("trackData");
            // FallBack = payment.GetValue<string>("fallback");
            // TraceNumber = payment.GetValue<string>("trackNumber");
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
            HydratePANData(headerData);
            HydrateTrackData(headerData);
            HydratePinDUKPTData(headerData);
            Hydrate3DesDukptData(headerData);
        }

        protected void HydrateTrackData(JsonDoc data) {
            var trackData = data.Get("trackData");
            if (trackData == null) {
                return;
            }
            TrackData = new TrackData();
            TrackData.ClearTrack2 = trackData.GetValue<string>("clearTrack2");
            TrackData.MaskedTrack2 = trackData.GetValue<string>("maskedTrack2");
            TrackData.ClearTrack1 = trackData.GetValue<string>("clearTrack1");
            TrackData.MaskedTrack1 = trackData.GetValue<string>("maskedTrack1");
            TrackData.ClearTrack3 = trackData.GetValue<string>("clearTrack3");
            TrackData.MaskedTrack3 = trackData.GetValue<string>("maskedTrack3");
        }

        protected void HydratePinDUKPTData(JsonDoc data) {
            var pinDUKPT = data.Get("PinDUKPT");
            if (pinDUKPT == null) {
                return;
            }
            PinDUKPT = new PinDUKPTResponse();
            PinDUKPT.PinBlock = pinDUKPT.GetValue<string>("PinBlock") ?? null;
            PinDUKPT.Ksn = pinDUKPT.GetValue<string>("Ksn") ?? null;
        }

        protected void Hydrate3DesDukptData(JsonDoc data) {
            var desDukpt = data.Get("3DesDukpt");
            if (desDukpt == null) {
                return;
            }
            ThreeDesDukpt = new ThreeDesDukpt();
            ThreeDesDukpt.EncryptedBlob = desDukpt.GetValue<string>("encryptedBlob");
            ThreeDesDukpt.Ksn = desDukpt.GetValue<string>("Ksn");
        }

        protected void HydrateTransactionData(JsonDoc data) {
            var transaction = data.Get("transaction");
            if (transaction == null)
            {
                return;
            }
            TipAmount = transaction.GetValue<decimal>("tipAmount");
            TransactionAmount = transaction.GetValue<decimal>("totalAmount");
            BaseAmount = transaction.GetValue<decimal>("baseAmount");
            ExtraChargeTotal = transaction.GetValue<decimal>("extraChargeTotal");
        }

        protected void HydrateEmvData(JsonDoc data) {
            var emv = data.Get("emv");
            if (emv == null) {
                return;
            }
            decimal defaultDecimal;
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

            ApplicationIdentifier = emv.GetValue<string>("4F");
            ApplicationLabel =  emv.GetValue<string>("50");
            EmvCardholderName = emv.GetValue<string>("5F20");
            TransactionCurrencyCode = emv.GetValue<string>("5F2A");
            ApplicationPAN = emv.GetValue<string>("5F34");
            ApplicationAIP = emv.GetValue<string>("82");
            DedicatedDF = emv.GetValue<string>("84");
            AuthorizedResponse = emv.GetValue<string>("8A");
            TerminalVerificationResults = emv.GetValue<string>("95");
            TransactionPIN = emv.GetValue<string>("99");
            TransactionDate = emv.GetValue<string>("9A");
            TransactionTSI = emv.GetValue<string>("9B");
            TransactionType = emv.GetValue<string>("9C");
            AmountAuthorized = emv.GetValue<string>("9F02");
            OtherAmount = emv.GetValue<string>("9F03");
            ApplicationId = emv.GetValue<string>("9F06");
            ApplicationICC = emv.GetValue<string>("9F08");
            ApplicationIAC = emv.GetValue<string>("9F0D");
            IACDenial = emv.GetValue<string>("9F0E");
            IACOnline = emv.GetValue<string>("9F0F");
            IssuerApplicationData = emv.GetValue<string>("9F10");
            ApplicationPreferredName = emv.GetValue<string>("9F12");
            TerminalCountryCode = emv.GetValue<string>("9F1A");
            IFDSerialNumber = emv.GetValue<string>("9F1E");
            ApplicationCryptogram = emv.GetValue<string>("9F26");
            //ApplicationCryptogramType = emv.GetValue<string>("9F27");
            TerminalCapabilities = emv.GetValue<string>("9F33");
            TerminalType = emv.GetValue<string>("9F35");
            ApplicationTransactionCounter = emv.GetValue<string>("9F36");
            UnpredictableNumber = emv.GetValue<string>("9F37");
            AdditionalTerminalCapabilities = emv.GetValue<string>("9F40");
            TransactionSequenceCounter = emv.GetValue<string>("9F41");
            TacDefault = emv.GetValue<string>("TacDefault");
            TacDenial = emv.GetValue<string>("TacDenial");
            TacOnline = emv.GetValue<string>("TacOnline");
            CardHolderVerificationMethod = emv.GetValue<string>("9F34");
            BatchId = emv.GetValue<int>("batchId");
            AvailableBalance = decimal.TryParse(emv.GetValue<string>("availableBalance"), out defaultDecimal) ? defaultDecimal : default(decimal?);
        }

        protected void HydrateDccData(JsonDoc data) {
            var dcc = data.Get("dcc");
            if (dcc == null) { 
                return; 
            }
                
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
        public string SequenceNumber { get; set; }
        public string TotalCount { get; set; }
        public string TotalAmount { get; set; }
    }
}

