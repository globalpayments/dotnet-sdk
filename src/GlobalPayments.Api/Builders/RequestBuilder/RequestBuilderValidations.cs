using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals;
using System;
using System.Reflection;

namespace GlobalPayments.Api.Builders.RequestBuilder {
    internal class RequestBuilderValidations {

        private Validations _validations;
        public RequestBuilderValidations(Validations validations) {
            _validations = validations;
        }
        /// <summary>
        /// Validate method for Terminals
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="actionType"></param>
        public void Validate<T>(T builder, TerminalReportType actionType) {
            foreach (var key in _validations.rules.Keys) {
                if (!key.Equals(actionType)) {
                    return;
                }
                foreach (var validation in _validations.rules[key]) {
                    if (validation.clause == null) continue;

                    // modifier
                    if (validation.constraint != null) {
                        Enum modifier = GetPropertyValue(builder, validation.constraint);
                        if (!Equals(validation.constraint, modifier))
                            continue;
                    }

                    // check precondition
                    if (validation.precondition != null) {
                        if (!validation.precondition.callback(actionType))
                            continue;
                    }

                    if (!validation.clause.callback(builder))
                        throw new BuilderException(validation.clause.message);
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
