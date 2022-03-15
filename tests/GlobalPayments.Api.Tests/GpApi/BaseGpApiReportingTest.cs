using System;

namespace GlobalPayments.Api.Tests.GpApi
{
    public abstract class BaseGpApiReportingTest : BaseGpApiTests
    {
        protected DateTime REPORTING_START_DATE = DateTime.UtcNow.AddYears(-1);
        protected DateTime REPORTING_END_DATE = DateTime.UtcNow;
        protected DateTime REPORTING_LAST_MONTH_DATE = DateTime.UtcNow.AddMonths(-1);

        protected readonly int FIRST_PAGE = 1;
        protected readonly int PAGE_SIZE = 10;
    }
}