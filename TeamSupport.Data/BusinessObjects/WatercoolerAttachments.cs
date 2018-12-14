using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class WatercoolerAttachment
  {
  }
  
  public partial class WatercoolerAttachments
  {
      public void LoadByType(int messageID, WaterCoolerAttachmentType type)
      {
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = "SET NOCOUNT OFF; SELECT [MessageID], [AttachmentID], [RefType], [CreatorID], [DateCreated] FROM [dbo].[WatercoolerAttachments] WHERE ([MessageID] = @MessageID and [RefType] = @RefType);";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("MessageID", messageID);
              command.Parameters.AddWithValue("RefType", type);
              Fill(command);
          }
      }
  }
  
}
