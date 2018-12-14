using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
 
  public partial class TagLink : BaseItem
  {
    public TagLinkProxy GetProxy()
    {
      TagLinkProxy result = new TagLinkProxy();
      result.CreatorID = this.CreatorID;
      result.RefID = this.RefID;
      result.RefType = this.RefType;
      result.TagID = this.TagID;
      result.TagLinkID = this.TagLinkID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
