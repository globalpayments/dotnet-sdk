using System.Diagnostics;
using System.Threading;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Terminals.HPA.VRF {
    [TestClass]
    public class HpaVerificationTests {
        IDeviceInterface _device;

        public HpaVerificationTests() {
            _device = DeviceService.Create(new ConnectionConfig {
                DeviceType = DeviceType.HPA_ISC250,
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "10.138.141.38",
                Port = "12345",
                RequestIdProvider = new RandomIdProvider()
            });
            Assert.IsNotNull(_device);
            _device.OpenLane();
        }

        private void PrintReceipt(ITerminalResponse response) {
            string receipt = "x_trans_type=" + response.TransactionType;
            receipt += "&x_application_label=" + response.ApplicationLabel;
            receipt += "&x_masked_card=" + response.MaskedCardNumber;
            receipt += "&x_application_id=" + response.ApplicationId;
            receipt += "&x_cryptogram_type=" + response.ApplicationCryptogramType;
            receipt += "&x_application_cryptogram=" + response.ApplicationCryptogram;
            receipt += "&x_expiration_date=" + response.ExpirationDate;
            receipt += "&x_entry_method=" + response.EntryMethod;
            receipt += "&x_approval=" + response.ApprovalCode;
            receipt += "&x_transaction_amount=" + response.TransactionAmount;
            receipt += "&x_amount_due=" + response.AmountDue;
            receipt += "&x_customer_verification_method=" + response.CardHolderVerificationMethod;
            receipt += "&x_signature_status=" + response.SignatureStatus;
            receipt += "&x_response_text=" + response.ResponseText;
            Debug.WriteLine(receipt);
        }

        [TestCleanup]
        public void WaitAndReset() {
            Thread.Sleep(3000);
            _device.Reset();
        }

        /*
            TEST CASE #1 – Contact Chip and Signature – Offline 
            Objective Process a contact transaction where the CVM’s supported are offline chip and signature
            Test Card Card #1 - MasterCard EMV
            Procedure Perform a complete transaction without error..
            Enter transaction amount $23.00.
        */
        [TestMethod]
        public void TestCase01() {
            var response = _device.Sale(23m)
                .WithSignatureCapture(true)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Debug.WriteLine("Response: " + response.ToString());
            Debug.WriteLine("Gateway Txn ID: " + response.TransactionId);
            PrintReceipt(response);
        }

        /*
            TEST CASE #2 - EMV Receipts 
            Objective	1. Verify receipt image conforms to EMV Receipt Requirements.
            2. Verify that signature capture functionality works.
            Test Card	Any card brand – Visa, MC, Discover, AMEX.
            Procedure	Run an EMV insert sale using any card brand.
            The device should get an Approval.
            Cardholder is prompted to sign on the device.
        */
        [TestMethod]
        public void TestCase02() {
            // print receipt for TestCase01
        }

        /*
            TEST CASE #3 - Approved Sale with Offline PIN
            Objective	Process an EMV contact sale with offline PIN.
            Test Card	Card #1 - MasterCard EMV
            Procedure	Insert the card in the chip reader and follow the instructions on the device.
            Enter transaction amount $25.00.
            When prompted for PIN, enter 4315.
            If no PIN prompt, device could be in QPS mode with limit above transaction amount.
        */
        [TestMethod]
        public void TestCase03() {
            var response = _device.Sale(25m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Debug.WriteLine("Response: " + response.ToString());
            Debug.WriteLine("Gateway Txn ID: " + response.TransactionId);
            PrintReceipt(response);
        }

        /*
            TEST CASE #4 -  Manually Entered Sale with AVS & CVV2/CID 
            (If AVS is supported)
            Objective	Process a keyed sale, with PAN & exp date, along with Address Verification and Card Security Code to confirm the application can support any or all of these.
            Test Card	Card #5 – MSD only MasterCard
            Procedure	1. Select sale function and manually key Test Card #5 for the amount of $90.08.
            a.	Enter PAN & expiration date.
            b.	Enter 321 for Card Security Code (CVV2, CID), if supporting this feature.
            Enter 76321 for AVS, if supporting this feature.
        */
        [TestMethod]
        public void TestCase04() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.Sale(90.08m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("Y", response.AvsResponseCode);
            Assert.AreEqual("M", response.CvvResponseCode);

            Debug.WriteLine("Response: " + response.ToString());
            Debug.WriteLine("Gateway Txn ID: " + response.TransactionId);
            PrintReceipt(response);
        }


        /*
            TEST CASE #5 - Partial Approval
            Objective	1. Ensure application can handle non-EMV swiped transactions.
            2. Validate partial approval support.
            Test Card	Card #4 – MSD only Visa
            Procedure	Run a credit sale and follow the instructions on the device to complete the transaction.
            Enter transaction amount $155.00 to receive a partial approval.
            Transaction is partially approved online with an amount due remaining.
        */
        [TestMethod]
        public void TestCase05() {
            var response = _device.Sale(155m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(55m, response.AmountDue);

            Debug.WriteLine("Response: " + response.ToString());
            Debug.WriteLine("Gateway Txn ID: " + response.TransactionId);
            PrintReceipt(response);
        }

        /*
            TEST CASE #6 - Online Void
            Objective	Process an online void.
            Test Card	Card #3 – EMV Visa w/ Signature CVM
            Procedure	Enter the Transaction ID to void.
            Transaction has been voided.
        */
        [TestMethod]
        public void TestCase06() {
            var response = _device.Sale(10m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var voidResponse = _device.Void()
                .WithTransactionId(response.TransactionId)
                .Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);

            Debug.WriteLine("Response: " + voidResponse.ToString());
            Debug.WriteLine("Gateway Txn ID: " + voidResponse.TransactionId);
        }

        /*
            TEST CASE #7 -  Debit Sale (with Cash Back) & Debit Void
            Objective	Confirm support of PIN debit sale, cash back, and debit void.
            Test Card	Confirm support of PIN debit sale, cash back, and debit void.
            Procedure	Debit Sale with Cash Back:
            Run a debit sale for $10.00 and follow the instructions on the device to complete the transaction.
            When prompted for Cash Back, enter $5.00 for the cash back amount.
            When prompted for PIN, enter 1234.
            Transaction is approved online.

            Void:
            Enter the Transaction ID to void.
            Transaction has been voided.

            Pass Criteria	The transaction is approved online.
            The transaction has been voided.
        */
            //[TestMethod]
            //public void TestCase07() {
            //    var response = _device.DebitSale(1, 10m)
            //        //.WithCashBack(5m)
            //        .Execute();
            //    Assert.IsNotNull(response);
            //    Assert.AreEqual("00", response.ResponseCode);

            //    Debug.WriteLine("Response: " + response.ToString());
            //    Debug.WriteLine("Gateway Txn ID: " + response.TransactionId);

            //    var voidResponse = _device.DebitReversal(2)
            //        .WithTransactionId(response.TransactionId)
            //        .Execute();
            //    Assert.IsNotNull(voidResponse);
            //    Assert.AreEqual("00", voidResponse.ResponseCode);

            //    Debug.WriteLine("Response: " + voidResponse.ToString());
            //    Debug.WriteLine("Gateway Txn ID: " + voidResponse.TransactionId);
            //}

            /*
                TEST CASE  #8 – Process Lane Open on SIP
                Objective	Display line items on the SIP.
                Test Card	NA
                Procedure	Start the process to open a lane on the POS.
            */
        [TestMethod]
        public void TestCase08() {
            var response = _device.OpenLane();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);
        }

        /*
            TEST CASE #9 – Credit Return 
            Objective	Confirm support of a Return transaction for credit.
            Test Card	Card #4 – MSD only Visa
            Procedure	1.	Select return function for the amount of $9.00
            2.	Swipe or Key Test card #4 through the MSR
            3.	Select credit on the device
        */
        [TestMethod]
        public void TestCase09() {
            var response = _device.Refund(9m)
                .WithAllowDuplicates(true)
                .Execute();
             Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Debug.WriteLine("Response: " + response.ToString());
            Debug.WriteLine("Gateway Txn ID: " + response.TransactionId);
        }

        /*
            TEST CASE #10 – HMS Gift 
            Objective	Transactions: Gift Balance Inquiry,  Gift Load,  Gift Sale/Redeem, Gift Replace
            Test Card	Gift Card (Card Present/Card Swipe)
            Procedure	Test System is a Stateless Environment, the responses are Static.
            1.	Gift Balance Inquiry (GiftCardBalance):
            a.	Should respond with a BalanceAmt of $10
            2.	Gift Load (GiftCardAddValue):
            a.	Initiate a Sale and swipe
            b.	Enter $8.00 as the amount
            3.	Gift Sale/Redeem (GiftCardSale):
            a.	Initiate a Sale and swipe
            b.	Enter $1.00 as the amount
            4.	Gift Card Replace (GiftCardReplace)
            a.	Initiate a Gift Card Replace
            b.	Swipe Card #1 – (Acct #: 5022440000000000098)
            c.	Manually enter  Card #2 –  (Acct #: “5022440000000000007”)
        */
        [TestMethod]
        public void TestCase10a() {
            var response = _device.Balance()
                .WithPaymentMethodType(PaymentMethodType.Gift)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(10m, response.BalanceAmount);

            Debug.WriteLine("Response: " + response.ToString());
            Debug.WriteLine("Gateway Txn ID: " + response.TransactionId);
        }

        [TestMethod]
        public void TestCase10b() {
            var response = _device.AddValue()
                .WithPaymentMethodType(PaymentMethodType.Gift)
               .WithAmount(8m)
               .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Debug.WriteLine("Response: " + response.ToString());
            Debug.WriteLine("Gateway Txn ID: " + response.TransactionId);
        }

        [TestMethod]
        public void TestCase10c() {
            var response = _device.Sale(1m)
                .WithPaymentMethodType(PaymentMethodType.Gift)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Debug.WriteLine("Response: " + response.ToString());
            Debug.WriteLine("Gateway Txn ID: " + response.TransactionId);
        }

        /*
         [TestMethod]
         public void TestCase10d() {
            var response = _device.GiftReplace(1).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Debug.WriteLine("Response: " + response.ToString());
            Debug.WriteLine("Gateway Txn ID: " + response.TransactionId);
         }
        */

         /*
             TEST CASE #11 – EBT Food Stamp
             Objective Transactions: Food Stamp Purchase, Food Stamp Return and Food Stamp Balance Inquiry
             Test Card   Card #4 – MSD only Visa
             1.Food Stamp Purchase(EBTFSPurchase) :
             a.Initiate an EBT sale transaction and swipe Test Card #4
             b.Select EBT Food Stamp if prompted.
             c.Enter $101.01 as the amount
             2.Food Stamp Return (EBTFSReturn):
             a.Intitiate an EBT return and manually enter Test Card #4
             b.Select EBT Food Stamp if prompted
             c.Enter $104.01 as the amount
             3.Food Stamp Balance Inquiry (EBTBalanceInquiry):
             a.Initiate an EBT blance inquiry transaction and swipe Test Card #4 Settle all transactions.
         */

        [TestMethod]
        public void TestCase11a() {
            var response = _device.Sale(101.01m)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Debug.WriteLine("Response: " + response.ToString());
            Debug.WriteLine("Gateway Txn ID: " + response.TransactionId);
        }

        [TestMethod]
        public void TestCase11b() {
            var response = _device.Refund(104.01m)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.DeviceResponseText);

            Debug.WriteLine("Response: " + response.ToString());
            Debug.WriteLine("Gateway Txn ID: " + response.TransactionId);
        }

        [TestMethod]
        public void TestCase11c() {
            var response = _device.Balance()
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Debug.WriteLine("Response: " + response.ToString());
            Debug.WriteLine("Gateway Txn ID: " + response.TransactionId);
        }

        /* 
            TEST CASE #12 – EBT Cash Benefits 
            Objective	Transactions: EBT Cash Benefits with Cash Back, EBT Cash Benefits Balance Inquiry and EBT Cash Benefits Withdraw
            Test Card	Card #4 – MSD only Visa
            EBT Cash Benefits w Cash Back (EBTCashBackPurchase):
            a.Initiate an EBT sale transaction and swipe Test Card #4
            b.Select EBT Cash Benefits if prompted
            c.Enter $101.01 as the amount
            d.Enter $5.00 as the cash back amount
            e.The settlement amount is $106.01
            2.EBT Cash Benefits Balance Inquiry (EBTBalanceInquiry):
            a.Initiate an EBT cash benefit balance inquiry transaction and swipe Test Card #4
        */
        [TestMethod]
        public void TestCase12a() {
            var response = _device.Sale(101.01m)
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Debug.WriteLine("Response: " + response.ToString());
            Debug.WriteLine("Gateway Txn ID: " + response.TransactionId);
        }

        [TestMethod]
        public void TestCase12b() { 
            var response = _device.Balance()
                .WithPaymentMethodType(PaymentMethodType.EBT)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Debug.WriteLine("Response: " + response.ToString());
            Debug.WriteLine("Gateway Txn ID: " + response.TransactionId);
        }

        /*
            TEST CASE #13 – Batch Close 
            (Mandatory if Conditional Test Cases are ran)
            Objective	Close the batch, ensuring all approved transactions (offline or online) are settled.
            Integrators are automatically provided accounts with auto-close enabled, so if manual batch transmission will not be performed in the production environment then it does not need to be tested.
            Test Card	N/A
            Procedure	Initiate a Batch Close command
            Pass Criteria	Batch submission must be successful.
            Batch Sequence #:
            References		HPA Specifications.
       
        [TestMethod]
        public void TestCase13() {
            _device.CloseLane();
            WaitAndReset();

            var response = _device.BatchClose();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);

            Debug.WriteLine("Response: " + response.ToString());
            Debug.WriteLine("Sequence #: " + response.SequenceNumber);
        }
        */

        /*
            TEST CASE #13 – End of Day 
            Objective End of Day, ensuring all approved transactions (offline or online) are settled. 
            Supported transactions: Reversal, Offline Decline, Transaction Certificate, Add Attachment, SendSAF, Batch Close, EMV PDL and Heartbeat
            Procedure	Initiate a End of Day command
            Pass Criteria	EOD submission must be successful.
            References		HPA Specifications.
        */
        [TestMethod]
        public void TestCase13() {
            _device.CloseLane();
            _device.Reset();

            var response = _device.EndOfDay();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.DeviceResponseCode);

            Debug.WriteLine("Reversal Response: " + response.ReversalResponse.ToString());
            Debug.WriteLine("Offline Decline Response: " + response.EmvOfflineDeclineResponse.ToString());
            Debug.WriteLine("TransactionCertificate Response: " + response.EmvTransactionCertificationResponse.ToString());
            Debug.WriteLine("Add Attachment Response: " + response.AttachmentResponse.ToString());
            Debug.WriteLine("SendSAF Response: " + response.SAFResponse.ToString());
            Debug.WriteLine("Batch Close Response: " + response.BatchCloseResponse.ToString());
            Debug.WriteLine("EmvPDL Response: " + response.EmvPDLResponse.ToString());
            Debug.WriteLine("Heartbeat Response: " + response.HeartBeatResponse.ToString());
        }
    }
}