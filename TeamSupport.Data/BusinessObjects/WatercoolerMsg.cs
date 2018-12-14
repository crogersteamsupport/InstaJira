using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class WatercoolerMsgItem
  {
  }
  
  public partial class WatercoolerMsg
  {
      public void LoadTop10Threads()
      {
          using (SqlCommand command = new SqlCommand())
          {
              // This query isn't right just a sample to pull the top 25.
              command.CommandText = @"SELECT TOP 10 * FROM WaterCoolerView as wcm left join watercoolerattachments as wcg on wcm.messageid = wcg.AttachmentID where wcm.messageparent = -1 and isdeleted=0 and OrganizationID = @OrganizationID and (isnull(wcg.AttachmentID,0) = 0 or isnull(wcg.AttachmentID,0) in (select groupid from groupusers where userid = @UserID)) ORDER BY LastModified DESC";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
              Fill(command);
          }
      }

      public void LoadMoreThreads(int msgcount)
      {
          using (SqlCommand command = new SqlCommand())
          {
              // This query isn't right just a sample to pull the top 25.
              //command.CommandText = @"WITH threads AS(SELECT   *, ROW_NUMBER() OVER (ORDER BY TimeStamp desc) AS rowid FROM WaterCoolerView)select rowid, MessageID, UserID, OrganizationID, TimeStamp, Message, MessageParent, IsDeleted, Avatar, UserName from threads WHERE OrganizationID = @OrganizationID AND MessageParent = -1 and rowid BETWEEN @start AND @end ORDER BY TimeStamp DESC";
              command.CommandText = @"WITH threads AS(SELECT wcv.MessageID, wcv.UserID, OrganizationID, TimeStamp, Message, MessageParent, IsDeleted, Avatar, UserName, LastModified, ROW_NUMBER() OVER (ORDER BY TimeStamp desc) AS rowid FROM WaterCoolerView as wcv 
                                    left join WatercoolerAttachments as wcg on wcv.messageid = wcg.messageid WHERE RefType = 4 and OrganizationID = @OrganizationID AND MessageParent = -1 and isdeleted=0 and isnull(wcg.AttachmentID,0) = 0 or isnull(wcg.AttachmentID,0) in (select groupid from groupusers where userid = @UserID))
                                    select rowid, threads.MessageID, threads.UserID, OrganizationID, TimeStamp, Message, MessageParent, IsDeleted, Avatar, UserName from threads left join WatercoolerAttachments as wcg on threads.messageid = wcg.messageid WHERE OrganizationID = @OrganizationID AND MessageParent = -1 and isdeleted=0 and rowid BETWEEN @start AND @end and isnull(wcg.AttachmentID,0) = 0 or isnull(wcg.AttachmentID,0) in (select groupid from groupusers where userid = @UserID) ORDER BY LastModified DESC";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
              command.Parameters.AddWithValue("@start", msgcount + 1);
              command.Parameters.AddWithValue("@end", msgcount + 5);
              Fill(command);
          }
      }

      public void LoadUpdatedThreads(int msgcount, int pausetime)
      {
          if (msgcount == 0)
              msgcount += 10;
          if (pausetime < 10)
              pausetime = 10;
          using (SqlCommand command = new SqlCommand())
          {
              // This query isn't right just a sample to pull the top 25.
              //command.CommandText = @"WITH threads AS(SELECT   *, ROW_NUMBER() OVER (ORDER BY TimeStamp desc) AS rowid FROM WaterCoolerView)select rowid, MessageID, UserID, OrganizationID, TimeStamp, Message, MessageParent, IsDeleted, Avatar, UserName from threads WHERE OrganizationID = @OrganizationID AND MessageParent = -1 and rowid BETWEEN @start AND @end ORDER BY TimeStamp DESC";
              command.CommandText = @"WITH threads AS(
                                    SELECT wcv.MessageID, wcv.UserID, OrganizationID, TimeStamp, Message, MessageParent, IsDeleted, Avatar, UserName, lastmodified, LastModifiedUserID, ROW_NUMBER() OVER (ORDER BY TimeStamp desc) AS rowid FROM WaterCoolerView as wcv left join watercoolergroupatt as wcg on wcv.messageid = wcg.messageid
                                    WHERE organizationid = @OrganizationID and MessageParent = -1 and DATEDIFF(second,wcv.LastModified,GETUTCDATE()) <= @timedelay and isdeleted=0 and isnull(wcg.groupid,0) = 0 or isnull(wcg.groupid,0) in (select groupid from groupusers where userid = @UserID))
                                    select rowid, threads.MessageID, threads.UserID, OrganizationID, TimeStamp, Message, MessageParent, IsDeleted, Avatar, UserName from threads where DATEDIFF(second,threads.LastModified,GETUTCDATE()) <= @timedelay
                                    ORDER BY LastModified asc";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
              command.Parameters.AddWithValue("@start", 0);
              command.Parameters.AddWithValue("@end", msgcount);
              command.Parameters.AddWithValue("@timedelay", pausetime + 2);

              Fill(command);
          }
      }

      public void LoadMessage(int messageID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              // This query isn't right just a sample to pull the top 25.
              command.CommandText = @"SELECT * FROM WaterCoolerView WHERE OrganizationID = @OrganizationID AND MessageID = @MsgID";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("@MsgID", messageID);
              Fill(command);
          }
      }

      public void LoadReplies(int messageID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = @"SELECT * FROM WaterCoolerView WHERE MessageParent = @ReplyTo AND isdeleted=0 AND OrganizationID = @OrganizationID ORDER BY TimeStamp ASC";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("@ReplyTo", messageID);
              Fill(command);
          }
      }

      public void LoadForIndexing(int organizationID, int max)
      {
        using (SqlCommand command = new SqlCommand())
        {
          string text = @"
            SELECT 
              TOP {0} 
              MessageID
            FROM 
              WatercoolerMsg wcm WITH(NOLOCK)
            WHERE 
              wcm.NeedsIndexing = 1
              AND wcm.OrganizationID = @OrganizationID
            ORDER BY 
              wcm.LastModified DESC";

          command.CommandText = string.Format(text, max.ToString());
          command.CommandType = CommandType.Text;
          command.Parameters.AddWithValue("@OrganizationID", organizationID);
          Fill(command);
        }        
      }
  }
  
}
