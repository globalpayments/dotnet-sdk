using System.Collections.Generic;
using System.Linq;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;

namespace GlobalPayments.Api.Entities {
    public abstract class RecurringEntity<TResult> : IRecurringEntity where TResult : class, IRecurringEntity {
        public string Id { get; set; }
        public string Key { get; set; }

        public TResult Create() {
            return RecurringService.Create(this as TResult);
        }

        public void Delete(bool force = false) {
            try {
                RecurringService.Delete(this as TResult, force);
            }
            catch (ApiException exc) {
                throw new ApiException("Failed to delete record, see inner exception for more details", exc);
            }
        }

        public static TResult Find(string id) {
            var client = ServicesContainer.Instance.GetRecurringClient();
            if (client.SupportsRetrieval) {
                var identifier = GetIdentifierName();
                var response = RecurringService.Search<List<TResult>>()
                    .AddSearchCriteria(identifier, id)
                    .Execute();
                var entity = response.FirstOrDefault();
                if (entity != null)
                    return RecurringService.Get<TResult>(entity.Key);
                return null;
            }
            throw new UnsupportedTransactionException();
        }

        public static List<TResult> FindAll() {
            var client = ServicesContainer.Instance.GetRecurringClient();
            if (client.SupportsRetrieval) {
                return RecurringService.Search<List<TResult>>().Execute();
            }
            throw new UnsupportedTransactionException();
        }

        private static string GetIdentifierName() {
            if (typeof(TResult).Equals(typeof(Customer)))
                return "customerIdentifier";
            else if (typeof(TResult).Equals(typeof(RecurringPaymentMethod)))
                return "paymentMethodIdentifier";
            else if (typeof(TResult).Equals(typeof(Schedule)))
                return "scheduleIdentifier";
            return string.Empty;
        }

        public void SaveChanges() {
            try {
                RecurringService.Edit(this as TResult);
            }
            catch (ApiException exc) {
                throw new ApiException("Update failed, see inner exception for more details", exc);
            }
        }
    }
}
