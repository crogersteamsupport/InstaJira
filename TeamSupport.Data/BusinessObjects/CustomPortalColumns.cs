using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CustomPortalColumn
  {
  }
  
  public partial class CustomPortalColumns
  {
        public static List<CustomPortalColumnProxy> GetDefaultColumns(int organizationID)
        {
            List<CustomPortalColumnProxy> defaultColumns = new List<CustomPortalColumnProxy>();
            CustomPortalColumnProxy column1 = new CustomPortalColumnProxy();
            column1.OrganizationID = organizationID;
            column1.Position = 0;
            column1.StockFieldID = 158;
            column1.CustomColumnID = 15;
            column1.FieldText = "Name";
            defaultColumns.Add(column1);

            CustomPortalColumnProxy column2 = new CustomPortalColumnProxy();
            column2.OrganizationID = organizationID;
            column2.Position = 1;
            column2.StockFieldID = 146;
            column2.CustomColumnID = 16;
            column2.FieldText = "TicketNumber";
            defaultColumns.Add(column2);


            CustomPortalColumnProxy column3 = new CustomPortalColumnProxy();
            column3.OrganizationID = organizationID;
            column3.Position = 2;
            column3.StockFieldID = 163;
            column3.CustomColumnID = 18;
            column3.FieldText = "DateCreated";
            defaultColumns.Add(column3);

            CustomPortalColumnProxy column4 = new CustomPortalColumnProxy();
            column3.OrganizationID = organizationID;
            column3.Position = 3;
            column3.StockFieldID = 134;
            column3.CustomColumnID = 19;
            column3.FieldText = "TicketID";
            defaultColumns.Add(column3);

            return defaultColumns;
        }
    }
  
}
