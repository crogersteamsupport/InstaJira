using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamSupport.EFData.Models
{
    [Table("Tickets")]
    public class Tickets
    {
        [Key]
        public Int32 TicketID { get; set; }

        public Int32? ReportedVersionID { get; set; }

        public Int32? SolvedVersionID { get; set; }
        
        public Int32? ProductID { get; set; }

        public Int32? GroupID { get; set; }

        public Int32? UserID { get; set; }

        [Required]
        public Int32 TicketStatusID { get; set; }

        [Required]
        public Int32 TicketTypeID { get; set; }

        [Required]
        public Int32 TicketSeverityID { get; set; }

        [Required]
        public Int32 OrganizationID { get; set; }

        [MaxLength(255)]
        public String Name { get; set; }

        public Int32? ParentID { get; set; }

        [Required]
        public Int32 TicketNumber { get; set; }

        [Required]
        public Boolean IsVisibleOnPortal { get; set; }

        [Required]
        public Boolean IsKnowledgeBase { get; set; }

        public DateTime? DateClosed { get; set; }

        public Int32? CloserID { get; set; }

        [MaxLength(50)]
        public String ImportID { get; set; }

        public DateTime? LastViolationTime { get; set; }

        public DateTime? LastWarningTime { get; set; }

        [MaxLength(50)]
        public String TicketSource { get; set; }

        [MaxLength(500)]
        public String PortalEmail { get; set; }

        public DateTime? SlaViolationTimeClosed { get; set; }
                       
        public DateTime? SlaViolationLastAction { get; set; }

        public DateTime? SlaViolationInitialResponse { get; set; }

        public DateTime? SlaWarningTimeClosed { get; set; }

        public DateTime? SlaWarningLastAction { get; set; }

        public DateTime? SlaWarningInitialResponse { get; set; }

        [Required]
        public Boolean NeedsIndexing { get; set; }

        public Int32? DocID { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public DateTime DateModified { get; set; }

        [Required]
        public Int32 CreatorID { get; set; }

        [Required]
        public Int32 ModifierID { get; set; }

        public DateTime? DueDate { get; set; }

        public Int32? KnowledgeBaseCategoryID { get; set; }

        public DateTime? DateModifiedBySalesForceSync { get; set; }

        public String SalesForceID { get; set; }

        public String JiraStatus { get; set; }

        public DateTime? DateModifiedByJiraSync { get; set; }

        public Boolean? SyncWithJira { get; set; }

        public Int32? JiraID { get; set; }

        public String JiraKey { get; set; }

        public String JiraLinkURL { get; set; }

        [MaxLength(500)]
        public String EmailReplyToAddress { get; set; }

        public Int32? ImportFileID { get; set; }

    }
}
