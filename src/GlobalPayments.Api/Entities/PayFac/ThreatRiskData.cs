using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.PayFac
{
    public class ThreatRiskData {
        /// <summary>
        /// SourceIp of Merchant, see ProPay Fraud Detection Solutions Manual
        /// </summary>
        public string MerchantSourceIP { get; set; }
        /// <summary>
        /// Threat Metrix Policy, see ProPay Fraud Detection Solutions Manual
        /// </summary>
        public string ThreatMetrixPolicy { get; set; }
        /// <summary>
        /// SessionId for Threat Metrix, see ProPay Fraud Detection Solutions Manual
        /// </summary>
        public string ThreatMetrixSessionID { get; set; }
    }
}
