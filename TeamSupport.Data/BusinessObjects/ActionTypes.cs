using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ActionType 
  {
  }

  public partial class ActionTypes 
  {
    partial void BeforeDBDelete(int actionTypeID)
    {
     
    }

    partial void AfterDBDelete(int actionTypeID)
    {
      ValidatePositions(LoginUser.OrganizationID);
    
    }
    
    public ActionType FindByName(string name)
    {
      foreach (ActionType actionType in this)
      {
        if (actionType.Name == name)
        {
          return actionType;
        }
      }
      return null;
    }

    public void LoadByOrganizationID(int organizationID, string orderBy = "Position")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ActionTypes WHERE OrganizationID = @OrganizationID ORDER BY " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }

   
  }
}
