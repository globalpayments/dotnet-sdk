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
            return new CreditTrackData {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                EntryMethod = entryMethod
            };
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
            return new CreditTrackData {
                Value = "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?",
                EntryMethod = entryMethod
            };
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
            return new CreditTrackData {
                Value = "%B6011000990156527^DIS TEST CARD^25121011000062111401?;6011000990156527=25121011000062111401?",
                EntryMethod = entryMethod
            };
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
            return new CreditTrackData {
                Value = "%B3727 006992 51018^AMEX TEST CARD^2512990502700?;372700699251018=2512990502700?",
                EntryMethod = entryMethod
            };
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
    }
}
