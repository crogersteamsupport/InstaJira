using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class PhoneNumbersViewItem
  {
    public string FormattedPhoneNumber
    {
      get
      {
        string phoneNumberString = (string)Row["PhoneNumber"];
        long phoneNumberLong;
        bool isNumber = long.TryParse(phoneNumberString, out phoneNumberLong);

        if (phoneNumberString.Length == 10 && isNumber)
        {
          return string.Format("{0:(###) ###-####}", phoneNumberLong);
        }
        else
        {
          return phoneNumberString;
        }
      }
      set { Row["PhoneNumber"] = CheckValue("PhoneNumber", value); }
    }
  }
  
  public partial class PhoneNumbersView
  {
    public void LoadByID(int refID, ReferenceType referenceType, string orderBy = "")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = " SELECT * FROM PhoneNumbersView WHERE (RefID = @RefID) AND (RefType = @RefType)";
        if (orderBy != string.Empty)
        {
          command.CommandText += " ORDER BY " + orderBy;
        }
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefID", refID);
        command.Parameters.AddWithValue("@RefType", referenceType);
        Fill(command, "PhoneNumbers,PhoneTypes");
      }
    }

  }
  
}
