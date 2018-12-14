using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessObjectGenerator
{
  public class Field
  {
    private string _propertyName;
    public string PropertyName
    {
      get { return _propertyName == "" ? _fieldName : _propertyName; }
      set { _propertyName = value; }
    }

    private string _fieldName;
    public string FieldName
    {
      get { return _fieldName; }
      set { _fieldName = value; }
    }

    private string _sqlDbType;
    public string SqlDbType
    {
      get { return _sqlDbType; }
      set { _sqlDbType = value; }
    }

    private string _dbType;
    public string DBType
    {
      get { return _dbType; }
      set { _dbType = value; }
    }

    private string _customType;
    public string CustomType
    {
      get { return _customType; }
      set { _customType = value; }
    }

    private string _systemType;
    public string SystemType
    {
      get { return _systemType; }
      set { _systemType = value; }
    }

    private int _size;
    public int Size
    {
      get { return _size; }
      set { _size = value; }
    }

    private int _precision;
    public int Precision
    {
      get { return _precision; }
      set { _precision = value; }
    }

    private int _scale;
    public int Scale
    {
      get { return _scale; }
      set { _scale = value; }
    }

    private bool _isAutoInc;
    public bool IsAutoInc
    {
      get { return _isAutoInc; }
      set { _isAutoInc = value; }
    }

    private bool _isPrimaryKey;
    public bool IsPrimaryKey
    {
      get { return _isPrimaryKey; }
      set { _isPrimaryKey = value; }
    }

    private bool _isIdentity;
    public bool IsIdentity
    {
      get { return _isIdentity; }
      set { _isIdentity = value; }
    }

    private bool _isReadOnly;
    public bool IsReadOnly
    {
      get { return _isReadOnly; }
      set { _isReadOnly = value; }
    }

    private bool _allowDBNull;
    public bool AllowDBNull
    {
      get { return _allowDBNull; }
      set { _allowDBNull = value; }
    }

    private bool _isNullable;
    public bool IsNullable
    {
      get { return _isNullable; }
      set { _isNullable = value; }
    }

  }
}
