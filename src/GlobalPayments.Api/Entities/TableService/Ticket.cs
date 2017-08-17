using System;
using System.Linq;
using System.Collections.Generic;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.TableService {
    public class Ticket : TableServiceResponse {
        public int BumpStatusId { get; set; }
        public int CheckId { get; set; }
        public DateTime CheckInTime { get; set; }
        public string PartyName { get; set; }
        public int PartyNumber { get; set; }
        public string Section { get; set; }
        public int? TableNumber { get; set; }
        public int WaitTime { get; set; }

        public Ticket(string json) : base(json) {
            ExpectedAction = "assignCheck";
            //, "assignCheck", "checkStatus", "tableStatus") { }
        }

        protected override void MapResponse(JsonDoc response) {
            base.MapResponse(response);

            CheckId = response.GetValue<int>("checkID");
            CheckInTime = response.GetValue<DateTime>("checkInTime");
            TableNumber = response.GetValue<int>("tableNumber");
            WaitTime = response.GetValue<int>("waitTime");
        }

        public TableServiceResponse BumpStatus(string bumpStatus, DateTime? bumpTime = null) {
            int bumpStatusId = ServicesContainer.Instance.GetReservationService().BumpStatusCollection[bumpStatus];
            if (bumpStatusId == 0)
                throw new MessageException(String.Format("Unknown status value: {0}", bumpStatus));
            return BumpStatus(bumpStatusId, bumpTime);
        }
        public TableServiceResponse BumpStatus(int bumpStatusId, DateTime? bumpTime = null) {
            var content = new MultipartForm()
                .Set("checkID", CheckId)
                .Set("bumpStatusID", bumpStatusId)
                .Set("bumpTime", bumpTime ?? DateTime.Now);

            var response = SendRequest<TableServiceResponse>("pos/bumpStatus", content);
            BumpStatusId = bumpStatusId;
            return response;
        }

        public TableServiceResponse ClearTable(DateTime? clearTime = null) {
            var content = new MultipartForm()
                .Set("checkID", CheckId)
                .Set("clearTime", clearTime ?? DateTime.Now);

            return SendRequest<TableServiceResponse>("pos/clearTable", content);
        }

        public TableServiceResponse OpenOrder(DateTime? openTime = null) {
            var content = new MultipartForm()
                .Set("checkID", CheckId)
                .Set("openTime", openTime ?? DateTime.Now);

            return SendRequest<TableServiceResponse>("pos/openOrder", content);
        }

        public TableServiceResponse SettleCheck() {
            return SettleCheck(string.Empty, null);
        }
        public TableServiceResponse SettleCheck(string bumpStatus = null, DateTime? settleTime = null) {
            int? bumpStatusId = null;
            if (!string.IsNullOrEmpty(bumpStatus)) {
                bumpStatusId = ServicesContainer.Instance.GetReservationService().BumpStatusCollection[bumpStatus];
                if (bumpStatusId == 0)
                    throw new MessageException(String.Format("Unknown status value: {0}", bumpStatus));
            }
            return SettleCheck(bumpStatusId, settleTime);
        }
        public TableServiceResponse SettleCheck(int? bumpStatusId = null, DateTime? settleTime = null) {
            var content = new MultipartForm()
                .Set("checkID", CheckId)
                .Set("bumpStatusID", bumpStatusId)
                .Set("settleTime", settleTime ?? DateTime.Now);

            var response = SendRequest<TableServiceResponse>("pos/settleCheck", content);
            if(bumpStatusId.HasValue)
                BumpStatusId = bumpStatusId.Value;
            return response;
        }

        public TableServiceResponse Transfer(int newTableNumber) {
            var content = new MultipartForm()
                .Set("checkID", CheckId)
                .Set("newTableNumber", newTableNumber);

            var response = SendRequest<TableServiceResponse>("pos/settleCheck", content);
            if(response.ResponseCode == "00")
                TableNumber = newTableNumber;
            return response;
        }

        public TableServiceResponse Update() {
            var content = new MultipartForm()
                .Set("checkID", CheckId)
                .Set("partyName", PartyName)
                .Set("partNum", PartyNumber)
                .Set("section", Section)
                .Set("bumpStatusID", BumpStatusId);

            return SendRequest<TableServiceResponse>("pos/editTable", content);
        }

        public static Ticket FromId(int checkId, int? tableNumber = null) {
            return new Ticket(string.Empty) {
                CheckId = checkId,
                TableNumber = tableNumber
            };
        }
    }
}
