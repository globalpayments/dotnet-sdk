using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class FileUploaded
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string TimeCreated { get; set; }
        public string Url { get; set; }
        public string ExpirationDate { get; set; }
    }
}
