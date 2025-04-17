using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.GpApi;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace GlobalPayments.Api.Builders.RequestBuilder.GpApi {
    internal class GpApiRecurringRequestBuilder<T> : IRequestBuilder<RecurringBuilder<T>> where T : class {
        private static RecurringBuilder<T> _builder;
        public Request BuildRequest(RecurringBuilder<T> builder, GpApiConnector gateway) {
            _builder = builder;
            JsonDoc data = null;

            var merchantUrl = !string.IsNullOrEmpty(gateway.GpApiConfig.MerchantId) ? $"/merchants/{gateway.GpApiConfig.MerchantId}" : string.Empty;
            
            switch (builder.TransactionType) {
                case TransactionType.Create:
                    if (builder.Entity is Customer) {
                        data = PreparePayerRequest();
                    }
                    return new Request {
                        Verb = HttpMethod.Post,
                        Endpoint = $"{merchantUrl}{GpApiRequest.PAYERS_ENDPOINT}",
                        RequestBody = data.ToString(),
                    };

                case TransactionType.Edit:
                    if (builder.Entity is Customer) {
                        data = PreparePayerRequest();
                    }
                    return new Request {
                        Verb = new HttpMethod("PATCH"),
                        Endpoint = $"{merchantUrl}{GpApiRequest.PAYERS_ENDPOINT}/{builder.Entity.Id}",
                        RequestBody = data.ToString(),
                    };

                default:
                    return null;
            }
        }

        private JsonDoc PreparePayerRequest() {
            var customer = _builder.Entity as Customer;
            var data = new JsonDoc()
               .Set("first_name", customer.FirstName)
               .Set("last_name", customer.LastName)
               .Set("reference", customer.Key);

            if (customer.PaymentMethods?.Count > 0) {
                List<Dictionary<string, object>> paymentsToAdd = new List<Dictionary<string, object>>();
                foreach (var payments in customer.PaymentMethods) {
                    var item = new Dictionary<string, object>();
                    item.Add("id", payments.Id);
                    item.Add("default", customer.PaymentMethods.First().Id == payments.Id ? "YES" : "NO");                    
                    paymentsToAdd.Add(item);
                }
                data.Set("payment_methods", paymentsToAdd);
            }
            return data;
        }
    }
}
