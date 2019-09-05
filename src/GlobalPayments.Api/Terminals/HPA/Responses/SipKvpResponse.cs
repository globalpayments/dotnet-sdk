using GlobalPayments.Api.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GlobalPayments.Api.Terminals.HPA.Responses {
    public class SipKvpResponse : SipBaseResponse {
        protected string category;
        protected string lastCategory;
        protected VariableDictionary fieldValues;

        public SipKvpResponse(byte[] buffer, params string[] messageIds) : base(buffer, messageIds) { }

        internal override void MapResponse(Element response) {
            base.MapResponse(response);

            //set category
            category = response.GetValue<string>("TableCategory");
            if (category == null) {
                category = lastCategory;
            }

            fieldValues = new VariableDictionary();
            foreach (Element field in response.GetAll("Field")) {
                fieldValues.Add(field.GetValue<string>("Key"), field.GetValue<string>("Value"));
            }
        }

        private string FormatCategory(string category) {
            string[] elements = category.Split(new char[0]);

            StringBuilder sb = new StringBuilder(elements[0].ToLower());
            for (int i = 1; i < elements.Length; i++) {
                string element = elements[i];
                sb.Append(element.Substring(0, 1).ToUpper());
                sb.Append(element.Substring(1).ToLower());
            }
            return sb.ToString();
        }
    }

    public class VariableDictionary : Dictionary<string, string> { }
}
