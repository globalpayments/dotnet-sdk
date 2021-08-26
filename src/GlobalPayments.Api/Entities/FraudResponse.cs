using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class FraudResponse
    {
        public class Rule
        {
            public String Name { get; set; }
            public String Id { get; set; }
            public String Action { get; set; }
        }

        public FraudFilterMode mode { get; set; }
        public String Result { get; set; }
        public List<Rule> Rules { get; set; }

        public FraudResponse()
        {
            Rules = new List<Rule>();
        }
    }
}
