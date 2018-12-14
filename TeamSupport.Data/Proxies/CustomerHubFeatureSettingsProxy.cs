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
  [KnownType(typeof(CustomerHubFeatureSettingProxy))]
  public class CustomerHubFeatureSettingProxy
  {
    public CustomerHubFeatureSettingProxy() {}
    [DataMember] public int CustomerHubFeatureSettingID { get; set; }
    [DataMember] public int CustomerHubID { get; set; }
    [DataMember] public bool EnableKnowledgeBase { get; set; }
    [DataMember] public bool EnableProducts { get; set; }
    [DataMember] public bool EnableTicketCreation { get; set; }
    [DataMember] public bool EnableMyTickets { get; set; }
    [DataMember] public bool EnableOrganizationTickets { get; set; }
    [DataMember] public bool EnableWiki { get; set; }
    [DataMember] public bool EnableTicketGroupSelection { get; set; }
    [DataMember] public bool EnableTicketProductSelection { get; set; }
    [DataMember] public bool EnableTicketProductVersionSelection { get; set; }
    [DataMember] public int? DefaultTicketTypeID { get; set; }
    [DataMember] public int? DefaultGroupTypeID { get; set; }
    [DataMember] public bool EnableCustomerProductAssociation { get; set; }
    [DataMember] public bool EnableChat { get; set; }
    [DataMember] public bool EnableCommunity { get; set; }
    [DataMember] public bool EnableScreenRecording { get; set; }
    [DataMember] public bool EnableVideoRecording { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int? ModifierID { get; set; }
    [DataMember] public bool EnableTicketSeverity { get; set; }
    [DataMember] public bool EnableTicketSeverityModification { get; set; }
    [DataMember] public bool RestrictProductVersions { get; set; }
    [DataMember] public bool EnableTicketNameModification { get; set; }
    [DataMember] public int KnowledgeBaseSortTypeID { get; set; }
    [DataMember] public int CommunitySortTypeID { get; set; }
    [DataMember] public bool EnableAnonymousProductAssociation { get; set; }
    [DataMember] public bool EnableCustomerSpecificKB { get; set; }
    [DataMember] public bool EnableCustomFieldModification { get; set; }
    [DataMember] public bool EnableProductFamilyFiltering { get; set; }
    [DataMember] public int ChatGroupID { get; set; }
          
  }
  
  public partial class CustomerHubFeatureSetting : BaseItem
  {
    public CustomerHubFeatureSettingProxy GetProxy()
    {
      CustomerHubFeatureSettingProxy result = new CustomerHubFeatureSettingProxy();
      result.ChatGroupID = this.ChatGroupID;
      result.EnableProductFamilyFiltering = this.EnableProductFamilyFiltering;
      result.EnableCustomFieldModification = this.EnableCustomFieldModification;
      result.EnableCustomerSpecificKB = this.EnableCustomerSpecificKB;
      result.EnableAnonymousProductAssociation = this.EnableAnonymousProductAssociation;
      result.CommunitySortTypeID = this.CommunitySortTypeID;
      result.KnowledgeBaseSortTypeID = this.KnowledgeBaseSortTypeID;
      result.EnableTicketNameModification = this.EnableTicketNameModification;
      result.RestrictProductVersions = this.RestrictProductVersions;
      result.EnableTicketSeverityModification = this.EnableTicketSeverityModification;
      result.EnableTicketSeverity = this.EnableTicketSeverity;
      result.ModifierID = this.ModifierID;
      result.EnableVideoRecording = this.EnableVideoRecording;
      result.EnableScreenRecording = this.EnableScreenRecording;
      result.EnableCommunity = this.EnableCommunity;
      result.EnableChat = this.EnableChat;
      result.EnableCustomerProductAssociation = this.EnableCustomerProductAssociation;
      result.DefaultGroupTypeID = this.DefaultGroupTypeID;
      result.DefaultTicketTypeID = this.DefaultTicketTypeID;
      result.EnableTicketProductVersionSelection = this.EnableTicketProductVersionSelection;
      result.EnableTicketProductSelection = this.EnableTicketProductSelection;
      result.EnableTicketGroupSelection = this.EnableTicketGroupSelection;
      result.EnableWiki = this.EnableWiki;
      result.EnableOrganizationTickets = this.EnableOrganizationTickets;
      result.EnableMyTickets = this.EnableMyTickets;
      result.EnableTicketCreation = this.EnableTicketCreation;
      result.EnableProducts = this.EnableProducts;
      result.EnableKnowledgeBase = this.EnableKnowledgeBase;
      result.CustomerHubID = this.CustomerHubID;
      result.CustomerHubFeatureSettingID = this.CustomerHubFeatureSettingID;
       
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
