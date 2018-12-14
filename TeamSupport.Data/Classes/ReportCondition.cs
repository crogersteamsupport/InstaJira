using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace TeamSupport.Data
{
  public enum ReportDataType
  {
    String,
    Int,
    Float,
    DateTime,
    Boolean
  }

  public enum ConditionOperator
  {
    IsEqualTo,
    IsNotEqualTo,
    IsGreaterThan,
    IsLessThan,
    IsInBetween,
    IsNotInBetween,
    StartsWith,
    EndsWith,
    Contains
  }

  [Serializable]
  public class ReportCondition
  {
    bool _isCustomField;

    public bool IsCustomField
    {
      get { return _isCustomField; }
      set { _isCustomField = value; }
    }
    int _fieldID;

    public int FieldID
    {
      get { return _fieldID; }
      set { _fieldID = value; }
    }

    string _displayName;

    public string DisplayName
    {
      get { return _displayName; }
      set { _displayName = value; }
    }

    ConditionOperator _conditionOperator;
    public ConditionOperator ConditionOperator
    {
      get { return _conditionOperator; }
      set { _conditionOperator = value; }
    }
    object _value1;

    public object Value1
    {
      get { return _value1; }
      set { _value1 = value; }
    }
    object _value2;

    public object Value2
    {
      get { return _value2; }
      set { _value2 = value; }
    }
  }

  [Serializable]
  public class ReportConditions
  {
    private LoginUser _loginUser;
    public LoginUser LoginUser
    {
      get { return _loginUser; }
      set { _loginUser = value; }
    }

    private List<ReportCondition> _items;
    public List<ReportCondition> Items
    {
      get { return _items; }
      set { _items = value; }
    }

    private bool _matchAll = true;
    public bool MatchAll
    {
      get { return _matchAll; }
      set { _matchAll = value; }
    }

    public ReportConditions(LoginUser loginUser)
    {
      _loginUser = loginUser;
      _items = new List<ReportCondition>();
    }

    public override string ToString()
    {
      StringBuilder builder = new StringBuilder();
      for (int i = 0; i < _items.Count; i++)
      {
        ReportCondition condition = _items[i];

        builder.Append("<div>");
        builder.Append("<a href=\"#\" onclick=\"SendAjaxRequest('DeleteCondition," + i.ToString() + "');\">(X)</a> ");
        builder.Append(condition.DisplayName);
        switch (condition.ConditionOperator)
        {
          case ConditionOperator.IsEqualTo:
            builder.Append(" is equal to ");
            break;
          case ConditionOperator.IsNotEqualTo:
            builder.Append(" is not equal to ");
            break;
          case ConditionOperator.IsGreaterThan:
            builder.Append(" is greater than ");
            break;
          case ConditionOperator.IsLessThan:
            builder.Append(" is less than ");
            break;
          case ConditionOperator.IsInBetween:
            builder.Append(" is in between ");
            break;
          case ConditionOperator.IsNotInBetween:
            builder.Append(" is not in between ");
            break;
          case ConditionOperator.StartsWith:
            builder.Append(" starts with ");
            break;
          case ConditionOperator.EndsWith:
            builder.Append(" ends with ");
            break;
          case ConditionOperator.Contains:
            builder.Append(" contains ");
            break;
          default:
            break;
        }
        if (condition.Value1 is DateTime)
          builder.Append(((DateTime)condition.Value1).ToString("d", _loginUser.CultureInfo));
        else
          builder.Append(condition.Value1.ToString());

        if (condition.ConditionOperator == ConditionOperator.IsInBetween || condition.ConditionOperator == ConditionOperator.IsNotInBetween)
        {
          if (condition.Value2 is DateTime)
            builder.Append(" and " + ((DateTime)condition.Value2).ToString("d", _loginUser.CultureInfo));
          else
            builder.Append(" and " + condition.Value2.ToString());
        }
        builder.AppendLine("</div>");
        builder.AppendLine();
      }
      return builder.ToString();
    }

    public string GetSQL()
    {
      StringBuilder builder = new StringBuilder();
      ReportTables tables = new ReportTables(_loginUser);
      tables.LoadAll();
      string con = _matchAll ? " AND " : " OR ";

      foreach (ReportCondition condition in _items)
      {
        string value = "";


        if (condition.Value1 is string && condition.Value1.ToString().ToLower() == "self")
        { 
          User user = Users.GetUser(_loginUser, _loginUser.UserID);
          condition.Value1 = user.FirstLastName;
        }

        if (condition.Value2 is string && condition.Value2.ToString().ToLower() == "self")
        {
          User user = Users.GetUser(_loginUser, _loginUser.UserID);
          condition.Value2 = user.FirstLastName;
        }

        if (!condition.IsCustomField)
        {
          ReportTableField field = (ReportTableField)ReportTableFields.GetReportTableField(_loginUser, condition.FieldID);

          if (condition.Value1.ToString().Trim() == "" && (condition.ConditionOperator == ConditionOperator.IsEqualTo || condition.ConditionOperator == ConditionOperator.IsNotEqualTo))
          {
              //ticketsview.customers will not be null
              if (field.ReportTableFieldID == 251) {

                  if (condition.ConditionOperator == ConditionOperator.IsEqualTo)
                  {
                      value = " = ''";
                  }
                  else if (condition.ConditionOperator == ConditionOperator.IsNotEqualTo)
                  {
                      value = " <> ''";
                  }
              }
              else
              {
                  if (condition.ConditionOperator == ConditionOperator.IsEqualTo)
                  {
                      value = " IS NULL";
                  }
                  else if (condition.ConditionOperator == ConditionOperator.IsNotEqualTo)
                  {
                      value = " IS NOT NULL";
                  }

              }
          }
          else
          {
            switch (field.DataType)
            {
              case "bit":
                if (condition.ConditionOperator == ConditionOperator.IsEqualTo)
                  value = " = ";
                else
                  value = " <> ";
                if ((bool)condition.Value1)
                  value = value + "1";
                else
                  value = value + "0";
                break;
              case "datetime":

                if (condition.ConditionOperator == ConditionOperator.IsGreaterThan || condition.ConditionOperator == ConditionOperator.IsLessThan)
                {

                  switch (condition.ConditionOperator)
                  {
                    case ConditionOperator.IsGreaterThan:
                      value = " > ";
                      break;
                    case ConditionOperator.IsLessThan:
                      value = " < ";
                      break;
                    default:
                      break;
                  }

                  DateTime date = (DateTime)condition.Value1;
                  value = value + "'" + DataUtils.DateToUtc(_loginUser, date).ToString("g", CultureInfo.GetCultureInfo("en-US")) + "'";
                }
                else
                {
                  DateTime date1 = (DateTime)condition.Value1;
                  DateTime date2;
                  if (condition.ConditionOperator == ConditionOperator.IsEqualTo || condition.ConditionOperator == ConditionOperator.IsNotEqualTo)
                    date2 = date1.AddDays(1);
                  else
                    date2 = (DateTime)condition.Value2;

                  value = " BETWEEN '" + DataUtils.DateToUtc(_loginUser, date1).ToString("g", CultureInfo.GetCultureInfo("en-US")) + "' AND '" + DataUtils.DateToUtc(_loginUser, date2).ToString("g", CultureInfo.GetCultureInfo("en-US")) + "'";
                  if (condition.ConditionOperator == ConditionOperator.IsNotInBetween || condition.ConditionOperator == ConditionOperator.IsNotEqualTo)
                    value = " NOT" + value;
                }
                break;
              case "int":
                if (condition.ConditionOperator != ConditionOperator.IsInBetween && condition.ConditionOperator != ConditionOperator.IsNotInBetween)
                {

                  switch (condition.ConditionOperator)
                  {
                    case ConditionOperator.IsEqualTo:
                      value = " = ";
                      break;
                    case ConditionOperator.IsNotEqualTo:
                      value = " <> ";
                      break;
                    case ConditionOperator.IsGreaterThan:
                      value = " > ";
                      break;
                    case ConditionOperator.IsLessThan:
                      value = " < ";
                      break;
                    default:
                      break;
                  }

                  int i = (int)condition.Value1;
                  value = value + i.ToString();
                }
                else
                {
                  int i1 = (int)condition.Value1;
                  int i2 = (int)condition.Value2;
                  value = " BETWEEN " + i1.ToString() + " AND " + i2.ToString();
                  if (condition.ConditionOperator == ConditionOperator.IsNotInBetween)
                    value = " NOT" + value;
                }
                break;
              case "float":
                if (condition.ConditionOperator != ConditionOperator.IsInBetween && condition.ConditionOperator != ConditionOperator.IsNotInBetween)
                {

                  switch (condition.ConditionOperator)
                  {
                    case ConditionOperator.IsEqualTo:
                      value = " = ";
                      break;
                    case ConditionOperator.IsNotEqualTo:
                      value = " <> ";
                      break;
                    case ConditionOperator.IsGreaterThan:
                      value = " > ";
                      break;
                    case ConditionOperator.IsLessThan:
                      value = " < ";
                      break;
                    default:
                      break;
                  }

                  double d = (double)condition.Value1;
                  value = value + d.ToString();
                }
                else
                {
                  int i1 = (int)condition.Value1;
                  int i2 = (int)condition.Value2;
                  value = " BETWEEN " + i1.ToString() + " AND " + i2.ToString();
                  if (condition.ConditionOperator == ConditionOperator.IsNotInBetween)
                    value = " NOT" + value;
                }
                break;
              default:
                switch (condition.ConditionOperator)
                {
                  case ConditionOperator.IsEqualTo:
                    value = " = '" + condition.Value1.ToString() + "'";
                    break;
                  case ConditionOperator.IsNotEqualTo:
                    value = " <> '" + condition.Value1.ToString() + "'";
                    break;
                  case ConditionOperator.StartsWith:
                    value = " LIKE '" + condition.Value1.ToString() + "%'";
                    break;
                  case ConditionOperator.EndsWith:
                    value = " LIKE '%" + condition.Value1.ToString() + "'";
                    break;
                  case ConditionOperator.Contains:
                    value = " LIKE '%" + condition.Value1.ToString() + "%'";
                    break;
                  default:
                    break;
                }
                break;
            }
          }

          value = tables.FindByReportTableID(field.ReportTableID).TableName + ".[" + field.FieldName +"]" + value;
        }
        else
        {
          CustomField field = (CustomField)CustomFields.GetCustomField(_loginUser, condition.FieldID);

          if (condition.Value1.ToString().Trim() == "")
          {
            if (condition.ConditionOperator == ConditionOperator.IsEqualTo)
            {
              value = " IS NULL";
            }
            else if (condition.ConditionOperator == ConditionOperator.IsNotEqualTo)
            {
              value = " IS NOT NULL";
            }
          }
          else
          {
            switch (field.FieldType)
            {
              case CustomFieldType.Text:
              case CustomFieldType.PickList:
                switch (condition.ConditionOperator)
                {
                  case ConditionOperator.IsEqualTo:
                    value = " = '" + condition.Value1.ToString() + "'";
                    break;
                  case ConditionOperator.IsNotEqualTo:
                    value = " <> '" + condition.Value1.ToString() + "'";
                    break;
                  case ConditionOperator.StartsWith:
                    value = " LIKE '" + condition.Value1.ToString() + "%'";
                    break;
                  case ConditionOperator.EndsWith:
                    value = " LIKE '%" + condition.Value1.ToString() + "'";
                    break;
                  case ConditionOperator.Contains:
                    value = " LIKE '%" + condition.Value1.ToString() + "%'";
                    break;
                  default:
                    break;
                }
                break;
              case CustomFieldType.Date:
              case CustomFieldType.Time:
              case CustomFieldType.DateTime:
                if (condition.ConditionOperator != ConditionOperator.IsInBetween && condition.ConditionOperator != ConditionOperator.IsNotInBetween)
                {

                  switch (condition.ConditionOperator)
                  {
                    case ConditionOperator.IsEqualTo:
                      value = " = ";
                      break;
                    case ConditionOperator.IsNotEqualTo:
                      value = " <> ";
                      break;
                    case ConditionOperator.IsGreaterThan:
                      value = " > ";
                      break;
                    case ConditionOperator.IsLessThan:
                      value = " < ";
                      break;
                    default:
                      break;
                  }

                  DateTime date = (DateTime)condition.Value1;
                  value = value + "'" + DataUtils.DateToUtc(_loginUser, date).ToString("g", _loginUser.CultureInfo) + "'";
                }
                else
                {
                  DateTime date1 = (DateTime)condition.Value1;
                  DateTime date2 = (DateTime)condition.Value2;
                  value = " BETWEEN '" + DataUtils.DateToUtc(_loginUser, date1).ToString("g", _loginUser.CultureInfo) + "' AND '" + DataUtils.DateToUtc(_loginUser, date2).ToString("g", _loginUser.CultureInfo) + "'";
                  if (condition.ConditionOperator == ConditionOperator.IsNotInBetween)
                    value = " NOT" + value;
                }
                break;
              case CustomFieldType.Boolean:
                if (condition.ConditionOperator == ConditionOperator.IsEqualTo)
                  value = " = ";
                else
                  value = " <> ";
                if ((bool)condition.Value1)
                  value = value + "'True'";
                else
                  value = value + "'False'";
                break;
              case CustomFieldType.Number:
                if (condition.ConditionOperator != ConditionOperator.IsInBetween && condition.ConditionOperator != ConditionOperator.IsNotInBetween)
                {

                  switch (condition.ConditionOperator)
                  {
                    case ConditionOperator.IsEqualTo:
                      value = " = ";
                      break;
                    case ConditionOperator.IsNotEqualTo:
                      value = " <> ";
                      break;
                    case ConditionOperator.IsGreaterThan:
                      value = " > ";
                      break;
                    case ConditionOperator.IsLessThan:
                      value = " < ";
                      break;
                    default:
                      break;
                  }

                  int i = (int)condition.Value1;
                  value = value + i.ToString();
                }
                else
                {
                  int i1 = (int)condition.Value1;
                  int i2 = (int)condition.Value2;
                  value = " BETWEEN " + i1.ToString() + " AND " + i2.ToString();
                  if (condition.ConditionOperator == ConditionOperator.IsNotInBetween)
                    value = " NOT" + value;
                }
                break;
              default:
                break;
            }
          }

          value = DataUtils.GetCustomFieldColumn(_loginUser, field, DataUtils.GetReportPrimaryKeyFieldName(field.RefType), true, false) + value;
        }
        if (builder.Length < 1)
        {
          builder.Append("((" + value + ")");
        }
        else
        {
          builder.Append(con + "(" + value + ")");

        }
      }
      if (builder.Length > 0) builder.Append(")");

      return builder.ToString().Trim();
    }

  }
}
