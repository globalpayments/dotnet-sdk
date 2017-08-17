using System.Linq;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.TableService {
    public class ServerAssignmentResponse : TableServiceResponse {
        public ShiftAssignments Assignments { get; private set; }

        public ServerAssignmentResponse(string json) : base(json) {
            ExpectedAction = "getServerAssignment";
        }

        protected override void MapResponse(JsonDoc response) {
            base.MapResponse(response);

            Assignments = new ShiftAssignments();

            // if we have a row, then it's an array
            if (response.Has("row")) {
                foreach (var row in response.GetEnumerator("row"))
                    AddAssignment(row);
            }
            else AddAssignment(response);
        }

        protected void AddAssignment(JsonDoc assignment) {
            var server = assignment.GetValue<string>("server");
            var tables = assignment.GetValue<string>("tables");

            // do something with this data
            if (!string.IsNullOrEmpty(tables)) {
                var ids = tables.Split(',').Select(p => int.Parse(p));
                Assignments.Add(server, ids);
            }
        }
    }
}
