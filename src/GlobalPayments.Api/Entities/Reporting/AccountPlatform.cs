namespace GlobalPayments.Api.Entities.Reporting {
    /// <summary>
    /// The processing platform associated with a GP-API account.
    /// </summary>
    public class AccountPlatform {
        /// <summary>
        /// The platform identifier.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The platform reference.
        /// </summary>
        public string Reference { get; set; }
        /// <summary>
        /// The platform provider name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The platform display name.
        /// </summary>
        public string PlatformName { get; set; }
        /// <summary>
        /// The platform merchant identifier.
        /// </summary>
        public string PlatformMid { get; set; }
        /// <summary>
        /// The masked certificate key returned by the gateway.
        /// </summary>
        public string MaskedCertstrKeyLast4 { get; set; }
        /// <summary>
        /// The platform terminal identifier.
        /// </summary>
        public string TermId { get; set; }
        /// <summary>
        /// The platform country code.
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// The platform contact email.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// The platform authentication setting.
        /// </summary>
        public string Authentication { get; set; }
        /// <summary>
        /// The masked secret key returned by the gateway, not the underlying credential.
        /// </summary>
        public string MaskedSecretKeyLast4 { get; set; }
    }
}
