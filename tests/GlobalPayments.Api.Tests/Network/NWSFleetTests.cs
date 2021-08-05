using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Network.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using UnitOfMeasure = GlobalPayments.Api.Network.Entities.UnitOfMeasure;

namespace GlobalPayments.Api.Tests.Network {
    [TestClass]
    public class NWSFleetTests {
        private CreditCardData card;
        private CreditTrackData track;
        private ProductData productData;
        private FleetData fleetData;

        public NWSFleetTests() {
            AcceptorConfig acceptorConfig = new AcceptorConfig {
                // data code values
                CardDataInputCapability = CardDataInputCapability.ContactlessEmv_ContactEmv_MagStripe_KeyEntry,
                CardHolderAuthenticationCapability = CardHolderAuthenticationCapability.None,
                TerminalOutputCapability = TerminalOutputCapability.Printing_Display,
                CardHolderAuthenticationEntity = CardHolderAuthenticationEntity.ByMerchant,
                //OperatingEnvironment = OperatingEnvironment.OnPremises_CardAcceptor_Unattended,
                // hardware software config values
                HardwareLevel = "34",
                SoftwareLevel = "21205710",

                // pos configuration values
                SupportsPartialApproval = false,
                SupportsShutOffAmount = true,
                SupportsReturnBalance = true,
                SupportsDiscoverNetworkReferenceId = true,
                SupportsAvsCnvVoidReferrals = true,
                SupportedEncryptionType = EncryptionType.TEP2
            };

            // gateway config
            NetworkGatewayConfig config = new NetworkGatewayConfig(NetworkGatewayType.NWS) {
                ServiceUrl = "test.txns-c.secureexchange.net",
                PrimaryPort = 15031,
                SecondaryEndpoint = "test.txns-e.secureexchange.net",
                SecondaryPort = 15031,
                CompanyId = "SPSA",
                TerminalId = "NWSDOTNET01",
                UniqueDeviceId = "0001",
                AcceptorConfig = acceptorConfig,
                EnableLogging = true,
                StanProvider = StanGenerator.GetInstance(),
                BatchProvider = BatchProvider.GetInstance()
            };

            ServicesContainer.ConfigureService(config);

            config.MerchantType = "5542";
            ServicesContainer.ConfigureService(config, "ICR");

            // MASTERCARD FLEET
            //card = TestCards.MasterCardFleetManual(true, true);
            //track = TestCards.MasterCardFleetSwipe();
            //productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            //productData.Add("03", UnitOfMeasure.Gallons, 1, 10);

            // VOYAGER FLEET
            //card = TestCards.VoyagerFleetManual(true, true);
            //track = TestCards.VoyagerFleetSwipe();

            fleetData = new FleetData {
                //ServicePrompt = "0",
                DriverId = "11411",
                VehicleNumber = "22031",
                OdometerReading = "1256"
            };

            // VISA
            card = TestCards.VisaFleetManual(true, true);
            track = TestCards.VisaFleetSwipe();
            //productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            //productData.Add("01", UnitOfMeasure.Gallons, 1, 10);

            //FleetOne
            //card = TestCards.FleetOneManual(true, true);
            //track = TestCards.FleetOneSwipe();

            //FleetCor Fuelman
            //card = TestCards.FuelmanManual(true, true);
            //track = TestCards.FuelmanSwipe();

            // FleetCor Fleetwide
            //card = TestCards.FleetWideManual(true, true);
            //track = TestCards.FleetWideSwipe();
        }

        [TestMethod]
        public void Test_000_batch_close() {
            BatchSummary summary = BatchService.CloseBatch(BatchCloseType.Forced);
            Assert.IsNotNull(summary);
            Assert.IsTrue(summary.IsBalanced);
        }

        [TestMethod]
        public void Test_001_manual_authorization() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = card.Authorize(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("100", pmi.FunctionCode);

            // check response
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_002_manual_sale() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_003_swipe_refund() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = track.Refund(10m)
                        .WithCurrency("USD")
                        .WithFleetData(fleetData)
                        .WithProductData(productData)
                        .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("200009", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_004_swipe_stand_in_capture() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);

            Transaction transaction = Transaction.FromNetwork(
                    10m,
                    "TYPE04",
                    new NtsData(FallbackCode.Received_IssuerTimeout, AuthorizerCode.Host_Authorized),
                    track
            );

            Transaction response = transaction.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithAuthenticatioNMethod(CardHolderAuthenticationMethod.ManualSignatureVerification)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("201", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_005_swipe_voice_capture() {
            Transaction transaction = Transaction.FromNetwork(
                        10m,
                        "TYPE04",
                        new NtsData(FallbackCode.None, AuthorizerCode.Voice_Authorized),
                        track
                );

            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);

            Transaction response = transaction.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .WithAuthenticatioNMethod(CardHolderAuthenticationMethod.ManualSignatureVerification)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("201", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_006_swipe_void() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);

            Transaction sale = track.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(sale);
            Assert.AreEqual("000", sale.ResponseCode);
            //Assert.IsNotNull(sale.ReferenceNumber);

            Transaction response = sale.Void(amount: sale.AuthorizedAmount)
                    .WithReferenceNumber("12345")
                    .WithAuthenticatioNMethod(CardHolderAuthenticationMethod.ManualSignatureVerification)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("441", pmi.FunctionCode);
            Assert.AreEqual("4351", pmi.MessageReasonCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("400", response.ResponseCode);
        }

        [TestMethod]
        public void Test_007_swipe_reverse_sale() {
            try {
                track.Charge(10m)
                        .WithCurrency("USD")
                        .WithForceGatewayTimeout(true)
                        .Execute();
                Assert.Fail("Did not throw a timeout");
            }
            catch (GatewayTimeoutException exc) {
                Assert.AreEqual(1, exc.ReversalCount);
                Assert.AreEqual("400", exc.ReversalResponseCode);
            }
        }

        [TestMethod]
        public void Test_008_ICR_authorization() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            productData.Add("01", UnitOfMeasure.Gallons, 2m, 5m, 10m);
            Transaction response = track.Authorize(10m, false)
                        .WithCurrency("USD")
                        .WithProductData(productData)
                        .WithFleetData(fleetData)
                        //.Execute();
                        .Execute("ICR");
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1100", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            //Assert.AreEqual("101", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);

            // partial approval cancellation
            //Transaction reversal = response.Cancel()
            //        .WithReferenceNumber(response.ReferenceNumber)
            //        .Execute();
            //Assert.IsNotNull(reversal);

            //pmi = reversal.MessageInformation;
            //Assert.AreEqual("1420", pmi.MessageTransactionIndicator);
            //Assert.AreEqual("000900", pmi.ProcessingCode);
            //Assert.AreEqual("441", pmi.FunctionCode);
            //Assert.AreEqual("4352", pmi.MessageReasonCode);

            //Assert.AreEqual("400", reversal.ResponseCode);
            //System.Diagnostics.Debug.WriteLine(reversal.HostResponseDate);
            //System.Diagnostics.Debug.WriteLine(reversal.SystemTraceAuditNumber);
            // Test_009


            Transaction captureResponse = response.Capture(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    //.Execute();
                    .Execute("ICR");
            Assert.IsNotNull(captureResponse);

            // check message data
            pmi = captureResponse.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1220", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            //Assert.AreEqual("201", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(captureResponse.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(captureResponse.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void Test_009_swipe_sale_product_01() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.Heartland);
            productData.Add("01", UnitOfMeasure.Gallons, 1m, 3.99m, 3.99m);

            Transaction response = track.Charge(3.99m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_010_swipe_sale_product_02() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);
            productData.Add("02", UnitOfMeasure.Gallons, 1m, 5m, 5m);

            Transaction response = track.Charge(5m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_011_swipe_sale_mc_product_03() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);
            productData.Add("03", UnitOfMeasure.Gallons, 1m, 10m, 10m);

            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_012_swipe_sale_voyager_product_04()
        {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);
            productData.Add("01", UnitOfMeasure.Gallons, 1m, 24m, 24m);
            productData.Add("59", UnitOfMeasure.Pounds, 1m, 10m, 10m);
            productData.Add("09", UnitOfMeasure.Quarts, 1m, 5m, 5m);
            productData.Add("27", UnitOfMeasure.Units, 1m, 6m, 6m);
            productData.Add("23", UnitOfMeasure.Units, 1m, 50m, 50m);
            productData.Add("14", UnitOfMeasure.Pounds, 1m, 3m, 3m);
            productData.Add("33", UnitOfMeasure.OtherOrUnknown, 1m, 2m, 2m);

            fleetData = new FleetData()
            {
                VehicleNumber = "22031",
                DriverId = "11411",
                OdometerReading = "1256"
            };

            Transaction response = track.Charge(100m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_013_swipe_sale_voyager_product_09()
        {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);
            productData.Add("09", UnitOfMeasure.Quarts, 1m, 100m, 100m);

            Transaction response = track.Charge(100m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_014_swipe_sale_voyager_product_14()
        {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);
            productData.Add("14", UnitOfMeasure.Units, 1m, 100m, 100m);

            Transaction response = track.Charge(100m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);

            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_015_swipe_sale_mc_product_16() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);
            productData.Add("16", UnitOfMeasure.Gallons, 1m, 10m, 10m);

            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        //[TestMethod]
        //public void Test_016_swipe_sale_voyager_product_23() {
        //    ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);
        //    productData.Add("23", UnitOfMeasure.Units, 1m, 10m, 10m);

        //    Transaction response = track.Charge(10m)
        //            .WithCurrency("USD")
        //            .WithProductData(productData)
        //            .WithFleetData(fleetData)
        //            .Execute();
        //    Assert.IsNotNull(response);

        //    // check message data
        //    PriorMessageInformation pmi = response.MessageInformation;
        //    Assert.IsNotNull(pmi);
        //    Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
        //    Assert.AreEqual("000900", pmi.ProcessingCode);
        //    Assert.AreEqual("200", pmi.FunctionCode);

        //    // check response
        //    Assert.AreEqual("000", response.ResponseCode);
        //}

        //[TestMethod]
        //public void Test_017_swipe_sale_voyager_product_27() {
        //    ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);
        //    productData.Add("27", UnitOfMeasure.Units, 1m, 10m, 10m);

        //    Transaction response = track.Charge(10m)
        //            .WithCurrency("USD")
        //            .WithProductData(productData)
        //            .WithFleetData(fleetData)
        //            .Execute();
        //    Assert.IsNotNull(response);

        //    // check message data
        //    PriorMessageInformation pmi = response.MessageInformation;
        //    Assert.IsNotNull(pmi);
        //    Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
        //    Assert.AreEqual("000900", pmi.ProcessingCode);
        //    Assert.AreEqual("200", pmi.FunctionCode);

        //    // check response
        //    Assert.AreEqual("000", response.ResponseCode);
        //}

        [TestMethod]
        public void Test_018_swipe_sale_mc_product_30() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);
            productData.Add("30", UnitOfMeasure.Quarts, 1m, 10m, 10m);

            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        //[TestMethod]
        //public void Test_019_swipe_sale_voyager_product_33() {
        //    ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);
        //    productData.Add("33", UnitOfMeasure.Units, 1m, 10m, 10m);

        //    Transaction response = track.Charge(10m)
        //            .WithCurrency("USD")
        //            .WithProductData(productData)
        //            .WithFleetData(fleetData)
        //            .Execute();
        //    Assert.IsNotNull(response);

        //    // check message data
        //    PriorMessageInformation pmi = response.MessageInformation;
        //    Assert.IsNotNull(pmi);
        //    Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
        //    Assert.AreEqual("000900", pmi.ProcessingCode);
        //    Assert.AreEqual("200", pmi.FunctionCode);

        //    // check response
        //    Assert.AreEqual("000", response.ResponseCode);
        //}

        [TestMethod]
        public void Test_020_swipe_sale_product_39() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);
            productData.Add("39", UnitOfMeasure.OtherOrUnknown, 1m, 10m, 10m);

            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_021_swipe_sale_mc_product_41() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);
            productData.Add("41", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_022_swipe_sale_mc_product_45() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);
            productData.Add("45", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        //[TestMethod]
        //public void Test_023_swipe_sale_voyager_product_59() {
        //    ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);
        //    productData.Add("59", UnitOfMeasure.Gallons, 1m, 10m, 10m);

        //    Transaction response = track.Charge(10m)
        //            .WithCurrency("USD")
        //            .WithProductData(productData)
        //            .WithFleetData(fleetData)
        //            .Execute();
        //    Assert.IsNotNull(response);

        //    // check message data
        //    PriorMessageInformation pmi = response.MessageInformation;
        //    Assert.IsNotNull(pmi);
        //    Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
        //    Assert.AreEqual("000900", pmi.ProcessingCode);
        //    Assert.AreEqual("200", pmi.FunctionCode);

        //    // check response
        //    Assert.AreEqual("000", response.ResponseCode);
        //}

        [TestMethod]
        public void Test_024_swipe_sale_mc_product_79() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);
            productData.Add("79", UnitOfMeasure.Units, 1m, 10m, 10m);

            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_025_swipe_sale_mc_product_99() {
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);
            productData.Add("99", UnitOfMeasure.OtherOrUnknown, 1m, 10m, 10m);

            Transaction response = track.Charge(10m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }

        [TestMethod]
        public void Test_026_swipe_sale_product_all() {
            track = TestCards.VisaFleetSwipe();
            ProductData productData = new ProductData(ServiceLevel.FullServe, ProductCodeSet.IssuerSpecific);            
            productData.Add("01", UnitOfMeasure.Gallons, 1m, 10m, 10m);
            productData.Add("03", UnitOfMeasure.Gallons, 2m, 10m, 20m);
            productData.Add("79", UnitOfMeasure.Quarts, 4m, 10m, 40m);
            productData.Add("45", UnitOfMeasure.Gallons, 1m, 10m, 10m);
            productData.Add("41", UnitOfMeasure.Gallons, 5m, 10m, 50m);
            productData.Add("99", UnitOfMeasure.Gallons, 2m, 15m, 30m);

            Transaction response = track.Charge(160m)
                    .WithCurrency("USD")
                    .WithProductData(productData)
                    .WithFleetData(fleetData)
                    .Execute();
            Assert.IsNotNull(response);

            // check message data
            PriorMessageInformation pmi = response.MessageInformation;
            Assert.IsNotNull(pmi);
            Assert.AreEqual("1200", pmi.MessageTransactionIndicator);
            Assert.AreEqual("000900", pmi.ProcessingCode);
            Assert.AreEqual("200", pmi.FunctionCode);
            System.Diagnostics.Debug.WriteLine(response.HostResponseDate);
            System.Diagnostics.Debug.WriteLine(response.SystemTraceAuditNumber);
            // check response
            Assert.AreEqual("000", response.ResponseCode);
        }
    }
}
