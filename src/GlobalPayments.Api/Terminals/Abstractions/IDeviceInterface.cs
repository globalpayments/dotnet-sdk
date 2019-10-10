using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.Messaging;

namespace GlobalPayments.Api.Terminals {
    public interface IDeviceInterface : IDisposable {
        event MessageSentEventHandler OnMessageSent;

        #region Admin Calls
        void Cancel();
        IDeviceResponse CloseLane();
        IDeviceResponse DisableHostResponseBeep();
        ISignatureResponse GetSignatureFile();
        IInitializeResponse Initialize();
        IDeviceResponse OpenLane();
        ISignatureResponse PromptForSignature(string transactionId = null);
        IDeviceResponse Reboot();
        IDeviceResponse Reset();
        string SendCustomMessage(DeviceMessage message);
        IDeviceResponse StartCard(PaymentMethodType paymentMethodType);
        ISAFResponse SendStoreAndForward();
        IDeviceResponse LineItem(string leftText, string rightText = null, string runningLeftText = null, string runningRightText = null);
        IDeviceResponse SetStoreAndForwardMode(bool enabled);
        IDeviceResponse SendFile(SendFileType fileType, string filePath);
        IEODResponse EndOfDay();
        #endregion

        #region reporting
        TerminalReportBuilder LocalDetailReport();
        #endregion

        #region Batch Calls
        IBatchCloseResponse BatchClose();
        #endregion

        #region Credit Calls
        TerminalAuthBuilder CreditAuth(decimal? amount = null);
        TerminalManageBuilder CreditCapture(decimal? amount = null);
        TerminalAuthBuilder CreditRefund(decimal? amount = null);
        TerminalAuthBuilder CreditSale(decimal? amount = null);
        TerminalAuthBuilder CreditVerify();
        TerminalManageBuilder CreditVoid();
        #endregion

        #region Debit Calls
        TerminalAuthBuilder DebitSale(decimal? amount = null);
        TerminalAuthBuilder DebitRefund(decimal? amount = null);
        #endregion

        #region Gift Calls
        TerminalAuthBuilder GiftSale(decimal? amount = null);
        TerminalAuthBuilder GiftAddValue(decimal? amount = null);
        TerminalManageBuilder GiftVoid();
        TerminalAuthBuilder GiftBalance();
        #endregion

        #region EBT Calls
        TerminalAuthBuilder EbtBalance();
        TerminalAuthBuilder EbtPurchase(decimal? amount = null);
        TerminalAuthBuilder EbtRefund(decimal? amount = null);
        TerminalAuthBuilder EbtWithdrawl(decimal? amount = null);
        #endregion
    }
}
