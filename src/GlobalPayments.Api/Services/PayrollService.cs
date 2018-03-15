using System;
using System.Linq;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Entities.Payroll;
using System.Collections.Generic;

namespace GlobalPayments.Api.Services {
    public class PayrollService : IDisposable {
        private string _configName;
        private PayrollConnector _connector;

        public PayrollService(PayrollConfig config, string configName = "default") {
            _configName = configName;
            ServicesContainer.ConfigureService(config, configName);

            // set the connector and sign in
            _connector = ServicesContainer.Instance.GetPayrollClient(configName);
            _connector.SignIn();
        }

        public List<ClientInfo> GetClientInfo(int federalEin) {
            var request = new ClientInfo {
                FederalEin = federalEin
            };

            var response = _connector.SendEncryptedRequest<ClientInfo>(request.GetClientInfoRequest);
            return response?.Results;
        }

        // GetEmployees
        public EmployeeFinder GetEmployees(string clientCode) {
            return new EmployeeFinder(_connector).WithClientCode(clientCode);
        }

        // AddEmployee
        public Employee AddEmployee(Employee employee) {
            var response = _connector.SendEncryptedRequest<Employee>(employee.AddEmployeeRequest);
            return response?.Results[0];
        }

        // UpdateEmployee
        public Employee UpdateEmployee(Employee employee) {
            var response = _connector.SendEncryptedRequest<Employee>(employee.UpdateEmployeeRequest);
            return response?.Results[0];
        }

        // TerminateEmployee
        public Employee TerminateEmployee(Employee employee, DateTime terminationDate, TerminationReason terminationReason, bool deactivateAccounts = false) {
            employee.TerminationDate = terminationDate;
            employee.TerminationReasonId = terminationReason.Id;
            employee.DeactivateAccounts = deactivateAccounts;

            _connector.SendEncryptedRequest<Employee>(employee.TerminateEmployeeRequest);
            var results = GetEmployees(employee.ClientCode)
                .WithEmployeeId(employee.EmployeeId)
                .Find();
            return results.FirstOrDefault();
        }

        // GetTerminationReasons
        public List<TerminationReason> GetTerminationReasons(string clientCode) {
            return GetTerminationReasons(new ClientInfo { ClientCode = clientCode });
        }
        public List<TerminationReason> GetTerminationReasons(ClientInfo client) {
            return GetPayrollCollectionItem<TerminationReason>(client);
        }

        // GetWorkLocations
        public List<WorkLocation> GetWorkLocations(string clientCode) {
            return GetWorkLocations(new ClientInfo { ClientCode = clientCode });
        }
        public List<WorkLocation> GetWorkLocations(ClientInfo client) {
            return GetPayrollCollectionItem<WorkLocation>(client);
        }

        // GetLaborFields
        public List<LaborField> GetLaborFields(string clientCode) {
            return GetLaborFields(new ClientInfo { ClientCode = clientCode });
        }
        public List<LaborField> GetLaborFields(ClientInfo client) {
            return GetPayrollCollectionItem<LaborField>(client);
        }

        // GetPayGroups
        public List<PayGroup> GetPayGroups(string clientCode) {
            return GetPayGroups(new ClientInfo { ClientCode = clientCode });
        }
        public List<PayGroup> GetPayGroups(ClientInfo client) {
            return GetPayrollCollectionItem<PayGroup>(client);
        }

        // GetPayItems
        public List<PayItem> GetPayItems(string clientCode) {
            return GetPayItems(new ClientInfo { ClientCode = clientCode });
        }
        public List<PayItem> GetPayItems(ClientInfo client) {
            return GetPayrollCollectionItem<PayItem>(client);
        }

        // PostPayData
        public bool PostPayrollData(params PayrollRecord[] payrollRecords) {
            return PostPayrollData(new PayrollData(payrollRecords));
        }
        public bool PostPayrollData(PayrollData payrollData) {
            var response = _connector.SendEncryptedRequest<PayrollData>(payrollData.PostPayrollRequest);
            return response != null;
        }

        private List<T> GetPayrollCollectionItem<T>(ClientInfo client) where T : PayrollCollectionItem {
            var response = _connector.SendEncryptedRequest<T>(client.GetCollectionRequestByType, typeof(T));
            return response?.Results as List<T>;
        }

        public void Dispose() {
            _connector.SignOut();
        }
    }

    public class EmployeeFinder {
        private PayrollConnector _connector;
        private EmployeeFilter _filter;

        internal EmployeeFinder(PayrollConnector connector) {
            _filter = new EmployeeFilter();
            _connector = connector;
        }

        public EmployeeFinder WithClientCode(string value) {
            _filter.ClientCode = value;
            return this;
        }
        public EmployeeFinder WithEmployeeId(int value) {
            _filter.EmployeeId = value;
            return this;
        }
        public EmployeeFinder ActiveOnly(bool value) {
            _filter.Active = value;
            return this;
        }
        public EmployeeFinder WithEmployeeOffset(int value) {
            _filter.EmployeeOffset = value;
            return this;
        }
        public EmployeeFinder WithEmployeeCount(int value) {
            _filter.EmployeeCount = value;
            return this;
        }
        public EmployeeFinder WithFromDate(DateTime value) {
            _filter.FromDate = value;
            return this;
        }
        public EmployeeFinder WithToDate(DateTime value) {
            _filter.ToDate = value;
            return this;
        }
        public EmployeeFinder WithPayType(FilterPayTypeCode value) {
            _filter.PayTypeCode = value;
            return this;
        }

        public List<Employee> Find() {
            var response = _connector.SendEncryptedRequest<Employee>(_filter.GetEmployeeRequest);
            return response.Results;
        }
    }
}
