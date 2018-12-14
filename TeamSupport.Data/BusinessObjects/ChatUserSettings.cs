using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ChatUserSetting
  {
  }
  
  public partial class ChatUserSettings
  {
    public static ChatUserSetting GetSetting(LoginUser loginUser, int userID)
    {
      ChatUserSetting setting = GetChatUserSetting(loginUser, userID);
      if (setting == null)
      {
        setting = (new ChatUserSettings(loginUser)).AddNewChatUserSetting();

        setting.CurrentChatID = -1;
        setting.IsAvailable = true;
        setting.LastChatRequestID = -1;
        setting.UserID = userID;
        
      }

      return setting;
    
    }
  }
  
}
