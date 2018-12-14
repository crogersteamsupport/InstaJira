using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(LoginHistoryItemProxy))]
  public class LoginHistoryItemProxy
  {
    public LoginHistoryItemProxy() {}
    [DataMember] public int LoginHistoryID { get; set; }
    [DataMember] public int? UserID { get; set; }
    [DataMember] public string IPAddress { get; set; }
    [DataMember] public string Browser { get; set; }
    [DataMember] public string Version { get; set; }
    [DataMember] public string MajorVersion { get; set; }
    [DataMember] public bool? CookiesEnabled { get; set; }
    [DataMember] public string Platform { get; set; }
    [DataMember] public string UserAgent { get; set; }
    [DataMember] public string Language { get; set; }
    [DataMember] public string PixelDepth { get; set; }
    [DataMember] public string ScreenHeight { get; set; }
    [DataMember] public string ScreenWidth { get; set; }
    [DataMember] public string URL { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
          
  }
  
  public partial class LoginHistoryItem : BaseItem
  {
    public LoginHistoryItemProxy GetProxy()
    {
      LoginHistoryItemProxy result = new LoginHistoryItemProxy();
      result.URL = this.URL;
      result.ScreenWidth = this.ScreenWidth;
      result.ScreenHeight = this.ScreenHeight;
      result.PixelDepth = this.PixelDepth;
      result.Language = this.Language;
      result.UserAgent = this.UserAgent;
      result.Platform = this.Platform;
      result.CookiesEnabled = this.CookiesEnabled;
      result.MajorVersion = this.MajorVersion;
      result.Version = this.Version;
      result.Browser = this.Browser;
      result.IPAddress = this.IPAddress;
      result.UserID = this.UserID;
      result.LoginHistoryID = this.LoginHistoryID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
