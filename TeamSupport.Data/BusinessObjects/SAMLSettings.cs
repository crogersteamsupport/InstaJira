using System;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class SAMLSettings
    {
        public void LoadByOrganizationID(int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM SAMLSettings WHERE OrganizationID = @OrganizationID";
                command.CommandText = InjectCustomFields(command.CommandText, "ProductID", ReferenceType.Products);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }
    }
}
