namespace GlobalPayments.Api.Entities
{
    /// <summary>
    /// Represents a billing or shipping address for the consumer.
    /// </summary>
    public class Address
    {
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
        public string Country { get; set; }
    }
}
