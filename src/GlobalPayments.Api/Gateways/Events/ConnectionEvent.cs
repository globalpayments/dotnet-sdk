using System;

namespace GlobalPayments.Api.Gateways.Events {
    public class ConnectionEvent : GatewayEvent {
        public string Endpoint { get; set; }
        public string Port { get; set; }
        public string Host { get; set; }
        public DateTime ConnectionStarted { get; set; }
        public DateTime ConnectionCompleted { get; set; }
        public int ConnectionAttempts { get; set; }
        public DateTime ConnectionFailOver { get; set; }
        public bool SslNavigation { get; set; }       
        private long GetConnectionTime() {
            DateTime d1 = ConnectionCompleted != null ? ConnectionCompleted : ConnectionFailOver;

            if (d1 != null) {
                return d1.Millisecond - ConnectionStarted.Millisecond;
            }
            else {
                return 0;
            }
        }
        public new string GetEventMessage() {
            string rvalue = base.GetEventMessage();
            return string.Concat(rvalue,string.Format("Connecting to {0} host ({1}:{2}); started: {3}; ssl success: {4}; completed: {5}; fail over: {6}; attempts: {7}; connection time (milliseconds): {8}ms.",
                    Host,
                    Endpoint,
                    Port,
                    ConnectionStarted.ToString("hh:mm:ss.SSS"),
                    SslNavigation,
                    ConnectionCompleted != null ? ConnectionCompleted.ToString("hh:mm:ss.SSS") : "null",
                    ConnectionFailOver != null ? ConnectionFailOver.ToString("hh:mm:ss.SSS") : "null",
                    ConnectionAttempts + 1,
                    GetConnectionTime()
            ));
        }

        public ConnectionEvent(string connectorName) : base(connectorName, GatewayEventType.Connection) { }
    }
}
