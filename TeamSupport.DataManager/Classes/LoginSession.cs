using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;

namespace TeamSupport.DataManager
{
  public class NamObjectItem
  {
    private string _text;
    public string Text
    {
      get { return _text; }
      set { _text = value; }
    }

    private object _value;
    public object Value
    {
      get { return _value; }
      set { _value = value; }
    }

    public NamObjectItem(string text, object value)
    {
      _value = value;
      _text = text;
    }

    public override string ToString()
    {
      return _text;
    }


  }

  public class LoginSession
  {
    private static LoginUser _loginUser;

    public static LoginUser LoginUser
    {
      get { return _loginUser; }
      set { _loginUser = value; }
    }
  
  }
}
