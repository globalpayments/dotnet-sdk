using GlobalPayments.Api.Entities.Enums;

namespace GlobalPayments.Api.Terminals.Abstractions {   
    public interface ISafParamsResponse : IDeviceResponse {
        SafMode SAFMode { get; set; }
        string StartDateTime { get; set; }
        string EndDateTime { get; set; }
        string DurationInDays { get; set; }
        string MaxNumberOfRecord { get; set; }
        string TotalCeilingAmount { get; set; }
        string CeilingAmountPerCardType { get; set; }
        string HALOPerCardType { get; set; }
        string UploadMode { get; set; }
        string AutoUploadIntervalTime { get; set; }
        string DeleteSAFConfirmation { get; set; }
    }
}
