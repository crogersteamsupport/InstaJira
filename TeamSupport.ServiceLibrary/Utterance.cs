using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Text.RegularExpressions;

namespace WatsonToneAnalyzer
{
    public enum User
    {
        customer,
        agent
    }

    public class Utterance
    {
        public string user;  // do not rename - must match IBM spec "user"
        public string text;  // do not rename - must match IBM spec "text"

        public Utterance(bool isAgent, string inputText)
        {
            user = (isAgent ? User.agent : User.customer).ToString();
            text = inputText;
            if (inputText.Length > MaxUtteranceLength)
                throw new Exception(String.Format("utterance text overflow: {0}", inputText));
        }

        static int MaxUtteranceLength = Int32.Parse(ConfigurationManager.AppSettings.Get("MaxUtteranceLength"));

        /// <summary> Pack Sentence to Utterances </summary>
        public static void PackLongSentenceToUtterances(string sentence, StringBuilder utteranceBuilder, bool isAgent, List<Utterance> results)
        {
            // sentence length overflow
            int offset = 0;
            while (sentence.Length - offset > MaxUtteranceLength)
            {
                int appendLength = MaxUtteranceLength - utteranceBuilder.Length;
                utteranceBuilder.Append(sentence.Substring(offset, appendLength));
                results.Add(new Utterance(isAgent, utteranceBuilder.ToString()));
                utteranceBuilder.Clear();
                offset += appendLength;
            }
            utteranceBuilder.Append(sentence.Substring(offset));
        }

    }


}
