namespace GlobalPayments.Api.Terminals.Abstractions
{
    public interface ISafSummaryReport : IDeviceResponse
    {
        string TotalCount { get; set; }
        string TotalAmount { get; set; }
    }
}