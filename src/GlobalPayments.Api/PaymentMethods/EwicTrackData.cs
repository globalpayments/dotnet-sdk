using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.PaymentMethods {
    public class EwicTrackData : Ewic, ITrackData {
        private string _trackData;
        private string _value;

        public string DiscretionaryData { get; set; }
        public EncryptionData EncryptionData { get; set; }
        public EntryMethod EntryMethod { get; set; }
        public string Expiry { get; set; }
        public string Pan { get; set; }
        public string PurchaseDeviceSequenceNumber { get; set; }
        public TrackNumber TrackNumber { get; set; }
        public string TrackData {
            get { return _trackData; }
            set {
                if (_value == null) {
                    Value = value;
                }
                else _trackData = value;
            }
        }
        public string Value {
            get { return _value; }
            set {
                _value = value;
                CardUtils.ParseTrackData(this);
            }
        }

        public EwicTrackData() {
        }
    }
}
