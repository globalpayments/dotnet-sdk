using System.Collections.Generic;

namespace GlobalPayments.Api.Entities {
    public class AutoSubstantiation {
        internal Dictionary<string, decimal?> Amounts;

        public decimal? ClinicSubTotal {
            get { return Amounts["SUBTOTAL_CLINIC_OR_OTHER_AMT"]; }
            set {
                Amounts["SUBTOTAL_CLINIC_OR_OTHER_AMT"] = value;
                Amounts["TOTAL_HEALTHCARE_AMT"] += value;
            }
        }

        public decimal? CopaySubTotal {
            get { return Amounts["SUBTOTAL_COPAY_AMT"]; }
            set {
                Amounts["SUBTOTAL_COPAY_AMT"] = value;
                Amounts["TOTAL_HEALTHCARE_AMT"] += value;
            }
        }

        public decimal? DentalSubTotal {
            get { return Amounts["SUBTOTAL_DENTAL_AMT"]; }
            set {
                Amounts["SUBTOTAL_DENTAL_AMT"] = value;
                Amounts["TOTAL_HEALTHCARE_AMT"] += value;
            }
        }
        public string MerchantVerificationValue { get; set; }

        public decimal? PrescriptionSubTotal {
            get { return Amounts["SUBTOTAL_PRESCRIPTION_AMT"]; }
            set {
                Amounts["SUBTOTAL_PRESCRIPTION_AMT"] = value;
                Amounts["TOTAL_HEALTHCARE_AMT"] += value;
            }
        }

        public bool RealTimeSubstantiation { get; set; }

        public decimal? TotalHealthcareAmount {
            get {
                return Amounts["TOTAL_HEALTHCARE_AMT"];
            }
        }

        public decimal? VisionSubTotal {
            get { return Amounts["SUBTOTAL_VISION__OPTICAL_AMT"]; }
            set {
                Amounts["SUBTOTAL_VISION__OPTICAL_AMT"] = value;
                Amounts["TOTAL_HEALTHCARE_AMT"] += value;
            }
        }

        public AutoSubstantiation() {
            Amounts = new Dictionary<string, decimal?> {
                { "TOTAL_HEALTHCARE_AMT", 0m },
                { "SUBTOTAL_PRESCRIPTION_AMT", 0m },
                { "SUBTOTAL_VISION__OPTICAL_AMT", 0m },
                { "SUBTOTAL_CLINIC_OR_OTHER_AMT", 0m },
                { "SUBTOTAL_DENTAL_AMT", 0m },
                { "SUBTOTAL_COPAY_AMT", 0m }
            };
        }
    }
}
