using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Utils {
    public class EmvData {
        private Dictionary<string, TlvData> tlvData;
        private Dictionary<string, TlvData> removedTags;
        private bool standInStatus;
        private string standInStatusReason;

        public TlvData GetTag(string tagName) {
            if (tlvData.ContainsKey(tagName)) {
                return tlvData[tagName];
            }
            return null;
        }
        public string GetAcceptedTagData() {
            if (tlvData.Count == 0) {
                return null;
            }

            string rvalue = "";
            foreach(TlvData tag in tlvData.Values) {
                rvalue = string.Concat(rvalue,tag.GetFullValue());
            }
            return rvalue;
        }
        public Dictionary<string, TlvData> GetAcceptedTags() { return tlvData; }
        public Dictionary<string, TlvData> GetRemovedTags() {
            return removedTags;
        }
        public bool GetStandInStatus() {
            return standInStatus;
        }
        public string GetStandInStatusReason() {
            return standInStatusReason;
        }
        public void SetStandInStatus(bool value, string reason) {
            this.standInStatus = value;
            this.standInStatusReason = reason;
        }
        public string GetCardSequenceNumber() {
            if (tlvData.ContainsKey("5F34")) {
                return tlvData["5F34"].GetValue();
            }
            return null;
        }
        public byte[] GetSendBuffer() {
            return StringUtils.BytesFromHex(GetAcceptedTagData());
        }
        public bool isContactlessMsd()
        {
            var entryMode = GetEntryMode();
            return entryMode != null ? entryMode == "91" : false;
        }

        public string GetEntryMode()
        {
            var posEntryMode = GetTag("9F39");
            if (posEntryMode!= null)
            {
                return posEntryMode.GetValue();
            }
            return null;
        }

        internal EmvData() {
            tlvData = new Dictionary<string, TlvData>();
            removedTags = new Dictionary<string, TlvData>();
        }
        internal void AddTag(string tag, string length, string value) {
            AddTag(tag, length, value, null);
        }
        internal void AddTag(string tag, string length, string value, string description) {
            AddTag(new TlvData(tag, length, value, description));
        }
        internal void AddTag(TlvData tagData) {
            tlvData[tagData.GetTag()] = tagData;
        }
        internal void AddRemovedTag(string tag, string length, string value) {
            AddRemovedTag(tag, length, value, null);
        }
        internal void AddRemovedTag(string tag, string length, string value, string description) {
            AddRemovedTag(new TlvData(tag, length, value, description));
        }
        internal void AddRemovedTag(TlvData tagData) {
            removedTags[tagData.GetTag()] = tagData;
        }
    }
}
