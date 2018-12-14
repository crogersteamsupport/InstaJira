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
  [KnownType(typeof(EmailPostProxy))]
  public class EmailPostProxy
  {
    public EmailPostProxy() {}
    [DataMember] public int EmailPostID { get; set; }
    [DataMember] public EmailPostType EmailPostType { get; set; }
    [DataMember] public int HoldTime { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public string Param1 { get; set; }
    [DataMember] public string Param2 { get; set; }
    [DataMember] public string Param3 { get; set; }
    [DataMember] public string Param4 { get; set; }
    [DataMember] public string Param5 { get; set; }
    [DataMember] public string Param6 { get; set; }
    [DataMember] public string Param7 { get; set; }
    [DataMember] public string Param8 { get; set; }
    [DataMember] public string Param9 { get; set; }
    [DataMember] public string Param10 { get; set; }
    [DataMember] public string Text1 { get; set; }
    [DataMember] public string Text2 { get; set; }
    [DataMember] public string Text3 { get; set; }
    [DataMember] public string LockProcessID { get; set; }
          
  }
  
  public partial class EmailPost : BaseItem
  {
    public EmailPostProxy GetProxy()
    {
      EmailPostProxy result = new EmailPostProxy();
      result.LockProcessID = this.LockProcessID;
      result.Text3 = this.Text3;
      result.Text2 = this.Text2;
      result.Text1 = this.Text1;
      result.Param10 = this.Param10;
      result.Param9 = this.Param9;
      result.Param8 = this.Param8;
      result.Param7 = this.Param7;
      result.Param6 = this.Param6;
      result.Param5 = this.Param5;
      result.Param4 = this.Param4;
      result.Param3 = this.Param3;
      result.Param2 = this.Param2;
      result.Param1 = this.Param1;
      result.CreatorID = this.CreatorID;
      result.HoldTime = this.HoldTime;
      result.EmailPostType = this.EmailPostType;
      result.EmailPostID = this.EmailPostID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
