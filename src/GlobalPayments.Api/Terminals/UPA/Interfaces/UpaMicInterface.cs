using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;

namespace GlobalPayments.Api.Terminals.UPA {
    internal class UpaMicInterface : IDeviceCommInterface {
        private ITerminalConfiguration _config;
        private GpApiConfig _gatewayConfig;
        private GpApiConnector _connector;

        public event MessageSentEventHandler OnMessageSent;

        public event MessageReceivedEventHandler OnMessageReceived;

        public UpaMicInterface(ITerminalConfiguration config) {
            _config = config;
            _gatewayConfig = _config.GatewayConfig as GpApiConfig;
        }

        public void Connect() {
            _connector = ServicesContainer.Instance.GetClient("_upa_passthrough") as GpApiConnector;
        }

        public void Disconnect() { /* NOM NOM */ }

        public byte[] Send(IDeviceMessage message) {
            Connect();

            string requestId = message.GetRequestField<JsonDoc>("data")?.GetValue<string>("requestId");

            var request = new JsonDoc();
            request.Set("merchant_id", _gatewayConfig.MerchantId);
            request.Set("account_id", _gatewayConfig.AccessTokenInfo?.TransactionProcessingAccountID);
            request.Set("account_name", _gatewayConfig.AccessTokenInfo?.TransactionProcessingAccountName);
            request.Set("channel", EnumConverter.GetMapping(Target.GP_API, _gatewayConfig.Channel));
            request.Set("country", _gatewayConfig.Country);
            request.Set("currency", _gatewayConfig.DeviceCurrency);
            request.Set("reference", requestId);
            request.Set("request", message.GetRequestBuilder() as JsonDoc);

            JsonDoc notifcations = request.SubElement("notifications");
            notifcations.Set("status_url", _gatewayConfig.MethodNotificationUrl);

            string response = _connector.ProcessPassThrough(request);

            var rBuffer = new List<byte>();
            foreach (char c in response) {
                rBuffer.Add((byte)c);
            }
            return rBuffer.ToArray();
        }
    }
}
