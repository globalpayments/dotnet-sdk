namespace GlobalPayments.Api.Network.Abstractions {
    public interface IDataElement<TResult> {
        TResult FromByteArray(byte[] buffer);
        byte[] ToByteArray();
    }
}
