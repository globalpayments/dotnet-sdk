using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;

namespace GlobalPayments.Api {
    public class NetworkGatewayConfig : Configuration {
        private NetworkGatewayType _gatewayType;
        NetworkConnector gateway;

        public NetworkGatewayConfig(NetworkGatewayType networkGatewayType = NetworkGatewayType.VAPS) {
            _gatewayType = networkGatewayType;
        }
        public ConnectionType connectionType = ConnectionType.ISDN;
        public MessageType messageType = MessageType.Heartland_POS_8583;
        public ProtocolType protocolType = ProtocolType.TCP_IP;
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
        public string EWICMerchantID { get; set; }
        public MessageType MessageType { get; set; }
        private string nodeIdentification;
        public string NodeIdentification {
            get {
                if (!string.IsNullOrEmpty(nodeIdentification)) {
                    return nodeIdentification;
                }
                return "    ";
            }
            set {
                nodeIdentification = value;
            } }       

        public ProtocolType ProtocolType { get; set; }
        public IRequestEncoder RequestEncoder { get; set; }
        public IStanProvider StanProvider { get; set; }
        public string TerminalId { get; set; }
        public string UniqueDeviceId { get; set; }
        public List<Transaction> ResentTransactions { get; set; }
        public Transaction ResentBatch { get; set; }
        public string SecondaryEndpoint { get; set; }
        public int PrimaryPort { get; set; }
        public int SecondaryPort { get; set; }
        internal override void ConfigureContainer(ConfiguredServices services) {
            if (_gatewayType == NetworkGatewayType.NWS) {
                gateway = new NWSConnector();
            }
            //else if(_gatewayType == NetworkGatewayType.VAPS) {
            //    gateway = new VapsConnector();
            //}
            // connection fields
            gateway.PrimaryEndpoint = ServiceUrl;
            gateway.PrimaryPort = PrimaryPort;
            gateway.SecondaryEndpoint = SecondaryEndpoint;
            gateway.SecondaryPort = SecondaryPort;
            gateway.Timeout = Timeout;
            gateway.EnableLogging = EnableLogging;
            gateway.ForceGatewayTimeout = ForceGatewayTimeout;

            // other fields
            gateway.CompanyId = CompanyId;
            gateway.ConnectionType = connectionType;
            gateway.MessageType = messageType;
            gateway.NodeIdentification = NodeIdentification;
            gateway.ProtocolType = protocolType;
            gateway.TerminalId = TerminalId;
            gateway.MerchantType = MerchantType;
            gateway.EWICMerchantId = EWICMerchantID;
            gateway.UniqueDeviceId = UniqueDeviceId;

            // acceptor config
            if (AcceptorConfig == null) {
                AcceptorConfig = new AcceptorConfig();
            }
            gateway.AcceptorConfig = AcceptorConfig;

            // stan provider
            gateway.StanProvider = StanProvider;

            // batch provider
            gateway.BatchProvider = BatchProvider;

            services.GatewayConnector = gateway;
        }

        //@Override
        public new void Validate() {

        }
    }   
}
