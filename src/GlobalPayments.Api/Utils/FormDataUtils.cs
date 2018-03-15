using System;
using System.Collections.Generic;
using System.Net.Http;

namespace GlobalPayments.Api.Utils {
    public class MultipartForm {
        protected MultipartFormDataContent _content;

        public MultipartFormDataContent Content { get { return _content; } }

        public MultipartForm(bool appendJsonFlag = true) {
            _content = new MultipartFormDataContent("--GlobalPaymentsSDK");

            if (appendJsonFlag)
                _content.Add(new StringContent("1"), "json");
        }

        public MultipartForm Set(string key, string value, bool force = false) {
            if (!string.IsNullOrEmpty(value) || force) {
                _content.Add(new StringContent(value), key);
            }
            return this;
        }

        public MultipartForm Set<T>(string key, T value, bool force = false) {
            if (!EqualityComparer<T>.Default.Equals(value, default(T)) || force || typeof(T) == typeof(bool)) {
                if (value is DateTime?)
                    _content.Add(new StringContent((value as DateTime?).Value.ToString("hh:MM:ss")), key);
                else _content.Add(new StringContent(value.ToString()), key);
            }
            return this;
        }

        public string ToJson() {
            var fieldValues = new JsonDoc();

            var list = _content.GetEnumerator();
            while (list.MoveNext()) {
                var content = list.Current;

                string key = content.Headers.ContentDisposition.Name;
                string value = content.ReadAsStringAsync().Result;

                fieldValues.Set(key, value);
            }

            var request = new JsonDoc();
            request.Set("fieldValues", fieldValues);

            return request.ToString();
        }
    }
}
