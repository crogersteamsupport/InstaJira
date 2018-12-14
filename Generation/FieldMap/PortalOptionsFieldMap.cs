using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class PortalOptions
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("PortalHTMLHeader", "PortalHTMLHeader", false, false, false);
      _fieldMap.AddMap("PortalHTMLFooter", "PortalHTMLFooter", false, false, false);
      _fieldMap.AddMap("UseRecaptcha", "UseRecaptcha", false, false, false);
      _fieldMap.AddMap("FontFamily", "FontFamily", false, false, false);
      _fieldMap.AddMap("FontColor", "FontColor", false, false, false);
      _fieldMap.AddMap("PageBackgroundColor", "PageBackgroundColor", false, false, false);
      _fieldMap.AddMap("UseCompanyInBasic", "UseCompanyInBasic", false, false, false);
      _fieldMap.AddMap("CompanyRequiredInBasic", "CompanyRequiredInBasic", false, false, false);
      _fieldMap.AddMap("HideUserAssignedTo", "HideUserAssignedTo", false, false, false);
      _fieldMap.AddMap("HideGroupAssignedTo", "HideGroupAssignedTo", false, false, false);
      _fieldMap.AddMap("BasicPortalColumnWidth", "BasicPortalColumnWidth", false, false, false);
      _fieldMap.AddMap("DisplayGroups", "DisplayGroups", false, false, false);
      _fieldMap.AddMap("PortalName", "PortalName", false, false, false);
      _fieldMap.AddMap("KBAccess", "KBAccess", false, false, false);
      _fieldMap.AddMap("DisplayProducts", "DisplayProducts", false, false, false);
      _fieldMap.AddMap("HonorSupportExpiration", "HonorSupportExpiration", false, false, false);
      _fieldMap.AddMap("HideCloseButton", "HideCloseButton", false, false, false);
      _fieldMap.AddMap("Theme", "Theme", false, false, false);
      _fieldMap.AddMap("AdvPortalWidth", "AdvPortalWidth", false, false, false);
      _fieldMap.AddMap("BasicPortalDirections", "BasicPortalDirections", false, false, false);
      _fieldMap.AddMap("DeflectionEnabled", "DeflectionEnabled", false, false, false);
      _fieldMap.AddMap("DisplayForum", "DisplayForum", false, false, false);
      _fieldMap.AddMap("DisplayFooter", "DisplayFooter", false, false, false);
      _fieldMap.AddMap("DisplayPortalPhone", "DisplayPortalPhone", false, false, false);
      _fieldMap.AddMap("DisplayAdvProducts", "DisplayAdvProducts", false, false, false);
      _fieldMap.AddMap("DisplayAdvKB", "DisplayAdvKB", false, false, false);
      _fieldMap.AddMap("DisplayProductVersion", "DisplayProductVersion", false, false, false);
      _fieldMap.AddMap("LandingPageHtml", "LandingPageHtml", false, false, false);
      _fieldMap.AddMap("DisplayLandingPage", "DisplayLandingPage", false, false, false);
      _fieldMap.AddMap("EnableScreenr", "EnableScreenr", false, false, false);
      _fieldMap.AddMap("PublicLandingPageHeader", "PublicLandingPageHeader", false, false, false);
      _fieldMap.AddMap("PublicLandingPageBody", "PublicLandingPageBody", false, false, false);
      _fieldMap.AddMap("TwoColumnFields", "TwoColumnFields", false, false, false);
      _fieldMap.AddMap("DisplayAdvArticles", "DisplayAdvArticles", false, false, false);
      _fieldMap.AddMap("DisplayTandC", "DisplayTandC", false, false, false);
      _fieldMap.AddMap("TermsAndConditions", "TermsAndConditions", false, false, false);
      _fieldMap.AddMap("RequestType", "RequestType", false, false, false);
      _fieldMap.AddMap("RequestGroup", "RequestGroup", false, false, false);
      _fieldMap.AddMap("AutoRegister", "AutoRegister", false, false, false);
      _fieldMap.AddMap("RequestAccess", "RequestAccess", false, false, false);
      _fieldMap.AddMap("DisablePublicMyTickets", "DisablePublicMyTickets", false, false, false);
      _fieldMap.AddMap("EnableSaExpiration", "EnableSaExpiration", false, false, false);
      _fieldMap.AddMap("DisplaySettings", "DisplaySettings", false, false, false);
      _fieldMap.AddMap("DisplayLogout", "DisplayLogout", false, false, false);
      _fieldMap.AddMap("RestrictProductVersion", "RestrictProductVersion", false, false, false);
      _fieldMap.AddMap("EnableVideoRecording", "EnableVideoRecording", false, false, false);
            
    }
  }
  
}
