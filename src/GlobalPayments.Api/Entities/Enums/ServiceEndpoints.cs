namespace GlobalPayments.Api.Entities {
    public static class ServiceEndpoints {
        public const string GLOBAL_ECOM_PRODUCTION = "https://api.realexpayments.com/epage-remote.cgi";
        public const string GLOBAL_ECOM_TEST = "https://api.sandbox.realexpayments.com/epage-remote.cgi";
        public const string PORTICO_PRODUCTION = "https://api2.heartlandportico.com";
        public const string PORTICO_TEST = "https://cert.api2.heartlandportico.com";
        public const string THREE_DS_AUTH_PRODUCTION = "https://api.globalpay-ecommerce.com/3ds2/";
        public const string THREE_DS_AUTH_TEST = "https://api.sandbox.globalpay-ecommerce.com/3ds2/";
        public const string PAYROLL_PRODUCTION = "https://taapi.heartlandpayrollonlinetest.com/PosWebUI";
        public const string PAYROLL_TEST = "https://taapi.heartlandpayrollonlinetest.com/PosWebUI/Test/Test";
        public const string TABLE_SERVICE_PRODUCTION = "https://www.freshtxt.com/api31/";
        public const string TABLE_SERVICE_TEST = "https://www.freshtxt.com/api31/";
        public const string GENIUS_API_PRODUCTION = "";
        public const string GENIUS_API_TEST = "https://ps1.merchantware.net/Merchantware/ws/RetailTransaction/v45/Credit.asmx";
        public const string GENIUS_TERMINAL_PRODUCTION = "";
        public const string GENIUS_TERMINAL_TEST = "https://transport.merchantware.net/v4/transportService.asmx";
        public const string TRANSIT_MULTIPASS_PRODUCTION = "https://gateway.transit-pass.com/servlets/TransNox_API_Server";
        public const string TRANSIT_MULTIPASS_TEST = "https://stagegw.transnox.com/servlets/TransNox_API_Server";
        public const string GP_API_PRODUCTION = "https://apis.globalpay.com/ucp";
        public const string GP_API_TEST = "https://apis.sandbox.globalpay.com/ucp";
        public const string GP_API_QA = "https://apis-uat.globalpay.com/ucp";
        public const string PROPAY_TEST = "https://xmltest.propay.com/API/PropayAPI.aspx";
        public const string PROPAY_TEST_CANADIAN = "https://xmltestcanada.propay.com/API/PropayAPI.aspx";
        public const string PROPAY_PRODUCTION = "https://epay.propay.com/API/PropayAPI.aspx";
        public const string PROPAY_PRODUCTION_CANADIAN = "https://www.propaycanada.ca/API/PropayAPI.aspx";
        internal const string BILLPAY_TEST = "https://testing.heartlandpaymentservices.net/";
        public const string BILLPAY_CERTIFICATION = "https://staging.heartlandpaymentservices.net/";
        public const string BILLPAY_PRODUCTION = "https://heartlandpaymentservices.net/";
        public const string Transaction_API_PRODUCTION = "";
        public const string Transaction_API_TEST = "https://api.pit.paygateway.com/transactions";
        public const string OPEN_BANKING_TEST = "https://api.sandbox.globalpay-ecommerce.com/openbanking";
        public const string OPEN_BANKING_PRODUCTION = "https://api.globalpay-ecommerce.com/openbanking";
        public const string DIAMOND_CLOUD_TEST = "https://qr-cert.simpletabcloud.com/tomcat/command";
        public const string DIAMOND_CLOUD_PROD = "https://qr.simpletabcloud.com/tomcat/command";
        public const string DIAMOND_CLOUD_PROD_EU= "https://qreu.simpletabcloud.com/tomcat/command";
        public const string GENIUS_MITC_PRODUCTION = "https://api.paygateway.com/transactions";
        public const string GENIUS_MITC_TEST = "https://api.pit.paygateway.com/transactions";
    }
}
