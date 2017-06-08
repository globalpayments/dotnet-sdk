using System;
using System.Text;
using GlobalPayments.Api.Terminals.Extensions;
using GlobalPayments.Api.Terminals.Abstractions;
using System.Collections.Generic;
using System.IO;

namespace GlobalPayments.Api.Terminals.PAX {
    internal class ExtDataSubGroup : IRequestSubGroup, IResponseSubGroup {
        private Dictionary<string, string> _collection = new Dictionary<string,string>();

        public string this[string key] {
            get {
                if (!_collection.ContainsKey(key))
                    //return string.Empty;
                    return null;
                return _collection[key];
            }
            set {
                if (!_collection.ContainsKey(key))
                    _collection.Add(key, null);
                _collection[key] = value;
            }
        }

        public ExtDataSubGroup() { }
        public ExtDataSubGroup(BinaryReader br) {
            var values = br.ReadToCode(ControlCodes.ETX);
            if (string.IsNullOrEmpty(values))
                return;

            var elements = values.Split((char)ControlCodes.US);
            foreach (var element in elements) {
                var kv = element.Split('=');

                try {
                    _collection.Add(kv[0].ToUpper(), kv[1]);
                }
                catch (IndexOutOfRangeException) { }
            }
        }

        public string GetElementString() {
            var sb = new StringBuilder();

            foreach (var key in _collection.Keys) {
                sb.Append("{0}={1}{2}".FormatWith(key, _collection[key], (char)ControlCodes.US));
            }

            return sb.ToString().TrimEnd((char)ControlCodes.US);
        }
    }
}
