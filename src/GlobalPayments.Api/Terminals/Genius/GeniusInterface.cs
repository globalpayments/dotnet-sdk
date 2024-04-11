using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.UPA;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.Genius.Builders;
using GlobalPayments.Api.Terminals.Genius.Enums;
using GlobalPayments.Api.Terminals.Messaging;

namespace GlobalPayments.Api.Terminals.Genius {
    internal class GeniusInterface : IDeviceInterface {
        private GeniusController _controller;
        public event MessageReceivedEventHandler OnMessageReceived;
        string IDeviceInterface.EcrId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        internal GeniusInterface(GeniusController controller) {
            _controller = controller;
        }

        event MessageSentEventHandler IDeviceInterface.OnMessageSent {
            add {
                throw new NotImplementedException();
            }

            remove {
                throw new NotImplementedException();
            }
        }
        public TerminalAuthBuilder CreditSale(decimal amount) {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Credit).WithAmount(amount);
        }
        public TerminalAuthBuilder CreditRefund(decimal amount) {
            return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.Credit).WithAmount(amount);
        }
        public TerminalManageBuilder RefundById(decimal? amount) {
            return new MitcManageBuilder(TransactionType.Sale, PaymentMethodType.Credit, TransactionType.Refund).WithAmount(amount);
        }
        public TerminalReportBuilder GetTransactionDetails(TransactionType transactionType, string transactionId, TransactionIdType transactionIdType) {
            return new TerminalReportBuilder(transactionType, transactionId, transactionIdType);
        }
        public TerminalManageBuilder CreditVoid() {
            return new MitcManageBuilder(TransactionType.Sale, PaymentMethodType.Credit, TransactionType.Void);
        }
        public TerminalManageBuilder DebitVoid() {
            return new MitcManageBuilder(TransactionType.Sale, PaymentMethodType.Debit, TransactionType.Void);
        }
        public TerminalManageBuilder VoidRefund() {
            return new MitcManageBuilder(TransactionType.Refund, PaymentMethodType.Credit, TransactionType.Void);
        }
        public TerminalAuthBuilder DebitSale(decimal amount) {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Debit).WithAmount(amount);
        }
        void IDeviceInterface.Cancel() {
            throw new NotImplementedException();
        }
        IDeviceResponse IDeviceInterface.CloseLane() {
            throw new NotImplementedException();
        }
        IDeviceResponse IDeviceInterface.DisableHostResponseBeep() {
            throw new NotImplementedException();
        }
        ISignatureResponse IDeviceInterface.GetSignatureFile() {
            throw new NotImplementedException();
        }
        IInitializeResponse IDeviceInterface.Initialize() {
            throw new NotImplementedException();
        }
        IDeviceResponse IDeviceInterface.LineItem(string leftText, string rightText, string runningLeftText, string runningRightText) {
            throw new NotImplementedException();
        }
        IDeviceResponse IDeviceInterface.OpenLane() {
            throw new NotImplementedException();
        }
        ISignatureResponse IDeviceInterface.PromptForSignature(string transactionId) {
            throw new NotImplementedException();
        }
        IDeviceResponse IDeviceInterface.Reboot() {
            throw new NotImplementedException();
        }
        IDeviceResponse IDeviceInterface.Reset() {
            throw new NotImplementedException();
        }
        string IDeviceInterface.SendCustomMessage(DeviceMessage message) {
            throw new NotImplementedException();
        }
        IDeviceResponse IDeviceInterface.SendFile(SendFileType fileType, string filePath) {
            throw new NotImplementedException();
        }
        IDeviceResponse IDeviceInterface.DeleteResource(string fileName) {
            throw new NotImplementedException();
        }
        IDeviceResponse IDeviceInterface.UpdateResource(UpdateResourceFileType fileType, byte[] fileData, bool isHttpDeviceConnectionMode) {
            throw new NotImplementedException();
        }
        ISAFResponse IDeviceInterface.SendStoreAndForward() {
            throw new NotImplementedException();
        }
        IDeviceResponse IDeviceInterface.SetStoreAndForwardMode(bool enabled) {
            throw new NotImplementedException();
        }
        ISafDeleteFileResponse IDeviceInterface.DeleteStoreAndForwardFile(SafIndicator safIndicator) {
            throw new NotImplementedException();
        }
        IDeviceResponse IDeviceInterface.SetStoreAndForwardMode(SafMode safMode) {
            throw new NotImplementedException();
        }
        IDeviceResponse IDeviceInterface.SetStoreAndForwardMode(SafMode safMode, string startDateTime
            , string endDateTime, string durationInDays, string maxNumber, string totalCeilingAmount
            , string ceilingAmountPerCardType, string haloPerCardType, string safUploadMode
            , string autoUploadIntervalTimeInMilliseconds, string deleteSafConfirmation) {
            throw new NotImplementedException();
        }
        ISafParamsResponse IDeviceInterface.GetStoreAndForwardParams() {
            throw new NotImplementedException();
        }
        ISafUploadResponse IDeviceInterface.SafUpload(SafIndicator safUploadIndicator) { 
            throw new NotImplementedException(); 
        }
        ISafSummaryReport IDeviceInterface.GetSafSummaryReport(SafIndicator safIndicator) {
            throw new NotImplementedException();
        }
        IDeviceResponse IDeviceInterface.StartCard(PaymentMethodType paymentMethodType) {
            throw new NotImplementedException();
        }
        TerminalReportBuilder IDeviceInterface.LocalDetailReport() {
            throw new NotImplementedException();
        }
        TerminalReportBuilder IDeviceInterface.GetSAFReport() {
            throw new NotImplementedException();
        }
        TerminalReportBuilder IDeviceInterface.GetBatchReport() {
            throw new NotImplementedException();
        }
        TerminalReportBuilder IDeviceInterface.GetOpenTabDetails() {
            throw new NotImplementedException();
        }
        IBatchCloseResponse IDeviceInterface.BatchClose() {
            throw new NotImplementedException();
        }
        IEODResponse IDeviceInterface.EndOfDay() {
            throw new NotImplementedException();
        }
        TerminalAuthBuilder IDeviceInterface.AddValue(decimal? amount) {
            throw new NotImplementedException();
        }
        TerminalAuthBuilder IDeviceInterface.Authorize(decimal? amount) {
            throw new NotImplementedException();
        }
        TerminalAuthBuilder IDeviceInterface.Balance() {
            throw new NotImplementedException();
        }
        TerminalManageBuilder IDeviceInterface.Capture(decimal? amount) {
            throw new NotImplementedException();
        }
        TerminalAuthBuilder IDeviceInterface.Refund(decimal? amount) {
            throw new NotImplementedException();
        }
        TerminalAuthBuilder IDeviceInterface.Sale(decimal? amount) {
            throw new NotImplementedException();
        }
        TerminalAuthBuilder IDeviceInterface.Verify() {
            throw new NotImplementedException();
        }
        TerminalManageBuilder IDeviceInterface.Void() {
            throw new NotImplementedException();
        }
        TerminalAuthBuilder IDeviceInterface.Withdrawal(decimal? amount) {
            throw new NotImplementedException();
        }
        TerminalManageBuilder IDeviceInterface.TipAdjust(decimal? amount) {
            throw new NotImplementedException();
        }
        TerminalAuthBuilder IDeviceInterface.Tokenize() {
            throw new NotImplementedException();
        }
        TerminalAuthBuilder IDeviceInterface.AuthCompletion() {
            throw new NotImplementedException();
        }
        TerminalManageBuilder IDeviceInterface.DeletePreAuth() {
            throw new NotImplementedException();
        }
        void IDisposable.Dispose() {
            throw new NotImplementedException();
        }
        public IBatchClearResponse BatchClear() {
            throw new NotImplementedException();
        }
        ISAFResponse IDeviceInterface.DeleteSaf(string safreferenceNumer, string tranNo) {
            throw new NotImplementedException();
        }
        IDeviceResponse IDeviceInterface.RegisterPOS(string appName, int launchOrder, bool remove, int silent) {
            throw new NotImplementedException();
        }
        IDeviceResponse IDeviceInterface.StartCardTransaction(UpaParam param, ProcessingIndicator indicator, UpaTransactionData transData) {
            throw new NotImplementedException();
        }
        ISignatureResponse IDeviceInterface.PromptAndGetSignatureFile(string prompt1, string prompt2, int? displayOption) {
            throw new NotImplementedException();
        }
        public TerminalManageBuilder IncreasePreAuth(decimal amount) {
            throw new NotImplementedException();
        }
    }
}

