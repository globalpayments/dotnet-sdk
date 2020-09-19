using GlobalPayments.Api.Network.Entities;

namespace GlobalPayments.Api.Network.Elements {
    public class DE123_ReconciliationTotal {
        public DE123_TransactionType TransactionType { get; set; }
        public DE123_TotalType TotalType { get; set; } = DE123_TotalType.NotSpecific;
        public string CardType { get; set; } = "    ";
        public int TransactionCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
