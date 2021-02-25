using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.PayFac;
using GlobalPayments.Api.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Tests.ProPay.TestData
{
    public class TestFundsData {
        public static FlashFundsPaymentCardData GetFlashFundsPaymentCardData() {
            var cardData = new FlashFundsPaymentCardData()
            {
                CreditCard = new CreditCardData()
                {
                    Number = "4895142232120006",
                    ExpMonth = 10,
                    ExpYear = 2020,
                    Cvn = "022",
                    CardHolderName = "Clint Eastwood"
                },
                CardholderAddress = new Address()
                {
                    StreetAddress1 = "900 Metro Center Blv",
                    City = "San Fransisco",
                    State = "CA",
                    PostalCode = "94404",
                    Country = "USA"
                }
            };

            return cardData;
        }
    }
}
