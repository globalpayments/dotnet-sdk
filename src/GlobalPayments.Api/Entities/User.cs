using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.Entities.PayFac;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities
{
    public class User {
        /// <summary>
        /// This is a label to identify the user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Global Payments time indicating when the object was created in ISO-8601 format.
        /// </summary>
        public DateTime? TimeCreated { get; set; }

        /// <summary>
        /// The date and time the resource object was last changed.
        /// </summary>
        public DateTime? TimeLastUpdated { get; set; }

        public string Email { get; set; }

        public List<Address> Addresses { get; set; }

        public PhoneNumber ContactPhone { get; set; }

        /// <summary>
        /// A further description of the status of merchant boarding.
        /// </summary>
        public string StatusDescription { get; set; }

        /// <summary>
        /// The result of the action executed.
        /// </summary>
        public string ResponseCode { get; set; }


        public UserReference UserReference { get; set; }

        public List<Person> PersonList { get; set; }

        public List<PaymentMethodList> PaymentMethods { get; set; }

        public Document Document { get; set; }

        public FundsAccountDetails FundsAccountDetails { get; set; }

        /// <summary>
        /// Creates an `User` object from an existing user ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userType"></param>
        /// <returns></returns>
        public static User FromId(string userId, UserType userType)
        {
            User user = new User();
            user.UserReference = new UserReference();
            user.UserReference.UserId = userId;
            user.UserReference.UserType = userType;
            
            return user;
        }

        public PayFacBuilder<User> Edit()
        {
            PayFacBuilder<User> builder = new PayFacBuilder<User>(TransactionType.Edit)
                         .WithUserReference(this.UserReference);

            if (UserReference.UserType != null) {
                builder = builder.WithModifier(EnumConverter.FromDescription<TransactionModifier>(UserReference.UserType.ToString()));
            }

            return builder;
        }

        public PayFacBuilder<User> UploadDocument(DocumentUploadData data)
        {
            PayFacBuilder<User> builder = new PayFacBuilder<User>(TransactionType.UploadDocument)
                         .WithUserReference(this.UserReference)
                         .WithDocumentUploadData(data);

            if (UserReference.UserType != null) {
                builder = builder.WithModifier(EnumConverter.FromDescription<TransactionModifier>(UserReference.UserType.ToString()));
            }
            return builder;
        }

        public PayFacBuilder<User> AddFunds()
        {
            PayFacBuilder<User> builder = new PayFacBuilder<User>(TransactionType.AddFunds)
                         .WithUserReference(this.UserReference);
            
            return builder;
        }
        
    }
}
