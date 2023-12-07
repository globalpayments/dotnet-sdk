using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.GpApi;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace GlobalPayments.Api.Builders.RequestBuilder.GpApi {
    internal class GpApiFileProcessingRequestBuilder : IRequestBuilder<FileProcessingBuilder> {
        public Request BuildRequest(FileProcessingBuilder builder, GpApiConnector gateway) {           
            switch (builder.FileProcessingActionType) {
                case FileProcessingActionType.CREATE_UPLOAD_URL:
                    var data = new JsonDoc()
                            .Set("merchant_id", gateway.GpApiConfig.MerchantId)
                            .Set("account_id", gateway.GpApiConfig.AccessTokenInfo.FileProcessingAccountID);

                    var notifications = new JsonDoc()
                        .Set("status_url", gateway.GpApiConfig.StatusUrl);

                    if (notifications.HasKeys()) {
                        data.Set("notifications", notifications);
                    }

                    return new Request {
                        Verb = HttpMethod.Post,
                        Endpoint = $"{GpApiRequest.FILE_PROCESSING}",
                        RequestBody = data.ToString(),
                    };
                    
                case FileProcessingActionType.GET_DETAILS: 
                    return new Request {
                        Verb = HttpMethod.Get,
                        Endpoint = $"{GpApiRequest.FILE_PROCESSING}/{builder.ResourceId}"                        
                    };
                    
                default:
                    return null;
            }
        }
    }
}
