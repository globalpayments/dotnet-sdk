using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GlobalPayments.Api.Utils {
    public class EmvUtils {
        private static Dictionary<string, string> knownTags;
        private static Dictionary<string, string> blackList;
        private static Dictionary<string, string> dataTypes;

        static EmvUtils() {
            blackList = new Dictionary<string, string>();
            blackList["57"] = "Track 2 Equivalent Data";

            blackList["5A"] = "Application Primary Account Number (PAN)";
            blackList["99"] = "Transaction PIN Data";
            blackList["5F20"] = "Cardholder Name";
            blackList["5F24"] = "Application Expiration Date";
            blackList["9F0B"] = "Cardholder Name Extended";
            blackList["9F1F"] = "Track 1 Discretionary Data";
            blackList["9F20"] = "Track 2 Discretionary Data";

            knownTags = new Dictionary<string, string>();
            knownTags["4F"] = "Application Dedicated File (ADF) Name";
            knownTags["50"] = "Application Label";
            knownTags["6F"] = "File Control Information (FCI) Template";
            knownTags["71"] = "Issuer Script Template 1";
            knownTags["72"] = "Issuer Script Template 2";
            knownTags["82"] = "Application Interchange Profile";
            knownTags["84"] = "Dedicated File (DF) Name";
            knownTags["86"] = "Issuer Script Command";
            knownTags["87"] = "Application Priority Indicator";
            knownTags["88"] = "Short File Identifier (SFI)";
            knownTags["8A"] = "Authorization Response Code (ARC)";
            knownTags["8C"] = "Card Rick Management Data Object List 1 (CDOL1)";
            knownTags["8D"] = "Card Rick Management Data Object List 2 (CDOL2)";
            knownTags["8E"] = "Cardholder Verification Method (CVM) List";
            knownTags["8F"] = "Certification Authority Public Key Index";
            knownTags["90"] = "Issuer Public Key Certificate";
            knownTags["91"] = "Issuer Authentication Data";
            knownTags["92"] = "Issuer Public Key Remainder";
            knownTags["93"] = "Signed Static Application Data";
            knownTags["94"] = "Application File Locator (AFL)";
            knownTags["95"] = "Terminal Verification Results (TVR)";
            knownTags["97"] = "Transaction Certification Data Object List (TDOL)";
            knownTags["9A"] = "Transaction Date";
            knownTags["9B"] = "Transaction Status Indicator";
            knownTags["9C"] = "Transaction Type";
            knownTags["9D"] = "Directory Definition File (DDF) Name";

            knownTags["5F25"] = "Application Effective Date";
            knownTags["5F28"] = "Issuer Country Code";
            knownTags["5F2A"] = "Transaction Currency Code";
            knownTags["5F2D"] = "Language Preference";
            knownTags["5F30"] = "Service Code";
            knownTags["5F34"] = "Application Primary Account Number (PAN) Sequence Number";
            knownTags["5F36"] = "Transaction Currency Exponent";

            knownTags["9F01"] = "Unknown";
            knownTags["9F02"] = "Amount, Authorized";
            knownTags["9F03"] = "Amount, Other";
            knownTags["9F05"] = "Application Discretionary Data";
            knownTags["9F06"] = "Application Identifier (AID)";
            knownTags["9F07"] = "Application Usage Control";
            knownTags["9F08"] = "Application Version Number";
            knownTags["9F09"] = "Application Version Number";
            knownTags["9F0D"] = "Issuer Action Code (IAC) - Default";
            knownTags["9F0E"] = "Issuer Action Code (IAC) - Denial";
            knownTags["9F0F"] = "Issuer Action Code (IAC) - Online";

            knownTags["9F10"] = "Issuer Application Data";
            knownTags["9F11"] = "Issuer Code Table Index";
            knownTags["9F12"] = "Application Preferred Name";
            knownTags["9F13"] = "Last Online Application Transaction Counter (ATC) Register";
            knownTags["9F14"] = "Lower Consecutive Offline Limit";
            knownTags["9F16"] = "Unknown";
            knownTags["9F17"] = "Personal Identification Number (PIN) Try Counter";
            knownTags["9F1A"] = "Terminal Country Code";
            knownTags["9F1B"] = "Terminal Floor Limit";
            knownTags["9F1C"] = "Unknown";
            knownTags["9F1D"] = "Terminal Risk Management Data";
            knownTags["9F1E"] = "Interface Device (IFD) Serial Number";

            knownTags["9F21"] = "Transaction Time";
            knownTags["9F22"] = "Certification Authority Public Key Modulus";
            knownTags["9F23"] = "Upper Consecutive Offline Limit";
            knownTags["9F26"] = "Application Cryptogram";
            knownTags["9F27"] = "Cryptogram Information Data";
            knownTags["9F2D"] = "Integrated Circuit Card (ICC) PIN Encipherment Public Key Certificate";
            knownTags["9F2E"] = "Integrated Circuit Card (ICC) PIN Encipherment Public Key Exponent";
            knownTags["9F2F"] = "Integrated Circuit Card (ICC) PIN Encipherment Public Key Remainder";

            knownTags["9F32"] = "Issuer Public Key Exponent";
            knownTags["9F33"] = "Terminal Capabilities";
            knownTags["9F34"] = "Cardholder Verification Method (CVM) Results";
            knownTags["9F35"] = "Terminal Type";
            knownTags["9F36"] = "Application Transaction Counter (ATC)";
            knownTags["9F37"] = "Unpredictable Number";
            knownTags["9F38"] = "Processing Options Data Object List (PDOL)";
            knownTags["9F39"] = "Point-Of-Service (POS) Entry Mode";
            knownTags["9F3B"] = "Application Reference Currency";
            knownTags["9F3C"] = "Transaction Reference Currency Code";
            knownTags["9F3D"] = "Transaction Reference Currency Conversion";

            knownTags["9F40"] = "Additional Terminal Capabilities";
            knownTags["9F41"] = "Transaction Sequence Counter";
            knownTags["9F42"] = "Application Currency Code";
            knownTags["9F43"] = "Application Reference Currency Exponent";
            knownTags["9F44"] = "Application Currency Exponent";
            knownTags["9F46"] = "Integrated Circuit Card (ICC) Public Key Certificate";
            knownTags["9F47"] = "Integrated Circuit Card (ICC) Public Key Exponent";
            knownTags["9F48"] = "Integrated Circuit Card (ICC) Public Key Remainder";
            knownTags["9F49"] = "Dynamic Data Authentication Data Object List (DDOL)";
            knownTags["9F4A"] = "Signed Data Authentication Tag List";
            knownTags["9F4B"] = "Signed Dynamic Application Data";
            knownTags["9F4C"] = "ICC Dynamic Number";
            knownTags["9F4E"] = "Unknown";

            knownTags["9F5B"] = "Issuer Script Results";
            knownTags["9F6E"] = "Form Factor Indicator/Third Party Data";
            knownTags["9F7C"] = "Customer Exclusive Data";

            knownTags["FFC6"] = "Terminal Action Code (TAC) Default";
            knownTags["FFC7"] = "Terminal Action Code (TAC) Denial";
            knownTags["FFC8"] = "Terminal Action Code (TAC) Online";

            // WEX EMV
            knownTags["42"] = "Issuer Identification Number (IIN or BIN)";
            knownTags["61"] = "Directory entry Template";
            knownTags["70"] = "Record Template";
            knownTags["73"] = "Directory Discretionary Template";
            knownTags["9F4D"] = "Log Entry";
            knownTags["9F4F"] = "Transaction Log Format";
            knownTags["9F52"] = "Card Verification Results (CVR)";
            knownTags["9F7E"] = "Issuer Life Cycle Data";
            knownTags["A5"] = "FCI Proprietary Template";
            knownTags["BF0C"] = "FCI Issuer Discretionary Data";
            knownTags["BF20"] = "PRO 00";
            knownTags["BF27"] = "PRO 07";
            knownTags["BF2E"] = "PRO 14";
            knownTags["C1"] = "Application Control";
            knownTags["C4"] = "Default Contact Profile31";
            knownTags["CA"] = "Previous Transaction History";
            knownTags["CB"] = "CRM Country Code";
            knownTags["CD"] = "CRM Currency Code";
            knownTags["D3"] = "PDOL Related data Length";
            knownTags["D8"] = "CAFL";
            knownTags["DF01"] = "Proprietary Data Element n°1";
            knownTags["DF02"] = "Proprietary Data Element n°2";
            knownTags["DF03"] = "Proprietary Data Element n°3";
            knownTags["DF04"] = "Proprietary Data Element n°4";
            knownTags["DF05"] = "Proprietary Data Element n°5";
            knownTags["DF06"] = "Proprietary Data Element n°6";
            knownTags["DF07"] = "Proprietary Data Element n°7";
            knownTags["DF08"] = "Proprietary Data Element n°8";
            knownTags["DF10"] = "Profile Selection Table";
            knownTags["DF11"] = "Currency Conversion Code 1";
            knownTags["DF12"] = "Currency Conversion Code 2";
            knownTags["DF13"] = "COTN counter";
            knownTags["DF14"] = "COTA accumulator";

            knownTags["DF15"] = "CIAC – Denial";
            knownTags["DF16"] = "CIAC – Default";
            knownTags["DF17"] = "CIAC – Online";
            knownTags["DF18"] = "LCOTA limit ";
            knownTags["DF19"] = "UCOTA limit";
            knownTags["DF1A"] = "MTAL limit ";
            knownTags["DF1B"] = "LCOL limit";

            knownTags["DF1C"] = "Upper Consecutive Offline Limit (UCOL)";
            knownTags["DF1D"] = "IADOL";

            knownTags["DF1E"] = "Derivation key Index";
            knownTags["DF30"] = "Fuel Card usage bitmap [Prompting], ATC Limit";
            knownTags["DF31"] = "Encrypted PIN cryptography failure limit";
            knownTags["DF32"] = "Purchase Restrictions (WEX refers to this as Chip Offline Purchase Restriction), Failed MAC limit";
            knownTags["DF33"] = "Lifetime MAC Limit";
            knownTags["DF34"] = "Chip Offline Purchase Restrictions Amount for Fuel*, Session MAC Limit";
            knownTags["DF35"] = "Chip Offline Purchase Restrictions Amount for non-Fuel*";

            knownTags["DF36"] = "Relationship Codes*";
            knownTags["DF37"] = "3rd Party Reference Data Generation 2*";
            knownTags["DF38"] = "Loyalty ID*";
            knownTags["DF39"] = "Purchase Device Sequence Number (with the suffix)* ";
            knownTags["DF40"] = "DDOL Related Data Length";
            knownTags["DF41"] = "CCDOL2 Related Data Length";
            knownTags["DF4D"] = "Transaction Log Setting parameter31";

            dataTypes = new Dictionary<string, string>();
            dataTypes["82"] = "b";
            dataTypes["8E"] = "b";
            dataTypes["95"] = "b";
            dataTypes["9B"] = "b";
            dataTypes["9F07"] = "b";
            dataTypes["9F33"] = "b";
            dataTypes["9F40"] = "b";
            dataTypes["9F5B"] = "b";
        }


        public static EmvData ParseTagData(string tagData) {
            return ParseTagData(tagData, false);
        }
        public static EmvData ParseTagData(string tagData, bool verbose) {
            tagData = tagData.ToUpper();

            EmvData rvalue = new EmvData();

            for (int i = 0; i < tagData.Length;) {
                try {
                    string tagName = tagData.Substring(i, 2);
                    i = i + 2;
                    if ((Convert.ToInt32(tagName, 16) & 0x1F) == 0x1F) {
                        tagName += tagData.Substring(i, 2);
                        i = i + 2;
                    }

                    string lengthStr = tagData.Substring(i, 2);
                    i = i + 2;
                    int length = Convert.ToInt32(lengthStr, 16);
                    if (length > 127) {
                        int bytesLength = length - 128;
                        lengthStr = tagData.Substring(i, bytesLength * 2);
                        i = i + (bytesLength * 2);
                        length = Convert.ToInt32(lengthStr, 16);
                    }
                    length *= 2;

                    string value = tagData.Substring(i, length);
                    i = i + length;

                    if (!blackList.ContainsKey(tagName)) {
                        TlvData approvedTag = new TlvData(tagName, lengthStr, value, knownTags[tagName]);
                        if (tagName.Equals("5F28") && !value.Equals("840")) {
                            rvalue.SetStandInStatus(false, "Card is not domestically issued");
                        }
                        else if (tagName.Equals("95")) {
                            byte[] valueBuffer = StringUtils.BytesFromHex(value);
                            byte[] maskBuffer = StringUtils.BytesFromHex("FC50FC2000");

                            for (int idx = 0; idx < valueBuffer.Length; idx++) {
                                if ((valueBuffer[idx] & maskBuffer[idx]) != 0x00) {
                                    rvalue.SetStandInStatus(false, string.Format("Invalid TVR status in byte {0} of tag 95", idx + 1));
                                }
                            }
                        }
                        else if (tagName.Equals("9B")) {
                            byte[] valueBuffer = StringUtils.BytesFromHex(value);
                            byte[] maskBuffer = StringUtils.BytesFromHex("E800");

                            for (int idx = 0; idx < valueBuffer.Length; idx++) {
                                if ((valueBuffer[idx] & maskBuffer[idx]) != maskBuffer[idx]) {
                                    rvalue.SetStandInStatus(false, string.Format("Invalid TSI status in byte {0} of tag 9B", idx + 1));
                                }
                            }
                        }
                        rvalue.AddTag(approvedTag);
                    }
                    else {
                        rvalue.AddRemovedTag(tagName, lengthStr, value, blackList[tagName]);
                    }
                }
                catch (FormatException) { }
                catch (IndexOutOfRangeException) { }
            }
            if (verbose) {
                Console.WriteLine("Accepted Tags:");
                foreach (string tagName in rvalue.GetAcceptedTags().Keys) {
                    TlvData tag = rvalue.GetTag(tagName);
                    bool appendBinary = dataTypes.ContainsKey(tagName);
                    Console.WriteLine(string.Format("TAG: {0} - {1}", tagName, tag.GetDescription()));
                    Console.WriteLine(string.Format("{0}: {1}{2}\r\n", tag.GetLength(), tag.GetValue(), appendBinary ? string.Format(" [{0}]", tag.GetBinaryValue()) : ""));
                }
                Console.WriteLine("Removed Tags:");
                foreach (string tagName in rvalue.GetRemovedTags().Keys) {
                    TlvData tag = rvalue.GetRemovedTags()[tagName];
                    Console.WriteLine(string.Format("TAG: {0} - {1}", tagName, tag.GetDescription()));
                    Console.WriteLine(string.Format("{0}: {1}\r\n", tag.GetLength(), tag.GetValue()));
                }
            }
            return rvalue;
        }
    }
}
