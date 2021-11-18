using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.Messaging;

namespace GlobalPayments.Api.Terminals {
    public abstract class DeviceInterface<T> : IDeviceInterface where T : DeviceController {
        protected T _controller;
        protected IRequestIdProvider _requestIdProvider;

        public event MessageSentEventHandler OnMessageSent;
        public string EcrId { get; set; }
        internal DeviceInterface(T controller) {
            _controller = controller;
            _controller.OnMessageSent += (message) => {
                OnMessageSent?.Invoke(message);
            };
            _requestIdProvider = _controller.RequestIdProvider;
        }

        #region Admin Methods
        public virtual void Cancel() {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public virtual IDeviceResponse Cancel(int? displayOption = null) {
            throw new System.NotImplementedException();
        }
        public virtual IDeviceResponse CloseLane() {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public virtual IDeviceResponse DisableHostResponseBeep() {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public virtual ISignatureResponse GetSignatureFile() {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public virtual IInitializeResponse Initialize() {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public virtual IDeviceResponse LineItem(string leftText, string rightText = null, string runningLeftText = null, string runningRightText = null) {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public virtual IDeviceResponse OpenLane() {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public virtual ISignatureResponse PromptForSignature(string transactionId = null) {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public virtual IDeviceResponse Reboot() {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public virtual IDeviceResponse Reset() {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public virtual string SendCustomMessage(DeviceMessage message) {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public virtual IDeviceResponse SendFile(SendFileType fileType, string filePath) {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public virtual ISAFResponse SendStoreAndForward() {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public virtual IDeviceResponse SetStoreAndForwardMode(bool enabled) {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public virtual IDeviceResponse StartCard(PaymentMethodType paymentMethodType) {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public virtual IDeviceResponse SendSaf() {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Batching
        public virtual IBatchCloseResponse BatchClose() {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public virtual IEODResponse EndOfDay() {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }
        #endregion

        #region Reporting Methods
        public virtual TerminalReportBuilder LocalDetailReport() {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public virtual TerminalReportBuilder GetSAFReport() {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public virtual TerminalReportBuilder GetBatchReport() {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }
        #endregion

        #region Transactions
        public virtual TerminalAuthBuilder AddValue(decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.AddValue, PaymentMethodType.Gift)
                .WithAmount(amount);
        }
        public virtual TerminalAuthBuilder Authorize(decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Auth, PaymentMethodType.Credit)
                .WithAmount(amount);
        }
        public virtual TerminalAuthBuilder Balance() {
            return new TerminalAuthBuilder(TransactionType.Balance, PaymentMethodType.Gift);
        }
        public virtual TerminalManageBuilder Capture(decimal? amount = null) {
            return new TerminalManageBuilder(TransactionType.Capture, PaymentMethodType.Credit)
                .WithAmount(amount);
        }
        public virtual TerminalAuthBuilder Refund(decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.Credit)
                .WithAmount(amount);
        }
        public virtual TerminalAuthBuilder Sale(decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Credit)
                .WithAmount(amount);
        }
        public virtual TerminalAuthBuilder Verify() {
            return new TerminalAuthBuilder(TransactionType.Verify, PaymentMethodType.Credit);
        }
        public virtual TerminalManageBuilder Void() {
            return new TerminalManageBuilder(TransactionType.Void, PaymentMethodType.Credit);
        }
        public virtual TerminalAuthBuilder Withdrawal(decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.BenefitWithdrawal, PaymentMethodType.EBT)
                .WithAmount(amount);
        }

        public virtual TerminalAuthBuilder TipAdjust() {
            throw new System.NotImplementedException();
        }
        public virtual TerminalAuthBuilder EodProcessing() {
            throw new System.NotImplementedException();
        }
        #endregion

        #region IDisposable
        public void Dispose() {
            _controller.Dispose();
        }
        #endregion
    }
}
