using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TokStorageItem 
  {

  }

  public partial class TokStorage : BaseCollection
  {
      public virtual void LoadByArchiveID(string archiveID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = "SET NOCOUNT OFF; SELECT [OrganizationID], [AmazonPath], [CreatedDate], [CreatorID], [ArchiveID] FROM [dbo].[TokStorage] WHERE ([archiveID] = @archiveID);";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("archiveID", archiveID);
              Fill(command);
          }
      }
  }
  
}
