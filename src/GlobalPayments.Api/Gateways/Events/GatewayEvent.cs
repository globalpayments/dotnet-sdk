using System;

namespace GlobalPayments.Api.Gateways.Events {
    public abstract class GatewayEvent {
        public string ConnectorName { get; set; }
        public GatewayEventType EventType { get; set; }
        public DateTime Timestamp { get; set; }

        internal GatewayEvent(string connectorName, GatewayEventType eventType) {
            this.ConnectorName = connectorName;
            this.EventType = eventType;
            this.Timestamp = DateTime.UtcNow;
        }
        public string GetTimestamp() {
            return Timestamp.ToString("MM-dd-yyyy hh:mm:ss.SSS");
        }
        public string GetEventMessage() {
            return string.Format("[{0}] - ", GetTimestamp());
        }
    }
}
