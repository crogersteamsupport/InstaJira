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
    /// Linq class to integrate with ActionSentimentScores Table
    /// </summary>
    [Table(Name = "ActionSentimentScores")]
    class ActionSentimentScore
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _actionSentimentScoreID;
        [Column(Storage = "_actionSentimentScoreID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ActionSentimentScoreID { get { return _actionSentimentScoreID; } }

        [Column]
        public int ActionSentimentID;
        [Column]
        public int SentimentID;
        [Column]
        public decimal SentimentScore;
#pragma warning restore CS0649
    }
}
