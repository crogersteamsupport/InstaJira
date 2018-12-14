using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;
using System.Data;

public partial class Frames_ProductInformation : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    int productID;

    try
    {
      productID = int.Parse(Request["ProductID"]);
    }
    catch (Exception)
    {
      Response.Write("");
      Response.End();
      return;
      
    }

    lblProperties.Visible = true;

    Product product = (Product)Products.GetProduct(UserSession.LoginUser, productID);
    lblProperties.Visible = product == null;

    if (product == null) return;

    DataTable table = new DataTable();
    table.Columns.Add("Name");
    table.Columns.Add("Value");

    table.Rows.Add(new string[] { "Name:", product.Name });
    table.Rows.Add(new string[] { "Description:", product.Description });
    table.Rows.Add(new string[] { "Product ID:", product.ProductID.ToString()});

    CustomFields fields = new CustomFields(UserSession.LoginUser);
    fields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, ReferenceType.Products);
    StringBuilder valueAsString = null;

    foreach (CustomField field in fields)
    {
      CustomValue value = CustomValues.GetValue(UserSession.LoginUser, field.CustomFieldID, productID);
      switch (value.FieldType)
      {
        case CustomFieldType.Date:
          valueAsString = new StringBuilder();
          if (!string.IsNullOrEmpty(value.Value))
          {
            try
            {
              DateTime valueAsDateTime = DataUtils.DateToLocal(UserSession.LoginUser, DateTime.Parse(value.Value));
              valueAsString.Append(valueAsDateTime.ToString("d", UserSession.LoginUser.CultureInfo));
            }
            catch
            {
              valueAsString.Append(value.Value);
            }
          }
          table.Rows.Add(new string[] { field.Name + ":", valueAsString.ToString() });
          break;
        case CustomFieldType.Time:
          valueAsString = new StringBuilder();
          if (!string.IsNullOrEmpty(value.Value))
          {
            try
            {
              DateTime valueAsDateTime = DataUtils.DateToLocal(UserSession.LoginUser, DateTime.Parse(value.Value));
              valueAsString.Append(valueAsDateTime.ToString("t", UserSession.LoginUser.CultureInfo));
            }
            catch
            {
              valueAsString.Append(value.Value);
            }
          }
          table.Rows.Add(new string[] { field.Name + ":", valueAsString.ToString() });
          break;
        case CustomFieldType.DateTime:
          valueAsString = new StringBuilder();
          if (!string.IsNullOrEmpty(value.Value))
          {
            try
            {
              DateTime valueAsDateTime = DataUtils.DateToLocal(UserSession.LoginUser, DateTime.Parse(value.Value));
              valueAsString.Append(valueAsDateTime.ToString("g", UserSession.LoginUser.CultureInfo));
            }
            catch
            {
              valueAsString.Append(value.Value);
            }
          }
          table.Rows.Add(new string[] { field.Name + ":", valueAsString.ToString() });
          break;
        default:
      table.Rows.Add(new string[] { field.Name + ":", value.Value });
          break;
      }
    }

    rptProperties.DataSource = table;
    rptProperties.DataBind();

  }


}
