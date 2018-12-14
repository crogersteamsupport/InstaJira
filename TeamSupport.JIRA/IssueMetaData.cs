using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TeamSupport.JIRA
{
	public class IssueMetaData
	{
		public class Schema
		{
			public string type { get; set; }
			public string system { get; set; }
			public string items { get; set; }
			public string custom { get; set; }
			public int customId { get; set; }
		}

		public class Summary
		{
			public bool required { get; set; }
			public Schema schema { get; set; }
			public string name { get; set; }
			public bool hasDefaultValue { get; set; }
			public List<string> operations { get; set; }
		}

		public class AllowedValue
		{
			public string self { get; set; }
			public string id { get; set; }
			public string key { get; set; }
			public string name { get; set; }
			public string value { get; set; }
			public string description { get; set; }
			public string iconUrl { get; set; }
			public bool subtask { get; set; }
			public int avatarId { get; set; }
			public bool archived { get; set; }
			public bool released { get; set; }
			public string releaseDate { get; set; }
			public string userReleaseDate { get; set; }
			public int projectId { get; set; }
		}

		public class Components
		{
			public bool required { get; set; }
			public Schema schema { get; set; }
			public string name { get; set; }
			public bool hasDefaultValue { get; set; }
			public List<string> operations { get; set; }
			public List<AllowedValue> allowedValues { get; set; }
		}

		public class Description
		{
			public bool required { get; set; }
			public Schema schema { get; set; }
			public string name { get; set; }
			public bool hasDefaultValue { get; set; }
			public List<string> operations { get; set; }
		}

		public class Project
		{
			public bool required { get; set; }
			public Schema schema { get; set; }
			public string name { get; set; }
			public bool hasDefaultValue { get; set; }
			public List<string> operations { get; set; }
			public List<AllowedValue> allowedValues { get; set; }
		}

		public class FixVersions
		{
			public bool required { get; set; }
			public Schema schema { get; set; }
			public string name { get; set; }
			public bool hasDefaultValue { get; set; }
			public List<string> operations { get; set; }
			public List<AllowedValue> allowedValues { get; set; }
		}

		public class Priority
		{
			public bool required { get; set; }
			public Schema schema { get; set; }
			public string name { get; set; }
			public bool hasDefaultValue { get; set; }
			public List<string> operations { get; set; }
			public List<AllowedValue> allowedValues { get; set; }
		}

		public class Labels
		{
			public bool required { get; set; }
			public Schema schema { get; set; }
			public string name { get; set; }
			public string autoCompleteUrl { get; set; }
			public bool hasDefaultValue { get; set; }
			public List<string> operations { get; set; }
		}

		public class Timetracking
		{
			public bool required { get; set; }
			public Schema schema { get; set; }
			public string name { get; set; }
			public bool hasDefaultValue { get; set; }
			public List<string> operations { get; set; }
		}

		public class Environment
		{
			public bool required { get; set; }
			public Schema schema { get; set; }
			public string name { get; set; }
			public bool hasDefaultValue { get; set; }
			public List<string> operations { get; set; }
		}

		public class Customfield
		{
			public string customName { get; set; }
			public bool required { get; set; }
			public Schema schema { get; set; }
			public string name { get; set; }
			public bool hasDefaultValue { get; set; }
			public List<string> operations { get; set; }
			public List<AllowedValue> allowedValues { get; set; }
		}

		public class Attachment
		{
			public bool required { get; set; }
			public Schema schema { get; set; }
			public string name { get; set; }
			public bool hasDefaultValue { get; set; }
			public List<object> operations { get; set; }
		}

		public class Versions
		{
			public bool required { get; set; }
			public Schema schema { get; set; }
			public string name { get; set; }
			public bool hasDefaultValue { get; set; }
			public List<string> operations { get; set; }
			public List<AllowedValue> allowedValues { get; set; }
		}

		public class Duedate
		{
			public bool required { get; set; }
			public Schema schema { get; set; }
			public string name { get; set; }
			public bool hasDefaultValue { get; set; }
			public List<string> operations { get; set; }
		}

		public class Fields
		{
			public Summary summary { get; set; }
			public IssuetypeField issuetype { get; set; }
			public Components components { get; set; }
			public Description description { get; set; }
			public Project project { get; set; }
			public FixVersions fixVersions { get; set; }
			public Priority priority { get; set; }
			public Labels labels { get; set; }
			public Timetracking timetracking { get; set; }
			public Environment environment { get; set; }
			public Attachment attachment { get; set; }
			public Versions versions { get; set; }
			public Duedate duedate { get; set; }
			public Dictionary<string, Customfield> customFields { get; set; }
		}

		public class IssuetypeField
		{
			public bool required { get; set; }
			public Schema schema { get; set; }
			public string name { get; set; }
			public bool hasDefaultValue { get; set; }
			public List<object> operations { get; set; }
			public List<AllowedValue> allowedValues { get; set; }
		}

		public class Issuetype
		{
			public string self { get; set; }
			public string id { get; set; }
			public string description { get; set; }
			public string iconUrl { get; set; }
			public string name { get; set; }
			public bool subtask { get; set; }
			public string expand { get; set; }
			public Fields fields { get; set; }
		}

		public class ProjectInfo
		{
			public string expand { get; set; }
			public string self { get; set; }
			public string id { get; set; }
			public string key { get; set; }
			public string name { get; set; }
			public object avatarUrls { get; set; }
			public List<Issuetype> issuetypes { get; set; }
		}

		public class RootObject
		{
			public string expand { get; set; }
			public List<ProjectInfo> projects { get; set; }
		}

		public static Dictionary<string, Customfield> GetCustomFields(string customFieldsJson)
		{
			Dictionary<string, Customfield> customFields = new Dictionary<string, Customfield>();

            try
			{
				JObject result = JObject.Parse(customFieldsJson);

				foreach (KeyValuePair<string, JToken> field in result["projects"][0]["issuetypes"][0]["fields"] as JObject)
				{
					if (!string.IsNullOrEmpty(field.Key)
						&& field.Key.ToLower().Contains("customfield_")
						&& !string.IsNullOrEmpty(field.Value.ToString()))
					{
						
						if (customFields == null)
						{
							customFields = new Dictionary<string, Customfield>();
						}

						Customfield customField = new Customfield();

						try
						{
							JObject jObject = JObject.Parse(field.Value.ToString());
							customField = JsonConvert.DeserializeObject<Customfield>(jObject.ToString());
						}
						catch (Exception ex)
						{
							
						}

						customFields.Add(field.Key, customField);
					}
				}
			}
			catch (Exception ex)
			{

			}

			return customFields;
		}
	}
}
