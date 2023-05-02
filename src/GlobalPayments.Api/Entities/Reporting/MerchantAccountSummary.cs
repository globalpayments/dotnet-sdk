using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Reporting
{
    public class MerchantAccountSummary
    {
        public string Id { get; set; }

        public MerchantAccountType? Type { get; set; }

        public string Name { get; set; }

        public MerchantAccountStatus? Status { get; set; }

        public List<Channel> Channels { get; set; }

        public List<string> Permissions { get; set; }

        public List<string> Countries { get; set; }

        public List<string> Currencies { get; set; }

        public List<PaymentMethodName> PaymentMethods { get; set; }

        public List<string> Configurations { get; set; }

        public List<Address> Addresses { get; set; }
    }
}
