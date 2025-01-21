using System;
using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.UPA;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.Genius.Enums;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Terminals.UPA;

namespace GlobalPayments.Api.Terminals {
    public interface IDeviceInterface : IDisposable {
        event MessageSentEventHandler OnMessageSent;
        event MessageReceivedEventHandler OnMessageReceived;
        ValidationRequest Validations { get; set; }
        string EcrId { get; set; }
        #region Admin Calls
        void Cancel();
        IDeviceResponse CloseLane();
        IDeviceResponse DisableHostResponseBeep();
        ISignatureResponse GetSignatureFile();
        IInitializeResponse Initialize();
        IDeviceResponse LineItem(string leftText, string rightText = null, string runningLeftText = null, string runningRightText = null);
        IDeviceResponse OpenLane();
        ISignatureResponse PromptForSignature(string transactionId = null);
        IDeviceResponse Reboot();
        IDeviceResponse Reset();
        IDeviceResponse GetAppInfo();
        IDeviceResponse ClearDataLake();
        IDeviceResponse ReturnToIdle();
        IDeviceScreen LoadUDData(UDData udData);
        IDeviceScreen RemoveUDData(UDData udData);        
        IDeviceResponse Scan(ScanData scanData = null);        
        IDeviceResponse Print(PrintData printData);
        IDeviceResponse SetTimeZone(string timezone);
        IDeviceResponse SetParam(KeyValuePair<string, string> parameter, string password = null, bool promptIfRestartRequired = false);
        IDeviceResponse GetParams(string[] parameters);
        /// <summary>
        /// <paramref name="debugLevel"/> is of type DebugLevel Enum
        /// <paramref name="logToConsole"/> is of type LogToConsole Enum
        /// </summary>
        /// <param name="debugLevel"></param>
        /// <param name="logToConsole"></param>
        /// <returns></returns>
        IDeviceResponse SetDebugLevel(Enum[] debugLevel, Enum logToConsole = null);
        IDeviceResponse GetDebugLevel();
        /// <summary>
        /// <paramref name="logFile"/> is of type LogFile Enum
        /// </summary>
        /// <param name="logFile"></param>
        /// <returns></returns>
        IDeviceResponse GetDebugInfo(Enum logFile = null);
        IDeviceResponse BroadcastConfiguration(bool enable);
        IDeviceResponse ExecuteUDDataFile(UDData udData);
        IDeviceResponse InjectUDDataFile(UDData udData);
        IDeviceResponse GetConfigContents(TerminalConfigType configType);
        TerminalManageBuilder UpdateTaxInfo(decimal? amount = null);
        TerminalManageBuilder UpdateLodginDetail(decimal? amount = null);
        IDeviceResponse CommunicationCheck();
        IDeviceResponse Logon();
        IBatchCloseResponse GetLastEOD();
        string SendCustomMessage(DeviceMessage message);
        IDeviceResponse SendFile(SendFileType fileType, string filePath);
        IDeviceResponse RemoveCard(string language = null);
        IDeviceResponse EnterPin(PromptMessages promptMessages, bool canBypass, string accountNumber);
        IDeviceResponse Prompt(PromptType promptType, PromptData promptData);
        IDeviceResponse GetGenericEntry(GenericData data);
        IDeviceResponse DisplayMessage(MessageLines messageLines);
        IDeviceResponse ReturnDefaultScreen(DisplayOption option);
        IDeviceResponse GetEncryptionType();
        ISAFResponse SendStoreAndForward();
        ISafDeleteFileResponse DeleteStoreAndForwardFile(SafIndicator safIndicator);
        ISAFResponse DeleteSaf(string safreferenceNumer, string tranNo=null);
        IDeviceResponse RegisterPOS(POSData posData);
        IDeviceResponse SetStoreAndForwardMode(bool enabled);
        IDeviceResponse SetStoreAndForwardMode(SafMode safMode);
        IDeviceResponse SetStoreAndForwardMode(SafMode safMode, string startDateTime = null
            , string endDateTime = null, string durationInDays = null, string maxNumber = null, string totalCeilingAmount = null
            , string ceilingAmountPerCardType = null, string haloPerCardType = null, string safUploadMode = null
            , string autoUploadIntervalTimeInMilliseconds = null, string deleteSafConfirmation = null);
        ISafParamsResponse GetStoreAndForwardParams();
        ISafSummaryReport GetSafSummaryReport(SafIndicator safIndicator);
        ISafUploadResponse SafUpload(SafIndicator safUploadIndicator); 
        IDeviceResponse StartCard(PaymentMethodType paymentMethodType);
        IDeviceResponse StartCardTransaction(UpaParam param, ProcessingIndicator indicator, UpaTransactionData transData);
        ISignatureResponse PromptAndGetSignatureFile(string prompt1, string prompt2, int? displayOption);
        IDeviceResponse Ping();
        IDeviceResponse UpdateResource(UpdateResourceFileType fileType, byte[] fileData, bool isHttpDeviceConnectionMode);
        IDeviceResponse DeleteResource(string fileName);
        TerminalAuthBuilder ContinueTransaction(decimal amount, bool isEmv = false);
        TerminalAuthBuilder CompleteTransaction();
        TerminalAuthBuilder ProcessTransaction(decimal amount, TransactionType transactionType = TransactionType.Sale);
        #endregion

        #region reporting
        TerminalReportBuilder LocalDetailReport();
        TerminalReportBuilder GetSAFReport();
        TerminalReportBuilder GetBatchReport();
        TerminalReportBuilder GetOpenTabDetails();
        ITerminalReport GetBatchDetails(string batchId, bool printReport = false);
        TerminalReportBuilder FindBatches();
        #endregion

        #region Batch Calls
        IBatchCloseResponse BatchClose();
        IBatchClearResponse BatchClear();
        IEODResponse EndOfDay();
        #endregion

        #region Credit Calls
        //TerminalAuthBuilder CreditAuth(decimal? amount = null);
        //TerminalManageBuilder CreditCapture(decimal? amount = null);
        TerminalAuthBuilder CreditRefund(decimal amount);
        TerminalAuthBuilder CreditSale(decimal amount);
        //TerminalAuthBuilder CreditVerify();
        TerminalManageBuilder CreditVoid();
        #endregion

        #region Debit Calls
        TerminalAuthBuilder DebitSale(decimal amount);
        //TerminalAuthBuilder DebitRefund(decimal? amount = null);
        #endregion

        #region Voids
        TerminalManageBuilder DebitVoid();
        TerminalManageBuilder VoidRefund();
        #endregion

        #region Gift Calls
        //TerminalAuthBuilder GiftSale(decimal? amount = null);
        //TerminalAuthBuilder GiftAddValue(decimal? amount = null);
        //TerminalManageBuilder GiftVoid();
        //TerminalAuthBuilder GiftBalance();
        #endregion

        #region EBT Calls
        //TerminalAuthBuilder EbtBalance();
        //TerminalAuthBuilder EbtPurchase(decimal? amount = null);
        //TerminalAuthBuilder EbtRefund(decimal? amount = null);
        //TerminalAuthBuilder EbtWithdrawl(decimal? amount = null);
        #endregion

        #region Generic Calls
        TerminalAuthBuilder AddValue(decimal? amount = null);
        TerminalAuthBuilder Authorize(decimal? amount = null);
        TerminalAuthBuilder Balance();
        TerminalManageBuilder Capture(decimal? amount = null);
        TerminalAuthBuilder Refund(decimal? amount = null);
        TerminalManageBuilder RefundById(decimal? amount = null);
        TerminalAuthBuilder Sale(decimal? amount = null);
        TerminalAuthBuilder Verify();
        TerminalManageBuilder Void();
        TerminalAuthBuilder Withdrawal(decimal? amount = null);
        TerminalManageBuilder TipAdjust(decimal? amount = null);
        TerminalAuthBuilder Tokenize();
        TerminalAuthBuilder AuthCompletion();
        TerminalManageBuilder DeletePreAuth();
        TerminalManageBuilder IncreasePreAuth(decimal amount);
        TerminalManageBuilder Reverse();
        #endregion

        #region Report Calls        
        TerminalReportBuilder GetTransactionDetails(TransactionType transactionType, string transactionId, TransactionIdType transactionIdType);
        #endregion
    }
}
