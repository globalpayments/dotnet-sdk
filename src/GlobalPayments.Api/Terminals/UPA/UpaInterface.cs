using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.UPA;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.Enums;
using GlobalPayments.Api.Terminals.UPA.Responses;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.UPA
{
    public class UpaInterface : DeviceInterface<UpaController>, IDeviceInterface {
        internal UpaInterface(UpaController controller) : base(controller) { }

        public override TerminalManageBuilder TipAdjust(decimal? amount) {
            return new TerminalManageBuilder(TransactionType.Edit, PaymentMethodType.Credit)
                .WithGratuity(amount);
        }
        public override TerminalManageBuilder Reverse() {
            return new TerminalManageBuilder(TransactionType.Reversal, PaymentMethodType.Credit);
        }
        public override TerminalAuthBuilder Tokenize()
        {
            return new TerminalAuthBuilder(TransactionType.Tokenize, PaymentMethodType.Credit);
        }

        public override TerminalAuthBuilder AuthCompletion()
        {
            return new TerminalAuthBuilder(TransactionType.Auth, PaymentMethodType.Credit);
        }

        public override TerminalManageBuilder DeletePreAuth()
        {
            return new TerminalManageBuilder(TransactionType.Delete, PaymentMethodType.Credit)
                .WithTransactionModifier(TransactionModifier.DeletePreAuth);
        }

        public override IEODResponse EndOfDay()
        {
            var requestId = _controller.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.EodProcessing));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new UpaEODResponse(jsonParse);
        }

        public override IDeviceResponse Reboot()
        {
            var requestId = _controller.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.Reboot));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override IDeviceResponse LineItem(string leftText, string rightText = null, string runningLeftText = null, string runningRightText = null)
        {
            var requestId = _controller.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.LineItemDisplay, leftText, rightText));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override void Cancel()
        {
            var requestId = _controller.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.CancelTransaction));
        }

        public override ISAFResponse SendStoreAndForward()
        {
            var requestId = _controller.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.SendSAF));
            string jsonObject = Encoding.UTF8.GetString(response);
            JsonDoc doc = JsonDoc.Parse(jsonObject);
            return new UpaSAFResponse(doc);
        }
        public override ISAFResponse DeleteSaf(string safReferenceNumber, string tranNo=null)
        {
            var requestId = _controller.GetRequestId();

            var body = new JsonDoc();
            body.Set("message", UpaMessageType.Msg);
            var data = body.SubElement("data");
            data.Set("command", UpaTransType.DeleteSAF);
            data.Set("EcrId", EcrId);
            data.Set("requestId", requestId);
            var data2 = data.SubElement("data");
            var transaction = data2.SubElement("transaction");
            if (!string.IsNullOrEmpty(tranNo))
            {
                transaction.Set("tranNo", tranNo);
            }
            if (!string.IsNullOrEmpty(safReferenceNumber))
            {
                transaction.Set("referenceNumber", safReferenceNumber);
            }
            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(body.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);           
            return new UpaSAFResponse(JsonDoc.Parse(jsonObject));
        }

        public override IDeviceResponse RegisterPOS(POSData posData) {
            if (string.IsNullOrEmpty(posData.AppName)) {
                throw new UnsupportedTransactionException($"{nameof(posData.AppName)} is a mandatory parameter.");
            }

            var requestId = _controller.GetRequestId();

            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, UpaTransType.RegisterPOS, out doc, out baseRequest);

            var request = baseRequest.SubElement("data");
            var requestParams = request.SubElement("params");
            requestParams.Set("appName", posData.AppName)
                .Set("launchOrder", posData.LaunchOrder > 0 ? posData.LaunchOrder.ToString() : null)
                .Set("remove", posData.Remove ? "true" : "false")
                .Set("silent", posData.Silent.ToString());

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);           
        }
        public override ISignatureResponse PromptAndGetSignatureFile(string prompt1, string prompt2, int? displayOption) {
            var requestId = _controller.GetRequestId();
            var doc = new JsonDoc();
            doc.Set("message", UpaMessageType.Msg);
            var data = doc.SubElement("data");
            data.Set("command", UpaTransType.GetSignature);
            data.Set("EcrId", EcrId);
            data.Set("requestId", requestId);
            var request = data.SubElement("data");
            var requestParams = request.SubElement("params");
            requestParams.Set("prompt1", prompt1);
            if (!string.IsNullOrEmpty(prompt2))
            {
                requestParams.Set("prompt2", prompt2);
            }
            if (displayOption.HasValue)
            {
                requestParams.Set("displayOption", displayOption);
            }

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            return new UpaSignatureResponse(JsonDoc.Parse(jsonObject));            
        }
        public override IDeviceResponse StartCardTransaction(UpaParam param, ProcessingIndicator indicator, UpaTransactionData transData)
        {
            var requestId = _controller.GetRequestId();
            var doc = new JsonDoc();
            doc.Set("message", UpaMessageType.Msg);
            var data = doc.SubElement("data");
            data.Set("command", UpaTransType.StartCardTransaction);
            data.Set("EcrId", EcrId);
            data.Set("requestId", requestId);
            var request = data.SubElement("data");
            if (param != null)
            {
                var requestParams = request.SubElement("params");
                requestParams.Set("timeOut", param.Timeout);
                requestParams.Set("acquisitionTypes", param.AcquisitionTypes.ToString());
                requestParams.Set("header", param.Header);
                requestParams.Set("displayTotalAmount", param.DisplayTotalAmount);
                requestParams.Set("PromptForManualEntryPassword", param.PromptForManual ? 1 : 0);
                requestParams.Set("brandIcon1", param.BrandIcon1);
                requestParams.Set("brandIcon2", param.BrandIcon2);
            }

            if (indicator != null)
            {
                var requestIndicator = request.SubElement("processingIndicators");
                requestIndicator.Set("quickChip", indicator.QuickChip);
                requestIndicator.Set("checkLuhn", indicator.CheckLuhn);
                requestIndicator.Set("securityCode", indicator.SecurityCode);
                requestIndicator.Set("cardTypeFilter", indicator.CardTypeFilter.ToString());
            }

            if (transData != null)
            {
                var requestTransaction = request.SubElement("transaction");
                requestTransaction.Set("totalAmount", Regex.Replace(string.Format("{0:c}", transData.TotalAmount), "[^0-9.]", ""));
                requestTransaction.Set("cashBackAmount", Regex.Replace(string.Format("{0:c}", transData.CashBackAmount), "[^0-9.]", ""));
                requestTransaction.Set("tranDate", transData.TranDate.ToString("MMddyyyy"));
                requestTransaction.Set("tranTime", transData.TranTime.ToString("hh:m:ss"));
                requestTransaction.Set("transactionType", transData.TransType.ToString());
            }

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            return new UpaGiftCardResponse(JsonDoc.Parse(jsonObject));
        }
        
        public virtual TerminalManageBuilder Void() {
            return new TerminalManageBuilder(TransactionType.Void, PaymentMethodType.Credit).
                WithTransactionModifier(TransactionModifier.NoValidationRequired);
        }

        #region Reporting
        public override TerminalReportBuilder GetSAFReport()
        {
            return new TerminalReportBuilder(TerminalReportType.GetSAFReport);
        }

        public override TerminalReportBuilder GetBatchReport()
        {
            return new TerminalReportBuilder(TerminalReportType.GetBatchReport);
        }

        public override ITerminalReport GetBatchDetails(string batchId, bool printReport = false) {
            var builder = (new TerminalReportBuilder(TerminalReportType.GetBatchDetails))
                .Where(UpaSearchCriteria.Batch, batchId);
            if (printReport) {
                builder.And(UpaSearchCriteria.ReportOutput,
                    string.Join("|", new string[] { ReportOutput.Print.ToString(), ReportOutput.ReturnData.ToString() })
                );
            }

            return builder.Execute();
        }

        public override IDeviceResponse Ping() {
            var requestId = _controller.RequestIdProvider.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.Ping));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override IDeviceResponse Reset() {
            var requestId = _controller.RequestIdProvider.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.Restart));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override IDeviceResponse GetAppInfo() {
            var requestId = _controller.RequestIdProvider.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.GetAppInfo));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override IDeviceResponse ClearDataLake() {
            var requestId = _controller.RequestIdProvider.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.ClearDataLake));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override IDeviceResponse SetTimeZone(string timeZone) {
            var requestId = _controller.RequestIdProvider.GetRequestId();

            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, UpaTransType.SetTimeZone, out doc, out baseRequest);            
            var request = baseRequest.SubElement("data");
            var requestParams = request.SubElement("params");
            requestParams.Set("timeZone", timeZone);
            
            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override IDeviceResponse GetParams(string[] parameters) {
            var requestId = _controller.RequestIdProvider.GetRequestId();

            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, UpaTransType.GetParam, out doc, out baseRequest);
            var request = baseRequest.SubElement("data");
            var requestParams = request.SubElement("params");
            requestParams.Set("configuration", parameters);


            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }
        
        public override IDeviceResponse ReturnToIdle() {
            var requestId = _controller.RequestIdProvider.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.ReturnToIdle));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override IDeviceScreen LoadUDData(UDData udData) {
            if (string.IsNullOrEmpty(udData.FileName)) {
                throw new UnsupportedTransactionException($"{nameof(udData.FileName)} is a mandatory parameter.");
            }

            var requestId = _controller.RequestIdProvider.GetRequestId();

            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, UpaTransType.LoadUDDataFile, out doc, out baseRequest);
            var request = baseRequest.SubElement("data");
            var requestParams = request.SubElement("params");
            requestParams.Set("fileType", udData.FileType.ToString())
                .Set("slotNum", udData.SlotNum.ToString())
                .Set("file", udData.FileName);

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new UDScreenResponse(jsonParse);
        } 
        
        public override IDeviceScreen RemoveUDData(UDData udData) {            
            var requestId = _controller.RequestIdProvider.GetRequestId();

            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, UpaTransType.RemoveUDDataFile, out doc, out baseRequest);
            var request = baseRequest.SubElement("data");
            var requestParams = request.SubElement("params");
            requestParams.Set("fileType", udData.FileType.ToString())
                .Set("slotNum", udData.SlotNum.ToString());

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new UDScreenResponse(jsonParse);
        }
        
        public override IDeviceResponse Scan(ScanData scanData = null) {
            var requestId = _controller.RequestIdProvider.GetRequestId();

            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, UpaTransType.Scan, out doc, out baseRequest);

            if (scanData != null) {                
                var dataParams = new JsonDoc().Set("header", !string.IsNullOrEmpty(scanData.Header) ? scanData.Header.ToUpper() : null)
                    .Set("prompt1", !string.IsNullOrEmpty(scanData.Prompt1) ? scanData.Prompt1.ToUpper() : null)
                    .Set("prompt2", !string.IsNullOrEmpty(scanData.Prompt2) ? scanData.Prompt2.ToUpper() : null)
                    .Set("displayOption", scanData.DisplayOption.HasValue ? scanData.DisplayOption.Value.ToString("D") : null)
                    .Set("timeOut", scanData.TimeOut != null ? scanData.TimeOut : null);

                if (dataParams.HasKeys()) {
                    var request = baseRequest.SubElement("data");
                    request.SetArrange("params", dataParams);
                }
            }

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        /// <summary>
        /// <paramref name="debugLevel"/> is of type DebugLevel Enum
        /// <paramref name="logToConsole"/> is of type LogToConsole Enum
        /// </summary>
        /// <param name="debugLevel"></param>
        /// <param name="logToConsole"></param>
        /// <returns></returns>
        public override IDeviceResponse SetDebugLevel(Enum[] debugLevel, Enum logToConsole = null)
        {
            var requestId = _controller.RequestIdProvider.GetRequestId();

            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, UpaTransType.SetDebugLevel, out doc, out baseRequest);
            var request = baseRequest.SubElement("data");
            var requestParams = request.SubElement("params");
            requestParams.Set("debugLevel", string.Join("|", debugLevel.ToList()))
                .Set("logToConsole", logToConsole != null ?  logToConsole.ToString("D") : null);

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override IDeviceResponse GetDebugLevel()
        {
            var requestId = _controller.RequestIdProvider.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.GetDebugLevel));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        /// <summary>
        /// <paramref name="logFile"/> is of type LogFile Enum
        /// </summary>
        /// <param name="logFile"></param>
        /// <returns></returns>
        public override IDeviceResponse GetDebugInfo(Enum logFile = null)
        {
            var requestId = _controller.RequestIdProvider.GetRequestId();

            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, UpaTransType.GetDebugInfo, out doc, out baseRequest);
            if (logFile != null) {
                var request = baseRequest.SubElement("data");
                var requestParams = request.SubElement("params");
                requestParams.Set("logFile", logFile.ToString("D"));
            }

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override IDeviceResponse BroadcastConfiguration(bool enable)
        {
            var requestId = _controller.RequestIdProvider.GetRequestId();

            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, UpaTransType.BroadcastConfiguration, out doc, out baseRequest);            
            var request = baseRequest.SubElement("data");
            var requestParams = request.SubElement("params");
            requestParams.Set("enable", enable ? "1" : "0");
            

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

                        
        public override IDeviceResponse Print(PrintData printData) {
            var requestId = _controller.RequestIdProvider.GetRequestId();

            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, UpaTransType.PrintData, out doc, out baseRequest);
            var request = baseRequest.SubElement("data");
            var requestParams = request.SubElement("params");
            string content = TerminalUtilities.BuildToBase64Content(printData.FilePath, UpaTransType.PrintData, true);
            var extension = printData.FilePath.Split('.');
            string mimeType = extension.LastOrDefault().ToLower();
            content = $"data:image/{mimeType};base64,{content}";
            requestParams.Set("content", content)
                .Set("line1", printData.Line1 ?? "")
                .Set("line2", printData.Line2 ?? "")
                .Set("displayOption", printData.DisplayOption.HasValue ? printData.DisplayOption.Value.ToString("D") : null);

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override TerminalManageBuilder UpdateTaxInfo(decimal? amount = null)
        {
            return new TerminalManageBuilder(TransactionType.Edit, PaymentMethodType.Credit)
                .WithTransactionModifier(TransactionModifier.UpdateTaxDetail)
                .WithTaxAmount(amount);
        }

        public override TerminalManageBuilder UpdateLodginDetail(decimal? amount = null)
        {
            return new TerminalManageBuilder(TransactionType.Edit, PaymentMethodType.Credit)
                .WithTransactionModifier(TransactionModifier.UpdateLodgingDetails)
                .WithAmount(amount);
        }

        public override IDeviceResponse CommunicationCheck()
        {
            var requestId = _controller.RequestIdProvider.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.CommunicationCheck));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override IDeviceResponse Logon()
        {
            var requestId = _controller.RequestIdProvider.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.Logon));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override IBatchCloseResponse GetLastEOD()
        {
            var requestId = _controller.RequestIdProvider.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.GetLastEOD));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override TerminalReportBuilder FindBatches() {          
            return new TerminalReportBuilder(TerminalReportType.FindBatches);
        }
        
        public override IDeviceResponse ExecuteUDDataFile(UDData udData)
        {
            var requestId = _controller.RequestIdProvider.GetRequestId();

            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, UpaTransType.ExecuteUDDataFile, out doc, out baseRequest);
            var request = baseRequest.SubElement("data");
            var requestParams = request.SubElement("params");
            requestParams.Set("fileType", udData.FileType.ToString())
                         .Set("displayOption", udData.DisplayOption.HasValue ? udData.DisplayOption.Value.ToString("D") : null)
                         .Set("slotNum", udData.SlotNum.ToString());

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override IDeviceResponse InjectUDDataFile(UDData udData)
        {
            var requestId = _controller.RequestIdProvider.GetRequestId();
            var content = string.Empty;
            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, UpaTransType.InjectUDDataFile, out doc, out baseRequest);
            var request = baseRequest.SubElement("data");
            var requestParams = request.SubElement("params");
            if (udData.FileType == UDFileType.HTML5) {
                content = TerminalUtilities.BuildStringFromFile(udData.FilePath).Replace('\"', '\'');
            }
            else {
                content = TerminalUtilities.BuildToBase64Content(udData.FilePath, UpaTransType.InjectUDDataFile, true);
            }
            requestParams.Set("fileType", udData.FileType.ToString())
                         .Set("fileName", udData.FileName)
                         .Set("content", content);

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override IDeviceResponse GetConfigContents(TerminalConfigType configType)
        {
            var requestId = _controller.RequestIdProvider.GetRequestId();

            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, UpaTransType.GetConfigContents, out doc, out baseRequest);
            var request = baseRequest.SubElement("data");
            var requestParams = request.SubElement("params");
            requestParams.Set("configType", configType.ToString("D"));

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override IDeviceResponse RemoveCard(string language = null)
        {
            var requestId = _controller.RequestIdProvider.GetRequestId();

            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, UpaTransType.RemoveCard, out doc, out baseRequest);

            if (!string.IsNullOrEmpty(language)) {
                var dataParams = new JsonDoc().Set("languageCode", language);
                if (dataParams.HasKeys()) {
                    var request = baseRequest.SubElement("data");
                    request.SetArrange("params", dataParams);
                }
            }          

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        } 
        public override IDeviceResponse EnterPin(PromptMessages promptMessages, bool canBypass, string accountNumber)
        {
            var requestId = _controller.RequestIdProvider.GetRequestId();

            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, UpaTransType.EnterPIN, out doc, out baseRequest);
            var request = baseRequest.SubElement("data");
            var requestParams = request.SubElement("params");
            requestParams.Set("prompt1", promptMessages.Prompt1)
                    .Set("prompt2", promptMessages.Prompt2)
                    .Set("prompt3", promptMessages.Prompt3);

            var dataTerminal = request.SubElement("terminal");
            dataTerminal.Set("canBypass", canBypass ? "Y" : "N");

            if (!string.IsNullOrEmpty(accountNumber)) {
                var dataTransaction = new JsonDoc().Set("accountNumber", accountNumber);
                request.SetArrange("transaction", dataTransaction);
            }

            ValidateMandatoryParams(doc, new List<string>() { "prompt1", "canBypass", "accountNumber" });

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }
        public override IDeviceResponse Prompt(PromptType promptType, PromptData promptData)
        {
            var requestId = _controller.RequestIdProvider.GetRequestId();

            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, 
                promptType == PromptType.OPTIONS ? UpaTransType.PromptwithOptions : UpaTransType.PromptMenu, out doc, out baseRequest);           
            
            var request = baseRequest.SubElement("data");
            var dataGeneralParams = new JsonDoc();

            if (promptData != null) {                
                if (!string.IsNullOrEmpty(promptData.Prompts.Prompt1)) {
                    dataGeneralParams.Set("prompt1", promptData.Prompts.Prompt1);
                }
                if(promptData.Timeout != null) {
                    dataGeneralParams.Set("timeout", promptData.Timeout.ToString());
                }                
            }

            int index = 0;
            if (promptData.Buttons != null) {
                foreach (var button in promptData.Buttons) {
                    var item = new JsonDoc().Set("color", button.Color)
                        .Set("text", button.Text);
                    index++;
                    dataGeneralParams.Set($"button{index}", item);
                }
            }            

            switch (promptType) {
                case PromptType.MENU:
                    if (promptData.Menu != null && promptData.Menu.Count() > 0) {
                        dataGeneralParams.Set("menu", promptData.Menu);
                    }
                    if (dataGeneralParams.HasKeys()) {
                        request.SetArrange("params", dataGeneralParams);
                    }

                    ValidateMandatoryParams(doc, new List<string>() { "prompt1", "menu", "button1" });
                    break;
                case PromptType.OPTIONS:
                    dataGeneralParams.Set("prompt2", promptData.Prompts.Prompt2)
                        .Set("prompt3", promptData.Prompts.Prompt3);

                    if (dataGeneralParams.HasKeys()) {
                        request.SetArrange("params", dataGeneralParams);
                    }

                    ValidateMandatoryParams(doc, new List<string>() { "button1"});
                    break;
                default:
                    break;
            }

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override IDeviceResponse GetGenericEntry(GenericData data)
        {
            var requestId = _controller.RequestIdProvider.GetRequestId();

            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, UpaTransType.GeneralEntry, out doc, out baseRequest);
            var request = baseRequest.SubElement("data");

            var requestParams = new JsonDoc()
            .Set("prompt1", data.Prompts.Prompt1)
                    .Set("prompt2", data.Prompts.Prompt2)
                    .Set("prompt3", data.Prompts.Prompt3)
                    .Set("button1", data.TextButton1 ?? null)
                    .Set("button2", data.TextButton2 ?? null)
                    .Set("timeOut", data.Timeout?.ToString() ?? null)
                    .Set("entryFormat", String.Join("|", data.EntryFormat.Select(p=>p.ToString())) ?? null)
                    .Set("minLen", data.EntryMinLen?.ToString() ?? null)
                    .Set("maxLen", data.EntryMaxLen?.ToString() ?? null)
                    .Set("alignment", EnumConverter.GetDescription(data.Alignment) ?? null);

            if (requestParams.HasKeys()) {
                request.SetArrange("params", requestParams);
            }

            ValidateMandatoryParams(doc, new List<string>() { "button1", "button2", "entryFormat", "maxLen" });

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }
        
        public override IDeviceResponse DisplayMessage(MessageLines messageLines)
        {
            var requestId = _controller.RequestIdProvider.GetRequestId();

            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, UpaTransType.DisplayMessage, out doc, out baseRequest);
            var request = baseRequest.SubElement("data");

            var requestParams = new JsonDoc()
            .Set("line1", messageLines.Line1)
                    .Set("line2", messageLines.Line2 ?? null)
                    .Set("line3", messageLines.Line3 ?? null)
                    .Set("line4", messageLines.Line4 ?? null)
                    .Set("line5", messageLines.Line5 ?? null)
                    .Set("timeOut", messageLines.Timeout?.ToString() ?? null);

            if (requestParams.HasKeys()) {
                request.SetArrange("params", requestParams);
            }

            ValidateMandatoryParams(doc, new List<string>() { "line1" });

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override IDeviceResponse ReturnDefaultScreen(DisplayOption option)
        {
            var requestId = _controller.RequestIdProvider.GetRequestId();

            JsonDoc doc, baseRequest;
            TerminalUtilities.BuildGeneralUpaRequest(requestId, EcrId, UpaTransType.ReturnDefaultScreen, out doc, out baseRequest);
            var request = baseRequest.SubElement("data");
            var requestParams = request.SubElement("params");
            requestParams.Set("displayOption", option.ToString("D"));

            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(doc.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override IDeviceResponse GetEncryptionType()
        {
            var requestId = _controller.RequestIdProvider.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.GetEncryptionType));
            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override TerminalReportBuilder GetOpenTabDetails() {
            return new TerminalReportBuilder(TerminalReportType.GetOpenTabDetails);
        }

        public override TerminalAuthBuilder ContinueTransaction(decimal amount, bool isEmv = false) {
            var transModifier = isEmv ? TransactionModifier.ContinueEMVTransaction : TransactionModifier.ContinueCardTransaction;

            return new TerminalAuthBuilder(TransactionType.Confirm, PaymentMethodType.Credit)
                .WithTransactionModifier(transModifier)
                .WithAmount(amount);
        }

        public override TerminalAuthBuilder CompleteTransaction() {
            return new TerminalAuthBuilder(TransactionType.Confirm, PaymentMethodType.Credit)
                .WithTransactionModifier(TransactionModifier.CompleteTransaction);
        }

        public override TerminalAuthBuilder ProcessTransaction(decimal amount, TransactionType transactionType = TransactionType.Sale) {
            return new TerminalAuthBuilder(transactionType, PaymentMethodType.Credit)
                .WithTransactionModifier(TransactionModifier.ProcessTransaction)
                .WithAmount(amount);
        }

        private void ValidateMandatoryParams(JsonDoc request, List<string> listMandatoryParams)
        {
            Validations.SetMandatoryParams(listMandatoryParams);
            var missingParams = new List<string>();
            Validations.Validate(request, out missingParams);
            if (missingParams.Count > 0) {
                throw new ArgumentException($"Mandatory params missing: {String.Join(",", missingParams)}");
            }
        }
        #endregion
    }
}