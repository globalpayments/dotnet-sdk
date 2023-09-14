using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPayments.Api.Tests.Portico {
    [TestClass]
    public class PorticoEcommerceTests {
        CreditCardData card;
        private string _token;  

        public PorticoEcommerceTests() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MZ64BQBBoHAA5N2pWWCvZ7c1HTKDM2g_4HsnyC6rIQ",
                IsSafDataSupported = true
            });

            card = TestCards.VisaManual();
            _token = card.Tokenize();
        }

        [TestMethod]
        public void EcomWithMoto() {
            Transaction response = card.Charge(9m)
                .WithCurrency("USD")
                .WithEcommerceInfo(new EcommerceInfo {
                    Channel = EcommerceChannel.MOTO
                })
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EcomWithDirectMarketShipDate() {
            Transaction response = card.Charge(10m)
                .WithCurrency("USD")
                .WithEcommerceInfo(new EcommerceInfo {
                    ShipDay = 25,
                    ShipMonth = 12
                })
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EcomWithDirectMarketInvoiceNoShipDate() {
            Transaction response = card.Charge(10m)
                .WithCurrency("USD")
                .WithEcommerceInfo(new EcommerceInfo {})
                .WithInvoiceNumber("1234567890")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EcomWithDirectMarketDataInvoiceAndShipDate() {

            EcommerceInfo ecom = new EcommerceInfo
            {
                ShipDay = DateTime.Now.AddDays(1).Day,
                ShipMonth = DateTime.Now.AddDays(1).Month
            };

            Transaction response = card.Charge(11m)
                .WithCurrency("USD")
                .WithEcommerceInfo(ecom)
                .WithInvoiceNumber("1234567890")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var item = ReportingService.TransactionDetail(response.TransactionId)
                .Execute();
            Assert.IsNotNull(item);
            Assert.AreEqual(ecom.ShipDay.ToString(), item.ShippingDay.ToString());
            Assert.AreEqual(ecom.ShipMonth.ToString(), item.ShippingMonth.ToString());
        }

        [TestMethod]
        public void EcomWithSecureEcommerceThreeDSecure() {
            card.ThreeDSecure = new ThreeDSecure {
                Cavv = "XXXXf98AAajXbDRg3HSUMAACAAA=",
                Eci = "5",
                Version = Secure3dVersion.One,
                Xid = "0l35fwh1sys3ojzyxelu4ddhmnu5zfke5vst"
            };
            Transaction response = card.Charge(10m)
                .WithCurrency("USD")
                .WithInvoiceNumber("1234567890")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }


        [TestMethod]
        public void EcomWithSecureEcommerceWithoutMobileType() {
            card.ThreeDSecure = new ThreeDSecure {
                PaymentDataSource = PaymentDataSourceType.APPLEPAY, 
                Cavv = "XXXXf98AAajXbDRg3HSUMAACAAA=",
                Eci = "7",
             };           
            Transaction response = card.Charge(10m)
                .WithCurrency("USD")
                .WithInvoiceNumber("1234567890")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EcomSaleWithSecureEcommerceWalletDataWithMobileType() {
           
            card.MobileType = MobilePaymentMethodType.APPLEPAY;
            card.PaymentSource = PaymentDataSourceType.APPLEPAYWEB;
            card.Token = _token;            

            Transaction response = card.Charge(10m)
                    .WithCurrency("USD")
                    .WithInvoiceNumber("1234567890")
                    .WithAllowDuplicates(true)
                    .Execute();
                Assert.IsNotNull(response);
                Assert.AreEqual("00", response.ResponseCode);          
        }

        [TestMethod]
        public void EcomAuthWithSecureEcommerceWalletDataWithMobileType()
        {

            card.MobileType = MobilePaymentMethodType.GOOGLEPAY;
            card.PaymentSource = PaymentDataSourceType.GOOGLEPAYWEB;
            card.Token = "{\"signature\":\"MEQCIE0Y69+lCD/pwrdE1G8Uz7UCVtSzZrFkCZc3cBEESamqAiBgFOk7Ra0TptPDHDnEWYf5fbJL3N6gd/4jZjKdOmbdOA\\u003d\\u003d\",\"protocolVersion\":\"ECv1\",\"signedMessage\":\"{\\\"encryptedMessage\\\":\\\"5RBYQVLQ4mP5EDU8NrGtK4G5b74txt5rjuOULZFyyCQmrMc77iemfzEoiJ6iE0JSliXeLIF8XdUnwU9pIwmlEf31n5+5ix+cIPVLccBKQFegJ38dIz11PG6zdOy9b7a5FkHt86BK3WLyWk5wcVyNInYfI9cM5Et6drujHWwCvr3FFQq53HIkYjW06KBzjuddA7mZHDAv8qnP0wTOnmVBJiBGeVNqNuINEX8WFN8J2ghkd/uoD2zT7CwzIXgx4JwEtDDmFSdV4dSv6MS+K05lYG2NAfvrgFsGDHpQ3T/Vh4mMAiWHJMNxGNvvtvGoEw6iBf4V1sSAygJX/FOR93OHxLNsXPMgZ54ymsh/odzO2j3hJGXdX6wCqbc+0PHILw2/7MDr+NdDcnwRI3ByRDTgQ0RYRB3MaBqzP6v74u/EAzUOfihqxlFT03GpdQGxelHBc1IHoe7lzQ\\\\u003d\\\\u003d\\\",\\\"ephemeralPublicKey\\\":\\\"BBile099xneBNimz2d/KJxw8Qhj6Fe98aeiseCY0ccZzS7pNY2zID5OEzYPKh4anTu32U9H8Dxu2g2oL7JMcD9c\\\\u003d\\\",\\\"tag\\\":\\\"uIrfAI8u37LJ6nlNLY4/XklR5OHktLZatgRjgj+pvNk\\\\u003d\\\"}\"}";

            Transaction response = card.Authorize(10m)
                    .WithCurrency("USD")
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void RefundIncludingEcommInfo() {
            var saleResponse = card.Charge(12.34m)
                .WithCurrency("USD")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);

            var ecomInfo1 = new EcommerceInfo();
            ecomInfo1.Channel = EcommerceChannel.MOTO;

            var tran = Transaction.FromId(saleResponse.TransactionId, PaymentMethodType.Credit);

            var refundResponse = tran.Refund(1.01m)
                .WithEcommerceInfo(ecomInfo1)
                .WithInvoiceNumber("invoice01a")
                .WithCurrency("USD")
                .Execute();
            Assert.IsNotNull(refundResponse);
            Assert.AreEqual("00", refundResponse.ResponseCode);
        }
    }
}
