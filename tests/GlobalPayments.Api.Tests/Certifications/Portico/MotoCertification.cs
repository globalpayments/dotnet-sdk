using System.Diagnostics;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Certifications.Portico
{
    [TestClass]
    public class MotoCertification
    {
        bool useTokens = false;

        string visa_token;
        string mastercard_token;
        string discover_token;
        string amex_token;

        EcommerceInfo ecom = new EcommerceInfo
        {
            Channel = EcommerceChannel.ECOM
        };

        public MotoCertification()
        {
            ServicesContainer.ConfigureService(new PorticoConfig
            {
                SecretApiKey = "skapi_cert_MRCQAQBC_VQACBE0rFaZlbDDPieMGP06JDAtjyS7NQ"
            });
        }

        [TestMethod]
        public void ecomm_000_CloseBatch()
        {
            try
            {
                var response = BatchService.CloseBatch();
                Assert.IsNotNull(response);
                Debug.WriteLine(string.Format("Batch ID: {0}", response.Id));
                Debug.WriteLine(string.Format("Sequence Number: {0}", response.SequenceNumber));
            }
            catch (GatewayException exc)
            {
                if (exc.ResponseMessage != "Transaction was rejected because it requires a batch to be open.")
                    Assert.Fail(exc.Message);
            }
        }

        [TestMethod]
        public void ecomm_001_verify_visa()
        {
            var card = new CreditCardData
            {
                Number = "4484958240202792",
                ExpMonth = 12,
                ExpYear = 2025
            };

            var response = card.Verify()
                .WithRequestMultiUseToken(useTokens)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void ecomm_002_verify_master_card()
        {
            var card = new CreditCardData
            {
                Number = "5356083898949891",
                ExpMonth = 12,
                ExpYear = 2025
            };

            var response = card.Verify()
                .WithRequestMultiUseToken(useTokens)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void ecomm_003_verify_discover()
        {
            var card = new CreditCardData
            {
                Number = "6223971100014620",
                ExpMonth = 12,
                ExpYear = 2025
            };

            var response = card.Verify()
                .WithAddress(new Address { PostalCode = "75024" })
                .WithRequestMultiUseToken(useTokens)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // StreetAddress Verification

        [TestMethod]
        public void ecomm_004_verify_amex()
        {
            var card = new CreditCardData
            {
                Number = "345039962663847",
                ExpMonth = 12,
                ExpYear = 2025
            };

            var response = card.Verify()
                .WithAddress(new Address { PostalCode = "75024" })
                .WithRequestMultiUseToken(useTokens)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Balance Inquiry (for Prepaid Card)

        [TestMethod]
        public void ecomm_005_balance_inquiry_visa()
        {
            var card = new CreditCardData
            {
                Number = "4664383951958601",
                ExpMonth = 12,
                ExpYear = 2025
            };

            var response = card.BalanceInquiry().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        // CREDIT SALE (For Multi-Use Token Only)

        [TestMethod, Ignore]
        public void ecomm_006_charge_visa_token()
        {
            var address = new Address
            {
                StreetAddress1 = "6860 Dallas Pkwy",
                PostalCode = "75024"
            };

            var card = new CreditCardData
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025
            };

            var response = card.Charge(13.01m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithRequestMultiUseToken(true)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            visa_token = response.Token;
        }

        [TestMethod, Ignore]
        public void ecomm_007_charge_master_card_token()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };

            var card = new CreditCardData
            {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var response = card.Charge(13.02m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithRequestMultiUseToken(true)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            mastercard_token = response.Token;
        }

        [TestMethod, Ignore]
        public void ecomm_008_charge_discover_token()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "750241234" };

            var card = new CreditCardData
            {
                Number = "6011000990156527",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var response = card.Charge(13.03m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithRequestMultiUseToken(true)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            discover_token = response.Token;
        }

        [TestMethod, Ignore]
        public void ecomm_009_charge_amex_token()
        {
            var address = new Address { StreetAddress1 = "6860 Dallas Pkwy", PostalCode = "75024" };

            var card = new CreditCardData
            {
                Number = "372700699251018",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "1234"
            };

            var response = card.Charge(13.04m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithRequestMultiUseToken(true)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            amex_token = response.Token;
        }

        // CREDIT SALE

        [TestMethod]
        public void ecomm_010_charge_visa()
        {
            var address = new Address { StreetAddress1 = "6860 Dallas Pkwy", PostalCode = "75024" };
            var card = useTokens ? new CreditCardData { Token = visa_token } : TestCards.VisaManual();

            var response = card.Charge(17.01m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // test case 35
            var voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_011_charge_master_card()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };
            var card = useTokens ? new CreditCardData { Token = mastercard_token } : TestCards.MasterCardManual();

            var chargeResponse = card.Charge(17.02m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_012_charge_discover()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "750241234" };
            var card = useTokens ? new CreditCardData { Token = discover_token } : TestCards.DiscoverManual();

            var chargeResponse = card.Charge(17.03m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_013_charge_amex()
        {
            var address = new Address { StreetAddress1 = "6860 Dallas Pkwy", PostalCode = "75024" };
            var card = useTokens ? new CreditCardData { Token = amex_token } : TestCards.AmexManual();

            var chargeResponse = card.Charge(17.04m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_014_charge_jcb()
        {
            var address = new Address { StreetAddress1 = "6860 Dallas Pkwy", PostalCode = "750241234" };

            var card = new CreditCardData
            {
                Number = "3566007770007321",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var chargeResponse = card.Charge(17.05m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_011b_charge_master_card()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };
            var card = TestCards.MasterCardSeries2Manual();

            var chargeResponse = card.Charge(17.02m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        // AUTHORIZATION

        [TestMethod]
        public void ecomm_015_authorization_visa()
        {
            var address = new Address { StreetAddress1 = "6860 Dallas Pkwy", PostalCode = "75024" };

            var card = new CreditCardData
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var authResponse = card.Authorize(17.06m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.ResponseCode);

            // test 015b Capture/AddToBatch
            var captureResponse = authResponse.Capture().Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_016_authorization_master_card()
        {
            var address = new Address { StreetAddress1 = "6860 Dallas Pkwy", PostalCode = "750241234" };

            var card = new CreditCardData
            {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var authResponse = card.Authorize(17.07m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.ResponseCode);

            // test 016b Capture/AddToBatch
            var captureResponse = authResponse.Capture().Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_017_authorization_discover()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };

            var card = new CreditCardData
            {
                Number = "6011000990156527",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var authResponse = card.Authorize(17.07m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .Execute();
            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_016b_authorization_master_card()
        {
            var address = new Address { StreetAddress1 = "6860 Dallas Pkwy", PostalCode = "750241234" };

            var card = new CreditCardData
            {
                Number = "2223000010005780",
                ExpMonth = 12,
                ExpYear = 2019,
                Cvn = "900"
            };

            var authResponse = card.Authorize(17.07m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(authResponse);
            Assert.AreEqual("00", authResponse.ResponseCode);

            // test 016b Capture/AddToBatch
            var captureResponse = authResponse.Capture().Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        // PARTIALLY - APPROVED SALE

        [TestMethod]
        public void ecomm_018_partial_approval_visa()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };

            var card = new CreditCardData
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var response = card.Charge(130m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .WithAllowPartialAuth(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(110.00m, response.AuthorizedAmount);
        }

        [TestMethod]
        public void ecomm_019_partial_approval_discover()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };

            var card = new CreditCardData
            {
                Number = "6011000990156527",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var response = card.Charge(145m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .WithAllowPartialAuth(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(65.00m, response.AuthorizedAmount);
        }

        [TestMethod]
        public void ecomm_020_partial_approval_master_card()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };

            var card = new CreditCardData
            {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var chargeResponse = card.Charge(155m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .WithAllowPartialAuth(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("10", chargeResponse.ResponseCode);
            Assert.AreEqual(100.00m, chargeResponse.AuthorizedAmount);

            // test case 36
            var voidResponse = chargeResponse.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        // LEVEL II CORPORATE PURCHASE CARD

        [TestMethod]
        public void ecomm_021_level_ii_response_b()
        {
            var address = new Address { StreetAddress1 = "6860 Dallas Pkwy", PostalCode = "750241234" };

            var card = new CreditCardData
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var chargeResponse = card.Charge(112.34m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .WithCommercialRequest(true)
                .Execute();
            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("B", chargeResponse.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.NOTUSED)
            {
                PoNumber = "9876543210"
            };

            var cpcResponse = chargeResponse.Edit()
                .WithCommercialData(commercialData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_022_level_ii_response_b()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "750241234" };

            var card = new CreditCardData
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var chargeResponse = card.Charge(112.34m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .WithCommercialRequest(true)
                .Execute();
            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("B", chargeResponse.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX)
            {
                TaxAmount = 1m
            };

            var cpcResponse = chargeResponse.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_023_level_ii_response_r()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };

            var card = new CreditCardData
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var chargeResponse = card.Charge(123.45m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithCommercialRequest(true)
                .Execute();
            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("R", chargeResponse.CommercialIndicator);

            var cpcResponse = chargeResponse.Edit()
                .WithCommercialData(new CommercialData(TaxType.TAXEXEMPT))
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_024_level_ii_response_s()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };

            var card = new CreditCardData
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var chargeResponse = card.Charge(134.56m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithCommercialRequest(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("S", chargeResponse.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX)
            {
                PoNumber = "9876543210",
                TaxAmount = 1m
            };

            var cpcResponse = chargeResponse.Edit()
                .WithCommercialData(commercialData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_025_level_ii_response_s()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };

            var card = new CreditCardData
            {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var chargeResponse = card.Charge(111.06m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithCommercialRequest(true)
                .Execute();
            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("S", chargeResponse.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.NOTUSED)
            {
                PoNumber = "9876543210"
            };

            var cpcResponse = chargeResponse.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_026_level_ii_response_s()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };

            var card = new CreditCardData
            {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var chargeResponse = card.Charge(111.07m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithCommercialRequest(true)
                .Execute();
            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("S", chargeResponse.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX)
            {
                TaxAmount = 1m
            };

            var cpcResponse = chargeResponse.Edit()
                .WithCommercialData(commercialData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_027_level_ii_response_s()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };

            var card = new CreditCardData
            {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var chargeResponse = card.Charge(111.08m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithCommercialRequest(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("S", chargeResponse.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX)
            {
                TaxAmount = 1m
            };

            var cpcResponse = chargeResponse.Edit()
                .WithCommercialData(commercialData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_028_level_ii_response_s()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };

            var card = new CreditCardData
            {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var chargeResponse = card.Charge(111.09m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithCommercialRequest(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("S", chargeResponse.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.TAXEXEMPT)
            {
                PoNumber = "9876543210"
            };

            var cpcResponse = chargeResponse.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_029_level_ii_no_response()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };

            var card = new CreditCardData
            {
                Number = "372700699251018",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "1234"
            };

            var chargeResponse = card.Charge(111.10m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithCommercialRequest(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("0", chargeResponse.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.NOTUSED)
            {
                PoNumber = "9876543210"
            };

            var cpcResponse = chargeResponse.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_030_level_ii_no_response()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "750241234" };

            var card = new CreditCardData
            {
                Number = "372700699251018",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "1234"
            };

            var chargeResponse = card.Charge(111.11m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithCommercialRequest(true)
                .Execute();
            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("0", chargeResponse.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX)
            {
                TaxAmount = 1m
            };

            var cpcResponse = chargeResponse.Edit()
                .WithCommercialData(commercialData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_031_level_ii_no_response()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "750241234" };

            var card = new CreditCardData
            {
                Number = "372700699251018",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "1234"
            };

            var chargeResponse = card.Charge(111.12m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithCommercialRequest(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("0", chargeResponse.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX)
            {
                PoNumber = "9876543210",
                TaxAmount = 1m
            };

            var cpcResponse = chargeResponse.Edit()
                .WithCommercialData(commercialData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_032_level_ii_no_response()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };

            var card = new CreditCardData
            {
                Number = "372700699251018",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "1234"
            };

            var chargeResponse = card.Charge(111.13m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithAddress(address)
                .WithCommercialRequest(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
            Assert.AreEqual("0", chargeResponse.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.TAXEXEMPT)
            {
                PoNumber = "9876543210"
            };

            var cpcResponse = chargeResponse.Edit()
                .WithCommercialData(commercialData)
                .Execute();

            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        // PRIOR / VOICE AUTHORIZATION

        [TestMethod]
        public void ecomm_033_offline_sale()
        {
            var card = new CreditCardData
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var response = card.Charge(17.10m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithOfflineAuthCode("654321")
                .WithInvoiceNumber("123456")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void ecomm_033_offline_authorization()
        {
            var card = new CreditCardData
            {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var response = card.Authorize(17.10m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithOfflineAuthCode("654321")
                .WithInvoiceNumber("123456")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // RETURN

        [TestMethod]
        public void ecomm_034_offline_credit_return()
        {
            var card = new CreditCardData
            {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var response = card.Refund(15.15m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithInvoiceNumber("123456")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void ecomm_034b_offline_credit_return()
        {
            var card = new CreditCardData
            {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var response = card.Refund(15.16m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithInvoiceNumber("123456")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // ONLINE VOID / REVERSAL

        [TestMethod]
        public void ecomm_035_void_ecomm_10()
        {
            // see test case 10
        }

        [TestMethod]
        public void ecomm_036_void_ecomm_20()
        {
            // see test case 20
        }

        // Time Out Reversal

        [TestMethod, ExpectedException(typeof(GatewayException))]
        public void ecomm_036b_timeout_reversal()
        {
            var sale = TestCards.VisaManual().Charge(911m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithClientTransactionId("987321654")
                .Execute();
            Assert.IsNotNull(sale);
            Assert.AreEqual("91", sale.ResponseCode);

            var response = Transaction.FromId(null, PaymentMethodType.Credit);
            response.ClientTransactionId = "987321654";

            var reversalResponse = response.Reverse(911m).Execute();
        }

        // One time bill payment

        [TestMethod]
        public void ecomm_010_charge_visa_onetime()
        {
            var address = new Address { StreetAddress1 = "6860 Dallas Pkwy", PostalCode = "75024" };
            var card = useTokens ? new CreditCardData { Token = visa_token } : TestCards.VisaManual();

            var response = card.Charge(13.11m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .WithOneTimePayment(true)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // test case 35
            var voidResponse = response.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_011_charge_mastercard_onetime()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "750241234" };
            var card = useTokens ? new CreditCardData { Token = mastercard_token } : TestCards.MasterCardManual();

            var chargeResponse = card.Charge(13.12m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .WithOneTimePayment(true)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_012_charge_discover_onetime()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };
            var card = useTokens ? new CreditCardData { Token = discover_token } : TestCards.DiscoverManual();

            var chargeResponse = card.Charge(13.13m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .WithOneTimePayment(true)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_013_charge_amex_onetime()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };
            var card = useTokens ? new CreditCardData { Token = amex_token } : TestCards.AmexManual();

            var chargeResponse = card.Charge(13.14m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .WithOneTimePayment(true)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_014_charge_jcb_onetime()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };

            var card = new CreditCardData
            {
                Number = "3566007770007321",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvn = "123"
            };

            var chargeResponse = card.Charge(13.15m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .WithOneTimePayment(true)
                .WithAllowDuplicates(true)
                .Execute();

            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        // HMS GIFT - REWARDS

        // ACTIVATE

        [TestMethod]
        public void ecomm_042_activate_gift_1()
        {
            var giftCard1 = new GiftCard { Number = "5022440000000000098" };
            var response = giftCard1.Activate(6.00m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void ecomm_043_activate_gift_2()
        {
            var giftCard2 = new GiftCard { Number = "5022440000000000007" };
            var response = giftCard2.Activate(7.00m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // LOAD / ADD VALUE

        [TestMethod]
        public void ecomm_044_add_value_gift_1()
        {
            var giftCard1 = new GiftCard { Number = "5022440000000000098" };
            var response = giftCard1.Activate(8.00m).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void ecomm_045_add_value_gift_2()
        {
            var giftCard2 = new GiftCard { Number = "5022440000000000007" };
            var response = giftCard2.Activate(8.00m).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // BALANCE INQUIRY

        [TestMethod]
        public void ecomm_046_balance_inquiry_gift_1()
        {
            var giftCard1 = new GiftCard { Number = "5022440000000000098" };
            var response = giftCard1.BalanceInquiry().Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(10m, response.BalanceAmount);
        }

        [TestMethod]
        public void ecomm_047_balance_inquiry_gift_2()
        {
            var giftCard2 = new GiftCard { Number = "5022440000000000007" };
            var response = giftCard2.BalanceInquiry().Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(10m, response.BalanceAmount);
        }

        // REPLACE / TRANSFER

        [TestMethod]
        public void ecomm_048_replace_gift_1()
        {
            var giftCard1 = new GiftCard { Number = "5022440000000000098" };
            var giftCard2 = new GiftCard { Number = "5022440000000000007" };
            var response = giftCard1.ReplaceWith(giftCard2).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(10m, response.BalanceAmount);
        }

        [TestMethod]
        public void ecomm_049_replace_gift_2()
        {
            var giftCard1 = new GiftCard { Number = "5022440000000000098" };
            var giftCard2 = new GiftCard { Number = "5022440000000000007" };
            var response = giftCard2.ReplaceWith(giftCard1).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(10m, response.BalanceAmount);
        }

        // SALE / REDEEM

        [TestMethod]
        public void ecomm_050_sale_gift_1()
        {
            var giftCard1 = new GiftCard { Number = "5022440000000000098" };
            var response = giftCard1.Charge(1.0m)
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void ecomm_051_sale_gift_2()
        {
            var giftCard2 = new GiftCard { Number = "5022440000000000007" };
            var response = giftCard2.Charge(2.0m)
                .WithCurrency("USD")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void ecomm_052_sale_gift_1_void()
        {
            var giftCard1 = new GiftCard { Number = "5022440000000000098" };

            var saleResponse = giftCard1.Charge(3.0m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);

            // test case 54
            var voidResponse = saleResponse.Void().Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void ecomm_053_sale_gift_2_reversal()
        {
            var giftCard2 = new GiftCard { Number = "5022440000000000007" };

            var saleResponse = giftCard2.Charge(4.0m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);

            // test case 55
            var reverseResponse = saleResponse.Reverse(4.0m).Execute();
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("00", reverseResponse.ResponseCode);
        }

        // VOID

        [TestMethod]
        public void ecomm_054_void_gift()
        {
            // see test case 52
        }

        // REVERSAL

        [TestMethod]
        public void ecomm_055_reversal_gift()
        {
            // see test case 53
        }

        [TestMethod]
        public void ecomm_056_reversal_gift_2()
        {
            var giftCard2 = new GiftCard { Number = "5022440000000000007" };

            var response = giftCard2.Reverse(2.0m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // DEACTIVATE

        [TestMethod]
        public void ecomm_057_deactivate_gift_1()
        {
            var giftCard1 = new GiftCard { Number = "5022440000000000098" };

            var response = giftCard1.Deactivate().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // RECEIPTS MESSAGING

        // ecomm_058_receipts_messaging: print and scan receipt for test 51

        // BALANCE INQUIRY

        [TestMethod]
        public void ecomm_059_balance_inquiry_rewards_1()
        {
            var giftCard1 = new GiftCard { Number = "5022440000000000098" };
            var response = giftCard1.BalanceInquiry().Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(0.00m, response.PointsBalanceAmount);
        }

        [TestMethod]
        public void ecomm_060_balance_inquiry_rewards_2()
        {
            var giftCard2 = new GiftCard { Number = "5022440000000000007" };
            var response = giftCard2.BalanceInquiry().Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(0.00m, response.PointsBalanceAmount);
        }

        // ALIAS

        [TestMethod]
        public void ecomm_061_create_alias_gift_1()
        {
            var card = GiftCard.Create("9725550100");
            Assert.IsNotNull(card);
        }

        [TestMethod]
        public void ecomm_062_create_alias_gift_2()
        {
            var card = GiftCard.Create("9725550100");
            Assert.IsNotNull(card);
        }

        [TestMethod]
        public void ecomm_063_add_alias_gift_1()
        {
            var giftCard1 = new GiftCard { Number = "5022440000000000098" };
            var response = giftCard1.AddAlias("2145550199").Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void ecomm_064_add_alias_gift_2()
        {
            var giftCard2 = new GiftCard { Number = "5022440000000000007" };

            var response = giftCard2.AddAlias("2145550199").Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void ecomm_065_delete_alias_gift_1()
        {
            var giftCard1 = new GiftCard { Number = "5022440000000000098" };

            var response = giftCard1.RemoveAlias("2145550199").Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        // SALE / REDEEM

        [TestMethod]
        public void ecomm_066_redeem_points_gift_1()
        {
            var giftCard1 = new GiftCard { Number = "5022440000000000098" };

            var response = giftCard1.Charge(100m)
                .WithCurrency("POINTS")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void ecomm_067_redeem_points_gift_2()
        {
            var giftCard2 = new GiftCard { Number = "5022440000000000007" };

            var response = giftCard2.Charge(200m)
                .WithCurrency("POINTS")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        [TestMethod]
        public void ecomm_068_redeem_points_gift_2()
        {
            var giftCard = new GiftCard { Alias = "9725550100" };

            var response = giftCard.Charge(300.00m)
                .WithCurrency("POINTS")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);
        }

        // REWARDS

        [TestMethod]
        public void ecomm_069_rewards_gift_1()
        {
            var giftCard1 = new GiftCard { Number = "5022440000000000098" };

            var response = giftCard1.Rewards(10.00m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void ecomm_070_rewards_gift_2()
        {
            var giftCard2 = new GiftCard { Number = "5022440000000000007" };

            var response = giftCard2.Rewards(11.00m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // REPLACE / TRANSFER

        [TestMethod]
        public void ecomm_071_replace_gift_1()
        {
            var giftCard1 = new GiftCard { Number = "5022440000000000098" };
            var giftCard2 = new GiftCard { Number = "5022440000000000007" };

            var response = giftCard1.ReplaceWith(giftCard2).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void ecomm_072_replace_gift_2()
        {
            var giftCard1 = new GiftCard { Number = "5022440000000000098" };
            var giftCard2 = new GiftCard { Number = "5022440000000000007" };

            var response = giftCard2.ReplaceWith(giftCard1).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // DEACTIVATE

        [TestMethod]
        public void ecomm_073_deactivate_gift_1()
        {
            var giftCard1 = new GiftCard { Number = "5022440000000000098" };

            var response = giftCard1.Deactivate().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void ecomm_074_deactivate_gift_2()
        {
            var giftCard2 = new GiftCard { Number = "5022440000000000007" };

            var response = giftCard2.Deactivate().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // RECEIPTS MESSAGING

        // ecomm_075_receipts_messaging: print and scan receipt for test 51

        // CLOSE BATCH

        [TestMethod]
        public void ecomm_999_CloseBatch()
        {
            try
            {
                var response = BatchService.CloseBatch();
                Assert.IsNotNull(response);
                Debug.WriteLine(string.Format("Batch ID: {0}", response.Id));
                Debug.WriteLine(string.Format("Sequence Number: {0}", response.SequenceNumber));
            }
            catch (GatewayException exc)
            {
                if (exc.ResponseMessage != "Transaction was rejected because it requires a batch to be open.")
                    Assert.Fail(exc.Message);
            }
        }
    }
}
