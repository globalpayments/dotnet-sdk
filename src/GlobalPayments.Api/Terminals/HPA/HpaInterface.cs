using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.HPA.Interfaces;
using GlobalPayments.Api.Terminals.HPA.Responses;
using GlobalPayments.Api.Terminals.Messaging;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Terminals.HPA
{
    public class HpaInterface : IDeviceInterface
    {
        private HpaController _controller;
        private IRequestIdProvider _requestIdProvider;
        public event MessageSentEventHandler OnMessageSent;

        internal HpaInterface(HpaController controller)
        {
            _controller = controller;
            _controller.OnMessageSent += (message) =>
            {
                OnMessageSent?.Invoke(message);
            };
            _requestIdProvider = _controller.RequestIdProvider;
        }

        #region Admin Messages
        public void Cancel()
        {
            var response = Reset();
           
        }

        public IDeviceResponse CloseLane()
        {
            return _controller.SendMessage<SipBaseResponse>(string.Format("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>LaneClose</Request><RequestId>{0}</RequestId></SIP>", _requestIdProvider.GetRequestId()), HPA_MSG_ID.LANE_CLOSE);
        }

        public IDeviceResponse DisableHostResponseBeep()
        {
            throw new NotImplementedException();
        }

        public ISignatureResponse GetSignatureFile()
        {
            throw new UnsupportedTransactionException("Signature data for this device type is automatically returned in the terminal response.");
        }

        public IInitializeResponse Initialize()
        {
            return _controller.SendMessage<SipInitializeResponse>(string.Format("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>GetAppInfoReport</Request><RequestId>{0}</RequestId></SIP>", _requestIdProvider.GetRequestId()), HPA_MSG_ID.GET_INFO_REPORT);
        }

        public IDeviceResponse OpenLane()
        {
           return _controller.SendMessage<SipBaseResponse>(string.Format("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>LaneOpen</Request><RequestId>{0}</RequestId></SIP>", _requestIdProvider.GetRequestId()), HPA_MSG_ID.LANE_OPEN);
        }

        public ISignatureResponse PromptForSignature(string transactionId = null)
        {
            return _controller.SendMessage<SipSignatureResponse>("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>SignatureForm</Request><FormText>PLEASE SIGN YOUR NAME</FormText></SIP>", HPA_MSG_ID.SIGNATURE_FORM);
        }

        public IDeviceResponse Reboot()
        {
            return _controller.SendMessage<SipBaseResponse>(string.Format("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>Reboot</Request><RequestId>{0}</RequestId></SIP>", _requestIdProvider.GetRequestId()), HPA_MSG_ID.REBOOT);
        }

        public IDeviceResponse Reset()
        {
            return _controller.SendMessage<SipBaseResponse>(string.Format("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>Reset</Request><RequestId>{0}</RequestId></SIP>", _requestIdProvider.GetRequestId()), HPA_MSG_ID.RESET);
        }

        public string SendCustomMessage(DeviceMessage message)
        {
            var response = _controller.Send(message);
            return Encoding.UTF8.GetString(response);
        }

        public IDeviceResponse StartCard(PaymentMethodType paymentMethodType)
        {
            return _controller.SendMessage<SipBaseResponse>(string.Format("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>StartCard</Request><RequestId>{0}</RequestId><CardGroup>{1}</CardGroup></SIP>", _requestIdProvider.GetRequestId(), paymentMethodType), HPA_MSG_ID.STARTCARD);
        }
        #endregion

        #region Batching
        public IBatchCloseResponse BatchClose()
        {
            return _controller.SendMessage<SipBatchResponse>("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>CloseBatch</Request></SIP>", HPA_MSG_ID.BATCH_CLOSE, HPA_MSG_ID.GET_BATCH_REPORT);
        }
        #endregion

        #region Credit
        public TerminalAuthBuilder CreditAuth(decimal? amount = default(decimal?))
        {
            return new TerminalAuthBuilder(TransactionType.Auth, PaymentMethodType.Credit).WithAmount(amount);
        }

        public TerminalManageBuilder CreditCapture(decimal? amount = default(decimal?))
        {
            return new TerminalManageBuilder(TransactionType.Capture, PaymentMethodType.Credit).WithAmount(amount);
        }

        public TerminalAuthBuilder CreditRefund(decimal? amount = default(decimal?))
        {
            return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.Credit).WithAmount(amount);
        }

        public TerminalAuthBuilder CreditSale(decimal? amount = default(decimal?))
        {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Credit).WithAmount(amount);
        }

        public TerminalAuthBuilder CreditVerify()
        {
            return new TerminalAuthBuilder(TransactionType.Verify, PaymentMethodType.Credit);
        }

        public TerminalManageBuilder CreditVoid()
        {
            return new TerminalManageBuilder(TransactionType.Void, PaymentMethodType.Credit);
        }
        #endregion

        #region Debit
        public TerminalAuthBuilder DebitSale(decimal? amount = null)
        {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Debit).WithAmount(amount);
        }

        public TerminalAuthBuilder DebitRefund(decimal? amount = null)
        {
            return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.Debit).WithAmount(amount);
        }
        #endregion

        #region Gift & Loyalty
        public TerminalAuthBuilder GiftSale(decimal? amount = null)
        {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.Gift).WithAmount(amount).WithCurrency(CurrencyType.CURRENCY);
        }

        public TerminalAuthBuilder GiftAddValue(decimal? amount = null)
        {
            return new TerminalAuthBuilder(TransactionType.AddValue, PaymentMethodType.Gift)
                
                .WithCurrency(CurrencyType.CURRENCY)
                .WithAmount(amount);
        }

        public TerminalManageBuilder GiftVoid()
        {
            return new TerminalManageBuilder(TransactionType.Void, PaymentMethodType.Gift).WithCurrency(CurrencyType.CURRENCY);
        }

        public TerminalAuthBuilder GiftBalance()
        {
            return new TerminalAuthBuilder(TransactionType.Balance, PaymentMethodType.Gift).WithCurrency(CurrencyType.CURRENCY);
        }
        #endregion

        #region EBT Methods
        public TerminalAuthBuilder EbtBalance()
        {
            return new TerminalAuthBuilder(TransactionType.Balance, PaymentMethodType.EBT);
        }

        public TerminalAuthBuilder EbtPurchase(decimal? amount = null)
        {
            return new TerminalAuthBuilder(TransactionType.Sale, PaymentMethodType.EBT).WithAmount(amount);
        }

        public TerminalAuthBuilder EbtRefund(decimal? amount = null)
        {
            return new TerminalAuthBuilder(TransactionType.Refund, PaymentMethodType.EBT).WithAmount(amount);
        }
        public TerminalAuthBuilder EbtWithdrawl(decimal? amount = null)
        {
            throw new UnsupportedTransactionException("This transaction is not currently supported for this payment type.");
        }
        #endregion
        public void Dispose()
        {
            CloseLane();
            _controller.Dispose();
        }
    }
}
