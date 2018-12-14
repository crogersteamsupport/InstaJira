using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class UserDevice
  {
  }
  
  public partial class UserDevices
  {

	  public virtual void LoadByUserIDAndDeviceID(int userID, string deviceID)
	  {
		  using (SqlCommand command = new SqlCommand())
		  {
              command.CommandText = "SELECT [UserDeviceID], [UserID], [DeviceID], [DateActivated], [IsActivated] FROM [dbo].[UserDevices] WHERE ([UserID] = @UserID AND [DeviceID] = @DeviceID AND datalength([DeviceID]) != 0)";
              command.CommandType = CommandType.Text;
			  command.Parameters.AddWithValue("UserID", userID);
			  command.Parameters.AddWithValue("DeviceID", deviceID);
			  Fill(command);
		  }
	  }
  }
  
}
