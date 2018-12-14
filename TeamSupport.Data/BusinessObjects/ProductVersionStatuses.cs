using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ProductVersionStatus 
  {
  }

  public partial class ProductVersionStatuses 
  {
    public ProductVersionStatus FindByName(string name)
    {
      foreach (ProductVersionStatus productVersionStatus in this)
      {
        if (productVersionStatus.Name == name)
        {
          return productVersionStatus;
        }
      }
      return null;
    }
    partial void BeforeDBDelete(int productVersionStatusID)
    {
      ProductVersionStatuses productVersionStatuses = new ProductVersionStatuses(LoginUser);
      productVersionStatuses.LoadAllPositions(LoginUser.OrganizationID);

      int id = -1;

      foreach (ProductVersionStatus productVersionStatus in productVersionStatuses)
      {
        if (productVersionStatus.ProductVersionStatusID != productVersionStatusID)
        {
          id = productVersionStatus.ProductVersionStatusID;
          break;
        }
      }

      if (id < 0)
      {
        throw new Exception("You cannot delete the last product version.");
      }

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE ProductVersions SET ProductVersionStatusID = @NewID WHERE (ProductVersionStatusID = @ProductVersionStatusID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductVersionStatusID", productVersionStatusID);
        command.Parameters.AddWithValue("@NewID", id);
        ExecuteNonQuery(command, "ProductVersions");
      }
    }



    public void LoadByOrganizationID(int organizationID, string orderBy = "Position")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ProductVersionStatuses WHERE OrganizationID = @OrganizationID  ORDER BY " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }



  }
}
