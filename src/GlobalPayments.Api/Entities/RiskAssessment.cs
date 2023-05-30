using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class RiskAssessment
    {
        /// <summary>
        /// A unique identifier for the risk assessment
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Time indicating when the object was created
        /// </summary>
        public DateTime? TimeCreated { get; set; }

        /// <summary>
        /// Indicates where the risk assessment is in its lifecycle.
        /// </summary>
        public RiskAssessmentStatus? Status { get; set; }

        /// <summary>
        /// The amount associated with the risk assessment.
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// The currency of the amount in ISO-4217(alpha-3)
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// A unique identifier for the merchant set by Global Payments
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// A meaningful label for the merchant set by Global Payments.
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// A unique identifier for the merchant account set by Global Payments.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// A meaningful label for the merchant account set by Global Payments.
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Merchant defined field to reference the risk assessment resource.
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// The result from the risk assessment service.
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// The result message from the risk assessment service that describes the result given.
        /// </summary>
        public string ResponseMessage { get; set; }

        
        public Card CardDetails { get; set; }

    
        public ThirdPartyResponse ThirdPartyResponse { get; set; }

        /// <summary>
        /// A unique identifier for the object created by Global Payments.
        /// </summary>
        public string ActionId { get; set; }
    }
}
