using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Utils;
using System.Text;
using Newtonsoft.Json;

namespace GlobalPayments.Api.Terminals.UPA {
    public class UpaInterface : DeviceInterface<UpaController>, IDeviceInterface {

        internal UpaInterface(UpaController controller) : base(controller) { }

        public override TerminalManageBuilder TipAdjust(decimal? amount) {
            return new TerminalManageBuilder(TransactionType.Edit, PaymentMethodType.Credit)
                .WithGratuity(amount);
        }

        public override TerminalAuthBuilder Tokenize() {
            return new TerminalAuthBuilder(TransactionType.Tokenize, PaymentMethodType.Credit);
        }

        public override TerminalAuthBuilder AuthCompletion() {
            return new TerminalAuthBuilder(TransactionType.Auth, PaymentMethodType.Credit);
        }

        public override TerminalManageBuilder DeletePreAuth()
        {
            return new TerminalManageBuilder(TransactionType.Delete, PaymentMethodType.Credit)
                .WithTransactionModifier(TransactionModifier.DeletePreAuth);
        }

        public override IEODResponse EndOfDay() {
            var requestId = _controller.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.EodProcessing));
            var jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new UpaEODResponse(jsonParse);
        }

        public override IDeviceResponse Reboot() {
            var requestId = _controller.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.Reboot));
            var json = Encoding.UTF8.GetString(response);

            var j = JsonConvert.DeserializeObject<BasicResponse>(json);

            return new DeviceResponse
            {
                Command = j.Data.Response,
                DeviceId = "",
                DeviceResponseCode = "",
                DeviceResponseText = "",
                Status = j.Data.CmdResult.Result,
                ReferenceNumber = j.Data.RequestId
            };
        }

        public override IDeviceResponse LineItem(string leftText, string rightText = null, string runningLeftText = null, string runningRightText = null) {
            var requestId = _controller.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.LineItemDisplay, leftText, rightText));
            var jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }
        
        public override void Cancel() {
            var requestId = _controller.GetRequestId();
            _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.CancelTransaction));
        }

        public override ISAFResponse SendStoreAndForward()
        {
            var requestId = _controller.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.SendSAF));
            var jsonObject = Encoding.UTF8.GetString(response);

            var doc = JsonDoc.Parse(jsonObject);
            return new UpaSAFResponse(doc);
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