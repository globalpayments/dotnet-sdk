using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Represents a bank entity with basic banking information.
    /// </summary>
    public class Bank {
        /// <summary>
        /// Gets or sets the name of the bank.
        /// </summary>
        public string Name;

        /// <summary>
        /// Gets or sets the bank's identifier code (e.g., SWIFT/BIC).
        /// </summary>
        public string IdentifierCode;

        /// <summary>
        /// Gets or sets the International Bank Account Number (IBAN).
        /// </summary>
        public string Iban;

        /// <summary>
        /// Gets or sets the bank code.
        /// </summary>
        public string Code;

        /// <summary>
        /// Gets or sets the bank account number.
        /// </summary>
        public string AccountNumber;
    }
}
