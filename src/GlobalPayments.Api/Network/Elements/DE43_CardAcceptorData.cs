using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Utils;
using System;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE43_CardAcceptorData : IDataElement<DE43_CardAcceptorData> {
        public Address Address { get; set; }

        public DE43_CardAcceptorData FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            Address address = new Address();
            address.Name = sp.ReadToChar('\\');
            address.StreetAddress1 = sp.ReadToChar('\\');
            address.City = sp.ReadToChar('\\');
            address.PostalCode = StringUtils.TrimEnd(sp.ReadString(10));
            address.State =StringUtils.TrimEnd(sp.ReadString(3));
            address.Country = sp.ReadString(3);
            this.Address = address;
            return this;
        }

        public byte[] ToByteArray() {
            string name = Address.Name != null ? Address.Name : "";
            string street = Address.StreetAddress1 != null ? Address.StreetAddress1 : "";
            string city = Address.City != null ? Address.City: "";
            string postalCode = Address.PostalCode != null ? Address.PostalCode : "";
            string state = Address.State != null ? Address.State : "";
            string country = Address.Country != null ? Address.Country : "";
            string rvalue = string.Concat(name,"\\",
                    street,"\\",
                    city,"\\",
                    StringUtils.PadRight(postalCode, 10, ' '),
                    StringUtils.PadRight(state, 3, ' '),
                    country);
            return Encoding.ASCII.GetBytes(rvalue);
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
