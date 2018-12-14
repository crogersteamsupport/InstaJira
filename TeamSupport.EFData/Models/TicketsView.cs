using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.EFData.Models
{

    [Table("TicketsView")]
    public class TicketsView
    {
        [Key]
        public Int32 TicketID { get; set; }

        [MaxLength(255)]
        public String ProductName { get; set; }

        [MaxLength(50)]
        public String ReportedVersion { get; set; }

        [MaxLength(50)]
        public String SolvedVersion { get; set; }

        [MaxLength(255)]
        public String GroupName { get; set; }

        [MaxLength(255)]
        public String TicketTypeName { get; set; }

        [MaxLength(201)]
        public String UserName { get; set; }

        [MaxLength(255)]
        public String Status { get; set; }

        public Int32? StatusPosition { get; set; }

        public Int32? SeverityPosition { get; set; }

        [Required]
        public Boolean IsClosed { get; set; }

        [MaxLength(255)]
        public String Severity { get; set; }

        [Required]
        public Int32 TicketNumber { get; set; }

        [Required]
        public Boolean IsVisibleOnPortal { get; set; }

        [Required]
        public Boolean IsKnowledgeBase { get; set; }

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
        public Int32 ModifierID { get; set; }

        [Required]
        public Int32 CreatorID { get; set; }

        [Required]
        public DateTime DateModified { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public DateTime? DateClosed { get; set; }

        public Int32? CloserID { get; set; }

        [Required]
        public Int32 DaysClosed { get; set; }

        public Int32? DaysOpened { get; set; }

        [MaxLength(201)]
        public String CloserName { get; set; }

        [MaxLength(201)]
        public String CreatorName { get; set; }

        [MaxLength(201)]
        public String ModifierName { get; set; }

        public Decimal? HoursSpent { get; set; }

        [MaxLength(8000)]
        public String Tags { get; set; }

        public Int32? SlaViolationTime { get; set; }

        public Int32? SlaWarningTime { get; set; }

        public Decimal? SlaViolationHours { get; set; }

        public Decimal? SlaWarningHours { get; set; }

        public Int32? MinsSinceCreated { get; set; }

        public Int32? DaysSinceCreated { get; set; }

        public Int32? MinsSinceModified { get; set; }

        public Int32? DaysSinceModified { get; set; }

        [MaxLength(8000)]
        public String Contacts { get; set; }

        [MaxLength(8000)]
        public String Customers { get; set; }

        public DateTime? SlaViolationTimeClosed { get; set; }

        public DateTime? SlaViolationLastAction { get; set; }

        public DateTime? SlaViolationInitialResponse { get; set; }

        public DateTime? SlaWarningTimeClosed { get; set; }

        public DateTime? SlaWarningLastAction { get; set; }

        public DateTime? SlaWarningInitialResponse { get; set; }

        [Required]
        public Boolean NeedsIndexing { get; set; }

        public DateTime? SlaViolationDate { get; set; }

        public DateTime? SlaWarningDate { get; set; }

        [Required]
        [MaxLength(50)]
        public String TicketSource { get; set; }

        public Int32? ForumCategory { get; set; }

        [MaxLength(100)]
        public String CategoryName { get; set; }

        [MaxLength(1024)]
        public String CreatorEmail { get; set; }

        [MaxLength(1024)]
        public String ModifierEmail { get; set; }

        public Int32? KnowledgeBaseCategoryID { get; set; }

        [MaxLength(100)]
        public String KnowledgeBaseCategoryName { get; set; }

        public DateTime? DueDate { get; set; }

        public String SalesForceID { get; set; }

        public DateTime? DateModifiedBySalesForceSync { get; set; }

        public DateTime? DateModifiedByJiraSync { get; set; }

        public Int32? JiraID { get; set; }

        public Boolean? SyncWithJira { get; set; }

        [MaxLength(8000)]
        public String JiraKey { get; set; }

        [MaxLength(8000)]
        public String JiraLinkURL { get; set; }

        [MaxLength(8000)]
        public String JiraStatus { get; set; }

        [MaxLength(500)]
        public String EmailReplyToAddress { get; set; }

        public Int32? ProductFamilyID { get; set; }

        public String ProductFamily { get; set; }

        public Boolean? IsSlaPaused { get; set; }


    }
}
