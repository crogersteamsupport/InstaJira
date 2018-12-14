using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using Telerik.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace TeamSupport.WebUtils
{

  public class CustomFieldControls
  {
    class CustomControl
    {
      public CustomControl(Control control, string valueScript, string saveMethod, int refID, int fieldID)
      {
        Control = control;
        ValueScript = valueScript;
        RefID = refID;
        FieldID = fieldID;
        SaveMethod = saveMethod;
      }

      public Control Control { get; set; }
      public string ValueScript { get; set; }
      public int RefID { get; set; }
      public int FieldID { get; set; }
      public string SaveMethod { get; set; }
    }

    private int _columns;
    private ReferenceType _refType;
    private HtmlTable _table;
    private int _refID;
    private bool _autoSave;
    private string _clientSaveEvent;

    private List<CustomControl> _customControls = new List<CustomControl>();

    private int _auxID;

    public int AuxID
    {
      get { return _auxID; }
      set { _auxID = value; }
    }

    public int RefID
    {
      get { return _refID; }
      set { _refID = value; }
    }

    public string Script 
    {
      get 
      {
        StringBuilder builder = new StringBuilder();
        //builder.Append(@"<script type=""text/javascript"" language=""javascript"">");
        builder.Append("function ConvertDate(dt){ if (!dt) return null; else return dt.format('s'); }");
        builder.Append("function SaveCustomControls(){");
        //builder.Append("try{");
        string saveString = "top.privateServices.{0}({1},{2},{3});";
        foreach (CustomControl control in _customControls)
        {
          string valueString = string.Format(control.ValueScript, control.Control.ClientID);
          //builder.Append(String.Format(saveString, control.SaveMethod, control.RefID.ToString(), control.FieldID.ToString(), valueString, ", function(result){alert('success');}, function(result){alert(result.get_message());}"));
          builder.Append(String.Format(saveString, control.SaveMethod, control.RefID.ToString(), control.FieldID.ToString(), valueString));
        }

        //builder.Append("}catch(err){alert(err.message);}}");
        builder.Append("}");
        //builder.Append("</script>");
        return builder.ToString();
      }
    }

    public CustomFieldControls(ReferenceType refType, int refID, int columns, HtmlTable table, string clientSaveEvent)
    {
      _refType = refType;
      _auxID = -1;
      _refID = refID;
      _columns = columns;
      _table = table;
      _autoSave = false;
      _clientSaveEvent = clientSaveEvent;
    }

    public CustomFieldControls(ReferenceType refType, int auxID, int refID, int columns, HtmlTable table, string clientSaveEvent)
    {
      _refType = refType;
      _auxID = auxID;
      _refID = refID;
      _columns = columns;
      _table = table;
      _autoSave = false;
      _clientSaveEvent = clientSaveEvent;
    }

    public CustomFieldControls(ReferenceType refType, int refID, int columns, HtmlTable table, bool autoSave)
    {
      _refType = refType;
      _auxID = -1;
      _refID = refID;
      _columns = columns;
      _table = table;
      _autoSave = autoSave;
      _clientSaveEvent = "";
    }

    public CustomFieldControls(ReferenceType refType, int auxID, int refID, int columns, HtmlTable table, bool autoSave)
    {
      _refType = refType;
      _auxID = auxID;
      _refID = refID;
      _columns = columns;
      _table = table;
      _autoSave = autoSave;
      _clientSaveEvent = "";
    }

    private Control GetCustomControl(Control parent, string id)
    {
      foreach (Control control in parent.Controls)
      {
        if (control.ID == id)
        {
          return control;
        }
        else if (control.HasControls())
        {
          Control result = GetCustomControl(control, id);
          if (result != null) return result;
        }
      }
      return null;
    }

    public int GetFieldCount()
    {
      CustomFields fields = new CustomFields(UserSession.LoginUser);
      fields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, _refType, _auxID);

      return fields.Count;
    }

    public void LoadValues()
    {
      CustomFields fields = new CustomFields(UserSession.LoginUser);
      fields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, _refType, _auxID);

      foreach (CustomField field in fields)
      {
        Control control = GetCustomControl(_table, FieldIDToControlID(field.CustomFieldID));
        CustomValue value = CustomValues.GetValue(UserSession.LoginUser, field.CustomFieldID, _refID);
        if (control != null && value != null)
        {
          try
          {
            if (control is RadInputControl)
            {
              (control as RadInputControl).Text = value.Value;
            }
            else if (control is CheckBox)
            {
              (control as CheckBox).Checked = bool.Parse(value.Value);
            }
            else if (control is RadComboBox)
            {
              (control as RadComboBox).SelectedValue = value.Value;
            }
            else if (control is RadDatePicker)
            {
              if (control is RadTimePicker)
              {
                if (value.Value == "")
                {
                  (control as RadTimePicker).SelectedDate = null;
                }
                else
                {
                  (control as RadTimePicker).SelectedDate = DataUtils.DateToLocal(UserSession.LoginUser, DateTime.Parse(value.Value));
                }
              }
              else
              {
                if (value.Value == "")
                {
                  (control as RadDatePicker).SelectedDate = null;
                }
                else
                {
                  (control as RadDatePicker).SelectedDate = DataUtils.DateToLocal(UserSession.LoginUser, DateTime.Parse(value.Value));
                }
              }
            }
            else if (control is RadDateTimePicker)
            {
              if (value.Value == "")
                (control as RadDateTimePicker).SelectedDate = null;
              else
                (control as RadDateTimePicker).SelectedDate = DataUtils.DateToLocal(UserSession.LoginUser, DateTime.Parse(value.Value));
            }

          }
          catch (Exception ex)
          {
            
          }
        }
      }
    
    }

    public string CheckRequiredCustomFields()
    {
        CustomFields fields = new CustomFields(UserSession.LoginUser);
        fields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, _refType, _auxID);

        foreach (CustomField field in fields)
        {
            Control control = GetCustomControl(_table, FieldIDToControlID(field.CustomFieldID));
            if (control != null)
            {
                CustomValue value = CustomValues.GetValue(UserSession.LoginUser, field.CustomFieldID, _refID);
                if (value.IsRequired)
                {
                    if (control is RadInputControl)
                    {
                        value.Value = (control as RadInputControl).Text;
                    }
                    else if (control is CheckBox)
                    {
                        value.Value = (control as CheckBox).Checked.ToString();
                    }
                    else if (control is RadComboBox)
                    {
                        if (field.IsFirstIndexSelect && (control as RadComboBox).SelectedIndex == 0)
                            value.Value = "";
                        else
                            value.Value = (control as RadComboBox).SelectedValue;
                    }
                    else if (control is RadDateTimePicker)
                    {
                        value.Value = DataUtils.DateToUtc(UserSession.LoginUser, (control as RadDateTimePicker).SelectedDate).ToString();
                    }

                    if (value.Value == "" || value.Value == null)
                        return value.Name + " is a required value, please enter a value before saving";
                }
            }
        }

        return "";
    }


    public void SaveCustomFields()
    {
      CustomFields fields = new CustomFields(UserSession.LoginUser);
      fields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, _refType, _auxID);

      foreach (CustomField field in fields)
      {
        Control control = GetCustomControl(_table, FieldIDToControlID(field.CustomFieldID));
        if (control != null)
        {
          CustomValue value = CustomValues.GetValue(UserSession.LoginUser, field.CustomFieldID, _refID);

          if (control is RadInputControl)
          {
            value.Value = (control as RadInputControl).Text;
          }
          else if (control is CheckBox)
          {
            value.Value = (control as CheckBox).Checked.ToString();
          }
          else if (control is RadComboBox)
          {
            value.Value = (control as RadComboBox).SelectedValue;
          }
          else if (control is RadDatePicker)
          {
            if (control is RadTimePicker)
            {
              DateTime? selectedNullableDateTime = (control as RadTimePicker).SelectedDate;
              if (selectedNullableDateTime != null)
              {
                DateTime selectedDateTime = (DateTime)selectedNullableDateTime;
                DateTime timeOnly = new DateTime(1970, 1, 1, selectedDateTime.Hour, selectedDateTime.Minute, 0, 0, UserSession.LoginUser.CultureInfo.Calendar);
                value.Value = DataUtils.DateToUtc(UserSession.LoginUser, timeOnly).ToString();
              }
            }
            else
            {
              value.Value = DataUtils.DateToUtc(UserSession.LoginUser, (control as RadDatePicker).SelectedDate).ToString();
            }
          }
          else if (control is RadDateTimePicker)
          {
            value.Value = DataUtils.DateToUtc(UserSession.LoginUser, (control as RadDateTimePicker).SelectedDate).ToString();
          }

          value.Collection.Save();
        }
      }
    }

    public void LoadCustomControls(int width)
    {
      LoadCustomControls(width, 0, null);
    }

    public void LoadCustomControls(int width, int currentColumn, HtmlTableRow currentRow)
    {
      _customControls.Clear();
      //Ticket ticket = (Ticket)Tickets.GetTicket(UserSession.LoginUser, ticketID);

      //if (ticket == null) return;

      CustomFields fields = new CustomFields(UserSession.LoginUser);
      fields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, _refType, _auxID);

      int cols = 0;
      HtmlTableRow tr = null;

      if (currentRow != null)
      {
        cols = currentColumn;
        tr = currentRow;
      }
      //string labelTDClass = "label" + _columns.ToString() + "ColTD";
      //string inputTDClass = "input" + _columns.ToString() + "ColTD";
      string labelTDClass = "labelColTD";
      string inputTDClass = "inputColTD";
      foreach (CustomField field in fields)
      {
        if (field.CustomFieldCategoryID == null) tr = AddCells(tr, field, labelTDClass, inputTDClass, width, ref cols);
      }
      
      if (tr != null) _table.Controls.Add(tr);
      cols = 0;

      CustomFieldCategories cats = new CustomFieldCategories(UserSession.LoginUser);
      cats.LoadByRefType(_refType, _auxID);

      foreach (CustomFieldCategory cat in cats)
      {
        if (tr != null) _table.Controls.Add(tr);
        cols = 0;
        tr = new HtmlTableRow();
        HtmlTableCell tdCat = new HtmlTableCell();
        tdCat.InnerHtml = cat.Category;
        tdCat.Attributes.Add("style", "font-size: 1.5em;");
        tdCat.Attributes.Add("colspan", _columns.ToString());
        tr.Controls.Add(tdCat);
        _table.Controls.Add(tr);
        tr = null;

        foreach (CustomField field in fields)
        {
          if (field.CustomFieldCategoryID == cat.CustomFieldCategoryID) tr = AddCells(tr, field, labelTDClass, inputTDClass, width, ref cols);
        }
      }
      
      if (tr != null)
      {
        HtmlTableCell tdLabel = new HtmlTableCell();
        tdLabel.InnerHtml = @"<div class=""tableLabelDiv"">&nbsp</div>";
        tdLabel.Attributes.Add("class", labelTDClass);
        tr.Controls.Add(tdLabel);

        HtmlTableCell tdInput = new HtmlTableCell();
        tdInput.InnerHtml = "&nbsp";
        tdInput.Attributes.Add("class", inputTDClass);
        tr.Controls.Add(tdInput);

        _table.Controls.Add(tr);
      }
    }

    private HtmlTableRow AddCells(HtmlTableRow tr, CustomField field, string labelTDClass, string inputTDClass, int width, ref int cols)
    {
      if (tr == null) tr = new HtmlTableRow();
      HtmlTableCell tdLabel = new HtmlTableCell();
      tdLabel.InnerHtml = @"<div class=""tableLabelDiv"">" + field.Name + ":</div>";
      tdLabel.Attributes.Add("class", labelTDClass);
      tr.Controls.Add(tdLabel);

      HtmlTableCell tdInput = new HtmlTableCell();
      tdInput.Attributes.Add("class", inputTDClass);
      Control fieldControl = CreateFieldControl(field, width);
      if (fieldControl != null)
      {
        tdInput.Controls.Add(fieldControl);
        Label validator = new Label();
        validator.Text = "*";
        validator.CssClass = "customFieldValid";
        // tdInput.Controls.Add(validator);
      }
      tr.Controls.Add(tdInput);

      cols++;
      if (cols > _columns - 1)
      {
        _table.Controls.Add(tr);
        cols = 0;
        tr = null;
      }
      return tr;
    
    }

    public void ClearCustomControls()
    {
      _customControls.Clear();
      _table.Controls.Clear();
    }

    private Control CreateFieldControl(CustomField field, int width)
    {
      Control result;
      switch (field.FieldType)
      {
        case CustomFieldType.Text: result = CreateTextControl(field, width); break;
        case CustomFieldType.Number: result = CreateNumberControl(field, width); break;
        case CustomFieldType.Date: result = CreateDateControl(field, width); break;
        case CustomFieldType.Time: result = CreateTimeControl(field, width); break;
        case CustomFieldType.DateTime: result = CreateDateTimeControl(field, width); break;
        case CustomFieldType.Boolean: result = CreateBooleanControl(field); break;
        case CustomFieldType.PickList: result = CreatePickListControl(field, width); break;
        default: result = null; break;
      }

      return result;
    }

    private int ControlIDToFieldID(string controlID)
    {
      int i = controlID.LastIndexOf('_') + 1;
      string s = "";
      if (i > 1) s = controlID.Substring(i, controlID.Length - i);

      return int.Parse(s);
    }

    private string FieldIDToControlID(int fieldID)
    {
      return "CustomControl_" + fieldID.ToString();
    }

    private WebControl CreateTextControl(CustomField field, int width)
    {
      RadTextBox result = new RadTextBox();
      result.Width = Unit.Pixel(width-3);
      if (_autoSave) result.ClientEvents.OnBlur =  "function(sender, args) { top.privateServices.SaveCustomFieldText(" +_refID.ToString() + "," +field.CustomFieldID.ToString()+", sender.get_value()); }";
      if (_clientSaveEvent != "") result.ClientEvents.OnBlur = _clientSaveEvent;
      result.ID = FieldIDToControlID(field.CustomFieldID);
      _customControls.Add(new CustomControl(result, "$find('{0}').get_value()", "SaveCustomFieldText", _refID, field.CustomFieldID));
      if (field.IsRequired) result.CssClass = "validate(required)";

      if (!String.IsNullOrEmpty(field.Mask))
      {
        result.CssClass += " masked";
        result.Attributes.Add("placeholder", field.Mask);
      }

      return result;
    }

    private WebControl CreateNumberControl(CustomField field, int width)
    {
      RadNumericTextBox result = new RadNumericTextBox();
      result.Width = Unit.Pixel(width-3);
      result.AutoPostBack = _autoSave;
      if (_autoSave) result.ClientEvents.OnBlur = "function(sender, args) { top.privateServices.SaveCustomFieldText(" + _refID.ToString() + "," + field.CustomFieldID.ToString() + ", sender.get_value()); }";
      if (_clientSaveEvent != "") result.ClientEvents.OnBlur = _clientSaveEvent;
      if (field.IsRequired) result.CssClass = "validate(required)";

      result.ID = FieldIDToControlID(field.CustomFieldID);
      _customControls.Add(new CustomControl(result, "$find('{0}').get_value()", "SaveCustomFieldText", _refID, field.CustomFieldID));
      return result;
    }

    private WebControl CreateDateControl(CustomField field, int width)
    {
      RadDatePicker result = new RadDatePicker();
      result.Width = Unit.Pixel(width);
      if (_autoSave) result.ClientEvents.OnDateSelected = "function(sender, args) { top.privateServices.SaveCustomFieldDate(" + _refID.ToString() + "," + field.CustomFieldID.ToString() + ", args.get_newValue()); }";
      if (_clientSaveEvent != "") result.ClientEvents.OnDateSelected = _clientSaveEvent;
      result.MinDate = new DateTime(1900, 1, 1);
      result.ID = FieldIDToControlID(field.CustomFieldID);
      result.Culture = UserSession.LoginUser.CultureInfo;
      if (field.IsRequired) result.CssClass = "validateDateTime";
      //_customControls.Add(new CustomControl(result, "/*$find('{0}').get_selectedDate()*/ new Date()", "SaveCustomFieldDate", _refID, field.CustomFieldID));
      _customControls.Add(new CustomControl(result, "ConvertDate($find('{0}').get_selectedDate())", "SaveCustomFieldDate", _refID, field.CustomFieldID));
      return result;


    }

    private WebControl CreateTimeControl(CustomField field, int width)
    {
      RadTimePicker result = new RadTimePicker();
      result.Width = Unit.Pixel(width);
      if (_autoSave) result.ClientEvents.OnDateSelected = "function(sender, args) { top.privateServices.SaveCustomFieldDate(" + _refID.ToString() + "," + field.CustomFieldID.ToString() + ", args.get_newValue()); }";
      if (_clientSaveEvent != "") result.ClientEvents.OnDateSelected = _clientSaveEvent;
      result.MinDate = new DateTime(1900, 1, 1);
      result.SelectedDate = new DateTime(1970, 1, 1, 0, 0, 0);
      result.ID = FieldIDToControlID(field.CustomFieldID);
      result.Culture = UserSession.LoginUser.CultureInfo;
      if (field.IsRequired) result.CssClass = "validateDateTime";
      //_customControls.Add(new CustomControl(result, "/*$find('{0}').get_selectedDate()*/ new Date()", "SaveCustomFieldDate", _refID, field.CustomFieldID));
      _customControls.Add(new CustomControl(result, "ConvertDate($find('{0}').get_selectedDate())", "SaveCustomFieldDate", _refID, field.CustomFieldID));
      return result;


    }

    private WebControl CreateDateTimeControl(CustomField field, int width)
    {
      RadDateTimePicker result = new RadDateTimePicker();
      result.Width = Unit.Pixel(width);
      if (_autoSave) result.ClientEvents.OnDateSelected = "function(sender, args) { top.privateServices.SaveCustomFieldDate(" + _refID.ToString() + "," + field.CustomFieldID.ToString() + ", args.get_newValue()); }";
      if (_clientSaveEvent != "") result.ClientEvents.OnDateSelected = _clientSaveEvent;
      result.MinDate = new DateTime(1900, 1, 1);
      result.ID = FieldIDToControlID(field.CustomFieldID);
      result.Culture = UserSession.LoginUser.CultureInfo;
      if (field.IsRequired) result.CssClass = "validateDateTime";
      //_customControls.Add(new CustomControl(result, "/*$find('{0}').get_selectedDate()*/ new Date()", "SaveCustomFieldDate", _refID, field.CustomFieldID));
      _customControls.Add(new CustomControl(result, "ConvertDate($find('{0}').get_selectedDate())", "SaveCustomFieldDate", _refID, field.CustomFieldID));
      return result;


    }

    private WebControl CreateBooleanControl(CustomField field)
    {
      CheckBox result = new CheckBox();
      result.ID = FieldIDToControlID(field.CustomFieldID);

      if (_clientSaveEvent != "") result.Attributes.Add("onclick", _clientSaveEvent + "(this);");
      else if (_autoSave) result.Attributes.Add("onclick", "function() { top.privateServices.SaveCustomFieldBool(" + _refID.ToString() + "," + field.CustomFieldID.ToString() + ", this.checked); }");
      _customControls.Add(new CustomControl(result, "document.getElementById('{0}').checked", "SaveCustomFieldBool", _refID, field.CustomFieldID));
      return result;
    }

    private WebControl CreatePickListControl(CustomField field, int width)
    {
      RadComboBox result = new RadComboBox();

      string[] items = field.ListValues.Split('|');

      foreach (string item in items)
      {
        result.Items.Add(new RadComboBoxItem(item, item));
      }

      result.Width = Unit.Pixel(width);
      if (_autoSave) result.OnClientSelectedIndexChanged = "function(sender, args) { top.privateServices.SaveCustomFieldText(" + _refID.ToString() + "," + field.CustomFieldID.ToString() + ", sender.get_text()); if (customPickListChanged) customPickListChanged(sender, args);}";
      else result.OnClientSelectedIndexChanged = "function(sender, args) { customPickListChanged(sender, args);}";
      if (_clientSaveEvent != "") result.OnClientSelectedIndexChanged = _clientSaveEvent;
      result.ID = FieldIDToControlID(field.CustomFieldID);
      if (field.IsRequired)
      {
        result.CssClass = field.IsFirstIndexSelect ? "validateCombo1" : "validateCombo";
      }
      
      _customControls.Add(new CustomControl(result, "$find('{0}').get_text()", "SaveCustomFieldText", _refID, field.CustomFieldID));
      return result;
    }


  }
}
