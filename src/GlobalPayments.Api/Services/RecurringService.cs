using System;
using System.Collections.Generic;
using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Services {
    public class RecurringService {
        public static T Create<T>(T entity, string configName = "default") where T : class, IRecurringEntity {
            var response = new RecurringBuilder<T>(TransactionType.Create, entity).Execute(configName);
            return response as T;
        }

        public static T Delete<T>(T entity, bool force = false, string configName = "default") where T : class, IRecurringEntity {
            var response = new RecurringBuilder<T>(TransactionType.Delete, entity).Execute(configName);
            return response as T;
        }

        public static T Edit<T>(T entity, string configName = "default") where T : class, IRecurringEntity {
            var response = new RecurringBuilder<T>(TransactionType.Edit, entity).Execute(configName);
            return response as T;
        }

        public static T Get<T>(string key, string configName = "default") where T : class, IRecurringEntity {
            var entity = Activator.CreateInstance<T>() as IRecurringEntity;
            entity.Key = key;

            var response = new RecurringBuilder<T>(TransactionType.Fetch, entity).Execute(configName);
            return response as T;
        }

        public static RecurringBuilder<T> Search<T>() where T : class, IEnumerable<IRecurringEntity> {
            return new RecurringBuilder<T>(TransactionType.Search);
        }
    }
}
