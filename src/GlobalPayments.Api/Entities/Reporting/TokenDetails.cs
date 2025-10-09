using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Reporting {
    /// <summary>
    /// Represents the details of a token update event, 
    /// including information about card number and expiration date changes, 
    /// as well as the associated token and update date.
    /// </summary>
    public class TokenDetails {
        /// <summary>
        /// Represents details of a token update action, including card and expiration changes.
        /// </summary>
        public string UpdateAction { get; set; }
        /// <summary>
        /// The value of the token associated with the update.
        /// </summary>
        public string TokenValue { get; set; }
        /// <summary>
        /// The previous card number before the update.
        /// </summary>
        public string PreviousCardNumber { get; set; }
        /// <summary>
        /// The new card number after the update.
        /// </summary>
        public string NewCardNumber { get; set; }
        /// <summary>
        /// The previous expiration date before the update.
        /// </summary>
        public string PreviousExpirationDate { get; set; }
        /// <summary>
        /// The new expiration date after the update.
        /// </summary>
        public string NewExpirationDate { get; set; }
        /// <summary>
        /// The date when the update occurred.
        /// </summary>
        public string UpdateDate { get; set; }
    }
}
