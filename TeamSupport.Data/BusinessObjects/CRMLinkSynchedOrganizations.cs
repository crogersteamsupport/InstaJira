using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CRMLinkSynchedOrganization
  {
  }
  
  public partial class CRMLinkSynchedOrganizations
  {
    public void LoadByCRMLinkTableID(int CRMLinkTableID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM CRMLinkSynchedOrganizations WHERE CRMLinkTableID = @CRMLinkTableID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@CRMLinkTableID", CRMLinkTableID);
        Fill(command);
      }
    }

    public bool CheckIfExists(string organizationCRMID)
    {
      bool result = false;
      foreach (CRMLinkSynchedOrganization item in this)
      {
        if (String.Equals(organizationCRMID, item.OrganizationCRMID, StringComparison.Ordinal))
        {
          result = true;
          break;
        }
      }
      return result;
    }

    public void DeleteByCRMLinkTableID(int CRMLinkTableID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE CRMLinkSynchedOrganizations WHERE CRMLinkTableID = @CRMLinkTableID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@CRMLinkTableID", CRMLinkTableID);
        ExecuteNonQuery(command, "CRMLinkSynchedOrganizations");
      }
    }
  }
  
}
