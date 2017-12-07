using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.HeartSIP.Interfaces;
using GlobalPayments.Api.Terminals.HeartSIP.Responses;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.HeartSIP {
    internal class HeartSipController : DeviceController {
        IDeviceInterface _device;

        public event MessageSentEventHandler OnMessageSent;

        internal MessageFormat Format {
            get {
                if (_settings != null)
                    return ConnectionMode == ConnectionModes.TCP_IP ? MessageFormat.HeartSIP : MessageFormat.Visa2nd;
                return MessageFormat.Visa2nd;
            }
        }

        internal override IDeviceInterface ConfigureInterface() {
            if (_device == null)
                _device = new HeartSipInterface(this);
            return _device;
        }

        internal HeartSipController(ITerminalConfiguration settings) : base(settings) {
            switch (_settings.ConnectionMode) {
                case ConnectionModes.TCP_IP:
                    _interface = new HeartSipTcpInterface(settings);
                    break;
                default:
                    throw new NotImplementedException("Connection Method not available for HeartSIP devices");
            }

            //_interface.Connect();
            _interface.OnMessageSent += (message) => {
                OnMessageSent?.Invoke(message);
            };
        }

        internal T SendMessage<T>(string message, params string[] messageIds) where T : SipBaseResponse {
            var response = _interface.Send(TerminalUtilities.BuildRequest(message, Format));
            return (T)Activator.CreateInstance(typeof(T), response, messageIds);
        }

        internal override TerminalResponse ProcessTransaction(TerminalAuthBuilder builder) {
            ElementTree et = new ElementTree();
            var transactionType = MapTransactionType(builder.TransactionType);

            Element request = et.Element("SIP");
            et.SubElement(request, "Version").Text("1.0");
            et.SubElement(request, "ECRId").Text("1004");
            et.SubElement(request, "Request").Text(transactionType);
            et.SubElement(request, "RequestId", builder.ReferenceNumber);
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

            var response = SendMessage<SipDeviceResponse>(et.ToString(request), transactionType);
            return response;
        }

        internal override TerminalResponse ManageTransaction(TerminalManageBuilder builder) {
            ElementTree et = new ElementTree();
            var transactionType = MapTransactionType(builder.TransactionType);

            Element request = et.Element("SIP");
            et.SubElement(request, "Version").Text("1.0");
            et.SubElement(request, "ECRId").Text("1004");
            et.SubElement(request, "Request").Text(MapTransactionType(builder.TransactionType));
            if (builder.TransactionType == TransactionType.Capture)
                et.SubElement(request, "RequestId", builder.ReferenceNumber);
            et.SubElement(request, "TransactionId", builder.TransactionId);

            if (builder.Gratuity != null)
                et.SubElement(request, "TipAmount").Text(builder.Gratuity.ToNumericCurrencyString());

            var response = SendMessage<SipDeviceResponse>(et.ToString(request), transactionType);
            return response;
        }

        private string MapTransactionType(TransactionType type) {
            switch (type) {
                case TransactionType.Sale:
                    return HSIP_MSG_ID.CREDIT_SALE;
                case TransactionType.Verify:
                    return HSIP_MSG_ID.CARD_VERIFY;
                case TransactionType.Refund:
                    return HSIP_MSG_ID.CREDIT_REFUND;
                case TransactionType.Void:
                    return HSIP_MSG_ID.CREDIT_VOID;
                case TransactionType.Balance:
                    return HSIP_MSG_ID.BALANCE;
                case TransactionType.AddValue:
                    return HSIP_MSG_ID.ADD_VALUE;
                case TransactionType.Auth:
                    return HSIP_MSG_ID.CREDIT_AUTH;
                case TransactionType.Edit:
                    return HSIP_MSG_ID.TIP_ADJUST;
                case TransactionType.Capture:
                    return HSIP_MSG_ID.CAPTURE;
                default:
                    throw new UnsupportedTransactionException();
            }
        }

        public new void Dispose() {
            _device.Dispose();
            _interface.Disconnect();
        }
    }

    public class HeartSipInterface : IDeviceInterface {
        private HeartSipController _controller;
        public event MessageSentEventHandler OnMessageSent;

        internal HeartSipInterface(HeartSipController controller) {
            _controller = controller;
            _controller.OnMessageSent += (message) => {
                OnMessageSent?.Invoke(message);
            };
        }

        #region Admin Messages
        public void Cancel() {
            var response = Reset();
        }

        public IDeviceResponse DisableHostResponseBeep() {
            throw new NotImplementedException();
        }

        public IDeviceResponse CloseLane() {
            return _controller.SendMessage<SipBaseResponse>("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>LaneClose</Request></SIP>", HSIP_MSG_ID.LANE_CLOSE);
        }

        public IInitializeResponse Initialize() {
            return _controller.SendMessage<SipInitializeResponse>("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>GetAppInfoReport</Request></SIP>", HSIP_MSG_ID.GET_INFO_REPORT);
        }

        public IDeviceResponse OpenLane() {
            return _controller.SendMessage<SipBaseResponse>("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>LaneOpen</Request></SIP>", HSIP_MSG_ID.LANE_OPEN);
        }

        public IDeviceResponse Reboot() {
            return _controller.SendMessage<SipBaseResponse>("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>Reboot</Request></SIP>", HSIP_MSG_ID.REBOOT);
        }

        public IDeviceResponse Reset() {
            return _controller.SendMessage<SipBaseResponse>("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>Reset</Request></SIP>", HSIP_MSG_ID.RESET);
        }
        #endregion

        #region Batching
        public IBatchCloseResponse BatchClose() {
            return _controller.SendMessage<SipBatchResponse>("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>CloseBatch</Request></SIP>", HSIP_MSG_ID.BATCH_CLOSE, HSIP_MSG_ID.GET_BATCH_REPORT);
        }
        #endregion

        #region Credit
        public TerminalAuthBuilder CreditAuth(int referenceNumber, decimal? amount = default(decimal?)) {
            return new TerminalAuthBuilder(TransactionType.Auth, PaymentMethodType.Credit).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }

        public TerminalManageBuilder CreditCapture(int referenceNumber, decimal? amount = default(decimal?)) {
            return new TerminalManageBuilder(TransactionType.Capture, PaymentMethodType.Credit).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }

        public TerminalAuthBuilder CreditRefund(int referenceNumber, decimal? amount = default(decimal?)) {
            return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.Credit).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }

        public TerminalAuthBuilder CreditSale(int referenceNumber, decimal? amount = default(decimal?)) {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Credit).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }

        public TerminalAuthBuilder CreditVerify(int referenceNumber) {
            return new TerminalAuthBuilder(TransactionType.Verify, PaymentMethodType.Credit).WithReferenceNumber(referenceNumber);
        }

        public TerminalManageBuilder CreditVoid(int referenceNumber) {
            return new TerminalManageBuilder(TransactionType.Void, PaymentMethodType.Credit).WithReferenceNumber(referenceNumber);
        }
        #endregion

        #region Debit
        public TerminalAuthBuilder DebitSale(int referenceNumber, decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Debit).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }

        public TerminalAuthBuilder DebitRefund(int referenceNumber, decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.Debit).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }
        #endregion

        #region Gift & Loyalty
        public TerminalAuthBuilder GiftSale(int referenceNumber, decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Gift).WithReferenceNumber(referenceNumber).WithAmount(amount).WithCurrency(CurrencyType.CURRENCY);
        }

        public TerminalAuthBuilder GiftAddValue(int referenceNumber, decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.AddValue, PaymentMethodType.Gift)
                .WithReferenceNumber(referenceNumber)
                .WithCurrency(CurrencyType.CURRENCY)
                .WithAmount(amount);
        }

        public TerminalManageBuilder GiftVoid(int referenceNumber) {
            return new TerminalManageBuilder(TransactionType.Void, PaymentMethodType.Gift).WithReferenceNumber(referenceNumber).WithCurrency(CurrencyType.CURRENCY);
        }

        public TerminalAuthBuilder GiftBalance(int referenceNumber) {
            return new TerminalAuthBuilder(TransactionType.Balance, PaymentMethodType.Gift).WithReferenceNumber(referenceNumber).WithCurrency(CurrencyType.CURRENCY);
        }
        #endregion

        #region EBT Methods
        public TerminalAuthBuilder EbtBalance(int referenceNumber) {
            return new TerminalAuthBuilder(TransactionType.Balance, PaymentMethodType.EBT).WithReferenceNumber(referenceNumber);
        }

        public TerminalAuthBuilder EbtPurchase(int referenceNumber, decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.EBT).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }

        public TerminalAuthBuilder EbtRefund(int referenceNumber, decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.EBT).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }
        public TerminalAuthBuilder EbtWithdrawl(int referenceNumber, decimal? amount = null) {
            throw new UnsupportedTransactionException("This transaction is not currently supported for this payment type.");
        }
        #endregion

        public void Dispose() {
            CloseLane();
            _controller.Dispose();
        }
    }
}
