using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using TidyNet;
using HtmlAgilityPack;
using System.Linq;

namespace TeamSupport.Data
{
    /// <summary>
    ///  reference: http://www.robertbeal.com/37/sanitising-html 
    /// </summary>
    public class HtmlUtility
    {
        private static readonly Regex HtmlTagExpression = new Regex(@"
            (?'tag_start'</?)
            (?'tag'\w+)((\s+
            (?'attribute'(\w+)(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+)))?)+\s*|\s*)
            (?'tag_end'/?>)",
            RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex HtmlAttributeExpression = new Regex(@"
            (?'attribute'\w+)
            (\s*=\s*)
            (""(?'value'.*?)""|'(?'value'.*?)')",
            RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Dictionary<string, List<string>> ValidHtmlTags = new Dictionary<string, List<string>>
        {
            {"p", new List<string>          {"style", "class", "align"}},
            {"div", new List<string>        {"style", "class", "align"}},
            {"span", new List<string>       {"style", "class"}},
            {"br", new List<string>         {"style", "class"}},
            {"hr", new List<string>         {"style", "class"}},
            {"label", new List<string>      {"style", "class"}},
            
            {"h1", new List<string>         {"style", "class"}},
            {"h2", new List<string>         {"style", "class"}},
            {"h3", new List<string>         {"style", "class"}},
            {"h4", new List<string>         {"style", "class"}},
            {"h5", new List<string>         {"style", "class"}},
            {"h6", new List<string>         {"style", "class"}},
            
            {"font", new List<string>       {"style", "class", "color", "face", "size"}},
            {"strong", new List<string>     {"style", "class"}},
            {"b", new List<string>          {"style", "class"}},
            {"em", new List<string>         {"style", "class"}},
            {"i", new List<string>          {"style", "class"}},
            {"u", new List<string>          {"style", "class"}},
            {"strike", new List<string>     {"style", "class"}},
            {"ol", new List<string>         {"style", "class"}},
            {"ul", new List<string>         {"style", "class"}},
            {"li", new List<string>         {"style", "class"}},
            {"blockquote", new List<string> {"style", "class"}},
            {"pre", new List<string>        {"style", "class"}},
            {"code", new List<string>       {"style", "class"}},
            
            {"a", new List<string>          {"style", "class", "href", "title", "target"}},
            {"img", new List<string>        {"style", "class", "src", "height", "width", "alt", "title", "hspace", "vspace", "border"}},

            {"table", new List<string>      {"style", "class"}},
            {"thead", new List<string>      {"style", "class"}},
            {"tbody", new List<string>      {"style", "class"}},
            {"tfoot", new List<string>      {"style", "class"}},
            {"th", new List<string>         {"style", "class", "scope"}},
            {"tr", new List<string>         {"style", "class"}},
            {"td", new List<string>         {"style", "class", "colspan"}},

            {"q", new List<string>          {"style", "class", "cite"}},
            {"cite", new List<string>       {"style", "class"}},
            {"abbr", new List<string>       {"style", "class"}},
            {"acronym", new List<string>    {"style", "class"}},
            {"del", new List<string>        {"style", "class"}},
            {"ins", new List<string>        {"style", "class"}}
        };

        /// <summary>
        /// Removes the invalid HTML tags.
        /// </summary>
        /// <param name="input">The text.</param>
        /// <returns></returns>
        public static string RemoveInvalidHtmlTags(string input)
        {
            var html = TidyHtml(input);

            if (string.IsNullOrEmpty(html))
            {
                return HttpUtility.HtmlEncode(input);
            }

            return HtmlTagExpression.Replace(html, new MatchEvaluator(match =>
            {
                var builder = new StringBuilder(match.Length);

                var tagStart = match.Groups["tag_start"];
                var tagEnd = match.Groups["tag_end"];
                var tag = match.Groups["tag"].Value;
                var attributes = match.Groups["attribute"];

                if (false == ValidHtmlTags.ContainsKey(tag))
                    return HttpUtility.HtmlEncode(match.Value);

                builder.Append(tagStart.Success ? tagStart.Value : "<");
                builder.Append(tag);

                foreach (Capture attribute in attributes.Captures)
                {
                    builder.Append(MatchHtmlAttribute(tag, attribute));
                }

                // add nofollow to all hyperlinks
                if (tagStart.Success && tagStart.Value == "<" && tag == "a")
                {
                    builder.Append(" rel=\"nofollow\"");
                }

                builder.Append(tagEnd.Success ? tagEnd.Value : ">");

                return builder.ToString();
            }));
        }

        private static string MatchHtmlAttribute(string tag, Capture capture)
        {
            var output = string.Empty;
            var match = HtmlAttributeExpression.Match(capture.Value);

            var attribute = match.Groups["attribute"].Value;
            var value = match.Groups["value"].Value;

            if (ValidHtmlTags[tag].Contains(attribute))
            {
                switch (attribute)
                {
                    case "src":
                    case "href":
                        if (Regex.IsMatch(value, @"https?://[^""]+"))
                            output = string.Format(" {0}=\"{1}\"", attribute, HttpUtility.UrlPathEncode(value));
                        break;
                    default:
                        output = string.Format(" {0}=\"{1}\"", attribute, value);
                        break;
                }
            }

            return output;
        }

        public static string TidyHtml(string text)
        {
            var doc = new Tidy();
            var messages = new TidyMessageCollection();
            var input = new MemoryStream();
            var output = new MemoryStream();

            var array = Encoding.UTF8.GetBytes(text);
            input.Write(array, 0, array.Length);
            input.Position = 0;

            /*
            // Disabled as it causes problems handling "font" tags
            // There are occurences when it will muck up a font tag to "fontface=...etc...
            //doc.Options.Xhtml = true;
            doc.Options.MakeClean = false;
            doc.Options.DocType = DocType.Strict;
            doc.Options.CharEncoding = CharEncoding.UTF8;
            doc.Options.LogicalEmphasis = true;

            doc.Options.SmartIndent = true;
            doc.Options.IndentContent = true;
            doc.Options.TidyMark = false;
            doc.Options.QuoteAmpersand = true;
            doc.Options.DropFontTags = false;
            doc.Options.DropEmptyParas = true;

            // Required to stop spaces being removed, and tabs added etc...
            doc.Options.Spaces = 0;
            doc.Options.WrapLen = 32000;
          */

            doc.Options.TidyMark = false;
            doc.Options.MakeClean = true;
            doc.Options.Word2000 = true;
            doc.Options.EncloseText = true;

            // Required to stop spaces being removed, and tabs added etc...
            doc.Options.Spaces = 0;
            doc.Options.WrapLen = 32000;
            doc.Parse(input, output, messages);
            //return Encoding.UTF8.GetString(output.ToArray());
            return RemoveTidyAdditions(Encoding.UTF8.GetString(output.ToArray()));
        }

        private static string RemoveTidyAdditions(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var start = text.IndexOf("<body");
            if (start > -1) start = text.IndexOf(">", start);
            var end = text.IndexOf("</body");

            if (start != -1 && end > start && end < text.Length)
            {
                text = text.Substring(start + 1, end - (start + 1));
            }
            else
            {
                return string.Empty;
            }

            return Regex.Replace(text, "[\r\n\t]*", string.Empty);
        }

        public static string StripHTML(string Content, bool addLineBreaks = false) {
            //Ticket 35785. Trying to keep blank lines in between. Done specifically for Cayenta (1081853) for now, if it looks good after a while we might need to open it to all orgs.
            if (addLineBreaks)
            {
                Content = Content.Replace("</p>", "</p>TeamSupportNewLine");
                Content = Content.Replace("<br>", "<br>TeamSupportNewLine");
                Content = Content.Replace("&nbsp;</div>", "&nbsp;</div>TeamSupportNewLine");
            }

            // While researching Ticket #20811 realized further Regex.Replace break in multiple lines. This added to temporarily remove break lines, and carriage returns.
            Content = Regex.Replace(Content, @"\n", "TeamSupportNewLine");
            Content = Regex.Replace(Content, @"\r", "TeamSupportNewLine");
            
            // Added to remove <style> tag and its contect based on http://forums.asp.net/t/1901224.aspx?Remove+Head+tag+from+HTML+
            Content = Regex.Replace(Content, "<style(.|\n)*?</style>", string.Empty);
            Content = Regex.Replace(Content, "<w:WordDocument>(.|\n)*?</w:WordDocument>", string.Empty);
            
            Content = Regex.Replace(Content, "<.*?>", string.Empty);
            Content = HttpUtility.HtmlDecode(Content);
            Content = StripComments(Content);

            //regex based on http://stackoverflow.com/questions/787932/using-c-regular-expressions-to-remove-html-tags/787949#787949
            Content = Regex.Replace(Content, @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", "", RegexOptions.Singleline);

            // Added to remove duplicate spaces based on http://texthandler.com/?module=remove_double_spaces_cc
            Content = Regex.Replace(Content, " {2,}", " ");

            Content = Regex.Replace(Content, "TeamSupportNewLine", Environment.NewLine);

            Content = Regex.Replace(Content, @"(?:(?:\r?\n)+ +){2,}", Environment.NewLine + Environment.NewLine);

            if (!addLineBreaks)
            {
                //regex to replace the contiguous newlines, leave just one. ref: http://stackoverflow.com/questions/11710966/building-a-regex-how-to-remove-redundant-line-breaks
                Content = Regex.Replace(Content, @"\s*\r\n\s*", "\r\n");
            }

            //Remove trailing empty lines
            Content = Content.TrimEnd('\r', '\n');

            return Content;
        }

        // Same as StripHTML but split table content with spaces
        public static string StripHTML2(string Content)
        {
            // While researching Ticket #20811 realized further Regex.Replace break in multiple lines. This added to temporarily remove break lines, and carriage returns.
            Content = Regex.Replace(Content, @"\n", "TeamSupportNewLine");
            Content = Regex.Replace(Content, @"\r", "TeamSupportNewLine");

            // Added to remove <style> tag and its contect based on http://forums.asp.net/t/1901224.aspx?Remove+Head+tag+from+HTML+
            Content = Regex.Replace(Content, "<style(.|\n)*?</style>", string.Empty);
            Content = Regex.Replace(Content, "<w:WordDocument>(.|\n)*?</w:WordDocument>", string.Empty);

            Content = Regex.Replace(Content, "<td>", " ");
            Content = Regex.Replace(Content, "</td>", " ");
            Content = Regex.Replace(Content, "<.*?>", string.Empty);
            Content = System.Web.HttpUtility.HtmlDecode(Content);
            Content = StripComments(Content);

            //regex based on http://stackoverflow.com/questions/787932/using-c-regular-expressions-to-remove-html-tags/787949#787949
            Content = Regex.Replace(Content, @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", "", RegexOptions.Singleline);

            // Added to remove duplicate spaces based on http://texthandler.com/?module=remove_double_spaces_cc
            Content = Regex.Replace(Content, " {2,}", " ");

            Content = Regex.Replace(Content, "TeamSupportNewLine", Environment.NewLine);

            Content = Regex.Replace(Content, @"(?:(?:\r?\n)+ +){2,}", Environment.NewLine + Environment.NewLine);

            //regex to replace the contiguous newlines, leave just one. ref: http://stackoverflow.com/questions/11710966/building-a-regex-how-to-remove-redundant-line-breaks
            Content = Regex.Replace(Content, @"\s*\r\n\s*", "\r\n");

            return Content;
        }

        public static string StripHTMLUsingAgilityPack(string Content)
        {
            StringBuilder output = new StringBuilder();
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(Content);

                var text = doc.DocumentNode.SelectNodes("//body//text()").Select(node => node.InnerText);

                foreach (string line in text)
                {
                    output.AppendLine(line);
                }
                return HttpUtility.HtmlDecode(output.ToString());
            }
            catch (Exception ex)
            {
                //No HTML Exists
            }
            return Content;
        }

        public static string StripComments(string Content) {
            Content = Regex.Replace(Content, @"<!--.*?-->", " ");
            return Content;
        }

        public static string TagHtml(LoginUser loginUser,  string text)
        {
          HtmlDocument doc = new HtmlDocument();
          doc.LoadHtml(text);
          if (doc != null && doc.DocumentNode != null)
          {
            StringBuilder builder = new StringBuilder();
            Tags tags = new Tags(loginUser);
            tags.LoadByOrganization(loginUser.OrganizationID);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//text()");
            if (nodes != null)
            {
              foreach (HtmlAgilityPack.HtmlTextNode node in nodes)
              {
                if (node.ParentNode != null && node.ParentNode.Name == "a") continue;
                foreach (Tag tag in tags)
                {
                  node.Text = DataUtils.CreateLinks(node.Text, tag.Value, "#", "tag-link tagid-" + tag.TagID.ToString(), "_blank");
                }

                builder.Append(node.Text);
              }
              using (StringWriter writer = new StringWriter())
              {
                doc.Save(writer);
                return writer.ToString();
              }
            }
          }

          return "";
        }

        public static string FixScreenRFrame(string html)
        {
          HtmlDocument doc = new HtmlDocument();
          doc.LoadHtml(html);
          //http://htmlagilitypack.codeplex.com/discussions/24346
          HtmlNodeCollection nc = doc.DocumentNode.SelectNodes("//iframe[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'https://teamsupport.viewscreencasts.com')]");
          if (nc != null)
          {
            foreach (HtmlNode node in nc)
            {
              node.InnerHtml = HttpUtility.HtmlDecode(node.InnerHtml);
            }
          }
          return doc.DocumentNode.WriteTo();
        }

        public static string CheckScreenR(LoginUser loginUser, string text)
        {
          if (SystemSettings.ReadString(loginUser, "KillScreenR", false.ToString()).ToLower().IndexOf("t") < 0) return text;
          HtmlDocument doc = new HtmlDocument();
          doc.LoadHtml(text);
          //http://htmlagilitypack.codeplex.com/discussions/24346
          HtmlNodeCollection nc = doc.DocumentNode.SelectNodes("//iframe[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'https://teamsupport.viewscreencasts.com')]");
          if (nc != null)
          {
            foreach (HtmlNode node in nc)
            {
              node.ParentNode.RemoveChild(node, false);
            }
          }
          return doc.DocumentNode.WriteTo();
        }

        public static string Sanitize(string text)
        {
          HtmlDocument doc = new HtmlDocument();
          doc.LoadHtml(text);
          //http://htmlagilitypack.codeplex.com/discussions/24346
          //Remove potentially harmful elements
          //HtmlNodeCollection nc = doc.DocumentNode.SelectNodes("//script|//link|//iframe|//frameset|//frame|//applet|//object|//embed");
          HtmlNodeCollection nc = doc.DocumentNode.SelectNodes("//script|//style|//link");
          if (nc != null)
          {
            foreach (HtmlNode node in nc)
            {
              node.ParentNode.RemoveChild(node, false);

            }
          }

          //remove hrefs to java/j/vbscript URLs
          nc = doc.DocumentNode.SelectNodes("//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'javascript')]|//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'jscript')]|//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'vbscript')]");
          if (nc != null)
          {

            foreach (HtmlNode node in nc)
            {
              node.SetAttributeValue("href", "#");
            }
          }


          //remove img with refs to java/j/vbscript URLs
          nc = doc.DocumentNode.SelectNodes("//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'javascript')]|//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'jscript')]|//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'vbscript')]");
          if (nc != null)
          {
            foreach (HtmlNode node in nc)
            {
              node.SetAttributeValue("src", "#");
            }
          }

          //remove on<Event> handlers from all tags
          nc = doc.DocumentNode.SelectNodes("//*[@onclick or @onmouseover or @onfocus or @onblur or @onmouseout or @ondoubleclick or @onload or @onunload]");
          if (nc != null)
          {
            foreach (HtmlNode node in nc)
            {
              node.Attributes.Remove("onFocus");
              node.Attributes.Remove("onBlur");
              //added handler to keep onclick events for ticket links to open new tab instead of new window
              if (node.Attributes["onClick"] != null && !node.Attributes["onClick"].Value.Contains("top.Ts"))
              {
                  node.Attributes.Remove("onClick");
              }
              node.Attributes.Remove("onMouseOver");
              node.Attributes.Remove("onMouseOut");
              node.Attributes.Remove("onDoubleClick");
              node.Attributes.Remove("onLoad");
              node.Attributes.Remove("onUnload");
            }
          }

          // remove any style attributes that contain the word expression (IE evaluates this as script)
          nc = doc.DocumentNode.SelectNodes("//*[contains(translate(@style, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'expression')]");
          if (nc != null)
          {
            foreach (HtmlNode node in nc)
            {
              node.Attributes.Remove("style");
            }
          }

          return doc.DocumentNode.WriteTo();


          //remove attributes handlers from all tags
          //nc = doc.DocumentNode.SelectNodes("//*[@class or @id or @style]");
          nc = doc.DocumentNode.SelectNodes("//*[@class or @id]");
          if (nc != null)
          {
            foreach (HtmlNode node in nc)
            {
              node.Attributes.Remove("id");
              node.Attributes.Remove("class");
              node.Attributes.Remove("style");
            }
          }

          return doc.DocumentNode.WriteTo();
        }


    }
}
