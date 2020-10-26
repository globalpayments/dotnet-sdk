using System.Collections.Generic;

namespace GlobalPayments.Api.Entities {
    public class CommercialData {
        public CommercialIndicator CommercialIndicator { get; private set; }

        public string Description { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal DutyAmount { get; set; }

        public string DestinationPostalCode { get; set; }

        public string DestinationCountryCode { get; set; }

        public decimal? FreightAmount { get; set; }

        public List<CommercialLineItem> LineItems { get; private set; }

        public string OriginPostalCode { get; set; }

        public string PoNumber { get; set; }

        public decimal? TaxAmount { get; set; }

        public TaxType TaxType { get; private set; }

        public CommercialData(TaxType taxType, CommercialIndicator level = CommercialIndicator.Level_II) {
            TaxType = taxType;
            CommercialIndicator = level;

            LineItems = new List<CommercialLineItem>();
        }

        public CommercialData AddLineItems(params CommercialLineItem[] items) {
            LineItems.AddRange(items);
            return this;
        }
    }
}
