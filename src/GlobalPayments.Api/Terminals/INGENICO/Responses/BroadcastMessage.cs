using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.INGENICO;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Terminals.INGENICO {
    public class BroadcastMessage {
        private byte[] _buffer;
        private string _code;
        private string _message;
        public string Code {
            get { return _code; }
        }
        public string Message {
            get { return _message; }
        }

        private Dictionary<string, string> _broadcastData = new Dictionary<string, string> {
            {"A0", "CONNECTING" },
            {"A1", "CONNECTION MADE" },
            {"A2", "APPROVED" },
            {"A3", "DECLINED" },
            {"A4", "INSERT CARD" },
            {"A5", "CARD ERROR" },
            {"A6", "PROCESSING ERROR" },
            {"A7", "REMOVE CARD" },
            {"A8", "TRY AGAIN" },
            {"A9", "PRESENT CARD" },
            {"AA", "RE-PRESENT CARD" },
            {"AB", "CARD NOT SUPPORTED" },
            {"AC", "PRESENT ONLY ONE CARD" },
            {"AD", "PLEASE WAIT" },
            {"AE", "BAD SWIPE" },
            {"AF", "CARD EXPIRED" },
            {"B0", "DECLINED BY CARD" },
            {"B1", "PIN ENTRY" },
            {"B2", "CASHBACK AMOUNT ENTRY" },
            {"B3", "PAPER OUT" },
        };

        public BroadcastMessage(byte[] buffer) {
            _buffer = buffer;
            ParseBroadcast(_buffer);
        }

        private void ParseBroadcast(byte[] broadBuffer) {
            if (broadBuffer.Length > 0) {
                var strBroadcast = ASCIIEncoding.UTF8.GetString(broadBuffer);
                int findIndex = strBroadcast.IndexOf(INGENICO_GLOBALS.BROADCAST);
                int findLen = 14 + 2; // additional 2 is for extra char '="'
                _code = strBroadcast.Substring(findIndex + findLen, 2);
                _message = _broadcastData[_code];
            }
            else {
                throw new MessageException("No broadcast message.");
            }
        }
    }
}
