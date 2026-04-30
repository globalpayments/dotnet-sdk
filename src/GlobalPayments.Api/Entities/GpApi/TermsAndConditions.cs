namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Represents the Terms and Conditions entity.
    /// </summary>
    public class TermsAndConditions {
        /// <summary>
        /// The URL of the terms and conditions.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The version of the terms and conditions.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The description of the terms and conditions.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The language of the terms and conditions.
        /// </summary>
        public string Language { get; set; }
    }
}
