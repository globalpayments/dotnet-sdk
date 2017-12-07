using GlobalPayments.Api.Builders;
using System;
using System.Text;

namespace GlobalPayments.Api.Entities {
    public class ThreeDSecure {
        /// <summary>
        /// The algorithm used.
        /// </summary>
        public int Algorithm { get; set; }

        internal decimal? Amount { get; set; }

        /// <summary>
        /// Consumer authentication (3DSecure) verification value.
        /// </summary>
        public string Cavv { get; set; }

        internal string Currency { get; set; }

        /// <summary>
        /// Consumer authentication (3DSecure) electronic commerce indicator.
        /// </summary>
        public string Eci { get; set; }

        /// <summary>
        /// The enrolment status:
        /// </summary>
        public bool Enrolled { get; set; }

        /// <summary>
        /// The URL of the Issuing Bank's ACS.
        /// </summary>
        public string IssuerAcsUrl { get; set; }

        public string MerchantData {
            get {
                if(Amount != null && Currency != null && OrderId != null)
                    return Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}.{1}.{2}", Amount, Currency, OrderId)));
                return string.Empty;
            }
            set {
                var merchantData = Encoding.UTF8.GetString(Convert.FromBase64String(value)).Split('.');
                Amount = decimal.Parse(merchantData[0]);
                Currency = merchantData[1];
                OrderId = merchantData[2];
            }
        }

        /// <summary>
        /// The order ID used for the initial transaction
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// The Payer Authentication Request returned by the Enrolment Server. Must be sent to the Issuing Bank's ACS (Access Control Server) URL.
        /// </summary>
        public string PayerAuthenticationRequest { get; set; }

        /// <summary>
        /// Consumer authentication (3DSecure) source.
        /// </summary>
        public string PaymentDataSource { get; set; }

        /// <summary>
        /// Consumer authentication (3DSecure) type.
        /// </summary>
        /// <remarks>
        /// Default value is `"3DSecure"`.
        /// </remarks>
        public string PaymentDataType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Consumer authentication (3DSecure) transaction ID.
        /// </summary>
        public string Xid { get; set; }

        public ThreeDSecure() {
            PaymentDataType = "3DSecure";
        }
    }
}
