using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPayments.Api.Entities
{
    public class Lodging
    {
        public int? FolioNumber { get; set; }
        public int? StayDuration { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public decimal? DailyRate { get; set; }
        public int? PreferredCustomer { get; set; }
        public List<ExtraChargeType> ExtraChargeTypes { get; set; }
        /// <summary>
        /// Automatically return the sum of ExtraChargeTypes added
        /// </summary>
        public decimal? ExtraChargeTotal { get { return ExtraChargeTypes.Count(); } }
    }
    //public class ExtraChargeTypes
    //{
    //    public bool HasRestaurantCharge { get; set; }
    //    public bool HasGiftShopCharge  { get; set; }
    //    public bool HasMiniBarCharge { get; set; }
    //    public bool HasTelephoneCharge { get; set; }
    //    public bool HasLaundryCharge { get; set; }
    //    public bool HasOtherCharge { get; set; }
    //}
}