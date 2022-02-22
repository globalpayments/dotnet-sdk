using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Services
{
    public class PayFacService : IDisposable {
        public PayFacBuilder CreateAccount() {
            return new PayFacBuilder(TransactionType.CreateAccount);
        }

        public PayFacBuilder EditAccount() {
            return new PayFacBuilder(TransactionType.EditAccount);
        }

        public PayFacBuilder ResetPassword() {
            return new PayFacBuilder(TransactionType.ResetPassword);
        }

        public PayFacBuilder RenewAccount() {
            return new PayFacBuilder(TransactionType.RenewAccount);
        }

        public PayFacBuilder UpdateBeneficialOwnershipInfo() {
            return new PayFacBuilder(TransactionType.UpdateBeneficialOwnership);
        }

        public PayFacBuilder DisownAccount() {
            return new PayFacBuilder(TransactionType.DisownAccount);
        }

        public PayFacBuilder UploadDocumentChargeback() {
            return new PayFacBuilder(TransactionType.UploadDocumentChargeback);
        }

        public PayFacBuilder UploadDocument() {
            return new PayFacBuilder(TransactionType.UploadDocument);
        }

        public PayFacBuilder ObtainSSOKey() {
            return new PayFacBuilder(TransactionType.ObtainSSOKey);
        }

        public PayFacBuilder UpdateBankAccountOwnershipInfo() {
            return new PayFacBuilder(TransactionType.UpdateBankAccountOwnership);
        }

        public PayFacBuilder AddFunds() {
            return new PayFacBuilder(TransactionType.AddFunds);
        }

        public PayFacBuilder SweepFunds() {
            return new PayFacBuilder(TransactionType.SweepFunds);
        }

        public PayFacBuilder AddCardFlashFunds() {
            return new PayFacBuilder(TransactionType.AddCardFlashFunds);
        }

        public PayFacBuilder PushMoneyToFlashFundsCard() {
            return new PayFacBuilder(TransactionType.PushMoneyFlashFunds);
        }

        public PayFacBuilder DisburseFunds() {
            return new PayFacBuilder(TransactionType.DisburseFunds);
        }

        public PayFacBuilder SpendBack() {
            return new PayFacBuilder(TransactionType.SpendBack);
        }

        public PayFacBuilder ReverseSplitPay() {
            return new PayFacBuilder(TransactionType.ReverseSplitPay);
        }

        public PayFacBuilder SplitFunds() {
            return new PayFacBuilder(TransactionType.SplitFunds);
        }

        public PayFacBuilder GetAccountDetails() {
            return new PayFacBuilder(TransactionType.GetAccountDetails);
        }

        public PayFacBuilder GetAccountDetailsEnhanced() {
            return new PayFacBuilder(TransactionType.GetAccountDetails, TransactionModifier.Additional);
        }

        public PayFacBuilder GetAccountBalance() {
            return new PayFacBuilder(TransactionType.GetAccountBalance);
        }

        public void Dispose() {
        }
    }
}
