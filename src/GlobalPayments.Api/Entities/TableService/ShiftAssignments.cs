using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.TableService {
    public class ShiftAssignments : Dictionary<string, IEnumerable<int>> {
        public override string ToString() {
            var sb = new StringBuilder();

            foreach (var key in this.Keys) {
                sb.Append(key + "-");
                sb.Append(string.Join(",", this[key]));
                sb.Append("|");
            }

            return sb.ToString().TrimEnd('|');
        }
    }
}
