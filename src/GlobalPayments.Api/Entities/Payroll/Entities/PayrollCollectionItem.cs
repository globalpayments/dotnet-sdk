using GlobalPayments.Api.Utils;
using System.Collections.Generic;

namespace GlobalPayments.Api.Entities.Payroll {
    public abstract class PayrollCollectionItem : PayrollEntity {
        private string _idField;
        private string _descriptionField;

        public string Id { get; set; }
        public string Description { get; set; }

        public PayrollCollectionItem(string idFieldName, string descriptionFieldName) {
            _idField = idFieldName;
            _descriptionField = descriptionFieldName;
        }

        internal override void FromJson(JsonDoc doc, PayrollEncoder encoder) {
            Id = doc.GetValue<string>(_idField);
            Description = doc.GetValue<string>(_descriptionField);
        }
    }

    public class WorkLocation : PayrollCollectionItem {
        public WorkLocation() : base("WorkLocationId", "WorkLocationDescription") { }
    }
    public class TerminationReason : PayrollCollectionItem {
        public TerminationReason() : base("TerminationReasonId", "ReasonDescription") { }
    }
    public class LaborField : PayrollCollectionItem {
        public List<LaborFieldLookup> Lookup { get; private set; }

        public LaborField() : base("LaborFieldId", "LaborFieldValue") { }

        internal override void FromJson(JsonDoc doc, PayrollEncoder encoder) {
            base.FromJson(doc, encoder);

            if(Description == null)
                Description = doc.GetValue<string>("LaborFieldTitle");

            if (doc.Has("laborfieldLookups")) {
                Lookup = new List<LaborFieldLookup>();
                foreach (var lookup in doc.GetEnumerator("laborfieldLookups")) {
                    Lookup.Add(new LaborFieldLookup {
                        Description = lookup.GetValue<string>("laborFieldDescription"),
                        Value = lookup.GetValue<string>("laborFieldValue")
                    });
                }
            }
        }
    }
    public class PayGroup : PayrollCollectionItem {
        public PayGroupFrequency Frequency { get; private set; }

        public PayGroup() : base("PayGroupId", "PayGroupName") { }

        internal override void FromJson(JsonDoc doc, PayrollEncoder encoder) {
            base.FromJson(doc, encoder);
            Frequency = doc.GetValue("payFrequency", EnumConverter.FromDescription<PayGroupFrequency>);
        }
    }
    public class PayItem : PayrollCollectionItem {
        public PayItem() : base("PayCode", "PayItemTitle") { }
    }

    public class LaborFieldLookup {
        public string Value { get; set; }
        public string Description { get; set; }
    }
}
