using System.Collections.Generic;
using System.Linq;

namespace GlobalPayments.Api.Entities.TableService {
    public class BumpStatusCollection {
        private Dictionary<string, int> _bumpStatus;

        public int this[string status] {
            get {
                if (_bumpStatus.ContainsKey(status))
                    return _bumpStatus[status];
                return 0;
            }
        }
        public string[] Keys {
            get { return _bumpStatus.Select(x => x.Key).ToArray(); }
        }

        public BumpStatusCollection(string statusString) {
            _bumpStatus = new Dictionary<string, int>();

            int index = 1;
            foreach (var status in statusString.Split(',')) {
                _bumpStatus.Add(status, index++);
            }
        }
    }
}
