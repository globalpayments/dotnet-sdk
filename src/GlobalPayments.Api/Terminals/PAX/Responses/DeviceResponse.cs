using System;
using System.IO;
using GlobalPayments.Api.Terminals.Extensions;
using System.Text;
using System.Collections.Generic;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Terminals.PAX {
    public abstract class PaxBaseResponse : TerminalResponse {
        protected List<string> _messageIds;
        protected byte[] _buffer;

        internal HostResponse HostResponse { get; set; }
        internal AmountResponse AmountResponse { get; set; }
        internal AccountResponse AccountResponse { get; set; }
        internal TraceResponse TraceResponse { get; set; }
        internal AvsResponse AvsResponse { get; set; }
        internal CommercialResponse CommercialResponse { get; set; }
        internal EcomSubGroup EcomResponse { get; set; }
        internal ExtDataSubGroup ExtDataResponse { get; set; }
        internal CheckSubGroup CheckSubResponse { get; set; }

        internal PaxBaseResponse(byte[] buffer, params string[] messageIds) {
            this._messageIds = new List<string>();
            this._messageIds.AddRange(messageIds);

            this._buffer = buffer;

            using (var br = new BinaryReader(new MemoryStream(buffer))) {
                this.ParseResponse(br);
            }
        }

        protected virtual void ParseResponse(BinaryReader br) {
            var code = (ControlCodes)br.ReadByte(); // STX
            this.Status = br.ReadToCode(ControlCodes.FS);
            this.Command = br.ReadToCode(ControlCodes.FS);
            this.Version = br.ReadToCode(ControlCodes.FS);
            this.DeviceResponseCode = br.ReadToCode(ControlCodes.FS);
            this.DeviceResponseText = br.ReadToCode(ControlCodes.FS);

            if (!_messageIds.Contains(Command))
                throw new MessageException(string.Format("Unexpected message type received. {0}.", this.Command));
        }

        public override string ToString() {
            var sb = new StringBuilder();
            foreach (byte b in _buffer) {
                if (Enum.IsDefined(typeof(ControlCodes), b)) {
                    var code = (ControlCodes)b;
                    sb.Append(string.Format("[{0}]", code.ToString()));
                }
                else sb.Append((char)b);
            }

            return sb.ToString();
        }
    }

    public class PaxDeviceResponse : PaxBaseResponse {
        public string ReferenceNumber { get; set; }

        public PaxDeviceResponse(byte[] buffer, params string[] messageIds)
            : base(buffer, messageIds) {
        }

        protected virtual void MapResponse() {
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
