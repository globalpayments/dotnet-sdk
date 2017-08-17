using System.Collections.Generic;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.TableService {
    public class ServerListResponse : TableServiceResponse {
        public List<string> Servers { get; set; }

        public ServerListResponse(string json) : base(json) {
            ExpectedAction = "getServerList";
        }

        protected override void MapResponse(JsonDoc response) {
            base.MapResponse(response);

            // populate servers
            var serverList = response.GetValue<string>("serverList");
            if (serverList != null) {
                Servers = new List<string>();
                foreach (var server in serverList.Split(',')) {
                    Servers.Add(server);
                }
            }
        }
    }
}
