using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;

public partial class UserControls_ReportFilterControl : System.Web.UI.UserControl
{
  public event EventHandler OnFilterChanged;

  public int ReportSubcategoryID
  {
    get
    {
      return int.Parse(fieldSubcategoryID.Value);
    }
    set
    {
      fieldSubcategoryID.Value = value.ToString();
      Clear();
      LoadAll(value);
    }
  }

  public ReportConditions ReportConditions
  {
    get
    {
      try
      {
        ReportConditions conditions = (ReportConditions)DataUtils.StringToObject(fieldFilterObject.Value);
        if (conditions == null) conditions = new ReportConditions(UserSession.LoginUser);
        return conditions;
      }
      catch (Exception ex)
      {
        return new ReportConditions(UserSession.LoginUser);
      }
    }
    set
    {
      if (value == null) value = new TeamSupport.Data.ReportConditions(UserSession.LoginUser);
      value.LoginUser = UserSession.LoginUser;
      fieldFilterObject.Value = DataUtils.ObjectToString(value);
      LoadAll(ReportSubcategoryID);
    }

  }

  public bool Changed
  {
    get { return fieldChanged.Value != "false"; }
  }

  public int ReportID 
  {
    get { return int.Parse(fieldReportID.Value); }
    set { fieldReportID.Value = value.ToString(); } 
  
  }

  private RadAjaxManager _manager;

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    _manager = RadAjaxManager.GetCurrent(Page);
    _manager.AjaxRequest += new RadAjaxControl.AjaxRequestDelegate(_manager_AjaxRequest);
    fieldChanged.Value = "false";
  }

  public void Clear()
  {
    fieldFilterObject.Value = "";
    SetCaptionConjunction(true);
  }

  private void SetCaptionConjunction(bool useAll)
  {
    string con = "ALL";
    if (!useAll) con = "ANY";

    litCaption.Text = "Select records where <a style=\"background-color:#D3E2F4; border: Solid 1px #5D8CC9; padding: 2px 4px; color: #00156E; text-decoration:none; \" href=\"#\" onclick=\"SendAjaxRequest('ChangeConjunction," + con + "');\">" + con + "</a> of the following conditions apply:";
  
  }

  void _manager_AjaxRequest(object sender, AjaxRequestEventArgs e)
  {
    ProcessAjaxRequest(e.Argument);
    if (OnFilterChanged != null) OnFilterChanged(this, new EventArgs());
  }

  public void ProcessAjaxRequest(string argument)
  {
    string[] args = argument.Split(',');

    if (args[0] == "DeleteCondition" && args.Length > 1)
    {
      ReportConditions conditions = ReportConditions;
      conditions.Items.RemoveAt(int.Parse(args[1]));
      ReportConditions = conditions;
      litDisplayText.Text = conditions.ToString();
    }
    else if (args[0] == "ChangeConjunction" && args.Length > 1)
    {
      bool useAll = args[1] != "ALL";
      ReportConditions conditions = ReportConditions;
      conditions.MatchAll = useAll;
      ReportConditions = conditions;
      SetCaptionConjunction(useAll);
      litDisplayText.Text = conditions.ToString();
    }
    else if (args[0] == "AddCondition")
    {
      AddCondition();
    }
  
  }

  private void LoadAll(int subcategoryID)
  {
    LoadFields(subcategoryID);
    ReportDataType dataType = GetDataType(cmbFields.SelectedValue);
    SetOperator(dataType);
    SetValueBoxes(dataType, (ConditionOperator)int.Parse(cmbOperator.SelectedValue));
    litDisplayText.Text = ReportConditions.ToString();
    SetCaptionConjunction(ReportConditions.MatchAll);

  }

  private void LoadFields(int subcategoryID)
  {
    cmbFields.Items.Clear();

    ReportSubcategory sub = (ReportSubcategory)ReportSubcategories.GetReportSubcategory(UserSession.LoginUser, subcategoryID);

    if (sub == null) return;

    int primaryTableID = sub.ReportCategoryTableID;
    int secondaryTableID = sub.ReportTableID != null ? (int)sub.ReportTableID : -1;

    ReportTableFields fields = new ReportTableFields(UserSession.LoginUser);
    fields.LoadByReportTableID(primaryTableID);
    foreach (ReportTableField field in fields)
    {
      cmbFields.Items.Add(new RadComboBoxItem(field.Row["TableAlias"].ToString() + " :: " + field.Alias, "R" + field.ReportTableFieldID.ToString()));
    }

    TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);
    ticketTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);

    ReportTable table = (ReportTable)ReportTables.GetReportTable(UserSession.LoginUser, primaryTableID);
    if (table.CustomFieldRefType != ReferenceType.None)
    {
      CustomFields customFields = new CustomFields(UserSession.LoginUser);
      customFields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, (ReferenceType)table.CustomFieldRefType);

      foreach (CustomField customField in customFields)
      {
        if (customField.RefType == ReferenceType.Tickets)
        {
          TicketType ticketType = ticketTypes.FindByTicketTypeID(customField.AuxID);
          if (ticketType != null)
          cmbFields.Items.Add(new RadComboBoxItem(table.Alias + " :: " + customField.Name + " (" + ticketType.Name + ")", "C" + customField.CustomFieldID.ToString()));
        }
        else
          cmbFields.Items.Add(new RadComboBoxItem(table.Alias + " :: " + customField.Name, "C" + customField.CustomFieldID.ToString()));
      }
    }

    if (secondaryTableID > -1)
    {
      fields = new ReportTableFields(UserSession.LoginUser);
      fields.LoadByReportTableID(secondaryTableID);
      foreach (ReportTableField field in fields)
      {
        cmbFields.Items.Add(new RadComboBoxItem(field.Row["TableAlias"].ToString() + " :: " + field.Alias, "R" + field.ReportTableFieldID.ToString()));
      }

      table = (ReportTable)ReportTables.GetReportTable(UserSession.LoginUser, secondaryTableID);
      if (table.CustomFieldRefType != ReferenceType.None)
      {
        CustomFields customFields = new CustomFields(UserSession.LoginUser);
        customFields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, (ReferenceType)table.CustomFieldRefType);

        foreach (CustomField customField in customFields)
        {
          if (customField.RefType == ReferenceType.Tickets)
          {
            TicketType ticketType = ticketTypes.FindByTicketTypeID(customField.AuxID);
            if (ticketType != null)
              cmbFields.Items.Add(new RadComboBoxItem(table.Alias + " :: " + customField.Name + " (" + ticketType.Name + ")", "C" + customField.CustomFieldID.ToString()));
          }
          else
            cmbFields.Items.Add(new RadComboBoxItem(table.Alias + " :: " + customField.Name, "C" + customField.CustomFieldID.ToString()));
        }
      }
    }


  }

  protected void cmbFields_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
  {
    ReportDataType dataType = GetDataType(cmbFields.SelectedValue);
    SetOperator(dataType);
    SetValueBoxes(dataType, (ConditionOperator)int.Parse(cmbOperator.SelectedValue));
  }

  protected void cmbOperator_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
  {
    ReportDataType dataType = GetDataType(cmbFields.SelectedValue);
    SetValueBoxes(dataType, (ConditionOperator)int.Parse(cmbOperator.SelectedValue));
  }

  private ReportDataType GetDataType(string value)
  {
    bool isCustomField = IsValueCustomField(value);
    int id = GetValueFieldID(value);

    ReportDataType dataType = ReportDataType.String;

    if (isCustomField)
    {
      CustomField field = (CustomField)CustomFields.GetCustomField(UserSession.LoginUser, id);
      switch (field.FieldType)
      {
        case CustomFieldType.Date:
        case CustomFieldType.Time:
        case CustomFieldType.DateTime:
          dataType = ReportDataType.DateTime;
          break;
        case CustomFieldType.Boolean:
          dataType = ReportDataType.Boolean;
          break;
        case CustomFieldType.Number:
          dataType = ReportDataType.Int;
          break;
        default:
          break;
      }
    }
    else
    {
      ReportTableField field = (ReportTableField)ReportTableFields.GetReportTableField(UserSession.LoginUser, id);

      switch (field.DataType)
      {
        case "bit":
          dataType = ReportDataType.Boolean;
          break;
        case "datetime":
          dataType = ReportDataType.DateTime;
          break;
        case "int":
          dataType = ReportDataType.Int;
          break;
        case "float":
          dataType = ReportDataType.Float;
          break;
        default:
          break;
      }
    }
    return dataType;
  }

  private void SetOperator(ReportDataType dataType)
  {
    cmbOperator.Items.Clear();
    switch (dataType)
    {
      case ReportDataType.String:
        cmbOperator.Items.Add(new RadComboBoxItem("Is equal to", ((int)ConditionOperator.IsEqualTo).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Is not equal to", ((int)ConditionOperator.IsNotEqualTo).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Contains", ((int)ConditionOperator.Contains).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Starts with", ((int)ConditionOperator.StartsWith).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Ends with", ((int)ConditionOperator.EndsWith).ToString()));
        break;
      case ReportDataType.Int:
        cmbOperator.Items.Add(new RadComboBoxItem("Is equal to", ((int)ConditionOperator.IsEqualTo).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Is not equal to", ((int)ConditionOperator.IsNotEqualTo).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Is greater than", ((int)ConditionOperator.IsGreaterThan).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Is less than", ((int)ConditionOperator.IsLessThan).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Is in between", ((int)ConditionOperator.IsInBetween).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Is not in between", ((int)ConditionOperator.IsNotInBetween).ToString()));
        break;
      case ReportDataType.Float:
        cmbOperator.Items.Add(new RadComboBoxItem("Is equal to", ((int)ConditionOperator.IsEqualTo).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Is not equal to", ((int)ConditionOperator.IsNotEqualTo).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Is greater than", ((int)ConditionOperator.IsGreaterThan).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Is less than", ((int)ConditionOperator.IsLessThan).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Is in between", ((int)ConditionOperator.IsInBetween).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Is not in between", ((int)ConditionOperator.IsNotInBetween).ToString()));
        break;
      case ReportDataType.DateTime:
        cmbOperator.Items.Add(new RadComboBoxItem("Is equal to", ((int)ConditionOperator.IsEqualTo).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Is not equal to", ((int)ConditionOperator.IsNotEqualTo).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Is greater than", ((int)ConditionOperator.IsGreaterThan).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Is less than", ((int)ConditionOperator.IsLessThan).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Is in between", ((int)ConditionOperator.IsInBetween).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Is not in between", ((int)ConditionOperator.IsNotInBetween).ToString()));
        break;
      case ReportDataType.Boolean:
        cmbOperator.Items.Add(new RadComboBoxItem("Is equal to", ((int)ConditionOperator.IsEqualTo).ToString()));
        cmbOperator.Items.Add(new RadComboBoxItem("Is not equal to", ((int)ConditionOperator.IsNotEqualTo).ToString()));
        break;
      default:
        break;
    }

    cmbOperator.SelectedIndex = 0;

  }

  private void SetValueBoxes(ReportDataType dataType, ConditionOperator op)
  {
    textValue.Visible = false;
    dateValue1.Visible = false;
    dateValue2.Visible = false;
    numValue1.Visible = false;
    numValue2.Visible = false;
    cmbBool.Visible = false;
    lblAnd.Visible = false;

    switch (dataType)
    {
      case ReportDataType.String:
        textValue.Visible = true;
        break;
      case ReportDataType.Int:
        numValue1.Visible = true;
        numValue2.Visible = op == ConditionOperator.IsNotInBetween || op == ConditionOperator.IsInBetween;
        break;
      case ReportDataType.Float:
        numValue1.Visible = true;
        numValue2.Visible = op == ConditionOperator.IsNotInBetween || op == ConditionOperator.IsInBetween;
        break;
      case ReportDataType.DateTime:
        dateValue1.Visible = true;
        dateValue2.Visible = op == ConditionOperator.IsNotInBetween || op == ConditionOperator.IsInBetween;
        break;
      case ReportDataType.Boolean:
        cmbBool.Visible = true;
        break;
      default:
        break;
    }
    lblAnd.Visible = op == ConditionOperator.IsNotInBetween || op == ConditionOperator.IsInBetween;
  }

  private bool IsValueCustomField(string value)
  {
    if (value.Length < 1) return false;
    return value[0] == 'C';
  }

  private int GetValueFieldID(string value)
  {
    if (value.Length < 1) return -1;
    string s = value.Substring(1, value.Length - 1);
    try
    {
      return int.Parse(s);
    }
    catch (Exception)
    {

      return -1;
    }

  }

  private void AddCondition()
  { 
    ReportConditions conditions = ReportConditions;
    ReportCondition condition = new ReportCondition();
    condition.IsCustomField = IsValueCustomField(cmbFields.SelectedValue);
    condition.FieldID = GetValueFieldID(cmbFields.SelectedValue);
    condition.DisplayName = cmbFields.SelectedItem.Text;
    condition.ConditionOperator = (ConditionOperator)int.Parse(cmbOperator.SelectedValue);
    ReportDataType dataType = GetDataType(cmbFields.SelectedValue);
    switch (dataType)
    {
      case ReportDataType.String:
        /*if (textValue.Text == "")
        {
          _manager.Alert("Please enter a value.");
          return;
        }*/
        condition.Value1 = textValue.Text;
        break;
      case ReportDataType.Int:
        if (numValue1.Value == null)
        {
          _manager.Alert("Please enter a value.");
          return;
        }
        if (numValue2.Visible && numValue2.Value == null)
        {
          _manager.Alert("Please enter the second value.");
          return;
        }

        condition.Value1 = (int)numValue1.Value;
        if (numValue2.Visible) condition.Value2 = (int)numValue2.Value;
        break;
      case ReportDataType.Float:
        if (numValue1.Value == null)
        {
          _manager.Alert("Please enter a value.");
          return;
        }
        if (numValue2.Visible && numValue2.Value == null)
        {
          _manager.Alert("Please enter the second value.");
          return;
        }

        condition.Value1 = (double)numValue1.Value;
        if (numValue2.Visible) condition.Value2 = (double)numValue2.Value;
        break;
      case ReportDataType.DateTime:
        if (dateValue1.SelectedDate == null)
        {
          _manager.Alert("Please enter a date.");
          return;
        }
        if (dateValue2.Visible && dateValue2.SelectedDate == null)
        {
          _manager.Alert("Please enter the second date.");
          return;
        }
        condition.Value1 = (DateTime)dateValue1.SelectedDate;
        if (dateValue2.Visible) condition.Value2 = (DateTime)dateValue2.SelectedDate;
        break;
      case ReportDataType.Boolean:
        condition.Value1 = cmbBool.SelectedIndex == 0;
        break;
      default:
        break;
    }

    if (condition.Value1 == null)
    {
      _manager.Alert("Please enter a value.");
      return;
    }

    if ((condition.ConditionOperator == ConditionOperator.IsNotInBetween || condition.ConditionOperator == ConditionOperator.IsInBetween) && condition.Value2 == null)
    {
      _manager.Alert("Please enter the second value.");
      return;
    }

    conditions.Items.Add(condition);
    ReportConditions = conditions;
    litDisplayText.Text = conditions.ToString();
  }
}
