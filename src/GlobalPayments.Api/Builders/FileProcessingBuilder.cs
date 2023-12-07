using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Builders {
    public class FileProcessingBuilder : BaseBuilder<FileProcessor> {        

        public string ResourceId { get; set; }

        public FileProcessingActionType FileProcessingActionType { get; set; }

        public FileProcessingBuilder(FileProcessingActionType actionType) {           
            FileProcessingActionType = actionType;
        }

        public override FileProcessor Execute(string configName = "default") {
            base.Execute(configName);
            var client = ServicesContainer.Instance.GetFileProcessingClient(configName);
            return client.ProcessFileUpload(this);
        }

        public FileProcessingBuilder WithResourceId(string resourceId) {
            ResourceId = resourceId;
            return this;
        }

        protected override void SetupValidations() {
            Validations.For(FileProcessingActionType.GET_DETAILS)
                .Check(() => ResourceId).IsNotNull();
        }
    }
}
