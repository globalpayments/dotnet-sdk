using GlobalPayments.Api.Entities.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class Document
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string TimeCreated { get; set; }
        public FileType Format { get; set; }
        public DocumentCategory Category { get; set; }
    }
}
