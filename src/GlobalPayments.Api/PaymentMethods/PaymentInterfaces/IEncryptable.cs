using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods {
    interface IEncryptable {
        EncryptionData EncryptionData { get; set; }
    }
}
