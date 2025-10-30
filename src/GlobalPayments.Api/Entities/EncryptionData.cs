namespace GlobalPayments.Api.Entities
{
    /// <summary>
    /// Details how encrypted track data was encrypted by the device
    /// in order for the gateway to decrypt the data.
    /// </summary>
    public class EncryptionData
    {
        /// <summary>
        /// The encryption version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The track number that is encrypted and supplied in
        /// the request.
        /// </summary>
        public string TrackNumber { get; set; }

        /// <summary>
        /// The key serial number (KSN) used at the point of sale;
        /// where applicable.
        /// </summary>
        public string KSN { get; set; }

        ///<summary>
        /// DataFormat is an optional field to be used for encryption
        /// Version "05" to define the encryption encoding format.
        ///</summary>
        public string DataFormat { get; set; }

        /// <summary>
        /// The key transmission block (KTB) used at the point of sale;
        /// where applicable.
        /// </summary>
        public string KTB { get; set; }

        /// <summary>
        /// Convenience method for creating version `01` encryption data.
        /// </summary>
        public static EncryptionData Version1() {
            return new EncryptionData {
                Version = "01"
            };
        }

        /// <summary>
        /// Convenience method for creating version `02` encryption data.
        /// </summary>
        public static EncryptionData Version2(string ktb, string trackNumber = null) {
            return new EncryptionData {
                Version = "02",
                TrackNumber = trackNumber,
                KTB = ktb
            };
        }
    }
}
