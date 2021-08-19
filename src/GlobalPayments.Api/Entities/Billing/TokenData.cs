using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.Billing {
    public class TokenData {
        public DateTime LastUsedDateUTC { get; set; }
        public bool IsExpired { get; set; }
        public bool ShareTokenWithGroup { get; set; }
        public List<string> Merchants { get; set; } = null;
    }
}
