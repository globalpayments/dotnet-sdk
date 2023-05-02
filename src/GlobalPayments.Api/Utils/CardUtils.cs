using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GlobalPayments.Api.Utils {
    public class CardUtils {
        private static readonly Regex AmexRegex = new Regex(@"^3[47]", RegexOptions.None);
        private static readonly Regex MasterCardRegex = new Regex(@"^(?:5[1-8]|222[1-9]|22[3-9][0-9]|2[3-6][0-9]{2}|27[01][0-9]|2720)", RegexOptions.None);
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
        private static readonly Regex FleetCorGasCardRegex = new Regex(@"^707685", RegexOptions.None);
        private static readonly Regex FleetCorFleetwideRegex = new Regex(@"^70768598", RegexOptions.None);
        private static readonly Regex FleetCorFuelmanPlusRegex = new Regex(@"^707649", RegexOptions.None);
        private static readonly Regex FleetOneRegex = new Regex(@"^501486", RegexOptions.None);

        private static readonly Regex TrackOnePattern = new Regex(@"%?[B0]?([\d]+)\^[^\^]+\^([\d]{4})([^?]+)?", RegexOptions.None);
        private static readonly Regex TrackTwoPattern = new Regex(@";?([\d]+)[=|w](\d{4})([^?]+)?", RegexOptions.None);

        private static Dictionary<string, Regex> _regexMap;
        private static Dictionary<string, Dictionary<string, string>> _fleetBinMap;
        private static Dictionary<string, Dictionary<string, string>> _purchaseBinMap;
        private static Dictionary<string, Dictionary<string, string>> _readyLinkBinMap;

        static CardUtils() {
            _regexMap = new Dictionary<string, Regex> { { "Amex", AmexRegex }, { "MC", MasterCardRegex }, { "Visa", VisaRegex }, { "DinersClub", DinersClubRegex }, { "EnRoute", RouteClubRegex }, { "Discover", DiscoverRegex }, { "Jcb", JcbRegex }, { "Voyager", VoyagerRegex }, { "Wex", WexRegex }, { "StoredValue", StoredValueRegex }, { "ValueLink", ValueLinkRegex }, { "HeartlandGift", HeartlandGiftRegex }, { "WorldFuels", WorldFuelRegex }, { "FleetCorGasCard", FleetCorGasCardRegex }, { "FleetCorFleetwide",  FleetCorFleetwideRegex }, { "FleetCorFuelmanPlus",  FleetCorFuelmanPlusRegex }, { "FleetOne",  FleetOneRegex }
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

            //FleetCorGasCard mappings
            var fleetCarGasCardMap = new Dictionary<string, string> { {  "707685", "707685" }
            };
            _fleetBinMap.Add("FleetCorGasCard", fleetCarGasCardMap);

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

            var masterCardPurchasingMap = new Dictionary<string, string> { { "513359", "513360" }, { "513721", "513722" }, { "513764", "513764" }, { "530249", "530249" }, { "540500", "540501" }, { "540505", "540511" }, { "540514", "540515" }, { "540517", "540517" }, { "540520", "540524" }, { "540526", "540529" }, { "540531", "540539" }, { "540542", "540543" }, { "540546", "540549" }, { "540554", "540554" }, { "540556", "540556" }, { "540561", "540562" }, { "540568", "540580" }, { "540582", "540583" }, { "540586", "540590" }, { "540592", "540596" }, { "540598", "540598" }, { "549297", "549297" }, { "552039", "552039" }, { "552587", "552587" }, { "552807", "552807" }, { "552902", "552906" }, { "552908", "552910" }, { "552913", "552914" }, { "552916", "552917" }, { "552924", "552927" }, { "552929", "552931" }, { "552934", "552934" }, { "552936", "552937" }, { "552941", "552942" }, { "552950", "552950" }, { "553424", "553424" }, { "553447", "553447" }, { "555000", "555001" }, { "555003", "555003" }, { "555005", "555006" }, { "555008", "555011" }, { "555013", "555014" }, { "555016", "555016" }, { "555018", "555021" }, { "555024", "555025" }, { "555027", "555027" }, { "555029", "555029" }, { "555033", "555033" }, { "555039", "555042" }, { "555078", "555078" }, { "555109", "555109" }, { "555225", "555225" }, { "555951", "555953" }, { "555957", "555958" }, { "555962", "555962" }, { "556024", "556024" }, { "558301", "558301" }
            };
            _purchaseBinMap.Add("MC", masterCardPurchasingMap);

            //VisaReadyLink ranges

            _readyLinkBinMap = new Dictionary<string, Dictionary<string, string>>();
            var visaReadyLinkMap = new Dictionary<string, string> { { "400029", "400029" }, { "400041", "400041" }, { "400046", "400047" }, { "411064", "411066" }, { "414351", "414352" }, { "417013", "417014" }, { "417947", "417948" }, { "428190", "428191" }, { "430763", "430764" }, { "435196", "435197" }, { "435541", "435542" }, { "435555", "435556" }, { "437817", "437818" }, { "441820", "441821" }, { "455150", "455151" }, { "467378", "467379" }, { "468442", "468443" }, { "470027", "470028" }, { "473909", "473910" }, { "475003", "475005" }, { "476046", "476047" }, { "479269", "479270" }, { "486204", "486205" }, { "489221", "489222" }, { "493146", "493147" }, { "493443", "493444" }, { "400123", "400123" }, { "400341", "400341" }, { "400403", "400403" }, { "400421", "400421" }, { "400528", "400528" }, { "400540", "400540" }, { "400542", "400542" }, { "400544", "400544" }, { "400548", "400548" }, { "400564", "400564" }, { "400908", "400908" }, { "401128", "401128" }, { "401349", "401349" }, { "401658", "401658" }, { "401707", "401707" }, { "401710", "401710" }, { "402055", "402055" }, { "402188", "402188" }, { "402407", "402407" }, { "402644", "402644" }, { "403156", "403156" }, { "403163", "403163" }, { "403169", "403169" }, { "403192", "403192" }, { "403448", "403448" }, { "403544", "403544" }, { "403743", "403743" }, { "403846", "403846" }, { "404051", "404051" }, { "404055", "404055" }, { "404065", "404065" }, { "404070", "404070" }, { "404094", "404094" }, { "404206", "404206" }, { "404283", "404283" }, { "404918", "404918" }, { "405397", "405397" }, { "405505", "405505" }, { "406497", "406497" }, { "406498", "406498" }, { "407197", "407197" }, { "407276", "407276" }, { "407309", "407309" }, { "407535", "407535" }, { "407635", "407635" }, { "407640", "407640" }, { "407894", "407894" }, { "407904", "407904" }, { "407909", "407909" }, { "407911", "407911" }, { "407970", "407970" }, { "407991", "407991" }, { "408535", "408535" }, { "408546", "408546" }, { "408594", "408594" }, { "408598", "408598" }, { "408954", "408954" }, { "409920", "409920" }, { "410146", "410146" }, { "410282", "410282" }, { "410466", "410466" }, { "410484", "410484" }, { "410722", "410722" }, { "410848", "410848" }, { "411133", "411133" }, { "411137", "411137" }, { "411270", "411270" }, { "411449", "411449" }, { "411461", "411461" }, { "412407", "412407" }, { "414002", "414002" }, { "414830", "414830" }, { "415381", "415381" }, { "416420", "416420" }, { "416422", "416422" }, { "416859", "416859" }, { "418654", "418654" }, { "419610", "419610" }, { "420490", "420490" }, { "420592", "420592" }, { "420700", "420700" }, { "420706", "420706" }, { "420728", "420728" }, { "420775", "420775" }, { "420790", "420790" }, { "421830", "421830" }, { "422311", "422311" }, { "422570", "422570" }, { "422572", "422572" }, { "422797", "422797" }, { "422799", "422799" }, { "422803", "422803" }, { "423138", "423138" }, { "423145", "423145" }, { "424099", "424099" }, { "424758", "424758" }, { "424793", "424793" }, { "424804", "424804" }, { "425060", "425060" }, { "425062", "425062" }, { "425064", "425064" }, { "425312", "425312" }, { "425954", "425954" }, { "427090", "427090" }, { "428938", "428938" }, { "429299", "429299" }, { "429375", "429375" }, { "430221", "430221" }, { "430223", "430223" }, { "430236", "430236" }, { "432650", "432650" }, { "433701", "433701" }, { "434219", "434219" }, { "434249", "434249" }, { "434470", "434470" }, { "435568", "435568" }, { "435581", "435581" }, { "435833", "435833" }, { "435877", "435877" }, { "435970", "435970" }, { "436199", "436199" }, { "436733", "436733" }, { "436744", "436744" }, { "436879", "436879" }, { "437317", "437317" }, { "437712", "437712" }, { "439331", "439331" }, { "439334", "439334" }, { "439461", "439461" }, { "441111", "441111" }, { "441617", "441617" }, { "442029", "442029" }, { "442044", "442044" }, { "442059", "442059" }, { "442744", "442744" }, { "442816", "442816" }, { "443021", "443021" }, { "443157", "443157" }, { "444083", "444083" }, { "446053", "446053" }, { "447029", "447029" }, { "447420", "447420" }, { "447470", "447470" }, { "447472", "447472" }, { "448093", "448093" }, { "448095", "448095" }, { "449274", "449274" }, { "449733", "449733" }, { "452263", "452263" }, { "452502", "452502" }, { "452561", "452561" }, { "453031", "453031" }, { "453038", "453038" }, { "453074", "453074" }, { "453911", "453911" }, { "454398", "454398" }, { "454801", "454801" }, { "454806", "454806" }, { "454810", "454810" }, { "454862", "454862" }, { "455918", "455918" }, { "455921", "455921" }, { "456436", "456436" }, { "456527", "456527" }, { "456534", "456534" }, { "456537", "456537" }, { "458424", "458424" }, { "458430", "458430" }, { "458432", "458432" }, { "458436", "458436" }, { "461235", "461235" }, { "461239", "461239" }, { "463497", "463497" }, { "463506", "463506" }, { "465387", "465387" }, { "465391", "465391" }, { "466178", "466178" }, { "467321", "467321" }, { "467339", "467339" }, { "467341", "467341" }, { "468004", "468004" }, { "468292", "468292" }, { "469069", "469069" }, { "469201", "469201" }, { "469208", "469208" }, { "469498", "469498" }, { "470198", "470198" }, { "471807", "471807" }, { "471867", "471867" }, { "471869", "471869" }, { "473618", "473618" }, { "473657", "473657" }, { "473807", "473807" }, { "475015", "475015" }, { "475019", "475019" }, { "475425", "475425" }, { "475429", "475429" }, { "475431", "475431" }, { "475675", "475675" }, { "475693", "475693" }, { "476084", "476084" }, { "476323", "476323" }, { "476334", "476334" }, { "476696", "476696" }, { "477805", "477805" }, { "478058", "478058" }, { "478262", "478262" }, { "478491", "478491" }, { "478499", "478499" }, { "478655", "478655" }, { "479145", "479145" }, { "479349", "479349" }, { "481170", "481170" }, { "481180", "481180" }, { "481237", "481237" }, { "481591", "481591" }, { "481663", "481663" }, { "481687", "481687" }, { "482163", "482163" }, { "483304", "483304" }, { "484056", "484056" }, { "484379", "484379" }, { "485214", "485214" }, { "485221", "485221" }, { "485507", "485507" }, { "485907", "485907" }, { "487093", "487093" }, { "487300", "487300" }, { "487490", "487490" }, { "488885", "488885" }, { "489233", "489233" }, { "489614", "489614" }, { "490718", "490718" }, { "491072", "491072" }, { "491215", "491215" }, { "491244", "491244" }, { "491274", "491274" }, { "491349", "491349" }, { "491377", "491377" }, { "493170", "493170" }, { "493451", "493451" }
            };
            _readyLinkBinMap.Add("Visa", visaReadyLinkMap);
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

        public static bool IsPurchase(string cardType, string pan)
        {
            if (!string.IsNullOrEmpty(pan))
            {
                int compareValue = int.Parse(pan.Substring(0, 6));
                string baseCardType = cardType.TrimEnd("Purchasing");

                if (_purchaseBinMap.ContainsKey(baseCardType))
                {
                    var binRanges = _purchaseBinMap[baseCardType];
                    foreach (string key in binRanges.Keys)
                    {
                        int lowerRange = int.Parse(key);
                        int upperRange = int.Parse(binRanges[key]);

                        if (compareValue >= lowerRange && compareValue <= upperRange)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool IsReadyLink(string cardType, string pan)
        {
            if (!string.IsNullOrEmpty(pan))
            {
                int compareValue = int.Parse(pan.Substring(0, 6));
                string baseCardType = cardType.TrimEnd("ReadyLink");

                if (_readyLinkBinMap.ContainsKey(baseCardType))
                {
                    var binRanges = _readyLinkBinMap[baseCardType];
                    foreach (string key in binRanges.Keys)
                    {
                        int lowerRange = int.Parse(key);
                        int upperRange = int.Parse(binRanges[key]);

                        if (compareValue >= lowerRange && compareValue <= upperRange)
                        {
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

                // we have a card type, check if it's a fleet/purchase/readylink card
                if (!rvalue.Equals("Unknown")) {
                    if (IsFleet(rvalue, pan)) {
                        rvalue += "Fleet";
                    }
                    else if (IsPurchase(rvalue, pan)) {
                        rvalue += "Purchasing";
                    }
                    else if (IsReadyLink(rvalue,pan)) {
                        rvalue += "ReadyLink";
                    }
                }
            }
            return rvalue;
        }

        public static string GetBaseCardType(string cardType)
        {
            var resultCardType = cardType;
            foreach (string cardTypeKey in _regexMap.Keys)
            {
                if (cardType.ToUpper().StartsWith(cardTypeKey.ToUpper()))
                {
                    resultCardType = cardTypeKey;
                    break;
                }
            }
            return resultCardType;
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
