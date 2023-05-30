using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    public abstract class TransactionApiEntity {
        internal abstract void FromJson(JsonDoc doc);
    }
}
