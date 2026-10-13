using System;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using System.Text;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Terminals.PAX.Responses;
using GlobalPayments.Api.Terminals.Enums;
using System.Xml.Linq;
using System.Threading;

namespace GlobalPayments.Api.Terminals.PAX {
    public class PaxInterface : DeviceInterface<PaxController> {
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
            var signatureResponse = new SignatureResponse(response, _controller.DeviceType.Value);
            if (signatureResponse.DeviceResponseCode == "000000")
                return GetSignatureFile();
            return signatureResponse;
        }
        public override IDeviceResponse ClearMessage()
        { 
            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A12_CLEAR_MESSAGE));
            return new PaxTerminalResponse(response, PAX_MSG_ID.A13_RSP_CLEAR_MESSAGE);
        }
        public override IDeviceResponse InputAccount(bool? allowMagStripeEntry, bool? allowManualEntry, bool? allowContactlessEntry, bool? allowScannerEntry, bool? expiryDatePrompt, int timeout, int? encryptionFlag, int? keySlot, int? minAccountLength, int? maxAccountLength, string edcType, string transactionType)
        {
            int? MagStripeEntry = allowMagStripeEntry.HasValue ? (allowMagStripeEntry.Value ? 1 : 0) : (int?)null;
            int? ManualEntry = allowManualEntry.HasValue ? (allowManualEntry.Value ? 1 : 0) : (int?)null;
            int? ContactlessEntry = allowContactlessEntry.HasValue ? (allowContactlessEntry.Value ? 1 : 0) : (int?)null;
            int? ScannerEntry = allowScannerEntry.HasValue ? (allowScannerEntry.Value ? 1 : 0) : (int?)null;
            int? DatePrompt = expiryDatePrompt.HasValue ? (expiryDatePrompt.Value ? 1 : 0) : (int?)null;

            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A30_INPUT_ACCOUNT,
                MagStripeEntry,
                ControlCodes.FS,
                ManualEntry,
                ControlCodes.FS,
                ContactlessEntry,
                ControlCodes.FS,
                ScannerEntry,
                ControlCodes.FS,
                DatePrompt,
                ControlCodes.FS,
                timeout,
                ControlCodes.FS,
                encryptionFlag,
                ControlCodes.FS,
                keySlot,
                ControlCodes.FS,
                minAccountLength,
                ControlCodes.FS,
                maxAccountLength,
                ControlCodes.FS,
                edcType,
                ControlCodes.FS,
                transactionType,
                ControlCodes.FS
            ));
            return new AccountInputResponse(response);
        }
        public override IDeviceResponse ShowMessage(string message1, string title, string message2, bool topDown, string taxLine, string totalLine, string imageName, string imageDesc, PaxLineItemAction lineItemAction, int itemIndex)
        {
            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A10_SHOW_MESSAGE,
                message1,
                ControlCodes.FS,
                title,
                ControlCodes.FS,
                message2,
                ControlCodes.FS,
                topDown,
                ControlCodes.FS,
                taxLine,
                ControlCodes.FS,
                totalLine,
                ControlCodes.FS,
                imageName,
                ControlCodes.FS,
                imageDesc,
                ControlCodes.FS,
                ((int)lineItemAction).ToString(),
                ControlCodes.FS,
                itemIndex
            ));
            return new PaxTerminalResponse(response, PAX_MSG_ID.A11_RSP_SHOW_MESSAGE);
        }
        public override IShowTextBoxResponse ShowTextBox(string title, string text, string button1Name
            , string button1Color, string button2Name, string button2Color, string button3Name
            , string button3Color
            , string timeout , string button1Key, string button2Key, string button3Key
            , bool enableHardKeyOnly, string hardKeyList, SignatureBoxDisplay signatureBoxDisplay
            , bool continuousScreen = false
            , int? barcodeType = null, string barcodeData = null, string inputTextTitle = null
            , bool showInputText = false, TextInputType inputType = TextInputType.AlphaNumeric
            , int minLength = 0
            , int maxLength = 32)
        {

            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A56_SHOW_TEXTBOX,
                title,
                ControlCodes.FS,
                text,
                ControlCodes.FS,
                button1Name,
                ControlCodes.FS,
                button1Color,
                ControlCodes.FS,
                button2Name,
                ControlCodes.FS,
                button2Color,
                ControlCodes.FS,
                button3Name,
                ControlCodes.FS,
                button3Color,
                ControlCodes.FS,
                timeout,
                ControlCodes.FS,
                button1Key,
                ControlCodes.FS,
                button2Key,
                ControlCodes.FS,
                button3Key,
                ControlCodes.FS,
                enableHardKeyOnly ? 1 : 0,
                ControlCodes.FS,
                hardKeyList,
                ControlCodes.FS,
                (int?)signatureBoxDisplay == 0 ? null : (int?)signatureBoxDisplay,
                ControlCodes.FS,
                continuousScreen ? 1 : 0,
                ControlCodes.FS,
                barcodeType,
                ControlCodes.FS,
                barcodeData,
                ControlCodes.FS,
                inputTextTitle,
                ControlCodes.FS,
                showInputText ? 1 : 0,
                ControlCodes.FS,
                (int)inputType,
                ControlCodes.FS,
                minLength,
                ControlCodes.FS,
                maxLength
            )); ;
            return new ShowTextBoxResponse(response);
        }
        public override IDeviceResponse SetTipPercentageOptions(int tipPercent1,int tipPercent2, int tipPercent3,bool noTipSelection)
        {
            string noTipOption = noTipSelection ? "Y" : "N";
            string tipPercentages = tipPercent1 + " " + tipPercent2 + " " + tipPercent3;
            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A04_SET_VARIABLE,
                "01",
                ControlCodes.FS,
                "tipPercentageOptions",
                ControlCodes.FS,
                tipPercentages,
                ControlCodes.FS,
                "noTipSelection",
                ControlCodes.FS,
                noTipOption,
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
        public override IDeviceResponse SetTipAmountOptions(int tipAmount1, int tipAmount2, int tipAmount3, bool noTipSelection)
        {
            string noTipOption = noTipSelection ? "Y" : "N";
            string tipAmounts = tipAmount1 + " " + tipAmount2 + " " + tipAmount3;
            var response = _controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A04_SET_VARIABLE,
                "01",
                ControlCodes.FS,
                "tipAmountOptions",
                ControlCodes.FS,
                tipAmounts,
                ControlCodes.FS,
                "noTipSelection",
                ControlCodes.FS,
                noTipOption,
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
    }
}