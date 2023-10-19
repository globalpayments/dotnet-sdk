using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Utils;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace GlobalPayments.Api.Terminals.UPA {
    public class UpaController : DeviceController {
        internal override IDeviceInterface ConfigureInterface() {
            if (_interface == null) {
                _interface = new UpaInterface(this);
            }
            return _interface;
        }
        internal override IDeviceCommInterface ConfigureConnector() {
            switch (_settings.ConnectionMode) {
                case ConnectionModes.TCP_IP:
                    return new UpaTcpInterface(_settings);
                case ConnectionModes.HTTP:
                case ConnectionModes.SERIAL:
                case ConnectionModes.SSL_TCP:
                case ConnectionModes.MIC:
                    return new UpaMicInterface(_settings);
                default:
                    throw new ApiException("Connection method not implemented for the specified device.");
            }
        }

        internal UpaController(ITerminalConfiguration settings) : base(settings) { }

        #region overrides
        internal override ITerminalResponse ProcessTransaction(TerminalAuthBuilder builder) {
            var request = BuildProcessTransaction(builder);
            return DoTransaction(request);
        }

        internal override ITerminalResponse ManageTransaction(TerminalManageBuilder builder) {
            var request = BuildManageTransaction(builder);
            return DoTransaction(request);
        }

        internal override ITerminalReport ProcessReport(TerminalReportBuilder builder) {
            var response = Send(BuildReportTransaction(builder));

            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);

            switch (builder.ReportType) {
                case TerminalReportType.GetSAFReport:
                    return new SafReportResponse(jsonParse);
                case TerminalReportType.GetBatchReport:
                    return new BatchReportResponse(jsonParse);
                case TerminalReportType.GetOpenTabDetails:
                    return new OpenTabDetailsResponse(jsonParse);
                default: return null;
            }
        }

        private bool IsTokenRequestApplicable(TransactionType transactionType) {            
            switch (transactionType) {
                case TransactionType.Tokenize:
                case TransactionType.Refund:
                    return true;
            default:
                    return false;
            }            
        }

        internal IDeviceMessage BuildProcessTransaction(TerminalAuthBuilder builder) {
            var pmt = builder.PaymentMethodType;
            var transType = builder.TransactionType;
            var transModifier = builder.TransactionModifier;

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
            baseRequest.Set("command", MapTransactionType(transType, transModifier, builder.RequestMultiUseToken, builder.Gratuity));
            baseRequest.Set("EcrId", builder.EcrId.ToString());
            baseRequest.Set("requestId", requestId.ToString());

            if (transType != TransactionType.Balance) {
                var txnData = baseRequest.SubElement("data");

                var txnParams = txnData.SubElement("params");
                txnParams.Set("clerkId", builder.ClerkId);
                
                if (!IsTokenRequestApplicable(transType)) {
                    txnParams.Set("tokenRequest", builder.RequestMultiUseToken ? "1" : "0");
                }
                if (builder.PaymentMethod is CreditCardData) {
                    txnParams.Set("tokenValue", ((CreditCardData)builder.PaymentMethod).Token);
                }
                if (builder.RequestMultiUseToken && (transType == TransactionType.Sale || transType == TransactionType.Refund || transType == TransactionType.Verify
                    || transType == TransactionType.Auth)) {
                    if (builder.CardOnFileIndicator != null) {
                        txnParams.Set("cardOnFileIndicator", EnumConverter.GetMapping(Target.UPA, builder.CardOnFileIndicator));
                    }
                    txnParams.Set("cardBrandTransId", builder.CardBrandTransId);
                }
                txnParams.Set("lineItemLeft", builder.LineItemLeft);
                txnParams.Set("lineItemRight", builder.LineItemRight);
                if (transType == TransactionType.Auth)
                    txnParams.Set("invoiceNbr", builder.InvoiceNumber);
                if (builder.ShippingDate != DateTime.MinValue && builder.InvoiceNumber != null) {
                    txnParams.Set("directMktInvoiceNbr", builder.InvoiceNumber);
                    txnParams.Set("directMktShipMonth", builder.ShippingDate.Month.ToString("00"));
                    txnParams.Set("directMktShipDay", builder.ShippingDate.Day.ToString("00"));
                }

                if (transType != TransactionType.Verify && transType != TransactionType.Refund && transType != TransactionType.Tokenize) {
                    var transaction = txnData.SubElement("transaction");
                    if (transType == TransactionType.Auth) {
                        transaction.Set("amount", ToCurrencyString(builder.Amount));
                        transaction.Set("preAuthAmount", ToCurrencyString(builder.PreAuthAmount));
                    }
                    else {
                        transaction.Set("baseAmount", ToCurrencyString(builder.Amount));
                        transaction.Set("cashBackAmount", ToCurrencyString(builder.CashBackAmount));
                        transaction.Set("tipAmount", ToCurrencyString(builder.Gratuity));
                        transaction.Set("taxIndicator", builder.TaxExempt);
                        transaction.Set("invoiceNbr", builder.InvoiceNumber);
                        transaction.Set("processCPC", builder.ProcessCPC);
                        transaction.Set("taxAmount", ToCurrencyString(builder.TaxAmount));
                    }

                    transaction.Set("referenceNumber", builder.TerminalRefNumber);


                    transaction.Set("prescriptionAmount", ToCurrencyString(builder.PrescriptionAmount));
                    transaction.Set("clinicAmount", ToCurrencyString(builder.ClinicAmount));
                    transaction.Set("dentalAmount", ToCurrencyString(builder.DentalAmount));
                    transaction.Set("visionOpticalAmount", ToCurrencyString(builder.VisionOpticalAmount));
                    transaction.Set("cardAcquisition", EnumConverter.GetMapping(Target.UPA, builder.CardAcquisition));
                }

                if (transType == TransactionType.Refund) {
                    var transaction = txnData.SubElement("transaction");
                    transaction.Set("totalAmount", ToCurrencyString(builder.Amount));
                    transaction.Set("invoiceNbr", builder.InvoiceNumber);
                    transaction.Set("referenceNumber", builder.TerminalRefNumber);
                }              
            }

            return TerminalUtilities.BuildUpaRequest(doc);
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
            if (!(builder.SearchBuilder.EcrId == null)) baseRequest.Set("EcrId", builder.SearchBuilder.EcrId);
            baseRequest.Set("requestId", requestId.ToString());

            if (builder.ReportType == TerminalReportType.GetOpenTabDetails)
                return TerminalUtilities.BuildUpaRequest(doc.ToString());

            var txnData = baseRequest.SubElement("data");
            var txnParams = txnData.SubElement("params");
            txnParams.Set("reportOutput", builder.SearchBuilder.ReportOutput);
            txnParams.Set("batch", builder.SearchBuilder.Batch);

            return TerminalUtilities.BuildUpaRequest(doc.ToString());
        }

        internal IDeviceMessage BuildManageTransaction(TerminalManageBuilder builder) {
            var transType = builder.TransactionType;
            var transModifier = builder.TransactionModifier;
            int requestId = builder.ReferenceNumber;
            bool isTipAdjust = IsTipAdjust(transType, builder.Gratuity);

            if (requestId == default(int) && RequestIdProvider != null) {
                requestId = RequestIdProvider.GetRequestId();
            }

            var doc = new JsonDoc();
            doc.Set("message", "MSG");

            var baseRequest = doc.SubElement("data");
            // Possibly update the requestToken parameter in the future if necessary
            baseRequest.Set("command", MapTransactionType(transType, transModifier, false, builder.Gratuity));
            baseRequest.Set("EcrId", builder.EcrId.ToString());
            baseRequest.Set("requestId", requestId.ToString());

            var txnData = baseRequest.SubElement("data");

            var transaction = txnData.SubElement("transaction");
            if (isTipAdjust)
            {               
                transaction.Set("tranNo", builder.TerminalRefNumber);
                transaction.Set("tipAmount", ToCurrencyString(builder.Gratuity));
                transaction.Set("invoiceNbr", builder.InvoiceNumber);
            }
            else
            {
                transaction.Set("referenceNumber", builder.TransactionId ?? StringUtils.PadLeft(builder.TerminalRefNumber, 4, '0'));
                transaction.Set("amount", ToCurrencyString(builder.Amount));
                transaction.Set("taxAmount", ToCurrencyString(builder.TaxAmount));
                transaction.Set("tipAmount", ToCurrencyString(builder.Gratuity));
                transaction.Set("taxIndicator", builder.TaxExempt);
                transaction.Set("invoiceNbr", builder.InvoiceNumber);
                transaction.Set("processCPC", builder.ProcessCPC);
            }
            

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
        private string MapTransactionType(TransactionType type, TransactionModifier? modifier = null, bool requestToken = false, decimal? gratuity = null) {
            switch (type) {
                case TransactionType.Sale:
                    return UpaTransType.SALE_REDEEM;
                case TransactionType.Void:
                    return UpaTransType.Void;
                case TransactionType.Balance:
                    return UpaTransType.BalanceInquiry;
                case TransactionType.Refund:
                    return UpaTransType.Refund;
                case TransactionType.Edit:
                    if (IsTipAdjust(type, gratuity)) {
                        return UpaTransType.TipAdjust;
                    }
                    else {
                        throw new ArgumentException("A tip amount must be included for this transaction type.");
                    }
                case TransactionType.Verify:
                    return UpaTransType.CardVerify;
                case TransactionType.Tokenize:
                    return UpaTransType.Tokenize;
                case TransactionType.Auth:
                    return UpaTransType.PreAuth;
                case TransactionType.Delete:
                    if (modifier == TransactionModifier.DeletePreAuth) {
                        return UpaTransType.DeletePreAuth;
                    }
                    else {
                        throw new UnsupportedTransactionException();
                    }
                case TransactionType.Capture:
                    return UpaTransType.AuthCompletion;
                default:
                    throw new UnsupportedTransactionException();
            }
        }

        private string MapReportType(TerminalReportType type) {
            switch (type) {
                case TerminalReportType.GetSAFReport:
                    return UpaTransType.GetSAFReport;
                case TerminalReportType.GetBatchReport:
                    return UpaTransType.GetBatchReport;
                case TerminalReportType.GetOpenTabDetails:
                    return UpaTransType.GetOpenTabDetails;
                default:
                    throw new UnsupportedTransactionException();
            }

        }

        bool IsTipAdjust(TransactionType transType, decimal? gratuity) {
            return (transType == TransactionType.Edit &&
                (gratuity != null || gratuity > 0m));
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
