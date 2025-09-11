using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.UPA;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Enums;
using GlobalPayments.Api.Terminals.UPA;
using GlobalPayments.Api.Terminals.UPA.Responses;
using GlobalPayments.Api.Utils.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace GlobalPayments.Api.Tests.Terminals.UPA {
    [TestClass]
    public class UpaAdminTests {
        IDeviceInterface _device;

        public UpaAdminTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.UPA_DEVICE,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "192.168.1.13",
                Port = "8081",
                Timeout = 15000,
                RequestIdProvider = new RandomIdProvider(),
                LogManagementProvider = new RequestConsoleLogger()
            });
            Assert.IsNotNull(_device);
        }

        [TestMethod]
        public void LineItem() {
            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "12";
            var response = _device.LineItem("11");
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);

            // Clear the device UI
            _device.ReturnToIdle();
        }

        [TestMethod]
        public void Ping() {
            var sendPings = 10;
            _device.OnMessageSent += (message) => {
                Debug.WriteLine($"Ping attempt {sendPings}");
                Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} {message}");
            };
            _device.OnMessageReceived += (message) => {
                Assert.IsNotNull(message);
                Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} {message}");
            };

            _device.EcrId = "12";
            do
            {
                try
                {
                    var response = _device.Ping();
                    Assert.IsNotNull(response);
                    Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
                    Assert.AreEqual("00", response.DeviceResponseCode);
                    Assert.AreEqual("Success", response.Status);
                    Debug.WriteLine("Ping Success");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                Thread.Sleep(3000);
            }while (--sendPings > 0);
        }

        [TestMethod]
        public void Restart() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            _device.EcrId = "1";
            var response = _device.Reset();
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void GetAppInfo() {
            _device.EcrId = "13";
            var response = _device.GetAppInfo();
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
            Assert.IsNotNull(((UPAResponseHandler)response).DeviceSerialNum);
            Assert.IsNotNull(((UPAResponseHandler)response).AppVersion);
            Assert.IsNotNull(((UPAResponseHandler)response).OsVersion);
            Assert.IsNotNull(((UPAResponseHandler)response).EmvSdkVersion);
            Assert.IsNotNull(((UPAResponseHandler)response).CTLSSdkVersion);
        }

        [TestMethod]
        public void GetConfigContents() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.GetConfigContents(TerminalConfigType.ContactlessTerminalConfiguration);

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
            Assert.IsNotNull(((UPAResponseHandler)response).ConfigContent.FileContent);
            Assert.IsNotNull(((UPAResponseHandler)response).ConfigContent.ConfigType);
            Assert.AreEqual(((UPAResponseHandler)response).ConfigContent.Length, ((UPAResponseHandler)response).ConfigContent.FileContent.Length);
        }

        [TestMethod]
        public void GetSignatureFile() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.GetSignatureFile();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void ClearDataLake() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.ClearDataLake();
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void SetTimeZone() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            _device.EcrId = "13";
            var response = _device.SetTimeZone("America/Los_Angeles");

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void SetParams() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            _device.EcrId = "13";
            var param = new KeyValuePair<string, string>("timeZone", "America/NewYork");
            var response = _device.SetParam(param, "993883", true);

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void SetParamsMissingPasswordField() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            _device.EcrId = "13";
            var param = new KeyValuePair<string, string>("TerminalNumber", "7708");
            var response = _device.SetParam(param);

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("ERR013", response.DeviceResponseCode);
            Assert.AreEqual("[password]", response.DeviceResponseText.Substring(0, 10));
        }

        [TestMethod]
        public void GetParams() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} {message}");
            };
            _device.OnMessageReceived += (message) => {
                Assert.IsNotNull(message);
                Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} {message}");
            };
            _device.EcrId = "13";
            try {
                var response = _device.GetParams(new string[] { "TimeZone", "ApplicationMode", "InvoicePromptSupported", "EODProcessingPWD", "VoidPassword", "TrainingModePWD", "InvoicePromptSupported", "BeepVolume", "HostPassword", "AdminPassword", "UserPassword", "ManagerPassword", "DeviceCapabilities", "DeviceAttributes", "DeveloperID", "ConnectionRetryAttempts", "ApplicationMode", "ApplicationId", "ApiKeySupported", "TerminalLanguage", "PinBypassIsSupported" });
                Assert.IsNotNull(response);
                Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
                Assert.AreEqual("00", response.DeviceResponseCode);
                Assert.AreEqual("Success", response.Status);
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void SetDebugLevel() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.SetDebugLevel(new Enum[] { DebugLevel.PACKETS, DebugLevel.DATA }, DebugLogsOutput.FILE);

            Thread.Sleep(25000);

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void GetDebugLevel() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.GetDebugLevel();
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void GetDebugInfo() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.GetDebugInfo(LogFile.Debuglog1);
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void BroadcastConfiguration() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.BroadcastConfiguration(false);
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void ReturnToIdle() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.ReturnToIdle();
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void LoadUDDataFile() {
            UDData udData = new UDData();
            udData.FileType = UDFileType.HTML5;
            udData.SlotNum = 2;
            udData.FileName = "PIA.html";

            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.LoadUDData(udData);
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void RemoveUDDataFile() {
            UDData udData = new UDData();
            udData.FileType = UDFileType.HTML5;
            udData.SlotNum = 1;

            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.RemoveUDData(udData);
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void ExecuteUdDataFile() {
            UDData udData = new UDData();
            udData.FileType = UDFileType.HTML5;
            udData.SlotNum = 2;
            udData.FileName = "PIA.html";

            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.ExecuteUDDataFile(udData);
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void InjectUdDataFile() {
            UDData udData = new UDData();
            udData.FileType = UDFileType.HTML5;
            udData.FileName = "example.html";
            udData.FilePath = $@"{System.IO.Directory.GetCurrentDirectory()}\Terminals\UPA\FileExamples\UDDataFile.html";


            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.InjectUDDataFile(udData);

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void Scan() {
            ScanData scanData = new ScanData();
            scanData.Header = "scan";
            scanData.Prompt1 = "scan qr code";
            scanData.DisplayOption = DisplayOption.NoScreenChange;
            scanData.TimeOut = 26;

            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.Scan(scanData);
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void ScanWithoutParams() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.Scan();
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void PrintData() {
            PrintData printData = new PrintData();
            printData.FilePath = $@"{System.IO.Directory.GetCurrentDirectory()}\Terminals\UPA\FileExamples\example.jpg";
            printData.Line1 = "Printing...";
            printData.Line2 = "Please Wait...";
            printData.DisplayOption = DisplayOption.NoScreenChange;

            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.Print(printData);
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void CommunicationCheck() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.CommunicationCheck();
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void Logon() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.Logon();
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void GetLastEOD() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.GetLastEOD();
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void RemoveCard() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.RemoveCard("en");
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void EnterPin() {
            var prompt = new PromptMessages();
            prompt.Prompt1 = "Enter PIN";
            prompt.Prompt2 = "EnterPin2";
            prompt.Prompt3 = "EnterPin3";
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.EnterPin(prompt, true, "1234567890123456");
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(IDeviceResponse));
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void PromptWithOptions() {
            var promptData = new PromptData();
            promptData.Prompts = new PromptMessages();
            promptData.Prompts.Prompt1 = "Prompt 1";
            promptData.Prompts.Prompt2 = "Prompt 2";
            promptData.Prompts.Prompt3 = "Prompt 3";
            var button1 = new Button();
            button1.Text = "Yes";
            button1.Color = "green";
            var button2 = new Button();
            button2.Text = "No";
            button2.Color = "red";
            var button3 = new Button();
            button3.Text = "Cancel";
            button3.Color = "blue";
            promptData.Buttons = new System.Collections.Generic.List<Button>() { button1, button2, button3 };

            _device.EcrId = "1";
            var response = _device.Prompt(PromptType.OPTIONS, promptData);

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
            Assert.IsNotNull(((UPAResponseHandler)response).ButtonPressed);
        }

        [TestMethod]
        public void PromptMenu() {
            var promptData = new PromptData();
            promptData.Prompts = new PromptMessages();
            promptData.Prompts.Prompt1 = "Select Application";

            var button1 = new Button();
            button1.Text = "Yes";
            button1.Color = "green";
            var button2 = new Button();
            button2.Text = "No";
            button2.Color = "red";
            var button3 = new Button();
            button3.Text = "Cancel";
            button3.Color = "blue";
            promptData.Buttons = new System.Collections.Generic.List<Button>() { button1, button2, button3 };
            promptData.Menu = new System.Collections.Generic.List<string>() { "Visa", "Mastercard" };

            _device.EcrId = "1";
            var response = _device.Prompt(PromptType.MENU, promptData);

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
            Assert.IsNotNull(((UPAResponseHandler)response).PromptMenuSelected);
        }

        [TestMethod]
        public void GeneralEntry() {
            var data = new GenericData();
            data.Prompts = new PromptMessages();
            data.Prompts.Prompt1 = "Enter Driver’s License";
            data.TextButton1 = "Cancel";
            data.TextButton2 = "OK";
            data.Timeout = 60;
            data.EntryFormat = new System.Collections.Generic.List<TextFormat>() { TextFormat.Password, TextFormat.Alphanumeric };
            data.EntryMinLen = 10;
            data.EntryMaxLen = 20;
            data.Alignment = InputAlignment.RIGHT_TO_LEFT;

            _device.EcrId = "1";
            var response = _device.GetGenericEntry(data);

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void DisplayMessage() {
            var messageLines = new MessageLines();
            messageLines.Line1 = "Please wait...";
            messageLines.Timeout = 0;

            _device.EcrId = "1";
            var response = _device.DisplayMessage(messageLines);

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void ReturnDefaultScreen() {
            _device.EcrId = "1";
            var response = _device.ReturnDefaultScreen(DisplayOption.ReturnToIdleScreen);

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void GetEncryptionType() {
            _device.EcrId = "1";
            var response = _device.GetEncryptionType();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void ContinueEMVTransaction() {
            var response = _device.ContinueTransaction(10.01m, true)
                .WithCashBack(0.1m)
                .WithQuickChip(false)
                .WithMerchantDecision(MerchantDecision.APPROVED)
                .WithLanguage("EN")
                .Execute();

            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
            Assert.IsNotNull(((UPAResponseHandler)response).EmvTags);
        }

        [TestMethod]
        public void CompleteEMVTransaction() {
            HostData hostData = new HostData();
            hostData.HostDecision = HostDecision.APPROVED;
            hostData.IssuerAuthData = "xxx";
            hostData.IssuerScripts = "aaa";
            var response = _device.CompleteTransaction()
                .WithQuickChip(false)
                .WithLanguage("EN")
                .WithHostData(hostData)
                .Execute();

            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
            Assert.IsNotNull(((UPAResponseHandler)response).EmvTags);
        }

        [TestMethod]
        public void ProcessCardTransactionSale() {
            var response = _device.ProcessTransaction(10.01m)
                .WithAcquisitionTypes(new System.Collections.Generic.List<AcquisitionType> { AcquisitionType.Contactless, AcquisitionType.Contact })
                .WithTimeout(3)
                .WithMerchantDecision(MerchantDecision.APPROVED)
                .WithQuickChip(true)
                .WithCheckLuhn(false)
                .WithCashBack(0.1m)
                .WithTransactionDate(DateTime.Now)
                .Execute();

            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void ContinueCardTransaction() {
            var response = _device.ContinueTransaction(10.01m)
                .WithCashBack(0.1m)
                .WithMerchantDecision(MerchantDecision.APPROVED)
                .WithLanguage("EN")
                .Execute();

            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
            Assert.IsNotNull(((UPAResponseHandler)response).EmvTags);
        }

        [TestMethod]
        public void LineItemWithRight() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "12";
            var response = _device.LineItem("Toothpaste", "12");
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);

            // Clear the device UI
            _device.Cancel();
        }

        [TestMethod]
        public void Reboot() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.Reboot();
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void SendStoreAndForward() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.SendStoreAndForward();
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void PromptAndGetSignatureFile() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.PromptAndGetSignatureFile("please Sign", "", null);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.SignatureData);

            // save as bmp
            SaveSignatureFile(response.SignatureData, @"C:\temp\signature11.bmp");

            // convert and save as png
            var pngData = BmpToPng(response.SignatureData);
            SaveSignatureFile(pngData, @"C:\temp\signature1.png");
        }

        [TestMethod]
        public void Cancel() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            _device.Cancel();
        }

        [TestMethod]
        public void StartCardTransaction() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var paramObj = new UpaParam() {
                Timeout = 60,
                AcquisitionTypes = AcquisitionType.Swipe,
                Header = "Header",
                DisplayTotalAmount = "Y",
                PromptForManual = true,
                BrandIcon1 = 4,
                BrandIcon2 = 3
            };
            var upaIndicator = new ProcessingIndicator() {
                QuickChip = "Y",
                CheckLuhn = "Y",
                SecurityCode = "Y",
                CardTypeFilter = CardTypeFilter.GIFT
            };
            var upaTrans = new UpaTransactionData() {
                TotalAmount = 11.2m,
                CashBackAmount = 2.5m,
                TranDate = DateTime.Now,
                TranTime = DateTime.Now,
                TransType = TransactionType.Sale
            };

            var response = _device.StartCardTransaction(paramObj, upaIndicator, upaTrans);
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void DeleteSafWithSafReferenceNumber() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.DeleteSaf("P0000007");
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void DeleteSafWithSafReferenceNumberAndTranNo() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";
            var response = _device.DeleteSaf("P0000007", "0005");
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void RegisterPOS() {
            POSData posData = new POSData();
            posData.AppName = "com.global.testapp";
            posData.LaunchOrder = 1;
            posData.Remove = false;

            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";

            var response = _device.RegisterPOS(posData);
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
        }

        #region InjectCarouselLogo Command Unit Tests

        [TestMethod]
        public void InjectCarouselLogo_ShouldReturnSuccessStatus() {
            UDData data = new UDData();
            data.FileName = "brand_logo_test.png";
            // The image files files are located in GlobalPayments.Api.Tests\Terminals\UPA\FileExamples\
            // If the test case is failing try by Updating the file path with absolute path as below,
            // for example: C:\Codes\sdk-private\tests\GlobalPayments.Api.Tests\Terminals\UPA\FileExamples\brand_logo_test.png
            data.FilePath = $@"{System.IO.Directory.GetCurrentDirectory()}\Terminals\UPA\FileExamples\brand_logo_test.png";
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";

            var response = _device.InjectCarouselLogo(data);
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void InjectCarouselLogo_ShouldThrowException_WhenFileNameDoesNotStartWithBrandLogo() {
            UDData data = new UDData();
            data.FileName = "test.png";
            data.FilePath = $@"{System.IO.Directory.GetCurrentDirectory()}\Terminals\UPA\FileExamples\test.png";
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";

            var ex = Assert.ThrowsException<MessageException>(() => {
                var response = _device.InjectCarouselLogo(data);
            });
            Assert.AreEqual("FileName must start with 'brand_logo_'.", ex.Message);
        }

        [TestMethod]
        public void InjectCarouselLogo_ShouldThrowException_WhenFileExtensionIsInvalid() {
            UDData data = new UDData();
            data.FileName = "brand_logo_test.html";
            data.FilePath = $@"{System.IO.Directory.GetCurrentDirectory()}\Terminals\UPA\FileExamples\brand_logo_test.html";
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";

            var ex = Assert.ThrowsException<MessageException>(() => {
                var response = _device.InjectCarouselLogo(data);
            });
            Assert.AreEqual("FileName must have a valid extension (.jpg, .jpeg, .bmp, .png, .gif).", ex.Message);
        }
        #endregion

        #region RemoveCarouselLogo Command Unit Tests
        [TestMethod]
        public void RemoveCarouselLogo_ShouldReturnSuccessStatus() {
            UDData data = new UDData();
            data.FileName = "brand_logo_test.png";
            data.FilePath = $@"{System.IO.Directory.GetCurrentDirectory()}\Terminals\UPA\FileExamples\brand_logo_test.png";
            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";

            var response = _device.RemoveCarouselLogo(data);
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        public void RemoveCarouselLogo_ShouldThrowException_WhenFileNameHasNoExtension() {
            UDData data = new UDData();
            data.FileName = "brand_logo_test";
            data.FilePath = $@"{System.IO.Directory.GetCurrentDirectory()}\Terminals\UPA\FileExamples\brand_logo_test.png";
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";

            var ex = Assert.ThrowsException<MessageException>(() => {
                var response = _device.RemoveCarouselLogo(data);
            });

            Assert.AreEqual("FileName must include a file extension and must not contain a file path.", ex.Message);
        }

        [TestMethod]
        public void RemoveCarouselLogo_ShouldThrowException_WhenFileNameContainsFilePath() {

            UDData data = new UDData();
            data.FileName = "Terminals\\UPA\\FileExamples\\brand_logo_test.png";
            data.FilePath = $@"{System.IO.Directory.GetCurrentDirectory()}\Terminals\UPA\FileExamples\brand_logo_test.png";
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";

            var ex = Assert.ThrowsException<MessageException>(() => {
                var response = _device.RemoveCarouselLogo(data);
            });

            Assert.AreEqual("FileName must include a file extension and must not contain a file path.", ex.Message);
        }
        #endregion

        #region SaveConfigFile Command Tests
        [TestMethod]
        // Test that SaveConfigFile returns Success when a valid UpaConfigContent is provided and no reinitialization is set
        public void SaveConfigFile_ShouldReturnSuccess_WhenValidConfigProvided_NoReinitialize() {
            // Arrange: Create a valid UpaConfigContent object with XML configuration
            var upaConfigContent = new UpaConfigContent() {
                ConfigType = TerminalConfigType.ContactTerminalConfiguration,
                FileContent = "",
                Length = 100,
                Reinitialize = Reinitialize.NoReinitializeApplication
            };

            upaConfigContent.FileContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                "<TerminalConfiguration>\n" +
                "    <ContactTerminalConfiguration>\n" +
                "        <TerminalType>Contact</TerminalType>\n" +
                "        <TerminalCapabilities>...</TerminalCapabilities>\n" +
                "    </ContactTerminalConfiguration>\n" +
                "</TerminalConfiguration>";

            // Attach a handler to OnMessageSent to verify a message is sent
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            // Set the ECR ID for the device
            _device.EcrId = "13";

            // Act: Call SaveConfigFile with the valid config
            var response = _device.SaveConfigFile(upaConfigContent);

            // Assert: The response should not be null and should indicate Success
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        // Test that SaveConfigFile returns Success when a valid UpaConfigContent is provided and reinitialization is set
        public void SaveConfigFile_ReturnsSuccess_WhenValidConfigProvided_ReinitializeApplication() {
            // Arrange: Create a valid UpaConfigContent object with XML configuration
            var upaConfigContent = new UpaConfigContent() {
                ConfigType = TerminalConfigType.ContactTerminalConfiguration,
                FileContent = "",
                Length = 100,
                Reinitialize = Reinitialize.ReinitializeApplication
            };

            upaConfigContent.FileContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                "<TerminalConfiguration>\n" +
                "    <ContactTerminalConfiguration>\n" +
                "        <TerminalType>Contact</TerminalType>\n" +
                "        <TerminalCapabilities>...</TerminalCapabilities>\n" +
                "    </ContactTerminalConfiguration>\n" +
                "</TerminalConfiguration>";

            // Attach a handler to OnMessageSent to verify a message is sent
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            // Set the ECR ID for the device
            _device.EcrId = "13";

            // Act: Call SaveConfigFile with the valid config
            var response = _device.SaveConfigFile(upaConfigContent);

            // Assert: The response should not be null and should indicate Success
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        // Test that SaveConfigFile throws a MessageException when upaConfig is null
        public void SaveConfigFile_ShouldThrow_WhenUpaConfigIsNull() {

            // Test that SaveConfigFile throws a MessageException when upaConfig is null
            var ex = Assert.ThrowsException<MessageException>(() => {
                _device.SaveConfigFile(null);
            });

            Assert.AreEqual("UpaConfigContent is invalid: upaConfig must not be null.", ex.Message);
        }

        [TestMethod]
        // Test that SaveConfigFile throws a MessageException when FileContent is null or empty
        public void SaveConfigFile_ShouldThrow_WhenFileContentIsNullOrEmpty() {
            // Create a UpaConfigContent object with specific properties for testing
            var upaConfig = new UpaConfigContent {
                ConfigType = TerminalConfigType.ContactTerminalConfiguration,
                FileContent = "",
                Length = 100
            };
            // Expect a MessageException when FileContent is empty
            var ex = Assert.ThrowsException<MessageException>(() => {
                _device.SaveConfigFile(upaConfig);
            });
            Assert.AreEqual("UpaConfigContent is invalid: FileContent must not be null or empty.", ex.Message);
        }

        [TestMethod]
        // Test that SaveConfigFile throws a MessageException when Length is negative
        public void SaveConfigFile_ShouldThrow_WhenLengthIsNegative() {
            // Create a UpaConfigContent with empty FileContent to test validation
            var upaConfig = new UpaConfigContent {
                ConfigType = TerminalConfigType.ContactTerminalConfiguration,
                FileContent = "",
                Length = -1
            };
            upaConfig.FileContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
               "<TerminalConfiguration>\n" +
               "    <ContactTerminalConfiguration>\n" +
               "        <TerminalType>Contact</TerminalType>\n" +
               "        <TerminalCapabilities>...</TerminalCapabilities>\n" +
               "    </ContactTerminalConfiguration>\n" +
               "</TerminalConfiguration>";
            // Expect a MessageException with the correct error message
            var ex = Assert.ThrowsException<MessageException>(() => {
                _device.SaveConfigFile(upaConfig);
            });
            Assert.AreEqual("UpaConfigContent is invalid: Length must be between 0 and 102400.", ex.Message);
        }

        [TestMethod]
        // Test that SaveConfigFile throws a MessageException when Length is too large (greater than 102400)
        public void SaveConfigFile_ShouldThrow_WhenLengthIsTooLarge() {
            // Create a UpaConfigContent with an invalid Length (too large)
            var upaConfig = new UpaConfigContent {
                ConfigType = TerminalConfigType.ContactTerminalConfiguration,
                FileContent = "<xml>...</xml>",
                Length = 10200000
            };
            // Expect a MessageException with the correct error message
            var ex = Assert.ThrowsException<MessageException>(() => {
                _device.SaveConfigFile(upaConfig);
            });
            Assert.AreEqual("UpaConfigContent is invalid: Length must be between 0 and 102400.", ex.Message);
        }

        #endregion

        #region SetLogoCarouselInterval Command Tests
        [TestMethod]
        // Test that SetLogoCarouselInterval returns Success when a valid interval is provided
        public void SetLogoCarouselInterval_ReturnsSuccess_WhenValidIntervalProvided_IsFullScreenFalse() {
            bool isFullScreen = false;
            int intervalTime = 5;
            // Attach a handler to OnMessageSent to verify a message is sent
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            // Set the ECR ID for the device
            _device.EcrId = "13";

            // Call SetLogoCarouselInterval with a valid interval and verify the response
            var response = _device.SetLogoCarouselInterval(intervalTime, isFullScreen);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        // Test that SetLogoCarouselInterval returns Success when a valid interval is provided
        public void SetLogoCarouselInterval_ReturnsSuccess_WhenValidIntervalProvided_IsFullScreenTrue() {
            bool isFullScreen = true;
            int intervalTime = 5;
            // Attach a handler to OnMessageSent to verify a message is sent
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            // Set the ECR ID for the device
            _device.EcrId = "13";

            // Call SetLogoCarouselInterval with a valid interval and verify the response
            var response = _device.SetLogoCarouselInterval(intervalTime, isFullScreen);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
            Assert.AreEqual("Success", response.Status);
        }

        // Test that SetLogoCarouselInterval throws a MessageException when the interval is out of the valid range (0-9 seconds)
        [TestMethod]
        public void SetLogoCarouselInterval_ShouldThrowMessageException_WhenIntervalIsOutOfRange() {
            bool isFullScreen = true;
            int intervalTime = 12;
            // Attach a handler to OnMessageSent to verify a message is sent
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            // Set the ECR ID for the device
            _device.EcrId = "13";

            // Expect a MessageException when interval is out of range
            var ex = Assert.ThrowsException<MessageException>(() => {
                var response = _device.SetLogoCarouselInterval(intervalTime, isFullScreen);
            });

            Assert.AreEqual("intervalTime must be between 0 and 9 seconds.", ex.Message);
        }
        #endregion

        #region GetBatteryPercentage Command Tests

        [TestMethod]
        // Test that GetBatteryPercentage returns a valid response containing BatteryLevel and Success status
        public void GetBatteryPercentage_ValidResponse_ContainsBatteryLevelAndSuccessStatus() {
            // Attach a handler to OnMessageSent to ensure a message is sent
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";

            // Call GetBatteryPercentage and validate the response
            var response = _device.GetBatteryPercentage();
            Assert.IsNotNull(response);
            Assert.IsNotNull(((UPAResponseHandler)response).BatteryLevel);
            Assert.AreEqual(100, ((UPAResponseHandler)response).BatteryLevel);
            Assert.AreEqual("Success", response.Status);
        }

        [TestMethod]
        // Test that GetBatteryPercentage throws a MessageException with the expected message when a connection timeout occurs
        public void GetBatteryPercentage_ShouldThrowMessageException_WhenConnectionTimeout() {
            // Attach a handler to OnMessageSent to ensure a message is sent
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };
            _device.EcrId = "13";

            // Assert that calling GetBatteryPercentage throws a MessageException with the correct message
            var Ex = Assert.ThrowsException<MessageException>(() => {
                _device.GetBatteryPercentage();
            });
            Assert.AreEqual("Connection not established within the specified timeout.", Ex.Message);
        }
        #endregion

        #region helper methods
        private void SaveSignatureFile(byte[] signatureData, string filename) {
            using (var sw = new BinaryWriter(File.OpenWrite(filename))) {
                sw.Write(signatureData);
                sw.Flush();
            }
        }

        private byte[] BmpToPng(byte[] signatureData) {
            using (var ms = new MemoryStream(signatureData)) {
                using (var pngStream = new MemoryStream()) {
                    //var bmp = new Bitmap(ms);
                    //bmp.Save(pngStream, ImageFormat.Png);
                    return pngStream.ToArray();
                }
            }
        }
        #endregion
    }
}
