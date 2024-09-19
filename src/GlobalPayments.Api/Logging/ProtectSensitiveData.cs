using GlobalPayments.Api.Entities;
using System.Collections.Generic;

namespace GlobalPayments.Api.Logging
{
    public static class ProtectSensitiveData
    {
        private static MaskedValueCollection HideValueCollection;

        public static Dictionary<string, string> HideValue(MaskedValueEntry entry) {
            if (HideValueCollection == null) {
                HideValueCollection = new MaskedValueCollection();
            }
            HideValueCollection.AddValue(entry);

            return HideValueCollection.ToDictionary();
        }

        public static Dictionary<string, string> HideValues(params MaskedValueEntry[] entries) {
            foreach (MaskedValueEntry entry in entries) {
                HideValue(entry);
            }
            return HideValueCollection.ToDictionary();
        }

        public static Dictionary<string, string> HideValue(string key, string value, int unmaskedLastChars = 0, int unmaskedFirstChars = 0) {
            HideValue(new MaskedValueEntry(key, value, unmaskedFirstChars, unmaskedLastChars));
            return HideValueCollection.ToDictionary();
        }

        public static Dictionary<string, string> HideValues(Dictionary<string, string> list, int unmaskedLastChars = 0, int unmaskedFirstChars = 0) {
            foreach (var item in list) {
                HideValue(new MaskedValueEntry(item.Key, item.Value, unmaskedFirstChars, unmaskedLastChars));
            }

            return HideValueCollection.ToDictionary();
        }

        public static void DisposeCollection() {
            if (HideValueCollection != null) {
                HideValueCollection.DisposeMaskValues();
            }
        }
    }
}
