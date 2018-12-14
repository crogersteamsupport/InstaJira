using System;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class SlaTickets
    {
        public void LoadPending()
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT * FROM SlaTickets WHERE IsPending = 1 ORDER BY DateModified";
                command.CommandType = CommandType.Text;
                Fill(command);
            }
        }
    }
}
