using System;
using System.Reflection;

namespace GlobalPayments.Api.Builders {
    internal class ValidationClause {
        internal Validations parent;
        internal ValidationTarget target;
        internal PropertyInfo property;
        internal Func<object, bool> callback;
        internal string message;
        internal bool precondition;

        public ValidationClause(Validations parent, ValidationTarget target, PropertyInfo property, bool precondition = false) {
            this.parent = parent;
            this.target = target;
            this.property = property;
            this.precondition = precondition;
        }

        public ValidationTarget IsNotNull(string message = null) {
            callback = (builder) => {
                var value = property.GetValue(builder);
                return value != null;
            };
            this.message = message ?? string.Format("{0} cannot be null for this transaction type.", property.Name);
            if (precondition)
                return target;
            else return parent.For(target.type).With(target.constraint);
        }

        public ValidationTarget IsNull(string message = null) {
            callback = (builder) => {
                var value = property.GetValue(builder);
                return value == null;
            };
            this.message = message ?? string.Format("{0} cannot be set for this transaction type.", property.Name);
            if (precondition)
                return target;
            else return parent.For(target.type).With(target.constraint);
        }
    }
}
