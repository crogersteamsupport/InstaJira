using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.EFData.Models
{
    [Table("ExceptionLogs")]
    public class ExceptionLogs : IAuditModel
    {
        [Key]
        public int ExceptionLogID {get;set;}
        public string URL { get; set; }
        public string PageInfo { get; set; }
        public string ExceptionName { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        [Required]
        public int CreatorID { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        public int ModifierID { get; set; }
        [Required]
        public DateTime DateModified { get; set; }
    }
}
