using GlobalPayments.Api.Terminals;

namespace GlobalPayments.Api.Services {
    public class DeviceService {
        public static IDeviceInterface Create(ServicesConfig config) {
            ServicesContainer.Configure(config);
            return ServicesContainer.Instance.GetDeviceInterface();
        }
    }
}
