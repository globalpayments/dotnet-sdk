namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Specifies the data residency region for GP-API transactions.
    /// When set to EU, payment transaction data will stay within systems
    /// physically located within the EU region.
    /// </summary>
    public enum DataResidency {
        /// <summary>
        /// No specific data residency region; uses the default global endpoints.
        /// </summary>
        None,

        /// <summary>
        /// European Union data residency; routes transaction data through EU-based systems.
        /// </summary>
        EU
    }
}
