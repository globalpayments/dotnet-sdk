using System;
using System.Collections.Generic;
using System.Net.Http;

namespace GlobalPayments.Api.Utils {
    public class MultipartForm {
        protected MultipartFormDataContent _content;

        public MultipartFormDataContent Content { get { return _content; } }

        public MultipartForm(string boundary = "--GlobalPaymentsSDK") {
            _content = new MultipartFormDataContent(boundary);
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
    }
}
