using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Dynamic;
using System.Web;
using System.Net;
using System.IO;


namespace TeamSupport.Data.WebHooks
{
  public class SlackMessage
  {
    class SlackField
    {
      public SlackField(string title, string value, bool isShort)
      {
        Title = title;
        Value = value;
        IsShort = isShort;
      }

      public string Title { get; set; }
      public string Value { get; set; }
      public bool IsShort { get; set; }
    }

    private List<SlackField> _slackFields;

    public SlackMessage(LoginUser loginUser)
    {
      IsPlain = false;
      LoginUser = new Data.LoginUser(loginUser.ConnectionString, -1, 1078, null); ;
      _slackFields = new List<SlackField>();
    }

    public LoginUser LoginUser { get; set; }
    public bool IsPlain { get; set; }
    public string TextPlain { get; set; }
    public string TextRich { get; set; }
    public string Color { get; set; }
    
    private string GetUserSlackID(int userID)
    {
      CustomValue customValue = CustomValues.GetValue(LoginUser, userID, "slackname");
      if (customValue != null && !string.IsNullOrWhiteSpace(customValue.Value)) return customValue.Value;
      return null;
    }

    public void AddField(string title, string value, bool isShort)
    {
      _slackFields.Add(new SlackField(title, value, isShort));
    }


    public void Send(int? userID)
    {
      if (userID == null) return;
      string slackUser = GetUserSlackID((int)userID);

      if (!string.IsNullOrWhiteSpace(slackUser))
      {
        Send("@" + slackUser);
      }
    }
    public void Send(string channel)
    {
      var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://hooks.slack.com/services/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");
      httpWebRequest.ContentType = "application/x-www-form-urlencoded";
      httpWebRequest.Method = "POST";

      using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
      {
        dynamic payload = new ExpandoObject();
        payload.channel = channel;
        payload.username = "TeamSupport";

        if (this.IsPlain)
        {
          payload.text = WebUtility.HtmlEncode(this.TextPlain);
        }
        else
        {
          List<dynamic> attachments = new List<dynamic>();
          dynamic attachment = new ExpandoObject();
          attachment.fallback = WebUtility.HtmlEncode(this.TextPlain);
          attachment.pretext = WebUtility.HtmlEncode(this.TextRich);
          attachment.color = this.Color;

          List<dynamic> fields = new List<dynamic>();

          foreach (SlackField slackField in _slackFields)
          {
            dynamic field = new ExpandoObject();
            field.title = WebUtility.HtmlEncode(slackField.Title);
            field.value = WebUtility.HtmlEncode(slackField.Value);
            field.shortxxx = slackField.IsShort;
            fields.Add(field);
          }

          attachment.fields = fields.ToArray();
          attachments.Add(attachment);
          payload.attachments = attachments.ToArray();
        }

        string json = JsonConvert.SerializeObject(payload);

        streamWriter.Write("payload=" + Uri.EscapeUriString(json.Replace("shortxxx", "short")));
        streamWriter.Flush();
        streamWriter.Close();

        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
          var result = streamReader.ReadToEnd();
        }
      }
    }
  }
}
