using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Terminals.INGENICO {
    public class IngenicoController : DeviceController {
        IDeviceInterface _device;

        public IngenicoController(ITerminalConfiguration settings) : base(settings) {
        }

        internal override IDeviceInterface ConfigureInterface() {
            if (_device == null) {
                _device = new IngenicoInterface(this);
            }
            return _device;
        }
        internal override IDeviceCommInterface ConfigureConnector() {
            switch (_settings.ConnectionMode) {
                case ConnectionModes.SERIAL:
                    return new IngenicoSerialInterface(_settings);
                case ConnectionModes.TCP_IP:
                case ConnectionModes.SSL_TCP:
                case ConnectionModes.HTTP:
                case ConnectionModes.TCP_IP_SERVER:
                    return new IngenicoTcpInterface(_settings);
                default:
                    throw new NotImplementedException();
            }
        }

        #region overrides
        internal override ITerminalResponse ManageTransaction(TerminalManageBuilder builder) {
            var request = BuildManageTransaction(builder);

            if (builder.TransactionType == TransactionType.Cancel) {
                return DoCancelRequest(request);
            }
            else if (builder.TransactionType == TransactionType.Reversal) {
                return DoReverseRequest(request);
            }
            else {
                return DoRequest(request);
            }
        }

        internal override ITerminalReport ProcessReport(TerminalReportBuilder builder) {
            var request = TerminalUtilities.BuildIngenicoRequest(INGENICO_REQ_CMD.RECEIPT.FormatWith(builder.ReceiptType), _settings.ConnectionMode);
            return ReportRequest(request);
        }

        internal override ITerminalResponse ProcessTransaction(TerminalAuthBuilder builder) {
            var request = BuildRequestMessage(builder);
            return DoRequest(request);
        }

        internal override byte[] SerializeRequest(TerminalAuthBuilder builder) {
            return BuildRequestMessage(builder).GetSendBuffer();
        }

        internal override byte[] SerializeRequest(TerminalManageBuilder builder) {
            throw new NotImplementedException();
        }

        internal override byte[] SerializeRequest(TerminalReportBuilder builder) {
            throw new NotImplementedException();
        }
        #endregion

        #region Methods
        internal IDeviceMessage BuildManageTransaction(TerminalManageBuilder builder) {
            var _refNumber = builder.ReferenceNumber;
            var _amount = ValidateAmount(builder.Amount);
            var _returnRep = 1;
            var _paymentMode = ValidatePaymentMode(builder.PaymentMode);
            var _paymentType = (int?)((IngenicoInterface)_device).paymentMethod ?? 0;
            var _currencyCode = (string.IsNullOrEmpty(builder.CurrencyCode) ? "826" : builder.CurrencyCode);
            var _privData = "EXT0100000";
            var _immediateAns = 0;
            var _forceOnline = 0;
            var _extendedData = "0000000000";

            // For Auth code value in extended data
            if (!string.IsNullOrEmpty(builder.AuthCode))
                _extendedData = ValidateExtendedData(builder.AuthCode, builder.ExtendedDataTags);
            else if (!builder.TransactionId.IsNull() && builder.TransactionType == TransactionType.Reversal)
                // For Reversal with Transaction Id value in extended data
                _extendedData = ValidateExtendedData(builder.TransactionId, ExtendedDataTags.TXN_COMMANDS_PARAMS);
            else
                _extendedData = ValidateExtendedData(builder.TransactionType.ToString(), ExtendedDataTags.TXN_COMMANDS);

            string message = string
                .Format("{0}{1}{2}{3}{4}{5}{6}A01{7}B01{8}{9}",
                builder.ReferenceNumber.ToString("00"),
                _amount?.ToString("00000000"),
                _returnRep,
                _paymentMode,
                _paymentType,
                _currencyCode,
                _privData,
                _immediateAns,
                _forceOnline,
                _extendedData
                );

            return TerminalUtilities.BuildIngenicoRequest(message, _settings.ConnectionMode);
        }

        internal IDeviceMessage BuildRequestMessage(TerminalAuthBuilder builder) {
            string message = string.Empty;
            if (!IsObjectNullOrEmpty(builder.ReportType))
                message = INGENICO_REQ_CMD.REPORT.FormatWith(builder.ReportType);
            else {
                var _referenceNumber = builder.ReferenceNumber;
                var _amount = builder.Amount;
                var _returnRep = 1;
                var _paymentMode = 0;
                var _paymentType = (int)((IngenicoInterface)_device).paymentMethod;
                var _currencyCode = "826";
                var _privateData = "EXT0100000";
                var _immediateAnswer = 0;
                var _forceOnline = 0;
                var _extendedData = "0000000000";

                var _cashbackAmount = builder.CashBackAmount;
                var _authCode = builder.AuthCode;
                string tableId = builder.TableNumber;

                // Validations
                _amount = ValidateAmount(_amount);
                _paymentMode = ValidatePaymentMode(builder.PaymentMode);
                _currencyCode = ValidateCurrency((string.IsNullOrEmpty(builder.CurrencyCode) ? _currencyCode : builder.CurrencyCode));

                if (!string.IsNullOrEmpty(tableId)) {
                    bool validateTableId = ValidateTableReference(tableId);
                    if (validateTableId)
                        _extendedData = ValidateExtendedData(tableId, builder.ExtendedDataTags);
                }

                if (!IsObjectNullOrEmpty(_cashbackAmount))
                    _extendedData = ValidateExtendedData(_cashbackAmount.ToString(), builder.ExtendedDataTags);
                else if (!string.IsNullOrEmpty(_authCode))
                    _extendedData = ValidateExtendedData(_authCode, builder.ExtendedDataTags);



                message = string.Format("{0}{1}{2}{3}{4}{5}{6}A01{7}B01{8}{9}",
                    _referenceNumber.ToString("00"),
                    _amount?.ToString("00000000"),
                    _returnRep,
                    _paymentMode,
                    _paymentType,
                    _currencyCode,
                    _privateData,
                    _immediateAnswer,
                    _forceOnline,
                    _extendedData);
            }

            return TerminalUtilities.BuildIngenicoRequest(message, _settings.ConnectionMode);
        }

        #region Validations
        private static bool IsObjectNullOrEmpty(object value) {
            bool response = false;
            if (value.IsNull() || string.IsNullOrWhiteSpace(value.ToString()))
                response = true;
            else response = false;

            return response;
        }
        private static bool ValidateTableReference(string value) {
            bool response = false;
            if (!string.IsNullOrEmpty(value) && value.Length <= 8)
                response = true;
            else throw new BuilderException("Table number must not be less than or equal 0 or greater than 8 numerics.");

            return response;
        }

        private static int ValidatePaymentMode(PaymentMode _paymentMode) {
            if (IsObjectNullOrEmpty(_paymentMode)) {
                _paymentMode = PaymentMode.APPLICATION;
            }

            return (int)_paymentMode;
        }

        private static string ValidateExtendedData(string value, ExtendedDataTags tags) {
            string extendedData = string.Empty;
            switch (tags) {
                case ExtendedDataTags.CASHB:
                    decimal? cashbackAmount = Convert.ToDecimal(value);
                    if (cashbackAmount > 0m && cashbackAmount < 1000000m)
                        cashbackAmount *= 100;
                    else if (cashbackAmount <= 0m)
                        throw new BuilderException("Cashback Amount must not be in less than or equal 0 value.");
                    else throw new BuilderException("Cashback Amount exceed.");

                    extendedData = "CASHB={0};".FormatWith(Convert.ToInt64(Math.Round(cashbackAmount.Value, MidpointRounding.AwayFromZero)));
                    break;
                case ExtendedDataTags.AUTHCODE:
                    extendedData = "AUTHCODE={0}".FormatWith(value);
                    break;
                case ExtendedDataTags.TABLE_NUMBER:
                    extendedData = "CMD=ID{0}".FormatWith(value);
                    break;
                case ExtendedDataTags.TXN_COMMANDS:
                    var transType = (TransactionType)Enum.Parse(typeof(TransactionType), value, true);
                    switch (transType) {
                        case TransactionType.Cancel:
                            extendedData = INGENICO_REQ_CMD.CANCEL;
                            break;
                        case TransactionType.Duplicate:
                            extendedData = INGENICO_REQ_CMD.DUPLICATE;
                            break;
                        case TransactionType.Reversal:
                            extendedData = INGENICO_REQ_CMD.REVERSE;
                            break;
                    }
                    break;
                case ExtendedDataTags.TXN_COMMANDS_PARAMS:
                    extendedData = INGENICO_REQ_CMD.REVERSE_WITH_ID.FormatWith(value);
                    break;
            }

            return extendedData;
        }

        private static string ValidateCurrency(string _currencyCode) {
            if (!string.IsNullOrWhiteSpace(_currencyCode)) {
                _currencyCode = _currencyCode.PadLeft(3, '0');
            }
            else _currencyCode = "826";

            return _currencyCode;
        }

        private decimal? ValidateAmount(decimal? _amount) {
            if (_amount == null)
                throw new BuilderException("Amount can not be null.");
            else if (_amount > 0 && _amount < 1000000m)
                _amount *= 100;
            else if (_amount >= 1000000m)
                throw new BuilderException("Amount exceed.");
            else
                throw new BuilderException("Invalid input amount.");
            return _amount;
        }

        #endregion

        public ITerminalConfiguration GetConfiguration() {
            return _settings;
        }
        #endregion

        #region Report Request
        internal IngenicoTerminalReportResponse ReportRequest(IDeviceMessage request) {
            var send = Send(request);
            return new IngenicoTerminalReportResponse(send);
        }
        #endregion

        #region Request
        internal IngenicoTerminalResponse DoRequest(IDeviceMessage request) {
            var response = Send(request);
            return new IngenicoTerminalResponse(response);
        }

        private CancelResponse DoCancelRequest(IDeviceMessage request) {
            var response = Send(request);
            return new CancelResponse(response);
        }

        private ReverseResponse DoReverseRequest(IDeviceMessage request) {
            var response = Send(request);
            return new ReverseResponse(response);
        }
        #endregion
    }
}