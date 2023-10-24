using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Billing;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Xml;

namespace GlobalPayments.Api.Gateways.BillPay {
    internal sealed class TokenInformationRequestResponse : BillPayResponseBase<Transaction> {
        public override Transaction Map() {
            var tokenDetailsElement = response.Get("a:TokenDetails");
            var accountHolderDataElement = tokenDetailsElement.Get("a:AccountHolderData");
            var merchantsElement = tokenDetailsElement.Get("a:Merchants");

            return new Transaction
            {
                ResponseCode = response.GetValue<string>("a:ResponseCode"),
                ResponseMessage = GetFirstResponseMessage(response),
                Address = new Address
                {
                    StreetAddress1 = accountHolderDataElement.GetValue<string>("b:Address"),
                    City = accountHolderDataElement.GetValue<string>("b:City"),
                    State = accountHolderDataElement.GetValue<string>("b:State"),
                    PostalCode = accountHolderDataElement.GetValue<string>("b:Zip"),
                    Country = accountHolderDataElement.GetValue<string>("b:Country"),
                },
                CustomerData = new Customer
                {
                    Company = accountHolderDataElement.GetValue<string>("b:BusinessName"),
                    FirstName = accountHolderDataElement.GetValue<string>("b:FirstName"),
                    LastName = accountHolderDataElement.GetValue<string>("b:LastName"),
                    MiddleName = accountHolderDataElement.GetValue<string>("b:MiddleName"),
                    WorkPhone = accountHolderDataElement.GetValue<string>("b:Phone"),
                    // PhoneRegionCode = accountHolderDataElement.GetValue<string>("b:PhoneRegionCode"),
                },
                CardholderName = accountHolderDataElement.GetValue<string>("b:NameOnCard"),
                CardExpMonth = tokenDetailsElement.GetValue<int>("a:ExpirationMonth"),
                CardExpYear = tokenDetailsElement.GetValue<int>("a:ExpirationYear"),
                CardLast4 = tokenDetailsElement.GetValue<string>("a:Last4"),
                PaymentMethodType = SetPaymentMethodType(tokenDetailsElement.GetValue<string>("a:PaymentMethod")),
                CardType = GetCardType(tokenDetailsElement.GetValue<string>("a:PaymentMethod")),
                Token = tokenDetailsElement.GetValue<string>("a:Token"),
                TokenData = new TokenData
                {
                    IsExpired = tokenDetailsElement.GetValue<bool>("a:IsExpired"),
                    LastUsedDateUTC = XmlConvert.ToDateTime(tokenDetailsElement.GetValue<string>("a:LastUsedDateUTC"), XmlDateTimeSerializationMode.Utc),
                    Merchants = PopulateMerchantListFromElement(merchantsElement),
                    ShareTokenWithGroup = tokenDetailsElement.GetValue<bool>("a:ShareTokenWithGroup"),
                },
            };
        }

        private List<string> PopulateMerchantListFromElement(Element merchantsElement) {
            if (merchantsElement.GetElement().ChildNodes.Count > 0) {
                List<string> merchantList = new List<string>();
                foreach (XmlNode node in merchantsElement.GetElement().ChildNodes) {
                    merchantList.Add(node.InnerText);
                }

                return merchantList;
            }

            return null;
        }
    }
}
