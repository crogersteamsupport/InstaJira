/* Key Words

<$ItemName$>
<$itemName$>
<$CollectionName$>
<$collectionName$>
<$PrimaryKey$>
<$primaryKey$>

<$BeginColumns=[ReadOnly, Insert, Delete, Nullable]$> "!" for not
<$CustomType$>
<$DBType$>
<$ColumnName$>
<$ColumnSize$>
<$EndColumns$>
  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace BusinessObjectGenerator
{

  public class ObjectGenerator
  {
    private enum FieldFilterType
    {
      All,
      DateTime,
      ReadOnly,
      AllowDBNull,
      Update,
      Insert,
      Nullable,
      NotDateTime,
      NotReadOnly,
      NotAllowDBNull,
      NotUpdate,
      NotInsert,
      NotNullable
    }

    public ObjectGenerator(string itemName, string collectionName, string tableName, string primaryKey, string conditions, bool isReadOnly)
    {
      _fields = new List<Field>();
      _isReadOnly = isReadOnly;
      _tableName = tableName;
      _primaryKey = primaryKey;
      _itemName = itemName;
      _collectionName = collectionName;
      _conditions = conditions.Split(',');

      for (int i = 0; i < _conditions.Length; i++)
      {
        _conditions[i] = _conditions[i].Trim();
      }
    }

    #region Properties

    private string _itemName;
    public string ItemName
    {
      get { return _itemName; }
    }

    private string _collectionName;
    public string CollectionName
    {
      get { return _collectionName; }
    }

    private string _tableName;
    public string TableName
    {
      get { return _tableName; }
    }

    private string _primaryKey;
    public string PrimaryKey
    {
      get { return _primaryKey; }
    }

    private string _modifierIDFieldName = "";
    public string ModifierIDFieldName
    {
      get { return _modifierIDFieldName; }
      set { _modifierIDFieldName = value; }
    }

    private string _dateModifiedFieldName = "";
    public string DateModifiedFieldName
    {
      get { return _dateModifiedFieldName; }
      set { _dateModifiedFieldName = value; }
    }

    private string _creatorIDFieldName = "";
    public string CreatorIDFieldName
    {
      get { return _creatorIDFieldName; }
      set { _creatorIDFieldName = value; }
    }

    private string _dateCreatedFieldName = "";
    public string DateCreatedFieldName
    {
      get { return _dateCreatedFieldName; }
      set { _dateCreatedFieldName = value; }
    }

    private string _baseItemClass = "";
    public string BaseItemClass
    {
      get { return _baseItemClass; }
      set { _baseItemClass = value; }
    }

    private string _baseCollectionClass = "";
    public string BaseCollectionClass
    {
      get { return _baseCollectionClass; }
      set { _baseCollectionClass = value; }
    }

    private string[] _conditions;

    public string[] Conditions
    {
      get { return _conditions; }
      set { _conditions = value; }
    }

    private bool _isReadOnly;
    public bool IsReadOnly
    {
      get { return _isReadOnly; }
      set { _isReadOnly = value; }
    }
      

    #endregion

    #region Private Members

    private List<Field> _fields;

    private string LowerCaseFirstLetter(string text)
    {
      if (text.Length < 1) return "";

      char c = char.ToLower(text[0]);
      return c + text.Remove(0, 1);
    }

    private string ConfirmNullableType(string type, bool allowDBNulls)
    {

      if (allowDBNulls && IsNullableType(type))
      {
        return type + "?";
      }
      return type;
    }

    private bool IsNullableType(string type)
    {
      bool result = false;
        
      switch (ConvertSystemTypeToShortHand(type).ToLower())
      {
        case "guid":
        case "byte":
        case "char":
        case "bool":
        case "sbyte":
        case "short":
        case "ushort":
        case "int":
        case "uint":
        case "long":
        case "ulong":
        case "float":
        case "double":
        case "decimal":
        case "datetime": result = true; break;
      }
      return result;


    }

    public static string ConvertSystemTypeToShortHand(string systemType)
    {
      switch (systemType)
      {
        case "System.Byte": return "byte";
        case "System.Char": return "char";
        case "System.Boolean": return "bool"; 
        case "System.SByte": return "sbyte";
        case "System.Int16": return "short";
        case "System.UInt16": return "ushort";
        case "System.Int32": return "int";
        case "System.UInt32": return "uint";
        case "System.Int64": return "long";
        case "System.UInt64": return "ulong";
        case "System.Single": return "float";
        case "System.Double": return "double";
        case "System.Decimal": return "double";
        case "System.Object": return "object";
        case "System.String": return "string";
        default: return systemType.Replace("System.", "");
      }
    }

    private bool IsUpdateField(string fieldName)
    {
      //return !(fieldName == _dateModifiedFieldName || fieldName == _dateCreatedFieldName || fieldName == _creatorIDFieldName);
      return !(fieldName == _dateCreatedFieldName || fieldName == _creatorIDFieldName);
    }

    private bool IsInsertField(Field field)
    {
      //return !(field.FieldName == _dateModifiedFieldName || field.FieldName == _dateCreatedFieldName || field.IsAutoInc);
      return !(field.IsAutoInc);
    }

    private bool IsConditionOk(string condition)
    {
      foreach (string s in _conditions)
      {
        if (s == condition)
        {
          return true;
        }
      }

      return false;
    }

    private FieldFilterType GetFilterType(string text)
    {
      FieldFilterType result = FieldFilterType.ReadOnly;
      switch (text)
      {
        case "All": result = FieldFilterType.All; break;
        case "DateTime": result = FieldFilterType.DateTime; break;
        case "ReadOnly": result = FieldFilterType.ReadOnly; break;
        case "AllowDBNull": result = FieldFilterType.AllowDBNull; break;
        case "Insert": result = FieldFilterType.Insert; break;
        case "Update": result = FieldFilterType.Update; break;
        case "Nullable": result = FieldFilterType.Nullable; break;
        case "!DateTime": result = FieldFilterType.NotDateTime; break;
        case "!ReadOnly": result = FieldFilterType.NotReadOnly; break;
        case "!AllowDBNull": result = FieldFilterType.NotAllowDBNull; break;
        case "!Insert": result = FieldFilterType.NotInsert; break;
        case "!Update": result = FieldFilterType.NotUpdate; break;
        case "!Nullable": result = FieldFilterType.NotNullable; break;
        default: result = FieldFilterType.ReadOnly; break;
      }
      return result;

    }

    #endregion

    #region Public Members

    public Field AddField()
    {
      Field field = new Field();
      _fields.Add(field);
      return field;
    }

    public string Generate(string templateText)
    {
      StoredProcGenerator procs = new StoredProcGenerator(_tableName, _itemName, _isReadOnly);
      procs.CreatorIDFieldName = CreatorIDFieldName;
      procs.DateCreatedFieldName = DateCreatedFieldName;
      procs.ModifierIDFieldName = ModifierIDFieldName;
      procs.DateModifiedFieldName = DateModifiedFieldName;

      procs._fields = _fields;

      string output = templateText;
      output = output.Replace("<$itemName$>", LowerCaseFirstLetter(_itemName));
      output = output.Replace("<$ItemName$>", _itemName);
      output = output.Replace("<$collectionName$>", LowerCaseFirstLetter(_collectionName));
      output = output.Replace("<$CollectionName$>", _collectionName);
      output = output.Replace("<$primaryKey$>", LowerCaseFirstLetter(_primaryKey));
      output = output.Replace("<$PrimaryKey$>", _primaryKey);
      output = output.Replace("<$BaseItemClass$>", _baseItemClass);
      output = output.Replace("<$BaseCollectionClass$>", _baseCollectionClass);
      output = output.Replace("<$TableName$>", _tableName);
      output = output.Replace("<$QueryInsert$>", procs.GenerateInsertQuery());
      output = output.Replace("<$QueryUpdate$>", procs.GenerateUpdateQuery());
      output = output.Replace("<$QueryDelete$>", procs.GenerateDeleteQuery());
      output = output.Replace("<$QuerySelect$>", procs.GenerateSelectQuery());
      
      int begin;
      int end;
      
      begin = output.IndexOf("<$BeginFields=[");
      end = output.IndexOf("<$EndFields$>");
      while (begin > -1 && end > -1)
      {
        int filterEnd = output.IndexOf(']', begin);
        string filter = output.Substring(begin + 15, filterEnd - begin - 15);
        string[] filters = filter.Split(',');
        List<FieldFilterType> filterTypes = new List<FieldFilterType>();
        foreach (string s in filters)
        {
          filterTypes.Add(GetFilterType(s.Trim()));
        }

        int contentStart = output.IndexOf("$>", begin);
        string content = output.Substring(contentStart + 2, end - contentStart - 2);
        output = output.Remove(begin, end - begin + 13);
        _fields.Reverse();

        foreach (Field field in _fields)
        {
          string dataType = ConfirmNullableType(field.CustomType, field.AllowDBNull);
          bool fieldOk = true;

          foreach (FieldFilterType filterType in filterTypes)
          {
            if (!fieldOk) break;
            switch (filterType)
            {
              case FieldFilterType.All: fieldOk = true; break;
              case FieldFilterType.DateTime: fieldOk = field.SystemType == "System.DateTime"; break;
              case FieldFilterType.ReadOnly: fieldOk = field.IsReadOnly; break;
              case FieldFilterType.AllowDBNull: fieldOk = field.AllowDBNull; break;
              case FieldFilterType.Update: fieldOk = IsUpdateField(field.FieldName); break;
              case FieldFilterType.Insert: fieldOk = IsInsertField(field); break;
              case FieldFilterType.Nullable: fieldOk = field.AllowDBNull; break;
              case FieldFilterType.NotDateTime: fieldOk = field.SystemType != "System.DateTime"; break;
              case FieldFilterType.NotReadOnly: fieldOk = !field.IsReadOnly; break;
              case FieldFilterType.NotAllowDBNull: fieldOk = !field.AllowDBNull; break;
              case FieldFilterType.NotUpdate: fieldOk = !IsUpdateField(field.FieldName); break;
              case FieldFilterType.NotInsert: fieldOk = !IsInsertField(field); break;
              case FieldFilterType.NotNullable: fieldOk = !field.AllowDBNull; break;
              default: fieldOk = false; break;
            }
          }

          if (fieldOk)
          {
            string selection = content;
            selection = selection.Replace("<$DataType$>", dataType);
            selection = selection.Replace("<$DBType$>", field.SqlDbType);
            selection = selection.Replace("<$PropertyName$>", field.PropertyName);
            selection = selection.Replace("<$propertyName$>", LowerCaseFirstLetter(field.PropertyName));
            selection = selection.Replace("<$FieldName$>", field.FieldName);
            selection = selection.Replace("<$fieldName$>", LowerCaseFirstLetter(field.FieldName));
            selection = selection.Replace("<$FieldSize$>", field.Size.ToString());
            selection = selection.Replace("<$FieldPrecision$>", field.Precision.ToString());
            selection = selection.Replace("<$FieldScale$>", field.Precision.ToString());
            output = output.Insert(begin, selection);
          }
        }
        
        begin = output.IndexOf("<$BeginFields=[");
        end = output.IndexOf("<$EndFields$>");
      }

      while (true)
      {
        begin = output.IndexOf("<$BeginCondition=[");
        if (begin < 0) break;
        int close = output.IndexOf("]", begin);
        string condition = output.Substring(begin + 18, close - begin - 18);
        end = output.IndexOf("<$EndCondition$>");

        if (IsConditionOk(condition))
        {
          //delete begin and end flags
          output = output.Remove(end, 16);
          output = output.Remove(begin, close - begin + 3);
        }
        else
        {
          //delete whole block
          output = output.Remove(begin, end - begin + 16);
        }
      }

      return output;
    }



    #endregion




  }
}
