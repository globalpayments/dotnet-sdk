using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class GpApiPayerTest : BaseGpApiTests
    {
        private Customer newCustomer;
        private CreditCardData card;
        private Address billingAddress;
        private Address shippingAddress;

        [TestInitialize]
        public void TestInitialize()
        {
            ServicesContainer.RemoveConfig();

            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);

            newCustomer = new Customer();
            newCustomer.Key = GenerationUtils.GenerateOrderId();
            newCustomer.FirstName = "James";
            newCustomer.LastName = "Mason";

            card = new CreditCardData();
            card.Number = "4263970000005262";
            card.ExpMonth = ExpMonth;
            card.ExpYear = ExpYear;
            card.Cvn = "131";
            card.CardHolderName = "James Mason";

            // billing address
            billingAddress = new Address();
            billingAddress.StreetAddress1 = "10 Glenlake Pkwy NE";
            billingAddress.StreetAddress2 = "no";
            billingAddress.City = "Birmingham";
            billingAddress.PostalCode = "50001";
            billingAddress.CountryCode = "US";
            billingAddress.State = "IL";

            // shipping address
            shippingAddress = new Address();
            shippingAddress.StreetAddress1 = "Apartment 852";
            shippingAddress.StreetAddress2 = "Complex 741";
            shippingAddress.StreetAddress3 = "no";
            shippingAddress.City = "Birmingham";
            shippingAddress.PostalCode = "50001";
            shippingAddress.State = "IL";
            shippingAddress.CountryCode = "US";
        }

        [TestMethod]
        public void CreatePayer()
        {
            var tokenizeResponse = card.Tokenize();
            card.Token = tokenizeResponse;

            newCustomer.AddPaymentMethod(tokenizeResponse, card);

            var card2 = new CreditCardData();
            card2.Number = "4012001038488884";
            card2.ExpMonth = ExpMonth;
            card2.ExpYear = ExpYear;
            card2.Cvn = "131";
            card2.CardHolderName = "James Mason";

            var tokenize2 = card2.Tokenize();
            card2.Token = tokenize2;

            Assert.IsNotNull(tokenize2);

            newCustomer.AddPaymentMethod(card2.Token, card2);

            var payer = newCustomer.Create();

            Assert.IsNotNull(payer.Id);
            Assert.AreEqual(newCustomer.FirstName, payer.FirstName);
            Assert.AreEqual(newCustomer.LastName, payer.LastName);
            Assert.IsNotNull(payer.PaymentMethods);
            foreach (var paymentMethod in payer.PaymentMethods) {
                Assert.IsTrue((new string[] { card2.Token, card.Token }).ToList().Contains(paymentMethod.Id));
            }
        }
        
        [TestMethod]
        public void CreatePayer_WithoutPaymentMethods()
        {
            var payer = newCustomer.Create();

            Assert.IsNotNull(payer.Id);
            Assert.AreEqual(newCustomer.FirstName, payer.FirstName);
            Assert.AreEqual(newCustomer.LastName, payer.LastName);
            Assert.IsNull(payer.PaymentMethods);
        }
        
        [TestMethod]
        public void CreatePayer_WithoutFirstName()
        {
            newCustomer = new Customer();
            newCustomer.Key = GenerationUtils.GenerateOrderId();
            newCustomer.LastName = "Mason";
            
            var exceptionCaught = false;
            try {
                newCustomer.Create();
            } catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields: first_name", e.Message);
                Assert.AreEqual("40005", e.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        [TestMethod]
        public void CreatePayer_WithoutLastName()
        {
            newCustomer = new Customer();
            newCustomer.Key = GenerationUtils.GenerateOrderId();
            newCustomer.FirstName = "James";
            
            var exceptionCaught = false;
            try {
                newCustomer.Create();
            } catch (GatewayException e) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadRequest - Request expects the following fields: last_name", e.Message);
                Assert.AreEqual("40005", e.ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void EditPayer()
        {
            var key = "payer-123";
            var id = "PYR_df7aebe8e356430caf1a3f3b5a8eef71";
            newCustomer.Key = key;
            newCustomer.Id = id;

            var tokenizeResponse = card.Tokenize();

            Assert.IsNotNull(tokenizeResponse);


            card.Token = tokenizeResponse;
            newCustomer.AddPaymentMethod(tokenizeResponse, card);
            
            var payer = newCustomer.SaveChanges();

            Assert.AreEqual(newCustomer.Key, payer.Key);

            Assert.IsTrue(payer.PaymentMethods.Count > 0);
            Assert.AreEqual(card.Token, payer.PaymentMethods[0].Id);
        }
        
        [TestMethod]
        public void EditPayer_WithoutCustomerId()
        {
            var key = "payer-123";
            newCustomer.Key = key;
            
            var tokenizeResponse = card.Tokenize();
            Assert.IsNotNull(tokenizeResponse);

            card.Token = tokenizeResponse;
            newCustomer.AddPaymentMethod(tokenizeResponse, card);
            
            var exceptionCaught = false;
            try {
                newCustomer.SaveChanges();
            } catch (ApiException e) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: BadGateway - Unable to process your request due to an error with a system down stream.", e.InnerException.Message);
                Assert.AreEqual("50046", (e.InnerException as GatewayException).ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        [TestMethod]
        public void EditPayer_WithoutRandomId()
        {
            var key = "payer-123";
            var id = "PYR_" + Guid.NewGuid();
            newCustomer.Key = key;
            newCustomer.Id = id;
            
            var tokenizeResponse = card.Tokenize();
            Assert.IsNotNull(tokenizeResponse);

            card.Token = tokenizeResponse;
            newCustomer.AddPaymentMethod(tokenizeResponse, card);
            
            var exceptionCaught = false;
            try {
                newCustomer.SaveChanges();
            } catch (ApiException e) {
                exceptionCaught = true;
                Assert.AreEqual("Status Code: NotFound - Payer "+ newCustomer.Id +" not found at this location", e.InnerException.Message);
                Assert.AreEqual("40008", (e.InnerException as GatewayException).ResponseMessage);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void BNPLInitiateStep()
        {
            newCustomer.Email = "james@example.com";
            newCustomer.Phone = new PhoneNumber() { CountryCode = "41", Number= "57774873", AreaCode = PhoneNumberType.Home.ToString() };
            newCustomer.Key = "12345678";
            var payer = newCustomer.Create();

            var product = new Product();
            product.ProductId = GenerationUtils.GenerateOrderId();
            product.ProductName = "iPhone 13";
            product.Description = "iPhone 13";
            product.Quantity = 1;
            product.UnitPrice = 550;
            product.NetUnitPrice = 550;
            product.TaxAmount = 0;
            product.DiscountAmount = 0;
            product.TaxPercentage = 0;
            product.Url = "https://www.example.com/iphone.html";
            product.ImageUrl = "https://www.example.com/iphone.png";

            List<Product> products = new List<Product>();
            products.Add(product);

            var paymentMethod = new BNPL();
            paymentMethod.BNPLType = BNPLType.AFFIRM;
            paymentMethod.ReturnUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net";
            paymentMethod.StatusUpdateUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net";
            paymentMethod.CancelUrl = "https://7b8e82a17ac00346e91e984f42a2a5fb.m.pipedream.net";

            var transaction = paymentMethod.Authorize(5.6m)
                .WithCurrency("USD")
                .WithMiscProductData(products)
                .WithAddress(shippingAddress, AddressType.Shipping)
                .WithAddress(billingAddress, AddressType.Billing)
                .WithPhoneNumber("41", "57774873", PhoneNumberType.Shipping)
                .WithCustomerData(payer)
                .WithBNPLShippingMethod(BNPLShippingMethod.DELIVERY)
                .WithOrderId("12365")
                .Execute();

            Assert.IsNotNull(transaction);
            Assert.AreEqual("SUCCESS", transaction.ResponseCode);
            Assert.AreEqual(TransactionStatus.Initiated.ToString().ToUpper(), transaction.ResponseMessage);
            Assert.IsNotNull(transaction.BNPLResponse.RedirectUrl);
        }
    }
}
