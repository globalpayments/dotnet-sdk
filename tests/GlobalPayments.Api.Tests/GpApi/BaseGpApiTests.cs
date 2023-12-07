using System;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Utils;
using GlobalPayments.Api.Utils.Logging;

namespace GlobalPayments.Api.Tests.GpApi {
    public abstract class BaseGpApiTests {
        protected const string Success = "SUCCESS";
        protected const string Declined = "DECLINED";
        protected const string Verified = "VERIFIED";
        protected const string Closed = "CLOSED";

        protected const string AppId = "4gPqnGBkppGYvoE5UX9EWQlotTxGUDbs";
        protected const string AppKey = "FQyJA5VuEQfcji2M";

        protected const string AppIdForMerchant = "A1feRdMmEB6m0Y1aQ65H0bDi9ZeAEB2t";
        protected const string AppKeyForMerchant = "5jPt1OpB6LLitgi7";

        protected static readonly int ExpMonth = DateTime.Now.Month;
        protected static readonly int ExpYear = DateTime.Now.Year + 1;

        protected static readonly DateTime StartDate = DateTime.UtcNow.AddDays(-30);
        protected static readonly DateTime EndDate = DateTime.UtcNow;

        protected static GpApiConfig GpApiConfigSetup( string appId, string appKey, Channel channel)
        {
            var gpApiConfig = new GpApiConfig {
                AppId = appId,
                AppKey = appKey,
                Channel = channel,
                ChallengeNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                MethodNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                MerchantContactUrl = "https://enp4qhvjseljg.x.pipedream.net/",
                // RequestLogger = new RequestFileLogger(@"C:\temp\transit\finger.txt"),
                RequestLogger = new RequestConsoleLogger(),
                EnableLogging = true,
                Country = "US",
                AccessTokenInfo = new AccessTokenInfo() { TransactionProcessingAccountName = "transaction_processing", RiskAssessmentAccountName = "EOS_RiskAssessment"}
                // DO NO DELETE - usage example for some settings
                // DynamicHeaders = new Dictionary<string, string>
                // {
                //     ["x-gp-platform"] = "prestashop;version=1.7.2",
                //     ["x-gp-extension"] = "coccinet;version=2.4.1"
                // }
            };

            return gpApiConfig;
        }

        protected string GetMapping<T>(T value, Target target = Target.GP_API) where T : Enum {
            return EnumConverter.GetMapping(target, value);
        }

        protected static void WaitForGpApiReplication() {
            Thread.Sleep(2000);
        }
    }
}