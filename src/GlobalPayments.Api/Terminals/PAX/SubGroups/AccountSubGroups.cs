using System;
using System.IO;
using System.Text;
using GlobalPayments.Api.Terminals.Extensions;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.PAX {
    internal class AccountRequest : IRequestSubGroup {
        public string AccountNumber { get; set; }
        public string EXPD { get; set; }
        public string CvvCode { get; set; }
        public string EbtType { get; set; }
        public string VoucherNumber { get; set; }
        public string DupOverrideFlag { get; set; }

        public string GetElementString() {
            var sb = new StringBuilder();
            sb.Append(AccountNumber);
            sb.Append((char)ControlCodes.US);
            sb.Append(EXPD);
            sb.Append((char)ControlCodes.US);
            sb.Append(CvvCode);
            sb.Append((char)ControlCodes.US);
            sb.Append(EbtType);
            sb.Append((char)ControlCodes.US);
            sb.Append(VoucherNumber);
            sb.Append((char)ControlCodes.US);
            sb.Append(DupOverrideFlag);

            return sb.ToString().TrimEnd((char)ControlCodes.US);
        }
    }

    internal class AccountResponse : IResponseSubGroup {
        public string AccountNumber { get; private set; }
        public EntryMode EntryMode { get; private set; }
        public string ExpireDate { get; private set; }
        public string EbtType { get; private set; }
        public string VoucherNumber { get; private set; }
        public string NewAccountNumber { get; private set; }
        public CardType CardType { get; private set; }
        public string CardHolder { get; private set; }
        public string CvdApprovalCode { get; private set; }
        public string CvdMessage { get; private set; }
        public bool CardPresent { get; private set; }

        public AccountResponse(BinaryReader br) {
            var values = br.ReadToCode(ControlCodes.FS);
            if (string.IsNullOrEmpty(values))
                return;

            var data = values.Split((char)ControlCodes.US);
            try {
                this.AccountNumber = data[0];
                this.EntryMode = (EntryMode)Int32.Parse(data[1]);
                this.ExpireDate = data[2];
                EbtType = data[3];
                VoucherNumber = data[4];
                NewAccountNumber = data[5];
                if(!string.IsNullOrEmpty(data[6]))
                    CardType = (CardType)Int32.Parse(data[6]);
                CardHolder = data[7];
                CvdApprovalCode = data[8];
                CvdMessage = data[9];
                CardPresent = data[10] == "0" ? true : false;
            }
            catch (IndexOutOfRangeException exc) {
                EventLogger.Instance.Error(exc.Message);
            }
        }
    }
}
