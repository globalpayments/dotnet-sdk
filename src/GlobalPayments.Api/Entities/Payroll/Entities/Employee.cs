using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.Entities.Payroll {
    public class Employee : PayrollEntity {
        public string ClientCode { get; set; }
        public int EmployeeId { get; set; }
        public EmploymentStatus EmploymentStatus { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; internal set; }
        public string TerminationReasonId { get; internal set; }
        public string EmployeeNumber { get; set; }
        public EmploymentCategory? EmploymentCategory { get; set; }
        public int? TimeClockId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Ssn { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string ZipCode { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public DateTime? BirthDay { get; set; }
        public Gender? Gender { get; set; }
        public int PayGroupId { get; set; }
        public PayTypeCode PayTypeCode { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? PerPaySalary { get; set; }
        public int WorkLocationId { get; set; }
        public bool DeactivateAccounts { get; internal set; }

        internal override void FromJson(JsonDoc doc, PayrollEncoder encoder) {
            Func<object, DateTime> dateConverter = (date) => {
                return DateTime.Parse(date.ToString());
            };

            ClientCode = encoder.Decode(doc.GetValue<string>("ClientCode"));
            EmployeeId = doc.GetValue<int>("EmployeeId");
            EmploymentStatus = doc.GetValue("EmploymentStatus", EnumConverter.FromDescription<EmploymentStatus>);
            HireDate = doc.GetValue("HireDate", dateConverter);
            TerminationDate = doc.GetValue("TerminationDate", dateConverter);
            TerminationReasonId = doc.GetValue<string>("TerminationReasonId");
            EmployeeNumber = doc.GetValue<string>("EmployeeNumber");
            EmploymentCategory = doc.GetValue("EmploymentCategory", EnumConverter.FromDescription<EmploymentCategory>);
            TimeClockId = doc.GetValue<int>("TimeClockId");
            FirstName = doc.GetValue<string>("FirstName");
            LastName = encoder.Decode(doc.GetValue<string>("LastName"));
            MiddleName = doc.GetValue<string>("MiddleName");
            Ssn = encoder.Decode(doc.GetValue<string>("Ssn"));
            Address1 = encoder.Decode(doc.GetValue<string>("Address1"));
            Address2 = doc.GetValue<string>("Address2");
            City = doc.GetValue<string>("City");
            StateCode = doc.GetValue<string>("StateCode");
            ZipCode = encoder.Decode(doc.GetValue<string>("ZipCode"));
            MaritalStatus = doc.GetValue("MaritalStatus", EnumConverter.FromDescription<MaritalStatus>);

            // Birthday
            string birthday = encoder.Decode(doc.GetValue<string>("BirthDay"));
            if(!string.IsNullOrEmpty(birthday))
                BirthDay = DateTime.Parse(birthday);

            Gender = doc.GetValue("Gender", EnumConverter.FromDescription<Gender>);
            PayGroupId = doc.GetValue<int>("PayGroupId");
            PayTypeCode = doc.GetValue("PayTypeCode", EnumConverter.FromDescription<PayTypeCode>);
            HourlyRate = decimal.Parse(encoder.Decode(doc.GetValue<string>("HourlyRate")));
            PerPaySalary = decimal.Parse(encoder.Decode(doc.GetValue<string>("PerPaySalary")));
            WorkLocationId = doc.GetValue<int>("WorkLocationId");
        }

        internal PayrollRequest AddEmployeeRequest(PayrollEncoder encoder, object[] args) {
            var requestBody = new JsonDoc()
                .Set("ClientCode", encoder.Encode(ClientCode))
                .Set("EmployeeId", EmployeeId)
                .Set("EmploymentStatus", EnumConverter.GetDescription(EmploymentStatus))
                .Set("HireDate", HireDate)
                //.Set("TerminationDate", TerminationDate)
                //.Set("TerminationReasonId", TerminationReasonId)
                //.Set("EmployeeNumber", EmployeeNumber)
                .Set("EmploymentCategory", EnumConverter.GetDescription(EmploymentCategory))
                .Set("TimeClockId", TimeClockId)
                .Set("FirstName", FirstName)
                .Set("LastName", encoder.Encode(LastName))
                .Set("MiddleName", MiddleName)
                .Set("SSN", encoder.Encode(Ssn))
                .Set("Address1", encoder.Encode(Address1))
                .Set("Address2", Address2)
                .Set("City", City)
                .Set("StateCode", StateCode)
                .Set("ZipCode", encoder.Encode(ZipCode))
                .Set("MaritalStatus", EnumConverter.GetDescription(MaritalStatus))
                .Set("BirthDate", encoder.Encode(BirthDay))
                .Set("Gender", EnumConverter.GetDescription(Gender))
                .Set("PayGroupId", PayGroupId)
                .Set("PayTypeCode", EnumConverter.GetDescription(PayTypeCode))
                .Set("HourlyRate", encoder.Encode(HourlyRate))
                .Set("PerPaySalary", encoder.Encode(PerPaySalary))
                .Set("WorkLocationId", WorkLocationId).ToString();

            return new PayrollRequest {
                Endpoint = @"/api/pos/employee/AddEmployee",
                RequestBody = requestBody
            };
        }

        internal PayrollRequest UpdateEmployeeRequest(PayrollEncoder encoder, object[] args) {
            var requestBody = new JsonDoc()
                .Set("ClientCode", encoder.Encode(ClientCode))
                .Set("EmployeeId", EmployeeId)
                .Set("EmploymentStatus", EnumConverter.GetDescription(EmploymentStatus))
                .Set("HireDate", HireDate)
                //.Set("TerminationDate", TerminationDate)
                //.Set("TerminationReasonId", TerminationReasonId)
                .Set("EmployeeNumber", EmployeeNumber)
                .Set("EmploymentCategory", EnumConverter.GetDescription(EmploymentCategory))
                .Set("TimeClockId", TimeClockId)
                .Set("FirstName", FirstName)
                .Set("LastName", encoder.Encode(LastName))
                .Set("MiddleName", MiddleName)
                .Set("SSN", encoder.Encode(Ssn))
                .Set("Address1", encoder.Encode(Address1))
                .Set("Address2", Address2)
                .Set("City", City)
                .Set("StateCode", StateCode)
                .Set("ZipCode", encoder.Encode(ZipCode))
                .Set("MaritalStatus", EnumConverter.GetDescription(MaritalStatus))
                .Set("BirthDate", encoder.Encode(BirthDay))
                .Set("Gender", EnumConverter.GetDescription(Gender))
                .Set("PayGroupId", PayGroupId)
                .Set("PayTypeCode", EnumConverter.GetDescription(PayTypeCode))
                .Set("HourlyRate", encoder.Encode(HourlyRate))
                .Set("PerPaySalary", encoder.Encode(PerPaySalary))
                .Set("WorkLocationId", WorkLocationId).ToString();

            return new PayrollRequest {
                Endpoint = @"/api/pos/employee/UpdateEmployee",
                RequestBody = requestBody
            };
        }

        internal PayrollRequest TerminateEmployeeRequest(PayrollEncoder encoder, object[] args) {
            var requestBody = new JsonDoc()
                .Set("ClientCode", encoder.Encode(ClientCode))
                .Set("EmployeeId", EmployeeId)
                .Set("TerminationDate", TerminationDate?.ToString("MM/dd/yyyy"))
                .Set("TerminationReasonId", TerminationReasonId)
                .Set("InactivateDirectDepositAccounts", DeactivateAccounts ? 1 : 0)
                .ToString();

            return new PayrollRequest {
                Endpoint = @"/api/pos/employee/TerminateEmployee",
                RequestBody = requestBody
            };
        }
    }

    public class EmployeeFilter : PayrollEntity {
        public string ClientCode { get; set; }
        public int? EmployeeId { get; set; }
        public bool Active { get; set; }
        public int? EmployeeOffset { get; set; }
        public int? EmployeeCount { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public FilterPayTypeCode? PayTypeCode { get; set; }

        internal override void FromJson(JsonDoc doc, PayrollEncoder encoder) {
            throw new NotImplementedException();
        }

        internal PayrollRequest GetEmployeeRequest(PayrollEncoder encoder, object[] args) {
            var requestBody = new JsonDoc()
                .Set("ClientCode", encoder.Encode(ClientCode))
                .Set("EmployeeId", EmployeeId)
                .Set("ActiveEmployeeOnly", Active)
                .Set("EmployeeOffset", EmployeeOffset)
                .Set("EmployeeCount", EmployeeCount)
                .Set("FromDate", FromDate?.ToString("MM/dd/yyyy HH:mm:ss"))
                .Set("ToDate", ToDate?.ToString("MM/dd/yyyy HH:mm:ss"))
                .Set("PayTypeCode", EnumConverter.GetDescription(PayTypeCode))
                .ToString();

            return new PayrollRequest {
                Endpoint = (EmployeeId != null) ? "/api/pos/employee/GetEmployee" : "/api/pos/employee/GetEmployees",
                RequestBody = requestBody
            };
        }
    }
}
