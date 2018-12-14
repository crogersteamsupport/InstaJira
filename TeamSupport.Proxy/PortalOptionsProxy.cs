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
  [KnownType(typeof(PortalOptionProxy))]
  public class PortalOptionProxy
  {
    public PortalOptionProxy() {}
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string PortalHTMLHeader { get; set; }
    [DataMember] public string PortalHTMLFooter { get; set; }
    [DataMember] public bool? UseRecaptcha { get; set; }
    [DataMember] public string FontFamily { get; set; }
    [DataMember] public string FontColor { get; set; }
    [DataMember] public string PageBackgroundColor { get; set; }
    [DataMember] public bool? UseCompanyInBasic { get; set; }
    [DataMember] public bool? CompanyRequiredInBasic { get; set; }
    [DataMember] public bool? HideUserAssignedTo { get; set; }
    [DataMember] public bool? HideGroupAssignedTo { get; set; }
    [DataMember] public int? BasicPortalColumnWidth { get; set; }
    [DataMember] public bool? DisplayGroups { get; set; }
    [DataMember] public string PortalName { get; set; }
    [DataMember] public bool? KBAccess { get; set; }
    [DataMember] public bool? DisplayProducts { get; set; }
    [DataMember] public bool? HonorSupportExpiration { get; set; }
    [DataMember] public bool? HideCloseButton { get; set; }
    [DataMember] public string Theme { get; set; }
    [DataMember] public int? AdvPortalWidth { get; set; }
    [DataMember] public string BasicPortalDirections { get; set; }
    [DataMember] public bool? DeflectionEnabled { get; set; }
    [DataMember] public bool? DisplayForum { get; set; }
    [DataMember] public bool DisplayFooter { get; set; }
    [DataMember] public bool DisplayPortalPhone { get; set; }
    [DataMember] public bool DisplayAdvProducts { get; set; }
    [DataMember] public bool DisplaySettings { get; set; }
    [DataMember] public bool DisplayLogout { get; set; }
    [DataMember] public bool DisplayAdvKB { get; set; }
    [DataMember] public bool DisplayProductVersion { get; set; }
    [DataMember] public bool RestrictProductVersion { get; set; }
    [DataMember] public string LandingPageHtml { get; set; }
    [DataMember] public bool DisplayLandingPage { get; set; }
    [DataMember] public bool EnableScreenr { get; set; }
    [DataMember] public bool EnableVideoRecording { get; set; }
    [DataMember] public string PublicLandingPageHeader { get; set; }
    [DataMember] public string PublicLandingPageBody { get; set; }
    [DataMember] public bool TwoColumnFields { get; set; }
    [DataMember] public bool DisplayAdvArticles { get; set; }
    [DataMember] public bool DisplayTandC { get; set; }
    [DataMember] public string TermsAndConditions { get; set; }
    [DataMember] public int? RequestType { get; set; }
    [DataMember] public int? RequestGroup { get; set; }
    [DataMember] public bool AutoRegister { get; set; }
    [DataMember] public bool RequestAccess { get; set; }
    [DataMember] public bool DisablePublicMyTickets { get; set; }
    [DataMember] public bool DisplayFbArticles { get; set; }
    [DataMember] public bool DisplayFbKB { get; set; }
    [DataMember] public bool EnableSaExpiration { get; set; }
    [DataMember] public bool AllowSeverityEditing { get; set; }
    [DataMember] public bool AllowNameEditing { get; set; }
    [DataMember] public string PortalForward { get; set; }
    }
}
