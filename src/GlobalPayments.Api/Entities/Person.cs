using GlobalPayments.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class Person
    {
        /// <summary>
        /// Describes the functions that a person can have in an organization.
        /// </summary>
        public PersonFunctions Functions { get; set; }

        /// <summary>
        /// Person's first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Middle's first name
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Person's last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Person's email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Person's date of birth
        /// </summary>
        public string DateOfBirth { get; set; }

        /// <summary>
        /// The national id number or reference for the person for their nationality. For example for Americans this would
        /// be SSN, for Canadians it would be the SIN, for British it would be the NIN.
        /// </summary>
        public string NationalIdReference { get; set; }

        /// <summary>
        /// The job title the person has.
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// The equity percentage the person owns of the business that is applying to Global Payments for payment processing services.
        /// </summary>
        public string EquityPercentage { get; set; }

        /// <summary>
        /// Customer's address
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// Person's home phone number
        /// </summary>
        public PhoneNumber HomePhone { get; set; }

        /// <summary>
        /// Person's work phone number
        /// </summary>
        public PhoneNumber WorkPhone { get; set; }
    }
}
