using GlobalPayments.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPayments.Api.Utils {
    public class CountryUtils {       
        private const int significantCountryMatch = 6;
        private const int significantCodeMatch = 3;
        private static CountryData CountryData;
        static CountryUtils(){
            CountryData = new CountryData();
        }

        public static bool IsCountry(Address address, string countryCode){
            if (address.CountryCode != null)
                return address.CountryCode.Equals(countryCode);
            else if (address.Country != null)
            {
                string code = GetCountryCodeByCountry(address.Country);
                if (code != null)
                    return code.Equals(countryCode);
                return false;
            }
            return false;
        }
        /// <summary>        
        /// </summary>
        /// <param name="countryCode"></param>
        /// <param name="countryCodeFormat"></param>
        /// <returns></returns>
        public static string GetCountryByCode(string countryCode){            
            if (countryCode == null)
                return null;

            string output = "";
           
            if (IsAlpha2(countryCode))
            {
                output = ConvertFromAlpha2(countryCode, CountryCodeFormat.Name);
            }
            else if (IsAlpha3(countryCode))
            {
                output = ConvertFromAlpha3(countryCode, CountryCodeFormat.Name);
            }
            else if (IsNumeric(countryCode))
            {
                output = ConvertFromNumeric(countryCode, CountryCodeFormat.Name);
            }            
            if (!string.IsNullOrEmpty(output))
            {
                return output;
            }
            else
            {
                if (countryCode.Length > 3)
                    return null;
                return FuzzyMatch(CountryData.CountryByAlpha2Code, countryCode, significantCodeMatch);
            }
        }
        /// <summary>
        /// Returns Country Code
        /// </summary>
        /// <param name="country"></param>
        /// <param name="countryCodeFormat">Alpha2 is the default</param>
        /// <returns></returns>
        public static string GetCountryCodeByCountry(string country, CountryCodeFormat format = CountryCodeFormat.Alpha2){
            CountryData CountryData = new CountryData();
            string output = "";
            if (country == null)
                return null;

            if (IsCountryName(country))
            {
                output = ConvertFromName(country, format);
            }
            else if (IsAlpha2(country))
            {
                output = ConvertFromAlpha2(country, format);
            }
            else if (IsAlpha3(country))
            {
                output = ConvertFromAlpha3(country, format);
            }
            else if (IsNumeric(country))
            {
                output = ConvertFromNumeric(country, format);
            }

            if (!string.IsNullOrEmpty(output))
            {
                return output;
            }
            else
            {
                // it's not a country match or a countryCode match so let's get fuzzy
                return (FuzzyByFormat(format, country));                
            }
        }
        private static string FuzzyByFormat(CountryCodeFormat format, string country){
            string fuzzyCountryMatch = "";
            string output = "";
            if (format == CountryCodeFormat.Alpha2)
            {
                fuzzyCountryMatch = FuzzyMatch(CountryData.Alpha2CodeByCountry, country, significantCountryMatch);
            }
            else if (format == CountryCodeFormat.Alpha3)
            {
                fuzzyCountryMatch = FuzzyMatch(CountryData.Alpha3CodeByCountry, country, significantCountryMatch);
            }
            else if (format == CountryCodeFormat.Numeric)
            {
                fuzzyCountryMatch = FuzzyMatch(CountryData.NumericCodeByCountry, country, significantCountryMatch);
            }

            if (fuzzyCountryMatch != null)
                return fuzzyCountryMatch;
            else
            {
                // assume if it's > 3 it's not a code and do not do fuzzy code matching
                if (country.Length > 3)
                    return null;

                // 3 or less, let's fuzzy match
                string fuzzyCodeMatch = "";
                if (format == CountryCodeFormat.Alpha2)
                {
                    fuzzyCodeMatch = FuzzyMatch(CountryData.CountryByAlpha2Code, country, significantCodeMatch);
                    if (fuzzyCodeMatch != null)
                        output = CountryData.Alpha2CodeByCountry[fuzzyCodeMatch];
                }
                else if (format == CountryCodeFormat.Alpha3)
                {
                    fuzzyCodeMatch = FuzzyMatch(CountryData.CountryByAlpha3Code, country, significantCodeMatch);
                    if (fuzzyCodeMatch != null)
                        output = CountryData.Alpha3CodeByCountry[fuzzyCodeMatch];
                }
                else if (format == CountryCodeFormat.Numeric)
                {
                    fuzzyCodeMatch = FuzzyMatch(CountryData.CountryByNumericCode, country, significantCodeMatch);
                    if (fuzzyCodeMatch != null)
                        output = CountryData.NumericCodeByCountry[fuzzyCodeMatch];
                }
                if (output != null)
                    return output;
            } return null;

        }
        private static string FuzzyMatch(Dictionary<string, string> dict, string query, int significantMatch)
        {
            string rvalue = null;
            Dictionary<string, string> matches = new Dictionary<string, string>();

            // now we can loop
            int highScore = -1;
            foreach (string key in dict.Keys)
            {
                int score = FuzzyScore(key, query);
                if (score > significantMatch && score > highScore)
                {
                    matches = new Dictionary<string, string>();

                    highScore = score;
                    rvalue = dict[key];
                    matches.Add(key, rvalue);
                }
                else if (score == highScore)
                {
                    matches.Add(key, dict[key]);
                }
            }

            if (matches.Count > 1)
                return matches.First().Value;
            return rvalue;
        }
        private static int FuzzyScore(string term, string query)
        {
            if (term == null || query == null)
            {
                throw new ArgumentException("Strings must not be null");
            }

            string termLowerCase = term.ToLower();
            string queryLowerCase = query.ToLower();

            int score = 0;
            int termIndex = 0;
            int previousMatchingCharacterIndex = int.MinValue;

            for (int queryIndex = 0; queryIndex < queryLowerCase.Length; queryIndex++)
            {
                char queryChar = queryLowerCase[queryIndex];

                bool termCharacterMatchFound = false;
                for (; termIndex < termLowerCase.Length && !termCharacterMatchFound; termIndex++)
                {
                    char termChar = termLowerCase[termIndex];

                    if (queryChar == termChar)
                    {
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
        /// <summary>
        /// Return Numeric Code for country
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public static string GetNumericCodeByCountry(string country){
            CountryData CountryData = new CountryData();
            if (IsCountryName(country) && CountryData.NumericCodeByCountry.ContainsKey(country))
            {
                return CountryData.NumericCodeByCountry[country];
            }
            if (IsAlpha2(country) && CountryData.NumericByAlpha2CountryCode.ContainsKey(country))
            {
                return CountryData.NumericByAlpha2CountryCode[country];
            }
            else if (IsAlpha3(country) && CountryData.NumericByAlpha3CountryCode.ContainsKey(country))
            {
                return CountryData.NumericByAlpha3CountryCode[country];
            }
            else if (IsNumeric(country))
            {
                return country;
            }
            return null;     
        }

        /// <summary>
        /// Converts from Name to requested format
        /// </summary>
        /// <param name="input"></param>
        /// <param name="countryCodeFormat"></param>
        /// <returns></returns>
        private static string ConvertFromName(string input, CountryCodeFormat countryCodeFormat){           

            if (countryCodeFormat == CountryCodeFormat.Alpha2 && CountryData.Alpha2CodeByCountry.ContainsKey(input))
            {
                return CountryData.Alpha2CodeByCountry[input];
            }
            else if (countryCodeFormat == CountryCodeFormat.Alpha3 && CountryData.Alpha3CodeByCountry.ContainsKey(input))
            {
                return CountryData.Alpha3CodeByCountry[input];
            }
            else if (countryCodeFormat == CountryCodeFormat.Numeric && CountryData.NumericCodeByCountry.ContainsKey(input))
            {
                return CountryData.NumericCodeByCountry[input];
            }
            else if (countryCodeFormat == CountryCodeFormat.Name)
            {
                return input;
            }
                return null;
        }
        /// <summary>
        /// Converts from Alpha2 to requested format
        /// </summary>
        /// <param name="input"></param>
        /// <param name="countryCodeFormat"></param>
        /// <returns></returns>
        private static string ConvertFromAlpha2(string input, CountryCodeFormat countryCodeFormat){  
            if (countryCodeFormat == CountryCodeFormat.Numeric && CountryData.NumericByAlpha2CountryCode.ContainsKey(input))
            {
                return CountryData.NumericByAlpha2CountryCode[input];
            }
            else if (countryCodeFormat == CountryCodeFormat.Alpha3 && CountryData.Alpha3CodeByAlpha2Code.ContainsKey(input))
            {
                return CountryData.Alpha3CodeByAlpha2Code[input];
            }
            else if (countryCodeFormat == CountryCodeFormat.Alpha2)
            {
                return input;
            }
            else if (countryCodeFormat == CountryCodeFormat.Name && CountryData.CountryByAlpha2Code.ContainsKey(input))
            {
                return CountryData.CountryByAlpha2Code[input];
            }
            return "";
        }
        /// <summary>
        /// Converts from Alpha3 to requested format
        /// </summary>
        /// <param name="input"></param>
        /// <param name="countryCodeFormat"></param>
        /// <returns></returns>
        private static string ConvertFromAlpha3(string input, CountryCodeFormat countryCodeFormat){          
            if (countryCodeFormat == CountryCodeFormat.Alpha2 && CountryData.Alpha2CodeByAlpha3Code.ContainsKey(input))
            {
                return CountryData.Alpha2CodeByAlpha3Code[input];
            }
            else if (countryCodeFormat == CountryCodeFormat.Numeric && CountryData.NumericByAlpha3CountryCode.ContainsKey(input))
            {
                return CountryData.NumericByAlpha3CountryCode[input];
            }
            else if (countryCodeFormat == CountryCodeFormat.Alpha3)
            {
                return input;
            }
            else if (countryCodeFormat == CountryCodeFormat.Name && CountryData.CountryByAlpha3Code.ContainsKey(input))
            {
                return CountryData.CountryByAlpha3Code[input];
            }
            return "";
        }
        /// <summary>
        /// Converts from Numeric to requested format
        /// </summary>
        /// <param name="input"></param>
        /// <param name="countryCodeFormat"></param>
        /// <returns></returns>
        private static string ConvertFromNumeric(string input, CountryCodeFormat countryCodeFormat){            
            if (countryCodeFormat == CountryCodeFormat.Alpha2 && CountryData.Alpha2CountryCodeByNumeric.ContainsKey(input))
            {
                return CountryData.Alpha2CountryCodeByNumeric[input];
            }
            else if (countryCodeFormat == CountryCodeFormat.Alpha3 && CountryData.Alpha3CountryCodeByNumeric.ContainsKey(input))
            {
                return CountryData.Alpha3CountryCodeByNumeric[input];
            }
            else if (countryCodeFormat == CountryCodeFormat.Numeric)
            {
                return input;
            }
            else if (countryCodeFormat == CountryCodeFormat.Name && CountryData.CountryByNumericCode.ContainsKey(input))
            {
                return CountryData.CountryByNumericCode[input];
            }
            return "";
        }
        private static bool IsCountryName(string input){            
            return CountryData.Alpha2CodeByCountry.ContainsKey(input);
        }
        private static bool IsAlpha2(string input){            
            return CountryData.CountryByAlpha2Code.ContainsKey(input);
        }
        private static bool IsAlpha3(string input){            
            return CountryData.Alpha2CodeByAlpha3Code.ContainsKey(input);
        }
        private static bool IsNumeric(string input){            
            return CountryData.Alpha2CountryCodeByNumeric.ContainsKey(input);
        }
    }
}
