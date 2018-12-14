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
  [KnownType(typeof(TicketAutomationTriggerLogicItemProxy))]
  public class TicketAutomationTriggerLogicItemProxy
  {
    public TicketAutomationTriggerLogicItemProxy() {}
    [DataMember] public int TriggerLogicID { get; set; }
    [DataMember] public int TriggerID { get; set; }
    [DataMember] public int TableID { get; set; }
    [DataMember] public int FieldID { get; set; }
    [DataMember] public string Measure { get; set; }
    [DataMember] public string TestValue { get; set; }
    [DataMember] public bool MatchAll { get; set; }
          
  }
  
  public partial class TicketAutomationTriggerLogicItem : BaseItem
  {
    public TicketAutomationTriggerLogicItemProxy GetProxy()
    {
      TicketAutomationTriggerLogicItemProxy result = new TicketAutomationTriggerLogicItemProxy();
      result.MatchAll = this.MatchAll;
      result.TestValue = this.TestValue;
      result.Measure = this.Measure;
      result.FieldID = this.FieldID;
      result.TableID = this.TableID;
      result.TriggerID = this.TriggerID;
      result.TriggerLogicID = this.TriggerLogicID;
       
       
       
      return result;
    }	
  }
}
