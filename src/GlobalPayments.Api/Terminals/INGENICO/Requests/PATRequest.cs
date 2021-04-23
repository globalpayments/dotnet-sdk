using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GlobalPayments.Api.Terminals.Ingenico.Requests {
    public class PATRequest {
        private byte[] _buffer;

        public PATRequestType RequestType { get; set; }
        public string WaiterId { get; set; }
        public string TableId { get; set; }
        public string TerminalId { get; set; }
        public string TerminalCurrency { get; set; }
        public string XMLData { get; set; }
        public TransactionOutcomeRequest TransactionOutcome { get; set; }

        public override string ToString() => Encoding.GetEncoding(28591).GetString(_buffer);

        // Assign passed value of buffer into private variable.
        public PATRequest(byte[] buffer) {
            _buffer = buffer;
            ParseData();
        }

        private void ParseData() {
            string strBuffer = Encoding.GetEncoding(28591).GetString(_buffer);

            // XML
            if (strBuffer.Contains(INGENICO_GLOBALS.XML_TAG)) {
                if (!strBuffer.EndsWith(">")) {
                    char[] xmlArr = strBuffer.ToCharArray();

                    for (int i = strBuffer.Length - 1; i <= strBuffer.Length; i--) {
                        if (xmlArr[i] == '>') {
                            XMLData = strBuffer.Substring(0, (i + 1));
                            break;
                        }
                    }
                } else {
                    XMLData = strBuffer;
                }

                // Convert String to XML
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(XMLData);

                XmlElement root = xmlDoc.DocumentElement;
                string rootTag = root.Name;

                if (rootTag == INGENICO_GLOBALS.ADDITIONAL_MSG) {
                    RequestType = PATRequestType.AdditionalMessage;
                } else if (rootTag == INGENICO_GLOBALS.TRANSFER_DATA) {
                    RequestType = PATRequestType.TransferData;
                } else if (rootTag == INGENICO_GLOBALS.TRANSACTION_XML) {
                    XmlNodeList nList = xmlDoc.GetElementsByTagName("RECEIPT");
                    XmlNode node = nList.Item(0);

                    if (node.NodeType == XmlNodeType.Element) {
                        XmlElement element = (XmlElement)node;
                        string sType = element.GetAttribute("STYPE");

                        if (sType.Equals("SPLITSALE REPORT")) {
                            RequestType = PATRequestType.SplitSaleReport;
                        } else if (sType.Equals("CUSTOMER")) {
                            RequestType = PATRequestType.Ticket;
                        } else {
                            RequestType = PATRequestType.EndOfDayReport;
                        }
                    } else {
                        throw new ApiException("First child node is not an element");
                    }
                } else {
                    throw new ApiException("The root tag of the xml cannot recognize");
                }
            } else {
                // Workaround for split sale but not final logic
                if (strBuffer.ToLower().Contains("split_sale")) {
                    RequestType = PATRequestType.SplitSaleReport;
                    XMLData = strBuffer;
                }

                // Message Frame 2
                else if (_buffer.Length >= INGENICO_GLOBALS.MSG_FRAME_TWO_LEN) {
                    RequestType = PATRequestType.TransactionOutcome;
                    TransactionOutcome = new TransactionOutcomeRequest(_buffer);
                } else {
                    // Message Frame 1
                    RequestType = (PATRequestType)strBuffer.Substring(11, 1).ToInt32();
                    string privData = strBuffer.Substring(16);

                    if (privData.Length < 55) {
                        switch (RequestType) {
                            case PATRequestType.TableLock:
                            case PATRequestType.TableUnlock:
                                TableId = privData;
                                break;
                            default:
                                break;
                        }
                    } else {
                        var tlvData = new TypeLengthValue(_buffer);

                        WaiterId = tlvData.GetValue((byte)PATPrivateDataCode.WaiterId, typeof(string))?.ToString();
                        TableId = tlvData.GetValue((byte)PATPrivateDataCode.TableId, typeof(string), TLVFormat.PayAtTable)?.ToString();
                        TerminalId = tlvData.GetValue((byte)PATPrivateDataCode.TerminalId, typeof(string))?.ToString();
                        TerminalCurrency = tlvData.GetValue((byte)PATPrivateDataCode.TerminalCurrency, typeof(string))?.ToString();
                    }
                }
            }
        }
    }
}
