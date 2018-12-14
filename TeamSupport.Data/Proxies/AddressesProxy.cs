using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  
  public partial class Address : BaseItem
  {
    public AddressProxy GetProxy()
    {
      AddressProxy result = new AddressProxy();
      result.ImportFileID = this.ImportFileID;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.Comment = this.Comment;
      result.Country = this.Country;
      result.Zip = this.Zip;
      result.State = this.State;
      result.City = this.City;
      result.Addr3 = this.Addr3;
      result.Addr2 = this.Addr2;
      result.Addr1 = this.Addr1;
      result.Description = this.Description;
      result.RefType = this.RefType;
      result.RefID = this.RefID;
      result.AddressID = this.AddressID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);

      string link = "http://maps.google.com/maps?q={0}";
      StringBuilder builder = new StringBuilder();
      if (!String.IsNullOrEmpty(this.Addr1))
      {
          builder.Append(this.Addr1.Replace(' ', '+'));
      }

      if (!String.IsNullOrEmpty(this.Addr2))
      {
          if (builder.Length > 0) builder.Append("+");
          builder.Append(this.Addr2.Replace(' ', '+'));
      }

      if (!String.IsNullOrEmpty(this.Addr3))
      {
          if (builder.Length > 0) builder.Append("+");
          builder.Append(this.Addr3.Replace(' ', '+'));
      }

      if (!String.IsNullOrEmpty(this.City))
      {
          if (builder.Length > 0) builder.Append(",");
          builder.Append(this.City.Replace(' ', '+'));
      }

      if (!String.IsNullOrEmpty(this.State))
      {
          if (builder.Length > 0) builder.Append(",");
          builder.Append(this.State.Replace(' ', '+'));
      }

      if (!String.IsNullOrEmpty(this.Country))
      {
          if (builder.Length > 0) builder.Append(",");
          builder.Append(this.Country.Replace(' ', '+'));
      }

      link = String.Format(link, builder.ToString());
      result.MapLink = link;
      return result;
    }	
  }
}
