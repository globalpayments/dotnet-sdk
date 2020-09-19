using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE54_AmountsAdditional : IDataElement<DE54_AmountsAdditional> {
        private Dictionary<DE3_AccountType, Dictionary<DE54_AmountTypeCode, DE54_AdditionalAmount>> amountMap;

        public DE54_AmountsAdditional() {
            amountMap = new Dictionary<DE3_AccountType, Dictionary<DE54_AmountTypeCode, DE54_AdditionalAmount>>();
        }

        public DE54_AdditionalAmount Get(DE3_AccountType accountType, DE54_AmountTypeCode amountType) {
            if (amountMap.ContainsKey(accountType)) {
                Dictionary<DE54_AmountTypeCode, DE54_AdditionalAmount> amounts = amountMap[accountType];
                if (amounts.ContainsKey(amountType)) {
                    return amounts[amountType];
                }
                return null;
            }
            return null;
        }

        public decimal? GetAmount(DE3_AccountType accountType, DE54_AmountTypeCode amountType) {
            DE54_AdditionalAmount entity = Get(accountType, amountType);
            if (entity != null) {
                return entity.Amount;
            }
            return 0;
        }

        public void Put(DE54_AmountTypeCode amountType, DE3_AccountType accountType, Iso4217_CurrencyCode currencyCode, decimal? amount) {
            DE54_AdditionalAmount entry = new DE54_AdditionalAmount();
            entry.AccountType = accountType;
            entry.AmountType = amountType;
            entry.CurrencyCode = currencyCode;
            entry.Amount = amount;
            Put(entry);
        }

        public void Put(DE54_AdditionalAmount entry) {
            if (amountMap.Count < 6) {
                if (!amountMap.ContainsKey(entry.AccountType)) {
                    amountMap[entry.AccountType] = new Dictionary<DE54_AmountTypeCode, DE54_AdditionalAmount>();
                }
                amountMap[entry.AccountType][entry.AmountType] = entry;
            }
            else {
                throw new BuilderException("You may only specify 6 additional amountMap.");
            }
        }

        public int Size() {
            return amountMap.Count;
        }

        public DE54_AmountsAdditional FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            byte[] entryBuffer = sp.ReadBytes(20);
            while (entryBuffer.Length > 0) {
                DE54_AdditionalAmount entry = new DE54_AdditionalAmount().FromByteArray(entryBuffer);
                if (!amountMap.ContainsKey(entry.AccountType)) {
                    amountMap[entry.AccountType] = new Dictionary<DE54_AmountTypeCode, DE54_AdditionalAmount>();
                }
                amountMap[entry.AccountType][entry.AmountType] = entry;
                entryBuffer = sp.ReadBytes(20);
            }
            return this;
        }

        public byte[] ToByteArray() {
            MessageWriter mw = new MessageWriter();
            foreach (Dictionary<DE54_AmountTypeCode, DE54_AdditionalAmount> amounts in amountMap.Values) {
                foreach (DE54_AdditionalAmount amount in amounts.Values) {
                    mw.AddRange(amount.ToByteArray());
                }
            }
            return mw.ToArray();
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
