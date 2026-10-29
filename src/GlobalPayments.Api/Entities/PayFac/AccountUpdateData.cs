using GlobalPayments.Api.Entities.Enums;
using System.Collections.Generic;

namespace GlobalPayments.Api.Entities.PayFac {
    /// <summary>
    /// Optional GP-API account edits, used with PayFacService.EditAccount.
    /// </summary>
    public class AccountUpdateData {
        /// <summary>
        /// The account status to apply; omitted status is unchanged.
        /// </summary>
        public MerchantAccountStatus? Status { get; set; }

        /// <summary>
        /// The payer's payment method reference.
        /// </summary>
        public string PaymentMethodReference { get; set; }

        /// <summary>
        /// The prepaid card replacement reason, for example LOST.
        /// </summary>
        public string CardReplacementReason { get; set; }

        /// <summary>
        /// The account status change reason, for example ReadyToProcess.
        /// </summary>
        public string StatusChangeReason { get; set; }

        /// <summary>
        /// The HTTPS endpoint to receive account status notifications.
        /// </summary>
        public string StatusUrl { get; set; }

        /// <summary>
        /// The capabilities to enable or disable on the account.
        /// </summary>
        public List<string> Capabilities { get; set; }

        /// <summary>
        /// The processing limits to update; omitted limits are unchanged.
        /// </summary>
        public AccountProcessingLimits ProcessingLimits { get; set; }
    }
}
