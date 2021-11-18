using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.Messaging;

namespace GlobalPayments.Api.Terminals {
    public abstract class DeviceController : IDisposable {
        protected IDeviceInterface _interface;
        protected ITerminalConfiguration _settings;
        protected IDeviceCommInterface _connector;

        public ConnectionModes? ConnectionMode {
            get {
                if (_settings != null)
                    return _settings.ConnectionMode;
                return null;
            }
        }
        public DeviceType? DeviceType {
            get {
                if (_settings != null)
                    return _settings.DeviceType;
                return null;
            }
        }
        public IRequestIdProvider RequestIdProvider {
            get {
                if (_settings != null)
                    return _settings.RequestIdProvider;
                return null;
            }
        }

        public event MessageSentEventHandler OnMessageSent;

        internal DeviceController(ITerminalConfiguration settings) {
            _settings = settings;
            _connector = ConfigureConnector();
            _connector.OnMessageSent += (message) => {
                OnMessageSent?.Invoke(message);
            };
        }

        public byte[] Send(IDeviceMessage message) {
            message.AwaitResponse = true;
            return _connector?.Send(message);
        }

        internal abstract IDeviceInterface ConfigureInterface();
        internal abstract IDeviceCommInterface ConfigureConnector();

        internal abstract ITerminalResponse ProcessTransaction(TerminalAuthBuilder builder);
        internal abstract ITerminalResponse ManageTransaction(TerminalManageBuilder builder);
        internal abstract ITerminalReport ProcessReport(TerminalReportBuilder builder);

        internal abstract byte[] SerializeRequest(TerminalAuthBuilder builder);
        internal abstract byte[] SerializeRequest(TerminalManageBuilder builder);
        internal abstract byte[] SerializeRequest(TerminalReportBuilder builder);

        public void Dispose() {
            _connector?.Disconnect();
        }
    }
}
