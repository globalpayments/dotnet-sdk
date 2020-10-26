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

        private static readonly Regex TrackOnePattern = new Regex(@"%?[B0]?([\d]+)\^[^\^]+\^([\d]{4})([^?]+)\??", RegexOptions.None);
        private static readonly Regex TrackTwoPattern = new Regex(@";?([\d]+)[=|w](\d{4})([^?]+)\??", RegexOptions.None);

        private static Dictionary<string, Regex> _regexMap;
        private static Dictionary<string, Dictionary<string, string>> _fleetBinMap;

        static CardUtils() {
            _regexMap = new Dictionary<string, Regex> {
                { "Amex", AmexRegex },
                { "MC", MasterCardRegex },
                { "Visa", VisaRegex },
                { "DinersClub", DinersClubRegex },
                { "EnRoute", RouteClubRegex },
                { "Discover", DiscoverRegex },
                { "Jcb", JcbRegex },
                { "Voyager", VoyagerRegex },
                { "Wex", WexRegex },
                { "StoredValue", StoredValueRegex },
                { "ValueLink", ValueLinkRegex },
                { "HeartlandGift", HeartlandGiftRegex }
            };

            // fleet bin ranges
            _fleetBinMap = new Dictionary<string, Dictionary<string, string>>();

            // visa fleet mappings
            var visaFleetMap = new Dictionary<string, string>();
            visaFleetMap.Add("448460", "448611");
            visaFleetMap.Add("448613", "448615");
            visaFleetMap.Add("448617", "448674");
            visaFleetMap.Add("448676", "448686");
            visaFleetMap.Add("448688", "448699");
            visaFleetMap.Add("461400", "461421");
            visaFleetMap.Add("461423", "461499");
            visaFleetMap.Add("480700", "480899");
            _fleetBinMap.Add("Visa", visaFleetMap);

            // mastercard fleet mappings
            var mcFleetMap = new Dictionary<string, string>();
            mcFleetMap.Add("553231", "553380");
            mcFleetMap.Add("556083", "556099");
            mcFleetMap.Add("556100", "556599");
            mcFleetMap.Add("556700", "556999");
            _fleetBinMap.Add("MC", mcFleetMap);

            // wright express fleet mappings
            var wexFleetMap = new Dictionary<string, string>();
            wexFleetMap.Add("690046", "690046");
            wexFleetMap.Add("707138", "707138");
            _fleetBinMap.Add("Wex", wexFleetMap);

            // voyager fleet
            var voyagerFleetMap = new Dictionary<string, string>();
            voyagerFleetMap.Add("708885", "708889");
            _fleetBinMap.Add("Voyager", voyagerFleetMap);
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
                }
            }
            return rvalue;
        }

        public static T ParseTrackData<T>(T paymentMethod) where T : ITrackData {
            string trackData = paymentMethod.Value;
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
                    paymentMethod.TrackNumber = TrackNumber.TrackTwo;
                    paymentMethod.Pan = matcher.Groups[1].Value;
                    paymentMethod.Expiry = matcher.Groups[2].Value;
                    paymentMethod.DiscretionaryData = matcher.Groups[3].Value;
                    paymentMethod.TrackData = matcher.Value.TrimStart('%');
                }
            }

            return paymentMethod;
        }
    }
}
