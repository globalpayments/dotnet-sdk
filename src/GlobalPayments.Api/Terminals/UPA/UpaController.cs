using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace GlobalPayments.Api.Terminals.UPA
{
    public class UpaController : DeviceController
    {
        internal override IDeviceInterface ConfigureInterface()
        {
            if (_interface == null)
            {
                _interface = new UpaInterface(this);
            }
            return _interface;
        }
        internal override IDeviceCommInterface ConfigureConnector()
        {
            switch (_settings.ConnectionMode)
            {
                case ConnectionModes.TCP_IP:
                    return new UpaTcpInterface(_settings);
                case ConnectionModes.HTTP:
                case ConnectionModes.SERIAL:
                case ConnectionModes.SSL_TCP:
                default:
                    throw new NotImplementedException();
            }
        }

        internal UpaController(ITerminalConfiguration settings) : base(settings) { }

        #region overrides
        internal override ITerminalResponse ProcessTransaction(TerminalAuthBuilder builder)
        {
            var request = BuildProcessTransaction(builder);
            return DoTransaction(request);
        }

        internal override ITerminalResponse ManageTransaction(TerminalManageBuilder builder)
        {
            var request = BuildManageTransaction(builder);
            return DoTransaction(request);
        }

        internal override ITerminalReport ProcessReport(TerminalReportBuilder builder)
        {
            var response = Send(BuildReportTransaction(builder));

            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);
            
            switch (builder.ReportType)
            {
                case TerminalReportType.GetSAFReport:
                    return new SafReportResponse(jsonParse);
                case TerminalReportType.GetBatchReport:
                    return new BatchReportResponse(jsonParse);
                default: return null;
            }
        }

        internal IDeviceMessage BuildProcessTransaction(TerminalAuthBuilder builder)
        {
            var pmt = builder.PaymentMethodType;
            var transType = builder.TransactionType;

            if (pmt != PaymentMethodType.Credit && pmt != PaymentMethodType.Debit && pmt != PaymentMethodType.EBT) {
                throw new UnsupportedTransactionException("The supplied payment method type is not supported");
            }

            int requestId = builder.ReferenceNumber;
            if (requestId == default(int) && RequestIdProvider != null) {
                requestId = RequestIdProvider.GetRequestId();
            }

            var doc = new JsonDoc();
            doc.Set("message", "MSG");

            var baseRequest = doc.SubElement("data");
            baseRequest.Set("command", MapTransactionType(builder.TransactionType, builder.RequestMultiUseToken));
            baseRequest.Set("EcrId", builder.EcrId.ToString());
            baseRequest.Set("requestId", requestId);

            if (transType != TransactionType.Balance && transType != TransactionType.EodProcessing) {
                var txnData = baseRequest.SubElement("data");

                var txnParams = txnData.SubElement("params");
                txnParams.Set("clerkId", builder.ClerkId);
                txnParams.Set("tokenRequest", builder.RequestMultiUseToken ? "1" : "0");
                if(builder.PaymentMethod is CreditCardData) {
                    txnParams.Set("tokenValue", ((CreditCardData)builder.PaymentMethod).Token);
                }
                
                txnParams.Set("lineItemLeft", builder.LineItemLeft);
                txnParams.Set("lineItemRight", builder.LineItemRight);

                if (transType != TransactionType.Verify && transType != TransactionType.Refund) {
                    var transaction = txnData.SubElement("transaction");
                    var baseAmount = builder.Amount - (builder.CashBackAmount ?? 0) - builder.TaxAmount - (builder.Gratuity ?? 0);
                    transaction.Set("baseAmount", ToCurrencyString(baseAmount));
                    transaction.Set("cashBackAmount", ToCurrencyString(builder.CashBackAmount));
                    transaction.Set("taxAmount", ToCurrencyString(builder.TaxAmount));
                    transaction.Set("tipAmount", ToCurrencyString(builder.Gratuity));
                    transaction.Set("taxIndicator", builder.TaxExempt);
                    transaction.Set("invoiceNbr", builder.InvoiceNumber);
                    transaction.Set("tranNo", builder.TerminalRefNumber);
                }

                if (transType == TransactionType.Refund) {
                    var transaction = txnData.SubElement("transaction");
                    transaction.Set("totalAmount", ToCurrencyString(builder.Amount));
                }
            }

            return TerminalUtilities.BuildUpaRequest(doc.ToString());
        }

        internal IDeviceMessage BuildReportTransaction(TerminalReportBuilder builder) {
            var requestId = builder.SearchBuilder.ReferenceNumber;
            if (requestId == default(string) && RequestIdProvider != null) {
                requestId = RequestIdProvider.GetRequestId().ToString();
            }

            var doc = new JsonDoc();
            doc.Set("message", "MSG");

            var baseRequest = doc.SubElement("data");
            baseRequest.Set("command", MapReportType(builder.ReportType));
            baseRequest.Set("EcrId", builder.SearchBuilder.EcrId.ToString());
            baseRequest.Set("requestId", requestId);

            var txnData = baseRequest.SubElement("data");
            var txnParams = txnData.SubElement("params");
            txnParams.Set("reportOutput", builder.SearchBuilder.ReportOutput);
            txnParams.Set("batch", builder.SearchBuilder.Batch);

            return TerminalUtilities.BuildUpaRequest(doc.ToString());
        }

        internal IDeviceMessage BuildManageTransaction(TerminalManageBuilder builder) {
            var transType = builder.TransactionType;
            int requestId = builder.ReferenceNumber;
            if (requestId == default(int) && RequestIdProvider != null) {
                requestId = RequestIdProvider.GetRequestId();
            }

            var doc = new JsonDoc();
            doc.Set("message", "MSG");

            var baseRequest = doc.SubElement("data");
            baseRequest.Set("command", MapTransactionType(transType));
            baseRequest.Set("EcrId", builder.EcrId.ToString());
            baseRequest.Set("requestId", requestId);

            var txnData = baseRequest.SubElement("data");

            var txnParams = txnData.SubElement("params");

            var transaction = txnData.SubElement("transaction");
            transaction.Set("tranNo", StringUtils.PadLeft(builder.TerminalRefNumber, 4, '0'));

            return TerminalUtilities.BuildUpaRequest(doc.ToString());
        }

        internal override byte[] SerializeRequest(TerminalAuthBuilder builder) {
            return BuildProcessTransaction(builder).GetSendBuffer();
        }
        internal override byte[] SerializeRequest(TerminalManageBuilder builder) {
            return BuildManageTransaction(builder).GetSendBuffer();
        }
        internal override byte[] SerializeRequest(TerminalReportBuilder builder) {
            return BuildReportTransaction(builder).GetSendBuffer();
        }

        #endregion

        #region Transaction Commands
        internal int GetRequestId() {
            int requestId = default(int);
            if (requestId == default(int) && RequestIdProvider != null) {
                requestId = RequestIdProvider.GetRequestId();
            }
            return requestId;
        }

        internal TransactionResponse DoTransaction(IDeviceMessage request) {
            request.AwaitResponse = true;
            var response = _connector.Send(request);

            if (response == null) {
                return null;
            }

            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);

            return new TransactionResponse(jsonParse);
        }
        #endregion

        #region Private Methods
        private string MapTransactionType(TransactionType type, bool requestToken = false) {
            switch (type)
            {
                case TransactionType.Sale:
                    return UpaTransType.SALE_REDEEM;
                case TransactionType.Void:
                    return UpaTransType.Void;
                case TransactionType.Balance:
                    return UpaTransType.BalanceInquiry;
                case TransactionType.Refund:
                    return UpaTransType.Refund;
                case TransactionType.TipAdjust:
                    return UpaTransType.TipAdjust;
                case TransactionType.Verify:
                    return UpaTransType.CardVerify;
                case TransactionType.EodProcessing:
                    return UpaTransType.EodProcessing;
                case TransactionType.CancelTransaction:
                    return UpaTransType.CancelTransaction;
                case TransactionType.Reboot:
                    return UpaTransType.Reboot;
                case TransactionType.LineItemDisplay:
                    return UpaTransType.LineItemDisplay;
                default:
                    throw new UnsupportedTransactionException();
            }
        }

        private string MapReportType(TerminalReportType type) {
            switch (type)
            {
                case TerminalReportType.GetSAFReport:
                    return UpaTransType.GetSAFReport;
                case TerminalReportType.GetBatchReport:
                    return UpaTransType.GetBatchReport;
                default:
                    throw new UnsupportedTransactionException();
            }

        }

        protected string ToCurrencyString(decimal? dec) {
            if (!dec.HasValue) {
                return null;
            }
            return Regex.Replace(string.Format("{0:c}", dec), "[^0-9.]", "");
        }
        #endregion
    }
}
