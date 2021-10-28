namespace GlobalPayments.Api.Entities.Enums
{
    public enum ProPayAccountStatus {
        ReadyToProcess,
        FraudAccount,
        RiskwiseDeclined,
        Hold,
        Canceled,
        FraudVictim,
        ClosedEula,
        ClosedExcessiveChargeback
    }
}
