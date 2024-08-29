using GlobalPayments.Api.Entities.Enums;

namespace GlobalPayments.Api.Entities.UPA {
    public class UDData {
        public UDFileType FileType { get; set; }
        public short SlotNum { get; set; }
        /// <summary>
        /// Filename of the file to be stored in the device. Must include the file extension.
        /// </summary>
        public string FileName { get; set; }
        public DisplayOption? DisplayOption { get; set; }
        public string FilePath { get; set; }
    }
}
