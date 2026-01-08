using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.Entities.Enums {
    /// <summary>
    /// Represents the method by which card data was acquired during a transaction.
    /// This enum supports bitwise operations to combine multiple acquisition types.
    /// </summary>
    [Flags]
    public enum AcquisitionType {
        /// <summary>
        /// No acquisition type specified.
        /// </summary>
        [Map(Target.UPA, "None")]
        None = 0,
        
        /// <summary>
        /// Card data acquired through contact (chip) reader.
        /// </summary>
        [Map(Target.UPA, "Contact")]
        Contact = 0x01,
        
        /// <summary>
        /// Card data acquired through contactless technology (NFC/RFID).
        /// </summary>
        [Map(Target.UPA, "Contactless")]
        Contactless = 0x02,
        
        /// <summary>
        /// Card data acquired by swiping the magnetic stripe.
        /// </summary>
        [Map(Target.UPA, "Swipe")]
        Swipe = 0x04,
        
        /// <summary>
        /// Card data entered manually by the user.
        /// </summary>
        [Map(Target.UPA, "Manual")]
        Manual = 0x08,
        
        /// <summary>
        /// Card data acquired by scanning (e.g., QR code or barcode).
        /// </summary>
        [Map(Target.UPA, "Scan")]
        Scan = 0x10,
        
        /// <summary>
        /// Card data acquired by inserting the chip card into the reader.
        /// </summary>
        [Map(Target.UPA, "Insert")]
        Insert = 0x20,
        
        /// <summary>
        /// Card data acquired by tapping the card or device on the reader.
        /// </summary>
        [Map(Target.UPA, "Tap")]
        Tap = 0x40
    }
}
