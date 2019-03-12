using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;

namespace GlobalPayments.Api.Terminals {
    internal abstract class DeviceController : IDisposable {
        protected ITerminalConfiguration _settings;
        protected IDeviceCommInterface _interface;
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
        internal DeviceController(ITerminalConfiguration settings) {
            _settings = settings;
        }

        public byte[] Send(IDeviceMessage message) {
            return _interface?.Send(message);
        }

        internal abstract IDeviceInterface ConfigureInterface();

        internal abstract ITerminalResponse ProcessTransaction(TerminalAuthBuilder builder);
        internal abstract ITerminalResponse ManageTransaction(TerminalManageBuilder builder);
        internal abstract ITerminalReport ProcessReport(TerminalReportBuilder builder);

        public void Dispose() {
            _interface?.Disconnect();
        }
    }
}
