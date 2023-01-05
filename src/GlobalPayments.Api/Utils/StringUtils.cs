using GlobalPayments.Api.Terminals;
using System;
using System.Text.RegularExpressions;

namespace GlobalPayments.Api.Utils {
    public class StringUtils {
        //public static bool IsNullOrEmpty(string value) {
        //    return value == null || value.Trim().Equals("");
        //}
        public static string PadLeft(Object input, int totalLength, char paddingCharacter) {
            if(input == null) {
                input = "";
            }
            return PadLeft(input.ToString(), totalLength, paddingCharacter);
        }
        public static string PadLeft(string input, int totalLength, char paddingCharacter) {
            string rvalue = input;
            if(rvalue == null) {
                rvalue = "";
            }

            while(rvalue.Length < totalLength) {
                rvalue = paddingCharacter + rvalue;
            }
            return rvalue;
        }
        public static string PadRight(string input, int totalLength, char paddingCharacter) {
            string rvalue = input;
            if(rvalue == null) {
                rvalue = "";
            }

            while(rvalue.Length < totalLength) {
                rvalue = rvalue + paddingCharacter;
            }
            return rvalue;
        }
        public static decimal ToAmount(string str) {
            if(string.IsNullOrEmpty(str))
                return 0;

            decimal amount = decimal.Parse(str);
            return decimal.Divide(amount,100m);
        }
        public static decimal ToFractionalAmount(string str) {
            if(string.IsNullOrEmpty(str)) {
                return 0;
            }

            int numDecimals = int.Parse(str.Substring(0, 1));
            int shiftValue = int.Parse(StringUtils.PadRight("1", numDecimals + 1, '0'));

            decimal qty = Math.Round(decimal.Parse(str.Substring(1)),numDecimals);
            return decimal.Divide(qty,new decimal(shiftValue));
        }
        public static string ToNumeric(decimal? amount) {
            if (amount == null) {
                return "";
            }
            else if (amount.ToString().Equals("0")) {
                return "000";
            }
            string currency = string.Format("{0:C}", amount);
            Regex rgx = new Regex("[^0-9]");
            //return TrimStart(currency.Replace("[^0-9]", ""), "0");
            return TrimStart(rgx.Replace(currency, ""), "0");
        }
        public static string ToNumeric(decimal? amount, int Length) {
            string rvalue = ToNumeric(amount);
            return PadLeft(rvalue, Length, '0');
        }
        public static string ToFractionalNumeric(decimal amount) {
            if(amount == 0) {
                return "";
            }
            int numberPlaces = BitConverter.GetBytes(decimal.GetBits(amount)[3])[2];            
            string rvalue = TrimStart(amount.ToString().Replace(".", ""), "0");
            return numberPlaces + rvalue;
        }
        public static string Join(string separator, Object[] fields) {
            string rvalue = "";
            foreach (var field in fields) {
                if (field == null) {
                    rvalue = string.Concat(rvalue, "" + separator);
                }
                else {
                    rvalue = string.Concat(rvalue, field.ToString() + separator);
                }
            }
            return rvalue.Substring(0, rvalue.Length - separator.Length);
        }
        public static string Trim(string str) {
            string rvalue = TrimEnd(str);
            return TrimStart(rvalue);
        }
        public static string TrimEnd(string str) {
            return TrimEnd(str, " ");
        }
        public static string TrimEnd(string str, string trimString) {
            string rvalue = str;
            while(rvalue.EndsWith(trimString)) {
                int trimLength = trimString.Length;
                rvalue = rvalue.Substring(0, rvalue.Length - trimLength);
            }
            return rvalue;
        }
        public static string TrimEnd(string str, params string[] trimChars) {
            string rvalue = str;
            foreach(string trimChar in trimChars) {
                rvalue = TrimEnd(rvalue, trimChar);
            }
            return rvalue;
        }
        public static string TrimEnd(string str, ControlCodes code) {
            // Strip the nulls off
            str = str.Replace("null", "");
            string trimChar = (char)code + "";

            return TrimEnd(str, trimChar);
        }
        public static string TrimStart(string str) {
            return TrimStart(str, " ");
        }
        public static string TrimStart(string str, string trimString) {
            string rvalue = str;
            while(rvalue.StartsWith(trimString)) {
                int trimLength = trimString.Length;
                rvalue = rvalue.Substring(trimLength);
            }
            return rvalue;
        }
        public static string TrimStart(string str, params string[] trimChars) {
            string rvalue = str;
            foreach(string trimChar in trimChars) {
                rvalue = TrimStart(rvalue, trimChar);
            }
            return rvalue;
        }
        public static string ToLVar(string str) {
            string Length = PadLeft(str.Length + "", 1, '0');
            return Length + str;
        }
        public static string ToLLVar(string str) {
            string Length = PadLeft(str.Length + "", 2, '0');
            return Length + str;
        }
        public static string ToLLLVar(string str) {
            string Length = PadLeft(str.Length + "", 3, '0');
            return Length + str;
        }   
        public static byte[] BytesFromHex(string hexString) {
            string s = hexString.ToLower();

            byte[] b = new byte[s.Length / 2];
            for (int i = 0; i < b.Length; i++) {
                int index = i * 2;
                int v =Convert.ToInt32(s.Substring(index, 2), 16);
                b[i] = (byte) v;
            }
            return b;
        }
        public static string HexFromBytes(byte[] buffer) {
            char[] hexArray = "0123456789ABCDEF".ToCharArray();
            char[] hexChars = new char[buffer.Length * 2];
            for ( int j = 0; j < buffer.Length; j++ ) {
                int v = buffer[j] & 0xFF;
                hexChars[j * 2] = hexArray[(uint)v >> 4];
                hexChars[j * 2 + 1] = hexArray[v & 0x0F];
            }
            return new string(hexChars);
        }

	    public static string Mask(string value) {
	    	string masked = null;
	    	Regex regex = new Regex(@"\\b(?:4[ -]*(?:\\d[ -]*){11}(?:(?:\\d[ -]*){3})?\\d|"
	    			+ "(?:5[ -]*[1-5](?:[ -]*\\d){2}|(?:2[ -]*){3}[1-9]|(?:2[ -]*){2}[3-9][ -]*"
	    			+ "\\d|2[ -]*[3-6](?:[ -]*\\d){2}|2[ -]*7[ -]*[01][ -]*\\d|2[ -]*7[ -]*2[ -]*0)(?:[ -]*"
	    			+ "\\d){12}|3[ -]*[47](?:[ -]*\\d){13}|3[ -]*(?:0[ -]*[0-5]|[68][ -]*\\d)(?:[ -]*"
	    			+ "\\d){11}|6[ -]*(?:0[ -]*1[ -]*1|5[ -]*\\d[ -]*\\d)(?:[ -]*"
	    			+ "\\d){12}|(?:2[ -]*1[ -]*3[ -]*1|1[ -]*8[ -]*0[ -]*0|3[ -]*5(?:[ -]*"
	    			+ "\\d){3})(?:[ -]*\\d){11})\\b");

	    	Match regexMatcher = regex.Match(value);
	    	if (regexMatcher!=null && regexMatcher.Success) {
                string card = regexMatcher.Groups[0].ToString();
	    		string strippedCard = card.Replace("[ -]+", "");
	    		string subSectionOfCard = strippedCard.Substring(6, strippedCard.Length - 4);
	    		string prefix = strippedCard.Substring(0, 6);
	    		string middle = PadLeft("X", subSectionOfCard.Length, 'X');
	    		string suffix = strippedCard.Substring(strippedCard.Length - 4, strippedCard.Length);
	    		string maskedCard = prefix + middle + suffix;
	    		masked = value.Replace(card, maskedCard);
	    	} else {
	    		masked = value;
	    	}
	    	return masked;
	    }
    }
}
