using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE48_Address : IDataElement<DE48_Address> {
        private char paddingChar = ' ';
        public DE48_AddressType AddressType { get; set; }
        public DE48_AddressUsage AddressUsage { get; set; }
        public Address Address { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public string Email { get; set; }

        public DE48_Address FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            //AddressType = sp.ReadStringConstant<DE48_AddressType>(1);
            //AddressUsage = sp.ReadStringConstant<DE48_AddressUsage>(1);
            AddressType = EnumConverter.FromMapping<DE48_AddressType>(Target.NWS, sp.ReadString(1));
            AddressUsage = EnumConverter.FromMapping<DE48_AddressUsage>(Target.NWS, sp.ReadString(1));
            string remainder = sp.ReadRemaining();
            if (remainder.Contains("^")) {
                paddingChar = '^';
            }
            sp = new StringParser(Encoding.ASCII.GetBytes(remainder));
            switch (AddressType) {
                case DE48_AddressType.StreetAddress: {
                        Address = new Address();
                        Address.StreetAddress1 =sp.ReadToChar('\\');
                        Address.StreetAddress2 =sp.ReadToChar('\\');
                        Address.City =sp.ReadToChar('\\');
                        Address.State = StringUtils.TrimEnd(sp.ReadString(3), " ", "^");
                        Address.PostalCode =StringUtils.TrimEnd(sp.ReadString(10), " ", "^");
                        Address.Country =StringUtils.TrimEnd(sp.ReadString(3), " ", "^");
                    }
                    break;
                case DE48_AddressType.AddressVerification: {
                        Address = new Address();
                        Address.PostalCode = StringUtils.TrimEnd(sp.ReadString(9), " ", "^");
                        Address.StreetAddress1 = sp.ReadRemaining();
                    }
                    break;
                case DE48_AddressType.PhoneNumber: {
                        PhoneNumber = new PhoneNumber();
                        PhoneNumber.CountryCode = sp.ReadToChar('\\');
                        PhoneNumber.AreaCode = sp.ReadToChar('\\');
                        PhoneNumber.Number = sp.ReadToChar('\\');
                        PhoneNumber.Extension = sp.ReadRemaining();
                    }
                    break;
                case DE48_AddressType.Email: {
                        Email = sp.ReadRemaining();
                    }
                    break;
                case DE48_AddressType.AddressVerification_Numeric: {
                        Address = new Address();
                        Address.PostalCode = StringUtils.TrimEnd(sp.ReadString(9));
                        Address.StreetAddress1 = StringUtils.TrimEnd(sp.ReadString(6));
                    }
                    break;
            }
            return this;
        }

        public byte[] ToByteArray() {
            string rvalue = string.Concat(EnumConverter.GetMapping(Target.NWS, AddressType)
                    , (EnumConverter.GetMapping(Target.NWS, AddressUsage)));
            switch (AddressType) {
                case DE48_AddressType.StreetAddress:
                    // street 1
                    if (Address.StreetAddress1 != null) {
                        rvalue = string.Concat(rvalue, Address.StreetAddress1);
                    }
                    // street 2
                    if (Address.StreetAddress2 != null) {
                        if (!string.IsNullOrEmpty(rvalue)) {
                            rvalue = string.Concat(rvalue, "\\");
                        }

                        rvalue = string.Concat(rvalue, Address.StreetAddress2);
                    }

                    // city
                    if (Address.City != null) {
                        if (!string.IsNullOrEmpty(rvalue)) {
                            rvalue = string.Concat(rvalue, "\\");
                        }

                        rvalue = string.Concat(rvalue, Address.City);
                    }

                    if (!string.IsNullOrEmpty(rvalue)) {
                        rvalue = string.Concat(rvalue, "\\");
                    }

                    rvalue = string.Concat(rvalue, (StringUtils.PadRight(Address.State, 3, ' '))
                            , (StringUtils.PadRight(Address.PostalCode, 10, paddingChar))
                            , (StringUtils.PadRight(Address.Country, 3, paddingChar)));
                    break;
                case DE48_AddressType.AddressVerification:
                    rvalue = string.Concat(rvalue, StringUtils.PadRight(Address.PostalCode, 9, paddingChar));
                    if (!string.IsNullOrEmpty(Address.StreetAddress1)) {
                        rvalue = string.Concat(rvalue, Address.StreetAddress1);
                    }
                    break;
                case DE48_AddressType.PhoneNumber:
                    rvalue = string.Concat(rvalue, (StringUtils.Join("\\", new string[] {
                            PhoneNumber.CountryCode,
                            PhoneNumber.AreaCode,
                            PhoneNumber.Number,
                            PhoneNumber.Extension
                    })));
                    break;
                case DE48_AddressType.Email:
                    rvalue = string.Concat(rvalue, Email);
                    break;
                case DE48_AddressType.AddressVerification_Numeric:
                    rvalue = string.Concat(rvalue, (StringUtils.PadRight(Address.PostalCode, 9, ' '))
                            , (StringUtils.PadRight(Address.StreetAddress1, 6, ' ')));
                    break;
            }
            return Encoding.ASCII.GetBytes(rvalue);
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
