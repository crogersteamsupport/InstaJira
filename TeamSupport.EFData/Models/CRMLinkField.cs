using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.EFData.Models
{
    [Table("CRMLinkFields")]
    public class CRMLinkField
    {
        [Key]
        public int CRMFieldID { get; set; }
        [Required]
        public int CRMLinkID { get; set; }
        public string CRMObjectName { get; set; }
        [Required]
        public string CRMFieldName { get; set; }
        public int? CustomFieldID { get; set; }
        public string TSFieldName { get; set; }
    }
}
