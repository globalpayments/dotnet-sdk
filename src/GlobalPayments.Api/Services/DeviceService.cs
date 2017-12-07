using GlobalPayments.Api.Terminals;

namespace GlobalPayments.Api.Services {
    public class DeviceService {
        public static IDeviceInterface Create(ServicesConfig config, string configName = "default") {
            ServicesContainer.Configure(config, configName);
            return ServicesContainer.Instance.GetDeviceInterface(configName);
        }
    }
}
