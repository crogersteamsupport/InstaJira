using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TeamSupport.Data;

namespace TeamSupport.DataManager
{
  public class BaseOrganizationUserControl : UserControl
  {
    private int _organizationID;
    public int OrganizationID
    {
      get { return _organizationID; }
      set 
      { 
        _organizationID = value; 
        
        if (!DesignMode) LoadOrganization((Organization)Organizations.GetOrganization(LoginSession.LoginUser, _organizationID));
      }
    }
    
    protected virtual void LoadOrganization(Organization organization)
    {
      
    }

  }
  
  
}
