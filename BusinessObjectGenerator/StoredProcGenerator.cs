using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace BusinessObjectGenerator
{
  

  class StoredProcGenerator
  {
    

    public StoredProcGenerator(string tableName, string itemName, bool isReadOnly)
    {
      _tableName = tableName;
      _itemName = itemName;
      _isReadOnly = isReadOnly; 
      _fields = new List<Field>();
      _builder = new StringBuilder();
    }

    #region Private Members

    private string _tableName;
    private string _itemName;
    private StringBuilder _builder;
    public List<Field> _fields;
    private string _dateCreatedFieldName = "";
    private string _creatorIDFieldName = "";
    private string _dateModifiedFieldName = "";
    private string _modifierIDFieldName = "";
    private bool _isReadOnly = false;

    #endregion

    #region Properties

    

    public string TableName
    {
      get { return _tableName; }
    }

    public string ItemName
    {
      get { return _itemName; }
      set { _itemName = value; }
    }

    public bool IsReadOnly
    {
      get { return _isReadOnly; }
    }

    public string ModifierIDFieldName
    {
      get { return _modifierIDFieldName; }
      set { _modifierIDFieldName = value; }
    }

    public string DateModifiedFieldName
    {
      get { return _dateModifiedFieldName; }
      set { _dateModifiedFieldName = value; }
    }

    public string CreatorIDFieldName
    {
      get { return _creatorIDFieldName; }
      set { _creatorIDFieldName = value; }
    }

    public string DateCreatedFieldName
    {
      get { return _dateCreatedFieldName; }
      set { _dateCreatedFieldName = value; }
    }

    #endregion

    public Field AddField()
    {
      Field field = new Field();
      _fields.Add(field);
      return field;
    }

    private string GetHeader(string name)
    {
      StringBuilder builder = new StringBuilder();
      builder.Append("IF EXISTS (SELECT * FROM sysobjects WHERE name = '");
      builder.Append(name);
      builder.Append("' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.");
      builder.Append(name);
      builder.AppendLine();
      builder.AppendLine("GO");
      builder.AppendLine();
      builder.Append("CREATE PROCEDURE dbo.");
      builder.AppendLine(name);
      return builder.ToString();
    }

    private string GetDataType(Field field)
    {
      if (field.DBType.ToLower() == "varchar" || field.DBType.ToLower() == "nvarchar")
      {
        return field.DBType.ToLower() + "(" + (field.Size < 0 ? "MAX" : field.Size.ToString()) + ")";
      }
      else
      {
        return field.DBType;
      }
    
    }

    private void DeleteComma(StringBuilder builder)
    {
      //builder.Remove(builder.Length - 3, 3);  

      for (int i = builder.Length-1; i >= 0; i--)
      {
        if (builder[i] == ',')
        {
          builder.Remove(i, builder.Length - i);
          break;
        }
      }
    }

    public string GenerateSelectQuery()
    {
      StringBuilder builder = new StringBuilder();
      builder.Append("SET NOCOUNT OFF; ");
      builder.Append("SELECT");


      foreach (Field field in _fields)
      {
        builder.Append(" [" + field.FieldName + "],");
      }
      DeleteComma(builder);

      builder.Append(" FROM [dbo].[" + _tableName + "]");
      builder.Append(" WHERE (");

      // WHERE clause
      foreach (Field field in _fields)
      {
        if (field.IsPrimaryKey)
        {
          builder.Append("[" + field.FieldName + "] = @" + field.FieldName + " AND ");
        }
      }
      builder.Remove(builder.Length - 5, 5);  // remove AND
      builder.Append(");");
      return builder.ToString();
    }

    public string GenerateInsertQuery()
    {
      StringBuilder builder = new StringBuilder();
      builder.Append("SET NOCOUNT OFF; ");
      builder.Append("INSERT INTO [dbo].[" + _tableName + "] (");
      foreach (Field field in _fields)
      {
        if (!field.IsAutoInc) builder.Append("    [" + field.FieldName + "],");
      }
      DeleteComma(builder);
      builder.Append(")");
      builder.Append(" VALUES (");

      foreach (Field field in _fields)
      {
        if (!field.IsAutoInc)
        {
          builder.Append(" @" + field.FieldName + ",");
        }
      }
      DeleteComma(builder);
      builder.Append("); ");

      builder.Append("SET @Identity = SCOPE_IDENTITY();");

      return builder.ToString();

    }

    public string GenerateUpdateQuery()
    {
      StringBuilder builder = new StringBuilder();
      builder.Append("SET NOCOUNT OFF; ");
      builder.Append("UPDATE [dbo].[" + _tableName + "] SET ");

      // Values
      foreach (Field field in _fields)
      {
        if (!field.IsPrimaryKey && !field.IsAutoInc)
        {
          //if (field.FieldName != _dateModifiedFieldName && field.FieldName != _dateCreatedFieldName && field.FieldName != _creatorIDFieldName)
          if (field.FieldName != _dateCreatedFieldName && field.FieldName != _creatorIDFieldName)
          {
            builder.Append("    [" + field.FieldName + "] = @" + field.FieldName + ",");
          }
          //else if (field.FieldName == _dateModifiedFieldName)
          // {
          //  builder.AppendLine("    [" + field.FieldName + "] = GetDate(),");
          // }
        }
      }
      DeleteComma(builder);

      builder.Append("  WHERE (");

      // WHERE clause
      foreach (Field field in _fields)
      {
        if (field.IsPrimaryKey)
        {
          builder.Append("[" + field.FieldName + "] = @" + field.FieldName + " AND ");
        }
      }
      builder.Remove(builder.Length - 5, 5);  // remove AND
      builder.Append(");");
      return builder.ToString();
    }

    public string GenerateDeleteQuery()
    {
      StringBuilder builder = new StringBuilder();
      builder.Append("SET NOCOUNT OFF; ");
      builder.Append(" DELETE FROM [dbo].[" + _tableName + "]");
      builder.Append(" WHERE (");

      // WHERE clause
      foreach (Field field in _fields)
      {
        if (field.IsPrimaryKey)
        {
          builder.Append("[" + field.FieldName + "] = @" + field.FieldName + " AND ");
        }
      }
      builder.Remove(builder.Length - 5, 5);  // remove AND
      builder.Append(");");
      return builder.ToString();
    }


    private string GenerateSelect()
    {
      StringBuilder builder = new StringBuilder();
      string name = "uspGeneratedSelect" + _itemName;
      builder.AppendLine(GetHeader(name));
      builder.AppendLine("(");

      //Parameters
      foreach (Field field in _fields)
      {
        if (field.IsPrimaryKey)
        {
          builder.Append("  @" + field.FieldName);
          builder.Append(" " + GetDataType(field));
          builder.AppendLine(",");
        }
      }
      DeleteComma(builder);

      builder.AppendLine();
      builder.AppendLine(")");
      builder.AppendLine("AS");
      builder.AppendLine("  SET NOCOUNT OFF;");
      builder.AppendLine("  SELECT");

      
      foreach (Field field in _fields)
      {
         builder.AppendLine("    [" + field.FieldName + "],");
      }
      DeleteComma(builder);

      builder.AppendLine();
      builder.AppendLine("  FROM [dbo].[" + _tableName + "]");
      builder.Append("  WHERE (");

      // WHERE clause
      foreach (Field field in _fields)
      {
        if (field.IsPrimaryKey)
        {
          builder.Append("[" + field.FieldName + "] = @" + field.FieldName + " AND ");
        }
      }
      builder.Remove(builder.Length - 5, 5);  // remove AND
      builder.AppendLine(")");
      builder.AppendLine("GO");
      builder.AppendLine();
      return builder.ToString();
    }

    private string GenerateInsert()
    {
      StringBuilder builder = new StringBuilder();
      string name = "uspGeneratedInsert" + _itemName;
      builder.AppendLine(GetHeader(name));
      builder.AppendLine("(");

      //Parameters
      foreach (Field field in _fields)
      {
        //if (!field.IsAutoInc && field.FieldName != _dateModifiedFieldName && field.FieldName != _dateCreatedFieldName)
        if (!field.IsAutoInc)
        {
          builder.Append("  @" + field.FieldName);
          builder.Append(" " + GetDataType(field));
          builder.AppendLine(",");
        }
      }

      builder.AppendLine("  @Identity int OUT");
      builder.AppendLine(")");
      builder.AppendLine("AS");
      builder.AppendLine("  SET NOCOUNT OFF;");
      builder.AppendLine("  INSERT INTO [dbo].[" + _tableName + "]");
      builder.AppendLine("  (");

      // Values
      foreach (Field field in _fields)
      {
        if (!field.IsAutoInc) builder.AppendLine("    [" + field.FieldName + "],");
      }
      DeleteComma(builder);
      builder.AppendLine(")");
      builder.AppendLine("  VALUES (");

      foreach (Field field in _fields)
      {
        //if (!field.IsAutoInc && field.FieldName != _dateModifiedFieldName && field.FieldName != _dateCreatedFieldName)
        if (!field.IsAutoInc)
        {
          builder.AppendLine("    @" + field.FieldName + ",");
        }
        //else if (!field.IsAutoInc)
        //{
        //  builder.AppendLine("    GetDate(),");
        //}
      }
      DeleteComma(builder);
      builder.AppendLine(")");

      builder.AppendLine();
      builder.AppendLine("SET @Identity = SCOPE_IDENTITY()");
      builder.AppendLine("GO");
      builder.AppendLine();

      return builder.ToString();

    }

    private string GenerateUpdate()
    {
      StringBuilder builder = new StringBuilder();
      string name = "uspGeneratedUpdate" + _itemName;
      builder.AppendLine(GetHeader(name));
      builder.AppendLine("(");
      
      //Parameters
      foreach (Field field in _fields)
      {
        //if (field.FieldName != _dateModifiedFieldName && field.FieldName != _dateCreatedFieldName && field.FieldName != _creatorIDFieldName)
        if (field.FieldName != _dateCreatedFieldName && field.FieldName != _creatorIDFieldName)
        {
          builder.Append("  @" + field.FieldName);
          builder.Append(" " + GetDataType(field));
          builder.AppendLine(",");
        }
      }
      DeleteComma(builder);

      builder.AppendLine();
      builder.AppendLine(")");
      builder.AppendLine("AS");
      builder.AppendLine("  SET NOCOUNT OFF;");
      builder.AppendLine("  UPDATE [dbo].[" + _tableName + "]");
      builder.AppendLine("  SET");

      // Values
      foreach (Field field in _fields)
      {
        if (!field.IsPrimaryKey && !field.IsAutoInc)
        {
          //if (field.FieldName != _dateModifiedFieldName && field.FieldName != _dateCreatedFieldName && field.FieldName != _creatorIDFieldName)
          if (field.FieldName != _dateCreatedFieldName && field.FieldName != _creatorIDFieldName)
          {
            builder.AppendLine("    [" + field.FieldName + "] = @" + field.FieldName + ",");
          }
          //else if (field.FieldName == _dateModifiedFieldName)
         // {
          //  builder.AppendLine("    [" + field.FieldName + "] = GetDate(),");
         // }
        }
      }
      DeleteComma(builder);

      builder.AppendLine();
      builder.Append("  WHERE (");

      // WHERE clause
      foreach (Field field in _fields)
      {
        if (field.IsPrimaryKey)
        {
          builder.Append("[" + field.FieldName + "] = @" + field.FieldName + " AND ");
        }
      }
      builder.Remove(builder.Length - 5, 5);  // remove AND
      builder.AppendLine(")");
      builder.AppendLine("GO");
      builder.AppendLine();
      return builder.ToString();
    }

    private string GenerateDelete()
    {
      StringBuilder builder = new StringBuilder();
      string name = "uspGeneratedDelete" + _itemName;
      builder.AppendLine(GetHeader(name));
      builder.AppendLine("(");

      //Parameters
      foreach (Field field in _fields)
      {
        if (field.IsPrimaryKey)
        {
          builder.Append("  @" + field.FieldName);
          builder.Append(" " + GetDataType(field));
          builder.AppendLine(",");
        }
      }
      DeleteComma(builder);

      builder.AppendLine();
      builder.AppendLine(")");
      builder.AppendLine("AS");
      builder.AppendLine("  SET NOCOUNT OFF;");
      builder.AppendLine("  DELETE FROM [dbo].[" + _tableName + "]");
      builder.Append("  WHERE (");

      // WHERE clause
      foreach (Field field in _fields)
      {
        if (field.IsPrimaryKey)
        {
          builder.Append("[" + field.FieldName + "] = @" + field.FieldName + " AND ");
        }
      }
      builder.Remove(builder.Length - 5, 5);  // remove AND
      builder.AppendLine(")");
      builder.AppendLine("GO");
      builder.AppendLine();
      return builder.ToString();
    }
    
    public string Generate()
    {
      _builder.Append(GenerateSelect());
      if (!_isReadOnly)
      {
        _builder.Append(GenerateInsert());
        _builder.Append(GenerateUpdate());
        _builder.Append(GenerateDelete());
      }
      
      return  _builder.ToString();
    }

  }
}
