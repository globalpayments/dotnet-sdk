namespace GlobalPayments.Api.Terminals.Abstractions {
    public interface IBatchCloseResponse : IDeviceResponse {
        string SequenceNumber { get; set; }
        string TotalCount { get; set; }
        string TotalAmount { get; set; }
    }
}
