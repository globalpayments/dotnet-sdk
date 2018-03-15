using System;
using System.Linq;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;

namespace GlobalPayments.Api.Entities.Payroll {
    public class PayrollData : PayrollEntity {
        private List<PayrollRecord> _records;

        public PayrollData(PayrollRecord[] records = null) {
            _records = new List<PayrollRecord>(records);
        }

        internal override void FromJson(JsonDoc doc, PayrollEncoder encoder) {
            throw new NotImplementedException();
        }

        internal PayrollRequest PostPayrollRequest(PayrollEncoder encoder, object[] args) {
            var payrollRecords = _records.Select(p => p.ToJson(encoder)).ToList();

            var requestBody = string.Format("[{0}]", string.Join(",", payrollRecords));

            return new PayrollRequest {
                Endpoint = @"/api/pos/timeclock/PostPayData",
                RequestBody = requestBody
            };
        }
    }

    public class PayrollRecord {
        public int? RecordId { get; set; }
        public string ClientCode { get; set; }
        public int? EmployeeId { get; set; }
        public List<LaborField> PayItemLaborFields { get; set; }
        //public List<PayItem> PayItemTitle { get; set; }
        public string PayItemTitle { get; set; }
        public decimal? Hours { get; set; }
        public decimal? Dollars { get; set; }
        public decimal? PayRate { get; set; }

        internal string ToJson(PayrollEncoder encoder) {
            return new JsonDoc()
                .Set("RecordId", RecordId)
                .Set("ClientCode", encoder.Encode(ClientCode))
                .Set("EmployeeId", EmployeeId)
                .Set("PayItemLaborFields", PayItemLaborFields)
                .Set("PayItemTitle", PayItemTitle)
                .Set("Hours", Hours)
                .Set("Dollars", Dollars)
                .Set("PayRate", PayRate)
                .ToString();
        }
    }
}
