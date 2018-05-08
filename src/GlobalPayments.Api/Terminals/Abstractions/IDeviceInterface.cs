using System;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Terminals.PAX;

namespace GlobalPayments.Api.Terminals {
    public interface IDeviceInterface : IDisposable {
        event MessageSentEventHandler OnMessageSent;

        #region Admin Calls
        void Cancel();
        IDeviceResponse CloseLane();
        IDeviceResponse DisableHostResponseBeep();
        IInitializeResponse Initialize();
        IDeviceResponse OpenLane();
        IDeviceResponse Reboot();
        IDeviceResponse Reset();
        ISignatureResponse GetSignatureFile();
        ISignatureResponse PromptForSignature(string transactionId = null);
        #endregion

        #region Batch Calls
        IBatchCloseResponse BatchClose();
        #endregion

        #region Credit Calls
        TerminalAuthBuilder CreditAuth(int referenceNumber, decimal? amount = null);
        TerminalManageBuilder CreditCapture(int referenceNumber, decimal? amount = null);
        TerminalAuthBuilder CreditRefund(int referenceNumber, decimal? amount = null);
        TerminalAuthBuilder CreditSale(int referenceNumber, decimal? amount = null);
        TerminalAuthBuilder CreditVerify(int referenceNumber);
        TerminalManageBuilder CreditVoid(int referenceNumber);
        #endregion

        #region Debit Calls
        TerminalAuthBuilder DebitSale(int referenceNumber, decimal? amount = null);
        TerminalAuthBuilder DebitRefund(int referenceNumber, decimal? amount = null);
        #endregion

        #region Gift Calls
        TerminalAuthBuilder GiftSale(int referenceNumber, decimal? amount = null);
        TerminalAuthBuilder GiftAddValue(int referenceNumber, decimal? amount = null);
        TerminalManageBuilder GiftVoid(int referenceNumber);
        TerminalAuthBuilder GiftBalance(int referenceNumber);
        #endregion

        #region EBT Calls
        TerminalAuthBuilder EbtBalance(int referenceNumber);
        TerminalAuthBuilder EbtPurchase(int referenceNumber, decimal? amount = null);
        TerminalAuthBuilder EbtRefund(int referenceNumber, decimal? amount = null);
        TerminalAuthBuilder EbtWithdrawl(int referenceNumber, decimal? amount = null);
        #endregion
    }
}
