using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class UserProductsViewItem
  {
  }
  
  public partial class UserProductsView
  {
	  public void LoadByContactIDMissingCompanyReference(int contactID)
	  {
		  using (SqlCommand command = new SqlCommand())
		  {
			  command.CommandText = @"
      SELECT
			upv.*
		FROM
			UserProductsView upv
			LEFT JOIN OrganizationProducts op
				ON upv.ProductID = op.ProductID
				AND upv.ProductVersionID = op.ProductVersionID
				AND op.OrganizationID = (SELECT OrganizationID FROM Users WHERE UserID = @UserID)
		WHERE
			upv.UserID = @UserID
			AND op.OrganizationProductID IS NULL";
			  command.CommandType = CommandType.Text;
			  command.Parameters.AddWithValue("@UserID", contactID);
			  //command.Parameters.AddWithValue("@start", start);
			  //command.Parameters.AddWithValue("@end", end);
			  Fill(command);
		  }
	  }
  }
  
}
