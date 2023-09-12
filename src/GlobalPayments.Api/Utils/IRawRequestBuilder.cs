namespace GlobalPayments.Api.Utils {
    public interface IRawRequestBuilder {
        T GetValue<T>(params string[] names);
    }
}
