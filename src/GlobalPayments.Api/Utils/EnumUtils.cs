using System;
using System.Reflection;

namespace GlobalPayments.Api.Utils {
    public class DescriptionAttribute : Attribute {
        public string Description { get; set; }

        public DescriptionAttribute(string description) {
            Description = description;
        }
    }

    internal class EnumConverter {
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
    }
}
