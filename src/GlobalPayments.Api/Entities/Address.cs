namespace GlobalPayments.Api.Entities
{
    public class Address
    {
        private string province;

        internal AddressType Type { get; set; }
        public string StreetAddress1 { get; set; }
        public string StreetAddress2 { get; set; }
        public string StreetAddress3 { get; set; }
        public string City { get; set; }
        public string Province {
            get { return province; }
            set { province = value; }
        }
        public string State {
            get { return province; }
            set { province = value; }
        }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}
