using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class CustomValue
    {
        //cf.Name AS FieldName, cf.ApiFieldName, cf.FieldType, cf.ListValues
        public string FieldName
        {
            get
            {
                if (Row.Table.Columns.Contains("FieldName") && Row["FieldName"] != DBNull.Value)
                {
                    return (string)Row["FieldName"];
                }
                else return "";
            }
        }
        public string ApiFieldName
        {
            get
            {
                if (Row.Table.Columns.Contains("ApiFieldName") && Row["ApiFieldName"] != DBNull.Value)
                {
                    return (string)Row["ApiFieldName"];
                }
                else return "";
            }
        }
        public string ListValues
        {
            get
            {
                if (Row.Table.Columns.Contains("ListValues") && Row["ListValues"] != DBNull.Value)
                {
                    return (string)Row["ListValues"];
                }
                else return "";
            }
        }
        public CustomFieldType FieldType
        {
            get
            {
                if (Row.Table.Columns.Contains("FieldType") && Row["FieldType"] != DBNull.Value)
                {
                    return (CustomFieldType)Row["FieldType"];
                }
                else return CustomFieldType.Text;
            }
        }
        public string Name
        {
            get
            {
                if (Row.Table.Columns.Contains("Name") && Row["Name"] != DBNull.Value)
                {
                    return (string)Row["Name"];
                }
                else return "";
            }
        }
        public string Description
        {
            get
            {
                if (Row.Table.Columns.Contains("Description") && Row["Description"] != DBNull.Value)
                {
                    return (string)Row["Description"];
                }
                else return "";
            }
        }
        public ReferenceType RefType
        {
            get
            {
                if (Row.Table.Columns.Contains("RefType") && Row["RefType"] != DBNull.Value)
                {
                    return (ReferenceType)Row["RefType"];
                }
                else return ReferenceType.None;
            }
        }
        public int? AuxID
        {
            get
            {
                if (Row.Table.Columns.Contains("AuxID") && Row["AuxID"] != DBNull.Value)
                {
                    return (int?)Row["AuxID"];
                }
                else return null;
            }
        }
        public int Position
        {
            get
            {
                if (Row.Table.Columns.Contains("Position") && Row["Position"] != DBNull.Value)
                {
                    return (int)Row["Position"];
                }
                else return -1;
            }
        }
        public bool IsVisibleOnPortal
        {
            get
            {
                if (Row.Table.Columns.Contains("IsVisibleOnPortal") && Row["IsVisibleOnPortal"] != DBNull.Value)
                {
                    return (bool)Row["IsVisibleOnPortal"];
                }
                else return false;
            }
        }
        public bool IsFirstIndexSelect
        {
            get
            {
                if (Row.Table.Columns.Contains("IsFirstIndexSelect") && Row["IsFirstIndexSelect"] != DBNull.Value)
                {
                    return (bool)Row["IsFirstIndexSelect"];
                }
                else return false;
            }
        }
        public bool IsRequired
        {
            get
            {
                if (Row.Table.Columns.Contains("IsRequired") && Row["IsRequired"] != DBNull.Value)
                {
                    return (bool)Row["IsRequired"];
                }
                else return false;
            }
        }
        public bool IsRequiredToClose
        {
            get
            {
                if (Row.Table.Columns.Contains("IsRequiredToClose") && Row["IsRequiredToClose"] != DBNull.Value)
                {
                    return (bool)Row["IsRequiredToClose"];
                }
                else return false;
            }
        }
        public string Mask
        {
            get
            {
                if (Row.Table.Columns.Contains("Mask") && Row["Mask"] != DBNull.Value)
                {
                    return (string)Row["Mask"];
                }
                else return "";
            }
        }
        public int CustomFieldCategoryID
        {
            get
            {
                if (Row.Table.Columns.Contains("CustomFieldCategoryID") && Row["CustomFieldCategoryID"] != DBNull.Value)
                {
                    return (int)Row["CustomFieldCategoryID"];
                }
                else return -1;
            }
        }
    }

    public partial class CustomValues
    {
        public void LoadByFieldID(int customFieldID, int refID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = //"SELECT * FROM CustomValues WHERE (RefID = @RefID) AND (CustomFieldID = @CustomFieldID)";

                @"
SELECT 
cv.CustomValueID, 
cv.RefID, 
cv.CustomValue, 
cv.DateCreated, 
cv.DateModified, 
cv.CreatorID, 
cv.ModifierID, 
cv.ImportFileID,
cf.Name, 
cf.ApiFieldName, 
cf.FieldType, 
cf.ListValues, 
cf.Description, 
cf.RefType, 
cf.AuxID, 
cf.Position, 
cf.IsVisibleOnPortal, 
cf.IsFirstIndexSelect,
cf.IsRequired,
cf.OrganizationID, 
cf.CustomFieldID,
cf.IsRequiredToClose,
cf.Mask
FROM CustomFields cf LEFT JOIN CustomValues cv on cv.CustomFieldID = cf.CustomFieldID 
WHERE cf.CustomFieldID = @CustomFieldID
AND cv.RefID=@RefID";

                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@RefID", refID);
                command.Parameters.AddWithValue("@CustomFieldID", customFieldID);
                Fill(command);
            }
        }

        public void LoadByApiFieldName(int organizationID, string apiFieldName, int refID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
    SELECT * FROM CustomValues cv
    LEFT JOIN CustomFields cf on cv.CustomFieldID=cf.CustomFieldID
    WHERE cf.ApiFieldName=@ApiFieldName
    AND cf.OrganizationID=@OrganizationID
    AND cv.RefID=@RefID";

                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@RefID", refID);
                command.Parameters.AddWithValue("@ApiFieldName", apiFieldName);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public void LoadByReferenceType(int organizationID, ReferenceType refType, int refID)
        {
            LoadByReferenceType(organizationID, refType, null, refID);
        }

        public void LoadByReferenceType(int organizationID, ReferenceType refType, int? auxID, int refID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
SELECT 
cv.CustomValueID, 
cv.RefID, 
cv.CustomValue, 
cv.DateCreated, 
cv.DateModified, 
cv.CreatorID, 
cv.ModifierID, 
cv.ImportFileID,
cf.Name, 
cf.ApiFieldName, 
cf.FieldType, 
cf.ListValues, 
cf.Description, 
cf.RefType, 
cf.AuxID, 
cf.Position, 
cf.IsVisibleOnPortal, 
cf.IsFirstIndexSelect,
cf.IsRequired,
cf.OrganizationID, 
cf.CustomFieldID,
cf.IsRequiredToClose,
cf.Mask,
cf.CustomFieldCategoryID
FROM CustomFields cf LEFT JOIN CustomValues cv on cv.CustomFieldID = cf.CustomFieldID AND cv.RefID=@RefID
WHERE cf.OrganizationID = @OrganizationID
AND cf.RefType=@RefType
AND (cf.AuxID = @AuxID OR @AuxID < 0)
ORDER BY cf.Position";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@RefID", refID);
                command.Parameters.AddWithValue("@RefType", (int)refType);
                command.Parameters.AddWithValue("@AuxID", auxID ?? -1);
                Fill(command, "CustomFields, CustomValues");
            }
        }

        public void LoadByReferenceTypeForPortal(int organizationID, ReferenceType refType, int refID)
        {
            LoadByReferenceTypeForPortal(organizationID, refType, null, refID);
        }

        public void LoadByReferenceTypeForPortal(int organizationID, ReferenceType refType, int? auxID, int refID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
SELECT 
cv.CustomValueID, 
cv.RefID, 
cv.CustomValue, 
cv.DateCreated, 
cv.DateModified, 
cv.CreatorID, 
cv.ModifierID, 
cv.ImportFileID,
cf.Name, 
cf.ApiFieldName, 
cf.FieldType, 
cf.ListValues, 
cf.Description, 
cf.RefType, 
cf.AuxID, 
cf.Position, 
cf.IsVisibleOnPortal, 
cf.IsFirstIndexSelect,
cf.IsRequired,
cf.OrganizationID, 
cf.CustomFieldID,
cf.IsRequiredToClose,
cf.Mask,
cf.CustomFieldCategoryID
FROM CustomFields cf LEFT JOIN CustomValues cv on cv.CustomFieldID = cf.CustomFieldID AND cv.RefID=@RefID
WHERE cf.OrganizationID = @OrganizationID
AND cf.IsVisibleOnPortal = 1
AND cf.RefType=@RefType
AND (cf.AuxID = @AuxID OR @AuxID < 0)
ORDER BY cf.Position";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@RefID", refID);
                command.Parameters.AddWithValue("@RefType", (int)refType);
                command.Parameters.AddWithValue("@AuxID", auxID ?? -1);
                Fill(command, "CustomFields, CustomValues");
            }
        }

        // The LoadByReferenceType causes invalid cast exception on cv properties of fields without values.
        // This new method only loads fields with existing values to prevent the invalid cast exception.
        public void LoadExistingOnlyByReferenceType(int organizationID, ReferenceType refType, int refID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
SELECT 
cv.CustomValueID, 
cv.RefID, 
cv.CustomValue, 
cv.DateCreated, 
cv.DateModified, 
cv.CreatorID, 
cv.ModifierID, 
cv.ImportFileID,
cf.Name, 
cf.ApiFieldName, 
cf.FieldType, 
cf.ListValues, 
cf.Description, 
cf.RefType, 
cf.AuxID, 
cf.Position, 
cf.IsVisibleOnPortal, 
cf.IsFirstIndexSelect,
cf.IsRequired,
cf.OrganizationID, 
cf.CustomFieldID,
cf.IsRequiredToClose,
cf.Mask,
cf.CustomFieldCategoryID
FROM CustomFields cf
JOIN CustomValues cv 
	on cv.CustomFieldID = cf.CustomFieldID 
	AND cv.RefID=@RefID
WHERE cf.OrganizationID = @OrganizationID
AND cf.RefType=@RefType
ORDER BY cf.Position";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@RefID", refID);
                command.Parameters.AddWithValue("@RefType", (int)refType);
                Fill(command, "CustomFields, CustomValues");
            }
        }

        public void LoadByReferenceTypeModifiedAfterRecovery(int organizationID, ReferenceType refType, int refID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
SELECT 
cv.CustomValueID, 
cv.RefID, 
cv.CustomValue, 
cv.DateCreated, 
cv.DateModified, 
cv.CreatorID, 
cv.ModifierID, 
cv.ImportFileID,
cf.Name, 
cf.ApiFieldName, 
cf.FieldType, 
cf.ListValues, 
cf.Description, 
cf.RefType, 
cf.AuxID, 
cf.Position, 
cf.IsVisibleOnPortal, 
cf.IsFirstIndexSelect,
cf.IsRequired,
cf.OrganizationID, 
cf.CustomFieldID,
cf.IsRequiredToClose,
cf.Mask,
cf.CustomFieldCategoryID
FROM CustomFields cf LEFT JOIN CustomValues cv on cv.CustomFieldID = cf.CustomFieldID AND cv.RefID=@RefID
WHERE cf.OrganizationID = @OrganizationID
AND cf.RefType=@RefType
AND cv.DateModified > '2015-09-17 05:55:21.897'
ORDER BY cf.Position";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@RefID", refID);
                command.Parameters.AddWithValue("@RefType", (int)refType);
                Fill(command, "CustomFields, CustomValues");
            }
        }

        public void LoadParentsByReferenceType(int organizationID, ReferenceType refType, int? auxID, int refID, int? parentProductID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
SELECT 
cv.CustomValueID, 
cv.RefID, 
cv.CustomValue, 
cv.DateCreated, 
cv.DateModified, 
cv.CreatorID, 
cv.ModifierID, 
cv.ImportFileID,
cf.Name, 
cf.ApiFieldName, 
cf.FieldType, 
cf.ListValues, 
cf.Description, 
cf.RefType, 
cf.AuxID, 
cf.Position, 
cf.IsVisibleOnPortal, 
cf.IsFirstIndexSelect,
cf.IsRequired,
cf.OrganizationID, 
cf.CustomFieldID,
cf.IsRequiredToClose,
cf.Mask,
cf.CustomFieldCategoryID
FROM 
    CustomFields cf 
    LEFT JOIN CustomValues cv 
        on cv.CustomFieldID = cf.CustomFieldID 
        AND cv.RefID=@RefID
WHERE cf.OrganizationID = @OrganizationID
AND cf.RefType=@RefType
AND (cf.AuxID = @AuxID OR @AuxID < 0)
AND cf.ParentCustomFieldID IS NULL 
AND (cf.ParentProductID IS NULL OR cf.ParentProductID = @ParentProductID)
ORDER BY cf.Position";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@RefID", refID);
                command.Parameters.AddWithValue("@RefType", (int)refType);
                command.Parameters.AddWithValue("@AuxID", auxID ?? -1);
                command.Parameters.AddWithValue("@ParentProductID", parentProductID ?? -1);
                Fill(command, "CustomFields, CustomValues");
            }
        }

        public void LoadByParentValue(int organizationID, ReferenceType refType, int? auxID, int refID, int parentID, string parentValue, int? productID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
SELECT 
cv.CustomValueID, 
cv.RefID, 
cv.CustomValue, 
cv.DateCreated, 
cv.DateModified, 
cv.CreatorID, 
cv.ModifierID, 
cv.ImportFileID,
cf.Name, 
cf.ApiFieldName, 
cf.FieldType, 
cf.ListValues, 
cf.Description, 
cf.RefType, 
cf.AuxID, 
cf.Position, 
cf.IsVisibleOnPortal, 
cf.IsFirstIndexSelect,
cf.IsRequired,
cf.OrganizationID, 
cf.CustomFieldID,
cf.IsRequiredToClose,
cf.Mask,
cf.CustomFieldCategoryID
FROM 
    CustomFields cf 
    LEFT JOIN CustomValues cv 
        on cv.CustomFieldID = cf.CustomFieldID 
        AND cv.RefID=@RefID
WHERE cf.OrganizationID = @OrganizationID
AND cf.RefType=@RefType
AND (cf.AuxID = @AuxID OR @AuxID < 0)
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
AND
(
    cf.ParentProductID = @ProductID
    OR cf.ParentProductID IS NULL
)
ORDER BY cf.Position";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@RefID", refID);
                command.Parameters.AddWithValue("@RefType", (int)refType);
                command.Parameters.AddWithValue("@AuxID", auxID ?? -1);
                command.Parameters.AddWithValue("@ParentID", parentID);
                command.Parameters.AddWithValue("@ParentValue", parentValue);
                command.Parameters.AddWithValue("@ProductID", productID ?? -1);
                Fill(command, "CustomFields, CustomValues");
            }
        }

        public CustomValue FindByCustomFieldID(int customFieldID)
        {
            foreach (CustomValue customValue in this)
            {
                if (customValue.CustomFieldID == customFieldID)
                {
                    return customValue;
                }
            }
            return null;
        }

        public DataTable GetParentsAndChildrenByRefID(int organizationID, int? auxID, int ticketID, int? parentProductID)
        {

            DataTable parentCustomValues = GetParentsByReferenceType(organizationID, auxID, ticketID, parentProductID);
            DataTable result = parentCustomValues.Clone();

            int parentID = -1;
            string parentValue = string.Empty;

            for (int i = 0; i < parentCustomValues.Rows.Count; i++)
            {
                result.ImportRow(parentCustomValues.Rows[i]);
                parentID = (int)parentCustomValues.Rows[i]["CustomFieldID"];
                parentValue = parentCustomValues.Rows[i]["CustomValue"].ToString();
                GetChildrenByParentValue(organizationID, auxID, ticketID, parentID, parentValue, parentProductID, ref result);
            }

            return result;
        }

        private DataTable GetParentsByReferenceType(int organizationID, int? auxID, int ticketID, int? parentProductID)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText = @"
        SELECT 
            Distinct 
            cv.CustomValueID, 
            cv.RefID, 
            cv.CustomValue, 
            cv.DateCreated, 
            cv.DateModified, 
            cv.CreatorID, 
            cv.ModifierID, 
            cv.ImportFileID,
            cf.Name, 
            cf.ApiFieldName, 
            cf.FieldType, 
            cf.ListValues, 
            cf.Description, 
            cf.RefType, 
            cf.AuxID, 
            cf.Position, 
            cf.IsVisibleOnPortal, 
            cf.IsFirstIndexSelect,
            cf.IsRequired,
            cf.OrganizationID, 
            cf.CustomFieldID,
            cf.IsRequiredToClose,
            cf.Mask,
            cf.CustomFieldCategoryID,
            CASE WHEN EXISTS(SELECT NULL FROM CustomFields cf2 WHERE cf2.ParentCustomFieldID = cf.CustomFieldID) THEN 1 ELSE 0 END As IsConditionalParent
        FROM
            CustomFields cf
            LEFT JOIN CustomValues cv
                ON cv.CustomFieldID = cf.CustomFieldID 
                AND cv.RefID = @RefID
        WHERE
            cf.OrganizationID = @OrganizationID
            AND cf.IsVisibleOnPortal = 1
            AND cf.RefType = 17
            AND (cf.AuxID = @AuxID OR @AuxID < 0)
            AND cf.ParentCustomFieldID IS NULL 
            AND (cf.ParentProductID IS NULL OR cf.ParentProductID = @ParentProductID)
        ORDER BY
            cf.CustomFieldCategoryID asc, cf.Position asc";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@RefID", ticketID);
            command.Parameters.AddWithValue("@AuxID", auxID ?? -1);
            command.Parameters.AddWithValue("@ParentProductID", parentProductID ?? -1);

            DataTable result = new DataTable();

            using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);

                command.Connection = connection;
                command.Transaction = transaction;
                try
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(result);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionLogs.LogException(LoginUser, ex, "Parent Custom Values", DataUtils.GetCommandTextSql(command));
                    throw;
                }
                connection.Close();
            }

            return result;
        }

        private void GetChildrenByParentValue(int organizationID, int? auxID, int ticketID, int parentID, string parentValue, int? productID, ref DataTable result)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText = @"
        SELECT 
            Distinct
            cv.CustomValueID, 
            cv.RefID, 
            cv.CustomValue, 
            cv.DateCreated, 
            cv.DateModified, 
            cv.CreatorID, 
            cv.ModifierID, 
            cv.ImportFileID,
            cf.Name, 
            cf.ApiFieldName, 
            cf.FieldType, 
            cf.ListValues, 
            cf.Description, 
            cf.RefType, 
            cf.AuxID, 
            cf.Position, 
            cf.IsVisibleOnPortal, 
            cf.IsFirstIndexSelect,
            cf.IsRequired,
            cf.OrganizationID, 
            cf.CustomFieldID,
            cf.IsRequiredToClose,
            cf.Mask,
            cf.CustomFieldCategoryID,
            CASE WHEN EXISTS(SELECT NULL FROM CustomFields cf2 WHERE cf2.ParentCustomFieldID = cf.CustomFieldID) THEN 1 ELSE 0 END As IsConditionalParent
        FROM
            CustomFields cf
            LEFT JOIN CustomValues cv
                ON cv.CustomFieldID = cf.CustomFieldID 
                AND cv.RefID = @RefID
        WHERE
            cf.OrganizationID = @OrganizationID
            AND cf.IsVisibleOnPortal = 1
            AND cf.RefType = 17
            AND (cf.AuxID = @AuxID OR @AuxID < 0)
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
            AND
            (
                cf.ParentProductID = @ProductID
                OR cf.ParentProductID IS NULL
            )
        ORDER BY
            cf.Position";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@RefID", ticketID);
            command.Parameters.AddWithValue("@AuxID", auxID ?? -1);
            command.Parameters.AddWithValue("@ParentID", parentID);
            command.Parameters.AddWithValue("@ParentValue", parentValue);
            command.Parameters.AddWithValue("@ProductID", productID ?? -1);

            DataTable children = new DataTable();

            using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);

                command.Connection = connection;
                command.Transaction = transaction;
                try
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(children);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionLogs.LogException(LoginUser, ex, "Children Custom Values", DataUtils.GetCommandTextSql(command));
                    throw;
                }
                connection.Close();
            }

            int childID = -1;
            string childValue = string.Empty;

            for (int i = 0; i < children.Rows.Count; i++)
            {
                result.ImportRow(children.Rows[i]);
                childID = (int)children.Rows[i]["CustomFieldID"];
                childValue = children.Rows[i]["CustomValue"].ToString();
                GetChildrenByParentValue(organizationID, auxID, ticketID, childID, childValue, productID, ref result);
            }
        }

        partial void BeforeRowEdit(CustomValue newValue)
        {
            CustomValue oldValue = CustomValues.GetCustomValue(LoginUser, newValue.CustomValueID);
            newValue.OrganizationID = LoginUser.OrganizationID;
            if (oldValue.Value == newValue.Value) return;
            CustomField customField = CustomFields.GetCustomField(LoginUser, newValue.CustomFieldID);
            string format = "Changed {0} from '{1}' to '{2}'.";
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, customField.RefType, newValue.RefID, string.Format(format, customField.Name, oldValue.Value, newValue.Value));
        }

        partial void BeforeRowInsert(CustomValue newValue)
        {
            newValue.OrganizationID = LoginUser.OrganizationID;
        }

        public static CustomValue GetValue(LoginUser loginUser, int customFieldID, int refID, bool createValue)
        {
            CustomValues values = new CustomValues(loginUser);
            values.LoadByFieldID(customFieldID, refID);

            if (values.IsEmpty)
            {
                CustomField field = CustomFields.GetCustomField(loginUser, customFieldID);
                values = new CustomValues(loginUser);
                CustomValue value = values.AddNewCustomValue();
                value.CustomFieldID = customFieldID;
                value.Value = "";
                if (field.FieldType == CustomFieldType.PickList)
                {
                    string[] items = field.ListValues.Split('|');
                    if (items.Length > 0) value.Value = items[0];
                }
                value.RefID = refID;
                if (createValue == false) return value;
                value.Collection.Save();
                values.LoadByFieldID(customFieldID, refID);
                return values[0];
            }
            else
            {
                return values[0];
            }

        }

        public static CustomValue GetValue(LoginUser loginUser, int customFieldID, int refID)
        {
            return GetValue(loginUser, customFieldID, refID, true);

        }

        public static CustomValue GetValue(LoginUser loginUser, int refID, string apiFieldName)
        {
            CustomFields customFields = new CustomFields(loginUser);
            customFields.LoadByOrganization(loginUser.OrganizationID);
            CustomField field = customFields.FindByApiFieldName(apiFieldName);
            if (field == null) return null;
            return GetValue(loginUser, field.CustomFieldID, refID);
        }

        public static void UpdateValue(LoginUser loginUser, int customFieldID, int refID, string value)
        {
            if (value == null) value = "";
            value = value.Trim();

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "uspSetCustomValue";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CustomFieldID", customFieldID);
                command.Parameters.AddWithValue("@RefID", refID);
                command.Parameters.AddWithValue("@CustomValue", value);
                command.Parameters.AddWithValue("@ModifierID", loginUser.UserID);
                SqlExecutor.ExecuteNonQuery(loginUser, command);
            }
        }

        public static void UpdateByAPIFieldName(LoginUser loginUser, CustomFields customFields, int refID, string apiFieldName, string value)
        {
            value = value ?? "";

            CustomField field = customFields.FindByApiFieldName(apiFieldName);
            //if (field == null) throw new Exception("Unable to find field '" + apiFieldName + "'");
            if (field != null) UpdateValue(loginUser, field.CustomFieldID, refID, value);
        }

        public static void UpdateByAPIFieldName(LoginUser loginUser, int refID, string apiFieldName, string value)
        {
            CustomFields customFields = new CustomFields(loginUser);
            customFields.LoadByOrganization(loginUser.OrganizationID);
            UpdateByAPIFieldName(loginUser, customFields, refID, apiFieldName, value);
        }



    }
}
