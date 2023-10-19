using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Terminals.UPA.Responses
{
    internal class UpaGiftCardResponse: DeviceResponse
    {
        const string INVALID_RESPONSE_FORMAT = "The response received is not in the proper format.";

        public string AcquisitionType { get; set; }
        public string LuhnCheckPassed { get; set; }
        public string DataEncryptionType { get; set; }
        public Pan Pan { get; set; }        
        public TrackData TrackData { get; set; }       
        public string EmvTags { get; set; }
        public string ExpDate { get; set; }
        public int Cvv { get; set; }
        public string ScannedData { get; set; }
        public object PinDUKPT { get; set; }        
        public ThreeDesDukpt ThreeDesDukpt { get; set; }


        public UpaGiftCardResponse(JsonDoc root)
        {
            string test = root.ToString();
            var firstDataNode = root.Get("data");
            if (firstDataNode == null)
            {
                throw new MessageException(INVALID_RESPONSE_FORMAT);
            }

            var cmdResult = firstDataNode.Get("cmdResult");
            if (cmdResult == null)
            {
                throw new MessageException(INVALID_RESPONSE_FORMAT);
            }

            Status = cmdResult.GetValue<string>("result");
            if (string.IsNullOrEmpty(Status))
            {
                var errorCode = cmdResult.GetValue<string>("errorCode");
                var errorMsg = cmdResult.GetValue<string>("errorMessage");
                DeviceResponseText = $"Error: {errorCode} - {errorMsg}";
            }
            else
            {
                // If the Status is not "Success", there is either nothing to process, or something else went wrong.
                // Skip the processing of the rest of the message, as we'll likely hit null reference exceptions
                if (Status == "Success")
                {
                    var secondDataNode = firstDataNode.Get("data");
                    if (secondDataNode == null)
                    {
                        throw new MessageException(INVALID_RESPONSE_FORMAT);
                    }
                    AcquisitionType = secondDataNode.GetValue<string>("acquisitionType");
                    var threedesDukpt = secondDataNode.Get("3DesDukpt");
                    ThreeDesDukpt = new ThreeDesDukpt();
                    ThreeDesDukpt.EncryptedBlob = threedesDukpt.GetValue<string>("encryptedBlob");
                    ThreeDesDukpt.Ksn = threedesDukpt.GetValue<string>("Ksn");
                    DataEncryptionType = secondDataNode.GetValue<string>("dataEncryptionType");
                    EmvTags = secondDataNode.GetValue<string>("EmvTags");
                    DeviceResponseCode = "00";
                    DeviceResponseText = "Success";
                }
                else
                {
                    // the only other option is "Failed"
                    var errorCode = cmdResult.GetValue<string>("errorCode");
                    var errorMsg = cmdResult.GetValue<string>("errorMessage");
                    DeviceResponseText = $"Error: {errorCode} - {errorMsg}";
                }
            }

        }
    }

    public class PinDUKPT
    {
        public string Pinblock { get; set; }
        public string Ksn { get; set; }
    }

    public class ThreeDesDukpt
    {
        public string EncryptedBlob { get; set; }
        public string Ksn { get; set; }
    }

    public class Pan
    {
        public string ClearPAN { get; set; }
        public string MaskedPAN { get; set; }
        public string EncryptedPAN { get; set; }
    }

    public class TrackData
    {
        public string ClearTrack2 { get; set; }
        public string MaskedTrack2 { get; set; }
        public string ClearTrack1 { get; set; }
        public string MaskedTrack1 { get; set; }
        public string ClearTrack3 { get; set; }
        public string MaskedTrack3 { get; set; }
    }
}
