using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ServiceSetting
  {
  }
  
  public partial class ServiceSettings
  {

    public virtual void LoadByKey(int serviceID, string key)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ServiceSettings WHERE ServiceID = @ServiceID AND  SettingKey = @Key";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("Key", key);
        command.Parameters.AddWithValue("ServiceID", serviceID);
        Fill(command);
      }
    }

    public static ServiceSetting GetServiceSetting(LoginUser loginUser, int serviceID, string key, string defaultValue)
    {
      ServiceSettings serviceSettings = new ServiceSettings(loginUser);
      serviceSettings.LoadByKey(serviceID, key);
      if (serviceSettings.IsEmpty)
      {
        ServiceSetting serviceSetting = (new ServiceSettings(loginUser)).AddNewServiceSetting();
        serviceSetting.ServiceID = serviceID;
        serviceSetting.SettingKey = key;
        serviceSetting.SettingValue = defaultValue;
        serviceSetting.Collection.Save();
        return serviceSetting;
      }
      else
      {
        return serviceSettings[0];
      }
    }
  }
  
}
