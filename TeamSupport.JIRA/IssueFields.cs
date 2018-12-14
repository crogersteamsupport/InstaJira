using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TeamSupport.JIRA
{
    public class IssueFields
    {
        public IssueFields()
        {
            status = new Status();
            timetracking = new Timetracking();

            labels = new List<String>();
            comments = new List<Comment>();
            issuelinks = new List<IssueLink>();
            attachment = new List<Attachment>();
            watchers = new List<JiraUser>();
		}

		public void SetCustomFields(string customFieldsJson)
		{
			try
			{
				JObject result = JObject.Parse(customFieldsJson);

				foreach (KeyValuePair<string, JToken> field in result["fields"] as JObject)
				{
					if (!string.IsNullOrEmpty(field.Key)
						&& field.Key.ToLower().Contains("customfield_")
						&& !string.IsNullOrEmpty(field.Value.ToString()))
					{
						if (customFields == null)
						{
							customFields = new Dictionary<string, CustomField>();
						}

						CustomField customField = new CustomField();

						try
						{
							JObject jObject = JObject.Parse(field.Value.ToString());
							customField = JsonConvert.DeserializeObject<CustomField>(jObject.ToString());
						}
						catch (Exception ex)
						{
							customField.Value = field.Value.ToString();
						}

						customFields.Add(field.Key, customField);
					}
				}
			}
			catch (Exception ex)
			{

			}
		}

		public String summary { get; set; }
        public String description { get; set; }
        public Timetracking timetracking { get; set; }
        public Status status { get; set; }

        public JiraUser reporter { get; set; }
        public JiraUser assignee { get; set; }
        public List<JiraUser> watchers { get; set; } 

        public List<String> labels { get; set; }
        public List<Comment> comments { get; set; }
        public List<IssueLink> issuelinks { get; set; }
        public List<Attachment> attachment { get; set; }
		public Priority priority { get; set; }

		public Dictionary<string, CustomField> customFields { get; set; }
	}

	public class CustomField
	{
		public int Id { get; set; }

		public string Value { get; set; }
	}
}
