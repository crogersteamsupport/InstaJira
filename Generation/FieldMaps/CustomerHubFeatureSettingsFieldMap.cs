using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class CustomerHubFeatureSettings
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CustomerHubFeatureSettingID", "CustomerHubFeatureSettingID", false, false, false);
      _fieldMap.AddMap("CustomerHubID", "CustomerHubID", false, false, false);
      _fieldMap.AddMap("EnableKnowledgeBase", "EnableKnowledgeBase", false, false, false);
      _fieldMap.AddMap("EnableProducts", "EnableProducts", false, false, false);
      _fieldMap.AddMap("EnableTicketCreation", "EnableTicketCreation", false, false, false);
      _fieldMap.AddMap("EnableMyTickets", "EnableMyTickets", false, false, false);
      _fieldMap.AddMap("EnableOrganizationTickets", "EnableOrganizationTickets", false, false, false);
      _fieldMap.AddMap("EnableWiki", "EnableWiki", false, false, false);
      _fieldMap.AddMap("EnableTicketGroupSelection", "EnableTicketGroupSelection", false, false, false);
      _fieldMap.AddMap("EnableTicketProductSelection", "EnableTicketProductSelection", false, false, false);
      _fieldMap.AddMap("EnableTicketProductVersionSelection", "EnableTicketProductVersionSelection", false, false, false);
      _fieldMap.AddMap("DefaultTicketTypeID", "DefaultTicketTypeID", false, false, false);
      _fieldMap.AddMap("DefaultGroupTypeID", "DefaultGroupTypeID", false, false, false);
      _fieldMap.AddMap("EnableCustomerProductAssociation", "EnableCustomerProductAssociation", false, false, false);
      _fieldMap.AddMap("EnableChat", "EnableChat", false, false, false);
      _fieldMap.AddMap("EnableCommunity", "EnableCommunity", false, false, false);
      _fieldMap.AddMap("EnableScreenRecording", "EnableScreenRecording", false, false, false);
      _fieldMap.AddMap("EnableVideoRecording", "EnableVideoRecording", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("EnableTicketSeverity", "EnableTicketSeverity", false, false, false);
      _fieldMap.AddMap("EnableTicketSeverityModification", "EnableTicketSeverityModification", false, false, false);
      _fieldMap.AddMap("RestrictProductVersions", "RestrictProductVersions", false, false, false);
      _fieldMap.AddMap("EnableTicketNameModification", "EnableTicketNameModification", false, false, false);
      _fieldMap.AddMap("KnowledgeBaseSortTypeID", "KnowledgeBaseSortTypeID", false, false, false);
      _fieldMap.AddMap("CommunitySortTypeID", "CommunitySortTypeID", false, false, false);
      _fieldMap.AddMap("EnableAnonymousProductAssociation", "EnableAnonymousProductAssociation", false, false, false);
      _fieldMap.AddMap("EnableCustomerSpecificKB", "EnableCustomerSpecificKB", false, false, false);
      _fieldMap.AddMap("EnableCustomFieldModification", "EnableCustomFieldModification", false, false, false);
      _fieldMap.AddMap("EnableProductFamilyFiltering", "EnableProductFamilyFiltering", false, false, false);
            
    }
  }
  
}
