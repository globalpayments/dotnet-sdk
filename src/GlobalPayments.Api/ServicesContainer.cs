using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.HeartSIP;
using GlobalPayments.Api.Terminals.PAX;

namespace GlobalPayments.Api
{
    /// <summary>
    /// Maintains references to the currently configured gateway/device objects
    /// </summary>
    /// <remarks>
    /// The public `ServicesContainer.Configure` method is the only call
    /// required of the integrator to configure the SDK's various gateway/device
    /// interactions. The configured gateway/device objects are handled
    /// internally by exposed APIs throughout the SDK.
    /// </remarks>
    public class ServicesContainer : IDisposable {
        private IPaymentGateway _gateway;
        private IRecurringService _recurring;
        private IDeviceInterface _device;
        private DeviceController _deviceControl;
        private TableServiceConnector _reservationInterface;
        private static  ServicesContainer _instance;

        internal static ServicesContainer Instance {
            get {
                if (_instance != null)
                    return _instance;
                else throw new ApiException("Services container not configured.");
            }
        }

        /// <summary>
        /// Configure the SDK's various gateway/device interactions
        /// </summary>
        public static void Configure(ServicesConfig config) {
            config.Validate();

            #region configure devices
            IDeviceInterface deviceInterface = null;
            DeviceController deviceController = null;
            if (config.DeviceConnectionConfig != null) {
                switch (config.DeviceConnectionConfig.DeviceType) {
                    case DeviceType.PAX_S300:
                        deviceController = new PaxController(config.DeviceConnectionConfig);
                        deviceInterface = deviceController.ConfigureInterface();
                        break;
                    case DeviceType.HSIP_ISC250:
                        deviceController = new HeartSipController(config.DeviceConnectionConfig);
                        deviceInterface = deviceController.ConfigureInterface();
                        break;
                    default:
                        break;
                }
            }
            #endregion

            #region configure reservations
            TableServiceConnector reservationConnector = null;
            if (config.ReservationProvider != null) {
                if (config.ReservationProvider == ReservationProviders.FreshTxt) {
                    reservationConnector = new TableServiceConnector {
                        ServiceUrl = "https://www.freshtxt.com/api31/",
                        Timeout = config.Timeout
                    };
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
                _instance = new ServicesContainer(gateway, gateway, deviceController, deviceInterface, reservationConnector);
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
                _instance = new ServicesContainer(gateway, payplan, deviceController, deviceInterface, reservationConnector);
            }
            #endregion
        }

        private ServicesContainer(IPaymentGateway gateway, IRecurringService recurring = null, DeviceController deviceController = null, IDeviceInterface deviceInterface = null, TableServiceConnector reservationInterface = null) {
            _gateway = gateway;
            _recurring = recurring;
            _device = deviceInterface;
            _deviceControl = deviceController;

            // reservation service
            _reservationInterface = reservationInterface;
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

        internal TableServiceConnector GetReservationService() {
            return _reservationInterface;
        }

        /// <summary>
        /// Implementation for `IDisposable`
        /// </summary>
        public void Dispose() {
            _deviceControl.Dispose();
        }
    }
}
