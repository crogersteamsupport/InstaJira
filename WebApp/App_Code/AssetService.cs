using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using System.Runtime.Serialization;
using dtSearch.Engine;
using System.Net;
using System.IO;
using System.Globalization;

namespace TSWebServices
{
  [ScriptService]
  [WebService(Namespace = "http://teamsupport.com/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class AssetService : System.Web.Services.WebService
  {

    public AssetService()
    {

      //Uncomment the following line if using designed components 
      //InitializeComponent(); 
    }

    [WebMethod]
    public AutocompleteItem[] FindAsset(string searchTerm)
    {
      Assets assets = new Assets(TSAuthentication.GetLoginUser());
      assets.LoadByLikeNameOrSerial(TSAuthentication.OrganizationID, searchTerm, 25);

      List<AutocompleteItem> list = new List<AutocompleteItem>();
      foreach (Asset asset in assets)
      {
        if (!string.IsNullOrEmpty(asset.SerialNumber)) list.Add(new AutocompleteItem(string.Format("{0} ({1})", asset.Name, asset.SerialNumber) , asset.AssetID.ToString()));
        else list.Add(new AutocompleteItem(asset.Name, asset.AssetID.ToString()));
      }

      return list.ToArray();
    }

    [WebMethod]
    public AssetsViewItemProxy GetAsset(int assetID)
    {
      AssetsViewItem asset = AssetsView.GetAssetsViewItem(TSAuthentication.GetLoginUser(), assetID);
      if (asset.OrganizationID != TSAuthentication.OrganizationID) return null;
      return asset.GetProxy();
    }

    [WebMethod]
    public AssetAssignmentsViewItemProxy[] GetAssetAssignments(int assetID)
    {
      AssetAssignmentsView assetAssignments = new AssetAssignmentsView(TSAuthentication.GetLoginUser());
      assetAssignments.LoadByAssetID(assetID);
      List<AssetAssignmentsViewItemProxy> result = new List<AssetAssignmentsViewItemProxy>();
      foreach (AssetAssignmentsViewItem assetAssignment in assetAssignments)
      {
        result.Add(assetAssignment.GetProxy());
      }
      return result.ToArray();
    }

    [WebMethod]
    public int SaveAsset(string data)
    {
      NewAssetSave info;
      try
      {
        info = Newtonsoft.Json.JsonConvert.DeserializeObject<NewAssetSave>(data);
      }
      catch (Exception e)
      {
        return -1;
      }

      LoginUser loginUser = TSAuthentication.GetLoginUser();
      Assets assets = new Assets(loginUser);
      Asset asset = assets.AddNewAsset();

      asset.OrganizationID =      TSAuthentication.OrganizationID;
      asset.Name =                info.Name;
      asset.ProductID =           info.ProductID;
      asset.ProductVersionID =    info.ProductVersionID;
      asset.SerialNumber =        info.SerialNumber;
      asset.WarrantyExpiration =  DataUtils.DateToUtc(TSAuthentication.GetLoginUser(), info.WarrantyExpiration);
      asset.Notes =               info.Notes;
      //Location 1=assigned (shipped), 2=warehouse, 3=junkyard
      asset.Location =            "2";

      asset.DateCreated =   DateTime.UtcNow;
      asset.DateModified =  DateTime.UtcNow;
      asset.CreatorID =     loginUser.UserID;
      asset.ModifierID =    loginUser.UserID;

      asset.Collection.Save();

      string description = String.Format("Created asset {0} ", GetAssetReference(asset));
      ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Insert, ReferenceType.Assets, asset.AssetID, description);

      foreach (CustomFieldSaveInfo field in info.Fields)
      {
        CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, asset.AssetID);
        if (field.Value == null)
        {
          customValue.Value = "";
        }
        else
        {
          if (customValue.FieldType == CustomFieldType.DateTime)
          {
            customValue.Value = ((DateTime)field.Value).ToString();
            //DateTime dt;
            //if (DateTime.TryParse(((string)field.Value), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
            //{
            //    customValue.Value = dt.ToUniversalTime().ToString();
            //}
          }
          else
          {
            customValue.Value = field.Value.ToString();
          }

        }

        customValue.Collection.Save();
      }

      AssetHistory history = new AssetHistory(loginUser);
      AssetHistoryItem historyItem = history.AddNewAssetHistoryItem();

      historyItem.OrganizationID =    loginUser.OrganizationID;
      historyItem.Actor =             loginUser.UserID;
      historyItem.AssetID =           asset.AssetID;
      historyItem.ActionTime =        DateTime.UtcNow;
      historyItem.ActionDescription = "Asset created.";
      historyItem.ShippedFrom =       0;
      historyItem.ShippedTo =         0;
      historyItem.TrackingNumber =    string.Empty;
      historyItem.ShippingMethod =    string.Empty;
      historyItem.ReferenceNum =      string.Empty;
      historyItem.Comments =          string.Empty;

      history.Save();

      return asset.AssetID;

    }

    [WebMethod]
    public string AssignAsset(int assetID, string data)
    {
      AssignAssetSave info;
      try
      {
        info = Newtonsoft.Json.JsonConvert.DeserializeObject<AssignAssetSave>(data);
      }
      catch (Exception e)
      {
        return "error";
      }

      LoginUser loginUser = TSAuthentication.GetLoginUser();
      Asset o = Assets.GetAsset(loginUser, assetID);
      //Location 1=assigned (shipped), 2=warehouse, 3=junkyard
      o.Location = "1";
      o.AssignedTo = info.RefID;
      DateTime now = DateTime.UtcNow;
      o.DateModified = now;
      o.ModifierID = loginUser.UserID;
      o.Collection.Save();

      AssetHistory assetHistory = new AssetHistory(loginUser);
      AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();

      assetHistoryItem.AssetID = assetID;
      assetHistoryItem.OrganizationID = loginUser.OrganizationID;
      assetHistoryItem.ActionTime = DateTime.UtcNow;
      assetHistoryItem.ActionDescription = "Asset Shipped on " + info.DateShipped.Month.ToString() + "/" + info.DateShipped.Day.ToString() + "/" + info.DateShipped.Year.ToString();
      assetHistoryItem.ShippedFrom = loginUser.OrganizationID;
      assetHistoryItem.ShippedFromRefType = (int)ReferenceType.Organizations;
      assetHistoryItem.ShippedTo = info.RefID;
      assetHistoryItem.TrackingNumber = info.TrackingNumber;
      assetHistoryItem.ShippingMethod = info.ShippingMethod;
      assetHistoryItem.ReferenceNum = info.ReferenceNumber;
      assetHistoryItem.Comments = info.Comments;

      assetHistoryItem.DateCreated = now;
      assetHistoryItem.Actor = loginUser.UserID;
      assetHistoryItem.RefType = info.RefType;
      assetHistoryItem.DateModified = now;
      assetHistoryItem.ModifierID = loginUser.UserID;

      assetHistory.Save();

      AssetAssignments assetAssignments = new AssetAssignments(loginUser);
      AssetAssignment assetAssignment = assetAssignments.AddNewAssetAssignment();

      assetAssignment.HistoryID = assetHistoryItem.HistoryID;

      assetAssignments.Save();

      string description = String.Format("Assigned asset to {0}.", info.AssigneeName);
      ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Assets, assetID, description);

      AssetsView assetsView = new AssetsView(loginUser);
      assetsView.LoadByAssetID(assetID);

      StringBuilder productVersionNumberDisplayName = new StringBuilder();
      if (!string.IsNullOrEmpty(assetsView[0].ProductVersionNumber))
      {
        productVersionNumberDisplayName.Append(" - " + assetsView[0].ProductVersionNumber);
      }

      StringBuilder serialNumberDisplayValue = new StringBuilder();
      if (string.IsNullOrEmpty(assetsView[0].SerialNumber))
      {
        serialNumberDisplayValue.Append("Empty");
      }
      else
      {
        serialNumberDisplayValue.Append(assetsView[0].SerialNumber);
      }

      StringBuilder warrantyExpirationDisplayValue = new StringBuilder();
      if (assetsView[0].WarrantyExpiration == null)
      {
        warrantyExpirationDisplayValue.Append("Empty");
      }
      else
      {
        warrantyExpirationDisplayValue.Append(((DateTime)assetsView[0].WarrantyExpiration).ToString(GetDateFormatNormal()));
      }

      return string.Format(@"<div class='list-group-item'>
                            <a href='#' id='{0}' class='assetLink'><h4 class='list-group-item-heading'>{1}</h4></a>
                            <div class='row'>
                                <div class='col-xs-8'>
                                    <p class='list-group-item-text'>{2}{3}</p>
                                </div>
                            </div>
                            <div class='row'>
                                <div class='col-xs-8'>
                                    <p class='list-group-item-text'>SN: {4} - Warr. Exp.: {5}</p>
                                </div>
                            </div>
                            </div>"

          , assetID
          , assetsView[0].DisplayName
          , assetsView[0].ProductName
          , productVersionNumberDisplayName
          , serialNumberDisplayValue
          , warrantyExpirationDisplayValue);
    }

    public string GetDateFormatNormal()
    {
      CultureInfo us = new CultureInfo(TSAuthentication.GetLoginUser().CultureInfo.ToString());
      return us.DateTimeFormat.ShortDatePattern;
    }

    [WebMethod]
    public int ReturnAsset(int assetID, string data)
    {
      AssignAssetSave info;
      try
      {
        info = Newtonsoft.Json.JsonConvert.DeserializeObject<AssignAssetSave>(data);
      }
      catch (Exception e)
      {
        return -1;
      }

      LoginUser loginUser = TSAuthentication.GetLoginUser();
      Asset o = Assets.GetAsset(loginUser, assetID);
      //Location 1=assigned (shipped), 2=warehouse, 3=junkyard
      o.Location = "2";
      o.AssignedTo = null;
      DateTime now = DateTime.UtcNow;
      o.DateModified = now;
      o.ModifierID = loginUser.UserID;
      o.Collection.Save();

      AssetAssignmentsView assetAssignmentsView = new AssetAssignmentsView(loginUser);
      assetAssignmentsView.LoadByAssetID(assetID);

      AssetHistory assetHistory = new AssetHistory(loginUser);
      AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();

      assetHistoryItem.AssetID = assetID;
      assetHistoryItem.OrganizationID = loginUser.OrganizationID;
      assetHistoryItem.ActionTime = DateTime.UtcNow;
      assetHistoryItem.ActionDescription = "Item returned to warehouse on " + info.DateShipped.Month.ToString() + "/" + info.DateShipped.Day.ToString() + "/" + info.DateShipped.Year.ToString();
      assetHistoryItem.ShippedFrom = assetAssignmentsView[0].ShippedTo;
      assetHistoryItem.ShippedFromRefType = assetAssignmentsView[0].RefType;
      assetHistoryItem.ShippedTo = loginUser.OrganizationID;
      assetHistoryItem.RefType = (int)ReferenceType.Organizations;
      assetHistoryItem.TrackingNumber = info.TrackingNumber;
      assetHistoryItem.ShippingMethod = info.ShippingMethod;
      assetHistoryItem.ReferenceNum = info.ReferenceNumber;
      assetHistoryItem.Comments = info.Comments;

      assetHistoryItem.DateCreated = now;
      assetHistoryItem.Actor = loginUser.UserID;
      assetHistoryItem.DateModified = now;
      assetHistoryItem.ModifierID = loginUser.UserID;

      assetHistory.Save();

      AssetAssignments assetAssignments = new AssetAssignments(loginUser);
      foreach (AssetAssignmentsViewItem assetAssignmentViewItem in assetAssignmentsView)
      {
        assetAssignments.DeleteFromDB(assetAssignmentViewItem.AssetAssignmentsID);
      }

      ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Assets, assetID, "Returned asset.");

      return assetID;
    }

    [WebMethod]
    public int JunkAsset(int assetID, string comments)
    {
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      Asset o = Assets.GetAsset(loginUser, assetID);
      //Location 1=assigned (shipped), 2=warehouse, 3=junkyard
      o.Location = "3";
      o.AssignedTo = null;
      DateTime now = DateTime.UtcNow;
      o.DateModified = now;
      o.ModifierID = loginUser.UserID;
      o.Collection.Save();

      AssetHistory assetHistory = new AssetHistory(loginUser);
      AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();

      assetHistoryItem.AssetID = assetID;
      assetHistoryItem.OrganizationID = loginUser.OrganizationID;
      assetHistoryItem.ActionTime = DateTime.UtcNow;
      assetHistoryItem.ActionDescription = "Asset assigned to Junkyard";
      assetHistoryItem.ShippedFrom = -1;
      assetHistoryItem.ShippedFromRefType = -1;
      assetHistoryItem.ShippedTo = -1;
      assetHistoryItem.RefType = -1;
      assetHistoryItem.TrackingNumber = string.Empty;
      assetHistoryItem.ShippingMethod = string.Empty;
      assetHistoryItem.ReferenceNum = string.Empty;
      assetHistoryItem.Comments = comments;

      assetHistoryItem.DateCreated = now;
      assetHistoryItem.Actor = loginUser.UserID;
      assetHistoryItem.DateModified = now;
      assetHistoryItem.ModifierID = loginUser.UserID;

      assetHistory.Save();

      ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Assets, assetID, "Junked asset.");

      return assetID;
    }

    private string GetAssetReference(Asset asset)
    {
      string result = "with AssetID: " +asset.AssetID.ToString();

      if (!String.IsNullOrEmpty(asset.Name))
      {
        result = asset.Name;
      }
      else if (!String.IsNullOrEmpty(asset.SerialNumber))
      {
        result = "with Serial Number: " + asset.SerialNumber;
      }

      return result;
    }

    [WebMethod]
    public string GetShortNameFromID(int assetID)
    {
      Assets assets = new Assets(TSAuthentication.GetLoginUser());
      assets.LoadByAssetID(assetID);

      if (assets.IsEmpty) return "N/A";

      string result = assets[0].AssetID.ToString();

      if (!String.IsNullOrEmpty(assets[0].Name))
      {
        if (assets[0].Name.Length > 10)
          result = assets[0].Name.Substring(0, 10).ToString() + "...";
        else
          result = assets[0].Name.ToString();
      }
      else if (!String.IsNullOrEmpty(assets[0].SerialNumber))
      {
        if (assets[0].SerialNumber.Length > 10)
          result = assets[0].SerialNumber.Substring(0, 10).ToString() + "...";
        else
          result = assets[0].SerialNumber.ToString();
      }

      return result;
    }

    [WebMethod]
    public AssetHistoryViewItemProxy[] LoadHistory(int assetID, int start)
    {
      AssetHistoryView history = new AssetHistoryView(TSAuthentication.GetLoginUser());
      history.LoadByAssetIDLimit(assetID, start);

      return history.GetAssetHistoryViewItemProxies();
    }

    [WebMethod]
    public ActionLogProxy[] LoadActionsHistory(int assetID, int start)
    {
      ActionLogs history = new ActionLogs(TSAuthentication.GetLoginUser());
      history.LoadByAssetIDLimit(assetID, start);

      return history.GetActionLogProxies();
    }

    [WebMethod]
    public string SetAssetName(int assetID, string value)
    {
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      Asset o = Assets.GetAsset(loginUser, assetID);
      string description = String.Format("Changed Name from \"{0}\" to \"{1}\".", o.Name, value);
      o.Name = value;
      o.DateModified = DateTime.UtcNow;
      o.ModifierID = loginUser.UserID;
      o.Collection.Save();
      ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Assets, assetID, description);
      return value != "" ? value : "Empty";
    }

    [WebMethod]
    public int SetAssetProduct(int assetID, int value, string oldName, string newName)
    {
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      Asset o = Assets.GetAsset(loginUser, assetID);
      o.ProductID = value;
      o.ProductVersionID = null;
      o.DateModified = DateTime.UtcNow;
      o.ModifierID = loginUser.UserID;
      o.Collection.Save();
      string description = String.Format("Changed Product from \"{0}\" to \"{1}\".", oldName, newName);
      ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Assets, assetID, description);
      return value;
    }

    [WebMethod]
    public int SetAssetProductVersion(int assetID, int value, string oldName, string newName)
    {
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      Asset o = Assets.GetAsset(loginUser, assetID);
      o.ProductVersionID = value;
      o.DateModified = DateTime.UtcNow;
      o.ModifierID = loginUser.UserID;
      o.Collection.Save();
      string description = String.Format("Changed Product Version from \"{0}\" to \"{1}\".", oldName, newName);
      ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Assets, assetID, description);
      return value;
    }

    [WebMethod]
    public string SetAssetSerialNumber(int assetID, string value)
    {
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      Asset o = Assets.GetAsset(loginUser, assetID);
      string description = String.Format("Changed Serial Number from \"{0}\" to \"{1}\".", o.SerialNumber, value);
      o.SerialNumber = value;
      o.DateModified = DateTime.UtcNow;
      o.ModifierID = loginUser.UserID;
      o.Collection.Save();
      ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Assets, assetID, description);
      return value != "" ? value : "Empty";
    }

    [WebMethod]
    public string SetAssetWarrantyExpiration(int assetID, object value)
    {
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      Asset o = Assets.GetAsset(loginUser, assetID);
      StringBuilder description = new StringBuilder();
      if (o.WarrantyExpiration == null)
      {
        description.Append(String.Format("Changed Warranty Expiration from \"{0}\" to \"{1}\".", "Unassigned", ((DateTime)value).ToString(GetDateFormatNormal())));
      }
      else
      {
        description.Append(String.Format("Changed Warranty Expiration from \"{0}\" to \"{1}\".", ((DateTime)o.WarrantyExpiration).ToString(GetDateFormatNormal()), ((DateTime)value).ToString(GetDateFormatNormal())));
      }
      o.WarrantyExpiration = (DateTime)value;
      o.DateModified = DateTime.UtcNow;
      o.ModifierID = loginUser.UserID;
      o.Collection.Save();
      ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Assets, assetID, description.ToString());
      return value.ToString() != "" ? value.ToString() : null;
    }

    [WebMethod]
    public string SetAssetNotes(int assetID, string value)
    {
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      Asset o = Assets.GetAsset(loginUser, assetID);
      string description = String.Format("Changed Notes from \"{0}\" to \"{1}\".", o.Notes, value);
      o.Notes = value;
      o.DateModified = DateTime.UtcNow;
      o.ModifierID = loginUser.UserID;
      o.Collection.Save();
      ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Assets, assetID, description);
      return value != "" ? value : "Empty";
    }

    [WebMethod]
    public string UpdateRecentlyViewed(string viewid)
    {
      int refType, refID;

      refType = (int)ReferenceType.Assets;
      refID = Convert.ToInt32(viewid.Substring(1));

      RecentlyViewedItem recent = (new RecentlyViewedItems(TSAuthentication.GetLoginUser()).AddNewRecentlyViewedItem());


      recent.RefID = refID;
      recent.RefType = refType;
      recent.DateViewed = DateTime.UtcNow;
      recent.UserID = TSAuthentication.GetLoginUser().UserID;
      recent.BaseCollection.Save();

      return GetRecentlyViewed();
    }

    [WebMethod]
    public string GetRecentlyViewed()
    {
      StringBuilder builder = new StringBuilder();
      RecentlyViewedItems recent = new RecentlyViewedItems(TSAuthentication.GetLoginUser());
      recent.LoadRecent(TSAuthentication.GetLoginUser().UserID, (int)ReferenceType.Assets);

      builder.Append(@"<ul class=""recent-list"">");
      foreach (RecentlyViewedItem item in recent)
      {
        builder.Append(CreateRecentlyViewed(item));
      }
      builder.Append("</ul>");
      return builder.ToString();
    }

    public string CreateRecentlyViewed(RecentlyViewedItem recent)
    {
      string recentHTML;
      AssetsView assetsView = new AssetsView(TSAuthentication.GetLoginUser());
      assetsView.LoadByAssetID(recent.RefID);
      recentHTML = @" 
        <li>
          <div class=""recent-info"">
            <h4><a class=""assetlink"" data-assetid=""{0}"" href=""""><i class=""fa {1} {2}""></i>{3}</a></h4>
          </div>
        </li>";
      string icon = "fa-truck";
      string color = "color-green";
      switch (assetsView[0].Location)
      {
        case "2":
          icon = "fa-home";
          color = "color-amber";
          break;
        case "3":
          icon = "fa-trash-o";
          color = "color-red";
          break;
      }
      var displayName = assetsView[0].Name;
      if (String.IsNullOrEmpty(displayName))
      {
        displayName = assetsView[0].SerialNumber;
        if (String.IsNullOrEmpty(displayName))
        {
          displayName = assetsView[0].AssetID.ToString();
        }
      }

      return string.Format(recentHTML, assetsView[0].AssetID, icon, color, displayName);
    }

    [WebMethod]
    public string LoadCustomControls(ReferenceType refType)
    {
      CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
      fields.LoadByReferenceType(TSAuthentication.OrganizationID, refType, -1);
      int count = 0;

      StringBuilder htmltest = new StringBuilder("");

      htmltest.Append("<div class='form-group'>");

      foreach (CustomField field in fields)
      {
        if (count == 0)
        {
          htmltest.Append("<div class='row'>");
          count++;
        }

        htmltest.AppendFormat("<div class='col-xs-4'><label for='{0}' class='col-xs-4 control-label'>{1}</label>", field.CustomFieldID, field.Name);
        switch (field.FieldType)
        {
          case CustomFieldType.Text: htmltest.AppendLine(CreateTextControl(field)); break;
          case CustomFieldType.Number: htmltest.AppendLine(CreateNumberControl(field)); break;
          case CustomFieldType.Time: htmltest.AppendLine(CreateTimeControl(field)); break;
          case CustomFieldType.Date: htmltest.AppendLine(CreateDateControl(field)); break;
          case CustomFieldType.DateTime: htmltest.AppendLine(CreateDateTimeControl(field)); break;
          case CustomFieldType.Boolean: htmltest.AppendLine(CreateBooleanControl(field)); break;
          case CustomFieldType.PickList: htmltest.AppendLine(CreatePickListControl(field)); break;
          default: break;
        }
        htmltest.Append("</div>");
        count++;

        if (count % 4 == 0)
        {
          htmltest.Append("</div>"); //end row
          count = 0;
        }
      }
      if (count != 0)
      {
        count = 0;
        htmltest.Append("</div>"); // end row if not closed
      }
      htmltest.Append("</div>"); //end form-group

      count = 0;

      return htmltest.ToString();
    }

    public string CreateTextControl(CustomField field, bool isEditable = false, int organizationID = -1)
    {
      StringBuilder html = new StringBuilder();

      if (isEditable)
      {
        CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
        html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
      }
      else
      {
        StringBuilder mask = new StringBuilder();
        if (!String.IsNullOrEmpty(field.Mask))
        {
          mask.Append("placeholder='" + field.Mask + "'");
        }
        html.AppendFormat("<div class='col-xs-8'><input class='form-control col-xs-10 customField {1}' id='{0}' name='{0}' {2}></div>", field.CustomFieldID, field.IsRequired ? "required" : "", mask.ToString());
      }
      return html.ToString();
    }

    public string CreateNumberControl(CustomField field, bool isEditable = false, int organizationID = -1)
    {
      StringBuilder html = new StringBuilder();
      if (isEditable)
      {
        CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
        html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
      }
      else
        html.AppendFormat("<div class='col-xs-8'><input class='form-control col-xs-10 customField number {1}' id='{0}'  name='{0}'></div>", field.CustomFieldID, field.IsRequired ? "required" : "");

      return html.ToString();
    }

    public string CreateDateControl(CustomField field, bool isEditable = false, int organizationID = -1)
    {
      StringBuilder html = new StringBuilder();
      if (isEditable)
      {
        CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
        html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
      }
      else
        html.AppendFormat("<div class='col-xs-8'><input class='form-control datepicker col-xs-10 customField {1}' id='{0}' type='_date' name='{0}'></div>", field.CustomFieldID, field.IsRequired ? "required" : "");

      return html.ToString();
    }

    public string CreateTimeControl(CustomField field, bool isEditable = false, int organizationID = -1)
    {
      StringBuilder html = new StringBuilder();
      if (isEditable)
      {
        CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
        html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
      }
      else
        html.AppendFormat("<div class='col-xs-8'><input class='form-control timepicker col-xs-10 customField {1}' id='{0}' type='_time'  name='{0}'></div>", field.CustomFieldID, field.IsRequired ? "required" : "");

      return html.ToString();
    }

    public string CreateDateTimeControl(CustomField field, bool isEditable = false, int organizationID = -1)
    {
      StringBuilder html = new StringBuilder();
      if (isEditable)
      {
        CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
        html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
      }
      else
        html.AppendFormat("<div class='col-xs-8'><input class='form-control datetimepicker col-xs-10 customField {1}' id='{0}' type='_datetime'  name='{0}'></div>", field.CustomFieldID, field.IsRequired ? "required" : "");

      return html.ToString();
    }

    public string CreateBooleanControl(CustomField field, bool isEditable = false, int organizationID = -1)
    {
      StringBuilder html = new StringBuilder();
      if (isEditable)
      {
        CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
        html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
      }
      else
      {
        html.AppendFormat("<div class='col-xs-1'><label><input class='customField' id='{0}' type='checkbox'></label></div>", field.CustomFieldID);
      }
      return html.ToString();
    }

    public string CreatePickListControl(CustomField field, bool isEditable = false, int organizationID = -1)
    {
      StringBuilder html = new StringBuilder();
      string[] items = field.ListValues.Split('|');
      if (isEditable)
      {
        CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
        html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='select'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
      }
      else
      {
        html.AppendFormat("<div class='col-xs-8'><select class='form-control customField' id='{0}'  name='{0}' type='picklist'>", field.CustomFieldID);
        foreach (string item in items)
        {
          html.AppendFormat("<option value='{0}'>{1}</option>", item, item);
        }
        html.Append("</select></div>");
      }
      return html.ToString();
    }

    [WebMethod]
    public CustomValueProxy[] GetCustomValues(int refID, ReferenceType refType)
    {
      CustomValues values = new CustomValues(TSAuthentication.GetLoginUser());
      values.LoadByReferenceType(TSAuthentication.OrganizationID, refType, refID);
      return values.GetCustomValueProxies();
    }

    [WebMethod]
    public AttachmentProxy[] LoadFiles(int refID, ReferenceType refType)
    {
        return TeamSupport.Data.Quarantine.WebAppQ.GetAttachmentProxies(refID, refType, TSAuthentication.GetLoginUser());
    }

  }

  public class NewAssetSave
  {
    public NewAssetSave() { }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public int ProductID { get; set; }
    [DataMember]
    public int? ProductVersionID { get; set; }
    [DataMember]
    public string SerialNumber { get; set; }
    [DataMember]
    public DateTime? WarrantyExpiration { get; set; }
    [DataMember]
    public string Notes { get; set; }
    [DataMember]
    public List<CustomFieldSaveInfo> Fields { get; set; }
  }

  public class AssignAssetSave
  {
    public AssignAssetSave() { }
    [DataMember]
    public int RefID { get; set; }
    [DataMember]
    public int RefType { get; set; }
    [DataMember]
    public DateTime DateShipped { get; set; }
    [DataMember]
    public string TrackingNumber { get; set; }
    [DataMember]
    public string ShippingMethod { get; set; }
    [DataMember]
    public string ReferenceNumber { get; set; }
    [DataMember]
    public string Comments { get; set; }
    [DataMember]
    public string AssigneeName { get; set; }
  }

}