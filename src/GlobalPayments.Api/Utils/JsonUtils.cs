using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GlobalPayments.Api.Entities;

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
            try {
                byte[] data = Convert.FromBase64String(value.ToString());
                return Encoding.UTF8.GetString(data, 0, data.Length);
            }
            catch (Exception exc) {
                throw new ApiException(exc.Message, exc);
            }
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

        public List<string> Keys {
            get {
                return _dict.Select(p => p.Key).ToList();
            }
        }

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
                if (_encoder != null) {
                    _dict.Add(key, _encoder.Encode(value));
                }
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

        public object GetValue(string name) {
            if (_dict.ContainsKey(name)) {
                return _dict[name];
            }
            return null;
        }

        public T GetValue<T>(params string[] names) {
            foreach (var name in names) {
                T value = GetValue<T>(name);
                if (!EqualityComparer<T>.Default.Equals(value, default(T))) {
                    return value;
                }
            }
            return default(T);
        }

        public T GetValue<T>(string name, Func<object, T> converter = null) {
            if (_dict.ContainsKey(name) && _dict[name] != null) {
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
                return new List<JsonDoc>();
            }
            return new List<JsonDoc>();
        }

        public IEnumerable<T> GetArray<T>(string name) {
            if (_dict.ContainsKey(name)) {
                if (_dict[name] is IEnumerable<T>)
                    return (IEnumerable<T>)_dict[name];
                return null;
            }
            return null;
        }

        public bool Has(string name) {
            return _dict.ContainsKey(name);
        }

        public bool HasKeys() {
            return _dict.Keys.Count > 0;
        }

        public static JsonDoc Parse(string json, IRequestEncoder encoder = null) {
            var parsed = JsonConvert.DeserializeObject(json);
            if (parsed is JObject) {
                return ParseObject(parsed as JObject, encoder);
            }
            return null;
        }

        public static T ParseSingleValue<T>(string json, string name, IRequestEncoder encoder = null) {
            var doc = Parse(json, encoder);
            return doc.GetValue<T>(name);
        }

        public static JsonDoc ParseObject(JObject obj, IRequestEncoder encoder) {
            var values = new Dictionary<string, object>();
            foreach (var child in obj.Children<JProperty>()) {
                if (child.Value is JArray) {
                    if (child.Value.First is JObject) {
                        var objs = ParseObjectArray(child.Value as JArray, encoder);
                        values.Add(child.Name, objs);
                    }
                    else {
                        var objs = ParseTypeArray<string>(child.Value as JArray, encoder);
                        values.Add(child.Name, objs);
                    }
                }
                else if (child.Value is JObject) {
                    values.Add(child.Name, ParseObject(child.Value as JObject, encoder));
                }
                else values.Add(child.Name, (child.Value as JValue).Value);
            }
            return new JsonDoc(values, encoder);
        }

        public static IEnumerable<T> ParseTypeArray<T>(JArray objs, IRequestEncoder encoder) {
            var response = new List<T>();
            foreach (var child in objs.Children<JToken>()) {
                response.Add(child.Value<T>());
            }
            return response;
        }

        public static IEnumerable<JsonDoc> ParseObjectArray(JArray objs, IRequestEncoder encoder) {
            var responses = new List<JsonDoc>();
            foreach (var child in objs.Children<JObject>()) {
                responses.Add(ParseObject(child, encoder));
            }
            return responses;
        }
    }
}
