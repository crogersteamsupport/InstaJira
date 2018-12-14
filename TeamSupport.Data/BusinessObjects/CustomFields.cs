using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CustomField 
  {
    public object GetValue(int id)
    {
      object result = null;

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "uspGetCustomValue";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@RefID", id);
        command.Parameters.AddWithValue("@CustomFieldID", CustomFieldID);

        object o = (BaseCollection as CustomFields).ExecuteScalar(command, "CustomValues");
        if (o != null && o != DBNull.Value) 
        {
          result = o;
        }
        else if (FieldType == CustomFieldType.PickList)
        {
          string[] items = ListValues.Split('|');
          if (items.Length > 0) result = items[0];
        }
      }

      return result;
    }

    public object GetTypedValue(int id)
    {
      try
      {
        object result = GetValue(id);
        switch (FieldType)
        {
          case CustomFieldType.Text: return result;
          case CustomFieldType.Date:
          case CustomFieldType.Time:
          case CustomFieldType.DateTime: return DateTime.Parse(result.ToString(), BaseCollection.LoginUser.CultureInfo);
          case CustomFieldType.Boolean: return bool.Parse(result.ToString());
          case CustomFieldType.Number: return double.Parse(result.ToString());
          case CustomFieldType.PickList: return result;
          default: return null;
        }
      }
      catch 
      {
        return null;
      }
    }

    public void SetValue(int id, object o)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "uspSetCustomValue";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@CustomFieldID", CustomFieldID);
        command.Parameters.AddWithValue("@RefID", id);
        command.Parameters.AddWithValue("@CustomValue", o.ToString());
        command.Parameters.AddWithValue("@ModifierID", BaseCollection.LoginUser.UserID);
        BaseCollection.ExecuteNonQuery(command, "CustomValues");
      }
    }
  }

  public partial class CustomFields 
  {

    public static string GenerateApiFieldName(string fieldName)
    {
      string name = fieldName.Trim();
      StringBuilder builder = new StringBuilder();
      foreach (char c in fieldName.Trim())
      {
        if (char.IsLetterOrDigit(c))
        {
          builder.Append(c);
        }
      }
      name = builder.ToString();
      return Char.IsDigit(name[0]) ? "_" + name : name;    
    }

    public CustomField FindByName(string name)
    {
      foreach (CustomField customField in this)
      {
        if (customField.Name.ToLower() == name.ToLower())
        {
          return customField;
        }
      }
      return null;
    }

    public CustomField FindByApiFieldName(string name)
    {
      foreach (CustomField customField in this)
      {
        if (customField.ApiFieldName.ToLower() == name.ToLower())
        {
          return customField;
        }
      }
      return null;
    }

    partial void BeforeDBDelete(int customFieldID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM CustomValues WHERE (CustomFieldID = @CustomFieldID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@CustomFieldID", customFieldID);
        ExecuteNonQuery(command, "CustomValues");
      }
    }

    public static string GetCustomFieldTypeName(CustomFieldType type)
    {
      string result;
      switch (type)
      {
        case CustomFieldType.Text: result = "Text"; break;
        case CustomFieldType.DateTime: result = "Date and Time"; break;
        case CustomFieldType.Boolean: result = "True or False"; break;
        case CustomFieldType.Number: result = "Number"; break;
        case CustomFieldType.PickList: result = "Pick List"; break;
        case CustomFieldType.Date: result = "Date"; break;
        case CustomFieldType.Time: result = "Time"; break;
        default: result = ""; break;
      }
      return result;
    }


    public static CustomField GetCustomFieldByApi(LoginUser loginUser, int organizationID, string apiFieldName)
    {
      CustomFields customFields = new CustomFields(loginUser);
      customFields.LoadByApiName(organizationID, apiFieldName);
      if (customFields.IsEmpty)
        return null;
      else
        return customFields[0];
    }


    public void LoadByApiName(int organizationID, string apiFieldName)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM CustomFields WHERE (OrganizationID = @OrganizationID) AND (ApiFieldName = @ApiFieldName) ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@ApiFieldName", apiFieldName);
        Fill(command, "CustomFields");
      }
    }

    public void LoadByOrganization(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM CustomFields WHERE (OrganizationID = @OrganizationID) ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command, "CustomFields");
      }
    }

    public void LoadByOrganizationFieldTypeAndTicketType(int organizationID, CustomFieldType fieldType, int ticketTypeID, int selfID, int? customFieldCategoryID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string exceptSelfClause = string.Empty;
        if (selfID != -1)
        {
            exceptSelfClause = " AND CustomFieldID <> @SelfID ";
        }

        string categoryClause = " AND CustomFieldCategoryID IS NULL ";
        if (customFieldCategoryID != null)
        {
            categoryClause = " AND CustomFieldCategoryID = @CustomFieldCategoryID ";
        }

        command.CommandText = @"
        SELECT 
          *
        FROM
          CustomFields
        WHERE
          OrganizationID = @OrganizationID
          AND FieldType = @FieldType
          AND AuxID = @TicketType
          " + exceptSelfClause + @"
          " + categoryClause + @"
        ORDER BY
          Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@FieldType", fieldType);
        command.Parameters.AddWithValue("@TicketType", ticketTypeID);
        command.Parameters.AddWithValue("@SelfID", selfID);
        command.Parameters.AddWithValue("@CustomFieldCategoryID", customFieldCategoryID ?? -1);
        Fill(command, "CustomFields");
      }

    }

    public void LoadByReferenceType(int organizationID, ReferenceType refType, int? auxID, string orderBy = "Position")
    {
      if (auxID == null) auxID = -1;
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM CustomFields WHERE (OrganizationID = @OrganizationID) AND (RefType = @RefType) AND (AuxID = @AuxID OR @AuxID < 0) ORDER BY " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@RefType", (int)refType);
        command.Parameters.AddWithValue("@AuxID", auxID);
        Fill(command, "CustomFields");
      }
    }

    public void LoadParentsByReferenceType(int organizationID, ReferenceType refType, int? auxID, string orderBy = "Position")
    {
        if (auxID == null) auxID = -1;
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
                SELECT 
                    * 
                FROM 
                    CustomFields 
                WHERE 
                    OrganizationID = @OrganizationID
                    AND RefType = @RefType
                    AND (AuxID = @AuxID OR @AuxID < 0)
                    AND ParentCustomFieldID IS NULL
                    AND ParentProductID IS NULL
                ORDER BY " 
                    + orderBy;
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@RefType", (int)refType);
            command.Parameters.AddWithValue("@AuxID", auxID);
            Fill(command, "CustomFields");
        }
    }

    public void LoadProductMatchingByReferenceType(int organizationID, ReferenceType refType, int auxID, int productID, string orderBy = "Position")
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
                SELECT 
                    * 
                FROM 
                    CustomFields 
                WHERE 
                    OrganizationID = @OrganizationID
                    AND RefType = @RefType
                    AND (AuxID = @AuxID OR @AuxID < 0)
                    AND ParentCustomFieldID IS NULL
                    AND ParentProductID = @ProductID
                ORDER BY "
                    + orderBy;
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@RefType", (int)refType);
            command.Parameters.AddWithValue("@AuxID", auxID);
            command.Parameters.AddWithValue("@ProductID", productID);
            Fill(command, "CustomFields");
        }
    }

		public void LoadPortalTicketCustomFields(int organizationID,  int ticketTypeID, int? productID, string orderBy = "CustomFieldCategoryID asc, Position asc")
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"
                SELECT *
								FROM CustomFields
								WHERE OrganizationID = @OrganizationID
								AND RefType = 17
								AND IsVisibleOnPortal = 1
								AND ParentCustomFieldID IS NULL
								AND ( (AuxID = @TicketTypeID AND ParentProductID IS Null) OR (AuxID = @TicketTypeID AND ParentProductID = @ProductID))
                ORDER BY "
								+ orderBy;
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@OrganizationID", organizationID);
				command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
				command.Parameters.AddWithValue("@ProductID", productID);
				Fill(command, "CustomFields");
			}
		}

		public void LoadPortalTicketCustomFields(int organizationID, int ticketTypeID, int? productID, int parentID, string parentValue, string orderBy = "CustomFieldCategoryID asc, Position asc")
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"
                SELECT *
								FROM CustomFields as cf
								WHERE OrganizationID = @OrganizationID
								AND RefType = 17
								AND IsVisibleOnPortal = 1
								AND cf.ParentCustomFieldID = @ParentID
								AND 
								(
										cf.ParentCustomValue = @ParentValue
										OR
										(
												@ParentValue IS NULL
												AND cf.ParentCustomValue IS NULL
										)
								)
								AND ( (AuxID = @TicketTypeID AND ParentProductID IS Null) OR (AuxID = @TicketTypeID AND ParentProductID = @ProductID))
                ORDER BY "
								+ orderBy;
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@OrganizationID", organizationID);
				command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
				command.Parameters.AddWithValue("@ParentID", parentID);
				command.Parameters.AddWithValue("@ParentValue", parentValue);
				command.Parameters.AddWithValue("@ProductID", productID);
				Fill(command, "CustomFields");
			}
		}

		public void LoadParentValueMatching(int organizationID, int parentCustomFieldID, string parentCustomValue, int productID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
                SELECT 
                    * 
                FROM 
                    CustomFields 
                WHERE 
                    OrganizationID = @OrganizationID
                    AND ParentCustomFieldID = @ParentCustomFieldID
                    AND ParentCustomValue = @ParentCustomValue
                    AND (ParentProductID IS NULL OR ParentProductID = @ProductID)
                ORDER BY 
                    Position";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@ParentCustomFieldID", parentCustomFieldID);
            command.Parameters.AddWithValue("@ParentCustomValue", parentCustomValue);
            command.Parameters.AddWithValue("@ProductID", productID);
            Fill(command, "CustomFields");
        }
    }

    public void LoadByReferenceType(int organizationID, ReferenceType refType)
    {
      LoadByReferenceType(organizationID, refType, -1);

    }

    public void LoadByTicketTypeID(int organizationID, int ticketTypeID)
    {
      LoadByReferenceType(organizationID, ReferenceType.Tickets, ticketTypeID);
    }

    public void LoadByPosition(int organizationID, ReferenceType refType, int auxID, int position)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM CustomFields WHERE (OrganizationID = @OrganizationID) AND (RefType = @RefType) AND (AuxID = @AuxID OR AuxID < 0) AND (Position = @Position)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@RefType", refType);
        command.Parameters.AddWithValue("@AuxID", auxID);
        command.Parameters.AddWithValue("@Position", position);
        Fill(command);
      }

    }

    public void ValidatePositions(int organizationID, ReferenceType refType, int auxID)
    {
      CustomFields fields = new CustomFields(LoginUser);
      fields.LoadByReferenceType(organizationID, refType, auxID);
      int i = 0;
      foreach (CustomField field in fields)
      {
        field.Position = i;
        i++;
      }
      fields.Save();
    }

    public void MovePositionUp(int customFieldID)
    {
      CustomFields fields1 = new CustomFields(LoginUser);
      fields1.LoadByCustomFieldID(customFieldID);
      ValidatePositions(fields1[0].OrganizationID, fields1[0].RefType, fields1[0].AuxID);
      if (fields1.IsEmpty || fields1[0].Position < 1) return;

      CustomFields fields2 = new CustomFields(LoginUser);
      fields2.LoadByPosition(fields1[0].OrganizationID, fields1[0].RefType, fields1[0].AuxID, fields1[0].Position - 1);
      if (!fields2.IsEmpty)
      {
        fields2[0].Position = fields2[0].Position + 1;
        fields2.Save();
      }

      fields1[0].Position = fields1[0].Position - 1;
      fields1.Save();
    }

    public void MovePositionDown(int customFieldID)
    {
      CustomFields fields1 = new CustomFields(LoginUser);
      fields1.LoadByCustomFieldID(customFieldID);
      ValidatePositions(fields1[0].OrganizationID, fields1[0].RefType, fields1[0].AuxID);
      if (fields1.IsEmpty || fields1[0].Position >= GetMaxPosition(fields1[0].OrganizationID, fields1[0].RefType, fields1[0].AuxID)) return;

      CustomFields fields2 = new CustomFields(LoginUser);
      fields2.LoadByPosition(fields1[0].OrganizationID, fields1[0].RefType, fields1[0].AuxID, fields1[0].Position + 1);
      if (!fields2.IsEmpty)
      {
        fields2[0].Position = fields2[0].Position - 1;
        fields2.Save();
      }

      fields1[0].Position = fields1[0].Position + 1;
      fields1.Save();
    }

    public virtual int GetMaxPosition(int organizationID, ReferenceType refType, int auxID, int? customFieldCategoryID)
    {
      int position = -1;

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = 
@"SELECT MAX(Position) 
FROM CustomFields 
WHERE (RefType = @RefType) 
AND (AuxID = @AuxID OR AuxID < 0) 
AND (OrganizationID = @OrganizationID) 
AND (ISNULL(CustomFieldCategoryID, -1) = @CustomFieldCategoryID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@RefType", refType);
        command.Parameters.AddWithValue("@AuxID", auxID);
        command.Parameters.AddWithValue("@CustomFieldCategoryID", customFieldCategoryID == null ? -1 : (int)customFieldCategoryID);

        object o = ExecuteScalar(command);
        if (o == DBNull.Value) return -1;
        position = (int)o;
      }
      return position;
      
    }

    public virtual int GetMaxPosition(int organizationID, ReferenceType refType, int auxID)
    {
      return GetMaxPosition(organizationID, refType, auxID, null);
    }

    
  }
}
