using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;

namespace GlobalPayments.Api.Terminals {
    public class DeviceResponse : IDeviceResponse, IBatchCloseResponse, ITerminalReport {
        /// <summary>
        /// device status at the time of transaction
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// the command used in the transaction
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// the version of software the terminal is running
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// the unique id of the terminal - Serial Number for Pax
        /// </summary>
        public string DeviceId { get; set; }

        // Functional
        /// <summary>
        /// response code returned by the device
        /// </summary>
        public string DeviceResponseCode { get; set; }

        /// <summary>
        /// response text returned by the device
        /// </summary>
        public string DeviceResponseText { get; set; }

        /// <summary>
        /// ECR reference number for the transaction
        /// </summary>
        public string ReferenceNumber { get; set; }
        public string SequenceNumber { get; set; }
        public string TotalCount { get; set; }
        public string TotalAmount { get; set; }
    }
}
