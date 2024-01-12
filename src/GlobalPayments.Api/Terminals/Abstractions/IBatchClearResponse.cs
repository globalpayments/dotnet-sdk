namespace GlobalPayments.Api.Terminals.Abstractions
{    public interface IBatchClearResponse : IDeviceResponse {
        string SequenceNumber { get; set; }
        string TotalCount { get; set; }
        string TotalAmount { get; set; }
    }
}
