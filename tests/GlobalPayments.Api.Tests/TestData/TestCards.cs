using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Tests.TestData {
    public static class TestCards {
        public static DebitTrackData AsDebit(this CreditTrackData card, string pinBlock) {
            return new DebitTrackData {
                Value = card.Value,
                EncryptionData = card.EncryptionData,
                PinBlock = pinBlock
            };
        }

        public static EBTTrackData AsEBT(this CreditTrackData card, string pinBlock) {
            return new EBTTrackData {
                Value = card.Value,
                EntryMethod = card.EntryMethod,
                EncryptionData = card.EncryptionData,
                PinBlock = pinBlock
            };
        }

        public static EBTCardData AsEBT(this CreditCardData card, string pinBlock) {
            return new EBTCardData {
                Number = card.Number,
                ExpMonth = card.ExpMonth,
                ExpYear = card.ExpYear,
                PinBlock = pinBlock,
                ReaderPresent = card.ReaderPresent,
                CardPresent = card.CardPresent
            };
        }

        public static CreditCardData VisaManual(bool cardPresent = false, bool readerPresent = false) {
            return new CreditCardData {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123",
                CardPresent = cardPresent,
                ReaderPresent = readerPresent
            };
        }

        public static CreditTrackData VisaSwipe(EntryMethod entryMethod = EntryMethod.Swipe) {
            CreditTrackData rvalue = new CreditTrackData {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                EntryMethod = entryMethod
            };
            return rvalue;
            //return new CreditTrackData {
            //    Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
            //    EntryMethod = entryMethod
            //};
        }

        public static CreditTrackData VisaFallbackSwipe(EntryMethod entryMethod = EntryMethod.Swipe) {
            return new CreditTrackData {
                Value = "%B4012002000060016^VI TEST CREDIT^251220118039000000000396?;4012002000060016=25122011803939600000?",
                EntryMethod = entryMethod
            };
        }

        public static CreditTrackData VisaSwipeEncrypted(EntryMethod entryMethod = EntryMethod.Swipe) {
            return new CreditTrackData {
                Value = "<E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|>;",
                EntryMethod = entryMethod,
                EncryptionData = new EncryptionData {
                    Version = "01"
                }
            };
        }

        public static CreditCardData MasterCardManual(bool cardPresent = false, bool readerPresent = false) {
            return new CreditCardData {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123",
                CardPresent = cardPresent,
                ReaderPresent = readerPresent
            };
        }

        public static CreditCardData MasterCardSeries2Manual(bool cardPresent = false, bool readerPresent = false) {
            return new CreditCardData {
                Number = "2223000010005798",
                ExpMonth = 12,
                ExpYear = 2019,
                Cvn = "988",
                CardPresent = cardPresent,
                ReaderPresent = readerPresent
            };
        }

        public static CreditTrackData MasterCardSwipe(EntryMethod entryMethod = EntryMethod.Swipe) {
            CreditTrackData rvalue = new CreditTrackData {
                Value = "%B5506740000004316^MC TEST CARD^251210199998888777766665555444433332?;5506740000004316=25121019999888877776?",
                EntryMethod = entryMethod
            };
            return rvalue;
            //return new CreditTrackData {
            //    Value = "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?",
            //    EntryMethod = entryMethod
            //};
        }

        public static CreditTrackData MasterCard24Swipe(EntryMethod entryMethod = EntryMethod.Swipe) {
            return new CreditTrackData {
                Value = "%B2223000010005780^TEST CARD/EMV BIN-2^19121010000000009210?;2223000010005780=19121010000000009210?",
                EntryMethod = entryMethod
            };
        }

        public static CreditTrackData MasterCard25Swipe(EntryMethod entryMethod = EntryMethod.Swipe) {
            return new CreditTrackData {
                Value = "%B2223000010005798^TEST CARD/EMV BIN-2^19121010000000003840?;2223000010005798=19121010000000003840?",
                EntryMethod = entryMethod
            };
        }

        public static CreditTrackData MasterCardSwipeEncrypted(EntryMethod entryMethod = EntryMethod.Swipe) {
            return new CreditTrackData {
                Value = "&lt;E1052711%B5473501000000014^MC TEST CARD^251200000000000000000000000000000000?|GVEY/MKaKXuqqjKRRueIdCHPPoj1gMccgNOtHC41ymz7bIvyJJVdD3LW8BbwvwoenI+|+++++++C4cI2zjMp|11;5473501000000014=25120000000000000000?|8XqYkQGMdGeiIsgM0pzdCbEGUDP|+++++++C4cI2zjMp|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                EntryMethod = entryMethod,
                EncryptionData = new EncryptionData {
                    Version = "01"
                }
            };
        }

        public static CreditCardData DiscoverManual(bool cardPresent = false, bool readerPresent = false) {
            return new CreditCardData {
                Number = "6011000990156527",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123",
                CardPresent = cardPresent,
                ReaderPresent = readerPresent
            };
        }

        public static CreditTrackData DiscoverSwipe(EntryMethod entryMethod = EntryMethod.Swipe) {
            CreditTrackData creditTrackData = new CreditTrackData {
                Value = "%B6011000990156527^DIS TEST CARD^25121011000062111401?;6011000990156527=25121011000062111401?",
                EntryMethod = entryMethod
            };
            return creditTrackData;
            //return new CreditTrackData {
            //    Value = "%B6011000990156527^DIS TEST CARD^25121011000062111401?;6011000990156527=25121011000062111401?",
            //    EntryMethod = entryMethod
            //};
        }
        public static CreditTrackData DiscoverSwipeEncrypted(EntryMethod entryMethod = EntryMethod.Swipe) {
            return new CreditTrackData {
                Value = "&lt;E1049711%B6011000000006527^DIS TEST CARD^25120000000000000000?|nqtDvLuS4VHJd1FymxBxihO5g/ZDqlHyTf8fQpjBwkk95cc6PG9V|+++++++C+LdWXLpP|11;6011000000006527=25120000000000000000?|8VfZvczP6iBqRis2XFypmktaipa|+++++++C+LdWXLpP|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                EntryMethod = entryMethod,
                EncryptionData = new EncryptionData {
                    Version = "01"
                }
            };
        }

        public static CreditCardData AmexManual(bool cardPresent = false, bool readerPresent = false) {
            return new CreditCardData {
                Number = "372700699251018",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "1234",
                CardPresent = cardPresent,
                ReaderPresent = readerPresent
            };
        }

        public static CreditTrackData AmexSwipe(EntryMethod entryMethod = EntryMethod.Swipe) {
            CreditTrackData rvalue = new CreditTrackData {
                Value = "%B3739 531923 51004^STANDARD ANSI             ^2008100812345?",
                EntryMethod = entryMethod
            };
            return rvalue;
            //return new CreditTrackData
            //{
            //    Value = "%B3727 006992 51018^AMEX TEST CARD^2512990502700?;372700699251018=2512990502700?",
            //    EntryMethod = entryMethod
            //};
        }

        public static CreditCardData JcbManual(bool cardPresent = false, bool readerPresent = false) {
            return new CreditCardData {
                Number = "3566007770007321",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123",
                CardPresent = cardPresent,
                ReaderPresent = readerPresent
            };
        }

        public static CreditTrackData JcbSwipe(EntryMethod entryMethod = EntryMethod.Swipe) {
            return new CreditTrackData {
                Value = "%B3566007770007321^JCB TEST CARD^2512101100000000000000000064300000?;3566007770007321=25121011000000076435?",
                EntryMethod = entryMethod
            };
        }

        public static GiftCard GiftCard1Swipe() {
            return new GiftCard {
                TrackData = "%B5022440000000000098^^391200081613?;5022440000000000098=391200081613?"
            };
        }

        public static GiftCard GiftCard2Manual() {
            return new GiftCard {
                Number = "5022440000000000007"
            };
        }        
        public static CreditCardData VisaFleetManual(bool cardPresent = false, bool readerPresent = false) {
            CreditCardData rvalue = new CreditCardData {
                Number = "4761360000000249",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123",
                CardPresent = cardPresent,
                ReaderPresent = readerPresent
            };
            return rvalue;
        }        
        public static CreditTrackData VisaFleetSwipe(EntryMethod entryMethod = EntryMethod.Swipe) {
            CreditTrackData rvalue = new CreditTrackData {
                Value = "%B4485536666666663^VISA TEST CARD/GOOD^25121019206100000001?;4485536666666663=16111019206100000001?",
                EntryMethod = entryMethod
            };
            return rvalue;
            //CreditTrackData rvalue = new CreditTrackData();
            //rvalue.Value = "%B4484630000000126^VISA TEST CARD/GOOD^25121019206100000001?;4484630000000126=16111019206100000001?";
            //rvalue.EntryMethod = entryMethod;
            //return rvalue;
        }        
        public static CreditCardData MasterCardFleetManual(bool cardPresent = false, bool readerPresent = false) {
            CreditCardData card = new CreditCardData {
                Number = "5567300000000016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123",
                CardPresent = cardPresent,
                ReaderPresent = readerPresent
            };

            return card;
        }        
        public static CreditTrackData MasterCardFleetSwipe(EntryMethod entryMethod = EntryMethod.Swipe) {
            CreditTrackData rvalue = new CreditTrackData {
                Value = "%B5567300000000016^MASTERCARD FLEET          ^2512101777766665555444433332111?;5567300000000016=25121019999888877724?",
                EntryMethod = entryMethod
            };
            return rvalue;
            //CreditTrackData track = new CreditTrackData();
            //track.Value = "%B5567300000000016^MASTERCARD FLEET          ^2512101777766665555444433332111?;5567300000000016=25121019999888877711?";
            //track.EntryMethod = entryMethod;

            //return track;
        }
        public static CreditCardData FleetOneManual(bool cardPresent = false, bool readerPresent = false) {
            CreditCardData rvalue = new CreditCardData {
                Number = "5014860000000000126",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123",
                CardPresent = cardPresent,
                ReaderPresent = readerPresent
            };
            return rvalue;
        }
        public static CreditTrackData FleetOneSwipe(EntryMethod entryMethod = EntryMethod.Swipe) {
            CreditTrackData rvalue = new CreditTrackData {
                Value = "%B5014860000000126^FLEETONE TEST CARD/GOOD^25121019206100000001?;5014860000000126=16111019206100000001?",
                EntryMethod = entryMethod
            };
            return rvalue;            
        }
        public static GiftCard ValueLinkManual() {
            GiftCard rvalue = new GiftCard();
            rvalue.SetValue("6010561234567890123");
            return rvalue;
        }
        public static GiftCard ValueLinkSwipe() {
            GiftCard rvalue = new GiftCard();
            rvalue.SetValue("7083559999009209310=999900100000000");
            return rvalue;
        }
        public static CreditCardData AmexManualEncrypted(bool cardPresent=true) {
            CreditCardData rvalue = new CreditCardData();
            rvalue.Number = "372700790311018";
            rvalue.ExpMonth = 12;
            rvalue.ExpYear = 2020;
            rvalue.CardPresent = cardPresent;
            rvalue.ReaderPresent = true;
            rvalue.EncryptionData = EncryptionData.Version2("/wECAQEEAoFGAgEH4gwTTDT6jRZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0yp142cX/wGCVF/gVBOFEiFbZxWq0ZQeADdyMNKbOOzxu2MsHhZ+MkDQrz1KJKJVOHQyV3/mnHBWsQPdlGpVkxK0GxFrxbtIxOwViiBZb2ySajpUat6o+MunOrz7ZsYeurOJHtrpYrLEmPgVwxL3dn3Br+XS5sF2pqtG4lq5MsmgAzzKH9/llZ+FDb1e0NJX/8Nso784bBAr3dmUqagCaWSVb4fcg", "1");
            return rvalue;
        }
        public static CreditTrackData AmexSwipeEncrypted(EntryMethod entryMethod=EntryMethod.Swipe) {
            CreditTrackData rvalue = new CreditTrackData();
            rvalue.Value = "B372700791018^AMEX TEST CARD^2512990ocSvC1w2YgC";
            rvalue.EntryMethod = entryMethod;
            rvalue.EncryptionData = EncryptionData.Version2("/wECAQEEAoFGAgEH4gwTTDT6jRZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0yp142cX/wGCVF/gVBOFEiFbZxWq0ZQeADdyMNKbOOzxu2MsHhZ+MkDQrz1KJKJVOHQyV3/mnHBWsQPdlGpVkxK0GxFrxbtIxOwViiBZb2ySajpUat6o+MunOrz7ZsYeurOJHtrpYrLEmPgVwxL3dn3Br+XS5sF2pqtG4lq5MsmgAzzKH9/llZ+FDb1e0NJX/8Nso784bBAr3dmUqagCaWSVb4fcg", "1");
            return rvalue;
        }
        /*
        SVS
        ;7083559900007000792=99990018010300000?
        ;7083559900007000776=99990013849500000?
        ;7083559900007000818=99990012504400000?
        */
        public static GiftCard SvsManual() {
            GiftCard rvalue = new GiftCard();
            rvalue.SetValue("6394700000001113");
            rvalue.Pin = "1234";
            return rvalue;
        }
        public static GiftCard SvsSwipe() {
            GiftCard rvalue = new GiftCard();
            rvalue.SetValue(";7083559900007000818=99990012504400000?");
            return rvalue;
        }

        public static CreditCardData VisaCorporateManual(bool cardPresent = false, bool readerPresent = false) {
            CreditCardData rvalue = new CreditCardData();
            rvalue.Number = "4013872718148777";
            rvalue.ExpMonth = 12;
            rvalue.ExpYear = 2025;
            rvalue.Cvn = "123";
            rvalue.CardPresent = cardPresent;
            rvalue.ReaderPresent = readerPresent;
            return rvalue;
        }

        public static CreditTrackData VisaCorporateSwipe(EntryMethod entryMethod = EntryMethod.Swipe) {
            CreditTrackData rvalue = new CreditTrackData();
            rvalue.Value = "%B4273594425847534^VISA TEST CARD/GOOD^2512101?;4273594425847534=1712101?";
            rvalue.EntryMethod = entryMethod;
            return rvalue;
        }

        public static CreditCardData VisaPurchasingManual(bool cardPresent = false, bool readerPresent = false) {
            CreditCardData rvalue = new CreditCardData();
            rvalue.Number = "4484104292153662";
            rvalue.ExpMonth = 12;
            rvalue.ExpYear = 2025;
            rvalue.Cvn = "123";
            rvalue.CardPresent = cardPresent;
            rvalue.ReaderPresent = readerPresent;
            return rvalue;
        }

        public static CreditTrackData VisaPurchasingSwipe(EntryMethod entryMethod = EntryMethod.Swipe) {
            CreditTrackData rvalue = new CreditTrackData();
            rvalue.Value = "%B4484104292153662^POSINT TEST VISA P CARD^2512501032100321001000?;4484104292153662=18035010321?";
            rvalue.EntryMethod = entryMethod;
            return rvalue;
        }

        public static CreditCardData MasterCardPurchasingManual(bool cardPresent = false, bool readerPresent = false) {
            CreditCardData rvalue = new CreditCardData();
            rvalue.Number = "5302490000004066";
            rvalue.ExpMonth = 12;
            rvalue.ExpYear = 2025;
            rvalue.Cvn = "123";
            rvalue.CardPresent = cardPresent;
            rvalue.ReaderPresent = readerPresent;
            return rvalue;
        }

        public static CreditTrackData MasterCardPurchasingSwipe(EntryMethod entryMethod = EntryMethod.Swipe) {
            CreditTrackData rvalue = new CreditTrackData();
            rvalue.Value = "%B5405059925478964^MASTERCARD TEST^25121011234567890123?;5405059925478964=18121011234567890123?";
            rvalue.EntryMethod = entryMethod;
            return rvalue;
        }

        public static CreditTrackData MasterCard2Swipe(EntryMethod entryMethod = EntryMethod.Swipe) {
            CreditTrackData rvalue = new CreditTrackData();
            rvalue.Value = "%B2223000010005780^TEST CARD/EMV BIN-2^25121010000000009210?;2223000010005780=25121010000000009210?";
            rvalue.EntryMethod = entryMethod;
            return rvalue;
        }
    }
}
