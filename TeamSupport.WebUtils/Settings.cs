using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TeamSupport.Data;
using System.Web.UI;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace TeamSupport.WebUtils
{

  public abstract class BaseSettings
  {
    public abstract void WriteString(string key, string value);
    public abstract string ReadString(string key, string defaultValue);
    private LoginUser _loginUser = TSAuthentication.GetLoginUser();
    public LoginUser LoginUser
    {
      get { return _loginUser; }
    }

    public object ReadSerialObject(string key)
    {
      return ReadJson<object>(key);
    }

    public void WriteSerialObject(string key, object value)
    {
      WriteJson<object>(key, value);
    }

    public T ReadJson<T>(string key)
    {
      string value = "";
      try
      {
        value = ReadString(key, string.Empty);
        if (value == string.Empty) return default(T);
        T result = Activator.CreateInstance<T>();
        MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(value));
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(result.GetType());
        result = (T)serializer.ReadObject(stream);
        stream.Close();
        stream.Dispose();
        return result;
      }
      catch(Exception ex)
      {
        ex.Data["Value"] = value;
        ex.Data["Length"] = value.Length.ToString();
        //ExceptionLogs.LogException(_loginUser, ex, "Read JSON Setting");
        return default(T);
      }
    }

    public void WriteJson<T>(string key, T value)
    {
      if (value == null)
      {
        WriteString(key, "");
        return;
      }
      DataContractJsonSerializer serializer = new DataContractJsonSerializer(value.GetType());
      MemoryStream stream = new MemoryStream();
      serializer.WriteObject(stream, value);
      string json = Encoding.Default.GetString(stream.ToArray());
      stream.Dispose();
      WriteString(key, json);
    }

    public string ReadString(string key) { return ReadString(key, ""); }
    public int ReadInt(string key) { return ReadInt(key, 0); }
    public int ReadInt(string key, int defaultValue)
    {
      string value = ReadString(key, defaultValue.ToString());
      return int.Parse(value);
    }

    public int[] ReadIntArray(string key)
    {
      List<int> result = new List<int>();
      string value = ReadString(key, "");
      if (value != "")
      {
        string[] items = value.Split(',');
        foreach (string item in items)
        {
          result.Add(int.Parse(item.Trim()));
        }
      }
      return result.ToArray();
    }

    public void WriteIntArray(string key, int[] value)
    {
      StringBuilder builder = new StringBuilder();
      foreach (int i in value)
      {
        if (builder.Length > 0) builder.Append(",");
        builder.Append(i.ToString());
      }
      WriteString(key, builder.ToString());
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

  }

  public class SessionSettings : BaseSettings
  {
    public override string ReadString(string key, string defaultValue)
    {
            object value = null; // HttpContext.Current.Session["SessionSetting-" + key];
      if (value == null) return defaultValue;
      else return (string)value;
    }

    public override void WriteString(string key, string value)
    {
      //HttpContext.Current.Session["SessionSetting-" + key] = value;
    }
  }

  public class UserDatabase : BaseSettings
  {
    public override string ReadString(string key, string defaultValue)
    {
      return UserSettings.ReadString(LoginUser, key, defaultValue);
    }

    public override void WriteString(string key, string value)
    {
      UserSettings.WriteString(LoginUser, key, value);
    }
  }

  public class OrganizationDatabase : BaseSettings
  {
    public override string ReadString(string key, string defaultValue)
    {
      return OrganizationSettings.ReadString(LoginUser, key, defaultValue);
    }

    public override void WriteString(string key, string value)
    {
      OrganizationSettings.WriteString(LoginUser, key, value);
    }
  }

  public class SystemDatabase : BaseSettings
  {
    public override string ReadString(string key, string defaultValue)
    {
      return SystemSettings.ReadString(LoginUser, key, defaultValue);
    }

    public override void WriteString(string key, string value)
    {
      SystemSettings.WriteString(LoginUser, key, value);
    }
  }

  public class Settings
  {
    public class Keys
    {
      public const string SelectedOrganizationID = "SelectedOrganizationID";
      public const string SelectedProductID = "SelectedProductID";
      public const string SelectedVersionID = "SelectedVersionID";
      public const string SelectedTicketID = "SelectedTicketID";
      public const string SelectedTabText = "SelectedTabText";
    }
    
    public static UserDatabase UserDB
    {
      get { return new UserDatabase(); }
    }

    public static OrganizationDatabase OrganizationDB
    {
      get { return new OrganizationDatabase(); }
    }

    public static SystemDatabase SystemDB
    {
      get { return new SystemDatabase(); }
    }

    public static SessionSettings Session
    {
      get { return new SessionSettings(); }
    }
  
  }

}
