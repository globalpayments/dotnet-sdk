namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Contains the Visa Direct AFT supplementary data required by GPEcom API authorizations.
    /// </summary>
    public class VisaDirectAftData {
        /// <summary>
        /// Gets or sets the sender reference number, serialized as field01.
        /// </summary>
        public string SenderReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the sender account number, serialized as field02.
        /// </summary>
        public string SenderAccountNumber { get; set; }

        /// <summary>
        /// Gets or sets the sender name, serialized as field03.
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// Gets or sets the sender address, serialized as field04.
        /// </summary>
        public string SenderAddress { get; set; }

        /// <summary>
        /// Gets or sets the sender city, serialized as field05.
        /// </summary>
        public string SenderCity { get; set; }

        /// <summary>
        /// Gets or sets the sender country, serialized as field08.
        /// </summary>
        public string SenderCountry { get; set; }

        /// <summary>
        /// Gets or sets the account number type, serialized as field22.
        /// </summary>
        public string AccountNumberType { get; set; }

        /// <summary>
        /// Gets or sets the recipient account number, serialized as field23.
        /// </summary>
        public string RecipientAccountNumber { get; set; }
    }
}
