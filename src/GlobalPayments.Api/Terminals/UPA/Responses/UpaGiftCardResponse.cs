using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.UPA;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Terminals.UPA.Responses {
    /// <summary>
    /// Represents the response from the UPA device for card transaction operations.
    /// This response is returned from commands like StartCardTransaction and contains
    /// detailed card data, encryption information, and transaction metadata.
    /// </summary>
    public class UpaGiftCardResponse: DeviceResponse {

        const string INVALID_RESPONSE_FORMAT = "The response received is not in the proper format.";

        /// <summary>
        /// Gets or sets the method used to acquire the card data.
        /// Possible values: Contact, Contactless, Swipe, Manual, Scan, Insert, Tap
        /// </summary>
        public string AcquisitionType { get; set; }
        
        /// <summary>
        /// Gets or sets whether the Luhn check (mod-10 algorithm) passed for the card number.
        /// Values: "Y" (passed), "N" (failed)
        /// </summary>
        public string LuhnCheckPassed { get; set; }
        
        /// <summary>
        /// Gets or sets the type of encryption used for sensitive card data.
        /// Example: "3DES", "AES"
        /// </summary>
        public string DataEncryptionType { get; set; }
        
        /// <summary>
        /// Gets or sets the three-digit service code from the card's magnetic stripe.
        /// Defines the card's usage restrictions and security requirements.
        /// </summary>
        public string ServiceCode { get; set; }
        
        /// <summary>
        /// Gets or sets whether the transaction fell back from chip to magnetic stripe.
        /// 1 = fallback occurred, 0 = no fallback
        /// </summary>
        public int Fallback { get; set; }
        
        /// <summary>
        /// Gets or sets the Primary Account Number (PAN) details.
        /// Contains clear, masked, and encrypted versions of the card number.
        /// </summary>
        public PANDetails Pan { get; set; }
        
        /// <summary>
        /// Gets or sets the magnetic stripe track data.
        /// Contains clear and masked versions of Track 1, Track 2, and Track 3 data.
        /// </summary>
        public TrackData TrackData { get; set; }
        
        /// <summary>
        /// Gets or sets the EMV chip tags data as a string.
        /// Contains transaction-specific data from EMV chip cards.
        /// </summary>
        public string EmvTags { get; set; }
        
        /// <summary>
        /// Gets or sets the card expiration date.
        /// Format: MMYY
        /// </summary>
        public string ExpiryDate { get; set; }
        
        /// <summary>
        /// Gets or sets the Card Verification Value (CVV/CVC).
        /// Three or four digit security code from the card.
        /// </summary>
        public int Cvv { get; set; }
        
        /// <summary>
        /// Gets or sets the billing address entered during manual card entry.
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// Gets or sets the ZIP/postal code entered during manual card entry.
        /// </summary>
        public string ZipCode { get; set; }
        
        /// <summary>
        /// Gets or sets data captured from barcode/QR code scanning.
        /// </summary>
        public string ScannedData { get; set; }
        
        /// <summary>
        /// Gets or sets the PIN block encrypted using DUKPT (Derived Unique Key Per Transaction).
        /// Contains encrypted PIN and Key Serial Number (KSN).
        /// </summary>
        public PinDUKPTResponse PinDUKPT { get; set; }
        
        /// <summary>
        /// Gets or sets the 3DES DUKPT encrypted data.
        /// Contains encrypted blob and Key Serial Number (KSN) for secure data transmission.
        /// </summary>
        public ThreeDesDukpt ThreeDesDukpt { get; set; }
        
        /// <summary>
        /// Gets or sets the card BIN (Bank Identification Number) details.
        /// Contains information about card type, brand, supported features, and capabilities.
        /// </summary>
        public CardBinDetails CardBinDetails { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpaGiftCardResponse"/> class
        /// by parsing the JSON response from the UPA device.
        /// </summary>
        /// <param name="root">The JSON document containing the response data from the UPA device.</param>
        /// <exception cref="MessageException">Thrown when the response format is invalid or required fields are missing.</exception>
        public UpaGiftCardResponse(JsonDoc root) {
            string test = root.ToString();
            var firstDataNode = root.Get("data");

            if (firstDataNode == null) {
                throw new MessageException(INVALID_RESPONSE_FORMAT);
            }

            var cmdResult = firstDataNode.Get("cmdResult");

            if (cmdResult == null) {
                throw new MessageException(INVALID_RESPONSE_FORMAT);
            }

            Status = cmdResult.GetValue<string>("result");

            if (string.IsNullOrEmpty(Status)) {
                var errorCode = cmdResult.GetValue<string>("errorCode");
                var errorMsg = cmdResult.GetValue<string>("errorMessage");
                DeviceResponseText = $"Error: {errorCode} - {errorMsg}";
            }
            else {

                // If the Status is not "Success", there is either nothing to process, or something else went wrong.
                // Skip the processing of the rest of the message, as we'll likely hit null reference exceptions
                if (Status == "Success") {
                    var secondDataNode = firstDataNode.Get("data");
                    if (secondDataNode == null) {
                        throw new MessageException(INVALID_RESPONSE_FORMAT);
                    }
                    
                    // Parse basic fields
                    AcquisitionType = secondDataNode.GetValue<string>("acquisitionType");
                    LuhnCheckPassed = secondDataNode.GetValue<string>("LuhnCheckPassed");
                    DataEncryptionType = secondDataNode.GetValue<string>("dataEncryptionType");
                    ServiceCode = secondDataNode.GetValue<string>("serviceCode");
                    Fallback = secondDataNode.GetValue<int>("fallback");
                    
                    // Parse PAN details
                    var panNode = secondDataNode.Get("PAN");

                    if (panNode != null) {
                        Pan = new PANDetails {
                            ClearPAN = panNode.GetValue<string>("clearPAN"),
                            MaskedPAN = panNode.GetValue<string>("maskedPAN"),
                            EncryptedPAN = panNode.GetValue<string>("encryptedPAN")
                        };
                    }
                    
                    // Parse Track Data
                    var trackDataNode = secondDataNode.Get("trackData");

                    if (trackDataNode != null) {
                        TrackData = new TrackData {
                            ClearTrack1 = trackDataNode.GetValue<string>("clearTrack1"),
                            MaskedTrack1 = trackDataNode.GetValue<string>("maskedTrack1"),
                            ClearTrack2 = trackDataNode.GetValue<string>("clearTrack2"),
                            MaskedTrack2 = trackDataNode.GetValue<string>("maskedTrack2"),
                            ClearTrack3 = trackDataNode.GetValue<string>("clearTrack3"),
                            MaskedTrack3 = trackDataNode.GetValue<string>("maskedTrack3")
                        };
                    }
                    
                    EmvTags = secondDataNode.GetValue<string>("EmvTags");
                    ExpiryDate = secondDataNode.GetValue<string>("expiryDate");
                    Cvv = secondDataNode.GetValue<int>("Cvv");
                    Address = secondDataNode.GetValue<string>("address");
                    ZipCode = secondDataNode.GetValue<string>("zipCode");
                    ScannedData = secondDataNode.GetValue<string>("ScannedData");
                    
                    // Parse PinDUKPT
                    var pinDukptNode = secondDataNode.Get("PinDUKPT");

                    if (pinDukptNode != null) {
                        PinDUKPT = new PinDUKPTResponse {
                            PinBlock = pinDukptNode.GetValue<string>("PinBlock"),
                            Ksn = pinDukptNode.GetValue<string>("Ksn")
                        };
                    }
                    
                    // Parse 3DesDukpt
                    var threedesDukpt = secondDataNode.Get("3DesDukpt");

                    if (threedesDukpt != null) {
                        ThreeDesDukpt = new ThreeDesDukpt {
                            EncryptedBlob = threedesDukpt.GetValue<string>("encryptedBlob"),
                            Ksn = threedesDukpt.GetValue<string>("Ksn")
                        };
                    }
                    
                    // Parse CardBinDetails
                    var cardBinNode = secondDataNode.Get("CardBinDetails");

                    if (cardBinNode != null) {
                        CardBinDetails = new CardBinDetails {
                            CardType = cardBinNode.GetValue<string>("cardType"),
                            CardBrand = cardBinNode.GetValue<string>("cardBrand"),
                            CardBrandShortName = cardBinNode.GetValue<string>("cardBrandShortName"),
                            CardSecurityPromptFlag = cardBinNode.GetValue<int>("cardSecurityPromptFlag"),
                            AVSFlag = cardBinNode.GetValue<int>("AVSFlag"),
                            CashBackFlag = cardBinNode.GetValue<int>("cashBackFlag"),
                            SurchargeFlag = cardBinNode.GetValue<int>("surchargeFlag"),
                            EBTCardType = cardBinNode.GetValue<string>("EBTCardType"),
                            DCCEligible = cardBinNode.GetValue<int>("DCCEligible")
                        };
                    }
                    
                    DeviceResponseCode = "00";
                    DeviceResponseText = "Success";
                }
                else {
                    // the only other option is "Failed"
                    var errorCode = cmdResult.GetValue<string>("errorCode");
                    var errorMsg = cmdResult.GetValue<string>("errorMessage");
                    DeviceResponseText = $"Error: {errorCode} - {errorMsg}";
                }
            }

        }
    }
}
