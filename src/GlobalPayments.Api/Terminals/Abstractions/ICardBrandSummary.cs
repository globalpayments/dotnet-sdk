using GlobalPayments.Api.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Terminals.Abstractions {
    public interface ICardBrandSummary {
        CardType CardType { get; }
        int CreditCount { get; }
        decimal? CreditAmount { get; }
        int DebitCount { get; }
        decimal? DebitAmount { get; }
        int SaleCount { get; }
        decimal? SaleAmount { get; }
        int RefundCount { get; }
        decimal? RefundAmount { get; }
        int TotalCount { get; }
        decimal? TotalAmount { get; }
    }
}
