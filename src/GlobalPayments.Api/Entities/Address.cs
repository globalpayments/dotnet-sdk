using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Represents a billing or shipping address for the consumer.
    /// </summary>
    public class Address {
        private string province;

        internal AddressType Type { get; set; }

        /// <summary>
        /// Consumer's street address 1.
        /// </summary>
        public string StreetAddress1 { get; set; }

        /// <summary>
        /// Consumer's street address 2.
        /// </summary>
        public string StreetAddress2 { get; set; }

        /// <summary>
        /// Consumer's street address 3.
        /// </summary>
        public string StreetAddress3 { get; set; }

        /// <summary>
        /// Consumer's city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Consumer's province.
        /// </summary>
        public string Province {
            get { return province; }
            set { province = value; }
        }

        /// <summary>
        /// Consumer's state.
        /// </summary>
        /// <remarks>
        /// Alias of `Address.Province`.
        /// </remarks>
        public string State {
            get { return province; }
            set { province = value; }
        }

        /// <summary>
        /// Consumer's postal/zip code.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Consumer's country.
        /// </summary>
        private string _country;
        public string Country {
            get {
                return _country;
            }
            set {
                _country = value;
                if (_countryCode == null)
                    _countryCode = CountryUtils.GetCountryCodeByCountry(_country);
            }
        }

        private string _countryCode;
        /// <summary>
        /// Consumer's country code
        /// </summary>
        public string CountryCode {
            get {
                return _countryCode;
            }
            set {
                _countryCode = value;
                if (_country == null)
                    _country = CountryUtils.GetCountryByCode(_countryCode);
            }
        }

        public bool IsCountry(string countryCode) {
            return CountryUtils.IsCountry(this, countryCode);
        }

        public string Name { get; set; }

        public Address(string streetAddress1 = null, string code = null) {
            this.StreetAddress1 = streetAddress1;
            this.PostalCode = code;
        }
    }
}
