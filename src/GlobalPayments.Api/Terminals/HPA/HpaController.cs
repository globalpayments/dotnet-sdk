using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.HPA.Interfaces;
using GlobalPayments.Api.Terminals.HPA.Responses;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Utils;
using System;
using System.Text;

namespace GlobalPayments.Api.Terminals.HPA {
    internal class HpaController : DeviceController {
        IDeviceInterface _device;

        public event MessageSentEventHandler OnMessageSent;

        internal MessageFormat Format {
            get {
                if (_settings != null)
                    return ConnectionMode == ConnectionModes.TCP_IP ? MessageFormat.HPA : MessageFormat.Visa2nd;
                return MessageFormat.Visa2nd;
            }
        }

        internal override IDeviceInterface ConfigureInterface() {
            if (_device == null)
                _device = new HpaInterface(this);
            return _device;
        }

        internal HpaController(ITerminalConfiguration settings) : base(settings) {
            switch (_settings.ConnectionMode) {
                case ConnectionModes.TCP_IP:
                    _interface = new HpaTcpInterface(settings);
                    break;
                default:
                    throw new NotImplementedException("Connection Method not available for HPA devices");
            }

            //_interface.Connect();
            _interface.OnMessageSent += (message) => {
                OnMessageSent?.Invoke(message);
            };
        }

        internal T SendMessage<T>(string message, params string[] messageIds) where T : SipBaseResponse {
            return SendMessage<T>(message, false, true, messageIds);
        }
        internal T SendMessage<T>(string message, bool keepAlive, bool awaitResponse, params string[] messageIds) {
            IDeviceMessage deviceMessage = TerminalUtilities.BuildRequest(message, Format);
            deviceMessage.KeepAlive = keepAlive;
            deviceMessage.AwaitResponse = awaitResponse;

            var response = _interface.Send(deviceMessage);
            if (awaitResponse) {
                return (T)Activator.CreateInstance(typeof(T), response, messageIds);
            }
            else return default(T);
        }

        internal T SendAdminMessage<T>(HpaAdminBuilder builder) where T : SipBaseResponse {
            int requestId = 1004;
            if (RequestIdProvider != null) {
                requestId = RequestIdProvider.GetRequestId();
            }
            builder.Set("RequestId", requestId);
            return SendMessage<T>(builder.BuildMessage(), builder.KeepAlive, builder.AwaitResponse, builder.MessageIds);
        }

        internal override ITerminalResponse ProcessTransaction(TerminalAuthBuilder builder) {
            var transactionType = MapTransactionType(builder.TransactionType);
            var response = SendMessage<SipDeviceResponse>(BuildProcessTransaction(builder), transactionType);
            return response;
        }
        
        internal override ITerminalResponse ManageTransaction(TerminalManageBuilder builder) {
            var transactionType = MapTransactionType(builder.TransactionType);
            var response = SendMessage<SipDeviceResponse>(BuildManageTransaction(builder), transactionType);
            return response;
        }

        internal override ITerminalReport ProcessReport(TerminalReportBuilder builder) {
            throw new NotImplementedException();
        }

        internal string BuildProcessTransaction(TerminalAuthBuilder builder) {
            int requestId = builder.ReferenceNumber;
            if (requestId == default(int) && RequestIdProvider != null) {
                requestId = RequestIdProvider.GetRequestId();
            }
            ElementTree et = new ElementTree();
            var transactionType = MapTransactionType(builder.TransactionType);

            Element request = et.Element("SIP");
            et.SubElement(request, "Version").Text("1.0");
            et.SubElement(request, "ECRId").Text("1004");
            et.SubElement(request, "Request").Text(transactionType);
            et.SubElement(request, "RequestId", requestId);
            et.SubElement(request, "CardGroup", builder.PaymentMethodType.ToString());
            et.SubElement(request, "ConfirmAmount").Text("0");
            et.SubElement(request, "BaseAmount").Text(builder.Amount.ToNumericCurrencyString());
            if (builder.Gratuity != null)
                et.SubElement(request, "TipAmount").Text(builder.Gratuity.ToNumericCurrencyString());
            else et.SubElement(request, "TipAmount").Text("0");

            // EBT amount
            if (builder.PaymentMethodType == PaymentMethodType.EBT)
                et.SubElement(request, "EBTAmount").Text(builder.Amount.ToNumericCurrencyString());

            // total
            et.SubElement(request, "TotalAmount").Text(builder.Amount.ToNumericCurrencyString());

            return et.ToString(request);
        }

        internal string BuildManageTransaction(TerminalManageBuilder builder) {
            int requestId = builder.ReferenceNumber;
            if (requestId == default(int) && RequestIdProvider != null) {
                requestId = RequestIdProvider.GetRequestId();
            }
            ElementTree et = new ElementTree();
            var transactionType = MapTransactionType(builder.TransactionType);

            Element request = et.Element("SIP");
            et.SubElement(request, "Version").Text("1.0");
            et.SubElement(request, "ECRId").Text("1004");
            et.SubElement(request, "Request").Text(MapTransactionType(builder.TransactionType));
             if ((builder.TransactionType == TransactionType.Capture)||(builder.TransactionType == TransactionType.Void))
                    et.SubElement(request, "RequestId", requestId);
            et.SubElement(request, "TransactionId", builder.TransactionId);

            if (builder.Gratuity != null)
                et.SubElement(request, "TipAmount").Text(builder.Gratuity.ToNumericCurrencyString());

            return et.ToString(request);
        }

        internal override byte[] SerializeRequest(TerminalAuthBuilder builder) {
            return Encoding.UTF8.GetBytes(BuildProcessTransaction(builder));
        }
        internal override byte[] SerializeRequest(TerminalManageBuilder builder) {
            return Encoding.UTF8.GetBytes(BuildManageTransaction(builder));
        }
        internal override byte[] SerializeRequest(TerminalReportBuilder builder) {
            throw new NotImplementedException();
        }

        private string MapTransactionType(TransactionType type) {
            switch (type) {
                case TransactionType.Sale:
                    return HPA_MSG_ID.CREDIT_SALE;
                case TransactionType.Verify:
                    return HPA_MSG_ID.CARD_VERIFY;
                case TransactionType.Refund:
                    return HPA_MSG_ID.CREDIT_REFUND;
                case TransactionType.Void:
                    return HPA_MSG_ID.CREDIT_VOID;
                case TransactionType.Balance:
                    return HPA_MSG_ID.BALANCE;
                case TransactionType.AddValue:
                    return HPA_MSG_ID.ADD_VALUE;
                case TransactionType.Auth:
                    return HPA_MSG_ID.CREDIT_AUTH;
                case TransactionType.Edit:
                    return HPA_MSG_ID.TIP_ADJUST;
                case TransactionType.Capture:
                    return HPA_MSG_ID.CAPTURE;
                default:
                    throw new UnsupportedTransactionException();
            }
        }

        public new void Dispose() {
            _device.Dispose();
            _interface.Disconnect();
        }
    }
}