﻿using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.Pax {
    [TestClass]
    public class PaxBatchTests {
        IDeviceInterface _device;

        public PaxBatchTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                    DeviceType = DeviceType.PAX_DEVICE,
                    ConnectionMode = ConnectionModes.TCP_IP,
                    IpAddress = "192.168.1.30",
                    Port = "10009",
                    RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void BatchClose() {
            var response = _device.BatchClose();
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);
        }
    }
}
