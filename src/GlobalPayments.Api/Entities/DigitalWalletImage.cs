namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Card art metadata returned by the GP-API Click to Pay decrypt endpoint.
    /// </summary>
    public class DigitalWalletImage {
        /// <summary>
        /// The width of the card art image, in pixels.
        /// </summary>
        public string Width { get; set; }
        /// <summary>
        /// The height of the card art image, in pixels.
        /// </summary>
        public string Height { get; set; }
        /// <summary>
        /// The URL where the card art image can be retrieved.
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// The status of the card art image.
        /// </summary>
        public string Status { get; set; }
    }
}
