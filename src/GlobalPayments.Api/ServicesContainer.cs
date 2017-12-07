using System;
using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.HeartSIP;
using GlobalPayments.Api.Terminals.PAX;

namespace GlobalPayments.Api {
    internal class ConfiguredServices : IDisposable {
        public IPaymentGateway GatewayConnector { get; set; }
        public IRecurringService RecurringConnector { get; set; }
        public IDeviceInterface DeviceInterface { get; private set; }
        private DeviceController _deviceController;
        public DeviceController DeviceController {
            get {
                return _deviceController;
            }
            set {
                _deviceController = value;
                DeviceInterface = value.ConfigureInterface();
            }
        }
        public TableServiceConnector ReservationConnector { get; set; }
        public void Dispose() {
            DeviceController.Dispose();
        }
    }

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
        private Dictionary<string, ConfiguredServices> _configurations;
        private static ServicesContainer _instance;

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
        public static void Configure(ServicesConfig config, string configName = "default") {
            config.Validate();

            var cs = new ConfiguredServices();

            #region configure devices
            if (config.DeviceConnectionConfig != null) {
                switch (config.DeviceConnectionConfig.DeviceType) {
                    case DeviceType.PAX_S300:
                        cs.DeviceController = new PaxController(config.DeviceConnectionConfig);
                        break;
                    case DeviceType.HSIP_ISC250:
                        cs.DeviceController = new HeartSipController(config.DeviceConnectionConfig);
                        break;
                    default:
                        break;
                }
            }
            #endregion

            #region configure reservations
            if (config.ReservationProvider != null) {
                if (config.ReservationProvider == ReservationProviders.FreshTxt) {
                    cs.ReservationConnector = new TableServiceConnector {
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
                cs.GatewayConnector = gateway;
                cs.RecurringConnector = gateway;
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
                cs.GatewayConnector = gateway;

                var payplan = new PayPlanConnector {
                    SecretApiKey = config.SecretApiKey,
                    Timeout = config.Timeout,
                    ServiceUrl = config.ServiceUrl + "/Portico.PayPlan.v2/"
                };
                cs.RecurringConnector = payplan;
            }
            #endregion

            if (_instance == null)
                _instance = new ServicesContainer();

            _instance.AddConfiguration(configName, cs);
        }

        private ServicesContainer() {
            _configurations = new Dictionary<string, ConfiguredServices>();
        }

        private void AddConfiguration(string configName, ConfiguredServices config) {
            if (_configurations.ContainsKey(configName))
                _configurations[configName] = config;
            else _configurations.Add(configName, config);
        }

        internal IPaymentGateway GetClient(string configName) {
            if (_configurations.ContainsKey(configName))
                return _configurations[configName].GatewayConnector;
            return null;
        }

        internal IDeviceInterface GetDeviceInterface(string configName) {
            if (_configurations.ContainsKey(configName))
                return _configurations[configName].DeviceInterface;
            return null;
        }

        internal DeviceController GetDeviceController(string configName) {
            if (_configurations.ContainsKey(configName))
                return _configurations[configName].DeviceController;
            return null;
        }

        internal IRecurringService GetRecurringClient(string configName) {
            if (_configurations.ContainsKey(configName))
                return _configurations[configName].RecurringConnector;
            return null;
        }

        internal TableServiceConnector GetReservationService(string configName) {
            if (_configurations.ContainsKey(configName))
                return _configurations[configName].ReservationConnector;
            return null;
        }

        /// <summary>
        /// Implementation for `IDisposable`
        /// </summary>
        public void Dispose() {
            foreach (var config in _configurations.Values)
                config.Dispose();
        }
    }
}
