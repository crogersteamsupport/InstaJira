using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace TeamSupport.Data
{
  public partial class Asset
  {
    public void FullReadFromXml(string data, bool isInsert)
    {
      this.ReadFromXml(data, isInsert);

      LoginUser user = Collection.LoginUser;
      FieldMap fieldMap = Collection.FieldMap;

      StringReader reader = new StringReader(data);
      DataSet dataSet = new DataSet();
      dataSet.ReadXml(reader);

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
        object productVersionID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "ProductVersionID", "ProductVersionNumber", ProductVersion.GetIDByName, false, this.ProductID);
        if (productVersionID != null) this.ProductVersionID = Convert.ToInt32(productVersionID);
      }
      catch
      {
      }
    }

  }
  
  public partial class Assets
  {
    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Assets WHERE OrganizationID = @OrganizationID ORDER BY Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByOrganizationIDCreatedAfterRestore(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Assets WHERE OrganizationID = @OrganizationID WHERE DateCreated > '2015-09-17 05:55:21.897' ORDER BY Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Assets WHERE AssetID IN (SELECT AssetID FROM AssetTickets WHERE TicketID = @TicketID ) ORDER BY Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadByLikeNameOrSerial(int organizationID, string searchTerm, int maxRows)
    {

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP (@MaxRows) * FROM Assets WHERE (Name LIKE '%'+@Term+'%' OR SerialNumber LIKE '%'+@Term+'%') AND (OrganizationID = @OrganizationID) ORDER BY Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@MaxRows", maxRows);
        command.Parameters.AddWithValue("@Term", searchTerm);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);

        Fill(command);
      }

    }

    public Asset FindByImportID(string importID)
    {
      importID = importID.ToLower().Trim();
      foreach (Asset asset in this)
      {
        if ((asset.ImportID != null && asset.ImportID.Trim().ToLower() == importID) || 
            (asset.SerialNumber != null && asset.SerialNumber.ToLower().Trim() == importID) || 
            (asset.Name != null && asset.Name.ToLower().Trim() == importID)
           )
        {
          return asset;
        }
      }
      return null;
    }

    public Asset FindByName(string name)
    {
      name = name.ToLower().Trim();
      foreach (Asset asset in this)
      {
        if (asset.Name != null && asset.Name.ToLower().Trim() == name)
        {
          return asset;
        }
      }
      return null;
    }

    public void LoadForIndexing(int organizationID, int max, bool isRebuilding)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string text = @"
        SELECT 
          TOP {0} 
          AssetID
        FROM 
          Assets a WITH(NOLOCK)
        WHERE 
          a.NeedsIndexing = 1
          AND a.OrganizationID = @OrganizationID
        ORDER BY 
          a.DateModified DESC";

        if (isRebuilding)
        {
          text = @"
          SELECT 
            AssetID
          FROM 
            Assets a WITH(NOLOCK)
          WHERE 
            a.OrganizationID = @OrganizationID
          ORDER BY 
            a.DateModified DESC";
        }

        command.CommandText = string.Format(text, max.ToString());
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByImportID(string importID, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Assets a WHERE a.ImportID = @ImportID AND a.OrganizationID = @OrganizationID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ImportID", importID);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }
  }
  
}
