using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.Terminals.UPA.Responses {
    public class UpaSignatureResponse : DeviceResponse, ISignatureResponse {
        
        public byte[] SignatureData { get; set; }
        const string INVALID_RESPONSE_FORMAT = "The response received is not in the proper format.";
        public string SigData { get; set; }
        public UpaSignatureResponse(JsonDoc root) {
            if (!TerminalUtilities.IsGpApiResponse(root)) {
                var firstDataNode = root.Get("data");
                if (firstDataNode == null) {
                    throw new MessageException(INVALID_RESPONSE_FORMAT);
                }

                var cmdResult = firstDataNode.Get("cmdResult");
                if (cmdResult == null) {
                    throw new MessageException(INVALID_RESPONSE_FORMAT);
                }

                Status = cmdResult.GetValue<string>("result");
                if (string.IsNullOrEmpty(Status)) {
                    var errorCode = cmdResult.GetValue<string>("errorCode");
                    var errorMsg = cmdResult.GetValue<string>("errorMessage");
                    DeviceResponseText = $"Error: {errorCode} - {errorMsg}";
                }
                else {
                    // If the Status is not "Success", there is either nothing to process, or something else went wrong.
                    // Skip the processing of the rest of the message, as we'll likely hit null reference exceptions
                    if (Status == "Success") {
                        var secondDataNode = firstDataNode.Get("data");
                        if (secondDataNode == null) {
                            throw new MessageException(INVALID_RESPONSE_FORMAT);
                        }
                        var signatureData = secondDataNode.GetValue<string>("signatureData");
                        SignatureData = Convert.FromBase64String(signatureData);
                    }
                    else {
                        // the only other option is "Failed"
                        var errorCode = cmdResult.GetValue<string>("errorCode");
                        var errorMsg = cmdResult.GetValue<string>("errorMessage");
                        DeviceResponseText = $"Error: {errorCode} - {errorMsg}";
                    }
                }
            }
            else {
                DeviceResponseText = root.GetValue<string>("status");
                var responseText = root.Get("action").GetValue<string>("result_code");
                DeviceResponseCode = responseText;
                var secondDataNode = root.Get("response").Get("data");
                if (secondDataNode == null){
                    throw new MessageException(INVALID_RESPONSE_FORMAT);
                }
                var signatureData = secondDataNode.GetValue<string>("signatureData");
                SignatureData = Convert.FromBase64String(signatureData);
            }

        }
    }
}
