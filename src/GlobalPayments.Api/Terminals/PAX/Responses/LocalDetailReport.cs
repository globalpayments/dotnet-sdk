using System.IO;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Extensions;

namespace GlobalPayments.Api.Terminals.PAX {
    public class LocalDetailReport : PaxTerminalReport {
        public int TotalRecords { get; set; }
        public int RecordNumber { get; set; }
        public string CardType { get; set; }
        public string TransactionType { get; set; }
        public string OriginalTransactionType { get; set; }

        // Transaction Details
        public string ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public string ApprovalCode { get; set; }
        public string AuthorizationCode { get; set; }
        public decimal? TransactionAmount { get; set; }
        public decimal? AmountDue { get; set; }
        public decimal? TipAmount { get; set; }
        public decimal? CashBackAmount { get; set; }
        public decimal? BalanceAmount { get; set; }
        public string MaskedCardNumber { get; set; }
        public string EntryMethod { get; set; }
        public string ExpirationDate { get; set; }
        public string PaymentType { get; set; }
        public string CardHolderName { get; set; }
        public string CvvResponseCode { get; set; }
        public string CvvResponseText { get; set; }
        public bool CardPresent { get; set; }
        public string TerminalRefNumber { get; set; }
        public string AvsResponseCode { get; set; }
        public string AvsResponseText { get; set; }
        public bool TaxExempt { get; set; }
        public string TaxExemptId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string CardBIN { get; set; }
        public string SignatureStatus { get; set; }
        public string ApplicationPreferredName { get; set; }
        public string ApplicationLabel { get; set; }
        public string ApplicationId { get; set; }
        public ApplicationCryptogramType ApplicationCryptogramType { get; set; }
        public string ApplicationCryptogram { get; set; }
        public string CardHolderVerificationMethod { get; set; }
        public string TerminalVerificationResults { get; set; }

        internal LocalDetailReport(byte[] buffer) : base(buffer, PAX_MSG_ID.R03_RSP_LOCAL_DETAIL_REPORT) {
        }

        protected override void ParseResponse(BinaryReader br) {
            base.ParseResponse(br);

            if (DeviceResponseCode == "000000") {
                TotalRecords = int.Parse(br.ReadToCode(ControlCodes.FS));
                RecordNumber = int.Parse(br.ReadToCode(ControlCodes.FS));
                HostResponse = new HostResponse(br);
                CardType = ((TerminalCardType)int.Parse(br.ReadToCode(ControlCodes.FS))).ToString().Replace("_", " ");
                TransactionType = ((TerminalTransactionType)int.Parse(br.ReadToCode(ControlCodes.FS))).ToString().Replace("_", " ");

                int originalTransactionTypeId;
                if (int.TryParse(br.ReadToCode(ControlCodes.FS), out originalTransactionTypeId)) {
                    TerminalTransactionType transType = (TerminalTransactionType)originalTransactionTypeId;
                    OriginalTransactionType = transType.ToString().Replace("_", " ");
                }
                
                AmountResponse = new AmountResponse(br);
                AccountResponse = new AccountResponse(br);
                TraceResponse = new TraceResponse(br);
                CashierResponse = new CashierSubGroup(br);
                CommercialResponse = new CommercialResponse(br);
                CheckSubResponse = new CheckSubGroup(br);
                ExtDataResponse = new ExtDataSubGroup(br);

                MapResponse();
            }
        }

        protected override void MapResponse() {
            base.MapResponse();

            // Host Data
            if (HostResponse != null) {
                ResponseCode = NormalizeResponse(HostResponse.HostResponseCode);
                ResponseText = HostResponse.HostResponseMessage;
                ApprovalCode = HostResponse.HostResponseCode;
                AuthorizationCode = HostResponse.AuthCode;
            }

            // Amount Response
            if (AmountResponse != null) {
                TransactionAmount = AmountResponse.ApprovedAmount;
                AmountDue = AmountResponse.AmountDue;
                TipAmount = AmountResponse.TipAmount;
                CashBackAmount = AmountResponse.CashBackAmount;
                BalanceAmount = AmountResponse.Balance1 ?? AmountResponse.Balance2;
            }

            // Account Response
            if (AccountResponse != null) {
                MaskedCardNumber = AccountResponse.AccountNumber.PadLeft(16, '*');
                EntryMethod = AccountResponse.EntryMode.ToString();
                ExpirationDate = AccountResponse.ExpireDate;
                PaymentType = AccountResponse.CardType.ToString().Replace("_", " ");
                CardHolderName = AccountResponse.CardHolder;
                CvvResponseCode = AccountResponse.CvdApprovalCode;
                CvvResponseText = AccountResponse.CvdMessage;
                CardPresent = AccountResponse.CardPresent;
            }

            // Trace Data
            if (TraceResponse != null) {
                TerminalRefNumber = TraceResponse.TransactionNumber;
                ReferenceNumber = TraceResponse.ReferenceNumber;
            }

            // AVS
            if (AvsResponse != null) {
                AvsResponseCode = AvsResponse.AvsResponseCode;
                AvsResponseText = AvsResponse.AvsResponseMessage;
            }

            // Commercial Info
            if (CommercialResponse != null) {
                TaxExempt = CommercialResponse.TaxExempt;
                TaxExemptId = CommercialResponse.TaxExemptId;
            }

            // Ext Data
            if (ExtDataResponse != null) {
                TransactionId = ExtDataResponse[EXT_DATA.HOST_REFERENCE_NUMBER];
                Token = ExtDataResponse[EXT_DATA.TOKEN];
                CardBIN = ExtDataResponse[EXT_DATA.CARD_BIN];
                SignatureStatus = ExtDataResponse[EXT_DATA.SIGNATURE_STATUS];

                // EMV Stuff
                ApplicationPreferredName = ExtDataResponse[EXT_DATA.APPLICATION_PREFERRED_NAME];
                ApplicationLabel = ExtDataResponse[EXT_DATA.APPLICATION_LABEL];
                ApplicationId = ExtDataResponse[EXT_DATA.APPLICATION_ID];
                ApplicationCryptogramType = ApplicationCryptogramType.TC;
                ApplicationCryptogram = ExtDataResponse[EXT_DATA.TRANSACTION_CERTIFICATE];
                CardHolderVerificationMethod = ExtDataResponse[EXT_DATA.CUSTOMER_VERIFICATION_METHOD];
                TerminalVerificationResults = ExtDataResponse[EXT_DATA.TERMINAL_VERIFICATION_RESULTS];
            }
        }

        private string NormalizeResponse(string input) {
            if (input == "0" || input == "85")
                return "00";
            return input;
        }
    }
}
