using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.PayFac
{
    public class UserReference
    {
        public string UserId { get; set; }
        public UserType? UserType { get; set; }
        public UserStatus? UserStatus { get; set; }
    }
}
