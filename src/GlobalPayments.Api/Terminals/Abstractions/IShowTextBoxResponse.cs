using GlobalPayments.Api.Entities.Enums;

namespace GlobalPayments.Api.Terminals.Abstractions
{
    public interface IShowTextBoxResponse : IDeviceResponse
    {
        string ButtonNumberSelected { get; set; }
        bool SignStatus { get; set; }
        string SigData { get; set; }
        string Text { get; set; }
    }
}