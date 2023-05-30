using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class UserLinks
    {
        /// <summary>
        /// Describes the relationship the associated link href value has to the current resource.
        /// </summary>
        public UserLevelRelationship? Rel { get; set; }

        /// <summary>
        /// A href link to the resources or resource actions as indicated in the corresponding rel value.
        /// </summary>
        public string Href { get; set; }
    }
}
