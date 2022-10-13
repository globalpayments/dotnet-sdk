using System;
using System.Linq.Expressions;
using System.Reflection;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Builders {
    internal class ValidationTarget {
        internal Validations parent;
        internal ValidationClause precondition;
        internal ValidationClause clause;

        internal Enum type;
        internal Enum constraint;

        public ValidationTarget(Validations parent, Enum type) {
            this.parent = parent;
            this.type = type;
        }

        public ValidationTarget With(Enum constraint) {
            this.constraint = constraint;
            return this;
        }

        public ValidationClause Check<T>(Expression<Func<T>> property) {
            var prop = ((MemberExpression)property.Body).Member as PropertyInfo;
            if (prop == null) {
                throw new BuilderException();
            }
            clause = new ValidationClause(parent, this, prop);
            return clause;
        }      

        public ValidationClause When<T>(Expression<Func<T>> property) {
            var prop = ((MemberExpression)property.Body).Member as PropertyInfo;
            if (prop == null) {
                throw new BuilderException();
            }
            precondition = new ValidationClause(parent, this, prop, true);
            return precondition;
        }
    }
}
