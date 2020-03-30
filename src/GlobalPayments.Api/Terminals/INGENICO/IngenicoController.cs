using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Utils;
using System;
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
            IDeviceMessage request = BuildManageTransaction(builder);

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
            IDeviceMessage request = TerminalUtilities.BuildIngenicoRequest(INGENICO_REQ_CMD.RECEIPT.FormatWith(builder.ReceiptType), _settings.ConnectionMode);
            return ReportRequest(request);
        }

        internal override ITerminalResponse ProcessTransaction(TerminalAuthBuilder builder) {
            IDeviceMessage request = BuildProcessTransaction(builder);
            return DoRequest(request);
        }

        internal override byte[] SerializeRequest(TerminalAuthBuilder builder) {
            return BuildProcessTransaction(builder).GetSendBuffer();
        }

        internal override byte[] SerializeRequest(TerminalManageBuilder builder) {
            return BuildManageTransaction(builder).GetSendBuffer();
        }

        internal override byte[] SerializeRequest(TerminalReportBuilder builder) {
            throw new NotImplementedException();
        }
        #endregion

        #region Methods
        internal IDeviceMessage BuildManageTransaction(TerminalManageBuilder builder) {
            int refNumber = builder.ReferenceNumber;
            decimal? amount = ValidateAmount(builder.Amount);
            int returnRep = 1;
            int paymentMode = ValidatePaymentMode(builder.PaymentMode);
            int? paymentType = (int?)((IngenicoInterface)_device).paymentMethod ?? 0;
            string currencyCode = (string.IsNullOrEmpty(builder.CurrencyCode) ? "826" : builder.CurrencyCode);
            string privData = "EXT0100000";
            int immediateAns = 0;
            int forceOnline = 0;
            string extendedData = "0000000000";

            // Validation for Authcode
            if (!string.IsNullOrEmpty(builder.AuthCode)) {
                extendedData = INGENICO_REQ_CMD.AUTHCODE.FormatWith(builder.AuthCode); 
            }
            // Validation for Reversal with Transaction Id value in Extended data
            else if (!builder.TransactionId.IsNull() && builder.TransactionType == TransactionType.Reversal) {
                extendedData = INGENICO_REQ_CMD.REVERSE_WITH_ID.FormatWith(builder.TransactionId);
            }
            // TODO: Remove Cancel, Duplicate, Reverse here this should be on Interface
            // Temporary for CANCEL.
            else if (builder.TransactionType == TransactionType.Cancel) {
                extendedData = INGENICO_REQ_CMD.CANCEL;
            }
            else if (builder.TransactionType == TransactionType.Duplicate) {
                extendedData = INGENICO_REQ_CMD.DUPLICATE;
            }
            else if (builder.TransactionType == TransactionType.Reversal) {
                extendedData = INGENICO_REQ_CMD.REVERSE;
            }

            // Concat all data to create a request string.
            var sb = new StringBuilder();

            sb.Append(builder.ReferenceNumber.ToString("00"));
            sb.Append(amount?.ToString("00000000"));
            sb.Append(returnRep);
            sb.Append(paymentMode);
            sb.Append(paymentType);
            sb.Append(currencyCode);
            sb.Append(privData);
            sb.Append("A01" + immediateAns);
            sb.Append("B01" + forceOnline);
            sb.Append(extendedData);

            return TerminalUtilities.BuildIngenicoRequest(sb.ToString(), _settings.ConnectionMode);
        }

        internal IDeviceMessage BuildProcessTransaction(TerminalAuthBuilder builder) {
            string message = string.Empty;
            if (!IsObjectNullOrEmpty(builder.ReportType))
                message = INGENICO_REQ_CMD.REPORT.FormatWith(builder.ReportType);
            else {
                int referenceNumber = builder.ReferenceNumber;
                decimal? amount = builder.Amount;
                int returnRep = 1;
                int paymentMode = 0;
                int paymentType = (int)((IngenicoInterface)_device).paymentMethod;
                string currencyCode = "826";
                string privateData = "EXT0100000";
                int immediateAnswer = 0;
                int forceOnline = 0;
                string extendedData = "0000000000";

                decimal? cashbackAmount = builder.CashBackAmount;
                string authCode = builder.AuthCode;
                string tableId = builder.TableNumber;

                // Validations
                if (referenceNumber == default(int) && RequestIdProvider != null) {
                    referenceNumber = RequestIdProvider.GetRequestId();
                }
                amount = ValidateAmount(amount);
                paymentMode = ValidatePaymentMode(builder.PaymentMode);
                currencyCode = ValidateCurrency((string.IsNullOrEmpty(builder.CurrencyCode) ? currencyCode : builder.CurrencyCode));

                if (!string.IsNullOrEmpty(tableId)) {
                    ValidateTableId(tableId);
                    extendedData = INGENICO_REQ_CMD.TABLE_WITH_ID.FormatWith(tableId);
                }
                else if (!string.IsNullOrEmpty(authCode)) {
                    extendedData = INGENICO_REQ_CMD.AUTHCODE.FormatWith(authCode);
                }
                else if (cashbackAmount != null) {
                    ValidateCashbackAmount(cashbackAmount);
                    cashbackAmount *= 100;
                    extendedData = INGENICO_REQ_CMD.CASHBACK.FormatWith(Convert.ToInt64(Math.Round(cashbackAmount.Value, MidpointRounding.AwayFromZero)));
                }

                // Concat all data to create a request string.
                var sb = new StringBuilder();

                sb.Append(referenceNumber.ToString("00").Substring(0, 2));
                sb.Append(amount?.ToString("00000000"));
                sb.Append(returnRep);
                sb.Append(paymentMode);
                sb.Append(paymentType);
                sb.Append(currencyCode);
                sb.Append(privateData);
                sb.Append("A01" + immediateAnswer);
                sb.Append("B01" + forceOnline);
                sb.Append(extendedData);

                message = sb.ToString();
            }

            return TerminalUtilities.BuildIngenicoRequest(message, _settings.ConnectionMode);
        }

        internal IngenicoTerminalReportResponse ReportRequest(IDeviceMessage request) {
            var send = Send(request);
            return new IngenicoTerminalReportResponse(send);
        }

        #region Validations
        private bool IsObjectNullOrEmpty(object value) {
            bool response = false;
            if (value.IsNull() || string.IsNullOrWhiteSpace(value.ToString())) {
                response = true;
            }
            else {
                response = false;
            }

            return response;
        }

        private void ValidateTableId(string value) {
            if (value.Length != 8) {
                throw new BuilderException("The required length for table number is 8.");
            }
        }

        private void ValidateCashbackAmount(decimal? value) {
            if (value >= 1000000m) {
                throw new BuilderException("Cashback Amount exceed.");
            }
            if (value < 0m) {
                throw new BuilderException("Cashback Amount must not be in less than zero.");
            }
        }

        private int ValidatePaymentMode(PaymentMode? paymentMode) {
            if (IsObjectNullOrEmpty(paymentMode)) {
                paymentMode = PaymentMode.APPLICATION;
            }

            return (int)paymentMode;
        }

        private string ValidateCurrency(string currencyCode) {
            if (!string.IsNullOrWhiteSpace(currencyCode)) {
                currencyCode = currencyCode.PadLeft(3, '0');
            }
            else {
                currencyCode = "826";
            }

            return currencyCode;
        }

        private decimal? ValidateAmount(decimal? amount) {
            if (amount != null && amount > 0 && amount < 1000000m) {
                amount *= 100;
            }
            else if (amount == null) {
                throw new BuilderException("Amount can not be null.");
            }
            else if (amount >= 1000000m) {
                throw new BuilderException("Amount exceed.");
            }
            else {
                throw new BuilderException("Invalid input amount.");
            }
            return amount;
        }

        #endregion

        #endregion

        #region Request
        internal IngenicoTerminalResponse DoRequest(IDeviceMessage request) {
            byte[] response = Send(request);
            return new IngenicoTerminalResponse(response);
        }

        private CancelResponse DoCancelRequest(IDeviceMessage request) {
            byte[] response = Send(request);
            return new CancelResponse(response);
        }

        private ReverseResponse DoReverseRequest(IDeviceMessage request) {
            byte[] response = Send(request);
            return new ReverseResponse(response);
        }
        #endregion
    }
}