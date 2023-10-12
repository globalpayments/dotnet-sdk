using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.HPA.Responses;
using GlobalPayments.Api.Terminals.Messaging;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace GlobalPayments.Api.Terminals.HPA {
    public class HpaInterface : DeviceInterface<HpaController> {
        internal HpaInterface(HpaController controller) : base(controller) {
        }

        #region Admin Messages
        public override void Cancel() {
            // TODO: Cancel for HPA?
            Reset();
        }

        public override IDeviceResponse CloseLane() {
            return _controller.SendAdminMessage<SipBaseResponse>(new HpaAdminBuilder(HPA_MSG_ID.LANE_CLOSE));
        }

        public override IInitializeResponse Initialize() {
            return _controller.SendAdminMessage<InitializeResponse>(new HpaAdminBuilder(HPA_MSG_ID.GET_INFO_REPORT));
        }

        public override IDeviceResponse OpenLane() {
            return _controller.SendAdminMessage<SipBaseResponse>(new HpaAdminBuilder(HPA_MSG_ID.LANE_OPEN));
        }

        public override ISignatureResponse PromptForSignature(string transactionId = null) {
            return _controller.SendAdminMessage<SignatureResponse>(new HpaAdminBuilder(HPA_MSG_ID.SIGNATURE_FORM).Set("FormText", "PLEASE SIGN YOUR NAME"));
        }

        public override IDeviceResponse Reboot() {
            return _controller.SendAdminMessage<SipBaseResponse>(new HpaAdminBuilder(HPA_MSG_ID.REBOOT));
        }

        public override IDeviceResponse Reset() {
            return _controller.SendAdminMessage<SipBaseResponse>(new HpaAdminBuilder(HPA_MSG_ID.RESET));
        }

        public override string SendCustomMessage(DeviceMessage message) {
            var response = _controller.Send(message);
            return Encoding.UTF8.GetString(response);
        }

        public override IDeviceResponse StartCard(PaymentMethodType paymentMethodType) {
            return _controller.SendAdminMessage<SipBaseResponse>(new HpaAdminBuilder(HPA_MSG_ID.STARTCARD).Set("CardGroup", paymentMethodType.ToString()));
        }

        public override ISAFResponse SendStoreAndForward() {
            return _controller.SendAdminMessage<SAFResponse>(new HpaAdminBuilder(HPA_MSG_ID.SENDSAF));
        }
        
        public override IDeviceResponse LineItem(string leftText, string rightText, string runningLeftText, string runningRightText) {
            if (string.IsNullOrEmpty(leftText)) {
                throw new ApiException("Left Text is Required field");
            }

            var message = new HpaAdminBuilder(HPA_MSG_ID.LINEITEM)
                .Set("LineItemTextLeft", leftText)
                .Set("LineItemTextRight", rightText)
                .Set("LineItemRunningTextLeft", runningLeftText)
                .Set("LineItemRunningTextRight", runningRightText);
            return _controller.SendAdminMessage<SipBaseResponse>(message);
        }

        public override IDeviceResponse SetStoreAndForwardMode(bool enabled) {
            return _controller.SendAdminMessage<SipBaseResponse>(new HpaAdminBuilder(HPA_MSG_ID.SETPARAMETERREPORT).Set("FieldCount", "1").Set("Key", "STORMD").Set("Value", enabled ? "1" : "0"));
        }

        public override IDeviceResponse SendFile(SendFileType imageType, string filePath) {
            if (string.IsNullOrEmpty(filePath)) {
                throw new ApiException("Filename is required for SendFile.");
            }

            // load the file
            var fileUpload = new HpaFileUpload(imageType, filePath);

            // build the initial message
            var builder = new HpaAdminBuilder(HPA_MSG_ID.SENDFILE) { KeepAlive = true }
                .Set("FileName", fileUpload.FileName)
                .Set("FileSize", fileUpload.FileSize)
                .Set("MultipleMessage", "1");

            var response = _controller.SendAdminMessage<SipSendFileResponse>(builder);
            if (response.DeviceResponseCode.Equals("00")) {
                IEnumerable<string> fileParts = fileUpload.GetFileParts(response.MaxDataSize / 5);
                foreach (var filePart in fileParts) {
                    string multipleMessage = (filePart.Equals(fileParts.Last())) ? "0" : "1";

                    var dataResponse = _controller.SendAdminMessage<SipSendFileResponse>(
                        new HpaAdminBuilder(HPA_MSG_ID.SENDFILE) {
                            KeepAlive = (multipleMessage.Equals("1")),
                            AwaitResponse = (multipleMessage.Equals("0"))
                        }
                        .Set("FileData", filePart)
                        .Set("MultipleMessage", multipleMessage)
                    );

                    if (dataResponse != null) {
                        response = dataResponse;
                    }
                }

                return response;
            }
            else throw new ApiException(string.Format("Failed to upload file: {0}", response.DeviceResponseText));
        }
        #endregion

        #region Reporting
        public override TerminalReportBuilder LocalDetailReport() {
            throw new NotImplementedException();
        }

        public override TerminalReportBuilder GetSAFReport()
        {
            return new TerminalReportBuilder(TerminalReportType.GetSAFReport);
        }

        #endregion

        #region Batching
        public override IBatchCloseResponse BatchClose() {
            return _controller.SendAdminMessage<BatchResponse>(new HpaAdminBuilder(HPA_MSG_ID.BATCH_CLOSE, HPA_MSG_ID.GET_BATCH_REPORT));
        }

        public override IEODResponse EndOfDay() {
            return _controller.SendAdminMessage<EODResponse>(new HpaAdminBuilder(
                HPA_MSG_ID.ENDOFDAY,
                HPA_MSG_ID.REVERSAL,
                HPA_MSG_ID.EMVOFFLINEDECLINE,
                HPA_MSG_ID.EMVCRYPTOGRAMTYPE,
                HPA_MSG_ID.ATTACHMENT,
                HPA_MSG_ID.SENDSAF,
                HPA_MSG_ID.GET_BATCH_REPORT,
                HPA_MSG_ID.HEARTBEAT,
                HPA_MSG_ID.BATCH_CLOSE,
                HPA_MSG_ID.EMV_PARAMETER_DOWNLOAD,
                HPA_MSG_ID.TRANSACTIONCERTIFICATE));
        }
        #endregion

        #region Transactions
        public override TerminalAuthBuilder Withdrawal(decimal? amount = null) {
            throw new UnsupportedTransactionException("This transaction is not currently supported for this payment type.");
        }
        #endregion

        #region Credit
        //public TerminalAuthBuilder CreditAuth(decimal? amount = default(decimal?)) {
        //    return new TerminalAuthBuilder(TransactionType.Auth, PaymentMethodType.Credit).WithAmount(amount);
        //}

        //public TerminalManageBuilder CreditCapture(decimal? amount = default(decimal?)) {
        //    return new TerminalManageBuilder(TransactionType.Capture, PaymentMethodType.Credit).WithAmount(amount);
        //}

        //public TerminalAuthBuilder CreditRefund(decimal? amount = default(decimal?)) {
        //    return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.Credit).WithAmount(amount);
        //}

        //public TerminalAuthBuilder CreditSale(decimal? amount = default(decimal?)) {
        //    return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Credit).WithAmount(amount);
        //}

        //public TerminalAuthBuilder CreditVerify() {
        //    return new TerminalAuthBuilder(TransactionType.Verify, PaymentMethodType.Credit);
        //}

        //public TerminalManageBuilder CreditVoid() {
        //    return new TerminalManageBuilder(TransactionType.Void, PaymentMethodType.Credit);
        //}
        #endregion

        #region Debit
        //public TerminalAuthBuilder DebitSale(decimal? amount = null) {
        //    return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Debit).WithAmount(amount);
        //}

        //public TerminalAuthBuilder DebitRefund(decimal? amount = null) {
        //    return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.Debit).WithAmount(amount);
        //}
        #endregion

        #region Gift & Loyalty
        //public TerminalAuthBuilder GiftSale(decimal? amount = null) {
        //    return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Gift).WithAmount(amount).WithCurrency(CurrencyType.CURRENCY);
        //}

        //public TerminalAuthBuilder GiftAddValue(decimal? amount = null) {
        //    return new TerminalAuthBuilder(TransactionType.AddValue, PaymentMethodType.Gift)
        //        .WithCurrency(CurrencyType.CURRENCY)
        //        .WithAmount(amount);
        //}

        //public TerminalManageBuilder GiftVoid() {
        //    return new TerminalManageBuilder(TransactionType.Void, PaymentMethodType.Gift).WithCurrency(CurrencyType.CURRENCY);
        //}

        //public TerminalAuthBuilder GiftBalance() {
        //    return new TerminalAuthBuilder(TransactionType.Balance, PaymentMethodType.Gift).WithCurrency(CurrencyType.CURRENCY);
        //}
        #endregion

        #region EBT Methods
        //public TerminalAuthBuilder EbtBalance() {
        //    return new TerminalAuthBuilder(TransactionType.Balance, PaymentMethodType.EBT);
        //}

        //public TerminalAuthBuilder EbtPurchase(decimal? amount = null) {
        //    return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.EBT).WithAmount(amount);
        //}

        //public TerminalAuthBuilder EbtRefund(decimal? amount = null) {
        //    return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.EBT).WithAmount(amount);
        //}

        //public TerminalAuthBuilder EbtWithdrawl(decimal? amount = null) {
        //    throw new UnsupportedTransactionException("This transaction is not currently supported for this payment type.");
        //}
        #endregion
    }
}

