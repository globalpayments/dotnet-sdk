using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Gateways.Interfaces
{
    public interface IFileProcessingService {
        FileProcessor ProcessFileUpload(FileProcessingBuilder builder);
    }
}
