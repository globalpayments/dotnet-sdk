using System;

namespace GlobalPayments.Api.Tests.GpApi
{
    public abstract class BaseGpApiReportingTest : BaseGpApiTests
    {
        protected DateTime ReportingStartDate = DateTime.UtcNow.AddYears(-1);
        protected DateTime ReportingEndDate = DateTime.UtcNow;
        protected DateTime ReportingLastMonthDate = DateTime.UtcNow.AddMonths(-1);

        protected const int FirstPage = 1;
        protected const int PageSize = 10;
    }
}