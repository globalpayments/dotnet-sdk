using GlobalPayments.Api.Utils;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPayments.Api.Terminals {
    public class ValidationRequest {
        private List<string> MandatoryParams;

        public void SetMandatoryParams(List<string> mandatoryParams) {
            MandatoryParams = mandatoryParams;
        }

        public List<string> GetMandatoryParams() {
            return MandatoryParams;
        }

        public void Validate(JsonDoc builder, out List<string> missingParams) {
            var paramsIn = GetParamsInside(builder);
            missingParams = MandatoryParams.Except(paramsIn).ToList();
        }

        private List<string> GetParamsInside(JsonDoc builder, List<string> paramsIn = null) {
            var builderString = builder.ToString();
            if (paramsIn == null)
                paramsIn = new List<string>();

            foreach (var param in MandatoryParams) {
                if (builderString.Contains(param)) {
                    paramsIn.Add(param);
                }
            }
            return paramsIn;
        }
    }
}
