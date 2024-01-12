using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Services {
    public class FileProcessingService {
        public FileProcessor Initiate() {
            return new FileProcessingBuilder(FileProcessingActionType.CREATE_UPLOAD_URL)
                .Execute();                
        }

        public FileProcessor GetDetails(string resourceId) {
            return new FileProcessingBuilder(FileProcessingActionType.GET_DETAILS)
                .WithResourceId(resourceId)
                .Execute();
        }
    }
}
