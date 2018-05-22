using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.HeartSIP.Interfaces;
using GlobalPayments.Api.Terminals.HeartSIP.Responses;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Terminals.HeartSIP {
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

        public IDeviceResponse CloseLane() {
            return _controller.SendMessage<SipBaseResponse>("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>LaneClose</Request></SIP>", HSIP_MSG_ID.LANE_CLOSE);
        }

        public IDeviceResponse DisableHostResponseBeep() {
            throw new NotImplementedException();
        }

        public ISignatureResponse GetSignatureFile() {
            throw new UnsupportedTransactionException("Signature data for this device type is automatically returned in the terminal response.");
        }

        public IInitializeResponse Initialize() {
            return _controller.SendMessage<SipInitializeResponse>("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>GetAppInfoReport</Request></SIP>", HSIP_MSG_ID.GET_INFO_REPORT);
        }

        public IDeviceResponse OpenLane() {
            return _controller.SendMessage<SipBaseResponse>("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>LaneOpen</Request></SIP>", HSIP_MSG_ID.LANE_OPEN);
        }

        public ISignatureResponse PromptForSignature(string transactionId = null) {
            return _controller.SendMessage<SipSignatureResponse>("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>SignatureForm</Request><FormText>PLEASE SIGN YOUR NAME</FormText></SIP>", HSIP_MSG_ID.SIGNATURE_FORM);
        }

        public IDeviceResponse Reboot() {
            return _controller.SendMessage<SipBaseResponse>("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>Reboot</Request></SIP>", HSIP_MSG_ID.REBOOT);
        }

        public IDeviceResponse Reset() {
            return _controller.SendMessage<SipBaseResponse>("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>Reset</Request></SIP>", HSIP_MSG_ID.RESET);
        }

        public string SendCustomMessage(DeviceMessage message) {
            var response = _controller.Send(message);
            return Encoding.UTF8.GetString(response);
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
