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
  [KnownType(typeof(TaskAssociationsViewItemProxy))]
  public class TaskAssociationsViewItemProxy
  {
    public TaskAssociationsViewItemProxy() {}
    [DataMember] public int ReminderID { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public int RefType { get; set; }
    [DataMember] public int? TicketNumber { get; set; }
    [DataMember] public string TicketName { get; set; }
    [DataMember] public string User { get; set; }
    [DataMember] public string Company { get; set; }
    [DataMember] public string Group { get; set; }
    [DataMember] public string Product { get; set; }
    [DataMember] public string Contact { get; set; }
          
  }
  
  public partial class TaskAssociationsViewItem : BaseItem
  {
    public TaskAssociationsViewItemProxy GetProxy()
    {
      TaskAssociationsViewItemProxy result = new TaskAssociationsViewItemProxy();
      result.Contact = this.Contact;
      result.Product = this.Product;
      result.Group = this.Group;
      result.Company = this.Company;
      result.User = this.User;
      result.TicketName = this.TicketName;
      result.TicketNumber = this.TicketNumber;
      result.RefType = this.RefType;
      result.RefID = this.RefID;
      result.ReminderID = this.ReminderID;
       
       
       
      return result;
    }	
  }
}
