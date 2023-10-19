using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.UPA;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.UPA.Responses;
using GlobalPayments.Api.Utils;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace GlobalPayments.Api.Terminals.UPA
{
    public class UpaInterface : DeviceInterface<UpaController>, IDeviceInterface
    {
        internal UpaInterface(UpaController controller) : base(controller) { }

        public override TerminalManageBuilder TipAdjust(decimal? amount) {
            return new TerminalManageBuilder(TransactionType.Edit, PaymentMethodType.Credit)
                .WithGratuity(amount);
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
        public override IDeviceResponse RegisterPOS(string appName, int launchOrder = 0, bool remove = false, int silent = 0)
        {
            var requestId = _controller.GetRequestId();

            var body = new JsonDoc();
            body.Set("message", UpaMessageType.Msg);
            var param = new JsonDoc();

            if (!string.IsNullOrEmpty(appName))
            {
                param.Set("appName", appName);
            }

            if (launchOrder>0)
            {
                param.Set("launchOrder", launchOrder.ToString());
            }
            if (remove)
            {
                param.Set("remove", "true");
            }
            else
            {
                param.Set("remove", "false");
            }
           
            param.Set("silent", silent.ToString());           

            var data = body.SubElement("data");
            data.Set("command", UpaTransType.RegisterPOS);
            data.Set("EcrId", EcrId);
            data.Set("requestId", requestId);
            var data2 = data.SubElement("data");
            data2.Set("params", param);            
                        
            var response = _controller.Send(TerminalUtilities.BuildUpaRequest(body.ToString()));
            string jsonObject = Encoding.UTF8.GetString(response);
           
            return new TransactionResponse(JsonDoc.Parse(jsonObject));
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

        #region Reporting
        public override TerminalReportBuilder GetSAFReport()
        {
            return new TerminalReportBuilder(TerminalReportType.GetSAFReport);
        }

        public override TerminalReportBuilder GetBatchReport()
        {
            return new TerminalReportBuilder(TerminalReportType.GetBatchReport);
        }

        public override TerminalReportBuilder GetOpenTabDetails()
        {
            return new TerminalReportBuilder(TerminalReportType.GetOpenTabDetails);
        }
        #endregion
    }
}