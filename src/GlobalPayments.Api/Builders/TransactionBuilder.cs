using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Elements;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.PaymentMethods;
using System.Collections.Generic;

namespace GlobalPayments.Api.Builders {
    public abstract class TransactionBuilder<TResult> : BaseBuilder<TResult> {
        internal TransactionType TransactionType { get; set; }
        internal TransactionModifier TransactionModifier { get; set; }
        internal IPaymentMethod PaymentMethod { get; set; }
        internal bool MultiCapture { get; set; }
        internal DccRateData DccRateData { get; set; }
        internal EWICData EwicData { get; set; }

        //network fields
        internal int BatchNumber{ get; set; }
        internal string CompanyId{ get; set; }
        internal FleetData FleetData{ get; set; }
        internal Dictionary<DE62_CardIssuerEntryTag, string> IssuerData{ get; set; }
        internal PriorMessageInformation PriorMessageInformation{ get; set; }
        internal ProductData ProductData { get; set; }
        internal string EWICIssuingEntity { get; set; }
        internal int SequenceNumber{ get; set; }
        internal int SystemTraceAuditNumber{ get; set; }
        internal string UniqueDeviceId{ get; set; }
        internal TransactionMatchingData TransactionMatchingData{ get; set; }
        // network fields
        internal bool ForceGatewayTimeout { get; set; }
        public TransactionBuilder(TransactionType type) : base() {
            TransactionType = type;
        }
        public TransactionBuilder(TransactionType type, IPaymentMethod paymentMethod) : base() {
            this.TransactionType = type;
            this.PaymentMethod = paymentMethod;
        }
    }
}
