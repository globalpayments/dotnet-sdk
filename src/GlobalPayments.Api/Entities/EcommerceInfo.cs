using System;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Ecommerce specific data to pass during authorization/settlement.
    /// </summary>
    public class EcommerceInfo {
        /// <summary>
        /// Consumer authentication (3DSecure) verification value.
        /// </summary>
        public string Cavv { get; set; }

        /// <summary>
        /// Identifies eCommerce vs mail order / telephone order (MOTO) transactions.
        /// </summary>
        /// <remarks>
        /// Default value is `EcommerceChannel.ECOM`.
        /// </remarks>
        public EcommerceChannel Channel { get; set; }

        /// <summary>
        /// Consumer authentication (3DSecure) electronic commerce indicator.
        /// </summary>
        public string Eci { get; set; }

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
        /// The expected shipping month.
        /// </summary>
        /// <remarks>
        /// Default value is the date of one day in the future.
        /// </remarks>
        public int ShipDay { get; set; }

        /// <summary>
        /// The expected shipping month.
        /// </summary>
        /// <remarks>
        /// Default value is the month of one day in the future.
        /// </remarks>
        public int ShipMonth { get; set; }

        /// <summary>
        /// Consumer authentication (3DSecure) transaction ID.
        /// </summary>
        public string Xid { get; set; }

        public EcommerceInfo() {
            Channel = EcommerceChannel.ECOM;
            ShipDay = DateTime.Now.AddDays(1).Day;
            ShipMonth = DateTime.Now.AddDays(1).Month;
            PaymentDataType = "3DSecure";
        }
    }

    /// <summary>
    /// Identifies eCommerce vs mail order / telephone order (MOTO) transactions.
    /// </summary>
    public enum EcommerceChannel {
        /// <summary>
        /// Identifies eCommerce transactions.
        /// </summary>
        ECOM,
        /// <summary>
        /// Identifies mail order / telephone order (MOTO) transactions.
        /// </summary>
        MOTO
    }
}
