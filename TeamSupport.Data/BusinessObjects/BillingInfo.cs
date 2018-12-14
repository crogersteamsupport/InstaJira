using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class BillingInfoItem 
  {
  }

  public partial class BillingInfo 
  {
    public static BillingInfoItem GetOrganizationBillingInfo(LoginUser loginUser, int organizationID)
    {
      BillingInfoItem item = (BillingInfoItem) GetBillingInfoItem(loginUser, organizationID);
      if (item == null)
      {
        BillingInfo info = new BillingInfo(loginUser);
        item = info.AddNewBillingInfoItem();
        item.NextInvoiceDate = DateTime.UtcNow.AddMonths(1);
        item.OrganizationID = loginUser.OrganizationID;
        item.AddressID = null;
        item.CreditCardID = null;
        item.IsAutomatic = true;
        item.StoragePrice = null;
        item.PortalPrice = null;
        item.UserPrice = null;
        info.Save();
      }

      return item;
    
    
    }
  }
}
