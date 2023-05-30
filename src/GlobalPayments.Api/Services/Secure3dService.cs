using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Services {
    public class Secure3dService {
        public static Secure3dBuilder CheckEnrollment(IPaymentMethod paymentMethod) {
            return new Secure3dBuilder(TransactionType.VerifyEnrolled)
                    .WithPaymentMethod(paymentMethod);
        }

        public static Secure3dBuilder InitiateAuthentication(IPaymentMethod paymentMethod, ThreeDSecure secureEcom) {
            if (paymentMethod is ISecure3d) {
                ((ISecure3d)paymentMethod).ThreeDSecure = secureEcom;
            }
            return new Secure3dBuilder(TransactionType.InitiateAuthentication)
                    .WithPaymentMethod(paymentMethod);
        }

        public static Secure3dBuilder GetAuthenticationData() {
            return new Secure3dBuilder(TransactionType.VerifySignature);
        }

    }
}
