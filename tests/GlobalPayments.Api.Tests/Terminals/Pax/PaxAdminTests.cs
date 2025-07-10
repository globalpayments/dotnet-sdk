using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.PAX;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace GlobalPayments.Api.Tests.Terminals.Pax {
    [TestClass]
    public class PaxAdminTests {
        IDeviceInterface _device;

        public PaxAdminTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.PAX_DEVICE,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "192.168.1.70",
                Port = "10009",
                RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void Initialize() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]A00[FS]1.35[FS][ETX]"));
            };

            var response = _device.Initialize();
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);
            Assert.IsNotNull(response.SerialNumber);
        }

        [TestMethod]
        public void Cancel() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.AreEqual("[STX]A14[FS]1.35[FS][ETX]^", message);
            };

            _device.Cancel();
        }

        [TestMethod]
        public void SetTipPercentages()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
              
            };

            var response = _device.SetTipPercentageOptions(5,10,15,false);
            Assert.IsNotNull(response);

            var res = _device.Sale(20m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(res);
            Assert.AreEqual("00", res.ResponseCode);

        }
        [TestMethod]
        public void SetTipAmounts()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);

            };

            var response = _device.SetTipAmountOptions(15, 25, 40, true);
            Assert.IsNotNull(response);

            var res = _device.Sale(20m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(res);
            Assert.AreEqual("00", res.ResponseCode);

        }
        [TestMethod]
        public void InputAccount()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);

            };

            var response = _device.InputAccount(true,true,false,true,true,300,0,null,16,19,null,null);
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);
        }
        [TestMethod]
        public void ShowTextBox()
        {
            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);

            };
            var response = _device.ShowTextBox("terminal title","","", ""
                , "", "",null,null,"600", "", ""
                ,null,false,null,SignatureBoxDisplay.NoSignatureBox,true,null
                ,null, "Enter SSN", true,TextInputType.AlphaNumeric,4,30);
            Assert.IsNotNull(response);
            //Assert.AreEqual("OK", response.DeviceResponseText);
            var resp = _device.Reset();
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);
        }
        [TestMethod]
        public void ShowMessage()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);

            };
            var response = _device.ShowMessage("Test message                $13","Test title","",true,"","","","",PaxLineItemAction.Delete,0);
            Assert.IsNotNull(response);
        }
        [TestMethod]
        public void ClearMessage()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);

            };
            var response = _device.ClearMessage();
            Assert.IsNotNull(response);
        }
        [TestMethod]
        public void UploadResourceFile()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]A18[FS]1.35[FS]"));
            };
            bool isHttpConnectionMode = false;
            //the test file only works for pax android devices A35, a920, a920 pro, a77 Aries 8, etc.
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TestData\mt30_ad1.zip");
            byte[] fileData = File.ReadAllBytes(path);
            var response = _device.UpdateResource(UpdateResourceFileType.RESOURCE_FILE, fileData, isHttpConnectionMode);
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);
        }
        [TestMethod]
        public void DeleteResourceFile()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]A22[FS]1.35[FS]"));
            };

            var response = _device.DeleteResource("mt30_ad1.png");
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);
        }
        [TestMethod]
        public void SetSAFMode()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]A54[FS]1.35[FS]"));
            };

            var response = _device.SetStoreAndForwardMode(SafMode.STAY_OFFLINE);
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);
        }
        [TestMethod]
        public void SetSAFModeParams()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]A54[FS]1.35[FS]"));
            };
            string start = null; // DateTime.Now.ToString("yyyyMMddHHmmss");  //  
            string end = null; // DateTime.Now.AddDays(1).ToString("yyyyMMddHHmmss");  // 
            string days = null; //= "1";  //
            string max = null; //= "50"; //
            string totalCeiling = null; //= "200000"; //
            string totalCeilingCardType = null; //= "4000"; //
            string haloPerCard = null; //= "200"; // "      "; //
            string uploadMode = ""; //"1"; //; 
            string autoUploadTime = null; //= "1000"; //
            string deleteSaf = null; //= ""; //

            var response = _device.SetStoreAndForwardMode(SafMode.STAY_OFFLINE, start, end,days, max, totalCeiling, totalCeilingCardType, haloPerCard, uploadMode, autoUploadTime, deleteSaf);
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);
        }
        [TestMethod]
        public void GetSAFModeParams()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]A78[FS]1.35[FS]"));
            };

            var response = _device.GetStoreAndForwardParams();
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);
        }
        [TestMethod]
        public void UploadSaf()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]B08[FS]1.35[FS]"));
            };

            var response = _device.SafUpload(SafIndicator.ALL_TRANSACTIONS);
            Assert.IsNotNull(response);
            Assert.IsTrue("OK" == response.DeviceResponseText || "NOT FOUND" == response.DeviceResponseText);
        }
        [TestMethod]
        public void DeleteSafRecords()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]B10[FS]1.35[FS]"));
            };

            var response = _device.DeleteStoreAndForwardFile(SafIndicator.NEWLY_STORED_TRANSACTIONS);
            Assert.IsNotNull(response);
            Assert.IsTrue("OK" == response.DeviceResponseText || "NOT FOUND" == response.DeviceResponseText);
        }
        [TestMethod]
        public void GetSafSummaryReport()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]R10[FS]1.35[FS]"));
            };

            var response = _device.GetSafSummaryReport(SafIndicator.ALL_TRANSACTIONS);
            Assert.IsNotNull(response);
            Assert.IsTrue("OK" == response.DeviceResponseText || "NOT FOUND" == response.DeviceResponseText);
        }
        [TestMethod]
        public void Reset() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]A16[FS]1.35[FS][ETX]"));
            };

            var response = _device.Reset();
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);
        }

        [TestMethod]
        public void Reboot() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.AreEqual("[STX]A26[FS]1.31[FS][ETX][", message);
            };

            var response = _device.Reboot();
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);
        }

        [TestMethod]
        public void GetSignature() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]A08[FS]1.35[FS]0[FS][ETX]"));
            };

            var response = _device.GetSignatureFile();
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);
        }

        [TestMethod]
        public void PromptForSignature() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                //Assert.IsTrue(message.StartsWith("[STX]A20"));
            };

            var response = _device.PromptForSignature();
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);

            if (response.SigData != null)
            {
                SaveSignatureFile(response.SigData, @"C:\temp\paxsig.txt");
            }
        }

        [TestMethod]
        public void TurnOffBeeps() {
            var response = _device.DisableHostResponseBeep();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void SaveImageTest()
        {
            var response = _device.GetSignatureFile();
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.SignatureData);

            // save as bmp
            //SaveSignatureFile(response.SignatureData, @"C:\temp\signature.bmp");

            // convert and save as png
            var pngData = BmpToPng(response.SignatureData);
            //SaveSignatureFile(pngData, @"C:\temp\signature.png");
        }

        [TestMethod]
        public void BuildSignatureTest()
        {
            //var signatureData = "24,90^24,89^25,88^26,86^29,83^34,77^41,69^49,60^55,53^60,46^63,42^65,39^66,35^66,32^65,28^62,25^58,22^50,21^40,22^31,25^25,28^21,30^18,33^17,34^17,36^17,39^20,41^25,44^32,46^42,47^54,48^65,48^74,48^84,48^93,48^100,49^108,50^114,51^118,53^122,54^125,55^127,57^129,58^130,60^132,62^133,64^134,66^135,68^136,69^136,71^137,72^137,73^136,72^136,71^0,65535^147,13^148,16^149,20^150,27^152,38^154,51^155,64^156,73^156,80^157,85^157,89^157,92^158,94^158,95^159,96^159,94^159,89^0,65535^123,22^124,21^126,20^131,18^138,15^147,12^156,10^165,9^172,10^177,12^180,14^182,16^182,18^181,22^177,26^170,32^159,40^144,50^132,57^125,62^122,64^121,65^122,65^0,65535^186,78^187,78^189,76^191,74^192,72^194,69^196,66^198,64^199,62^199,60^199,58^197,55^194,54^190,53^185,53^180,55^176,58^171,62^168,67^166,71^165,76^165,81^167,85^170,88^174,90^180,90^188,90^195,87^203,84^208,81^212,79^214,76^215,75^216,72^217,70^0,65535^221,57^221,55^222,54^221,54^220,56^217,59^215,63^212,68^210,74^209,78^210,82^212,86^215,88^220,89^224,88^229,85^233,80^238,72^242,62^244,54^244,46^244,39^243,33^243,28^242,25^241,22^240,21^239,21^237,25^236,32^234,40^233,49^233,58^235,64^236,69^238,73^240,76^243,78^245,79^247,79^251,78^254,75^0,65535^260,69^260,71^260,72^261,73^261,74^261,75^262,76^262,74^262,70^262,66^263,61^265,56^266,51^269,47^272,44^275,41^279,40^0,65535^301,64^300,64^298,62^297,61^294,60^290,58^285,58^278,60^273,62^268,65^265,69^263,74^263,79^266,84^269,88^275,90^281,90^288,88^293,84^297,80^300,75^301,71^301,68^301,64^299,60^296,57^292,55^287,55^280,58^0,65535^~";

            //byte[] buffer = TerminalUtilities.BuildSignatureImage(signatureData, 350);
            var response = _device.GetSignatureFile();
            Assert.IsNotNull(response);
            if(response.SigData != null)
            {
                SaveSignatureFile(response.SigData, @"C:\temp\paxsig.txt");
            }
        }

        #region helper methods
        private void SaveSignatureFile(string signatureData, string filename)
        {
            //using (var sw = new BinaryWriter(File.OpenWrite(filename))) {
            //    sw.Write(signatureData);
            //    sw.Flush();
            //}
            File.WriteAllText(filename, signatureData);
        }

        private byte[] BmpToPng(byte[] signatureData) {
            using (var ms = new MemoryStream(signatureData))
            using (var pngStream = new MemoryStream()) {
                //var bmp = new Bitmap(ms);
                //bmp.Save(pngStream, ImageFormat.Png);
                return pngStream.ToArray();
            }
        }
        #endregion
    }
}
