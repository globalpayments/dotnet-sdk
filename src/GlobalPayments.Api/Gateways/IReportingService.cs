using GlobalPayments.Api.Builders;

namespace GlobalPayments.Api.Gateways {
    internal interface IReportingService {
        T ProcessReport<T>(ReportBuilder<T> builder) where T : class;
    }
}
