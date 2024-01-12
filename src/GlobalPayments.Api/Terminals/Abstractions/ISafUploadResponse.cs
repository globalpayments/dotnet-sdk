namespace GlobalPayments.Api.Terminals.Abstractions
{
    public interface ISafUploadResponse : IDeviceResponse {
        string TotalCount { get; set; }
        string TotalAmount { get; set; }
        string TimeStamp { get; set; }
        string UploadedCount { get; set; }
        string UploadedAmount { get; set; }
        string FailedCount { get; set; }
        string FailedTotal { get; set; }
        string TorInfo { get; set; }
    }
}