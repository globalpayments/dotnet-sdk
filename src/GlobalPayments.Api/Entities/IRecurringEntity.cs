namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Base interface for recurring resource types.
    /// </summary>
    public interface IRecurringEntity {
        /// <summary>
        /// All resource should be supplied a merchant-/application-defined ID.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// All resources should be supplied a gateway-defined ID.
        /// </summary>
        string Key { get; set; }
    }
}
