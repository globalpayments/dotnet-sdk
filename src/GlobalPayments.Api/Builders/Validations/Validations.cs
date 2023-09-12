using System;
using System.Reflection;
using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using System.Linq;

namespace GlobalPayments.Api.Builders {
    internal class Validations {
        public Dictionary<Enum, List<ValidationTarget>> rules;

        public Validations() {
            rules = new Dictionary<Enum, List<ValidationTarget>>();
        }

        public ValidationTarget For(Enum type) {
            if (!rules.ContainsKey(type))
                rules.Add(type, new List<ValidationTarget>());

            var target = new ValidationTarget(this, type);
            rules[type].Add(target);
            return target;
        }

        public void Validate<T>(BaseBuilder<T> builder) {
            foreach (var key in rules.Keys) {
                Enum value = GetPropertyValue(builder, key);
                if (value == null && builder is TransactionBuilder<T>) {
                    value = GetPropertyValue(((TransactionBuilder<T>)builder).PaymentMethod, key);
                    if (value == null)
                        continue;
                }

                if (key.HasFlag(value)) {
                    foreach (var validation in rules[key]) {
                        Type enumType = key.GetType();
                        var hasFlagAttribute = enumType.GetTypeInfo().CustomAttributes.Any(x => x.AttributeType == typeof(FlagsAttribute));

                        if (!(hasFlagAttribute) && !(value.Equals(key))) continue;

                        if (validation.clause == null) continue;

                        // modifier
                        if (validation.constraint != null) {
                            Enum modifier = GetPropertyValue(builder, validation.constraint);
                            if (!Equals(validation.constraint, modifier))
                                continue;
                        }

                        // check precondition
                        if (validation.precondition != null) {
                            if (!validation.precondition.callback(builder))
                                continue;
                        }

                        if (!validation.clause.callback(builder))
                            throw new BuilderException(validation.clause.message);
                    }
                }
            }
        }

        private Enum GetPropertyValue(object obj, object comp) {
            if (obj == null) return null;

            foreach (var propInfo in obj.GetType().GetRuntimeProperties()) {
                if (propInfo.Name == comp.GetType().Name)
                    return (Enum)propInfo.GetValue(obj);
            }
            return null;
        }
    }
}
