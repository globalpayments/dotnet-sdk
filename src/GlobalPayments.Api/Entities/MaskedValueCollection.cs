using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class MaskedValueCollection
    {
        protected Dictionary<string, string> MaskValues;

        private Dictionary<string, string> GetValues()
        {
            return MaskValues;
        }

        public Dictionary<string, string> HideValue(string key, string value, int unmaskedLastChars = 0, int unmaskedFirstChars = 0)
        {
            AddValue(key, value, unmaskedLastChars, unmaskedFirstChars);
            return GetValues();
        }

        public void DisposeMaskValues()
        {
            if (MaskValues != null) {
                MaskValues = null;
            }
        }

        protected bool AddValue(string key, string value, int unmaskedLastChars = 0, int unmaskedFirstChars = 4)
        {
            if (MaskValues == null) {
                MaskValues = new Dictionary<string, string>();
            }
            if (!ValidateValue(value) || MaskValues.ContainsKey(key)) {
                return false;
            }           
            MaskValues[key] = Disguise(value, unmaskedLastChars, unmaskedFirstChars);
            return true;
        }

        protected bool ValidateValue(string value)
        {
            if (string.IsNullOrEmpty(value)) {
                return false;
            }
            return true;
        }

        private string Disguise(string value, int unmaskedLastChars = 0, int unmaskedFirstChars = 0, char maskSymbol = 'X')
        {
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
