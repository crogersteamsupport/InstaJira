using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CustomerRelationship
  {
  }
  
  public partial class CustomerRelationships
  {
    public virtual void LoadByCustomerAndRelatedCustomerIDs(int customerID, int relatedCustomerID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SET NOCOUNT OFF; 
        SELECT 
            *
        FROM 
            [dbo].[CustomerRelationships] 
        WHERE 
            [CustomerID] = @CustomerID
            AND RelatedCustomerID = @RelatedCustomerID;";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CustomerID", customerID);
        command.Parameters.AddWithValue("RelatedCustomerID", relatedCustomerID);
        Fill(command);
      }
    }
  }
  
}
