using System.Linq;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlobalPayments.Api.Entities.Payroll;
using System;

namespace GlobalPayments.Api.Tests.Payroll {
    [TestClass]
    public class PayrollTests {
        private PayrollService _service;

        public PayrollTests() {
            _service = new PayrollService(new PayrollConfig {
                Username = "testapiuser.russell",
                Password = "payroll3!",
                ApiKey = "iGF9UtaLc526poWWNgUpiCoO3BckcZUKNF3nhyKul8A=",
                Timeout = -1
            });
        }

        [TestMethod]
        public void GetClientInfoTest() {
            var clients = _service.GetClientInfo(578901244);
            Assert.IsNotNull(clients);
            if (clients.Count > 0) {
                Assert.IsNotNull(clients[0].ClientCode);
                Assert.IsNotNull(clients[0].ClientName);
                Assert.AreEqual(578901244, clients[0].FederalEin);
            }
        }

        [TestMethod]
        public void GetActiveEmployeesTest() {
            var employees = _service.GetEmployees("0140SY42")
                .ActiveOnly(true)
                .Find();
            Assert.IsNotNull(employees);
            Assert.IsFalse(employees.Any(p => p.EmploymentStatus != EmploymentStatus.Active));
        }

        [TestMethod]
        public void GetInactiveEmployeesTest() {
            var employees = _service.GetEmployees("0140SY42")
                .ActiveOnly(false)
                .Find();
            Assert.IsNotNull(employees);
            Assert.IsTrue(employees.Any(p => p.EmploymentStatus != EmploymentStatus.Active));
        }

        [TestMethod]
        public void Get20EmployeesTest() {
            var employees = _service.GetEmployees("0140SY42")
                .ActiveOnly(false)
                .WithEmployeeCount(20)
                .Find();
            Assert.IsNotNull(employees);
            Assert.IsTrue(employees.Count == 20);
        }

        [TestMethod]
        public void GetEmployeesInDateRangeTest() {
            var employees = _service.GetEmployees("0140SY42")
                .WithFromDate(DateTime.Parse("01/01/2014"))
                .WithToDate(DateTime.Parse("01/01/2015"))
                .Find();
            Assert.IsNotNull(employees);
            Assert.IsFalse(false);
        }

        [TestMethod]
        public void GetHourlyEmployeesTest() {
            var employees = _service.GetEmployees("0140SY42")
                .WithPayType(FilterPayTypeCode.Hourly)
                .Find();
            Assert.IsNotNull(employees);
            Assert.IsFalse(employees.Any(p => p.PayTypeCode != PayTypeCode.Hourly));
        }

        [TestMethod]
        public void Get1099EmployeesTest() {
            var employees = _service.GetEmployees("0140SY42")
                .WithPayType(FilterPayTypeCode.T1099)
                .Find();
            Assert.IsNotNull(employees);
            Assert.IsFalse(employees.Any(p => p.PayTypeCode == PayTypeCode.Hourly));
        }

        [TestMethod]
        public void GetSingleEmployeeTest() {
            var employees = _service.GetEmployees("0140SY42")
                .WithEmployeeId(284045)
                .Find();
            Assert.IsNotNull(employees);
            Assert.AreEqual(1, employees.Count);
        }

        [TestMethod]
        public void UpdateEmployeeTest() {
            var employee = _service.GetEmployees("0140SY42")
                .ActiveOnly(true)
                .Find().FirstOrDefault();
            Assert.IsNotNull(employee);

            var status = MaritalStatus.Married;
            if (employee.MaritalStatus == MaritalStatus.Married)
                status = MaritalStatus.Single;
            employee.MaritalStatus = status;

            var response = _service.UpdateEmployee(employee);
            Assert.IsNotNull(response);
            Assert.AreEqual(status, employee.MaritalStatus);
        }

        [TestMethod]
        public void GetTerminationReasonsTest() {
            var terminationReasons = _service.GetTerminationReasons("0140SY42");
            Assert.IsNotNull(terminationReasons);
            Assert.IsTrue(terminationReasons.Count > 0);
        }

        [TestMethod]
        public void TerminateEmployeeTest() {
            var employee = _service.GetEmployees("0140SY42")
                .WithEmployeeId(284045)
                .Find().FirstOrDefault();
            Assert.IsNotNull(employee);

            var terminationReason = _service.GetTerminationReasons("0140SY42").FirstOrDefault();
            Assert.IsNotNull(terminationReason);

            var terminationDate = DateTime.Now;
            var response = _service.TerminateEmployee(employee, terminationDate, terminationReason, false);
            Assert.IsNotNull(response);
            Assert.AreEqual(terminationDate, employee.TerminationDate);
            Assert.AreEqual(response.EmploymentStatus, EmploymentStatus.Terminated);
        }

        [TestMethod]
        public void GetWorLocationsTest() {
            var collection = _service.GetWorkLocations("0140SY42");
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
        }

        [TestMethod]
        public void GetLaborFieldsTest() {
            var collection = _service.GetLaborFields("0140SY42");
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count == 0);
        }

        [TestMethod]
        public void GetPayGroupsTest() {
            var collection = _service.GetPayGroups("0140SY42");
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
        }

        [TestMethod]
        public void GetPayItemsTest() {
            var collection = _service.GetPayItems("0140SY42");
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
        }

        [TestMethod]
        public void PostPayDataTest() {
            var payItem = _service.GetPayItems("0140SY42").FirstOrDefault();

            var response = _service.PostPayrollData(new PayrollRecord {
                RecordId = 1,
                ClientCode = "0140SY42",
                EmployeeId = 284045,
                Hours = 80,
                PayItemTitle = payItem.Description
            });
            Assert.IsNotNull(response);
        }
    }
}
