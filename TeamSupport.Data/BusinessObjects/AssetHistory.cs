using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace TeamSupport.Data
{
  public partial class AssetHistoryItem
  {
    public void FullReadFromXml(string data, bool isInsert)
    {
      //Of the 7 writeable fields only 2 are IDs. So we'll do a normal read and then add code for the 2 ID fields.
      this.ReadFromXml(data, isInsert);

      LoginUser user = Collection.LoginUser;
      FieldMap fieldMap = Collection.FieldMap;

      StringReader reader = new StringReader(data);
      DataSet dataSet = new DataSet();
      dataSet.ReadXml(reader);

      try
      {
        if (this.ShippedTo == null && this.RefType != null)
        {
          if ((ReferenceType)this.RefType == ReferenceType.Organizations)
          {
            object organizationID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "ShippedTo", "NameAssignedTo", Organization.GetIDByName, false, user.OrganizationID);
            if (organizationID != null) this.ShippedTo = Convert.ToInt32(organizationID);
          }
          else if ((ReferenceType)this.RefType == ReferenceType.Contacts)
          {
            //The get contactID by name cannot be used
          }
        }
      }
      catch
      {
      }

      // chances are the update is also going to use this
      // if that is the case we might need to add support for assignments
      // we'll find out once we reach the update code
    }
  }
  
  public partial class AssetHistory
  {
    // This probably needs to be moved to AssetHistoryView
    public void LoadByAssetID(int assetID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT 
          a.actiondescription as 'Action Description', 
          a.actiontime as 'Time', 
          u.lastname+', '+u.firstname as 'User', 
          isnull(a.comments,'') as 'Comments', 
          isnull(o1.name,'') as 'Shipped From', 
          isnull(o2.name,'') as 'Shipped To', 
          a.shippingmethod as 'Shipping Method', 
          a.trackingnumber as 'Tracking Number', 
          isnull(a.ReferenceNum,'') as 'Reference Number'
        FROM
          AssetHistory as a 
          LEFT JOIN Organizations as o1 
            ON a.shippedfrom = o1.organizationid 
          LEFT JOIN Organizations as o2 
            ON a.shippedto = o2.organizationid 
          LEFT JOIN users as u 
            ON a.actor = u.userid
        WHERE
          a.AssetID = @AssetID 
          AND a.OrganizationID = @OrganizationID
        ORDER BY
          a.DateCreated desc";
        command.CommandText = InjectCustomFields(command.CommandText, "UserID", ReferenceType.Contacts);
        command.CommandType = CommandType.Text;
        //command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

  }
  
}
