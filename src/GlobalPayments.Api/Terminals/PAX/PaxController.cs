using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Terminals.PAX {
    public class PaxController : DeviceController {
        internal override IDeviceInterface ConfigureInterface() {
            if (_interface == null) {
                _interface = new PaxInterface(this);
            }
            return _interface;
        }
        internal override IDeviceCommInterface ConfigureConnector() {
            switch (_settings.ConnectionMode) {
                case ConnectionModes.TCP_IP:
                    return new PaxTcpInterface(_settings);
                case ConnectionModes.HTTP:
                    return new PaxHttpInterface(_settings);
                case ConnectionModes.SERIAL:
                case ConnectionModes.SSL_TCP:
                default:
                    throw new NotImplementedException();
            }
        }

        internal PaxController(ITerminalConfiguration settings) : base(settings) {            
        }

        #region overrides
        internal override ITerminalResponse ProcessTransaction(TerminalAuthBuilder builder) {
            var request = BuildProcessTransaction(builder);
            switch (builder.PaymentMethodType) {
                case PaymentMethodType.Credit:
                    return DoCredit(request);
                case PaymentMethodType.Debit:
                    return DoDebit(request);
                case PaymentMethodType.Gift:
                    return DoGift(request);
                case PaymentMethodType.EBT:
                    return DoEBT(request);
                default:
                    throw new UnsupportedTransactionException();
            }
        }
        internal override ITerminalResponse ManageTransaction(TerminalManageBuilder builder) {
            var request = BuildManageTransaction(builder);
            switch (builder.PaymentMethodType) {
                case PaymentMethodType.Credit:
                    return DoCredit(request);
                case PaymentMethodType.Gift:
                    return DoGift(request);
                case PaymentMethodType.EBT:
                    return DoEBT(request);
                default:
                    throw new UnsupportedTransactionException();
            }
        }
        internal override ITerminalReport ProcessReport(TerminalReportBuilder builder) {
            var response = Send(BuildReportTransaction(builder));
            switch (builder.ReportType) {
                case TerminalReportType.LocalDetailReport:
                    return new LocalDetailReport(response);
                default: return null;
            }
        }

        internal IDeviceMessage BuildProcessTransaction(TerminalAuthBuilder builder) {
            int requestId = builder.ReferenceNumber;
            if (requestId == default(int) && RequestIdProvider != null) {
                requestId = RequestIdProvider.GetRequestId();
            }

            // create sub groups
            var amounts = new AmountRequest();
            var account = new AccountRequest();
            var avs = new AvsRequest();
            var trace = new TraceRequest {
                ReferenceNumber = requestId.ToString(),
                ClientTransactionId = builder.ClientTransactionId
            };
            var cashier = new CashierSubGroup();
            var commercial = new CommercialRequest();
            var ecom = new EcomSubGroup();
            var extData = new ExtDataSubGroup();

            // amounts sub group
            amounts.TransactionAmount = "{0:c}".FormatWith(builder.Amount).ToNumeric();
            amounts.CashBackAmount = "{0:c}".FormatWith(builder.CashBackAmount).ToNumeric();
            amounts.TipAmount = "{0:c}".FormatWith(builder.Gratuity).ToNumeric();
            amounts.TaxAmount = "{0:c}".FormatWith(builder.TaxAmount).ToNumeric();

            // account sub group
            if (builder.PaymentMethod != null) {
                if (builder.PaymentMethod is CreditCardData) {
                    var card = builder.PaymentMethod as CreditCardData;
                    if (string.IsNullOrEmpty(card.Token)) {
                        account.AccountNumber = card.Number;
                        account.EXPD = card.ShortExpiry;
                        if (builder.TransactionType != TransactionType.Verify && builder.TransactionType != TransactionType.Refund)
                            account.CvvCode = card.Cvn;
                    }
                    else extData[EXT_DATA.TOKEN] = card.Token;
                }
                else if (builder.PaymentMethod is TransactionReference) {
                    var reference = builder.PaymentMethod as TransactionReference;
                    if (!string.IsNullOrEmpty(reference.AuthCode))
                        trace.AuthCode = reference.AuthCode;
                    if (!string.IsNullOrEmpty(reference.TransactionId))
                        extData[EXT_DATA.HOST_REFERENCE_NUMBER] = reference.TransactionId;
                }
                else if (builder.PaymentMethod is GiftCard) {
                    var card = builder.PaymentMethod as GiftCard;
                    account.AccountNumber = card.Number;
                }
                else if (builder.PaymentMethod is EBTCardData) {
                    var card = builder.PaymentMethod as EBTCardData;
                }
            }
            if (builder.AllowDuplicates) account.DupOverrideFlag = "1";

            // Avs Sub Group
            if (builder.Address != null) {
                avs.ZipCode = builder.Address.PostalCode;
                avs.Address = builder.Address.StreetAddress1;
            }

            // Trace Sub Group
            trace.InvoiceNumber = builder.InvoiceNumber;

            // Commercial Group
            commercial.CustomerCode = builder.CustomerCode;
            commercial.PoNumber = builder.PoNumber;
            commercial.TaxExempt = builder.TaxExempt;
            commercial.TaxExemptId = builder.TaxExemptId;

            // Additional Info sub group
            if (builder.RequestMultiUseToken)
                extData[EXT_DATA.TOKEN_REQUEST] = "1";

            if (builder.SignatureCapture)
                extData[EXT_DATA.SIGNATURE_CAPTURE] = "1";

            if (builder.AutoSubstantiation != null) {
                extData[EXT_DATA.PASS_THROUGH_DATA] = BuildAutoSubPassThruData(builder.AutoSubstantiation);
            }

            string transType = MapTransactionType(builder.TransactionType, builder.RequestMultiUseToken);
            switch (builder.PaymentMethodType) {
                case PaymentMethodType.Credit:
                    return BuildCredit(transType, amounts, account, trace, avs, cashier, commercial, ecom, extData);
                case PaymentMethodType.Debit:
                    return BuildDebit(transType, amounts, account, trace, cashier, extData);
                case PaymentMethodType.Gift:
                    var messageId = builder.Currency == CurrencyType.CURRENCY ? PAX_MSG_ID.T06_DO_GIFT : PAX_MSG_ID.T08_DO_LOYALTY;
                    return BuildGift(messageId, transType, amounts, account, trace, cashier, extData);
                case PaymentMethodType.EBT:
                    if (builder.Currency != null)
                        account.EbtType = builder.Currency.ToString().Substring(0, 1);
                    return BuildEBT(transType, amounts, account, trace, cashier);
                default:
                    throw new UnsupportedTransactionException();
            }
        }

        private string BuildAutoSubPassThruData(AutoSubstantiation autoSubstantiation) {
            var autoSub = new List<string>();

            if (autoSubstantiation.TotalHealthcareAmount != default(decimal)) {
                autoSub.Add("HealthCare,{0}".FormatWith("{0:c}".FormatWith(autoSubstantiation.TotalHealthcareAmount).ToNumeric()));
            }
            if (autoSubstantiation.PrescriptionSubTotal != default(decimal)) {
                autoSub.Add("Rx,{0}".FormatWith("{0:c}".FormatWith(autoSubstantiation.PrescriptionSubTotal).ToNumeric()));
            }
            if (autoSubstantiation.VisionSubTotal != default(decimal)) {
                autoSub.Add("Vision,{0}".FormatWith("{0:c}".FormatWith(autoSubstantiation.VisionSubTotal).ToNumeric()));
            }
            if (autoSubstantiation.DentalSubTotal != default(decimal)) {
                autoSub.Add("Dental,{0}".FormatWith("{0:c}".FormatWith(autoSubstantiation.DentalSubTotal).ToNumeric()));
            }
            if (autoSubstantiation.ClinicSubTotal != default(decimal)) {
                autoSub.Add("Clinical,{0}".FormatWith("{0:c}".FormatWith(autoSubstantiation.ClinicSubTotal).ToNumeric()));
            }

            if (autoSub.Count == 0) {
                return string.Empty;
            }

            return "FSA:{0}".FormatWith(string.Join("|", autoSub));
        }

        internal IDeviceMessage BuildManageTransaction(TerminalManageBuilder builder) {
            int requestId = builder.ReferenceNumber;
            if (requestId == default(int) && RequestIdProvider != null) {
                requestId = RequestIdProvider.GetRequestId();
            }
            var amounts = new AmountRequest();
            var account = new AccountRequest();
            var trace = new TraceRequest {
                ReferenceNumber = requestId.ToString(),
                ClientTransactionId = builder.ClientTransactionId
            };
            var extData = new ExtDataSubGroup();

            // amounts
            if (builder.Amount.HasValue) {
                var _amount = builder.Amount;
                amounts.TransactionAmount = "{0:c}".FormatWith(_amount).ToNumeric();
            }

            // ADDING THIS HERE CAUSES IT TO FAIL SKIPPING IT HERE
            //if (gratuity.HasValue)
            //    amounts.TipAmount = "{0:c}".FormatWith(gratuity).ToNumeric();

            if (builder.PaymentMethod != null) {
                if (builder.PaymentMethod is TransactionReference) {
                    var reference = builder.PaymentMethod as TransactionReference;
                    if (!string.IsNullOrEmpty(reference.TransactionId))
                        extData[EXT_DATA.HOST_REFERENCE_NUMBER] = reference.TransactionId;
                }
                else if (builder.PaymentMethod is GiftCard) {
                    var card = builder.PaymentMethod as GiftCard;
                    account.AccountNumber = card.Number;
                }
            }

            string transType = MapTransactionType(builder.TransactionType);
            switch (builder.PaymentMethodType) {
                case PaymentMethodType.Credit:
                    return BuildCredit(transType, amounts, account, trace, new AvsRequest(), new CashierSubGroup(), new CommercialRequest(), new EcomSubGroup(), extData);
                case PaymentMethodType.Gift:
                    var messageId = builder.Currency == CurrencyType.CURRENCY ? PAX_MSG_ID.T06_DO_GIFT : PAX_MSG_ID.T08_DO_LOYALTY;
                    return BuildGift(messageId, transType, amounts, account, trace, new CashierSubGroup(), extData);
                case PaymentMethodType.EBT:
                    return BuildEBT(transType, amounts, account, trace, new CashierSubGroup());
                default:
                    throw new UnsupportedTransactionException();
            }
        }

        internal IDeviceMessage BuildReportTransaction(TerminalReportBuilder builder) {
            string messageId = MapReportType(builder.ReportType);

            IDeviceMessage request = null;
            switch (builder.ReportType) {
                case TerminalReportType.LocalDetailReport: {
                        var criteria = builder.SearchBuilder;

                        // additional data
                        var additionalData = new ExtDataSubGroup();
                        if (criteria.MerchantId.HasValue) {
                            additionalData[EXT_DATA.MERCHANT_ID] = criteria.MerchantId.ToString();
                        }
                        if (!string.IsNullOrEmpty(criteria.MerchantName)) {
                            additionalData[EXT_DATA.MERCHANT_NAME] = criteria.MerchantName;
                        }

                        request = TerminalUtilities.BuildRequest(
                            messageId,
                            "01",  // EDC TYPE SET TO ALL
                            ControlCodes.FS,
                            criteria.TransactionType.HasValue ? ((int)criteria.TransactionType.Value).ToString().PadLeft(2, '0') : string.Empty,
                            ControlCodes.FS,
                            criteria.CardType.HasValue ? ((int)criteria.CardType.Value).ToString().PadLeft(2, '0') : string.Empty,
                            ControlCodes.FS,
                            criteria.RecordNumber.HasValue ? criteria.RecordNumber.ToString() : string.Empty,
                            ControlCodes.FS,
                            criteria.TerminalReferenceNumber.HasValue ? criteria.TerminalReferenceNumber.ToString() : string.Empty,
                            ControlCodes.FS,
                            criteria.AuthCode ?? string.Empty,
                            ControlCodes.FS,
                            criteria.ReferenceNumber ?? string.Empty,
                            ControlCodes.FS,
                            additionalData
                        );
                    }
                    break;
                default: {
                        throw new UnsupportedTransactionException(string.Format("Unsupported report type: {0}", builder.ReportType.ToString()));
                    };
            }

            return request;
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

        private string MapTransactionType(TransactionType type, bool requestToken = false) {
            switch (type) {
                case TransactionType.AddValue:
                    return PAX_TXN_TYPE.ADD;
                case TransactionType.Auth:
                    return PAX_TXN_TYPE.AUTH;
                case TransactionType.Balance:
                    return PAX_TXN_TYPE.BALANCE;
                case TransactionType.BenefitWithdrawal:
                    return PAX_TXN_TYPE.WITHDRAWAL;
                case TransactionType.Capture:
                    return PAX_TXN_TYPE.POSTAUTH;
                case TransactionType.Refund:
                    return PAX_TXN_TYPE.RETURN;
                case TransactionType.Reversal:
                    return PAX_TXN_TYPE.REVERSAL;
                case TransactionType.Sale:
                    return PAX_TXN_TYPE.SALE_REDEEM;
                case TransactionType.Verify:
                    return requestToken ? PAX_TXN_TYPE.TOKENIZE : PAX_TXN_TYPE.VERIFY;
                case TransactionType.Void:
                    return PAX_TXN_TYPE.VOID;
                default:
                    throw new UnsupportedTransactionException();
            }
        }
        private string MapReportType(TerminalReportType type) {
            switch (type) {
                case TerminalReportType.LocalDetailReport:
                    return PAX_MSG_ID.R02_LOCAL_DETAIL_REPORT;
                default:
                    throw new UnsupportedTransactionException();
            }
        }
        #endregion

        #region Transaction Commands
        internal IDeviceMessage BuildRequest(string messageId, string txnType, params IRequestSubGroup[] subGroups) {
            var commands = new List<object> { txnType, ControlCodes.FS };
            if (subGroups.Length > 0) {
                commands.Add(subGroups[0]);
                for (int i = 1; i < subGroups.Length; i++) {
                    commands.Add(ControlCodes.FS);
                    commands.Add(subGroups[i]);
                }
            }

            return TerminalUtilities.BuildRequest(messageId, commands.ToArray());
        }

        internal IDeviceMessage BuildCredit(string txnType, AmountRequest amounts, AccountRequest accounts, TraceRequest trace, AvsRequest avs, CashierSubGroup cashier, CommercialRequest commercial, EcomSubGroup ecom, ExtDataSubGroup extData) {
            return BuildRequest(PAX_MSG_ID.T00_DO_CREDIT, txnType, amounts, accounts, trace, avs, cashier, commercial, ecom, extData);
        }
        internal CreditResponse DoCredit(IDeviceMessage request) {
            var response = _connector.Send(request);
            return new CreditResponse(response);
        }

        internal IDeviceMessage BuildDebit(string txnType, AmountRequest amounts, AccountRequest accounts, TraceRequest trace, CashierSubGroup cashier, ExtDataSubGroup extData) {
            return BuildRequest(PAX_MSG_ID.T02_DO_DEBIT, txnType, amounts, accounts, trace, cashier, extData);
        }
        internal DebitResponse DoDebit(IDeviceMessage request) {
            var response = _connector.Send(request);
            return new DebitResponse(response);
        }

        internal IDeviceMessage BuildEBT(string txnType, AmountRequest amounts, AccountRequest accounts, TraceRequest trace, CashierSubGroup cashier) {
            return BuildRequest(PAX_MSG_ID.T04_DO_EBT, txnType, amounts, accounts, trace, cashier, new ExtDataSubGroup());
        }
        internal EbtResponse DoEBT(IDeviceMessage request) {
            var response = _connector.Send(request);
            return new EbtResponse(response);
        }

        internal IDeviceMessage BuildGift(string messageId, string txnType, AmountRequest amounts, AccountRequest accounts, TraceRequest trace, CashierSubGroup cashier, ExtDataSubGroup extData = null) {
            return BuildRequest(messageId, txnType, amounts, accounts, trace, cashier, extData);
        }
        internal GiftResponse DoGift(IDeviceMessage request) {
            var response = _connector.Send(request);
            return new GiftResponse(response);
        }

        internal IDeviceMessage BuildCash(string txnType, AmountRequest amounts, TraceRequest trace, CashierSubGroup cashier) {
            return BuildRequest(PAX_MSG_ID.T10_DO_CASH, txnType, amounts, trace, cashier, new ExtDataSubGroup());
        }
        internal CashResponse DoCash(IDeviceMessage request) {
            var response = _connector.Send(request);
            return new CashResponse(response);
        }

        internal IDeviceMessage BuildCheck(string txnType, AmountRequest amounts, CheckSubGroup check, TraceRequest trace, CashierSubGroup cashier, ExtDataSubGroup extData) {
            return BuildRequest(PAX_MSG_ID.T12_DO_CHECK, txnType, amounts, check, trace, cashier, extData);
        }
        internal CheckSubResponse DoCheck(IDeviceMessage request) {
            var response = _connector.Send(request);
            return new CheckSubResponse(response);
        }
        #endregion
    }
}
