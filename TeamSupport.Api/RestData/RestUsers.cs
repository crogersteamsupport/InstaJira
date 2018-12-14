using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using TeamSupport.Data;
using System.Net;

namespace TeamSupport.Api
{
    public class RestUsers
    {

        public static string GetUser(RestCommand command, int userID)
        {
            UsersViewItem item = UsersView.GetUsersViewItem(command.LoginUser, userID);
            if (item.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

            return item.GetXml("User", true);
        }

        public static string GetUsers(RestCommand command, bool orderByDateCreated = false)
        {
            UsersView items = new UsersView(command.LoginUser);
            if (orderByDateCreated)
            {
                items.LoadByOrganizationID(command.Organization.OrganizationID, false, "DateCreated DESC");
            }
            else
            {
                items.LoadByOrganizationID(command.Organization.OrganizationID, false);
            }
            return items.GetXml("Users", "User", true, command.Filters);
        }

        public static string CreateUser(RestCommand command)
        {

            if (Organizations.GetUserCount(command.LoginUser, command.LoginUser.OrganizationID) >= command.Organization.UserSeats)
            {
                throw new RestException(HttpStatusCode.Forbidden, "You are already at your maximum user seat count for your license.");
            }
            Addresses addresses = new Addresses(command.LoginUser);
            Address address = addresses.AddNewAddress();

            PhoneNumbers phoneNumbers = new PhoneNumbers(command.LoginUser);
            PhoneNumber phoneNumber = phoneNumbers.AddNewPhoneNumber();

            Users users = new Users(command.LoginUser);
            User user = users.AddNewUser();
            user.FullReadFromXml(command.Data, true, ref phoneNumber, ref address);
            if (String.IsNullOrWhiteSpace(user.FirstName))
            {
                throw new RestException(HttpStatusCode.BadRequest, "FirstName is required.");
            };
            if (String.IsNullOrWhiteSpace(user.LastName))
            {
                throw new RestException(HttpStatusCode.BadRequest, "LastName is required.");
            };
            if (String.IsNullOrWhiteSpace(user.Email) || user.Email.IndexOf('@') < 0 || user.Email.IndexOf('.') < 0 || !users.IsEmailValid(user.Email, -1, command.LoginUser.OrganizationID))
            {
                throw new RestException(HttpStatusCode.BadRequest, "Email is invalid.");
            };
            user.OrganizationID = command.LoginUser.OrganizationID;
            user.ActivatedOn = DateTime.UtcNow;
            user.LastLogin = DateTime.UtcNow.AddHours(-1);
            user.LastActivity = DateTime.UtcNow.AddHours(-1);
            user.EnforceSingleSession = true;
            user.NeedsIndexing = true;
            user.Collection.Save();
            user.UpdateCustomFieldsFromXml(command.Data);

            if (!String.IsNullOrEmpty(phoneNumber.Number) || !String.IsNullOrEmpty(phoneNumber.Extension))
            {
                phoneNumber.RefType = ReferenceType.Users;
                phoneNumber.RefID = user.UserID;
                phoneNumbers.Save();
            }

            if (!String.IsNullOrEmpty(address.Description)
            || !String.IsNullOrEmpty(address.Addr1)
            || !String.IsNullOrEmpty(address.Addr2)
            || !String.IsNullOrEmpty(address.Addr3)
            || !String.IsNullOrEmpty(address.City)
            || !String.IsNullOrEmpty(address.Country)
            || !String.IsNullOrEmpty(address.Description)
            || !String.IsNullOrEmpty(address.State)
            || !String.IsNullOrEmpty(address.Zip))
            {
                address.RefType = ReferenceType.Users;
                address.RefID = user.UserID;
                addresses.Save();
            }

            return UsersView.GetUsersViewItem(command.LoginUser, user.UserID).GetXml("User", true);
        }

        public static string UpdateUser(RestCommand command, int userID)
        {
            User user = Users.GetUser(command.LoginUser, userID);
            if (user == null) throw new RestException(HttpStatusCode.BadRequest);
            Organization organization = Organizations.GetOrganization(command.LoginUser, user.OrganizationID);
            if (organization.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
            user.ReadFromXml(command.Data, false);
            user.Collection.Save();
            user.UpdateCustomFieldsFromXml(command.Data);
            return UsersView.GetUsersViewItem(command.LoginUser, user.UserID).GetXml("User", true);
        }

        public static string DeleteUser(RestCommand command, int userID)
        {
            UsersViewItem user = UsersView.GetUsersViewItem(command.LoginUser, userID);
            if (user == null) throw new RestException(HttpStatusCode.BadRequest);
            Organization organization = Organizations.GetOrganization(command.LoginUser, user.OrganizationID);
            if (organization.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
            string result = user.GetXml("User", true);
            Users.MarkUserDeleted(command.LoginUser, userID);
            return result;
        }


    }
}
