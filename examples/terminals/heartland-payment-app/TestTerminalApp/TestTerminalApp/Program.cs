using System;
using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;

namespace TestTerminalApp {
    class Program {
        static void Main(string[] args) {
            IDeviceInterface device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.HSIP_ISC250,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "10.12.220.130",
                Port = "12345",
                Timeout = 30000
            });

            try {
                device.OpenLane();

                Console.WriteLine("Please swipe the card Shane.");
                var transaction = device.CreditSale(1, 32m)
                    .WithAllowDuplicates(true)
                    .WithSignatureCapture(true)
                    .Execute();

                if (transaction.ResponseCode == "00" || transaction.ResponseCode == "10") {
                    PrintReceipt(transaction);
                    AnyKeyToContinue();
                }
                else {
                    Console.WriteLine("{0} - {1}", transaction.ResponseCode, transaction.ResponseText);
                    Console.WriteLine("{0} - {1}", transaction.DeviceResponseCode, transaction.DeviceResponseText);
                    AnyKeyToContinue();
                }
            }
            catch (BuilderException exc) {
                // validation errors
                Console.WriteLine(exc.Message);
                AnyKeyToContinue();
            }
            catch (MessageException exc) {
                // trouble talking to the device
                Console.WriteLine(exc.Message);
                AnyKeyToContinue();
            }
            catch (GatewayException exc) {
                // trouble talking to the gateway
                Console.WriteLine(exc.Message);
                Console.WriteLine(exc.ResponseCode, exc.ResponseMessage);
                AnyKeyToContinue();
            }
            catch (ApiException exc) {
                // every exception ever!!!
                Console.WriteLine(exc.Message);
                AnyKeyToContinue();
            }
            finally {
                device.Reset();
                device.CloseLane();
                device.Dispose();
            }
        }

        public static void AnyKeyToContinue() {
            Console.WriteLine("\r\n\r\nHit any key to continue.");
            Console.ReadKey();
            Console.Clear();
        }

        public static void PrintReceipt(TerminalResponse response) {
            // build receipt string
            Console.WriteLine("ABC RETAIL SHOP");
            Console.WriteLine("1 Heartland Way");
            Console.WriteLine("Jeffersonville, IN 47316");
            Console.WriteLine("888-798-3133");
            Console.WriteLine();
            Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
            Console.WriteLine(response.TransactionType);
            Console.WriteLine();

            // EMV stuff
            if (!string.IsNullOrEmpty(response.ApplicationLabel)) {
                Console.WriteLine("ACCT:     {0}", response.MaskedCardNumber);
                Console.WriteLine("APP NAME: {0}", response.ApplicationLabel);
                Console.WriteLine("AID:      {0}", response.ApplicationId);
                Console.WriteLine("{0}:      {1}", response.ApplicationCryptogramType, response.ApplicationCryptogram);
            }
            else {
                Console.WriteLine("ACCT:     {0}", response.MaskedCardNumber);
                Console.WriteLine("EXP:      ****");
            }
            Console.WriteLine("ENTRY:    {0}", response.EntryMethod);
            Console.WriteLine("APPROVAL: {0}", response.AuthorizationCode);
            Console.WriteLine();
            Console.WriteLine("TOTAL:    {0:c}", response.TransactionAmount);

            if (response.AmountDue != null && response.AmountDue != 0) {
                Console.WriteLine("AMOUNT DUE: {0:c}", response.AmountDue);
            }
            Console.WriteLine();
            Console.WriteLine();

            Action printSignature = () => {
                Console.WriteLine("I agree to pay the above total amount according to card issuer agreement");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("X___________________________________");
                Console.WriteLine("SIGNATURE");
            };

            var signatureRequiredCvms = new List<string> { "3", "5", "6" };
            var signatureRequiredStatus = new List<string> { "2", "3", "6" };
            if (response.EntryMethod == "Chip") {
                var cvm = response.CardHolderVerificationMethod;
                if (signatureRequiredCvms.Contains(cvm)) {
                    var sign_status = response.SignatureStatus ?? "0";
                    if (sign_status == "1" || sign_status == "5")
                        Console.WriteLine("Signature Captured Electronically");
                    else if (signatureRequiredStatus.Contains(sign_status))
                        printSignature();
                }
            }
            else printSignature();

            Console.WriteLine();
            Console.WriteLine(response.ResponseText);
            Console.WriteLine("MERCHANT COPY");
        }
    }
}