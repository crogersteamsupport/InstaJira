using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace WatsonToneAnalyzer
{
    /// <summary>
    /// Linq class to integrate with ActionSentiments Table
    /// </summary>
    [Table(Name = "ActionSentiments")]
    class ActionSentiment
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _actionSentimentID;
        [Column(Storage= "_actionSentimentID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ActionSentimentID {  get { return _actionSentimentID; } }

        [Column]
        public int ActionID;
        [Column]
        public int TicketID;
        [Column]
        public int UserID;
        [Column]
        public int OrganizationID;
        [Column]
        public bool IsAgent;
        [Column]
        public DateTime DateCreated;
#pragma warning restore CS0649
    }

}
