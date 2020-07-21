using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE3_ProcessingCode : IDataElement<DE3_ProcessingCode> {
        public DE3_TransactionType TransactionType { get; set; }
        public DE3_AccountType FromAccount { get; set; }
        public DE3_AccountType ToAccount { get; set; }

        public DE3_ProcessingCode FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            //TransactionType = sp.ReadStringConstant<DE3_TransactionType>(2);
            //FromAccount = sp.ReadStringConstant<DE3_AccountType>(2);
            //ToAccount = sp.ReadStringConstant<DE3_AccountType>(2);
            TransactionType = EnumConverter.FromMapping<DE3_TransactionType>(Target.NWS, sp.ReadString(2));
            FromAccount = EnumConverter.FromMapping<DE3_AccountType>(Target.NWS, sp.ReadString(2));
            ToAccount = EnumConverter.FromMapping<DE3_AccountType>(Target.NWS, sp.ReadString(2));
            return this;
        }

        public byte[] ToByteArray() {
            return Encoding.UTF8.GetBytes(string.Concat(EnumConverter.GetMapping(Target.NWS, TransactionType),
                    EnumConverter.GetMapping(Target.NWS, FromAccount),
                    EnumConverter.GetMapping(Target.NWS, ToAccount)));
        }
    }
}
