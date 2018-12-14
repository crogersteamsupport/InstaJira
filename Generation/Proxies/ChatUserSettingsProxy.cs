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
  [KnownType(typeof(ChatUserSettingProxy))]
  public class ChatUserSettingProxy
  {
    public ChatUserSettingProxy() {}
    [DataMember] public int UserID { get; set; }
    [DataMember] public int CurrentChatID { get; set; }
    [DataMember] public bool IsAvailable { get; set; }
    [DataMember] public int LastChatRequestID { get; set; }
          
  }
  
  public partial class ChatUserSetting : BaseItem
  {
    public ChatUserSettingProxy GetProxy()
    {
      ChatUserSettingProxy result = new ChatUserSettingProxy();
      result.LastChatRequestID = this.LastChatRequestID;
      result.IsAvailable = this.IsAvailable;
      result.CurrentChatID = this.CurrentChatID;
      result.UserID = this.UserID;
       
       
       
      return result;
    }	
  }
}
