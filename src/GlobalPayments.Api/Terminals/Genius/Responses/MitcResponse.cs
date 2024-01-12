using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPayments.Api.Terminals.Genius.Responses
{
    public class MitcResponse : ITerminalResponse, ITerminalReport
    {
        public string GatewayResponseCode { get; set; }
        public string GatewayResponseMessage { get; set; }
        public string InvoiceNumber { get; set; }
        public string ResponseDateTime { get; set; }
        public decimal GratuityAmount { get; set; }
        public string Status { get; set; }
        public string ApprovalCode { get; set; }
        public string CvvResultCode { get; set; }        
        public string CashbackAmount { get; set; }
        public string AuthorizedAmount { get; set; }
        public string TokenResponseCode { get; set; }
        public string TokenResponseMsg { get; set; }        
        public ICC ICC { get; set; }
        public string TraceNumber { get; set; }
        public string CurrencyCode { get; set; }
        public string TenderType { get; set; }       
        public string ClientTransactionId { get; set; }        
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public string Type { get; set; }
        public string PostalCode { get; set; }
        public string CustomerId { get; set; }
        public string TransactionId { get; set; }
        public string MaskedCardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string EntryMethod { get; set; }
        public string Token { get; set; }
        public string DeviceResponseCode { get; set; }
        public string DeviceResponseText { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public string TerminalRefNumber { get; set; }
        public string SignatureStatus { get; set; }
        public byte[] SignatureData { get; set; }
        public string TransactionType { get; set; }
        public string AuthorizationCode { get; set; }
        public decimal? TransactionAmount { get; set; }
        public decimal? AmountDue { get; set; }
        public decimal? BalanceAmount { get; set; }
        public string CardBIN { get; set; }
        public bool CardPresent { get; set; }
        public string ExpirationDate { get; set; }
        public decimal? TipAmount { get; set; }
        public decimal? CashBackAmount { get; set; }
        public string AvsResponseCode { get; set; }
        public string AvsResponseText { get; set; }
        public string CvvResponseCode { get; set; }
        public string CvvResponseText { get; set; }
        public bool TaxExempt { get; set; }
        public string TaxExemptId { get; set; }
        public string TicketNumber { get; set; }
        public string PaymentType { get; set; }
        public string ApplicationPreferredName { get; set; }
        public string ApplicationLabel { get; set; }
        public string ApplicationId { get; set; }
        public ApplicationCryptogramType ApplicationCryptogramType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ApplicationCryptogram { get; set; }
        public string CardHolderVerificationMethod { get; set; }
        public string TerminalVerificationResults { get; set; }
        public decimal? MerchantFee { get; set; }
        public string Command { get; set; }
        public string Version { get; set; }
        public string ReferenceNumber { get; set; }

        public MitcResponse(int responseCode, string responseMessage, string responseData)
        {
            try
            {
                MapStatusCode(responseCode, responseMessage);
                var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseData);
                HashMapper(values);                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void MapStatusCode(int statusCode, string statusMessage)
        {
            switch (statusCode.ToString())
            {
                case "200":
                case "201":
                case "473":
                    ResponseCode = "00";
                    ResponseText = "success";
                    break;
                case "470":
                case "472":
                    ResponseCode = "05";
                    ResponseText = "declined";
                    break;
                case "471":
                case "474":
                    ResponseCode = "10";
                    ResponseText = "partial approval";
                    break;
                case "400":
                case "401":
                case "402":
                case "403":
                case "404":
                case "409":
                case "429":
                case "500":
                case "503":
                    ResponseCode = "ER";
                    break;
                default:
                    break;
            }

            GatewayResponseCode = statusCode.ToString();
            GatewayResponseMessage = statusMessage;
        }

        private void HashMapper(Dictionary<string, object> keyValues)
        {
            foreach (KeyValuePair<string, object> entry in keyValues)
            {
                string key = entry.Key;
                object value = entry.Value;
                Type valueType = entry.Value.GetType();

                if (value is string)
                {
                    AssignValues((string)value, key);
                }
                else if (valueType.Name == "JArray")
                {
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    foreach (JObject content in ((JArray)value).Children<JObject>())
                    {
                        foreach (JProperty prop in content.Properties())
                        {
                            Type type = prop.Value.GetType();

                            if (type.Name == "JValue")
                            {
                                dict.Add(prop.Name, prop.Value.ToString());
                            }
                            else if (type.Name == "JObject")
                            {
                                dict.Add(prop.Name, prop.Value);
                            }
                        }
                    }
                    HashMapper(dict);
                }
                else if (valueType.Name == "JObject")
                {
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    if (key.ToLower() == "icc")
                    {
                        ICC =JsonConvert.DeserializeObject<ICC>(value.ToString());                        
                        continue;
                    }
                    foreach (var content in ((JObject)value).Children<JProperty>())
                    {
                        foreach (var prop in content.Children())
                        {
                            if (prop.Children().Count() > 1)
                            {
                                foreach (var item in (JsonConvert.DeserializeObject<Dictionary<string, object>>(prop.ToString())))
                                {
                                    if (!dict.ContainsKey(item.Key))
                                    {
                                        dict.Add(item.Key, item.Value);
                                    }
                                }
                            }
                            else
                            {
                                if (!dict.ContainsKey(content.Name))
                                {
                                    dict.Add(content.Name, content.Value.ToString());
                                }
                            }
                        }
                    }

                    HashMapper(dict);
                }
                else
                {
                    throw new ArgumentException(value.ToString());
                }
            }
        }

        private void AssignValues(string value, string key)
        {
            if (key == "invoice_number")
            {
                InvoiceNumber = value;
            }
            if (key == "amount")
            {
                TransactionAmount = decimal.Parse(value);
            }
            if (key == "currency_code")
            {
                CurrencyCode = value;
            }
            if (key == "gratuity_amount")
            {
                GratuityAmount = decimal.Parse(value);
            }
            if (key == "tender_type")
            {
                TenderType = value;
            }
            if (key == "entry_type")
            {
                EntryMethod = value;
            }
            if (key == "id")
            {
                TransactionId = value;
            }
            if (key == "reference_id")
            {
                ClientTransactionId = value;
            }
            if (key == "transaction_datetime")
            {
                ResponseDateTime = value;
            }
            if (key == "approval_code")
            {
                ApprovalCode = value;
            }
            if (key == "avs_response")
            {
                AvsResponseCode = value;
            }
            if (key == "avs_response_description")
            {
                AvsResponseText = value;
            }
            if (key == "cardsecurity_response")
            {
                CvvResultCode = value;
            }
            if (key == "cashback_amount")
            {
                CashbackAmount = value;
            }
            if (key == "type")
            {
                PaymentType = value;
            }
            if (key == "masked_card_number")
            {
                MaskedCardNumber = value;
            }
            if (key == "cardholder_name")
            {
                CardHolderName = value;
            }
            if (key == "expiry_month")
            {
                ExpMonth = value;
            }
            if (key == "expiry_year")
            {
                ExpYear = value;
            }
            if (key == "token")
            {
                Token = value;
            }            
            if (key == "balance")
            {
                BalanceAmount = decimal.Parse(value);
            }
            if (key == "postal_code")
            {
                PostalCode = value;
            }
            if (key == "rfmiq")
            {
                CustomerId = value;
            }
            if (key == "debit_trace_number")
            {
                TraceNumber = value;
            }
            if (key == "tokenization_error_code")
            {
                TokenResponseCode = value;
            }
            if (key == "tokenization_error_message")
            {
                TokenResponseMsg = value;
            }
            if (key == "amount_authorized")
            {
                AuthorizedAmount = value;
            }
            
        }

    }


    public class ICC
    {
        public ICC()
        {

        }
        public string Cardholder_Verification_Method_Results { get; set; }
        public string Issuer_Application_Data { get; set; }
        public string Terminal_Verification_Results { get; set; }
        public string Unpredictable_number { get; set; }
        public string Pos_entry_mode { get; set; }
        public string Cryptogram_information_data { get; set; }
        public string Cvm_method { get; set; }
        public string Iac_default { get; set; }
        public string Iac_denial { get; set; }
        public string Authorization_response_code { get; set; }
        public string Dedicated_file_name { get; set; }
        public string Application_label { get; set; }
        public string Application_interchange_profile { get; set; }
        public string Application_version_number { get; set; }
        public string Application_transaction_counter { get; set; }
        public string Application_usage_control { get; set; }
        public string Aapplication_preferred_name { get; set; }
        public string Application_display_name { get; set; }
        public string Application_cryptogram { get; set; }
        public string Terminal_type { get; set; }
        public string Terminal_country_code { get; set; }
        public string Tac_default { get; set; }
        public string Tac_denial { get; set; }
        public string Tac_online { get; set; }
        public string Transaction_type { get; set; }
        public string Transaction_currency_code { get; set; }
        public string Transaction_status_information { get; set; }

    }
}

