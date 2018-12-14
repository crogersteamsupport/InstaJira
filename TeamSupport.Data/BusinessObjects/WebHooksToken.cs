using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Data
{
	public partial class WebHooksToken
	{
		/// <summary>
		/// Loads the record with the matching OrganizationID
		/// </summary>
		/// <param name="organizationID"></param>
		public void LoadByOrganizationID(int organizationID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "SELECT * FROM WebHooksToken WHERE OrganizationID = @OrganizationID";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@OrganizationID", organizationID);
				Fill(command);
			}
		}
	}
}
