using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class SearchCustomFilter
  {
  }
  
  public partial class SearchCustomFilters
  {
    public void LoadByUserID(int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM dbo.SearchCustomFilters WHERE UserID = @UserID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command, "SearchCustomFilters");
      }
    }

    public string GetJoinClause()
    {
      StringBuilder resultBuilder = new StringBuilder();

      int counterSuffix = 0;
      foreach (SearchCustomFilter filter in this)
      {

        if (filter.TableID == -1)
        {
          counterSuffix++;

          resultBuilder.Append(@"	
          JOIN
	        (
		        SELECT
			        stv.TicketID
		        FROM
			        dbo.TicketsView stv
			        JOIN dbo.CustomFields cf
		            ON cf.RefType = 17
		            AND stv.TicketTypeID = cf.AuxID
		          JOIN dbo.CustomValues cv
		            ON cf.CustomFieldID = cv.CustomFieldID
		            AND stv.TicketID = cv.RefID
		        WHERE
			        cf.CustomFieldID = " + filter.FieldID.ToString() + @"
			        AND cv.CustomValue " + filter.Measure + " '" + filter.TestValue + @"'
	        ) table" + counterSuffix.ToString() + @"
		        ON tv.TicketID = table" + counterSuffix.ToString() + @".TicketID ");
        }
      }

      return resultBuilder.ToString();
    }

    public string ConvertToWhereClause()
    {
      StringBuilder resultBuilder = new StringBuilder();

      ReportTableFields ticketsViewFields = new ReportTableFields(base.LoginUser);
      ticketsViewFields.LoadByReportTableID(10);

      foreach (SearchCustomFilter filter in this)
      {
        if (filter.TableID == 10)
        {
          string fieldName = ticketsViewFields.FindByReportTableFieldID(filter.FieldID).FieldName;

          if (string.Equals(filter.Measure, "contains", StringComparison.OrdinalIgnoreCase))
          {
            resultBuilder.Append(" AND tv.[" + fieldName + "] LIKE '%" + filter.TestValue + "%'");
          }
          else
          {
            resultBuilder.Append(" AND tv.[" + fieldName + "] " + filter.Measure + " '" + filter.TestValue + "'");
          }
        }
      }

      return resultBuilder.ToString();
    }

    public string ConvertToWikiEquivalentWhereClause()
    {
      StringBuilder resultBuilder = new StringBuilder();

      ReportTableFields ticketsViewFields = new ReportTableFields(base.LoginUser);
      ticketsViewFields.LoadByReportTableID(10);

      foreach (SearchCustomFilter filter in this)
      {
        string fieldName = ticketsViewFields.FindByReportTableFieldID(filter.FieldID).FieldName;
        string wikiEquivalentFieldName = DataUtils.GetWikiEquivalentFieldName(fieldName);
        resultBuilder.Append(" AND wav.[" + wikiEquivalentFieldName + "] " + filter.Measure + " '" + filter.TestValue + "'");
        // It is pending to code around custom fields and any other fieldName that is not part of the ticketsView
      }

      return resultBuilder.ToString();
    }

    public string ConvertToNotesEquivalentWhereClause()
    {
      StringBuilder resultBuilder = new StringBuilder();

      ReportTableFields ticketsViewFields = new ReportTableFields(base.LoginUser);
      ticketsViewFields.LoadByReportTableID(10);

      foreach (SearchCustomFilter filter in this)
      {
        string fieldName = ticketsViewFields.FindByReportTableFieldID(filter.FieldID).FieldName;
        string notesEquivalentFieldName = DataUtils.GetNotesEquivalentFieldName(fieldName);
        resultBuilder.Append(" AND nv.[" + notesEquivalentFieldName + "] " + filter.Measure + " '" + filter.TestValue + "'");
      }

      return resultBuilder.ToString();
    }

    public string ConvertToProductVersionEquivalentWhereClause()
    {
      StringBuilder resultBuilder = new StringBuilder();

      ReportTableFields ticketsViewFields = new ReportTableFields(base.LoginUser);
      ticketsViewFields.LoadByReportTableID(10);

      foreach (SearchCustomFilter filter in this)
      {
        string fieldName = ticketsViewFields.FindByReportTableFieldID(filter.FieldID).FieldName;
        string productVersionEquivalentFieldName = DataUtils.GetProductVersionsEquivalentFieldName(fieldName);
        resultBuilder.Append(" AND pvv.[" + productVersionEquivalentFieldName + "] " + filter.Measure + " '" + filter.TestValue + "'");
      }

      return resultBuilder.ToString();
    }

    public string ConvertToWaterCoolerEquivalentWhereClause()
    {
      StringBuilder resultBuilder = new StringBuilder();

      ReportTableFields ticketsViewFields = new ReportTableFields(base.LoginUser);
      ticketsViewFields.LoadByReportTableID(10);

      foreach (SearchCustomFilter filter in this)
      {
        string fieldName = ticketsViewFields.FindByReportTableFieldID(filter.FieldID).FieldName;
        string waterCoolerEquivalentFieldName = DataUtils.GetWaterCoolerEquivalentFieldName(fieldName);
        resultBuilder.Append(" AND wcv.[" + waterCoolerEquivalentFieldName + "] " + filter.Measure + " '" + filter.TestValue + "'");
      }

      return resultBuilder.ToString();
    }

  }
  
}
