using GlobalPayments.Api.Entities;
using System.Collections.Generic;

namespace GlobalPayments.Api.Logging
{
    public static class ProtectSensitiveData
    {
        private static MaskedValueCollection HideValueCollection;

        // The collection is process-global static state reused across concurrent callers.
        // Serialize every add/dispose/read so concurrent transactions cannot corrupt the
        // backing dictionary (which previously threw "collection was modified" / non-concurrent
        // access exceptions from the shared gateway path under load).
        private static readonly object _sync = new object();

        // Caller must hold _sync.
        private static MaskedValueCollection EnsureCollection() {
            if (HideValueCollection == null) {
                HideValueCollection = new MaskedValueCollection();
            }
            return HideValueCollection;
        }

        public static Dictionary<string, string> HideValue(MaskedValueEntry entry) {
            lock (_sync) {
                EnsureCollection().AddValue(entry);
                return HideValueCollection.ToDictionary();
            }
        }

        public static Dictionary<string, string> HideValues(params MaskedValueEntry[] entries) {
            lock (_sync) {
                var collection = EnsureCollection();
                foreach (MaskedValueEntry entry in entries) {
                    collection.AddValue(entry);
                }
                return collection.ToDictionary();
            }
        }

        public static Dictionary<string, string> HideValue(string key, string value, int unmaskedLastChars = 0, int unmaskedFirstChars = 0) {
            return HideValue(new MaskedValueEntry(key, value, unmaskedFirstChars, unmaskedLastChars));
        }

        public static Dictionary<string, string> HideValues(Dictionary<string, string> list, int unmaskedLastChars = 0, int unmaskedFirstChars = 0) {
            lock (_sync) {
                var collection = EnsureCollection();
                foreach (var item in list) {
                    collection.AddValue(new MaskedValueEntry(item.Key, item.Value, unmaskedFirstChars, unmaskedLastChars));
                }
                return collection.ToDictionary();
            }
        }

        public static void DisposeCollection() {
            lock (_sync) {
                if (HideValueCollection != null) {
                    HideValueCollection.DisposeMaskValues();
                }
            }
        }
    }
}
