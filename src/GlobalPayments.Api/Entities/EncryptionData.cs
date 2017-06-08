namespace GlobalPayments.Api.Entities
{
    public class EncryptionData
    {
        public string Version { get; set; }
        public string TrackNumber { get; set; }
        public string KSN { get; set; }
        public string KTB { get; set; }

        public static EncryptionData Version1() {
            return new EncryptionData {
                Version = "01"
            };
        }

        public static EncryptionData Version2(string ktb, string trackNumber = null) {
            return new EncryptionData {
                Version = "02",
                TrackNumber = trackNumber,
                KTB = ktb
            };
        }
    }
}
