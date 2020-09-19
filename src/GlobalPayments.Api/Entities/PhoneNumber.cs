using GlobalPayments.Api.Utils;
using System;
using System.Text;

namespace GlobalPayments.Api.Entities {
    public class PhoneNumber {
        public string CountryCode { get; set; }
        public string AreaCode { get; set; }
        public string Number { get; set; }
        public string Extension { get; set; }
        public new string ToString() {
            StringBuilder sb = new StringBuilder();
            // country code (default to 1)
            if (string.IsNullOrEmpty(CountryCode)) {
                CountryCode = "1";
            }
            sb.Append(string.Concat("+",(CountryCode)));
            // append area code if present
            if (!string.IsNullOrEmpty(AreaCode)) {
                sb.Append(string.Format("({0})", AreaCode));
            }
            // put the number
            sb.Append(Number);
            // put extension if present
            if (!string.IsNullOrEmpty(Extension)) {
                sb.Append(string.Format("EXT: {0}", Extension));
            }
            return sb.ToString();
        }
    }
}