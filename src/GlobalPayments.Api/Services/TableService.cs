using System;
using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.TableService;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Services {
    public class TableService {
        public string[] BumpStatuses {
            get {
                return ServicesContainer.Instance.GetReservationService().BumpStatusCollection.Keys;
            }
        }

        public TableService(ServicesConfig config) {
            ServicesContainer.Configure(config);
        }

        protected T SendRequest<T>(string endpoint, MultipartForm formData) where T : TableServiceResponse {
            var connector = ServicesContainer.Instance.GetReservationService();
            if (!connector.Configured && !endpoint.Equals("user/login"))
                throw new ConfigurationException("Reservation service has not been configured properly. Please ensure you have logged in first.");

            var response = connector.Call(endpoint, formData);
            return Activator.CreateInstance(typeof(T), response) as T;
        }

        public LoginResponse Login(string username, string password) {
            var content = new MultipartForm()
                .Set("username", username)
                .Set("password", password);

            var response = SendRequest<LoginResponse>("user/login", content);

            // configure the connector
            var connector = ServicesContainer.Instance.GetReservationService();
            connector.LocationId = response.LocationId;
            connector.SecurityToken = response.Token;
            connector.SessionId = response.SessionId;
            connector.BumpStatusCollection = new BumpStatusCollection(response.TableStatus);

            return response;
        }

        public Ticket AssignCheck(int tableNumber, int checkId, DateTime? startTime = null) {
            var content = new MultipartForm()
                .Set("tableNumber", tableNumber)
                .Set("checkID", checkId)
                .Set("startTime", startTime ?? DateTime.Now);

            var response = SendRequest<Ticket>("pos/assignCheck", content);
            response.TableNumber = tableNumber;
            return response;
        }

        public Ticket QueryTableStatus(int tableNumber) {
            var content = new MultipartForm()
                .Set("tableNumber", tableNumber);

            var response = SendRequest<Ticket>("pos/tableStatus", content);
            response.TableNumber = tableNumber;
            return response;
        }

        public Ticket QueryCheckStatus(int checkId) {
            var content = new MultipartForm()
                .Set("checkID", checkId);

            return SendRequest<Ticket>("pos/checkStatus", content);
        }

        public ServerListResponse GetServerList() {
            return SendRequest<ServerListResponse>("pos/getServerList", new MultipartForm());
        }

        public ServerListResponse UpdateServerList(IEnumerable<string> serverList) {
            var _serverList = string.Join(",", serverList);

            var content = new MultipartForm()
                .Set("serverList", _serverList);

            SendRequest<TableServiceResponse>("pos/updateServerList", content);
            return GetServerList();
        }

        public ServerAssignmentResponse GetServerAssignments() {
            return SendRequest<ServerAssignmentResponse>("pos/getServerAssignment", new MultipartForm());
        }
        public ServerAssignmentResponse GetServerAssignments(string serverName) {
            var content = new MultipartForm()
                .Set("serverName", serverName);

            return SendRequest<ServerAssignmentResponse>("pos/getServerAssignment", content);
        }
        public ServerAssignmentResponse GetServerAssignments(int tableNumber) {
            var content = new MultipartForm()
                .Set("tableNumber", tableNumber);

            return SendRequest<ServerAssignmentResponse>("pos/getServerAssignment", content);
        }

        public ServerAssignmentResponse AssignShift(ShiftAssignments shiftData) {
            var content = new MultipartForm()
                .Set("shiftData", shiftData.ToString());

            SendRequest<TableServiceResponse>("pos/assignShift", content);
            return GetServerAssignments();
        }
    }
}
