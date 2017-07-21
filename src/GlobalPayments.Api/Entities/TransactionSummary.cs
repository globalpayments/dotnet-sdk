using System;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Transaction-level report data
    /// </summary>
    public class TransactionSummary {
        /// <summary>
        /// The originally requested authorization amount.
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// The originally requested convenience amount.
        /// </summary>
        public decimal? ConvenienceAmt { get; set; }

        /// <summary>
        /// The originally requested shipping amount.
        /// </summary>
        public decimal? ShippingAmt { get; set; }

        /// <summary>
        /// The authorization code provided by the issuer.
        /// </summary>
        public string AuthCode { get; set; }

        /// <summary>
        /// The authorized amount.
        /// </summary>
        public decimal? AuthorizedAmount { get; set; }

        /// <summary>
        /// The client transaction ID sent in the authorization request.
        /// </summary>
        public string ClientTransactionId { get; set; }

        /// <summary>
        /// The device ID where the transaction was ran; where applicable.
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// The original response code from the issuer.
        /// </summary>
        public string IssuerResponseCode { get; set; }

        /// <summary>
        /// The original response message from the issuer.
        /// </summary>
        public string IssuerResponseMessage { get; set; }

        /// <summary>
        /// The authorized card number, masked.
        /// </summary>
        public string MaskedCardNumber { get; set; }

        /// <summary>
        /// The gateway transaction ID of the authorization request.
        /// </summary>
        public string OriginalTransactionId { get; set; }

        /// <summary>
        /// The original response code from the gateway.
        /// </summary>
        public string GatewayResponseCode { get; set; }

        /// <summary>
        /// The original response message from the gateway.
        /// </summary>
        public string GatewayResponseMessage { get; set; }

        /// <summary>
        /// The reference number provided by the issuer.
        /// </summary>
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// The transaction type.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// The settled from the authorization.
        /// </summary>
        public decimal? SettlementAmount { get; set; }

        /// <summary>
        /// The transaction status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The date/time of the original transaction.
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// The gateway transaction ID of the transaction.
        /// </summary>
        public string TransactionId { get; set; }
    }
}
