namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Represents the display configuration for a Hosted Payment Page, controlling how the
    /// payment page is embedded within an iframe on the merchant's site.
    /// </summary>
    public class DisplayConfiguration {
        /// <summary>
        /// The domain used to size the iframe hosting the payment page.
        /// </summary>
        public string IframeDimensionsDomain { get; set; }

        /// <summary>
        /// The domain that will receive the response posted from the iframe hosting the payment page.
        /// </summary>
        public string IframeResponseDomain { get; set; }
    }
}
