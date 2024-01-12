using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.PayFac;
using GlobalPayments.Api.Entities.Reporting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Services
{
    public class PayFacService : IDisposable {
        public PayFacBuilder<Transaction> CreateAccount() {
            return new PayFacBuilder<Transaction>(TransactionType.Create);
        }

        public PayFacBuilder<Transaction> EditAccount() {
            return new PayFacBuilder<Transaction>(TransactionType.Edit, TransactionModifier.Account);
        }

        public PayFacBuilder<Transaction> ResetPassword() {
            return new PayFacBuilder<Transaction>(TransactionType.ResetPassword);
        }

        public PayFacBuilder<Transaction> RenewAccount() {
            return new PayFacBuilder<Transaction>(TransactionType.RenewAccount);
        }

        public PayFacBuilder<Transaction> UpdateBeneficialOwnershipInfo() {
            return new PayFacBuilder<Transaction>(TransactionType.UpdateBeneficialOwnership);
        }

        public PayFacBuilder<Transaction> DisownAccount() {
            return new PayFacBuilder<Transaction>(TransactionType.Deactivate);
        }

        public PayFacBuilder<Transaction> UploadDocumentChargeback() {
            return new PayFacBuilder<Transaction>(TransactionType.UploadDocumentChargeback);
        }

        public PayFacBuilder<Transaction> UploadDocument() {
            return new PayFacBuilder<Transaction>(TransactionType.UploadDocument);
        }
        
        public PayFacBuilder<Transaction> ObtainSSOKey() {
            return new PayFacBuilder<Transaction>(TransactionType.ObtainSSOKey);
        }

        public PayFacBuilder<Transaction> UpdateBankAccountOwnershipInfo() {
            return new PayFacBuilder<Transaction>(TransactionType.UpdateBankAccountOwnership);
        }

        public PayFacBuilder<Transaction> AddFunds() {
            return new PayFacBuilder<Transaction>(TransactionType.AddFunds);
        }

        public PayFacBuilder<Transaction> SweepFunds() {
            return new PayFacBuilder<Transaction>(TransactionType.SweepFunds);
        }

        public PayFacBuilder<Transaction> AddCardFlashFunds() {
            return new PayFacBuilder<Transaction>(TransactionType.AddCardFlashFunds);
        }

        public PayFacBuilder<Transaction> PushMoneyToFlashFundsCard() {
            return new PayFacBuilder<Transaction>(TransactionType.PushMoneyFlashFunds);
        }

        public PayFacBuilder<Transaction> DisburseFunds() {
            return new PayFacBuilder<Transaction>(TransactionType.DisburseFunds);
        }

        public PayFacBuilder<Transaction> SpendBack() {
            return new PayFacBuilder<Transaction>(TransactionType.SpendBack);
        }

        public PayFacBuilder<Transaction> ReverseSplitPay() {
            return new PayFacBuilder<Transaction>(TransactionType.Reversal);
        }

        public PayFacBuilder<Transaction> SplitFunds() {
            return new PayFacBuilder<Transaction>(TransactionType.SplitFunds);
        }

        public PayFacBuilder<Transaction> GetAccountDetails() {
            return new PayFacBuilder<Transaction>(TransactionType.GetAccountDetails);
        }

        public PayFacBuilder<Transaction> GetAccountDetailsEnhanced() {
            return new PayFacBuilder<Transaction>(TransactionType.GetAccountDetails, TransactionModifier.Additional);
        }

        public PayFacBuilder<Transaction> GetAccountBalance() {
            return new PayFacBuilder<Transaction>(TransactionType.Balance);
        }

        public PayFacBuilder<User> CreateMerchant() {
            return new PayFacBuilder<User>(TransactionType.Create)
                .WithModifier(TransactionModifier.Merchant);
        }

        public PayFacBuilder<User> GetMerchantInfo(string merchantId)
        {
            var userReference = new UserReference();
            userReference.UserId = merchantId;
            userReference.UserType = Entities.Enums.UserType.MERCHANT;

            return new PayFacBuilder<User>(TransactionType.Fetch)
                .WithModifier(TransactionModifier.Merchant)
                .WithUserReference(userReference);
        }

        public PayFacBuilder<Transaction> OrderDevice() {
            return new PayFacBuilder<Transaction>(TransactionType.OrderDevice);
        }

        public void Dispose() {
        }
    }
}
