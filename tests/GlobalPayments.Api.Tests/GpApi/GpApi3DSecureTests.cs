using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace GlobalPayments.Api.Tests.GpApi {
    [TestClass]
    public class GpApi3DSecureTests : BaseGpApiTests {
        #region Constants
        private const string AVAILABLE = "AVAILABLE";
        private const string AUTHENTICATION_COULD_NOT_BE_PERFORMED = "AUTHENTICATION_COULD_NOT_BE_PERFORMED";
        private const string AUTHENTICATION_FAILED = "AUTHENTICATION_FAILED";
        private const string AUTHENTICATION_SUCCESSFUL = "AUTHENTICATION_SUCCESSFUL";
        private const string CHALLENGE_REQUIRED = "CHALLENGE_REQUIRED";
        private const string ENROLLED = "ENROLLED";
        private const string NOT_ENROLLED = "NOT_ENROLLED";
        private const string SUCCESS_AUTHENTICATED = "SUCCESS_AUTHENTICATED";
        #endregion

        private CreditCardData card;
        private RecurringPaymentMethod stored;
        private Address shippingAddress;
        private Address billingAddress;
        private BrowserData browserData;

        public GpApi3DSecureTests() {
            ServicesContainer.ConfigureService(new GpApiConfig {
                AppId = "P3LRVjtGRGxWQQJDE345mSkEh2KfdAyg",
                AppKey = "ockJr6pv6KFoGiZA",
                Country = "GB",
                ChallengeNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                MethodNotificationUrl = "https://ensi808o85za.x.pipedream.net/",
                //WebProxy = new CustomWebProxy("http://localhost:8866"),
            });

            // Create card data
            card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = 2025,
                CardHolderName = "John Smith"
            };

            // Stored card
            stored = new RecurringPaymentMethod(
                "20190809-Realex",
                "20190809-Realex-Credit"
            );

            // Shipping address
            shippingAddress = new Address {
                StreetAddress1 = "Apartment 852",
                StreetAddress2 = "Complex 741",
                StreetAddress3 = "no",
                City = "Chicago",
                PostalCode = "5001",
                State = "IL",
                CountryCode = "840"
            };

            // Billing address
            billingAddress = new Address {
                StreetAddress1 = "Flat 456",
                StreetAddress2 = "House 789",
                StreetAddress3 = "no",
                City = "Halifax",
                PostalCode = "W5 9HR",
                CountryCode = "826"
            };

            // Browser data
            browserData = new BrowserData {
                AcceptHeader = "text/html,application/xhtml+xml,application/xml;q=9,image/webp,img/apng,*/*;q=0.8",
                ColorDepth = ColorDepth.TWENTY_FOUR_BITS,
                IpAddress = "123.123.123.123",
                JavaEnabled = true,
                Language = "en",
                ScreenHeight = 1080,
                ScreenWidth = 1920,
                ChallengeWindowSize = ChallengeWindowSize.WINDOWED_600X400,
                Timezone = "0",
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64, x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.110 Safari/537.36"
            };
        }

        [TestMethod]
        public void FullCycle_v1() {
            card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = 2025,
                CardHolderName = "John Smith"
            };

            // Check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency("GBP")
                .WithAmount(10.01m)
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithChallengeRequestIndicator(ChallengeRequestIndicator.CHALLENGE_MANDATED)
                .WithTransactionInitiator(StoredCredentialInitiator.CardHolder)
                .Execute();
            
            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.One, secureEcom.Version);
            Assert.AreEqual(CHALLENGE_REQUIRED, secureEcom.Status);
            Assert.IsTrue(secureEcom.ChallengeMandated);
            Assert.IsNotNull(secureEcom.IssuerAcsUrl);
            Assert.IsNotNull(secureEcom.PayerAuthenticationRequest);
            Assert.IsNotNull(secureEcom.ChallengeReturnUrl);
            Assert.IsNotNull(secureEcom.MessageType);
            Assert.IsNotNull(secureEcom.SessionDataFieldName);

            // Perform ACS authetication
            GpApi3DSecureAcsClient acsClient = new GpApi3DSecureAcsClient(secureEcom.IssuerAcsUrl);
            string payerAuthenticationResponse;
            string authResponse = acsClient.Authenticate_v1(secureEcom, out payerAuthenticationResponse);
            Assert.AreEqual("{\"success\":true}", authResponse);

            // Get authentication data
            secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(secureEcom.ServerTransactionId)
                .WithPayerAuthenticationResponse(payerAuthenticationResponse)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(AUTHENTICATION_SUCCESSFUL, secureEcom.Status);

            card.ThreeDSecure = secureEcom;

            // Create transaction
            Transaction response = card.Charge(10.01m)
                .WithCurrency("GBP")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void FullCycle_v1_WithTokenizedPaymentMethod() {
            card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = 2025,
                CardHolderName = "John Smith"
            };

            // Tokenize payment method
            var tokenizedCard = new CreditCardData() {
                Token = card.Tokenize()
            };

            Assert.IsNotNull(tokenizedCard.Token);

            // Check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(tokenizedCard)
                .WithCurrency("GBP")
                .WithAmount(10.01m)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.One, secureEcom.Version);
            Assert.AreEqual(CHALLENGE_REQUIRED, secureEcom.Status);
            Assert.IsTrue(secureEcom.ChallengeMandated);
            Assert.IsNotNull(secureEcom.IssuerAcsUrl);
            Assert.IsNotNull(secureEcom.PayerAuthenticationRequest);
            Assert.IsNotNull(secureEcom.ChallengeReturnUrl);
            Assert.IsNotNull(secureEcom.MessageType);
            Assert.IsNotNull(secureEcom.SessionDataFieldName);

            // Perform ACS authetication
            GpApi3DSecureAcsClient acsClient = new GpApi3DSecureAcsClient(secureEcom.IssuerAcsUrl);
            string payerAuthenticationResponse;
            string authResponse = acsClient.Authenticate_v1(secureEcom, out payerAuthenticationResponse);
            Assert.AreEqual("{\"success\":true}", authResponse);

            // Get authentication data
            secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(secureEcom.ServerTransactionId)
                .WithPayerAuthenticationResponse(payerAuthenticationResponse)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(AUTHENTICATION_SUCCESSFUL, secureEcom.Status);

            tokenizedCard.ThreeDSecure = secureEcom;
            
            // Create transaction
            Transaction response = tokenizedCard.Charge(10.01m)
                .WithCurrency("GBP")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_AutheticationUnavailable_v1() {
            card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = 2025,
                CardHolderName = "John Smith"
            };

            // Check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency("GBP")
                .WithAmount(10.01m)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.One, secureEcom.Version);
            Assert.AreEqual(CHALLENGE_REQUIRED, secureEcom.Status);
            Assert.IsTrue(secureEcom.ChallengeMandated);
            Assert.IsNotNull(secureEcom.IssuerAcsUrl);
            Assert.IsNotNull(secureEcom.PayerAuthenticationRequest);
            Assert.IsNotNull(secureEcom.ChallengeReturnUrl);
            Assert.IsNotNull(secureEcom.MessageType);
            Assert.IsNotNull(secureEcom.SessionDataFieldName);

            // Perform ACS authetication
            GpApi3DSecureAcsClient acsClient = new GpApi3DSecureAcsClient(secureEcom.IssuerAcsUrl);
            string payerAuthenticationResponse;
            string authResponse = acsClient.Authenticate_v1(secureEcom, out payerAuthenticationResponse, AuthenticationResultCode.Unavailable);
            Assert.AreEqual("{\"success\":true}", authResponse);

            // Get authentication data
            secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(secureEcom.ServerTransactionId)
                .WithPayerAuthenticationResponse(payerAuthenticationResponse)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(AUTHENTICATION_COULD_NOT_BE_PERFORMED, secureEcom.Status);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_AuthenticationAttemptAcknowledge_v1() {
            card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = 2025,
                CardHolderName = "John Smith"
            };

            // Check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency("GBP")
                .WithAmount(10.01m)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.One, secureEcom.Version);
            Assert.AreEqual(CHALLENGE_REQUIRED, secureEcom.Status);
            Assert.IsTrue(secureEcom.ChallengeMandated);
            Assert.IsNotNull(secureEcom.IssuerAcsUrl);
            Assert.IsNotNull(secureEcom.PayerAuthenticationRequest);
            Assert.IsNotNull(secureEcom.ChallengeReturnUrl);
            Assert.IsNotNull(secureEcom.MessageType);
            Assert.IsNotNull(secureEcom.SessionDataFieldName);

            // Perform ACS authetication
            GpApi3DSecureAcsClient acsClient = new GpApi3DSecureAcsClient(secureEcom.IssuerAcsUrl);
            string payerAuthenticationResponse;
            string authResponse = acsClient.Authenticate_v1(secureEcom, out payerAuthenticationResponse, AuthenticationResultCode.AttemptAcknowledge);
            Assert.AreEqual("{\"success\":true}", authResponse);

            // Get authentication data
            secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(secureEcom.ServerTransactionId)
                .WithPayerAuthenticationResponse(payerAuthenticationResponse)
                .Execute();

            Assert.IsNotNull(secureEcom);
            //ToDo: Status should have a value
            Assert.AreEqual(string.Empty, secureEcom.Status);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_AuthenticationFailed_v1() {
            card = new CreditCardData {
                Number = "4012001037141112",
                ExpMonth = 12,
                ExpYear = 2025,
                CardHolderName = "John Smith"
            };

            // Check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency("GBP")
                .WithAmount(10.01m)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.One, secureEcom.Version);
            Assert.AreEqual(CHALLENGE_REQUIRED, secureEcom.Status);
            Assert.IsTrue(secureEcom.ChallengeMandated);
            Assert.IsNotNull(secureEcom.IssuerAcsUrl);
            Assert.IsNotNull(secureEcom.PayerAuthenticationRequest);
            Assert.IsNotNull(secureEcom.ChallengeReturnUrl);
            Assert.IsNotNull(secureEcom.MessageType);
            Assert.IsNotNull(secureEcom.SessionDataFieldName);

            // Perform ACS authetication
            GpApi3DSecureAcsClient acsClient = new GpApi3DSecureAcsClient(secureEcom.IssuerAcsUrl);
            string payerAuthenticationResponse = string.Empty;
            string authResponse = acsClient.Authenticate_v1(secureEcom, out payerAuthenticationResponse, AuthenticationResultCode.Failed);
            Assert.AreEqual("{\"success\":true}", authResponse);

            // Get authentication data
            secureEcom = Secure3dService.GetAuthenticationData()
                .WithPayerAuthenticationResponse(payerAuthenticationResponse)
                .WithServerTransactionId(secureEcom.ServerTransactionId)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(AUTHENTICATION_FAILED, secureEcom.Status);
        }

        [TestMethod]
        public void CardHolderNotEnrolled_v1() {
            card = new CreditCardData {
                Number = "4917000000000087",
                ExpMonth = 12,
                ExpYear = 2025,
                CardHolderName = "John Smith"
            };

            // Check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency("GBP")
                .WithAmount(10.01m)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(Secure3dVersion.One, secureEcom.Version);
            Assert.AreEqual(NOT_ENROLLED, secureEcom.Enrolled);
            Assert.AreEqual(NOT_ENROLLED, secureEcom.Status);
        }

        [TestMethod]
        public void FullCycle_v2() {
            // Frictionless scenario
            card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = 2025,
                CardHolderName = "John Doe"
            };

            // Check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency("GBP")
                .WithAmount(10.01m)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);
            Assert.AreEqual(AVAILABLE, secureEcom.Status);

            // Initiate authentication
            ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(10.01m)
                .WithCurrency("GBP")
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            Assert.IsNotNull(initAuth);
            Assert.AreEqual(SUCCESS_AUTHENTICATED, initAuth.Status);

            // Get authentication data
            secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(initAuth.ServerTransactionId)
                .Execute();

            Assert.AreEqual(SUCCESS_AUTHENTICATED, secureEcom.Status);

            card.ThreeDSecure = secureEcom;

            // Create transaction
            Transaction response = card.Charge(10.01m)
                .WithCurrency("GBP")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void FullCycle_v2_WithTokenizedPaymentMethod() {
            // Frictionless scenario
            card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = 12,
                ExpYear = 2025,
                CardHolderName = "John Doe"
            };

            // Tokenize payment method
            var tokenizedCard = new CreditCardData() {
                Token = card.Tokenize()
            };

            Assert.IsNotNull(tokenizedCard.Token);

            // Check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency("GBP")
                .WithAmount(10.01m)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);
            Assert.AreEqual(AVAILABLE, secureEcom.Status);

            // Initiate authentication
            ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(tokenizedCard, secureEcom)
                .WithAmount(10.01m)
                .WithCurrency("GBP")
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            Assert.IsNotNull(initAuth);
            Assert.AreEqual(SUCCESS_AUTHENTICATED, initAuth.Status);

            // Get authentication data
            secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(initAuth.ServerTransactionId)
                .Execute();

            Assert.AreEqual(SUCCESS_AUTHENTICATED, secureEcom.Status);

            tokenizedCard.ThreeDSecure = secureEcom;

            // Create transaction
            Transaction response = tokenizedCard.Charge(10.01m)
                .WithCurrency("GBP")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }

        [TestMethod]
        public void CardHolderEnrolled_ChallengeRequired_v2() {
            // Challenge required scenario
            card = new CreditCardData {
                Number = "4012001038488884",
                ExpMonth = 12,
                ExpYear = 2025,
                CardHolderName = "John Smith"
            };

            // Check enrollment
            ThreeDSecure secureEcom = Secure3dService.CheckEnrollment(card)
                .WithCurrency("GBP")
                .WithAmount(10.01m)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(ENROLLED, secureEcom.Enrolled, "Card not enrolled");
            Assert.AreEqual(Secure3dVersion.Two, secureEcom.Version);
            Assert.AreEqual(AVAILABLE, secureEcom.Status);

            // Initiate authentication
            ThreeDSecure initAuth = Secure3dService.InitiateAuthentication(card, secureEcom)
                .WithAmount(10.01m)
                .WithCurrency("GBP")
                .WithAuthenticationSource(AuthenticationSource.BROWSER)
                .WithMethodUrlCompletion(MethodUrlCompletion.YES)
                .WithOrderCreateDate(DateTime.Now)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithBrowserData(browserData)
                .Execute();

            Assert.IsNotNull(initAuth);
            Assert.AreEqual(CHALLENGE_REQUIRED, initAuth.Status);
            Assert.IsTrue(initAuth.ChallengeMandated);
            Assert.IsNotNull(initAuth.IssuerAcsUrl);
            Assert.IsNotNull(initAuth.PayerAuthenticationRequest);

            // Perform ACS authetication
            GpApi3DSecureAcsClient acsClient = new GpApi3DSecureAcsClient(initAuth.IssuerAcsUrl);
            string authResponse = acsClient.Authenticate_v2(initAuth);
            Assert.AreEqual("{\"success\":true}", authResponse);

            // Get authentication data
            secureEcom = Secure3dService.GetAuthenticationData()
                .WithServerTransactionId(initAuth.ServerTransactionId)
                .Execute();

            Assert.IsNotNull(secureEcom);
            Assert.AreEqual(SUCCESS_AUTHENTICATED, secureEcom.Status);

            card.ThreeDSecure = secureEcom;

            // Create transaction
            Transaction response = card.Charge(10.01m)
                .WithCurrency("GBP")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual(SUCCESS, response?.ResponseCode);
            Assert.AreEqual(GetMapping(TransactionStatus.Captured), response?.ResponseMessage);
        }
    }

    /// <summary>
    /// ACS Authentication Simulator result codes
    /// </summary>
    public enum AuthenticationResultCode : int {
        Successful = 0,
        Unavailable = 5,
        AttemptAcknowledge = 7,
        Failed = 9,
    }

    /// <summary>
    /// This 3DS ACS client mocks the ACS Authentication Simulator used for testing purposes
    /// </summary>
    public class GpApi3DSecureAcsClient {
        private string _redirectUrl;

        public GpApi3DSecureAcsClient(string redirectUrl) {
            _redirectUrl = redirectUrl;
        }

        private string SubmitFormData(string formUrl, List<KeyValuePair<string, string>> formData) {
            HttpClient httpClient = new HttpClient() {
                Timeout = TimeSpan.FromMilliseconds(60000)
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, formUrl);
            HttpResponseMessage response = null;
            try {
                request.Content = new FormUrlEncodedContent(formData);
                response = httpClient.SendAsync(request).Result;
                var rawResponse = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode != HttpStatusCode.OK) {
                    throw new Exception(rawResponse);
                }

                return rawResponse;
            }
            catch (Exception) {
                throw;
            }
            finally { }
        }

        private string GetFormAction(string rawHtml, string formName) {
            var searchString = $"name=\"{formName}\" action=\"";

            var index = rawHtml.IndexOf(searchString, comparisonType: StringComparison.InvariantCultureIgnoreCase);
            if (index > -1) {
                index = index + searchString.Length;
                var length = rawHtml.IndexOf("\"", index) - index;
                return rawHtml.Substring(index, length);
            }
            return null;
        }

        private string GetInputValue(string rawHtml, string inputName) {
            var searchString = $"name=\"{inputName}\" value=\"";

            var index = rawHtml.IndexOf(searchString, comparisonType: StringComparison.InvariantCultureIgnoreCase);
            if (index > -1) {
                index = index + searchString.Length;
                var length = rawHtml.IndexOf("\"", index) - index;
                return rawHtml.Substring(index, length);
            }
            return null;
        }

        /// <summary>
        /// Performs ACS authentication for 3DS v1
        /// </summary>
        /// <param name="secureEcom"></param>
        /// <param name="paRes"></param>
        /// <param name="authenticationResultCode"></param>
        /// <returns></returns>
        public string Authenticate_v1(ThreeDSecure secureEcom, out string paRes, AuthenticationResultCode authenticationResultCode = AuthenticationResultCode.Successful) {
            // Step 1
            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>(secureEcom.MessageType, secureEcom.PayerAuthenticationRequest));
            formData.Add(new KeyValuePair<string, string>(secureEcom.SessionDataFieldName, secureEcom.ServerTransactionId));
            formData.Add(new KeyValuePair<string, string>("TermUrl", secureEcom.ChallengeReturnUrl));
            formData.Add(new KeyValuePair<string, string>("AuthenticationResultCode", authenticationResultCode.ToString("D")));
            string rawResponse = SubmitFormData(secureEcom.IssuerAcsUrl, formData);

            paRes = GetInputValue(rawResponse, "PaRes");

            // Step 2
            formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("MD", GetInputValue(rawResponse, "MD")));
            formData.Add(new KeyValuePair<string, string>("PaRes", paRes));
            rawResponse = SubmitFormData(GetFormAction(rawResponse, "PAResForm"), formData);

            return rawResponse;
        }

        /// <summary>
        /// Performs ACS authentication for 3DS v2
        /// </summary>
        /// <param name="secureEcom"></param>
        /// <returns></returns>
        public string Authenticate_v2(ThreeDSecure secureEcom) {
            // Step 1
            var formData = new List<KeyValuePair<string, string>>();
            //ToDo: use secureEcom.MessageType instead of "creq"
            formData.Add(new KeyValuePair<string, string>("creq", secureEcom.PayerAuthenticationRequest));
            //ToDo: use secureEcom.SessionDataFieldName instead of "threeDSSessionData"
            formData.Add(new KeyValuePair<string, string>("threeDSSessionData", secureEcom.ServerTransactionId));
            string rawResponse = SubmitFormData(secureEcom.IssuerAcsUrl, formData);

            // Step 2
            formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("get-status-type", "true"));
            do {
                rawResponse = SubmitFormData(secureEcom.IssuerAcsUrl, formData);
                System.Threading.Thread.Sleep(5000);
            } while (rawResponse.Trim() == "IN_PROGRESS");

            // Step 3
            formData = new List<KeyValuePair<string, string>>();
            rawResponse = SubmitFormData(secureEcom.IssuerAcsUrl, formData);

            // Step 4
            formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("cres", GetInputValue(rawResponse, "cres")));
            rawResponse = SubmitFormData(GetFormAction(rawResponse, "ResForm"), formData);

            return rawResponse;
        }
    }
}
