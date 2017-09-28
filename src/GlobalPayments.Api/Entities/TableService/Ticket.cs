using System;
using System.Linq;
using System.Collections.Generic;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.TableService {
    /// <summary>
    /// Represents a check/table in the table service calls
    /// </summary>
    /// <remarks>
    /// Mostly used in table service calls
    /// </remarks>
    public class Ticket : TableServiceResponse {
        /// <summary>
        /// The ID of the tables current bump status
        /// </summary>
        public int BumpStatusId { get; set; }

        /// <summary>
        /// ID of the check associated with this ticket
        /// </summary>
        public int CheckId { get; set; }

        /// <summary>
        /// Checkin time of the customer
        /// </summary>
        public DateTime CheckInTime { get; set; }

        /// <summary>
        /// Customer's party name
        /// </summary>
        public string PartyName { get; set; }

        /// <summary>
        /// Number assigned to the customer's party
        /// </summary>
        public int PartyNumber { get; set; }

        /// <summary>
        /// Section of the restaurant
        /// </summary>
        public string Section { get; set; }

        /// <summary>
        /// Table number associcated with this ticket/check
        /// </summary>
        public int? TableNumber { get; set; }

        /// <summary>
        /// Time in minutes the customer has been waiting
        /// </summary>
        public int WaitTime { get; set; }

        /// <summary>
        /// Ticket constructor
        /// </summary>
        /// <param name="json">Json Response from the Table Service API</param>
        public Ticket(string json) : base(json) {
            ExpectedAction = "assignCheck";
        }

        protected override void MapResponse(JsonDoc response) {
            base.MapResponse(response);

            CheckId = response.GetValue<int>("checkID");
            CheckInTime = response.GetValue<DateTime>("checkInTime");
            TableNumber = response.GetValue<int>("tableNumber");
            WaitTime = response.GetValue<int>("waitTime");
        }

        /// <summary>
        /// This generally would be used to notifiy Freshxt of the kitchen bump time. This will allow the host to see a color
        /// status change on a table.The bumpStatusID is the status ID inside freshtxt that you want to set the bump to
        /// be.This can be configured inside the freshtxt application and is returned with every login in the
        /// tableStatus variable.The first item in that list is ID= 1, next id ID= 2 and so on.Setting ID = 0 will clear the
        /// tableStatus back to none.
        /// </summary>
        /// <param name="bumpStatus">The string value of the bump status</param>
        /// <param name="bumpTime">optional: the time of the status change</param>
        /// <returns>TableServiceResponse</returns>
        public TableServiceResponse BumpStatus(string bumpStatus, DateTime? bumpTime = null) {
            int bumpStatusId = ServicesContainer.Instance.GetReservationService().BumpStatusCollection[bumpStatus];
            if (bumpStatusId == 0)
                throw new MessageException(String.Format("Unknown status value: {0}", bumpStatus));
            return BumpStatus(bumpStatusId, bumpTime);
        }

        /// <summary>
        /// This generally would be used to notifiy Freshxt of the kitchen bump time. This will allow the host to see a color
        /// status change on a table.The bumpStatusID is the status ID inside freshtxt that you want to set the bump to
        /// be.This can be configured inside the freshtxt application and is returned with every login in the
        /// tableStatus variable.The first item in that list is ID= 1, next id ID= 2 and so on.Setting ID = 0 will clear the
        /// tableStatus back to none.
        /// </summary>
        /// <param name="bumpStatusId">The ID of the bump status</param>
        /// <param name="bumpTime">optional: the time of the status change</param>
        /// <returns>TableServiceResponse</returns>
        public TableServiceResponse BumpStatus(int bumpStatusId, DateTime? bumpTime = null) {
            var content = new MultipartForm()
                .Set("checkID", CheckId)
                .Set("bumpStatusID", bumpStatusId)
                .Set("bumpTime", bumpTime ?? DateTime.Now);

            var response = SendRequest<TableServiceResponse>("pos/bumpStatus", content);
            BumpStatusId = bumpStatusId;
            return response;
        }

        /// <summary>
        /// Allows POS to update/clear the party from the table inside Freshtxt.
        /// </summary>
        /// <param name="clearTime">optional: Time the table was cleared</param>
        /// <returns>TableServiceResponse</returns>
        public TableServiceResponse ClearTable(DateTime? clearTime = null) {
            var content = new MultipartForm()
                .Set("checkID", CheckId)
                .Set("clearTime", clearTime ?? DateTime.Now);

            return SendRequest<TableServiceResponse>("pos/clearTable", content);
        }


        /// <summary>
        /// This would be used to notifiy Freshxt of the order placement time.
        /// </summary>
        /// <param name="openTime">optional: The order placement time</param>
        /// <returns>TableServiceResponse</returns>
        public TableServiceResponse OpenOrder(DateTime? openTime = null) {
            var content = new MultipartForm()
                .Set("checkID", CheckId)
                .Set("openTime", openTime ?? DateTime.Now);

            return SendRequest<TableServiceResponse>("pos/openOrder", content);
        }

        /// <summary>
        /// Allow POS to settle the check within Freshtxt and optionally change the tableStatus. This will allow the host to
        /// see a color status change on a table.The bumpStatusID is the status ID inside freshtxt that you want to set the
        /// bump to be. This can be configured inside the freshtxt application and is returned with every login in the
        /// tableStatus variable.The first item in that list is ID= 1, next id ID= 2 and so on.Setting ID = 0 will clear the
        /// tableStatus back to none.
        /// </summary>
        /// <returns>TableServiceResponse</returns>
        public TableServiceResponse SettleCheck() {
            return SettleCheck(string.Empty, null);
        }
        /// <summary>
        /// Allow POS to settle the check within Freshtxt and optionally change the tableStatus. This will allow the host to
        /// see a color status change on a table.The bumpStatusID is the status ID inside freshtxt that you want to set the
        /// bump to be. This can be configured inside the freshtxt application and is returned with every login in the
        /// tableStatus variable.The first item in that list is ID= 1, next id ID= 2 and so on.Setting ID = 0 will clear the
        /// tableStatus back to none.
        /// </summary>
        /// <returns>TableServiceResponse</returns>
        public TableServiceResponse SettleCheck(string bumpStatus = null, DateTime? settleTime = null) {
            int? bumpStatusId = null;
            if (!string.IsNullOrEmpty(bumpStatus)) {
                bumpStatusId = ServicesContainer.Instance.GetReservationService().BumpStatusCollection[bumpStatus];
                if (bumpStatusId == 0)
                    throw new MessageException(String.Format("Unknown status value: {0}", bumpStatus));
            }
            return SettleCheck(bumpStatusId, settleTime);
        }
        /// <summary>
        /// Allow POS to settle the check within Freshtxt and optionally change the tableStatus. This will allow the host to
        /// see a color status change on a table.The bumpStatusID is the status ID inside freshtxt that you want to set the
        /// bump to be. This can be configured inside the freshtxt application and is returned with every login in the
        /// tableStatus variable.The first item in that list is ID= 1, next id ID= 2 and so on.Setting ID = 0 will clear the
        /// tableStatus back to none.
        /// </summary>
        /// <returns>TableServiceResponse</returns>
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

        /// <summary>
        /// Allows POS to transfer a party from a table to another table number newTableNumber . This will preserve
        /// all data associated with that party.
        /// </summary>
        /// <param name="newTableNumber">The new table number to assign to the existing table</param>
        /// <returns>TableServiceResponse</returns>
        public TableServiceResponse Transfer(int newTableNumber) {
            var content = new MultipartForm()
                .Set("checkID", CheckId)
                .Set("newTableNumber", newTableNumber);

            var response = SendRequest<TableServiceResponse>("pos/settleCheck", content);
            if(response.ResponseCode == "00")
                TableNumber = newTableNumber;
            return response;
        }

        /// <summary>
        /// Allows POS to alter the partyName, partyNum (covers), section, and bumpStatusID for a table.
        /// </summary>
        /// <returns>TableServiceResponse</returns>
        public TableServiceResponse Update() {
            var content = new MultipartForm()
                .Set("checkID", CheckId)
                .Set("partyName", PartyName)
                .Set("partNum", PartyNumber)
                .Set("section", Section)
                .Set("bumpStatusID", BumpStatusId);

            return SendRequest<TableServiceResponse>("pos/editTable", content);
        }

        /// <summary>
        /// Creates a Ticket object from an existing check/table number
        /// </summary>
        /// <param name="checkId">The check ID assigned to this ticket</param>
        /// <param name="tableNumber">optional: the table number assigned to the ticket.</param>
        /// <returns>Ticket</returns>
        public static Ticket FromId(int checkId, int? tableNumber = null) {
            return new Ticket(string.Empty) {
                CheckId = checkId,
                TableNumber = tableNumber
            };
        }
    }
}
