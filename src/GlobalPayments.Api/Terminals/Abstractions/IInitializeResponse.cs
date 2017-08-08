using System;
namespace GlobalPayments.Api.Terminals.Abstractions {
    public interface IInitializeResponse : IDeviceResponse {
        string SerialNumber { get; set; }
    }
}
