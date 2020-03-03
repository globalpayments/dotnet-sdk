using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.INGENICO;
using GlobalPayments.Api.Terminals.Messaging;

namespace GlobalPayments.Api.Terminals {
    public interface IDeviceInterface : IDisposable {
        event MessageSentEventHandler OnMessageSent;
        event BroadcastMessageEventHandler OnBroadcastMessage;

        #region Admin Calls

        /// <summary>
        /// A method for Cancelling a live transaction.
        /// </summary>
        /// <param name="amount">Amount to be passed for cancel request.</param>
        /// <returns>TerminalManageBuilder</returns>
        TerminalManageBuilder Cancel(decimal? amount = null);
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
        /// <summary>
        /// Instruct the terminal to print the receipt of the last completed transaction.
        /// </summary>
        /// <param name="type">Receipt Type</param>
        /// <returns></returns>
        TerminalReportBuilder GetLastReceipt(ReceiptType type = ReceiptType.TICKET);

        /// <summary>
        /// Instruct the terminal to get the report in XML format of all the transactions.
        /// </summary>
        /// <param name="type">Report Type</param>
        /// <returns></returns>
        TerminalAuthBuilder GetReport(INGENICO.ReportType type);
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

        /// <summary>
        /// Intructs the terminal to transact hotel mode pre-auth.
        /// </summary>
        /// <param name="amount">Pre-auth Amount</param>
        /// <returns></returns>
        TerminalAuthBuilder Authorize(decimal? amount = null);
        TerminalAuthBuilder Balance();
        TerminalManageBuilder Capture(decimal? amount = null);

        /// <summary>
        /// Intructs the terminal to complete the completed hotel mode pre-auth transaction.
        /// </summary>
        /// <param name="amount">Complete Amount/param>
        /// <returns></returns>
        TerminalAuthBuilder Completion(decimal? amount = null);

        /// <summary>
        /// Instruct the temrinal to refund the last completed transaction.
        /// </summary>
        /// <param name="amount">Refund Amount</param>
        /// <returns></returns>
        TerminalAuthBuilder Refund(decimal? amount = null);

        /// <summary>
        /// Instruct the terminal to process sale transaction.
        /// </summary>
        /// <param name="amount">Sale Amount</param>
        /// <returns></returns>
        TerminalAuthBuilder Sale(decimal? amount = null);

        /// <summary>
        /// Verify the account of the card holder.
        /// </summary>
        /// <returns></returns>
        TerminalAuthBuilder Verify();
        TerminalManageBuilder Void();
        TerminalAuthBuilder Withdrawal(decimal? amount = null);
        #endregion

        #region Terminal Management

        /// <summary>
        /// The terminal immediately performs a reversal of the last completed transaction if no Transaction Id is set.
        /// </summary>
        /// <param name="amount">Amount to be passed for cancel request.</param>
        /// <returns>TerminalManageBuilder</returns>
        TerminalManageBuilder Reverse(decimal? amount = null);

        /// <summary>
        /// The terminal immediately initiates a duplicate of the last completed transaction
        /// </summary>
        /// <param name="amount">Amount to be passed for cancel request.</param>
        /// <returns>TerminalManageBuilder</returns>
        TerminalManageBuilder Duplicate(decimal? amount = null);
        #endregion
    }
}
