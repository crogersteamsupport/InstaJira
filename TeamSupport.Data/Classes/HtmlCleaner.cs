using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace TeamSupport.Data
{

  public class HtmlCleaner
  {
    public static string CleanHtml(string html)
    {
      return RemoveScript(RemoveStyles(RemoveHeader(RemoveComments(html))));
    }

    public static string RemoveScript(string html)
    {
      if (!String.IsNullOrEmpty(html))
      {
        //this is a copy of the client-side strip scripts filter.
        //html = Regex.Replace(html, "<(SCRIPT)([^>]*)/>", "", RegexOptions.IgnoreCase);
        //html = Regex.Replace(html, "<(SCRIPT)([^>]*)>[\\s\\S]*?</(SCRIPT)([^>]*)>", "", RegexOptions.IgnoreCase);
        //html = Regex.Replace(html, "<script[^>]*>[\\s\\S]*?</script>", string.Empty, RegexOptions.IgnoreCase);
        html = Regex.Replace(html, "(<SCRIPT\b[^>]*>(.*?)</SCRIPT>)|(<SCRIPT\b[^>]*/>)", string.Empty, RegexOptions.IgnoreCase);
        html = Regex.Replace(html, "(<BASE\b[^>]*>(.*?)</BASE>)|(<BASE\b[^>]*/>)", string.Empty, RegexOptions.IgnoreCase);
        
      }
      return html;
    }

    public static string RemoveHeader(string html)
    {
       return Regex.Replace(html, "<head[^>]*>[\\s\\S]*?</head>", string.Empty, RegexOptions.IgnoreCase);
    }

    public static string RemoveStyles(string html)
    {
      return Regex.Replace(html, "<style[^>]*>[\\s\\S]*?</style>", string.Empty, RegexOptions.IgnoreCase);
    }

    public static string RemoveComments(string html)
    {
      return Regex.Replace(html, "<!--[\\s\\S]*?-->", string.Empty, RegexOptions.IgnoreCase);
    }

    public static Dictionary<int, string> ExtractCodeSamples(ref string actionDescription)
    {
        Dictionary<int, string> codeSamples = new Dictionary<int, string>();
        string codeSample = string.Empty;
        int startPosition = 0;
        int endPosition = 0;
        int codeSampleCount = 0;
        string codeSampleOpeningTag = "<code>";
        string codeSampleClosingTag = "</code>";

        while (actionDescription.Substring(startPosition).ToLower().Contains(codeSampleOpeningTag) && actionDescription.Substring(endPosition).ToLower().Contains(codeSampleClosingTag))
        {
            codeSampleCount = codeSampleCount + 1;
            startPosition = actionDescription.IndexOf(codeSampleOpeningTag);
            endPosition = actionDescription.IndexOf(codeSampleClosingTag);
            codeSample = actionDescription.Substring(startPosition + codeSampleOpeningTag.Length, endPosition - startPosition - codeSampleClosingTag.Length + 1);
            codeSample = Environment.NewLine + HttpUtility.HtmlDecode(codeSample) + Environment.NewLine;
            actionDescription = actionDescription.Substring(0, startPosition) + "##codesample" + codeSampleCount.ToString() + "##" + actionDescription.Substring(endPosition + codeSampleClosingTag.Length);
            codeSamples.Add(codeSampleCount, codeSample);
            startPosition = 0;
            endPosition = 0;
        }

        return codeSamples;
    }

    public static void AddCodeSamples(ref string actionDescription, Dictionary<int, string> codeSamples)
    {
        foreach (KeyValuePair<int, string> code in codeSamples)
        {
            actionDescription = actionDescription.Replace("##codesample" + code.Key.ToString() + "##", code.Value);
        }
    }
  }
}
