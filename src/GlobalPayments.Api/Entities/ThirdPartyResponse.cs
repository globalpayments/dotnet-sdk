using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class ThirdPartyResponse
    {
        public string Platform { get; set; }

        /// <summary>
        /// Data json string that represents the raw data received from another platform.
        /// </summary>
        public string Data { get; set; }
    }
}
