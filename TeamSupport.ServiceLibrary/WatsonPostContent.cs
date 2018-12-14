using System.Collections.Generic;
using Newtonsoft.Json;

namespace WatsonToneAnalyzer
{
    /// <summary>
    /// Convert the action text and users into utterances for the post to watson
    /// </summary>
    class WatsonPostContent
    {
        public List<Utterance> utterances;  // do not rename - must match IBM spec "utterances"

        /// <summary>default constructor</summary>
        public WatsonPostContent() { utterances = new List<Utterance>(); }

        /// <summary>serialize to JSON - escapes \b, \f, \n, \r, \t, ", and \
        public string ToJSON() { return JsonConvert.SerializeObject(this); }

        public void Add(Utterance addition) { utterances.Add(addition); }
        public void Add(List<Utterance> additions) { utterances.AddRange(additions); }
    }
}
