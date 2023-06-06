using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Environment = GlobalPayments.Api.Entities.Environment;
using System.Collections.Generic;
using System.Text;
using GlobalPayments.Api.Utils.Logging;

namespace GlobalPayments.Api.Tests.ProPay
{
    [TestClass]
    public class ProPayInNetworkTransactionTests {
        private PayFacService _service;

        public ProPayInNetworkTransactionTests() {
            _service = new PayFacService();
            ServicesContainer.ConfigureService(new PorticoConfig()
            {
                CertificationStr = "ba7988a23134c54b95e55aee761351",
                TerminalID = "761351",
                Environment = Environment.TEST,
                EnableLogging = true,
                RequestLogger = new RequestFileLogger(@"C:\temp\profac\requestlog.txt"),
                X509CertificatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ProPay\TestData\testCertificate.crt"),
                //X509CertificateBase64String = "MIICpDCCAYygAwIBAgIIS7Y5fijJytIwDQYJKoZIhvcNAQENBQAwETEPMA0GA1UEAwwGUFJPUEFZMB4XDTE5MDkxOTAwMDAwMFoXDTI5MDkxOTAwMDAwMFowEzERMA8GA1UEAwwIMTI3LjAuMDEwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCCwvq2ho43oeeGX3L9+2aD7bna7qjdLwWumeIpwhPZLa44MeQ5100wy4W2hKk3pOb5yaHqyhzoHDriveQnq/EpZJk9m7sizXsxZtBHtt+wghSZjdNhnon3R54SH5J7oEPybRSAKXSEzHjN+kCu7W3TmXSLve6YuODnjUpbOcAsHG2wE+zpCoEbe8toH5Tt7g8HzEc5mJYkkILTq6j9pwDE50r2NVbV3SXwmQ1ifxf54Z9EFB5bQv5cI3+GL/VwlQeJdiKMGj1rs8zTR8TjbAjVlJbz6bBkFItUsqexgwAHIJZAaU7an8ZamGRlPjf6dp3mOEu4B47igNj5KOSgCNdRAgMBAAEwDQYJKoZIhvcNAQENBQADggEBAF88u367yrduqd3PfEIo2ClaI2QPRIIWKKACMcZDl3z1BzVzNFOZNG2vLcSuKnGRH89tJPCjyxdJa0RyDTkXMSLqb5FgUseEjmj3ULAvFqLZNW35PY9mmlmCY+S3CC/bQR4iyPLo8lsRq0Nl6hlvB440+9zS8UQjtc2957QgcXfD427UJb698gXzsfQcNeaQWy8pNm7FzDfHTJbo/t6FOpmfR+RMZky9FrlWabInkrkf3w2XJL0uUAYU9jGQa+l/vnZD2KNzs1mO1EqkS6yB/fsn85mkgGe4Vfbo9GQ/S+KmDujewFA0ma7O03fy1W5v6Amn/nAcFTCddVL3BDNEtOM=",
                ProPayUS = true
            });
        }

        [TestMethod]
        public void DisburseFunds() {
            var response = _service.DisburseFunds() // This method in the ProPay API requires a different, special CertificationStr value from a disbursement account
                .WithReceivingAccountNumber("718134204")
                .WithAmount("100")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void SpendBackTransaction() {
            var response = _service.SpendBack()
                .WithAccountNumber("718037672")
                .WithReceivingAccountNumber("718134204")
                .WithAmount("100")
                .WithAllowPending(false)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void ReverseSplitPay() {
            var response = _service.ReverseSplitPay()
                .WithAccountNumber("718580393")//"718037672")
                .WithAmount("10")
                .WithCCAmount("0")
                .WithRequireCCRefund(false)
                .WithTransNum("7")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.PayFacData.Amount);
            Assert.IsNotNull(response.PayFacData.RecAccountNum);
            Assert.IsNotNull(response.PayFacData.TransNum);
        }

        [TestMethod]
        public void SplitFunds() {
            var response = _service.SplitFunds()
                .WithAccountNumber("718580391")
                .WithReceivingAccountNumber("718580393")
                .WithAmount("100")
                //.WithTransNum("4")
                //.WithGatewayTransactionId("2814958054")
                .WithCardBrandTransactionId("SURN3DP8G1103")
                .WithGlobaltransId("719D277C-C36D-4886-B79F-E54BA919CAB8")
                .WithGlobalTransSource("portico")
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
    }
}
