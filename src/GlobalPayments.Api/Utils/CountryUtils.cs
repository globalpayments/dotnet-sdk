using GlobalPayments.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPayments.Api.Utils {
    public class CountryUtils {
        private static Dictionary<string, string> countryCodeMapByCountry;
        private static Dictionary<string, string> countryMapByCountryCode;
        private static Dictionary<string, string> countryCodeMapByNumericCode;
        private static Dictionary<string, string> numericCodeMapByCountryCode;
        private const int significantCountryMatch = 6;
        private const int significantCodeMatch = 3;

        static CountryUtils() {
            #region build dictionaries
            countryCodeMapByCountry = new Dictionary<string, string>();
            countryCodeMapByCountry.Add("Afghanistan", "AF");
            countryCodeMapByCountry.Add("Åland Islands", "AX");
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
            countryCodeMapByCountry.Add("Côte d'Ivoire", "CI");
            countryCodeMapByCountry.Add("Croatia", "HR");
            countryCodeMapByCountry.Add("Cuba", "CU");
            countryCodeMapByCountry.Add("Curaçao", "CW");
            countryCodeMapByCountry.Add("Cyprus", "CY");
            countryCodeMapByCountry.Add("Czechia", "CZ");
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
            countryCodeMapByCountry.Add("North Macedonia", "MK");
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
            countryCodeMapByCountry.Add("Netherlands Antilles", "AN");
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
            countryCodeMapByCountry.Add("Réunion", "RE");
            countryCodeMapByCountry.Add("Romania", "RO");
            countryCodeMapByCountry.Add("Russian Federation", "RU");
            countryCodeMapByCountry.Add("Rwanda", "RW");
            countryCodeMapByCountry.Add("Saint Barthélemy", "BL");
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
            countryCodeMapByCountry.Add("Eswatini", "SZ");
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
            countryMapByCountryCode = new Dictionary<string, string>();
            foreach (var country in countryCodeMapByCountry.Keys) {
                countryMapByCountryCode.Add(countryCodeMapByCountry[country], country);
            }

            countryCodeMapByNumericCode = new Dictionary<string, string>();
            countryCodeMapByNumericCode.Add("004", "AF");
            countryCodeMapByNumericCode.Add("008", "AL");
            countryCodeMapByNumericCode.Add("010", "AQ");
            countryCodeMapByNumericCode.Add("012", "DZ");
            countryCodeMapByNumericCode.Add("016", "AS");
            countryCodeMapByNumericCode.Add("020", "AD");
            countryCodeMapByNumericCode.Add("024", "AO");
            countryCodeMapByNumericCode.Add("028", "AG");
            countryCodeMapByNumericCode.Add("031", "AZ");
            countryCodeMapByNumericCode.Add("032", "AR");
            countryCodeMapByNumericCode.Add("036", "AU");
            countryCodeMapByNumericCode.Add("040", "AT");
            countryCodeMapByNumericCode.Add("044", "BS");
            countryCodeMapByNumericCode.Add("048", "BH");
            countryCodeMapByNumericCode.Add("050", "BD");
            countryCodeMapByNumericCode.Add("051", "AM");
            countryCodeMapByNumericCode.Add("052", "BB");
            countryCodeMapByNumericCode.Add("056", "BE");
            countryCodeMapByNumericCode.Add("060", "BM");
            countryCodeMapByNumericCode.Add("064", "BT");
            countryCodeMapByNumericCode.Add("068", "BO");
            countryCodeMapByNumericCode.Add("070", "BA");
            countryCodeMapByNumericCode.Add("072", "BW");
            countryCodeMapByNumericCode.Add("074", "BV");
            countryCodeMapByNumericCode.Add("076", "BR");
            countryCodeMapByNumericCode.Add("084", "BZ");
            countryCodeMapByNumericCode.Add("086", "IO");
            countryCodeMapByNumericCode.Add("090", "SB");
            countryCodeMapByNumericCode.Add("092", "VG");
            countryCodeMapByNumericCode.Add("096", "BN");
            countryCodeMapByNumericCode.Add("100", "BG");
            countryCodeMapByNumericCode.Add("104", "MM");
            countryCodeMapByNumericCode.Add("108", "BI");
            countryCodeMapByNumericCode.Add("112", "BY");
            countryCodeMapByNumericCode.Add("116", "KH");
            countryCodeMapByNumericCode.Add("120", "CM");
            countryCodeMapByNumericCode.Add("124", "CA");
            countryCodeMapByNumericCode.Add("132", "CV");
            countryCodeMapByNumericCode.Add("136", "KY");
            countryCodeMapByNumericCode.Add("140", "CF");
            countryCodeMapByNumericCode.Add("144", "LK");
            countryCodeMapByNumericCode.Add("148", "TD");
            countryCodeMapByNumericCode.Add("152", "CL");
            countryCodeMapByNumericCode.Add("156", "CN");
            countryCodeMapByNumericCode.Add("158", "TW");
            countryCodeMapByNumericCode.Add("162", "CX");
            countryCodeMapByNumericCode.Add("166", "CC");
            countryCodeMapByNumericCode.Add("170", "CO");
            countryCodeMapByNumericCode.Add("174", "KM");
            countryCodeMapByNumericCode.Add("175", "YT");
            countryCodeMapByNumericCode.Add("178", "CG");
            countryCodeMapByNumericCode.Add("180", "CD");
            countryCodeMapByNumericCode.Add("184", "CK");
            countryCodeMapByNumericCode.Add("188", "CR");
            countryCodeMapByNumericCode.Add("191", "HR");
            countryCodeMapByNumericCode.Add("192", "CU");
            countryCodeMapByNumericCode.Add("196", "CY");
            countryCodeMapByNumericCode.Add("203", "CZ");
            countryCodeMapByNumericCode.Add("204", "BJ");
            countryCodeMapByNumericCode.Add("208", "DK");
            countryCodeMapByNumericCode.Add("212", "DM");
            countryCodeMapByNumericCode.Add("214", "DO");
            countryCodeMapByNumericCode.Add("218", "EC");
            countryCodeMapByNumericCode.Add("222", "SV");
            countryCodeMapByNumericCode.Add("226", "GQ");
            countryCodeMapByNumericCode.Add("231", "ET");
            countryCodeMapByNumericCode.Add("232", "ER");
            countryCodeMapByNumericCode.Add("233", "EE");
            countryCodeMapByNumericCode.Add("234", "FO");
            countryCodeMapByNumericCode.Add("238", "FK");
            countryCodeMapByNumericCode.Add("239", "GS");
            countryCodeMapByNumericCode.Add("242", "FJ");
            countryCodeMapByNumericCode.Add("246", "FI");
            countryCodeMapByNumericCode.Add("248", "AX");
            countryCodeMapByNumericCode.Add("250", "FR");
            countryCodeMapByNumericCode.Add("254", "GF");
            countryCodeMapByNumericCode.Add("258", "PF");
            countryCodeMapByNumericCode.Add("260", "TF");
            countryCodeMapByNumericCode.Add("262", "DJ");
            countryCodeMapByNumericCode.Add("266", "GA");
            countryCodeMapByNumericCode.Add("268", "GE");
            countryCodeMapByNumericCode.Add("270", "GM");
            countryCodeMapByNumericCode.Add("275", "PS");
            countryCodeMapByNumericCode.Add("276", "DE");
            countryCodeMapByNumericCode.Add("288", "GH");
            countryCodeMapByNumericCode.Add("292", "GI");
            countryCodeMapByNumericCode.Add("296", "KI");
            countryCodeMapByNumericCode.Add("300", "GR");
            countryCodeMapByNumericCode.Add("304", "GL");
            countryCodeMapByNumericCode.Add("308", "GD");
            countryCodeMapByNumericCode.Add("312", "GP");
            countryCodeMapByNumericCode.Add("316", "GU");
            countryCodeMapByNumericCode.Add("320", "GT");
            countryCodeMapByNumericCode.Add("324", "GN");
            countryCodeMapByNumericCode.Add("328", "GY");
            countryCodeMapByNumericCode.Add("332", "HT");
            countryCodeMapByNumericCode.Add("334", "HM");
            countryCodeMapByNumericCode.Add("336", "VA");
            countryCodeMapByNumericCode.Add("340", "HN");
            countryCodeMapByNumericCode.Add("344", "HK");
            countryCodeMapByNumericCode.Add("348", "HU");
            countryCodeMapByNumericCode.Add("352", "IS");
            countryCodeMapByNumericCode.Add("356", "IN");
            countryCodeMapByNumericCode.Add("360", "ID");
            countryCodeMapByNumericCode.Add("364", "IR");
            countryCodeMapByNumericCode.Add("368", "IQ");
            countryCodeMapByNumericCode.Add("372", "IE");
            countryCodeMapByNumericCode.Add("376", "IL");
            countryCodeMapByNumericCode.Add("380", "IT");
            countryCodeMapByNumericCode.Add("384", "CI");
            countryCodeMapByNumericCode.Add("388", "JM");
            countryCodeMapByNumericCode.Add("392", "JP");
            countryCodeMapByNumericCode.Add("398", "KZ");
            countryCodeMapByNumericCode.Add("400", "JO");
            countryCodeMapByNumericCode.Add("404", "KE");
            countryCodeMapByNumericCode.Add("408", "KP");
            countryCodeMapByNumericCode.Add("410", "KR");
            countryCodeMapByNumericCode.Add("414", "KW");
            countryCodeMapByNumericCode.Add("417", "KG");
            countryCodeMapByNumericCode.Add("418", "LA");
            countryCodeMapByNumericCode.Add("422", "LB");
            countryCodeMapByNumericCode.Add("426", "LS");
            countryCodeMapByNumericCode.Add("428", "LV");
            countryCodeMapByNumericCode.Add("430", "LR");
            countryCodeMapByNumericCode.Add("434", "LY");
            countryCodeMapByNumericCode.Add("438", "LI");
            countryCodeMapByNumericCode.Add("440", "LT");
            countryCodeMapByNumericCode.Add("442", "LU");
            countryCodeMapByNumericCode.Add("446", "MO");
            countryCodeMapByNumericCode.Add("450", "MG");
            countryCodeMapByNumericCode.Add("454", "MW");
            countryCodeMapByNumericCode.Add("458", "MY");
            countryCodeMapByNumericCode.Add("462", "MV");
            countryCodeMapByNumericCode.Add("466", "ML");
            countryCodeMapByNumericCode.Add("470", "MT");
            countryCodeMapByNumericCode.Add("474", "MQ");
            countryCodeMapByNumericCode.Add("478", "MR");
            countryCodeMapByNumericCode.Add("480", "MU");
            countryCodeMapByNumericCode.Add("484", "MX");
            countryCodeMapByNumericCode.Add("492", "MC");
            countryCodeMapByNumericCode.Add("496", "MN");
            countryCodeMapByNumericCode.Add("498", "MD");
            countryCodeMapByNumericCode.Add("499", "ME");
            countryCodeMapByNumericCode.Add("500", "MS");
            countryCodeMapByNumericCode.Add("504", "MA");
            countryCodeMapByNumericCode.Add("508", "MZ");
            countryCodeMapByNumericCode.Add("512", "OM");
            countryCodeMapByNumericCode.Add("516", "NA");
            countryCodeMapByNumericCode.Add("520", "NR");
            countryCodeMapByNumericCode.Add("524", "NP");
            countryCodeMapByNumericCode.Add("528", "NL");
            countryCodeMapByNumericCode.Add("530", "AN");
            countryCodeMapByNumericCode.Add("531", "CW");
            countryCodeMapByNumericCode.Add("533", "AW");
            countryCodeMapByNumericCode.Add("534", "SX");
            countryCodeMapByNumericCode.Add("535", "BQ");
            countryCodeMapByNumericCode.Add("540", "NC");
            countryCodeMapByNumericCode.Add("548", "VU");
            countryCodeMapByNumericCode.Add("554", "NZ");
            countryCodeMapByNumericCode.Add("558", "NI");
            countryCodeMapByNumericCode.Add("562", "NE");
            countryCodeMapByNumericCode.Add("566", "NG");
            countryCodeMapByNumericCode.Add("570", "NU");
            countryCodeMapByNumericCode.Add("574", "NF");
            countryCodeMapByNumericCode.Add("578", "NO");
            countryCodeMapByNumericCode.Add("580", "MP");
            countryCodeMapByNumericCode.Add("581", "UM");
            countryCodeMapByNumericCode.Add("583", "FM");
            countryCodeMapByNumericCode.Add("584", "MH");
            countryCodeMapByNumericCode.Add("585", "PW");
            countryCodeMapByNumericCode.Add("586", "PK");
            countryCodeMapByNumericCode.Add("591", "PA");
            countryCodeMapByNumericCode.Add("598", "PG");
            countryCodeMapByNumericCode.Add("600", "PY");
            countryCodeMapByNumericCode.Add("604", "PE");
            countryCodeMapByNumericCode.Add("608", "PH");
            countryCodeMapByNumericCode.Add("612", "PN");
            countryCodeMapByNumericCode.Add("616", "PL");
            countryCodeMapByNumericCode.Add("620", "PT");
            countryCodeMapByNumericCode.Add("624", "GW");
            countryCodeMapByNumericCode.Add("626", "TL");
            countryCodeMapByNumericCode.Add("630", "PR");
            countryCodeMapByNumericCode.Add("634", "QA");
            countryCodeMapByNumericCode.Add("638", "RE");
            countryCodeMapByNumericCode.Add("642", "RO");
            countryCodeMapByNumericCode.Add("643", "RU");
            countryCodeMapByNumericCode.Add("646", "RW");
            countryCodeMapByNumericCode.Add("652", "BL");
            countryCodeMapByNumericCode.Add("654", "SH");
            countryCodeMapByNumericCode.Add("659", "KN");
            countryCodeMapByNumericCode.Add("660", "AI");
            countryCodeMapByNumericCode.Add("662", "LC");
            countryCodeMapByNumericCode.Add("663", "MF");
            countryCodeMapByNumericCode.Add("666", "PM");
            countryCodeMapByNumericCode.Add("670", "VC");
            countryCodeMapByNumericCode.Add("674", "SM");
            countryCodeMapByNumericCode.Add("678", "ST");
            countryCodeMapByNumericCode.Add("682", "SA");
            countryCodeMapByNumericCode.Add("686", "SN");
            countryCodeMapByNumericCode.Add("688", "RS");
            countryCodeMapByNumericCode.Add("690", "SC");
            countryCodeMapByNumericCode.Add("694", "SL");
            countryCodeMapByNumericCode.Add("702", "SG");
            countryCodeMapByNumericCode.Add("703", "SK");
            countryCodeMapByNumericCode.Add("704", "VN");
            countryCodeMapByNumericCode.Add("705", "SI");
            countryCodeMapByNumericCode.Add("706", "SO");
            countryCodeMapByNumericCode.Add("710", "ZA");
            countryCodeMapByNumericCode.Add("716", "ZW");
            countryCodeMapByNumericCode.Add("724", "ES");
            countryCodeMapByNumericCode.Add("728", "SS");
            countryCodeMapByNumericCode.Add("729", "SD");
            countryCodeMapByNumericCode.Add("732", "EH");
            countryCodeMapByNumericCode.Add("740", "SR");
            countryCodeMapByNumericCode.Add("744", "SJ");
            countryCodeMapByNumericCode.Add("748", "SZ");
            countryCodeMapByNumericCode.Add("752", "SE");
            countryCodeMapByNumericCode.Add("756", "CH");
            countryCodeMapByNumericCode.Add("760", "SY");
            countryCodeMapByNumericCode.Add("762", "TJ");
            countryCodeMapByNumericCode.Add("764", "TH");
            countryCodeMapByNumericCode.Add("768", "TG");
            countryCodeMapByNumericCode.Add("772", "TK");
            countryCodeMapByNumericCode.Add("776", "TO");
            countryCodeMapByNumericCode.Add("780", "TT");
            countryCodeMapByNumericCode.Add("784", "AE");
            countryCodeMapByNumericCode.Add("788", "TN");
            countryCodeMapByNumericCode.Add("792", "TR");
            countryCodeMapByNumericCode.Add("795", "TM");
            countryCodeMapByNumericCode.Add("796", "TC");
            countryCodeMapByNumericCode.Add("798", "TV");
            countryCodeMapByNumericCode.Add("800", "UG");
            countryCodeMapByNumericCode.Add("804", "UA");
            countryCodeMapByNumericCode.Add("807", "MK");
            countryCodeMapByNumericCode.Add("818", "EG");
            countryCodeMapByNumericCode.Add("826", "GB");
            countryCodeMapByNumericCode.Add("831", "GG");
            countryCodeMapByNumericCode.Add("832", "JE");
            countryCodeMapByNumericCode.Add("833", "IM");
            countryCodeMapByNumericCode.Add("834", "TZ");
            countryCodeMapByNumericCode.Add("840", "US");
            countryCodeMapByNumericCode.Add("850", "VI");
            countryCodeMapByNumericCode.Add("854", "BF");
            countryCodeMapByNumericCode.Add("858", "UY");
            countryCodeMapByNumericCode.Add("860", "UZ");
            countryCodeMapByNumericCode.Add("862", "VE");
            countryCodeMapByNumericCode.Add("876", "WF");
            countryCodeMapByNumericCode.Add("882", "WS");
            countryCodeMapByNumericCode.Add("887", "YE");
            countryCodeMapByNumericCode.Add("894", "ZM");

            // build the inverse
            numericCodeMapByCountryCode = new Dictionary<string, string>();
            foreach (var numericCode in countryCodeMapByNumericCode.Keys) {
                numericCodeMapByCountryCode.Add(countryCodeMapByNumericCode[numericCode], numericCode);
            }
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
            if (countryMapByCountryCode.ContainsKey(countryCode))
                return countryMapByCountryCode[countryCode];
            else {
                if (countryCode.Length > 3)
                    return null;

                return FuzzyMatch(countryMapByCountryCode, countryCode, significantCodeMatch);
            }
        }

        public static string GetCountryCodeByCountry(string country) {
            if (country == null)
                return null;

            // These can be tricky... first check for direct match
            if (countryCodeMapByCountry.ContainsKey(country)) {
                return countryCodeMapByCountry[country];
            }
            else {
                // check the inverse, in case we have a countryCode in the country field
                if (countryMapByCountryCode.ContainsKey(country)) {
                    return country;
                }
                else {
                    // check for codes in case we have a numericCode in the country field
                    if (countryCodeMapByNumericCode.ContainsKey(country)) {
                        return countryCodeMapByNumericCode[country];
                    }
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
                            string fuzzyCodeMatch = FuzzyMatch(countryMapByCountryCode, country, significantCodeMatch);
                            if (fuzzyCodeMatch != null)
                                return countryCodeMapByCountry[fuzzyCodeMatch];
                            return null;
                        }
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
                return matches.First().Value;
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

        public static string GetNumericCodeByCountry(string country) {
            if (countryCodeMapByNumericCode.ContainsKey(country)) {
                return country;
            }
            else {
                string countryCode = GetCountryCodeByCountry(country);
                if (countryCode != null && numericCodeMapByCountryCode.ContainsKey(countryCode)) {
                    return numericCodeMapByCountryCode[countryCode];
                }
                return null;
            }
        }
    }
}
