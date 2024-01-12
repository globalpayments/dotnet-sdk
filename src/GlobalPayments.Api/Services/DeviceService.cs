using GlobalPayments.Api.Terminals;

namespace GlobalPayments.Api.Services {
    public class DeviceService {
        public static IDeviceInterface Create(ConnectionConfig config, string configName = "default") {
            ServicesContainer.ConfigureService(config, configName);
            if (config.GatewayConfig != null) {
                ServicesContainer.ConfigureService(config.GatewayConfig, "_upa_passthrough");
            }
            return ServicesContainer.Instance.GetDeviceInterface(configName);
        }

        public static IDeviceInterface FindDeviceController(string configName = "default")
        {
            return ServicesContainer.Instance.GetDeviceInterface(configName);
        }
    }
}
