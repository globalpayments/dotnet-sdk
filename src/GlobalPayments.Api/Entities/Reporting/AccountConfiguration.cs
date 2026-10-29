using System.Collections.Generic;

namespace GlobalPayments.Api.Entities.Reporting {
    /// <summary>
    /// A supported combination of GP-API account processing settings.
    /// </summary>
    public class AccountConfiguration {
        /// <summary>
        /// The processing configuration identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The processing configuration status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The device serial number, when assigned.
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// The actions supported by this configuration.
        /// </summary>
        public List<string> Actions { get; set; }

        /// <summary>
        /// The processing platform for this configuration.
        /// </summary>
        public AccountPlatform Platform { get; set; }

        /// <summary>
        /// The supported country codes.
        /// </summary>
        public List<string> Countries { get; set; }

        /// <summary>
        /// The supported currency codes.
        /// </summary>
        public List<string> Currencies { get; set; }

        /// <summary>
        /// The supported processing channels.
        /// </summary>
        public List<Channel> Channels { get; set; }

        /// <summary>
        /// The supported payment methods.
        /// </summary>
        public List<PaymentMethodName> PaymentMethods { get; set; }
    }
}
