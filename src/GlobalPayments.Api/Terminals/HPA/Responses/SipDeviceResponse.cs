using System;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Terminals.HPA.Responses {
    public class SipDeviceResponse : SipBaseResponse, IDeviceResponse {
        public SipDeviceResponse(byte[] buffer, params string[] messageIds) : base(buffer, messageIds) { }

        internal override void MapResponse(Element response) {
            base.MapResponse(response);

            ApprovalCode = AuthorizationCode = response.GetValue<string>("ApprovalCode");
            AmountDue = response.GetValue<string>("BalanceDueAmount").ToAmount();
            AvsResponseCode = response.GetValue<string>("AVS");
            AvsResponseText = response.GetValue<string>("AVSRsltText", "AVSResultText");
            TransactionType = response.GetValue<string>("CardGroup");
            BalanceAmount = response.GetValue<string>("AvailableBalance", "BalanceReturned").ToAmount();
            CardHolderName = response.GetValue<string>("CardHolderName");
            CvvResponseCode = response.GetValue<string>("CVV");
            CvvResponseText = response.GetValue<string>("CVVRsltText", "CVVResultText");
            EntryMethod = response.GetValue<string>("CardAcquisition");
            MaskedCardNumber = response.GetValue<string>("MaskedPAN");
            PaymentType = response.GetValue<string>("CardType");
            // PinVerified
            // QPSQualified
            ResponseCode = NormalizeResponse(response.GetValue<string>("ResponseCode", "Result"));
            TransactionId = response.GetValue<string>("ResponseId", "TransactionId");
            ResponseText = response.GetValue<string>("ResponseText", "ResultText");
            SignatureStatus = response.GetValue<string>("SignatureLine");
            // StoreAndForward
            // TipAdjustAllowed
            TerminalRefNumber = response.GetValue<string>("ReferenceNumber");
            TransactionAmount = response.GetValue<string>("AuthorizedAmount").ToAmount();

            // EMV
            ApplicationId = response.GetValue<string>("EMV_AID");
            ApplicationLabel = response.GetValue<string>("EMV_ApplicationName");
            ApplicationCryptogram = response.GetValue<string>("EMV_Cryptogram");
            if(response.Has("EMV_CryptogramType"))
                ApplicationCryptogramType = (ApplicationCryptogramType)Enum.Parse(typeof(ApplicationCryptogramType), response.GetValue<string>("EMV_CryptogramType"));
            CardHolderVerificationMethod = response.GetValue<string>("EMV_TSI");
            TerminalVerificationResults = response.GetValue<string>("EMV_TVR");

            // Signature
            var attachmentData = response.GetValue<string>("AttachmentData");
            if(!string.IsNullOrEmpty(attachmentData))
                SignatureData = Convert.FromBase64String(attachmentData);
        }
    }
}
