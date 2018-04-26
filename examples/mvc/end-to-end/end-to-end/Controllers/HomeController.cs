using System;
using GlobalPayments.Api;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Text;

namespace GlobalPayments.Controllers {
    public class HomeController : Controller {
        public IActionResult Index() {
            return View();
        }

        public IActionResult ProcessPayment(OrderDetails details) {
            /*
             * The services container holds the various configurations the SDK needs to 
             * handle your card processing. It is important to note that the services container
             * is a singleton. If you need to hold multiple configurations for the same service type
             * (i.e. multiple gateway configurations) then it is important to name your configurations.
             * Failing to do so will result in the default connector being replaced each time the ConfigureService
             * method is called for a specific configuration type. You can add as many as you like
             * and the services container will keep track of them all. Here we are configuring a gateway
             * connector and calling it "MyGlobalPaymentsDemo".
             */
            ServicesContainer.ConfigureService(new GatewayConfig {
                SecretApiKey = "skapi_cert_MYl2AQAowiQAbLp5JesGKh7QFkcizOP2jcX9BrEMqQ",
                // The following variables will be provided to you during certification
                VersionNumber = "0000",
                DeveloperId = "000000",
                ServiceUrl = "https://cert.api2.heartlandportico.com"
            }, "MyGlobalPaymentsDemo");

            try {
                var address = new Address {
                    StreetAddress1 = details.Address,
                    City = details.City,
                    State = details.State,
                    Country = "United States",
                    PostalCode = details.Zip ?? string.Empty
                };

                var creditCard = new CreditCardData {
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

                if (authResponse.ResponseCode == "00") {
                    SendEmail();

                    return View("Success", new SuccessModel {
                        FirstName = details.FirstName,
                        TransactionId = authResponse.TransactionId
                    });
                }
                else {
                    return View("Error", model: "Transaction failed: " + authResponse.ResponseMessage);
                }                
            }
            catch (BuilderException exc) {
                /* 
                 * Handle invalid input exceptions:
                 * i.e. Missing amount, currency or payment method
                */
                return View("Error", model: "Invalid Input: " + exc.Message);
            }
            catch (GatewayException exc) {
                // handle errors related to gateway communication and invalid requests
                return View("Error", model: "invalid cc number, gateway-timeout, etc: " + exc.Message);
            }
            catch (ApiException exc) {
                // handle everything else
                return View("Error", model: "Something went wrong: " + exc.Message);
            }
        }

        private void SendEmail() {
            // This information would need to be replaced with your own
            // or call your own email sending methods
            try {
                
            }
            catch (Exception) {
                byte[] buffer = Encoding.UTF8.GetBytes("<strong>Couldn't Send Email</strong>");
                Response.Body.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
