using GlobalPayments.Api.Terminals.Genius.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Terminals.Genius.Enums
{   

    public enum MitcRequestType 
    {
        CARD_PRESENT_SALE,
        CARD_PRESENT_REFUND,
        REPORT_SALE_GATEWAY_ID,
        REPORT_SALE_CLIENT_ID,
        REPORT_REFUND_GATEWAY_ID,
        REPORT_REFUND_CLIENT_ID,
        REFUND_BY_CLIENT_ID,
        VOID_CREDIT_SALE,
        VOID_DEBIT_SALE,
        VOID_REFUND
    }

    public static class MitcRequestTypeExtensions
    {
        public static GeniusMitcRequest.HttpMethod GetVerb(this MitcRequestType mitcRequest)
        {
            switch (mitcRequest)
            {
                case MitcRequestType.CARD_PRESENT_SALE:
                    return GeniusMitcRequest.HttpMethod.POST;
                case MitcRequestType.CARD_PRESENT_REFUND:
                    return GeniusMitcRequest.HttpMethod.POST;
                case MitcRequestType.REPORT_SALE_CLIENT_ID:
                    return GeniusMitcRequest.HttpMethod.GET;
                case MitcRequestType.REPORT_REFUND_CLIENT_ID:
                    return GeniusMitcRequest.HttpMethod.GET;
                case MitcRequestType.REFUND_BY_CLIENT_ID:
                    return GeniusMitcRequest.HttpMethod.POST;
                case MitcRequestType.VOID_CREDIT_SALE:
                    return GeniusMitcRequest.HttpMethod.PUT;
                case MitcRequestType.VOID_DEBIT_SALE:
                    return GeniusMitcRequest.HttpMethod.PUT;
                case MitcRequestType.VOID_REFUND:
                    return GeniusMitcRequest.HttpMethod.PUT;
                default:
                    throw new NotImplementedException();
            }
        }
        public static string GetURLEndpoint(this MitcRequestType mitcRequest)
        {
            switch (mitcRequest)
            {
                case MitcRequestType.CARD_PRESENT_SALE:
                    return "/cardpresent/sales";
                case MitcRequestType.CARD_PRESENT_REFUND:
                    return "/cardpresent/returns";
                case MitcRequestType.REPORT_SALE_CLIENT_ID:
                    return "/card/sales/reference_id/%s";
                case MitcRequestType.REPORT_REFUND_CLIENT_ID:
                    return "/card/returns/reference_id/%s";
                case MitcRequestType.REFUND_BY_CLIENT_ID:
                    return "/creditsales/reference_id/%s/creditreturns";
                case MitcRequestType.VOID_CREDIT_SALE:
                    return "/creditsales/reference_id/%s/voids";
                case MitcRequestType.VOID_DEBIT_SALE:
                    return "/debitsales/reference_id/%s/voids";
                case MitcRequestType.VOID_REFUND:
                    return "/creditreturns/reference_id/%s/voids";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
