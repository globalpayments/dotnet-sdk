using System;
using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.TableService;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Services {
    public class TableService {
        private readonly string _configName;

        /// <summary>
        /// string array of the bump statuses as reported by the table service API.
        /// </summary>
        public string[] BumpStatuses {
            get {
                return ServicesContainer.Instance.GetTableServiceClient(_configName).BumpStatusCollection.Keys;
            }
        }

        /// <summary>
        /// Table Service constructor
        /// </summary>
        /// <param name="config">ServicesConfig object used to configure the services container.</param>
        public TableService(TableServiceConfig config, string configName = "default") {
            _configName = configName;
            ServicesContainer.ConfigureService(config, configName);
        }

        protected T SendRequest<T>(string endpoint, MultipartForm formData) where T : TableServiceResponse {
            var connector = ServicesContainer.Instance.GetTableServiceClient(_configName);
            if (!connector.Configured && !endpoint.Equals("user/login"))
                throw new ConfigurationException("Reservation service has not been configured properly. Please ensure you have logged in first.");

            var response = connector.Call(endpoint, formData);
            return Activator.CreateInstance(typeof(T), response, _configName) as T;
        }

        /// <summary>
        /// Used to log into the table service API and configure the connector for subsequent calls.
        /// </summary>
        /// <param name="username">Username of the user.</param>
        /// <param name="password">Password of the user.</param>
        /// <returns></returns>
        public LoginResponse Login(string username, string password) {
            var content = new MultipartForm()
                .Set("username", username)
                .Set("password", password);

            var response = SendRequest<LoginResponse>("user/login", content);

            // configure the connector
            var connector = ServicesContainer.Instance.GetTableServiceClient(_configName);
            connector.LocationId = response.LocationId;
            connector.SecurityToken = response.Token;
            connector.SessionId = response.SessionId;
            connector.BumpStatusCollection = new BumpStatusCollection(response.TableStatus);

            return response;
        }

        /// <summary>
        /// This will assign a seated party with the specificied tabled number with a checkID from your POS. Practical uses
        /// for this is when a server opens a check, your POS sends the tableNumber and checkID and Freshtxt returns
        /// the party waitTime and checkInTime.This must be set to use any other POS API calls.
        /// </summary>
        /// <param name="tableNumber">table number to assign to the check</param>
        /// <param name="checkId">ID of the check in the system</param>
        /// <param name="startTime">optional: time the ticket was assigned.</param>
        /// <returns>Ticket</returns>
        public Ticket AssignCheck(int tableNumber, int checkId, DateTime? startTime = null) {
            var content = new MultipartForm()
                .Set("tableNumber", tableNumber)
                .Set("checkID", checkId)
                .Set("startTime", startTime ?? DateTime.Now);

            var response = SendRequest<Ticket>("pos/assignCheck", content);
            response.TableNumber = tableNumber;
            return response;
        }

        /// <summary>
        /// Allows POS to query table/check status by table number. Occupied and enabled/disabled statuses are
        /// returned, along with the checkID and bumpStatusID if a check is currently assigned.
        /// </summary>
        /// <param name="tableNumber">table number to query</param>
        /// <returns>Ticket</returns>
        public Ticket QueryTableStatus(int tableNumber) {
            var content = new MultipartForm()
                .Set("tableNumber", tableNumber);

            var response = SendRequest<Ticket>("pos/tableStatus", content);
            response.TableNumber = tableNumber;
            return response;
        }

        /// <summary>
        /// Allows POS to query table/check status by table number. Occupied and enabled/disabled statuses are
        /// returned, along with the checkID and bumpStatusID if a check is currently assigned.
        /// </summary>
        /// <param name="checkId">check id to query</param>
        /// <returns>Ticket</returns>
        public Ticket QueryCheckStatus(int checkId) {
            var content = new MultipartForm()
                .Set("checkID", checkId);

            return SendRequest<Ticket>("pos/checkStatus", content);
        }

        /// <summary>
        /// Allows POS to get the current server list. Will return empty if no list is present.
        /// </summary>
        /// <returns>ServerListResponse</returns>
        public ServerListResponse GetServerList() {
            return SendRequest<ServerListResponse>("pos/getServerList", new MultipartForm());
        }

        /// <summary>
        /// Allows POS to update the server name list inside Freshtxt. The complete list will be replaced with this method.
        /// </summary>
        /// <param name="serverList">Enumerable list of server names</param>
        /// <returns>ServerListResponse</returns>
        public ServerListResponse UpdateServerList(IEnumerable<string> serverList) {
            var _serverList = string.Join(",", serverList);

            var content = new MultipartForm()
                .Set("serverList", _serverList);

            SendRequest<TableServiceResponse>("pos/updateServerList", content);
            return GetServerList();
        }

        /// <summary>
        /// Allows POS to query for currently assigned tables to servers. Without the optional variables a complete list of
        /// currently assigned servers along with their tables will be returned.If serverName is supplied, only the
        /// tables assigned to that server will be returned. If tableNumber is supplied, only the server and other tables
        /// assigned to that server will be returned.If both optional variables are supplied the server name will take
        /// precedent.
        /// </summary>
        /// <returns>ServerAssignmentResponse</returns>
        public ServerAssignmentResponse GetServerAssignments() {
            return SendRequest<ServerAssignmentResponse>("pos/getServerAssignment", new MultipartForm());
        }
        /// <summary>
        /// Allows POS to query for currently assigned tables to servers. Without the optional variables a complete list of
        /// currently assigned servers along with their tables will be returned.If serverName is supplied, only the
        /// tables assigned to that server will be returned. If tableNumber is supplied, only the server and other tables
        /// assigned to that server will be returned.If both optional variables are supplied the server name will take
        /// precedent.
        /// </summary>
        /// <param name="serverName">server name to query</param>
        /// <returns>ServerAssignmentResponse</returns>
        public ServerAssignmentResponse GetServerAssignments(string serverName) {
            var content = new MultipartForm()
                .Set("serverName", serverName);

            return SendRequest<ServerAssignmentResponse>("pos/getServerAssignment", content);
        }
        /// <summary>
        /// Allows POS to query for currently assigned tables to servers. Without the optional variables a complete list of
        /// currently assigned servers along with their tables will be returned.If serverName is supplied, only the
        /// tables assigned to that server will be returned. If tableNumber is supplied, only the server and other tables
        /// assigned to that server will be returned.If both optional variables are supplied the server name will take
        /// precedent.
        /// </summary>
        /// <param name="tableNumber">table number to query server for</param>
        /// <returns>ServerAssignmentResponse</returns>
        public ServerAssignmentResponse GetServerAssignments(int tableNumber) {
            var content = new MultipartForm()
                .Set("tableNumber", tableNumber);

            return SendRequest<ServerAssignmentResponse>("pos/getServerAssignment", content);
        }

        /// <summary>
        /// Allows POS to assign and update shift data within Freshtxt. With this single call, new section/station
        /// assignments can be made as well as server assignments to those sections.
        /// </summary>
        /// <param name="shiftData">Shift assignments object</param>
        /// <returns>ServerAssignmentResponse</returns>
        public ServerAssignmentResponse AssignShift(ShiftAssignments shiftData) {
            var content = new MultipartForm()
                .Set("shiftData", shiftData.ToString());

            SendRequest<TableServiceResponse>("pos/assignShift", content);
            return GetServerAssignments();
        }
    }
}
