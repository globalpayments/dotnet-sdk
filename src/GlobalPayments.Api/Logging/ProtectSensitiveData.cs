using GlobalPayments.Api.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Logging
{
    public static class ProtectSensitiveData
    {
        private static MaskedValueCollection HideValueCollection;
        public static Dictionary<string, string> HideValue(string key, string value, int unmaskedLastChars = 0, int unmaskedFirstChars = 0)
        {
            if (HideValueCollection == null) {
                HideValueCollection = new MaskedValueCollection();
            }
            return HideValueCollection.HideValue(key, value, unmaskedLastChars, unmaskedFirstChars);
        }

        public static Dictionary<string, string> HideValues(Dictionary<string, string> list, int unmaskedLastChars = 0, int unmaskedFirstChars = 0)
        {
            if (HideValueCollection == null) {
                HideValueCollection = new MaskedValueCollection();
            }
            var maskedList = new Dictionary<string, string>();
            foreach (var item in list) {
                maskedList = HideValueCollection.HideValue(item.Key, item.Value, unmaskedLastChars, unmaskedFirstChars);
            }

            return maskedList ?? null;
        }

        public static void DisposeCollection()
        {
            if (HideValueCollection != null)
            {
                HideValueCollection.DisposeMaskValues();
            }
        }
    }
}
