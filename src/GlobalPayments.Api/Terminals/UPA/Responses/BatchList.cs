using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;

namespace GlobalPayments.Api.Terminals.UPA.Responses {
    public class BatchList : ITerminalReport {       
        public string Status { get; set; }
        public string Command { get; set; }
        public string Version { get; set; }
        public string DeviceResponseCode { get; set; }
        public string DeviceResponseText { get; set; }
        public string ReferenceNumber { get; set; }
        public string EcrId { get; set; }
        public List<int> Batches { get; set; }

        const string INVALID_RESPONSE_FORMAT = "The response received is not in the proper format.";

        public BatchList(JsonDoc jsonResponse) {
            ParseResponse(jsonResponse);
        }

        private void ParseResponse(JsonDoc jsonResponse) {
            if ((jsonResponse.Get("data") == null) ||
                (jsonResponse.Get("data")?.Get("cmdResult") == null)) {
                throw new GatewayException(INVALID_RESPONSE_FORMAT);
            }
            var firstDataNode = jsonResponse.Get("data");
            var cmdResult = firstDataNode.Get("cmdResult");

            Status = cmdResult.GetValue<string>("result") ?? null;
            Command = firstDataNode.GetValue<string>("response");
            EcrId = firstDataNode.GetValue<string>("ecrId") ?? null;
            if (string.IsNullOrEmpty(Status) || Status != "Success") {
                DeviceResponseText = $"Error: {cmdResult.GetValue<string>("errorCode")} - {cmdResult.GetValue<string>("errorMessage")}";
                return;
            }
            if (firstDataNode.Has("data")) {
                if (Batches == null) {
                    Batches = new List<int>();
                }

                foreach (var batch in firstDataNode.Get("data").GetArray<JsonDoc>("batchesAvail")) {
                    Batches.Add(batch.GetValue<int>("batchId"));
                }
            }
        }
    }
}
