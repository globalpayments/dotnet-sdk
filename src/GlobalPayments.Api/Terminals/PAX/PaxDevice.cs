using System;
using System.Collections.Generic;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Extensions;

namespace GlobalPayments.Api.Terminals.PAX {
    public delegate void MessageSentEventHandler(string message);

    internal abstract class DeviceController : IDisposable {
        protected ITerminalConfiguration _settings;
        protected IDeviceCommInterface _interface;

        internal DeviceController(ITerminalConfiguration settings) {
            settings.Validate();
            _settings = settings;
        }

        public byte[] Send(IDeviceMessage message) {
            return _interface?.Send(message);
        }

        internal abstract IDeviceInterface ConfigureInterface();

        internal abstract TerminalResponse ProcessTransaction(TerminalAuthBuilder builder);
        internal abstract TerminalResponse ManageTransaction(TerminalManageBuilder builder);

        public void Dispose() {
            _interface?.Disconnect();
        }
    }

    internal class PaxController : DeviceController {
        IDeviceInterface _device;

        public event MessageSentEventHandler OnMessageSent;
        public ConnectionModes ConnectionMode {
            get {
                if(_settings != null)
                    return _settings.ConnectionMode;
                return ConnectionModes.SERIAL;
            }
        }

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

            _interface.Connect();
            _interface.OnMessageSent += (message) => {
                OnMessageSent?.Invoke(message);
            };
        }

        #region overrides
        internal override TerminalResponse ProcessTransaction(TerminalAuthBuilder builder) {
            // create sub groups
            var amounts = new AmountRequest();
            var account = new AccountRequest();
            var avs = new AvsRequest();
            var trace = new TraceRequest {
                ReferenceNumber = builder.ReferenceNumber.ToString()
            };
            var cashier = new CashierSubGroup();
            var commercial = new CommercialRequest();
            var ecom = new EcomSubGroup();
            var extData = new ExtDataSubGroup();

            // amounts sub group
            amounts.TransactionAmount = "{0:c}".FormatWith(builder.Amount).ToNumeric();
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
                default:
                    throw new UnsupportedTransactionException();
            }
        }
        internal override TerminalResponse ManageTransaction(TerminalManageBuilder builder) {
            var amounts = new AmountRequest();
            var account = new AccountRequest();
            var trace = new TraceRequest {
                ReferenceNumber = builder.ReferenceNumber.ToString(),
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
                case TransactionType.Capture:
                    return PAX_TXN_TYPE.POSTAUTH;
                case TransactionType.Refund:
                    return PAX_TXN_TYPE.RETURN;
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
            var response = DoTransaction(PAX_MSG_ID.T04_DO_EBT, txnType, accounts, trace, cashier, new ExtDataSubGroup());
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

    public class PaxInterface : IDeviceInterface {
        private PaxController controller;
        public event MessageSentEventHandler OnMessageSent;
        
        internal PaxInterface(PaxController controller) {
            this.controller = controller;
            controller.OnMessageSent += (message) => {
                OnMessageSent?.Invoke(message);
            };
        }

        #region Administration Messages
        // A00 - INITIALIZE
        public InitializeResponse Initialize() {
            var response = controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A00_INITIALIZE));
            return new InitializeResponse(response);
        }

        // A14 - CANCEL
        public void Cancel() {
            if (controller.ConnectionMode == ConnectionModes.HTTP)
                throw new MessageException("The cancel command is not available in HTTP mode");

            controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A14_CANCEL));
        }

        // A16 - RESET
        public PaxDeviceResponse Reset() {
            var response = controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A16_RESET));
            return new PaxDeviceResponse(response, PAX_MSG_ID.A17_RSP_RESET);
        }

        // A26 - REBOOT
        public PaxDeviceResponse Reboot() {
            var response = controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A26_REBOOT));
            return new PaxDeviceResponse(response, PAX_MSG_ID.A27_RSP_REBOOT);
        }

        public PaxDeviceResponse DisableHostResponseBeep() {
            var response = controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A04_SET_VARIABLE,
                "00",
                ControlCodes.FS,
                "hostRspBeep",
                ControlCodes.FS,
                "N"
            ));
            return new PaxDeviceResponse(response, PAX_MSG_ID.A05_RSP_SET_VARIABLE);
        }
        #endregion

        #region Credit Methods
        public TerminalAuthBuilder CreditAuth(int referenceNumber, decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Auth, PaymentMethodType.Credit).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }
        
        public TerminalManageBuilder CreditCapture(int referenceNumber, decimal? amount = null) {
            return new TerminalManageBuilder(TransactionType.Capture, PaymentMethodType.Credit).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }

        //public TerminalManageBuilder CreditEdit(decimal? amount = null) {
        //    return new TerminalManageBuilder(TransactionType.Edit).WithAmount(amount);
        //}

        public TerminalAuthBuilder CreditRefund(int referenceNumber, decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.Credit).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }

        public TerminalAuthBuilder CreditSale(int referenceNumber, decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Credit).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }

        public TerminalAuthBuilder CreditVerify(int referenceNumber) {
            return new TerminalAuthBuilder(TransactionType.Verify, PaymentMethodType.Credit).WithReferenceNumber(referenceNumber);
        }

        public TerminalManageBuilder CreditVoid(int referenceNumber) {
            return new TerminalManageBuilder(TransactionType.Void, PaymentMethodType.Credit).WithReferenceNumber(referenceNumber);
        }
        #endregion

        #region Debit Methods
        public TerminalAuthBuilder DebitRefund(int referenceNumber, decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.Debit).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }

        public TerminalAuthBuilder DebitSale(int referenceNumber, decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Debit).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }
        #endregion

        #region EBT Methods
        #endregion

        #region Gift Methods
        public TerminalAuthBuilder GiftSale(int referenceNumber, decimal? amount = null) {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Gift).WithReferenceNumber(referenceNumber).WithAmount(amount).WithCurrency(CurrencyType.CURRENCY);
        }

        public TerminalAuthBuilder GiftAddValue(int referenceNumber) {
            return new TerminalAuthBuilder(TransactionType.AddValue, PaymentMethodType.Gift).WithReferenceNumber(referenceNumber).WithCurrency(CurrencyType.CURRENCY);
        }

        public TerminalManageBuilder GiftVoid(int referenceNumber) {
            return new TerminalManageBuilder(TransactionType.Void, PaymentMethodType.Gift).WithReferenceNumber(referenceNumber).WithCurrency(CurrencyType.CURRENCY);
        }

        public TerminalAuthBuilder GiftBalance(int referenceNumber) {
            return new TerminalAuthBuilder(TransactionType.Balance, PaymentMethodType.Gift).WithReferenceNumber(referenceNumber).WithCurrency(CurrencyType.CURRENCY);
        }
        #endregion

        #region Cash Methods
        #endregion

        #region Check Methods
        #endregion

        #region Batch Commands
        public BatchCloseResponse BatchClose() {
            var response = controller.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.B00_BATCH_CLOSE, DateTime.Now.ToString("YYYYMMDDhhmmss")));
            return new BatchCloseResponse(response);
        }
        #endregion

        #region Reporting Commands
        #endregion
    }
}
