using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Terminals.Abstractions
{
    public interface IAccountInputResponse : IDeviceResponse
    {
        int EntryMode { get; set; }
        string Track1Data { get; set; }
        string Track2Data { get; set; }
        string Track3Data { get; set; }
        string PAN {  get; set; }
        string ExpiryDate { get; set; }
        string QRCode { get; set; }
        string KSN { get; set; }
        string AdditionalInfo { get; set; }
    }
}