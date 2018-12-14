using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using TeamSupport.Data;

namespace TeamSupport.Handlers
{
  public class TSHighrise
  {
    
    public static int CreateCompany(string companyName, string phone, string email, ProductType productType, string promo, string leadSource, string campaign, string background, string[] tags = null)
    {
      string company = @"
<company>
  <name>{0}</name>
  <background>{1}</background>
  <visible-to>Everyone</visible-to>
  <contact-data>
    <email-addresses>
      <email-address>
        <address>{2}</address>
        <location>Work</location>
      </email-address>
    </email-addresses>
    <phone-numbers>
      <phone-number>
        <number>{3}</number>
        <location>Work</location>
      </phone-number>
    </phone-numbers>
  </contact-data>
  <!-- custom fields -->
  <subject_datas type=""array"">
    <!-- Lead Source -->
    <subject_data>
      <value>{4}</value>
      <subject_field_id type=""integer"">909557</subject_field_id>
    </subject_data>
    <!-- Campaign -->
    <subject_data>
      <value>{5}</value>
      <subject_field_id type=""integer"">909690</subject_field_id>
    </subject_data>
    <!-- Industry -->
    <subject_data>
      <value>{6}</value>
      <subject_field_id type=""integer"">684681</subject_field_id>
    </subject_data>
    <!-- Promo -->
    <subject_data>
      <value>{7}</value>
      <subject_field_id type=""integer"">909519</subject_field_id>
    </subject_data>
    <!-- Edition -->
    <subject_data>
      <value>{8}</value>
      <subject_field_id type=""integer"">909518</subject_field_id>
    </subject_data>
    <!-- Refered by -->
    <subject_data>
      <value>{9}</value>
      <subject_field_id type=""integer"">182919</subject_field_id>
    </subject_data>
  </subject_datas>
</company>
";
      company = string.Format(company, companyName, background, email, phone, leadSource, campaign, "", promo, productType == ProductType.Enterprise ? "Enterprise" : "Support Desk", "");

      int result = GetResponseID(SendRequest(company, "companies.xml"));
      TagObject("companies", result, tags);

      return result;

    }

    public static int CreatePerson(string firstName, string lastName, string companyName, string phone, string email, ProductType productType, string promo, string leadSource, string campaign, string background, string[] tags = null)
    {
      
      string person = @"
<person>
  <first-name>{0}</first-name>
  <last-name>{1}</last-name>
  <title></title>
  <company-name>{2}</company-name>
  <background>{11}</background>
  <linkedin_url></linkedin_url>
  <contact-data>
    <email-addresses>
      <email-address>
        <address>{3}</address>
        <location>Work</location>
      </email-address>
    </email-addresses>
    <phone-numbers>
      <phone-number>
        <number>{4}</number>
        <location>Work</location>
      </phone-number>
    </phone-numbers>
  </contact-data>
  <!-- start of custom fields -->
  <subject_datas type=""array"">
    <!-- Lead Source -->
    <subject_data>
      <value>{5}</value>
      <subject_field_id type=""integer"">909557</subject_field_id>
    </subject_data>
    <!-- Campaign -->
    <subject_data>
      <value>{6}</value>
      <subject_field_id type=""integer"">909690</subject_field_id>
    </subject_data>
    <!-- Industry -->
    <subject_data>
      <value>{7}</value>
      <subject_field_id type=""integer"">684681</subject_field_id>
    </subject_data>
    <!-- Promo -->
    <subject_data>
      <value>{8}</value>
      <subject_field_id type=""integer"">909519</subject_field_id>
    </subject_data>
    <!-- Edition -->
    <subject_data>
      <value>{9}</value>
      <subject_field_id type=""integer"">909518</subject_field_id>
    </subject_data>
    <!-- Refered by -->
    <subject_data>
      <value>{10}</value>
      <subject_field_id type=""integer"">182919</subject_field_id>
    </subject_data>
  </subject_datas>
  <!-- end of custom fields -->
</person>
";
      person = string.Format(person, firstName, lastName, companyName, email, phone, leadSource, campaign, "", promo, productType == ProductType.Enterprise ? "Enterprise" : "Support Desk", "", background);

      int result = GetResponseID(SendRequest(person, "people.xml"));
      TagObject("people", result, tags);
      return result;
    }

    public static void TagObject(string subject, int id, string[] values)
    {
      if (values == null) return;

      foreach (string value in values)
      {
        TagObject(subject, id, value);
      }

    }

    public static void TagObject(string subject, int id, string value)
    {
      string tag = string.Format("<name>{0}</name>", value);
      SendRequest(tag, string.Format("{0}/{1}/tags.xml", subject, id.ToString()));
    }

    public static void CreateTaskDate(string body, DateTime date, string categoryID, string subjectType, string subjectID, string ownerID, bool isPublic, bool doNotify)
    {

      string dt = date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
      string task = @"
<task>
  <body>{0}</body>
  <frame>specific</frame>
  <due-at type=""datetime"">{1}</due-at>

  <!-- optional -->
  <subject-type>{2}</subject-type>
  <subject-id>{3}</subject-id>
  <category-id type=""integer"">{4}</category-id>
  <recording-id type=""integer""></recording-id>
  <owner-id type=""integer"">{5}</owner-id>
  <public type=""boolean"">{6}</public>
  <notify type=""boolean"">{7}</notify>
</task>
";
      task = string.Format(task, body, dt, subjectType, subjectID, categoryID, ownerID, isPublic ? "true" : "false", doNotify ? "true" : "false");

      SendRequest(task, "tasks.xml");

    }

    public static void CreateTaskFrame(string body, string frame, string categoryID, string subjectType, string subjectID, string ownerID, bool isPublic, bool doNotify)
    {
      //Party|Company|Kase|Deal
      string task = @"
<task>
  <body>{0}</body>
  <frame>{1}</frame>

  <!-- optional -->
  <subject-type>{2}</subject-type>
  <subject-id>{3}</subject-id>
  <category-id type=""integer"">{4}</category-id>
  <recording-id type=""integer""></recording-id>
  <owner-id type=""integer"">{5}</owner-id>
  <public type=""boolean"">{6}</public>
  <notify type=""boolean"">{7}</notify>
</task>
";
      task = string.Format(task, body, frame, subjectType, subjectID, categoryID, ownerID, isPublic ? "true" : "false", doNotify ? "true" : "false");

      SendRequest(task, "tasks.xml");

    }
    private static string SendRequest(string requestXml, string url)
    {
      requestXml = System.Text.RegularExpressions.Regex.Replace(requestXml, @"\t|\n|\r", "");
      string result = "";
      string baseUrl = "https://murocsystems.highrisehq.com/";
      byte[] bytes = UTF8Encoding.UTF8.GetBytes(requestXml);
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + url);
      request.Credentials = new NetworkCredential("5f06c8f00ef7e13316a0cfd0d87a7887b7e887a5", "X");
      request.Method = "POST";
      request.UserAgent = "TeamSupport";
      request.ContentType = "application/xml";
      request.ContentLength = bytes.Length;
      request.PreAuthenticate = true;

      using (var writer = request.GetRequestStream())
      {
        writer.Write(bytes, 0, bytes.Length);
      }

      using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
      {
        StreamReader reader = new StreamReader(response.GetResponseStream());
        result = reader.ReadToEnd();
      }  
      return result;
    }

    private static int GetResponseID(string response)
    { 
      XmlDocument doc = new XmlDocument();
        doc.XmlResolver = null;
        doc.LoadXml(response);
      return int.Parse(doc.DocumentElement.SelectSingleNode("//id").FirstChild.Value);
    }



  }
}
