using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;
using GlobalPayments.Api.Terminals.Genius.Builders;
using GlobalPayments.Api.Terminals.Genius.Enums;
using GlobalPayments.Api.Terminals.Genius.Interfaces;
using GlobalPayments.Api.Terminals.Genius.Request;
using GlobalPayments.Api.Terminals.Genius.Responses;
using GlobalPayments.Api.Utils;
using System;

namespace GlobalPayments.Api.Terminals.Genius
{
    public class GeniusController : DeviceController
    {
        private IDeviceInterface _device;      
        private MitcGateway _mitcGateway;
      
        internal override IDeviceCommInterface ConfigureConnector()
        {
            switch (_settings.ConnectionMode)
            {
                case ConnectionModes.MEET_IN_THE_CLOUD:
                    return new GeniusHttpInterface(_settings);
                case ConnectionModes.SERIAL:
                case ConnectionModes.SSL_TCP:
                case ConnectionModes.TCP_IP:
                default:
                    throw new NotImplementedException();
            }
        }

        internal GeniusController(ConnectionConfig settings) : base(settings)
        {
            if (settings.ConnectionMode == ConnectionModes.MEET_IN_THE_CLOUD)
            {
                _mitcGateway = new MitcGateway(settings);
            }
            else
            {
                throw new ConfigurationException("Unsupported device configuration.");
            }
        }

        internal override ITerminalResponse ProcessTransaction(TerminalAuthBuilder builder)
        {
            JsonDoc healthcareAmount = new JsonDoc();

            if (builder.AutoSubstantiation != null)
            {
                AutoSubstantiation autoSub = builder.AutoSubstantiation;
                healthcareAmount.Set("copay_amount", autoSub.CopaySubTotal.ToString())
                                .Set("clinical_amount", autoSub.ClinicSubTotal.ToString())
                                .Set("dental_amount", autoSub.DentalSubTotal.ToString())
                                .Set("prescription_amount", autoSub.PrescriptionSubTotal.ToString())
                                .Set("vision_amount", autoSub.VisionSubTotal.ToString())
                                .Set("healthcare_total_amount", autoSub.TotalHealthcareAmount.ToString());
            }
            JsonDoc purchaseOrder = new JsonDoc();

            if (builder.Address != null && builder.Address.PostalCode != null)
                purchaseOrder.Set("destination_postal_code", builder.Address.PostalCode);           

            purchaseOrder
                    .Set("po_number", builder.PoNumber)
                    .Set("tax_amount", builder.TaxAmount.ToString());

            JsonDoc payment = new JsonDoc();
            payment.Set("amount", builder.Amount.ToString())
                   .Set("currency_code", "840")
                   .Set("invoice_number", builder.InvoiceNumber);

            if (builder.TransactionType == TransactionType.Sale)
            {
                payment
                       .Set("healthcare_amounts", healthcareAmount);

            }

            JsonDoc receipt = new JsonDoc();

            if (builder.ClerkNumber != null)
            {
                receipt.Set("clerk_id", builder.ClerkNumber);
            }
            else
            {
                receipt.Set("clerk_id", "NA");
            }

            JsonDoc processingIndicator = new JsonDoc();
            processingIndicator.Set("allow_duplicate", builder.AllowDuplicates);
            if (builder.TransactionType == TransactionType.Sale)
            {
                processingIndicator.Set("create_token", builder.RequestMultiUseToken)
                                   .Set("partial_approval", builder.AllowPartialAuth);
            }

            JsonDoc terminal = new JsonDoc().Set("terminal_id", _mitcGateway.Config.GeniusMitcConfig.TerminalId);

            JsonDoc transaction = new JsonDoc();
            if (_mitcGateway.Config.GeniusMitcConfig.AllowKeyEntry)
            {
                transaction.Set("keyed_entry_mode", "allowed");
            }

            transaction.Set("country_code", "840")
                       .Set("language", "en-US");

            if (processingIndicator.Keys.Count>0)
                transaction.Set("processing_indicators", processingIndicator);

            if (builder.RequestMultiUseToken)
            {
                if (builder.CardOnFileIndicator == StoredCredentialInitiator.CardHolder)
                {
                    transaction.Set("create_token_reason", "unscheduled_customer_initiated_transaction");
                }
                else
                {
                    transaction.Set("create_token_reason", "unscheduled_merchant_initiated_transaction");
                }
            }

            transaction.Set("terminal", terminal);

            JsonDoc request = new JsonDoc();
            // Generating the reference ID.
            request.Set("reference_id", builder.ClientTransactionId);

            request.Set("payment", payment);

            request.Set("receipt", receipt);

            request.Set("transaction", transaction);

            MitcRequestType requestType = new MitcRequestType();

            if (builder.TransactionType == TransactionType.Sale)
            {
                requestType = MitcRequestType.CARD_PRESENT_SALE;
            }
            else if (builder.TransactionType == TransactionType.Refund)
            {
                requestType = MitcRequestType.CARD_PRESENT_REFUND;
            }

            try
            {
                return Send(request.ToString(), requestType, null);
            }
            catch (GatewayException e)
            {
                throw (e);
            }           
        }
        internal ITerminalResponse ManageTransaction(MitcManageBuilder builder)
        {
            try
            {
                JsonDoc request = new JsonDoc();
                MitcRequestType requestType = new MitcRequestType();
                string targetId = builder.ClientTransactionId;

                /* VoidCredit, VoidCreditRefund && RefundSale Request*/
                if ((builder.FollowOnTransactionType == TransactionType.Void || builder.FollowOnTransactionType == TransactionType.Refund)
                        && (builder.PaymentMethodType != PaymentMethodType.Debit))
                {

                    JsonDoc payment = new JsonDoc();
                    if (builder.Amount != null)
                    {
                        payment.Set("amount", builder.Amount.ToString());
                    }
                    request.Set("payment", payment);

                    /* RefundSale Request*/
                    if (builder.OriginalTransType == TransactionType.Sale && builder.FollowOnTransactionType == TransactionType.Refund)
                    {
                        payment.Set("invoice_number", builder.InvoiceNumber);
                        JsonDoc customer = new JsonDoc();
                        if (builder.CompanyId != null)
                        {
                            customer.Set("id", builder.Customer.Id);
                            customer.Set("title", builder.Customer.Title);
                            customer.Set("first_name", builder.Customer.FirstName);
                            customer.Set("middle_name", builder.Customer.MiddleName);
                            customer.Set("last_name", builder.Customer.LastName);
                            customer.Set("business_name", builder.Customer.Company);
                            customer.Set("email", builder.Customer.Email);
                            customer.Set("phone", builder.Customer.HomePhone);
                            JsonDoc address = new JsonDoc();
                            address.Set("line1", builder.Customer.Address.StreetAddress1);
                            address.Set("line2", builder.Customer.Address.StreetAddress2);
                            address.Set("city", builder.Customer.Address.City);
                            address.Set("state", builder.Customer.Address.State);
                            address.Set("country", builder.Customer.Address.Country);
                            address.Set("postal_code", builder.Customer.Address.PostalCode);
                            customer.Set("billing_address", address);
                        }
                        request.Set("customer", customer);
                    }
                }

                JsonDoc transaction = new JsonDoc();
                JsonDoc processingIndicators = new JsonDoc();

                /* VoidDebit Request*/
                if (builder.OriginalTransType == TransactionType.Sale && builder.PaymentMethodType == PaymentMethodType.Debit
                        && builder.FollowOnTransactionType == TransactionType.Void)
                {
                    transaction.Set("message_authentication_code", builder.MessageAuthCode);
                    transaction.Set("reason_code", builder.ReasonCode);
                    transaction.Set("tracking_id", builder.TrackingId);

                    JsonDoc receipt = new JsonDoc();
                    receipt.Set("signature_image", builder.SignatureImage);
                    receipt.Set("signature_format", builder.SignatureFormat);
                    receipt.Set("signature_line", builder.SignatureLine);
                    request.Set("receipt", receipt);
                }

                if (builder.Receipt)
                {
                    processingIndicators.Set("generate_receipt", true);
                }
                else
                {
                    processingIndicators.Set("generate_receipt", false);
                }

                /* RefundSale Request*/
                if (builder.OriginalTransType == TransactionType.Sale && builder.FollowOnTransactionType == TransactionType.Refund)
                {
                    transaction.Set("soft_descriptor", builder.SoftDescriptor);
                    if (builder.isAllowDuplicates())
                    {
                        processingIndicators.Set("allow_duplicate", builder.isAllowDuplicates());
                    }
                }
                transaction.Set("processing_indicators", processingIndicators);
                request.Set("transaction", transaction);

                if (builder.FollowOnTransactionType == TransactionType.Void)
                {
                    if (builder.OriginalTransType == TransactionType.Refund)
                    {
                        requestType = MitcRequestType.VOID_REFUND;
                    }
                    else
                    {
                        if (builder.PaymentMethodType == PaymentMethodType.Credit)
                        {
                            requestType = MitcRequestType.VOID_CREDIT_SALE;
                        }
                        else
                        {
                            requestType = MitcRequestType.VOID_DEBIT_SALE;
                        }
                    }
                }
                else if (builder.FollowOnTransactionType == TransactionType.Refund)
                {
                    requestType = MitcRequestType.REFUND_BY_CLIENT_ID;
                }
                try
                {
                    return Send(request.ToString(), requestType, targetId);
                }
                catch (GatewayException ex)
                {
                    throw ex;
                }
            }
            catch (BuilderException ex)
            {
                throw ex;
            }
        }     

        internal override byte[] SerializeRequest(TerminalAuthBuilder builder)
        {
            throw new NotImplementedException();
        }     

        public MitcResponse Send(string message, MitcRequestType requestType, string targetId)
        {
            string endpoint = "";
            GeniusMitcRequest.HttpMethod verb = new GeniusMitcRequest.HttpMethod();
            switch (requestType)
            {
                case MitcRequestType.CARD_PRESENT_SALE:
                    endpoint = requestType.GetURLEndpoint();
                    verb = requestType.GetVerb();
                    break;
                case MitcRequestType.CARD_PRESENT_REFUND:
                    endpoint = requestType.GetURLEndpoint();
                    verb = requestType.GetVerb();
                    break;
                case MitcRequestType.REPORT_SALE_CLIENT_ID:
                    endpoint = requestType.GetURLEndpoint();
                    endpoint = endpoint.Replace("%s", targetId);
                    verb = requestType.GetVerb();
                    break;
                case MitcRequestType.REPORT_REFUND_CLIENT_ID:
                    endpoint = requestType.GetURLEndpoint();
                    endpoint = endpoint.Replace("%s", targetId);
                    verb = requestType.GetVerb();
                    break;
                case MitcRequestType.REFUND_BY_CLIENT_ID:
                    endpoint = "/creditsales/reference_id/" + targetId + "/creditreturns"; 
                    verb = requestType.GetVerb();
                    break;
                case MitcRequestType.VOID_CREDIT_SALE:
                    endpoint = requestType.GetURLEndpoint();
                    endpoint = endpoint.Replace("%s", targetId);
                    verb = requestType.GetVerb();
                    break;
                case MitcRequestType.VOID_DEBIT_SALE:
                    endpoint = requestType.GetURLEndpoint();
                    endpoint = endpoint.Replace("%s", targetId);
                    verb = requestType.GetVerb();
                    break;
                case MitcRequestType.VOID_REFUND:
                    endpoint = requestType.GetURLEndpoint();
                    endpoint = endpoint.Replace("%s", targetId);
                    verb = requestType.GetVerb();
                    break;
            }
            return _mitcGateway.DoTransaction(verb, endpoint, message, requestType);
        }

        internal override IDeviceInterface ConfigureInterface()
        {
            if (_device == null)
                _device = new GeniusInterface(this);
            return _device;
        }      
        internal override ITerminalReport ProcessReport(TerminalReportBuilder builder)
        {
            MitcRequestType requestType;

            if (builder.TransactionType == TransactionType.Sale)
            {
                if (builder.TransactionIdType == TransactionIdType.CLIENT_TRANSACTION_ID)
                {
                    requestType = MitcRequestType.REPORT_SALE_CLIENT_ID;
                }
                else
                {
                    requestType = MitcRequestType.REPORT_SALE_GATEWAY_ID;
                }
            }
            else if (builder.TransactionType == TransactionType.Refund)
            {
                if (builder.TransactionIdType == TransactionIdType.CLIENT_TRANSACTION_ID)
                {
                    requestType = MitcRequestType.REPORT_REFUND_CLIENT_ID;

                }
                else
                {
                    requestType = MitcRequestType.REPORT_REFUND_GATEWAY_ID;
                }
            }
            else
            {
                throw new BuilderException("Target transaction type must be either a sale or refund");
            }

            return Send(null, requestType, builder.TransactionId);
        }
        internal override ITerminalResponse ManageTransaction(TerminalManageBuilder builder)
        {
            throw new NotImplementedException();
        }

        internal override byte[] SerializeRequest(TerminalManageBuilder builder)
        {
            throw new NotImplementedException();
        }

        internal override byte[] SerializeRequest(TerminalReportBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
