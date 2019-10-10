using System;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Messaging;
using System.Text;

namespace GlobalPayments.Api.Terminals.PAX {
    public class PaxInterface : IDeviceInterface {
        private PaxController controller;
        private IRequestIdProvider requestIdProvider;
        public event MessageSentEventHandler OnMessageSent;
        
        internal PaxInterface(PaxController controller) {
            this.controller = controller;
            controller.OnMessageSent += (message) => {
                OnMessageSent?.Invoke(message);
            };
            requestIdProvider = controller.RequestIdProvider;
        }

        #region Administration Messages
        // A00 - INITIALIZE
        public IInitializeResponse Initialize() {
            var response = controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A00_INITIALIZE));
            return new InitializeResponse(response);
        }

        // A08 - GET SIGNATURE
        public ISignatureResponse GetSignatureFile() {
            var response = controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A08_GET_SIGNATURE,
                0, ControlCodes.FS
            ));
            return new SignatureResponse(response, controller.DeviceType.Value);
        }

        // A14 - CANCEL
        public void Cancel() {
            if (controller.ConnectionMode == ConnectionModes.HTTP) {
                throw new MessageException("The cancel command is not available in HTTP mode");
            }

            try {
                controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A14_CANCEL));
            }
            catch (MessageException exc) {
                if (!exc.Message.Equals("Terminal returned EOT for the current message.")) {
                    throw;
                }
            }
        }

        // A16 - RESET
        public IDeviceResponse Reset() {
            var response = controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A16_RESET));
            return new PaxTerminalResponse(response, PAX_MSG_ID.A17_RSP_RESET);
        }

        // A20 - DO SIGNATURE
        public ISignatureResponse PromptForSignature(string transactionId = null) {
            var response = controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A20_DO_SIGNATURE,
                (transactionId != null) ? 1 : 0,
                ControlCodes.FS,
                transactionId ?? string.Empty,
                ControlCodes.FS,
                (transactionId != null) ? "00" : "",
                ControlCodes.FS,
                300
            ));
            var signatureResponse = new SignatureResponse(response);
            if (signatureResponse.DeviceResponseCode == "000000")
                return GetSignatureFile();
            return signatureResponse;
        }

        // A26 - REBOOT
        public IDeviceResponse Reboot() {
            var response = controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A26_REBOOT));
            return new PaxTerminalResponse(response, PAX_MSG_ID.A27_RSP_REBOOT);
        }

        // A04 - SET VARIABLE
        public IDeviceResponse DisableHostResponseBeep() {
            var response = controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A04_SET_VARIABLE,
                "01",
                ControlCodes.FS,
                "hostRspBeep",
                ControlCodes.FS,
                "N",
                ControlCodes.FS,
                ControlCodes.FS,
                ControlCodes.FS,
                ControlCodes.FS,
                ControlCodes.FS,
                ControlCodes.FS,
                ControlCodes.FS,
                ControlCodes.FS,
                ControlCodes.FS
            ));
            return new PaxTerminalResponse(response, PAX_MSG_ID.A05_RSP_SET_VARIABLE);
        }

        public IDeviceResponse CloseLane() {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public IDeviceResponse OpenLane() {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public IDeviceResponse StartCard(PaymentMethodType paymentMethodType) {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public ISAFResponse SendStoreAndForward() {
            throw new UnsupportedTransactionException();
        }

        public IDeviceResponse LineItem(string leftText, string rightText = null, string runningLeftText = null, string runningRightText = null) {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public IDeviceResponse SetStoreAndForwardMode(bool enabled) {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public IDeviceResponse SendFile(SendFileType fileType, string filePath) {
            throw new UnsupportedTransactionException("This function is not supported by the currently configured device.");
        }

        public IEODResponse EndOfDay() {
            throw new UnsupportedTransactionException("PAX does not support the EOD option");
        }

        public string SendCustomMessage(DeviceMessage message) {
            var response = controller.Send(message);
            return Encoding.UTF8.GetString(response);
        }
        #endregion

        #region Reporting Messages
        public TerminalReportBuilder LocalDetailReport() {
            return new TerminalReportBuilder(TerminalReportType.LocalDetailReport);
        }
        #endregion

        #region Credit Methods
        public TerminalAuthBuilder CreditAuth(decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Auth, PaymentMethodType.Credit).WithAmount(amount);
        }
        
        public TerminalManageBuilder CreditCapture(decimal? amount = null) {
            return new TerminalManageBuilder(TransactionType.Capture, PaymentMethodType.Credit).WithAmount(amount);
        }

        //public TerminalManageBuilder CreditEdit(decimal? amount = null) {
        //    return new TerminalManageBuilder(TransactionType.Edit).WithAmount(amount);
        //}

        public TerminalAuthBuilder CreditRefund(decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.Credit).WithAmount(amount);
        }

        public TerminalAuthBuilder CreditSale(decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Credit).WithAmount(amount);
        }

        public TerminalAuthBuilder CreditVerify() {
            return new TerminalAuthBuilder(TransactionType.Verify, PaymentMethodType.Credit);
        }

        public TerminalManageBuilder CreditVoid() {
            return new TerminalManageBuilder(TransactionType.Void, PaymentMethodType.Credit);
        }
        #endregion

        #region Debit Methods
        public TerminalAuthBuilder DebitRefund(decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.Debit).WithAmount(amount);
        }

        public TerminalAuthBuilder DebitSale(decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Debit).WithAmount(amount);
        }
        #endregion

        #region EBT Methods
        public TerminalAuthBuilder EbtBalance() {
            return new TerminalAuthBuilder(TransactionType.Balance, PaymentMethodType.EBT);
        }

        public TerminalAuthBuilder EbtPurchase(decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.EBT).WithAmount(amount);
        }

        public TerminalAuthBuilder EbtRefund(decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.EBT).WithAmount(amount);
        }

        public TerminalAuthBuilder EbtWithdrawl(decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.BenefitWithdrawal, PaymentMethodType.EBT).WithAmount(amount);
        }
        #endregion

        #region Gift Methods
        public TerminalAuthBuilder GiftSale(decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Gift).WithAmount(amount).WithCurrency(CurrencyType.CURRENCY);
        }

        public TerminalAuthBuilder GiftAddValue(decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.AddValue, PaymentMethodType.Gift)
                
                .WithCurrency(CurrencyType.CURRENCY)
                .WithAmount(amount);
        }

        public TerminalManageBuilder GiftVoid() {
            return new TerminalManageBuilder(TransactionType.Void, PaymentMethodType.Gift).WithCurrency(CurrencyType.CURRENCY);
        }

        public TerminalAuthBuilder GiftBalance() {
            return new TerminalAuthBuilder(TransactionType.Balance, PaymentMethodType.Gift).WithCurrency(CurrencyType.CURRENCY);
        }
        #endregion

        #region Cash Methods
        #endregion

        #region Check Methods
        #endregion

        #region Batch Commands
        public IBatchCloseResponse BatchClose() {
            var response = controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.B00_BATCH_CLOSE, DateTime.Now.ToString("YYYYMMDDhhmmss")));
            return new BatchCloseResponse(response);
        }
        #endregion
        public void Dispose() {
            controller.Dispose();
        }
    }
}