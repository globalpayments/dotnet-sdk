using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE123_ReconciliationTotals : IDataElement<DE123_ReconciliationTotals> {
        public string EntryFormat { get; set; } = "00";
        public List<DE123_ReconciliationTotal> Totals { get; set; }

        public int GetEntryCount() {
            return Totals.Count;
        }

        public DE123_ReconciliationTotals() {
            Totals = new List<DE123_ReconciliationTotal>();
        }

        public void SetTotalCredits(decimal totalAmount) {
            DE123_ReconciliationTotal total = new DE123_ReconciliationTotal {
                TransactionCount = 0,
                TransactionType = DE123_TransactionType.CreditLessReversals,
                TotalAmount = totalAmount
            };
            Totals.Add(total);
        }

        public void SetTotalDebits(int transactionCount, decimal totalAmount) {
            DE123_ReconciliationTotal total = new DE123_ReconciliationTotal {
                TransactionCount = transactionCount,
                TransactionType = DE123_TransactionType.DebitLessReversals,
                TotalAmount = totalAmount
            };
            Totals.Add(total);
        }

        public void SetProprietaryTotals(int transactionCount, decimal totalAmount) {
            DE123_ReconciliationTotal total = new DE123_ReconciliationTotal {
                CardType = "PL",
                TransactionCount = transactionCount,
                TransactionType = DE123_TransactionType.DebitLessReversals,
                TotalAmount = totalAmount
            };
            Totals.Add(total);
        }

        public void SetVisaSaleTotals(int transactionCount, decimal totalAmount) {
            DE123_ReconciliationTotal total = new DE123_ReconciliationTotal {
                CardType = "VI",
                TransactionCount = transactionCount,
                TransactionType = DE123_TransactionType.DebitLessReversals,
                TotalAmount = totalAmount
            };
            Totals.Add(total);
        }

        public void SetMasterCardSaleTotals(int transactionCount, decimal totalAmount) {
            DE123_ReconciliationTotal total = new DE123_ReconciliationTotal {
                CardType = "MC",
                TransactionCount = transactionCount,
                TransactionType = DE123_TransactionType.DebitLessReversals,
                TotalAmount = totalAmount
            };
            Totals.Add(total);
        }

        public void SetOtherCreditSaleTotals(int transactionCount, decimal totalAmount) {
            DE123_ReconciliationTotal total = new DE123_ReconciliationTotal {
                CardType = "OH",
                TransactionCount = transactionCount,
                TransactionType = DE123_TransactionType.DebitLessReversals,
                TotalAmount = totalAmount
            };
            Totals.Add(total);
        }

        public void SetCreditSaleTotals(int transactionCount, decimal totalAmount) {
            DE123_ReconciliationTotal total = new DE123_ReconciliationTotal {
                CardType = "CT",
                TransactionCount = transactionCount,
                TransactionType = DE123_TransactionType.DebitLessReversals,
                TotalAmount = totalAmount
            };
            Totals.Add(total);
        }

        public void SetDebitSaleTotals(int transactionCount, decimal totalAmount) {
            DE123_ReconciliationTotal total = new DE123_ReconciliationTotal {
                CardType = "DB",
                TransactionCount = transactionCount,
                TransactionType = DE123_TransactionType.DebitLessReversals,
                TotalAmount = totalAmount
            };
            Totals.Add(total);
        }


        public void SetCreditRefundTotals(int transactionCount, decimal totalAmount) {
            DE123_ReconciliationTotal total = new DE123_ReconciliationTotal {
                CardType = "CT",
                TransactionCount = transactionCount,
                TransactionType = DE123_TransactionType.CreditLessReversals,
                TotalAmount = totalAmount
            };
            Totals.Add(total);
        }

        public void SetDebitRefundTotals(int transactionCount, decimal totalAmount) {
            DE123_ReconciliationTotal total = new DE123_ReconciliationTotal {
                CardType = "DB",
                TransactionCount = transactionCount,
                TransactionType = DE123_TransactionType.CreditLessReversals,
                TotalAmount = totalAmount
            };
            Totals.Add(total);
        }


        public void SetCreditVoidTotals(int transactionCount, decimal totalAmount) {
            DE123_ReconciliationTotal total = new DE123_ReconciliationTotal {
                CardType = "CT",
                TransactionCount = transactionCount,
                TransactionType = DE123_TransactionType.AllVoids_Voids,
                TotalAmount = totalAmount
            };
            Totals.Add(total);
        }

        public void SetDebitVoidTotals(int transactionCount, decimal totalAmount) {
            DE123_ReconciliationTotal total = new DE123_ReconciliationTotal {
                CardType = "DB",
                TransactionCount = transactionCount,
                TransactionType = DE123_TransactionType.AllVoids_Voids,
                TotalAmount = totalAmount
            };
            Totals.Add(total);
        }

        public DE123_ReconciliationTotals FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            EntryFormat = sp.ReadString(2);
            int entryCount = sp.ReadInt(2);

            for (int i = 0; i < entryCount; i++) {
                DE123_ReconciliationTotal total = new DE123_ReconciliationTotal {
                    TransactionType = EnumConverter.FromMapping<DE123_TransactionType>(Target.NWS, sp.ReadString(3))
                };
                total.TotalType = EnumConverter.FromMapping<DE123_TotalType>(Target.NWS,sp.ReadString(3));
                total.CardType = StringUtils.TrimEnd(sp.ReadString(4));
                total.TransactionCount = int.Parse(sp.ReadToChar('\\'));
                total.TotalAmount = StringUtils.ToAmount(sp.ReadToChar('\\'));

                Totals.Add(total);
            }

            return this;
        }

        public byte[] ToByteArray() {
            string rvalue = string.Concat(EntryFormat,StringUtils.PadLeft(GetEntryCount(), 2, '0'));

            foreach(DE123_ReconciliationTotal total in Totals) {
                rvalue = string.Concat(rvalue, EnumConverter.GetMapping(Target.NWS, total.TransactionType),
                    (EnumConverter.GetMapping(Target.NWS, total.TotalType)),
                        (StringUtils.PadRight(total.CardType, 4, ' ')),
                        (total.TransactionCount + "\\"),
                        (StringUtils.ToNumeric(total.TotalAmount) + "\\"));
            }

            return Encoding.UTF8.GetBytes(rvalue);
        }
    }
}
