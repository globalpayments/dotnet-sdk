using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobalPayments.Api.Utils {
    public interface IRequestEncoder {
        string Encode(object value);
        string Decode(object value);
    }

    public class Base64Encoder : IRequestEncoder {
        public string Encode(object value) {
            if (value == null) return null;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value.ToString()));
        }

        public string Decode(object value) {
            byte[] data = Convert.FromBase64String(value.ToString());
            return Encoding.UTF8.GetString(data, 0, data.Length);
        }
    }
    
    public static class JsonEncoders {
        public static Base64Encoder Base64Encoder {
            get {
                return new Base64Encoder();
            }
        }
    }

    public class JsonDoc {
        Dictionary<string, object> _dict;
        IRequestEncoder _encoder;

        public JsonDoc(IRequestEncoder encoder = null) {
            _dict = new Dictionary<string, object>();
            _encoder = encoder;
        }

        private JsonDoc(Dictionary<string, object> values, IRequestEncoder encoder = null) {
            _dict = values;
            _encoder = encoder;
        }

        // request stuff
        public JsonDoc Remove(string key) {
            _dict.Remove(key);
            return this;
        }

        public JsonDoc Set<T>(string key, T value, bool force = false) {
            if (!EqualityComparer<T>.Default.Equals(value, default(T)) || force || typeof(T) == typeof(bool)) {
                if (_encoder != null)
                    _dict.Add(key, _encoder.Encode(value));
                else _dict.Add(key, value);
            }
            return this;
        }

        public JsonDoc SubElement(string name) {
            var subRequest = new JsonDoc();
            _dict.Add(name, subRequest);
            return subRequest;
        }

        public new string ToString() {
            Dictionary<string, object> final = Finalize();
            return JsonConvert.SerializeObject(final);
        }

        public Dictionary<string, object> Finalize() {
            var final = new Dictionary<string, object>();
            foreach (var key in _dict.Keys) {
                if (_dict[key] is JsonDoc)
                    final.Add(key, (_dict[key] as JsonDoc).Finalize());
                else final.Add(key, _dict[key]);
            }
            return final;
        }

        // response stuff
        public JsonDoc Get(string name) {
            if (_dict.ContainsKey(name)) {
                if (_dict[name] is JsonDoc)
                    return (JsonDoc)_dict[name];
                return null;
            }
            return null;
        }

        public T GetValue<T>(string name, Func<object, T> converter = null) {
            if (_dict.ContainsKey(name)) {
                try {
                    var value = _dict[name];
                    if (_encoder != null)
                        value = _encoder.Decode(value);
                    if (converter != null)
                        return converter(value);
                    else return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (InvalidCastException) {
                    return (T)_dict[name];
                }
            }
            return default(T);
        }

        public IEnumerable<JsonDoc> GetEnumerator(string name) {
            if (_dict.ContainsKey(name)) {
                if (_dict[name] is IEnumerable<JsonDoc>)
                    return (IEnumerable<JsonDoc>)_dict[name];
                return null;
            }
            return null;
        }

        public bool Has(string name) {
            return _dict.ContainsKey(name);
        }

        public static JsonDoc Parse(string json, IRequestEncoder encoder = null) {
            var parsed = JsonConvert.DeserializeObject(json);
            if (parsed is JObject) {
                return ParseObject(parsed as JObject, encoder);
            }
            return null;
        }

        public static JsonDoc ParseObject(JObject obj, IRequestEncoder encoder) {
            var values = new Dictionary<string, object>();
            foreach (var child in obj.Children<JProperty>()) {
                if (child.Value is JArray) {
                    var objs = ParseArray(child.Value as JArray, encoder);
                    values.Add(child.Name, objs);
                }
                else if (child.Value is JObject) {
                    values.Add(child.Name, ParseObject(child.Value as JObject, encoder));
                }
                else values.Add(child.Name, (child.Value as JValue).Value);
            }
            return new JsonDoc(values, encoder);
        }

        public static IEnumerable<JsonDoc> ParseArray(JArray objs, IRequestEncoder encoder) {
            var responses = new List<JsonDoc>();
            foreach (var child in objs.Children<JObject>()) {
                responses.Add(ParseObject(child, encoder));
            }
            return responses;
        }
    }
}
