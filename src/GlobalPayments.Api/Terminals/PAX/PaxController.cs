using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Terminals.PAX {
    internal class PaxController : DeviceController {
        IDeviceInterface _device;

        public event MessageSentEventHandler OnMessageSent;

        internal override IDeviceInterface ConfigureInterface() {
            if (_device == null) {
                _device = new PaxInterface(this);
            }
            return _device;
        }

        internal PaxController(ITerminalConfiguration settings) : base(settings) {
            switch (_settings.ConnectionMode) {
                case ConnectionModes.TCP_IP:
                    _interface = new PaxTcpInterface(settings);
                    break;
                case ConnectionModes.HTTP:
                    _interface = new PaxHttpInterface(settings);
                    break;
                case ConnectionModes.SERIAL:
                case ConnectionModes.SSL_TCP:
                    throw new NotImplementedException();
            }

            //_interface.Connect();
            _interface.OnMessageSent += (message) => {
                OnMessageSent?.Invoke(message);
            };
        }

        #region overrides
        internal override TerminalResponse ProcessTransaction(TerminalAuthBuilder builder) {
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
                        account.EXPD = "{0}{1}".FormatWith(card.ExpMonth, card.ExpYear);
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

            string transType = MapTransactionType(builder.TransactionType, builder.RequestMultiUseToken);
            switch (builder.PaymentMethodType) {
                case PaymentMethodType.Credit:
                    return DoCredit(transType, amounts, account, trace, avs, cashier, commercial, ecom, extData);
                case PaymentMethodType.Debit:
                    return DoDebit(transType, amounts, account, trace, cashier, extData);
                case PaymentMethodType.Gift:
                    var messageId = builder.Currency == CurrencyType.CURRENCY ? PAX_MSG_ID.T06_DO_GIFT : PAX_MSG_ID.T08_DO_LOYALTY;
                    return DoGift(messageId, transType, amounts, account, trace, cashier, extData);
                case PaymentMethodType.EBT:
                    if (builder.Currency != null)
                        account.EbtType = builder.Currency.ToString().Substring(0, 1);
                    return DoEBT(transType, amounts, account, trace, cashier);
                default:
                    throw new UnsupportedTransactionException();
            }
        }
        internal override TerminalResponse ManageTransaction(TerminalManageBuilder builder) {
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
                    return DoCredit(transType, amounts, account, trace, new AvsRequest(), new CashierSubGroup(), new CommercialRequest(), new EcomSubGroup(), extData);
                case PaymentMethodType.Gift:
                    var messageId = builder.Currency == CurrencyType.CURRENCY ? PAX_MSG_ID.T06_DO_GIFT : PAX_MSG_ID.T08_DO_LOYALTY;
                    return DoGift(messageId, transType, amounts, account, trace, new CashierSubGroup(), extData);
                case PaymentMethodType.EBT:
                    return DoEBT(transType, amounts, account, trace, new CashierSubGroup());
                default:
                    throw new UnsupportedTransactionException();
            }
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
        #endregion

        #region Transaction Commands
        internal byte[] DoTransaction(string messageId, string txnType, params IRequestSubGroup[] subGroups) {
            var commands = new List<object> { txnType, ControlCodes.FS };
            if (subGroups.Length > 0) {
                commands.Add(subGroups[0]);
                for (int i = 1; i < subGroups.Length; i++) {
                    commands.Add(ControlCodes.FS);
                    commands.Add(subGroups[i]);
                }
            }

            var message = TerminalUtilities.BuildRequest(messageId, commands.ToArray());
            return _interface.Send(message);
        }

        internal CreditResponse DoCredit(string txnType, AmountRequest amounts, AccountRequest accounts, TraceRequest trace, AvsRequest avs, CashierSubGroup cashier, CommercialRequest commercial, EcomSubGroup ecom, ExtDataSubGroup extData) {
            var response = DoTransaction(PAX_MSG_ID.T00_DO_CREDIT, txnType, amounts, accounts, trace, avs, cashier, commercial, ecom, extData);
            return new CreditResponse(response);
        }

        internal DebitResponse DoDebit(string txnType, AmountRequest amounts, AccountRequest accounts, TraceRequest trace, CashierSubGroup cashier, ExtDataSubGroup extData) {
            var response = DoTransaction(PAX_MSG_ID.T02_DO_DEBIT, txnType, amounts, accounts, trace, cashier, extData);
            return new DebitResponse(response);
        }

        internal EbtResponse DoEBT(string txnType, AmountRequest amounts, AccountRequest accounts, TraceRequest trace, CashierSubGroup cashier) {
            var response = DoTransaction(PAX_MSG_ID.T04_DO_EBT, txnType, amounts, accounts, trace, cashier, new ExtDataSubGroup());
            return new EbtResponse(response);
        }

        internal GiftResponse DoGift(string messageId, string txnType, AmountRequest amounts, AccountRequest accounts, TraceRequest trace, CashierSubGroup cashier, ExtDataSubGroup extData = null) {
            var response = DoTransaction(messageId, txnType, amounts, accounts, trace, cashier, extData);
            return new GiftResponse(response);
        }

        internal CashResponse DoCash(string txnType, AmountRequest amounts, TraceRequest trace, CashierSubGroup cashier) {
            var response = DoTransaction(PAX_MSG_ID.T10_DO_CASH, txnType, amounts, trace, cashier, new ExtDataSubGroup());
            return new CashResponse(response);
        }

        internal CheckSubResponse DoCheck(string txnType, AmountRequest amounts, CheckSubGroup check, TraceRequest trace, CashierSubGroup cashier, ExtDataSubGroup extData) {
            var response = DoTransaction(PAX_MSG_ID.T12_DO_CHECK, txnType, amounts, check, trace, cashier, extData);
            return new CheckSubResponse(response);
        }
        #endregion
    }
}
