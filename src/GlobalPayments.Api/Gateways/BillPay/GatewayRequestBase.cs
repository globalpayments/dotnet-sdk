using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.Utils;
using System.Linq;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal abstract class GatewayRequestBase : XmlGateway {
        public const string publicEndpoint = "BillingDataManagement/v3/BillingDataManagementService.svc/BillingDataManagementService";

        public Credentials Credentials { get; set; }

        /// <summary>
        /// Creates a SOAP envelope with the necessary namespaces
        /// </summary>
        /// <param name="soapAction">The method name that is the target of the invocation</param>
        /// <returns>The Element that represents the envelope node</returns>
        protected Element CreateSOAPEnvelope(ElementTree et, string soapAction) {
            SetSOAPAction(soapAction);
            AddXMLNS(et);

            var envelope = et.Element("soapenv:Envelope");

            return envelope;
        }

        /// <summary>
        /// Creates and sets the SOAPAction header using the supplied method name
        /// </summary>
        /// <param name="soapAction">The method name that is the target of the invocation</param>
        private void SetSOAPAction(string soapAction) {
            if (Headers.Any(x => x.Key == "SOAPAction")) {
                Headers["SOAPAction"] = $@"https://test.heartlandpaymentservices.net/BillingDataManagement/v3/BillingDataManagementService/IBillingDataManagementService/{soapAction}";
            } else {
                Headers.Add("SOAPAction", $@"https://test.heartlandpaymentservices.net/BillingDataManagement/v3/BillingDataManagementService/IBillingDataManagementService/{soapAction}");
            }
        }

        /// <summary>
        /// Adds the XML Namespaces neccessary to make BillPay SOAP requests
        /// </summary>
        /// <param name="et">The element tree for the SOAP request</param>
        private void AddXMLNS(ElementTree et) {
            et.AddNamespace("soapenv", @"http://schemas.xmlsoap.org/soap/envelope/");
            et.AddNamespace("bil", @"https://test.heartlandpaymentservices.net/BillingDataManagement/v3/BillingDataManagementService");
            et.AddNamespace("bdms", @"http://schemas.datacontract.org/2004/07/BDMS.NewModel");
            et.AddNamespace("hps", @"http://schemas.datacontract.org/2004/07/HPS.BillerDirect.ACHCard.Wrapper");
            et.AddNamespace("pos", @"http://schemas.datacontract.org/2004/07/POSGateway.Wrapper");
            et.AddNamespace("bdm", @"https://test.heartlandpaymentservices.net/BillingDataManagement/v3/BDMServiceAdmin");
        }
    }
}
