using System;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Ecommerce specific data to pass during authorization/settlement.
    /// </summary>
    public class EcommerceInfo {
        /// <summary>
        /// Identifies eCommerce vs mail order / telephone order (MOTO) transactions.
        /// </summary>
        /// <remarks>
        /// Default value is `EcommerceChannel.ECOM`.
        /// </remarks>
        public EcommerceChannel Channel { get; set; }

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

        public EcommerceInfo() {
            Channel = EcommerceChannel.ECOM;
            ShipDay = DateTime.Now.AddDays(1).Day;
            ShipMonth = DateTime.Now.AddDays(1).Month;
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
        MOTO,
        /// <summary>
        /// Identifies mail order transactions.
        /// </summary>
        MAIL,
        /// <summary>
        /// Identifies telephone order transactions.
        /// </summary>
        PHONE
    }
}
