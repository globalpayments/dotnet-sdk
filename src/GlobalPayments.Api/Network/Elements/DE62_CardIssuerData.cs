using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE62_CardIssuerData : IDataElement<DE62_CardIssuerData> {
        public List<DE62_2_CardIssuerEntry> CardIssuerEntries { get; set; }

        public int GetNumEntries() {
            return CardIssuerEntries.Count;
        }

        public DE62_CardIssuerData() {
            CardIssuerEntries = new List<DE62_2_CardIssuerEntry>();
        }

        public void Add(DE62_2_CardIssuerEntry entry) {
            if (!string.IsNullOrEmpty(entry.IssuerEntry)) {
                CardIssuerEntries.Add(entry);
            }
        }

        public void Add(DE62_CardIssuerEntryTag tag, string value) {
            DE62_2_CardIssuerEntry entry = new DE62_2_CardIssuerEntry(tag, value);
            Add(entry);
        }

        public void Add(DE62_CardIssuerEntryTag tag, string tagValue, string value) {
            DE62_2_CardIssuerEntry entry = new DE62_2_CardIssuerEntry(tag, tagValue, value);
            Add(entry);
        }

        public string Get(DE62_CardIssuerEntryTag tag) {
            foreach (DE62_2_CardIssuerEntry entry in CardIssuerEntries) {
                if (entry.IssuerTag.Equals(tag)) {
                    return entry.IssuerEntry;
                }
            }
            return null;
        }

        public string Get(string tagValue) {
            foreach (DE62_2_CardIssuerEntry entry in CardIssuerEntries) {
                if (entry.IssuerTagValue.Equals(tagValue)) {
                    return entry.IssuerEntry;
                }
            }
            return null;
        }

        public DE62_CardIssuerData FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);

            int numEntries = sp.ReadInt(2);
            for(int i = 0; i < numEntries; i++) {
                string tagValue = sp.ReadString(3);
                //DE62_CardIssuerEntryTag tag = ReverseStringEnumMap<DE62_CardIssuerEntryTag>.Parse<DE62_CardIssuerEntryTag>(tagValue);
                //DE62_CardIssuerEntryTag tag = (DE62_CardIssuerEntryTag)Enum.Parse(typeof(DE62_CardIssuerEntryTag), tagValue);
                DE62_CardIssuerEntryTag tag = EnumConverter.FromMapping<DE62_CardIssuerEntryTag>(Target.NWS, tagValue);
                if (tag == default(DE62_CardIssuerEntryTag) || tag == 0) { // find one of the other values
                    tag = DE62_CardIssuerEntryTagClass.FindPartial(tagValue);
                }
                string issuerEntryData = sp.ReadLLVAR();

                DE62_2_CardIssuerEntry entry = new DE62_2_CardIssuerEntry(tag, issuerEntryData) {
                    IssuerTagValue = tagValue
                };
                CardIssuerEntries.Add(entry);
            }
            return this;
        }

        public byte[] ToByteArray() {
            string rvalue = StringUtils.PadLeft(CardIssuerEntries.Count, 2, '0');
            foreach(DE62_2_CardIssuerEntry entry in CardIssuerEntries) {
                // put the tag value if present
                if(!string.IsNullOrEmpty(entry.IssuerTagValue)) {
                    rvalue = string.Concat(rvalue,entry.IssuerTagValue);
                }
                else {
                    rvalue = string.Concat(rvalue, EnumConverter.GetMapping(Target.NWS, entry.IssuerTag));
                }
                // put the entry value
                rvalue = string.Concat(rvalue,StringUtils.PadLeft(entry.IssuerEntry.Length, 2, '0'),
                        entry.IssuerEntry);
            }
            return Encoding.ASCII.GetBytes(rvalue);
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
