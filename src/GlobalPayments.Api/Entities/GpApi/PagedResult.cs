using System.Collections.Generic;

namespace GlobalPayments.Api.Entities {
    public class PagedResult<T> {
        public int TotalRecordCount { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
        public string Order { get; set; }
        public string OrderBy { get; set; }
        public List<T> Results { get; set; }

        public PagedResult() {
            Results = new List<T>();
        }

        public void Add(T item) {
            Results.Add(item);
        }
    }
}
