﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace TeamSupport.ServiceLibrary
{
    public class TFS
    {
        private static string _hostname;
        private static string _accessToken;
        private static string _username;
        private static string _password;
        private static List<WorkItemField> _workItemFields;

        public TFS()
        {
        }

        public TFS(string hostname, string accessToken)
        {
            _hostname = hostname;
            _accessToken = accessToken;
            _username = string.Empty;
            _password = string.Empty;
        }

        public TFS(string hostname, string username, string password)
        {
            _hostname = hostname;
            _username = username;
            _password = password;
            _accessToken = string.Empty;
        }

        //vv
        private string MakeRequest(string uri, ApiMethod method, string patchDocument = null)
        {
            string result = null;
            string contentType = "application/json";

            if (method == ApiMethod.Patch)
            {
                contentType = "application/json-patch+json";
            }

            using (var client = new WebClient { UseDefaultCredentials = false })
            {
                client.Headers.Add(HttpRequestHeader.ContentType, contentType);

                bool useApiToken = !string.IsNullOrEmpty(AccessToken);

                if (useApiToken)
                {
                    client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + EncodedCredentials);
                }
                else
                {
                    NetworkCredential netCred = new NetworkCredential(UserName, Password);
                    client.Credentials = netCred;
                }

                if (method == ApiMethod.Get)
                {
                    Stream stream = client.OpenRead(uri);
                    StreamReader sr = new StreamReader(stream);
                    result = sr.ReadToEnd();
                }
                else
                {
                    byte[] response = client.UploadData(uri, Encoding.UTF8.GetBytes(patchDocument));
                    result = client.Encoding.GetString(response);
                }
            }

            return result;
        }

        public string CheckCredentialsAndHost()
        {
            string responseBody = null;

            try
            {
                responseBody = MakeRequest(string.Format("{0}/_apis/wit/workitems/1", HostName), ApiMethod.Get);

                //This is what we check for vs team services (cloud). If the credentials are wrong then there is a result back, with the html code for the sign in page.
                if (!responseBody.Contains("<!DOCTYPE html PUBLIC") && !responseBody.Contains("Visual Studio Team Services | Sign In"))
                {
                    responseBody = null;
                }
            }
            catch (Exception ex)
            {
                //we are just checking if credentials and hostname are correct.
                if (ex.Message.ToLower().Contains("unauthorized") || ex.Message.ToLower().Contains("401"))
                {
                    //invalid credentials
                    responseBody = ex.Message;
                }
                else if (ex.Message.ToLower().Contains("could not be resolved"))
                {
                    //hostname is invalid
                    responseBody = ex.Message;
                }
                else if (ex.Message.ToLower().Contains("error") || ex.Message.ToLower().Contains("404"))
                {
                    //hostname is invalid
                    responseBody = ex.Message;
                }
            }

            return responseBody;
        }

        public string GetWorkItemsJsonBy(List<int> workItemIds, DateTime? lastLink)
        {
            var result = "";
            //vv see a few lines below. string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", AccessToken)));
            string idsCommaSeparated = string.Join(",", workItemIds);
            string dateOnly = "1900-01-01";
            
            if (lastLink.HasValue)
            {
                //vv We substract one day because of the GreaterThan '>' logic in the query below. wiql does not allow time precision for dates. (it is possible with other library which we are not using now)
                dateOnly = lastLink.Value.AddDays(-1).ToString("yyyy-MM-dd");
            }

            //create wiql object
            var wiql = new
            {
                query = "Select [id] " +
                        "From WorkItems " +
                        "Where " +
                        "[System.ChangedDate] > '" + dateOnly + "' " +
                        "And [id] IN (" + idsCommaSeparated + ") " +
                        "Order By [Changed Date] Desc"
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HostName);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", EncodedCredentials); //ToDo //vv check that this is the same as: credentials

                //serialize the wiql object into a json string. MediaType needs to be application/json for a post call
                var postValue = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(wiql), Encoding.UTF8, "application/json");
                var method = new HttpMethod("POST");

                //ToDo //vv The example I found for this uses the api version 2.2, the other examples uses 1.0. Check which one should we use and where to check the api versions.
                var httpRequestMessage = new HttpRequestMessage(method, string.Format("{0}/_apis/wit/wiql?api-version=2.2", HostName)) { Content = postValue };
                var httpResponseMessage = client.SendAsync(httpRequestMessage).Result;

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    WorkItemQueryResult workItemQueryResult = httpResponseMessage.Content.ReadAsAsync<WorkItemQueryResult>().Result;

                    //now that we have a bunch of work items, build a list of id's so we can get details
                    var builder = new StringBuilder();

                    foreach (var item in workItemQueryResult.WorkItems)
                    {
                        builder.Append(item.Id.ToString()).Append(",");
                    }

                    string ids = builder.ToString().TrimEnd(new char[] { ',' });

					if (!string.IsNullOrEmpty(ids))
					{
						//vv we could just bring the specific fields if needed, I don't see this happening now.
						//string fieldsCommaSeparated = string.Join(",", fields);
						//string queryString = string.Format("_apis/wit/workitems?ids={0}&fields={1}&asOf={2}&api-version=2.2", ids, fieldsCommaSeparated, workItemQueryResult.AsOf);

						//ToDo //vv The 'asOf' should be the last processed timestamp
						//ToDo //vv we probably should have a 'max' of ids here because there is a limit in the query string size. We'll need to loop if needed. https://stackoverflow.com/questions/812925/what-is-the-maximum-possible-length-of-a-query-string

						string queryString = string.Format("_apis/wit/workitems?ids={0}&asOf={1}&api-version=2.2", ids, workItemQueryResult.AsOf);
						HttpResponseMessage getWorkItemsHttpResponse = client.GetAsync(queryString).Result;

						if (getWorkItemsHttpResponse.IsSuccessStatusCode)
						{
							result = getWorkItemsHttpResponse.Content.ReadAsStringAsync().Result;
						}
						else
						{
							//ToDo //vv we have to return the error somehow..
						}
					}
                }
                else
                {
                    //ToDo //vv we have to return the error somehow..
                }
            }

            return result;
        }

		public WorkItems GetWorkItemsBy(List<int> workItemIds, DateTime? lastLink)
		{
			WorkItems result = new WorkItems();
			//vv see a few lines below. string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", AccessToken)));
			string idsCommaSeparated = string.Join(",", workItemIds);
			string dateOnly = "1900-01-01";

			if (lastLink.HasValue)
			{
				//vv We substract one day because of the GreaterThan '>' logic in the query below. wiql does not allow time precision for dates. (it is possible with other library which we are not using now)
				dateOnly = lastLink.Value.AddDays(-1).ToString("yyyy-MM-dd");
			}

			//create wiql object
			var wiql = new
			{
				query = "Select [id] " +
						"From WorkItems " +
						"Where " +
						"[System.ChangedDate] > '" + dateOnly + "' " +
						"And [id] IN (" + idsCommaSeparated + ") " +
						"Order By [Changed Date] Desc"
			};

            var patchValue = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(wiql), Encoding.UTF8, "application/json");

            try
            {
                string response = MakeRequest(string.Format("{0}/_apis/wit/wiql?api-version=2.2", HostName), ApiMethod.Post, patchValue.ReadAsStringAsync().Result);
                WorkItemQueryResult workItemQueryResult = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkItemQueryResult>(response);

                //now that we have a bunch of work items, build a list of id's so we can get details
                var builder = new StringBuilder();

                foreach (var item in workItemQueryResult.WorkItems)
                {
                    builder.Append(item.Id.ToString()).Append(",");
                }

                string ids = builder.ToString().TrimEnd(new char[] { ',' });

                if (!string.IsNullOrEmpty(ids))
                {
                    //vv we could just bring the specific fields if needed, I don't see this happening now.
                    //string fieldsCommaSeparated = string.Join(",", fields);
                    //string queryString = string.Format("_apis/wit/workitems?ids={0}&fields={1}&asOf={2}&api-version=2.2", ids, fieldsCommaSeparated, workItemQueryResult.AsOf);

                    //ToDo //vv The 'asOf' should be the last processed timestamp
                    //ToDo //vv we probably should have a 'max' of ids here because there is a limit in the query string size. We'll need to loop if needed. https://stackoverflow.com/questions/812925/what-is-the-maximum-possible-length-of-a-query-string

                    string queryString = string.Format("_apis/wit/workitems?ids={0}&asOf={1}&api-version=2.2", ids, workItemQueryResult.AsOf);
                    response = MakeRequest(HostName + "/" + queryString, ApiMethod.Get);

                    result = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkItems>(response);
                }
            }
            catch (Exception ex)
            {
                //var contents = response.Content.ReadAsStringAsync().Result;
                //TFSErrorsResponse tfsError = Newtonsoft.Json.JsonConvert.DeserializeObject<TFSErrorsResponse>(contents);
                //throw new TFSClientException(tfsError);
            }

			return result;
		}

		public WorkItem GetWorkItemBy(int workItemId, bool expandAll = false)
		{
			WorkItem workItem = new WorkItem();

			try
			{
                string response = MakeRequest(string.Format("{0}/_apis/wit/workItems/{1}?api-version=2.2{2}", HostName, workItemId, (expandAll ? "&$expand=all" : "")), ApiMethod.Get);
                workItem = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkItem>(response);
			}
			catch (Exception ex)
			{
                //var contents = response.Content.ReadAsStringAsync().Result;
                //TFSErrorsResponse tfsError = Newtonsoft.Json.JsonConvert.DeserializeObject<TFSErrorsResponse>(contents);
                //throw new TFSClientException(tfsError);
            }

			return workItem;
		}

		public WorkItemCommentList GetCommentsBy(int workItemId)
		{
			WorkItemCommentList comments = new WorkItemCommentList();

			try
			{
                string response = MakeRequest(string.Format("{0}/_apis/wit/workItems/{1}/comments", HostName, workItemId), ApiMethod.Get);
                comments = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkItemCommentList>(response);
			}
			catch (Exception ex)
			{
                //var contents = response.Content.ReadAsStringAsync().Result;
                //TFSErrorsResponse tfsError = Newtonsoft.Json.JsonConvert.DeserializeObject<TFSErrorsResponse>(contents);
                //throw new TFSClientException(tfsError);
            }

            return comments;
		}

		public WorkItemComment GetCommentBy(int workItemId, int revisionId)
		{
			WorkItemComment comments = new WorkItemComment();

			try
			{
                string response = MakeRequest(string.Format("{0}/_apis/wit/workItems/{1}/comments/{2}", HostName, workItemId, revisionId), ApiMethod.Get);
                comments = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkItemComment>(response);
			}
			catch (Exception ex)
			{
                //var contents = response.Content.ReadAsStringAsync().Result;
                //TFSErrorsResponse tfsError = Newtonsoft.Json.JsonConvert.DeserializeObject<TFSErrorsResponse>(contents);
                //throw new TFSClientException(tfsError);
            }

            return comments;
		}

		public WorkItem CreateWorkItem(List<WorkItemField> fields, string project, string type)
        {
            WorkItem workItem = new WorkItem();
			Object[] patchDocument = GetPatchDocument(fields);
            var patchValue = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

            try
            {
                string result = MakeRequest(HostName + "/" + project + "/_apis/wit/workitems/$" + type + "?api-version=2.2", ApiMethod.Patch, patchValue.ReadAsStringAsync().Result);
                workItem = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkItem>(result);
            }
            catch (Exception ex)
            {
                //var contents = response.Content.ReadAsStringAsync().Result;
                //TFSErrorsResponse tfsError = Newtonsoft.Json.JsonConvert.DeserializeObject<TFSErrorsResponse>(contents);
                //throw new TFSClientException(tfsError);
            }

            return workItem;
        }

		public int CreateComment(int workItemId, string comment)
		{
			int commentId = 0;
			List<WorkItemField> field = new List<WorkItemField>()
			{
				new WorkItemField { referenceName = "System.History", value = comment }
			};

			Object[] patchDocument = GetPatchDocument(field);

            var patchValue = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

            try
            {
                string result = MakeRequest(HostName + "/_apis/wit/workitems/" + workItemId + "?api-version=2.2", ApiMethod.Patch, patchValue.ReadAsStringAsync().Result);
                WorkItem workItem = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkItem>(result);
                commentId = (int)workItem.Rev;
            }
            catch (Exception ex)
            {
                //var contents = response.Content.ReadAsStringAsync().Result;
                //TFSErrorsResponse tfsError = Newtonsoft.Json.JsonConvert.DeserializeObject<TFSErrorsResponse>(contents);
                //throw new TFSClientException(tfsError);
            }

			return commentId;
		}

		public void CreateTeamSupportHyperlink(int workItemId, string remoteLink, string comment)
		{
			Object[] patchDocument = GetPatchDocumentForRelations(RelationsType.Hyperlink, remoteLink, comment);

            var patchValue = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

            try
            {
                string result = MakeRequest(HostName + "/_apis/wit/workitems/" + workItemId + "?api-version=2.2", ApiMethod.Patch, patchValue.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                //var contents = response.Content.ReadAsStringAsync().Result;
                //TFSErrorsResponse tfsError = Newtonsoft.Json.JsonConvert.DeserializeObject<TFSErrorsResponse>(contents);
                //throw new TFSClientException(tfsError);
            }
		}

		public WorkItem UpdateWorkItem(int workItemId, List<WorkItemField> fields)
		{
			WorkItem workItem = new WorkItem();
			Object[] patchDocument = GetPatchDocument(fields);

            var patchValue = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

            try
            {
                string result = MakeRequest(HostName + "/_apis/wit/workitems/" + workItemId.ToString() + "?api-version=2.2", ApiMethod.Patch, patchValue.ReadAsStringAsync().Result);
                workItem = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkItem>(result);
            }
            catch (Exception ex)
            {
                //var contents = response.Content.ReadAsStringAsync().Result;
                //TFSErrorsResponse tfsError = Newtonsoft.Json.JsonConvert.DeserializeObject<TFSErrorsResponse>(contents);
                //throw new TFSClientException(tfsError);
            }

			return workItem;
		}

		public void DeleteTeamSupportHyperlink(int workItemId, int ticketId)
		{
			WorkItem workItem = new WorkItem();
			workItem = GetWorkItemBy(workItemId, expandAll: true);

			//Find the position of the TeamSupport hyperlink
			for (int i =0; i < workItem.Relations.Count(); i++)
			{
				if (string.Compare(workItem.Relations[i].Rel, RelationsType.Hyperlink.ToString(), ignoreCase: true) == 0
					&& workItem.Relations[i].Url.Contains("teamsupport.com")
					&& workItem.Relations[i].Url.Contains(string.Format("ticketid={0}", ticketId.ToString())))
				{
					Object[] patchDocument = GetPatchDocumentForRelationsDelete(RelationsType.Hyperlink, i);

                    var patchValue = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                    try
                    {
                        string result = MakeRequest(HostName + "/_apis/wit/workitems/" + workItemId + "?api-version=2.2", ApiMethod.Patch, patchValue.ReadAsStringAsync().Result);
                    }
                    catch (Exception ex)
                    {
                        //var contents = response.Content.ReadAsStringAsync().Result;
                        //TFSErrorsResponse tfsError = Newtonsoft.Json.JsonConvert.DeserializeObject<TFSErrorsResponse>(contents);
                        //throw new TFSClientException(tfsError);
                    }
				}
			}
		}

		public bool UploadAttachment(int workItemId, string filePath, string fileName)
		{
			bool result = false;
			string URI = HostName + "/DefaultCollection/_apis/wit/attachments?fileName=" + fileName + "&api-version=2.2"; //Use correct values here
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URI);
			string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
			request.Headers.Add("Authorization", "Basic " + EncodedCredentials);
			request.Method = "POST";
			//vv request.ContentType = "application/octet-stream; boundary=" + boundary;
			request.ContentType = "application/octet-stream;";//vv
			request.Accept = "text/html, application/xhtml+xml, */*";
			request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");

			MemoryStream stream = new MemoryStream();
			Stream postDataStream = GetPostStream(filePath, boundary);
			request.ContentLength = postDataStream.Length;
			Stream reqStream = request.GetRequestStream();
			postDataStream.Position = 0;

			byte[] buffer = new byte[1024];
			int bytesRead = 0;

			while ((bytesRead = postDataStream.Read(buffer, 0, buffer.Length)) != 0)
			{
				reqStream.Write(buffer, 0, bytesRead);
			}

			postDataStream.Close();
			reqStream.Close();

			StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream());
			string streamResult = sr.ReadToEnd();

			if (!string.IsNullOrEmpty(streamResult))
			{
				AttachmentInfo attachmentinfo = Newtonsoft.Json.JsonConvert.DeserializeObject<AttachmentInfo>(streamResult);
				Object[] patchDocument = GetPatchDocumentForRelations(RelationsType.AttachedFile, attachmentinfo.url, fileName);

                var patchValue = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                try
                {
                    string resultContent = MakeRequest(HostName + "/_apis/wit/workitems/" + workItemId + "?api-version=2.2", ApiMethod.Patch, patchValue.ReadAsStringAsync().Result);
                    result = true;
                }
                catch (Exception ex)
                {
                    //var contents = response.Content.ReadAsStringAsync().Result;
                    //TFSErrorsResponse tfsError = Newtonsoft.Json.JsonConvert.DeserializeObject<TFSErrorsResponse>(contents);
                    //throw new TFSClientException(tfsError);
                }
			}

			return result;
		}

		private void GetWorkItemsFields()
        {
            List<WorkItemField> resultList = new List<WorkItemField>();

            try
            {
                string response = MakeRequest(string.Format("{0}/_apis/wit/fields?api-version=2.2", HostName), ApiMethod.Get);
                WorkItemFields workItemFields = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkItemFields>(response);
                resultList = new List<WorkItemField>(workItemFields.value);
            }
            catch (Exception ex)
            {
                //var contents = response.Content.ReadAsStringAsync().Result;
                //TFSErrorsResponse tfsError = Newtonsoft.Json.JsonConvert.DeserializeObject<TFSErrorsResponse>(contents);
                //throw new TFSClientException(tfsError);
            }

            _workItemFields = resultList;
        }

		private Object[] GetPatchDocument(List<WorkItemField> fields)
		{
			int i = 0;
			Object[] patchDocument = new Object[fields.Count];
			string path = "fields";

			foreach (WorkItemField field in fields)
			{
				patchDocument[i] = new
				{
					op = field.operation.ToString(),
					path = string.Format("/{0}/", path) + field.referenceName,
					value = field.value
				};

				i++;
			}

			return patchDocument;
		}

		private Object[] GetPatchDocumentForRelations(RelationsType type, string url, string comment, Operation operation = Operation.add, int linkPosition = 0)
		{
			int i = 0;
			Object[] patchDocument = new Object[1];
			string path = "relations";

			switch (operation)
			{
				case Operation.add:
					object attributes =
					new
					{
						comment = comment
					};

					patchDocument[0] = new
					{
						op = operation.ToString(),
						path = string.Format("/{0}/-", path),
						value = new
						{
							rel = type.ToString(),
							url = url,
							attributes = attributes
						}
					};
					break;
				case Operation.remove:
					patchDocument[0] = new
					{
						op = operation.ToString(),
						path = string.Format("/{0}/{1}", path, linkPosition)
					};
					break;
				default:
					break;
			}

			return patchDocument;
		}

		private Object[] GetPatchDocumentForRelationsDelete(RelationsType type, int linkPosition)
		{
			return GetPatchDocumentForRelations(type, null, null, Operation.remove, linkPosition);
		}

		private static Stream GetPostStream(string filePath, string boundary)
		{
			Stream postDataStream = new MemoryStream();
			FileInfo fileInfo = new FileInfo(filePath);
			//string fileHeaderTemplate = "--" + boundary + Environment.NewLine +
			//"Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" +
			//Environment.NewLine + "Content-Type: application/octet-stream" + Environment.NewLine + Environment.NewLine;
			//byte[] fileHeaderBytes = Encoding.UTF8.GetBytes(string.Format(fileHeaderTemplate, "UploadFile", fileInfo.FullName));
			//postDataStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);

			FileStream fileStream = fileInfo.OpenRead();
			byte[] buffer = new byte[1024];
			int bytesRead = 0;

			while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
			{
				postDataStream.Write(buffer, 0, bytesRead);
			}

			fileStream.Close();

			//byte[] endBoundaryBytes = Encoding.UTF8.GetBytes(Environment.NewLine + "--" + boundary);
			//postDataStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);

			return postDataStream;
		}

        [Serializable]
		public class WorkItems
		{
			public int count { get; set; }
			public WorkItem[] value { get; set; }
		}

		public class WorkItemFields
        {
            public int count { get; set; }
            public WorkItemField[] value { get; set; }
        }

        public class WorkItemField
        {
            public string name { get; set; }
            public string referenceName { get; set; }
            public string type { get; set; }
            public bool readOnly { get; set; }
            public string url { get; set; }
            public string value { get; set; }
			public Operation operation { get; set; }

			public WorkItemField()
			{
				operation = Operation.add;
			}
		}

        [DataContract]
        public class WorkItemCommentList
        {
            public WorkItemCommentList() { }

            [DataMember]
            public IEnumerable<WorkItemComment> Comments { get; set; }
            [DataMember]
            public int Count { get; set; }
            [DataMember]
            public int FromRevisionCount { get; set; }
            [DataMember]
            public int TotalCount { get; set; }
        }

        private class AttachmentInfo
		{
			public string id { get; set; }
			public string url { get; set; }
		}

		[Serializable]
		public class TFSClientException : Exception
		{
			private readonly string response;
			public TFSClientException() { }
			public TFSClientException(string message) : base(message) { }
			public TFSClientException(string message, string response) : base(message) { this.response = response; }
			public TFSClientException(string message, Exception inner) : base(message, inner) { }
			public TFSClientException(TFSErrorsResponse tfsError)
			{
				ErrorResponse = tfsError;
			}

			public TFSErrorsResponse ErrorResponse { get; private set; }
		}

		public class TFSErrorsResponse
		{
			[Newtonsoft.Json.JsonProperty(PropertyName = "$id")]
			public string id { get; set; }
			public object innerException { get; set; }
			public string message { get; set; }
			public string typeName { get; set; }
			public string typeKey { get; set; }
			public int errorCode { get; set; }
			public int eventId { get; set; }
		}

	//To get the other relation types do a GET to: https://{url}/DefaultCollection/_apis/wit/workitemrelationtypes?api-version=2.2
	//For now we are only using these two.
	    private enum RelationsType : byte
		{
			Unknown = 0,
			Hyperlink = 1,
			AttachedFile = 2
		}

		public enum Operation : byte
		{
			Unknown = 0,
			add = 1,
			replace = 2,
			remove = 3,
			test = 4
		}

        public enum ApiMethod : byte
        {
            Unsupported = 0,
            Get = 1,
            Put = 2,
            Post = 3,
            Delete = 4,
            Patch = 5
        }

        #region Properties
        public string HostName
        {
            get
            {
                return _hostname.TrimEnd('/');
            }
            set
            {
                _hostname = value;
            }
        }

        public string AccessToken
        {
            get
            {
                return _accessToken;
            }
            set
            {
                _accessToken = value;
            }
        }

        public string UserName
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        private string EncodedCredentials
        {
            get
            {
                if (!string.IsNullOrEmpty(_accessToken))
                {
                    return Data.DataUtils.GetEncodedCredentials(string.Empty, AccessToken);
                }
                else
                {
                    return Data.DataUtils.GetEncodedCredentials(UserName, Password);
                }
            }
        }

        public List<WorkItemField> WorkItemsFields
        {
            get
            {
                if (_workItemFields == null || !_workItemFields.Any())
                {
                    GetWorkItemsFields();
                }
                return _workItemFields;
            }
        }
        #endregion
    }
}
