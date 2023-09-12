using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;

namespace GlobalPayments.Api.Gateways {
    public abstract class NetworkConnector : NetworkGateway, IPaymentGateway {
        private IBatchProvider batchProvider;
        public AcceptorConfig AcceptorConfig { get; set; }
        public IBatchProvider BatchProvider {
            get {
                return batchProvider;
            }
            set {
                this.batchProvider = value;
                if (this.batchProvider != null && this.batchProvider.GetRequestEncoder() != null) {
                    RequestEncoder = batchProvider.GetRequestEncoder();
                }
            }
        }
        public CharacterSet characterSet = CharacterSet.ASCII;
        public string CompanyId { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public string MerchantType { get; set; }
        public string EWICMerchantId { get; set; }
        public MessageType MessageType { get; set; }
        public string NodeIdentification { get; set; }
        public ProtocolType ProtocolType { get; set; }
        public IRequestEncoder RequestEncoder { get; set; }
        public IStanProvider StanProvider { get; set; }
        public string TerminalId { get; set; }
        public string UniqueDeviceId { get; set; }
        public LinkedList<Transaction> ResentTransactions { get; set; }
        public Transaction ResentBatch { get; set; }
        public bool SupportsHostedPayments => false;
        public bool SupportsOpenBanking => false;


        public abstract Transaction ProcessAuthorization(AuthorizationBuilder builder);

        public abstract Transaction ManageTransaction(ManagementBuilder builder);

        public abstract string SerializeRequest(AuthorizationBuilder builder);
    }
}
