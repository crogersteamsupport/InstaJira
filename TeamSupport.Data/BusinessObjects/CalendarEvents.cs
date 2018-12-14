using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CalendarEvent
  {
  }
  
  public partial class CalendarEvents
  {
      public void LoadbyMonth(DateTime date, int orgID, string Type, string ID, int userID)
      {

          using (SqlCommand command = new SqlCommand())
          {
              //command.CommandText = "SELECT * from CalendarEvents WHERE (Month(StartDate) = @month) AND (Year(StartDate) = @year) AND (OrganizationID = @orgID)" + additional;
              //command.CommandText = "SELECT * FROM CalendarEvents WHERE CalendarID IN ( ( SELECT cv.CalendarID FROM CalendarView cv WHERE cv.OrganizationID=@OrgID AND cv.CreatorID = @UserID and (cv.RefID=@AttID and cv.RefType=@PageID or (@PageID=-1 and @AttID=-1)) ) UNION ( SELECT cv.CalendarID FROM CalendarView cv WHERE cv.OrganizationID=@OrgID AND (SELECT COUNT(*) FROM CalendarRef cr WHERE cr.CalendarID = cv.CalendarID AND RefType=3) < 1 AND (SELECT COUNT(*) FROM CalendarRef cr WHERE cr.CalendarID = cv.CalendarID AND RefType=4) < 1 and (cv.RefID=@AttID and cv.RefType=@PageID or (@PageID=-1 and @AttID=-1)) ) UNION ( SELECT cv.CalendarID FROM CalendarView cv WHERE cv.OrganizationID=@OrgID AND (SELECT COUNT(*) FROM CalendarRef cr WHERE cr.CalendarID = cv.CalendarID AND RefType=3 AND RefID=@UserID) > 0 AND (cv.RefID=@AttID and cv.RefType=@PageID or (@PageID=-1 and @AttID=-1)) ) UNION ( SELECT cv.CalendarID FROM CalendarView cv WHERE cv.OrganizationID=@OrgID AND (SELECT COUNT(*) FROM CalendarRef cr WHERE cr.CalendarID = cv.CalendarID AND cr.RefType=4 AND cr.RefID IN ( SELECT gu.GroupID FROM GroupUsers gu WHERE gu.UserID=@UserID ) ) > 0 AND (cv.RefID=@AttID and cv.RefType=@PageID or (@PageID=-1 and @AttID=-1)) ) )";
              command.CommandText = "SELECT * FROM CalendarEvents WHERE CalendarID IN ( ( SELECT cv.CalendarID FROM CalendarView cv WHERE cv.OrganizationID=@OrgID AND cv.CreatorID = @UserID and (Month(StartDate) = @month or Month(EndDate) = @month) AND (Year(StartDate) = @year) and (cv.RefID=@AttID and cv.RefType=@PageID or (@PageID=-1 and @AttID=-1)) ) UNION ( SELECT cv.CalendarID FROM CalendarView cv WHERE cv.OrganizationID=@OrgID AND (SELECT COUNT(*) FROM CalendarRef cr WHERE cr.CalendarID = cv.CalendarID AND RefType=3) < 1 AND (SELECT COUNT(*) FROM CalendarRef cr WHERE cr.CalendarID = cv.CalendarID AND RefType=4) < 1 and (Month(StartDate) = @month  or Month(EndDate) = @month) AND (Year(StartDate) = @year) and (cv.RefID=@AttID and cv.RefType=@PageID or (@PageID=-1 and @AttID=-1)) ) UNION ( SELECT cv.CalendarID FROM CalendarView cv WHERE cv.OrganizationID=@OrgID AND (SELECT COUNT(*) FROM CalendarRef cr WHERE cr.CalendarID = cv.CalendarID AND RefType=3 AND RefID=@UserID) > 0 and (Month(StartDate) = @month  or Month(EndDate) = @month) AND (Year(StartDate) = @year) AND (cv.RefID=@AttID and cv.RefType=@PageID or (@PageID=-1 and @AttID=-1)) ) UNION ( SELECT cv.CalendarID FROM CalendarView cv WHERE cv.OrganizationID=@OrgID AND (SELECT COUNT(*) FROM CalendarRef cr WHERE cr.CalendarID = cv.CalendarID AND cr.RefType=4 AND cr.RefID IN ( SELECT gu.GroupID FROM GroupUsers gu WHERE gu.UserID=@UserID ) ) > 0 and (Month(StartDate) = @month  or Month(EndDate) = @month) AND (Year(StartDate) = @year) AND (cv.RefID=@AttID and cv.RefType=@PageID or (@PageID=-1 and @AttID=-1)) ) )";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@month", date.Month);
              command.Parameters.AddWithValue("@year", date.Year);
              command.Parameters.AddWithValue("@OrgID", orgID);
              command.Parameters.AddWithValue("@UserID", userID);
              command.Parameters.AddWithValue("@AttID", ID);
              command.Parameters.AddWithValue("@PageID", Type);
              Fill(command);
          }
      }

      public void LoadAll(int orgID, int userID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              //command.CommandText = "SELECT * from CalendarEvents WHERE (Month(StartDate) = @month) AND (Year(StartDate) = @year) AND (OrganizationID = @orgID)" + additional;
              command.CommandText = "SELECT * FROM CalendarEvents WHERE CalendarID IN ( ( SELECT cv.CalendarID FROM CalendarView cv WHERE cv.OrganizationID=@OrgID AND cv.CreatorID = @UserID and (cv.RefID=@AttID and cv.RefType=@PageID or (@PageID=-1 and @AttID=-1)) ) UNION ( SELECT cv.CalendarID FROM CalendarView cv WHERE cv.OrganizationID=@OrgID AND (SELECT COUNT(*) FROM CalendarRef cr WHERE cr.CalendarID = cv.CalendarID AND RefType=3) < 1 AND (SELECT COUNT(*) FROM CalendarRef cr WHERE cr.CalendarID = cv.CalendarID AND RefType=4) < 1 and (cv.RefID=@AttID and cv.RefType=@PageID or (@PageID=-1 and @AttID=-1)) ) UNION ( SELECT cv.CalendarID FROM CalendarView cv WHERE cv.OrganizationID=@OrgID AND (SELECT COUNT(*) FROM CalendarRef cr WHERE cr.CalendarID = cv.CalendarID AND RefType=3 AND RefID=@UserID) > 0 AND (cv.RefID=@AttID and cv.RefType=@PageID or (@PageID=-1 and @AttID=-1)) ) UNION ( SELECT cv.CalendarID FROM CalendarView cv WHERE cv.OrganizationID=@OrgID AND (SELECT COUNT(*) FROM CalendarRef cr WHERE cr.CalendarID = cv.CalendarID AND cr.RefType=4 AND cr.RefID IN ( SELECT gu.GroupID FROM GroupUsers gu WHERE gu.UserID=@UserID ) ) > 0 AND (cv.RefID=@AttID and cv.RefType=@PageID or (@PageID=-1 and @AttID=-1)) ) )";
              command.CommandType = CommandType.Text;
              //command.Parameters.AddWithValue("@month", date.Month);
              //command.Parameters.AddWithValue("@year", date.Year);
              command.Parameters.AddWithValue("@OrgID", orgID);
              command.Parameters.AddWithValue("@UserID", userID);
              command.Parameters.AddWithValue("@AttID", -1);
              command.Parameters.AddWithValue("@PageID", -1);
              Fill(command);
          }
      }

        public void LoadHolidays(int organizationId)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM CalendarEvents WHERE OrganizationID = @organizationId AND IsHoliday = 1";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@organizationId", organizationId);
                Fill(command);
            }
        }
    }
  
}
