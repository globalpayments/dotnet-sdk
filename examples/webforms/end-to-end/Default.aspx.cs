using System;
using System.Web.UI;
using GlobalPayments.Api;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;

namespace end_to_end
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Context.Request.QueryString.HasKeys())
                ProcessPayment();
        }

        private void ProcessPayment()
        {
            var details = GetOrderDetails();
            string response;

            ServicesContainer.ConfigureService(new GatewayConfig
            {
                SecretApiKey = "skapi_cert_MYl2AQAowiQAbLp5JesGKh7QFkcizOP2jcX9BrEMqQ",
                // The following variables will be provided to you during certification
                VersionNumber = "0000",
                DeveloperId = "000000",
                ServiceUrl = "https://cert.api2.heartlandportico.com"
            }, "MyGlobalPaymentsDemo");

            try
            {
                var address = new Address
                {
                    StreetAddress1 = details.Address,
                    City = details.City,
                    State = details.State,
                    Country = "United States",
                    PostalCode = details.Zip ?? string.Empty
                };

                var creditCard = new CreditCardData
                {
                    Token = details.Token_value,
                    CardHolderName = string.Format("{0} {1}", details.FirstName ?? string.Empty, details.LastName ?? string.Empty).Trim()
                };

                /*
                 * Use method chaining to define your transactions and set all of your transaction
                 * options before finally calling the Execute method. Remember, if you are using 
                 * named configurations to pass the name of the configuration you want to be used to
                 * the Execute method.
                 */
                var authResponse = creditCard.Charge(15.15m)
                    .WithCurrency("USD")
                    .WithAddress(address)
                    .WithAllowDuplicates(true)
                    .Execute("MyGlobalPaymentsDemo");

                if (authResponse.ResponseCode == "00")
                {
                    response = "Success!";
                    Response.Redirect("~/Response.aspx?FirstName="+details.FirstName+"&TransactionId="+authResponse.TransactionId+"&response="+response);
                    SendEmail();
                }
                else
                {
                    response = "Failed!";
                    Response.Redirect("~/Response.aspx?FirstName=" + details.FirstName + "&response=" + response);
                }
            }
            catch (BuilderException exc)
            {
                /* 
                 * Handle invalid input exceptions:
                 * i.e. Missing amount, currency or payment method
                */
                Response.Write("<h3>Error</h3>" + "<strong>InValidInput: " + exc.Message + "</strong>");
            }
            catch (GatewayException exc)
            {
                // handle errors related to gateway communication and invalid requests
                Response.Write("<h3>Error</h3>" + "<strong>invalid cc number, gateway-timeout, etc: " + exc.Message + "</strong>");
            }
            catch (ApiException exc)
            {
                // handle everything else
                Response.Write("<h3>Error</h3>" + "<strong>Something Went Wrong " + exc.Message + "</strong>");
            }
        }

        public void SendEmail()
        {
            // This information would need to be replaced with your own
            // or call your own email sending methods

            try
            {
                
            }
            catch (Exception)
            {
                Response.Write("<strong>Couldn't Send Email</strong>");
            }
        }        

        private OrderDetails GetOrderDetails()
        {
            var query = Context.Request.QueryString;

            return new OrderDetails {
                FirstName = query["FirstName"],
                LastName = query["LastName"],
                PhoneNumber = query["PhoneNumber"],
                Email = query["Email"],
                Address = query["Address"],
                City = query["City"],
                Zip = query["Zip"],
                Token_value = query["token_value"],
                Card_type = query["card_type"],
                Exp_month = query["exp_month"],
                Exp_year = query["exp_year"],
                Last_four = query["last_four"],
            };
        }
    }

    internal class OrderDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Token_value { get; set; }
        public string Card_type { get; set; }
        public string Last_four { get; set; }
        public string Exp_month { get; set; }
        public string Exp_year { get; set; }
    }
}