using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.Messaging;

namespace GlobalPayments.Api.Terminals {
    public interface IDeviceInterface : IDisposable {
        event MessageSentEventHandler OnMessageSent;
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
        string SendCustomMessage(DeviceMessage message);
        IDeviceResponse SendFile(SendFileType fileType, string filePath);
        ISAFResponse SendStoreAndForward();
        IDeviceResponse SetStoreAndForwardMode(bool enabled);
        IDeviceResponse StartCard(PaymentMethodType paymentMethodType);
        #endregion

        #region reporting
        TerminalReportBuilder LocalDetailReport();
        TerminalReportBuilder GetSAFReport();
        TerminalReportBuilder GetBatchReport();
        #endregion

        #region Batch Calls
        IBatchCloseResponse BatchClose();
        IEODResponse EndOfDay();
        #endregion

        #region Credit Calls
        //TerminalAuthBuilder CreditAuth(decimal? amount = null);
        //TerminalManageBuilder CreditCapture(decimal? amount = null);
        //TerminalAuthBuilder CreditRefund(decimal? amount = null);
        //TerminalAuthBuilder CreditSale(decimal? amount = null);
        //TerminalAuthBuilder CreditVerify();
        //TerminalManageBuilder CreditVoid();
        #endregion

        #region Debit Calls
        //TerminalAuthBuilder DebitSale(decimal? amount = null);
        //TerminalAuthBuilder DebitRefund(decimal? amount = null);
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
        TerminalAuthBuilder Sale(decimal? amount = null);
        TerminalAuthBuilder Verify();
        TerminalManageBuilder Void();
        TerminalAuthBuilder Withdrawal(decimal? amount = null);
        TerminalAuthBuilder TipAdjust();
        #endregion
    }
}
