using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.Enums {
    /// <summary>
    /// Specifies the storage mode for sensitive data.
    /// </summary>
    public enum StorageMode {
        /// <summary>
        /// Always store the data.
        /// </summary>
        [Map(Target.GP_API, "ALWAYS")]
        ALWAYS,
        /// <summary>
        /// Never store the data.
        /// </summary>
        [Map(Target.GP_API, "OFF")]
        OFF,
        /// <summary>
        /// Prompt the user before storing the data.
        /// </summary>
        [Map(Target.GP_API, "PROMPT")]
        PROMPT,
        /// <summary>
        /// Store the data only if the transaction is successful.
        /// </summary>
        [Map(Target.GP_API, "ON_SUCCESS")]
        ON_SUCCESS
    }
}
