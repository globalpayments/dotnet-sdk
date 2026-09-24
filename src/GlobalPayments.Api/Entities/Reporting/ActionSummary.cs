using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Represents the details of an action returned by the GP-API Actions endpoints.
    /// </summary>
    public class ActionSummary {
        /// <summary>
        /// Gets the unique identifier of the action.
        /// </summary>
        public string Id { get; internal set; }
        /// <summary>
        /// Gets the type of action performed.
        /// </summary>
        public string Type { get; internal set; }
        /// <summary>
        /// Gets the date and time when the action was created.
        /// </summary>
        public DateTime TimeCreated { get; internal set; }
        /// <summary>
        /// Gets the resource affected by the action.
        /// </summary>
        public string Resource { get; internal set; }
        /// <summary>
        /// Gets the URL used to request the affected resource.
        /// </summary>
        public string ResourceRequestUrl { get; internal set; }
        /// <summary>
        /// Gets the API version used for the action.
        /// </summary>
        public string Version { get; internal set; }
        /// <summary>
        /// Gets the identifier of the parent resource associated with the action.
        /// </summary>
        public string ResourceParentId { get; internal set; }
        /// <summary>
        /// Gets the identifier of the resource affected by the action.
        /// </summary>
        public string ResourceId { get; internal set; }
        /// <summary>
        /// Gets the status of the affected resource.
        /// </summary>
        public string ResourceStatus { get; internal set; }
        /// <summary>
        /// Gets the HTTP response code returned for the action.
        /// </summary>
        public string HttpResponseCode { get; internal set; }
        /// <summary>
        /// Gets the HTTP response message returned for the action.
        /// </summary>
        public string HttpResponseMessage { get; internal set; }
        /// <summary>
        /// Gets the GP-API response code returned for the action.
        /// </summary>
        public string ResponseCode { get; internal set; }
        /// <summary>
        /// Gets the detailed response code returned for the action.
        /// </summary>
        public string ResponseDetailedCode { get; internal set; }
        /// <summary>
        /// Gets the detailed response message returned for the action.
        /// </summary>
        public string ResponseDetailedMessage { get; internal set; }
        /// <summary>
        /// Gets the identifier of the application that initiated the action.
        /// </summary>
        public string AppId { get; internal set; }
        /// <summary>
        /// Gets the name of the application that initiated the action.
        /// </summary>
        public string AppName { get; internal set; }
        /// <summary>
        /// Gets the developer associated with the application that initiated the action.
        /// </summary>
        public string AppDeveloper { get; internal set; }
        /// <summary>
        /// Gets the identifier of the merchant associated with the action.
        /// </summary>
        public string MerchantId { get; internal set; }
        /// <summary>
        /// Gets the identifier of the account associated with the action.
        /// </summary>
        public string AccountId { get; internal set; }
        /// <summary>
        /// Gets the name of the account associated with the action.
        /// </summary>
        public string AccountName { get; internal set; }
        /// <summary>
        /// Gets the name of the merchant associated with the action.
        /// </summary>
        public string MerchantName { get; internal set; }
        /// <summary>
        /// Gets the source location from which the action request originated.
        /// </summary>
        public string SourceLocation { get; internal set; }
        /// <summary>
        /// Gets the destination location targeted by the action request.
        /// </summary>
        public string DestinationLocation { get; internal set; }
        /// <summary>
        /// Gets the metrics (X-GP header fields) returned for the action.
        /// </summary>
        public Dictionary<string, string> Metrics { get; internal set; }
    }
}
