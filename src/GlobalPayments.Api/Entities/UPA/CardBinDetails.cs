namespace GlobalPayments.Api.Entities.UPA {
    /// <summary>
    /// Represents card BIN (Bank Identification Number) details retrieved from a UPA device.
    /// Contains information about card type, brand, and supported features.
    /// </summary>
    public class CardBinDetails {
        /// <summary>
        /// Gets or sets the type of card (e.g., Credit, Debit).
        /// </summary>
        public string CardType { get; set; }
        
        /// <summary>
        /// Gets or sets the card brand (e.g., Visa, Mastercard, American Express).
        /// </summary>
        public string CardBrand { get; set; }
        
        /// <summary>
        /// Gets or sets the abbreviated card brand name.
        /// </summary>
        public string CardBrandShortName { get; set; }
        
        /// <summary>
        /// Gets or sets a flag indicating whether card security (CVV/CVC) prompt is required.
        /// </summary>
        public int CardSecurityPromptFlag { get; set; }
        
        /// <summary>
        /// Gets or sets a flag indicating whether Address Verification System (AVS) is supported.
        /// </summary>
        public int AVSFlag { get; set; }
        
        /// <summary>
        /// Gets or sets a flag indicating whether cash back is supported for this card.
        /// </summary>
        public int CashBackFlag { get; set; }
        
        /// <summary>
        /// Gets or sets a flag indicating whether surcharge is applicable for this card.
        /// </summary>
        public int SurchargeFlag { get; set; }
        
        /// <summary>
        /// Gets or sets the EBT (Electronic Benefits Transfer) card type, if applicable.
        /// </summary>
        public string EBTCardType { get; set; }
        
        /// <summary>
        /// Gets or sets a flag indicating whether the card is eligible for Dynamic Currency Conversion (DCC).
        /// </summary>
        public int DCCEligible { get; set; }
    }
}
