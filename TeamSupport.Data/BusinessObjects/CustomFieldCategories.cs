using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CustomFieldCategory
  {
        public string ProductFamilyName
        {
            get
            {
                if (Row.Table.Columns.Contains("ProductFamilyName") && Row["ProductFamilyName"] != DBNull.Value)
                {
                    return (string)Row["ProductFamilyName"];
                }
                else return "";
            }
        }
  }
  
  public partial class CustomFieldCategories
  {

    partial void BeforeRowDelete(int customFieldCategoryID)
    {
      CustomFieldCategories cats = new CustomFieldCategories(LoginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE CustomFields SET CustomFieldCategoryID = NULL WHERE CustomFieldCategoryID = @CustomFieldCategoryID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@CustomFieldCategoryID", customFieldCategoryID);
        cats.ExecuteNonQuery(command, "CustomFields");
      }
    }

    public void LoadByRefType(ReferenceType refType)
    {
      LoadByRefType(refType, null);
    }

    public void LoadByRefType(ReferenceType refType, int? auxID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
            @"
            SELECT 
                c.*, 
                pf.Name AS 'ProductFamilyName' 
            FROM 
                CustomFieldCategories c
                LEFT JOIN ProductFamilies pf
                    ON c.ProductFamilyID = pf.ProductFamilyID
            WHERE 
                c.RefType = @RefType
                AND (c.AuxID = @AuxID OR @AuxID < 0)
                AND c.OrganizationID = @OrganizationID
            ORDER BY 
                c.Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefType", refType);
        command.Parameters.AddWithValue("@AuxID", auxID == null ? -1 : (int)auxID);
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        Fill(command);
      }

    }

    public void LoadByRefTypeWithUserRights(ReferenceType refType, int? auxID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
            @"
                SELECT 
                    c.*, 
                    pf.Name AS 'ProductFamilyName' 
                FROM
                    CustomFieldCategories c
                    JOIN Organizations o
                        ON c.OrganizationID = o.OrganizationID
                    LEFT JOIN ProductFamilies pf
                        ON c.ProductFamilyID = pf.ProductFamilyID
                WHERE 
                    RefType = @RefType
                    AND (AuxID = @AuxID OR @AuxID < 0)
                    AND c.OrganizationID = @OrganizationID
                    AND
                    (
                        o.UseProductFamilies = 0
                        OR (SELECT ProductFamiliesRights FROM Users WHERE UserID = @UserID) = 0
                        OR c.ProductFamilyID IS NULL
                        OR c.ProductFamilyID IN (SELECT ProductFamilyID FROM UserRightsProductFamilies WHERE UserID = @UserID)
                    )
                ORDER BY 
                    Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefType", refType);
        command.Parameters.AddWithValue("@AuxID", auxID == null ? -1 : (int)auxID);
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
        Fill(command);
      }

    }

    public void LoadByOrganization(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT * FROM CustomFieldCategories WHERE (OrganizationID = @OrganizationID) ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        Fill(command);
      }

    }

    public static int GetMaxPosition(LoginUser loginUser, ReferenceType refType, int? auxID)
    {
      CustomFieldCategories cats = new CustomFieldCategories(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"SELECT MAX(Position) FROM CustomFieldCategories 
WHERE (RefType = @RefType) 
AND (AuxID = @AuxID OR @AuxID < 0)
AND (OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefType", refType);
        command.Parameters.AddWithValue("@AuxID", auxID == null ? -1 : (int)auxID);
        command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
        object o = cats.ExecuteScalar(command);
        if (o == null || o == DBNull.Value) return -1;
        return (int)o;
      }
    }

  }
  
}
