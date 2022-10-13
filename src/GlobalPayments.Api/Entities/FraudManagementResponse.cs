using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class FraudManagementResponse
    {
        /// <summary>
        /// This element indicates the mode the Fraud Filter executed in
        /// </summary>
        public string FraudResponseMode { get; set; }

        /// <summary>
        /// This field is used to determine what the overall result the Fraud Filter returned
        /// </summary>
        public string FraudResponseResult { get; set; }

        /// <summary>
        /// Filter rules
        /// </summary>
        public List<FraudRule> FraudResponseRules { get; set; }

        public string FraudResponseMessage { get; set; }
    }
}
