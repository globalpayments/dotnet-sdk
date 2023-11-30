using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Terminals.Diamond.Interfaces {
    internal class DiamondInterface : DeviceInterface<DiamondController> {
        public DiamondInterface(DiamondController controller) : base(controller) {
        }

        public override TerminalManageBuilder TipAdjust(decimal? tipAmount = null) {
            return (new TerminalManageBuilder(TransactionType.Edit, PaymentMethodType.Credit))
                .WithGratuity(tipAmount);
        }

        public override TerminalReportBuilder LocalDetailReport() {
            return new TerminalReportBuilder(TerminalReportType.LocalDetailReport);
        }

        public override TerminalManageBuilder DeletePreAuth() {
            return (new TerminalManageBuilder(TransactionType.Delete, PaymentMethodType.Credit))
                .WithTransactionModifier(TransactionModifier.DeletePreAuth);
        }

        public override TerminalManageBuilder IncreasePreAuth(decimal amount) {
            return (new TerminalManageBuilder(TransactionType.Auth, PaymentMethodType.Credit))
                .WithTransactionModifier(TransactionModifier.Incremental);
        }

        public override IBatchCloseResponse BatchClose() {
            return (new TerminalAuthBuilder(TransactionType.BatchClose, PaymentMethodType.Credit))
                .Execute() as DeviceResponse;
        }

        public override TerminalManageBuilder RefundById(decimal? amount = null) {
            return new TerminalManageBuilder(TransactionType.Refund, PaymentMethodType.Credit);                
        }
    }
}
