using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Globalization;

namespace TeamSupport.Data
{
    public partial class User : BaseItem
    {
        public UserProxy GetProxy()
        {

            UserProxy result = new UserProxy();
            result.CanBulkMerge = this.CanBulkMerge;
            result.PortalLimitOrgChildrenTickets = this.PortalLimitOrgChildrenTickets;
            result.ImportFileID = this.ImportFileID;
            result.verificationCode = this.verificationCode;
            result.verificationPhoneNumber = this.verificationPhoneNumber;
            result.CalGUID = this.CalGUID;
            result.BlockEmailFromCreatingOnly = this.BlockEmailFromCreatingOnly;
            result.ProductFamiliesRights = (ProductFamiliesRightType)this.ProductFamiliesRights;
            result.DisableExporting = this.DisableExporting;
            result.DisablePublic = this.DisablePublic;
            result.PortalLimitOrgTickets = this.PortalLimitOrgTickets;
            result.UserCanPinAction = this.UserCanPinAction;
            result.AllowUserToEditAnyAction = this.AllowUserToEditAnyAction;
            result.RestrictUserFromEditingAnyActions = this.RestrictUserFromEditingAnyActions;
            result.FontSize = this.FontSize;
            result.FontFamily = this.FontFamily;
            result.FontSizeDescription = this.FontSizeDescription;
            result.FontFamilyDescription = this.FontFamilyDescription;
            result.AllowAnyTicketCustomer = this.AllowAnyTicketCustomer;
            result.NeedsIndexing = this.NeedsIndexing;
            result.EnforceSingleSession = this.EnforceSingleSession;
            result.SalesForceID = this.SalesForceID;
            if (!this.LinkedIn.StartsWith("http"))
                result.LinkedIn = "http://" + this.LinkedIn;
            else
                result.LinkedIn = this.LinkedIn;
            result.TicketRights = this.TicketRights;
            result.MenuItems = this.MenuItems;
            result.OnlyEmailAfterHours = this.OnlyEmailAfterHours;
            result.BlockInboundEmail = this.BlockInboundEmail;
            result.ShowWelcomePage = this.ShowWelcomePage;
            result.IsClassicView = this.IsClassicView;
            result.DoNotAutoSubscribe = this.DoNotAutoSubscribe;
            result.OrgsUserCanSeeOnPortal = this.OrgsUserCanSeeOnPortal;
            result.ModifierID = this.ModifierID;
            result.CreatorID = this.CreatorID;
            result.ImportID = this.ImportID;
            result.SessionID = this.SessionID;
            result.LastVersion = this.LastVersion;
            result.OrganizationID = this.OrganizationID;
            result.SubscribeToNewTickets = this.SubscribeToNewTickets;
            result.SubscribeToNewActions = this.SubscribeToNewActions;
            result.ReceiveAllGroupNotifications = this.ReceiveAllGroupNotifications;
            result.ReceiveUnassignedGroupEmails = this.ReceiveUnassignedGroupEmails;
            result.ReceiveTicketNotifications = this.ReceiveTicketNotifications;
            result.InOfficeComment = this.InOfficeComment;
            result.InOffice = this.InOffice;
            result.PrimaryGroupID = this.PrimaryGroupID;
            result.IsChatUser = this.IsChatUser;
            result.IsPortalUser = this.IsPortalUser;
            result.IsPasswordExpired = this.IsPasswordExpired;
            result.IsFinanceAdmin = this.IsFinanceAdmin;
            result.IsSystemAdmin = this.IsSystemAdmin;
            result.LastWaterCoolerID = this.LastWaterCoolerID;
            result.CultureName = this.CultureName;
            result.TimeZoneID = this.TimeZoneID;
            result.MarkDeleted = this.MarkDeleted;
            result.IsActive = this.IsActive;
            result.CryptedPassword = this.CryptedPassword;
            result.Title = (this.Title);
            result.LastName = (this.LastName);
            result.MiddleName = (this.MiddleName);
            result.FirstName = (this.FirstName);
            result.Email = (this.Email);
            result.UserID = this.UserID;

            result.LastLogin = DateTime.SpecifyKind(this.LastLoginUtc, DateTimeKind.Utc);
            result.LastActivity = DateTime.SpecifyKind(this.LastActivityUtc, DateTimeKind.Utc);
            result.ActivatedOn = DateTime.SpecifyKind(this.ActivatedOnUtc, DateTimeKind.Utc);
            result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
            result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);

            result.PasswordCreatedUtc = this.PasswordCreatedUtcUtc == null ? this.PasswordCreatedUtcUtc : DateTime.SpecifyKind((DateTime)this.PasswordCreatedUtcUtc, DateTimeKind.Utc);
            result.verificationCodeExpiration = this.verificationCodeExpirationUtc == null ? this.verificationCodeExpirationUtc : DateTime.SpecifyKind((DateTime)this.verificationCodeExpirationUtc, DateTimeKind.Utc);
            result.DeactivatedOn = this.DeactivatedOnUtc == null ? this.DeactivatedOnUtc : DateTime.SpecifyKind((DateTime)this.DeactivatedOnUtc, DateTimeKind.Utc);
            result.LastPing = this.LastPingUtc == null ? this.LastPingUtc : DateTime.SpecifyKind((DateTime)this.LastPingUtc, DateTimeKind.Utc);
            if (!string.IsNullOrEmpty(this.TimeZoneID))
            {
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(this.TimeZoneID);
                if (timeZone != null) result.timeZoneDisplay = timeZone.DisplayName;
            }
            if (!string.IsNullOrEmpty(this.CultureName))
            {
                CultureInfo culture = new CultureInfo(this.CultureName);
                if (culture != null) result.CultureDisplay = culture.DisplayName;
            }
            result.UserInformation = this.UserInformation;
            result.AppChatID = this.AppChatID;
            result.AppChatStatus = this.AppChatStatus;
            result.ChangeTicketVisibility = this.ChangeTicketVisibility;
            result.CanChangeCommunityVisibility = this.CanChangeCommunityVisibility;
            result.FilterInactive = this.FilterInactive;
            result.ChangeKbVisibility = this.ChangeKBVisibility;
            result.CanCreateCompany = this.CanCreateCompany;
            result.CanCreateContact = this.CanCreateContact;
            result.CanEditCompany = this.CanEditCompany;
            result.CanEditContact = this.CanEditContact;
            result.CanCreateAsset = this.CanCreateAsset;
            result.CanEditAsset = this.CanEditAsset;
            result.CanCreateProducts = this.CanCreateProducts;
            result.CanEditProducts = this.CanEditProducts;
            result.CanCreateVersions = this.CanCreateVersions;
            result.CanEditVersions = this.CanEditVersions;
            Attachments att = new Attachments(BaseCollection.LoginUser);
            att.LoadByReference(ReferenceType.UserPhoto, this.UserID);

            if (att.Count > 0)
            {
                result.Avatar = String.Format("/dc/{0}/avatar/{1}", this.OrganizationID, att[0].AttachmentID);
            }
            else
                result.Avatar = "../images/blank_avatar.png";



            return result;
        }
    }
}
