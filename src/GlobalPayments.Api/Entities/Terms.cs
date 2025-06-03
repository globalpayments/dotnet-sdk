using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Represents the terms result return by GP API
    /// </summary>
    public class Terms {
        /// <summary>
        /// Reprenents the reference to the installment option being offered. 
        /// This field is applicable only if the installment.program is SIP.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Indicates if installment.term.time_unit_number is days, months or years
        /// </summary>
        public string TimeUnit { get; set; }
        /// <summary>
        /// Indicates the total number of payments to be made over the course of the installment payment plan.
        /// </summary>
        public List<int> TimeUnitNumbers { get; set; }
    }
}
