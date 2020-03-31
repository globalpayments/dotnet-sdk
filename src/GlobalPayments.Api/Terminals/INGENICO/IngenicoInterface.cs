using System;
using System.Collections.Generic;
using System.Text;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.Ingenico {
    public class IngenicoInterface : DeviceInterface<IngenicoController>, IDeviceInterface {
        internal PaymentType? paymentMethod = null;
        internal IngenicoInterface(IngenicoController controller) : base(controller) {
        }

        #region Payment Transaction Management

        public override TerminalAuthBuilder Sale(decimal? amount = null) {
            paymentMethod = PaymentType.Sale;
            return base.Sale(amount);
        }

        public override TerminalAuthBuilder Refund(decimal? amount = null) {
            paymentMethod = PaymentType.Refund;
            return base.Refund(amount);
        }

        public override TerminalManageBuilder Capture(decimal? amount = null) {
            paymentMethod = PaymentType.CompletionMode;
            return base.Capture(amount);
        }

        /// <summary>
        /// Authorize method is Equivalent of Pre-Authorisation from Ingenico
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public override TerminalAuthBuilder Authorize(decimal? amount = null) {
            paymentMethod = PaymentType.PreAuthMode;
            return base.Authorize(amount);
        }

        /// <summary>
        /// Completion method is the Hotel Mode Completion of Ingenico
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public override TerminalAuthBuilder Completion(decimal? amount = null) {
            paymentMethod = PaymentType.CompletionMode;
            return base.Completion(amount);
        }

        public override TerminalAuthBuilder Verify() {
            paymentMethod = PaymentType.AccountVerification;
            return base.Verify();
        }
        #endregion

        #region XML & Report Management
        public override TerminalReportBuilder GetReport(ReportType type) {
            return base.GetReport(type);
        }

        public override TerminalReportBuilder GetLastReceipt(ReceiptType type = ReceiptType.TICKET) {
            return base.GetLastReceipt(type);
        }
        #endregion

        #region Transaction Management

        public override TerminalManageBuilder Cancel(decimal? amount = null) {
            if (amount != null) {
                return base.Cancel(amount);
            }
            else throw new BuilderException("Amount can't be null.");
        }

        public override TerminalManageBuilder Reverse(decimal? amount = null) {
            if (amount != null) {
                return base.Reverse(amount);
            }
            else throw new BuilderException("Amount can't be null.");
        }

        ///// <summary>
        ///// Duplicate falls under lost transaction recovery and we have mechanisms for this which we'll need to look into further 
        ///// </summary>
        public override TerminalManageBuilder Duplicate(decimal? amount = null) {
            return base.Duplicate(amount);
        }

        #endregion
    }
}