using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class PaymentStatistics
    {
        /// <summary>
        /// The total monthly sales of the merchant.
        /// </summary>
        public decimal? TotalMonthlySalesAmount { get; set; }

        /// <summary>
        /// The total monthly sales of the merchant.
        /// </summary>
        public decimal? AverageTicketSalesAmount { get; set; }

        /// <summary>
        /// The merchants highest ticket amount.
        /// </summary>
        public decimal? HighestTicketSalesAmount { get; set; }
    }
}
