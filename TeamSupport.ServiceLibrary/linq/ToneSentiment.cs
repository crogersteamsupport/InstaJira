using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using System.Data.Linq;

namespace WatsonToneAnalyzer
{
    [Table(Name = "ToneSentiments")]
    class ToneSentiment
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _sentimentID;
        [Column(Storage = "_sentimentID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int SentimentID { get { return _sentimentID; } }

        [Column]
        public string SentimentName;
        [Column]
        public decimal? SentimentMultiplier;
#pragma warning restore CS0649

        public static ToneSentiment[] ToneSentiments { get; private set; }
        public static void Initialize(DataContext db)
        {
            Table<ToneSentiment> tones = db.GetTable<ToneSentiment>();
            ToneSentiments = (from tone in tones select tone).ToArray();
        }

    }
}
