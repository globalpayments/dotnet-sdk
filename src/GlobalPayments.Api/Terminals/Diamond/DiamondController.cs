using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Builders.RequestBuilder;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.Diamond.Entities;
using GlobalPayments.Api.Terminals.Diamond.Interfaces;
using GlobalPayments.Api.Terminals.Diamond.Responses;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace GlobalPayments.Api.Terminals.Diamond {
    public class DiamondController : DeviceController {

        private DiamondCloudConfig _settings;
        private Dictionary<string, string> EndpointExceptions = new Dictionary<string, string>() {
            { DiamondCloudRequest.CAPTURE_EU, Region.EU.ToString() },
            { DiamondCloudRequest.CANCEL_AUTH, Region.EU.ToString() },
            { DiamondCloudRequest.INCREASE_AUTH, Region.EU.ToString() },
            { DiamondCloudRequest.RECONCILIATION, Region.EU.ToString() },
            { DiamondCloudRequest.CAPTURE, Region.US.ToString() },
            { DiamondCloudRequest.EBT_FOOD, Region.US.ToString() },
            { DiamondCloudRequest.EBT_BALANCE, Region.US.ToString() },
            { DiamondCloudRequest.EBT_RETURN, Region.US.ToString() },
            { DiamondCloudRequest.GIFT_RELOAD, Region.US.ToString() },
            { DiamondCloudRequest.GIFT_BALANCE, Region.US.ToString() },
            { DiamondCloudRequest.GIFT_REDEEM, Region.US.ToString() }
        };
        public DiamondController(DiamondCloudConfig settings) : base(settings) {
            _settings = settings;
        }

        internal override ITerminalResponse ProcessTransaction(TerminalAuthBuilder builder) {
            var request = BuildProcessTransaction(builder);
            return DoTransaction(request);
        }

        internal override ITerminalResponse ManageTransaction(TerminalManageBuilder builder) {
            var request = BuildManageTransaction(builder);
            return DoTransaction(request);
        }

        internal override ITerminalReport ProcessReport(TerminalReportBuilder builder) {
            new RequestBuilderValidations(SetupValidationsReport(builder))
                .Validate(builder, builder.ReportType);

            JsonDoc request = new JsonDoc();
            switch (builder.ReportType) {
                case TerminalReportType.LocalDetailReport:
                    request.Set("endpoint", $"/{_settings.PosID}/details/{builder.SearchBuilder.ReferenceNumber}");

                    JsonDoc param = new JsonDoc();
                    param.Set("POS_ID", _settings.PosID)
                        .Set("cloud_id", builder.SearchBuilder.ReferenceNumber);
                    request.Set("queryParams", param);

                    request.Set("verb", HttpMethod.Get);

                    break;
                default:
                    throw new GatewayException("Report type not defined!");
            }
            byte[] payload = Encoding.UTF8.GetBytes(request.ToString());
            var requestToDo = new DeviceMessage(payload);

            return DoTransaction(requestToDo);
        }

        internal override IDeviceInterface ConfigureInterface() {
            if (_interface == null)
                _interface = new DiamondInterface(this);
            return _interface;
        }

        internal override IDeviceCommInterface ConfigureConnector() {
            switch (base._settings.ConnectionMode) {
                case ConnectionModes.DIAMOND_CLOUD:
                    return new DiamondHttpInterface(base._settings);
                default:
                    throw new NotImplementedException();
            }
        }

        private DiamondCloudResponse DoTransaction(IDeviceMessage request) {
            request.AwaitResponse = true;
            var response = _connector.Send(request);

            if (response == null) {
                return null;
            }

            string jsonObject = Encoding.UTF8.GetString(response);

            return new DiamondCloudResponse(jsonObject);
        }                

        private IDeviceMessage BuildProcessTransaction(TerminalAuthBuilder builder) {
            JsonDoc body = new JsonDoc();
            JsonDoc request = new JsonDoc();
            JsonDoc param = new JsonDoc();
            string endpoint = string.Empty;
            param.Set("POS_ID", _settings.PosID);
            switch (builder.TransactionType) {
                case TransactionType.Sale:
                    request.Set("verb", HttpMethod.Post);
                    switch (builder.PaymentMethodType) {
                        case PaymentMethodType.EBT:
                            endpoint = DiamondCloudRequest.EBT_FOOD;
                            body.Set("amount", builder.Amount.ToNumericCurrencyString());
                            break;

                        case PaymentMethodType.Gift:
                            endpoint = DiamondCloudRequest.GIFT_REDEEM;
                            body.Set("amount", builder.Amount.ToNumericCurrencyString());
                            break;

                        default:
                            endpoint = DiamondCloudRequest.SALE;
                            body.Set("amount", builder.Amount.ToNumericCurrencyString())
                                .Set("panDataToken", "")
                                .Set("tip_amount", builder.Gratuity.ToNumericCurrencyString())
                                .Set("cash_back", builder.CashBackAmount.ToNumericCurrencyString());
                            break;
                    }
                    break;
                case TransactionType.Refund:
                    request.Set("verb", HttpMethod.Post);
                    if (builder.PaymentMethodType == PaymentMethodType.EBT) {
                        endpoint = DiamondCloudRequest.EBT_RETURN;
                        body.Set("amount", builder.Amount.ToNumericCurrencyString());
                    }
                    else {
                        endpoint = DiamondCloudRequest.SALE_RETURN;
                        body.Set("amount", builder.Amount.ToNumericCurrencyString())
                            .Set("panDataToken", "");
                    }
                    break;
                case TransactionType.Auth:
                    endpoint = DiamondCloudRequest.AUTH;
                    request.Set("verb", HttpMethod.Post);
                    body.Set("amount", builder.Amount.ToNumericCurrencyString())
                        .Set("panDataToken", "");
                    break;
                case TransactionType.Balance:
                    if (_settings.Region != Region.EU.ToString()) {
                        request.Set("verb", HttpMethod.Post);
                        if (builder.PaymentMethodType == PaymentMethodType.EBT) {
                            endpoint = DiamondCloudRequest.EBT_BALANCE;
                        }
                        if (builder.PaymentMethodType == PaymentMethodType.Gift) {
                            endpoint = DiamondCloudRequest.GIFT_BALANCE;
                        }
                    }
                    else {
                        throw new GatewayException($"Transaction type {TransactionType.Balance.ToString()} for payment type not supported " +
                            $"in {_settings.Region}");
                    }
                    break;
                case TransactionType.BatchClose:
                    request.Set("verb", HttpMethod.Post);
                    endpoint = DiamondCloudRequest.RECONCILIATION;
                    break;
                case TransactionType.AddValue:
                    if (builder.PaymentMethodType == PaymentMethodType.Gift) {
                        request.Set("verb", HttpMethod.Post);
                        endpoint = DiamondCloudRequest.GIFT_RELOAD;
                        body.Set("amount", builder.Amount.ToNumericCurrencyString());
                    }
                    break;
                default:
                    throw new GatewayException($"Transaction type {builder.TransactionType} not supported!");
            }

            if (!ValidateEndpoint(endpoint)) {
                throw new GatewayException($"This feature is not supported in {_settings.Region} region!");
            }

            request.Set("endpoint", $"/{ _settings.PosID}{endpoint}")
            .Set("body", body)
            .Set("queryParams", param);

            byte[] payload = Encoding.UTF8.GetBytes(request.ToString());

            return new DeviceMessage(payload);
        }

        private IDeviceMessage BuildManageTransaction(TerminalManageBuilder builder) {
            JsonDoc body = new JsonDoc();
            JsonDoc request = new JsonDoc();
            JsonDoc param = new JsonDoc();
            string endpoint = string.Empty;
            param.Set("POS_ID", _settings.PosID);
            switch (builder.TransactionType) {
                case TransactionType.Void:
                    endpoint = DiamondCloudRequest.VOID;
                    request.Set("verb", HttpMethod.Post);
                    body.Set("transaction_id", builder.TransactionId);
                    break;
                case TransactionType.Edit:
                    endpoint = DiamondCloudRequest.TIP_ADJUST;
                    request.Set("verb", HttpMethod.Post);
                    body.Set("tip_amount", builder.Gratuity.ToNumericCurrencyString())
                        .Set("amount", builder.Amount.ToNumericCurrencyString())
                        .Set("transaction_id", builder.TransactionId);
                    break;
                case TransactionType.Capture:                    
                    endpoint = DiamondCloudRequest.CAPTURE;
                    if (_settings.Region == Region.EU.ToString()) {
                        endpoint = DiamondCloudRequest.CAPTURE_EU;
                    }
                    request.Set("verb", HttpMethod.Post);
                    body.Set("tip_amount", builder.Gratuity.ToNumericCurrencyString())
                            .Set("amount", builder.Amount.ToNumericCurrencyString())
                            .Set("transaction_id", builder.TransactionId);
                    break;
                case TransactionType.Delete:
                    if (builder.TransactionModifier == TransactionModifier.DeletePreAuth) {
                        endpoint = DiamondCloudRequest.CANCEL_AUTH;
                        request.Set("verb", HttpMethod.Post);
                        body.Set("transaction_id", builder.TransactionId);
                    }
                    break;
                case TransactionType.Auth:
                    if (builder.TransactionModifier == TransactionModifier.Incremental) {
                        endpoint = DiamondCloudRequest.INCREASE_AUTH;
                        request.Set("verb", HttpMethod.Post);
                        body.Set("amount", builder.Amount.ToNumericCurrencyString())
                            .Set("transaction_id", builder.TransactionId);
                    }
                    break;
                case TransactionType.Refund:                    
                    endpoint = DiamondCloudRequest.SALE_RETURN;
                    request.Set("verb", HttpMethod.Post);
                    body.Set("transaction_id", builder.TransactionId);                    
                    break;
                default:
                    throw new GatewayException($"Transaction type {builder.TransactionType} not supported!");
            }

            if (!ValidateEndpoint(endpoint)) {
                throw new GatewayException($"This feature is not supported in {_settings.Region} region!");
            }

            request.Set("endpoint", $"/{_settings.PosID}{endpoint}")
                .Set("body", body)
            .Set("queryParams", param);

            byte[] payload = Encoding.UTF8.GetBytes(request.ToString());

            return new DeviceMessage(payload);
        }

        internal override byte[] SerializeRequest(TerminalAuthBuilder builder) {
            throw new UnsupportedTransactionException();
        }

        internal override byte[] SerializeRequest(TerminalManageBuilder builder) {
            throw new UnsupportedTransactionException();
        }

        internal override byte[] SerializeRequest(TerminalReportBuilder builder) {
            throw new UnsupportedTransactionException();
        }

        #region Private methods
        private Validations SetupValidationsReport(TerminalReportBuilder _builder) {
            var validations = new Validations();
            validations.For(TerminalReportType.LocalDetailReport).Check(() => _builder.SearchBuilder).PropertyOf(nameof(TerminalSearchBuilder.ReferenceNumber)).IsNotNull();

            return validations;
        }

        private bool ValidateEndpoint(string endpoint) {
            if (string.IsNullOrEmpty(endpoint)) {
                throw new GatewayException("Payment type not supported!");
            }

            if (EndpointExceptions.ContainsKey(endpoint)) {
                if (EndpointExceptions.GetValue<string>(endpoint) != _settings.Region) {
                    return false;
                }
            }

            return true;
        }
        #endregion
    }
}
