using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Diamond.Responses;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Services {
    public class DeviceCloudService {
        private ConnectionConfig Config;

        public DeviceCloudService(ConnectionConfig config) {
            this.Config = config;
            ServicesContainer.ConfigureService(config);
        }

        public ITerminalResponse ParseResponse(string response) {
            if (string.IsNullOrEmpty(response)) {
                throw new ApiException("Enable to parse : empty response");
            }
            if (!JsonDoc.IsJson(response)) {
                throw new ApiException("Unexpected response format!");
            }

            switch (this.Config.ConnectionMode) {
                case ConnectionModes.DIAMOND_CLOUD:
                   return new  DiamondCloudResponse(response);                   
                default:
                    throw new UnsupportedTransactionException("The selected gateway does not support this response type!");
            }           
        }
    }
}
