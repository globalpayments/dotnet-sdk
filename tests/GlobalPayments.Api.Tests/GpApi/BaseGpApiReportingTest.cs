using GlobalPayments.Api.Utils;
using System;
using System.Threading;

namespace GlobalPayments.Api.Tests.GpApi
{
    public abstract class BaseGpApiReportingTest : BaseGpApiTests
    {
        protected DateTime REPORTING_START_DATE = DateTime.UtcNow.AddYears(-1);
        protected DateTime REPORTING_END_DATE = DateTime.UtcNow;
        protected DateTime REPORTING_LAST_MONTH_DATE = DateTime.UtcNow.AddMonths(-1);

        protected int FIRST_PAGE = 1;
        protected int PAGE_SIZE = 10;
    }
}