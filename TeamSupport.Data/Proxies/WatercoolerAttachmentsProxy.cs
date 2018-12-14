using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;


namespace TeamSupport.Data
{

    public partial class WatercoolerAttachment : BaseItem
    {
        public WatercoolerAttachmentProxy GetProxy()
        {
            WatercoolerAttachmentProxy result = new WatercoolerAttachmentProxy();
            result.CreatorID = this.CreatorID;
            result.RefType = this.RefType;
            result.AttachmentID = this.AttachmentID;
            result.MessageID = this.MessageID;

            result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);

            result.CreatorName = Users.GetUserFullName(BaseCollection.LoginUser, this.CreatorID);

            if (this.RefType == WaterCoolerAttachmentType.Group)
            {
                var group = Groups.GetGroup(BaseCollection.LoginUser, this.AttachmentID);
                if (group != null)
                    result.GroupName = group.Name;
                else
                    return null;
            }

            if (this.RefType == WaterCoolerAttachmentType.Ticket)
            {
                var ticket = Tickets.GetTicketByNumber(BaseCollection.LoginUser, this.AttachmentID);
                if (ticket != null)
                    result.TicketName = ticket.Name;
                else
                    return null;
            }

            if (this.RefType == WaterCoolerAttachmentType.Product)
            {
                var product = Products.GetProduct(BaseCollection.LoginUser, this.AttachmentID);
                if (product != null)
                    result.ProductName = product.Name;
                else
                    return null;
            }

            if (this.RefType == WaterCoolerAttachmentType.Company)
            {
                var organization = Organizations.GetOrganization(BaseCollection.LoginUser, this.AttachmentID);
                if (organization != null)
                    result.CompanyName = organization.Name;
                else
                    return null;
            }

            if (this.RefType == WaterCoolerAttachmentType.User)
            {
                var user = Users.GetUserFullName(BaseCollection.LoginUser, this.AttachmentID);
                if (user != null)
                    result.UserName = user;
                else
                    return null;
            }

            if (this.RefType == WaterCoolerAttachmentType.Activities)
            {
                var activity = Notes.GetNote(BaseCollection.LoginUser, this.AttachmentID);
                if (activity != null)
                {
                    result.ActivityRefType = activity.RefType;
                    result.ActivityTitle = activity.Title;
                    result.ActivityRefID = activity.RefID;
                }
                else
                    return null;
            }

            return result;
        }
    }
}
