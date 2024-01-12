using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Terminals.Genius.ServiceConfigs
{
    public class MitcConfig
    {
        public string xWebId;
        public string TerminalId;
        public string AuthKey;
        public string ApiSecret;
        public string ApiKey;
        public string TargetDevice;


        /** Optional         * 
         * Version-4 UUID you generate to identify each request you send. 
         * Add a prefix of MER- to your ID. For example: 
         * MER-ba96b9c5-828c-434c-be74-d73c8e853526         * 
         * Note: If you don’t send a value for this parameter, we generate a value with a prefix of API- and return it in the header of the response.         
         */
        public string RequestId;

        /// <summary>
        /// The default environment
        /// </summary>
        public Entities.Environment Environment = Entities.Environment.PRODUCTION;

        /// <summary>
        /// *Required
        /// Name is given to integration by the integrators.
        /// Will default to '.NET-SDK' if none is provided.
        /// </summary>
        public string AppName = ".NET-SDK";

        /// <summary>
        /// Version number given to the integration by the integrators.
        /// </summary>
        public string AppVersion = "";

        /// <summary>
        /// * Required
        /// Currently supported regions:
        /// US - United States
        ///CA - Canada
        /// AU - Australia
        /// NZ - New Zealand
        /// </summary>
        public string Region = "US";

        /// <summary>
        /// *Optional
        ///'true' will allow card number entry on device
        /// </summary>
        public Boolean AllowKeyEntry = true;

        /// <summary>
        /// *Opional
        /// To enable the logging request and responses.
        /// </summary>
        private Boolean EnableLogging = false;

        public MitcConfig(string xWebId, string terminalId, string authKey, string apiSecret, string apiKey, string targetDevice, string requestId=null)
        {
            this.xWebId = xWebId;
            this.TerminalId = terminalId;
            this.AuthKey = authKey;
            this.ApiSecret = apiSecret;
            this.ApiKey = apiKey;
            this.TargetDevice = targetDevice;
            this.RequestId = requestId;
        }
    }
}
