using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Linq;
using TeamSupport.Data.BusinessObjects;
using Newtonsoft.Json;

namespace TeamSupport.Data
{

    public partial class Ticket
    {
        public TicketsViewItem GetTicketView()
        {
            return TicketsView.GetTicketsViewItem(BaseCollection.LoginUser, TicketID);
        }

        public void RemoveCommunityTicket()
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "DELETE FROM ForumTickets WHERE (TicketID = @TicketID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", TicketID);
                Collection.ExecuteNonQuery(command, "ForumTickets");
            }
        }

        public void AddCommunityTicket(int forumCategoryID)
        {
            RemoveCommunityTicket();
            ForumCategory cat = ForumCategories.GetForumCategory(Collection.LoginUser, forumCategoryID);
            if (cat == null) return;
            if (cat.OrganizationID != Collection.LoginUser.OrganizationID) return;

            ForumTicket ft = (new ForumTickets(Collection.LoginUser)).AddNewForumTicket();
            ft.TicketID = TicketID;
            ft.ForumCategory = forumCategoryID;
            ft.ViewCount = 0;
            ft.Collection.Save();
        }

        public static bool UserHasRights(User user, int? groupID, int? userID, int? ticketID, bool isKB)
        {
            if ((ProductFamiliesRightType)user.ProductFamiliesRights != ProductFamiliesRightType.AllFamilies)
            {
                if (ticketID != null && !Ticket.UserHasProductFamilyRights(user, (int)ticketID))
                {
                    return false;
                }
            }

            if (isKB) return true;

            if (user.TicketRights == TicketRightType.Assigned && (userID == null || userID != user.UserID)) return false;

            if (user.TicketRights == TicketRightType.Groups && (userID != null || groupID != null))
            {
                if (userID == null || userID != user.UserID)
                {
                    Groups groups = new Groups(user.Collection.LoginUser);
                    if (groupID != null)
                    {
                        groups.LoadByUserID(user.UserID);
                        foreach (Group group in groups)
                        {
                            if (group.GroupID == groupID) { return true; }
                        }
                    }
                    return false;
                }
            }
            if (user.TicketRights == TicketRightType.Customers)
            {
                if (userID == null || userID != user.UserID)
                {
                    SqlCommand command = new SqlCommand();
                    command.CommandText = @"
SELECT COUNT(*)
FROM OrganizationTickets ot
INNER JOIN UserRightsOrganizations uro ON ot.OrganizationID = uro.OrganizationID
WHERE uro.UserID = @UserID
AND ot.TicketID = @TicketID
";
                    command.Parameters.AddWithValue("UserID", user.UserID);
                    command.Parameters.AddWithValue("TicketID", ticketID);
                    return SqlExecutor.ExecuteInt(user.Collection.LoginUser, command) > 0;
                }
            }
            return true;
        }

        public static bool UserHasRights(User user, Ticket ticket)
        {
            return UserHasRights(user, ticket.GroupID, ticket.UserID, ticket.TicketID, ticket.IsKnowledgeBase);
        }

        public bool UserHasRights(User user)
        {
            return UserHasRights(user, this.GroupID, this.UserID, this.TicketID, this.IsKnowledgeBase);
        }

        public static bool UserHasProductFamilyRights(User user, int ticketID)
        {
            ProductFamilies productFamiliesWithRights = new ProductFamilies(user.Collection.LoginUser);
            productFamiliesWithRights.LoadByUserRights(user.UserID);
            TicketsViewItem ticket = TicketsView.GetTicketsViewItem(user.Collection.LoginUser, ticketID);
            bool result = false;
            if (ticket.UserID == user.UserID || ticket.ProductID == null)
            {
                result = true;
            }
            else
            {
                foreach (ProductFamily productFamilyWithRights in productFamiliesWithRights)
                {
                    if (ticket.ProductFamilyID == productFamilyWithRights.ProductFamilyID)
                    {
                        result = true;
                        break;
                    }
                }
            }
            if (!result)
            {
                Organization account = Organizations.GetOrganization(user.Collection.LoginUser, ticket.OrganizationID);
                if (!account.UseProductFamilies)
                {
                    result = true;
                }
            }

            return result;

            //      This code works but is taking too long in production. Till we figure out a way to fix that it needs to be replaced.
            //      using (SqlConnection connection = new SqlConnection(user.Collection.LoginUser.ConnectionString))
            //      {
            //        connection.Open();
            //        SqlCommand command = connection.CreateCommand();
            //        command.CommandText = @"
            //            SELECT
            //                COUNT(*)
            //            FROM
            //                Users u
            //                JOIN Organizations o
            //                    ON u.OrganizationID = o.OrganizationID
            //                LEFT JOIN UserRightsProductFamilies urpf
            //                    ON u.UserID = urpf.UserID
            //                LEFT JOIN Products p
            //                    ON urpf.ProductFamilyID = p.ProductFamilyID
            //                LEFT JOIN Tickets t
            //                    ON p.ProductID = t.ProductID
            //            WHERE
            //                u.UserID = @UserID
            //                AND
            //                (
            //                    o.UseProductFamilies = 0
            //                    OR t.TicketID = @TicketID
            //                )
            //        ";
            //        command.Parameters.AddWithValue("UserID", user.UserID);
            //        command.Parameters.AddWithValue("TicketID", ticketID);
            //        int result = (int)command.ExecuteScalar();
            //        connection.Close();
            //        return result > 0;
            //      }
        }

        public void FullReadFromXml(string data, bool isInsert, ref string description, ref int? contactID, ref int? customerID)
        {
            LoginUser user = Collection.LoginUser;
            FieldMap fieldMap = Collection.FieldMap;

            StringReader reader = new StringReader(data);
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(reader);

            try
            {
                object name = (string)DataUtils.GetValueFromObject(user, fieldMap, dataSet, "Name", string.Empty, null, false, null);
                if (name != null) this.Name = Convert.ToString(name);
            }
            catch
            {
            }

            try
            {
                object descriptionObject = (string)DataUtils.GetValueFromObject(user, fieldMap, dataSet, "Description", string.Empty, null, false, null, true);
                if (descriptionObject != null) description = Convert.ToString(descriptionObject);
            }
            catch
            {
            }

            try
            {
                object isKnowledgeBase = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "IsKnowledgeBase", string.Empty, null, false, null);
                if (isKnowledgeBase != null) this.IsKnowledgeBase = Convert.ToBoolean(isKnowledgeBase);
            }
            catch
            {
            }

            try
            {
                object isVisibleOnPortal = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "IsVisibleOnPortal", string.Empty, null, false, null);
                if (isVisibleOnPortal != null) this.IsVisibleOnPortal = Convert.ToBoolean(isVisibleOnPortal);
            }
            catch
            {
            }

            try
            {
                object ticketTypeID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "TicketTypeID", "TicketTypeName", TicketType.GetIDByName, false, null);
                if (ticketTypeID != null) this.TicketTypeID = Convert.ToInt32(ticketTypeID);
            }
            catch
            {
            }

            try
            {
                object ticketStatusID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "TicketStatusID", "Status", TicketStatus.GetIDByName, true, this.TicketTypeID);
                if (ticketStatusID != null) this.TicketStatusID = Convert.ToInt32(ticketStatusID);
            }
            catch
            {
            }

            try
            {
                object ticketSeverityID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "TicketSeverityID", "Severity", TicketSeverity.GetIDByName, false, null);
                if (ticketSeverityID != null) this.TicketSeverityID = Convert.ToInt32(ticketSeverityID);
            }
            catch
            {
            }

            try
            {
                object userID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "UserID", "UserEmail", User.GetIDByEmail, false, user.OrganizationID);
                if (userID != null) this.UserID = Convert.ToInt32(userID);
            }
            catch
            {
            }

            try
            {
                object groupID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "GroupID", "GroupName", Group.GetIDByName, false, null);
                if (groupID != null) this.GroupID = Convert.ToInt32(groupID);
            }
            catch
            {
            }

            try
            {
                object productID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "ProductID", "ProductName", Product.GetIDByName, false, null);
                if (productID != null) this.ProductID = Convert.ToInt32(productID);
            }
            catch
            {
            }

            try
            {
                object reportedVersionID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "ReportedVersionID", "ReportedVersion", ProductVersion.GetIDByName, true, this.ProductID);
                if (reportedVersionID != null) this.ReportedVersionID = Convert.ToInt32(reportedVersionID);
            }
            catch
            {
            }

            try
            {
                object solvedVersionID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "SolvedVersionID", "SolvedVersion", ProductVersion.GetIDByName, true, this.ProductID);
                if (solvedVersionID != null) this.SolvedVersionID = Convert.ToInt32(solvedVersionID);
            }
            catch
            {
            }

            try
            {
                object knowledgeBaseCategoryID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "KnowledgeBaseCategoryID", "KnowledgeBaseCategoryName", KnowledgeBaseCategory.GetIDByName, true, this.ProductID);
                if (knowledgeBaseCategoryID != null) this.KnowledgeBaseCategoryID = Convert.ToInt32(knowledgeBaseCategoryID);
            }
            catch
            {
            }

            try
            {
                object parentID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "ParentID", string.Empty, null, false, null);
                if (parentID != null) this.ParentID = Convert.ToInt32(parentID);
            }
            catch
            {
            }

            try
            {
                object dateCreated = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "DateCreated", string.Empty, null, false, null);
                if (dateCreated != null) this.DateCreated = Convert.ToDateTime(dateCreated);
            }
            catch
            {
            }

            try
            {
                object dateClosed = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "DateClosed", string.Empty, null, false, null);
                if (dateClosed != null) this.DateClosed = Convert.ToDateTime(dateClosed);
            }
            catch
            {
            }

            try
            {
                object closerID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "CloserID", "CloserEmail", User.GetIDByEmail, false, user.OrganizationID);
                if (closerID != null) this.CloserID = Convert.ToInt32(closerID);
            }
            catch
            {
            }

            try
            {
                object importID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "ImportID", string.Empty, null, false, null);
                if (importID != null) this.ImportID = Convert.ToString(importID);
            }
            catch
            {
            }

            try
            {
                object creatorID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "CreatorID", "CreatorEmail", User.GetIDByEmail, false, user.OrganizationID);
                if (creatorID != null) this.CreatorID = Convert.ToInt32(creatorID);
            }
            catch
            {
            }

            try
            {
                object contactIDObject = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "ContactID", "ContactEmail", User.GetIDByEmail, false, null, true);
                if (contactIDObject != null)
                {
                    contactID = Convert.ToInt32(contactIDObject);
                }
                else
                {
                    DataRow row = dataSet.Tables[0].Rows[0];
                    DataColumn column = dataSet.Tables[0].Columns["ContactEmail"];
                    if (row[column] != DBNull.Value && row[column].ToString().Trim() != string.Empty)
                    {
                        Users newUserCollection = new Users(user);
                        User newUser = newUserCollection.AddNewUser();
                        newUser.Email = row[column].ToString().Trim();
                        newUser.LastName = newUser.Email;
                        newUser.IsActive = true;
                        newUser.ActivatedOn = DateTime.UtcNow;
                        newUser.LastLogin = DateTime.UtcNow;
                        newUser.LastActivity = DateTime.UtcNow.AddHours(-1);
                        newUser.IsPasswordExpired = true;

                        if (newUser.Email.Contains("@"))
                        {
                            Organizations matchDomainCompany = new Organizations(user);
                            matchDomainCompany.LoadFirstDomainMatch(user.OrganizationID, newUser.Email.Substring(newUser.Email.LastIndexOf("@") + 1));
                            if (!matchDomainCompany.IsEmpty)
                            {
                                newUser.OrganizationID = matchDomainCompany[0].OrganizationID;
                            }
                        }

                        if (newUser.OrganizationID == 0)
                        {
                            newUser.OrganizationID = Organizations.GetUnknownCompanyID(user);
                        }

                        newUserCollection.Save();
                        contactID = newUser.UserID;
                    }
                }
            }
            catch
            {
            }

            try
            {
                object customerIDObject = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "CustomerID", "CustomerName", Organization.GetIDByName, false, user.OrganizationID, true);
                if (customerIDObject != null)
                {
                    customerID = Convert.ToInt32(customerIDObject);
                }
            }
            catch
            {
            }
        }

        public void UpdateSalesForceData()
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
          UPDATE
            Tickets
          SET
            SalesForceID = @SalesForceID,
            DateModifiedBySalesForceSync = @DateModifiedBySalesForceSync
          WHERE
            TicketID = @TicketID
        ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@SalesForceID", this.SalesForceID);
                command.Parameters.AddWithValue("@DateModifiedBySalesForceSync", this.DateModifiedBySalesForceSyncUtc);
                command.Parameters.AddWithValue("@TicketID", this.TicketID);
                this.BaseCollection.ExecuteNonQuery(command, "Tickets");
            }
        }

        public Ticket Clone()
        {
            int cloneTicketId = 0;
            LoginUser loginUser = Collection.LoginUser;
            Tickets tickets = new Tickets(loginUser);
            Ticket clone = tickets.AddNewTicket();
            ClonePropertiesTo(clone);

            clone.Collection.Save();
            cloneTicketId = clone.TicketID;

            DeleteEmailPostsByTicketId(cloneTicketId);

            string actionLog = string.Format("{0} cloned ticket {1} into {2}.", loginUser.GetUserFullName(), this.TicketNumber, clone.TicketNumber);
            ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Tickets, cloneTicketId, actionLog);

            Actions clonedActions = new Actions(loginUser);
            Actions originalActions = new Actions(loginUser);

            try
            {
                //Clone Ticket's actions
                originalActions.LoadByTicketID(this.TicketID);

                foreach (Action action in originalActions.OrderBy(o => o.DateCreated).ToList())
                {
                    Action clonedAction = clonedActions.AddNewAction();
                    action.ClonePropertiesTo(clonedAction);
                    clonedAction.TicketID = cloneTicketId;
                }

                clonedActions.BulkSave();

                DeleteEmailPostsByTicketId(cloneTicketId);
            }
            catch (Exception ex)
            {
                actionLog = string.Format("Failed to clone ticket {0} Actions into {1}.", this.TicketNumber, clone.TicketNumber);
                ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Tickets, cloneTicketId, actionLog);
                ExceptionLogs.LogException(loginUser, ex, "Cloning Ticket", "Tickets.Clone - Actions");
            }

            try
            {
                TicketRelationships originalTicketRelationships = new TicketRelationships(loginUser);
                originalTicketRelationships.LoadByTicketID(this.TicketID);
                TicketRelationships clonedTicketRelationships = new TicketRelationships(loginUser);

                foreach (TicketRelationship relationship in originalTicketRelationships)
                {
                    TicketRelationship clonedRelationship = clonedTicketRelationships.AddNewTicketRelationship();
                    clonedRelationship.OrganizationID = relationship.OrganizationID;
                    clonedRelationship.Ticket1ID = relationship.Ticket1ID == this.TicketID ? cloneTicketId : relationship.Ticket1ID;
                    clonedRelationship.Ticket2ID = relationship.Ticket2ID == this.TicketID ? cloneTicketId : relationship.Ticket2ID;
                    clonedRelationship.CreatorID = relationship.CreatorID;
                    clonedRelationship.DateCreated = relationship.DateCreatedUtc;
                }

                clonedTicketRelationships.BulkSave();
            }
            catch (Exception ex)
            {
                actionLog = string.Format("Failed to clone ticket {0} Relationships into {1}.", this.TicketNumber, clone.TicketNumber);
                ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Tickets, cloneTicketId, actionLog);
                ExceptionLogs.LogException(loginUser, ex, "Cloning Ticket", "Tickets.Clone - Relationships");
            }

            try
            {
                ContactsView originalTicketContacts = new ContactsView(loginUser);
                originalTicketContacts.LoadByTicketID(this.TicketID);

                foreach (ContactsViewItem contact in originalTicketContacts)
                {
                    clone.Collection.AddContact(contact.UserID, cloneTicketId);
                }

                DeleteEmailPostsByTicketId(cloneTicketId);

                Organizations originalTicketOrganizations = new Organizations(loginUser);
                originalTicketOrganizations.LoadByNotContactTicketID(this.TicketID);

                foreach (Organization organization in originalTicketOrganizations)
                {
                    if (!originalTicketContacts.Where(p => p.OrganizationID == organization.OrganizationID).Any())
                    {
                        clone.Collection.AddOrganization(organization.OrganizationID, cloneTicketId);
                    }
                }

                DeleteEmailPostsByTicketId(cloneTicketId);
            }
            catch (Exception ex)
            {
                actionLog = string.Format("Failed to clone ticket {0} Contacts/Companies into {1}.", this.TicketNumber, clone.TicketNumber);
                ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Tickets, cloneTicketId, actionLog);
                ExceptionLogs.LogException(loginUser, ex, "Cloning Ticket", "Tickets.Clone - Contacts/Companies");
            }

            try
            {
                ForumTickets originalTicketForums = new ForumTickets(loginUser);
                originalTicketForums.LoadByTicketID(this.TicketID);

                foreach (int forumCategoryId in originalTicketForums.Select(p => p.ForumCategory))
                {
                    clone.AddCommunityTicket((int)forumCategoryId);
                }
            }
            catch (Exception ex)
            {
                actionLog = string.Format("Failed to clone ticket {0} Community into {1}.", this.TicketNumber, clone.TicketNumber);
                ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Tickets, cloneTicketId, actionLog);
                ExceptionLogs.LogException(loginUser, ex, "Cloning Ticket", "Tickets.Clone - Community");
            }

            try
            {
                TagLinks originalTicketTags = new TagLinks(loginUser);
                originalTicketTags.LoadByReference(ReferenceType.Tickets, this.TicketID);
                TagLinks clonedTicketTags = new TagLinks(loginUser);

                foreach (TagLink tag in originalTicketTags)
                {
                    TagLink clonedTicketTag = clonedTicketTags.AddNewTagLink();
                    clonedTicketTag.TagID = tag.TagID;
                    clonedTicketTag.RefType = ReferenceType.Tickets;
                    clonedTicketTag.RefID = cloneTicketId;
                    clonedTicketTag.CreatorID = tag.CreatorID;
                    clonedTicketTag.DateCreated = tag.DateCreatedUtc;
                }

                clonedTicketTags.BulkSave();

                DeleteEmailPostsByTicketId(cloneTicketId);
            }
            catch (Exception ex)
            {
                actionLog = string.Format("Failed to clone ticket {0} Tags into {1}.", this.TicketNumber, clone.TicketNumber);
                ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Tickets, cloneTicketId, actionLog);
                ExceptionLogs.LogException(loginUser, ex, "Cloning Ticket", "Tickets.Clone - Tags");
            }

            try
            {
                CustomValues originalTicketCustomValues = new CustomValues(loginUser);
                originalTicketCustomValues.LoadExistingOnlyByReferenceType(this.OrganizationID, ReferenceType.Tickets, this.TicketID);
                CustomValues clonedCustomValues = new CustomValues(loginUser);

                foreach (CustomValue customValue in originalTicketCustomValues)
                {
                    CustomValue clonedTicketCustomValue = clonedCustomValues.AddNewCustomValue();
                    clonedTicketCustomValue.CustomFieldID = customValue.CustomFieldID;
                    clonedTicketCustomValue.RefID = cloneTicketId;
                    clonedTicketCustomValue.Value = customValue.Value.ToString();
                    clonedTicketCustomValue.CreatorID = customValue.CreatorID;
                    clonedTicketCustomValue.ModifierID = customValue.ModifierID;
                    clonedTicketCustomValue.DateCreated = customValue.DateCreatedUtc;
                    clonedTicketCustomValue.DateModified = customValue.DateModifiedUtc;
                    clonedTicketCustomValue.OrganizationID = loginUser.OrganizationID;
                }

                clonedCustomValues.BulkSave();

                DeleteEmailPostsByTicketId(cloneTicketId);
            }
            catch (Exception ex)
            {
                actionLog = string.Format("Failed to clone ticket {0} Custom Values into {1}.", this.TicketNumber, clone.TicketNumber);
                ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Tickets, cloneTicketId, actionLog);
                ExceptionLogs.LogException(loginUser, ex, "Cloning Ticket", "Tickets.Clone - Custom Values");
            }

            try
            {
                UsersView originalTicketSubscribers = new UsersView(loginUser);
                originalTicketSubscribers.LoadBySubscription(this.TicketID, ReferenceType.Tickets);

                foreach (UsersViewItem subscriber in originalTicketSubscribers)
                {
                    Subscriptions.AddSubscription(loginUser, subscriber.UserID, ReferenceType.Tickets, cloneTicketId);
                }

                DeleteEmailPostsByTicketId(cloneTicketId);
            }
            catch (Exception ex)
            {
                actionLog = string.Format("Failed to clone ticket {0} Subscribers into {1}.", this.TicketNumber, clone.TicketNumber);
                ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Tickets, cloneTicketId, actionLog);
                ExceptionLogs.LogException(loginUser, ex, "Cloning Ticket", "Tickets.Clone - Subscribers");
            }

            try
            {
                UsersView originalTicketQueuers = new UsersView(loginUser);
                originalTicketQueuers.LoadByTicketQueue(this.TicketID);

                foreach (UsersViewItem queuer in originalTicketQueuers)
                {
                    TicketQueue.Enqueue(loginUser, cloneTicketId, queuer.UserID);
                }

                DeleteEmailPostsByTicketId(cloneTicketId);
            }
            catch (Exception ex)
            {
                actionLog = string.Format("Failed to clone ticket {0} Queuers into {1}.", this.TicketNumber, clone.TicketNumber);
                ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Tickets, cloneTicketId, actionLog);
                ExceptionLogs.LogException(loginUser, ex, "Cloning Ticket", "Tickets.Clone - Queuers");
            }

            try
            {
                Reminders clonedTicketReminders = new Reminders(loginUser);
                clonedTicketReminders.LoadByItemAll(ReferenceType.Tickets, this.TicketID, null);
                Reminders reminders = new Reminders(loginUser);

                foreach (Reminder reminder in clonedTicketReminders)
                {
                    Reminder clonedTicketReminder = reminders.AddNewReminder();
                    clonedTicketReminder.OrganizationID = this.OrganizationID;
                    clonedTicketReminder.RefID = cloneTicketId;
                    clonedTicketReminder.RefType = ReferenceType.Tickets;
                    clonedTicketReminder.Description = reminder.Description;
                    clonedTicketReminder.DueDate = reminder.DueDateUtc;
                    clonedTicketReminder.UserID = reminder.UserID;
                    clonedTicketReminder.IsDismissed = reminder.IsDismissed;
                    clonedTicketReminder.HasEmailSent = reminder.HasEmailSent;
                    clonedTicketReminder.CreatorID = reminder.CreatorID;
                    clonedTicketReminder.DateCreated = reminder.DateCreatedUtc;
                }

                reminders.BulkSave();

                DeleteEmailPostsByTicketId(cloneTicketId);
            }
            catch (Exception ex)
            {
                actionLog = string.Format("Failed to clone ticket {0} Reminders into {1}.", this.TicketNumber, clone.TicketNumber);
                ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Tickets, cloneTicketId, actionLog);
                ExceptionLogs.LogException(loginUser, ex, "Cloning Ticket", "Tickets.Clone - Reminders");
            }

            try
            {
                Assets originalTicketAssets = new Assets(loginUser);
                originalTicketAssets.LoadByTicketID(this.TicketID);

                foreach (Asset asset in originalTicketAssets)
                {
                    clone.Collection.AddAsset(asset.AssetID, cloneTicketId);
                }

                DeleteEmailPostsByTicketId(cloneTicketId);
            }
            catch (Exception ex)
            {
                actionLog = string.Format("Failed to clone ticket {0} Assets into {1}.", this.TicketNumber, clone.TicketNumber);
                ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Tickets, cloneTicketId, actionLog);
                ExceptionLogs.LogException(loginUser, ex, "Cloning Ticket", "Tickets.Clone - Assets");
            }

            //try
            //{
            //    TicketLinkToJira linkToJira = new TicketLinkToJira(loginUser);
            //    linkToJira.LoadByTicketID(this.TicketID);

            //    if (linkToJira.Count > 0)
            //    {
            //        TicketLinkToJiraItemProxy originalTicketJiraLink = linkToJira[0].GetProxy();
            //        TicketLinkToJira clonedTicketJiraLink = new TicketLinkToJira(loginUser);
            //        TicketLinkToJiraItem clonedJiraLink = clonedTicketJiraLink.AddNewTicketLinkToJiraItem();
            //        clonedJiraLink.TicketID = cloneTicketId;
            //        clonedJiraLink.SyncWithJira = originalTicketJiraLink.SyncWithJira;
            //        clonedJiraLink.JiraID = originalTicketJiraLink.JiraID;
            //        clonedJiraLink.JiraKey = originalTicketJiraLink.JiraKey;
            //        clonedJiraLink.JiraLinkURL = originalTicketJiraLink.JiraLinkURL;
            //        clonedJiraLink.JiraStatus = originalTicketJiraLink.JiraStatus;
            //        clonedJiraLink.CreatorID = originalTicketJiraLink.CreatorID;
            //        clonedJiraLink.CrmLinkID = originalTicketJiraLink.CrmLinkID;

            //        DateTime dt;

            //        if (DateTime.TryParse((originalTicketJiraLink.DateModifiedByJiraSync.ToString()).Replace("UTC", "GMT"), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
            //        {
            //            clonedJiraLink.DateModifiedByJiraSync = dt.ToUniversalTime();
            //        }

            //        clonedJiraLink.Collection.Save();

            //        DeleteEmailPostsByTicketId(cloneTicketId);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    actionLog = string.Format("Failed to clone ticket {0} Jira Link into {1}.", this.TicketNumber, clone.TicketNumber);
            //    ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Tickets, cloneTicketId, actionLog);
            //    ExceptionLogs.LogException(loginUser, ex, "Cloning Ticket", "Tickets.Clone - JiraLink");
            //}

            try
            {
                //attachments!!!
                if (clonedActions.Any() && originalActions.Any())
                {
                    Attachments originalAttachments = new Attachments(loginUser);
                    originalAttachments.LoadByTicketId(this.TicketID);
                    Attachments clonedAttachments = new Attachments(loginUser);
                    clonedActions.LoadByTicketID(cloneTicketId);

                    foreach (Attachment attachment in originalAttachments)
                    {
                        Attachment clonedAttachment = clonedAttachments.AddNewAttachment();
                        clonedAttachment.OrganizationID = attachment.OrganizationID;
                        clonedAttachment.FileType = attachment.FileType;
                        clonedAttachment.FileSize = attachment.FileSize;
                        clonedAttachment.Description = attachment.Description;
                        clonedAttachment.DateCreated = attachment.DateCreatedUtc;
                        clonedAttachment.DateModified = attachment.DateModifiedUtc;
                        clonedAttachment.CreatorID = attachment.CreatorID;
                        clonedAttachment.ModifierID = attachment.ModifierID;
                        clonedAttachment.RefType = attachment.RefType;
                        clonedAttachment.SentToJira = attachment.SentToJira;
                        clonedAttachment.ProductFamilyID = attachment.ProductFamilyID;
                        clonedAttachment.FileName = attachment.FileName;
                        clonedAttachment.FilePathID = attachment.FilePathID;

                        //these need to be for the new ticket
                        Action originalAction = originalActions.Where(a => a.ActionID == attachment.RefID).FirstOrDefault();
                        Action matchingClone = clonedActions.Where(c => c.ActionTypeID == originalAction.ActionTypeID
                                                                                            && c.SystemActionTypeID == originalAction.SystemActionTypeID
                                                                                            && c.IsVisibleOnPortal == originalAction.IsVisibleOnPortal
                                                                                            && c.IsKnowledgeBase == originalAction.IsKnowledgeBase
                                                                                            && c.Pinned == originalAction.Pinned
                                                                                            && c.IsClean == originalAction.IsClean
                                                                                            && c.TimeSpent == originalAction.TimeSpent
                                                                                            && c.ActionSource + "" == originalAction.ActionSource + ""
                                                                                            && c.Description == originalAction.Description
                                                                                            && c.Name == originalAction.Name).FirstOrDefault();

                        clonedAttachment.RefID = matchingClone.ActionID;

                        string clonedActionAttachmentPath = attachment.Path.Substring(0, attachment.Path.IndexOf(@"\Actions\") + @"\Actions\".Length)
                                                            + matchingClone.ActionID
                                                            + attachment.Path.Substring(attachment.Path.IndexOf(originalAction.ActionID.ToString()) + originalAction.ActionID.ToString().Length);

                        if (!Directory.Exists(Path.GetDirectoryName(clonedActionAttachmentPath)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(clonedActionAttachmentPath));
                        }

                        clonedAttachment.Path = clonedActionAttachmentPath;

                        File.Copy(attachment.Path, clonedAttachment.Path);
                    }

                    clonedAttachments.BulkSave();

                    DeleteEmailPostsByTicketId(cloneTicketId);
                }
            }
            catch (Exception ex)
            {
                actionLog = string.Format("Failed to clone ticket {0} Attachments into {1}.", this.TicketNumber, clone.TicketNumber);
                ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Tickets, cloneTicketId, actionLog);
                ExceptionLogs.LogException(loginUser, ex, "Cloning Ticket", "Tickets.Clone - Attachments");
            }

            //The SLA datetime values were modified by the added Actions, so they need to be reset to the original Ticket values
            clone.SlaViolationInitialResponse = this.SlaViolationInitialResponseUtc;
            clone.SlaViolationLastAction = this.SlaViolationLastActionUtc;
            clone.SlaViolationTimeClosed = this.SlaViolationTimeClosedUtc;
            clone.SlaWarningInitialResponse = this.SlaWarningInitialResponseUtc;
            clone.SlaWarningLastAction = this.SlaWarningLastActionUtc;
            clone.SlaWarningTimeClosed = this.SlaWarningTimeClosedUtc;
            clone.Collection.Save();

            //Delete the EmailPost of the SLA values updated
            DeleteEmailPostsByTicketId(cloneTicketId);

            try
            {
                Tasks cloningTicketTasks = new Tasks(loginUser);
                cloningTicketTasks.LoadByItemAll(ReferenceType.Tickets, this.TicketID);

                string logDescription = string.Format("{0} cloned task for cloned ticket {1} into {2}.", loginUser.GetUserFullName(), this.TicketNumber, clone.TicketNumber);
                CloneTicketTasks(loginUser, cloningTicketTasks, cloneTicketId, ref logDescription, null);
            }
            catch (Exception ex)
            {
                actionLog = string.Format("Failed to clone ticket {0} Tasks into {1}.", this.TicketNumber, clone.TicketNumber);
                ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Tickets, cloneTicketId, actionLog);
                ExceptionLogs.LogException(loginUser, ex, "Cloning Ticket", "Tickets.Clone - Tasks");
            }

            ActionLogs cloneActionLogs = new ActionLogs(loginUser);
            cloneActionLogs.LoadByTicketID(cloneTicketId);

            foreach (ActionLog log in cloneActionLogs)
            {
                if (!log.Description.StartsWith("Failed to clone")
                    && !log.Description.Contains(string.Format("cloned ticket {0} into {1}", this.TicketNumber, clone.TicketNumber)))
                {
                    log.Delete();
                }
            }

            cloneActionLogs.Save();

            return clone;
        }

        /// <summary>
        /// This will clone all Ticket object writable properties using reflection. This way we make sure that if there are more fields added (or deleted) in this table this will still work.
        /// This was the easier way to do this due to the amount of properties to process and the high possibility of this table scheme being updated often.
        /// I wanted to do this with serialization (JsonConvert.Deserialize/Serialize object), but didn't work due to how our Data objects are generated (Collections in them), and the primary key.
        /// </summary>
        /// <param name="clone">The empty initialized Ticket object to clone to.</param>
        private void ClonePropertiesTo(Ticket clone)
        {
            foreach (System.Reflection.PropertyInfo sourcePropertyInfo in this.GetType().GetProperties())
            {
                if (sourcePropertyInfo.CanWrite
                    && sourcePropertyInfo.Name.ToLower() != "basecollection"
                    && sourcePropertyInfo.Name.ToLower() != "ticketnumber"
                    && sourcePropertyInfo.Name.ToLower() != "salesforceid"
                    && sourcePropertyInfo.PropertyType != typeof(DateTime)
                    && sourcePropertyInfo.PropertyType != typeof(DateTime?))
                {
                    System.Reflection.PropertyInfo destPropertyInfo = clone.GetType().GetProperty(sourcePropertyInfo.Name);

                    destPropertyInfo.SetValue(
                        clone,
                        sourcePropertyInfo.GetValue(this, null),
                        null);
                }
            }

            //DateTime properties are special it needs to be the UTC value. The DateTime (and DateTime?) properties always have a UTC version
            foreach (System.Reflection.PropertyInfo sourcePropertyInfo in this.GetType().GetProperties())
            {
                if (sourcePropertyInfo.CanRead
                    && sourcePropertyInfo.Name.Substring(sourcePropertyInfo.Name.Length - 3).ToLower() == "utc"
                    && (sourcePropertyInfo.PropertyType == typeof(DateTime)
                        || sourcePropertyInfo.PropertyType == typeof(DateTime?)))
                {
                    System.Reflection.PropertyInfo destPropertyInfo = clone.GetType().GetProperty(sourcePropertyInfo.Name.Substring(0, sourcePropertyInfo.Name.Length - 3));

                    destPropertyInfo.SetValue(
                        clone,
                        sourcePropertyInfo.GetValue(this, null),
                        null);
                }
            }


            clone.Name = string.Format("{0} (Clone)", this.Name);
            clone.DateCreated = DateTime.UtcNow;
            clone.DateModified = DateTime.UtcNow;
            clone.NeedsIndexing = true;
        }

        private void CloneTicketTasks(LoginUser loginUser, Tasks cloningTicketTasks, int cloneTicketId, ref string logDescription, int? newParentID)
        {
            foreach (Task cloningTask in cloningTicketTasks)
            {
                //if subtask and parent in cloningtasks skip as it will be processed as subtask
                if (cloningTask.ParentID != null)
                {
                    Task parentInCloningTicketTasks = cloningTicketTasks.FindByTaskID((int)cloningTask.ParentID);
                    if (parentInCloningTicketTasks != null)
                    {
                        continue;
                    }
                }
                Tasks newTasks = new Tasks(loginUser);
                Task newTask = newTasks.AddNewTask();
                if (newParentID == null)
                {
                    newTask.ParentID = cloningTask.ParentID;
                }
                else
                {
                    newTask.ParentID = newParentID;
                }
                newTask.OrganizationID = this.OrganizationID;
                newTask.Name = cloningTask.Name;
                newTask.Description = cloningTask.Description;
                newTask.DueDate = cloningTask.DueDateUtc;
                newTask.UserID = cloningTask.UserID;
                newTask.IsComplete = cloningTask.IsComplete;
                newTask.DateCompleted = cloningTask.DateCompletedUtc;
                newTask.CreatorID = cloningTask.CreatorID;
                newTask.DateCreated = cloningTask.DateCreatedUtc;
                newTask.ModifierID = cloningTask.ModifierID;
                newTask.DateModified = cloningTask.DateModifiedUtc;
                newTask.NeedsIndexing = true;
                newTasks.Save();

                TaskLogs taskLogs = new TaskLogs(loginUser);
                TaskLogs.AddTaskLog(loginUser, newTask.TaskID, logDescription);

                if (cloningTask.ReminderID != null)
                {
                    Reminders cloningReminders = new Data.Reminders(loginUser);
                    cloningReminders.LoadByReminderID((int)cloningTask.ReminderID);

                    foreach (Reminder cloningReminder in cloningReminders)
                    {
                        Reminders newReminders = new Data.Reminders(loginUser);
                        Reminder newReminder = newReminders.AddNewReminder();
                        newReminder.RefID = newTask.TaskID;
                        newReminder.OrganizationID = this.OrganizationID;
                        newReminder.RefType = cloningReminder.RefType;
                        newReminder.Description = cloningReminder.Description;
                        newReminder.DueDate = cloningReminder.DueDateUtc;
                        newReminder.UserID = cloningReminder.UserID;
                        newReminder.IsDismissed = cloningReminder.IsDismissed;
                        newReminder.HasEmailSent = cloningReminder.HasEmailSent;
                        newReminder.CreatorID = cloningReminder.CreatorID;
                        newReminder.DateCreated = cloningReminder.DateCreatedUtc;
                        newReminders.Save();

                        newTask.ReminderID = newReminder.ReminderID;
                        newTasks.Save();
                    }
                }

                TaskAssociations cloningTaskAssociations = new Data.TaskAssociations(loginUser);
                cloningTaskAssociations.LoadByTaskIDOnly(cloningTask.TaskID);

                TaskAssociations newTaskAssociations = new TaskAssociations(loginUser);

                foreach (TaskAssociation cloningTaskAssociation in cloningTaskAssociations)
                {
                    TaskAssociation newTaskAssociation = newTaskAssociations.AddNewTaskAssociation();
                    newTaskAssociation.TaskID = newTask.TaskID;
                    newTaskAssociation.RefType = cloningTaskAssociation.RefType;
                    newTaskAssociation.CreatorID = cloningTaskAssociation.CreatorID;
                    newTaskAssociation.DateCreated = cloningTaskAssociation.DateCreatedUtc;
                    if (cloningTaskAssociation.RefType == (int)ReferenceType.Tickets && cloningTaskAssociation.RefID == this.TicketID)
                    {
                        newTaskAssociation.RefID = cloneTicketId;
                    }
                    else
                    {
                        newTaskAssociation.RefID = cloningTaskAssociation.RefID;
                    }
                }

                newTaskAssociations.BulkSave();

                Tasks cloningSubTasks = new Tasks(loginUser);
                cloningSubTasks.LoadByParentID(cloningTask.TaskID);

                CloneTicketTasks(loginUser, cloningSubTasks, cloneTicketId, ref logDescription, newTask.TaskID);
            }
        }

        private void DeleteEmailPostsByTicketId(int ticketId)
        {
            EmailPosts emailPost = new EmailPosts(Collection.LoginUser);
            emailPost.LoadByTicketId(ticketId);
            emailPost.DeleteAll();
            emailPost.Save();
        }

		[Obsolete("This is not needed anymore in neither APP or the services, please don't use. Moved to SlaProcessor.cs which is the only place that should use it.")]
		public bool IsSlaPaused(int triggerId, int organizationId)
        {
            bool isPaused = false;
            TicketStatuses statuses = new TicketStatuses(Collection.LoginUser);
            statuses.LoadByStatusIDs(OrganizationID, new int[] { TicketStatusID });

            if (statuses != null && statuses.Any())
            {
                isPaused = statuses[0].PauseSLA;
            }

            //check if "Pause on Specific Dates"
            if (!isPaused)
            {
                List<DateTime> daysToPause = SlaTriggers.GetSpecificDaysToPause(triggerId);
                isPaused = daysToPause.Where(p => DateTime.Compare(p.Date, DateTime.UtcNow.Date) == 0).Any();
            }

            //If Pause on Company Holidays is selected
            SlaTrigger slaTrigger = SlaTriggers.GetSlaTrigger(Collection.LoginUser, triggerId);

            if (!isPaused && slaTrigger.PauseOnHoliday)
            {
                isPaused = SlaTriggers.IsOrganizationHoliday(organizationId, DateTime.UtcNow);
            }

            return isPaused;
        }

        public bool IsSlaStatusPaused()
        {
            bool isPaused = false;
            TicketStatuses statuses = new TicketStatuses(Collection.LoginUser);
            statuses.LoadByStatusIDs(OrganizationID, new int[] { TicketStatusID });

            if (statuses != null && statuses.Any())
            {
                isPaused = statuses[0].PauseSLA;
            }

            return isPaused;
        }
    }

    public partial class Tickets
    {
        private string _actionLogInstantMessage = null;

        public string ActionLogInstantMessage
        {
            get
            {
                return _actionLogInstantMessage;
            }
            set
            {
                _actionLogInstantMessage = value;
            }
        }

        partial void BeforeRowInsert(Ticket ticket)
        {
            if (ticket.TicketNumber < 2)
            {
                ticket.TicketNumber = GetMaxTicketNumber(ticket.OrganizationID) + 1;
                if (ticket.TicketNumber < 2) ticket.TicketNumber = 1000;
                //if (ticket.TicketNumber < 1) ticket.TicketNumber = 1;

                // CHECK IF TICKET CLOSED
                TicketStatus ticketStatus = TicketStatuses.GetTicketStatus(LoginUser, ticket.TicketStatusID);
                if (ticketStatus.IsClosed)
                {
                    ticket.CloserID = LoginUser.UserID;
                    ticket.DateClosed = DateTime.UtcNow;
                }
            }
            ticket.Name = (ticket.Row["Name"] == DBNull.Value) ? string.Empty : ticket.Name;
        }

        partial void AfterRowInsert(Ticket ticket)
        {
            string description = "Created Ticket Number ";
            if (_actionLogInstantMessage != null)
            {
                description = _actionLogInstantMessage;
            }
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tickets, ticket.TicketID, description + ticket.TicketNumber.ToString());
            _actionLogInstantMessage = null;

            // CHECK IF TICKET CLOSED
            TicketStatus ticketStatus = TicketStatuses.GetTicketStatus(LoginUser, ticket.TicketStatusID);
            if (ticketStatus.IsClosed)
            {
                ticket.CloserID = LoginUser.UserID;
                ticket.DateClosed = DateTime.UtcNow;

                description = "Closed " + GetTicketLink(ticket);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
            }

        }

        public static int GetMyOpenUnreadTicketCount(LoginUser loginUser, int userID)
        {
            int cnt = 0;
            using (SqlCommand command = new SqlCommand())
            {
                /*
        command.CommandText = @"
SELECT COUNT(*) FROM Tickets t
LEFT JOIN UserTicketStatusesView utsv ON t.TicketID=utsv.TicketID AND utsv.ViewerID = @ViewerID
LEFT JOIN TicketStatuses ts ON ts.TicketStatusID = t.TicketStatusID
WHERE t.UserID=@ViewerID
AND utsv.IsRead = 0
AND ts.IsClosed = 0";
        command.CommandType = CommandType.Text;
        */
                command.CommandText = "TicketCountForMyOpenUnreadGet";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ViewerID", userID);

                object o = SqlExecutor.ExecuteScalar(loginUser, command);
                if (o == DBNull.Value) return -1;
                cnt = (int)o;
            }
            return cnt;
        }

        private bool _updateChildTickets = true;
        public void UpdateChildTickets(Ticket ticket)
        {
            Tickets tickets = new Tickets(LoginUser);
            tickets.LoadChildren(ticket.TicketID);
            tickets._updateChildTickets = false;

            foreach (Ticket item in tickets)
            {
                if (item.TicketTypeID == ticket.TicketTypeID)
                    item.TicketStatusID = ticket.TicketStatusID;
            }
            tickets.Save();

        }

        partial void AfterRowEdit(Ticket ticket)
        {
            if (_updateChildTickets) UpdateChildTickets(ticket);
        }

        partial void BeforeRowDelete(int ticketID)
        {
            Tickets.DeleteRelationships(LoginUser, ticketID);

            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, ticketID);

            string description = "Deleted Ticket Number " + ticket.TicketNumber.ToString();
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Tickets, ticket.TicketID, description);

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE Tickets SET ModifierID = @ModifierID WHERE (TicketID = @TicketID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                command.Parameters.AddWithValue("@ModifierID", LoginUser.UserID);
                ticket.Collection.ExecuteNonQuery(command, "TicketRelationships");
            }
        }

        public static string GetTicketLink(Ticket ticket)
        {

            //      return "<a class=\"actionLogLink\" href=\"Ticket.aspx?ticketid=" + ticket.TicketID + "\">Ticket Number " + ticket.TicketNumber.ToString() + "</a> ";

            return "<a class=\"actionLogLink\" href=\"javascript:openTicketWindow(" + ticket.TicketID.ToString() + ");\">Ticket Number " + ticket.TicketNumber.ToString() + "</a> ";
        }

        public static string GetTicketLink(LoginUser loginUser, int ticketID)
        {
            Ticket ticket = (Ticket)Tickets.GetTicket(loginUser, ticketID);
            return GetTicketLink(ticket);
        }

        partial void BeforeRowEdit(Ticket ticket)
        {
            TicketGridViewItem oldTicketView = (TicketGridViewItem)TicketGridView.GetTicketGridViewItem(LoginUser, ticket.TicketID);
            string description = "";
            string name1;
            string name2;


            if (oldTicketView.GroupID != ticket.GroupID)
            {
                if (oldTicketView.GroupID == null) { name1 = "Unassigned"; } else { name1 = oldTicketView.GroupName; }
                if (ticket.GroupID == null) { name2 = "Unassigned"; } else { Group group = (Group)Groups.GetGroup(LoginUser, (int)ticket.GroupID); name2 = group.Name; }
                description = "Reassigned " + GetTicketLink(ticket) + " from group '" + name1 + "' to group '" + name2 + "'";
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
                if (oldTicketView.GroupID != null)
                    ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Groups, (int)oldTicketView.GroupID, description);
            }

            if (oldTicketView.IsKnowledgeBase != ticket.IsKnowledgeBase)
            {
                if (ticket.IsKnowledgeBase)
                    description = "Added " + GetTicketLink(ticket) + " to the knowledge base.";
                else
                    description = "Removed " + GetTicketLink(ticket) + " from the knowledge base.";
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
            }

            if (oldTicketView.IsVisibleOnPortal != ticket.IsVisibleOnPortal)
            {
                if (ticket.IsVisibleOnPortal)
                    description = "Visible to customers set to true.";
                else
                    description = "Visible to customers set to false.";
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
            }

            ticket.Name = (ticket.Row["Name"] == DBNull.Value) ? string.Empty : ticket.Name;
            if (oldTicketView.Name != ticket.Name)
            {
                description = "Changed ticket name from '" + oldTicketView.Name + "' to '" + ticket.Name + "' for " + GetTicketLink(ticket);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
            }


            if (oldTicketView.ProductID != ticket.ProductID)
            {
                if (oldTicketView.ProductID == null) { name1 = "Unassigned"; } else { name1 = oldTicketView.ProductName; }
                if (ticket.ProductID == null) { name2 = "Unassigned"; } else { Product product = (Product)Products.GetProduct(LoginUser, (int)ticket.ProductID); name2 = product.Name; }
                description = "Changed  product from '" + name1 + "' to '" + name2 + "' for " + GetTicketLink(ticket);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
                if (oldTicketView.ProductID != null)
                    ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Products, (int)oldTicketView.ProductID, description);
            }

            if (oldTicketView.ReportedVersionID != ticket.ReportedVersionID)
            {
                if (oldTicketView.ReportedVersionID == null) { name1 = "Unassigned"; } else { name1 = oldTicketView.ReportedVersion; }
                if (ticket.ReportedVersionID == null) { name2 = "Unassigned"; } else { ProductVersion productVersion = (ProductVersion)ProductVersions.GetProductVersion(LoginUser, (int)ticket.ReportedVersionID); name2 = productVersion.VersionNumber; }
                description = "Changed reported version from '" + name1 + "' to '" + name2 + "' for " + GetTicketLink(ticket);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
                if (oldTicketView.ReportedVersionID != null)
                    ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.ProductVersions, (int)oldTicketView.ReportedVersionID, description);
            }

            if (oldTicketView.SolvedVersionID != ticket.SolvedVersionID)
            {
                if (oldTicketView.SolvedVersionID == null) { name1 = "Unassigned"; } else { name1 = oldTicketView.SolvedVersion; }
                if (ticket.SolvedVersionID == null) { name2 = "Unassigned"; } else { ProductVersion productVersion = (ProductVersion)ProductVersions.GetProductVersion(LoginUser, (int)ticket.SolvedVersionID); name2 = productVersion.VersionNumber; }
                description = "Changed resolved version from '" + name1 + "' to '" + name2 + "' for " + GetTicketLink(ticket);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
                if (oldTicketView.SolvedVersionID != null)
                    ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.ProductVersions, (int)oldTicketView.SolvedVersionID, description);
            }

            if (oldTicketView.TicketSeverityID != ticket.TicketSeverityID)
            {
                name1 = oldTicketView.Severity;
                TicketSeverity ticketSeverity = (TicketSeverity)TicketSeverities.GetTicketSeverity(LoginUser, (int)ticket.TicketSeverityID);
                name2 = ticketSeverity.Name;
                description = "Changed severity from '" + name1 + "' to '" + name2 + "' for " + GetTicketLink(ticket);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
            }

            if (oldTicketView.TicketStatusID != ticket.TicketStatusID)
            {
                name1 = oldTicketView.Status;
                TicketStatus ticketStatus = (TicketStatus)TicketStatuses.GetTicketStatus(LoginUser, (int)ticket.TicketStatusID);
                name2 = ticketStatus.Name;
                description = "Changed status from '" + name1 + "' to '" + name2 + "' for " + GetTicketLink(ticket);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
            }

            if (oldTicketView.TicketTypeID != ticket.TicketTypeID)
            {
                name1 = oldTicketView.TicketTypeName;
                TicketType ticketType = (TicketType)TicketTypes.GetTicketType(LoginUser, (int)ticket.TicketTypeID);
                name2 = ticketType.Name;
                description = "Changed ticket type from '" + name1 + "' to '" + name2 + "' for " + GetTicketLink(ticket);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
            }

            if (oldTicketView.UserID != ticket.UserID)
            {
                if (oldTicketView.UserID == null) { name1 = "Unassigned"; } else { name1 = oldTicketView.UserName; }
                if (ticket.UserID == null) { name2 = "Unassigned"; } else { User u = (User)Users.GetUser(LoginUser, (int)ticket.UserID); name2 = u.FirstName + " " + u.LastName; }
                description = "Reassigned " + GetTicketLink(ticket) + " from user '" + name1 + "' to user '" + name2 + "'";
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
                if (oldTicketView.UserID != null)
                    ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, (int)oldTicketView.UserID, description);
            }

            // CHECK IF TICKET CLOSED
            if (oldTicketView.TicketStatusID != ticket.TicketStatusID)
            {
                bool oldClosed = oldTicketView.IsClosed;
                TicketStatus ticketStatus = (TicketStatus)TicketStatuses.GetTicketStatus(LoginUser, (int)ticket.TicketStatusID);
                bool newClosed = ticketStatus.IsClosed;
                if (newClosed)
                {
                    if (!oldClosed)
                    {
                        description = "Closed " + GetTicketLink(ticket);
                        ticket.CloserID = LoginUser.UserID;
                        ticket.DateClosed = DateTime.UtcNow;
                        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
                    }
                }
                else
                {
                    if (oldClosed)
                    {
                        description = "Reopened " + GetTicketLink(ticket);
                        ticket.CloserID = null;
                        ticket.DateClosed = null;
                        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
                    }
                }
            }
        }

        public int GetMaxTicketNumber(int organizationID)
        {
            int max = -1;

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT MAX(TicketNumber) FROM Tickets WHERE (OrganizationID = @OrganizationID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                object o = ExecuteScalar(command);
                if (o == DBNull.Value) return -1;
                max = (int)o;
            }
            return max;
        }

        public void LoadRelated(int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT t.* FROM Tickets t WHERE t.TicketID IN (SELECT Ticket2ID FROM TicketRelationships WHERE Ticket1ID = @TicketID) OR t.TicketID IN (SELECT Ticket1ID FROM TicketRelationships WHERE Ticket2ID = @TicketID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                Fill(command);
            }
        }

        public void LoadChildren(int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT t.* FROM Tickets t WHERE t.ParentID = @TicketID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                Fill(command);
            }
        }

        /// <summary>
        /// Loads Tickets for a TeamSupport Customer.  All the tickets in the user's organization.
        /// </summary>
        /// <param name="organizationID"></param>

        public void LoadByOrganizationID(int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Tickets WHERE (OrganizationID = @OrganizationID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command, "Tickets,Actions");
            }
        }

        public Ticket FindByImportID(string importID)
        {
            importID = importID.Trim().ToLower();
            foreach (Ticket ticket in this)
            {
                if (!string.IsNullOrWhiteSpace(ticket.ImportID) && ticket.ImportID.Trim().ToLower() == importID)
                {
                    return ticket;
                }
            }
            return null;
        }

        public Ticket FindByTicketNumber(int ticketNumber)
        {
            foreach (Ticket ticket in this)
            {
                if (ticket.TicketNumber == ticketNumber)
                {
                    return ticket;
                }
            }
            return null;
        }

        public void LoadByUserID(int? userID, int ticketTypeID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM TicketGridView WHERE (UserID = @UserID) AND (TicketTypeID = @TicketTypeID) AND (OrganizationID = @OrganizationID)";
                command.CommandType = CommandType.Text;
                if (userID == null) command.Parameters.AddWithValue("@UserID", DBNull.Value);
                else command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
                command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
                Fill(command, "TicketGridView,Actions");
            }
        }

        public void LoadByGroupID(int? groupID, int ticketTypeID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM TicketGridView WHERE (GroupID = @GroupID) AND (TicketTypeID = @TicketTypeID) AND (OrganizationID = @OrganizationID)";
                command.CommandType = CommandType.Text;
                if (groupID == null) command.Parameters.AddWithValue("@GroupID", DBNull.Value);
                else command.Parameters.AddWithValue("@GroupID", groupID);

                command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
                command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
                Fill(command, "TicketGridView,Actions");
            }
        }

        public void LoadByIsClosed(bool isClosed, int ticketTypeID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM TicketGridView WHERE (IsClosed = @IsClosed) AND (TicketTypeID = @TicketTypeID) AND (OrganizationID = @OrganizationID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@IsClosed", isClosed);
                command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
                command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
                Fill(command, "TicketGridView,Actions");
            }
        }

        public void LoadByTicketType(int ticketTypeID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Tickets WHERE (TicketTypeID = @TicketTypeID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
                Fill(command, "Tickets");
            }
        }

        public void LoadByCRMLinkItem(CRMLinkTableItem item)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
              SELECT
                *
              FROM
                Tickets t
                JOIN OrganizationTickets ot
                  ON t.TicketID = ot.TicketID
                JOIN Organizations o
                  ON o.OrganizationID = ot.OrganizationID
                LEFT JOIN TicketStatuses ts
                  ON t.TicketStatusID = ts.TicketStatusID
              WHERE
                t.OrganizationID = @OrgID
                AND
                (
                  (
                    @LastTicketID > 0
                    AND t.TicketID > @LastTicketID
                  )
                  OR
                  (
                    @LastTicketID <= 0
                    AND ts.IsClosed = 0
                  )
                )
                AND ISNULL(o.CRMLinkID,'') <> ''
            ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrgID", item.OrganizationID);
                command.Parameters.AddWithValue("@LastTicketID", item.LastTicketID);

                Fill(command, "Tickets");
            }
        }

        public void LoadTop5KBByCategoryID(int categoryID, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT TOP 5 TicketID, NAME
																FROM Tickets
																WHERE
																	OrganizationID              = @OrganizationID
																	AND IsKnowledgeBase         = 1
																	AND IsVisibleOnPortal         = 1
																	AND KnowledgeBaseCategoryID = @KnowledgeBaseCategoryID
																ORDER BY
																	DateModified desc";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@KnowledgeBaseCategoryID", categoryID);
                Fill(command, "Tickets");
            }
        }

        public void LoadTopXKBByCategoryID(int categoryID, int organizationID, int limit)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT TOP (@Limit) TicketID, NAME
																FROM Tickets
																WHERE
																	OrganizationID              = @OrganizationID
																	AND IsKnowledgeBase         = 1
																	AND IsVisibleOnPortal         = 1
																	AND KnowledgeBaseCategoryID = @KnowledgeBaseCategoryID
																ORDER BY
																	DateModified desc";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@KnowledgeBaseCategoryID", categoryID);
                command.Parameters.AddWithValue("@Limit", limit);
                Fill(command, "Tickets");
            }
        }

        public void LoadKBByCategoryID(int? categoryID, int organizationID, int customerID, int contactID, bool enableCustomerSpecificKB, bool enableCustomerProductAssociation, bool enableAnonymousProductAssociation, int sortOrder = 0)
        {
            using (SqlCommand command = new SqlCommand())
            {
                string isKnowledgeBaseClause = categoryID != null ? String.Format(" = {0} ", categoryID) : " IS NULL ";

                StringBuilder builder = new StringBuilder();

                if (enableCustomerSpecificKB && ((customerID > 0 || enableAnonymousProductAssociation) && enableCustomerProductAssociation))
                {
                    builder.Append(@"
                        SELECT Distinct
	                        t.TicketID
                            ,t.Name
	                        ,t.DateModified
                        FROM Tickets AS T
                        LEFT JOIN dbo.OrganizationTickets as OT
	                        ON OT.TicketID = T.TicketID
                        LEFT JOIN dbo.OrganizationProducts as OP
	                        on OP.OrganizationID = @CustomerID
                        WHERE
	                        t.OrganizationID = @OrganizationID
                            AND t.IsKnowledgeBase = 1
                            AND t.IsVisibleOnPortal = 1
                            AND t.KnowledgeBaseCategoryID" + isKnowledgeBaseClause + @"
                            AND (
			                        --Customer Match for sure included no matter what
			                        OT.OrganizationID = @customerID
			                        OR
                                    (
				                        --Tickets are assigned to no organization
				                        OT.TicketID IS NULL
				                        AND (
						                        --No product is associated to the ticket OR customer has the product in question
						                        T.ProductID IS NULL
						                        OR OP.ProductID = T.ProductID
					                        )
				                    )
		                        )
                    ");
                }
                else if (enableCustomerSpecificKB)
                {
                    builder.Append(@"
                        SELECT Distinct
	                        t.TicketID
                            ,t.Name
	                        ,t.DateModified
                        FROM Tickets AS T
                        LEFT JOIN dbo.OrganizationTickets as OT
	                        ON OT.TicketID = T.TicketID
                        WHERE
	                        t.OrganizationID = @OrganizationID
                            AND t.IsKnowledgeBase = 1
                            AND t.IsVisibleOnPortal = 1
                            AND t.KnowledgeBaseCategoryID" + isKnowledgeBaseClause + @"
                            AND (
		                        --Customer Match for sure included no matter what
		                        OT.OrganizationID = @customerID
		                        OR OT.TicketID IS NULL
	                        )
                   ");
                }
                else if ((customerID > 0 || enableAnonymousProductAssociation) && enableCustomerProductAssociation)
                {
                    builder.Append(@"
                        SELECT Distinct
	                        t.TicketID
                            ,t.Name
	                        ,t.DateModified
                        FROM Tickets AS T
                        LEFT JOIN dbo.OrganizationProducts as OP
	                        on OP.OrganizationID = @CustomerID
                        WHERE
	                        t.OrganizationID = @OrganizationID
                            AND t.IsKnowledgeBase = 1
                            AND t.IsVisibleOnPortal = 1
                            AND t.KnowledgeBaseCategoryID" + isKnowledgeBaseClause + @"
                            AND
		                        (
			                        T.ProductID is null
			                        OR OP.ProductID = T.ProductID
		                        )
                    ");
                }
                else {
                    builder.Append(@"
                        SELECT Distinct
	                        t.TicketID
                            ,t.Name
	                        ,t.DateModified
                        FROM Tickets AS T
                        WHERE
	                        t.OrganizationID = @OrganizationID
                            AND t.IsKnowledgeBase = 1
                            AND t.IsVisibleOnPortal = 1
                            AND t.KnowledgeBaseCategoryID" + isKnowledgeBaseClause
                );
                }

                if (sortOrder == (int)SortType.LastModified)
                {
                    builder.Append(@" ORDER BY t.DateModified desc");
                }
                else if (sortOrder == (int)SortType.Alphabetical)
                {
                    builder.Append(@" ORDER BY t.Name asc");
                }
                command.CommandText = builder.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@CustomerID", customerID);
                command.Parameters.AddWithValue("@ContactID", contactID);
                if (categoryID != null) { command.Parameters.AddWithValue("@KnowledgeBaseCategoryID", categoryID); }
                Fill(command, "Tickets");
            }
        }

        public void LoadForumByCategoryID(int forumCategoryID, int organizationID, int customerID, int contactID, bool enforceCustomerProduct = true)
        {
            using (SqlCommand command = new SqlCommand())
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(@"
                                SELECT t.TicketID, t.Name, t.DateModified, t.TicketStatusID, t.IsKnowledgeBase
                                FROM Tickets as T
                                Inner Join ForumTickets as FT on T.TicketID = FT.TicketID
                                Inner Join ForumCategories as FC on FT.ForumCategory = FC.CategoryID
                                WHERE
	                                t.OrganizationID         = @OrganizationID
	                                AND t.IsVisibleOnPortal  = 1
	                                AND FT.ForumCategory = @ForumCategoryID");
                builder.Append(@" ORDER BY t.DateModified desc");
                command.CommandText = builder.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@CustomerID", customerID);
                command.Parameters.AddWithValue("@ContactID", contactID);
                command.Parameters.AddWithValue("@ForumCategoryID", forumCategoryID);
                Fill(command, "Tickets");
            }
        }

        public void LoadPortalUserTickets(int userID, bool isClosed)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT T.*
																FROM UserTickets as UT
																INNER JOIN Tickets as T ON UT.TicketID = T.TicketID
																INNER JOIN TicketStatuses as TS ON T.TicketStatusID = TS.TicketStatusID
																WHERE UT.TicketID = T.TicketID
																		AND UT.UserID = @UserID
																		AND T.IsVisibleOnPortal = @IsClosed
																		AND TS.IsClosed = 0
																		AND T.TicketID NOT IN ( SELECT TicketID FROM forumtickets )";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@IsClosed", isClosed);
                Fill(command, "TicketGridView,Actions");
            }
        }

        public void AddOrganization(int organizationID, int ticketID)
        {
            if (GetAssociatedOrganizationCount(LoginUser, organizationID, ticketID) > 0) return;

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "INSERT INTO OrganizationTickets (TicketID, OrganizationID, DateCreated, CreatorID, DateModified, ModifierID) VALUES (@TicketID, @OrganizationID, @DateCreated, @CreatorID, @DateModified, @ModifierID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@TicketID", ticketID);
                command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
                command.Parameters.AddWithValue("@CreatorID", LoginUser.UserID);
                command.Parameters.AddWithValue("@DateModified", DateTime.UtcNow);
                command.Parameters.AddWithValue("@ModifierID", LoginUser.UserID);
                ExecuteNonQuery(command, "OrganizationTickets");
            }


            Organization org = (Organization)Organizations.GetOrganization(LoginUser, organizationID);
            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, ticketID);
            string description = "Added '" + org.Name + "' to the customer list for " + GetTicketLink(ticket);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tickets, ticketID, description);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Organizations, organizationID, description);
        }

        public void AddOrganization(int organizationID, int ticketID, int importFileID)
        {
            if (GetAssociatedOrganizationCount(LoginUser, organizationID, ticketID) > 0) return;

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "INSERT INTO OrganizationTickets (TicketID, OrganizationID, DateCreated, CreatorID, DateModified, ModifierID, ImportFileID) VALUES (@TicketID, @OrganizationID, @DateCreated, @CreatorID, @DateModified, @ModifierID, @ImportFileID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@TicketID", ticketID);
                command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
                command.Parameters.AddWithValue("@CreatorID", LoginUser.UserID);
                command.Parameters.AddWithValue("@DateModified", DateTime.UtcNow);
                command.Parameters.AddWithValue("@ModifierID", LoginUser.UserID);
                command.Parameters.AddWithValue("@ImportFileID", importFileID);
                ExecuteNonQuery(command, "OrganizationTickets");
            }


            Organization org = (Organization)Organizations.GetOrganization(LoginUser, organizationID);
            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, ticketID);
            string description = "Added '" + org.Name + "' to the customer list for " + GetTicketLink(ticket);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tickets, ticketID, description);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Organizations, organizationID, description);
        }

        public static int GetAssetCount(LoginUser loginUser, int assetID, int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM AssetTickets WHERE (TicketID = @TicketID) AND (AssetID = @AssetID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@AssetID", assetID);
                command.Parameters.AddWithValue("@TicketID", ticketID);

                Tickets tickets = new Tickets(loginUser);
                return (int)tickets.ExecuteScalar(command);
            }
        }


        public void AddAsset(int assetID, int ticketID)
        {
            Asset asset = Assets.GetAsset(LoginUser, assetID);
            Ticket ticket = Tickets.GetTicket(LoginUser, ticketID);
            if (asset.OrganizationID != ticket.OrganizationID) return;
            if (GetAssetCount(LoginUser, assetID, ticketID) > 0) return;

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "INSERT INTO AssetTickets (TicketID, AssetID, DateCreated, CreatorID) VALUES (@TicketID, @AssetID, @DateCreated, @CreatorID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@AssetID", assetID);
                command.Parameters.AddWithValue("@TicketID", ticketID);
                command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
                command.Parameters.AddWithValue("@CreatorID", LoginUser.UserID);
                ExecuteNonQuery(command, "AssetTickets");
            }

            string description = "Added '" + asset.Name + "' to the asset list for " + GetTicketLink(ticket);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tickets, ticketID, description);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Assets, assetID, description);
        }

        public void AddAsset(int assetID, int ticketID, int importFileID)
        {
            Asset asset = Assets.GetAsset(LoginUser, assetID);
            Ticket ticket = Tickets.GetTicket(LoginUser, ticketID);
            if (asset.OrganizationID != ticket.OrganizationID) return;
            if (GetAssetCount(LoginUser, assetID, ticketID) > 0) return;

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "INSERT INTO AssetTickets (TicketID, AssetID, DateCreated, CreatorID, ImportFileID) VALUES (@TicketID, @AssetID, @DateCreated, @CreatorID, @ImportFileID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@AssetID", assetID);
                command.Parameters.AddWithValue("@TicketID", ticketID);
                command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
                command.Parameters.AddWithValue("@CreatorID", LoginUser.UserID);
                command.Parameters.AddWithValue("@ImportFileID", importFileID);
                ExecuteNonQuery(command, "AssetTickets");
            }

            string description = "Added '" + asset.Name + "' to the asset list for " + GetTicketLink(ticket);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tickets, ticketID, description);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Assets, assetID, description);
        }


        public void RemoveAsset(int assetID, int ticketID)
        {
            Asset asset = Assets.GetAsset(LoginUser, assetID);
            Ticket ticket = Tickets.GetTicket(LoginUser, ticketID);

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "DELETE FROM AssetTickets WHERE (TicketID = @TicketID) AND (AssetID = @AssetID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@AssetID", assetID);
                command.Parameters.AddWithValue("@TicketID", ticketID);
                ExecuteNonQuery(command, "AssetTickets");
            }

            string description = "Removed '" + asset.Name + "' to the asset list for " + GetTicketLink(ticket);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Tickets, ticketID, description);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Assets, assetID, description);
        }


        public static void DeleteRelationships(LoginUser loginUser, int ticketID)
        {
            Tickets tickets = new Tickets(loginUser);
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "DELETE FROM TicketRelationships WHERE (Ticket1ID = @TicketID) OR (Ticket2ID = @TicketID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                tickets.ExecuteNonQuery(command, "TicketRelationships");
            }
        }

        public void RemoveOrganization(int organizationID, int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "DELETE FROM OrganizationTickets WHERE (TicketID = @TicketID) AND (OrganizationID = @OrganizationID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@TicketID", ticketID);
                ExecuteNonQuery(command, "OrganizationTickets");
            }
            Organization org = (Organization)Organizations.GetOrganization(LoginUser, organizationID);
            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, ticketID);
            string description = "Removed '" + org.Name + "' from the customer list for " + GetTicketLink(ticket);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Tickets, ticketID, description);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Organizations, organizationID, description);
        }

        public void AddContact(int userID, int ticketID)
        {
            try
            {

                using (SqlCommand command = new SqlCommand())
                {
                    //This will throw an exception if userid and ticketid combination already exist.
                    //command.CommandText = "INSERT INTO UserTickets (TicketID, UserID, DateCreated, CreatorID) VALUES (@TicketID, @UserID, @DateCreated, @CreatorID)";
                    command.CommandText = @"INSERT INTO UserTickets (TicketID, UserID, DateCreated, CreatorID) 
                                            SELECT @TicketID, @UserID, @DateCreated, @CreatorID
                                            WHERE NOT EXISTS(SELECT * FROM UserTickets WHERE TicketID = @TicketID and UserID = @UserID)";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@TicketID", ticketID);
                    command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
                    command.Parameters.AddWithValue("@CreatorID", LoginUser.UserID);
                    ExecuteNonQuery(command, "UserTickets");
                }
            }
            catch (Exception)
            {
            }


            UsersViewItem user = UsersView.GetUsersViewItem(LoginUser, userID);

            AddOrganization(user.OrganizationID, ticketID);
            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, ticketID);
            string description = "Added '" + user.FirstName + " " + user.LastName + "' to the contact list for " + GetTicketLink(ticket);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tickets, ticketID, description);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Users, userID, description);
        }

        public void AddContact(int userID, int ticketID, int importFileID)
        {
            try
            {

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "INSERT INTO UserTickets (TicketID, UserID, DateCreated, CreatorID, ImportFileID) VALUES (@TicketID, @UserID, @DateCreated, @CreatorID, @ImportFileID)";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@TicketID", ticketID);
                    command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
                    command.Parameters.AddWithValue("@CreatorID", LoginUser.UserID);
                    command.Parameters.AddWithValue("@ImportFileID", importFileID);
                    ExecuteNonQuery(command, "UserTickets");
                }
            }
            catch (Exception)
            {
            }


            UsersViewItem user = UsersView.GetUsersViewItem(LoginUser, userID);

            AddOrganization(user.OrganizationID, ticketID, importFileID);
            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, ticketID);
            string description = "Added '" + user.FirstName + " " + user.LastName + "' to the contact list for " + GetTicketLink(ticket);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tickets, ticketID, description);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Users, userID, description);
        }

        public void SetUserAsSentToSalesForce(int userID, int ticketID)
        {
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "UPDATE OrganizationTickets SET SentToSalesForce = 0 WHERE TicketID = @TicketID";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@TicketID", ticketID);
                    ExecuteNonQuery(command, "UserTickets");
                }

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "UPDATE UserTickets SET SentToSalesForce = 0 WHERE TicketID = @TicketID";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@TicketID", ticketID);
                    ExecuteNonQuery(command, "UserTickets");
                }

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "UPDATE UserTickets SET SentToSalesForce = 1 WHERE UserID = @UserID AND TicketID = @TicketID";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@TicketID", ticketID);
                    ExecuteNonQuery(command, "UserTickets");
                }
            }
            catch (Exception)
            {
            }
        }

        public void SetOrganizationAsSentToSalesForce(int organizationID, int ticketID)
        {
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "UPDATE OrganizationTickets SET SentToSalesForce = 0 WHERE TicketID = @TicketID";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@TicketID", ticketID);
                    ExecuteNonQuery(command, "UserTickets");
                }

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "UPDATE UserTickets SET SentToSalesForce = 0 WHERE TicketID = @TicketID";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@TicketID", ticketID);
                    ExecuteNonQuery(command, "UserTickets");
                }

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "UPDATE OrganizationTickets SET SentToSalesForce = 1 WHERE OrganizationID = @organizationID AND TicketID = @TicketID";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@organizationID", organizationID);
                    command.Parameters.AddWithValue("@TicketID", ticketID);
                    ExecuteNonQuery(command, "UserTickets");
                }
            }
            catch (Exception)
            {
            }
        }

        public void LoadByContact(int userID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT t.* FROM Tickets t WHERE t.TicketID IN (SELECT TicketID FROM UserTickets WHERE UserID = @UserID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                Fill(command);
            }
        }

        public void LoadbyUserMonth(DateTime date, int userID, int orgID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * from Tickets WHERE (Month(DueDate) = @month) AND (Year(DueDate) = @year) AND (OrganizationID = @OrgID) and ((GroupID in (select GroupID from GroupUsers where UserID = @UserID)) or (UserID = @UserID))";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@month", date.Month);
                command.Parameters.AddWithValue("@year", date.Year);
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@OrgID", orgID);
                Fill(command);
            }
        }

        public void LoadbyCompanyMonth(DateTime date, int companyID, int orgID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * from Tickets WHERE (Month(DueDate) = @month) AND (Year(DueDate) = @year) AND (OrganizationID = @OrgID) AND ((TicketID in (select TicketID from OrganizationTickets where OrganizationID = @companyID)))";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@month", date.Month);
                command.Parameters.AddWithValue("@year", date.Year);
                command.Parameters.AddWithValue("@companyID", companyID);
                command.Parameters.AddWithValue("@OrgID", orgID);
                Fill(command);
            }
        }

        public void LoadbyGroupMonth(DateTime date, int offset, int groupID, int orgID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * from Tickets WHERE (Month(DATEADD(hour, @offset, DueDate)) = @month) AND (Year(DATEADD(hour, @offset, DueDate)) = @year) AND (GroupID = @groupID) AND (OrganizationID = @OrgID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@offset", offset);
                command.Parameters.AddWithValue("@month", date.Month);
                command.Parameters.AddWithValue("@year", date.Year);
                command.Parameters.AddWithValue("@groupID", groupID);
                command.Parameters.AddWithValue("@OrgID", orgID);
                Fill(command);
            }
        }

        public void LoadAllDueDates(int userID, int orgID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * from Tickets WHERE (UserID = @UserID) AND (OrganizationID = @OrgID) and (DueDate IS NOT NULL)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@OrgID", orgID);
                Fill(command);
            }
        }

        public void RemoveContact(int userID, int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "DELETE FROM UserTickets WHERE (TicketID = @TicketID) AND (UserID = @UserID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@TicketID", ticketID);
                ExecuteNonQuery(command, "UserTickets");
            }
            UsersViewItem user = UsersView.GetUsersViewItem(LoginUser, userID);

            if (GetAssociatedContactCount(LoginUser, user.OrganizationID, ticketID) < 1)
            {
                RemoveOrganization(user.OrganizationID, ticketID);
            }

            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, ticketID);
            string description = "Removed '" + user.FirstName + " " + user.LastName + "' from the contact list for " + GetTicketLink(ticket);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Tickets, ticketID, description);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Users, userID, description);
        }

        private void AddGridParameter(SqlCommand command, string name, int id)
        {
            if (id > -1)
            {
                command.CommandText = command.CommandText + " AND (" + name + " = @" + name + ")";
                command.Parameters.AddWithValue("@" + name, id);
            }
            else if (id == -2)
            {
                command.CommandText = command.CommandText + " AND (" + name + " is null)";
            }
            else if (id == -3)
            {
                command.CommandText = command.CommandText + " AND (IsClosed = 0)";
            }
            else if (id == -4)
            {
                command.CommandText = command.CommandText + " AND (IsClosed = 1)";
            }
        }

        public int LoadForGridCount(int organizationID, int ticketTypeID, int ticketStatusID, int ticketSeverityID,
          int userID, int groupID, int productID, int reportedVersionID, int resolvedVersionID,
          int customerID, bool? onlyPortal, bool? onlyKnowledgeBase,
          DateTime? dateCreatedBegin, DateTime? dateCreatedEnd, DateTime? dateModifiedBegin, DateTime? dateModifiedEnd,
          string search)
        {
            if (search.Trim() == "") search = "\"\"";
            using (SqlCommand command = new SqlCommand())
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(@" SELECT COUNT(*)
                          FROM dbo.TicketGridView tgv LEFT JOIN Tickets t ON tgv.TicketID = t.TicketID
                          WHERE (tgv.OrganizationID = @OrganizationID)
                          AND ((tgv.TicketTypeID = @TicketTypeID) OR (@TicketTypeID = -1))
                          AND ((tgv.TicketSeverityID = @TicketSeverityID) OR (@TicketSeverityID = -1))
                          AND ((tgv.ProductID = @ProductID) OR (@ProductID = -1))
                          AND ((tgv.ReportedVersionID = @ReportedVersionID) OR (@ReportedVersionID = -1))
                          AND ((tgv.SolvedVersionID = @ResolvedVersionID) OR (@ResolvedVersionID = -1))
                          AND (((@UserID = -2) AND (tgv.UserID IS NULL)) OR (@UserID = tgv.UserID) OR (@UserID = -1))
                          AND (((@GroupID = -2) AND (tgv.GroupID IS NULL)) OR (@GroupID = tgv.GroupID) OR (@GroupID = -1))
                          AND ((tgv.TicketStatusID = @TicketStatusID) OR (@TicketStatusID = -1) OR ((@TicketStatusID = -3) AND (tgv.IsClosed = 0)) OR ((@TicketStatusID = -4) AND (tgv.IsClosed = 1)))
                          AND ((@CustomerID = -1) OR (EXISTS(SELECT * FROM OrganizationTickets ot WHERE (ot.OrganizationID = @CustomerID) AND (ot.TicketID = tgv.TicketID))))
                          AND ((@IsPortal is null) OR (tgv.IsVisibleOnPortal = @IsPortal))
                          AND ((@IsKnowledgeBase is null) OR (tgv.IsKnowledgeBase = @IsKnowledgeBase))
                          AND ((@DateCreatedBegin is null) OR (tgv.DateCreated >= @DateCreatedBegin))
                          AND ((@DateCreatedEnd is null) OR (tgv.DateCreated <= @DateCreatedEnd))
                          AND ((@DateModifiedBegin is null) OR (tgv.DateModified >= @DateModifiedBegin))
                          AND ((@DateModifiedEnd is null) OR (tgv.DateModified <= @DateModifiedEnd))
                          AND ((@Search = '""""')
                                OR (CONTAINS((t.[Name]), @Search))
                                OR EXISTS (SELECT * FROM Actions a WHERE (a.TicketID = tgv.TicketID) AND CONTAINS((a.[Description], a.[Name]), @Search))
                                OR EXISTS (SELECT * FROM CustomValues cv
                                        LEFT JOIN CustomFields cf ON cv.CustomFieldID = cf.CustomFieldID
                                        WHERE (cf.RefType = 17)
                                        AND (cv.RefID = tgv.TicketID)
                                        AND CONTAINS((cv.[CustomValue]), @Search))
                                OR (tgv.TicketNumber LIKE '%'+@SearchClean+'%'))
                        ");

                command.CommandText = builder.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
                command.Parameters.AddWithValue("@TicketStatusID", ticketStatusID);
                command.Parameters.AddWithValue("@TicketSeverityID", ticketSeverityID);
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@GroupID", groupID);
                command.Parameters.AddWithValue("@ProductID", productID);
                command.Parameters.AddWithValue("@ReportedVersionID", reportedVersionID);
                command.Parameters.AddWithValue("@ResolvedVersionID", resolvedVersionID);
                command.Parameters.AddWithValue("@CustomerID", customerID);
                command.Parameters.AddWithValue("@IsPortal", onlyPortal == null ? (object)DBNull.Value : onlyPortal);
                command.Parameters.AddWithValue("@IsKnowledgeBase", onlyKnowledgeBase == null ? (object)DBNull.Value : onlyKnowledgeBase);
                command.Parameters.AddWithValue("@DateCreatedBegin", dateCreatedBegin == null ? (object)DBNull.Value : dateCreatedBegin);
                command.Parameters.AddWithValue("@DateCreatedEnd", dateCreatedEnd == null ? (object)DBNull.Value : dateCreatedEnd);
                command.Parameters.AddWithValue("@DateModifiedBegin", dateModifiedBegin == null ? (object)DBNull.Value : dateModifiedBegin);
                command.Parameters.AddWithValue("@DateModifiedEnd", dateModifiedEnd == null ? (object)DBNull.Value : dateModifiedEnd);
                command.Parameters.AddWithValue("@Search", search);
                command.Parameters.AddWithValue("@SearchClean", search.Replace("*", "").Replace("%", "").Replace("\"", ""));

                return (int)ExecuteScalar(command, "TicketGridView,Actions");
            }
        }


        public void LoadForGrid(int pageIndex, int pageSize, int organizationID, int ticketTypeID, int ticketStatusID, int ticketSeverityID,
          int userID, int groupID, int productID, int reportedVersionID, int resolvedVersionID,
          int customerID, bool? onlyPortal, bool? onlyKnowledgeBase,
          DateTime? dateCreatedBegin, DateTime? dateCreatedEnd, DateTime? dateModifiedBegin, DateTime? dateModifiedEnd,
          string search, string sortColumn, bool sortAsc)
        {
            if (search.Trim() == "") search = @"""""";

            using (SqlCommand command = new SqlCommand())
            {
                string sort = sortColumn;
                switch (sortColumn)
                {
                    case "Severity": sort = "SeverityPosition"; break;
                    case "Status": sort = "StatusPosition"; break;
                    default: break;
                }
                StringBuilder builder = new StringBuilder();
                builder.Append("WITH TicketRows AS (SELECT ROW_NUMBER() OVER (ORDER BY tgv.");
                builder.Append(sort);
                if (sortAsc) builder.Append(" ASC");
                else builder.Append(" DESC");
                builder.Append(") AS RowNumber, tgv.*");
                builder.Append(@"


                                  FROM TicketGridView tgv LEFT JOIN Tickets t ON tgv.TicketID = t.TicketID
                                  WHERE (tgv.OrganizationID = @OrganizationID)
                                  AND ((tgv.TicketTypeID = @TicketTypeID) OR (@TicketTypeID = -1))
                                  AND ((tgv.TicketSeverityID = @TicketSeverityID) OR (@TicketSeverityID = -1))
                                  AND ((tgv.ProductID = @ProductID) OR (@ProductID = -1))
                                  AND ((tgv.ReportedVersionID = @ReportedVersionID) OR (@ReportedVersionID = -1))
                                  AND ((tgv.SolvedVersionID = @ResolvedVersionID) OR (@ResolvedVersionID = -1))
                                  AND (((@UserID = -2) AND (tgv.UserID IS NULL)) OR (@UserID = tgv.UserID) OR (@UserID = -1))
                                  AND (((@GroupID = -2) AND (tgv.GroupID IS NULL)) OR (@GroupID = tgv.GroupID) OR (@GroupID = -1))
                                  AND ((tgv.TicketStatusID = @TicketStatusID) OR (@TicketStatusID = -1) OR ((@TicketStatusID = -3) AND (tgv.IsClosed = 0)) OR ((@TicketStatusID = -4) AND (tgv.IsClosed = 1)))
                                  AND ((@CustomerID = -1) OR (EXISTS(SELECT * FROM OrganizationTickets ot WHERE (ot.OrganizationID = @CustomerID) AND (ot.TicketID = tgv.TicketID))))
                                  AND ((@IsPortal is null) OR (tgv.IsVisibleOnPortal = @IsPortal))
                                  AND ((@IsKnowledgeBase is null) OR (tgv.IsKnowledgeBase = @IsKnowledgeBase))
                                  AND ((@DateCreatedBegin is null) OR (tgv.DateCreated >= @DateCreatedBegin))
                                  AND ((@DateCreatedEnd is null) OR (tgv.DateCreated <= @DateCreatedEnd))
                                  AND ((@DateModifiedBegin is null) OR (tgv.DateModified >= @DateModifiedBegin))
                                  AND ((@DateModifiedEnd is null) OR (tgv.DateModified <= @DateModifiedEnd))
                                  AND ((@Search = '""""') OR (tgv.TicketNumber LIKE '%'+@SearchClean+'%')
                                        OR (CONTAINS((t.[Name]), @Search))
                                        OR EXISTS (SELECT * FROM Actions a WHERE (a.TicketID = tgv.TicketID) AND CONTAINS((a.[Description], a.[Name]), @Search))
                                        --OR EXISTS (SELECT * FROM CustomValues cv
                                          --      LEFT JOIN CustomFields cf ON cv.CustomFieldID = cf.CustomFieldID
                                            --    WHERE (cf.RefType = 17)
                                              --  AND (cv.RefID = tgv.TicketID)
                                                --AND CONTAINS((cv.[CustomValue]), @Search))
                                        )
                                )

                                  SELECT * FROM TicketRows
                                  WHERE RowNumber BETWEEN @PageIndex*@PageSize+1 AND @PageIndex*@PageSize+@PageSize
                                  ORDER BY RowNumber ASC");

                command.CommandText = builder.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@PageIndex", pageIndex);
                command.Parameters.AddWithValue("@PageSize", pageSize);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
                command.Parameters.AddWithValue("@TicketStatusID", ticketStatusID);
                command.Parameters.AddWithValue("@TicketSeverityID", ticketSeverityID);
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@GroupID", groupID);
                command.Parameters.AddWithValue("@ProductID", productID);
                command.Parameters.AddWithValue("@ReportedVersionID", reportedVersionID);
                command.Parameters.AddWithValue("@ResolvedVersionID", resolvedVersionID);
                command.Parameters.AddWithValue("@CustomerID", customerID);
                command.Parameters.AddWithValue("@IsPortal", onlyPortal == null ? (object)DBNull.Value : onlyPortal);
                command.Parameters.AddWithValue("@IsKnowledgeBase", onlyKnowledgeBase == null ? (object)DBNull.Value : onlyKnowledgeBase);
                command.Parameters.AddWithValue("@DateCreatedBegin", dateCreatedBegin == null ? (object)DBNull.Value : dateCreatedBegin);
                command.Parameters.AddWithValue("@DateCreatedEnd", dateCreatedEnd == null ? (object)DBNull.Value : dateCreatedEnd);
                command.Parameters.AddWithValue("@DateModifiedBegin", dateModifiedBegin == null ? (object)DBNull.Value : dateModifiedBegin);
                command.Parameters.AddWithValue("@DateModifiedEnd", dateModifiedEnd == null ? (object)DBNull.Value : dateModifiedEnd);
                command.Parameters.AddWithValue("@Search", search);
                command.Parameters.AddWithValue("@SearchClean", search.Replace("*", "").Replace("%", "").Replace("\"", ""));


                Fill(command, "TicketGridView,Actions");
            }

            /* using (SqlCommand command = new SqlCommand())
             {
               command.CommandText = "uspSelectTicketPage";
               command.CommandType = CommandType.StoredProcedure;
               command.Parameters.AddWithValue("@PageIndex", pageIndex);
               command.Parameters.AddWithValue("@PageSize", pageSize);
               command.Parameters.AddWithValue("@OrganizationID", organizationID);
               command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
               command.Parameters.AddWithValue("@TicketStatusID", ticketStatusID);
               command.Parameters.AddWithValue("@TicketSeverityID", ticketSeverityID);
               command.Parameters.AddWithValue("@UserID", userID);
               command.Parameters.AddWithValue("@GroupID", groupID);
               command.Parameters.AddWithValue("@ProductID", productID);
               command.Parameters.AddWithValue("@ReportedVersionID", reportedVersionID);
               command.Parameters.AddWithValue("@ResolvedVersionID", resolvedVersionID);
               command.Parameters.AddWithValue("@CustomerID", customerID);
               command.Parameters.AddWithValue("@IsPortal", onlyPortal == null ? (object)DBNull.Value : onlyPortal);
               command.Parameters.AddWithValue("@IsKnowledgeBase", onlyKnowledgeBase == null ? (object)DBNull.Value : onlyKnowledgeBase);
               command.Parameters.AddWithValue("@DateCreatedBegin", dateCreatedBegin == null ? (object)DBNull.Value : dateCreatedBegin);
               command.Parameters.AddWithValue("@DateCreatedEnd", dateCreatedEnd == null ? (object)DBNull.Value : dateCreatedEnd);
               command.Parameters.AddWithValue("@DateModifiedBegin", dateModifiedBegin == null ? (object)DBNull.Value : dateModifiedBegin);
               command.Parameters.AddWithValue("@DateModifiedEnd", dateModifiedEnd == null ? (object)DBNull.Value : dateModifiedEnd);
               command.Parameters.AddWithValue("@Search", search);
               command.Parameters.AddWithValue("@SortColumn", "Name");
               command.Parameters.AddWithValue("@SortAsc", false);

               Fill(command, "TicketGridView,Actions");
             }*/
        }

        public int LoadForSearchCount(int organizationID, int ticketTypeID, int ticketStatusID, int ticketSeverityID,
          int userID, int groupID, int productID, int reportedVersionID, int resolvedVersionID,
          int customerID, bool? onlyPortal, bool? onlyKnowledgeBase,
          DateTime? dateCreatedBegin, DateTime? dateCreatedEnd, DateTime? dateModifiedBegin, DateTime? dateModifiedEnd,
          string search)
        {
            if (search.Trim() == "") search = "\"\"";
            using (SqlCommand command = new SqlCommand())
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(@" SELECT COUNT(*)
                          FROM dbo.TicketGridView tgv LEFT JOIN Tickets t ON tgv.TicketID = t.TicketID
                          WHERE (tgv.OrganizationID = @OrganizationID)
                          AND ((tgv.TicketTypeID = @TicketTypeID) OR (@TicketTypeID = -1))
                          AND ((tgv.TicketSeverityID = @TicketSeverityID) OR (@TicketSeverityID = -1))
                          AND ((tgv.ProductID = @ProductID) OR (@ProductID = -1))
                          AND ((tgv.ReportedVersionID = @ReportedVersionID) OR (@ReportedVersionID = -1))
                          AND ((tgv.SolvedVersionID = @ResolvedVersionID) OR (@ResolvedVersionID = -1))
                          AND (((@UserID = -2) AND (tgv.UserID IS NULL)) OR (@UserID = tgv.UserID) OR (@UserID = -1))
                          AND (((@GroupID = -2) AND (tgv.GroupID IS NULL)) OR (@GroupID = tgv.GroupID) OR (@GroupID = -1))
                          AND ((tgv.TicketStatusID = @TicketStatusID) OR (@TicketStatusID = -1) OR ((@TicketStatusID = -3) AND (tgv.IsClosed = 0)) OR ((@TicketStatusID = -4) AND (tgv.IsClosed = 1)))
                          AND ((@CustomerID = -1) OR (EXISTS(SELECT * FROM OrganizationTickets ot WHERE (ot.OrganizationID = @CustomerID) AND (ot.TicketID = tgv.TicketID))))
                          AND ((@IsPortal is null) OR (tgv.IsVisibleOnPortal = @IsPortal))
                          AND ((@IsKnowledgeBase is null) OR (tgv.IsKnowledgeBase = @IsKnowledgeBase))
                          AND ((@DateCreatedBegin is null) OR (tgv.DateCreated >= @DateCreatedBegin))
                          AND ((@DateCreatedEnd is null) OR (tgv.DateCreated <= @DateCreatedEnd))
                          AND ((@DateModifiedBegin is null) OR (tgv.DateModified >= @DateModifiedBegin))
                          AND ((@DateModifiedEnd is null) OR (tgv.DateModified <= @DateModifiedEnd))
                          AND ((@Search = '""""')
                                OR (CONTAINS((t.[Name]), @Search))
                                OR EXISTS (SELECT * FROM Actions a WHERE (a.TicketID = tgv.TicketID) AND CONTAINS((a.[Description], a.[Name]), @Search))
                                OR EXISTS (SELECT * FROM CustomValues cv
                                        LEFT JOIN CustomFields cf ON cv.CustomFieldID = cf.CustomFieldID
                                        WHERE (cf.RefType = 17)
                                        AND (cv.RefID = tgv.TicketID)
                                        AND CONTAINS((cv.[CustomValue]), @Search))
                                OR (tgv.TicketNumber LIKE '%'+@SearchClean+'%'))
                        ");

                command.CommandText = builder.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
                command.Parameters.AddWithValue("@TicketStatusID", ticketStatusID);
                command.Parameters.AddWithValue("@TicketSeverityID", ticketSeverityID);
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@GroupID", groupID);
                command.Parameters.AddWithValue("@ProductID", productID);
                command.Parameters.AddWithValue("@ReportedVersionID", reportedVersionID);
                command.Parameters.AddWithValue("@ResolvedVersionID", resolvedVersionID);
                command.Parameters.AddWithValue("@CustomerID", customerID);
                command.Parameters.AddWithValue("@IsPortal", onlyPortal == null ? (object)DBNull.Value : onlyPortal);
                command.Parameters.AddWithValue("@IsKnowledgeBase", onlyKnowledgeBase == null ? (object)DBNull.Value : onlyKnowledgeBase);
                command.Parameters.AddWithValue("@DateCreatedBegin", dateCreatedBegin == null ? (object)DBNull.Value : dateCreatedBegin);
                command.Parameters.AddWithValue("@DateCreatedEnd", dateCreatedEnd == null ? (object)DBNull.Value : dateCreatedEnd);
                command.Parameters.AddWithValue("@DateModifiedBegin", dateModifiedBegin == null ? (object)DBNull.Value : dateModifiedBegin);
                command.Parameters.AddWithValue("@DateModifiedEnd", dateModifiedEnd == null ? (object)DBNull.Value : dateModifiedEnd);
                command.Parameters.AddWithValue("@Search", search);
                command.Parameters.AddWithValue("@SearchClean", search.Replace("*", "").Replace("%", "").Replace("\"", ""));

                return (int)ExecuteScalar(command, "TicketGridView,Actions");
            }
        }

        public void LoadForSearch(int organizationID, string search)
        {
            if (search.Trim() == "") search = @"""""";

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
  SELECT tgv.*
  FROM TicketGridView tgv
    WHERE (tgv.OrganizationID = @OrganizationID)
    AND (
      (@Search = '""""')
      OR (tgv.TicketNumber LIKE '%'+@SearchClean+'%')
      --OR (tgv.Name LIKE @SearchClean+'%')
      OR EXISTS (SELECT * FROM Tickets t WHERE (t.TicketID = tgv.TicketID) AND CONTAINS((t.[Name]), @Search))
      OR EXISTS (SELECT * FROM Actions a WHERE (a.TicketID = tgv.TicketID) AND CONTAINS((a.[Description], a.[Name]), @Search))
      OR EXISTS (SELECT * FROM CustomValues cv LEFT JOIN CustomFields cf ON cv.CustomFieldID = cf.CustomFieldID WHERE (cf.RefType = 17) AND (cv.RefID = tgv.TicketID) AND CONTAINS((cv.[CustomValue]), @Search))
  )";
                /*
                command.CommandText = @"
        select tgv.*
        from ticketgridview tgv
        where (tgv.OrganizationID = @OrganizationID)
        and (
            (tgv.TicketNumber LIKE '%'+@SearchClean+'%') or (tgv.name like '%'+@search+'%')
            or tgv.ticketid in (select distinct(ticketid) from actions where contains(description,@search) or contains(name,@search))
            )
        order by tgv.ticketnumber desc

        ";*/
                UseCache = true;
                CacheExpirationSeconds = 300;

                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@Search", search);
                command.Parameters.AddWithValue("@SearchClean", search.Replace("*", "").Replace("%", "").Replace("\"", ""));
                Fill(command, "TicketSearch");
            }


        }

        public void LoadForTags(string tags)
        {
            string[] tagArray = tags.Split(',');

            StringBuilder builder = new StringBuilder(
      @"SELECT tgv.[TicketID]
      ,tgv.[ProductName]
      ,tgv.[ReportedVersion]
      ,tgv.[SolvedVersion]
      ,tgv.[GroupName]
      ,tgv.[TicketTypeName]
      ,tgv.[UserName]
      ,tgv.[Status]
      ,tgv.[StatusPosition]
      ,tgv.[SeverityPosition]
      ,tgv.[IsClosed]
      ,tgv.[Severity]
      ,tgv.[TicketNumber]
      ,tgv.[IsVisibleOnPortal]
      ,tgv.[IsKnowledgeBase]
      ,tgv.[ReportedVersionID]
      ,tgv.[SolvedVersionID]
      ,tgv.[ProductID]
      ,tgv.[GroupID]
      ,tgv.[UserID]
      ,tgv.[TicketStatusID]
      ,tgv.[TicketTypeID]
      ,tgv.[TicketSeverityID]
      ,tgv.[OrganizationID]
      ,tgv.[Name]
      ,tgv.[ParentID]
      ,tgv.[ModifierID]
      ,tgv.[CreatorID]
      ,tgv.[DateModified]
      ,tgv.[DateCreated]
      ,tgv.[DateClosed]
      ,tgv.[CloserID]
      ,tgv.[DaysClosed]
      ,tgv.[DaysOpened]
      ,tgv.[CloserName]
      ,tgv.[CreatorName]
      ,tgv.[ModifierName]
      ,tgv.[SlaViolationTime]
      ,tgv.[SlaWarningTime]
      ,tgv.[SlaViolationHours]
      ,tgv.[SlaWarningHours]
FROM TicketGridView tgv
WHERE tgv.OrganizationID = @OrganizationID"
      );
            for (int i = 0; i < tagArray.Length; i++)
            {
                builder.Append(" AND EXISTS (SELECT * FROM TagLinksView WHERE TagLinksView.RefID=tgv.TicketID AND TagLinksView.RefType=17 AND TagLinksView.Value = @Value" + i.ToString() + ")");
            }

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = builder.ToString();
                UseCache = false;
                CacheExpirationSeconds = 300;

                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
                for (int i = 0; i < tagArray.Length; i++)
                {
                    command.Parameters.AddWithValue("@Value" + i.ToString(), tagArray[i]);
                }

                Fill(command, "TicketTags");
            }


        }

        public void LoadByDescription(int organizationID, string description)
        {
            LoadByDescription(organizationID, description, 0);
        }

        public void LoadKBByDescription(int organizationID, string description, int maxRecords)
        {
            if (description.Trim().Length < 1) return;
            string commandText;
            using (SqlCommand command = new SqlCommand())
            {
                StringBuilder builder = new StringBuilder();

                int number;
                string cleanSearch = description.Replace("*", "").Replace("%", "").Replace("\"", "");
                bool isNumber = int.TryParse(cleanSearch, out number);
                /*
                if (isNumber)
                {
                  commandText = " SELECT TOP {0} CAST(TicketNumber AS VARCHAR(50)) + ':  ' + Name AS TicketDescription, TicketID, TicketNumber FROM Tickets" +
                                        " WHERE OrganizationID = @OrganizationID" +
                                        " AND (TicketNumber LIKE @SearchClean+'%')" +
                                        " ORDER BY TicketNumber DESC";
                }
                else*/
                {
                    commandText = @"
SELECT TOP {0} CAST(TicketNumber AS VARCHAR(50)) + ':  ' + Name AS TicketDescription, TicketID, TicketNumber FROM Tickets t
WHERE OrganizationID = @OrganizationID
AND IsKnowledgebase = 1
AND (
  CONTAINS((t.[Name]), @Search)
  OR TicketNumber LIKE @SearchClean+'%'
  OR EXISTS (SELECT * FROM Actions a WHERE (a.TicketID = t.TicketID) AND CONTAINS((a.[Description], a.[Name]), @Search))
  OR EXISTS (SELECT * FROM CustomValues cv LEFT JOIN CustomFields cf ON cv.CustomFieldID = cf.CustomFieldID WHERE (cf.RefType = 17) AND (cv.RefID = t.TicketID) AND CONTAINS(cv.[CustomValue], @Search))
)
ORDER BY TicketNumber DESC";
                }


                command.CommandText = String.Format(commandText, (maxRecords < 1 ? 15 : maxRecords).ToString());
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@Search", description);
                command.Parameters.AddWithValue("@SearchClean", cleanSearch);
                UseCache = true;
                CacheExpirationSeconds = 300;
                Fill(command, "QuickSearch");
            }

        }

        public void LoadByDescription(int organizationID, string description, int maxRecords)
        {
            if (description.Trim().Length < 1) return;
            string commandText;
            using (SqlCommand command = new SqlCommand())
            {
                StringBuilder builder = new StringBuilder();

                int number;
                string cleanSearch = description.Replace("*", "").Replace("%", "").Replace("\"", "");
                bool isNumber = int.TryParse(cleanSearch, out number);
                /*
                if (isNumber)
                {
                  commandText = " SELECT TOP {0} CAST(TicketNumber AS VARCHAR(50)) + ':  ' + Name AS TicketDescription, TicketID, TicketNumber FROM Tickets " +
                                        " WHERE OrganizationID = @OrganizationID" +
                                        " AND (TicketNumber LIKE @SearchClean+'%')" +
                                        " ORDER BY TicketNumber DESC";
                }
                else*/
                {
                    commandText = @"
SELECT TOP {0} CAST(TicketNumber AS VARCHAR(50)) + ':  ' + Name AS TicketDescription, TicketID, TicketNumber FROM Tickets t (NOLOCK)
WHERE OrganizationID = @OrganizationID
AND (
  CONTAINS((t.[Name]), @Search)
  OR TicketNumber LIKE @SearchClean+'%'
  OR EXISTS (SELECT * FROM Actions a WHERE (a.TicketID = t.TicketID) AND CONTAINS((a.[Description], a.[Name]), @Search))
  OR EXISTS (SELECT * FROM CustomValues cv LEFT JOIN CustomFields cf ON cv.CustomFieldID = cf.CustomFieldID WHERE (cf.RefType = 17) AND (cv.RefID = t.TicketID) AND CONTAINS(cv.[CustomValue], @Search))
)
ORDER BY TicketNumber DESC";
                }


                command.CommandText = String.Format(commandText, (maxRecords < 1 ? 15 : maxRecords).ToString());
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@Search", description);
                command.Parameters.AddWithValue("@SearchClean", cleanSearch);
                UseCache = true;
                CacheExpirationSeconds = 300;
                Fill(command, "QuickSearch");
            }

        }

        public static bool IsUserSubscribed(LoginUser loginUser, int userID, int ticketID)
        {
            return Subscriptions.IsUserSubscribed(loginUser, userID, ReferenceType.Tickets, ticketID);
        }

        public static int GetAssociatedOrganizationCount(LoginUser loginUser, int organizationID, int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM OrganizationTickets WHERE (TicketID = @TicketID) AND (OrganizationID = @OrganizationID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@TicketID", ticketID);

                Tickets tickets = new Tickets(loginUser);
                return (int)tickets.ExecuteScalar(command);

            }
        }

        public static int GetAssociatedContactCount(LoginUser loginUser, int organizationID, int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText =
        @"SELECT COUNT(*) FROM Users u
LEFT JOIN UserTickets ut ON ut.UserID = u.UserID
WHERE ut.TicketID = @TicketID
AND u.OrganizationID = @OrganizationID
";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@TicketID", ticketID);

                Tickets tickets = new Tickets(loginUser);
                return (int)tickets.ExecuteScalar(command);

            }
        }

        public void RemoveSubscription(int userID, int ticketID)
        {
            Subscriptions.RemoveSubscription(LoginUser, userID, ReferenceType.Tickets, ticketID);
        }

        public void AddSubscription(int userID, int ticketID)
        {
            Subscriptions.AddSubscription(LoginUser, userID, ReferenceType.Tickets, ticketID);
        }

        public static Ticket GetTicketByNumber(LoginUser loginUser, int organizationID, int ticketNumber)
        {
            Tickets tickets = new Tickets(loginUser);
            tickets.LoadByTicketNumber(organizationID, ticketNumber);
            if (tickets.IsEmpty) return null;
            else return tickets[0];
        }

        public static Ticket GetTicketByNumber(LoginUser loginUser, int ticketNumber)
        {
            return GetTicketByNumber(loginUser, loginUser.OrganizationID, ticketNumber);
        }

        public static int GetTicketActionTime(LoginUser loginUser, int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT SUM(TimeSpent) FROM Actions WHERE TicketID = @TicketID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);

                Tickets tickets = new Tickets(loginUser);
                object o = tickets.ExecuteScalar(command, "Tickets,Actions");
                if (o == DBNull.Value)
                    return 0;
                else
                    return (int)o;
            }
        }

        public static int GetUserOpenTicketCount(LoginUser loginUser, int userID, int ticketTypeID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM Tickets t LEFT JOIN TicketStatuses ts ON ts.TicketStatusID = t.TicketStatusID WHERE (t.TicketTypeID = @TicketTypeID) AND (t.UserID = @UserID) AND (ts.IsClosed = 0)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);

                Tickets tickets = new Tickets(loginUser);
                return (int)tickets.ExecuteScalar(command, "Tickets");
            }
        }

        public static int GetContactOpenTicketCount(LoginUser loginUser, int userID, int ticketTypeID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM Tickets t LEFT JOIN TicketStatuses ts ON ts.TicketStatusID = t.TicketStatusID WHERE (t.TicketTypeID = @TicketTypeID) AND (ts.IsClosed = 0) AND EXISTS(SELECT * FROM UserTickets ut WHERE (t.TicketID = ut.TicketID) AND (ut.UserID = @userID))";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@userID", userID);
                command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);

                Tickets tickets = new Tickets(loginUser);
                return (int)tickets.ExecuteScalar(command, "Tickets");
            }
        }

        public static int GetContactClosedTicketCount(LoginUser loginUser, int userID, int ticketTypeID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM Tickets t LEFT JOIN TicketStatuses ts ON ts.TicketStatusID = t.TicketStatusID WHERE (t.TicketTypeID = @TicketTypeID) AND (ts.IsClosed = 1) AND EXISTS(SELECT * FROM UserTickets ut WHERE (t.TicketID = ut.TicketID) AND (ut.UserID = @userID))";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@userID", userID);
                command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);

                Tickets tickets = new Tickets(loginUser);
                return (int)tickets.ExecuteScalar(command, "Tickets");
            }
        }

        public static int GetOrganizationOpenTicketCount(LoginUser loginUser, int organizationID, int ticketTypeID, bool includeChildren = false)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                    SELECT
                        COUNT(*)
                    FROM
                        Tickets t
                        LEFT JOIN TicketStatuses ts
                            ON ts.TicketStatusID = t.TicketStatusID
                    WHERE
                        t.TicketTypeID = @TicketTypeID
                        AND ts.IsClosed = 0
                        AND EXISTS
                        (
                            SELECT
                                *
                            FROM
                                OrganizationTickets ot
                            WHERE
                                t.TicketID = ot.TicketID
                                AND ot.OrganizationID IN
                                (
                                    SELECT
                                        @OrganizationID
                                    UNION
                                    SELECT
                                        CustomerID
                                    FROM
                                        CustomerRelationships
                                    WHERE
                                        RelatedCustomerID = @OrganizationID
                                        AND @IncludeChildren = 1
                                )
                        )";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
                command.Parameters.AddWithValue("@IncludeChildren", includeChildren);

                Tickets tickets = new Tickets(loginUser);
                return (int)tickets.ExecuteScalar(command, "Tickets");
            }
        }

        public static int GetOrganizationClosedTicketCount(LoginUser loginUser, int organizationID, int ticketTypeID, bool includeChildren = false)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                    SELECT
                        COUNT(*)
                    FROM
                        Tickets t
                        LEFT JOIN TicketStatuses ts
                            ON ts.TicketStatusID = t.TicketStatusID
                    WHERE
                        t.TicketTypeID = @TicketTypeID
                        AND ts.IsClosed = 1
                        AND EXISTS
                        (
                            SELECT
                                *
                            FROM
                                OrganizationTickets ot
                            WHERE
                                t.TicketID = ot.TicketID
                                AND ot.OrganizationID IN
                                (
                                    SELECT
                                        @OrganizationID
                                    UNION
                                    SELECT
                                        CustomerID
                                    FROM
                                        CustomerRelationships
                                    WHERE
                                        RelatedCustomerID = @OrganizationID
                                        AND @IncludeChildren = 1
                                )
                        )";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
                command.Parameters.AddWithValue("@IncludeChildren", includeChildren);

                Tickets tickets = new Tickets(loginUser);
                return (int)tickets.ExecuteScalar(command, "Tickets");
            }
        }

        public static int GetProductOpenTicketCount(LoginUser loginUser, int productID, int ticketTypeID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
        SELECT
          COUNT(*)
        FROM
          Tickets t
          LEFT JOIN TicketStatuses ts
            ON ts.TicketStatusID = t.TicketStatusID
        WHERE
          t.TicketTypeID = @TicketTypeID
          AND ts.IsClosed = 0
          AND t.ProductID = @ProductID
        ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ProductID", productID);
                command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);

                Tickets tickets = new Tickets(loginUser);
                return (int)tickets.ExecuteScalar(command, "Tickets");
            }
        }

        public static int GetProductClosedTicketCount(LoginUser loginUser, int productID, int ticketTypeID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
        SELECT
          COUNT(*)
        FROM
          Tickets t
          LEFT JOIN TicketStatuses ts
            ON ts.TicketStatusID = t.TicketStatusID
        WHERE
          t.TicketTypeID = @TicketTypeID
          AND ts.IsClosed = 1
          AND t.ProductID = @ProductID
        ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ProductID", productID);
                command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);

                Tickets tickets = new Tickets(loginUser);
                return (int)tickets.ExecuteScalar(command, "Tickets");
            }
        }

        public static int GetProductVersionOpenTicketCount(LoginUser loginUser, int productVersionID, int ticketTypeID, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
        SELECT
          COUNT(*)
        FROM
          Tickets t
          LEFT JOIN TicketStatuses ts
            ON ts.TicketStatusID = t.TicketStatusID
        WHERE
          t.TicketTypeID = @TicketTypeID
          AND ts.IsClosed = 0
          and t.organizationid = @organizationID
          AND (t.ReportedVersionID = @ProductVersionID OR t.SolvedVersionID = @ProductVersionID)
        ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
                command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
                command.Parameters.AddWithValue("@organizationID", organizationID);
                Tickets tickets = new Tickets(loginUser);
                return (int)tickets.ExecuteScalar(command, "Tickets");
            }
        }

        public static int GetProductVersionClosedTicketCount(LoginUser loginUser, int productVersionID, int ticketTypeID, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
        SELECT
          COUNT(*)
        FROM
          Tickets t
          LEFT JOIN TicketStatuses ts
            ON ts.TicketStatusID = t.TicketStatusID
        WHERE
          t.TicketTypeID = @TicketTypeID
          AND ts.IsClosed = 1
          and t.organizationid = @organizationID
          AND (t.ReportedVersionID = @ProductVersionID OR t.SolvedVersionID = @ProductVersionID)
        ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
                command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
                command.Parameters.AddWithValue("@organizationID", organizationID);
                Tickets tickets = new Tickets(loginUser);
                return (int)tickets.ExecuteScalar(command, "Tickets");
            }
        }

        public static int GetProductFamilyOpenTicketCount(LoginUser loginUser, int productFamilyID, int ticketTypeID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
        SELECT
          COUNT(*)
        FROM
          Tickets t
          JOIN TicketStatuses ts
            ON ts.TicketStatusID = t.TicketStatusID
          JOIN Products p
            ON t.ProductID = p.ProductID
        WHERE
          t.TicketTypeID = @TicketTypeID
          AND ts.IsClosed = 0
          AND p.ProductFamilyID = @ProductFamilyID
        ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ProductFamilyID", productFamilyID);
                command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);

                Tickets tickets = new Tickets(loginUser);
                return (int)tickets.ExecuteScalar(command, "Tickets");
            }
        }

        public static int GetProductFamilyClosedTicketCount(LoginUser loginUser, int productFamilyID, int ticketTypeID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
        SELECT
          COUNT(*)
        FROM
          Tickets t
          JOIN TicketStatuses ts
            ON ts.TicketStatusID = t.TicketStatusID
          JOIN Products p
            ON t.ProductID = p.ProductID
        WHERE
          t.TicketTypeID = @TicketTypeID
          AND ts.IsClosed = 1
          AND p.ProductFamilyID = @ProductFamilyID
        ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ProductFamilyID", productFamilyID);
                command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);

                Tickets tickets = new Tickets(loginUser);
                return (int)tickets.ExecuteScalar(command, "Tickets");
            }
        }

        public void LoadByGroupUnassigned(int userID, int top)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT TOP " + top.ToString() + @" tgv.* FROM TicketGridView tgv
                                WHERE (tgv.UserID is null)
                                AND (tgv.IsClosed = 0)
                                AND EXISTS(SELECT * FROM GroupUsers gu WHERE (GroupID = tgv.GroupID) AND (UserID = @UserID))
                                ORDER BY tgv.DateModified DESC";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                Fill(command, "TicketGridView");
            }
        }

        public void LoadByRecentKnowledgeBase(int organizationID, int top)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT TOP " + top.ToString() + @" tgv.* FROM TicketGridView tgv
                               WHERE (tgv.OrganizationID = @OrganizationID)
                               AND (tgv.IsKnowledgeBase = 1)
                               ORDER BY tgv.DateModified DESC";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command, "TicketGridView");
            }
        }

        public void LoadByPopularKnowledgeBase(int organizationID, int customerID, bool enableCustomerSpecificKB, bool enableCustomerProductAssociation, bool enableAnonymousProductAssociation, int top)
        {
            using (SqlCommand command = new SqlCommand())
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(@"
                        SELECT DISTINCT TOP " + top + @"
                            t.TicketID
                            ,t.Name
                            ,t.DateModified
                            ,ticketratings.VIEWS
                        FROM TicketsView AS T
                        LEFT JOIN dbo.OrganizationTickets as OT
	                        ON OT.TicketID = T.TicketID
                        LEFT JOIN dbo.OrganizationProducts as OP
	                        on OP.OrganizationID = @CustomerID
                        LEFT JOIN dbo.ticketratings ON T.TicketID = ticketratings.ticketid
                        WHERE
	                        t.OrganizationID = @OrganizationID
	                        AND t.IsKnowledgeBase = 1
	                        AND t.IsVisibleOnPortal = 1
                ");

                if (enableCustomerSpecificKB && ((customerID > 0 || enableAnonymousProductAssociation) && enableCustomerProductAssociation))
                {
                    builder.Append(@"
                            AND (
		                            --Customer Match for sure includeded no matter what
		                            OT.OrganizationID = @customerID
		                            OR
                                    (
			                            --Tickets are assigned to no organization
			                            OT.TicketID is null
			                            AND (
					                            --No product is associated to the ticket OR customer has the product in question
					                            T.ProductID is null
					                            OR OP.ProductID = T.ProductID
				                            )
		                            )
	                            )
                    ");
                }
                else if (enableCustomerSpecificKB)
                {
                    builder.Append(@"
                            AND (
		                            OT.OrganizationID = @customerID
		                            OR OT.TicketID is null
		                        )
                   ");
                }
                else if ((customerID > 0 || enableAnonymousProductAssociation) && enableCustomerProductAssociation)
                {
                    builder.Append(@"
                            AND (
					                T.ProductID is null
					                OR OP.ProductID = T.ProductID
	                            )
                    ");
                }

                builder.Append(@" ORDER BY ticketratings.VIEWS DESC");

                command.CommandText = builder.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@CustomerID", customerID);
                Fill(command);
            }
        }

        public void LoadByRecentKnowledgeBase(int organizationID, int customerID, bool enableCustomerSpecificKB, bool enableCustomerProductAssociation, bool enableAnonymousProductAssociation, int top)
        {
            using (SqlCommand command = new SqlCommand())
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(@"
                        SELECT DISTINCT TOP " + top + @"
                            t.TicketID
                            ,t.Name
                            ,t.DateModified
                        FROM TicketsView AS T
                        LEFT JOIN dbo.OrganizationTickets as OT
	                        ON OT.TicketID = T.TicketID
                        LEFT JOIN dbo.OrganizationProducts as OP
	                        on OP.OrganizationID = @CustomerID
                        WHERE
	                        t.OrganizationID = @OrganizationID
	                        AND t.IsKnowledgeBase = 1
	                        AND t.IsVisibleOnPortal = 1
                ");

                if (enableCustomerSpecificKB && ((customerID > 0 || enableAnonymousProductAssociation) && enableCustomerProductAssociation))
                {
                    builder.Append(@"
                            AND (
		                        --Customer Match for sure includeded no matter what
		                        OT.OrganizationID = @customerID
		                        OR
                                (
			                        --Tickets are assigned to no organization
			                        OT.TicketID is null
			                        AND (
					                        --No product is associated to the ticket OR customer has the product in question
					                        T.ProductID is null
					                        OR OP.ProductID = T.ProductID
				                        )
		                        )
	                        )
                    ");
                }
                else if (enableCustomerSpecificKB)
                {
                    builder.Append(@"
                            AND (
		                            OT.OrganizationID = @customerID
		                            OR OT.TicketID is null
		                        )
                   ");
                }
                else if ((customerID > 0 || enableAnonymousProductAssociation) && enableCustomerProductAssociation)
                {
                    builder.Append(@"
                            AND (
					                T.ProductID is null
					                OR OP.ProductID = T.ProductID
	                            )
                    ");
                }

                builder.Append(@" ORDER BY t.DateModified DESC");

                command.CommandText = builder.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@CustomerID", customerID);
                Fill(command);
            }
        }

        public void LoadByTicketNumber(int organizationID, int ticketNumber)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Tickets WHERE OrganizationID = @OrganizationID AND TicketNumber = @TicketNumber";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@TicketNumber", ticketNumber);
                Fill(command, "TicketGridView");
            }
        }

        public void LoadByTicketIDs(int organizationID, int[] ticketIDs)
        {
            using (SqlCommand command = new SqlCommand())
            {
                string ids = DataUtils.IntArrayToCommaString(ticketIDs);

                command.CommandText = "SELECT * FROM Tickets WHERE OrganizationID = @OrganizationID AND TicketID IN (" + ids + ")";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public void ReplaceTicketType(int oldID, int newID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE Tickets SET TicketTypeID = @newID WHERE (TicketTypeID = @oldID)";
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@oldID", oldID);
                command.Parameters.AddWithValue("@newID", newID);
                ExecuteNonQuery(command, "Tickets");
            }
        }

        public void ReplaceTicketStatus(int oldID, int newID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE Tickets SET TicketStatusID = @newID WHERE (TicketStatusID = @oldID)";
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@oldID", oldID);
                command.Parameters.AddWithValue("@newID", newID);
                ExecuteNonQuery(command, "Tickets");
            }
        }

        public void ReplaceTicketSeverity(int oldID, int newID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE Tickets SET TicketSeverityID = @newID WHERE (TicketSeverityID = @oldID)";
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@oldID", oldID);
                command.Parameters.AddWithValue("@newID", newID);
                ExecuteNonQuery(command, "Tickets");
            }
        }

		[Obsolete("This is not needed anymore in neither APP or the services, please don't use. Moved to SlaProcessor.cs which is the only place that should use it.")]
        public void LoadAllUnnotifiedAndExpiredSla()
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText =
        @"
SELECT ticketnumber, t.*, sn.*
FROM Tickets t
LEFT JOIN SlaNotifications sn ON t.TicketID = sn.TicketID
WHERE
t.DateClosed IS NULL
AND
(
  (
    t.SlaViolationTimeClosed IS NOT NULL AND
    t.SlaViolationTimeClosed < DATEADD(DAY, 1, GETUTCDATE()) AND
	t.SlaViolationTimeClosed > DATEADD(DAY, -1, GETUTCDATE()) AND
    t.SlaViolationTimeClosed > DATEADD(MINUTE, 10, ISNULL(sn.TimeClosedViolationDate, '1/1/1980'))
  )
  OR
  (
    t.SlaWarningTimeClosed IS NOT NULL AND
    t.SlaWarningTimeClosed < DATEADD(DAY, 1, GETUTCDATE()) AND
	t.SlaWarningTimeClosed > DATEADD(DAY, -1, GETUTCDATE()) AND
    t.SlaWarningTimeClosed > DATEADD(MINUTE, 10, ISNULL(sn.TimeClosedWarningDate, '1/1/1980'))
  )
  OR
  (
    t.SlaViolationLastAction IS NOT NULL AND
    t.SlaViolationLastAction < DATEADD(DAY, 1, GETUTCDATE()) AND
	t.SlaViolationLastAction > DATEADD(DAY, -1, GETUTCDATE()) AND
    t.SlaViolationLastAction > DATEADD(MINUTE, 10, ISNULL(sn.LastActionViolationDate, '1/1/1980'))
  )
  OR
  (
    t.SlaWarningLastAction IS NOT NULL AND
    t.SlaWarningLastAction < DATEADD(DAY, 1, GETUTCDATE()) AND
	t.SlaWarningLastAction > DATEADD(DAY, -1, GETUTCDATE()) AND
    t.SlaWarningLastAction > DATEADD(MINUTE, 10, ISNULL(sn.LastActionWarningDate, '1/1/1980'))
  )
  OR
  (
    t.SlaViolationInitialResponse IS NOT NULL AND
    t.SlaViolationInitialResponse < DATEADD(DAY, 1, GETUTCDATE()) AND
	t.SlaViolationInitialResponse > DATEADD(DAY, -1, GETUTCDATE()) AND
    t.SlaViolationInitialResponse > DATEADD(MINUTE, 10, ISNULL(sn.InitialResponseViolationDate, '1/1/1980'))
  )
  OR
  (
    t.SlaWarningInitialResponse IS NOT NULL AND
    t.SlaWarningInitialResponse < DATEADD(DAY, 1, GETUTCDATE()) AND
	t.SlaWarningInitialResponse > DATEADD(DAY, -1, GETUTCDATE()) AND
    t.SlaWarningInitialResponse > DATEADD(MINUTE, 10, ISNULL(sn.InitialResponseWarningDate, '1/1/1980'))
  )
)
";

                command.CommandType = CommandType.Text;
                Fill(command);
            }
        }


        public void AddTags(Tag tag, int ticketID)
        {
            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, ticketID);
            string description = "Added '" + tag.Value + "' to the tag list for " + GetTicketLink(ticket);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tickets, ticketID, description);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tags, tag.TagID, description);

        }

        public void RemoveTags(Tag tag, int ticketID)
        {
            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, ticketID);
            string description = "Removed '" + tag.Value + "' from the tag list for " + GetTicketLink(ticket);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Tickets, ticketID, description);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Tags, tag.TagID, description);

        }

        public void LoadBySalesForceID(string salesForceID, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Tickets WHERE SalesForceID = @SalesForceID AND OrganizationID = @OrganizationID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@SalesForceID", salesForceID);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public void GetUsersTickets(int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM userstickets where ticketid = @ticketID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ticketID", ticketID);
                Fill(command);
            }
        }

        public void MergeUpdateContact(int oldticketID, int newticketID)
        {

            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = @"
            BEGIN transaction

            DELETE FROM UserTickets
            WHERE TicketID = @oldticketID AND UserID IN(SELECT UserID FROM UserTickets WHERE TicketID=@newticketID);

            UPDATE UserTickets
            SET TicketID = @newticketID
            WHERE TicketID = @oldticketID;

            /*If no error run*/
            COMMIT transaction
            /*If error run*/
            ROLLBACK transaction
            ";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@newticketID", newticketID);
                    command.Parameters.AddWithValue("@oldticketID", oldticketID);
                    ExecuteNonQuery(command, "UserTickets");
                }
            }
            catch (Exception e)
            {

            }

            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
            string description = "Merged '" + ticket.TicketNumber + "' Customers";
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        }

        public void MergeUpdateOrganizations(int oldticketID, int newticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
            BEGIN transaction

            DELETE FROM OrganizationTickets
            WHERE TicketID = @oldticketID AND OrganizationID IN(SELECT OrganizationID FROM OrganizationTickets WHERE TicketID=@newticketID);

            UPDATE OrganizationTickets
            SET TicketID = @newticketID
            WHERE TicketID = @oldticketID;

            /*If no error run*/
            COMMIT transaction
            /*If error run*/
            ROLLBACK transaction
            ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@newticketID", newticketID);
                command.Parameters.AddWithValue("@oldticketID", oldticketID);
                ExecuteNonQuery(command, "OrganizationTickets");
            }

            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
            string description = "Merged '" + ticket.TicketNumber + "' Organizations";
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        }

        public void MergeUpdateTags(int oldticketID, int newticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE Taglinks SET RefID=@newticketID WHERE (RefID = @oldticketID) AND RefType = @refType";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@newticketID", newticketID);
                command.Parameters.AddWithValue("@oldticketID", oldticketID);
                command.Parameters.AddWithValue("@refType", ReferenceType.Tickets);
                ExecuteNonQuery(command, "Taglinks");
            }

            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
            string description = "Merged '" + ticket.TicketNumber + "' Tags";
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        }

        public void MergeUpdateCustomFields(int oldticketID, int newticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE CustomValues SET RefID=@newticketID, OrganizationID=@organizationID WHERE (RefID = @oldticketID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@newticketID", newticketID);
                command.Parameters.AddWithValue("@oldticketID", oldticketID);
				command.Parameters.AddWithValue("@organizationID", LoginUser.OrganizationID);
				ExecuteNonQuery(command, "CustomValues");
            }

            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
            string description = "Merged '" + ticket.TicketNumber + "' Tags";
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        }

        public void MergeUpdateSubscribers(int oldticketID, int newticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE Subscriptions SET RefID=@newticketID WHERE (RefID = @oldticketID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@newticketID", newticketID);
                command.Parameters.AddWithValue("@oldticketID", oldticketID);
                ExecuteNonQuery(command, "Subscriptions");
            }

            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
            string description = "Merged '" + ticket.TicketNumber + "' Subscribers";
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        }

        public void MergeUpdateQueuers(int oldticketID, int newticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE TicketQueue SET TicketID=@newticketID WHERE (TicketID = @oldticketID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@newticketID", newticketID);
                command.Parameters.AddWithValue("@oldticketID", oldticketID);
                ExecuteNonQuery(command, "TicketQueue");
            }

            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
            string description = "Merged '" + ticket.TicketNumber + "' Queuers";
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        }

        public void MergeUpdateReminders(int oldticketID, int newticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                UPDATE
                    Reminders
                SET
                    RefID = @newticketID
                WHERE
                    RefID = @oldticketID
                    AND RefType = @reftype

                UPDATE
                    TaskAssociations
                SET
                    RefID = @newticketID
                WHERE
                    RefID = @oldticketID
                    AND RefType = @reftype";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@newticketID", newticketID);
                command.Parameters.AddWithValue("@oldticketID", oldticketID);
                command.Parameters.AddWithValue("@reftype", ReferenceType.Tickets);
                ExecuteNonQuery(command, "Reminders");
            }

            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
            string description = "Merged '" + ticket.TicketNumber + "' Reminders";
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        }

        public void MergeUpdateAssets(int oldticketID, int newticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE AssetTickets SET TicketID=@newticketID WHERE (TicketID = @oldticketID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@newticketID", newticketID);
                command.Parameters.AddWithValue("@oldticketID", oldticketID);
                ExecuteNonQuery(command, "AssetTickets");
            }

            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
            string description = "Merged '" + ticket.TicketNumber + "' Assets";
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        }

        public void MergeUpdateActions(int oldticketID, int newticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE Actions WITH (ROWLOCK) SET TicketID=@newticketID WHERE (TicketID = @oldticketID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@newticketID", newticketID);
                command.Parameters.AddWithValue("@oldticketID", oldticketID);
                ExecuteNonQuery(command, "Actions");
            }

            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
            string description = "Merged '" + ticket.TicketNumber + "' Actions";
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        }

        public void MergeUpdateRelationships(int oldticketID, int newticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE TicketRelationships SET Ticket1ID=@newticketID WHERE (Ticket1ID = @oldticketID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@newticketID", newticketID);
                command.Parameters.AddWithValue("@oldticketID", oldticketID);
                ExecuteNonQuery(command, "TicketRelationships");
            }
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE TicketRelationships SET Ticket2ID=@newticketID WHERE (Ticket2ID = @oldticketID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@newticketID", newticketID);
                command.Parameters.AddWithValue("@oldticketID", oldticketID);
                ExecuteNonQuery(command, "TicketRelationships");
            }
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE Tickets WITH (ROWLOCK) SET ParentID=@newticketID WHERE (ParentID = @oldticketID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@newticketID", newticketID);
                command.Parameters.AddWithValue("@oldticketID", oldticketID);
                ExecuteNonQuery(command, "Tickets");
            }
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE Tickets WITH (ROWLOCK) SET ParentID=null WHERE (ParentID = ticketID) and (ParentID = @newticketID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@newticketID", newticketID);
                ExecuteNonQuery(command, "Tickets");
            }

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "DELETE FROM TicketRelationships WHERE (Ticket1ID = @newticketID AND Ticket2ID = @newticketID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@newticketID", newticketID);
                ExecuteNonQuery(command, "TicketRelationships");
            }
            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
            string description = "Merged '" + ticket.TicketNumber + "' Relationships";
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        }

        public void MergeAttachments(int oldticketID, int newticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE attachments SET RefID=@newticketID WHERE (RefID = @oldticketID) AND RefType = @refType";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@newticketID", newticketID);
                command.Parameters.AddWithValue("@oldticketID", oldticketID);
                command.Parameters.AddWithValue("@refType", ReferenceType.Actions);
                ExecuteNonQuery(command, "attachments");
            }

            Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
            string description = "Merged '" + ticket.TicketNumber + "' Action Attachments";
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        }
        
        public void LoadFirstJiraSynced(int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT TOP 1 t.* FROM Tickets t JOIN TicketLinkToJira j ON t.TicketID = j.TicketID WHERE t.OrganizationID = @OrganizationID ORDER BY t.DateCreated";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public void LoadFirstTFSSynced(int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                    SELECT
                        TOP 1 t.*
                    FROM
                        Tickets t
                        JOIN TicketLinkToTFS tfs
                            ON t.TicketID = tfs.TicketID
                    WHERE
                        t.OrganizationID = @OrganizationID
                    ORDER BY
                        t.DateCreated";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public int GetProductVersionTicketCount(int productVersionID, int closed, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
        SELECT
          COUNT(*)
        FROM
          Tickets t
          JOIN TicketStatuses ts
            ON t.TicketStatusID = ts.TicketStatusID
        WHERE
          ts.IsClosed = @closed
          and t.organizationid = @organizationID
          AND (t.ReportedVersionID = @ProductVersionID OR t.SolvedVersionID = @ProductVersionID)
        ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
                command.Parameters.AddWithValue("@closed", closed);
                command.Parameters.AddWithValue("@organizationID", organizationID);
                object o = ExecuteScalar(command);
                if (o == null || o == DBNull.Value) return 0;
                return (int)o;
            }
        }

        public int GetProductTicketCount(int productID, int closed)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
        SELECT
          COUNT(*)
        FROM
          Tickets t
          JOIN TicketStatuses ts
            ON t.TicketStatusID = ts.TicketStatusID
        WHERE
          ts.IsClosed = @closed
          AND t.ProductID = @ProductID
        ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ProductID", productID);
                command.Parameters.AddWithValue("@closed", closed);
                object o = ExecuteScalar(command);
                if (o == null || o == DBNull.Value) return 0;
                return (int)o;
            }
        }

        public void LoadByImportID(string importID, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Tickets WHERE ImportID = @ImportID AND OrganizationID = @OrganizationID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ImportID", importID);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public void LoadbyCompany(int companyID, int orgID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
			 SELECT
				t.*
			FROM
				Tickets t
				JOIN OrganizationTickets ot
			WHERE
				t.OrganizationID = @OrgID
				AND ot.OrganizationID = @companyID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@companyID", companyID);
                command.Parameters.AddWithValue("@OrgID", orgID);
                Fill(command);
            }
        }

        public static void UpdateTicketSla(LoginUser loginUser,
                                            int TicketId,
                                            DateTime? SlaViolationInitialResponse,
                                            DateTime? SlaViolationLastAction,
                                            DateTime? SlaViolationTimeClosed,
                                            DateTime? SlaWarningInitialResponse,
                                            DateTime? SlaWarningLastAction,
                                            DateTime? SlaWarningTimeClosed)
        {
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE [dbo].[Tickets]
    SET
        SlaViolationInitialResponse = @SlaViolationInitialResponse,
        SlaViolationLastAction = @SlaViolationLastAction,
        SlaViolationTimeClosed = @SlaViolationTimeClosed,
        SlaWarningInitialResponse = @SlaWarningInitialResponse,
        SlaWarningLastAction = @SlaWarningLastAction,
        SlaWarningTimeClosed = @SlaWarningTimeClosed
    WHERE TicketID = @TicketId";
                    command.Parameters.AddWithValue("@TicketId", TicketId);
                    command.Parameters.AddWithValue("@SlaViolationInitialResponse", (object)SlaViolationInitialResponse ?? DBNull.Value);
                    command.Parameters.AddWithValue("@SlaViolationLastAction", (object)SlaViolationLastAction ?? DBNull.Value);
                    command.Parameters.AddWithValue("@SlaViolationTimeClosed", (object)SlaViolationTimeClosed ?? DBNull.Value);
                    command.Parameters.AddWithValue("@SlaWarningInitialResponse", (object)SlaWarningInitialResponse ?? DBNull.Value);
                    command.Parameters.AddWithValue("@SlaWarningLastAction", (object)SlaWarningLastAction ?? DBNull.Value);
                    command.Parameters.AddWithValue("@SlaWarningTimeClosed", (object)SlaWarningTimeClosed ?? DBNull.Value);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
    }
}
