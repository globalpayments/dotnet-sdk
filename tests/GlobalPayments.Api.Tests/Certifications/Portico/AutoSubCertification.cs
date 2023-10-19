using System.Diagnostics;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Certifications.Portico
{
    [TestClass]
    public class AutoSubCertification
    {
        public AutoSubCertification()
        {
            ServicesContainer.ConfigureService(new PorticoConfig
            {
                SecretApiKey = "skapi_cert_MaePAQBr-1QAqjfckFC8FTbRTT120bVQUlfVOjgCBw"
            }, "retail");

            ServicesContainer.ConfigureService(new PorticoConfig
            {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
            }, "ecomm");
        }

        [TestMethod]
        public void Retail_000_CloseBatch()
        {
            ServicesContainer.ConfigureService(new PorticoConfig
            {
                SecretApiKey = "skapi_cert_MaePAQBr-1QAqjfckFC8FTbRTT120bVQUlfVOjgCBw"
            });

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

        // Address Verification

        [TestMethod]
        public void Retail_003_CardVerifyAmex()
        {
            var address = new Address
            {
                PostalCode = "75024"
            };
            var manual_amex = TestCards.AmexManual(true, false);

            var response = manual_amex.Verify()
                    .WithAddress(address)
                    .WithRequestMultiUseToken(true)
                    .Execute("retail");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Retail_007_CardVerifyVisa()
        {
            var visa_enc = TestCards.VisaSwipeEncrypted();

            var response = visa_enc.Verify()
                    .WithRequestMultiUseToken(true)
                    .Execute("retail");
            Assert.IsNotNull(response, "response is null");
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);

            var token = new CreditCardData
            {
                Token = response.Token
            };

            var saleResponse = token.Charge(25m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute("retail");
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
        }

        [TestMethod]
        public void Retail_008a_CardVerifyMastercardSwipe()
        {
            var card_enc = TestCards.MasterCardSwipeEncrypted();

            var response = card_enc.Verify()
                    .WithRequestMultiUseToken(true)
                    .Execute("retail");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var token = new CreditCardData
            {
                Token = response.Token
            };

            var saleResponse = token.Charge(26m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute("retail");
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
        }

        [TestMethod]
        public void Retail_008b_CardVerifyMastercardSwipe()
        {
            var card_enc = TestCards.MasterCard24Swipe();
            var response = card_enc.Verify()
                    .WithRequestMultiUseToken(true)
                    .Execute("retail");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var token = new CreditCardData
            {
                Token = response.Token
            };

            var saleResponse = token.Charge(26m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute("retail");
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);
        }

        // Manually Entered - Card Present

        [TestMethod]
        public void Retail_009_ChargeVisaManualCardPresent()
        {
            var address = new Address
            {
                PostalCode = "750241234",
                StreetAddress1 = "6860 Dallas Pkwy",
            };
            var manual_card = TestCards.VisaManual(true, true);

            var response = manual_card.Charge(27m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute("retail");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Retail_010_ChargeMasterCardManualCardPresent()
        {
            var address = new Address
            {
                PostalCode = "75024",
                StreetAddress1 = "6860 Dallas Pkwy"
            };
            var manual_card = TestCards.MasterCardManual(true, true);

            var response = manual_card.Charge(28m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute("retail");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // CREDIT SALE Swiped Transactions

        [TestMethod]
        public void Retail_011_ChargeVisaSwipe()
        {
            var card = TestCards.VisaSwipe();
            var autoSub = new AutoSubstantiation
            {
                PrescriptionSubTotal = 29
            };

            var response = card.Charge(29m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithAutoSubstantiation(autoSub)
                    .Execute("retail");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Retail_012_ChargeVisaSwipe()
        {
            var card = TestCards.VisaSwipe();
            var autoSub = new AutoSubstantiation
            {
                VisionSubTotal = 21
            };

            var response = card.Charge(21m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithAutoSubstantiation(autoSub)
                    .Execute("retail");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Retail_013_ChargeVisaSwipe()
        {
            var card = TestCards.VisaSwipe();
            var autoSub = new AutoSubstantiation
            {
                ClinicSubTotal = 21,
                DentalSubTotal = 10
            };

            var response = card.Charge(31m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithAutoSubstantiation(autoSub)
                    .Execute("retail");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Retail_014_ChargeVisaSwipe()
        {
            var card = TestCards.VisaSwipe();
            var autoSub = new AutoSubstantiation
            {
                PrescriptionSubTotal = 22,
                VisionSubTotal = 10
            };

            var response = card.Charge(32m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithAutoSubstantiation(autoSub)
                    .Execute("retail");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Retail_015_ChargeMastercardSwipe()
        {
            var card = TestCards.MasterCardSwipe();
            var autoSub = new AutoSubstantiation
            {
                PrescriptionSubTotal = 33
            };

            var response = card.Charge(33m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithAutoSubstantiation(autoSub)
                    .Execute("retail");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // test 27
            var reversal = card.Reverse(33.00m).Execute("retail");
            Assert.IsNotNull(reversal);
            Assert.AreEqual("00", reversal.ResponseCode);
        }

        [TestMethod]
        public void Retail_016_ChargeMastercardSwipe()
        {
            var card = TestCards.MasterCardSwipe();
            var autoSub = new AutoSubstantiation
            {
                VisionSubTotal = 24
            };

            var response = card.Charge(24m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithAutoSubstantiation(autoSub)
                    .Execute("retail");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Retail_017_ChargeMastercardSwipe()
        {
            var card = TestCards.MasterCardSwipe();
            var autoSub = new AutoSubstantiation
            {
                ClinicSubTotal = 32,
                DentalSubTotal = 10
            };

            var response = card.Charge(42m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithAutoSubstantiation(autoSub)
                    .Execute("retail");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Retail_018_ChargeMastercardSwipe()
        {
            var card = TestCards.MasterCardSwipe();
            var autoSub = new AutoSubstantiation
            {
                PrescriptionSubTotal = 26,
                VisionSubTotal = 10
            };

            var response = card.Charge(36m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .WithAutoSubstantiation(autoSub)
                    .Execute("retail");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Retail_018b_ChargeMastercardSwipe()
        {
            var card = TestCards.MasterCard25Swipe();

            var response = card.Charge(11.50m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute("retail");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        //Partially Approved

        [TestMethod]
        public void Retail_022_ChargeDiscoverSwipePartialApproval()
        {
            var card = TestCards.DiscoverSwipe();

            var response = card.Charge(130.00m)
                    .WithCurrency("USD")
                    .WithAllowPartialAuth(true)
                    .Execute("retail");
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(110.00m, response.AuthorizedAmount);

            // test case 28
            var reversal = response.Reverse(110.00m).Execute("retail");
            Assert.IsNotNull(reversal);
            Assert.AreEqual("00", reversal.ResponseCode);
        }

        [TestMethod]
        public void Retail_023_ChargeMasterManualPartialApproval()
        {
            var address = new Address
            {
                PostalCode = "75024"
            };
            var card = TestCards.MasterCardManual(true, true);

            var response = card.Charge(155.00m)
                    .WithCurrency("USD")
                    .WithAllowPartialAuth(true)
                    .WithAddress(address)
                    .Execute("retail");
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(100.00m, response.AuthorizedAmount);
        }

        [TestMethod]
        public void Retail_001_CloseBatch()
        {
            ServicesContainer.ConfigureService(new PorticoConfig
            {
                SecretApiKey = "skapi_cert_MaePAQBr-1QAqjfckFC8FTbRTT120bVQUlfVOjgCBw",
                ServiceUrl = "https://cert.api2.heartlandportico.com"
            });

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
        public void Ecom_000_CloseBatch()
        {
            ServicesContainer.ConfigureService(new PorticoConfig
            {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A",
                ServiceUrl = "https://cert.api2.heartlandportico.com"
            });

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

        // StreetAddress Verification

        [TestMethod]
        public void Ecomm_003_Verify_Amex()
        {
            var address = new Address
            {
                PostalCode = "75024"
            };
            var card = TestCards.AmexManual();

            var response = card.Verify()
                .WithAddress(address)
                .WithRequestMultiUseToken(true)
                .Execute("ecomm");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Ecomm_007_Verify_Visa()
        {
            var address = new Address { StreetAddress1 = "6860 Dallas Pkwy", PostalCode = "75024" };
            var manual_card = TestCards.VisaManual();

            var response = manual_card.Verify()
                .WithRequestMultiUseToken(true)
                .Execute("ecomm");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var token = new CreditCardData
            {
                Token = response.Token
            };

            var saleResponse = token.Charge(25m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .Execute("ecomm");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Ecomm_08a_Verify_Master_Card()
        {
            var address = new Address { StreetAddress1 = "6860 Dallas Pkwy", PostalCode = "75024" };
            var manual_card = TestCards.MasterCardManual();

            var response = manual_card.Verify()
                .WithRequestMultiUseToken(true)
                .Execute("ecomm");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var token = new CreditCardData
            {
                Token = response.Token
            };

            var chargeResponse = token.Charge(26m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .Execute("ecomm");
            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        [TestMethod]
        public void Ecomm_08b_Verify_Master_Card()
        {
            var address = new Address { StreetAddress1 = "6860 Dallas Pkwy", PostalCode = "75024" };
            var manual_card = TestCards.MasterCardSeries2Manual();

            var response = manual_card.Verify()
                .WithRequestMultiUseToken(true)
                .Execute("ecomm");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var token = new CreditCardData
            {
                Token = response.Token
            };

            var chargeResponse = token.Charge(26m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithInvoiceNumber("123456")
                .WithAllowDuplicates(true)
                .Execute("ecomm");
            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("00", chargeResponse.ResponseCode);
        }

        [TestMethod]
        public void Ecomm_009_Charge_Visa()
        {
            var address = new Address { StreetAddress1 = "6860 Dallas Pkwy", PostalCode = "75024" };
            var card = TestCards.VisaManual();

            var response = card.Charge(27m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .Execute("ecomm");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Ecomm_010_Charge_Master_Card()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };
            var card = TestCards.MasterCardManual();

            var response = card.Charge(28m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .Execute("ecomm");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Ecomm_010b_Charge_Master_Card_2Manual()
        {
            var card = TestCards.MasterCardSeries2Manual();

            var response = card.Charge(28m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute("ecomm");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Keyed transactions

        [TestMethod]
        public void Ecomm_011_Charge_Visa()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };
            var autoSub = new AutoSubstantiation
            {
                PrescriptionSubTotal = 29
            };
            var card = TestCards.VisaManual();

            var response = card.Charge(29m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .WithAutoSubstantiation(autoSub)
                .Execute("ecomm");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Ecomm_012_Charge_Visa()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };
            var autoSub = new AutoSubstantiation
            {
                VisionSubTotal = 21
            };
            var card = TestCards.VisaManual();

            var response = card.Charge(21m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .WithAutoSubstantiation(autoSub)
                .Execute("ecomm");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Ecomm_013_Charge_Visa()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };
            var autoSub = new AutoSubstantiation
            {
                ClinicSubTotal = 21,
                DentalSubTotal = 10
            };
            var card = TestCards.VisaManual();

            var response = card.Charge(31m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .WithAutoSubstantiation(autoSub)
                .Execute("ecomm");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Ecomm_014_Charge_Visa()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };
            var autoSub = new AutoSubstantiation
            {
                PrescriptionSubTotal = 22,
                VisionSubTotal = 10
            };
            var card = TestCards.VisaManual();

            var response = card.Charge(32m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .WithAutoSubstantiation(autoSub)
                .Execute("ecomm");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Ecomm_015_Charge_Master_Card()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };
            var autoSub = new AutoSubstantiation
            {
                PrescriptionSubTotal = 33
            };
            var card = TestCards.MasterCardManual();

            var response = card.Charge(33m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .WithAutoSubstantiation(autoSub)
                .Execute("ecomm");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // test case 27
            var voidResponse = response.Void().Execute("ecomm");
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        [TestMethod]
        public void Ecomm_016_Charge_Master_Card()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };
            var autoSub = new AutoSubstantiation
            {
                VisionSubTotal = 24
            };
            var card = TestCards.MasterCardManual();

            var response = card.Charge(24m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .WithAutoSubstantiation(autoSub)
                .Execute("ecomm");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Ecomm_017_Charge_Master_Card()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };
            var autoSub = new AutoSubstantiation
            {
                ClinicSubTotal = 32,
                DentalSubTotal = 10
            };
            var card = TestCards.MasterCardManual();

            var response = card.Charge(42m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .WithAutoSubstantiation(autoSub)
                .Execute("ecomm");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Ecomm_018_Charge_Master_Card()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };
            var autoSub = new AutoSubstantiation
            {
                PrescriptionSubTotal = 26,
                VisionSubTotal = 10
            };
            var card = TestCards.MasterCardManual();

            var response = card.Charge(36m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .WithAutoSubstantiation(autoSub)
                .Execute("ecomm");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Partially Approved Sale

        [TestMethod]
        public void Ecomm_022_Partial_Approval_Discover()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };
            var card = TestCards.DiscoverManual();

            var response = card.Charge(130m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithAllowPartialAuth(true)
                .Execute("ecomm");
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(110m, response.AuthorizedAmount);

            // test case 28
            var reverseResponse = response.Reverse(110m).Execute("ecomm");
            Assert.IsNotNull(reverseResponse);
            Assert.AreEqual("00", reverseResponse.ResponseCode);
        }

        [TestMethod]
        public void Ecomm_023_Partial_Approval_Master_Card()
        {
            var address = new Address { StreetAddress1 = "6860", PostalCode = "75024" };
            var card = TestCards.MasterCardManual();

            var chargeResponse = card.Charge(145m)
                .WithCurrency("USD")
                .WithAddress(address)
                .WithAllowPartialAuth(true)
                .Execute("ecomm");
            Assert.IsNotNull(chargeResponse);
            Assert.AreEqual("10", chargeResponse.ResponseCode);
            Assert.AreEqual(65.00m, chargeResponse.AuthorizedAmount);
        }

        [TestMethod]
        public void Ecom_001_CloseBatch()
        {
            ServicesContainer.ConfigureService(new PorticoConfig
            {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
            });

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
