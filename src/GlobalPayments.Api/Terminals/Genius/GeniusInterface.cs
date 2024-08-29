using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.Genius.Builders;
using GlobalPayments.Api.Terminals.Genius.Enums;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Terminals.UPA;

namespace GlobalPayments.Api.Terminals.Genius {
    internal class GeniusInterface : DeviceInterface<GeniusController> {        
        internal GeniusInterface(GeniusController controller) : base(controller) {           
        }

        public override TerminalAuthBuilder CreditSale(decimal amount) {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Credit).WithAmount(amount);
        }
        public override TerminalAuthBuilder CreditRefund(decimal amount) {
            return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.Credit).WithAmount(amount);
        }
        public override TerminalManageBuilder RefundById(decimal? amount) {
            return new MitcManageBuilder(TransactionType.Sale, PaymentMethodType.Credit, TransactionType.Refund).WithAmount(amount);
        }
        public override TerminalReportBuilder GetTransactionDetails(TransactionType transactionType, string transactionId, TransactionIdType transactionIdType) {
            return new TerminalReportBuilder(transactionType, transactionId, transactionIdType);
        }
        public override TerminalManageBuilder CreditVoid() {
            return new MitcManageBuilder(TransactionType.Sale, PaymentMethodType.Credit, TransactionType.Void);
        }
        public override TerminalManageBuilder DebitVoid() {
            return new MitcManageBuilder(TransactionType.Sale, PaymentMethodType.Debit, TransactionType.Void);
        }
        public override TerminalManageBuilder VoidRefund() {
            return new MitcManageBuilder(TransactionType.Refund, PaymentMethodType.Credit, TransactionType.Void);
        }
        public override TerminalAuthBuilder DebitSale(decimal amount) {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Debit).WithAmount(amount);
        }
    }
}

