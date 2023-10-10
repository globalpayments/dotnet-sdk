using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class UserAccount
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public UserAccount(string id, string name = null) {
            Id = id;
            Name = name;
        }
    }
}
