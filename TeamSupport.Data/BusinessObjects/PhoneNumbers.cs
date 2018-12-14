using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class PhoneNumber 
  {
    
    public string PhoneTypeName
    {
      get 
      { 
        if (Row.Table.Columns.Contains("PhoneTypeName") && Row["PhoneTypeName"] != DBNull.Value)
        {
          return (string)Row["PhoneTypeName"]; 
        }
        else
	      {
          return "Other";
	      }
              
      }
    }

    public string FormattedNumber
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

  public partial class PhoneNumbers
  {
    public void LoadByID(int refID, ReferenceType referenceType)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = " SELECT pn.*, pt.Name as PhoneTypeName FROM PhoneNumbers pn LEFT JOIN PhoneTypes pt ON pn.PhoneTypeID = pt.PhoneTypeID" +
                              " WHERE (RefID = @RefID) AND (RefType = @RefType)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefID", refID);
        command.Parameters.AddWithValue("@RefType", referenceType);
        Fill(command, "PhoneNumbers,PhoneTypes");
      }
    }
    
    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT pn.* FROM PhoneNumbers pn
                                LEFT JOIN Users u ON pn.RefID = u.UserID AND pn.RefType = 22
                                LEFT JOIN Organizations uo ON u.OrganizationID = uo.OrganizationID
                                LEFT JOIN Organizations o On pn.RefID = o.OrganizationID AND pn.RefType = 9
                                WHERE uo.ParentID = @OrganizationID OR o.ParentID = @OrganizationID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command, "PhoneNumbers,PhoneTypes,Users,Organizations");
      }
    }

    public void LoadByMyOrganization(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT pn.* FROM PhoneNumbers pn
                                LEFT JOIN Organizations o On pn.RefID = o.OrganizationID AND pn.RefType = 9
                                WHERE o.OrganizationID = @OrganizationID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command, "PhoneNumbers,PhoneTypes,Users,Organizations");
      }
    }

    public void ReplacePhoneType(int oldID, int newID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE PhoneNumbers SET PhoneTypeID = @newID WHERE (PhoneTypeID = @oldID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@oldID", oldID);
        command.Parameters.AddWithValue("@newID", newID);
        ExecuteNonQuery(command, "PhoneNumbers");
      }
    }


    public PhoneNumber FindByPhoneTypeID(int typeID)
    {
        foreach (PhoneNumber number in this)
        {
            if (number.PhoneTypeID == typeID)
            {
                
                return number;
            }
        }
        return null;
    }

  }

  public class CustomerSearchPhone
  {
    public CustomerSearchPhone() { }
    public CustomerSearchPhone(PhoneNumber number)
    {
      this.type = number.PhoneTypeName;
      this.number = number.Number;
      this.ext = number.Extension;
    }
    public string type { get; set; }
    public string number { get; set; }
    public string ext { get; set; }
  }
}
