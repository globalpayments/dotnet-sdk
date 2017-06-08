using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.PAX;

namespace GlobalPayments.Api.Terminals {
    public interface IDeviceInterface {
        event MessageSentEventHandler OnMessageSent;

        #region Admin Calls
        void Cancel();
        PaxDeviceResponse DisableHostResponseBeep();
        InitializeResponse Initialize();
        PaxDeviceResponse Reboot();
        PaxDeviceResponse Reset();
        #endregion

        #region Batch Calls
        BatchCloseResponse BatchClose();
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
        TerminalAuthBuilder GiftAddValue(int referenceNumber);
        TerminalManageBuilder GiftVoid(int referenceNumber);
        TerminalAuthBuilder GiftBalance(int referenceNumber);
        #endregion
    }
}
