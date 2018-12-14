using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TeamSupport.JIRA
{
	public class JiraErrorsResponse
	{
		[JsonProperty("errorMessages")]
		public string[] ErrorMessages { get; set; }

		[JsonProperty("errors")]
		public JiraErrors Errors { get; set; }

		public bool HasErrors
		{
			get
			{
				bool hasErrors = (ErrorMessages != null && ErrorMessages.Any())
								|| (Errors != null &&
									(!string.IsNullOrEmpty(Errors.Issuetype)
									|| !string.IsNullOrEmpty(Errors.Project)
									|| !string.IsNullOrEmpty(Errors.Priority)
									|| (Errors.CustomFields.Any() && Errors.CustomFields.Count > 0)));
				return hasErrors;
			}
		}

		public JiraErrorsResponse()
		{
		}

		public static JiraErrorsResponse Get(WebException webException)
		{
			JiraErrorsResponse errorsResponse = new JiraErrorsResponse();

			if (webException != null && webException.Response != null)
			{
				StreamReader reader = new StreamReader(webException.Response.GetResponseStream());
				string content = reader.ReadToEnd();
				reader.Close();

				try
				{
					errorsResponse = JsonConvert.DeserializeObject<JiraErrorsResponse>(content);

					if (errorsResponse != null && errorsResponse.Errors != null)
					{
						errorsResponse.Errors.CustomFields = GetCustomFieldsErrors(content);
					}
				}
				catch (Exception ex)
				{
					List<string> error = new List<string>() { ex.Message, content };
					errorsResponse.ErrorMessages = error.ToArray();
				}
			}

			return errorsResponse;
		}

		public override string ToString()
		{
			StringBuilder result = new StringBuilder();

			if (ErrorMessages != null && ErrorMessages.Any())
			{
				foreach (string errormessage in ErrorMessages)
				{
					if (result.Length > 0)
					{
						result.Append(Environment.NewLine);
					}

					result.Append(errormessage);
				}
			}

			if (Errors != null)
			{
				foreach (PropertyInfo propertyInfo in Errors.GetType().GetProperties())
				{
					if (propertyInfo.CanRead)
					{
						if (propertyInfo.GetValue(Errors, null) != null)
						{
							string propertyName = propertyInfo.Name;
							string propertyValue = propertyInfo.GetValue(Errors, null).ToString();

							if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType)
								&& propertyInfo.PropertyType.IsGenericType
								&& propertyInfo.PropertyType.GetGenericArguments().Length > 0)
							{
								if (propertyName.ToLower() == "customfields")
								{
									if (result.Length > 0)
									{
										result.Append(Environment.NewLine);
									}

									foreach (KeyValuePair<string, string> customFieldError in Errors.CustomFields)
									{
										result.Append(string.Format("{0}: {1}", customFieldError.Key, customFieldError.Value));
									}
								}
							}
							else
							{
								if (result.Length > 0)
								{
									result.Append(Environment.NewLine);
								}

								result.Append(string.Format("{0}: {1}", propertyName, propertyValue));
							}
						}
					}
				}
			}

			return result.ToString();
		}

		private static Dictionary<string, string> GetCustomFieldsErrors(string customFieldsJson)
		{
			Dictionary<string, string> customFieldErrors = new Dictionary<string, string>();

			try
			{
				JObject jObject = JObject.Parse(customFieldsJson);
                var msgProperty = jObject.Property("errors");

				if (jObject != null && msgProperty != null)
				{
					foreach (KeyValuePair<string, JToken> field in jObject["errors"] as JObject)
					{
						customFieldErrors.Add(field.Key, field.Value.ToString());
					}
				}
			}
			catch (Exception ex)
			{
			}

			return customFieldErrors;
		}
	}

	public class JiraErrors
	{

		[JsonProperty("issuetype")]
		public string Issuetype { get; set; }

		[JsonProperty("project")]
		public string Project { get; set; }

		[JsonProperty("priority")]
		public string Priority { get; set; }

		[JsonProperty("errors")]
		public Dictionary<string, string> CustomFields { get; set; }
	}
}
