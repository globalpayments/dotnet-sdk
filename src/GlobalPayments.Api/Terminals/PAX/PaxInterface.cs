using System;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using System.Text;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Terminals.PAX.Responses;

namespace GlobalPayments.Api.Terminals.PAX {
    public class PaxInterface : DeviceInterface<PaxController>, IDeviceInterface {
        internal PaxInterface(PaxController controller) : base(controller) {
        }
        #region Administration Messages
        public override IInitializeResponse Initialize() {
            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A00_INITIALIZE));
            return new InitializeResponse(response);
        }
        public override ISignatureResponse GetSignatureFile() {
            var response = _controller.Send(TerminalUtilities.BuildRequest(
                PAX_MSG_ID.A08_GET_SIGNATURE,
                0,
                ControlCodes.FS
            ));
            return new SignatureResponse(response, _controller.DeviceType.Value);
        }
        public override void Cancel() {
            if (_controller.ConnectionMode == ConnectionModes.HTTP) {
                throw new MessageException("The cancel command is not available in HTTP mode");
            }

            try {
                _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A14_CANCEL));
            }
            catch (MessageException exc) {
                if (!exc.Message.Equals("Terminal returned EOT for the current message.")) {
                    throw;
                }
            }
        }
        public override IDeviceResponse Reset() {
            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A16_RESET));
            return new PaxTerminalResponse(response, PAX_MSG_ID.A17_RSP_RESET);
        }
        public override ISafDeleteFileResponse DeleteStoreAndForwardFile(SafIndicator safIndicator)
        {
            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.B10_DELETE_SAF_FILE, ((int)safIndicator).ToString()));
            return new SafDeleteFileResponse(response);
        }
        public override IDeviceResponse SetStoreAndForwardMode(SafMode safMode) {
            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A54_SET_SAF_PARAMETERS
                , ((int)safMode).ToString()
                , ControlCodes.FS
                , ControlCodes.FS
                , ControlCodes.FS
                , ControlCodes.FS
                , ControlCodes.FS
                , ControlCodes.FS
                , ControlCodes.FS
                , ControlCodes.FS
                , ControlCodes.FS
                , ControlCodes.FS
            ));
            return new PaxTerminalResponse(response, PAX_MSG_ID.A55_RSP_SET_SAF_PARAMETERS);
        }
        public override IDeviceResponse SetStoreAndForwardMode(SafMode safMode, string startDateTime = null
            , string endDateTime = null, string durationInDays = null, string maxNumber = null, string totalCeilingAmount = null
            , string ceilingAmountPerCardType = null, string haloPerCardType = null, string safUploadMode = null
            , string autoUploadIntervalTimeInMilliseconds = null, string deleteSafConfirmation = null)
        {
            
            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A54_SET_SAF_PARAMETERS
                ,((int)safMode).ToString()
                ,ControlCodes.FS
                ,startDateTime
                ,ControlCodes.FS
                ,endDateTime
                ,ControlCodes.FS
                ,durationInDays
                ,ControlCodes.FS
                ,maxNumber
                ,ControlCodes.FS
                ,totalCeilingAmount
                ,ControlCodes.FS
                ,ceilingAmountPerCardType
                ,ControlCodes.FS
                ,haloPerCardType
                ,ControlCodes.FS
                ,safUploadMode
                ,ControlCodes.FS
                ,autoUploadIntervalTimeInMilliseconds
                ,ControlCodes.FS
                ,deleteSafConfirmation));
            return new PaxTerminalResponse(response, PAX_MSG_ID.A55_RSP_SET_SAF_PARAMETERS);
        }
        public override ISafUploadResponse SafUpload(SafIndicator safUploadIndicator)
        {
            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.B08_SAF_UPLOAD, ((int)safUploadIndicator).ToString()));
            return new SafUploadResponse(response);
        }
        public override ISafParamsResponse GetStoreAndForwardParams()
        {
            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A78_GET_SAF_PARAMETERS));
            return new SafParamsResponse(response);
        }
        public override ISafSummaryReport GetSafSummaryReport(SafIndicator safReportIndicator)
        {
            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.R10_SAF_SUMMARY_REPORT, ((int)safReportIndicator).ToString()));
            return new SafSummaryReport(response);
        }
        public override ISignatureResponse PromptForSignature(string transactionId = null) {
            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A20_DO_SIGNATURE,
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
        public override IDeviceResponse Reboot() {
            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A26_REBOOT));
            return new PaxTerminalResponse(response, PAX_MSG_ID.A27_RSP_REBOOT);
        }
        public override IDeviceResponse DisableHostResponseBeep() {
            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A04_SET_VARIABLE,
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
        public override string SendCustomMessage(DeviceMessage message) {
            var response = _controller.Send(message);
            return Encoding.UTF8.GetString(response);
        }
        public override IDeviceResponse DeleteResource(string fileName)
        {
            byte[] response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A22_DELETE_IMAGE, fileName));
            return new PaxTerminalResponse(response, PAX_MSG_ID.A23_RSP_DELETE_IMAGE);
        }
        public override IDeviceResponse UpdateResource(
                            UpdateResourceFileType fileType, 
                            byte[] fileData, 
                            bool isHttpDeviceConnectionMode
                        )
        {
            int chunkSize = isHttpDeviceConnectionMode ? 3000 : 4000;
            byte[] response = null;
            int offset = 0;

            while (offset < fileData.Length)
            {
                int length = Math.Min(chunkSize, fileData.Length - offset);
                byte[] dataPacket = new byte[length];
                Array.Copy(fileData, offset, dataPacket, 0, length);
                bool isLastDataPacket = (offset + length) == fileData.Length;
                string base64String = Convert.ToBase64String(dataPacket);

                response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A18_UPDATE_RESOURCE_FILE,
                                    offset,
                                    ControlCodes.FS,
                                    base64String,
                                    ControlCodes.FS,
                                    isLastDataPacket ? "0" : "1",
                                    ControlCodes.FS,
                                    (int)fileType,
                                    ControlCodes.FS,
                                    "0",
                                    ControlCodes.FS
                                ));
                var resp = new PaxTerminalResponse(response, PAX_MSG_ID.A19_RSP_UPDATE_RESOURCE_FILE);

                if (resp.DeviceResponseCode != "000000")
                {
                    return new PaxTerminalResponse(response, PAX_MSG_ID.A19_RSP_UPDATE_RESOURCE_FILE);
                }
                if (isLastDataPacket)
                {
                    return new PaxTerminalResponse(response, PAX_MSG_ID.A19_RSP_UPDATE_RESOURCE_FILE);
                }
                offset += length;

            }

            return new PaxTerminalResponse(response, PAX_MSG_ID.A19_RSP_UPDATE_RESOURCE_FILE);
        }
        #endregion

        #region Reporting Messages
        public override TerminalReportBuilder LocalDetailReport() {
            return new TerminalReportBuilder(TerminalReportType.LocalDetailReport);
        }
        #endregion

        #region Batch Commands
        public override IBatchCloseResponse BatchClose() {
            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.B00_BATCH_CLOSE, DateTime.Now.ToString("YYYYMMDDhhmmss")));
            return new BatchCloseResponse(response);
        }
        public override IBatchClearResponse BatchClear()
        {
            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.B04_BATCH_CLEAR, "01")); //Sending specific Credit EDC type.
            return new BatchClearResponse(response);
        }
        #endregion

        #region Credit Methods
        //public TerminalAuthBuilder CreditAuth(decimal? amount = null) {
        //    return new TerminalAuthBuilder(TransactionType.Auth, PaymentMethodType.Credit).WithAmount(amount);
        //}

        //public TerminalManageBuilder CreditCapture(decimal? amount = null) {
        //    return new TerminalManageBuilder(TransactionType.Capture, PaymentMethodType.Credit).WithAmount(amount);
        //}

        ////public TerminalManageBuilder CreditEdit(decimal? amount = null) {
        ////    return new TerminalManageBuilder(TransactionType.Edit).WithAmount(amount);
        ////}

        //public TerminalAuthBuilder CreditRefund(decimal? amount = null) {
        //    return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.Credit).WithAmount(amount);
        //}

        //public TerminalAuthBuilder CreditSale(decimal? amount = null) {
        //    return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Credit).WithAmount(amount);
        //}

        //public TerminalAuthBuilder CreditVerify() {
        //    return new TerminalAuthBuilder(TransactionType.Verify, PaymentMethodType.Credit);
        //}

        //public TerminalManageBuilder CreditVoid() {
        //    return new TerminalManageBuilder(TransactionType.Void, PaymentMethodType.Credit);
        //}
        #endregion

        #region Debit Methods
        //public TerminalAuthBuilder DebitRefund(decimal? amount = null) {
        //    return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.Debit).WithAmount(amount);
        //}

        //public TerminalAuthBuilder DebitSale(decimal? amount = null) {
        //    return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Debit).WithAmount(amount);
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
        //    return new TerminalAuthBuilder(TransactionType.BenefitWithdrawal, PaymentMethodType.EBT).WithAmount(amount);
        //}
        #endregion

        #region Gift Methods
        //public TerminalAuthBuilder GiftSale(decimal? amount = null) {
        //    return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Gift)
        //        .WithAmount(amount)
        //        .WithCurrency(CurrencyType.CURRENCY);
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
    }
}