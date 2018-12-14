using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;

namespace TeamSupport.Data
{
    public partial class DeletedTicket
    {
    }

    public partial class DeletedTickets
    {
        public void LoadByOrganizationID(int organizationId, NameValueCollection filters, int? PageNumber = null, int? PageSize = null)
        {
            //Get the column names
            LoadColumns("DeletedTickets");

            string orderBy = "ORDER BY DateDeleted DESC";
            string sql = string.Empty;

            using (SqlCommand command = new SqlCommand())
            {
                SqlParameterCollection filterParameters = command.Parameters;
                string whereClause = DataUtils.BuildWhereClausesFromFilters(this.LoginUser, this, organizationId, filters, ReferenceType.DeletedTickets, "ID", null, ref filterParameters);
                sql = @"
                SELECT 
                    *
                FROM
                    DeletedTickets
                WHERE
                    OrganizationID = @organizationId " + whereClause + " " + orderBy;

                sql = DataUtils.AddPaging(sql, PageSize, PageNumber, command);

                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@organizationId", organizationId);

                Fill(command);
            }
        }
    }

}
