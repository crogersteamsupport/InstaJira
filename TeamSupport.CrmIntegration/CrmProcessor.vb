Imports TeamSupport.Data
Imports TeamSupport.ServiceLibrary
Imports System.IO
Imports System.Text

Namespace TeamSupport
    Namespace CrmIntegration


        <Serializable()> Public Class CrmProcessor
            Inherits ServiceThread

            Public Overridable ReadOnly Property IsStopped() As Boolean
                Get
                    SyncLock Me
                        Return _isStopped Or ServiceStopped Or CrmPool.CrmPoolStopped
                    End SyncLock
                End Get
            End Property

            Private _crmLinkID As Integer
            Public ReadOnly Property CrmLinkID() As Integer
                Get
                    Dim result As Integer
                    SyncLock Me
                        result = _crmLinkID
                    End SyncLock
                    Return result
                End Get
            End Property

            Public Sub New(ByVal cmrLinkId As Integer)
                _crmLinkID = cmrLinkId
                IsLoop = False
            End Sub

            Public Overrides Sub Run()
                Dim CrmLinkRow As CRMLinkTableItem = CRMLinkTable.GetCRMLinkTableItem(LoginUser, _crmLinkID)

                Try
                    ProcessCrmLink(CrmLinkRow)
                Catch ex As Exception
                    ex.Data("CRMLinkID") = _crmLinkID
                    ExceptionLogs.LogException(LoginUser, ex, "Service - " & ServiceName, CrmLinkRow.Row)
                End Try

                'update last processed date/time
                CrmLinkRow.LastProcessed = DateTime.UtcNow
                CrmLinkRow.Collection.Save()
            End Sub

            ''' <summary>
            ''' Process a given integration
            ''' </summary>
            ''' <param name="CRMLinkTableItem">A row from CRMLinkTable, which maps to a single integration for a given company</param>
            ''' <remarks></remarks>
            Public Sub ProcessCrmLink(ByVal CRMLinkTableItem As CRMLinkTableItem)
                If IsStopped Then
                    Return
                End If

                Try
                    'CrmLinkTableItem.CRMType should match up with one of the items in the IntegrationType enumeration
                    Dim CRMType As IntegrationType = [Enum].Parse(GetType(IntegrationType), CRMLinkTableItem.CRMType, True)

                    'set up log per crm link item
                    Dim Log As SyncLog = Nothing
                    Dim LogPath As String = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Logs")

                    If Not Directory.Exists(LogPath) Then
                        Directory.CreateDirectory(LogPath)
                    End If

                    LogPath = Path.Combine(LogPath, CRMLinkTableItem.OrganizationID.ToString())

                    Log = New SyncLog(LogPath, CRMType)

                    Dim CRM As Integration = Nothing

                    Select Case CRMType
                        Case IntegrationType.Batchbook
                            If Settings.ReadBool("BatchBook Enabled", True) Then
                                CRM = New BatchBook(CRMLinkTableItem, Log, LoginUser, Me)
                            Else
                                Return
                            End If
                        Case IntegrationType.Highrise
                            If Settings.ReadBool("Highrise Enabled", True) Then
                                CRM = New Highrise(CRMLinkTableItem, Log, LoginUser, Me)
                            Else
                                Return
                            End If
                        Case IntegrationType.SalesForce
                            If Settings.ReadBool("SalesForce Enabled", True) Then
                                CRM = New SalesForce(CRMLinkTableItem, Log, LoginUser, Me)
                            Else
                                Return
                            End If
                        Case IntegrationType.MailChimp
                            If Settings.ReadBool("MailChimpEnabled", True) Then
                                CRM = New MailChimp(CRMLinkTableItem, Log, LoginUser, Me)
                            Else
                                Return
                            End If
                        Case IntegrationType.ZohoCRM
                            If Settings.ReadBool("ZohoCRMEnabled", True) Then
                                CRM = New ZohoCRM(CRMLinkTableItem, Log, LoginUser, Me)
                            Else
                                Return
                            End If
                        Case IntegrationType.ZohoReports
                            If Settings.ReadBool("ZohoReportsEnabled", True) Then
                                CRM = New ZohoReports(CRMLinkTableItem, Log, LoginUser, Me)
                            Else
                                Return
                            End If
                        Case IntegrationType.Jira
                            If Settings.ReadBool("Jira Enabled", True) Then
                                CRM = New Jira(CRMLinkTableItem, Log, LoginUser, Me)
                            Else
                                Return
                            End If
                        Case IntegrationType.Oracle
                            If Settings.ReadBool("Oracle Enabled", True) Then
                                CRM = New Oracle(CRMLinkTableItem, Log, LoginUser, Me)
                            Else
                                Return
                            End If
                        Case IntegrationType.HubSpot
                            If Settings.ReadBool("HubSpot Enabled", True) Then
                                CRM = New HubSpot(CRMLinkTableItem, Log, LogPath, LoginUser, Me)
                            End If
                        Case IntegrationType.TFS
							If Settings.ReadBool("TeamFoundationServices Enabled", True) Then
								CRM = New TFS(CRMLinkTableItem, Log, LoginUser, Me)
							Else
								Return
							End If
						Case IntegrationType.ServiceNow
							'This integration is not done by this service. See WebHooksProcessor.
							Return
					End Select

                    Dim isDebug As Boolean = Settings.ReadBool("Debug", False)

                    If isDebug Then
                        Log.Write("Running as Debug. Processing only TeamSupport Sandbox account (13679)")
                    End If

                    If (CRM IsNot Nothing AndAlso Not isDebug) OrElse (CRM IsNot Nothing AndAlso isDebug AndAlso CRMLinkTableItem.OrganizationID = 13679) Then
                        Dim jiraInstanceName As String = String.Empty

                        If (Not String.IsNullOrEmpty(CRMLinkTableItem.InstanceName) AndAlso CRMType = IntegrationType.Jira) Then
                            jiraInstanceName = String.Format(" Instance: {0}", CRMLinkTableItem.InstanceName)
                        End If

						Log.Write(String.Format("Thread [{2}] Begin processing {0} sync.{1}",
									CRMType.ToString(),
									If(String.IsNullOrEmpty(jiraInstanceName), "", jiraInstanceName),
									Thread.ManagedThreadId))

                        Try
                            'if sync processed successfully, log that message. otherwise log an error
                            If CRM.PerformSync() Then
                                CRMLinkTableItem.LastLink = DateTime.UtcNow
                                CRMLinkTableItem.Collection.Save()
                                Dim synchedOrganizations As New CRMLinkSynchedOrganizations(LoginUser)
                                synchedOrganizations.DeleteByCRMLinkTableID(CRMLinkTableItem.CRMLinkID)
								Log.Write(String.Format("Thread {0} Finished processing successfully.", Thread.ManagedThreadId))

                                Dim pushTicketsAndPullCasesMessage As StringBuilder = New StringBuilder()
                                If CRMLinkTableItem.PullCasesAsTickets Then
                                    pushTicketsAndPullCasesMessage.Append(" pulling cases")
                                End If
                                If CRMLinkTableItem.PushTicketsAsCases Then
                                    If pushTicketsAndPullCasesMessage.Length > 0 Then
                                        pushTicketsAndPullCasesMessage.Append(" and")
                                    End If
                                    pushTicketsAndPullCasesMessage.Append(" pushing tickets as cases")
                                End If
                                If CRMLinkTableItem.SendBackTicketData Then
                                    If pushTicketsAndPullCasesMessage.Length > 0 Then
                                        pushTicketsAndPullCasesMessage.Append(" and")
                                    End If
                                    pushTicketsAndPullCasesMessage.Append(" pushing tickets as ")
                                    If CRMLinkTableItem.CRMType = "Jira" Then
                                        pushTicketsAndPullCasesMessage.Append("issues")
                                    Else
                                        pushTicketsAndPullCasesMessage.Append("notes")
                                    End If
                                End If
                                Integration.LogSyncResult(CRMType.ToString() & " Sync Completed" & pushTicketsAndPullCasesMessage.ToString() & ".",
                                   CRMLinkTableItem.OrganizationID,
                                   LoginUser)
                            Else
                                'migrating towards using IntegrationException instead of IntegrationError
                                If CRM.Exception IsNot Nothing Then
                                    CRM.LogSyncResult(String.Format("Error reported in {0} sync{1}: {2}", CRMType.ToString(), jiraInstanceName, CRM.Exception.Message))
                                    Log.Write(String.Format("Error reported in {0} sync: {1}", CRMType.ToString(), CRM.Exception.Message))
                                ElseIf CRM.ErrorCode <> IntegrationError.None And CRM.ErrorCode <> IntegrationError.Unknown Then
                                    Integration.LogSyncResult(String.Format("Error reported in {0} sync: {1}", CRMType.ToString(), CRM.ErrorCode.ToString()), CRMLinkTableItem.OrganizationID, LoginUser)
                                    Log.Write(String.Format("Error reported in {0} sync: {1}", CRMType.ToString(), CRM.ErrorCode.ToString()))
                                Else
                                    Integration.LogSyncResult(String.Format("Error reported in {0} sync. Last link date/time not updated.", CRMType.ToString()), CRMLinkTableItem.OrganizationID, LoginUser)
                                    Log.Write(String.Format("Error reported in {0} sync. Last link date/time not updated.", CRMType.ToString()))
                                End If

                                Log.Write("Finished processing with errors.")
                            End If
                        Catch ex As Exception
                            Log.Write(String.Format("Sync Error: {0}", ex.Message))
                            Log.Write(String.Format("Sync Error: {0}", ex.StackTrace))
                            Throw ex
                        End Try
                    End If
                Catch ex As Exception
                    Integration.LogSyncResult(String.Format("{0} Sync Error Reported.", CRMLinkTableItem.CRMType), CRMLinkTableItem.OrganizationID, LoginUser)
                    ExceptionLogs.LogException(LoginUser, ex, "CRM Processor", CRMLinkTableItem.Row)
                End Try
            End Sub

        End Class

    End Namespace
End Namespace


