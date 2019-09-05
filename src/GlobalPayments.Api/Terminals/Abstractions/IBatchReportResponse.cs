using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.HPA.Responses;
using System.Collections.Generic;

namespace GlobalPayments.Api.Terminals.Abstractions {
    public interface IBatchReportResponse : IDeviceResponse {
        BatchSummary BatchSummary { get; }
        CardBrandSummary VisaSummary { get; }
        CardBrandSummary MasterCardSummary { get; }
        CardBrandSummary AmexSummary { get; }
        CardBrandSummary DiscoverSummary { get; }
        CardBrandSummary PaypalSummary { get; }
        List<TransactionSummary> TransactionSummaries { get; }
    }
}
