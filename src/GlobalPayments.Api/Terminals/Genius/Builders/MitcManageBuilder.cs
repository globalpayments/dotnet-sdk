using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.Genius.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Terminals.Genius.Builders
{
    public class MitcManageBuilder : TerminalManageBuilder
    {
        public TransactionType FollowOnTransactionType;
        public bool Receipt = false;
        public bool AllowDuplicates = false;
        public TransactionType OriginalTransType;
        internal decimal? Amount { get; set; }

        internal MitcManageBuilder(TransactionType originalTransType, PaymentMethodType paymentType, TransactionType followOnTransType) : base(originalTransType, paymentType)
        {

            FollowOnTransactionType = followOnTransType;
            this.OriginalTransType = originalTransType;
        }
        public MitcManageBuilder WithAmount(decimal amount)
        {
            Amount = amount;
            return this;
        }
        public MitcManageBuilder WithAllowDuplicates(bool value)
        {
            AllowDuplicates = value;
            return this;
        }

        public bool isAllowDuplicates()
        {
            return AllowDuplicates;
        }

        public MitcManageBuilder WithReceipt(bool value)
        {
            Receipt = value;
            return this;
        }

        public override byte[] Serialize(string configName = "default")
        {
            throw new NotImplementedException();
        }
        
        public override ITerminalResponse Execute(string configName)
        {
            GeniusController device = (GeniusController)ServicesContainer.Instance.GetDeviceController(configName);            
            try
            {
                return device.ManageTransaction(this);
            }
            catch (ApiException e)
            {
                throw e;
            }
           
        }
    }
}
