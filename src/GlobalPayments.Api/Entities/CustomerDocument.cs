using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class CustomerDocument
    {        
        public string Reference { get; set; }        
        public string Issuer { get; set; }       
        public CustomerDocumentType Type { get; set; }
    }
}
