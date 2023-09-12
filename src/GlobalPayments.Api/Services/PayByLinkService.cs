using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Reporting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Services
{
    public class PayByLinkService
    {
        public static AuthorizationBuilder Create(PayByLinkData payByLinkData, decimal amount)
        {
            return (new AuthorizationBuilder(TransactionType.Create))
                .WithAmount(amount)
                .WithPayByLinkData(payByLinkData);
        }

        public static ManagementBuilder Edit(string payLinkId)
        {
            return (new ManagementBuilder(TransactionType.PayByLinkUpdate))
                .WithPaymentLinkId(payLinkId);
        }


        public static TransactionReportBuilder<PayByLinkSummary> PayByLinkDetail(string payByLinkId)
        {
            return (new TransactionReportBuilder<PayByLinkSummary>(ReportType.PayByLinkDetail))
                .WithPayByLinkId(payByLinkId);
        }

        public static ReportBuilder<PagedResult<PayByLinkSummary>> FindPayByLink(int page, int pageSize)
        {
            return (new TransactionReportBuilder<PagedResult<PayByLinkSummary>>(ReportType.FindPayByLinkPaged))
                .WithPaging(page, pageSize);
        }
    }
}
