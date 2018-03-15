using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.Payroll {
    public enum EmploymentStatus {
        [Description("A")]
        Active,
        [Description("I")]
        Inactive,
        [Description("T")]
        Terminated
    }

    public enum EmploymentCategory {
        [Description("FT")]
        FullTime,
        [Description("PT")]
        PartTime
    }

    public enum MaritalStatus {
        [Description("M")]
        Married,
        [Description("S")]
        Single
    }

    public enum Gender {
        [Description("F")]
        Female,
        [Description("M")]
        Male
    }

    public enum FilterPayTypeCode {
        [Description("H")]
        Hourly,
        [Description("1099")]
        T1099,
    }

    public enum PayTypeCode {
        [Description("H")]
        Hourly,
        [Description("S")]
        Salary,
        [Description("T99")]
        T1099,
        [Description("T99H")]
        T1099_Hourly,
        [Description("C")]
        Commision,
        [Description("Ah")]
        AutoHourly,
        [Description("Ms")]
        ManualSalary
    }
}
