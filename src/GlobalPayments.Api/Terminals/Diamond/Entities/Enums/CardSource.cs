namespace GlobalPayments.Api.Terminals.Diamond.Entities.Enums {
    public enum CardSource {
        CONTACTLESS = 'B',
        MANUAL = 'M',
        MAGSTRIPE = 'C',
        ICC = 'P',
        UNKNOWN = '?'
    }
}
