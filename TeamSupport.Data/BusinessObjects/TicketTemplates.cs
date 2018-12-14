using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TicketTemplate
  {
  }
  
  public partial class TicketTemplates
  {

    public void LoadByOrganization(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT t.*, tt.Name FROM TicketTemplates t LEFT JOIN TicketTypes tt ON tt.TicketTypeID = t.TicketTypeID WHERE (t.OrganizationID = @OrganizationID) ORDER BY t.TemplateType, t.TemplateText, tt.Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public static TicketTemplate GetByTicketType(LoginUser loginUser, int ticketTypeID)
    {
      TicketTemplates templates = new TicketTemplates(loginUser);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketTemplates WHERE (OrganizationID = @OrganizationID) AND TicketTypeID = @TicketTypeID AND IsEnabled = 1 AND TemplateType = 0";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
        templates.Fill(command);
      }
      if (templates.IsEmpty) return null;
      return templates[0];
    
    }

    public static TicketTemplate GetByActionType(LoginUser loginUser, int actionTypeID)
    {
        TicketTemplates templates = new TicketTemplates(loginUser);

        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT * FROM TicketTemplates WHERE (OrganizationID = @OrganizationID) AND TicketTypeID = @TicketTypeID AND IsEnabled = 1 AND TemplateType = 2";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
            command.Parameters.AddWithValue("@TicketTypeID", actionTypeID);
            templates.Fill(command);
        }
        if (templates.IsEmpty) return null;
        return templates[0];

    }

    public static TicketTemplate GetByTriggerText(LoginUser loginUser, string triggerText)
    {
      TicketTemplates templates = new TicketTemplates(loginUser);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketTemplates WHERE (OrganizationID = @OrganizationID) AND RTRIM(TriggerText) = @TriggerText AND IsEnabled = 1 AND TemplateType = 1";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
        command.Parameters.AddWithValue("@TriggerText", triggerText.Trim());
        templates.Fill(command);
      }
      if (templates.IsEmpty) return null;
      return templates[0];

    }

    public static int GetTriggerTextCount(LoginUser loginUser)
    {
      TicketTemplates templates = new TicketTemplates(loginUser);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT COUNT(*) FROM TicketTemplates WHERE (OrganizationID = @OrganizationID) AND IsEnabled = 1 AND TemplateType = 1";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
        object o = templates.ExecuteScalar(command);
        if (o == null || o == DBNull.Value) return 0;
        return (int)o;
      }

    }


    public static int GetTicketTypeCount(LoginUser loginUser)
    {
      TicketTemplates templates = new TicketTemplates(loginUser);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT COUNT(*) FROM TicketTemplates WHERE (OrganizationID = @OrganizationID) AND IsEnabled = 1 AND TemplateType = 0";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
        object o = templates.ExecuteScalar(command);
        if (o == null || o == DBNull.Value) return 0;
        return (int)o;
      }

    }
  }
  
}
