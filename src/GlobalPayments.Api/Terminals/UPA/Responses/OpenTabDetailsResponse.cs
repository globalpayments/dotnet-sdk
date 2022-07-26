using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;

namespace GlobalPayments.Api.Terminals.UPA
{
    public class OpenTabDetailsResponse : ITerminalReport {
        private string invalidFormatMessage = "The response received is not in the proper format.";
        public string Status { get; set; }
        public string Command { get; set; }
        public string Version { get; set; }
        public string DeviceResponseCode { get; set; }
        public string DeviceResponseText { get; set; }
        public string ReferenceNumber { get; set; }

        public string Multiplemessage { get; set; }
        public string MerchantName { get; set; }
        public List<OpenTab> OpenTabs = new List<OpenTab>();

        public OpenTabDetailsResponse(JsonDoc root) {
            var firstDataNode = root.Get("data");
            if (firstDataNode == null) {
                throw new MessageException(invalidFormatMessage);
            }

            var cmdResult = firstDataNode.Get("cmdResult");
            if (cmdResult == null) {
                throw new MessageException(invalidFormatMessage);
            }

            Status = cmdResult.GetValue<string>("result");
            if (string.IsNullOrEmpty(Status)) {
                var errorCode = cmdResult.GetValue<string>("errorCode");
                var errorMsg = cmdResult.GetValue<string>("errorMessage");
                DeviceResponseText = $"Error: {errorCode} - {errorMsg}";
            }
            else {
                if (Status == "Success") {
                    var secondDataNode = firstDataNode.Get("data");
                    if (secondDataNode == null) {
                        throw new MessageException(invalidFormatMessage);
                    }

                    MerchantName = secondDataNode.GetValue<string>("merchantName");
                    Multiplemessage = secondDataNode.GetValue<string>("multipleMessage");
                    var openTabList = secondDataNode.GetArray<JsonDoc>("OpenTabDetails");

                    foreach (JsonDoc openTab in openTabList) {
                        OpenTab tab = new OpenTab();
                        tab.AuthorizedAmount = openTab.GetValue<decimal>("authorizedAmount");
                        tab.CardType = openTab.GetValue<string>("cardType");
                        tab.MaskedPan = openTab.GetValue<string>("maskedPan");
                        tab.TransactionId = openTab.GetValue<string>("referenceNumber");
                        OpenTabs.Add(tab);
                    }
                }
                else {
                    var errorCode = cmdResult.GetValue<string>("errorCode");
                    var errorMsg = cmdResult.GetValue<string>("errorMessage");
                    DeviceResponseText = $"Error: {errorCode} - {errorMsg}";
                }
            }
        }
    }
}
