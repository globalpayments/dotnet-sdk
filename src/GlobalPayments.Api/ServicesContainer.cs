using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Gateways.Interfaces;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;

namespace GlobalPayments.Api {
    public class ConfiguredServices : IDisposable {
        private Dictionary<Secure3dVersion, ISecure3dProvider> _secure3dProviders;

        internal IPayFacProvider PayFacProvider { get; set; }

        internal IOpenBankingProvider OpenBankingProvider { get; set; }

        internal IPaymentGateway GatewayConnector { get; set; }

        internal IRecurringService RecurringConnector { get; set; }

        internal IReportingService ReportingService { get; set; }

        internal IDeviceInterface DeviceInterface { get; private set; }

        internal IBillingProvider BillingProvider { get; set; }

        private DeviceController _deviceController;
        internal DeviceController DeviceController {
            get {
                return _deviceController;
            }
            set {
                _deviceController = value;
                DeviceInterface = value.ConfigureInterface();
            }
        }

        internal OnlineBoardingConnector BoardingConnector { get; set; }

        internal TableServiceConnector TableServiceConnector { get; set; }

        internal PayrollConnector PayrollConnector { get; set; }

        internal IFraudCheckService FraudService { get; set; }

        internal IFileProcessingService FileProcessingService { get; set; }

        internal ISecure3dProvider GetSecure3DProvider(Secure3dVersion version) {
            if (_secure3dProviders.ContainsKey(version)) {
                return _secure3dProviders[version];
            }
            else if (version.Equals(Secure3dVersion.Any)) {
                var provider = _secure3dProviders[Secure3dVersion.Two];
                if (provider == null) {
                    provider = _secure3dProviders[Secure3dVersion.One];
                }
                return provider;
            }
            return null;
        }
        internal void SetSecure3dProvider(Secure3dVersion version, ISecure3dProvider provider) {
            if (_secure3dProviders.ContainsKey(version)) {
                _secure3dProviders[version] = provider;
            }
            else _secure3dProviders.Add(version, provider);
        }

        internal void SetOpenBankingProvider(IOpenBankingProvider provider) {
            if (this.OpenBankingProvider == null) {
                this.OpenBankingProvider = provider;
            }
        }

        internal void SetPayFacProvider(IPayFacProvider provider) {
            if (this.PayFacProvider == null) {
                this.PayFacProvider = provider;
            }
        }


        public ConfiguredServices() {
            _secure3dProviders = new Dictionary<Secure3dVersion, ISecure3dProvider>();
        }

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
        private ConcurrentDictionary<string, ConfiguredServices> _configurations;
        private static ServicesContainer _instance;

        internal static ServicesContainer Instance {
            get {
                if (_instance == null)
                    _instance = new ServicesContainer();
                return _instance;
            }
        }

        /// <summary>
        /// Configure the SDK's various gateway/device interactions
        /// </summary>
        public static void Configure(ServicesConfig config, string configName = "default") {
            config.Validate();

            // configure devices
            ConfigureService(config.DeviceConnectionConfig, configName);

            // configure table service
            ConfigureService(config.TableServiceConfig, configName);

            // configure payroll
            ConfigureService(config.PayrollConfig, configName);

            // configure gateways
            ConfigureService(config.GatewayConfig, configName);
        }

        public static void ConfigureService<T>(T config, string configName = "default") where T: Configuration {
            if (config == null) {
                Instance.removeConfiguration(configName);
            } else {
                if (!config.Validated)
                    config.Validate();

                var cs = Instance.GetConfiguration(configName);
                config.ConfigureContainer(cs);

                Instance.AddConfiguration(configName, cs);
            }
        }

        private ServicesContainer() {
            _configurations = new ConcurrentDictionary<string, ConfiguredServices>();
        }

        private ConfiguredServices GetConfiguration(string configName) {
            if (_configurations.ContainsKey(configName))
                return _configurations[configName];
            return new ConfiguredServices();
        }

        private void AddConfiguration(string configName, ConfiguredServices config) {
            if (_configurations.ContainsKey(configName))
                _configurations[configName] = config;
            else if(!_configurations.TryAdd(configName, config)) {
                throw new ConfigurationException($"Failed to add configuration: {configName}.");
            }
        }

        internal IPaymentGateway GetClient(string configName) {
            if (_configurations.ContainsKey(configName))
                return _configurations[configName].GatewayConnector;
            throw new ApiException("The specified configuration has not been configured for gateway processing.");
        }

        internal IDeviceInterface GetDeviceInterface(string configName) {
            if (_configurations.ContainsKey(configName))
                return _configurations[configName].DeviceInterface;
            throw new ApiException("The specified configuration has not been configured for terminal interaction.");
        }

        internal DeviceController GetDeviceController(string configName) {
            if (_configurations.ContainsKey(configName))
                return _configurations[configName].DeviceController;
            throw new ApiException("The specified configuration has not been configured for terminal interaction.");
        }

        internal IRecurringService GetRecurringClient(string configName) {
            if (_configurations.ContainsKey(configName))
                return _configurations[configName].RecurringConnector;
            throw new ApiException("The specified configuration has not been configured for recurring processing.");
        }

        internal TableServiceConnector GetTableServiceClient(string configName) {
            if (_configurations.ContainsKey(configName))
                return _configurations[configName].TableServiceConnector;
            throw new ApiException("The specified configuration has not been configured for table service.");
        }

        internal OnlineBoardingConnector GetBoardingConnector(string configName) {
            if (_configurations.ContainsKey(configName))
                return _configurations[configName].BoardingConnector;
            return null;
        }

        internal PayrollConnector GetPayrollClient(string configName) {
            if (_configurations.ContainsKey(configName))
                return _configurations[configName].PayrollConnector;
            throw new ApiException("The specified configuration has not been configured for payroll.");
        }

        internal IReportingService GetReportingClient(string configName) {
            if (_configurations.ContainsKey(configName))
                return _configurations[configName].ReportingService;
            throw new ApiException("The specified configuration has not been configured for reporting.");
        }

        internal ISecure3dProvider GetSecure3d(string configName, Secure3dVersion version) {
            if (_configurations.ContainsKey(configName)) {
                var provider = _configurations[configName].GetSecure3DProvider(version);
                if (provider != null) {
                    return provider;
                }
                throw new ConfigurationException(string.Format("Secure 3d is not configured for version {0}.", version));
            }
            throw new ConfigurationException("Secure 3d is not configured on the connector.");
        }

        internal IPayFacProvider GetPayFac(string configName) {
            if (_configurations.ContainsKey(configName))
                return _configurations[configName].PayFacProvider;
            throw new ConfigurationException("PayFacProvider is not configured");
        }

        internal IOpenBankingProvider GetOpenBanking(string configName) {
            if (_configurations.ContainsKey(configName))
                return _configurations[configName].OpenBankingProvider;
            throw new ConfigurationException("OpenBankingProvider is not configured");
        }
        
        internal IBillingProvider GetBillingClient(string configName) {
            if (_configurations.ContainsKey(configName))
            {
                return _configurations[configName].BillingProvider;
            }

            throw new ApiException("The specified configuration has not been configured for gateway processing.");
        }

        internal IFraudCheckService GetFraudCheckClient(string configName) {
            if (_configurations.ContainsKey(configName))
            {
                return _configurations[configName].FraudService;
            }

            throw new ApiException("The specified configuration has not been configured for fraud check.");
        }
        internal IFileProcessingService GetFileProcessingClient(string configName)
        {
            if (_configurations.ContainsKey(configName)) {
                return _configurations[configName].FileProcessingService;
            }

            throw new ApiException("The specified configuration has not been configured for file processing.");
        }
        internal void removeConfiguration(String configName) {
            if(_configurations.ContainsKey(configName)) {
                ConfiguredServices config = new ConfiguredServices();
                if(!_configurations.TryRemove(configName,out config)) {
                    throw new ConfigurationException($"Failed to remove configuration: {configName}.");
                }
            }
        }

        public static void RemoveConfig(string config = "default") {
            Instance.removeConfiguration(config);
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
