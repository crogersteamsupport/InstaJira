using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(AttachmentProxy))]
  public class AttachmentProxy
  {
    public AttachmentProxy() {}
    [DataMember] public int AttachmentID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string FileName { get; set; }
    [DataMember] public string FileType { get; set; }
    [DataMember] public long FileSize { get; set; }
    [DataMember] public string Path { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public int RefType { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public bool SentToJira { get; set; }
    [DataMember] public Guid AttachmentGUID { get; set; }
    [DataMember] public int? ProductFamilyID { get; set; }
    [DataMember] public bool SentToTFS { get; set; }
    [DataMember] public bool SentToSnow { get; set; }
    [DataMember] public int? FilePathID { get; set; }
          
  }
  
  public partial class Attachment : BaseItem
  {
    public AttachmentProxy GetProxy()
    {
      AttachmentProxy result = new AttachmentProxy();
      result.FilePathID = this.FilePathID;
      result.SentToSnow = this.SentToSnow;
      result.SentToTFS = this.SentToTFS;
      result.ProductFamilyID = this.ProductFamilyID;
      result.AttachmentGUID = this.AttachmentGUID;
      result.SentToJira = this.SentToJira;
      result.RefID = this.RefID;
      result.RefType = this.RefType;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.Description = this.Description;
      result.Path = this.Path;
      result.FileSize = this.FileSize;
      result.FileType = this.FileType;
      result.FileName = this.FileName;
      result.OrganizationID = this.OrganizationID;
      result.AttachmentID = this.AttachmentID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
