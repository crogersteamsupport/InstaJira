﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using System.Configuration;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  public class Settings
  {
    private LoginUser _loginUser;
    private int _serviceID;

    public Settings(LoginUser loginUser, string serviceName)
    {
      _loginUser = loginUser;
      _serviceID = Services.GetService(loginUser, serviceName).ServiceID;
    }

    public string ReadString(string key) { return ReadString(key, ""); }

    public string ReadString(string key, string defaultValue)
    {
      NameValueCollection settings = ConfigurationManager.AppSettings;

      bool useDBSettings = false;

      if (settings["UseDBSettings"] == null)
      {
        WriteString("UseDBSettings", "0");
      }
      else
      {
        useDBSettings = settings["UseDBSettings"] == "1";
      }

      if (useDBSettings)
      {
        return ServiceSettings.GetServiceSetting(_loginUser, _serviceID, key, defaultValue).SettingValue;
      }

      string result = settings[key];

      if (string.IsNullOrWhiteSpace(result))
      { 
        // look up in old system settings table, we can take out after all the services have been updated
        result = ServiceSettings.GetServiceSetting(_loginUser, _serviceID, key, defaultValue).SettingValue;
        WriteString(key, result);
      }

      return result;
    }

    public int ReadInt(string key) { return ReadInt(key, 0); }
    public int ReadInt(string key, int defaultValue)
    {
      string value = ReadString(key, defaultValue.ToString());
      try
      {
        return int.Parse(value);
      }
      catch (Exception)
      {
        return defaultValue;        
      }
    }

    public bool ReadBool(string key) { return ReadBool(key, false); }
    public bool ReadBool(string key, bool defaultValue)
    {
      string value = ReadString(key, defaultValue.ToString());
      try
      {
        return bool.Parse(value);

      }
      catch (Exception)
      {
        return defaultValue;
      }
    }

    public void WriteInt(string key, int value) { WriteString(key, value.ToString()); }
    public void WriteBool(string key, bool value) { WriteString(key, value.ToString()); }
    public void WriteString(string key, string value) 
    {
      var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      var settings = configFile.AppSettings.Settings;

      if (settings[key] == null)
      {
        settings.Add(key, value);
      }
      else
      {
        settings[key].Value = value;
      }

      configFile.Save(ConfigurationSaveMode.Modified);
      ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
      
      ServiceSetting setting = ServiceSettings.GetServiceSetting(_loginUser, _serviceID, key, "");
      setting.SettingValue = value;
      setting.Collection.Save();
    }

  }
}
