using System;
using System.Diagnostics;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Certifications.Portico
{
    [TestClass]
    public class RetailCertification
    {
        bool useTokens = false;

        string visa_token;
        string mastercard_token;
        string discover_token;
        string amex_token;

        public RetailCertification()
        {
            ServicesContainer.ConfigureService(new PorticoConfig
            {
                SecretApiKey = "skapi_cert_MaePAQBr-1QAqjfckFC8FTbRTT120bVQUlfVOjgCBw"
            });
        }

        [TestMethod]
        public void retail_000_CloseBatch()
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

        /*
            CREDIT CARD FUNCTIONS
            CARD VERIFY
            ACCOUNT VERIFICATION
         */

        [TestMethod]
        public void retail_001_CardVerifyVisa()
        {
            var visa_enc = TestCards.VisaSwipeEncrypted();

            var response = visa_enc.Verify()
                    .WithRequestMultiUseToken(useTokens)
                    .Execute();
            Assert.IsNotNull(response, "response is null");
            Assert.AreEqual("00", response.ResponseCode, response.ResponseMessage);

            if (useTokens)
            {
                Assert.IsNotNull(response.Token, "token is null");

                var token = new CreditCardData
                {
                    Token = response.Token
                };

                var saleResponse = token.Charge(15.01m)
                    .WithAllowDuplicates(true)
                    .Execute();
                Assert.IsNotNull(saleResponse);
                Assert.AreEqual("00", saleResponse.ResponseCode);
            }
        }

        [TestMethod]
        public void retail_002_CardVerifyMastercardSwipe()
        {
            var card_enc = TestCards.MasterCardSwipeEncrypted();

            var response = card_enc.Verify()
                    .WithRequestMultiUseToken(useTokens)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            if (useTokens)
            {
                Assert.IsNotNull(response.Token);

                var token = new CreditCardData
                {
                    Token = response.Token
                };

                var saleResponse = token.Charge(15.02m)
                    .WithAllowDuplicates(true)
                    .Execute();
                Assert.IsNotNull(saleResponse);
                Assert.AreEqual("00", saleResponse.ResponseCode);
            }
        }

        [TestMethod]
        public void retail_003_CardVerifyDiscover()
        {
            var discover_enc = TestCards.DiscoverSwipeEncrypted();
            var response = discover_enc.Verify()
                    .WithRequestMultiUseToken(useTokens)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            if (useTokens)
            {
                Assert.IsNotNull(response.Token);

                var token = new CreditCardData
                {
                    Token = response.Token
                };

                var saleResponse = token.Charge(15.03m)
                    .WithAllowDuplicates(true)
                    .Execute();
                Assert.IsNotNull(saleResponse);
                Assert.AreEqual("00", saleResponse.ResponseCode);
            }
        }

        // Address Verification

        [TestMethod]
        public void retail_004_CardVerifyAmex()
        {
            var address = new Address
            {
                PostalCode = "75024"
            };

            var manual_amex = TestCards.AmexManual(false, true);

            var response = manual_amex.Verify()
                    .WithAddress(address)
                    .WithRequestMultiUseToken(useTokens)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            if (useTokens)
            {
                Assert.IsNotNull(response.Token);

                var token = new CreditCardData
                {
                    Token = response.Token
                };

                var saleResponse = token.Charge(15.04m)
                    .WithAllowDuplicates(true)
                    .Execute();
                Assert.IsNotNull(saleResponse);
                Assert.AreEqual("00", saleResponse.ResponseCode);
            }
        }

        // Balance Inquiry (for Prepaid)

        [TestMethod]
        public void retail_005_BalanceInquiryVisa()
        {
            var visa_enc = TestCards.VisaSwipeEncrypted();

            var response = visa_enc.BalanceInquiry().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // CREDIT SALE (For multi-use token only)

        [TestMethod]
        public void retail_006_ChargeVisaSwipeToken()
        {
            var card = TestCards.VisaSwipe();
            var response = card.Charge(15.01m)
                    .WithCurrency("USD")
                    .WithRequestMultiUseToken(true)
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            visa_token = response.Token;
        }

        [TestMethod]
        public void retail_007_ChargeMastercardSwipeToken()
        {
            var card = TestCards.MasterCardSwipe();
            var response = card.Charge(15.02m)
                    .WithCurrency("USD")
                    .WithRequestMultiUseToken(true)
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            mastercard_token = response.Token;
        }

        [TestMethod]
        public void retail_008_ChargeDiscoverSwipeToken()
        {
            var card = TestCards.DiscoverSwipe();
            var response = card.Charge(15.03m)
                    .WithCurrency("USD")
                    .WithRequestMultiUseToken(true)
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            discover_token = response.Token;
        }

        [TestMethod]
        public void retail_009_ChargeAmexSwipeToken()
        {
            var card = TestCards.AmexSwipe();
            var response = card.Charge(15.04m)
                    .WithCurrency("USD")
                    .WithRequestMultiUseToken(true)
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            amex_token = response.Token;
        }

        /*
            CREDIT SALE
            SWIPED
         */

        [TestMethod]
        public void retail_010_ChargeVisaSwipe()
        {
            var card = TestCards.VisaSwipe();
            var response = card.Charge(15.01m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // test 59
            var reverse = response.Reverse(15.01m).Execute();
            Assert.IsNotNull(reverse);
            Assert.AreEqual("00", reverse.ResponseCode);
        }

        [TestMethod]
        public void retail_011_ChargeMastercardSwipe()
        {
            var card = TestCards.MasterCardSwipe();
            var response = card.Charge(15.02m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_012_ChargeDiscoverSwipe()
        {
            var card = TestCards.DiscoverSwipe();
            var response = card.Charge(15.03m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_013_ChargeAmexSwipe()
        {
            var card = TestCards.AmexSwipe();
            var response = card.Charge(15.04m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_014_ChargeJbcSwipe()
        {
            var card = TestCards.JcbSwipe();
            var response = card.Charge(15.05m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // test case 58
            var refund = response.Refund(15.05m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(refund);
            Assert.AreEqual("00", refund.ResponseCode);
        }

        #region Retail card
        [TestMethod]
        public void retail_014a_ChargeRetailMastercard24()
        {
            var card = TestCards.MasterCard24Swipe();
            var response = card.Charge(15.34m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_014b_ChargeRetailMastercard25()
        {
            var card = TestCards.MasterCard25Swipe();
            var response = card.Charge(15.34m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
        #endregion

        [TestMethod]
        public void retail_015_ChargeVisaSwipe()
        {
            var card = TestCards.VisaSwipe();
            var response = card.Charge(15.06m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // test case 63
            var reversal = response.Reverse(15.06m).WithAuthAmount(5.06m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Manually Entered - Card Present

        [TestMethod]
        public void retail_016_ChargeVisaManualCardPresent()
        {
            var address = new Address
            {
                PostalCode = "750241234",
                StreetAddress1 = "6860 Dallas Pkwy"
            };

            var manual_card = TestCards.VisaManual(true, true);
            var response = manual_card.Charge(16.01m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_017_ChargeMasterCardManualCardPresent()
        {
            var address = new Address
            {
                PostalCode = "75024",
                StreetAddress1 = "6860 Dallas Pkwy"
            };

            var manual_card = TestCards.MasterCardManual(true, true);
            var response = manual_card.Charge(16.02m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // test case 60
            var reverse = response.Reverse(16.02m).Execute();
            Assert.IsNotNull(reverse);
            Assert.AreEqual("00", reverse.ResponseCode);
        }

        [TestMethod]
        public void retail_018_ChargeDiscoverManualCardPresent()
        {
            var address = new Address
            {
                PostalCode = "750241234"
            };

            var manual_card = TestCards.DiscoverManual(true, true);
            var response = manual_card.Charge(16.03m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_019_ChargeAmexManualCardPresent()
        {
            var address = new Address
            {
                PostalCode = "75024",
                StreetAddress1 = "6860"
            };

            var manual_card = TestCards.AmexManual(true, true);
            var response = manual_card.Charge(16.04m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_020_ChargeJcbManualCardPresent()
        {
            var address = new Address
            {
                PostalCode = "75024"
            };

            var manual_card = TestCards.JcbManual(true, true);
            var response = manual_card.Charge(16.05m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_021_ChargeDiscoverManualCardPresent()
        {
            var address = new Address
            {
                PostalCode = "750241234",
                StreetAddress1 = "6860 Dallas Pkwy"
            };

            var manual_card = TestCards.DiscoverManual(true, true);
            var response = manual_card.Charge(16.07m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // test case 64
            var reversal = response.Reverse(16.07m)
                    .WithAuthAmount(6.07m)
                    .Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("00", reversal.ResponseCode);
        }

        // Manually Entered - Card Not Present

        [TestMethod]
        public void retail_022_ChargeVisaManualCardNotPresent()
        {
            var address = new Address
            {
                PostalCode = "750241234",
                StreetAddress1 = "6860 Dallas Pkwy"
            };

            var manual_card = useTokens ? new CreditCardData { Token = visa_token } : TestCards.VisaManual(false, true);

            var response = manual_card.Charge(17.01m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_023_ChargeMasterCardManualCardNotPresent()
        {
            var address = new Address
            {
                PostalCode = "75024",
                StreetAddress1 = "6860 Dallas Pkwy"
            };

            var manual_card = useTokens ? new CreditCardData { Token = mastercard_token } : TestCards.MasterCardManual(false, true);

            var response = manual_card.Charge(17.02m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // test case 61
            var reversal = response.Reverse(17.02m).Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("00", reversal.ResponseCode);
        }

        [TestMethod]
        public void retail_024_ChargeDiscoverManualCardNotPresent()
        {
            var address = new Address
            {
                PostalCode = "750241234"
            };

            var manual_card = useTokens ? new CreditCardData { Token = mastercard_token } : TestCards.DiscoverManual(false, true);

            var response = manual_card.Charge(17.03m)
                    .WithCurrency("USD").WithAddress(address).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_025_ChargeAmexManualCardNotPresent()
        {
            var address = new Address
            {
                PostalCode = "75024",
                StreetAddress1 = "6860"
            };

            var manual_card = useTokens ? new CreditCardData { Token = amex_token } : TestCards.AmexManual(false, true);

            var response = manual_card.Charge(17.04m)
                    .WithCurrency("USD").WithAddress(address).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_026_ChargeJcbManualCardNotPresent()
        {
            var address = new Address
            {
                PostalCode = "75024"
            };

            var manual_card = TestCards.JcbManual(false, true);
            var response = manual_card.Charge(17.05m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Contactless

        [TestMethod]
        public void retail_027_ChargeVisaContactless()
        {
            var card = TestCards.VisaSwipe(EntryMethod.Proximity);
            var response = card.Charge(18.01m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_028_ChargeMastercardContactless()
        {
            var card = TestCards.MasterCardSwipe(EntryMethod.Proximity);

            var response = card.Charge(18.02m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_029_ChargeDiscoverContactless()
        {
            var card = TestCards.DiscoverSwipe(EntryMethod.Proximity);

            var response = card.Charge(18.03m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_030_ChargeAmexContactless()
        {
            var card = TestCards.AmexSwipe(EntryMethod.Proximity);

            var response = card.Charge(18.04m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // AUTHORIZATION

        [TestMethod]
        public void retail_031_AuthorizeVisaSwipe()
        {
            var card = TestCards.VisaSwipe();

            // 031a authorize
            var response = card.Authorize(15.08m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // 031b capture
            var captureResponse = response.Capture().Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_032_AuthorizeVisaSwipeAdditionalAuth()
        {
            var card = TestCards.VisaSwipe();

            // 032a authorize
            var response = card.Authorize(15.09m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // 032b Additional Auth (restaurant only)

            // 032c Add to batch
            var captureResponse = response.Capture().Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_033_AuthorizeMasterCardSwipe()
        {
            var card = TestCards.MasterCardSwipe();

            // 033a authorize
            var response = card.Authorize(15.10m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // 033b capture
            var captureResponse = response.Capture().Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_033a_AuthorizeDiscoverSwipe()
        {
            var card = TestCards.DiscoverSwipe();

            var response = card.Authorize(15.10m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // AUTHORIZATION - Manually Entered, Card Present

        [TestMethod]
        public void retail_034_AuthorizeVisaManualCardPresent()
        {
            var address = new Address
            {
                PostalCode = "75024",
                StreetAddress1 = "6860 Dallas Pkwy"
            };

            var card = TestCards.VisaManual(true, true);

            // 034a authorize
            var response = card.Authorize(16.08m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // 034b capture
            var captureResponse = response.Capture().Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_035_AuthorizeVisaManualCardPresentAdditionalAuth()
        {
            var address = new Address
            {
                PostalCode = "75024",
                StreetAddress1 = "6860 Dallas Pkwy"
            };

            var card = TestCards.VisaManual(true, true);

            // 035a authorize
            var response = card.Authorize(16.09m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // 035b Additional Auth (restaurant only)

            // 035c Add to batch
            var captureResponse = response.Capture().Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_036_AuthorizeMasterCardManualCardPresent()
        {
            var address = new Address
            {
                PostalCode = "75024",
                StreetAddress1 = "6860 Dallas Pkwy"
            };

            var card = TestCards.MasterCardManual(true, true);

            // 036a authorize
            var response = card.Authorize(16.10m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // 036b capture
            var captureResponse = response.Capture().Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_036a_AuthorizeDiscoverManualCardPresent()
        {
            var address = new Address
            {
                PostalCode = "750241234"
            };

            var card = TestCards.DiscoverManual(true, true);
            var response = card.Authorize(16.10m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // AUTHORIZATION - Manually Entered, Card Not Present

        [TestMethod]
        public void retail_037_AuthorizeVisaManual()
        {
            var address = new Address
            {
                PostalCode = "750241234",
                StreetAddress1 = "6860 Dallas Pkwy"
            };

            var card = TestCards.VisaManual(false, true);

            // 034a authorize
            var response = card.Authorize(17.08m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // 034b capture
            var captureResponse = response.Capture().Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_038_AuthorizeMasterCardManual()
        {
            var address = new Address
            {
                PostalCode = "75024",
                StreetAddress1 = "6860"
            };

            var card = TestCards.MasterCardManual(false, true);

            // 036a authorize
            var response = card.Authorize(17.09m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // 036b capture
            var captureResponse = response.Capture().Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_038a_AuthorizeDiscoverManual()
        {
            var address = new Address
            {
                PostalCode = "750241234"
            };

            var card = TestCards.DiscoverManual(false, true);

            var response = card.Authorize(17.10m)
                .WithCurrency("USD")
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // PARTIALLY APPROVED SALE (Required)

        [TestMethod]
        public void retail_039_ChargeDiscoverSwipePartialApproval()
        {
            var card = TestCards.DiscoverSwipe();

            var response = card.Charge(40.00m)
                    .WithCurrency("USD")
                    .WithAllowPartialAuth(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(40.00m, response.AuthorizedAmount);
        }

        [TestMethod]
        public void retail_040_ChargeVisaSwipePartialApproval()
        {
            var card = TestCards.VisaSwipe();
            var response = card.Charge(130.00m)
                    .WithCurrency("USD")
                    .WithAllowPartialAuth(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(110.00m, response.AuthorizedAmount);
        }

        [TestMethod]
        public void retail_041_ChargeDiscoverManualPartialApproval()
        {
            var address = new Address
            {
                PostalCode = "75024"
            };
            var card = TestCards.DiscoverManual(true, true);

            var response = card.Charge(145.00m)
                    .WithCurrency("USD")
                    .WithAllowPartialAuth(true)
                    .WithAddress(address)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(65.00m, response.AuthorizedAmount);
        }

        [TestMethod, Ignore]
        public void retail_042_ChargeMasterCardSwipePartialApproval()
        {
            var card = TestCards.MasterCardSwipe();
            var response = card.Charge(155.00m)
                    .WithCurrency("USD")
                    .WithAllowPartialAuth(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(100.00m, response.AuthorizedAmount);

            // test case 62
            var reversal = response.Reverse(100.00m).Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("00", reversal.ResponseCode);
        }

        /*
            SALE WITH GRATUITY
            Tip Edit (Tip at Settlement)
         */

        [TestMethod]
        public void retail_043_ChargeVisaSwipeEditGratuity()
        {
            var card = TestCards.VisaSwipe();
            var response = card.Charge(15.12m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var editResponse = response.Edit()
                .WithAmount(18.12m)
                .WithGratuity(3.00m)
                .Execute();
            Assert.IsNotNull(editResponse);
            Assert.AreEqual("00", editResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_044_ChargeMasterCardManualEditGratuity()
        {
            var address = new Address
            {
                PostalCode = "75024"
            };

            var card = TestCards.MasterCardManual(true, true);
            var response = card.Charge(15.13m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var editResponse = response.Edit()
                .WithAmount(18.13m)
                .WithGratuity(3.00m)
                .Execute();
            Assert.IsNotNull(editResponse);
            Assert.AreEqual("00", editResponse.ResponseCode);
        }

        // Tip on Purchase

        [TestMethod]
        public void retail_045_ChargeVisaManualGratuity()
        {
            var address = new Address
            {
                PostalCode = "75024"
            };

            var card = TestCards.VisaManual(true, true);

            var response = card.Charge(18.61m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .WithGratuity(3.50m)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_046_ChargeMasterCardSwipeGratuity()
        {
            var card = TestCards.MasterCardSwipe();

            var response = card.Charge(18.62m)
                    .WithCurrency("USD")
                    .WithGratuity(3.50m)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var editResponse = response.Edit()
                    .WithAmount(18.12m)
                    .WithGratuity(3.00m)
                    .Execute();
            Assert.IsNotNull(editResponse);
            Assert.AreEqual("00", editResponse.ResponseCode);
        }

        // LEVEL II CORPORATE PURCHASE CARD

        [TestMethod]
        public void retail_047_LevelIIVisaSwipeResponseB()
        {
            var card = TestCards.VisaSwipe();

            var response = card.Charge(112.34m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("B", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX)
            {
                TaxAmount = 1m
            };

            var cpcResponse = response.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod, Ignore]
        public void retail_047a_LevelIIVisaSwipeResonseB()
        {
            var card = TestCards.VisaSwipe();

            var response = card.Charge(112.35m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("B", response.CommercialIndicator);

            var cpcResponse = response.Edit()
                .WithCommercialData(new CommercialData(TaxType.NOTUSED))
                .Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_048_LevelIIVisaSwipeResponseR()
        {
            var card = TestCards.VisaSwipe();

            var response = card.Charge(123.45m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("R", response.CommercialIndicator);

            var cpcResponse = response.Edit()
                .WithCommercialData(new CommercialData(TaxType.TAXEXEMPT))
                .Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_049_LevelIIVisaManualResponseS()
        {
            var address = new Address
            {
                PostalCode = "75024"
            };

            var card = TestCards.VisaManual(true, true);

            var response = card.Charge(134.56m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX)
            {
                PoNumber = "9876543210",
                TaxAmount = 1m
            };

            var cpcResponse = response.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_050_LevelIIMasterCardSwipeResponseS()
        {
            var card = TestCards.MasterCardSwipe();

            var response = card.Charge(111.06m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.NOTUSED)
            {
                PoNumber = "9876543210"
            };

            var cpcResponse = response.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_051_LevelIIMasterCardManualResponseS()
        {
            var address = new Address
            {
                PostalCode = "75024"
            };

            var card = TestCards.MasterCardManual(true, true);

            var response = card.Charge(111.07m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX)
            {
                PoNumber = "9876543210",
                TaxAmount = 1m
            };

            var cpcResponse = response.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_051a_LevelIIMasterCardManualResponseS()
        {
            var address = new Address
            {
                PostalCode = "75024"
            };

            var card = TestCards.MasterCardManual(true, true);
            var response = card.Charge(111.08m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX)
            {
                PoNumber = "9876543210",
                TaxAmount = 1m
            };

            var cpcResponse = response.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_052_LevelIIMasterCardManualResponseS()
        {
            var address = new Address
            {
                PostalCode = "75024"
            };

            var card = TestCards.MasterCardManual(true, true);
            var response = card.Charge(111.09m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.TAXEXEMPT)
            {
                PoNumber = "9876543210"
            };

            var cpcResponse = response.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_053_LevelIIAmexSwipeNoResponse()
        {
            var card = TestCards.AmexSwipe();
            var response = card.Charge(111.10m)
                    .WithCurrency("USD")
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("0", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.SALESTAX)
            {
                TaxAmount = 1m
            };

            var cpcResponse = response.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_054_LevelIIAmexManualNoResponse()
        {
            var address = new Address
            {
                PostalCode = "75024"
            };

            var card = TestCards.AmexManual(true, true);

            var response = card.Charge(111.11m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("0", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.NOTUSED)
            {
                PoNumber = "9876543210"
            };

            var cpcResponse = response.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_055_LevelIIAmexManualNoResponse()
        {
            var address = new Address
            {
                PostalCode = "75024"
            };

            var card = TestCards.AmexManual(true, true);
            var response = card.Charge(111.12m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("0", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.NOTUSED)
            {
                PoNumber = "9876543210"
            };

            var cpcResponse = response.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_055a_LevelIIAmexManualNoResponse()
        {
            var address = new Address
            {
                PostalCode = "75024"
            };

            var card = TestCards.AmexManual(true, true);
            var response = card.Charge(111.13m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .WithCommercialRequest(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("0", response.CommercialIndicator);

            var commercialData = new CommercialData(TaxType.TAXEXEMPT)
            {
                PoNumber = "9876543210"
            };

            var cpcResponse = response.Edit()
                .WithCommercialData(commercialData)
                .Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        // OFFLINE SALE / AUTHORIZATION

        [TestMethod]
        public void retail_056_OfflineChargeVisaManual()
        {
            var card = TestCards.VisaManual(false, true);

            var response = card.Charge(15.12m)
                    .WithCurrency("USD")
                    .WithOfflineAuthCode("654321")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_056_OfflineAuthVisaManual()
        {
            var card = TestCards.VisaManual(false, true);

            var response = card.Authorize(15.11m)
                    .WithCurrency("USD")
                    .WithOfflineAuthCode("654321")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // RETURN

        [TestMethod]
        public void retail_057_ReturnMasterCard()
        {
            var card = TestCards.MasterCardManual(false, true);

            var response = card.Refund(15.11m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_057a_ReturnMasterCardSwipe()
        {
            var card = TestCards.MasterCardSwipe();
            var response = card.Refund(15.15m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // ONLINE VOID / REVERSAL (Required)

        // PIN DEBIT CARD FUNCTIONS

        [TestMethod]
        public void retail_065_DebitSaleVisaSwipe()
        {
            var card = TestCards.VisaSwipe().AsDebit("32539F50C245A6A93D123412324000AA");

            var response = card.Charge(14.01m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_066_DebitSaleMasterCardSwipe()
        {
            var card = TestCards.MasterCardSwipe().AsDebit("F505AD81659AA42A3D123412324000AB");

            var response = card.Charge(14.02m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // test case 71
            var reversal = card.Reverse(14.02m).Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("00", reversal.ResponseCode);
        }

        [TestMethod]
        public void retail_067_DebitSaleVisaSwipeCashBack()
        {
            var card = TestCards.VisaSwipe().AsDebit("32539F50C245A6A93D123412324000AA");

            var response = card.Charge(14.03m)
                    .WithCurrency("USD")
                    .WithCashBack(5.00m)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_067a_DebitSaleMasterCard()
        {
            var card = TestCards.MasterCardSwipe().AsDebit("F505AD81659AA42A3D123412324000AB");

            var response = card.Charge(14.04m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // PARTIALLY APPROVED PURCHASE

        [TestMethod]
        public void retail_068_DebitSaleMasterCardPartialApproval()
        {
            var card = TestCards.MasterCardSwipe().AsDebit("F505AD81659AA42A3D123412324000AB");

            var response = card.Charge(33.00m)
                    .WithCurrency("USD")
                    .WithAllowPartialAuth(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(22.00m, response.AuthorizedAmount);
        }

        [TestMethod]
        public void retail_069_DebitSaleVisaPartialApproval()
        {
            var card = TestCards.VisaSwipe().AsDebit("32539F50C245A6A93D123412324000AA");

            var response = card.Charge(44.00m)
                    .WithCurrency("USD")
                    .WithAllowPartialAuth(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(33.00m, response.AuthorizedAmount);

            // test case 72
            var reversal = card.Reverse(33.00m).Execute();
            Assert.IsNotNull(reversal);
            Assert.AreEqual("00", reversal.ResponseCode);
        }

        // RETURN

        [TestMethod]
        public void retail_070_DebitReturnVisaSwipe()
        {
            var card = TestCards.VisaSwipe().AsDebit("32539F50C245A6A93D123412324000AA");

            var response = card.Refund(14.07m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_070a_DebitReturnVisaSwipe()
        {
            var card = TestCards.VisaSwipe().AsDebit("32539F50C245A6A93D123412324000AA");

            var response = card.Refund(14.08m)
                    .WithCurrency("USD")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var reversalResponse = card.Reverse(14.08m).Execute();
            Assert.IsNotNull(reversalResponse);
            Assert.AreEqual("00", reversalResponse.ResponseCode);
        }

        // REVERSAL

        /*
           EBT FUNCTIONS
            Food Stamp Purchase
         */

        [TestMethod]
        public void retail_080_EbtfsPurchaseVisaSwipe()
        {
            var card = TestCards.VisaSwipe().AsEBT("32539F50C245A6A93D123412324000AA");

            var response = card.Charge(101.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_081_EbtfsPurchaseVisaManual()
        {
            var card = TestCards.VisaManual(false, true).AsEBT("32539F50C245A6A93D123412324000AA");

            var response = card.Charge(102.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Food Stamp Electronic Voucher (Manual Entry Only)

        [TestMethod]
        public void retail_082_EbtVoucherPurchaseVisa()
        {
            var card = TestCards.VisaManual(false, true).AsEBT("32539F50C245A6A93D123412324000AA");
            card.SerialNumber = "123456789012345";
            card.ApprovalCode = "123456";

            var response = card.Charge(103.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Food Stamp Balance Inquiry

        [TestMethod]
        public void retail_083_EbtfsReturnVisaSwipe()
        {
            var card = TestCards.VisaSwipeEncrypted().AsEBT("32539F50C245A6A93D123412324000AA");

            var response = card.Refund(104.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_084_EbtfsReturnVisaManual()
        {
            var card = TestCards.VisaManual(false, true).AsEBT("32539F50C245A6A93D123412324000AA");

            var response = card.Refund(105.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Food Stamp Balance Inquiry

        [TestMethod]
        public void retail_085_EbtBalanceInquiryVisaSwipe()
        {
            var card = TestCards.VisaSwipeEncrypted().AsEBT("32539F50C245A6A93D123412324000AA");

            var response = card.BalanceInquiry().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_086_EbtBalanceInquiryVisaManual()
        {
            var card = TestCards.VisaManual(true, true).AsEBT("32539F50C245A6A93D123412324000AA");

            var response = card.BalanceInquiry().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /*
            Assert.AreEqual("00", response.ResponseCode);
            EBT CASH BENEFITS
            Cash Back Purchase
         */

        [TestMethod]
        public void retail_087_EbtCashBackPurchaseVisaSwipe()
        {
            var card = TestCards.VisaSwipeEncrypted().AsEBT("32539F50C245A6A93D123412324000AA");

            var response = card.Charge(106.01m)
                    .WithCurrency("USD")
                    .WithCashBack(5.00m)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_088_EbtCashBackPurchaseVisaManual()
        {
            var card = TestCards.VisaManual(false, true).AsEBT("32539F50C245A6A93D123412324000AA");

            var response = card.Charge(107.01m)
                    .WithCurrency("USD")
                    .WithCashBack(5.00m)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // No Cash Back Purchase

        [TestMethod]
        public void retail_089_EbtCashBackPurchaseVisaSwipeNoCashBack()
        {
            var card = TestCards.VisaSwipeEncrypted().AsEBT("32539F50C245A6A93D123412324000AA");

            var response = card.Charge(108.01m)
                .WithCurrency("USD")
                .WithCashBack(0m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_090_EbtCashBackPurchaseVisaManualNoCashBack()
        {
            var card = TestCards.VisaManual(false, true).AsEBT("32539F50C245A6A93D123412324000AA");

            var response = card.Charge(109.01m)
                    .WithCurrency("USD")
                    .WithCashBack(0m)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Cash Back Balance Inquiry

        [TestMethod]
        public void retail_091_EbtBalanceInquiryVisaSwipeCash()
        {
            var card = TestCards.VisaSwipeEncrypted().AsEBT("32539F50C245A6A93D123412324000AA");

            var response = card.BalanceInquiry(InquiryType.CASH).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_092_EbtBalanceInquiryVisaManualCash()
        {
            var card = TestCards.VisaManual(true, true).AsEBT("32539F50C245A6A93D123412324000AA");

            var response = card.BalanceInquiry(InquiryType.CASH).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Bash Benefits Withdrawal

        [TestMethod]
        public void retail_093_EbtBenefitWithDrawalVisaSwipe()
        {
            var card = TestCards.VisaSwipeEncrypted().AsEBT("32539F50C245A6A93D123412324000AA");

            var response = card.Charge(110.01m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_094_EbtBenefitWithDrawalVisaManual()
        {
            var card = TestCards.VisaManual(false, true).AsEBT("32539F50C245A6A93D123412324000AA");

            var response = card.Charge(111.01m)
                .WithCurrency("USD")
                .WithCashBack(0m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /*
            HMS GIFT - REWARDS
            GIFT
            ACTIVATE
         */

        [TestMethod]
        public void retail_095_ActivateGift1Swipe()
        {
            var card = TestCards.GiftCard1Swipe();

            var response = card.Activate(6.00m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_096_ActivateGift2Manual()
        {
            var card = TestCards.GiftCard2Manual();

            var response = card.Activate(7.00m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // ADD VALUE

        [TestMethod]
        public void retail_097_AddValueGift1Swipe()
        {
            var card = TestCards.GiftCard1Swipe();

            var response = card.AddValue(8.00m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_098_AddValueGift2Manual()
        {
            var card = TestCards.GiftCard2Manual();

            var response = card.Activate(9.00m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // BALANCE INQUIRY

        [TestMethod]
        public void retail_099_BalanceInquiryGift1Swipe()
        {
            var card = TestCards.GiftCard1Swipe();

            var response = card.BalanceInquiry().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(10.00m, response.BalanceAmount);
        }

        [TestMethod]
        public void retail_100_BalanceInquiryGift2Manual()
        {
            var card = TestCards.GiftCard2Manual();

            var response = card.BalanceInquiry().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(10.00m, response.BalanceAmount);
        }

        // REPLACE / TRANSFER

        [TestMethod]
        public void retail_101_ReplaceGift1Swipe()
        {
            var oldCard = TestCards.GiftCard1Swipe();
            var newCard = TestCards.GiftCard2Manual();

            var response = oldCard.ReplaceWith(newCard).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_102_ReplaceGift2Manual()
        {
            var newCard = TestCards.GiftCard1Swipe();
            var oldCard = TestCards.GiftCard2Manual();

            var response = oldCard.ReplaceWith(newCard).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // SALE / REDEEM

        [TestMethod]
        public void retail_103_SaleGift1Swipe()
        {
            var card = TestCards.GiftCard1Swipe();

            var response = card.Charge(1.00m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_104_SaleGift2Manual()
        {
            var card = TestCards.GiftCard2Manual();

            var response = card.Charge(2.00m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_105_SaleGift1VoidSwipe()
        {
            var card = TestCards.GiftCard1Swipe();

            var response = card.Charge(3.00m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // test case 107
            var voidResponse = response.Void().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_106_SaleGift2ReversalManual()
        {
            var card = TestCards.GiftCard2Manual();

            var response = card.Charge(4.00m)
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            //test case 108
            var voidResponse = response.Reverse(4.00m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // VOID

        // REVERSAL

        // DEACTIVATE

        [TestMethod]
        public void retail_109_DeactivateGift1()
        {
            var card = TestCards.GiftCard1Swipe();

            var response = card.Deactivate().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // RECEIPTS MESSAGING

        [TestMethod]
        public void retail_110_ReceiptsMessaging()
        {
            // PRINT AND SCAN RECEIPT FOR TEST 107
        }

        /*
            REWARDS
            BALANCE INQUIRY
         */

        [TestMethod]
        public void retail_111_BalanceInquiryRewards1()
        {
            var card = TestCards.GiftCard1Swipe();

            var response = card.BalanceInquiry().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(0m, response.PointsBalanceAmount);
        }

        [TestMethod]
        public void retail_112_BalanceInquiryRewards2()
        {
            var card = TestCards.GiftCard2Manual();

            var response = card.BalanceInquiry().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(0m, response.PointsBalanceAmount);
        }

        // ALIAS

        [TestMethod]
        public void retail_113_CreateAliasGift1()
        {
            var card = GiftCard.Create("9725550100");
            Assert.IsNotNull(card);
        }

        [TestMethod]
        public void retail_114_CreateAliasGift2()
        {
            var card = GiftCard.Create("9725550100");
            Assert.IsNotNull(card);
        }

        [TestMethod]
        public void retail_115_AddAliasGift1()
        {
            var card = TestCards.GiftCard1Swipe();

            var response = card.AddAlias("2145550199").Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_116_AddAliasGift2()
        {
            var card = TestCards.GiftCard2Manual();

            var response = card.AddAlias("2145550199").Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_117_DeleteAliasGift1()
        {
            var card = TestCards.GiftCard1Swipe();

            var response = card.RemoveAlias("2145550199").Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_999_CloseBatch()
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
