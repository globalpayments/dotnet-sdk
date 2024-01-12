using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class BlockedCardType
    {
        public bool? Consumerdebit { get; set; }
        public bool? Consumercredit { get; set; }
        public bool? Commercialcredit { get; set; }
        public bool? Commercialdebit { get; set; }
    }
}
