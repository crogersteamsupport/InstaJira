using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Configuration;

namespace WatsonToneAnalyzer
{
    /// <summary>
    /// Linq class to integrate with ActionToAnalyze Table
    /// </summary>
    [Table(Name = "ActionToAnalyze")]
    class ActionToAnalyze
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        // note Int64 - different ID data type from other ID's
        private Int64 _actionToAnalyzeID;
        [Column(Storage = "_actionToAnalyzeID", IsPrimaryKey = true, IsDbGenerated = true)]
        public Int64 ActionToAnalyzeID { get { return _actionToAnalyzeID; } }

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
        [Column]
        public string ActionDescription;
#pragma warning restore CS0649

        string _watsonText = null;
        Dictionary<string, Tones> _tones = null;

        static Dictionary<string, int> _sentimentMultiplier;
        public static void Initialize(DataContext db)
        {
            // we need the multipliers from ToneSentiment (frustrated = -1, satisfied = +1...)
            ToneSentiment[] toneSentiments = ToneSentiment.ToneSentiments;
            _sentimentMultiplier = new Dictionary<string, int>();
            foreach (ToneSentiment toneSentiment in toneSentiments)
                _sentimentMultiplier[toneSentiment.SentimentName.Trim()] = Convert.ToInt32(toneSentiment.SentimentMultiplier);
        }

        bool ContainsNegativeTones()
        {
            return _tones.Where(t => _sentimentMultiplier[t.Value.tone_id] < 0).Any();
        }

        IEnumerable<Tones> NegativeTones()
        {
            return _tones.Where(t => _sentimentMultiplier[t.Value.tone_id] < 0).Select(pair => pair.Value);
        }

        IEnumerable<Tones> PositiveTones()
        {
            return _tones.Where(t => _sentimentMultiplier[t.Value.tone_id] > 0).Select(pair => pair.Value);
        }

        /// <summary> 
        /// Decide if the action is primarily positive or negative sentiment
        /// Return the list of only those tones
        /// </summary>
        public List<Tones> GetTones()
        {
            // no sentiment detected
            List<Tones> result = new List<Tones>();
            if ((_tones == null) || !_tones.Any())
                return result;

            // one sentiment
            if (_tones.Count == 1)
            {
                result.Add(_tones.First().Value);
                return result;
            }

            // negative sentiment?
            if (ContainsNegativeTones())
                return NegativeTones().ToList();

            return PositiveTones().ToList();
        }

        /// <summary>remove HTML, whitespace, email addresses...</summary>
        public string WatsonText()
        {
            if (_watsonText != null)
                return _watsonText;
            return _watsonText = CleanString(ActionDescription);
        }

        static int MaxActionTextLength = Int32.Parse(ConfigurationManager.AppSettings.Get("MaxActionTextLength"));

        public static string CleanString(string RawHtml)
        {
            // remove email addresses first (even if contains spaces)
            string text = Regex.Replace(RawHtml, @"\s+@", "@");
            Regex ItemRegex = new Regex(@"[\w\d._%+-]+@[ \w\d.-]+\.[\w]{2,3}");

            text = Regex.Replace(text, @"<[^>]*>", String.Empty); //remove html tags
            text = Regex.Replace(text, "&nbsp;", " "); //remove HTML space
            text = Regex.Replace(text, @"[\d-]", String.Empty); //removes all digits [0-9]
            text = Regex.Replace(text, @"\s+", " ");   // remove whitespace
            if (text.Length > MaxActionTextLength)
                text = text.Substring(0, MaxActionTextLength);
            return text;
        }

        /// <summary> Delete ActionToAnalyze </summary>
        public void DeleteOnSubmit(DataContext db)
        {
            Table<ActionToAnalyze> table = db.GetTable<ActionToAnalyze>();
            try
            {
                // linq classes have an attach state to the DB table row
                if (table.GetOriginalEntityState(this) == null)
                    table.Attach(this); // must be attached to delete
            }
            catch (Exception e2)
            {
                WatsonEventLog.WriteEntry("Exception with table.Attach - ", e2);
                Console.WriteLine(e2.ToString());
            }

            table.DeleteOnSubmit(this);
        }

        // MQ message
        public static ActionToAnalyze Factory(string message)
        {
            return JsonConvert.DeserializeObject<ActionToAnalyze>(message);
        }

        static int MaxUtteranceLength = Int32.Parse(ConfigurationManager.AppSettings.Get("MaxUtteranceLength"));

        /// <summary> Can we fit the action into a single utterance? </summary>
        public bool TryGetUtterance(out Utterance utterance)
        {
            string watsonText = WatsonText();
            if (watsonText.Length > MaxUtteranceLength)
            {
                utterance = null;
                return false;
            }

            utterance = new Utterance(IsAgent, watsonText);
            return true;
        }

        /// <summary> Pack the action into utterances </summary>
        public List<Utterance> PackActionToUtterances()
        {
            // forward to pure function
            return PackActionToUtterances(IsAgent, WatsonText());
        }

        /// <summary> Pack Action to Utterances </summary>
        public static List<Utterance> PackActionToUtterances(bool isAgent, string text)
        {
            if (text.Length <= MaxUtteranceLength)
                throw new Exception("Action would have fit into a single utterance");

            // pack sentences into utterances
            List<Utterance> results = new List<Utterance>();
            StringBuilder utteranceBuilder = new StringBuilder();
            string[] sentences = Regex.Split(text, @"(?<=[.?!,;:])");
            foreach (string sentence in sentences)
            {
                // sentence fits in utterance
                if (utteranceBuilder.Length + sentence.Length <= MaxUtteranceLength)
                {
                    utteranceBuilder.Append(sentence);
                    continue;
                }

                // sentence too long to fit into current utterance
                if (sentence.Length < MaxUtteranceLength)
                {
                    results.Add(new Utterance(isAgent, utteranceBuilder.ToString()));
                    utteranceBuilder.Clear();
                    utteranceBuilder.Append(sentence);  // start a new utterance
                    continue;
                }

                Utterance.PackLongSentenceToUtterances(sentence, utteranceBuilder, isAgent, results);
            }

            // create an utterance with the remainder
            results.Add(new Utterance(isAgent, utteranceBuilder.ToString()));

            // does collection of utterances add up to text?
            string test = String.Empty;
            foreach (Utterance utterance in results)
                test += utterance.text;
            if (String.CompareOrdinal(text, test) != 0)
                Debugger.Break();

            return results;
        }

        /// <summary> Accumulate all the sentiments for all the utterances </summary>
        public void AddSentiment(UtteranceResponse utterance)
        {
            if (!utterance.tones.Any())
                return;

            if (_tones == null)
                _tones = new Dictionary<string, Tones>();

            foreach (Tones tone in utterance.tones)
            {
                if (!_tones.ContainsKey(tone.tone_id) || (tone.score > _tones[tone.tone_id].score))
                    _tones[tone.tone_id] = tone;
            }
        }
    }
}
