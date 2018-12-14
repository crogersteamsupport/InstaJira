using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamSupport.EFData.Models
{
    [Table("TicketLinkToJira")]
    public class TicketLinkToJira
    {
        [Key]
        public int id { get; set; }
        public int? TicketID { get; set; }
        public DateTime? DateModifiedByJiraSync { get; set; }
        public bool? SyncWithJira { get; set; }
        public int? JiraID { get; set; }
        public string JiraKey { get; set; }
        public string JiraLinkURL { get; set; }
        public string JiraStatus { get; set; }
        public int? CreatorID { get; set; }
        public int? CrmLinkID { get; set; }
    }
}
