using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.EFData.Models
{
    [Table("TicketTypes")]
    public class TicketTypes
    {
        [Key]
        public int TicketTypeID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Position { get; set; }
        [Required]
        public int OrganizationID { get; set; }
        [Required]
        public string IconUrl { get; set; }
        [Required]
        public bool IsVisibleOnPortal { get; set; }//wrong name
        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        public DateTime DateModified { get; set; }
        [Required]
        public int CreatorID { get; set; }
        [Required]
        public int ModifierID{ get; set; }
        public int? ProductFamilyID { get; set; }//wrong name
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool ExcludeFromCDI { get; set; }
    }
}
