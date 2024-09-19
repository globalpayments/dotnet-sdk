using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Entities {
    public class MaskedValueEntry {
        public string Key { get; private set; }
        public string Value { get; private set; }
        public int NumLeadingChars { get; private set; }
        public int NumTrailingChars { get; private set; }

        public MaskedValueEntry(string key, string value, int numLeadingChars = 0, int numTrailingChars = 0) {
            Key = key;
            Value = value;
            NumLeadingChars = numLeadingChars;
            NumTrailingChars = numTrailingChars;
        }
    }

    public class MaskedValueCollection {
        private Dictionary<string, string> _maskedValues;

        public Dictionary<string, string> ToDictionary() {
            return _maskedValues;
        }

        public void DisposeMaskValues() {
            if (_maskedValues != null) {
                _maskedValues = null;
            }
        }

        public bool AddValue(MaskedValueEntry entry) {
            if (_maskedValues == null) {
                _maskedValues = new Dictionary<string, string>();
            }
            if (!ValidateValue(entry.Value) || _maskedValues.ContainsKey(entry.Key)) {
                return false;
            }
            _maskedValues[entry.Key] = Disguise(entry.Value, entry.NumTrailingChars, entry.NumLeadingChars);
            return true;
        }

        private bool ValidateValue(string value) {
            if (string.IsNullOrEmpty(value)) {
                return false;
            }
            return true;
        }

        private string Disguise(string value, int unmaskedLastChars = 0, int unmaskedFirstChars = 0, char maskSymbol = 'X') {
            // not enough chars to unmask ?
            //
            if (unmaskedLastChars >= value.Length) {
                unmaskedLastChars = 0;
            }

            // at least half must be masked ?
            if (unmaskedLastChars > (value.Length / 2)) {
                unmaskedLastChars = (int)Math.Round(((decimal)(unmaskedLastChars / 2)));
            }

            // leading unmasked chars
            if (unmaskedLastChars < 0) {
                var unmasked = value.Substring(value.Length - (unmaskedLastChars * -1), (unmaskedLastChars * -1));
                return unmasked.PadLeft(unmaskedLastChars, maskSymbol);
            }

            // trailing unmasked chars
            var unmaskedFirstData = value.Substring(0, unmaskedFirstChars);
            var unmaskedLastData = value.Substring(value.Length - unmaskedLastChars, unmaskedLastChars);

            return unmaskedFirstData + string.Empty.PadLeft(value.Length - (unmaskedLastChars + unmaskedFirstChars), maskSymbol) + unmaskedLastData;
        }

    }
}
