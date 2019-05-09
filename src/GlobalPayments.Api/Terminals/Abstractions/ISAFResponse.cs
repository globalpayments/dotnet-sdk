using System.Collections.Generic;
namespace GlobalPayments.Api.Terminals.Abstractions {
    public interface ISAFResponse : IDeviceResponse {
        int TotalCount { get; }
        decimal? TotalAmount { get; }

        Dictionary<SummaryType, SummaryResponse> Approved { get; }
        Dictionary<SummaryType, SummaryResponse> Pending { get; }
        Dictionary<SummaryType, SummaryResponse> Declined { get; }
    }
}
