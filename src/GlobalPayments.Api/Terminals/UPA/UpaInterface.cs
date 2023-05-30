using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Utils;
using System.Text;
using log4net;

namespace GlobalPayments.Api.Terminals.UPA {
    public class UpaInterface : DeviceInterface<UpaController>, IDeviceInterface {

        private readonly ILog _logger = LogManager.GetLogger(typeof(UpaInterface));

        internal UpaInterface(UpaController controller) : base(controller) { }

        public override TerminalAuthBuilder TipAdjust(decimal? amount) {
            return new TerminalAuthBuilder(TransactionType.Edit, PaymentMethodType.Credit)
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
            string jsonObject = Encoding.UTF8.GetString(response);

            if(_logger.IsDebugEnabled)
                _logger.Debug($"Raw Response: {jsonObject}");

            var jsonParse = JsonDoc.Parse(jsonObject);
            return new UpaEODResponse(jsonParse);
        }

        public override IDeviceResponse Reboot() {
            var requestId = _controller.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.Reboot));
            string jsonObject = Encoding.UTF8.GetString(response);

            if(_logger.IsDebugEnabled)
                _logger.Debug($"Raw Response: {jsonObject}");

            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }

        public override IDeviceResponse LineItem(string leftText, string rightText = null, string runningLeftText = null, string runningRightText = null) {
            var requestId = _controller.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.LineItemDisplay, leftText, rightText));
            string jsonObject = Encoding.UTF8.GetString(response);

            if(_logger.IsDebugEnabled)
                _logger.Debug($"Raw Response: {jsonObject}");

            var jsonParse = JsonDoc.Parse(jsonObject);
            return new TransactionResponse(jsonParse);
        }
        
        public override void Cancel() {
            var requestId = _controller.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.CancelTransaction));
        }

        public override ISAFResponse SendStoreAndForward()
        {
            var requestId = _controller.GetRequestId();
            var response = _controller.Send(TerminalUtilities.BuildUpaAdminRequest(requestId, EcrId, UpaTransType.SendSAF));
            string jsonObject = Encoding.UTF8.GetString(response);

            if(_logger.IsDebugEnabled)
                _logger.Debug($"Raw Response: {jsonObject}");

            JsonDoc doc = JsonDoc.Parse(jsonObject);
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