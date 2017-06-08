using System.Collections.Generic;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Builders {
    public class RecurringBuilder<TResult> : TransactionBuilder<TResult> where TResult : class {
        internal string Key { get; set; }
        internal string OrderId { get; set; }
        internal IRecurringEntity Entity { get; set; }
        internal Dictionary<string, string> SearchCriteria { get; set; }

        internal RecurringBuilder<TResult> AddSearchCriteria(string key, string value) {
            SearchCriteria.Add(key, value);
            return this;
        }

        internal RecurringBuilder(TransactionType type, IRecurringEntity entity = null) : base(type) {
            SearchCriteria = new Dictionary<string, string>();
            if (entity != null) {
                Entity = entity;
                Key = entity.Key;
            }
        }

        public override TResult Execute() {
            base.Execute();

            var client = ServicesContainer.Instance.GetRecurringClient();
            return client.ProcessRecurring(this);
        }

        protected override void SetupValidations() {
            Validations.For(TransactionType.Edit | TransactionType.Delete | TransactionType.Fetch).Check(() => Key).IsNotNull();
            Validations.For(TransactionType.Search).Check(() => SearchCriteria).IsNotNull();
        }
    }
}
