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
  [KnownType(typeof(NoteProxy))]
  public class NoteProxy
  {
    public NoteProxy() {}
    [DataMember] public int NoteID { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public string Title { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public bool NeedsIndexing { get; set; }
    [DataMember] public string CreatorName { get; set; }
    [DataMember]
    public bool IsAlert { get; set; }
    [DataMember] public int? ImportFileID { get; set; }
    [DataMember] public int? ProductFamilyID { get; set; }
    [DataMember] public string ProductFamily { get; set; }
    [DataMember] public int ActivityType { get; set; }
    [DataMember] public DateTime? DateOccurred { get; set; }
    [DataMember] public string ActivityTypeString { get; set; }
    [DataMember] public AttachmentProxy[] Attachments { get; set; }

    [DataMember] public string Owner { get; set; }
  }
  
}
