using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace TeamSupport.Data
{
  partial class Attachment 
  {
    public string CreatorName
    {
      get
      {
        if (Row.Table.Columns.Contains("CreatorName") && Row["CreatorName"] != DBNull.Value)
        {
          return (string)Row["CreatorName"];
        }
        else return "";
      }
    }

    public string ProductFamily
    {
      get
      {
        if (Row.Table.Columns.Contains("ProductFamily") && Row["ProductFamily"] != DBNull.Value)
        {
          return (string)Row["ProductFamily"];
        }
        else return "";
      }
    }

    public void DeleteFile()
    {
      try { File.Delete(Path); }
      catch (Exception) { }
    
    }

        public static void CreateActionAttachment(AttachmentProxy attachmentProxy)
        {
            Attachment attachment = (new Attachments(new LoginUser(attachmentProxy.CreatorID, attachmentProxy.OrganizationID))).AddNewAttachment();
            attachment.RefType = AttachmentProxy.References.Actions;
            attachment.RefID = ((ActionAttachmentProxy)attachmentProxy).ActionID;
            attachment.OrganizationID = attachmentProxy.OrganizationID;
            attachment.FileName = attachmentProxy.FileName;
            attachment.Path = attachmentProxy.Path;
            attachment.FileType = attachmentProxy.FileType;
            attachment.FileSize = attachmentProxy.FileSize;
            attachment.FilePathID = 3;  // ActionModel.ActionPathIndex
            attachment.Description = attachmentProxy.Description;
            attachment.ProductFamilyID = attachmentProxy.ProductFamilyID;
            attachment.Collection.Save();
        }
    }

    internal partial class Attachments
  {

    public void LoadByAttachmentGUID(Guid attachmentGUID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT a.* FROM Attachments a WHERE AttachmentGUID = @attachmentGUID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@attachmentGUID", attachmentGUID);
        Fill(command);
      }

    }

    public static Attachment GetAttachment(LoginUser loginUser, Guid attachmentGUID)
    {
      Attachments attachments = new Attachments(loginUser);
      attachments.LoadByAttachmentGUID(attachmentGUID);
      if (attachments.IsEmpty)
        return null;
      else
        return attachments[0];
    }

    public void LoadByActionID(int actionID, string orderBy = "")
    {
      LoadByReference((ReferenceType)ReferenceType.Actions, actionID, orderBy);
    }

    public void LoadByWatercoolerID(int WaterCoolerID)
    {
        LoadByReference(ReferenceType.WaterCooler, WaterCoolerID);
    }

    public void LoadByOrganizationID(int organizationID)
    {
      LoadByReference(ReferenceType.Organizations, organizationID);
    }

    public void LoadByReference(ReferenceType refType, int refID, string orderBy = "", bool includeCompanyChildren = false)
    {
      using (SqlCommand command = new SqlCommand())
      {
                command.CommandType = CommandType.Text;

                if (includeCompanyChildren)
                {
                    command.CommandText = $@"
            SELECT 
                a.*
                , (u.FirstName + ' ' + u.LastName) AS CreatorName 
            FROM 
                Attachments a 
                LEFT JOIN Users u 
                    ON u.UserID = a.CreatorID 
            WHERE 
                RefType = {(int)refType}
                AND RefID IN
                (
                    SELECT
                        @RefID
                    UNION
                    SELECT
                        CustomerID
                    FROM
                        CustomerRelationships
                    WHERE
                        RelatedCustomerID = @RefID
                        AND @IncludeCompanyChildren = 1
                )";

                    command.Parameters.AddWithValue("@IncludeCompanyChildren", includeCompanyChildren);
                }
                else
                {
                    command.CommandText = $"SELECT a.*, (u.FirstName + ' ' + u.LastName) AS CreatorName FROM Attachments a LEFT JOIN Users u ON u.UserID = a.CreatorID WHERE (RefID = @RefID) AND (RefType = {(int)refType})";

                }
        
        if (orderBy != string.Empty)
        {
          command.CommandText += " ORDER BY " + orderBy;
        }
        
        command.Parameters.AddWithValue("@RefID", refID);
        
       
        Fill(command);
      }
    }

	  public void LoadByTicketId(int ticketId)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"SELECT * FROM Attachments WHERE RefID IN (SELECT ActionID FROM Actions WHERE TicketID = @ticketId) order by datecreated desc";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@ticketId", ticketId);
				Fill(command);
			}
		}

    public void LoadByReferenceAndUserRights(ReferenceType refType, int refID, int viewerID, string orderBy = "", bool includeCompanyChildren = false)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
            SELECT 
                a.*
                , (u.FirstName + ' ' + u.LastName) AS CreatorName 
                , f.Name AS ProductFamily
            FROM 
                Attachments a 
                LEFT JOIN Users u 
                    ON u.UserID = a.CreatorID 
                LEFT JOIN ProductFamilies f
                    ON a.ProductFamilyID = f.ProductFamilyID
            WHERE 
                RefType = @RefType
                AND RefID IN
                (
                    SELECT
                        @RefID
                    UNION
                    SELECT
                        CustomerID
                    FROM
                        CustomerRelationships
                    WHERE
                        RelatedCustomerID = @RefID
                        AND @IncludeCompanyChildren = 1
                )
                AND
                (
                    EXISTS (SELECT UserID FROM Users WHERE UserID = @ViewerID AND ProductFamiliesRights = 0)
                    OR a.ProductFamilyID IS NULL
                    OR a.ProductFamilyID IN
                    (
                        SELECT
                            urpf.ProductFamilyID
                        FROM
                            UserRightsProductFamilies urpf
                        WHERE
                            urpf.UserID = @ViewerID
                    )
                )";
        if (orderBy != string.Empty)
        {
          command.CommandText += " ORDER BY " + orderBy;
        }
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefID", refID);
        command.Parameters.AddWithValue("@RefType", refType);
        command.Parameters.AddWithValue("@IncludeCompanyChildren", includeCompanyChildren);
        command.Parameters.AddWithValue("@ViewerID", viewerID);
        Fill(command);
      }
    }

    public void LoadForJira(int actionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT a.*, (u.FirstName + ' ' + u.LastName) AS CreatorName FROM Attachments a LEFT JOIN Users u ON u.UserID = a.CreatorID WHERE (RefID = @RefID) AND (RefType = 0) AND SentToJira = 0";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefID", actionID);
        Fill(command);
      }
    }

    public void LoadForTFS(int actionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
            SELECT
                a.*, 
                (u.FirstName + ' ' + u.LastName) AS CreatorName 
            FROM
                Attachments a 
            LEFT JOIN Users u 
                ON u.UserID = a.CreatorID
            WHERE
                RefID = @RefID
                AND RefType = 0 
                AND SentToTFS = 0";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefID", actionID);
        Fill(command);
      }
    }

		public void LoadForIntegration(int actionID, IntegrationType integration)
		{
			using (SqlCommand command = new SqlCommand())
			{
				string commandSql = string.Empty;
				commandSql = @"
            SELECT
                a.*, 
                (u.FirstName + ' ' + u.LastName) AS CreatorName 
            FROM
                Attachments a 
            LEFT JOIN Users u 
                ON u.UserID = a.CreatorID
            WHERE
                RefID = @RefID
                AND RefType = 0 
				{0}";

				string integrationFilter = string.Empty;

				switch (integration)
				{
					case IntegrationType.Jira:
						integrationFilter = "AND SentToJira = 0";
						break;
					case IntegrationType.TFS:
						integrationFilter = "AND SentToTFS = 0";
						break;
					case IntegrationType.ServiceNow:
						integrationFilter = "AND SentToSnow = 0";
						break;
					default:
						break;
				}

				commandSql = string.Format(commandSql, integrationFilter);
				command.CommandText = commandSql;
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@RefID", actionID);
				Fill(command);
			}
		}

		public void TempLoadFix()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
select a.* from attachments a 
left join organizations o on o.OrganizationID=a.OrganizationID
where a.RefType=0 
--and o.OrganizationID in (305383, 421709 )
and a.DateCreated > '2012-04-24 13:03:25.980'
and ISNULL(a.path, '') = ''
--and FileType not like '%image%'
order by o.Name, a.DateCreated desc
";

        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public static void DeleteAttachmentAndFile(LoginUser loginUser, int attachmentID)
    {
      Attachment attachment = GetAttachment(loginUser, attachmentID);
      if (attachment != null)
      {
        try { File.Delete(attachment.Path); } catch (Exception) {  }
        attachment.Delete();
        attachment.Collection.Save();
      }
    }

    public static string GetAttachmentPath(LoginUser loginUser, AttachmentProxy.References refType, int refID)
    {
      string root = AttachmentPath.GetRoot(loginUser, loginUser.OrganizationID);
      string type = null;
      switch (refType)
      {
        case AttachmentProxy.References.None:
          break;
        case AttachmentProxy.References.Actions: type = "Actions"; break;
        case AttachmentProxy.References.Organizations: type = "OrganizationAttachments"; break;
        case AttachmentProxy.References.ProductVersions:
          break;
        case AttachmentProxy.References.Users:
          break;
        case AttachmentProxy.References.Contacts:
          break;
        case AttachmentProxy.References.UserPhoto:
          type = "UserPhoto";
          break;
        default:
          break;
      }

      if (type == null) throw new Exception("Error: Cannot convert reference type to string.");

      return Path.Combine(Path.Combine(root, type), refID.ToString()) + "\\";
    }

    public static string GetAttachmentPath(LoginUser loginUser, AttachmentProxy.References refType, int refID, int filePathID)
    {
      string root = AttachmentPath.GetRoot(loginUser, loginUser.OrganizationID, filePathID);
      string type = null;
      switch (refType)
      {
        case AttachmentProxy.References.None:
          break;
        case AttachmentProxy.References.Actions: type = "Actions"; break;
        case AttachmentProxy.References.Organizations: type = "OrganizationAttachments"; break;
        case AttachmentProxy.References.ProductVersions:
          break;
        case AttachmentProxy.References.Users:
          break;
        case AttachmentProxy.References.Contacts:
          break;
        case AttachmentProxy.References.UserPhoto:
          type = "UserPhoto";
          break;
        default:
          break;
      }

      if (type == null) throw new Exception("Error: Cannot convert reference type to string.");

      return Path.Combine(Path.Combine(root, type), refID.ToString()) + "\\";
    }

    public static string VerifyUniqueFileName(string directory, string fileName)
    {
      string path = Path.Combine(directory, fileName);
      string result = fileName;
      int i = 0;
      while (File.Exists(path))
      {
        i++;
        if (i > 20) break;
        string name = Path.GetFileNameWithoutExtension(fileName);
        string ext = Path.GetExtension(fileName);
        result = name + " (" + i.ToString() + ")" + ext;
        path = Path.Combine(directory, result);
      }

      return result;
    }
    
    public void LoadKBByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
            SELECT 
                at.*
            FROM 
                Attachments at 
                JOIN Actions ac 
                    ON at.RefID = ac.ActionID 
            WHERE 
                at.RefType = 0
                AND ac.TicketID = @TicketID
                AND ac.IsKnowledgeBase = 1
            ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public string GetText()
    {
        if (this.Count > 0)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                result.Append(this[i].FileName);
                if (i < this.Count - 1)
                {
                    result.Append(", ");
                }
                else
                {
                    result.Append(".");
                }
            }
            return result.ToString();
        }
        else
        {
            return "[None]";
        }
    }

  }
}
