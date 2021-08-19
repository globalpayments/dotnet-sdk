using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Billing {
    public class AuthorizationRecord {
        public string AddToBatchReferenceNumber { get; set; }
        public decimal? Amount { get; set; }
        public string AuthCode { get; set; }
        public string AuthorizationType { get; set; }
        public string AvsResultCode { get; set; }
        public string AvsResultText { get; set; }
        public string CardEntryMethod { get; set; }
        public string CvvResultCode { get; set; }
        public string CvvResultText { get; set; }
        public string EmvApplicationCryptogram { get; set; }
        public string EmvApplicationCryptogramType { get; set; }
        public string EmvApplicationID { get; set; }
        public string EmvApplicationName { get; set; }
        public string EmvCardholderVerificationMethod { get; set; }
        public string EmvIssuerResponse { get; set; }
        public string EmvSignatureRequired { get; set; }
        public string Gateway { get; set; }
        public string GatewayBatchID { get; set; }
        public string GatewayDescription { get; set; }
        public string MaskedAccountNumber { get; set; }
        public string MaskedRoutingNumber { get; set; }
        public string PaymentMethod { get; set; }
        public int? ReferenceAuthorizationID { get; set; }
        public string ReferenceNumber { get; set; }
        public string RoutingNumber { get; set; }
        public int? AuthorizationID { get; set; }
        public decimal? NetAmount { get; set; }
        public int? OriginalAuthorizationID { get; set; }
    }
}
