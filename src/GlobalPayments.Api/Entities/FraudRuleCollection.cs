using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class FraudRuleCollection
    {
        public List<FraudRule> Rules { get; set; }

        public FraudRuleCollection()
        {
            Rules = new List<FraudRule>();
        }

        public void AddRule(string key, FraudFilterMode mode)
        {
            if (hasRule(key))
                return;

            Rules.Add(new FraudRule
            {
                Key = key,
                Mode = mode
            });
        }

        private bool hasRule(string key)
        {
            return Rules.Exists(x => x.Key == key);
        }
    }
}
