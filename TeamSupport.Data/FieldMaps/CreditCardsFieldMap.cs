using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class CreditCards
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CreditCardID", "CreditCardID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("DisplayNumber", "DisplayNumber", false, false, false);
      _fieldMap.AddMap("CreditCardType", "CreditCardType", false, false, false);
      _fieldMap.AddMap("CardNumber", "CardNumber", false, false, false);
      _fieldMap.AddMap("SecurityCode", "SecurityCode", false, false, false);
      _fieldMap.AddMap("ExpirationDate", "ExpirationDate", false, false, false);
      _fieldMap.AddMap("NameOnCard", "NameOnCard", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModfied", "DateModfied", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
            
    }
  }
  
}
