﻿using GlobalPayments.Api.Entities;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GlobalPayments.Api.Builders {
    internal class ValidationClause {
        internal Validations parent;
        internal ValidationTarget target;
        internal PropertyInfo property;
        internal string subProperty;
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
                if (subProperty == null) {
                    var value = property.GetValue(builder);
                    return value != null;
                }
                else {
                    var parentValue = property.GetValue(builder);
                    if (parentValue == null) {
                        subProperty = null;
                        return false;
                    }

                    var value = parentValue.GetType().GetProperty(subProperty).GetValue(parentValue);
                    return value != null;
                    
                }
            };
            this.message = message ?? string.Format("{0} cannot be null for this transaction type.", string.IsNullOrEmpty(subProperty) ? property.Name : subProperty);
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

        public ValidationTarget Equals(object expected, string message = null) {
            callback = (builder) => {
                var value = property.GetValue(builder);
                return value.Equals(expected);
            };
            this.message = message ?? string.Format("{0} was not the expected value {1}", property.Name, expected.ToString());
            if (precondition)
                return target;
            else return parent.For(target.type).With(target.constraint);
        }

        public ValidationTarget DoesNotEqual(object expected, string message = null) {
            callback = (builder) => {
                var value = property.GetValue(builder);
                return !value.Equals(expected);
            };
            this.message = message ?? string.Format("{0} cannot be the value {1}", property.Name, expected.ToString());
            if (precondition)
                return target;
            else return parent.For(target.type).With(target.constraint);
        }

        public ValidationTarget Is<T>(string message = null) {
            callback = (builder) => {
                var value = property.GetValue(builder);
                return value is T;
            };
            this.message = message ?? string.Format("{0} must be of type {1}", property.Name, typeof(T).Name);
            if (precondition) {
                return target;
            }
            else return parent.For(target.type).With(target.constraint);
        }

        public ValidationTarget IsTrue(string message = null) {
            return Equals(true, message);
        }
        public ValidationTarget IsFalse(string message = null) {
            return Equals(false, message);
        }

        /// <summary>
        /// Validate a property from a parent class
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public ValidationClause PropertyOf(string property) {
            if (!string.IsNullOrEmpty(property)) {
                this.subProperty = property;
            }
            return this;
        }
    }
}
