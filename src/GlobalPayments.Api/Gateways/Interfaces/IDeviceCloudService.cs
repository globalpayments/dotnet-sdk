using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Gateways {
    public interface IDeviceCloudService {
        string ProcessPassThrough(JsonDoc request);
    }
}
