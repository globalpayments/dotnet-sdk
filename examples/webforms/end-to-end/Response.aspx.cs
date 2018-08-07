using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace end_to_end {
    public partial class Success : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            string firstname = Request.QueryString["FirstName"];
            string transactionId = Request.QueryString["TransactionId"];
            string response = Request.QueryString["response"];
            if (response == "Success!") {
                header.InnerText = response;
                lblStatus.Text = "Thank you, " + firstname + ", for your order of $15.15. <p>Transaction Id: " + transactionId + "</p>";
            }
            else if (response == "Failed!") {
                header.InnerText = response;
                lblStatus.Text = "Thank you," + firstname;
            }
        }
    }
}