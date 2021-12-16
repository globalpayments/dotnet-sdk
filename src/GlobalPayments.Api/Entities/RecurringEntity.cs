using System.Collections.Generic;
using System.Linq;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Base implementation for recurring resource types.
    /// </summary>
    public abstract class RecurringEntity<TResult> : IRecurringEntity where TResult : class, IRecurringEntity {
        /// <summary>
        /// All resource should be supplied a merchant-/application-defined ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// All resources should be supplied a gateway-defined ID.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Creates a resource
        /// </summary>
        /// <returns>TResult</returns>
        public TResult Create(string configName = "default") {
            return RecurringService.Create(this as TResult, configName);
        }

        /// <summary>
        /// Delete a record from the gateway.
        /// </summary>
        /// <param name="force">Indicates if the deletion should be forced</summary>
        /// <exception cref="ApiException">Thrown when the record cannot be deleted.</exception>
        public void Delete(bool force = false, string configName = "default") {
            try {
                RecurringService.Delete(this as TResult, force, configName);
            }
            catch (ApiException exc) {
                throw new ApiException("Failed to delete record, see inner exception for more details", exc);
            }
        }

        /// <summary>
        /// Searches for a specific record by `id`.
        /// </summary>
        /// <param name="id">The ID of the record to find</summary>
        /// <returns>`TResult` or `null` if the record cannot be found.</returns>
        /// <exception cref="UnsupportedTransactionException">
        /// Thrown when gateway does not support retrieving recurring records.
        /// </exception>
        public static TResult Find(string id, string configName = "default") {
            var client = ServicesContainer.Instance.GetRecurringClient(configName);
            if (client.SupportsRetrieval) {
                var identifier = GetIdentifierName();
                var response = RecurringService.Search<List<TResult>>()
                    .AddSearchCriteria(identifier, id)
                    .Execute();
                var entity = response.FirstOrDefault();
                if (entity != null && entity.Id == id)
                    return RecurringService.Get<TResult>(entity.Key);
                return null;
            }
            throw new UnsupportedTransactionException();
        }

        /// <summary>
        /// Lists all records of type `TResult`.
        /// </summary>
        /// <exception cref="UnsupportedTransactionException">
        /// Thrown when gateway does not support retrieving recurring records.
        /// </exception>
        public static List<TResult> FindAll(string configName = "default") {
            var client = ServicesContainer.Instance.GetRecurringClient(configName);
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

        /// <summary>
        /// The current record should be updated.
        /// </summary>
        /// <remarks>
        /// Any modified properties will be persisted with the gateway.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when the record cannot be updated.</exception>
        public void SaveChanges(string configName = "default") {
            try {
                RecurringService.Edit(this as TResult, configName);
            }
            catch (ApiException exc) {
                throw new ApiException("Update failed, see inner exception for more details", exc);
            }
        }
    }
}
