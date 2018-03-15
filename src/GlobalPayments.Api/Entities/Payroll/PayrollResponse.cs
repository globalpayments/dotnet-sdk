using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Entities.Payroll {
    internal abstract class BasePayrollResponse {
        internal int TotalRecords { get; private set; }
        internal List<JsonDoc> RawResults { get; private set; }
        public DateTime? Timestamp { get; private set; }
        internal int StatusCode { get; private set; }
        internal string ResponseMessage { get; private set; }

        internal BasePayrollResponse(string rawResponse) {
            var doc = JsonDoc.Parse(rawResponse);
            MapResponseValues(doc);

            if (StatusCode != 200) {
                throw new ApiException(ResponseMessage);
            }
        }

        internal virtual void MapResponseValues(JsonDoc doc) {
            TotalRecords = doc.GetValue<int>("TotalRecords");
            RawResults = doc.GetEnumerator("Results") as List<JsonDoc>;
            Timestamp = doc.GetValue<DateTime?>("Timestamp", (input) => {
                if(input != null)
                    return DateTime.Parse(input.ToString());
                return null;
            });
            StatusCode = doc.GetValue<int>("StatusCode");
            ResponseMessage = doc.GetValue<string>("ResponseMessage");
        }
    }

    internal class PayrollResponse<TResult> : BasePayrollResponse where TResult : PayrollEntity {
        public List<TResult> Results { get; set; }

        public PayrollResponse(string rawResponse, PayrollEncoder encoder) : base(rawResponse) {
            Results = new List<TResult>();
            if (RawResults != null) {
                foreach (var result in RawResults) {
                    var item = Activator.CreateInstance(typeof(TResult)) as TResult;
                    item.FromJson(result, encoder);
                    Results.Add(item);
                }
            }
        }
    }
}
