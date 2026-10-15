namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// An account and its permissions included in a GP-API access token scope.
    /// </summary>
    public class GpApiAccount {
        /// <summary>
        /// The unique identifier of the account.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The display name of the account.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The permissions granted to the account in the access token scope.
        /// </summary>
        public string[] Permissions { get; set; }
    }
}
