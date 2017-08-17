using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.TableService;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Reservations {
    [TestClass]
    public class TableServiceTests {
        private TableService _service;

        public TableServiceTests() {
            _service = new TableService(new ServicesConfig {
                ReservationProvider = ReservationProviders.FreshTxt
            });
        }

        [TestInitialize]
        public void Login() {
            var response = _service.Login("globa10", "glob8859");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void AssignCheck() {
            var response = _service.AssignCheck(1, 1);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void OpenOrder() {
            var assignedCheck = Ticket.FromId(1, 1);

            var orderOpened = assignedCheck.OpenOrder();
            Assert.IsNotNull(orderOpened);
            Assert.AreEqual("00", orderOpened.ResponseCode);
        }

        [TestMethod]
        public void BumpStatus() {
            var assignedCheck = Ticket.FromId(1, 1);

            var statusResponse = assignedCheck.BumpStatus(_service.BumpStatuses[0]);
            Assert.IsNotNull(statusResponse);
            Assert.AreEqual("00", statusResponse.ResponseCode);
            Assert.AreEqual(assignedCheck.BumpStatusId, 1);
        }

        [TestMethod, ExpectedException(typeof(MessageException))]
        public void BumpStatusWithBadStatus() {
            Ticket.FromId(1, 1).BumpStatus("badStatus");
        }

        [TestMethod]
        public void SettleCheck() {
            var assignedCheck = Ticket.FromId(1, 1);

            var response = assignedCheck.SettleCheck();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void SettleCheckWithStatus() {
            var assignedCheck = Ticket.FromId(1, 1);

            var response = assignedCheck.SettleCheck(_service.BumpStatuses[1]);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual(assignedCheck.BumpStatusId, 2);
        }

        [TestMethod, ExpectedException(typeof(MessageException))]
        public void SettleCheckWithBadStatus() {
            Ticket.FromId(1, 1).SettleCheck("badStatus");
        }

        [TestMethod]
        public void ClearTable() {
            var assignedCheck = Ticket.FromId(1, 1);

            var response = assignedCheck.ClearTable();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void QueryTableStatus() {
            var response = _service.QueryTableStatus(1);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void QueryCheckStatus() {
            var response = _service.QueryCheckStatus(1);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EditTable() {
            var assignedCheck = Ticket.FromId(1, 1);
            assignedCheck.PartyName = "Party Of One";
            assignedCheck.PartyNumber = 1;
            assignedCheck.Section = "Lonley Section";

            var response = assignedCheck.Update();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void Transfer() {
            var assignedCheck = Ticket.FromId(1, 1);

            var response = assignedCheck.Transfer(2);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GetServerList() {
            var response = _service.GetServerList();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.Servers);
        }

        [TestMethod]
        public void UpdateServerList() {
            var servers = new string[] {
                "Russ", "Shane", "Mark", "Salina"
            };

            var response = _service.UpdateServerList(servers);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            Assert.IsNotNull(response.Servers);
            Assert.AreEqual(4, response.Servers.Count);
            Assert.AreEqual("Russ,Shane,Mark,Salina", string.Join(",", response.Servers));
        }

        [TestMethod]
        public void GetAllServerAssignments() {
            var response = _service.GetServerAssignments();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GetServerAssignmentsByServer() {
            var response = _service.GetServerAssignments("Russell");
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void GetTableAssignmentsByTable() {
            var response = _service.GetServerAssignments(1);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void AssignShifts() {
            var assignments = new ShiftAssignments();
            assignments.Add("Russell", new int[] { 1, 2, 3, 4 });
            assignments.Add("Shane", new int[] { 200, 201, 202, 203 });
            assignments.Add("Mark", new int[] { 304, 305, 306 });
            assignments.Add("Salina", new int[] { 409, 408, 409 });

            var response = _service.AssignShift(assignments);
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // check return values
            Assert.IsNotNull(response.Assignments);
            Assert.IsTrue(response.Assignments.ContainsKey("Russell"));
            foreach (var table in response.Assignments["Russell"]) {
                Assert.IsTrue(new List<int>() { 1, 2, 3, 4 }.Contains(table));
            }

            Assert.IsTrue(response.Assignments.ContainsKey("Shane"));
            foreach (var table in response.Assignments["Shane"]) {
                Assert.IsTrue(new List<int>() { 200, 201, 202, 203 }.Contains(table));
            }

            Assert.IsTrue(response.Assignments.ContainsKey("Mark"));
            foreach (var table in response.Assignments["Mark"]) {
                Assert.IsTrue(new List<int>() { 304, 305, 306 }.Contains(table));
            }

            Assert.IsTrue(response.Assignments.ContainsKey("Salina"));
            foreach (var table in response.Assignments["Salina"]) {
                Assert.IsTrue(new List<int>() { 409, 408, 409 }.Contains(table));
            }
        }
    }
}
