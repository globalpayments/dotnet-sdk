using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.Genius.Interfaces;
using GlobalPayments.Api.Utils;
using System;
using System.Text;

namespace GlobalPayments.Api.Terminals.Genius {
    public class GeniusController : DeviceController {
        GeniusConfig _gatewayConfig;

        internal override IDeviceInterface ConfigureInterface() {
            if (_interface == null) {
                _interface = new GeniusInterface(this);
            }
            return _interface;
        }
        internal override IDeviceCommInterface ConfigureConnector() {
            switch (_settings.ConnectionMode) {
                case ConnectionModes.HTTP:
                    return new GeniusHttpInterface(_settings);
                case ConnectionModes.SERIAL:
                case ConnectionModes.SSL_TCP:
                case ConnectionModes.TCP_IP:
                default:
                    throw new NotImplementedException();
            }
        }

        internal GeniusController(ITerminalConfiguration settings) : base(settings) {
            if (settings.GatewayConfig is GeniusConfig) {
                _gatewayConfig = settings.GatewayConfig as GeniusConfig;
            }
            else throw new ConfigurationException("GatewayConfig must be of type GeniusConfig for this device type.");
        }

        internal override ITerminalResponse ProcessTransaction(TerminalAuthBuilder builder) {
            IDeviceMessage message = BuildProcessTransaction(builder);
            var response = _connector.Send(message);
            throw new NotImplementedException();
        }

        internal override ITerminalResponse ManageTransaction(TerminalManageBuilder builder) {
            throw new NotImplementedException();
        }

        internal override ITerminalReport ProcessReport(TerminalReportBuilder builder) {
            throw new NotImplementedException();
        }

        private IDeviceMessage BuildProcessTransaction(TerminalAuthBuilder builder) {
            ElementTree et = new ElementTree();

            var create = et.Element("CreateTransaction").Set("xmlns", "http://transport.merchantware.net/v4/");
            et.SubElement(create, "merchantName").Text(_gatewayConfig.MerchantName);
            et.SubElement(create, "merchantSiteId").Text(_gatewayConfig.MerchantSiteId);
            et.SubElement(create, "merchantKey").Text(_gatewayConfig.MerchantKey);

            var request = et.SubElement(create, "request");
            et.SubElement(request, "TransactionType", MapRequestType(builder));
        	et.SubElement(request, "Amount", builder.Amount.ToCurrencyString());
            et.SubElement(request, "ClerkId", _gatewayConfig.ClerkId ?? _gatewayConfig.RegisterNumber); // TODO: This should come from the config somewhere
            et.SubElement(request, "OrderNumber", builder.InvoiceNumber);
            et.SubElement(request, "Dba", _gatewayConfig.DBA); // TODO: This should come from the config somewhere
            et.SubElement(request, "SoftwareName", "GP SDK"); // TODO: This should come from the config somewhere
            et.SubElement(request, "SoftwareVersion", "2.*"); // TODO: This should come from the config somewhere
            et.SubElement(request, "TransactionId", builder.ClientTransactionId);
            et.SubElement(request, "TerminalId", _gatewayConfig.TerminalId);
            et.SubElement(request, "PoNumber", builder.PoNumber);
            et.SubElement(request, "ForceDuplicate", builder.AllowDuplicates);

            byte[] payload = Encoding.UTF8.GetBytes(BuildEnvelope(et, create));
            return new DeviceMessage(payload);
        }

        internal override byte[] SerializeRequest(TerminalAuthBuilder builder) {
            return BuildProcessTransaction(builder).GetSendBuffer();
        }

        internal override byte[] SerializeRequest(TerminalManageBuilder builder) {
            throw new NotImplementedException();
        }

        internal override byte[] SerializeRequest(TerminalReportBuilder builder) {
            throw new NotImplementedException();
        }

        private string BuildEnvelope(ElementTree et, Element transaction) {
            var envelope = et.Element("soap:Envelope");
            var body = et.SubElement(envelope, "soap:Body");

            body.Append(transaction);

            return et.ToString(envelope);
        }

        private string MapRequestType(TerminalAuthBuilder builder) {
            TransactionType transType = builder.TransactionType;

            switch (transType) {
                case TransactionType.Auth: {
                        if (builder.TransactionModifier.Equals(TransactionModifier.Offline)) {
                            return "ForceCapture";
                        }
                        return "Authorize";
                    }
                case TransactionType.BatchClose: {
                        return "SettleBatch";
                    }
                case TransactionType.Capture: {
                        return "Capture";
                    }
                case TransactionType.Edit: {
                        return "AdjustTip";
                    }
                //AttachSignature
                //FindBoardedCard
                case TransactionType.Refund: {
                        return "Refund";
                    }
                case TransactionType.Sale: {
                        return "Sale";
                    }
                case TransactionType.TokenDelete: {
                        return "UnboardCard";
                    }
                case TransactionType.TokenUpdate: {
                        return "UpdateBoardedCard";
                    }
                case TransactionType.Verify: {
                        return "BoardCard";
                    }
                case TransactionType.Void: {
                        return "Void";
                    }
                default: { throw new UnsupportedTransactionException(); }
            }
        }
    }
}
