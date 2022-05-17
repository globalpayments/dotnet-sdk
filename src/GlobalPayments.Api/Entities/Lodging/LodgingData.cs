using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Entities {
    public class LodgingData {
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public Dictionary <ExtraChargeType, decimal> ExtraCharges { get; set; }
        public string FolioNumber { get; set; }
        public decimal? Rate { get; set; }
        public int StayDuration { get; set; }
        public PrestigiousPropertyLimit? PrestigiousPropertyLimit { get; set; }
        public bool NoShow { get; set; }
        public AdvancedDepositType? AdvancedDepositType { get; set; }
        public string LodgingDataEdit { get; set; }
        public bool PreferredCustomer { get; set; }
        public string bookingReference { get; set; }
        public List<LodgingItems> Items { get; set; }

        public decimal ExtraChargeAmount {
            get {
                decimal total = 0m;
                foreach (decimal amount in ExtraCharges.Values) {
                    total = total + amount;
                }
             return total;
            }
        }

        public LodgingData AddExtraCharge(ExtraChargeType extraChargeType, decimal amount = 0m) {
            if (ExtraCharges == null) {
                ExtraCharges = new Dictionary<ExtraChargeType, decimal>();
            }

            decimal value = 0m;
            if (!ExtraCharges.ContainsKey(extraChargeType)) {
                ExtraCharges.Add(extraChargeType, value);
            }

            if (ExtraCharges.TryGetValue(extraChargeType, out value)) {
                ExtraCharges[extraChargeType] = value + amount;
            }
            return this;
        }
    }
}