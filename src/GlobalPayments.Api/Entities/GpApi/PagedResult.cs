using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Represents a paged response returned by a GP-API reporting endpoint.
    /// </summary>
    /// <typeparam name="T">The type of records returned in the result set.</typeparam>
    public class PagedResult<T> {
        /// <summary>
        /// Gets or sets the total number of records that match the search criteria.
        /// </summary>
        public int TotalRecordCount { get; set; }
        /// <summary>
        /// Gets or sets the number of records returned on the current page.
        /// </summary>
        public int CurrentPageSize { get; set; }
        /// <summary>
        /// Gets or sets the maximum number of records requested for a page.
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Gets or sets the direction used to order the result set.
        /// </summary>
        public string Order { get; set; }
        /// <summary>
        /// Gets or sets the field used to order the result set.
        /// </summary>
        public string OrderBy { get; set; }
        /// <summary>
        /// Gets or sets the identifier of the merchant associated with the response.
        /// </summary>
        public string MerchantId { get; set; }
        /// <summary>
        /// Gets or sets the name of the merchant associated with the response.
        /// </summary>
        public string MerchantName { get; set; }
        /// <summary>
        /// Gets or sets the start of the time-created range applied to the search.
        /// </summary>
        public DateTime? FromTimeCreated { get; set; }
        /// <summary>
        /// Gets or sets the end of the time-created range applied to the search.
        /// </summary>
        public DateTime? ToTimeCreated { get; set; }
        /// <summary>
        /// Gets or sets the action metadata returned for the executed list request.
        /// </summary>
        public GlobalPayments.Api.Entities.Action Action { get; set; }
        /// <summary>
        /// Gets or sets the records returned on the current page.
        /// </summary>
        public List<T> Results { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResult{T}"/> class.
        /// </summary>
        public PagedResult() {
            Results = new List<T>();
        }

        /// <summary>
        /// Adds a record to the current page of results.
        /// </summary>
        /// <param name="item">The record to add.</param>
        public void Add(T item) {
            Results.Add(item);
        }
    }
}
