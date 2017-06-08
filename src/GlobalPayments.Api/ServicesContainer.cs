using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.PAX;

namespace GlobalPayments.Api
{
    public class ServicesContainer : IDisposable {
        private IPaymentGateway _gateway;
        private IRecurringService _recurring;
        private IDeviceInterface _device;
        private DeviceController _deviceControl;
        private static  ServicesContainer _instance;

        internal static ServicesContainer Instance {
            get {
                if (_instance != null)
                    return _instance;
                else throw new ApiException("Services container not configured.");
            }
        }

        public static void Configure(ServicesConfig config) {
            config.Validate();

            #region configure devices
            IDeviceInterface deviceInterface = null;
            DeviceController deviceController = null;
            if (config.DeviceConnectionConfig != null) {
                switch (config.DeviceConnectionConfig.DeviceType) {
                    case DeviceType.PaxS300:
                        deviceController = new PaxController(config.DeviceConnectionConfig);
                        deviceInterface = deviceController.ConfigureInterface();
                        break;
                    default:
                        break;
                }
            }
            #endregion

            #region configure gateways
            if (!string.IsNullOrEmpty(config.MerchantId)) {
                var gateway = new RealexConnector {
                    AccountId = config.AccountId,
                    Channel = config.Channel,
                    MerchantId = config.MerchantId,
                    RebatePassword = config.RebatePassword,
                    RefundPassword = config.RefundPassword,
                    SharedSecret = config.SharedSecret,
                    Timeout = config.Timeout,
                    ServiceUrl = config.ServiceUrl,
                    HostedPaymentConfig = config.HostedPaymentConfig
                };
                _instance = new ServicesContainer(gateway, gateway, deviceController, deviceInterface);
            }
            else {
                var gateway = new PorticoConnector {
                    SiteId = config.SiteId,
                    LicenseId = config.LicenseId,
                    DeviceId = config.DeviceId,
                    Username = config.Username,
                    Password = config.Password,
                    SecretApiKey = config.SecretApiKey,
                    DeveloperId = config.DeveloperId,
                    VersionNumber = config.VersionNumber,
                    Timeout = config.Timeout,
                    ServiceUrl = config.ServiceUrl + "/Hps.Exchange.PosGateway/PosGatewayService.asmx"
                };
                var payplan = new PayPlanConnector {
                    SecretApiKey = config.SecretApiKey,
                    Timeout = config.Timeout,
                    ServiceUrl = config.ServiceUrl + "/Portico.PayPlan.v2/"
                };
                _instance = new ServicesContainer(gateway, payplan, deviceController, deviceInterface);
            }
            #endregion
        }

        private ServicesContainer(IPaymentGateway gateway, IRecurringService recurring = null, DeviceController deviceController = null, IDeviceInterface deviceInterface = null) {
            _gateway = gateway;
            _recurring = recurring;
            _device = deviceInterface;
            _deviceControl = deviceController;
        }

        internal IPaymentGateway GetClient() {
            return _gateway;
        }

        internal IDeviceInterface GetDeviceInterface() {
            return _device;
        }

        internal DeviceController GetDeviceController() {
            return _deviceControl;
        }

        internal IRecurringService GetRecurringClient() {
            return _recurring;
        }

        public void Dispose() {
            _deviceControl.Dispose();
        }
    }
}
