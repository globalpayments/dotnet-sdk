using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.HPA.Responses {
    internal class SipBatchResponse : SipBaseResponse, IBatchCloseResponse {
        public string SequenceNumber { get; set; }
        public string TotalCount { get; set; }
        public string TotalAmount { get; set; }

        public SipBatchResponse(byte[] buffer, params string[] messageIds) : base(buffer, messageIds) { }
        internal override void MapResponse(Element response) {
            base.MapResponse(response);

            if (response.Has("BatchSeqNbr")) {
                ResponseCode = response.GetValue<string>("ResponseCode", "Result");
                ResponseText = response.GetValue<string>("ResponseText", "ResultText");
                SequenceNumber = response.GetValue<string>("BatchSeqNbr");
                TotalAmount = response.GetValue<string>("BatchTxnAmt");
                TotalCount = response.GetValue<string>("BatchTxnCnt");
            }

            Element[] cardRecords = response.GetAll("CardSummaryRecord");
            foreach (Element record in cardRecords) {
                // TODO: Handle the card records
            }

            Element[] transactionRecords = response.GetAll("TransactionSummaryReport");
            foreach (Element record in transactionRecords) {
                // TODO: Handle the transaction records
            }

            // TODO: Handle the detail responses
            Element details = response.Get("BatchDetailRecord");
            //details.GetValue<string>("ReferenceNumber");
            //details.GetValue<string>("TransactionTime");
            //details.GetValue<string>("MaskedPAN");
            //details.GetValue<string>("CardType");
            //details.GetValue<string>("TransactionType");
            //details.GetValue<string>("CardAcquisition");
            //details.GetValue<string>("ApprovalCode");
            //details.GetValue<string>("BalanceReturned");
            //details.GetValue<string>("BaseAmount");
            //details.GetValue<string>("CashbackAmount");
            //details.GetValue<string>("TaxAmount");
            //details.GetValue<string>("TipAmount");
            //details.GetValue<string>("DonationAmount");
            //details.GetValue<string>("TotalAmount");

            // TODO: Store and Forward
            /*
            <ApprovedSAFSummaryRecord>
                <NumberTransactions>[Number]</NumberTransactions>
                <TotalAmount>[Amount]</TotalAmount>
            </ApprovedSAFSummaryRecord>
            <PendingSAFSummaryRecord>
                <NumberTransactions>[Number]</NumberTransactions>
                <TotalAmount>[Amount]</TotalAmount>
            </PendingSAFSummaryRecord>
            <DeclinedSAFSummaryRecord>
                <NumberTransactions>[Number]</NumberTransactions>
                <TotalAmount>[Amount]</TotalAmount>
            </DeclinedSAFSummaryRecord>
            */

            var safRecords = response.GetAll("ApprovedSAFRecord");
            foreach (Element record in safRecords) {
                /*
                <ApprovedSAFRecord>
                <ReferenceNumber>[Number]</ ReferenceNumber>
                <TransactionTime>[Number]</ TransactionTime>
                <MaskedPAN>[Number]</ MaskedPAN>
                <CardType>[Card Type]</CardType>
                <TransactionType>[Transaction Type]</TransactionType>
                <CardAcquisition>[Card Acquisition]</CardAcquisition>
                <ApprovalCode>[Code]</ApprovalCode>
                <BaseAmount>[Amount]</BaseAmount>
                <TaxAmount>[Amount]</TaxAmount>
                <TaxAmount>[Amount]</TaxAmount>
                <TipAmount>[Amount]</TipAmount>
                <TotalAmount>[Amount]</TotalAmount>
                </ApprovedSAFRecord>
                */
            }
        }
    }
}
