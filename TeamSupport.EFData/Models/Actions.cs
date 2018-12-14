using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamSupport.EFData.Models
{
    [Table("Actions")]
    public class Actions
    {
        [Key]
        public Int32 ActionID { get; set; }

        public Int32? ActionTypeID { get; set; }

        [Required]
        public Int32 SystemActionTypeID { get; set; }

        [Required]
        [MaxLength(1000)]
        public String Name { get; set; }

        public Int32? TimeSpent { get; set; }

        public DateTime? DateStarted { get; set; }

        [Required]
        public Boolean IsVisibleOnPortal { get; set; }

        [Required]
        public Boolean IsKnowledgeBase { get; set; }

        [MaxLength(50)]
        public String ImportID { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public DateTime DateModified { get; set; }

        [Required]
        public Int32 CreatorID { get; set; }

        [Required]
        public Int32 ModifierID { get; set; }

        [Required]
        public Int32 TicketID { get; set; }

        [MaxLength(50)]
        public String ActionSource { get; set; }

        public DateTime? DateModifiedBySalesForceSync { get; set; }

        [MaxLength(100)]
        public String SalesForceID { get; set; }

        public DateTime? DateModifiedByJiraSync { get; set; }

        public Int32? JiraID { get; set; }

        [Required]
        public Boolean Pinned { get; set; }

        public String Description { get; set; }

        [Required]
        public Boolean IsClean { get; set; }

        public Int32? ImportFileID { get; set; }

    }
}
