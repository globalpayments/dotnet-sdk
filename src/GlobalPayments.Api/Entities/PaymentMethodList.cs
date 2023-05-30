using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class PaymentMethodList
    {
        public PaymentMethodFunction Function { get; set; }        

        public object PaymentMethod {get;set;}
    }
}
