using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Network.Elements{
    public class DE22_PosDataCode : IDataElement<DE22_PosDataCode> {
        public CardDataInputCapability CardDataInputCapability { get; set; }
        public CardHolderAuthenticationCapability CardHolderAuthenticationCapability { get; set; }
        public bool? CardCaptureCapability { get; set; }
        public OperatingEnvironment OperatingEnvironment { get; set; }
        public DE22_CardHolderPresence CardHolderPresence { get; set; }
        public DE22_CardPresence CardPresence { get; set; }
        public DE22_CardDataInputMode CardDataInputMode { get; set; }
        public CardHolderAuthenticationMethod CardHolderAuthenticationMethod { get; set; }
        public CardHolderAuthenticationEntity CardHolderAuthenticationEntity { get; set; }
        public CardDataOutputCapability CardDataOutputCapability { get; set; }
        public TerminalOutputCapability TerminalOutputCapability { get; set; }
        public PinCaptureCapability PinCaptureCapability { get; set; }

        public DE22_PosDataCode() {
            CardDataInputCapability = CardDataInputCapability.MagStripe_KeyEntry;
            CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.None;
            CardCaptureCapability = false;
            OperatingEnvironment = OperatingEnvironment.NoTerminalUsed;
            CardHolderPresence = DE22_CardHolderPresence.CardHolder_Present;
            CardPresence = DE22_CardPresence.CardPresent;
            CardDataInputMode = DE22_CardDataInputMode.MagStripe;
            CardHolderAuthenticationMethod = CardHolderAuthenticationMethod.NotAuthenticated;
            CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.NotAuthenticated;
            CardDataOutputCapability = CardDataOutputCapability.None;
            TerminalOutputCapability = TerminalOutputCapability.None;
            PinCaptureCapability = PinCaptureCapability.TwelveCharacters;
        }

        public DE22_PosDataCode FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            //CardDataInputCapability = sp.ReadStringConstant<CardDataInputCapability>(1);
            //CardHolderAuthenticationCapability = sp.ReadStringConstant<CardHolderAuthenticationCapability>(1);
            CardDataInputCapability = EnumConverter.FromMapping<CardDataInputCapability>(Target.NWS, sp.ReadString(1));
            CardHolderAuthenticationCapability = EnumConverter.FromMapping<CardHolderAuthenticationCapability>(Target.NWS, sp.ReadString(1));
            CardCaptureCapability = sp.ReadBoolean();
            OperatingEnvironment = EnumConverter.FromMapping<OperatingEnvironment>(Target.NWS, sp.ReadString(1));
            CardHolderPresence = EnumConverter.FromMapping<DE22_CardHolderPresence>(Target.NWS, sp.ReadString(1));
            CardPresence = EnumConverter.FromMapping<DE22_CardPresence>(Target.NWS, sp.ReadString(1));
            CardDataInputMode = EnumConverter.FromMapping<DE22_CardDataInputMode>(Target.NWS, sp.ReadString(1));
            CardHolderAuthenticationMethod = EnumConverter.FromMapping<CardHolderAuthenticationMethod>(Target.NWS, sp.ReadString(1));
            CardHolderAuthenticationEntity = EnumConverter.FromMapping<CardHolderAuthenticationEntity>(Target.NWS, sp.ReadString(1));
            CardDataOutputCapability = EnumConverter.FromMapping<CardDataOutputCapability>(Target.NWS, sp.ReadString(1));
            TerminalOutputCapability = EnumConverter.FromMapping<TerminalOutputCapability>(Target.NWS, sp.ReadString(1));
            PinCaptureCapability = EnumConverter.FromMapping<PinCaptureCapability>(Target.NWS, sp.ReadString(1));
            //OperatingEnvironment = sp.ReadStringConstant<OperatingEnvironment>(1);
            //CardHolderPresence = sp.ReadStringConstant<DE22_CardHolderPresence>(1);
            //CardPresence = sp.ReadStringConstant<DE22_CardPresence>(1);
            //CardDataInputMode = sp.ReadStringConstant<DE22_CardDataInputMode>(1);
            //CardHolderAuthenticationMethod = sp.ReadStringConstant<DE22_CardHolderAuthenticationMethod>(1);
            //CardHolderAuthenticationEntity = sp.ReadStringConstant<CardHolderAuthenticationEntity>(1);
            //CardDataOutputCapability = sp.ReadStringConstant<CardDataOutputCapability>(1);
            //TerminalOutputCapability = sp.ReadStringConstant<TerminalOutputCapability>(1);
            //PinCaptureCapability = sp.ReadStringConstant<PinCaptureCapability>(1);
            return this;
        }

        public byte[] ToByteArray() {
            return Encoding.ASCII.GetBytes(string.Concat(EnumConverter.GetMapping(Target.NWS, CardDataInputCapability),
                    EnumConverter.GetMapping(Target.NWS, CardHolderAuthenticationCapability),
                    ((bool)CardCaptureCapability ? "1" : "0"),
                    EnumConverter.GetMapping(Target.NWS, OperatingEnvironment),
                    EnumConverter.GetMapping(Target.NWS, CardHolderPresence),
                    EnumConverter.GetMapping(Target.NWS, CardPresence),
                    EnumConverter.GetMapping(Target.NWS, CardDataInputMode),
                    EnumConverter.GetMapping(Target.NWS, CardHolderAuthenticationMethod),
                    EnumConverter.GetMapping(Target.NWS, CardHolderAuthenticationEntity),
                    EnumConverter.GetMapping(Target.NWS, CardDataOutputCapability),
                    EnumConverter.GetMapping(Target.NWS, TerminalOutputCapability),
                    EnumConverter.GetMapping(Target.NWS, PinCaptureCapability)));
        }
    }
}
