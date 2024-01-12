namespace GlobalPayments.Api.Terminals.Abstractions
{
    public interface ISafDeleteFileResponse : IDeviceResponse {
        string TotalCount { get; set; }
        string TorInfo { get; set; }
    }
}