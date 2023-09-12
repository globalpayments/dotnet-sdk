using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities {
    public enum ShippingMethod {        
        BILLING_ADDRESS,        
        ANOTHER_VERIFIED_ADDRESS,        
        UNVERIFIED_ADDRESS,       
        SHIP_TO_STORE,       
        DIGITAL_GOODS,        
        TRAVEL_AND_EVENT_TICKETS,       
        OTHER
    }
}
