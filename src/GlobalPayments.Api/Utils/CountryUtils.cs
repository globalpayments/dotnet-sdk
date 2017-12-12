using GlobalPayments.Api.Entities;
using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Utils {
    public class CountryUtils {
        private static Dictionary<string, string> countryCodeMapByCountry;
        private static Dictionary<string, string> countryMapByCode;
        private const int significantCountryMatch = 6;
        private const int significantCodeMatch = 3;

        static CountryUtils() {
            #region build dictionaries
            countryCodeMapByCountry = new Dictionary<string, string>();
            countryCodeMapByCountry.Add("Afghanistan", "AF");
            countryCodeMapByCountry.Add("Aland Islands", "AX");
            countryCodeMapByCountry.Add("Albania", "AL");
            countryCodeMapByCountry.Add("Algeria", "DZ");
            countryCodeMapByCountry.Add("American Samoa", "AS");
            countryCodeMapByCountry.Add("Andorra", "AD");
            countryCodeMapByCountry.Add("Angola", "AO");
            countryCodeMapByCountry.Add("Anguilla", "AI");
            countryCodeMapByCountry.Add("Antarctica", "AQ");
            countryCodeMapByCountry.Add("Antigua and Barbuda", "AG");
            countryCodeMapByCountry.Add("Argentina", "AR");
            countryCodeMapByCountry.Add("Armenia", "AM");
            countryCodeMapByCountry.Add("Aruba", "AW");
            countryCodeMapByCountry.Add("Australia", "AU");
            countryCodeMapByCountry.Add("Austria", "AT");
            countryCodeMapByCountry.Add("Azerbaijan", "AZ");
            countryCodeMapByCountry.Add("Bahamas", "BS");
            countryCodeMapByCountry.Add("Bahrain", "BH");
            countryCodeMapByCountry.Add("Bangladesh", "BD");
            countryCodeMapByCountry.Add("Barbados", "BB");
            countryCodeMapByCountry.Add("Belarus", "BY");
            countryCodeMapByCountry.Add("Belgium", "BE");
            countryCodeMapByCountry.Add("Belize", "BZ");
            countryCodeMapByCountry.Add("Benin", "BJ");
            countryCodeMapByCountry.Add("Bermuda", "BM");
            countryCodeMapByCountry.Add("Bhutan", "BT");
            countryCodeMapByCountry.Add("Bolivia (Plurinational State of)", "BO");
            countryCodeMapByCountry.Add("Bonaire, Sint Eustatius and Saba", "BQ");
            countryCodeMapByCountry.Add("Bosnia and Herzegovina", "BA");
            countryCodeMapByCountry.Add("Botswana", "BW");
            countryCodeMapByCountry.Add("Bouvet Island", "BV");
            countryCodeMapByCountry.Add("Brazil", "BR");
            countryCodeMapByCountry.Add("British Indian Ocean Territory", "IO");
            countryCodeMapByCountry.Add("Brunei Darussalam", "BN");
            countryCodeMapByCountry.Add("Bulgaria", "BG");
            countryCodeMapByCountry.Add("Burkina Faso", "BF");
            countryCodeMapByCountry.Add("Burundi", "BI");
            countryCodeMapByCountry.Add("Cambodia", "KH");
            countryCodeMapByCountry.Add("Cameroon", "CM");
            countryCodeMapByCountry.Add("Canada", "CA");
            countryCodeMapByCountry.Add("Cabo Verde", "CV");
            countryCodeMapByCountry.Add("Cayman Islands", "KY");
            countryCodeMapByCountry.Add("Central African Republic", "CF");
            countryCodeMapByCountry.Add("Chad", "TD");
            countryCodeMapByCountry.Add("Chile", "CL");
            countryCodeMapByCountry.Add("China", "CN");
            countryCodeMapByCountry.Add("Christmas Island", "CX");
            countryCodeMapByCountry.Add("Cocos (Keeling) Islands", "CC");
            countryCodeMapByCountry.Add("Colombia", "CO");
            countryCodeMapByCountry.Add("Comoros", "KM");
            countryCodeMapByCountry.Add("Congo", "CG");
            countryCodeMapByCountry.Add("Congo (Democratic Republic of the)", "CD");
            countryCodeMapByCountry.Add("Cook Islands", "CK");
            countryCodeMapByCountry.Add("Costa Rica", "CR");
            countryCodeMapByCountry.Add("CÃ´te d'Ivoire", "CI");
            countryCodeMapByCountry.Add("Croatia", "HR");
            countryCodeMapByCountry.Add("Cuba", "CU");
            countryCodeMapByCountry.Add("Curacao", "CW");
            countryCodeMapByCountry.Add("Cyprus", "CY");
            countryCodeMapByCountry.Add("Czech Republic", "CZ");
            countryCodeMapByCountry.Add("Denmark", "DK");
            countryCodeMapByCountry.Add("Djibouti", "DJ");
            countryCodeMapByCountry.Add("Dominica", "DM");
            countryCodeMapByCountry.Add("Dominican Republic", "DO");
            countryCodeMapByCountry.Add("Ecuador", "EC");
            countryCodeMapByCountry.Add("Egypt", "EG");
            countryCodeMapByCountry.Add("El Salvador", "SV");
            countryCodeMapByCountry.Add("Equatorial Guinea", "GQ");
            countryCodeMapByCountry.Add("Eritrea", "ER");
            countryCodeMapByCountry.Add("Estonia", "EE");
            countryCodeMapByCountry.Add("Ethiopia", "ET");
            countryCodeMapByCountry.Add("Falkland Islands (Malvinas)", "FK");
            countryCodeMapByCountry.Add("Faroe Islands", "FO");
            countryCodeMapByCountry.Add("Fiji", "FJ");
            countryCodeMapByCountry.Add("Finland", "FI");
            countryCodeMapByCountry.Add("France", "FR");
            countryCodeMapByCountry.Add("French Guiana", "GF");
            countryCodeMapByCountry.Add("French Polynesia", "PF");
            countryCodeMapByCountry.Add("French Southern Territories", "TF");
            countryCodeMapByCountry.Add("Gabon", "GA");
            countryCodeMapByCountry.Add("Gambia", "GM");
            countryCodeMapByCountry.Add("Georgia", "GE");
            countryCodeMapByCountry.Add("Germany", "DE");
            countryCodeMapByCountry.Add("Ghana", "GH");
            countryCodeMapByCountry.Add("Gibraltar", "GI");
            countryCodeMapByCountry.Add("Greece", "GR");
            countryCodeMapByCountry.Add("Greenland", "GL");
            countryCodeMapByCountry.Add("Grenada", "GD");
            countryCodeMapByCountry.Add("Guadeloupe", "GP");
            countryCodeMapByCountry.Add("Guam", "GU");
            countryCodeMapByCountry.Add("Guatemala", "GT");
            countryCodeMapByCountry.Add("Guernsey", "GG");
            countryCodeMapByCountry.Add("Guinea", "GN");
            countryCodeMapByCountry.Add("Guinea-Bissau", "GW");
            countryCodeMapByCountry.Add("Guyana", "GY");
            countryCodeMapByCountry.Add("Haiti", "HT");
            countryCodeMapByCountry.Add("Heard Island and McDonald Islands", "HM");
            countryCodeMapByCountry.Add("Holy See", "VA");
            countryCodeMapByCountry.Add("Honduras", "HN");
            countryCodeMapByCountry.Add("Hong Kong", "HK");
            countryCodeMapByCountry.Add("Hungary", "HU");
            countryCodeMapByCountry.Add("Iceland", "IS");
            countryCodeMapByCountry.Add("India", "IN");
            countryCodeMapByCountry.Add("Indonesia", "ID");
            countryCodeMapByCountry.Add("Iran (Islamic Republic of)", "IR");
            countryCodeMapByCountry.Add("Iraq", "IQ");
            countryCodeMapByCountry.Add("Ireland", "IE");
            countryCodeMapByCountry.Add("Isle of Man", "IM");
            countryCodeMapByCountry.Add("Israel", "IL");
            countryCodeMapByCountry.Add("Italy", "IT");
            countryCodeMapByCountry.Add("Jamaica", "JM");
            countryCodeMapByCountry.Add("Japan", "JP");
            countryCodeMapByCountry.Add("Jersey", "JE");
            countryCodeMapByCountry.Add("Jordan", "JO");
            countryCodeMapByCountry.Add("Kazakhstan", "KZ");
            countryCodeMapByCountry.Add("Kenya", "KE");
            countryCodeMapByCountry.Add("Kiribati", "KI");
            countryCodeMapByCountry.Add("Korea (Democratic People's Republic of)", "KP");
            countryCodeMapByCountry.Add("Korea (Republic of)", "KR");
            countryCodeMapByCountry.Add("Kuwait", "KW");
            countryCodeMapByCountry.Add("Kyrgyzstan", "KG");
            countryCodeMapByCountry.Add("Lao People's Democratic Republic", "LA");
            countryCodeMapByCountry.Add("Latvia", "LV");
            countryCodeMapByCountry.Add("Lebanon", "LB");
            countryCodeMapByCountry.Add("Lesotho", "LS");
            countryCodeMapByCountry.Add("Liberia", "LR");
            countryCodeMapByCountry.Add("Libya", "LY");
            countryCodeMapByCountry.Add("Liechtenstein", "LI");
            countryCodeMapByCountry.Add("Lithuania", "LT");
            countryCodeMapByCountry.Add("Luxembourg", "LU");
            countryCodeMapByCountry.Add("Macao", "MO");
            countryCodeMapByCountry.Add("Macedonia (the former Yugoslav Republic of)", "MK");
            countryCodeMapByCountry.Add("Madagascar", "MG");
            countryCodeMapByCountry.Add("Malawi", "MW");
            countryCodeMapByCountry.Add("Malaysia", "MY");
            countryCodeMapByCountry.Add("Maldives", "MV");
            countryCodeMapByCountry.Add("Mali", "ML");
            countryCodeMapByCountry.Add("Malta", "MT");
            countryCodeMapByCountry.Add("Marshall Islands", "MH");
            countryCodeMapByCountry.Add("Martinique", "MQ");
            countryCodeMapByCountry.Add("Mauritania", "MR");
            countryCodeMapByCountry.Add("Mauritius", "MU");
            countryCodeMapByCountry.Add("Mayotte", "YT");
            countryCodeMapByCountry.Add("Mexico", "MX");
            countryCodeMapByCountry.Add("Micronesia (Federated States of)", "FM");
            countryCodeMapByCountry.Add("Moldova (Republic of)", "MD");
            countryCodeMapByCountry.Add("Monaco", "MC");
            countryCodeMapByCountry.Add("Mongolia", "MN");
            countryCodeMapByCountry.Add("Montenegro", "ME");
            countryCodeMapByCountry.Add("Montserrat", "MS");
            countryCodeMapByCountry.Add("Morocco", "MA");
            countryCodeMapByCountry.Add("Mozambique", "MZ");
            countryCodeMapByCountry.Add("Myanmar", "MM");
            countryCodeMapByCountry.Add("Namibia", "NA");
            countryCodeMapByCountry.Add("Nauru", "NR");
            countryCodeMapByCountry.Add("Nepal", "NP");
            countryCodeMapByCountry.Add("Netherlands", "NL");
            countryCodeMapByCountry.Add("New Caledonia", "NC");
            countryCodeMapByCountry.Add("New Zealand", "NZ");
            countryCodeMapByCountry.Add("Nicaragua", "NI");
            countryCodeMapByCountry.Add("Niger", "NE");
            countryCodeMapByCountry.Add("Nigeria", "NG");
            countryCodeMapByCountry.Add("Niue", "NU");
            countryCodeMapByCountry.Add("Norfolk Island", "NF");
            countryCodeMapByCountry.Add("Northern Mariana Islands", "MP");
            countryCodeMapByCountry.Add("Norway", "NO");
            countryCodeMapByCountry.Add("Oman", "OM");
            countryCodeMapByCountry.Add("Pakistan", "PK");
            countryCodeMapByCountry.Add("Palau", "PW");
            countryCodeMapByCountry.Add("Palestine, State of", "PS");
            countryCodeMapByCountry.Add("Panama", "PA");
            countryCodeMapByCountry.Add("Papua New Guinea", "PG");
            countryCodeMapByCountry.Add("Paraguay", "PY");
            countryCodeMapByCountry.Add("Peru", "PE");
            countryCodeMapByCountry.Add("Philippines", "PH");
            countryCodeMapByCountry.Add("Pitcairn", "PN");
            countryCodeMapByCountry.Add("Poland", "PL");
            countryCodeMapByCountry.Add("Portugal", "PT");
            countryCodeMapByCountry.Add("Puerto Rico", "PR");
            countryCodeMapByCountry.Add("Qatar", "QA");
            countryCodeMapByCountry.Add("RÃ©union", "RE");
            countryCodeMapByCountry.Add("Romania", "RO");
            countryCodeMapByCountry.Add("Russian Federation", "RU");
            countryCodeMapByCountry.Add("Rwanda", "RW");
            countryCodeMapByCountry.Add("Saint BarthÃ©lemy", "BL");
            countryCodeMapByCountry.Add("Saint Helena, Ascension and Tristan da Cunha", "SH");
            countryCodeMapByCountry.Add("Saint Kitts and Nevis", "KN");
            countryCodeMapByCountry.Add("Saint Lucia", "LC");
            countryCodeMapByCountry.Add("Saint Martin (French part)", "MF");
            countryCodeMapByCountry.Add("Saint Pierre and Miquelon", "PM");
            countryCodeMapByCountry.Add("Saint Vincent and the Grenadines", "VC");
            countryCodeMapByCountry.Add("Samoa", "WS");
            countryCodeMapByCountry.Add("San Marino", "SM");
            countryCodeMapByCountry.Add("Sao Tome and Principe", "ST");
            countryCodeMapByCountry.Add("Saudi Arabia", "SA");
            countryCodeMapByCountry.Add("Senegal", "SN");
            countryCodeMapByCountry.Add("Serbia", "RS");
            countryCodeMapByCountry.Add("Seychelles", "SC");
            countryCodeMapByCountry.Add("Sierra Leone", "SL");
            countryCodeMapByCountry.Add("Singapore", "SG");
            countryCodeMapByCountry.Add("Sint Maarten (Dutch part)", "SX");
            countryCodeMapByCountry.Add("Slovakia", "SK");
            countryCodeMapByCountry.Add("Slovenia", "SI");
            countryCodeMapByCountry.Add("Solomon Islands", "SB");
            countryCodeMapByCountry.Add("Somalia", "SO");
            countryCodeMapByCountry.Add("South Africa", "ZA");
            countryCodeMapByCountry.Add("South Georgia and the South Sandwich Islands", "GS");
            countryCodeMapByCountry.Add("South Sudan", "SS");
            countryCodeMapByCountry.Add("Spain", "ES");
            countryCodeMapByCountry.Add("Sri Lanka", "LK");
            countryCodeMapByCountry.Add("Sudan", "SD");
            countryCodeMapByCountry.Add("Suriname", "SR");
            countryCodeMapByCountry.Add("Svalbard and Jan Mayen", "SJ");
            countryCodeMapByCountry.Add("Swaziland", "SZ");
            countryCodeMapByCountry.Add("Sweden", "SE");
            countryCodeMapByCountry.Add("Switzerland", "CH");
            countryCodeMapByCountry.Add("Syrian Arab Republic", "SY");
            countryCodeMapByCountry.Add("Taiwan, Province of China", "TW");
            countryCodeMapByCountry.Add("Tajikistan", "TJ");
            countryCodeMapByCountry.Add("Tanzania, United Republic of", "TZ");
            countryCodeMapByCountry.Add("Thailand", "TH");
            countryCodeMapByCountry.Add("Timor-Leste", "TL");
            countryCodeMapByCountry.Add("Togo", "TG");
            countryCodeMapByCountry.Add("Tokelau", "TK");
            countryCodeMapByCountry.Add("Tonga", "TO");
            countryCodeMapByCountry.Add("Trinidad and Tobago", "TT");
            countryCodeMapByCountry.Add("Tunisia", "TN");
            countryCodeMapByCountry.Add("Turkey", "TR");
            countryCodeMapByCountry.Add("Turkmenistan", "TM");
            countryCodeMapByCountry.Add("Turks and Caicos Islands", "TC");
            countryCodeMapByCountry.Add("Tuvalu", "TV");
            countryCodeMapByCountry.Add("Uganda", "UG");
            countryCodeMapByCountry.Add("Ukraine", "UA");
            countryCodeMapByCountry.Add("United Arab Emirates", "AE");
            countryCodeMapByCountry.Add("United Kingdom of Great Britain and Northern Ireland", "GB");
            countryCodeMapByCountry.Add("United States of America", "US");
            countryCodeMapByCountry.Add("United States Minor Outlying Islands", "UM");
            countryCodeMapByCountry.Add("Uruguay", "UY");
            countryCodeMapByCountry.Add("Uzbekistan", "UZ");
            countryCodeMapByCountry.Add("Vanuatu", "VU");
            countryCodeMapByCountry.Add("Venezuela (Bolivarian Republic of)", "VE");
            countryCodeMapByCountry.Add("Viet Nam", "VN");
            countryCodeMapByCountry.Add("Virgin Islands (British)", "VG");
            countryCodeMapByCountry.Add("Virgin Islands (U.S.)", "VI");
            countryCodeMapByCountry.Add("Wallis and Futuna", "WF");
            countryCodeMapByCountry.Add("Western Sahara", "EH");
            countryCodeMapByCountry.Add("Yemen", "YE");
            countryCodeMapByCountry.Add("Zambia", "ZM");
            countryCodeMapByCountry.Add("Zimbabwe", "ZW");

            // build the inverse
            countryMapByCode = new Dictionary<string, string>();
            foreach (var country in countryCodeMapByCountry.Keys)
                countryMapByCode.Add(countryCodeMapByCountry[country], country);
            #endregion
        }

        public static bool IsCountry(Address address, string countryCode) {
            if (address.CountryCode != null)
                return address.CountryCode.Equals(countryCode);
            else if (address.Country != null) {
                string code = GetCountryCodeByCountry(address.Country);
                if (code != null)
                    return code.Equals(countryCode);
                return false;
            }
            return false;
        }

        public static string GetCountryByCode(string countryCode) {
            if (countryCode == null)
                return null;

            // These should be ISO so just check if it's there and return
            if (countryMapByCode.ContainsKey(countryCode))
                return countryMapByCode[countryCode];
            else {
                if (countryCode.Length > 3)
                    return null;

                return FuzzyMatch(countryMapByCode, countryCode, significantCodeMatch);
            }
        }

        public static string GetCountryCodeByCountry(string country) {
            if (country == null)
                return null;

            // These can be tricky... first check for direct match
            if (countryCodeMapByCountry.ContainsKey(country))
                return countryCodeMapByCountry[country];
            else {
                // check the inverse, in case we have a countryCode in the country field
                if (countryMapByCode.ContainsKey(country))
                    return country;
                else {
                    // it's not a country match or a countryCode match so let's get fuzzy
                    string fuzzyCountryMatch = FuzzyMatch(countryCodeMapByCountry, country, significantCountryMatch);
                    if (fuzzyCountryMatch != null)
                        return fuzzyCountryMatch;
                    else {
                        // assume if it's > 3 it's not a code and do not do fuzzy code matching
                        if (country.Length > 3)
                            return null;

                        // 3 or less, let's fuzzy match
                        string fuzzyCodeMatch = FuzzyMatch(countryMapByCode, country, significantCodeMatch);
                        if (fuzzyCodeMatch != null)
                            return countryCodeMapByCountry[fuzzyCodeMatch];
                        return null;
                    }
                }
            }
        }

        private static string FuzzyMatch(Dictionary<string, string> dict, string query, int significantMatch) {
            string rvalue = null;
            Dictionary<string, string> matches = new Dictionary<string, string>();

            // now we can loop
            int highScore = -1;
            foreach (string key in dict.Keys) {
                int score = FuzzyScore(key, query);
                if (score > significantMatch && score > highScore) {
                    matches = new Dictionary<string, string>();

                    highScore = score;
                    rvalue = dict[key];
                    matches.Add(key, rvalue);
                }
                else if (score == highScore) {
                    matches.Add(key, dict[key]);
                }
            }

            if (matches.Count > 1)
                return null;
            return rvalue;
        }

        private static int FuzzyScore(string term, string query) {
            if (term == null || query == null) {
                throw new ArgumentException("Strings must not be null");
            }

            string termLowerCase = term.ToLower();
            string queryLowerCase = query.ToLower();

            int score = 0;
            int termIndex = 0;
            int previousMatchingCharacterIndex = int.MinValue;

            for (int queryIndex = 0; queryIndex < queryLowerCase.Length; queryIndex++) {
                char queryChar = queryLowerCase[queryIndex];

                bool termCharacterMatchFound = false;
                for (; termIndex < termLowerCase.Length && !termCharacterMatchFound; termIndex++) {
                    char termChar = termLowerCase[termIndex];

                    if (queryChar == termChar) {
                        score++;

                        if (previousMatchingCharacterIndex + 1 == termIndex)
                            score += 2;

                        previousMatchingCharacterIndex = termIndex;
                        termCharacterMatchFound = true;
                    }
                }
            }
            return score;
        }
    }
}
