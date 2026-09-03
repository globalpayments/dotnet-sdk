using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitOfMeasure = GlobalPayments.Api.Network.Entities.UnitOfMeasure;

namespace GlobalPayments.Api.Tests.Network.Certification {
    [TestClass]
    public class NWSFleetTokenizationCertificationTest {
        private CreditCardData card;
        private FleetData fleetData;
        AcceptorConfig acceptorConfig = new AcceptorConfig();

        public NWSFleetTokenizationCertificationTest() {
            Address address = new Address();
            address.Name = "My STORE";
            address.StreetAddress1 = "1 MY STREET";
            address.City = "MYTOWN";
            address.PostalCode = "90210";
            address.State = "KY";
            address.Country = "USA";

            acceptorConfig.Address = address;

            // data code values
            acceptorConfig.CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry;
            acceptorConfig.CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.PIN;
            acceptorConfig.TerminalOutputCapability = TerminalOutputCapability.Printing_Display;

            // hardware software config values
            acceptorConfig.HardwareLevel = "34";
            acceptorConfig.SoftwareLevel = "21205710";

            // pos configuration values
            acceptorConfig.SupportsPartialApproval = true;
            acceptorConfig.SupportsShutOffAmount = true;
            acceptorConfig.SupportsReturnBalance = true;
            acceptorConfig.SupportsDiscoverNetworkReferenceId = true;
            acceptorConfig.SupportsAvsCnvVoidReferrals = true;
            acceptorConfig.SupportsEmvPin = true;
            acceptorConfig.EchoSettlementData = true;
            acceptorConfig.IncrementalAuthIndicator = false;
            //DE 127
            acceptorConfig.ServiceType = ServiceType.GPN_API;
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.DeTokenize;
            acceptorConfig.TokenizationType = TokenizationType.MerchantTokenization;
            acceptorConfig.MerchantId = "650000011573667";

            // gateway config
            NetworkGatewayConfig config = new NetworkGatewayConfig(Api.Network.Enums.NetworkGatewayType.NWS);
            config.ServiceUrl = "test.txns-c.secureexchange.net";
            config.PrimaryPort = 15031;
            config.SecondaryEndpoint = "test.txns-e.secureexchange.net";
            config.SecondaryPort = 15031;
            config.CompanyId = "SPSA";
            config.TerminalId = "NWSDOTNET01";
            config.UniqueDeviceId = "0001";
            config.AcceptorConfig = acceptorConfig;
            config.EnableLogging = true;
            config.StanProvider = StanGenerator.GetInstance();
            config.BatchProvider = BatchProvider.GetInstance();

            ServicesContainer.ConfigureService(config);

            fleetData = new FleetData {
                DriverId = "11411",
                VehicleNumber = "22031",
                OdometerReading = "1256",
                PurchaseDeviceSequenceNumber = "12345"
            };
        }

        #region Visa Fleet
        /// <summary>
        /// Single Use Token Generation
        /// </summary>
        [TestMethod]
        public void Test_File_Action_Visa_SingleUseToken() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleUseToken;
            card = TestCards.VisaFleetManual();
            card.TokenizationData = "4485530000000127";
            card.Cvn = "123";
            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        /// <summary>
        /// Single use to multi use token swaping
        /// </summary>
        [TestMethod]
        public void Test_CombinedFile_Action_Visa_SingleToMultiUseToken() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleToMultiUseToken;
            card = TestCards.VisaFleetManual();
            card.TokenizationData = "8D3596F4FE4F9E3D18C55AC2BB6E56EE";
            card.Cvn = "123";
            Transaction response = card.FileAction()
                .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_SingleToMultiUseToken_Negative_InvalidToken() {
            // Attempt to swap an invalid single use token for a multi use token
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleToMultiUseToken;

            card = TestCards.VisaFleetManual();
            card.TokenizationData = "INVALID_SINGLE_USE_TOKEN";
            card.Cvn = "123";

            Transaction response = card.FileAction().Execute();

            Assert.IsNotNull(response);
            Assert.AreNotEqual("000", response.ResponseCode, "Expected failure when using invalid single use token");
        }

        //Visa Fleet
        [TestMethod]
        public void Test_File_Action_Visa() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.Tokenize;
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.VisaFleetManual();
            card.TokenizationData = "4485530000000127";
            Transaction response = card.FileAction()
                .WithFleetData(fleetData)
                .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_001_Credit_Manual_Auth_Visa() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);

            card = TestCards.VisaFleetManual();
            card.TokenizationData = "8D3596F4FE4F9E3D18C55AC2BB6E56EE";
            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }
        [TestMethod]
        public void Test_002_Credit_Manual_Sale_Visa() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.VisaFleetManual();
            card.TokenizationData = "8D3596F4FE4F9E3D18C55AC2BB6E56EE";
            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //force draft capture
        [TestMethod]
        public void Test_016_AuthCapture_Visa() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.VisaFleetManual();
            card.TokenizationData = "8D3596F4FE4F9E3D18C55AC2BB6E56EE";
            Transaction response = card.Authorize(10m, true)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(response.AuthorizedAmount)
                    .WithCurrency("USD")
                    .WithFleetData(fleetData)
                    .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_004_Credit_Refund_Visa() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.VisaFleetManual();
            card.TokenizationData = "8D3596F4FE4F9E3D18C55AC2BB6E56EE";
            Transaction response = card.Refund(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_Credit_Balance_Inquiry_Visa() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.VisaFleetManual();
            card.TokenizationData = "8D3596F4FE4F9E3D18C55AC2BB6E56EE";
            Transaction response = card.BalanceInquiry()
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Sale_Reversal_Visa() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.VisaFleetManual();
            card.TokenizationData = "8D3596F4FE4F9E3D18C55AC2BB6E56EE";

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithFleetData(fleetData)
                    .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(response);

            response.NTSData = ntsData;
            Assert.AreEqual("000", response.ResponseCode);

            Transaction reversal = response.Reverse(10m)
                    .WithCurrency("USD")
                    .WithFleetData(fleetData)
                    .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("400", reversal.ResponseCode);
        }

        [TestMethod]
        public void Test_015_Credit_Void_Visa() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.VisaFleetManual();
            card.TokenizationData = "8D3596F4FE4F9E3D18C55AC2BB6E56EE";

            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            // reverse the transaction
            Transaction reverseResponse = response.Void()
                        .WithReferenceNumber("12345")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }
        #endregion

        #region MasterCard Fleet
        /// <summary>
        /// Single Use Token Generation
        /// </summary>
        [TestMethod]
        public void Test_File_Action_SingleUseToken_MCFleet() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleUseToken;
            card = TestCards.MasterCardFleetManual();
            card.TokenizationData = "5567300000000016";
            card.Cvn = "123";
            Transaction response = card.FileAction()
                .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        /// <summary>
        /// Single use to multi use token swaping
        /// </summary>
        [TestMethod]
        public void Test_CombinedFile_Action_SingleToMultiUseToken_MCFleet() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleToMultiUseToken;
            card = TestCards.MasterCardFleetManual();
            card.TokenizationData = "338DC01CC3B65DDAFB83A4F021A10267";
            card.Cvn = "123";
            Transaction response = card.FileAction()
                .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_SingleToMultiUseToken_Negative_InvalidToken_MCFleet() {
            // Attempt to swap an invalid single use token for a multi use token
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleToMultiUseToken;

            card = TestCards.MasterCardFleetManual();
            card.TokenizationData = "INVALID_SINGLE_USE_TOKEN";
            card.Cvn = "123";

            Transaction response = card.FileAction().Execute();

            Assert.IsNotNull(response);
            Assert.AreNotEqual("000", response.ResponseCode, "Expected failure when using invalid single use token");
        }

        //Visa Fleet
        [TestMethod]
        public void Test_File_Action_MCFleet() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.Tokenize;
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.MasterCardFleetManual();
            card.TokenizationData = "5567300000000016";
            Transaction response = card.FileAction()
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_001_Credit_Manual_Auth_MCFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);

            card = TestCards.MasterCardFleetManual();
            card.TokenizationData = "338DC01CC3B65DDAFB83A4F021A10267";
            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Credit_Manual_Sale_MCFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.MasterCardFleetManual();
            card.TokenizationData = "338DC01CC3B65DDAFB83A4F021A10267";
            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //force draft capture
        [TestMethod]
        public void Test_016_AuthCapture_MCFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.MasterCardFleetManual();
            card.TokenizationData = "338DC01CC3B65DDAFB83A4F021A10267";
            Transaction response = card.Authorize(10m, true)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(response.AuthorizedAmount)
                    .WithCurrency("USD")
                    .WithFleetData(fleetData)
                    .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_004_Credit_Refund_MCFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.MasterCardFleetManual();
            card.TokenizationData = "338DC01CC3B65DDAFB83A4F021A10267";
            Transaction response = card.Refund(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_Credit_Balance_Inquiry_MCFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.MasterCardFleetManual();
            card.TokenizationData = "338DC01CC3B65DDAFB83A4F021A10267";
            Transaction response = card.BalanceInquiry()
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Sale_Reversal_MCFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.MasterCardFleetManual();
            card.TokenizationData = "338DC01CC3B65DDAFB83A4F021A10267";

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithFleetData(fleetData)
                    .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(response);

            response.NTSData = ntsData;
            Assert.AreEqual("000", response.ResponseCode);

            Transaction reversal = response.Reverse(10m)
                    .WithCurrency("USD")
                    .WithFleetData(fleetData)
                    .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("400", reversal.ResponseCode);
        }

        [TestMethod]
        public void Test_015_Credit_Void_MCFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.MasterCardFleetManual();
            card.TokenizationData = "338DC01CC3B65DDAFB83A4F021A10267";

            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            // reverse the transaction
            Transaction reverseResponse = response.Void()
                        .WithReferenceNumber("12345")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }
        #endregion

        #region Voyager Fleet
        /// <summary>
        /// Single Use Token Generation
        /// </summary>
        [TestMethod]
        public void Test_File_Action_SingleUseToken_VoyagerFleet() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleUseToken;
            card = TestCards.VoyagerFleetManual();
            card.TokenizationData = "7088869008250005064";
            card.Cvn = "123";
            Transaction response = card.FileAction()
                .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        /// <summary>
        /// Single use to multi use token swaping
        /// </summary>
        [TestMethod]
        public void Test_CombinedFile_Action_SingleToMultiUseToken_VoyagerFleet() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleToMultiUseToken;
            card = TestCards.VoyagerFleetManual();
            card.TokenizationData = "C07341CB7DFFC17C5FA7A31C08938BDA";
            card.Cvn = "123";
            Transaction response = card.FileAction()
                .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_SingleToMultiUseToken_Negative_InvalidToken_VoyagerFleet() {
            // Attempt to swap an invalid single use token for a multi use token
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleToMultiUseToken;

            card = TestCards.VoyagerFleetManual();
            card.TokenizationData = "INVALID_SINGLE_USE_TOKEN";
            card.Cvn = "123";

            Transaction response = card.FileAction().Execute();

            Assert.IsNotNull(response);
            Assert.AreNotEqual("000", response.ResponseCode, "Expected failure when using invalid single use token");
        }

        [TestMethod]
        public void Test_File_Action_VoyagerFleet() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.Tokenize;
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.VoyagerFleetManual();
            card.TokenizationData = "7088869008250005064";
            Transaction response = card.FileAction()
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_001_Credit_Manual_Auth_VoyagerFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);

            card = TestCards.VoyagerFleetManual();
            card.TokenizationData = "C07341CB7DFFC17C5FA7A31C08938BDA";
            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Credit_Manual_Sale_VoyagerFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.VoyagerFleetManual();
            card.TokenizationData = "C07341CB7DFFC17C5FA7A31C08938BDA";
            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //force draft capture
        [TestMethod]
        public void Test_016_AuthCapture_VoyagerFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.VoyagerFleetManual();
            card.TokenizationData = "C07341CB7DFFC17C5FA7A31C08938BDA";
            Transaction response = card.Authorize(10m, true)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(response.AuthorizedAmount)
                    .WithCurrency("USD")
                    .WithFleetData(fleetData)
                    .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_004_Credit_Refund_VoyagerFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.VoyagerFleetManual();
            card.TokenizationData = "C07341CB7DFFC17C5FA7A31C08938BDA";
            Transaction response = card.Refund(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Sale_Reversal_VoyagerFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.VoyagerFleetManual();
            card.TokenizationData = "C07341CB7DFFC17C5FA7A31C08938BDA";

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithFleetData(fleetData)
                    .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(response);

            response.NTSData = ntsData;
            Assert.AreEqual("000", response.ResponseCode);

            Transaction reversal = response.Reverse(10m)
                    .WithCurrency("USD")
                    .WithFleetData(fleetData)
                    .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("400", reversal.ResponseCode);
        }

        [TestMethod]
        public void Test_015_Credit_Void_VoyagerFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.VoyagerFleetManual();
            card.TokenizationData = "C07341CB7DFFC17C5FA7A31C08938BDA";

            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            // reverse the transaction
            Transaction reverseResponse = response.Void()
                        .WithReferenceNumber("12345")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }
        #endregion

        #region Wex Fleet
        /// <summary>
        /// Single Use Token Generation
        /// </summary>
        [TestMethod]
        public void Test_File_Action_SingleUseToken_WexFleet() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleUseToken;
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.WexFleetManual();
            card.TokenizationData = "6900460430006149231";
            card.Cvn = "123";
            Transaction response = card.FileAction()
                    .WithFleetData(fleetData)
                    .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        /// <summary>
        /// Single use to multi use token swaping
        /// </summary>
        [TestMethod]
        public void Test_CombinedFile_Action_SingleToMultiUseToken_WexFleet() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleToMultiUseToken;
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 2m, 5m, 10m);

            card = TestCards.WexFleetManual();
            card.TokenizationData = "4D4DD8EF76EA14F0922F8D0A3B70249D";
            card.Cvn = "123";
            Transaction response = card.FileAction()
                .WithFleetData(fleetData)
                .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_SingleToMultiUseToken_Negative_InvalidToken_WexFleet() {
            // Attempt to swap an invalid single use token for a multi use token
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleToMultiUseToken;

            card = TestCards.WexFleetManual();
            card.TokenizationData = "INVALID_SINGLE_USE_TOKEN";
            card.Cvn = "123";

            Transaction response = card.FileAction()
                .WithFleetData(fleetData)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreNotEqual("000", response.ResponseCode, "Expected failure when using invalid single use token");
        }

        [TestMethod]
        public void Test_File_Action_WexFleet() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.Tokenize;
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.WexFleetManual();
            card.TokenizationData = "6900460430006149231";
            Transaction response = card.FileAction()
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_001_Credit_Manual_Auth_WexFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 2m, 5m, 10m);

            var card = TestCards.WexFleetManual();
            card.TokenizationData = "4D4DD8EF76EA14F0922F8D0A3B70249D";
            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Credit_Manual_Sale_WexFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 10.720m, 4.664m, 50m);
            card = TestCards.WexFleetManual();
            card.CardType = "WexFleet";
            card.TokenizationData = "4D4DD8EF76EA14F0922F8D0A3B70249D";
            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_016_AuthCapture_WexFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.WexFleetManual();
            card.TokenizationData = "4D4DD8EF76EA14F0922F8D0A3B70249D";
            Transaction response = card.Authorize(10m, true)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(response.AuthorizedAmount)
                    .WithCurrency("USD")
                    .WithFleetData(fleetData)
                    .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_004_Credit_Refund_WexFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 10.720m, 4.664m, 50m);
            card = TestCards.WexFleetManual();
            card.TokenizationData = "4D4DD8EF76EA14F0922F8D0A3B70249D";
            Transaction response = card.Refund(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .WithTransactionMatchingData(new TransactionMatchingData("0000040067", "0114"))
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_Credit_Balance_Inquiry_WexFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.WexFleetManual();
            card.TokenizationData = "4D4DD8EF76EA14F0922F8D0A3B70249D";
            Transaction response = card.BalanceInquiry()
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Sale_Reversal_WexFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.WexFleetManual();
            card.TokenizationData = "4D4DD8EF76EA14F0922F8D0A3B70249D";

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithFleetData(fleetData)
                    .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(response);

            response.NTSData = ntsData;
            Assert.AreEqual("000", response.ResponseCode);

            Transaction reversal = response.Reverse(10m)
                    .WithCurrency("USD")
                    .WithFleetData(fleetData)
                    .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("400", reversal.ResponseCode);
        }

        [TestMethod]
        public void Test_015_Credit_Void_WexFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 10.720m, 4.664m, 50m);
            card = TestCards.WexFleetManual();
            card.TokenizationData = "4D4DD8EF76EA14F0922F8D0A3B70249D";

            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            // reverse the transaction
            Transaction reverseResponse = response.Void()
                        .WithReferenceNumber("12345")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }
        #endregion

        #region Fuelman Fleet
        /// <summary>
        /// Single Use Token Generation
        /// </summary>
        [TestMethod]
        public void Test_File_Action_SingleUseToken_FuelmanFleet() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleUseToken;
            card = TestCards.FuelmanFleetManual();
            card.TokenizationData = "70764912345100040";
            card.Cvn = "123";
            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        /// <summary>
        /// Single use to multi use token swaping
        /// </summary>
        [TestMethod]
        public void Test_CombinedFile_Action_SingleToMultiUseToken_FuelmanFleet() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleToMultiUseToken;
            card = TestCards.FuelmanFleetManual();
            card.TokenizationData = "49F7F41810D331EBF7FDFFDA22A2A9A6";
            card.Cvn = "123";
            Transaction response = card.FileAction()
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_SingleToMultiUseToken_Negative_InvalidToken_FuelmanFleet() {
            // Attempt to swap an invalid single use token for a multi use token
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleToMultiUseToken;

            card = TestCards.FuelmanFleetManual();
            card.TokenizationData = "INVALID_SINGLE_USE_TOKEN";
            card.Cvn = "123";

            Transaction response = card.FileAction().Execute();

            Assert.IsNotNull(response);
            Assert.AreNotEqual("000", response.ResponseCode, "Expected failure when using invalid single use token");
        }

        [TestMethod]
        public void Test_File_Action_FuelmanFleet() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.Tokenize;
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.FuelmanFleetManual();
            card.TokenizationData = "70764912345100040";
            Transaction response = card.FileAction()
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_001_Credit_Manual_Auth_FuelmanFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);

            card = TestCards.FuelmanFleetManual();
            card.TokenizationData = "49F7F41810D331EBF7FDFFDA22A2A9A6";
            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Credit_Manual_Sale_FuelmanFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 10.720m, 4.664m, 50m);

            card = TestCards.FuelmanFleetManual();
            card.TokenizationData = "49F7F41810D331EBF7FDFFDA22A2A9A6";
            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //force draft capture
        [TestMethod]
        public void Test_016_AuthCapture_FuelmanFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.FuelmanFleetManual();
            card.TokenizationData = "49F7F41810D331EBF7FDFFDA22A2A9A6";
            Transaction response = card.Authorize(10m, true)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(response.AuthorizedAmount)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_004_Credit_Refund_FuelmanFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.FuelmanFleetManual();
            card.TokenizationData = "49F7F41810D331EBF7FDFFDA22A2A9A6";
            Transaction response = card.Refund(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_Credit_Balance_Inquiry_FuelmanFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.FuelmanFleetManual();
            card.TokenizationData = "49F7F41810D331EBF7FDFFDA22A2A9A6";
            Transaction response = card.BalanceInquiry()
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Sale_Reversal_FuelmanFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.FuelmanFleetManual();
            card.TokenizationData = "49F7F41810D331EBF7FDFFDA22A2A9A6";

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithFleetData(fleetData)
                    .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(response);

            response.NTSData = ntsData;
            Assert.AreEqual("000", response.ResponseCode);

            Transaction reversal = response.Reverse(10m)
                    .WithCurrency("USD")
                    .WithFleetData(fleetData)
                    .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("400", reversal.ResponseCode);
        }

        [TestMethod]
        public void Test_015_Credit_Void_FuelmanFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 10.720m, 4.664m, 50m);
            card = TestCards.FuelmanFleetManual();
            card.TokenizationData = "49F7F41810D331EBF7FDFFDA22A2A9A6";

            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            // void the transaction
            Transaction reverseResponse = response.Void()
                        .WithReferenceNumber("12345")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }
        #endregion

        #region FleetWide Fleet
        /// <summary>
        /// Single Use Token Generation
        /// </summary>
        [TestMethod]
        public void Test_File_Action_SingleUseToken_FleetWideFleet() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleUseToken;
            card = TestCards.FleetWideManual();
            card.TokenizationData = "70768512345200000";
            card.Cvn = "123";
            Transaction response = card.FileAction()
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        /// <summary>
        /// Single use to multi use token swaping
        /// </summary>
        [TestMethod]
        public void Test_CombinedFile_Action_SingleToMultiUseToken_FleetWideFleet() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleToMultiUseToken;
            card = TestCards.FleetWideManual();
            card.TokenizationData = "753619C69CABC0B36AAE5D31EBF977DE";
            card.Cvn = "123";
            Transaction response = card.FileAction()
                .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_SingleToMultiUseToken_Negative_InvalidToken_FleetWideFleet() {
            // Attempt to swap an invalid single use token for a multi use token
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.SingleToMultiUseToken;

            card = TestCards.FleetWideManual();
            card.TokenizationData = "INVALID_SINGLE_USE_TOKEN";
            card.Cvn = "123";

            Transaction response = card.FileAction().Execute();

            Assert.IsNotNull(response);
            Assert.AreNotEqual("000", response.ResponseCode, "Expected failure when using invalid single use token");
        }

        [TestMethod]
        public void Test_File_Action_FleetWideFleet() {
            acceptorConfig.TokenizationOperationType = TokenizationOperationType.Tokenize;
            card = TestCards.FleetWideManual();
            card.TokenizationData = "70768512345200115";
            Transaction response = card.FileAction()
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_001_Credit_Manual_Auth_FleetWideFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);

            card = TestCards.FleetWideManual();
            card.TokenizationData = "753619C69CABC0B36AAE5D31EBF977DE";
            Transaction response = card.Authorize(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_Credit_Manual_Sale_FleetWideFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 10.720m, 4.664m, 50m);
            card = TestCards.FleetWideManual();
            card.TokenizationData = "5D3D94E81EA0E914B9EAFBE41E7ED05A";
            Transaction response = card.Charge(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        //force draft capture
        [TestMethod]
        public void Test_016_AuthCapture_FleetWideFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.FleetWideManual();
            card.TokenizationData = "753619C69CABC0B36AAE5D31EBF977DE";
            Transaction response = card.Authorize(10m, true)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            Transaction captureResponse = response.Capture(response.AuthorizedAmount)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_004_Credit_Refund_FleetWideFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.FleetWideManual();
            card.TokenizationData = "753619C69CABC0B36AAE5D31EBF977DE";
            Transaction response = card.Refund(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_Credit_Balance_Inquiry_FleetWideFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.FleetWideManual();
            card.TokenizationData = "753619C69CABC0B36AAE5D31EBF977DE";
            Transaction response = card.BalanceInquiry()
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_Sale_Reversal_FleetWideFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.GlobalPayments);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            card = TestCards.FleetWideManual();
            card.TokenizationData = "753619C69CABC0B36AAE5D31EBF977DE";

            NtsData ntsData = new NtsData(FallbackCode.Received_IssuerUnavailable, AuthorizerCode.Terminal_Authorized);
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithFleetData(fleetData)
                    .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(response);

            response.NTSData = ntsData;
            Assert.AreEqual("000", response.ResponseCode);

            Transaction reversal = response.Reverse(10m)
                    .WithCurrency("USD")
                    .WithFleetData(fleetData)
                    .WithProductData(productData)
                    .Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("400", reversal.ResponseCode);
        }

        [TestMethod]
        public void Test_015_Credit_Void_FleetWideFleet() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Conexxus_3_Digit);
            productData.Add("001", UnitOfMeasure.Gallons, 10.720m, 4.664m, 50m);
            card = TestCards.FleetWideManual();
            card.TokenizationData = "753619C69CABC0B36AAE5D31EBF977DE";

            Transaction response = card.Charge(1m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000", response.ResponseCode);

            // reverse the transaction
            Transaction reverseResponse = response.Void()
                        .WithReferenceNumber("12345")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("400", reverseResponse.ResponseCode);
        }
        #endregion
    }
}
