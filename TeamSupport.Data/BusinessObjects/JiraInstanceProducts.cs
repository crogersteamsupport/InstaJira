using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
	public partial class JiraInstanceProductsItem
	{
	}

	public partial class JiraInstanceProducts
	{
		public void LoadByOrganizationAndLinkId(int organizationId, int crmLinkId, string crmType)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText =
				@"
					 SELECT 
						JiraInstanceProducts.* 
					 FROM 
						JiraInstanceProducts
						JOIN CRMLinkTable
							ON JiraInstanceProducts.CrmLinkId = CRMLinkTable.CRMLinkID
					 WHERE
						CRMLinkTable.CRMLinkID = @CrmLinkId
						AND CRMLinkTable.OrganizationID = @OrganizationId
						AND CRMLinkTable.CRMType = @CrmType
				  ";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@CrmLinkId", crmLinkId);
				command.Parameters.AddWithValue("@OrganizationId", organizationId);
				command.Parameters.AddWithValue("@CrmType", crmType);

				Fill(command);
			}
		}

    public void LoadByProductAndOrganization(int productId, int organizationId, string crmType)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
        @"
					 SELECT 
						JiraInstanceProducts.* 
					 FROM 
						JiraInstanceProducts
						JOIN CRMLinkTable
							ON JiraInstanceProducts.CrmLinkId = CRMLinkTable.CRMLinkID
					 WHERE
						JiraInstanceProducts.ProductId = @ProductId
						AND CRMLinkTable.OrganizationID = @OrganizationId
						AND CRMLinkTable.CRMType = @CrmType
				  ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductId", productId);
        command.Parameters.AddWithValue("@OrganizationId", organizationId);
        command.Parameters.AddWithValue("@CrmType", crmType);

        Fill(command);
      }
    }
	}
}
