using System;
using System.IO;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Tests.FileProcessing;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class FileProcessingTest : BaseGpApiTests {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ServicesContainer.RemoveConfig();
            var gpApiConfig = GpApiConfigSetup("fWkEqBHQNyLrWCAtp1vCWDbo10kf5jr6", "EkOH93AQKuGlj8Ty", Channel.CardPresent);
            gpApiConfig.StatusUrl = "https://eo9faqlbl8wkwmx.m.pipedream.net/";
            ServicesContainer.ConfigureService(gpApiConfig);
        }

        [TestMethod]
        public void CreateUploadUrl() {
            FileProcessor response = new FileProcessingService().Initiate();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual("INITIATED", response.ResponseMessage);
            Assert.IsNotNull(response.UploadUrl);

            var client = new FileProcessingClient(response.UploadUrl);
            var fileName = @"FileProcessing\FilesToUpload\202310261147-Create-Token-Tokenization-FPR-1000records.csv.encrypted.txt";
            var fullPath = Path.GetFullPath(fileName);          
            var result = client.UploadFile(fullPath);
            Assert.IsTrue(result);

            FileProcessor fp = new FileProcessingService().GetDetails(response.ResourceId);

            Assert.AreEqual("SUCCESS", fp.ResponseCode);
            Assert.AreEqual("INITIATED", fp.Status);
        }

        [TestMethod]
        public void GetFileUploadDetails() {
            string resourceId = "FPR_971edc6eb0944d8d890dcba7a2a41bea";
            FileProcessor response = new FileProcessingService().GetDetails(resourceId);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual("COMPLETED", response.Status);
            Assert.IsTrue(int.Parse(response.TotalRecordCount) > 0);
            Assert.IsNotNull(response.FilesUploaded[0].Url);
        }

        [TestMethod]
        public void GetFileUploadDetails_WithoutResourceId() {
            Boolean exceptionError = false;
            try {
                FileProcessor response = new FileProcessingService().GetDetails(null);
            }
            catch (BuilderException ex) {
                Assert.AreEqual("ResourceId cannot be null for this transaction type.", ex.Message);
                exceptionError = true;
            }
            finally {
                Assert.IsTrue(exceptionError);
            }
        }
    }
}