using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.UPA;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.Genius.Enums;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Terminals.PAX;
using GlobalPayments.Api.Terminals.UPA;
using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Terminals
{
    public abstract class DeviceInterface<T> : IDeviceInterface where T : DeviceController {
        const string ERROR_MESSAGE = "This method is not supported by the currently configured device.";
        protected T _controller;
        protected IRequestIdProvider _requestIdProvider;
        public event MessageSentEventHandler OnMessageSent;
        public event MessageReceivedEventHandler OnMessageReceived;
        public ValidationRequest Validations { get; set; }
        public string EcrId { get; set; }
        internal DeviceInterface(T controller) {
            _controller = controller;
            _controller.OnMessageSent += (message) => {
                OnMessageSent?.Invoke(message);
            };
            _controller.OnMessageReceived += (message) => {
                OnMessageReceived?.Invoke(message);
            };
            _requestIdProvider = _controller.RequestIdProvider;
            Validations = new ValidationRequest();
        }

        #region Admin Methods
        public virtual void Cancel() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse Cancel(int? displayOption = null) {
            throw new System.NotImplementedException();
        }
        public virtual IDeviceResponse CloseLane() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse DisableHostResponseBeep() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual ISignatureResponse GetSignatureFile() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual ISignatureResponse PromptAndGetSignatureFile(string prompt1, string prompt2, int? displayOption) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IInitializeResponse Initialize() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse LineItem(string leftText, string rightText = null, string runningLeftText = null, string runningRightText = null) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse OpenLane() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual ISignatureResponse PromptForSignature(string transactionId = null) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse Reboot() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse Reset() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual string SendCustomMessage(DeviceMessage message) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse SendFile(SendFileType fileType, string filePath) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse RemoveCard(string language = null) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        } 
        public virtual IDeviceResponse EnterPin(PromptMessages promptMessages, bool canBypass, string accountNumber) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse Prompt(PromptType promptType, PromptData promptData) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse GetGenericEntry(GenericData data) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse DisplayMessage(MessageLines messageLines) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        } 
        public virtual IDeviceResponse ReturnDefaultScreen(DisplayOption option) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse GetEncryptionType() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual ISAFResponse SendStoreAndForward() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual ISAFResponse DeleteSaf(string safRefNumber, string tranNo="")  {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }        
        public virtual IDeviceResponse RegisterPOS(POSData posData) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse SetStoreAndForwardMode(bool enabled) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse SetStoreAndForwardMode(SafMode safMode)
        {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual ISafDeleteFileResponse DeleteStoreAndForwardFile(SafIndicator safIndicator)
        {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual ISafUploadResponse SafUpload(SafIndicator safUploadIndicator)
        {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual ISafSummaryReport GetSafSummaryReport(SafIndicator safUploadIndicator)
        {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse SetStoreAndForwardMode(SafMode safMode, string startDateTime
            , string endDateTime,string durationInDays, string maxNumber, string totalCeilingAmount
            , string ceilingAmountPerCardType, string haloPerCardType, string safUploadMode
            , string autoUploadIntervalTimeInMilliseconds, string deleteSafConfirmation) 
        {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual ISafParamsResponse GetStoreAndForwardParams()
        {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse StartCard(PaymentMethodType paymentMethodType) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse SendSaf() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse StartCardTransaction(UpaParam param, ProcessingIndicator indicator, UpaTransactionData transData) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse ClearMessage()
        {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse InputAccount(bool? allowMagStripeEntry, bool? allowManualEntry, bool? allowContactlessEntry, bool? allowScannerEntry, bool? expiryDatePrompt, int timeout, int? encryptionFlag, int? keySlot, int? minAccountLength, int? maxAccountLength, string edcType, string transactionType)
        {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse ShowMessage(string message1, string title, string message2, bool topDown, string taxLine, string totalLine, string imageName, string imageDesc, PaxLineItemAction lineItemAction, int itemIndex)
        {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IShowTextBoxResponse ShowTextBox(string title, string text, string button1Name, string button1Color, string button2Name, string button2Color, string button3Name, string button3Color
            , string timeout, string button1Key, string button2Key, string button3Key, bool enableHardKeyOnly, string hardKeyList, SignatureBoxDisplay signatureBoxDisplay, bool continuousScreen = false
            , int? barcodeType = null, string barcodeData = null, string inputTextTitle = null, bool showInputText = false, TextInputType inputType = TextInputType.AlphaNumeric, int minLength = 0
            , int maxLength = 32)
        {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse SetTipPercentageOptions(int tipPercent1,int tipPercent2, int tipPercent3, bool noTipSelection)
        {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse SetTipAmountOptions(int tipPercent1, int tipPercent2, int tipPercent3, bool noTipSelection)
        {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        #endregion

        #region Batching
        public virtual IBatchCloseResponse BatchClose() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IBatchClearResponse BatchClear()
        {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IEODResponse EndOfDay() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        #endregion

        #region Reporting Methods
        public virtual TerminalReportBuilder LocalDetailReport() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual TerminalReportBuilder GetSAFReport() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual TerminalReportBuilder GetBatchReport() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual TerminalReportBuilder GetOpenTabDetails() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
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
        public virtual TerminalManageBuilder TipAdjust(decimal? amount) {
            return new TerminalManageBuilder(TransactionType.Edit, PaymentMethodType.Credit)
                .WithGratuity(amount);
        }
        public virtual TerminalAuthBuilder EodProcessing() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual TerminalAuthBuilder Tokenize() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual TerminalAuthBuilder AuthCompletion() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual TerminalManageBuilder DeletePreAuth() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual TerminalManageBuilder IncreasePreAuth(decimal amount) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }        
        #endregion

        #region IDisposable
        public void Dispose() {
            _controller.Dispose();
        }
        public virtual TerminalManageBuilder RefundById(decimal? amount) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual TerminalAuthBuilder CreditSale(decimal amount) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual TerminalAuthBuilder CreditRefund(decimal amount) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual TerminalManageBuilder CreditVoid() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual TerminalAuthBuilder DebitSale(decimal amount) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }      
        public virtual TerminalManageBuilder DebitVoid() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual TerminalManageBuilder VoidRefund() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }       
        public virtual TerminalReportBuilder GetTransactionDetails(TransactionType transactionType, string transactionId, TransactionIdType transactionIdType) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse GetAppInfo() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse ClearDataLake() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse SetTimeZone(string timezone) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse SetTipPercentageOptions() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse SetParam(KeyValuePair<string,string> parameter, string password = null, bool promptIfRestartRequired = false) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse GetParams(string[] parameters) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        /// <summary>
        /// <paramref name="debugLevel"/> is of type DebugLevel Enum
        /// <paramref name="logToConsole"/> is of type LogToConsole Enum
        /// </summary>
        /// <param name="debugLevel"></param>
        /// <param name="logToConsole"></param>
        /// <returns></returns>
        public virtual IDeviceResponse SetDebugLevel(Enum[] debugLevel, Enum logToConsole = null) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse GetDebugLevel() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        /// <summary>
        /// <paramref name="logFile"/> is of type LogFile Enum
        /// </summary>
        /// <param name="logFile"></param>
        /// <returns></returns>
        public virtual IDeviceResponse GetDebugInfo(Enum logFile = null) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse BroadcastConfiguration(bool enable) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse ReturnToIdle() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceScreen LoadUDData(UDData udData) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceScreen RemoveUDData(UDData udData) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }        
        public virtual IDeviceResponse Scan(ScanData scanData = null) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }       
        public virtual IDeviceResponse Print(PrintData printData) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual TerminalReportBuilder FindBatches() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse Ping() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }

        public virtual ITerminalReport GetBatchDetails(string batchId, bool printReport = false) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }

        public virtual TerminalManageBuilder Reverse() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }

        public virtual IDeviceResponse UpdateResource(UpdateResourceFileType fileType, byte[] fileData, bool isHttpDeviceConnectionMode) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }

        public virtual IDeviceResponse DeleteResource(string fileName) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual TerminalManageBuilder UpdateTaxInfo(decimal? amount = null) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual TerminalManageBuilder UpdateLodginDetail(decimal? amount = null) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse CommunicationCheck() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse Logon() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IBatchCloseResponse GetLastEOD() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse ExecuteUDDataFile(UDData udData) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse InjectUDDataFile(UDData udData)  {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual IDeviceResponse GetConfigContents(TerminalConfigType configType) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual TerminalAuthBuilder ContinueTransaction(decimal amount, bool isEmv = false) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        } 
        public virtual TerminalAuthBuilder CompleteTransaction() {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        public virtual TerminalAuthBuilder ProcessTransaction(decimal amount, TransactionType transactionType = TransactionType.Sale) {
            throw new UnsupportedTransactionException(ERROR_MESSAGE);
        }
        #endregion
    }
}
