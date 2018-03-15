using System;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;

namespace GlobalPayments.Api.Entities.Payroll {
    public class ClientInfo : PayrollEntity {
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public int? FederalEin { get; set; }

        internal override void FromJson(JsonDoc doc, PayrollEncoder encoder) {
            ClientCode = doc.GetValue("ClientCode", encoder.Decode);
            ClientName = doc.GetValue<string>("ClientName");
            FederalEin = int.Parse(doc.GetValue("FederalEin", encoder.Decode));
        }

        internal PayrollRequest GetClientInfoRequest(PayrollEncoder encoder, object[] args) {
            var request = new JsonDoc()
                .Set("FederalEin", encoder.Encode(FederalEin))
                .ToString();

            return new PayrollRequest {
                Endpoint = "/api/pos/client/getclients",
                RequestBody = request
            };
        }

        internal PayrollRequest GetCollectionRequestByType(PayrollEncoder encoder, object[] args) {
            var endpoints = new Dictionary<Type, string> {
                { typeof(TerminationReason), @"/api/pos/termination/GetTerminationReasons" },
                { typeof(WorkLocation),  @"/api/pos/worklocation/GetWorkLocations" },
                { typeof(LaborField), @"/api/pos/laborField/GetLaborFields" },
                { typeof(PayGroup), @"/api/pos/payGroup/GetPayGroups" },
                { typeof(PayItem), @"/api/pos/payItem/GetPayItems" }
            };

            var type = args[0] as Type;
            
            var requestBody = new JsonDoc()
                .Set("ClientCode", encoder.Encode(ClientCode))
                .ToString();

            return new PayrollRequest {
                Endpoint = endpoints[type],
                RequestBody = requestBody
            };
        }
    }
}
