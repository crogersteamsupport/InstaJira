using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamSupport.Data
{
  public class FieldMapItem
  {
    private string _publicName;
    public string PublicName
    {
      get { return _publicName; }
    }
    private string _privateName;
    public string PrivateName
    {
      get { return _privateName; }
    }
    private bool _update;
    public bool Update
    {
      get { return _update; }
    }
    private bool _insert;
    public bool Insert
    {
      get { return _insert; }
    }
    private bool _select;
    public bool Select
    {
      get { return _select; }
    }

    public FieldMapItem(string privateName, string publicName, bool update, bool insert, bool select)
    {
      _publicName = publicName;
      _privateName = privateName;
      _update = update;
      _insert = insert;
      _select = select;
    }

  }


  public class FieldMap : IEnumerable<FieldMapItem>
  {
    private List<FieldMapItem> _items;

    public List<FieldMapItem> Items
    {
      get { return _items; }
      set { _items = value; }
    }


    public FieldMap()
    {
      _items = new List<FieldMapItem>();
    }

    public void AddMap(string privateName, string publicField, bool update, bool insert, bool select)
    {
      _items.Add(new FieldMapItem(privateName, publicField, update, insert, select));
    }

    public string GetPublicName(string privateName)
    {
      privateName = privateName.ToLower();
      foreach (FieldMapItem item in _items)
      {
        if (item.PrivateName.ToLower() == privateName)
        {
          return item.PublicName;
        }
      }
      return "";
    }

    public string GetPrivateField(string publicName)
    {
      publicName = publicName.ToLower();
      foreach (FieldMapItem item in _items)
      {
        if (item.PublicName.ToLower() == publicName)
        {
          return item.PrivateName;
        }
      }
      return "";
    }



    public FieldMapItem this[int index]
    {
      get { return _items[index]; }
    }

    #region IEnumerable<FieldMapItem> Members

    public IEnumerator<FieldMapItem> GetEnumerator()
    {
      foreach (FieldMapItem item in _items)
      {
        yield return item;
      }
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion
  }
}
