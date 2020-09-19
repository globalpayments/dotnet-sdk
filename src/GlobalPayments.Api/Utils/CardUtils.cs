using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GlobalPayments.Api.Utils {
    public class CardUtils {
        private static readonly Regex AmexRegex = new Regex(@"^3[47]", RegexOptions.None);
        private static readonly Regex MasterCardRegex = new Regex(@"^(?:5[1-5]|222[1-9]|22[3-9][0-9]|2[3-6][0-9]{2}|27[01][0-9]|2720)", RegexOptions.None);
        private static readonly Regex VisaRegex = new Regex(@"^4", RegexOptions.None);
        private static readonly Regex DinersClubRegex = new Regex(@"^3(?:0[0-5]|[68][0-9])", RegexOptions.None);
        private static readonly Regex RouteClubRegex = new Regex(@"^(2014|2149)", RegexOptions.None);
        private static readonly Regex DiscoverRegex = new Regex(@"^6(?:011|5[0-9]{2})", RegexOptions.None);
        private static readonly Regex JcbRegex = new Regex(@"^(?:2131|1800|35\d{3})", RegexOptions.None);
        private static readonly Regex VoyagerRegex = new Regex(@"^70888[5-9]", RegexOptions.None);
        private static readonly Regex WexRegex = new Regex(@"^(?:690046|707138)", RegexOptions.None);
        private static readonly Regex StoredValueRegex = new Regex(@"^(?:600649|603261|603571|627600|639470)", RegexOptions.None);
        private static readonly Regex ValueLinkRegex = new Regex(@"^(?:601056|603225)", RegexOptions.None);
        private static readonly Regex HeartlandGiftRegex = new Regex(@"^(?:502244|627720|708355)", RegexOptions.None);
        private static readonly Regex WorldFuelRegex = new Regex(@"^7000009[5-8]", RegexOptions.None);
        private static readonly Regex FleetCorFleetwideRegex = new Regex(@"^70768598", RegexOptions.None);
        private static readonly Regex FleetCorFuelmanPlusRegex = new Regex(@"^707649", RegexOptions.None);
        private static readonly Regex FleetOneRegex = new Regex(@"^501486", RegexOptions.None);

        private static readonly Regex TrackOnePattern = new Regex(@"%?[B0]?([\d]+)\^[^\^]+\^([\d]{4})([^?]+)?", RegexOptions.None);
        private static readonly Regex TrackTwoPattern = new Regex(@";?([\d]+)[=|w](\d{4})([^?]+)?", RegexOptions.None);

        private static Dictionary<string, Regex> _regexMap;
        private static Dictionary<string, Dictionary<string, string>> _fleetBinMap;
        private static Dictionary<string, Dictionary<string, string>> _purchaseBinMap;

        static CardUtils() {
            _regexMap = new Dictionary<string, Regex> { { "Amex", AmexRegex }, { "MC", MasterCardRegex }, { "Visa", VisaRegex }, { "DinersClub", DinersClubRegex }, { "EnRoute", RouteClubRegex }, { "Discover", DiscoverRegex }, { "Jcb", JcbRegex }, { "Voyager", VoyagerRegex }, { "Wex", WexRegex }, { "StoredValue", StoredValueRegex }, { "ValueLink", ValueLinkRegex }, { "HeartlandGift", HeartlandGiftRegex }, { "WorldFuels", WorldFuelRegex }, { "FleetCorFleetwide",  FleetCorFleetwideRegex }, { "FleetCorFuelmanPlus",  FleetCorFuelmanPlusRegex }, { "FleetOne",  FleetOneRegex }
            };

            // fleet bin ranges
            _fleetBinMap = new Dictionary<string, Dictionary<string, string>>();            

            // visa fleet mappings
            var visaFleetMap = new Dictionary<string, string> { { "448460", "448460" }, { "448462", "448468" }, { "448470", "448516" }, { "448518", "448589" }, { "448591", "448591" }, { "448593", "448611" }, { "448613", "448615" }, { "448617", "448674" }, { "448676", "448686" }, { "448688", "448699" }, { "461400", "461421" }, { "480701", "480899" }
            };
            _fleetBinMap.Add("Visa", visaFleetMap);

            // mastercard fleet mappings
            var mcFleetMap = new Dictionary<string, string> { { "553231", "553380" }, { "556083", "556099" }, { "556100", "556599" }, { "556700", "556999" }
            };
            _fleetBinMap.Add("MC", mcFleetMap);

            // wright express fleet mappings
            var wexFleetMap = new Dictionary<string, string> { { "690046", "690046" }, { "707138", "707138" }
            };
            _fleetBinMap.Add("Wex", wexFleetMap);

            // voyager fleet
            var voyagerFleetMap = new Dictionary<string, string> { { "708885", "708889" }
            };
            _fleetBinMap.Add("Voyager", voyagerFleetMap);

            //WorldFuel fleet
            var worldFuelFleetMap = new Dictionary<string, string> { { "70000095", "70000095" }, { "70000096", "70000096" }, { "70000097", "70000097" }, { "70000098", "70000098" }
            };
            _fleetBinMap.Add("WorldFuel", worldFuelFleetMap);

            //FleetCorFleetwide mappings
            var fleetCorFleetwideMap = new Dictionary<string, string> { { "70768598", "70768598" }
            };
            _fleetBinMap.Add("FleetCorFleetwide", fleetCorFleetwideMap);

            //FleetCorFuelmanPlus mappings
            var fleetCorFuelmanPlusMap = new Dictionary<string, string> { { "707649", "707649" }
            };
            _fleetBinMap.Add("FleetCorFuelmanPlus", fleetCorFuelmanPlusMap);

            //FleetOne mappings
            var fleetOneMap = new Dictionary<string, string> { { "501486", "501486" }
            };
            _fleetBinMap.Add("FleetOne", fleetOneMap);

            // PurchaseBin ranges
            _purchaseBinMap = new Dictionary<string, Dictionary<string, string>>();

            var visaPurchasingMap = new Dictionary<string, string> { { "405607","405607" }, { "415928","415928" }, { "418308","418308" }, { "424604","424604" }, { "427533","427533" }, { "430736","430736" }, { "433085","433085" }, { "443085","443085" }, { "434868","434873" }, { "448410","448410" }, { "448419","448419" }, { "448421","448421" }, { "448452","448452" }, { "448461","448462" }, { "448469","448469" }, { "448485","448486" }, { "448483","448483" }, { "448489","448489" }, { "448491","448491" }, { "448515","448515" }, { "448517","448517" }, { "448524","448524" }, { "448535","448535" }, { "448546","448546" }, { "448548","448548" }, { "448557","448557" }, { "448562","448562" }, { "448569","448569" }, { "448598","448598" }, { "448609","448609" }, { "448626","448626" }, { "452072","452072" }, { "461422","461422" }, { "461431","461431" }, { "461437","461437" }, { "461481","461481" }, { "461490","461490" }, { "471500","471500" }, { "471503","471503" }, { "471508","471508" }, { "471511","471511" }, { "471522","471522" }, { "471529","471529" }, { "471539","471539" }, { "471556","471556" }, { "471569","471569" }, { "471573","471573" }, { "471575","471575" }, { "471578","471578" }, { "471586","471586" }, { "471592","471592" }, { "471596","471596" }, { "471630","471630" }, { "471640","471640" }, { "480439","480439" }, { "480452","480452" }, { "480455","480455" }, { "480458","480458" }, { "480470","480470" }, { "480725","480725" }, { "480824","480824" }, { "485901","485901" }, { "485910","485910" }, { "485915","485915" }, { "485948","485948" }, { "485983","485983" }, { "485986","485986" }, { "485997","485997" }, { "485999","485999" }, { "486509","486509" }, { "486535","486535" }, { "486560","486560" }, { "486576","486576" }, { "486580","486580" }, { "486583","486583" }, { "486640","486640" }, { "486670","486670" }, { "486690","486690" }, { "489629","489629" }, { "448499","448501" }, { "448506","448507" }, { "448559","448560" }, { "448564","448565" }, { "448600","448604" }, { "448606","448607" }, { "448620","448622" }, { "448672","448673" }, { "448676","448679" }, { "448692","448693" }, { "461426","461429" }, { "461470","461471" },    { "461487","461488" }, { "471515","471516" }, { "471524","471526" }, { "471545","471546" }, { "471552","471553" }, { "471563","471564" }, { "480406","480408" }, { "480411","480413" }, { "480420","480421" }, { "480423","480424" }, { "480722","480723" }, { "486511","486512" }, { "486516","486517" }, { "486521","486523" }, { "486524","486528" }, { "486550","486552" }, { "486554","486555" }, { "486588","486591" }
            };
            _purchaseBinMap.Add("Visa", visaPurchasingMap);

            var MasterCardPurchasingMap = new Dictionary<string, string> { { "513359", "513360" }, { "513721", "513722" }, { "513764", "513764" }, { "530249", "530249" }, { "540500", "540501" }, { "540505", "540511" }, { "540514", "540515" }, { "540517", "540517" }, { "540520", "540524" }, { "540526", "540529" }, { "540531", "540539" }, { "540542", "540543" }, { "540546", "540549" }, { "540554", "540554" }, { "540556", "540556" }, { "540561", "540562" }, { "540568", "540580" }, { "540582", "540583" }, { "540586", "540590" }, { "540592", "540596" }, { "540598", "540598" }, { "549297", "549297" }, { "552039", "552039" }, { "552587", "552587" }, { "552807", "552807" }, { "552902", "552906" }, { "552908", "552910" }, { "552913", "552914" }, { "552916", "552917" }, { "552924", "552927" }, { "552929", "552931" }, { "552934", "552934" }, { "552936", "552937" }, { "552941", "552942" }, { "552950", "552950" }, { "553424", "553424" }, { "553447", "553447" }, { "555000", "555001" }, { "555003", "555003" }, { "555005", "555006" }, { "555008", "555011" }, { "555013", "555014" }, { "555016", "555016" }, { "555018", "555021" }, { "555024", "555025" }, { "555027", "555027" }, { "555029", "555029" }, { "555033", "555033" }, { "555039", "555042" }, { "555078", "555078" }, { "555109", "555109" }, { "555225", "555225" }, { "555951", "555953" }, { "555957", "555958" }, { "555962", "555962" }, { "556024", "556024" }, { "558301", "558301" }
            };
            _purchaseBinMap.Add("MC", MasterCardPurchasingMap);
        }

        public static bool IsFleet(string cardType, string pan) {
            if (!string.IsNullOrEmpty(pan)) {
                int compareValue = int.Parse(pan.Substring(0, 6));
                string baseCardType = cardType.TrimEnd("Fleet");

                if (_fleetBinMap.ContainsKey(baseCardType)) {
                    var binRanges = _fleetBinMap[baseCardType];
                    foreach (string key in binRanges.Keys) {
                        int lowerRange = int.Parse(key);
                        int upperRange = int.Parse(binRanges[key]);

                        if (compareValue >= lowerRange && compareValue <= upperRange) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static string MapCardType(string pan) {
            string rvalue = "Unknown";
            if (!string.IsNullOrEmpty(pan)) {
                pan = pan.Replace(" ", "").Replace("-", "");
                foreach (string cardType in _regexMap.Keys) {
                    if (_regexMap[cardType].IsMatch(pan)) {
                        rvalue = cardType;
                        break;
                    }
                }

                // we have a card type, check if it's a fleet card
                if (!rvalue.Equals("Unknown")) {
                    if (IsFleet(rvalue, pan)) {
                        rvalue += "Fleet";
                    }
                    if (IsPurchase(rvalue, pan)) {
                        rvalue += "Purchasing";
                    }
                }
            }
            return rvalue;
        }

        public static bool IsPurchase(string cardType, string pan) {
            if (!string.IsNullOrEmpty(pan)) {
                int compareValue = int.Parse(pan.Substring(0, 6));
                string baseCardType = cardType.TrimEnd("Purchasing");

                if (_purchaseBinMap.ContainsKey(baseCardType)) {
                    var binRanges = _purchaseBinMap[baseCardType];
                    foreach (string key in binRanges.Keys) {
                        int lowerRange = int.Parse(key);
                        int upperRange = int.Parse(binRanges[key]);

                        if (compareValue >= lowerRange && compareValue <= upperRange) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static T ParseTrackData<T>(T paymentMethod) where T : ITrackData {
            string trackData = Regex.Replace(paymentMethod.Value, @"(?<=\d)\p{Zs}(?=\d)", "");
            //string trackData = paymentMethod.Value;
            Match matcher = TrackTwoPattern.Match(trackData);
            if (matcher.Success) {
                string pan = matcher.Groups[1].Value;
                string expiry = matcher.Groups[2].Value;
                string discretionary = matcher.Groups[3].Value;

                if (!string.IsNullOrEmpty(discretionary)) {
                    if (string.Concat(pan, expiry, discretionary).Length == 37 && discretionary.ToLower().EndsWith("f")) {
                        discretionary = discretionary.Substring(0, discretionary.Length - 1);
                    }
                }

                paymentMethod.TrackNumber = TrackNumber.TrackTwo;
                paymentMethod.Pan = pan;
                paymentMethod.Expiry = expiry;
                paymentMethod.DiscretionaryData = discretionary;
                paymentMethod.TrackData = "{0}={1}{2}".FormatWith(pan, expiry, discretionary);
            }
            else {
                matcher = TrackOnePattern.Match(trackData);
                if (matcher.Success) {
                    paymentMethod.TrackNumber = TrackNumber.TrackOne;
                    paymentMethod.Pan = matcher.Groups[1].Value;
                    paymentMethod.Expiry = matcher.Groups[2].Value;
                    paymentMethod.DiscretionaryData = matcher.Groups[3].Value;
                    paymentMethod.TrackData = matcher.Value.TrimStart('%');
                }
            }

            return paymentMethod;
        }
        public static GiftCard ParseTrackData(GiftCard paymentMethod) {
            string trackData = paymentMethod.Value;
            Match matcher = TrackTwoPattern.Match(trackData);
            if (matcher.Success) {
                paymentMethod.TrackNumber = TrackNumber.TrackTwo;
                paymentMethod.Pan = matcher.Groups[1].Value;
                paymentMethod.Expiry = matcher.Groups[2].Value; 
                paymentMethod.TrackData = matcher.Value.TrimStart(';');
            }
            else {
                matcher = TrackOnePattern.Match(trackData);
                if (matcher.Success) {
                    paymentMethod.TrackNumber = TrackNumber.TrackOne;
                    paymentMethod.Pan = matcher.Groups[1].Value;
                    paymentMethod.Expiry = matcher.Groups[2].Value;
                    paymentMethod.TrackData = matcher.Value.TrimStart('%');
                }
            }

            return paymentMethod;
        }
    }
}
