using System;
using System.Threading;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Tests.GpApi {
    public abstract class BaseGpApiTests {
        protected const string SUCCESS = "SUCCESS";
        protected const string DECLINED = "DECLINED";
        protected const string VERIFIED = "VERIFIED";
        protected const string CLOSED = "CLOSED";

        protected static string APP_ID = "yDkdruxQ7hUjm8p76SaeBVAUnahESP5P";
        protected static string APP_KEY = "o8C8CYrgXNELI46x";
        
        protected static string APP_ID_FOR_DCC = "mivbnCh6tcXhrc6hrUxb3SU8bYQPl9pd";
        protected static string APP_KEY_FOR_DCC = "Yf6MJDNJKiqObYAb";
        
        protected static string APP_ID_FOR_BATCH = "P3LRVjtGRGxWQQJDE345mSkEh2KfdAyg";
        protected static string APP_KEY_FOR_BATCH = "ockJr6pv6KFoGiZA";

        protected static readonly int expMonth = DateTime.Now.Month;
        protected static readonly int expYear = DateTime.Now.Year + 1;
        
        protected string GetMapping<T>(T value, Target target = Target.GP_API) where T : Enum {
            return EnumConverter.GetMapping(target, value);
        }

        protected void waitForGpApiReplication() {
            Thread.Sleep(2000);
        }
    }
}