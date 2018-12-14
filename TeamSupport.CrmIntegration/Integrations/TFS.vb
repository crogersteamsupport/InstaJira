Imports Newtonsoft.Json.Linq
Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Reflection
Imports System.Text
Imports TeamSupport.Data
Imports Newtonsoft.Json
Imports Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
Imports TFSLibrary = TeamSupport.ServiceLibrary.TFS

Namespace TeamSupport
	Namespace CrmIntegration
		Public Module TFSExtensions
			<Extension()>
			Public Sub Add(Of T)(ByRef arr As T(), item As T)
				Array.Resize(arr, arr.Length + 1)
				arr(arr.Length - 1) = item
			End Sub

			<Extension()>
			Public Function SplitBy(Of T)(ByRef source As List(Of T), ByVal splitSize As Integer) As List(Of List(Of T))
				Return source.Select(Function(x, i) New With {.Index = i, .Value = x}) _
				.GroupBy(Function(x) x.Index \ splitSize) _
				.Select(Function(x) x.Select(Function(v) v.Value).ToList()).ToList()
			End Function
		End Module

		Public Class TFS
			Inherits Integration

			Private _baseURI As String
			'Private _encodedCredentials As String
			Private _tfs As TFSLibrary = New TFSLibrary()
			Private _tfsExceptionMessageFormat As String = "TFS Error Message: {0}"
			Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor)
				MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, IntegrationType.Jira)
			End Sub

			Public Overrides Function PerformSync() As Boolean
				Dim Success As Boolean = True

				If ValidateSyncData() Then
					Success = SyncTickets()
				Else
					Success = False
				End If

				Return Success
			End Function

			Private Function ValidateSyncData() As Boolean
				Dim result As Boolean = True

				If CRMLinkRow.HostName Is Nothing Then
					result = False
					AddLog("HostName is missing and it is required to sync.")
				Else
					Dim protocol As String = String.Empty
					If CRMLinkRow.HostName.Length > 4 AndAlso CRMLinkRow.HostName.Substring(0, 4).ToLower() <> "http" Then
						protocol = "https://"
					End If
					_baseURI = protocol + CRMLinkRow.HostName
				End If

				If CRMLinkRow.SecurityToken1 Is Nothing Then
					If CRMLinkRow.Username Is Nothing OrElse CRMLinkRow.Password Is Nothing Then
						result = False
						AddLog("The API Token and the Username and or Password are missing and they are required to sync.")
					End If
				End If

				'Make sure credentials are good
				If (result) Then
					Try
						Dim logFile As String = Log.Path & "\" & Log.FileNamePath
						If (Not String.IsNullOrEmpty(CRMLinkRow.SecurityToken1)) Then
							_tfs = New TFSLibrary(_baseURI, CRMLinkRow.SecurityToken1, logFile)
						Else
							_tfs = New TFSLibrary(_baseURI, CRMLinkRow.Username, CRMLinkRow.Password, CRMLinkRow.UseNetworkCredentials, logFile)
						End If

						Dim workItemId As Integer
						workItemId = 1
						Dim errorMessage As String = _tfs.CheckCredentialsAndHost(workItemId)

						If (String.IsNullOrEmpty(errorMessage)) Then
							AddLog("Tfs credentials ok.")
						Else
							AddLog("Tfs credentials didn't work. " + errorMessage, LogType.TextAndReport)
							result = False
						End If
					Catch ex As Exception
						result = False
						_exception = New IntegrationException(ex.InnerException.Message, ex)
					End Try
				End If

				Return result
			End Function

			Private Function SyncTickets() As Boolean
				Dim numberOfWorkItemsToPullAsTickets As Integer = 0
				Dim ticketLinkToTFS As TicketLinkToTFS = New TicketLinkToTFS(User)
				Dim workItemsToPullAsTickets As TFSLibrary.WorkItems = New TFSLibrary.WorkItems()

				ticketLinkToTFS.LoadByCrmLinkId(CRMLinkRow.CRMLinkID, True)

				If ticketLinkToTFS.Any AndAlso ticketLinkToTFS.Count > 0 Then
					Try
						workItemsToPullAsTickets = GetWorkItemsToPullAsTickets(ticketLinkToTFS, numberOfWorkItemsToPullAsTickets)
					Catch tfsEx As TFSLibrary.TFSClientException
						Dim tfsExMessage As String = String.Empty
						tfsExMessage = tfsEx.ErrorResponse.ErrorMessage + vbNewLine + "EventId: " + tfsEx.ErrorResponse.eventId.ToString() + ". TypeKey: " + tfsEx.ErrorResponse.typeKey
						_exception = New IntegrationException(tfsExMessage, tfsEx)
						Return False
					Catch ex As Exception
						Log.Write(Exception.Message)
						Log.Write(Exception.StackTrace)
						_exception = New IntegrationException(Exception.Message, Exception)
						Return False
					End Try
				End If

				Dim ticketsLinksToTFSToPushAsWorkItems As TicketLinkToTFS = Nothing
				Dim ticketsToPushAsWorkItems As TicketsView = GetTicketsToPushAsWorkItems(ticketsLinksToTFSToPushAsWorkItems)
				Dim allStatuses As TicketStatuses = New TicketStatuses(User)
				Dim newActionsTypeID As Integer = 0

				If ticketsToPushAsWorkItems.Count > 0 OrElse numberOfWorkItemsToPullAsTickets > 0 Then
					allStatuses.LoadByOrganizationID(CRMLinkRow.OrganizationID)
					newActionsTypeID = GetNewActionsTypeID(CRMLinkRow.OrganizationID)
				End If

				If (IsHostMatching(_tfs.HostName, CRMLinkRow.HostName)) Then
					If ticketsToPushAsWorkItems.Count > 0 Then
						PushTicketsAndActionsAsWorkItemsAndComments(ticketsToPushAsWorkItems, ticketsLinksToTFSToPushAsWorkItems, allStatuses, newActionsTypeID)
					End If

					If numberOfWorkItemsToPullAsTickets > 0 Then
						PullWorkItemsAndCommentsAsTicketsAndActions(workItemsToPullAsTickets, allStatuses, newActionsTypeID, ticketLinkToTFS)
					End If
				Else
					AddLog("The host of the _tfs object does not match to the crmlinktable hostname. Skipping all sync. This could be the same to what happened before on updates to wrong orgs.")
				End If

				Return Not SyncError
			End Function

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
					Dim threadNumber As Integer = Processor.Thread.ManagedThreadId
					Log.Write(String.Format("Thread [{0}] {1}", threadNumber, exception))
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

			Private Function GetWorkItemsToPullAsTickets(ByRef ticketLinkToTFS As TicketLinkToTFS, ByRef numberOfWorkItemsToPull As Integer) As TFSLibrary.WorkItems
				Dim tfsIdList As List(Of Integer) = ticketLinkToTFS.Where(Function(w) w.CrmLinkID IsNot Nothing).Select(Function(p) CType(p.TFSID, Integer)).ToList()
				Dim result As TFSLibrary.WorkItems = New TFSLibrary.WorkItems()
				Dim recentClause As String = String.Empty
				numberOfWorkItemsToPull = 0

				'Search only for the tfs ids we have, those are the ones linked and the only ones that we need to check for updates
				If (tfsIdList.Count > 0) Then
					'//vv we might end up doing this in batches too. We'll see. Well now we have!! vsts only returns a max of 200 results
					Dim vstsMaxResults As Integer = 200
					Dim workItemsList As TFSLibrary.WorkItems = New TFSLibrary.WorkItems()
					Dim workItemsBatch As TFSLibrary.WorkItems = New TFSLibrary.WorkItems()
					Dim tfsIdListBatches As List(Of List(Of Integer)) = tfsIdList.SplitBy(vstsMaxResults)

					For Each batchIds As List(Of Integer) In tfsIdListBatches
						workItemsBatch = _tfs.GetWorkItemsBy(batchIds, CRMLinkRow.LastLinkUtc)

						If (workItemsList Is Nothing OrElse workItemsList.value Is Nothing OrElse workItemsList.value.Count = 0) Then
							workItemsList = workItemsBatch
						Else
							For Each singleWorkItem In workItemsBatch.value
								workItemsList.value.Add(singleWorkItem)
							Next
						End If
					Next

					If (workItemsList IsNot Nothing AndAlso workItemsList.value IsNot Nothing AndAlso workItemsList.count > 0) Then
						workItemsList.count = workItemsList.value.Count
						numberOfWorkItemsToPull += workItemsList.count
						result = workItemsList
					End If
				End If

				AddLog("Got " + numberOfWorkItemsToPull.ToString() + " Work Items To Pull As Tickets.")
				Return result
			End Function

			Private Function GetMinutesSinceLastLink() As Integer
				Dim datesDifference As TimeSpan = DateTime.UtcNow.Subtract(CRMLinkRow.LastLinkUtc)
				Return datesDifference.TotalMinutes + 30
			End Function

			Private Function GetMinutesSinceFirstSyncedTicket() As Integer
				Dim firstSyncedTicket As Tickets = New Tickets(MyBase.User)
				firstSyncedTicket.LoadFirstTFSSynced(CRMLinkRow.OrganizationID)
				Dim result As Integer = 0
				If firstSyncedTicket.Count > 0 Then
					Dim datesDifference As TimeSpan = DateTime.UtcNow.Subtract(firstSyncedTicket(0).DateCreatedUtc)
					result = datesDifference.TotalMinutes + 30
				End If
				Return result
			End Function

			Private Function GetTicketsToPushAsWorkItems(ByRef ticketsLinksToTFSToPushAsWorkItems As TicketLinkToTFS) As TicketsView
				Dim result As New TicketsView(User)
				result.LoadToPushToTFS(CRMLinkRow)
				AddLog("Got " + result.Count.ToString() + " Tickets To Push As Work Items.")

				ticketsLinksToTFSToPushAsWorkItems = New TicketLinkToTFS(User)
				ticketsLinksToTFSToPushAsWorkItems.LoadToPushToTFS(CRMLinkRow)

				Return result
			End Function

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

			Private Sub PushTicketsAndActionsAsWorkItemsAndComments(ByVal ticketsToPushAsWorkItems As TicketsView,
															ByVal ticketsLinksToTFSToPushAsWorkItems As TicketLinkToTFS,
															ByVal allStatuses As TicketStatuses,
															ByVal newActionsTypeID As Integer)
				Dim URI As String = _baseURI
				Dim attachmentFileSizeLimit As Integer = 0
				Dim attachmentEnabled As Boolean = GetAttachmentEnabled(attachmentFileSizeLimit)
				Dim crmLinkError As CRMLinkError = Nothing
				Dim ticketData As StringBuilder = New StringBuilder()
				Dim TFSProjectName As String = String.Empty
				Dim customMappingFields As New CRMLinkFields(User)
				Dim ticketTypeMappings As New CRMLinkFields(User)
				ticketTypeMappings.LoadByObjectType("TicketType", CRMLinkRow.CRMLinkID)

				Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(User)
				Dim crmLinkAttachmentErrors As CRMLinkErrors = New CRMLinkErrors(User)

				'Get the errors only for the tickets to be processed
				crmLinkErrors.LoadByOperationAndObjectIds(CRMLinkRow.OrganizationID,
														CRMLinkRow.CRMType,
														GetDescription(Orientation.OutToTFS),
														GetDescription(ObjectType.Ticket),
														ticketsToPushAsWorkItems.Select(Function(p) p.TicketID.ToString()).ToList(),
														isCleared:=False)

				For Each ticket As TicketsViewItem In ticketsToPushAsWorkItems
					If (ticket.OrganizationID <> CRMLinkRow.OrganizationID OrElse Not IsHostMatching(_tfs.HostName, CRMLinkRow.HostName)) Then
						AddLog(String.Format("The ticket {0} of org {1} belongs to a different organization that the one processing {2}, or the hosts does match between tfs object and crmlinkrow: {3} : {4}. Doing nothing.", ticket.TicketNumber, ticket.OrganizationID, CRMLinkRow.OrganizationID, _tfs.HostName, CRMLinkRow.HostName))
						Continue For
					End If

					Dim ticketLinkToTFS As TicketLinkToTFSItem = ticketsLinksToTFSToPushAsWorkItems.FindByTicketID(ticket.TicketID)
					Dim ticketLinkToTFSVerify As TicketLinkToTFS = New TicketLinkToTFS(User)
					ticketLinkToTFSVerify.LoadByTicketID(ticket.TicketID)

					If (ticketLinkToTFSVerify.Count = 0) Then
						AddLog("The ticket link record has been deleted. Link was removed or cancelled. Doing nothing.")
						Continue For
					End If

					AddLog(String.Format("Processing ticket #{0}. TicketId: {1}. OrganizationId: {2}. CrmLinkTable.Id and Org {3}:{4}", ticket.TicketNumber, ticket.TicketID, ticket.OrganizationID, CRMLinkRow.CRMLinkID, CRMLinkRow.OrganizationID))
					customMappingFields = New CRMLinkFields(User)
					customMappingFields.LoadByObjectTypeAndCustomFieldAuxID(GetDescription(ObjectType.Ticket), CRMLinkRow.CRMLinkID, ticket.TicketTypeID)

					Dim updateTicketFlag As Boolean = False
					'Dim sendCustomMappingFields As Boolean = False
					Dim workItemFields As List(Of TFSLibrary.WorkItemField)
					Dim workItemValues As List(Of TFSLibrary.WorkItemField) = New List(Of TFSLibrary.WorkItemField)

					Try
						crmLinkError = crmLinkErrors.FindUnclearedByObjectIDAndFieldName(ticket.TicketID, String.Empty)
						TFSProjectName = GetProjectName(ticket, crmLinkErrors)
						workItemFields = GetWorkItemFields(ticket, TFSProjectName, crmLinkError, Orientation.OutToTFS)
					Catch tfsEx As TFSLibrary.TFSClientException
						AddLog(tfsEx.ErrorResponse.ErrorMessage + " " + tfsEx.StackTrace)
						Continue For
					Catch ex As Exception
						AddLog(String.Format("Exception in PushTicketsAndActionsAsWorkItemsAndComments: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace))
						Continue For
					End Try

					Dim workItem As WorkItem = New WorkItem()
					Dim isNew As Boolean = False

					'Create new WorkItem
					If ticketLinkToTFS.TFSTitle Is Nothing OrElse ticketLinkToTFS.TFSTitle.IndexOf("Error") > -1 Then
						Try
							crmLinkError = crmLinkErrors.FindUnclearedByObjectIDAndFieldName(ticket.TicketID, String.Empty)
							AddLog("No TFS Id. Creating work item...")
							isNew = True

							Dim actionDescriptionId As Integer

							workItemValues = GetTicketData(ticket, workItemFields, TFSProjectName, actionDescriptionId, customMappingFields, crmLinkErrors)
							ticketData.Append(String.Join(",", workItemValues.Select(Function(p) String.Format("""{0}""=""{1}""", p.name, p.value)).ToArray()))
							Dim mappedTicketType As CRMLinkField = ticketTypeMappings.FindByTSFieldName(ticket.TicketTypeName)
							If mappedTicketType Is Nothing Then
								workItem = _tfs.CreateWorkItem(workItemValues, TFSProjectName, ticket.TicketTypeName)
							Else
								workItem = _tfs.CreateWorkItem(workItemValues, TFSProjectName, mappedTicketType.CRMFieldName)
							End If

							'We don't know if the workItem's type has Description or not, so we will always add a Comment with it too.
							If (workItem IsNot Nothing AndAlso workItem.Id > 0) Then
								Dim TFSComment As String = BuildCommentBody(ticket.TicketNumber, workItemValues.Where(Function(f) f.name = "Description").FirstOrDefault().value, 1, ticket.CreatorID)
								_tfs.CreateComment(workItem.Id, TFSComment)
							End If

							updateTicketFlag = True

							'Check if Ticket Description Action has Attachment
							If (attachmentEnabled AndAlso actionDescriptionId > 0) Then
								Dim actionDescriptionAttachment As Data.AttachmentProxy = TeamSupport.Data.Quarantine.ServiceQ.GetAttachmentProxy(User, actionDescriptionId)
								'The Action Description should always be 1, if for any reason this is not the case call: Actions.GetActionPosition(User, actionDescriptionId)
								Dim actionPosition As Integer = 1
								PushAttachments(actionDescriptionId, ticket.TicketNumber, workItem, attachmentFileSizeLimit, actionPosition)
							End If

							ClearCrmLinkError(crmLinkError)
						Catch tfsEx As TFSLibrary.TFSClientException
							Dim errorMessage As String = tfsEx.ErrorResponse.typeKey
							AddLog(String.Format(_tfsExceptionMessageFormat, tfsEx.ErrorResponse.ErrorMessage))
							AddLog(tfsEx.Message,
									LogType.Report,
									crmLinkError,
									String.Format("WorkItem was not created due to:{0}{1}", Environment.NewLine, tfsEx.ErrorResponse.ErrorMessage),
									Orientation.OutToJira,
									ObjectType.Ticket,
									ticket.TicketID,
									String.Empty,
									ticketData.ToString(),
									OperationType.Create)

							Dim updateLinkToTFS As Boolean = True

							'ToDo need to set the rest of the cases
							Select Case tfsEx.ErrorResponse.typeKey.ToLower()
								Case "ProjectDoesNotExistWithNameExceptiont" 'ToDo pending
									errorMessage = "Error: Specify Project (Product)."
								Case "workitemtypenotfoundexception"
									errorMessage = "Error: Specify valid Type."
									'Case "project mismatch" 'ToDo pending
									'	errorMessage = "Error: Specify valid Type and/or Project (Product)."
								Case Else
									errorMessage = tfsEx.ErrorResponse.ErrorMessage
									updateLinkToTFS = False
							End Select

							If updateLinkToTFS Then
								ticketLinkToTFS.TFSTitle = errorMessage
								ticketLinkToTFS.DateModifiedByTFSSync = DateTime.UtcNow()
								ticketLinkToTFS.Collection.Save()
							End If
						Catch ex As Exception
							Dim errorMessage As String = ex.Message
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

						'WorkItem already exists. 
						'We are not updating WorkItems, but if this is a second ticket relating to WorkItem we add a link and update ticket fields for TFS
					ElseIf ticketLinkToTFS.TFSID IsNot Nothing AndAlso Not ticketLinkToTFS.DateModifiedByTFSSync.HasValue Then
						crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID, "update")

						Try
							workItem = _tfs.GetWorkItemBy(ticketLinkToTFS.TFSID)
							updateTicketFlag = True
							AddLog("No DateModifiedByTFSSync. We'll add link.")

							'An update error could has been caused here or below.
							'We'll only clear when successfull below.
						Catch tfsEx As TFSLibrary.TFSClientException
							Dim TFSErrors As String = tfsEx.ErrorResponse.ErrorMessage

							If (Not String.IsNullOrEmpty(TFSErrors)) Then
								AddLog(String.Format(_tfsExceptionMessageFormat,
														TFSErrors,
														Environment.NewLine,
														vbTab,
														TFSErrors.ToString()))
								AddLog(tfsEx.StackTrace,
											LogType.Report,
											crmLinkError,
											String.Format("Could not get TFS WorkItem:{0}{1}", Environment.NewLine, TFSErrors.ToString()),
											Orientation.OutToJira,
											ObjectType.Ticket,
											ticket.TicketID,
											"update",
											URI,
											OperationType.Create)
							Else
								AddLog(String.Format("{0}{1}{2}", tfsEx.Message, Environment.NewLine, tfsEx.StackTrace))
							End If

							Continue For
						Catch ex As Exception
							AddLog(String.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace))
							Continue For
						End Try
					Else
						AddLog("Already linked. Doing nothing.")
					End If

					If updateTicketFlag Then
						crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID, "update")

						Try
							Dim creatorName As String = "TeamSupport"

							If ticketLinkToTFS.CreatorID IsNot Nothing AndAlso ticketLinkToTFS.CreatorID > 0 Then
								Dim creator As Users = New Users(User)
								creator.LoadByUserID(ticketLinkToTFS.CreatorID)
								creatorName = String.Format("{0}.{1} {2} linked", creator(0).FirstName.Substring(0, 1),
																				If(String.IsNullOrEmpty(creator(0).MiddleName), "", " " + creator(0).MiddleName.Substring(0, 1) + "."),
																				creator(0).LastName)
							End If

							'Add the TS hyperlink to the workitem
							Dim ticketName As String = DataUtils.GetJsonCompatibleString(HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(ticket.Name)))
							Dim domain As String = SystemSettings.ReadStringForCrmService(User, "AppDomain", "https://app.teamsupport.com")
							Dim remoteLink As String = String.Format("{0}/Ticket.aspx?ticketid={1}", domain, ticket.TicketID.ToString())

							_tfs.CreateTeamSupportHyperlink(workItem.Id, remoteLink, String.Format("{0} Ticket #{1} - {2}", creatorName, ticket.TicketNumber, ticketName))
							ticketLinkToTFS.TFSID = workItem.Id

							If (workItem.Fields.Where(Function(w) w.Key = "System.Title").Any()) Then
								ticketLinkToTFS.TFSTitle = workItem.Fields.Where(Function(w) w.Key = "System.Title").Select(Function(p) p.Value)(0)
							Else
								ticketLinkToTFS.TFSTitle = ticketName
							End If

							If (workItem.Fields.Where(Function(w) w.Key = "System.State").Any()) Then
								ticketLinkToTFS.TFSState = workItem.Fields.Where(Function(w) w.Key = "System.State").Select(Function(p) p.Value)(0)
							Else
								ticketLinkToTFS.TFSState = "System.State not found in workitem" 'ToDo //vv what to do if the System.Title is not found?
							End If

							Dim workItemProject = String.Empty

							If (workItem.Fields.Where(Function(w) w.Key = "System.TeamProject").Any()) Then
								workItemProject = workItem.Fields.Where(Function(w) w.Key = "System.TeamProject").Select(Function(p) p.Value)(0)
							Else
								workItemProject = TFSProjectName
								AddLog("The System.TeamProject field was not found in the workitem, using the project obtained from the integration settings that apply to the ticket.")
							End If

							ticketLinkToTFS.TFSURL = _tfs.HostName + "/" + workItemProject + "/_workitems/edit/" + workItem.Id.ToString()

							If CRMLinkRow.UpdateStatus Then
								Dim newStatus As TicketStatus = allStatuses.FindByName(ticketLinkToTFS.TFSState, ticket.TicketTypeID)

								If newStatus IsNot Nothing Then
									Dim updateTicket As Tickets = New Tickets(User)
									updateTicket.LoadByTicketID(ticket.TicketID)
									updateTicket(0).TicketStatusID = newStatus.TicketStatusID
									ticketLinkToTFS.DateModifiedByTFSSync = DateTime.UtcNow
									updateTicket.Save()
									AddLog(String.Format("Updated status with linked work item state: {0} ({1})", newStatus.Name, newStatus.TicketStatusID))
								Else
									AddLog(String.Format("The Work Item State '{0}' in TFS does not exist for the Ticket Type {1}.", ticketLinkToTFS.TFSState, ticket.TicketTypeName),
										LogType.TextAndReport,
										crmLinkError,
										String.Format("State was not updated because Work Item State '{0}' does not exist in the current Ticket statuses.", ticketLinkToTFS.TFSState),
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
								newAction(0).Description = "Ticket has been synced with TFS' work item " + workItem.Fields.Where(Function(w) w.Key = "System.Title").Select(Function(p) p.Value).FirstOrDefault() + " with state """ + ticketLinkToTFS.TFSState + """."
								ticketLinkToTFS.DateModifiedByTFSSync = DateTime.UtcNow()
								newAction.Save()

								Dim newActionLinkToTFS As ActionLinkToTFS = New ActionLinkToTFS(User)
								newActionLinkToTFS.AddNewActionLinkToTFSItem(newAction(0).ActionID)
								newActionLinkToTFS(0).TFSID = -1
								newActionLinkToTFS(0).DateModifiedByTFSSync = ticketLinkToTFS.DateModifiedByTFSSync
								newActionLinkToTFS.Save()

								AddLog("Added comment indicating linked work item state.")
							End If

							ticketLinkToTFS.Collection.Save()
							AddLog("Updated ticketLinkToTFS fields for ticket")

							ClearCrmLinkError(crmLinkError)

							Dim message As String = String.Format("{0} updated ticket {1}.", Enums.GetDescription(IntegrationType.TFS), ticket.TicketNumber)
							SendPusherMessage("ticket-dispatch-" + ticket.OrganizationID.ToString(), "DisplayTicketUpdate", New With {.ticket = ticket.TicketNumber.ToString(), .update = message})
						Catch ex As Exception
							AddLog(ex.ToString() + ex.StackTrace,
								LogType.Report,
								crmLinkError,
								"Error creating the RemoteLink in the WorkItem. " + ex.Message,
								Orientation.OutToJira,
								ObjectType.Ticket,
								ticket.TicketID.ToString(),
								"update",
								Nothing,
								OperationType.Update)
						End Try
					End If

					If workItem.Id Is Nothing Then
						Try
							workItem = _tfs.GetWorkItemBy(ticketLinkToTFS.TFSID)
						Catch ex As Exception
							AddLog(ex.ToString() + ex.StackTrace,
								LogType.Report,
								crmLinkError,
								ex.Message,
								Orientation.OutToJira,
								ObjectType.Ticket,
								ticket.TicketID.ToString(),
								"update",
								Nothing,
								OperationType.Update)
						End Try
					End If

					If workItem.Id IsNot Nothing Then
						PushActionsAsComments(ticket.TicketID, ticket.TicketNumber, workItem, attachmentEnabled, attachmentFileSizeLimit)

						'We are now updating the custom mapping fields. We do a call per field to minimize the impact of invalid values attempted to be assigned.
						If workItemFields IsNot Nothing Then
							For Each field As TFSLibrary.WorkItemField In workItemFields
								UpdateWorkItemField(workItem.Id, customMappingFields, ticket, field, crmLinkErrors, URI)
							Next
						End If
					End If
				Next
			End Sub

			Private Function GetAttachmentEnabled(ByRef attachmentFileSizeLimit As Integer) As String
				Dim result As Boolean = False

				'ToDo //vv It looks like TFS does not have a enable/disable for attachments, so it'll be true here until we find if TFS has this check. We will not set the attachmentFileSizeLimit yet either. However, there is a limit of 100 attachments per work item, we could check on this at some point.
				result = True
				AddLog("Attachment enabled is " + result.ToString())

				Return result
			End Function

			Private Function GetProjectName(ByVal ticket As TicketsViewItem, ByVal crmLinkErrors As CRMLinkErrors) As String
				Dim TFSProjectName As String = CRMLinkRow.DefaultProject
				Dim crmLinkError As CRMLinkError = crmLinkErrors.FindUnclearedByObjectIDAndFieldName(ticket.TicketID, "ProjectName")

				If CRMLinkRow.AlwaysUseDefaultProjectKey Then
					AddLog(String.Format("Using Default Project Name ""{0}""", TFSProjectName))
				Else
					Dim ticketProductVersion As ProductVersion

					If Not ticket.ReportedVersionID Is Nothing AndAlso ticket.ReportedVersionID > 0 Then
						ticketProductVersion = ProductVersions.GetProductVersion(User, ticket.ReportedVersionID)
					End If

					If Not ticket.ReportedVersionID Is Nothing AndAlso Not ticketProductVersion Is Nothing AndAlso Not String.IsNullOrEmpty(ticketProductVersion.TFSProjectName) Then
						TFSProjectName = ticketProductVersion.TFSProjectName
						AddLog(String.Format("TFS Project Name ""{0}"" from Product Version {1}", TFSProjectName, ticket.ReportedVersion))
					Else
						Dim ticketProduct As Product

						If Not ticket.ProductID Is Nothing Then
							ticketProduct = Products.GetProduct(User, ticket.ProductID)

							If Not String.IsNullOrEmpty(ticketProduct.TFSProjectName) Then
								TFSProjectName = ticketProduct.TFSProjectName
								AddLog(String.Format("TFS Project Name ""{0}"" from Product {1}", TFSProjectName, ticket.ProductName))
							Else
								TFSProjectName = ticket.ProductName
								AddLog(String.Format("TFS Project Name ""{0}"" like Product Name", TFSProjectName))
							End If
						Else
							TFSProjectName = CRMLinkRow.DefaultProject
							AddLog(String.Format("Default Project Name ""{0}"" to be used as TFS Project Name after not found in ProductVersion, Product, or Product Name", TFSProjectName))
						End If
					End If
				End If

				If String.IsNullOrEmpty(TFSProjectName) Then
					Dim message As String = If(CRMLinkRow.AlwaysUseDefaultProjectKey, "AlwaysUseDefaultProjectKey but no Default Project.", "Couldn't find a TFS Project Name in ProductVersion, Product, Product Name in Ticket, or Default Project to use for integration.")
					AddLog(message,
						LogType.TextAndReport,
						crmLinkError,
						"Error attempting to get TFS project.",
						Orientation.OutToJira,
						ObjectType.Ticket,
						ticket.TicketID,
						"ProjectName",
						Nothing,
						OperationType.Unknown)
					Dim ex As Exception = New Exception(message)
					Throw ex
				End If

				ClearCrmLinkError(crmLinkError)

				Return TFSProjectName
			End Function

			Private Function ClearCrmLinkError(ByRef crmLinkError As CRMLinkError)
				If crmLinkError IsNot Nothing Then
					crmLinkError.IsCleared = True
					crmLinkError.DateModified = DateTime.UtcNow()
					crmLinkError.Collection.Save()
				End If
			End Function

			Private Function GetWorkItemFields(ByVal ticket As TicketsViewItem,
										ByVal TFSProjectName As String,
										ByRef crmLinkError As CRMLinkError,
										ByVal orientation As Orientation) As List(Of TFSLibrary.WorkItemField)
				Dim workItemTypeName As String = ticket.TicketTypeName
				Dim result As JObject = Nothing
				Dim fields As List(Of TFSLibrary.WorkItemField)

				workItemTypeName = Replace(workItemTypeName, " ", "+")
				TFSProjectName = Replace(TFSProjectName, " ", "+")

				Try
					fields = _tfs.WorkItemsFields

					If fields Is Nothing OrElse Not fields.Any Then
						'//TFS api does not return fields per type, it just returns ALL fields.
						AddLog("TFS did not return any work item fields.",
							LogType.Report,
							crmLinkError,
							"TFS did not return any work item fields.",
							orientation,
							ObjectType.Ticket,
							ticket.TicketID,
							"",
							Nothing,
							OperationType.Unknown)
						AddLog("TFS did not return any work item fields.")
					Else
						'ToDo //vv ?? result = JObject.Parse(fields)
						ClearCrmLinkError(crmLinkError)
					End If
				Catch tfsEx As TFSLibrary.TFSClientException
					Throw
				End Try

				Return fields
			End Function

			Private Function GetTicketData(ByVal ticket As TicketsViewItem,
										ByVal workItemFields As List(Of TFSLibrary.WorkItemField),
										ByVal TFSProjectName As String,
										ByRef actionDescriptionId As Integer,
										ByRef customMappingFields As CRMLinkFields,
										ByRef crmLinkErrors As CRMLinkErrors) As List(Of TFSLibrary.WorkItemField)
				Dim workItemValues As List(Of TFSLibrary.WorkItemField) = New List(Of TFSLibrary.WorkItemField)
				Dim fieldValue As TFSLibrary.WorkItemField

				fieldValue = workItemFields.Where(Function(w) w.name = "Title").FirstOrDefault
				fieldValue.value = DataUtils.GetJsonCompatibleString(HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(ticket.Name)))
				workItemValues.Add(fieldValue)

				Dim actionDescription As Action = Actions.GetTicketDescription(User, ticket.TicketID)
				actionDescriptionId = actionDescription.ActionID

				Dim addLines As Boolean = True
				Dim description As String = HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(actionDescription.Description), addLines)
				fieldValue = workItemFields.Where(Function(w) w.name = "Description").FirstOrDefault
				fieldValue.value = description
				workItemValues.Add(fieldValue)

				'//vv Specific for Alegeus now, we need to find out how to get the 'required' fields from TFS, currently the call to get the fields does not say
				If (CRMLinkRow.OrganizationID = 1150007) Then
					fieldValue = workItemFields.Where(Function(w) w.referenceName = "Alegeus.TargetVersion").FirstOrDefault
					Dim customValues As New CustomValues(User)
					Dim cRMLinkField As CRMLinkField = customMappingFields.FindByCRMFieldName("Alegeus.TargetVersion")
					customValues.LoadByFieldID(cRMLinkField.CustomFieldID, ticket.TicketID)

					If customValues.Count > 0 Then
						fieldValue.value = customValues(0).Value
						workItemValues.Add(fieldValue)
					End If
				End If

				Dim customField As StringBuilder = New StringBuilder()

				For Each tfsField As TFSLibrary.WorkItemField In workItemFields
					Dim customValues As New CustomValues(User)
					Dim cRMLinkField As CRMLinkField = customMappingFields.FindByCRMFieldName(tfsField.referenceName)
					fieldValue = workItemFields.Where(Function(w) w.name = tfsField.name).FirstOrDefault

					If cRMLinkField IsNot Nothing AndAlso cRMLinkField.CustomFieldID IsNot Nothing Then
						customValues.LoadByFieldID(cRMLinkField.CustomFieldID, ticket.TicketID)

						If customValues.Count > 0 Then
							fieldValue.value = customValues(0).Value
							workItemValues.Add(fieldValue)
						Else
							Dim customFields As New CustomFields(User)
							customFields.LoadByCustomFieldID(cRMLinkField.CustomFieldID)

							If customFields(0).FieldType = CustomFieldType.Boolean Then
								fieldValue.value = False.ToString()
								workItemValues.Add(fieldValue)
							End If
						End If
					ElseIf cRMLinkField IsNot Nothing AndAlso cRMLinkField.TSFieldName IsNot Nothing AndAlso ticket.Row(cRMLinkField.TSFieldName) IsNot Nothing Then
						fieldValue.value = ticket.Row(cRMLinkField.TSFieldName)
						workItemValues.Add(fieldValue)
					ElseIf cRMLinkField IsNot Nothing Then
						AddLog("TicketID " + ticket.TicketID.ToString() + "'s field '" + cRMLinkField.CRMFieldName + "' was not included because custom field " +
											cRMLinkField.CRMFieldID.ToString() + " CustomFieldID and TSFieldName are null.")
					End If
				Next

				Return workItemValues
			End Function

			Private Function GetDataLineValue(ByVal fieldKey As String, ByVal fieldType As Object, ByVal fieldValue As String, Optional ByVal workItemID As Integer = 0) As String
				Dim result As String = Nothing

				'//vv What cases should we handle? we might need to be filling this over time
				Select Case fieldType.ToLower()
					'Case "select"
					'Case "multiselect"
					Case "date", "datetime"
						result = If(Not String.IsNullOrEmpty(fieldValue), Convert.ToDateTime(fieldValue).ToString(), fieldValue)
						'Case "float"
						'Case "string"
						'Case "radiobuttons"
					Case Else
						result = fieldValue
				End Select

				Return result
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

			Private Sub PushAttachments(
			ByVal actionID As Integer,
			ByVal ticketNumber As Integer,
			ByVal workItem As WorkItem,
			ByVal fileSizeLimit As Integer,
			ByVal actionPosition As Integer)

				Dim attachments As AttachmentProxy() = TeamSupport.Data.Quarantine.ServiceQ.LoadForTFS(User, actionID)

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

                        'ToDo //vv attachmentFileSizeLimit is always zero at this point because I have not found if TFS limits the attachments or not.
                        If (fileSizeLimit > 0 AndAlso fs.Length > fileSizeLimit) Then
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
                                _tfs.UploadAttachment(workItem.Id, attachment.Path, attachment.FileName)

                                'Dim URIString As String = ""
                                'Dim request As HttpWebRequest = WebRequest.Create(URIString)
                                'request.Headers.Add("Authorization", "Basic " + _encodedCredentials)
                                ''request.Headers.Add("X-Atlassian-Token", "nocheck")
                                'request.Method = "POST"
                                'Dim boundary As String = String.Format("----------{0:N}", Guid.NewGuid())
                                'request.ContentType = String.Format("multipart/form-data; boundary={0}", boundary)
                                'request.UserAgent = Client

                                'Dim content = New MemoryStream()
                                'Dim writer = New StreamWriter(content)
                                'writer.WriteLine("--{0}", boundary)
                                'writer.WriteLine("Content-Disposition: form-data; name=""file""; filename=""{0}""", ("TeamSupport Ticket #" + ticketNumber.ToString() + " action #" + actionPosition.ToString() + " - " + attachment.FileName))
                                'writer.WriteLine("Content-Type: application/octet-stream")
                                'writer.WriteLine()
                                'writer.Flush()
                                'Dim data(fs.Length) As Byte
                                'fs.Read(data, 0, data.Length)
                                'fs.Close()
                                'content.Write(data, 0, data.Length)
                                'writer.WriteLine()
                                'writer.WriteLine("--" + boundary + "--")
                                'writer.Flush()
                                'content.Seek(0, SeekOrigin.Begin)
                                'request.ContentLength = content.Length

                                'Using requestStream As Stream = request.GetRequestStream()
                                '	content.WriteTo(requestStream)
                                '	requestStream.Close()
                                'End Using

                                'Using response As HttpWebResponse = request.GetResponse()
                                '	Log.Write("Attachment """ + attachment.FileName + """ sent.")
                                '	response.Close()
                                'End Using

                                'content.Flush()
                                'content.Close()
                                attachment.SentToTFS = True
                                TeamSupport.Data.Quarantine.ServiceQ.SetAttachmentSentToTFS(User, attachment.AttachmentID)

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

            Private Sub PushActionsAsComments(
				ByVal ticketID As Integer,
				ByVal ticketNumber As Integer,
				ByVal workItem As WorkItem,
				ByVal attachmentEnabled As Boolean,
				ByVal attachmentFileSizeLimit As Integer)

				Dim actionsToPushAsComments As Actions = New Actions(User)
				actionsToPushAsComments.LoadToPushToTFS(CRMLinkRow, ticketID)
				AddLog("Found " + actionsToPushAsComments.Count.ToString() + " actions to push as comments.")

				Dim actionLinkToTFS As ActionLinkToTFS = New ActionLinkToTFS(User)
				actionLinkToTFS.LoadToPushToTFS(CRMLinkRow, ticketID)

				Dim crmLinkActionErrors As CRMLinkErrors = New CRMLinkErrors(User)
				crmLinkActionErrors.LoadByOperationAndObjectIds(CRMLinkRow.OrganizationID,
														CRMLinkRow.CRMType,
														GetDescription(Orientation.OutToJira),
														GetDescription(ObjectType.Action),
														actionsToPushAsComments.Select(Function(p) p.ActionID.ToString()).ToList(),
														isCleared:=False)
				Dim crmLinkError As CRMLinkError = Nothing

				For Each actionToPushAsComment As Action In actionsToPushAsComments
					Dim actionLinkToTFSItem As ActionLinkToTFSItem = actionLinkToTFS.FindByActionID(actionToPushAsComment.ActionID)
					Dim actionPosition As Integer = Actions.GetActionPosition(User, actionToPushAsComment.ActionID)
					crmLinkError = crmLinkActionErrors.FindByObjectIDAndFieldName(actionToPushAsComment.ActionID.ToString(), String.Empty)
					AddLog("Processing actionID: " + actionToPushAsComment.ActionID.ToString())

					If actionLinkToTFSItem Is Nothing Then
						Try
							Dim TFSComment As String = BuildCommentBody(ticketNumber, actionToPushAsComment.Description, actionPosition, actionToPushAsComment.CreatorID)
							Dim commentId As Integer = _tfs.CreateComment(workItem.Id, TFSComment)
							Dim newActionLinkToTFS As ActionLinkToTFS = New ActionLinkToTFS(User)
							Dim newActionLinkToTFSItem As ActionLinkToTFSItem = newActionLinkToTFS.AddNewActionLinkToTFSItem(actionToPushAsComment.ActionID)

							newActionLinkToTFSItem.TFSID = commentId
							newActionLinkToTFSItem.DateModifiedByTFSSync = DateTime.UtcNow
							newActionLinkToTFS.Save()
							AddLog(String.Format("Created comment {0} for action {1}", commentId, actionToPushAsComment.ActionID))

							ClearCrmLinkError(crmLinkError)
						Catch ex As Exception
							AddLog(ex.ToString() + ex.StackTrace)
							Continue For
						End Try
					Else
						Try
							AddLog("action.TFSID: " + actionLinkToTFSItem.TFSID.ToString())
							If actionLinkToTFSItem.TFSID <> -1 Then
								'It is not possible to update/modify a comment in TFS, so we'll create a new one specifying it was updated in TeamSupport.
								'This won't be offered as of now. If we create a new one instead we'll run into the issue of the old comment in TFS becoming un-linked in TS and bringing it over again (duplication)
								AddLog("Sync is not updating comments in TFS, this is not an error. As designed, neither this or any other integrations *update* existing comments/notes in those other systems.")

								'Dim TFSComment As String = BuildCommentBody(ticketNumber, actionToPushAsComment.Description, actionPosition, actionToPushAsComment.CreatorID)
								'TFSComment = String.Format("{0}{1}{2}", TFSComment, Environment.NewLine, "Action updated in TeamSupport.")
								'Dim commentId As Integer = _tfs.CreateComment(workItem.Id, TFSComment)
								'actionLinkToTFSItem.TFSID = commentId
								'actionLinkToTFSItem.DateModifiedByTFSSync = DateTime.UtcNow
								'actionLinkToTFSItem.Collection.Save()
								'Log.Write("updated comment for actionID: " + actionToPushAsComment.ActionID.ToString())
							End If

							ClearCrmLinkError(crmLinkError)
						Catch ex As Exception
							AddLog(ex.ToString() + ex.StackTrace)
							Continue For
						End Try
					End If

					If (attachmentEnabled) Then
						PushAttachments(actionToPushAsComment.ActionID, ticketNumber, workItem, attachmentFileSizeLimit, actionPosition)
					End If
				Next

				actionLinkToTFS.Save()
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

			Private Sub UpdateWorkItemField(ByRef workItemID As Integer,
			ByRef customMappingFields As CRMLinkFields,
			ByRef ticket As TicketsViewItem,
			ByRef field As TFSLibrary.WorkItemField,
			ByRef crmLinkErrors As CRMLinkErrors,
			Optional ByRef URI As String = Nothing)

				Dim updateFieldRequestBody As StringBuilder = New StringBuilder()
				Dim cRMLinkField As CRMLinkField = customMappingFields.FindByCRMFieldName(field.name)

				If cRMLinkField IsNot Nothing Then
					Dim value As String = Nothing
					Dim notIncludedMessage As String = String.Empty
					Dim findCustom As New CustomValues(User)
					Dim crmLinkError As CRMLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID, cRMLinkField.TSFieldName)

					If cRMLinkField.CustomFieldID IsNot Nothing Then
						findCustom.LoadByFieldID(cRMLinkField.CustomFieldID, ticket.TicketID)

						If findCustom.Count > 0 Then
							Dim customValue As String = findCustom(0).Value
							value = GetDataLineValue(field.name, field.type, customValue)
						Else
							Dim customFields As New CustomFields(User)
							customFields.LoadByCustomFieldID(cRMLinkField.CustomFieldID)
							Dim isBooleanField As Boolean = customFields(0).FieldType = CustomFieldType.Boolean

							'If the custom field is boolean and do not have any value then it is False. As seen in the UI
							If isBooleanField Then
								value = GetDataLineValue(field.name, field.type, False.ToString())
							Else
								notIncludedMessage = GetFieldNotIncludedMessage(ticket.TicketID, field.name, findCustom.Count = 0)
							End If
						End If
					ElseIf cRMLinkField.TSFieldName IsNot Nothing Then
						If ticket.Row(cRMLinkField.TSFieldName) IsNot Nothing Then
							Dim tsFieldValue As String = String.Empty

							If Not IsDBNull(ticket.Row(cRMLinkField.TSFieldName)) Then
								tsFieldValue = ticket.Row(cRMLinkField.TSFieldName)
							End If

							value = GetDataLineValue(field.name, field.type, tsFieldValue)
						Else
							notIncludedMessage = GetFieldNotIncludedMessage(ticket.TicketID, field.name, ticket.Row(cRMLinkField.TSFieldName) Is Nothing)
						End If
					Else
						AddLog("Field '" + field.name + "' was not included because custom field " +
								cRMLinkField.CRMFieldID.ToString() + " CustomFieldID and TSFieldName are null.")
					End If

					If value IsNot Nothing Then
						Try
							Dim workItemValues As List(Of TFSLibrary.WorkItemField) = New List(Of TFSLibrary.WorkItemField)

							field.value = value
							workItemValues.Add(field)
							_tfs.UpdateWorkItem(workItemID, workItemValues)

							ClearCrmLinkError(crmLinkError)
						Catch ex As Exception
							AddLog(ex.ToString() + ex.StackTrace,
								LogType.TextAndReport,
								crmLinkError,
								ex.Message,
								Orientation.OutToJira,
								ObjectType.Ticket,
								ticket.TicketID.ToString(),
								"create",
								JsonConvert.SerializeObject(field),
								OperationType.Update)
						End Try
					Else
						AddLog("No value found for the field " + field.name,
								LogType.TextAndReport,
								crmLinkError,
								notIncludedMessage,
								Orientation.OutToJira,
								ObjectType.Ticket,
								ticket.TicketID,
								field.name,
								Nothing,
								OperationType.Update)
					End If
				End If
			End Sub

			'//vv Private Sub PullWorkItemsAndCommentsAsTicketsAndActions(ByVal workItemsToPullAsTickets As JArray, ByVal allStatuses As TicketStatuses, ByVal newActionsTypeID As Integer, ByVal ticketLinkToTFS As TicketLinkToTFS)
			Private Sub PullWorkItemsAndCommentsAsTicketsAndActions(ByVal workItemsToPullAsTickets As TFSLibrary.WorkItems, ByVal allStatuses As TicketStatuses, ByVal newActionsTypeID As Integer, ByVal ticketLinkToTFS As TicketLinkToTFS)
				Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
				Dim ticketIds As List(Of Integer) = New List(Of Integer)()

				'//vv
				'For i = 0 To workItemsToPullAsTickets.count - 1
				'	ticketIds.AddRange(GetLinkedTicketIDs(workItemsToPullAsTickets(i), ticketLinkToTFS))
				'Next
				For Each workItem As WorkItem In workItemsToPullAsTickets.value
					ticketIds.AddRange(GetLinkedTicketIDs(workItem, ticketLinkToTFS))
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

				Dim ticketTypeMappings As CRMLinkFields = New CRMLinkFields(User)
				ticketTypeMappings.LoadByObjectType("TicketType", CRMLinkRow.CRMLinkID)

				For Each workItem As WorkItem In workItemsToPullAsTickets.value
					Dim newComments As List(Of WorkItemComment) = Nothing

					For Each ticketID As Integer In GetLinkedTicketIDs(workItem, ticketLinkToTFS)
						Dim updateTicket As Tickets = New Tickets(User)
						updateTicket.LoadByTicketID(ticketID)

						If updateTicket.Count > 0 AndAlso updateTicket(0).OrganizationID = CRMLinkRow.OrganizationID Then
							crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticketID.ToString(), String.Empty)

							Try
								Dim ticketLinkToTFSItem As TicketLinkToTFSItem = ticketLinkToTFS.FindByTicketID(ticketID)
								Dim sendToPusher As Boolean = False

								UpdateTicketWithWorkItemData(ticketID,
															 workItem,
															 newActionsTypeID,
															 allStatuses,
															 crmLinkError,
															 crmLinkErrors,
															 ticketLinkToTFSItem,
															 ticketTypeMappings,
															 sendToPusher)
								ClearCrmLinkError(crmLinkError)

								If newComments Is Nothing Then
									newComments = New List(Of WorkItemComment)
									If CRMLinkRow.UseNetworkCredentials Then
										newComments = GetNewHistoryAsComments(workItem.Id, ticketID)
									Else
										newComments = GetNewComments(workItem.Id, ticketID)
									End If
								End If

								AddNewCommentsInTicket(ticketID, newComments, newActionsTypeID, crmLinkActionErrors)

								If (newComments.Any() OrElse sendToPusher) Then
									Dim ticket As Tickets = New Tickets(User)
									ticket.LoadByTicketID(ticketID)

									If (ticket.Any()) Then
										Dim message As String = String.Format("{0} updated ticket {1}.", Enums.GetDescription(IntegrationType.TFS), ticket(0).TicketNumber)
										SendPusherMessage("ticket-dispatch-" + ticket(0).OrganizationID.ToString(), "DisplayTicketUpdate", New With {.ticket = ticket(0).TicketNumber.ToString(), .update = message})
									End If
								End If
							Catch ex As Exception
								AddLog(ex.ToString() + ex.StackTrace,
									LogType.Text,
									crmLinkError,
									"Error when updating ticket with Work Item data.",
									Orientation.IntoTeamSupport,
									ObjectType.Ticket,
									ticketID.ToString(),
									String.Empty,
									JsonConvert.SerializeObject(workItem),
									OperationType.Update)
							End Try
						ElseIf updateTicket.Count > 0 Then
							AddLog("Ticket with ID: """ + ticketID.ToString() + """ belongs to a different organization and was not updated.")
						Else
							AddLog("Ticket with ID: """ + ticketID.ToString() + """ was not found to be updated.")
						End If
					Next
				Next
			End Sub

			Private Function GetLinkedTicketIDs(ByVal workItem As WorkItem, ByVal ticketLinkToTFS As TicketLinkToTFS) As List(Of Integer)
				Dim result As List(Of Integer) = New List(Of Integer)()

				For Each ticketLinkToTFSItem As TicketLinkToTFSItem In ticketLinkToTFS
					Try
						If (Not IsHostMatching(CRMLinkRow.HostName, workItem.Url)) Then
							AddLog("The host of the workitem url does not match to the crmlinktable hostname. Skipping the rest of the updates for this ticket. This could be the same to what happened before on updates to wrong orgs.")
							Continue For
						End If

						If ticketLinkToTFSItem.TFSID = Convert.ToInt32(workItem.Id) Then
							result.Add(ticketLinkToTFSItem.TicketID)
						End If
					Catch ex As Exception
						Log.Write("Exception finding work item id. Skipping work item " + workItem.Id.ToString() + ", please review.")
					End Try
				Next

				Return result
			End Function

			Private Sub UpdateTicketWithWorkItemData(ByVal ticketID As Integer,
													 ByVal workItem As WorkItem,
													 ByVal newActionsTypeID As Integer,
													 ByVal allStatuses As TicketStatuses,
													 ByRef crmLinkError As CRMLinkError,
													 ByRef crmlinkErrors As CRMLinkErrors,
													 ByVal ticketLinkToTFSItem As TicketLinkToTFSItem,
													 ByRef ticketTypeMappings As CRMLinkFields,
													 ByRef sendPusherMessage As Boolean)
				Dim updateTicket As Tickets = New Tickets(User)
				updateTicket.LoadByTicketID(ticketID)

				If updateTicket.Count > 0 AndAlso updateTicket(0).OrganizationID = CRMLinkRow.OrganizationID Then
					Dim ticketTypeId As Integer = 0
					Dim customFields As New CRMLinkFields(User)
					Dim allTypes As TicketTypes = New TicketTypes(User)
					'Dim ticketLinkToTFS As TicketLinkToTFS = New TicketLinkToTFS(User)

					'ticketLinkToTFS.LoadByTicketID(updateTicket(0).TicketID)
					allTypes.LoadByOrganizationID(CRMLinkRow.OrganizationID)

					'For Each field As KeyValuePair(Of String, JToken) In CType(workItem("fields"), JObject)
					'    'If field.Key.Trim().ToLower() = "issuetype" Then
					'    Dim issueTypeName As String = GetFieldValue(field)
					'        Dim ticketType As TicketType = allTypes.FindByName(issueTypeName)

					'        If ticketType IsNot Nothing Then
					'            ticketTypeId = allTypes.FindByName(issueTypeName).TicketTypeID
					'            customFields.LoadByObjectTypeAndCustomFieldAuxID(GetDescription(ObjectType.Ticket), CRMLinkRow.CRMLinkID, ticketTypeId)
					'        End If

					'        Exit For
					'    End If
					'Next

					If ticketTypeId = 0 Then
						customFields.LoadByObjectTypeAndCustomFieldAuxID(GetDescription(ObjectType.Ticket), CRMLinkRow.CRMLinkID, updateTicket(0).TicketTypeID)
					End If

					Dim ticketValuesChanged = False
					Dim workItemStateChangedNotUpdatedInTicket = False
					Dim ticketView As TicketsView = New TicketsView(User)
					ticketView.LoadByTicketID(ticketID)
					Dim tfsProjectName As String = GetProjectName(ticketView(0), crmlinkErrors)
					Dim workItemFields As List(Of TFSLibrary.WorkItemField) = GetWorkItemFields(ticketView(0), tfsProjectName, crmLinkError, Orientation.IntoTeamSupport)
					Dim ticketsFieldMap As Tickets = New Tickets(User)

					'//vv For Each field As KeyValuePair(Of String, JToken) In CType(workItem("fields"), JObject)
					'For Each field As KeyValuePair(Of String, JToken) In CType(workItem.Fields, JObject)
					For Each field As KeyValuePair(Of String, Object) In workItem.Fields
						Dim value As String = Nothing
						Dim fieldNameByKey As String = GetFieldNameByKey(field.Key.ToString(), workItemFields)
						Dim cRMLinkField As CRMLinkField = customFields.FindByCRMFieldName(fieldNameByKey)

						If (cRMLinkField Is Nothing) Then
							cRMLinkField = customFields.FindByCRMFieldName(field.Key.ToString())

							If (Not cRMLinkField Is Nothing) Then
								AddLog(String.Format("The mapped field was not found by name {0}, it was searched and found by reference name {1} instead.", fieldNameByKey, field.Key))
							End If
						End If

						Dim crmLinkCustomFieldError As CRMLinkError = Nothing

						Try
							'Verify the field is mapped or part of the Select Case below (if more added there then add them to this check too)
							Dim isCustomMappedField As Boolean = cRMLinkField IsNot Nothing _
															OrElse field.Key.Trim().ToLower() = "system.workitemtype" _
															OrElse field.Key.Trim().ToLower() = "system.teamproject" _
															OrElse field.Key.Trim().ToLower() = "system.state"

							If (isCustomMappedField) Then
								crmLinkCustomFieldError = crmlinkErrors.FindByObjectIDAndFieldName(ticketID.ToString(), field.Key)
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
								Case "system.workitemtype"
									Dim currentType As TicketType = allTypes.FindByTicketTypeID(updateTicket(0).TicketTypeID)
									Dim newType As TicketType = Nothing
									Dim mappedTicketType As CRMLinkField = ticketTypeMappings.FindByCRMFieldName(value)
									If mappedTicketType Is Nothing Then
										newType = allTypes.FindByName(value)
									Else
										newType = allTypes.FindByName(mappedTicketType.TSFieldName)
									End If

									Dim updateType As Boolean = CRMLinkRow.UpdateTicketType

									If updateType AndAlso newType IsNot Nothing AndAlso newType.TicketTypeID <> currentType.TicketTypeID Then
										updateTicket(0).TicketTypeID = newType.TicketTypeID
										ticketValuesChanged = True
									End If
								Case "system.teamproject"
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
								Case "system.state"
									Dim currentStatus As TicketStatus = allStatuses.FindByTicketStatusID(updateTicket(0).TicketStatusID)
									Dim newStatus As TicketStatus = allStatuses.FindByName(value, updateTicket(0).TicketTypeID)
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
										If ticketLinkToTFSItem IsNot Nothing AndAlso (ticketLinkToTFSItem.TFSState Is Nothing OrElse ticketLinkToTFSItem.TFSState <> value) Then
											Dim fromStatement As String = String.Empty

											If ticketLinkToTFSItem.TFSState IsNot Nothing Then
												fromStatement = " from """ + ticketLinkToTFSItem.TFSState + """"
											End If

											Dim newAction As Actions = New Actions(User)
											newAction.AddNewAction()
											newAction(0).ActionTypeID = newActionsTypeID
											newAction(0).TicketID = updateTicket(0).TicketID
											newAction(0).Description = "TFS' Work Item " + workItem.Fields("System.Title").ToString() + "'s state changed" + fromStatement + " to """ + value + """."

											Dim actionLinkToTFS As ActionLinkToTFS = New ActionLinkToTFS(User)
											Dim actionLinkToTFSItem As ActionLinkToTFSItem = actionLinkToTFS.AddNewActionLinkToTFSItem(newAction(0).ActionID)

											actionLinkToTFSItem.TFSID = -1
											actionLinkToTFSItem.DateModifiedByTFSSync = DateTime.UtcNow()
											newAction.Save()
											'actionLinkToTFSItem.ActionID = newAction(0).ActionID
											actionLinkToTFS.Save()
											ticketValuesChanged = True
										End If
									End If

									If (ticketLinkToTFSItem.TFSState.ToLower().Trim() <> value.ToLower().Trim()) Then
										ticketLinkToTFSItem.TFSState = value
										workItemStateChangedNotUpdatedInTicket = True
									End If
							End Select
						End If
					Next

					If ticketValuesChanged Then
						ticketLinkToTFSItem.DateModifiedByTFSSync = DateTime.UtcNow
						updateTicket.Save()
						ticketLinkToTFSItem.Collection.Save()

						Dim actionLogDescription As String = "Updated Ticket with TFS Work Item Title: '" + workItem.Fields("System.Title").ToString() + "' changes."
						ActionLogs.AddActionLog(User, ActionLogType.Update, ReferenceType.Tickets, updateTicket(0).TicketID, actionLogDescription)
					Else
						AddLog("ticketID: " + updateTicket(0).TicketID.ToString() + " values were not updated because have not changed.")

						If workItemStateChangedNotUpdatedInTicket Then
							ticketLinkToTFSItem.DateModifiedByTFSSync = DateTime.UtcNow
							ticketLinkToTFSItem.Collection.Save()
							AddLog("The ticket status in the TicketLinkToTFS was updated because it changed but was not updated in TeamSupport because either it is setup to be excluded or it does not exist. TFS State:" + ticketLinkToTFSItem.TFSState)
						End If
					End If

					sendPusherMessage = ticketValuesChanged
				Else
					AddLog("Ticket with ID: """ + ticketID.ToString() + """ was not found to be updated.")
					sendPusherMessage = False
				End If
			End Sub

			Private Function GetFieldValue(ByVal field As KeyValuePair(Of String, Object)) As String
				Dim result As String = Nothing

				'If field.Key.ToString().ToLower().Contains("customfield") = True Then
				'    If field.Value.HasValues AndAlso TypeOf (field.Value) Is JObject Then
				'        result = field.Value("value").ToString()
				'    ElseIf field.Value.HasValues AndAlso TypeOf (field.Value) Is JArray Then
				'        Try
				'            Dim fieldValue = field.Value(0).ToString()
				'            Dim fieldValueDictionary = fieldValue.Split(","c).Select(Function(kvp) kvp.Split("="c)).ToDictionary(
				'                                                                                                    Function(kvp) kvp(0),
				'                                                                                                    Function(kvp) kvp(1))

				'            If (fieldValueDictionary.Where(Function(p) p.Key.ToLower() = "name").Any()) Then
				'                result = fieldValueDictionary.Where(Function(p) p.Key.ToLower() = "name").Select(Function(p) p.Value).FirstOrDefault()
				'            Else
				result = field.Value.ToString()
				'            End If
				'        Catch ex As Exception
				'            Try
				'                'ToDo //vv This probably is a multiselect jira field. For now we are only taking the first value. We need to handle those fields correctly
				'                result = field.Value(0)("value").ToString()
				'            Catch innerEx As Exception
				'                AddLog(String.Format("Unhandled Jira field type {0} with value: {1}{2}", field.Key.ToString(), Environment.NewLine, field.Value(0).ToString()))
				'            End Try

				'        End Try

				'    Else
				'        result = field.Value.ToString()
				'    End If

				'    If result.ToLower() = "yes" Then
				'        result = "True"
				'    ElseIf result.ToLower() = "no" Then
				'        result = "False"
				'    End If
				'Else
				'    Select Case field.Key.ToString().ToLower()
				'        Case "aggregatetimeestimate",
				'    "aggregatetimeoriginalestimate",
				'    "aggregatetimespent",
				'    "created",
				'    "description",
				'    "duedate",
				'    "environment",
				'    "lastviewed",
				'    "resolutiondate",
				'    "summary",
				'    "updated",
				'    "workratio",
				'    "timeestimate",
				'    "timeoriginalestimate",
				'    "timespent"
				'            result = field.Value.ToString()
				'        Case "assignee"
				'            'Because some orgs are mapping to the Assignee Jira field and they seem to be storing the email address then we need to leave it here, for the rest we should use the Name so it also links from TS to Jira when creating the issue
				'            If (CRMLinkRow.OrganizationID = 461956 OrElse CRMLinkRow.OrganizationID = 869700 OrElse CRMLinkRow.OrganizationID = 884116) Then
				'                result = field.Value("emailAddress").ToString()
				'            Else
				'                result = field.Value("name").ToString()
				'            End If
				'        Case "reporter"
				'            'Because some orgs are mapping to the Assignee Jira field and they seem to be storing the email address then we need to leave it here, for the rest we should use the Name so it also links from TS to Jira when creating the issue
				'            If (CRMLinkRow.OrganizationID = 930653 OrElse CRMLinkRow.OrganizationID = 1028984 OrElse CRMLinkRow.OrganizationID = 1136748 OrElse CRMLinkRow.OrganizationID = 995322) Then
				'                result = field.Value("emailAddress").ToString()
				'            Else
				'                result = field.Value("name").ToString()
				'            End If
				'        Case "issuetype", "status", "priority", "resolution"
				'            result = field.Value("name").ToString()
				'        Case "progress", "worklog"
				'            result = field.Value("total").ToString()
				'        Case "project"
				'            result = field.Value("key").ToString()
				'        Case "votes"
				'            result = field.Value("votes").ToString()
				'        Case "watches"
				'            result = field.Value("watchCount").ToString()
				'        Case "timetracking"
				'            result = field.Value("timeSpentSeconds").ToString()
				'        Case "aggregrateprogress"
				'            result = field.Value("progress").ToString()
				'        Case "attachment"
				'            Dim attachmentsArray As JArray = DirectCast(field.Value, JArray)
				'            Dim resultBuilder As StringBuilder = New StringBuilder()
				'            Dim preffix = String.Empty

				'            For i = 0 To attachmentsArray.Count - 1
				'                resultBuilder.Append(preffix)
				'                resultBuilder.Append(attachmentsArray(i)("content").ToString())

				'                If preffix = String.Empty Then
				'                    preffix = ", "
				'                End If
				'            Next

				'            result = resultBuilder.ToString()
				'        Case "labels"
				'            Dim labelsArray As JArray = DirectCast(field.Value, JArray)
				'            Dim resultBuilder As StringBuilder = New StringBuilder()
				'            Dim preffix = String.Empty

				'            For i = 0 To labelsArray.Count - 1
				'                resultBuilder.Append(preffix)
				'                resultBuilder.Append(labelsArray(i).ToString())

				'                If preffix = String.Empty Then
				'                    preffix = ", "
				'                End If
				'            Next

				'            result = resultBuilder.ToString()
				'        Case "issuelinks"
				'            Dim issuelinksArray As JArray = DirectCast(field.Value, JArray)
				'            Dim resultBuilder As StringBuilder = New StringBuilder()
				'            Dim preffix = String.Empty

				'            For i = 0 To issuelinksArray.Count - 1
				'                resultBuilder.Append(preffix)

				'                If issuelinksArray(i)("outwardIssue") IsNot Nothing Then
				'                    resultBuilder.Append(issuelinksArray(i)("outwardIssue")("key").ToString())
				'                Else
				'                    resultBuilder.Append(issuelinksArray(i)("inwardIssue")("key").ToString())
				'                End If

				'                If preffix = String.Empty Then
				'                    preffix = ", "
				'                End If
				'            Next

				'            result = resultBuilder.ToString()
				'        Case "versions", "fixversions"
				'            Dim versionsArray As JArray = DirectCast(field.Value, JArray)
				'            Dim resultBuilder As StringBuilder = New StringBuilder()
				'            Dim preffix = String.Empty

				'            For i = 0 To versionsArray.Count - 1
				'                resultBuilder.Append(preffix)

				'                Try
				'                    resultBuilder.Append(versionsArray(i)("name").ToString())
				'                Catch ex As Exception
				'                    resultBuilder.Append(versionsArray(i)("description").ToString())
				'                End Try

				'                If preffix = String.Empty Then
				'                    preffix = ", "
				'                End If
				'            Next

				'            result = resultBuilder.ToString()
				'        Case "subtasks"
				'            Dim subtasksArray As JArray = DirectCast(field.Value, JArray)
				'            Dim resultBuilder As StringBuilder = New StringBuilder()
				'            Dim preffix = String.Empty

				'            For i = 0 To subtasksArray.Count - 1
				'                resultBuilder.Append(preffix)
				'                resultBuilder.Append(subtasksArray(i)("fields")("summary").ToString())

				'                If preffix = String.Empty Then
				'                    preffix = ", "
				'                End If
				'            Next

				'            result = resultBuilder.ToString()
				'        Case "components"
				'            Dim componentsArray As JArray = DirectCast(field.Value, JArray)
				'            Dim resultBuilder As StringBuilder = New StringBuilder()
				'            Dim preffix = String.Empty

				'            For i = 0 To componentsArray.Count - 1
				'                resultBuilder.Append(preffix)
				'                resultBuilder.Append(componentsArray(i)("name").ToString())

				'                If preffix = String.Empty Then
				'                    preffix = ", "
				'                End If
				'            Next

				'            result = resultBuilder.ToString()
				'    End Select
				'End If

				Return result
			End Function

			Private Function GetFieldNameByKey(ByVal fieldKey As String, ByVal workItemFields As List(Of TFSLibrary.WorkItemField))
				Dim result As StringBuilder = New StringBuilder()

				If workItemFields IsNot Nothing Then
					For Each field As TFSLibrary.WorkItemField In workItemFields
						If field.referenceName IsNot Nothing AndAlso fieldKey = field.referenceName Then
							result.Append(field.name)
							Exit For
						End If
					Next
				End If

				'If result.Length = 0 Then
				'    result.Append(fieldKey)
				'End If

				Return result.ToString()
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

			Private Function GetNewComments(ByVal workItemId As Integer, ByVal ticketID As Integer) As List(Of WorkItemComment)
				Dim newComments As List(Of WorkItemComment) = New List(Of WorkItemComment)
				Dim comments As TFSLibrary.WorkItemCommentList = _tfs.GetCommentsBy(workItemId)
				Dim ticketActionsLinked As ActionLinkToTFS = New ActionLinkToTFS(User)

				ticketActionsLinked.LoadByTicketID(ticketID)

				For Each comment As WorkItemComment In comments.Comments
					If GetIsNewComment(comment, ticketActionsLinked) Then
						newComments.Add(comment)
					End If
				Next

				Return newComments
			End Function

			Private Function GetNewHistoryAsComments(ByVal workItemId As Integer, ByVal ticketID As Integer) As List(Of WorkItemComment)
				Dim newHistory As List(Of WorkItemHistory) = New List(Of WorkItemHistory)
				Dim history As TFSLibrary.WorkItemHistoryList = _tfs.GetHistoryBy(workItemId)
				Dim ticketActionsLinked As ActionLinkToTFS = New ActionLinkToTFS(User)

				ticketActionsLinked.LoadByTicketID(ticketID)

				Dim newComments As List(Of WorkItemComment) = New List(Of WorkItemComment)

				For Each historyEntry As WorkItemHistory In history.value
					Dim comment As WorkItemComment = ConvertHistoryToComment(historyEntry)
					If GetIsNewComment(comment, ticketActionsLinked) Then
						newComments.Add(comment)
					End If
				Next

				Return newComments
			End Function

			Private Function ConvertHistoryToComment(ByVal history As WorkItemHistory) As WorkItemComment
				Dim result As WorkItemComment = New WorkItemComment()

				result.Text = history.Value
				result.RevisedBy = history.RevisedBy
				result.RevisedDate = history.RevisedDate
				result.Revision = history.Rev
				result.Url = history.Url
				'result.Links = history.Links

				Return result
			End Function
			Private Function GetIsNewComment(ByVal comment As WorkItemComment, ByVal ticketActionsLinked As ActionLinkToTFS) As Boolean
				Dim result As Boolean = False
				Dim crmLinkHostName As String = CRMLinkRow.HostName
				Dim commentUrl As String = comment.Url
				crmLinkHostName = ExtractSubAndMainDomainFromURL(crmLinkHostName)
				commentUrl = ExtractSubAndMainDomainFromURL(commentUrl)

				If (comment.Text.Length < 19 OrElse comment.Text.Substring(0, 20) <> "TeamSupport ticket #" AndAlso crmLinkHostName = commentUrl) Then
					Dim pulledComment As ActionLinkToTFSItem = ticketActionsLinked.FindByTFSID(CType(comment.Revision.ToString(), Integer?))
					If pulledComment Is Nothing Then
						result = True
					End If
				ElseIf (crmLinkHostName <> commentUrl) Then
					AddLog("The url of the comment pulled from TFS does not match the hostname of the CRMLinkTable record, skipping. CRMLinkRow: id=" + CRMLinkRow.CRMLinkID.ToString() + " OrgId=" + CRMLinkRow.OrganizationID.ToString() + " HostName=" + CRMLinkRow.HostName.ToString() + ". Comment url=" + comment.Url.ToString())
				End If

				Return result
			End Function

			Private Sub AddNewCommentsInTicket(ByVal ticketID As Integer, ByRef newComments As List(Of WorkItemComment), ByVal newActionsTypeID As Integer, ByRef crmLinkActionErrors As CRMLinkErrors)
				Dim crmLinkError As CRMLinkError = Nothing

				For i = 0 To newComments.Count - 1
					crmLinkError = crmLinkActionErrors.FindByObjectIDAndFieldName(newComments(i).Revision.ToString(), String.Empty)

					Try
						Dim updateActions As Actions = New Actions(User)
						updateActions.AddNewAction()
						updateActions(0).TicketID = ticketID
						updateActions(0).ActionTypeID = newActionsTypeID

						Dim commentDescription = newComments(i).Text
						Dim firstLine As String = "<p><em>Comment added in TFS {0}{1}</em></p> <p>&nbsp;</p>"
						Dim author As String = String.Empty

						Try
							author = newComments(i).RevisedBy.Name
						Catch ex As Exception
							AddLog("The revisedBy name was not found for the comment.")
						End Try

						Dim addedOnTFSString As String = String.Empty

						Try
							Dim addedOnTFS As Date = Convert.ToDateTime(newComments(i).RevisedDate)

							If (DateDiff(DateInterval.Day, Today.Date, addedOnTFS.Date) <> 0) Then
								addedOnTFSString = addedOnTFS.ToString()
							End If
						Catch ex As Exception
							AddLog("The revisedDate was not found for the comment.")
						End Try

						firstLine = String.Format(firstLine,
												If(String.IsNullOrEmpty(author), "", "by " + author),
												If(String.IsNullOrEmpty(addedOnTFSString), "", " on " + addedOnTFSString))
						Dim TFSCommentId = CType(newComments(i).Revision.ToString(), Integer)
						commentDescription = firstLine + commentDescription
						updateActions(0).Description = commentDescription
						updateActions.ActionLogInstantMessage = "TFS Comment ID: " + TFSCommentId.ToString() + " Created In TeamSupport Action "
						updateActions.Save()
						Dim actionLinkToTFS As ActionLinkToTFS = New ActionLinkToTFS(User)
						Dim actionLinkToTFSItem As ActionLinkToTFSItem = actionLinkToTFS.AddNewActionLinkToTFSItem(updateActions(0).ActionID)
						actionLinkToTFSItem.TFSID = TFSCommentId
						actionLinkToTFSItem.DateModifiedByTFSSync = DateTime.UtcNow
						'actionLinkToTFSItem.ActionID = updateActions(0).ActionID
						actionLinkToTFS.Save()

						ClearCrmLinkError(crmLinkError)
					Catch ex As Exception
						AddLog(ex.ToString() + ex.StackTrace,
							LogType.TextAndReport,
							crmLinkError,
							ex.Message,
							Orientation.IntoTeamSupport,
							ObjectType.Action,
							newComments(i).Revision.ToString(),
							String.Empty,
							newComments(i).Text.ToString(),
							OperationType.Create)
					End Try
				Next
			End Sub

			Private Function IsHostMatching(ByVal hostName1, ByVal hostName2) As Boolean
				Dim host1 As Uri = New Uri(hostName1)
				Dim host2 As Uri = New Uri(hostName2)

				Return host1.Host = host2.Host
			End Function

			Private Function ExtractSubAndMainDomainFromURL(URL As String) As String
				' cut-off any url encoded data
				URL = URL.Split("?"c)(0)
				'return array of segments between slashes
				Dim URLparts() As String = URL.Split("/"c)
				'find first segment with periods/full-stops
				Dim Domain As String = Array.Find(URLparts, Function(x) (x.Contains(".")))
				'check if nothing returned - if necessary
				If IsNothing(Domain) Then Domain = String.Empty

				Return Domain
			End Function
		End Class
	End Namespace
End Namespace