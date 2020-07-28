using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.Pax {
    [TestClass]
    public class PaxLevel2Tests {
        IDeviceInterface _device;

        public PaxLevel2Tests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.PAX_S300,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "10.12.220.172",
                Port = "10009",
                RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(_device);
        }

        // PoNumber
        [TestMethod]
        public void CheckPoNumber() {
            _device.OnMessageSent += (message) => {
                //Assert.IsTrue(message.StartsWith("[STX]T00[FS]1.35[FS]01[FS]1000[FS][FS]1[FS][FS][FS]123456789[FS][FS][ETX]"));
            };

            var response = _device.Sale(10m)
                .WithPoNumber("123456789")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // CustomerCode
        [TestMethod]
        public void CheckCustomerCode() {
            _device.OnMessageSent += (message) => {
                //Assert.IsTrue(message.StartsWith("[STX]T00[FS]1.35[FS]01[FS]1100[US][US][US][US]122[FS][FS]1[FS][FS][FS][US]123456789[FS][FS][ETX]"));
            };

            var response = _device.Sale(11m)
                .WithCustomerCode("123456789")
                .WithTaxAmount(1.22m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // TaxExempt
        [TestMethod]
        public void CheckTaxExcemptTrue() {
            _device.OnMessageSent += (message) => {
                //Assert.IsTrue(message.StartsWith("[STX]T00[FS]1.35[FS]01[FS]1200[FS][FS]1[FS][FS][FS][US]123456789[US]1[FS][FS][ETX]"));
            };

            var response = _device.Sale(12m)
                .WithCustomerCode("123456789")
                .WithTaxType(TaxType.TAXEXEMPT)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void CheckTaxExcemptFalse() {
            _device.OnMessageSent += (message) => {
                //Assert.IsTrue(message.StartsWith("[STX]T00[FS]1.35[FS]01[FS]1300[US][US][US][US]122[FS][FS]1[FS][FS][FS][US]987654321[US]0[FS][FS][ETX]"));
            };

            var response = _device.Sale(13m)
                .WithTaxAmount(1.22m)
                .WithCustomerCode("987654321")
                .WithTaxType(TaxType.SALESTAX)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // TaxExemptId
        [TestMethod]
        public void CheckTaxExemptId() {
            _device.OnMessageSent += (message) => {
                //Assert.IsTrue(message.StartsWith("[STX]T00[FS]1.35[FS]01[FS]1400[FS][FS]1[FS][FS][FS][US]987654321[US]1[US]987654321[FS][FS][ETX]"));
            };

            var response = _device.Sale(14m)
                .WithCustomerCode("987654321")
                .WithTaxType(TaxType.TAXEXEMPT, "987654321")
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // All fields
        [TestMethod]
        public void CheckAllFields() {
            _device.OnMessageSent += (message) => {
                //Assert.IsTrue(message.StartsWith("[STX]T00[FS]1.35[FS]01[FS]1500[FS][FS]1[FS][FS][FS]123456789[US]8675309[US]1[US]987654321[FS][FS][ETX]"));
            };

            var response = _device.Sale(15m)
                .WithPoNumber("123456789")
                .WithCustomerCode("8675309")
                .WithTaxType(TaxType.TAXEXEMPT, "987654321").Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
    }
}
