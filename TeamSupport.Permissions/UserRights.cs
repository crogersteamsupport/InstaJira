using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSupport.Data;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Data.Linq;

namespace TeamSupport.Permissions
{
    public class UserRights
    {
        //public bool CanOpenAttachment(ConnectionContext context, AttachmentProxy attachment)
        public static bool CanOpenAttachment(LoginUser loginUser, AttachmentProxy attachment)
        {
            switch (attachment.RefType)
            {
                case AttachmentProxy.References.Actions:
                case AttachmentProxy.References.Tasks:
                case AttachmentProxy.References.Organizations:
                    return OrganizationOrParentOrganization(loginUser, attachment);

                case AttachmentProxy.References.ProductVersions:
                    return CheckProduct(loginUser, attachment);

                case AttachmentProxy.References.UserPhoto:
                case AttachmentProxy.References.CustomerHubLogo:
                case AttachmentProxy.References.Assets:
                case AttachmentProxy.References.ChatAttachments:
                case AttachmentProxy.References.CompanyActivity:
                case AttachmentProxy.References.ContactActivity:
                case AttachmentProxy.References.Contacts:
                case AttachmentProxy.References.Users:
                case AttachmentProxy.References.WaterCooler:
                default:
                    return true;  // no authentication required (HubLogo...)
            }
        }

        static bool TryGetParentID(DataContext db, LoginUser loginUser, out int parentID)
        {
            string query = $"SELECT ParentID FROM Organizations WHERE OrganizationID={loginUser.OrganizationID}";
            parentID = db.ExecuteQuery<int>(query).Min();
            return parentID != 1;   // no parent organization?
        }

        static bool TryGetParentID(DataContext db, AttachmentProxy attachment, out int parentID)
        {
            string query = $"SELECT ParentID FROM Attachments att JOIN Organizations o on att.OrganizationID=o.OrganizationID WHERE att.AttachmentID={attachment.AttachmentID}";
            parentID = db.ExecuteQuery<int>(query).Min();
            return parentID != 1;   // attachment creator org have ParentID?
        }

        static bool OrganizationOrParentOrganization(LoginUser loginUser, AttachmentProxy attachment)
        {
            // same organization
            if (attachment.OrganizationID == loginUser.OrganizationID)
                return true;

            // User in PARENT Organization can see attachments created by a user from:
            //     * same parent organization
            //     * any child organizations
            // User in child organization can see attachments created by a user from:
            //     * their organization
            //     * in some cases those in the parent organization
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
            using (DataContext db = new DataContext(connection))
            {
                // attachment in parent organization
                if (TryGetParentID(db, loginUser, out int parentID) && (attachment.OrganizationID == parentID))
                    return true;

                // loginUser in parent organization
                if (TryGetParentID(db, attachment, out parentID))   // attachment created by user from child organization
                    return (loginUser.OrganizationID == parentID);
            }

            return false;
        }

        static bool CheckProduct(LoginUser loginUser, AttachmentProxy attachment)
        {
            // Member of parent org?
            if (attachment.OrganizationID == loginUser.OrganizationID)
                return true;

            // contact has access to product version?
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
            using (DataContext db = new DataContext(connection))
            {
                string query = @"Select a.AttachmentID FROM Users as u 
                    JOIN OrganizationProducts op on op.OrganizationID = u.OrganizationID
                    JOIN Products p on p.ProductID = op.ProductID 
                    JOIN ProductVersions pv on pv.ProductID = op.ProductID 
                    JOIN Attachments a on a.RefID = pv.ProductVersionID 
                    WHERE " + $"a.AttachmentID={attachment.AttachmentID} AND u.UserID={loginUser.UserID} AND a.RefType={(int)AttachmentProxy.References.ProductVersions}";

                bool any = db.ExecuteQuery<int>(query).Any();
                return any;
            }
        }

    }
}
