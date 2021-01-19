using System;
using System.Reflection;

namespace GlobalPayments.Api.Utils {
    public class DescriptionAttribute : Attribute {
        public string Description { get; set; }

        public DescriptionAttribute(string description) {
            Description = description;
        }
    }

    [Flags]
    public enum Target {
        NWS,
        VAPS,
        Transit,
        Portico,
        GP_API
    }

    [AttributeUsage(AttributeTargets.Enum)]
    public class MapTargetAttribute : Attribute {
        public Target Target { get; private set; }

        public MapTargetAttribute(Target target) {
            Target = target;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class MapAttribute : Attribute {
        public Target Target { get; private set; }
        public string Value { get; set; }

        public MapAttribute(Target target, string value) {
            Target = target;
            Value = value;
        }
    }

    public class EnumConverter {
        public static string GetDescription(object value) {
            if (value is Enum) {
                var description = value.GetType().GetRuntimeField(value.ToString()).GetCustomAttribute<DescriptionAttribute>();
                return description?.Description;
            }
            return null;
        }

        public static T FromDescription<T>(object value) {
            var fields = typeof(T).GetRuntimeFields();
            foreach (var field in fields) {
                var attr = field.GetCustomAttribute<DescriptionAttribute>();
                if (attr != null && attr.Description.Equals(value)) {
                    var rvalue = (T)Enum.Parse(typeof(T), field.Name);
                    return rvalue;
                }
            }
            return default(T);
        }

        public static string GetMapping(Target target, object value) {
            if (value is Enum) {
                var mappings = value.GetType().GetRuntimeField(value.ToString()).GetCustomAttributes<MapAttribute>();
                foreach (var mapping in mappings) {
                    if (mapping.Target.Equals(target)) {
                        return mapping.Value;
                    }
                }
                return null;
            }
            return null;
        }
        public static T FromMapping<T>(Target target, object value) {
            var fields = typeof(T).GetRuntimeFields();            
            foreach (var field in fields) {
                var attr = field.GetCustomAttribute<MapAttribute>();
                if (attr != null && attr.Value.Equals(value) && attr.Target.Equals(target)) {
                    var rvalue = (T)Enum.Parse(typeof(T), field.Name);
                    return rvalue;
                }
            }
            return default(T);

        }
    }

    //public class EnumUtils {
        //public static bool IsDefined<V>(byte value) where V : System.Enum {
        //    return Parse<V>(value) != null;
        //}
        //public static V Parse<V>(byte value) where V : System.Enum {
        //    ReverseByteEnumMap<V> map = new ReverseByteEnumMap<V>();
        //    return map.Get(value);
        //}
        //public static V Parse<V>(string value) where V : System.Enum {
        //    ReverseStringEnumMap<V> map = new ReverseStringEnumMap<V>();
        //    return map.Get(value);
        //}
        //public static V Parse<V>(int value) where V : System.Enum {
        //    ReverseIntEnumMap<V> map = new ReverseIntEnumMap<V>();
        //    return map.Get(value);
        //}
    //}
}
