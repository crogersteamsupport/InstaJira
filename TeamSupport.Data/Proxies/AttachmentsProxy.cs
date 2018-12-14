using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
 
  partial class Attachment : BaseItem
  {
    public AttachmentProxy GetProxy()
    {
      AttachmentProxy result = AttachmentProxy.ClassFactory(this.RefType);
      result.FilePathID = this.FilePathID;
      result.SentToSnow = this.SentToSnow;
      result.SentToTFS = this.SentToTFS;
      result.ProductFamilyID = this.ProductFamilyID;
      result.SentToJira = this.SentToJira;
      result.RefID = this.RefID;
      //result.RefType = this.RefType;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.Description = this.Description;
      result.Path = this.Path;
      result.FileSize = this.FileSize;
      result.FileType = this.FileType;
      result.FileName = this.FileName;
      result.OrganizationID = this.OrganizationID;
      result.AttachmentID = this.AttachmentID;
      result.CreatorName = this.CreatorName;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
