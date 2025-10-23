using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Enums {
    /// <summary>
    /// Specifies the storage mode for sensitive data.
    /// </summary>
    public enum StorageMode {
        /// <summary>
        /// Always store the data.
        /// </summary>
        ALWAYS,
        /// <summary>
        /// Never store the data.
        /// </summary>
        OFF,
        /// <summary>
        /// Prompt the user before storing the data.
        /// </summary>
        PROMPT
    }
}
