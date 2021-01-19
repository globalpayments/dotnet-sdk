using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Entities {
    internal abstract class BaseGpApiResponse {
        internal int TotalCount { get; private set; }
        internal string MerchantId { get; private set; }
        internal string MerchantName { get; private set; }
        internal string AccountId { get; private set; }
        internal string AccountName { get; private set; }
        internal List<JsonDoc> RawResults { get; private set; }

        private string ResultsField { get; set; }

        internal BaseGpApiResponse(string rawResponse, string resultsField) {
            ResultsField = resultsField;

            var doc = JsonDoc.Parse(rawResponse);

            MapResponseValues(doc);

            //if (StatusCode != 200)
            //{
            //    throw new ApiException(ResponseMessage);
            //}
        }

        internal virtual void MapResponseValues(JsonDoc doc) {
            TotalCount = doc.GetValue<int>("total_count");

            MerchantId = doc.GetValue<string>("merchant_id");
            MerchantName = doc.GetValue<string>("merchant_name");
            AccountId = doc.GetValue<string>("account_id");
            AccountName = doc.GetValue<string>("account_name");
            if (!string.IsNullOrEmpty(ResultsField))
                RawResults = doc.GetEnumerator(ResultsField) as List<JsonDoc>;
        }
    }

    internal class GpApiResponse<T> : BaseGpApiResponse where T : GpApiEntity {
        public List<T> Results { get; set; }

        public GpApiResponse(string rawResponse, string resultsField) : base(rawResponse, resultsField) {
            Results = new List<T>();
            if (RawResults != null) {
                foreach (var result in RawResults) {
                    var item = Activator.CreateInstance(typeof(T)) as T;
                    item.FromJson(result);
                    Results.Add(item);
                }
            }
        }
    }
}
