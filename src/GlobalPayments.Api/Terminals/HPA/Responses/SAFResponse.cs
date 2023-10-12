using System;
using System.Collections.Generic;
using System.Linq;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Terminals.HPA.Responses {
    public class SAFResponse : SipBaseResponse, ISAFResponse, ITerminalReport {
        public decimal? TotalAmount { get; set; }
        public int TotalCount { get; set; }

        public Dictionary<SummaryType, SummaryResponse> Approved { get; set; }
        public Dictionary<SummaryType, SummaryResponse> Pending { get; set; }
        public Dictionary<SummaryType, SummaryResponse> Declined { get; set; }

        private string LastCategory = "";
        private TransactionSummary LastTransactionSummary;

        public SAFResponse(byte[] buffer, params string[] messageIds) : base(buffer, messageIds) { }
          
        internal override void MapResponse(Element response) {
            base.MapResponse(response);

            string category = response.GetValue<string>("TableCategory") ?? "";
            if (String.IsNullOrEmpty(category)) {
                category = LastCategory;
            }
            var fieldValues = new Dictionary<string, string>();
            foreach (Element field in response.GetAll("Field")) {
                fieldValues.Add(field.GetValue<string>("Key"), field.GetValue<string>("Value"));
            }

            if (category.EndsWith("SUMMARY", StringComparison.OrdinalIgnoreCase)) {
                SummaryType ReportSummaryType = MapSummaryType(category);

                if (ReportSummaryType != SummaryType.Unknown)
                {
                    var summary = new SummaryResponse
                    {
                        SummaryType = MapSummaryType(category),
                        Count = fieldValues.GetValue<int>("Count"),
                        Amount = fieldValues.GetAmount("Amount"),
                        TotalAmount = fieldValues.GetAmount("Total Amount"),
                        AuthorizedAmount = fieldValues.GetAmount("Authorized Amount"),
                        AmountDue = fieldValues.GetAmount("Balance Due Amount"),
                    };

                    if (category.Contains("APPROVED"))
                    {
                        if (Approved == null)
                        {
                            Approved = new Dictionary<SummaryType, SummaryResponse>();
                        }
                        Approved.Add(summary.SummaryType, summary);
                    }
                    else if (category.StartsWith("PENDING"))
                    {
                        if (Pending == null)
                        {
                            Pending = new Dictionary<SummaryType, SummaryResponse>();
                        }
                        Pending.Add(summary.SummaryType, summary);
                    }
                    else if (category.StartsWith("DECLINED"))
                    {
                        if (Declined == null)
                        {
                            Declined = new Dictionary<SummaryType, SummaryResponse>();
                        }
                        Declined.Add(summary.SummaryType, summary);
                    }
                }
            }
            else if (category.EndsWith("RECORD", StringComparison.OrdinalIgnoreCase)) {
                TransactionSummary trans;
                if (category.Equals(LastCategory, StringComparison.OrdinalIgnoreCase)) {
                    trans = LastTransactionSummary;
                }
                else {
                     trans = new TransactionSummary {
                        TransactionId = fieldValues.GetValue<string>("TransactionId"),
                        OriginalTransactionId = fieldValues.GetValue<string>("OriginalTransactionId"),
                        TransactionDate = fieldValues.GetValue<string>("TransactionTime").ToDateTime(),
                        TransactionType = fieldValues.GetValue<string>("TransactionType"),
                        MaskedCardNumber = fieldValues.GetValue<string>("MaskedPAN"),
                        CardType = fieldValues.GetValue<string>("CardType"),
                        CardEntryMethod = fieldValues.GetValue<string>("CardAcquisition"),
                        AuthCode = fieldValues.GetValue<string>("ApprovalCode"),
                        IssuerResponseCode = fieldValues.GetValue<string>("ResponseCode"),
                        IssuerResponseMessage = fieldValues.GetValue<string>("ResponseText"),
                        // BaseAmount - Not doing this one
                        TaxAmount = fieldValues.GetAmount("TaxAmount"),
                        GratuityAmount = fieldValues.GetAmount("TipAmount"),
                        Amount = fieldValues.GetAmount("RequestAmount"),
                        AuthorizedAmount = fieldValues.GetAmount("Authorized Amount"),
                        AmountDue = fieldValues.GetAmount("Balance Due Amount")
                    };
                    if (fieldValues.GetBoolean("HostTimeOut") != null) {
                        trans.HostTimeout = (bool)fieldValues.GetBoolean("HostTimeOut");
                    }
                }
                if (!(category.Equals(LastCategory))) {
                    if (category.StartsWith("APPROVED")) {
                        var summary = Approved[SummaryType.Approved];
                        summary.Transactions.Add(trans);
                    }
                    else if (category.StartsWith("PENDING")) {
                        var summary = Pending[SummaryType.Pending];
                        summary.Transactions.Add(trans);
                    }
                    else if (category.StartsWith("DECLINED")) {
                        var summary = Declined[SummaryType.Declined];
                        summary.Transactions.Add(trans);
                    }
                }
                LastTransactionSummary = trans;
            }
            LastCategory = category;
        }

        private SummaryType MapSummaryType(string category) {
            if (category.Equals(SAFReportType.APPROVED, StringComparison.OrdinalIgnoreCase)) {
                return SummaryType.Approved;
            }
            else if (category.Equals(SAFReportType.PENDING, StringComparison.OrdinalIgnoreCase)) {
                return SummaryType.Pending;
            }
            else if (category.Equals(SAFReportType.DECLINED, StringComparison.OrdinalIgnoreCase)) {
                return SummaryType.Declined;
            }
            else if (category.Equals(SAFReportType.OFFLINE_APPROVED, StringComparison.OrdinalIgnoreCase)) {
                return SummaryType.OfflineApproved;
            }
            else if (category.Equals(SAFReportType.PARTIALLY_APPROVED, StringComparison.OrdinalIgnoreCase)) {
                return SummaryType.PartiallyApproved;
            }
            else if (category.Equals(SAFReportType.APPROVED_VOID, StringComparison.OrdinalIgnoreCase)) {
                return SummaryType.VoidApproved;
            }
            else if (category.Equals(SAFReportType.PENDING_VOID, StringComparison.OrdinalIgnoreCase)) {
                return SummaryType.VoidPending;
            }
            else if (category.Equals(SAFReportType.DECLINED_VOID, StringComparison.OrdinalIgnoreCase)) {
                return SummaryType.VoidDeclined;
            }
            else if (category.Equals(SAFReportType.PROVISIONAL, StringComparison.OrdinalIgnoreCase))
            {      
                return SummaryType.Provsional;
            }
            else if (category.Equals(SAFReportType.DISCARDED, StringComparison.OrdinalIgnoreCase))
            {       
                return SummaryType.Discarded;
            }
            else if (category.Equals(SAFReportType.REVERSAL, StringComparison.OrdinalIgnoreCase))
            {         
                return SummaryType.Reversal;
            }
            else if (category.Equals(SAFReportType.EMV_DECLINED, StringComparison.OrdinalIgnoreCase))
            {     
                return SummaryType.EmvDeclined;
            }
            else if (category.Equals(SAFReportType.ATTACHMENT, StringComparison.OrdinalIgnoreCase))
            {       
                return SummaryType.Attachment;
            }
            else if (category.Equals(SAFReportType.PROVISIONAL_VOID, StringComparison.OrdinalIgnoreCase))
            {  
                return SummaryType.VoidProvisional;
            }
            else if (category.Equals(SAFReportType.DISCARDED_VOID, StringComparison.OrdinalIgnoreCase))
            {   
                return SummaryType.VoidDiscarded;
            }
            return SummaryType.Unknown;
        }
    }
}
