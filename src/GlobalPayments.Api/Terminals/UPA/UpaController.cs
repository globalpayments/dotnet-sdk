using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.Enums;
using GlobalPayments.Api.Terminals.UPA.Responses;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
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
            CheckRequest(request.GetRequestBuilder() as JsonDoc);
            return DoTransaction(request);
        }

        internal override ITerminalResponse ManageTransaction(TerminalManageBuilder builder) {
            var request = BuildManageTransaction(builder);            
            CheckRequest(request.GetRequestBuilder() as JsonDoc);
            return DoTransaction(request);
        }

        private void CheckRequest(JsonDoc request) {
            var command = request.Get("data").GetValue<string>("command") ?? null;
            switch (command) {
                case UpaTransType.UpdateLodgingDetails:
                    _interface.Validations.SetMandatoryParams(new List<string>() { "referenceNumber", "amount" });
                    break;
                case UpaTransType.ContinueEmvTransaction:
                    _interface.Validations.SetMandatoryParams(new List<string>() { "merchantDecision", "quickChip", "totalAmount" });
                    break;
                case UpaTransType.CompleteEMVTransaction:
                    _interface.Validations.SetMandatoryParams(new List<string>() { "quickChip", "hostDecision" });
                    break;
                case UpaTransType.ProcessCardTransaction:
                    _interface.Validations.SetMandatoryParams(new List<string>() { "acquisitionTypes", "merchantDecision", "quickChip", "totalAmount", "transactionType" });
                    break;
                case UpaTransType.ContinueCardTransaction:
                    _interface.Validations.SetMandatoryParams(new List<string>() { "merchantDecision", "totalAmount" });
                    break;
                default:
                    _interface.Validations.SetMandatoryParams(new List<string>());
                    break;
            }

            var missingParams = new List<string>();
            _interface.Validations.Validate(request, out missingParams);
            if (missingParams.Count > 0) {
                throw new ArgumentException($"Mandatory params missing: {String.Join(",",missingParams)}");
            }
        }

        internal override ITerminalReport ProcessReport(TerminalReportBuilder builder) {
            var response = Send(BuildReportTransaction(builder));

            string jsonObject = Encoding.UTF8.GetString(response);
            var jsonParse = JsonDoc.Parse(jsonObject);

            switch (builder.ReportType) {
                case TerminalReportType.GetSAFReport:
                    return new SafReportResponse(jsonParse);
                case TerminalReportType.GetBatchReport:                    
                case TerminalReportType.GetBatchDetails:
                    return new BatchReportResponse(jsonParse);
                case TerminalReportType.GetOpenTabDetails:
                    return new OpenTabDetailsResponse(jsonParse);
                case TerminalReportType.FindBatches:
                    return new BatchList(jsonParse);
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
            baseRequest.Set("command", MapTransactionType(transType, transModifier, builder.RequestMultiUseToken, builder.Gratuity, builder.PaymentMethod));
            baseRequest.Set("EcrId", builder.EcrId.ToString());
            baseRequest.Set("requestId", requestId.ToString());

            if (transType != TransactionType.Balance) {
                var txnData = baseRequest.SubElement("data");

                ///Params
                #region Params                
                var txnParams = new JsonDoc();               
                txnParams.Set("clerkId", builder.ClerkId);

                //if (!IsTokenRequestApplicable(transType)) {
                if (builder.RequestMultiUseToken) {
                    txnParams.Set("tokenRequest", builder.RequestMultiUseToken ? "1" : "0");
                }
                //}
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

                txnParams.Set("merchantDecision", EnumConverter.GetDescription(builder.MerchantDecision));
                txnParams.Set("languageCode", builder.Language);

                if(builder.AcquisitionTypes != null) {
                    List<string> acquisitionStrings = new List<string>();
                    builder.AcquisitionTypes.ForEach(item => acquisitionStrings.Add(EnumConverter.GetMapping(Target.UPA, item)));
                    
                    txnParams.Set("acquisitionTypes", string.Join("|", acquisitionStrings));
                }
                if (txnParams.HasKeys()){
                    txnData.SetArrange("params", txnParams);
                }
                #endregion

                ///Processing Indicator
                #region Processing Indicator

                var txnProcessing = new JsonDoc();
                txnProcessing.Set("quickChip", builder.IsQuickChip.HasValue ? builder.IsQuickChip.Value ? "Y" : "N" : null)
                    .Set("checkLuhn", builder.HasCheckLuhn.HasValue ? builder.HasCheckLuhn.Value ? "Y" : "N" : null)
                    .Set("securityCode", builder.HasSecurityCode.HasValue ? builder.HasSecurityCode.Value ? "Y" : "N" : null);
                if (txnProcessing.HasKeys()) {
                    txnData.SetArrange("processingIndicators", txnProcessing);
                }
                
                #endregion

                ///Transaction
                #region Transaction
                if (transType != TransactionType.Verify && transType != TransactionType.Refund && transType != TransactionType.Tokenize 
                    && builder.TransactionModifier != TransactionModifier.CompleteTransaction) {
                    var transaction = txnData.SubElement("transaction");
                    if (transType == TransactionType.Auth) {
                        transaction.Set("amount", ToCurrencyString(builder.Amount));
                        transaction.Set("preAuthAmount", ToCurrencyString(builder.PreAuthAmount));
                    }
                    else if(transType == TransactionType.Sale) {
                        if (builder.TransactionModifier == TransactionModifier.ProcessTransaction) {
                            transaction.Set("totalAmount", ToCurrencyString(builder.Amount));
                        }
                        transaction.Set("baseAmount", ToCurrencyString(builder.Amount));
                        transaction.Set("tipAmount", ToCurrencyString(builder.Gratuity));
                    }
                    else if (transType == TransactionType.Confirm) { 
                        if(builder.TransactionModifier == TransactionModifier.ContinueCardTransaction || 
                            builder.TransactionModifier == TransactionModifier.ContinueEMVTransaction) {
                            transaction.Set("totalAmount", ToCurrencyString(builder.Amount));
                        }
                    }
                    else
                    {
                        transaction.Set("baseAmount", ToCurrencyString(builder.Amount));                        
                        transaction.Set("tipAmount", ToCurrencyString(builder.Gratuity));
                        transaction.Set("taxIndicator", builder.TaxExempt);
                        transaction.Set("invoiceNbr", builder.InvoiceNumber);
                        transaction.Set("processCPC", builder.ProcessCPC.HasValue ? builder.ProcessCPC.Value ? "1" : "0" : null);
                        transaction.Set("taxAmount", ToCurrencyString(builder.TaxAmount));
                    }

                    transaction.Set("cashBackAmount", ToCurrencyString(builder.CashBackAmount));
                    transaction.Set("referenceNumber", builder.TerminalRefNumber);
                    transaction.Set("prescriptionAmount", ToCurrencyString(builder.PrescriptionAmount));
                    transaction.Set("clinicAmount", ToCurrencyString(builder.ClinicAmount));
                    transaction.Set("dentalAmount", ToCurrencyString(builder.DentalAmount));
                    transaction.Set("visionOpticalAmount", ToCurrencyString(builder.VisionOpticalAmount));
                    transaction.Set("cardAcquisition", EnumConverter.GetMapping(Target.UPA, builder.CardAcquisition));

                    if (builder.TransactionModifier == TransactionModifier.ProcessTransaction) {
                        transaction.Set("transactionType", transType.ToString());
                    }
                    if (builder.TransactionDate != null) {
                        transaction.Set("tranDate", builder.TransactionDate.Value.ToString("MMddyyyy"))
                            .Set("tranTime", builder.TransactionDate.Value.ToString("H:m:s"));            
                    }
                }

                if (transType == TransactionType.Refund) {
                    var transaction = txnData.SubElement("transaction");
                    transaction.Set("totalAmount", ToCurrencyString(builder.Amount));
                    transaction.Set("invoiceNbr", builder.InvoiceNumber);
                    transaction.Set("referenceNumber", builder.TerminalRefNumber);
                }
                #endregion

                ///HostField
                #region HostField                
                if(builder.HostData != null) {
                    var hostData = new JsonDoc();
                    hostData.Set("hostDecision", EnumConverter.GetDescription(builder.HostData.HostDecision))
                        .Set("issuerScripts", builder.HostData.IssuerScripts)
                        .Set("issuerAuthData", builder.HostData.IssuerAuthData);
                    if (hostData.HasKeys()) {
                        txnData.SetArrange("host", hostData);
                    }
                }
                #endregion
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
            baseRequest.Set("EcrId", builder.SearchBuilder.EcrId ??  _interface.EcrId ?? null );
            baseRequest.Set("requestId", requestId.ToString());

            if (builder.ReportType == TerminalReportType.GetOpenTabDetails)
                return TerminalUtilities.BuildUpaRequest(doc.ToString());

            var txnParams = new JsonDoc().Set("reportOutput", builder.SearchBuilder.ReportOutput)
                            .Set("batch", builder.SearchBuilder.Batch);

            if (txnParams.HasKeys()) {
                var txnData = baseRequest.SubElement("data");
                txnData.SetArrange("params", txnParams);
            }

            return TerminalUtilities.BuildUpaRequest(doc.ToString());
        }

        internal IDeviceMessage BuildManageTransaction(TerminalManageBuilder builder) {
            var transType = builder.TransactionType;
            var transModifier = builder.TransactionModifier;
            int intDefault;
            int requestId = builder.ReferenceNumber == 0 ? int.TryParse(builder.TerminalRefNumber, out intDefault) ? intDefault : default(int) : 0;
            bool isTipAdjust = IsTipAdjust(transType, builder.Gratuity);

            if (requestId == default(int) && RequestIdProvider != null) {
                requestId = RequestIdProvider.GetRequestId();
            }

            var doc = new JsonDoc();
            doc.Set("message", "MSG");

            var baseRequest = doc.SubElement("data");
            // Possibly update the requestToken parameter in the future if necessary
            baseRequest.Set("command", MapTransactionType(transType, transModifier, false, builder.Gratuity));
            baseRequest.Set("EcrId", builder.EcrId?.ToString());
            baseRequest.Set("requestId", requestId.ToString());

            var txnData = baseRequest.SubElement("data");

            var transaction = txnData.SubElement("transaction");

            SetTransactionParams(builder, ref transaction);

            if(builder.LodgingData != null) {
                var lodging = txnData.SubElement("lodging");
                SetLodgingFields(builder, ref lodging);
            }

            return TerminalUtilities.BuildUpaRequest(doc.ToString());
        }

        private void SetLodgingFields(TerminalManageBuilder builder, ref JsonDoc lodging) {
            var chargeTypes = new int[10];
            foreach (var item in builder.LodgingData.ExtraChargeTypes) {
                chargeTypes[((int)item) - 1] = 1;
            }
            lodging.Set("folioNumber", builder.LodgingData.FolioNumber?.ToString() ?? null)
                .Set("extraChargeTypes", chargeTypes)
                .Set("extraChargeTotal", ToCurrencyString(builder.LodgingData.ExtraChargeTotal));
        }

        private void SetTransactionParams(TerminalManageBuilder builder, ref JsonDoc transaction)
        {            
            if (builder.Amount != null && builder.Amount > 0)
            {
                switch (builder.TransactionType) {
                    case TransactionType.Refund:
                            transaction.Set("totalAmount", ToCurrencyString(builder.Amount));
                        break;
                    case TransactionType.Auth:
                    case TransactionType.Capture:
                            transaction.Set("amount", ToCurrencyString(builder.Amount));
                        break;
                    case TransactionType.Delete:
                        if (builder.TransactionModifier == TransactionModifier.DeletePreAuth) {
                                transaction.Set("preAuthAmount", ToCurrencyString(builder.Amount));
                        }
                        break;
                    case TransactionType.Reversal:
                            transaction.Set("authorizedAmount", ToCurrencyString(builder.Amount));
                        break;
                    case TransactionType.Edit:
                        if (builder.TransactionModifier == TransactionModifier.UpdateLodgingDetails) {
                                transaction.Set("amount", ToCurrencyString(builder.Amount));
                        }
                        break;
                        default:
                            transaction.Set("baseAmount", ToCurrencyString(builder.Amount));
                        break;
                }
            }
            transaction.Set("tipAmount", ToCurrencyString(builder.Gratuity));
            transaction.Set("cashBackAmount", ToCurrencyString(builder.CashBackAmount));
            transaction.Set("taxAmount", ToCurrencyString(builder.TaxAmount));
            transaction.Set("invoiceNbr", builder.InvoiceNumber ?? null);
            transaction.Set("tranNo", StringUtils.PadLeft(builder.TerminalRefNumber, 4, '0'));//builder.TerminalRefNumber ?? null);
            transaction.Set("referenceNumber", !string.IsNullOrEmpty(builder.TransactionId) ? builder.TransactionId :
                !string.IsNullOrEmpty(builder.TerminalRefNumber) ? StringUtils.PadLeft(builder.TerminalRefNumber, 4, '0') : null);
            transaction.Set("taxIndicator", builder.TaxExempt ?? null);
            transaction.Set("processCPC", builder.ProcessCPC ?? null);
            transaction.Set("purchaseOrder", builder.OrderId ?? null);
            transaction.Set("clerkId", builder.ClerkId ?? null);

            if((builder.TransactionType == TransactionType.Reversal || builder.TransactionModifier == TransactionModifier.UpdateTaxDetail) 
                && transaction.Has("referenceNumber")) {
                transaction.Remove("referenceNumber");
            }
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
        private string MapTransactionType(TransactionType type, TransactionModifier? modifier = null, bool requestToken = false, decimal? gratuity = null, IPaymentMethod paymentMethod = null) {
            switch (type) {
                case TransactionType.Sale:
                    switch (modifier) {
                        case TransactionModifier.ProcessTransaction:
                            return UpaTransType.ProcessCardTransaction;                        
                        default:
                            if (paymentMethod != null && (paymentMethod is CreditCardData) && 
                                ((CreditCardData)paymentMethod).EntryMethod == ManualEntryMethod.Mail) {
                                return UpaTransType.MailOrder;
                            }
                            if (paymentMethod != null && (paymentMethod is CreditCardData) &&
                               ((CreditCardData)paymentMethod).EntryMethod == ManualEntryMethod.Phone) {
                                return UpaTransType.ForceSale;
                            }
                            return UpaTransType.SALE_REDEEM;
                    }
                    
                case TransactionType.Void:
                    return UpaTransType.Void;
                case TransactionType.Balance:
                    return UpaTransType.BalanceInquiry;
                case TransactionType.Refund:
                    switch (modifier) {
                        case TransactionModifier.ProcessTransaction:
                            return UpaTransType.ProcessCardTransaction;
                        default:
                            return UpaTransType.Refund;
                    }                    
                case TransactionType.Edit:
                    switch (modifier) {
                        case TransactionModifier.UpdateTaxDetail:
                            return UpaTransType.UpdateTaxInfo;
                        case TransactionModifier.UpdateLodgingDetails:
                            return UpaTransType.UpdateLodgingDetails;
                        default:
                            break;
                    }
                    if (IsTipAdjust(type, gratuity)) {
                        return UpaTransType.TipAdjust;
                    }
                    else {
                        throw new ArgumentException("A tip amount must be included for this transaction type.");
                    }
                case TransactionType.Verify:
                    return UpaTransType.CardVerify;
                case TransactionType.Reversal:
                    return UpaTransType.Reversal;
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
                case TransactionType.Confirm:
                    switch (modifier) {
                        case TransactionModifier.ContinueEMVTransaction:
                            return UpaTransType.ContinueEmvTransaction;
                        case TransactionModifier.ContinueCardTransaction:
                            return UpaTransType.ContinueCardTransaction;
                        default:
                            return UpaTransType.CompleteEMVTransaction;                            
                    }
                    
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
                case TerminalReportType.GetBatchDetails:
                    return UpaTransType.GetBatchDetails;
                case TerminalReportType.GetOpenTabDetails:
                    return UpaTransType.GetOpenTabDetails;
                case TerminalReportType.FindBatches:
                    return UpaTransType.AvailableBatches;
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
