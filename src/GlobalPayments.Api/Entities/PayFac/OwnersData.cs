using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities.PayFac
{
    public class OwnersData {
        /// <summary>
        /// Owner title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Owner first name
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Owner last name
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Owner email ID
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Date of birth of the owner. Must be in 'mm-dd-yyyy' format.
        /// </summary>
        public string DateOfBirth { get; set; }
        /// <summary>
        /// Social Security Number of the owner. Should be 9 digits.
        /// </summary>
        public string SSN { get; set; }
        /// <summary>
        /// Percentage stake in company by owner. Must be whole number between 0 and 100.
        /// </summary>
        public string Percentage { get; set; }
        /// <summary>
        /// Address of the owner
        /// </summary>
        public Address OwnerAddress { get; set; }

        public OwnersData() {
            OwnerAddress = new Address();
        }
    }
}
