Imports Newtonsoft.Json.Linq
Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Reflection
Imports System.Text
Imports TeamSupport.Data
Imports Newtonsoft.Json
Imports TeamSupport.JIRA

Namespace TeamSupport
  Namespace CrmIntegration
	Public Class Jira
      Inherits Integration

		Private _baseURI As String
		Private _encodedCredentials As String
		Private _issueTypeFieldsList As Dictionary(Of IssueTypeFields, JObject) = New Dictionary(Of IssueTypeFields, JObject)
		Private _jiraExceptionMessageFormat As String = "Jira InnerException Message: {0}{1}{2}{2}{2}Jira ErrorResponse: {3}"

		Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor)
			MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, IntegrationType.Jira)
		End Sub

		Public Overrides Function PerformSync() As Boolean
                Dim Success As Boolean = True
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls

                If ValidateSyncData() Then
				Success = SyncTickets()
			Else
				Success = False
			End If

			Return Success
		End Function

		Private Function ValidateSyncData() As Boolean
			Dim result As Boolean = True
			_issueTypeFieldsList =  New Dictionary(Of IssueTypeFields, JObject)

			If CRMLinkRow.HostName Is Nothing Then
				result = False
				AddLog("HostName is missing and it is required to sync.")
			Else
				Dim protocol As String = String.Empty
				If CRMLinkRow.HostName.Length > 4 AndAlso CRMLinkRow.HostName.Substring(0, 4).ToLower() <> "http" Then
					protocol = "https://"
				End If
				_baseURI = protocol + CRMLinkRow.HostName + "/rest/api/latest"
			End If

			If CRMLinkRow.Username Is Nothing Then
				result = False
				AddLog("Username is missing and it is required to sync.")
			ElseIf CRMLinkRow.Password Is Nothing AndAlso CRMLinkRow.SecurityToken Is Nothing Then
				result = False
				AddLog("Token and password are both missing, at least one is required to sync.")
			Else
				_encodedCredentials = DataUtils.GetEncodedCredentials(CRMLinkRow.Username, If(String.IsNullOrEmpty(CRMLinkRow.SecurityToken), CRMLinkRow.Password, CRMLinkRow.SecurityToken))
            End If

			'Make sure credentials are good
			If (result) Then
				Try
                        Dim jiraClient As JiraClient = New JiraClient(_baseURI.Replace("/rest/api/latest", ""), CRMLinkRow.Username, If(String.IsNullOrEmpty(CRMLinkRow.SecurityToken), CRMLinkRow.Password, CRMLinkRow.SecurityToken))
                        Dim serverInfo As ServerInfo = jiraClient.GetServerInfo()
					AddLog("Jira credentials ok.")
				Catch jiraEx As JiraClientException
					result = False
					_exception = New IntegrationException(jiraEx.InnerException.Message, jiraEx)
				End Try
			End If

			Return result
		End Function

		Private Function SyncTickets() As Boolean
			Dim numberOfIssuesToPullAsTickets As Integer = 0
			Dim ticketLinkToJira As TicketLinkToJira = New TicketLinkToJira(User)
			Dim issuesToPullAsTickets As List(Of JObject) = New List(Of JObject)

			ticketLinkToJira.LoadByCrmLinkId(CRMLinkRow.CRMLinkID, True)

			If ticketLinkToJira.Any AndAlso ticketLinkToJira.Count > 0 Then
				Try
					issuesToPullAsTickets = GetIssuesToPullAsTickets(ticketLinkToJira, numberOfIssuesToPullAsTickets)
				Catch ex As Exception
					AddLog("GetIssuesToPullAsTickets with POST failed, using old version now.")
					issuesToPullAsTickets = New List(Of JObject)
					numberOfIssuesToPullAsTickets = 0
					Try
						issuesToPullAsTickets = GetIssuesToPullAsTickets(numberOfIssuesToPullAsTickets)
					Catch exception As Exception
						Log.Write(exception.Message)
						Log.Write(exception.StackTrace)
						_exception = New IntegrationException(exception.Message, exception)
						Return False
					End Try
				End Try
			End If

			Dim ticketsLinksToJiraToPushAsIssues As TicketLinkToJira = Nothing
			Dim ticketsToPushAsIssues As TicketsView = GetTicketsToPushAsIssues(ticketsLinksToJiraToPushAsIssues)
			Dim allStatuses As TicketStatuses = New TicketStatuses(User)
			Dim newActionsTypeID As Integer = 0

			If ticketsToPushAsIssues.Count > 0 OrElse numberOfIssuesToPullAsTickets > 0 Then
				allStatuses.LoadByOrganizationID(CRMLinkRow.OrganizationID)
				newActionsTypeID = GetNewActionsTypeID(CRMLinkRow.OrganizationID)
			End If
			
                Dim ticketTypeMappings As New CRMLinkFields(User)
                ticketTypeMappings.LoadByObjectType("TicketType", CRMLinkRow.CRMLinkID)

			If ticketsToPushAsIssues.Count > 0 Then
                    PushTicketsAndActionsAsIssuesAndComments(ticketsToPushAsIssues, ticketsLinksToJiraToPushAsIssues, allStatuses, newActionsTypeID, ticketTypeMappings)
			End If

			If numberOfIssuesToPullAsTickets > 0 Then
				For Each batchOfIssuesToPullAsTicket As JObject In issuesToPullAsTickets
                        PullIssuesAndCommentsAsTicketsAndActions(batchOfIssuesToPullAsTicket("issues"), allStatuses, newActionsTypeID, ticketTypeMappings)
				Next
			End If

			Return Not SyncError
		End Function

      Private Function GetTicketsToPushAsIssues(ByRef ticketsLinksToJiraToPushAsIssues As TicketLinkToJira) As TicketsView
        Dim result As New TicketsView(User)
        result.LoadToPushToJira(CRMLinkRow)
        AddLog("Got " + result.Count.ToString() + " Tickets To Push As Issues.")

        ticketsLinksToJiraToPushAsIssues = New TicketLinkToJira(User)
        ticketsLinksToJiraToPushAsIssues.LoadToPushToJira(CRMLinkRow)

        Return result
      End Function

		''' <summary>
	  ''' Search by POSTing the query
	  ''' </summary>
	  ''' <param name="jiraIdList">List of the jira ids to search. These are the ones we know have been linked, already in TeamSupport in the TicketLinkToJira table.</param>
	  ''' <param name="numberOfIssuesToPull">variable that will have the count of the issues found.</param>
	  ''' <returns>A list of JObject with the issues found based on the query.</returns>
		Private Function GetIssuesToPullAsTickets(ByRef ticketLinkToJira As TicketLinkToJira, ByRef numberOfIssuesToPull As Integer) As List(Of JObject)
			Dim jiraIdList As List(Of Integer) = ticketLinkToJira.Where(Function(w) w.CrmLinkID IsNot Nothing).Select(Function(p) CType(p.JiraID, Integer)).ToList()
			Dim result As List(Of JObject) = New List(Of JObject)
			Dim recentClause As String = String.Empty

			If CRMLinkRow.LastLink IsNot Nothing Then
				recentClause = "updated>-" + GetMinutesSinceLastLink().ToString() + "m"
			Else
				Dim minutesSinceFirstSyncedTicket As Integer = GetMinutesSinceFirstSyncedTicket()

				If minutesSinceFirstSyncedTicket > 0 Then
					recentClause = "updated>-" + minutesSinceFirstSyncedTicket.ToString() + "m"
				Else
					Log.Write("No tickets have been synced, therefore no issues to pull exist.")
					Return result
				End If
			End If

			'Search only for the jira ids we have, those are the ones linked and the only ones that we need to check for updates
			Dim URI As String = _baseURI + "/search"
			Dim jiraIdClause As String = String.Empty

			If (jiraIdList.Any() AndAlso jiraIdList.Count > 0) Then
				jiraIdClause = String.Join(",", jiraIdList.ToArray())
			End If

			If (Not String.IsNullOrEmpty(jiraIdClause)) Then
				Dim needToGetMore As Boolean = True
				Dim maxResults As Integer? = Nothing
				Dim body As StringBuilder = New StringBuilder()
				Dim startAt As Integer = 0
				Dim batch As JObject = New JObject
				Dim listWasScrubbed As Boolean = False

				While needToGetMore
					listWasScrubbed = False
					body.Append("{")
					body.Append(String.Format("""jql"": ""id IN ({0}) AND {1} ORDER BY updated asc"",", jiraIdClause, recentClause))
					body.Append("""fields"": [""*all""],")
					body.Append(String.Format("""startAt"": {0}", startAt))
					body.Append("}")

					Try
						batch = GetAPIJObject(URI, "POST", body.ToString())
					Catch webEx As WebException
						Dim jiraErrors As JiraErrorsResponse = JiraErrorsResponse.Get(webEx)

						If (jiraErrors IsNot Nothing AndAlso jiraErrors.HasErrors) Then
							AddLog("Error when searching issues, scrubbing.")
							ScrubIssues(jiraIdList, jiraErrors, ticketLinkToJira)

							If (jiraIdList IsNot Nothing AndAlso jiraIdList.Count > 0 AndAlso jiraIdList.Any()) Then
								jiraIdClause = String.Join(",", jiraIdList.ToArray())
								startAt = 0
								body.Clear()
								batch = New JObject()
								listWasScrubbed = True
							Else
								Exit While
							End If
						Else
							numberOfIssuesToPull = 0
							Throw webEx
						End If
					End Try

					If (Not listWasScrubbed) Then
						result.Add(batch)
						body.Clear()
						Dim batchTotal As Integer = batch("issues").Count
						numberOfIssuesToPull += batchTotal

						If maxResults Is Nothing Then
							maxResults = CType(batch("maxResults").ToString(), Integer?)
						End If

						If batchTotal = maxResults Then
							startAt = numberOfIssuesToPull
						Else
							needToGetMore = False
						End If
					End If
				End While
			End If

			Log.Write("Got " + numberOfIssuesToPull.ToString() + " Issues To Pull As Tickets.")
			Return result
		End Function

		''' <summary>
		''' Removes the non-existing issues in Jira from the list we have in the TicketLinkToJira table
		''' </summary>
		''' <param name="jiraIdList">A reference to the list we got from TicketLinkToJira</param>
		''' <param name="url">Organization's Jira url</param>
		''' <param name="recentClause">Recent clause. To narrow the results.</param>
		Private Sub ScrubIssues(ByRef jiraIdList As List(Of Integer), ByVal errorsList As JiraErrorsResponse, ByRef ticketLinkToJira As TicketLinkToJira)
			Dim nonExistingIssuesList As List(Of Integer) = New List(Of Integer)
			Dim query As StringBuilder = New StringBuilder()

			'As of 03/12/16 the format of the error message returned by Jira when the issueId does not exist is: "A value with ID '## Id Here ##' does not exist for the field 'id'."				
			For Each issueId As Integer In jiraIdList
				If (errorsList.ErrorMessages.Where(Function(p) p.Contains("'" + issueId.ToString() + "'")).Any()) Then
					nonExistingIssuesList.Add(issueId)

					Dim ticketLinkToJiraId As Integer = ticketLinkToJira.Where(Function(p) p.JiraID = issueId AndAlso p.CrmLinkID = CRMLinkRow.CRMLinkID).Select(Function(p) p.id).FirstOrDefault()
					Dim ticketLinkToJiraItem As TicketLinkToJiraItem = ticketLinkToJira.FindByid(ticketLinkToJiraId)
					ticketLinkToJiraItem.JiraID = Nothing
					ticketLinkToJiraItem.DateModifiedByJiraSync = Date.UtcNow
					ticketLinkToJiraItem.SyncWithJira = 0
					ticketLinkToJiraItem.JiraStatus = "DoesNotExist. IssueId: " + issueId.ToString()
					ticketLinkToJiraItem.Collection.Save()
				End If
			Next

			If (nonExistingIssuesList.Any() AndAlso nonExistingIssuesList.Count > 0) Then
				AddLog(String.Format("IssueIds not found in Jira that will be excluded on sync: {0}", String.Join(",", nonExistingIssuesList.ToArray())))
				jiraIdList = jiraIdList.Except(nonExistingIssuesList).ToList()
			End If
		End Sub

		'Private Sub ScrubIssues(ByRef jiraIdList As List(Of Integer), ByVal url As String, ByVal recentClause As String, ByRef ticketLinkToJira As TicketLinkToJira)
		'	Dim nonExistingIssuesList As List(Of Integer) = New List(Of Integer)
		'	Dim query As StringBuilder = New StringBuilder()

		'	'If we get here then one or more issues don't exist in Jira. We'll have to go one by one! and NULL their jiraId here
		'	For Each issueId As Integer In jiraIdList
		'		query.Clear()
		'		query.Append(_baseURI)
		'		query.Append("/issue/")
		'		query.Append(issueId.ToString())
		'		query.Append("?fields=id")
					
		'		Try
		'			Dim result As JObject = GetAPIJObject(query.ToString(), "GET", String.Empty, False)
		'		Catch ex2 As Exception
		'			nonExistingIssuesList.Add(issueId)
		'			Dim ticketLinkToJiraId As Integer = ticketLinkToJira.Where(Function(p) p.JiraID = issueId AndAlso p.CrmLinkID = CRMLinkRow.CRMLinkID).Select(Function(p) p.id).FirstOrDefault()
		'			Dim ticketLinkToJiraItem As TicketLinkToJiraItem = ticketLinkToJira.FindByid(ticketLinkToJiraId)
		'			ticketLinkToJiraItem.JiraID = Nothing
		'			ticketLinkToJiraItem.DateModifiedByJiraSync = Date.UtcNow
		'			ticketLinkToJiraItem.SyncWithJira = 0
		'			ticketLinkToJiraItem.JiraStatus = "DoesNotExist. IssueId: " + issueId.ToString()
		'			ticketLinkToJiraItem.Collection.Save()
		'		End Try
		'	Next

		'	If (nonExistingIssuesList.Any() AndAlso nonExistingIssuesList.Count > 0) Then
		'		Log.Write(String.Format("IssueIds not found in Jira that will be excluded on sync: {0}", String.Join(",", nonExistingIssuesList.ToArray())))
		'		jiraIdList = jiraIdList.Except(nonExistingIssuesList).ToList()
		'	End If
		'End Sub

        Private Function GetMinutesSinceLastLink() As Integer
          Dim datesDifference As TimeSpan = DateTime.UtcNow.Subtract(CRMLinkRow.LastLinkUtc)
          Return datesDifference.TotalMinutes + 30
        End Function

        Private Function GetMinutesSinceFirstSyncedTicket() As Integer
          Dim firstSyncedTicket As Tickets = New Tickets(MyBase.User)
          firstSyncedTicket.LoadFirstJiraSynced(CRMLinkRow.OrganizationID)
          Dim result As Integer = 0
          If firstSyncedTicket.Count > 0 Then
            Dim datesDifference As TimeSpan = DateTime.UtcNow.Subtract(firstSyncedTicket(0).DateCreatedUtc)
            result = datesDifference.TotalMinutes + 30
          End If
          Return result
        End Function

		Private Function GetAPIJObject(ByVal URI As String, ByVal verb As String, ByVal body As String) As JObject
			AddLog(String.Format("{0} URI: {1}", verb, URI))
			
			If verb <> "GET" AndAlso Not String.IsNullOrEmpty(body) Then
				AddLog("body: " + body)
			End If

			Dim result As JObject
			
			Using response As HttpWebResponse = MakeHTTPRequest(_encodedCredentials, URI, verb, "application/json", Client, body)
				Dim responseReader As New StreamReader(response.GetResponseStream())
				result = JObject.Parse(responseReader.ReadToEnd)
				responseReader.Close()
				response.Close()

				AddLog("responseReader and response closed. Exiting Using.")
			End Using

			Return result
		End Function

        Private Function GetAPIJArray(ByVal URI As String, ByVal verb As String, ByVal body As String) As JArray
		  Log.Write(String.Format("{0} URI: {1}", verb, URI))

			If verb <> "GET" AndAlso Not String.IsNullOrEmpty(body) Then
				Log.Write("body: " + body)
			End If

			Dim result As JArray

			Using response As HttpWebResponse = MakeHTTPRequest(_encodedCredentials, URI, verb, "application/json", Client, body)
				Dim responseReader As New StreamReader(response.GetResponseStream())
				result = JArray.Parse(responseReader.ReadToEnd)
				response.Close()
			End Using
          
			Return result
		End Function

	Private Function MakeHTTPRequest(
        ByVal encodedCredentials As String,
        ByVal URI As String,
        ByVal method As String,
        ByVal contentType As String,
        ByVal userAgent As String,
        ByVal body As String) As HttpWebResponse

        Dim request As HttpWebRequest = WebRequest.Create(URI)
        request.Headers.Add("Authorization", "Basic " + encodedCredentials)
        request.Method = method
        request.ContentType = contentType
		request.UserAgent = userAgent

                If CRMLinkRow.OrganizationID = 869700 OrElse CRMLinkRow.OrganizationID = 794765 OrElse CRMLinkRow.OrganizationID = 881342 Then
                    request.Timeout = 600000
                ElseIf CRMLinkRow.OrganizationID = 1081853 Then
                    request.Timeout = 180000
                Else
                    request.Timeout = 120000
                End If

                If method.ToUpper = "POST" OrElse method.ToUpper = "PUT" Then
                    Dim bodyByteArray = UTF8Encoding.UTF8.GetBytes(body)
                    request.ContentLength = bodyByteArray.Length

                    Using requestStream As Stream = request.GetRequestStream()
                        requestStream.Write(bodyByteArray, 0, bodyByteArray.Length)
                        requestStream.Close()
                        Log.Write("requestStream closed. Exiting Using.")
                    End Using
                End If

                Return request.GetResponse()
            End Function

            Private Sub PushTicketsAndActionsAsIssuesAndComments(ByVal ticketsToPushAsCases As TicketsView,
                                                            ByVal ticketsLinksToJiraToPushAsIssues As TicketLinkToJira,
                                                            ByVal allStatuses As TicketStatuses,
                                                            ByVal newActionsTypeID As Integer,
                                                            ByRef ticketTypeMappings As CRMLinkFields)
                Dim URI As String = _baseURI + "/issue"
                Dim attachmentFileSizeLimit As Integer = 0
                Dim attachmentEnabled As Boolean = GetAttachmentEnabled(attachmentFileSizeLimit)
                Dim crmLinkError As CRMLinkError = Nothing
                Dim ticketData As StringBuilder = Nothing
                Dim jiraProjectKey As String = String.Empty
                Dim customMappingFields As New CRMLinkFields(User)
                Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(User)
                Dim crmLinkAttachmentErrors As CRMLinkErrors = New CRMLinkErrors(User)

                'Get the errors only for the tickets to be processed
                crmLinkErrors.LoadByOperationAndObjectIds(CRMLinkRow.OrganizationID,
                                                        CRMLinkRow.CRMType,
                                                        GetDescription(Orientation.OutToJira),
                                                        GetDescription(ObjectType.Ticket),
                                                        ticketsToPushAsCases.Select(Function(p) p.TicketID.ToString()).ToList(),
                                                        isCleared:=False)

                For Each ticket As TicketsViewItem In ticketsToPushAsCases
                    Dim ticketLinkToJira As TicketLinkToJiraItem = ticketsLinksToJiraToPushAsIssues.FindByTicketID(ticket.TicketID)
                    Dim ticketLinkToJiraVerify As TicketLinkToJira = New TicketLinkToJira(User)
                    ticketLinkToJiraVerify.LoadByTicketID(ticket.TicketID)

                    If (ticketLinkToJiraVerify.Count = 0) Then
                        AddLog("The ticket link record has been deleted. Link was removed or cancelled. Doing nothing.")
                        Continue For
                    End If

                    AddLog(String.Format("Processing ticket #{0}. TicketId: {1}", ticket.TicketNumber, ticket.TicketID))
                    customMappingFields = New CRMLinkFields(User)
                    customMappingFields.LoadByObjectTypeAndCustomFieldAuxID(GetDescription(ObjectType.Ticket), CRMLinkRow.CRMLinkID, ticket.TicketTypeID)

                    Dim updateTicketFlag As Boolean = False
                    Dim sendCustomMappingFields As Boolean = False
                    Dim issueFields As JObject = Nothing
                    Dim ticketTypeName As String = ticket.TicketTypeName
                    Dim mappedTicketType As CRMLinkField = ticketTypeMappings.FindByTSFieldName(ticket.TicketTypeName)
                    If mappedTicketType IsNot Nothing Then
                        ticketTypeName = mappedTicketType.CRMFieldName
                    End If

                    Try
                        crmLinkError = crmLinkErrors.FindUnclearedByObjectIDAndFieldName(ticket.TicketID, String.Empty)
                        jiraProjectKey = GetProjectKey(ticket, crmLinkErrors)
                        issueFields = GetIssueFields(ticket, jiraProjectKey, crmLinkError, Orientation.OutToJira, ticketTypeName)
                    Catch webEx As WebException
                        Dim jiraErrors As JiraErrorsResponse = JiraErrorsResponse.Get(webEx)

                        If (jiraErrors IsNot Nothing AndAlso jiraErrors.HasErrors) Then
                            AddLog(String.Format(_jiraExceptionMessageFormat,
                                                    webEx.Message,
                                                    Environment.NewLine,
                                                    vbTab,
                                                    jiraErrors.ToString()))
                            AddLog(webEx.StackTrace,
                                    LogType.Report,
                                    crmLinkError,
                                    String.Format("Error attempting to get Jira project '{0}' information:{1}{2}.", jiraProjectKey, Environment.NewLine, jiraErrors.ToString()),
                                    Orientation.OutToJira,
                                    ObjectType.Ticket,
                                    ticket.TicketID,
                                    String.Empty,
                                    Nothing,
                                    OperationType.Unknown)
                        Else
                            AddLog(webEx.ToString() + webEx.StackTrace)
                        End If

                        Continue For
                    Catch ex As Exception
                        AddLog(String.Format("Exception in PushTicketsAndActionsAsIssuesAndComments: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace))
                        Continue For
                    End Try

                    Dim issue As JObject = Nothing
                    Dim isNew As Boolean = False
                    'Create new issue
                    If ticketLinkToJira.JiraKey Is Nothing OrElse ticketLinkToJira.JiraKey.IndexOf("Error") > -1 Then
                        Try
                            crmLinkError = crmLinkErrors.FindUnclearedByObjectIDAndFieldName(ticket.TicketID, String.Empty)
                            AddLog("No JiraKey. Creating issue...")
                            URI = _baseURI + "/issue"
                            isNew = True

                            Dim actionDescriptionId As Integer
                            ticketData = New StringBuilder()
                            ticketData.Append(GetTicketData(ticket, issueFields, jiraProjectKey, actionDescriptionId, customMappingFields, crmLinkErrors, ticketTypeName))
                            issue = GetAPIJObject(URI, "POST", ticketData.ToString())
                            'The create issue response does not include status and we need it to initialize the synched ticket. So, we do a GET on the recently created issue.
                            URI = _baseURI + "/issue/" + issue("key").ToString()
                            issue = GetAPIJObject(URI, "GET", String.Empty)
                            updateTicketFlag = True
                            sendCustomMappingFields = CRMLinkRow.IncludeIssueNonRequired

                            'Check if Ticket Description Action has Attachment
                            If (attachmentEnabled AndAlso actionDescriptionId > 0) Then
                                Dim actionDescriptionAttachment As Data.AttachmentProxy = TeamSupport.Data.Quarantine.ServiceQ.GetAttachmentProxy(User, actionDescriptionId)
                                'The Action Description should always be 1, if for any reason this is not the case call: Actions.GetActionPosition(User, actionDescriptionId)
                                Dim actionPosition As Integer = 1
                                PushAttachments(actionDescriptionId, ticket.TicketNumber, issue, issue("key"), attachmentFileSizeLimit, actionPosition)
                            End If

                            ClearCrmLinkError(crmLinkError)
                        Catch webEx As WebException
                            Dim jiraErrors As JiraErrorsResponse = JiraErrorsResponse.Get(webEx)

                            If (jiraErrors IsNot Nothing AndAlso jiraErrors.HasErrors) Then
                                AddLog(String.Format(_jiraExceptionMessageFormat,
                                                    webEx.Message,
                                                    Environment.NewLine,
                                                    vbTab,
                                                    jiraErrors.ToString()))
                                AddLog(webEx.StackTrace,
                                        LogType.Report,
                                        crmLinkError,
                                        String.Format("Jira Issue was not created due to:{0}{1}", Environment.NewLine, jiraErrors.ToString()),
                                        Orientation.OutToJira,
                                        ObjectType.Ticket,
                                        ticket.TicketID,
                                        String.Empty,
                                        ticketData.ToString(),
                                        OperationType.Create)
                            Else
                                AddLog(webEx.ToString() + webEx.StackTrace)
                            End If

                            Continue For
                        Catch ex As Exception
                            Dim updateLinkToJira As Boolean = True
                            Dim errorMessage As String = String.Empty

                            Select Case ex.Message
                                Case "no project"
                                    errorMessage = "Error: Specify Project (Product)."
                                Case "type mismatch"
                                    errorMessage = "Error: Specify valid Type."
                                Case "project mismatch"
                                    errorMessage = "Error: Specify valid Project (Product)."
                                Case Else
                                    errorMessage = ex.Message
                                    updateLinkToJira = False
                            End Select

                            If updateLinkToJira Then
                                ticketLinkToJira.JiraKey = errorMessage
                                ticketLinkToJira.DateModifiedByJiraSync = DateTime.UtcNow()
                                ticketLinkToJira.Collection.Save()
                            End If

                            AddLog(ex.ToString() + ex.StackTrace,
                                LogType.TextAndReport,
                                crmLinkError,
                                errorMessage,
                                Orientation.OutToJira,
                                ObjectType.Ticket,
                                ticket.TicketID.ToString(),
                                "create",
                                ticketData.ToString(),
                                OperationType.Create)
                            Continue For
                        End Try

                        'Issue already exists. 
                        'We are not updating issues, but if this is a second ticket relating to issue we add a remote link and update ticket fields for Jira
                    ElseIf ticketLinkToJira.JiraID Is Nothing Then
                        crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID, "update")

                        Try
                            URI = _baseURI + "/issue/" + ticketLinkToJira.JiraKey
                            issue = GetAPIJObject(URI, "GET", String.Empty)
                            updateTicketFlag = True
                            Log.Write("No JiraID. We'll add link.")

                            'An update error could has been caused here or below.
                            'We'll only clear when successfull below.
                        Catch webEx As WebException
                            Dim invalidJiraKey As String = String.Empty

							If ticketLinkToJira.JiraKey.ToLower().Contains("http") _
							Or ticketLinkToJira.JiraKey.ToLower().Contains(":") _
							Or ticketLinkToJira.JiraKey.ToLower().Contains("/") _
							Or ticketLinkToJira.JiraKey.ToLower().Contains("[") _
							Or ticketLinkToJira.JiraKey.ToLower().Contains("]") Then
								invalidJiraKey = "The JiraKey entered is not valid or does not exist in Jira."
							End If

							If (String.IsNullOrEmpty(invalidJiraKey)) Then
                                Dim jiraErrors As JiraErrorsResponse = JiraErrorsResponse.Get(webEx)

                                If (jiraErrors IsNot Nothing AndAlso jiraErrors.HasErrors) Then
                                    AddLog(String.Format(_jiraExceptionMessageFormat,
                                                        webEx.Message,
                                                        Environment.NewLine,
                                                        vbTab,
                                                        jiraErrors.ToString()))
                                    AddLog(webEx.StackTrace,
                                            LogType.Report,
                                            crmLinkError,
                                            String.Format("Could not get Jira Issue:{0}{1}", Environment.NewLine, jiraErrors.ToString()),
                                            Orientation.OutToJira,
                                            ObjectType.Ticket,
                                            ticket.TicketID,
                                            "update",
                                            URI,
                                            OperationType.Create)
                                Else
                                    AddLog(String.Format("{0}{1}{2}", webEx.Message, Environment.NewLine, webEx.StackTrace))
                                End If
                            Else
                                AddLog(String.Format(_jiraExceptionMessageFormat,
                                                    webEx.Message,
                                                    Environment.NewLine,
                                                    vbTab,
                                                    invalidJiraKey))
                                AddLog(webEx.StackTrace,
                                        LogType.Report,
                                        crmLinkError,
                                        String.Format("Could not get Jira Issue:{0}{1}", Environment.NewLine, invalidJiraKey),
                                        Orientation.OutToJira,
                                        ObjectType.Ticket,
                                        ticket.TicketID,
                                        "update",
                                        URI,
                                        OperationType.Create)
                            End If

                            Continue For
                        Catch ex As Exception
                            AddLog(String.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace))
                            Continue For
                        End Try
                    Else
                        Log.Write("Already linked. Doing nothing.")
                    End If

                    If updateTicketFlag Then
                        crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID, "update")

                        Try
                            Dim creatorName As String = "TeamSupport"

                            If ticketLinkToJira.CreatorID IsNot Nothing AndAlso ticketLinkToJira.CreatorID > 0 Then
                                Dim creator As Users = New Users(User)
                                creator.LoadByUserID(ticketLinkToJira.CreatorID)
                                creatorName = String.Format("{0}.{1} {2} linked", creator(0).FirstName.Substring(0, 1),
                                                                                If(String.IsNullOrEmpty(creator(0).MiddleName), "", " " + creator(0).MiddleName.Substring(0, 1) + "."),
                                                                                creator(0).LastName)
                            End If

                            AddRemoteLinkInJira(issue("id").ToString(), issue("key").ToString(), ticket.TicketID.ToString(), ticket.TicketNumber.ToString(), ticket.Name, creatorName)

                            ticketLinkToJira.JiraID = CType(issue("id").ToString(), Integer?)
                            ticketLinkToJira.JiraKey = issue("key").ToString()
                            ticketLinkToJira.JiraLinkURL = CRMLinkRow.HostName + "/browse/" + ticketLinkToJira.JiraKey
                            ticketLinkToJira.JiraStatus = issue("fields")("status")("name").ToString()

                            If CRMLinkRow.UpdateStatus Then
                                Dim newStatus As TicketStatus = allStatuses.FindByName(ticketLinkToJira.JiraStatus, ticket.TicketTypeID)

                                If newStatus IsNot Nothing Then
                                    Dim updateTicket As Tickets = New Tickets(User)
                                    updateTicket.LoadByTicketID(ticket.TicketID)
                                    updateTicket(0).TicketStatusID = newStatus.TicketStatusID
                                    ticketLinkToJira.DateModifiedByJiraSync = DateTime.UtcNow
                                    updateTicket.Save()
                                    AddLog(String.Format("Updated status with linked issue status: {0} ({1})", newStatus.Name, newStatus.TicketStatusID))
                                Else
                                    AddLog(String.Format("The Issue status '{0}' in Jira does not exist for the Ticket Type {1}.", ticketLinkToJira.JiraStatus, ticket.TicketTypeName),
                                        LogType.TextAndReport,
                                        crmLinkError,
                                        String.Format("Status was not updated because Issue status '{0}' does not exist in the current Ticket statuses.", ticketLinkToJira.JiraStatus),
                                        Orientation.OutToJira,
                                        ObjectType.Ticket,
                                        ticket.TicketID.ToString(),
                                        "update",
                                        "Status",
                                        OperationType.Update)
                                End If
                            Else
                                Dim newAction As Actions = New Actions(User)
                                newAction.AddNewAction()
                                newAction(0).ActionTypeID = newActionsTypeID
                                newAction(0).TicketID = ticket.TicketID
                                newAction(0).Description = "Ticket has been synced with Jira's issue " + issue("key").ToString() + " with status """ + ticketLinkToJira.JiraStatus + """."
                                ticketLinkToJira.DateModifiedByJiraSync = DateTime.UtcNow()
                                newAction.Save()

                                Dim newActionLinkToJira As ActionLinkToJira = New ActionLinkToJira(User)
                                newActionLinkToJira.AddNewActionLinkToJiraItem()
                                newActionLinkToJira(0).ActionID = newAction(0).ActionID
                                newActionLinkToJira(0).JiraID = -1
                                newActionLinkToJira(0).DateModifiedByJiraSync = ticketLinkToJira.DateModifiedByJiraSync
                                newActionLinkToJira.Save()

                                AddLog("Added comment indicating linked issue status.")
                            End If

                            ticketLinkToJira.Collection.Save()
                            AddLog("Updated ticketLinkToJira fields for ticket")

                            ClearCrmLinkError(crmLinkError)
                        Catch ex As Exception
                            AddLog(ex.ToString() + ex.StackTrace,
                                LogType.Report,
                                crmLinkError,
                                "Error creating the RemoteLink in the Issue. " + ex.Message,
                                Orientation.OutToJira,
                                ObjectType.Ticket,
                                ticket.TicketID.ToString(),
                                "update",
                                Nothing,
                                OperationType.Update)
                        End Try
                    End If

                    PushActionsAsComments(ticket.TicketID, ticket.TicketNumber, issue, ticketLinkToJira.JiraKey, attachmentEnabled, attachmentFileSizeLimit)

                    If sendCustomMappingFields Then
                        'We are now updating the custom mapping fields. We do a call per field to minimize the impact of invalid values attempted to be assigned.
                        If issueFields IsNot Nothing Then
                            For Each field As KeyValuePair(Of String, JToken) In issueFields
                                Try
                                    If (field.Value("name").ToString().ToLower() = "time tracking") Then
                                        UpdateIssueTimeTrackingFields(issue("id"), customMappingFields, ticket, field, crmLinkErrors, URI)
                                    Else
                                        UpdateIssueField(issue("id"), customMappingFields, ticket, field, crmLinkErrors, URI)
                                    End If
                                Catch ex As Exception
                                    AddLog(ex.ToString() + ex.StackTrace,
                                    LogType.Report,
                                    crmLinkError,
                                    "Error updating the field " + field.Value("name").ToString() + " in the Issue. " + ex.Message,
                                    Orientation.OutToJira,
                                    ObjectType.Ticket,
                                    ticket.TicketID.ToString(),
                                    "update",
                                    Nothing,
                                    OperationType.Update)
                                End Try
                            Next
                        End If
                    ElseIf isNew Then
                        AddLog("Include Non-Required Fields On Issue Creation: Off. Only creating issue with required fields.")
                    End If
                Next
            End Sub

            Private Sub UpdateIssueField(ByRef issueId As Integer,
            ByRef customMappingFields As CRMLinkFields,
            ByRef ticket As TicketsViewItem,
            ByRef field As KeyValuePair(Of String, JToken),
            ByRef crmLinkErrors As CRMLinkErrors,
            Optional ByRef URI As String = Nothing)

                Dim updateFieldRequestBody As StringBuilder = New StringBuilder()
                Dim fieldName = field.Value("name").ToString()
                Dim fieldKey = field.Key.ToString()
                Dim cRMLinkField As CRMLinkField = customMappingFields.FindByCRMFieldName(fieldName)

                If cRMLinkField IsNot Nothing Then
                    Dim value As String = Nothing
                    Dim notIncludedMessage As String = String.Empty
                    Dim findCustom As New CustomValues(User)
                    Dim crmLinkError As CRMLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID, cRMLinkField.TSFieldName)

                    If cRMLinkField.CustomFieldID IsNot Nothing Then
                        findCustom.LoadByFieldID(cRMLinkField.CustomFieldID, ticket.TicketID)

                        If findCustom.Count > 0 Then
                            Dim customValue As String = findCustom(0).Value

                            If (cRMLinkField.CRMFieldName.ToLower() = "sprint") Then
                                Dim jiraClientAgile As JiraClient = New JiraClient(_baseURI.Replace("/rest/api/latest", ""), CRMLinkRow.Username, If(String.IsNullOrEmpty(CRMLinkRow.SecurityToken), CRMLinkRow.Password, CRMLinkRow.SecurityToken), "agile")
                                Dim boards As List(Of Board) = jiraClientAgile.GetBoards()
                                Dim sprint As Sprint = New Sprint With {.id = 0, .name = ""}

                                Try
                                    For Each board As Board In boards
                                        Dim sprints As List(Of Sprint) = jiraClientAgile.GetSprintsByBoardId(board.id)

                                        If (sprints.Where(Function(p) p.name.ToLower() = customValue.ToLower()).Any()) Then
                                            sprint = sprints.Where(Function(p) p.name.ToLower() = customValue.ToLower()).FirstOrDefault()
                                            Exit For
                                        End If
                                    Next

                                    customValue = sprint.id
                                Catch jiraEx As JiraClientException
                                    AddLog(String.Format(_jiraExceptionMessageFormat,
                                                    jiraEx.InnerException.Message,
                                                    Environment.NewLine,
                                                    vbTab,
                                                    DirectCast(jiraEx.InnerException, JiraClientException).ErrorResponse))
                                End Try

                                customValue = sprint.id
                            End If

                            value = GetDataLineValue(fieldKey, field.Value("schema")("custom"), customValue)
                        Else
                            Dim customFields As New CustomFields(User)
                            customFields.LoadByCustomFieldID(cRMLinkField.CustomFieldID)
                            Dim isBooleanField As Boolean = customFields(0).FieldType = CustomFieldType.Boolean

                            'If the custom field is boolean and do not have any value then it is False. As seen in the UI
                            If isBooleanField Then
                                value = GetDataLineValue(fieldKey, field.Value("schema")("custom"), False.ToString())
                            Else
                                notIncludedMessage = GetFieldNotIncludedMessage(ticket.TicketID, fieldName, findCustom.Count = 0)
                            End If
                        End If
                    ElseIf cRMLinkField.TSFieldName IsNot Nothing Then
					If ticket.Row(cRMLinkField.TSFieldName) IsNot Nothing Then
							value = GetDataLineValue(fieldKey, field.Value("schema")("custom"), ticket.Row(cRMLinkField.TSFieldName).ToString()) '//vv
                        Else
						notIncludedMessage = GetFieldNotIncludedMessage(ticket.TicketID, fieldName, ticket.Row(cRMLinkField.TSFieldName) Is Nothing)
                        End If
                    Else
                        AddLog("Field '" + fieldName + "' was not included because custom field " +
                                cRMLinkField.CRMFieldID.ToString() + " CustomFieldID and TSFieldName are null.")
                    End If

                    If value IsNot Nothing Then
                        updateFieldRequestBody = New StringBuilder()
                        updateFieldRequestBody.Append("{")
                        updateFieldRequestBody.Append("""fields"":{")
                        updateFieldRequestBody.Append("""" + field.Key.ToString() + """:" + value)
                        updateFieldRequestBody.Append("}")
                        updateFieldRequestBody.Append("}")

                        Try
                            Dim jiraClient As JiraClient = New JiraClient(_baseURI.Replace("/rest/api/latest", ""), CRMLinkRow.Username, If(String.IsNullOrEmpty(CRMLinkRow.SecurityToken), CRMLinkRow.Password, CRMLinkRow.SecurityToken))
                            jiraClient.UpdateIssueFieldByParameter(issueId, updateFieldRequestBody.ToString())
                            ClearCrmLinkError(crmLinkError)
                        Catch jiraEx As JiraClientException
                            AddLog(String.Format(_jiraExceptionMessageFormat,
                                                jiraEx.InnerException.Message,
                                                Environment.NewLine,
                                                vbTab,
                                                DirectCast(jiraEx.InnerException, JiraClientException).ErrorResponse))
                            AddLog(jiraEx.StackTrace,
                                LogType.Report,
                                crmLinkError,
                                jiraEx.Message,
                                Orientation.OutToJira,
                                ObjectType.Ticket,
                                ticket.TicketID.ToString(),
                                fieldName,
                                value,
                                OperationType.Update)
                        Catch ex As Exception
                            Try
                                Dim issue As JObject = GetAPIJObject(URI, "PUT", updateFieldRequestBody.ToString())
                                ClearCrmLinkError(crmLinkError)
                            Catch webEx As WebException
                                Dim jiraErrors As JiraErrorsResponse = JiraErrorsResponse.Get(webEx)

                                If (jiraErrors IsNot Nothing AndAlso jiraErrors.HasErrors) Then
                                    AddLog(String.Format(_jiraExceptionMessageFormat,
                                                        webEx.Message,
                                                        Environment.NewLine,
                                                        vbTab,
                                                        jiraErrors.ToString()))
                                    AddLog(webEx.StackTrace,
                                            LogType.Report,
                                            crmLinkError,
                                            jiraErrors.ToString(),
                                            Orientation.OutToJira,
                                            ObjectType.Ticket,
                                            ticket.TicketID.ToString(),
                                            fieldName,
                                            value,
                                            OperationType.Update)
                                Else
                                    AddLog(webEx.ToString() + webEx.StackTrace)
                                End If
                            Catch exception As Exception
                                Dim exBody As String = value

                                If updateFieldRequestBody IsNot Nothing Then
                                    exBody = updateFieldRequestBody.ToString()
                                End If

                                If exception.Message <> "Error reading JObject from JsonReader. Path '', line 0, position 0." Then
                                    AddLog(ex.ToString() + ex.StackTrace,
                                    LogType.TextAndReport,
                                    crmLinkError,
                                    "Fields for TimeTracking with body " + exBody + ", was not sent because an exception ocurred.",
                                    Orientation.OutToJira,
                                    ObjectType.Ticket,
                                    ticket.TicketID.ToString(),
                                    fieldName,
                                    value,
                                    OperationType.Update)
                                Else
                                    Log.Write("Exception trying to Update timetracking fields. " + exception.Message + ". " + exBody)
                                End If
                            End Try
                        End Try
                    Else
                        AddLog("No value found for the field " + fieldName,
                                LogType.TextAndReport,
                                crmLinkError,
                                notIncludedMessage,
                                Orientation.OutToJira,
                                ObjectType.Ticket,
                                ticket.TicketID,
                                fieldName,
                                Nothing,
                                OperationType.Update)
                    End If
                End If
            End Sub

            Private Sub UpdateIssueTimeTrackingFields(ByRef issueId As Integer,
            ByRef customMappingFields As CRMLinkFields,
            ByRef ticket As TicketsViewItem,
            ByRef field As KeyValuePair(Of String, JToken),
            ByRef crmLinkErrors As CRMLinkErrors,
            Optional ByRef URI As String = Nothing)

                Dim value As String = Nothing
                Dim updateFieldRequestBody As StringBuilder = New StringBuilder()
                Dim timeTrackingFields As List(Of String) = New List(Of String)(New String() {"originalEstimate", "remainingestimate"})
                Dim customFieldIdForLog As Integer? = Nothing
                Dim customFieldNameForLog As String = Nothing
                Dim crmLinkError As CRMLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID, "timetracking")

                For Each timeTrackingField As String In timeTrackingFields
                    Dim cRMLinkField As CRMLinkField = customMappingFields.FindByCRMFieldName(timeTrackingField)
                    Dim notIncludedMessage As String = String.Empty

                    If cRMLinkField IsNot Nothing Then
                        Dim findCustom As New CustomValues(User)
                        customFieldIdForLog = cRMLinkField.CustomFieldID
                        customFieldNameForLog = cRMLinkField.TSFieldName

                        If cRMLinkField.CustomFieldID IsNot Nothing Then
                            findCustom.LoadByFieldID(cRMLinkField.CustomFieldID, ticket.TicketID)

                            If findCustom.Count > 0 Then
                                Dim dataLineValue As String = GetDataLineValue(timeTrackingField, field.Value("schema")("custom"), findCustom(0).Value)
                                value = If(value IsNot Nothing, value + "," + dataLineValue, dataLineValue)
                            Else
                                notIncludedMessage = GetFieldNotIncludedMessage(ticket.TicketID, timeTrackingField, findCustom.Count = 0)
                            End If
                        ElseIf cRMLinkField.TSFieldName IsNot Nothing Then
                            If ticket.Row(cRMLinkField.TSFieldName) IsNot Nothing Then
                                Dim dataLineValue As String = GetDataLineValue(timeTrackingField, field.Value("schema")("custom"), ticket.Row(cRMLinkField.TSFieldName))
                                value = If(value IsNot Nothing, "," + dataLineValue, dataLineValue)
                            Else
                                notIncludedMessage = GetFieldNotIncludedMessage(ticket.TicketID, timeTrackingField, ticket.Row(cRMLinkField.TSFieldName) Is Nothing)
                            End If
                        Else
                            AddLog("Field '" + timeTrackingField + "' was not included because custom field " +
                                    cRMLinkField.CRMFieldID.ToString() + " CustomFieldID and TSFieldName are null.")
                        End If

                        If (Not String.IsNullOrEmpty(notIncludedMessage)) Then
                            AddLog(notIncludedMessage)
                        End If
                    End If
                Next

                If value IsNot Nothing Then
                    updateFieldRequestBody = New StringBuilder()
                    updateFieldRequestBody.Append("{")
                    updateFieldRequestBody.Append("""fields"":{")
                    updateFieldRequestBody.Append("""" + field.Key.ToString() + """:{" + value + "}")
                    updateFieldRequestBody.Append("}")
                    updateFieldRequestBody.Append("}")

                    Try
                        Dim jiraClient As JiraClient = New JiraClient(_baseURI.Replace("/rest/api/latest", ""), CRMLinkRow.Username, If(String.IsNullOrEmpty(CRMLinkRow.SecurityToken), CRMLinkRow.Password, CRMLinkRow.SecurityToken))
                        jiraClient.UpdateIssueFieldByParameter(issueId, updateFieldRequestBody.ToString())
                        ClearCrmLinkError(crmLinkError)
                    Catch jiraEx As JiraClientException
                        AddLog(String.Format(_jiraExceptionMessageFormat,
                                                    jiraEx.InnerException.Message,
                                                    Environment.NewLine,
                                                    vbTab,
                                                    DirectCast(jiraEx.InnerException, JiraClientException).ErrorResponse))
                        AddLog(jiraEx.StackTrace,
                                LogType.Report,
                                crmLinkError,
                                jiraEx.Message,
                                Orientation.OutToJira,
                                ObjectType.Ticket,
                                ticket.TicketID.ToString(),
                                "timetracking",
                                value,
                                OperationType.Update)
                    Catch ex As Exception
                        Try
                            Dim issue As JObject = GetAPIJObject(URI, "PUT", updateFieldRequestBody.ToString())
                            ClearCrmLinkError(crmLinkError)
                        Catch webEx As WebException
                            Dim jiraErrors As JiraErrorsResponse = JiraErrorsResponse.Get(webEx)

                            If (jiraErrors IsNot Nothing AndAlso jiraErrors.HasErrors) Then
                                AddLog(String.Format(_jiraExceptionMessageFormat,
                                                            webEx.Message,
                                                            Environment.NewLine,
                                                            vbTab,
                                                            jiraErrors.ToString()))
                                AddLog(webEx.StackTrace,
                                            LogType.Report,
                                            crmLinkError,
                                            jiraErrors.ToString(),
                                            Orientation.OutToJira,
                                            ObjectType.Ticket,
                                            ticket.TicketID.ToString(),
                                            "timetracking",
                                            value,
                                            OperationType.Update)
                            Else
                                AddLog(webEx.ToString() + webEx.StackTrace)
                            End If
                        Catch exception As Exception
                            Dim exBody As String = value

                            If updateFieldRequestBody IsNot Nothing Then
                                exBody = updateFieldRequestBody.ToString()
                            End If

                            If exception.Message <> "Error reading JObject from JsonReader. Path '', line 0, position 0." Then
                                AddLog(ex.ToString() + ex.StackTrace,
                                    LogType.TextAndReport,
                                    crmLinkError,
                                    "Fields for  TimeTracking with body " + exBody + ", was not sent because an exception ocurred.",
                                    Orientation.OutToJira,
                                    ObjectType.Ticket,
                                    ticket.TicketID.ToString(),
                                    "timetracking",
                                    value,
                                    OperationType.Update)
                            Else
                                AddLog("Exception trying to Update timetracking fields. " + exception.Message + ". " + exBody)
                            End If
                        End Try
                    End Try
                End If
            End Sub

            Private Function GetAttachmentEnabled(ByRef attachmentFileSizeLimit As Integer) As String
                Dim result As Boolean = False

                Dim URI As String = _baseURI + "/attachment/meta"
                Dim batch As JObject = GetAPIJObject(URI, "GET", String.Empty)
                result = Convert.ToBoolean(batch("enabled").ToString())
                attachmentFileSizeLimit = Convert.ToInt32(batch("uploadLimit").ToString())
                Log.Write("Attachment enabled is " + result.ToString())

                Return result
            End Function

            Private Function GetTicketData(ByVal ticket As TicketsViewItem,
                                        ByRef fields As JObject,
                                        ByVal jiraProjectKey As String,
                                        ByRef actionDescriptionId As Integer,
                                        ByRef customMappingFields As CRMLinkFields,
                                        ByRef crmLinkErrors As CRMLinkErrors,
                                        ByRef ticketTypeName As String) As String
                Dim result As StringBuilder = New StringBuilder()
                Dim customField As StringBuilder = New StringBuilder()
                customField = BuildRequiredFields(ticket, fields, customMappingFields, crmLinkErrors)
                result.Append("{")
                result.Append("""fields"":{")
                result.Append("""summary"":""" + DataUtils.GetJsonCompatibleString(HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(ticket.Name))) + """,")
				result.Append("""issuetype"":{""name"":""" + ticketTypeName + """},")

				If customField.ToString().Trim().Length > 0 Then
					result.Append(customField.ToString() + ",")
				End If

				result.Append("""project"":{""key"":""" + jiraProjectKey + """},")
                Dim actionDescription As Action = Actions.GetTicketDescription(User, ticket.TicketID)
                actionDescriptionId = actionDescription.ActionID
				'            Dim addLines As Boolean = True
				'Dim description As String = HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(actionDescription.Description), addLines)

				Dim description As String = actionDescription.Description
				'Extract the code tag contents (if exists) and re-add after the html strip
				Dim codeSamples As New Dictionary(Of Integer, String)
				codeSamples = HtmlCleaner.ExtractCodeSamples(description)
				Dim addLines As Boolean = True
				description = HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(description), addLines)
				HtmlCleaner.AddCodeSamples(description, codeSamples)

				result.Append("""description"":""" + DataUtils.GetJsonCompatibleString(description) + """")
                result.Append("}")
                result.Append("}")

                Return result.ToString()
            End Function

            Private Function GetIssueFields(ByVal ticket As TicketsViewItem,
                                        ByVal jiraProjectKey As String,
                                        ByRef crmLinkError As CRMLinkError,
                                        ByVal orientation As Orientation,
                                        ByVal issueTypeName As String) As JObject
                Dim addTypeFieldsToList As Boolean = True
                Dim result As JObject = Nothing

                issueTypeName = Replace(issueTypeName, " ", "+")
                jiraProjectKey = Replace(jiraProjectKey, " ", "+")
				addTypeFieldsToList = True

				If (addTypeFieldsToList) Then
                    Dim URI As String = _baseURI +
                                    "/issue/createmeta?projectKeys=" +
                                    jiraProjectKey.ToUpper() +
                                    "&issuetypeNames=" + issueTypeName +
                                    "&expand=projects.issuetypes.fields"
                    'View an example of this response in 
                    'https://developer.atlassian.com/display/JIRADEV/JIRA+REST+API+Example+-+Discovering+meta-data+for+creating+issues

                    Try
                        Dim fields = GetAPIJObject(URI, "GET", String.Empty)

                        If (fields("projects").Count > 0) Then
                            For i = 0 To fields("projects")(0)("issuetypes").Count - 1
                                If Replace(fields("projects")(0)("issuetypes")(i)("name").ToString().ToLower(), " ", "+") = issueTypeName.ToLower() Then
                                    result = CType(fields("projects")(0)("issuetypes")(i)("fields"), JObject)
                                    Exit For
                                End If
                            Next
                        End If

                        If result Is Nothing Then
                            AddLog(String.Format("Issue Type was '{0}' not found in Project Type '{1}' in Jira.", issueTypeName, jiraProjectKey),
                                LogType.Report,
                                crmLinkError,
                                String.Format("Issue Type was '{0}' not found in Project Type '{1}' in Jira.", issueTypeName, jiraProjectKey),
                                orientation,
                                ObjectType.Ticket,
                                ticket.TicketID,
                                "",
                                Nothing,
                                OperationType.Unknown)
                            AddLog("Type was not found in list of project types. If an exception ahead, chances are it was caused by missing type.")
                        Else
							ClearCrmLinkError(crmLinkError)
						End If
                    Catch ex As Exception
                        AddLog(String.Format("Exception rised attempting to get createmeta.{0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, "URI: " + URI, "Type: " + issueTypeName))
                        Throw New Exception("project mismatch")
                    End Try
                End If

                Return result
            End Function

            Private Function GetProjectKey(ByVal ticket As TicketsViewItem, ByVal crmLinkErrors As CRMLinkErrors) As String
                Dim jiraProjectKey As String = CRMLinkRow.DefaultProject
                Dim crmLinkError As CRMLinkError = crmLinkErrors.FindUnclearedByObjectIDAndFieldName(ticket.TicketID, "ProjectKey")

                If CRMLinkRow.AlwaysUseDefaultProjectKey Then
                    Log.Write(String.Format("Using Default Project Key ""{0}""", jiraProjectKey))
                Else
                    Dim ticketProductVersion As ProductVersion

                    If Not ticket.ReportedVersionID Is Nothing AndAlso ticket.ReportedVersionID > 0 Then
                        ticketProductVersion = ProductVersions.GetProductVersion(User, ticket.ReportedVersionID)
                    End If

                    If Not ticket.ReportedVersionID Is Nothing AndAlso Not ticketProductVersion Is Nothing AndAlso Not String.IsNullOrEmpty(ticketProductVersion.JiraProjectKey) Then
                        jiraProjectKey = ticketProductVersion.JiraProjectKey
                        Log.Write(String.Format("Jira Project Key ""{0}"" from Product Version {1}", jiraProjectKey, ticket.ReportedVersion))
                    Else
                        Dim ticketProduct As Product

                        If Not ticket.ProductID Is Nothing Then
                            ticketProduct = Products.GetProduct(User, ticket.ProductID)

                            If Not String.IsNullOrEmpty(ticketProduct.JiraProjectKey) Then
                                jiraProjectKey = ticketProduct.JiraProjectKey
                                Log.Write(String.Format("Jira Project Key ""{0}"" from Product {1}", jiraProjectKey, ticket.ProductName))
                            Else
                                jiraProjectKey = ticket.ProductName
                                Log.Write(String.Format("Jira Project Key ""{0}"" like Product Name", jiraProjectKey))
                            End If
                        Else
                            jiraProjectKey = CRMLinkRow.DefaultProject
                            Log.Write(String.Format("Default Project Key ""{0}"" to be used as Jira Project Key after not found in ProductVersion, Product, or Product Name", jiraProjectKey))
                        End If
                    End If
                End If

                If String.IsNullOrEmpty(jiraProjectKey) Then
                    Dim message As String = If(CRMLinkRow.AlwaysUseDefaultProjectKey, "AlwaysUseDefaultProjectKey but no Default Project.", "Couldn't find a Jira Project Key in ProductVersion, Product, Product Name in Ticket, or Default Project to use for integration.")
                    AddLog(message,
                        LogType.TextAndReport,
                        crmLinkError,
                        "Error attempting to get Jira project.",
                        Orientation.OutToJira,
                        ObjectType.Ticket,
                        ticket.TicketID,
                        "ProjectKey",
                        Nothing,
                        OperationType.Unknown)
                    Dim ex As Exception = New Exception(message)
                    Throw ex
                End If

                ClearCrmLinkError(crmLinkError)

                Return jiraProjectKey
            End Function

            Private Function GetFieldNotIncludedMessage(
              ByVal ticketID As Integer,
              ByVal fieldName As String,
              ByVal isNull As Boolean,
              Optional ByVal isNewIssue As Boolean = True,
              Optional ByVal isCreateable As Boolean = True,
              Optional ByVal isUpdateable As Boolean = True) As String

                Dim message As StringBuilder = New StringBuilder("Field '" + fieldName + "' was not included because ")
                If isNull Then
                    message.Append("it was null")
                End If
                If isNull AndAlso Not ((isNewIssue AndAlso isCreateable) OrElse isUpdateable) Then
                    message.Append(" and ")
                End If
                If Not ((isNewIssue AndAlso isCreateable) OrElse isUpdateable) Then
                    message.Append("the field is not updatable.")
                End If

                Return message.ToString()
            End Function

            Private Function GetDataLineValue(ByVal fieldKey As String, ByVal fieldType As Object, ByVal fieldValue As String, Optional ByVal issueId As Integer = 0) As String
                Dim result As String = Nothing
                Select Case fieldKey.ToLower()
                    Case "reporter"
                        'Because some orgs are mapping to the Reporter Jira field and they seem to be storing the email address then we need to leave it here, for the rest we should use the Name so it also links from TS to Jira when creating the issue
                        If (CRMLinkRow.OrganizationID = 930653 OrElse CRMLinkRow.OrganizationID = 1028984 OrElse CRMLinkRow.OrganizationID = 1136748 OrElse CRMLinkRow.OrganizationID = 995322) Then
                            result = "{""emailAddress"":""" + fieldValue + """}"
                        Else
                            result = "{""name"":""" + fieldValue + """}"
                        End If
                    Case "assignee"
                        'Because some orgs are mapping to the Assignee Jira field and they seem to be storing the email address then we need to leave it here, for the rest we should use the Name so it also links from TS to Jira when creating the issue
                        If (CRMLinkRow.OrganizationID = 461956 OrElse CRMLinkRow.OrganizationID = 869700 OrElse CRMLinkRow.OrganizationID = 884116) Then
                            result = "{""emailAddress"":""" + fieldValue + """}"
                        Else
                            result = "{""name"":""" + fieldValue + """}"
                        End If
                    Case "issuetype", "status", "priority", "resolution"
                        result = "{""name"":""" + fieldValue + """}"
                    Case "progress", "worklog"
                        result = "{""total"":""" + fieldValue + """}"
                    Case "project"
                        result = "{""key"":""" + fieldValue + """}"
                    Case "votes"
                        result = "{""votes"":""" + fieldValue + """}"
                    Case "watches"
                        result = "{""watchCount"":""" + fieldValue + """}"
                    Case "timetracking"
                        result = "{""timeSpentSeconds"":""" + fieldValue + """}"
                    Case "aggregrateprogress"
                        result = "{""progress"":""" + fieldValue + """}"
                    Case "attachment", "labels", "issuelinks", "versions", "fixversions", "subtasks", "components"
                        result = "[{""name"":""" + fieldValue + """}]"
                    Case "originalestimate"
                        result = """originalEstimate"":""" + fieldValue + """"
                    Case "remainingestimate"
                        result = """remainingEstimate"":""" + fieldValue + """"
					Case "duedate"
						'Jira date format for duedate: yyyy-MM-dd
						Dim dueDateFull As DateTime = Convert.ToDateTime(fieldValue)
						result = """" + dueDateFull.ToString("yyyy-MM-dd") + """"
					Case Else
                        result = """" + fieldValue + """"
                        If fieldType IsNot Nothing Then
                            Dim fieldTypeString = fieldType.ToString()
                            If fieldTypeString.Length > 50 Then
                                Select Case fieldTypeString.Substring(50, fieldTypeString.Length - 50).ToLower()
                                    Case "select"
                                        result = "{""value"":""" + fieldValue + """}"
                                    Case "multiselect"
                                        result = "[{""value"":""" + fieldValue + """}]"
                                    Case "date"
                                        result = """" + Convert.ToDateTime(fieldValue).ToString("'yyyy'-'MM'-'dd'") + """"
                                    Case "datetime"
                                        result = """" + Convert.ToDateTime(fieldValue).ToString("'yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'") + """"
                                    Case "float"
                                        result = fieldValue
                        'text field (single-line)
                                    Case "string"
                                        result = "{""" + fieldValue + """}"
                                    Case "radiobuttons"
                                        If fieldValue = "True" Then
                                            result = "{""value"":""Yes""}"
                                        Else
                                            result = "{""value"":""No""}"
                                        End If
                                End Select
                            End If
                        End If
                End Select
                Return result
            End Function

            Private Sub AddRemoteLinkInJira(ByVal issueID As String, ByVal issueKey As String, ByVal ticketID As String, ByVal ticketNumber As String, ByVal ticketName As String, ByVal creatorName As String)
                Dim domain As String = SystemSettings.ReadStringForCrmService(User, "AppDomain", "https://app.teamsupport.com")

                Try
                    Dim globalId As String = "system=" + domain + "/Ticket.aspx?ticketid=&id=" + ticketID
                    Dim jiraClient As JiraClient = New JiraClient(_baseURI.Replace("/rest/api/latest", ""), CRMLinkRow.Username, If(String.IsNullOrEmpty(CRMLinkRow.SecurityToken), CRMLinkRow.Password, CRMLinkRow.SecurityToken))
                    Dim remoteLink As RemoteLink = New RemoteLink With {.url = domain + "/Ticket.aspx?ticketid=" + ticketID,
                                                                    .title = creatorName + " Ticket #" + ticketNumber,
                                                                    .summary = DataUtils.GetJsonCompatibleString(HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(ticketName))),
                                                                    .icon = New Icon With {.title = "TeamSupport Logo",
                                                                                            .url16x16 = domain + "/vcr/1_6_5/Images/icons/TeamSupportLogo16.png"}}
                    Dim issueRef As IssueRef = New IssueRef With {.id = issueID, .key = issueKey}

                    Log.Write("Creating the RemoteLink with data:")
                    Log.Write(JsonConvert.SerializeObject(remoteLink))

                    Dim remoteLinkCreated As RemoteLink = jiraClient.CreateRemoteLink(issueRef, remoteLink, globalId)

                    If remoteLinkCreated IsNot Nothing AndAlso Not String.IsNullOrEmpty(remoteLinkCreated.id) AndAlso remoteLinkCreated.id <> "0" Then
                        Log.Write("RemoteLink created. Id: " + remoteLinkCreated.id)
                    Else
                        Log.Write("RemoteLink was not created and no error was sent back from Jira.")
                    End If
                Catch jiraEx As JiraClientException
                    AddLog(String.Format(_jiraExceptionMessageFormat,
                                                    jiraEx.InnerException.Message,
                                                    Environment.NewLine,
                                                    vbTab,
                                                    DirectCast(jiraEx.InnerException, JiraClientException).ErrorResponse))
                    AddLog("Adding remote link with JiraClient object failed. Using old POST with JObject method.")

                    Dim remoteLinkData As StringBuilder = New StringBuilder()
                    remoteLinkData.Append("{")
                    'Global ID initialized as documentation examples in two parts separated by &. First part is the domain and the second one the id.
                    remoteLinkData.Append("""globalid"":""system=" + domain + "/Ticket.aspx?ticketid=&id=" + ticketID + """,")
                    remoteLinkData.Append("""application"":{")
                    remoteLinkData.Append("""name"":""Team Support""},")
                    remoteLinkData.Append("""object"":{")
                    remoteLinkData.Append("""icon"":{")
                    remoteLinkData.Append("""url16x16"":""" + domain + "/vcr/1_6_5/Images/icons/TeamSupportLogo16.png"",")
                    remoteLinkData.Append("""title"":""TeamSupport Logo""},")
                    remoteLinkData.Append("""title"":""" + creatorName + " Ticket #" + ticketNumber + """,")
                    remoteLinkData.Append("""summary"":""" + DataUtils.GetJsonCompatibleString(HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(ticketName))) + """,")
                    remoteLinkData.Append("""url"":""" + domain + "/Ticket.aspx?ticketid=" + ticketID + """")
                    remoteLinkData.Append("}")
                    remoteLinkData.Append("}")

                    Dim URI As String = _baseURI + "/issue/" + issueID + "/remotelink"
                    Log.Write("AddRemoteLink URI: " + URI)
                    Log.Write("AddRemoteLink Data:" + remoteLinkData.ToString())
                    Try
                        Dim response As JObject = GetAPIJObject(URI, "POST", remoteLinkData.ToString())
                    Catch ex As Exception
                        AddLog(String.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace))
                        Throw ex
                    End Try
                End Try
            End Sub

            Private Sub PushActionsAsComments(
                ByVal ticketID As Integer,
                ByVal ticketNumber As Integer,
                ByVal issue As JObject,
                ByVal issueKey As String,
                ByVal attachmentEnabled As Boolean,
                ByVal attachmentFileSizeLimit As Integer)

                Dim actionsToPushAsComments As Actions = New Actions(User)
                actionsToPushAsComments.LoadToPushToJira(CRMLinkRow, ticketID)
                Log.Write("Found " + actionsToPushAsComments.Count.ToString() + " actions to push as comments.")

                Dim actionLinkToJira As ActionLinkToJira = New ActionLinkToJira(User)
                actionLinkToJira.LoadToPushToJira(CRMLinkRow, ticketID)

                Dim crmLinkActionErrors As CRMLinkErrors = New CRMLinkErrors(User)
                crmLinkActionErrors.LoadByOperationAndObjectIds(CRMLinkRow.OrganizationID,
                                                        CRMLinkRow.CRMType,
                                                        GetDescription(Orientation.OutToJira),
                                                        GetDescription(ObjectType.Action),
                                                        actionsToPushAsComments.Select(Function(p) p.ActionID.ToString()).ToList(),
                                                        isCleared:=False)
                Dim crmLinkError As CRMLinkError = Nothing
                Dim body As StringBuilder = Nothing

                For Each actionToPushAsComment As Action In actionsToPushAsComments
                    Dim actionLinkToJiraItem As ActionLinkToJiraItem = actionLinkToJira.FindByActionID(actionToPushAsComment.ActionID)
                    Dim actionPosition As Integer = Actions.GetActionPosition(User, actionToPushAsComment.ActionID)
                    crmLinkError = crmLinkActionErrors.FindByObjectIDAndFieldName(actionToPushAsComment.ActionID.ToString(), String.Empty)
                    Log.Write("Processing actionID: " + actionToPushAsComment.ActionID.ToString())

                    If actionLinkToJiraItem Is Nothing Then
                        Try
                            If issue Is Nothing Then
                                issue = GetAPIJObject(_baseURI + "/issue/" + issueKey, "GET", String.Empty)
                            End If

                            Dim jiraClient As JiraClient = New JiraClient(_baseURI.Replace("/rest/api/latest", ""), CRMLinkRow.Username, If(String.IsNullOrEmpty(CRMLinkRow.SecurityToken), CRMLinkRow.Password, CRMLinkRow.SecurityToken))
                            Dim issueRef As IssueRef = New IssueRef With {.id = issue("id"), .key = issueKey}

                            body = New StringBuilder()
                            Dim jiraComment As String = BuildCommentBody(ticketNumber, actionToPushAsComment.Description, actionPosition, actionToPushAsComment.CreatorID)
                            body.Append(jiraComment)
                            Dim commentCreated As Comment = jiraClient.CreateComment(issueRef, jiraComment)
                            Dim newActionLinkToJira As ActionLinkToJira = New ActionLinkToJira(User)
                            Dim newActionLinkToJiraItem As ActionLinkToJiraItem = newActionLinkToJira.AddNewActionLinkToJiraItem()

                            newActionLinkToJiraItem.ActionID = actionToPushAsComment.ActionID
                            newActionLinkToJiraItem.JiraID = commentCreated.id
                            newActionLinkToJiraItem.DateModifiedByJiraSync = DateTime.UtcNow
                            newActionLinkToJira.Save()
                            Log.Write("Created comment for action")

                            ClearCrmLinkError(crmLinkError)
                        Catch jiraEx As JiraClientException
                            AddLog(String.Format(_jiraExceptionMessageFormat,
                                                    jiraEx.InnerException.Message,
                                                    Environment.NewLine,
                                                    vbTab,
                                                    DirectCast(jiraEx.InnerException, JiraClientException).ErrorResponse))
                            AddLog(jiraEx.StackTrace,
                                LogType.Report,
                                crmLinkError,
                                jiraEx.Message,
                                Orientation.OutToJira,
                                ObjectType.Action,
                                actionToPushAsComment.ActionID.ToString(),
                                String.Empty,
                                If(body IsNot Nothing, body.ToString(), Nothing),
                                OperationType.Create)
                            Continue For
                        Catch ex As Exception
                            AddLog(ex.ToString() + ex.StackTrace)
                            Continue For
                        End Try
                    Else
                        Try
                            Log.Write("action.JiraID: " + actionLinkToJiraItem.JiraID.ToString())
                            If actionLinkToJiraItem.JiraID <> -1 Then
                                If issue Is Nothing Then
                                    issue = GetAPIJObject(_baseURI + "/issue/" + issueKey, "GET", String.Empty)
                                End If

                                Dim jiraClient As JiraClient = New JiraClient(_baseURI.Replace("/rest/api/latest", ""), CRMLinkRow.Username, If(String.IsNullOrEmpty(CRMLinkRow.SecurityToken), CRMLinkRow.Password, CRMLinkRow.SecurityToken))
                                Dim issueRef As IssueRef = New IssueRef With {.id = issue("id"), .key = issueKey}

                                body = New StringBuilder()
                                body.Append(BuildCommentBody(ticketNumber, actionToPushAsComment.Description, actionPosition, actionToPushAsComment.CreatorID))
                                Dim commentUpdated As Comment = jiraClient.UpdateComment(issueRef, actionLinkToJiraItem.JiraID, body.ToString())

                                actionLinkToJiraItem.DateModifiedByJiraSync = DateTime.UtcNow
                                Log.Write("updated comment for actionID: " + actionToPushAsComment.ActionID.ToString())
                            End If

                            ClearCrmLinkError(crmLinkError)
                        Catch jiraEx As JiraClientException
                            AddLog(String.Format(_jiraExceptionMessageFormat,
                                                    jiraEx.InnerException.Message,
                                                    Environment.NewLine,
                                                    vbTab,
                                                    DirectCast(jiraEx.InnerException, JiraClientException).ErrorResponse))
                            AddLog(jiraEx.StackTrace,
                                LogType.Report,
                                crmLinkError,
                                jiraEx.Message,
                                Orientation.OutToJira,
                                ObjectType.Action,
                                actionToPushAsComment.ActionID.ToString(),
                                String.Empty,
                                If(body IsNot Nothing, body.ToString(), Nothing),
                                OperationType.Update)

                            Continue For
                        Catch ex As Exception
                            AddLog(ex.ToString() + ex.StackTrace)
                            Continue For
                        End Try
                    End If

                    If (attachmentEnabled) Then
                        PushAttachments(actionToPushAsComment.ActionID, ticketNumber, issue, issueKey, attachmentFileSizeLimit, actionPosition)
                    End If
                Next

                actionLinkToJira.Save()
            End Sub

            Private Sub PushAttachments(
            ByVal actionID As Integer,
            ByVal ticketNumber As Integer,
            ByVal issue As JObject,
            ByVal issueKey As String,
            ByVal fileSizeLimit As Integer,
            ByVal actionPosition As Integer)

                Dim attachments As AttachmentProxy() = TeamSupport.Data.Quarantine.ServiceQ.LoadForJira(User, actionID)

                Dim crmLinkAttachmentErrors As CRMLinkErrors = New CRMLinkErrors(User)
                crmLinkAttachmentErrors.LoadByOperationAndObjectIds(CRMLinkRow.OrganizationID,
                                                                CRMLinkRow.CRMType,
                                                                GetDescription(Orientation.OutToJira),
                                                                GetDescription(ObjectType.Attachment),
                                                                attachments.Select(Function(p) p.AttachmentID.ToString()).ToList(),
                                                                isCleared:=False)
                Dim crmLinkError As CRMLinkError = Nothing
                Dim attachmentError As String = String.Empty

                For Each attachment As Data.AttachmentProxy In attachments
                    crmLinkError = crmLinkAttachmentErrors.FindByObjectIDAndFieldName(attachment.AttachmentID.ToString(), "file")

                    If (Not File.Exists(attachment.Path)) Then
                        attachmentError = "Attachment was not sent as it was not found on server"
                        AddLog(attachmentError,
                            LogType.TextAndReport,
                            crmLinkError,
                            attachmentError,
                            Orientation.OutToJira,
                            ObjectType.Attachment,
                            attachment.AttachmentID,
                            "file",
                            attachment.FileName,
                            OperationType.Create)
                    Else
                        Dim fs = New FileStream(attachment.Path, FileMode.Open, FileAccess.Read)

                        If (fs.Length > fileSizeLimit) Then
                            attachmentError = String.Format("Attachment was not sent as its file size ({0}) exceeded the file size limit of {1}", fs.Length.ToString(), fileSizeLimit.ToString())
                            AddLog(attachmentError,
                                LogType.TextAndReport,
                                crmLinkError,
                                attachmentError,
                                Orientation.OutToJira,
                                ObjectType.Attachment,
                                attachment.AttachmentID,
                                "file",
                                attachment.FileName,
                                OperationType.Create)
                        Else
                            Try
                                Dim URIString As String = _baseURI + "/issue/" + issue("id").ToString() + "/attachments/"
                                Dim request As HttpWebRequest = WebRequest.Create(URIString)
                                request.Headers.Add("Authorization", "Basic " + _encodedCredentials)
                                request.Headers.Add("X-Atlassian-Token", "nocheck")
                                request.Method = "POST"
                                Dim boundary As String = String.Format("----------{0:N}", Guid.NewGuid())
                                request.ContentType = String.Format("multipart/form-data; boundary={0}", boundary)
                                request.UserAgent = Client

                                Dim content = New MemoryStream()
                                Dim writer = New StreamWriter(content)
                                writer.WriteLine("--{0}", boundary)
                                writer.WriteLine("Content-Disposition: form-data; name=""file""; filename=""{0}""", ("TeamSupport Ticket #" + ticketNumber.ToString() + " action #" + actionPosition.ToString() + " - " + attachment.FileName))
                                writer.WriteLine("Content-Type: application/octet-stream")
                                writer.WriteLine()
                                writer.Flush()
                                Dim data(fs.Length) As Byte
                                fs.Read(data, 0, data.Length)
                                fs.Close()
                                content.Write(data, 0, data.Length)
                                writer.WriteLine()
                                writer.WriteLine("--" + boundary + "--")
                                writer.Flush()
                                content.Seek(0, SeekOrigin.Begin)
                                request.ContentLength = content.Length

                                Using requestStream As Stream = request.GetRequestStream()
                                    content.WriteTo(requestStream)
                                    requestStream.Close()
                                End Using

                                Using response As HttpWebResponse = request.GetResponse()
                                    Log.Write("Attachment """ + attachment.FileName + """ sent.")
                                    response.Close()
                                End Using

                                content.Flush()
                                content.Close()
                                attachment.SentToJira = True
                                TeamSupport.Data.Quarantine.ServiceQ.SetAttachmentSentToJira(User, attachment.AttachmentID)


                                ClearCrmLinkError(crmLinkError)
                            Catch ex As Exception
                                AddLog(String.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace),
                                    LogType.TextAndReport,
                                    crmLinkError,
                                    "Attachment could not be sent. " + ex.Message.ToString(),
                                    Orientation.OutToJira,
                                    ObjectType.Attachment,
                                    attachment.AttachmentID,
                                    "file",
                                    attachment.FileName,
                                    OperationType.Create)
                            End Try
                        End If
                    End If
                Next

            End Sub

            Private Function BuildCommentBody(ByVal ticketNumber As String, ByVal actionDescription As String, ByVal actionPosition As Integer, creatorId As Integer) As String
                Dim result As StringBuilder = New StringBuilder()
                Dim creatorUser As UsersViewItem = UsersView.GetUsersViewItem(User, creatorId)
                Dim creatorUserName As String = If(creatorUser IsNot Nothing, String.Format(" added by {0} {1}", creatorUser.FirstName, creatorUser.LastName), String.Empty)

                result.Append("TeamSupport ticket #" + ticketNumber.ToString() + " comment #" + actionPosition.ToString() + creatorUserName + ":")
                result.Append(Environment.NewLine)
                result.Append(Environment.NewLine)

                'Extract the code tag contents (if exists) and re-add after the html strip
                Dim codeSamples As New Dictionary(Of Integer, String)
                codeSamples = HtmlCleaner.ExtractCodeSamples(actionDescription)
                Dim addLines As Boolean = True
                actionDescription = HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(actionDescription), addLines)
                HtmlCleaner.AddCodeSamples(actionDescription, codeSamples)

                result.Append(actionDescription)

                Return result.ToString()
            End Function

            Private Sub PullIssuesAndCommentsAsTicketsAndActions(ByVal issuesToPullAsTickets As JArray,
                                                                 ByVal allStatuses As TicketStatuses,
                                                                 ByVal newActionsTypeID As Integer,
                                                                 ByRef ticketTypeMappings As CRMLinkFields)
                Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
                Dim ticketIds As List(Of Integer) = New List(Of Integer)()

                For i = 0 To issuesToPullAsTickets.Count - 1
                    ticketIds.AddRange(GetLinkedTicketIDs(issuesToPullAsTickets(i)))
                Next

                crmLinkErrors.LoadByOperationAndObjectIds(CRMLinkRow.OrganizationID,
                                                        CRMLinkRow.CRMType,
                                                        GetDescription(Orientation.IntoTeamSupport),
                                                        GetDescription(ObjectType.Ticket),
                                                        ticketIds.Select(Function(p) p.ToString()).ToList(),
                                                        isCleared:=False)

                Dim crmLinkError As CRMLinkError = Nothing
                Dim crmLinkActionErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
                crmLinkActionErrors.LoadByOperation(CRMLinkRow.OrganizationID,
                                                CRMLinkRow.CRMType,
                                                GetDescription(Orientation.IntoTeamSupport),
                                                GetDescription(ObjectType.Action),
                                                isCleared:=False)

                For i = 0 To issuesToPullAsTickets.Count - 1
                    Dim newComments As JArray = Nothing

                    For Each ticketID As Integer In GetLinkedTicketIDs(issuesToPullAsTickets(i))
                        Dim updateTicket As Tickets = New Tickets(User)
                        updateTicket.LoadByTicketID(ticketID)

                        If updateTicket.Count > 0 AndAlso updateTicket(0).OrganizationID = CRMLinkRow.OrganizationID Then
                            crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticketID.ToString(), String.Empty)

                            Try
                                UpdateTicketWithIssueData(ticketID, issuesToPullAsTickets(i), newActionsTypeID, allStatuses, crmLinkError, crmLinkErrors, ticketTypeMappings)
                                ClearCrmLinkError(crmLinkError)
                            Catch ex As Exception
                                AddLog(ex.ToString() + ex.StackTrace,
                                    LogType.Text,
                                    crmLinkError,
                                    "Error when updating ticket with Issue data.",
                                    Orientation.IntoTeamSupport,
                                    ObjectType.Ticket,
                                    ticketID.ToString(),
                                    String.Empty,
                                    JsonConvert.SerializeObject(issuesToPullAsTickets(i)),
                                    OperationType.Update)
                            End Try

                            If newComments Is Nothing Then
                                newComments = GetNewComments(issuesToPullAsTickets(i)("fields")("comment"), ticketID)
                            End If

                            AddNewCommentsInTicket(ticketID, newComments, newActionsTypeID, crmLinkActionErrors)
                        ElseIf updateTicket.Count > 0 Then
                            AddLog("Ticket with ID: """ + ticketID.ToString() + """ belongs to a different organization and was not updated.")
                        Else
                            AddLog("Ticket with ID: """ + ticketID.ToString() + """ was not found to be updated.")
                        End If
                    Next
                Next
            End Sub

            Private Function GetNewActionsTypeID(ByVal organizationID As Integer) As Integer
                Dim result As Integer = 0
                Dim actionTypes As ActionTypes = New ActionTypes(User)
                actionTypes.LoadByOrganizationID(organizationID)

                Dim newActionType As ActionType = actionTypes.FindByName("Comment")
                If newActionType IsNot Nothing Then
                    result = newActionType.ActionTypeID
                Else
                    actionTypes = New ActionTypes(User)
                    actionTypes.LoadByPosition(organizationID, 0)
                    result = actionTypes(0).ActionTypeID
                End If

                Return result
            End Function

            Private Function GetLinkedTicketIDs(ByVal issue As JObject) As List(Of Integer)
                Dim result As List(Of Integer) = New List(Of Integer)()
                Dim URI As String = _baseURI + "/issue/" + issue("id").ToString() + "/remotelink"
                Dim remoteLinks As JArray = GetAPIJArray(URI, "GET", String.Empty)

                Log.Write("remoteLinks.Count: " + remoteLinks.Count.ToString())
                For i = 0 To remoteLinks.Count - 1
                    If remoteLinks(i)("application")("name") = "Team Support" Then
                        Dim remoteLinkURL As String = remoteLinks(i)("object")("url").ToString()
                        Log.Write("remoteLinkURL: " + remoteLinkURL)

                        Try
                            Dim remoteLinkSplit() As String = Split(remoteLinkURL, "ticketid=")
                            result.Add(CType(remoteLinkSplit(1), Integer))
                        Catch ex As Exception
                            Log.Write("ticketid= string not found in remotelink. Skipping issue " + issue("id").ToString() + ", please review.")
                        End Try
                    End If
                Next

                Return result
            End Function

            Private Function BuildRequiredFields(ByVal ticket As TicketsViewItem,
                                            ByRef fields As JObject,
                                            ByRef customMappingFields As CRMLinkFields,
                                            ByRef crmLinkErrors As CRMLinkErrors)
                Dim result As StringBuilder = New StringBuilder()
                Dim crmLinkError As CRMLinkError

                Try
                    If (fields IsNot Nothing) Then
                        Dim requiredFields As List(Of String) = New List(Of String)()

                        For Each field As KeyValuePair(Of String, JToken) In fields
                            If field.Value("required") Then
                                If field.Key = "summary" OrElse field.Key = "issuetype" OrElse field.Key = "project" OrElse field.Key = "description" Then
                                    Continue For
                                End If

                                Dim value As String = Nothing
                                Dim fieldName As String = field.Value("name").ToString()
                                Dim notIncludedMessage As String = String.Empty
                                crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID.ToString(), fieldName)
                                Dim cRMLinkField As CRMLinkField = customMappingFields.FindByCRMFieldName(fieldName)
                                requiredFields.Add(fieldName)

                                If cRMLinkField IsNot Nothing Then
                                    If cRMLinkField.CustomFieldID IsNot Nothing Then
                                        Dim findCustom As New CustomValues(User)
                                        findCustom.LoadByFieldID(cRMLinkField.CustomFieldID, ticket.TicketID)
                                        If findCustom.Count > 0 Then
                                            value = GetDataLineValue(field.Key.ToString(), field.Value("schema")("custom"), findCustom(0).Value)
                                        Else
                                            notIncludedMessage = GetFieldNotIncludedMessage(ticket.TicketID, fieldName, findCustom.Count = 0)
                                        End If
                                    ElseIf cRMLinkField.TSFieldName IsNot Nothing Then
                                        If ticket.Row(cRMLinkField.TSFieldName) IsNot Nothing Then
                                            value = GetDataLineValue(field.Key.ToString(), field.Value("schema")("custom"), ticket.Row(cRMLinkField.TSFieldName))
                                        Else
                                            notIncludedMessage = GetFieldNotIncludedMessage(ticket.TicketID, fieldName, ticket.Row(cRMLinkField.TSFieldName) Is Nothing)
                                        End If
                                    Else
                                        AddLog("TicketID " + ticket.TicketID.ToString() + "'s field '" + fieldName + "' was not included because custom field " +
                                            cRMLinkField.CRMFieldID.ToString() + " CustomFieldID and TSFieldName are null.")
                                    End If
                                End If

                                If value IsNot Nothing Then
                                    If result.ToString().Trim().Length > 0 Then
                                        result.Append(",")
                                    End If

                                    result.Append("""" + field.Key.ToString() + """:" + value)
                                    ClearCrmLinkError(crmLinkError)
                                Else
                                    AddLog("No value found for the required field " + fieldName,
                                    LogType.TextAndReport,
                                    crmLinkError,
                                    If(String.IsNullOrEmpty(notIncludedMessage), "No value found for the required field " + fieldName, notIncludedMessage),
                                    Orientation.OutToJira,
                                    ObjectType.Ticket,
                                    ticket.TicketID,
                                    fieldName,
                                    Nothing,
                                    OperationType.Update)
                                End If
                            End If
                        Next

                        '//Clear required fields errors that are not required anymore
                        crmLinkErrors.ClearRequiredFieldErrors(ticket.TicketID, requiredFields)
                    Else
                        AddLog("Cannot build the required fields because there is no fields returned from Jira for the project and issue type to check.")
                    End If
                Catch ex As Exception
                    Dim requiredField As String = String.Empty
                    If (fields IsNot Nothing) Then
                        requiredField = fields.ToString()
                    End If

                    AddLog("Exception building the required fields: " + requiredField + Environment.NewLine + ex.ToString() + ex.StackTrace)
                End Try

                Return result
            End Function

            Private Sub UpdateTicketWithIssueData(ByVal ticketID As Integer,
                                                  ByVal issue As JObject,
                                                  ByVal newActionsTypeID As Integer,
                                                  ByVal allStatuses As TicketStatuses,
                                                  ByRef crmLinkError As CRMLinkError,
                                                  ByRef crmlinkErrors As CRMLinkErrors,
                                                  ByRef ticketTypeMappings As CRMLinkFields)
                Dim updateTicket As Tickets = New Tickets(User)
                updateTicket.LoadByTicketID(ticketID)

                If updateTicket.Count > 0 AndAlso updateTicket(0).OrganizationID = CRMLinkRow.OrganizationID Then
					Dim newTicketType As TicketType
					Dim customFields As New CRMLinkFields(User)
                    Dim allTypes As TicketTypes = New TicketTypes(User)
                    Dim ticketLinkToJira As TicketLinkToJira = New TicketLinkToJira(User)
					Dim issueTypeName As String = String.Empty
					ticketLinkToJira.LoadByTicketID(updateTicket(0).TicketID)
                    allTypes.LoadByOrganizationID(CRMLinkRow.OrganizationID)

                    For Each field As KeyValuePair(Of String, JToken) In CType(issue("fields"), JObject)
                        If field.Key.Trim().ToLower() = "issuetype" Then
							issueTypeName = GetFieldValue(field)
							Dim mappedTicketType As CRMLinkField = ticketTypeMappings.FindByCRMFieldName(issueTypeName)

							If mappedTicketType IsNot Nothing Then
								newTicketType = allTypes.FindByName(mappedTicketType.TSFieldName)
							Else
								newTicketType = allTypes.FindByName(issueTypeName)
							End If

							If newTicketType IsNot Nothing Then
								customFields.LoadByObjectTypeAndCustomFieldAuxID(GetDescription(ObjectType.Ticket), CRMLinkRow.CRMLinkID, newTicketType.TicketTypeID)
							End If

                            Exit For
                        End If
                    Next

                    Dim ticketValuesChanged = False
                    Dim issueStatusChangedNotUpdatedInTicket = False
                    Dim ticketView As TicketsView = New TicketsView(User)
                    ticketView.LoadByTicketID(ticketID)
                    Dim jiraProjectKey As String = GetProjectKey(ticketView(0), crmLinkErrors)
					Dim issueFields As JObject = GetIssueFields(ticketView(0), jiraProjectKey, crmLinkError, Orientation.IntoTeamSupport, issueTypeName)
					Dim ticketsFieldMap As Tickets = New Tickets(User)

					If newTicketType Is Nothing Then
						customFields.LoadByObjectTypeAndCustomFieldAuxID(GetDescription(ObjectType.Ticket), CRMLinkRow.CRMLinkID, updateTicket(0).TicketTypeID)
					End If

					For Each field As KeyValuePair(Of String, JToken) In CType(issue("fields"), JObject)
                        Dim value As String = Nothing
                        Dim cRMLinkField As CRMLinkField = customFields.FindByCRMFieldName(GetFieldNameByKey(field.Key.ToString(), issueFields))
                        Dim crmLinkCustomFieldError As CRMLinkError = Nothing

                        Try
                            'Verify the field is mapped or part of the Select Case below (if more added there then add them to this check too)
                            Dim isCustomMappedField As Boolean = cRMLinkField IsNot Nothing _
                                                            OrElse field.Key.Trim().ToLower() = "issuetype" _
                                                            OrElse field.Key.Trim().ToLower() = "project" _
                                                            OrElse field.Key.Trim().ToLower() = "status"

                            If (isCustomMappedField) Then
                                crmLinkCustomFieldError = crmLinkErrors.FindByObjectIDAndFieldName(ticketID.ToString(), field.Key)
                                value = GetFieldValue(field)
                            Else
                                'Uncomment this line in case more logging is needed to troubleshoot fields not synced
                                'AddLog(String.Format("Issue field {0} is not mapped, so it was not processed.", field.Key))
                                Continue For
                            End If

                            ClearCrmLinkError(crmLinkCustomFieldError)
                        Catch ex As Exception
                            AddLog(ex.ToString() + ex.StackTrace,
                            LogType.TextAndReport,
                            crmLinkCustomFieldError,
                            String.Format("Field: ""{0}"" was not updated because the following exception ocurred getting its value: {1}", field.Key, ex.StackTrace),
                            Orientation.IntoTeamSupport,
                            ObjectType.Ticket,
                            updateTicket(0).TicketID.ToString(),
                            field.Key,
                            Nothing,
                            OperationType.Update)

                            Continue For
                        End Try

                        If cRMLinkField IsNot Nothing Then
                            Try
                                Dim fieldMapItem As FieldMapItem = ticketsFieldMap.FieldMap.Items.Find(Function(c) c.PrivateName = cRMLinkField.TSFieldName)
                                Dim canBeUpdated As Boolean = True

                                If fieldMapItem IsNot Nothing AndAlso (Not fieldMapItem.Insert Or Not fieldMapItem.Update) Then
                                    canBeUpdated = False
                                End If

                                Dim translatedFieldValue As String = value

                                If cRMLinkField.CustomFieldID IsNot Nothing Then
                                    Dim findCustom As New CustomValues(User)
                                    Dim thisCustom As CustomValue

                                    findCustom.LoadByFieldID(cRMLinkField.CustomFieldID, ticketID)

                                    If findCustom.Count > 0 Then
                                        thisCustom = findCustom(0)
                                    Else
                                        thisCustom = (New CustomValues(User)).AddNewCustomValue()
                                        thisCustom.CustomFieldID = cRMLinkField.CustomFieldID
                                        thisCustom.RefID = ticketID
                                    End If

                                    If (IsDBNull(thisCustom.Value) OrElse thisCustom.Value <> translatedFieldValue) And canBeUpdated Then
                                        thisCustom.Value = translatedFieldValue
                                        thisCustom.Collection.Save()
                                        ticketValuesChanged = True
                                    End If
                                ElseIf cRMLinkField.TSFieldName IsNot Nothing Then
                                    Try
                                        If (IsDBNull(updateTicket(0).Row(cRMLinkField.TSFieldName)) OrElse updateTicket(0).Row(cRMLinkField.TSFieldName) <> translatedFieldValue) And canBeUpdated Then
                                            updateTicket(0).Row(cRMLinkField.TSFieldName) = translatedFieldValue
                                            ticketValuesChanged = True
                                        End If
                                    Catch ex As Exception
                                        'When a ticket view field is mapped an exception migth be thrown when looking for it in the Tickets table
                                        'The following is to look for its reference field in the Tickets table (e.g. Severity ? TicketSeverityID) and the related value.
                                        Dim ticketsTableRelatedValue As Integer? = Nothing
                                        Dim ticketsTableRelatedFieldName As String = GetTicketsTableRelatedFieldName(cRMLinkField.TSFieldName, translatedFieldValue, ticketsTableRelatedValue)

                                        If Not String.IsNullOrEmpty(ticketsTableRelatedFieldName) AndAlso Not ticketsTableRelatedValue Is Nothing Then
                                            If IsDBNull(updateTicket(0).Row(ticketsTableRelatedFieldName)) OrElse updateTicket(0).Row(ticketsTableRelatedFieldName) <> ticketsTableRelatedValue Then
                                                updateTicket(0).Row(ticketsTableRelatedFieldName) = ticketsTableRelatedValue
                                                ticketValuesChanged = True
                                            End If
                                        End If
                                    End Try
                                End If
                            Catch mappingException As Exception
                                AddLog("The following exception was caught mapping the ticket field """ &
                                        field.Key &
                                        """ with """ &
                                        cRMLinkField.TSFieldName &
                                        """: " &
                                        mappingException.Message &
                                        " " & mappingException.StackTrace)
                            End Try
                        Else
                            Select Case field.Key.Trim().ToLower()
                                Case "issuetype"
                                    Dim currentType As TicketType = allTypes.FindByTicketTypeID(updateTicket(0).TicketTypeID)
									Dim updateType As Boolean = CRMLinkRow.UpdateTicketType

									If updateType AndAlso newTicketType IsNot Nothing AndAlso newTicketType.TicketTypeID <> currentType.TicketTypeID Then
										updateTicket(0).TicketTypeID = newTicketType.TicketTypeID
										ticketValuesChanged = True
									End If
								Case "project"
                                    Dim allProducts As Products = New Products(User)
                                    allProducts.LoadByOrganizationID(CRMLinkRow.OrganizationID)
                                    Dim newProduct As Product = allProducts.FindByName(value)

                                    If newProduct IsNot Nothing Then
                                        If updateTicket(0).ProductID IsNot Nothing Then
                                            If newProduct.ProductID <> updateTicket(0).ProductID Then
                                                updateTicket(0).ProductID = newProduct.ProductID
                                                ticketValuesChanged = True
                                            End If
                                        Else
                                            updateTicket(0).ProductID = newProduct.ProductID
                                            ticketValuesChanged = True
                                        End If
                                    End If
                                Case "status"
                                    Dim currentStatus As TicketStatus = allStatuses.FindByTicketStatusID(updateTicket(0).TicketStatusID)
                                    Dim newStatus As TicketStatus

									If newTicketType IsNot Nothing Then
										newStatus = allStatuses.FindByName(value, newTicketType.TicketTypeID)
									Else
										newStatus = allStatuses.FindByName(value, updateTicket(0).TicketTypeID)
                                    End If
                                    Dim isCurrentStatusExcluded As Boolean = False

                                    If (Not String.IsNullOrEmpty(CRMLinkRow.ExcludedTicketStatusUpdate)) Then
                                        Dim exclusionStatusList As List(Of Integer) = New List(Of Integer)

                                        For Each statusExcluded As String In CRMLinkRow.ExcludedTicketStatusUpdate.Split(",")
                                            exclusionStatusList.Add(CInt(statusExcluded))
                                        Next

                                        isCurrentStatusExcluded = exclusionStatusList.Where(Function(p) p = currentStatus.TicketStatusID).Any()
                                    End If

                                    If CRMLinkRow.UpdateStatus _
                                    AndAlso newStatus IsNot Nothing _
                                    AndAlso newStatus.TicketStatusID <> currentStatus.TicketStatusID _
                                    AndAlso Not isCurrentStatusExcluded Then
                                        updateTicket(0).TicketStatusID = newStatus.TicketStatusID
                                        ticketValuesChanged = True
                                    ElseIf Not CRMLinkRow.UpdateStatus Then
                                        If ticketLinkToJira.Count > 0 AndAlso (ticketLinkToJira(0).JiraStatus Is Nothing OrElse ticketLinkToJira(0).JiraStatus <> value) Then
                                            Dim fromStatement As String = String.Empty

                                            If ticketLinkToJira(0).JiraStatus IsNot Nothing Then
                                                fromStatement = " from """ + ticketLinkToJira(0).JiraStatus + """"
                                            End If

                                            Dim newAction As Actions = New Actions(User)
                                            newAction.AddNewAction()
                                            newAction(0).ActionTypeID = newActionsTypeID
                                            newAction(0).TicketID = updateTicket(0).TicketID
                                            newAction(0).Description = "Jira's Issue " + issue("key").ToString() + "'s status changed" + fromStatement + " to """ + value + """."

                                            Dim actionLinkToJira As ActionLinkToJira = New ActionLinkToJira(User)
                                            Dim actionLinkToJiraItem As ActionLinkToJiraItem = actionLinkToJira.AddNewActionLinkToJiraItem()

                                            actionLinkToJiraItem.JiraID = -1
                                            actionLinkToJiraItem.DateModifiedByJiraSync = DateTime.UtcNow()
                                            newAction.Save()
                                            actionLinkToJiraItem.ActionID = newAction(0).ActionID
                                            actionLinkToJira.Save()
                                            ticketValuesChanged = True
                                        End If
                                    End If

                                    If (ticketLinkToJira(0).JiraStatus.ToLower().Trim() <> value.ToLower().Trim()) Then
                                        ticketLinkToJira(0).JiraStatus = value
                                        issueStatusChangedNotUpdatedInTicket = True
                                    End If
                            End Select
                        End If
                    Next

                    If ticketValuesChanged Then
                        ticketLinkToJira(0).DateModifiedByJiraSync = DateTime.UtcNow
                        updateTicket.Save()
                        ticketLinkToJira.Save()

                        Dim actionLogDescription As String = "Updated Ticket with Jira Issue Key: '" + issue("key").ToString() + "' changes."
                        ActionLogs.AddActionLog(User, ActionLogType.Update, ReferenceType.Tickets, updateTicket(0).TicketID, actionLogDescription)
                    Else
                        AddLog("ticketID: " + updateTicket(0).TicketID.ToString() + " values were not updated because have not changed.")

                        If issueStatusChangedNotUpdatedInTicket Then
                            ticketLinkToJira(0).DateModifiedByJiraSync = DateTime.UtcNow
                            ticketLinkToJira.Save()
                            AddLog("The ticket status in the TicketLinkToJira was updated because it changed but was not updated in TeamSupport because either it is setup to be excluded or it does not exist. JiraStatus:" + ticketLinkToJira(0).JiraStatus)
                        End If
                    End If
                Else
                    AddLog("Ticket with ID: """ + ticketID.ToString() + """ was not found to be updated.")
                End If
            End Sub

            Private Function GetFieldNameByKey(ByVal fieldKey As String, ByVal issueFields As JObject)
                Dim result As StringBuilder = New StringBuilder()

                If issueFields IsNot Nothing Then
                    For Each field As KeyValuePair(Of String, JToken) In issueFields
                        If field.Key IsNot Nothing AndAlso fieldKey = field.Key Then
                            result.Append(field.Value("name").ToString())
                            Exit For
                        End If
                    Next
                End If

                If result.Length = 0 Then
                    result.Append(fieldKey)
                End If

                Return result.ToString()
            End Function

            Private Function GetFieldValue(ByVal field As KeyValuePair(Of String, JToken)) As String
                Dim result As String = Nothing

                If field.Key.ToString().ToLower().Contains("customfield") = True Then
                    If field.Value.HasValues AndAlso TypeOf (field.Value) Is JObject Then
                        result = field.Value("value").ToString()
                    ElseIf field.Value.HasValues AndAlso TypeOf (field.Value) Is JArray Then
                        Try
                            Dim fieldValue = field.Value(0).ToString()
                            Dim fieldValueDictionary = fieldValue.Split(","c).Select(Function(kvp) kvp.Split("="c)).ToDictionary(
                                                                                                                    Function(kvp) kvp(0),
                                                                                                                    Function(kvp) kvp(1))

                            If (fieldValueDictionary.Where(Function(p) p.Key.ToLower() = "name").Any()) Then
                                result = fieldValueDictionary.Where(Function(p) p.Key.ToLower() = "name").Select(Function(p) p.Value).FirstOrDefault()
                            Else
                                result = field.Value.ToString()
                            End If
                        Catch ex As Exception
                            Try
                                'ToDo //vv This probably is a multiselect jira field. For now we are only taking the first value. We need to handle those fields correctly
                                result = field.Value(0)("value").ToString()
                            Catch innerEx As Exception
                                AddLog(String.Format("Unhandled Jira field type {0} with value: {1}{2}", field.Key.ToString(), Environment.NewLine, field.Value(0).ToString()))
                            End Try

                        End Try

                    Else
                        result = field.Value.ToString()
                    End If

                    If result.ToLower() = "yes" Then
                        result = "True"
                    ElseIf result.ToLower() = "no" Then
                        result = "False"
                    End If
                Else
                    Select Case field.Key.ToString().ToLower()
                        Case "aggregatetimeestimate",
                    "aggregatetimeoriginalestimate",
                    "aggregatetimespent",
                    "created",
                    "description",
                    "duedate",
                    "environment",
                    "lastviewed",
                    "resolutiondate",
                    "summary",
                    "updated",
                    "workratio",
                    "timeestimate",
                    "timeoriginalestimate",
                    "timespent"
                            result = field.Value.ToString()
                        Case "assignee"
                            'Because some orgs are mapping to the Assignee Jira field and they seem to be storing the email address then we need to leave it here, for the rest we should use the Name so it also links from TS to Jira when creating the issue
                            If (CRMLinkRow.OrganizationID = 461956 OrElse CRMLinkRow.OrganizationID = 869700 OrElse CRMLinkRow.OrganizationID = 884116) Then
                                result = field.Value("emailAddress").ToString()
                            Else
                                result = field.Value("name").ToString()
                            End If
                        Case "reporter"
                            'Because some orgs are mapping to the Assignee Jira field and they seem to be storing the email address then we need to leave it here, for the rest we should use the Name so it also links from TS to Jira when creating the issue
                            If (CRMLinkRow.OrganizationID = 930653 OrElse CRMLinkRow.OrganizationID = 1028984 OrElse CRMLinkRow.OrganizationID = 1136748 OrElse CRMLinkRow.OrganizationID = 995322) Then
                                result = field.Value("emailAddress").ToString()
                            Else
                                result = field.Value("name").ToString()
                            End If
                        Case "issuetype", "status", "priority", "resolution"
                            result = field.Value("name").ToString()
				Case "progress", "worklog"
					result = field.Value("total").ToString()
				Case "project"
					result = field.Value("key").ToString()
				Case "votes"
					result = field.Value("votes").ToString()
				Case "watches"
					result = field.Value("watchCount").ToString()
				Case "timetracking"
					result = field.Value("timeSpentSeconds").ToString()
				Case "aggregrateprogress"
					result = field.Value("progress").ToString()
				Case "attachment"
					Dim attachmentsArray As JArray = DirectCast(field.Value, JArray)
					Dim resultBuilder As StringBuilder = New StringBuilder()
					Dim preffix = String.Empty

					For i = 0 To attachmentsArray.Count - 1
						resultBuilder.Append(preffix)
						resultBuilder.Append(attachmentsArray(i)("content").ToString())

						If preffix = String.Empty Then
							preffix = ", "
						End If
					Next

					result = resultBuilder.ToString()
				Case "labels"
					Dim labelsArray As JArray = DirectCast(field.Value, JArray)
					Dim resultBuilder As StringBuilder = New StringBuilder()
					Dim preffix = String.Empty

					For i = 0 To labelsArray.Count - 1
						resultBuilder.Append(preffix)
						resultBuilder.Append(labelsArray(i).ToString())

						If preffix = String.Empty Then
							preffix = ", "
						End If
					Next

					result = resultBuilder.ToString()
				Case "issuelinks"
					Dim issuelinksArray As JArray = DirectCast(field.Value, JArray)
					Dim resultBuilder As StringBuilder = New StringBuilder()
					Dim preffix = String.Empty

					For i = 0 To issuelinksArray.Count - 1
						resultBuilder.Append(preffix)

						If issuelinksArray(i)("outwardIssue") IsNot Nothing Then
							resultBuilder.Append(issuelinksArray(i)("outwardIssue")("key").ToString())
						Else
							resultBuilder.Append(issuelinksArray(i)("inwardIssue")("key").ToString())
						End If

						If preffix = String.Empty Then
							preffix = ", "
						End If
					Next
					
					result = resultBuilder.ToString()
				Case "versions", "fixversions"
					Dim versionsArray As JArray = DirectCast(field.Value, JArray)
					Dim resultBuilder As StringBuilder = New StringBuilder()
					Dim preffix = String.Empty

					For i = 0 To versionsArray.Count - 1
						resultBuilder.Append(preffix)

						Try
							resultBuilder.Append(versionsArray(i)("name").ToString())
						Catch ex As Exception
							resultBuilder.Append(versionsArray(i)("description").ToString())
						End Try

						If preffix = String.Empty Then
							preffix = ", "
						End If
					Next

					result = resultBuilder.ToString()
				Case "subtasks"
					Dim subtasksArray As JArray = DirectCast(field.Value, JArray)
					Dim resultBuilder As StringBuilder = New StringBuilder()
					Dim preffix = String.Empty

					For i = 0 To subtasksArray.Count - 1
						resultBuilder.Append(preffix)
						resultBuilder.Append(subtasksArray(i)("fields")("summary").ToString())

						If preffix = String.Empty Then
							preffix = ", "
						End If
					Next

					result = resultBuilder.ToString()
				Case "components"
					Dim componentsArray As JArray = DirectCast(field.Value, JArray)
					Dim resultBuilder As StringBuilder = New StringBuilder()
					Dim preffix = String.Empty
					
					For i = 0 To componentsArray.Count - 1
						resultBuilder.Append(preffix)
						resultBuilder.Append(componentsArray(i)("name").ToString())

						If preffix = String.Empty Then
							preffix = ", "
						End If
					Next

					result = resultBuilder.ToString()
				End Select
			End If

			Return result
		End Function

            Private Function GetTicketsTableRelatedFieldName(ByVal mappedFieldName As String, ByVal value As String, ByRef ticketsTableRelatedValue As Integer?)
                Dim result As StringBuilder = New StringBuilder()
                Select Case mappedFieldName
                    'This is linked to issue product as long as status and type
                    'Case "ProductName"
                    '  result.Append("ProductID")
                    '  Dim products As Products = New Products(User)
                    '  products.LoadByProductName(CRMLinkRow.OrganizationID, value)
                    '  If products.Count > 0 Then
                    '    ticketsTableRelatedValue = products(0).ProductID
                    '  End If
                    Case "GroupName"
                        result.Append("GroupID")
                        Dim groups As Groups = New Groups(User)
                        groups.LoadByGroupName(CRMLinkRow.OrganizationID, value, 1)
                        If groups.Count > 0 Then
                            ticketsTableRelatedValue = groups(0).GroupID
                        End If
                    Case "UserName"
                        result.Append("UserID")
                        Dim givenUser As Users = New Users(User)
                        givenUser.LoadByName(value, CRMLinkRow.OrganizationID, False, False, False)
                        If givenUser.Count > 0 Then
                            ticketsTableRelatedValue = givenUser(0).UserID
                        End If
                    Case "Severity"
                        result.Append("TicketSeverityID")
                        Dim severity As TicketSeverities = New TicketSeverities(User)
                        severity.LoadByName(CRMLinkRow.OrganizationID, value)
                        If severity.Count > 0 Then
                            ticketsTableRelatedValue = severity(0).TicketSeverityID
                        End If
                End Select
                Return result.ToString()
            End Function

            Private Function GetNewComments(ByVal comments As JObject, ByVal ticketID As Integer) As JArray
                Dim result As JArray = New JArray()

                Dim ticketActionsLinked As ActionLinkToJira = New ActionLinkToJira(User)
                ticketActionsLinked.LoadByTicketID(ticketID)

                For i = 0 To comments("comments").Count - 1
                    If GetIsNewComment(comments("comments")(i), ticketActionsLinked) Then
                        result.Add(comments("comments")(i))
                    End If
                Next
                Return result
            End Function

            Private Function GetIsNewComment(ByVal comment As JObject, ByVal ticketActionsLinked As ActionLinkToJira) As Boolean
                Dim result As Boolean = False

                If comment("visibility") Is Nothing Then
                    If comment("body").ToString().Length < 20 OrElse comment("body").ToString().Substring(0, 20) <> "TeamSupport ticket #" Then
                        Dim pulledComment As ActionLinkToJiraItem = ticketActionsLinked.FindByJiraID(CType(comment("id").ToString(), Integer?))
                        If pulledComment Is Nothing Then
                            result = True
                        End If
                    End If
                End If

                Return result
            End Function

		Private Sub AddNewCommentsInTicket(ByVal ticketID As Integer, ByRef newComments As JArray, ByVal newActionsTypeID As Integer, ByRef crmLinkActionErrors As CRMLinkErrors)
			Dim crmLinkError As CRMLinkError = Nothing

			For i = 0 To newComments.Count - 1
				crmLinkError = crmLinkActionErrors.FindByObjectIDAndFieldName(newComments(i)("id").ToString(), String.Empty)

				Try
					Dim updateActions As Actions = New Actions(User)
					updateActions.AddNewAction()
					updateActions(0).TicketID = ticketID
					updateActions(0).ActionTypeID = newActionsTypeID

					Dim commentDescription = newComments(i)("body").ToString()
					Dim firstLine As String = "<p><em>Comment added in Jira {0}{1}</em></p> <p>&nbsp;</p>"
					Dim author As String = String.Empty

					Try
						author = newComments(i)("author")("displayName").ToString()
					Catch ex As Exception
						AddLog("The author displayName was not found for the comment.")
					End Try

					Dim addedOnJiraString As String = String.Empty

					Try
						Dim addedOnJira As Date = Convert.ToDateTime(newComments(i)("created"))

						If (DateDiff(DateInterval.Day, Today.Date, addedOnJira.Date) <> 0) Then
							addedOnJiraString = addedOnJira.ToString()
						End If
					Catch ex As Exception
						AddLog("The created date was not found for the comment.")
					End Try

					firstLine = String.Format(firstLine,
												If(String.IsNullOrEmpty(author), "", "by " + author),
												If(String.IsNullOrEmpty(addedOnJiraString), "", " on " + addedOnJiraString))
					Dim jiraCommentId = CType(newComments(i)("id").ToString(), Integer)
					commentDescription = firstLine + commentDescription
					updateActions(0).Description = commentDescription
					updateActions.ActionLogInstantMessage = "Jira Comment ID: " + jiraCommentId.ToString() + " Created In TeamSupport Action "
					Dim actionLinkToJira As ActionLinkToJira = New ActionLinkToJira(User)
					Dim actionLinkToJiraItem As ActionLinkToJiraItem = actionLinkToJira.AddNewActionLinkToJiraItem()
					actionLinkToJiraItem.JiraID = jiraCommentId
					actionLinkToJiraItem.DateModifiedByJiraSync = DateTime.UtcNow
					updateActions.Save()
					actionLinkToJiraItem.ActionID = updateActions(0).ActionID
					actionLinkToJira.Save()

					ClearCrmLinkError(crmLinkError)
				Catch ex As Exception
                        AddLog(ex.ToString() + ex.StackTrace,
							LogType.TextAndReport,
							crmLinkError,
                            ex.Message,
                            Orientation.IntoTeamSupport,
                            ObjectType.Action,
                            newComments(i)("id").ToString(),
                            String.Empty,
                            newComments(i)("body").ToString(),
							OperationType.Create)
				End Try
			Next
		End Sub

            Private Sub AddLog(ByVal exception As String,
                            Optional ByVal logTo As LogType = LogType.Text,
                            Optional ByRef crmLinkError As CRMLinkError = Nothing,
                            Optional ByRef errorMessage As String = Nothing,
                            Optional ByVal orientation As Orientation = Orientation.OutToJira,
                            Optional ByVal objectType As ObjectType = ObjectType.Unknown,
                            Optional ByVal objectId As Integer = 0,
                            Optional ByVal objectFieldName As String = "",
                            Optional ByVal objectData As String = Nothing,
							Optional ByVal operationType As OperationType = OperationType.Unknown)

			If (logTo = LogType.Text OrElse logTo = LogType.TextAndReport) Then
				Log.Write(exception)
			End If
			
			If (logTo = LogType.Report OrElse logTo = LogType.TextAndReport) Then
				Try
					If crmLinkError Is Nothing Then
						Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
						crmLinkError = newCrmLinkError.AddNewCRMLinkError()
						crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
						crmLinkError.CRMType = CRMLinkRow.CRMType
						crmLinkError.Orientation = GetDescription(orientation)
						crmLinkError.ObjectType = GetDescription(objectType)
						crmLinkError.ObjectID = objectId.ToString()
						crmLinkError.ObjectFieldName = objectFieldName
						crmLinkError.ObjectData = objectData
						crmLinkError.Exception = exception
						crmLinkError.OperationType = GetDescription(operationType)
						crmLinkError.ErrorCount = 1
						crmLinkError.IsCleared = False
						crmLinkError.ErrorMessage = errorMessage
						newCrmLinkError.Save()
					Else
						crmLinkError.ErrorCount = crmLinkError.ErrorCount + 1
						crmLinkError.IsCleared = False
						crmLinkError.ObjectData = objectData
						crmLinkError.Exception = exception
						crmLinkError.Collection.Save()
					End If
				Catch errorException As Exception
					Log.Write(errorException.ToString() + errorException.StackTrace)
				End Try
			End If
		End Sub

		Shared Function GetDescription(ByVal EnumConstant As [Enum]) As String
			Dim fi As FieldInfo = EnumConstant.GetType().GetField(EnumConstant.ToString())
			Dim attr() As DescriptionAttribute = DirectCast(
			fi.GetCustomAttributes(GetType(DescriptionAttribute), False), 
			DescriptionAttribute())

			If attr.Length > 0 Then
				Return attr(0).Description
			Else
				Return EnumConstant.ToString()
			End If
		End Function

		Private Function ClearCrmLinkError(ByRef crmLinkError As CRMLinkError)
			If crmLinkError IsNot Nothing Then
				crmLinkError.IsCleared = True
				crmLinkError.DateModified = DateTime.UtcNow()
				crmLinkError.Collection.Save()
			End If
		End Function

		<Obsolete("This method will be deprecated, use GetIssuesToPullAsTickets that POSTs the search instead.") >
      Private Function GetIssuesToPullAsTickets(ByRef numberOfIssuesToPull As Integer) As List(Of JObject)
        Dim result As List(Of JObject) = New List(Of JObject)

        Dim recentClause As String = String.Empty
        If CRMLinkRow.LastLink IsNot Nothing Then
          recentClause = "updated>-" + GetMinutesSinceLastLink().ToString() + "m+"
        Else
          Dim minutesSinceFirstSyncedTicket As Integer = GetMinutesSinceFirstSyncedTicket()
          If minutesSinceFirstSyncedTicket > 0 Then
            recentClause = "updated>-" + minutesSinceFirstSyncedTicket.ToString() + "m+"
          Else
            Log.Write("No tickets have been synced, therefore no issues to pull exist.")
            Return result
          End If
        End If

		Dim needToGetMore As Boolean = True
		Dim startAt As String = String.Empty
        Dim maxResults As Integer? = Nothing

        While needToGetMore
          Dim URI As String = _baseURI + "/search?jql=" + recentClause + "order+by+updated+asc&fields=*all" + startAt
          Dim batch As JObject = GetAPIJObject(URI, "GET", String.Empty)
          result.Add(batch)

          Dim batchTotal As Integer = batch("issues").Count
          numberOfIssuesToPull += batchTotal

          If maxResults Is Nothing Then
            maxResults = CType(batch("maxResults").ToString(), Integer?)
          End If

          If batchTotal = maxResults Then
            startAt = "&startAt=" + (numberOfIssuesToPull).ToString()
          Else
            needToGetMore = False
          End If
        End While

        Log.Write("Got " + numberOfIssuesToPull.ToString() + " Issues To Pull As Tickets.")
        Return result
      End Function

		Private Class IssueTypeFields
			Private _issueType As String
			Private _project As String

			Property IssueType As String
				Get
					Return _issueType
				End Get
				Set(ByVal value As String)
					_issueType = value
				End Set
			End Property

			Property Project As String
				Get
					Return _project
				End Get
				Set(ByVal value As String)
					_project = value
				End Set
			End Property
		End Class
	End Class

		Public Enum LogType
			Text
			Report
			TextAndReport
		End Enum

		Public Enum Orientation
			<Description("in")>
			IntoTeamSupport
            <Description("out")>
            OutToJira
            <Description("out")>
            OutToTFS
        End Enum

		Public Enum OperationType
			<Description("unknown")>
			Unknown
			<Description("login")>
			Login
			<Description("create")>
			Create
			<Description("update")>
			Update
		End Enum

		Public Enum ObjectType
			<Description("")>
			Unknown
			<Description("ticket")>
			Ticket
			<Description("action")>
			Action
			<Description("attachment")>
			Attachment
		End Enum
  End Namespace
End Namespace