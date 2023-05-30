using GlobalPayments.Api.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.PaymentMethods
{
    public class EwicCardData : Ewic, ICardData
    {
        public bool CardPresent { get; set; }
        public string CardType { get; set; }
        public string Cvn { get; set; }
        public CvnPresenceIndicator CvnPresenceIndicator { get; set; }
        public string Number { get; set; }
        public int? ExpMonth { get; set ; }
        public int? ExpYear { get; set; }
        public bool ReaderPresent { get; set; }
        public ManualEntryMethod? EntryMethod { get; set; }
        public EntryMethod? OriginalEntryMethod { get; set; }

        public string ShortExpiry => throw new NotImplementedException();

        public string CardHolderName { get; set; }
    }
}
