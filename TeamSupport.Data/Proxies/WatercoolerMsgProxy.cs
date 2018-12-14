using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(WatercoolerMsgItemProxy))]
  public class WatercoolerMsgItemProxy
  {
    public WatercoolerMsgItemProxy() {}
    [DataMember] public int MessageID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string TimeStamp { get; set; }
    [DataMember] public string Message { get; set; }
    [DataMember] public int? MessageParent { get; set; }
    [DataMember] public bool IsDeleted { get; set; }
    [DataMember] public DateTime LastModified { get; set; }
    [DataMember] public string UserName { get; set; }              
    [DataMember] public bool NeedsIndexing { get; set; }
  }
  
  public partial class WatercoolerMsgItem : BaseItem
  {
    public WatercoolerMsgItemProxy GetProxy()
    {
      WatercoolerMsgItemProxy result = new WatercoolerMsgItemProxy();
      result.NeedsIndexing = this.NeedsIndexing;
      result.IsDeleted = this.IsDeleted;
      result.MessageParent = this.MessageParent;
      result.Message = (MakeLink(this.Message));
      result.OrganizationID = this.OrganizationID;
      result.UserID = this.UserID;
      result.MessageID = this.MessageID;
      result.UserName = Users.GetUserFullName(BaseCollection.LoginUser, this.UserID);

      result.TimeStamp = DateTime.SpecifyKind(this.TimeStampUtc, DateTimeKind.Utc).ToString("o");
      result.LastModified = DateTime.SpecifyKind(this.LastModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }

    public string MakeLink(string txt)
    {
        //Regex regx = new Regex("http://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);
        //(http://|)(www\.)?([^\.]+)\.(\w{2})$
        //(?<http>(http:[/][/]|www.)([a-z]|[A-Z]|[0-9]|[/.]|[~])*)
        Regex regx = new Regex("http[s]?://[^\\s<>\"]+|www\\.[^\\s<>\"]+", RegexOptions.IgnoreCase);
        MatchCollection mactches = regx.Matches(txt);

        foreach (Match match in mactches)
        {
            txt = txt.Replace(match.Value, "<a target='_blank' class='ts-link ui-state-default' href='" + (match.Value.StartsWith("http://") ? match.Value : "http://" + match.Value) + "'>" + match.Value + "</a>");
        }

        return txt;
    }
  }
}
