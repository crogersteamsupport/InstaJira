using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ChatSetting
  {
  }
  
  public partial class ChatSettings
  {

    public static ChatSetting GetSetting(LoginUser loginUser, int organizationID)
    {
      ChatSetting setting = GetChatSetting(loginUser, organizationID);
      if (setting == null)
      {
        setting = (new ChatSettings(loginUser)).AddNewChatSetting();
        setting.OrganizationID = organizationID;
        setting.UseCss = false;
        setting.ClientCss = "";
        setting.Collection.Save();
        
      }

      return setting;

    }

  }
  
}
