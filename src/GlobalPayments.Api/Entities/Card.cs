using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class Card
    {        
        public string CardHolderName { get; set; }   
        public string CardNumber { get; set; }    
        public string MaskedCardNumber { get; set; }    
        public string CardExpMonth { get; set; }    
        public string CardExpYear { get; set; }    
        public string Token { get; set; }
        /// <summary>
        /// Masked card number with last 4 digits showing.
        /// </summary>
        public string MaskedNumberLast4 { get; set; }

        /// <summary>
        /// Indicates the card brand that issued the card.
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// The unique reference created by the brands/schemes to uniquely identify the transaction.
        /// </summary>
        public string BrandReference { get; set; }

        /// <summary>
        /// Contains the fist 6 digits of the card
        /// </summary>
        public string Bin { get; set; }

        /// <summary>
        /// The issuing country that the bin is associated with.
        /// </summary>
        public string BinCountry { get; set; }

        /// <summary>
        /// The card providers description of their card product.
        /// </summary>
        public string AccountType { get; set; }

        /// <summary>
        /// The label of the issuing bank or financial institution of the bin.
        /// </summary>
        public string Issuer { get; set; }
    }
}
