using System;

namespace TeamSupport.EFData.Models
{
    public interface IAuditModel
    {
        int CreatorID { get; set; }
        DateTime DateCreated { get; set; }
        int ModifierID { get; set;}
        DateTime DateModified { get; set; }
    }
}