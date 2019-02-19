using System;
using System.Collections.Generic;
using System.Text;
using GlobalPayments.Api.Terminals;

namespace GlobalPayments.Api.Tests.Terminals
{
    public class RequestIdProvider : IRequestIdProvider {
        private Random random;

        public RequestIdProvider() { 
            random = new Random(DateTime.Now.Millisecond);
        }

        public int GetRequestId() {
            return new Random().Next(100000, 999999);
        }
    }
}
