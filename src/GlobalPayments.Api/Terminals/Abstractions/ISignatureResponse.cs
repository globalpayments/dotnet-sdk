namespace GlobalPayments.Api.Terminals.Abstractions {
    public interface ISignatureResponse : IDeviceResponse {
        byte[] SignatureData { get; set; }
        string SigData { get; set; }
    }
}
