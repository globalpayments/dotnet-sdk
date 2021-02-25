using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.PayFac
{
    public class BeneficialOwnerData {
        public string OwnersCount { get; set; }
        public List<OwnersData> OwnersList { get; set; }

        public BeneficialOwnerData() {
            OwnersList = new List<OwnersData>();
        }
    }
}
