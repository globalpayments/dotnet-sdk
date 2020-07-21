using GlobalPayments.Api.Network.Entities;

namespace GlobalPayments.Api.Network.Elements {
    public class DE62_2_CardIssuerEntry {
        public DE62_CardIssuerEntryTag IssuerTag { get; set; }
        public string IssuerTagValue { get; set; }
        public string IssuerEntry { get; set; }

        public DE62_2_CardIssuerEntry(DE62_CardIssuerEntryTag tag, string entry) : this(tag, null, entry) { }

        public DE62_2_CardIssuerEntry(DE62_CardIssuerEntryTag tag, string tagValue, string entry) {
            this.IssuerTag = tag;
            this.IssuerTagValue = tagValue;
            this.IssuerEntry = entry;
        }
    }
}
