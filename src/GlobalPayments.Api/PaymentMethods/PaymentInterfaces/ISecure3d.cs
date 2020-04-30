using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.PaymentMethods {
    interface ISecure3d {
        ThreeDSecure ThreeDSecure { get; set; }
    }
}
