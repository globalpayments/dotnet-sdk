using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    public abstract class GpApiEntity {
        internal abstract void FromJson(JsonDoc doc);
    }
}
