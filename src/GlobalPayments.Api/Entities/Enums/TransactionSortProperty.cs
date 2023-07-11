using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    [MapTarget(Target.GP_API)]
    public enum TransactionSortProperty {
        /// <summary>
        /// Only available for Transactions report
        /// </summary>
        [Map(Target.GP_API, "ID")]
        Id,

        /// <summary>
        /// Avalable for both Transactions and Settled Transactions report
        /// </summary>
        [Map(Target.GP_API, "TIME_CREATED")]
        TimeCreated,

        /// <summary>
        /// Only available for Settled Transactions report
        /// </summary>
        [Map(Target.GP_API, "STATUS")]
        Status,

        /// <summary>
        /// Avalable for both Transactions and Settled Transactions report
        /// </summary>
        [Map(Target.GP_API, "TYPE")]
        Type,

        /// <summary>
        /// Only available for Settled Transactions report
        /// </summary>
        [Map(Target.GP_API, "DEPOSIT_ID")]
        DepositId
    }
}
