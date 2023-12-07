using System.Collections.Generic;

namespace GlobalPayments.Api.Entities {
    public class FileProcessor {
        public string ResourceId { get; set; }
        public string UploadUrl { get; set; }
        public string ExpirationDate { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string TotalRecordCount { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public List<FileUploaded> FilesUploaded { get; set; }
    }
}
