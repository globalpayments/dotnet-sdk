using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.GpApi {
    /// <summary>
    /// Represents configuration settings required for Portico token authentication.
    /// </summary>
    public class PorticoTokenConfig {
        /// <summary>
        /// Gets or sets the site identifier.
        /// </summary>
        public int SiteId { get; set; }

        /// <summary>
        /// Gets or sets the account's license ID.
        /// </summary>
        public int LicenseId { get; set; }

        /// <summary>
        /// Gets or sets the account's device ID.
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the account's username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the account's password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the secret API key used for authenticating requests.
        /// </summary>
        /// <remarks>
        /// This property should be handled with care to avoid exposing sensitive information.
        /// Ensure the API key is stored securely and only accessed by authorized components.
        /// </remarks>
        public string SecretApiKey { get; set; }
    }
}
