using System;
using System.Threading;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Tests.GpApi {
    public abstract class BaseGpApiTests {
        protected const string SUCCESS = "SUCCESS";
        protected const string DECLINED = "DECLINED";
        protected const string VERIFIED = "VERIFIED";
        protected const string CLOSED = "CLOSED";

        protected static readonly string AppId = "x0lQh0iLV0fOkmeAyIDyBqrP9U5QaiKc";
        protected static readonly string AppKey = "DYcEE2GpSzblo0ib";

        protected static readonly string AppIdForDcc = "mivbnCh6tcXhrc6hrUxb3SU8bYQPl9pd";
        protected static readonly string AppKeyForDcc = "Yf6MJDNJKiqObYAb";
        
        protected static readonly string AppIdForBatch = "P3LRVjtGRGxWQQJDE345mSkEh2KfdAyg";
        protected static readonly string AppKeyForBatch = "ockJr6pv6KFoGiZA";

        protected static readonly int ExpMonth = DateTime.Now.Month;
        protected static readonly int ExpYear = DateTime.Now.Year + 1;

        protected static readonly DateTime startDate = DateTime.UtcNow.AddDays(-30);
        protected static readonly DateTime endDate = DateTime.UtcNow;

        protected string GetMapping<T>(T value, Target target = Target.GP_API) where T : Enum {
            return EnumConverter.GetMapping(target, value);
        }

        protected static void WaitForGpApiReplication() {
            Thread.Sleep(2000);
        }
    }
}