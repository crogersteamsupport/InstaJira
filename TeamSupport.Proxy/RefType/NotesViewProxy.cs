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
  [KnownType(typeof(NotesViewItemProxy))]
  public class NotesViewItemProxy
  {
    public NotesViewItemProxy() {}
    [DataMember] public int NoteID { get; set; }
    [DataMember] public int RefType { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public string Title { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public bool NeedsIndexing { get; set; }
    [DataMember] public string CreatorName { get; set; }
    [DataMember] public string ModifierName { get; set; }
    [DataMember] public int? ParentOrganizationID { get; set; }
    [DataMember] public string OrganizationName { get; set; }
    [DataMember] public string ContactName { get; set; }
          
  }
  
}
