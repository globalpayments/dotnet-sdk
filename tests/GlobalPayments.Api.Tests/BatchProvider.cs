using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Elements;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GlobalPayments.Api.Tests {
    public class BatchProvider : IBatchProvider {
        private readonly Object objectLock = new Object();
        private string fileName = "C:\\temp\\batch.dat";
        private int batchNumber = 1;
        private int sequenceNumber = 1;
        private int transactionCount = 0;
        private decimal totalDebits = 0m;
        private decimal totalCredits = 0m;
        private IRequestEncoder requestEncoder = null;
        private List<string> encodedRequests;
        private PriorMessageInformation priorMessageInformation;

        public int ProprietaryTransactionCount { get; set; } = 0;
        public decimal ProprietaryTransactionSales { get; set; } = 0;
        public int VisaTransactionCount { get; set; } = 0;
        public decimal VisaTransactionSales { get; set; } = 0;
        public int MasterCardTransactionCount { get; set; } = 0;
        public decimal MasterCardTransactionSales { get; set; } = 0;
        public int OtherCardTransactionCount { get; set; } = 0;
        public decimal OtherCardTransactionSales { get; set; } = 0;
        public int CreditCardTransactionCount { get; set; } = 0;
        public decimal CreditCardTransactionSales { get; set; } = 0;
        public int DebitEBTTransactionCount { get; set; } = 0;
        public decimal DebitEBTTransactionSales { get; set; } = 0;
        public int CreditVoidTransactionCount { get; set; } = 0;
        public decimal CreditVoidTransactionSales { get; set; } = 0;
        public int DebitEBTVoidTransactionCount { get; set; } = 0;
        public decimal DebitEBTVoidTransactionSales { get; set; } = 0;
        public int CreditReturnTransactionCount { get; set; } = 0;
        public decimal CreditReturnTransactionSales { get; set; } = 0;
        public int DebitReturnTransactionCount { get; set; } = 0;
        public decimal DebitReturnTransactionSales { get; set; } = 0;
        public string CardType { get; set; }
        public int TotalTransactionCount { get; set; } = 0;

        private static BatchProvider _instance;

        private BatchProvider() {
            encodedRequests = new List<string>();

            lock (objectLock) {
                StreamReader br = null;
                try {
                    br = new StreamReader(File.OpenRead(fileName));

                    // read the batch, sequence and transaction data
                    string batchData = br.ReadLine();
                    if (batchData != null) {
                        string[] elements = batchData.Split("|");

                        sequenceNumber = int.Parse(elements[0]);
                        batchNumber = int.Parse(elements[1]);
                        //transactionCount = int.Parse(elements[2]);
                        //totalCredits = new decimal(Convert.ToDouble(elements[3]));
                        //totalDebits = new decimal(Convert.ToDouble(elements[4]));
                        ProprietaryTransactionCount = int.Parse(elements[2]);
                        ProprietaryTransactionSales = new decimal(Convert.ToDouble(elements[3]));
                        VisaTransactionCount = int.Parse(elements[4]);
                        VisaTransactionSales = new decimal(Convert.ToDouble(elements[5]));
                        MasterCardTransactionCount = int.Parse(elements[6]);
                        MasterCardTransactionSales = new decimal(Convert.ToDouble(elements[7]));
                        OtherCardTransactionCount = int.Parse(elements[8]);
                        OtherCardTransactionSales = new decimal(Convert.ToDouble(elements[9]));
                        CreditCardTransactionCount = int.Parse(elements[10]);
                        CreditCardTransactionSales = new decimal(Convert.ToDouble(elements[11]));
                        DebitEBTTransactionCount = int.Parse(elements[12]);
                        DebitEBTTransactionSales = new decimal(Convert.ToDouble(elements[13]));
                        CreditVoidTransactionCount = int.Parse(elements[14]);
                        CreditVoidTransactionSales = new decimal(Convert.ToDouble(elements[15]));
                        DebitEBTVoidTransactionCount = int.Parse(elements[16]);
                        DebitEBTVoidTransactionSales = new decimal(Convert.ToDouble(elements[17]));
                        CreditReturnTransactionCount = int.Parse(elements[18]);
                        CreditReturnTransactionSales = new decimal(Convert.ToDouble(elements[19]));
                        DebitReturnTransactionCount = int.Parse(elements[20]);
                        DebitReturnTransactionSales = new decimal(Convert.ToDouble(elements[21]));
                        TotalTransactionCount = int.Parse(elements[22]);

                        for (int i = 0; i < TotalTransactionCount; i++) {
                            string request = br.ReadLine();
                            if (request != null) {
                                encodedRequests.Add(request);
                            }
                        }
                    }
                    else Save();
                }
                catch (IOException) {
                    Save();
                }
                finally {
                    if (br != null) {
                        try {
                            br.Close();
                        }
                        catch (IOException) { /* NOM NOM */ }
                    }
                }
            }
        }

        public static BatchProvider GetInstance() {
            if (_instance == null) {
                _instance = new BatchProvider();
            }
            return _instance;
        }

        public int GetBatchNumber() {
            return batchNumber;
        }

        public int GetSequenceNumber() {
            if (sequenceNumber == 99) {
                throw new Exception();
            }

            lock (objectLock) {
                sequenceNumber += 1;
                Save();
            }
            return sequenceNumber;
        }

        public int GetTransactionCount() {
            return transactionCount;
        }

        public decimal GetTotalCredits() {
            return totalCredits;
        }

        public decimal GetTotalDebits() {
            return totalDebits;
        }

        public IRequestEncoder GetRequestEncoder() {
            return requestEncoder;
        }

        public List<string> GetEncodedRequests() {
            return encodedRequests;
        }

        public PriorMessageInformation GetPriorMessageData() {
            return priorMessageInformation;
        }

        public void SetPriorMessageData(PriorMessageInformation priorMessageInformation) {
            this.priorMessageInformation = priorMessageInformation;
        }

        public void CloseBatch(bool inBalance) {
            lock (objectLock) {
                sequenceNumber = 1;
                if (batchNumber == 99) {
                    batchNumber = 1;
                }
                else batchNumber += 1;
                //transactionCount = 0;
                //totalDebits = 0m;
                //totalCredits = 0m;
                ProprietaryTransactionCount = 0;
                ProprietaryTransactionSales = 0;
                VisaTransactionCount = 0;
                VisaTransactionSales = 0;
                MasterCardTransactionCount = 0;
                MasterCardTransactionSales = 0;
                OtherCardTransactionCount = 0;
                OtherCardTransactionSales = 0;
                CreditCardTransactionCount = 0;
                CreditCardTransactionSales = 0;
                DebitEBTTransactionCount = 0;
                DebitEBTTransactionSales = 0;
                CreditVoidTransactionCount = 0;
                CreditVoidTransactionSales = 0;
                DebitEBTVoidTransactionCount = 0;
                DebitEBTVoidTransactionSales = 0;
                CreditReturnTransactionCount = 0;
                CreditReturnTransactionSales = 0;
                DebitReturnTransactionCount = 0;
                DebitReturnTransactionSales = 0;
                TotalTransactionCount = 0;
                encodedRequests.Clear();

                Save();
            }
        }

        public void ReportDataCollect(TransactionType transactionType, PaymentMethodType paymentMethodType, decimal amount, string encodedRequest) {
            lock (objectLock) {
                System.Diagnostics.Debug.WriteLine($"PMI:\n{priorMessageInformation?.MessageTransactionIndicator} | {priorMessageInformation?.FunctionCode}");

                TotalTransactionCount += 1;
                encodedRequests.Add(encodedRequest);
                switch (transactionType) {
                    case TransactionType.Capture:
                    case TransactionType.Sale: {
                            if (paymentMethodType.Equals(PaymentMethodType.Credit)) {
                                CardType = "CT ";
                                CreditCardTransactionCount += 1;
                                CreditCardTransactionSales += amount;
                            }                            
                            else if (paymentMethodType.Equals(PaymentMethodType.Debit) || paymentMethodType.Equals(PaymentMethodType.EBT)) {
                                CardType = "DB ";
                                DebitEBTTransactionCount += 1;
                                DebitEBTTransactionSales += amount;
                            }                            
                            else {
                                CardType = "OH ";
                                OtherCardTransactionCount += 1;
                                OtherCardTransactionSales += amount;
                                CreditCardTransactionCount += 1;
                                CreditCardTransactionSales += amount;
                            }
                        }
                        break;
                    case TransactionType.Reversal: {
                            if (priorMessageInformation.MessageTransactionIndicator == "1100" && priorMessageInformation.FunctionCode == "101") {
                                // We don't want to reverse authorizations
                                break;
                            }

                            if (paymentMethodType.Equals(PaymentMethodType.Credit)) {
                                CardType = "CT ";
                                CreditCardTransactionCount -= 1;
                                CreditCardTransactionSales -= amount;
                            }
                            else if (paymentMethodType.Equals(PaymentMethodType.Debit) || paymentMethodType.Equals(PaymentMethodType.EBT)) {
                                CardType = "DB ";
                                DebitEBTTransactionCount -= 1;
                                DebitEBTTransactionSales -= amount;
                            }                            
                            else {
                                CardType = "OH ";
                                OtherCardTransactionCount -= 1;
                                OtherCardTransactionSales -= amount;
                                CreditCardTransactionCount -= 1;
                                CreditCardTransactionSales -= amount;
                            }
                        }
                        break;
                    case TransactionType.Refund: {
                            if (paymentMethodType.Equals(PaymentMethodType.Credit)) {
                                CardType = "CT ";
                                CreditReturnTransactionCount += 1;
                                CreditReturnTransactionSales += amount;
                            }
                            else if (paymentMethodType.Equals(PaymentMethodType.Debit) || paymentMethodType.Equals(PaymentMethodType.EBT)) {
                                CardType = "DB ";
                                DebitReturnTransactionCount += 1;
                                DebitReturnTransactionSales += amount;
                            }
                            else {
                                CardType = "OH ";
                                CreditReturnTransactionCount += 1;
                                CreditReturnTransactionSales += amount;
                            }
                        }
                        break;
                    case TransactionType.Void: {
                            if (priorMessageInformation.MessageTransactionIndicator == "1100" && (priorMessageInformation.FunctionCode == "101" || priorMessageInformation.FunctionCode == "100")) {
                                if (paymentMethodType.Equals(PaymentMethodType.Credit)) {
                                    CardType = "CT ";
                                    CreditVoidTransactionCount += 1;
                                    CreditVoidTransactionSales += amount;
                                }
                                else if (paymentMethodType.Equals(PaymentMethodType.Debit) || paymentMethodType.Equals(PaymentMethodType.EBT)) {
                                    CardType = "DB ";
                                    DebitEBTVoidTransactionCount += 1;
                                    DebitEBTVoidTransactionSales += amount;
                                }
                                else {
                                    CardType = "OH ";
                                    CreditVoidTransactionCount += 1;
                                    CreditVoidTransactionSales += amount;
                                }

                                break;
                            }

                            bool priorTransactionWasReturn = false;
                            if (priorMessageInformation.MessageTransactionIndicator == "1220" && priorMessageInformation.FunctionCode == "200") {
                                // This should mean the last transaction was a Return/Refund
                                priorTransactionWasReturn = true;
                            }

                            if (paymentMethodType.Equals(PaymentMethodType.Credit)) {
                                CardType = "CT ";
                                CreditVoidTransactionCount += 1;
                                CreditVoidTransactionSales += amount;

                                if (priorTransactionWasReturn) {
                                    CreditReturnTransactionCount -= 1;
                                    CreditReturnTransactionSales -= amount;
                                }
                                else {
                                    CreditCardTransactionCount -= 1;
                                    CreditCardTransactionSales -= amount;
                                }
                            }
                            else if (paymentMethodType.Equals(PaymentMethodType.Debit) || paymentMethodType.Equals(PaymentMethodType.EBT)) {
                                CardType = "DB ";
                                DebitEBTVoidTransactionCount += 1;
                                DebitEBTVoidTransactionSales += amount;

                                if (priorTransactionWasReturn) {
                                    DebitReturnTransactionCount -= 1;
                                    DebitReturnTransactionSales -= amount;
                                }
                                else {
                                    DebitEBTTransactionCount -= 1;
                                    DebitEBTTransactionSales -= amount;
                                }
                            }
                        }
                        break;
                }
                Save();
            }
        }

        private void Save() {
            StreamWriter bw = null;
            try {
                bw = new StreamWriter(File.OpenWrite(fileName));
                bw.Write(string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}|{20}|{21}|{22}\r\n", 
                    sequenceNumber, batchNumber,
                    ProprietaryTransactionCount, ProprietaryTransactionSales,
                    VisaTransactionCount, VisaTransactionSales,
                    MasterCardTransactionCount,MasterCardTransactionSales,
                    OtherCardTransactionCount,OtherCardTransactionSales,
                    CreditCardTransactionCount,CreditCardTransactionSales,
                    DebitEBTTransactionCount,DebitEBTTransactionSales,
                    CreditReturnTransactionCount,CreditReturnTransactionSales,
                    DebitReturnTransactionCount,DebitReturnTransactionSales,
                    CreditVoidTransactionCount,CreditVoidTransactionSales,
                    DebitEBTVoidTransactionCount,DebitEBTVoidTransactionSales,
                    TotalTransactionCount));

                foreach (string request in encodedRequests) {
                    bw.Write(request + "\r\n");
                }
            }
            catch (IOException) { /* NOM NOM */ }
            finally {
                if (bw != null) {
                    try {
                        bw.Flush();
                        bw.Close();
                    }
                    catch (IOException) { /* NOM NOM */ }
                }
            }
        }
    }
}
