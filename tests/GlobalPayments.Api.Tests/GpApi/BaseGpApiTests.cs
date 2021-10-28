using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.Tests.GpApi {
    public abstract class BaseGpApiTests {
        protected const string SUCCESS = "SUCCESS";
        protected const string DECLINED = "DECLINED";
        protected const string VERIFIED = "VERIFIED";
        protected const string CLOSED = "CLOSED";

        public static string APP_ID = "yDkdruxQ7hUjm8p76SaeBVAUnahESP5P";
        public static string APP_KEY = "o8C8CYrgXNELI46x";
        protected string GetMapping<T>(T value, Target target = Target.GP_API) where T : Enum {
            return EnumConverter.GetMapping(target, value);
        }
    }
}
