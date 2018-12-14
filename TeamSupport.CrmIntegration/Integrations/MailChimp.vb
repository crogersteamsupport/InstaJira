Imports TeamSupport.Data
Imports System.IO
Imports System.Net
Imports System.Text

Namespace TeamSupport
    Namespace CrmIntegration

        Public Class MailChimp
            Inherits Integration

            Private Const BatchSize As Integer = 4000
            Private datacenter As String
            Private apiVersion As String = "3.0"

            Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor)
                MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, IntegrationType.MailChimp)

                'parse the api key to get datacenter for url
                datacenter = CRMLinkRow.SecurityToken1.Substring(CRMLinkRow.SecurityToken1.LastIndexOf("-") + 1)
            End Sub


            Public Overrides Function PerformSync() As Boolean
                'get the mailchimp list to sync to
                Dim listID As String = GetImportListID()

                If Processor.IsStopped Then
                    Return False
                End If

                If listID IsNot Nothing Then
                    Log.Write("list id: " & listID)
                    'get customer email addresses for this account
                    Dim theseCustomers As New Organizations(User)
                    theseCustomers.LoadByParentID(CRMLinkRow.OrganizationID, False)

                    Dim emailBatches As New List(Of StringBuilder)()
                    Dim emailBatch As StringBuilder = Nothing
                    Dim unsubBatch As StringBuilder = Nothing
                    Dim emailsAdded As List(Of String) = New List(Of String)
                    Dim emailsRemoved As List(Of String) = New List(Of String)
                    Dim emailIndex As Integer = 0
                    Dim unsubIndex As Integer = 0
                    Dim memberBodySingleItem = "{{
                            		""email_address"":""{0}"", ""status"":""{1}"", ""merge_fields"": {{ ""FNAME"": ""{2}"", ""LNAME"": ""{3}""{4} }}
                            	}},"

                    For Each customer As Organization In theseCustomers

                        Dim theseContacts As New Users(User)
                        theseContacts.LoadByOrganizationID(customer.OrganizationID, False)

                        For Each contact As User In theseContacts
                            If Processor.IsStopped Then
                                Return False
                            End If

                            If contact.IsActive AndAlso customer.IsActive Then
                                If (CRMLinkRow.LastLink Is Nothing Or CType(contact.Row("DateModified"), DateTime).AddMinutes(30) > CRMLinkRow.LastLink _
                                    Or CType(customer.Row("DateModified"), DateTime).AddMinutes(30) > CRMLinkRow.LastLink) And Not contact.MarkDeleted Then
                                    If emailBatch Is Nothing Then
                                        emailBatch = New StringBuilder()
                                    End If

                                    Dim companyName As String = String.Empty

                                    'Send company name to MMERGE3 field of MC. Only for TeamSupport (1078)
                                    If CRMLinkRow.OrganizationID = 1078 OrElse CRMLinkRow.OrganizationID = 13679 Then
                                        Dim contactView As ContactsView = New ContactsView(User)
                                        Dim filter As Specialized.NameValueCollection = New Specialized.NameValueCollection()
                                        filter.Add("email", contact.Email)
                                        contactView.LoadByParentOrganizationID(CRMLinkRow.OrganizationID, filter, orderBy:="DateModified DESC", limitNumber:=1, useMaxDop:=True)

                                        Dim contactCompany As String = String.Empty
                                        If Not contactView Is Nothing AndAlso contactView.Count > 0 Then
                                            contactCompany = contactView(0).Organization
                                        End If

                                        companyName = ", ""MMERGE3"": """ & Web.HttpUtility.UrlEncode(contactCompany) + """"
                                    End If

                                    If (String.IsNullOrEmpty(contact.Email)) Then
                                        Log.Write("This contact was excluded of the batch because it contains no email: " + String.Format(memberBodySingleItem, contact.Email, "subscribed", contact.FirstName, contact.LastName, companyName))
                                    ElseIf (emailsAdded.Count > 0 AndAlso emailsAdded.Find(Function(x) x = contact.Email) = contact.Email) Then
                                        Log.Write("This contact was excluded of the batch because the email is duplicated in other contact in this batch: " + String.Format(memberBodySingleItem, contact.Email, "subscribed", contact.FirstName, contact.LastName, companyName))
                                    ElseIf (Not contact.Email.Contains("@")) Then
                                        Log.Write("This contact was excluded of the batch because it does not appear to have a valid email: " + String.Format(memberBodySingleItem, contact.Email, "subscribed", contact.FirstName, contact.LastName, companyName))
                                    Else
                                        emailBatch.Append(String.Format(memberBodySingleItem, contact.Email.Trim(), "subscribed", contact.FirstName, contact.LastName, companyName))
                                        emailsAdded.Add(contact.Email.Trim())
                                        emailIndex += 1
                                    End If
                                End If

                                If emailIndex = BatchSize Then
                                    emailBatches.Add(emailBatch)
                                    emailIndex = 0
                                    emailBatch = Nothing
                                End If

                            Else
                                If CRMLinkRow.LastLink Is Nothing Or CType(contact.Row("DateModified"), DateTime).AddMinutes(30) > CRMLinkRow.LastLink _
                                    Or CType(customer.Row("DateModified"), DateTime).AddMinutes(30) > CRMLinkRow.LastLink Then

                                    If unsubBatch Is Nothing Then
                                        unsubBatch = New StringBuilder()
                                    End If

                                    If (String.IsNullOrEmpty(contact.Email)) Then
                                        Log.Write("This contact was excluded for unsubscribe because it contains no email: " + String.Format(memberBodySingleItem, contact.Email, "subscribed", contact.FirstName, contact.LastName, emailsRemoved))
                                    ElseIf (emailsRemoved.Count > 0 AndAlso emailsRemoved.Find(Function(x) x = contact.Email) = contact.Email) Then
                                        Log.Write("This contact was excluded for unsubscribe because the email is duplicated in other contact in this batch: " + String.Format(memberBodySingleItem, contact.Email, "subscribed", contact.FirstName, contact.LastName, emailsRemoved))
                                    ElseIf (Not contact.Email.Contains("@")) Then
                                        Log.Write("This contact was excluded for unsubscribe because it does not appear to have a valid email: " + String.Format(memberBodySingleItem, contact.Email, "subscribed", contact.FirstName, contact.LastName, emailsRemoved))
                                    Else
                                        unsubBatch.Append(String.Format(memberBodySingleItem, contact.Email, "unsubscribed", contact.FirstName, contact.LastName, String.Empty))
                                        emailsRemoved.Add(contact.Email.Trim())
                                        unsubIndex += 1
                                    End If
                                End If
                            End If
                        Next
                    Next

                    If emailBatch IsNot Nothing Then
                        emailBatches.Add(emailBatch)
                    End If

                    If emailBatches.Count > 0 Then
                        'send the lists to mailchimp
                        For Each mailList As StringBuilder In emailBatches
                            If Not BatchSubscribeUnsubscribe_v3(mailList.ToString().Substring(0, mailList.ToString().Length - 1), listID) Then
                                Log.Write("Error in BatchSubscribe.")
                                Return False
                            End If
                        Next

                        Log.Write(emailsAdded.Count.ToString() + " email addresses sent to Mailchimp in " & emailBatches.Count.ToString() & " batches.")
                    Else
                        Log.Write("No new email addresses to sync.")
                    End If

                    If unsubBatch IsNot Nothing Then
                        If Not BatchSubscribeUnsubscribe_v3(unsubBatch.ToString().Substring(0, unsubBatch.ToString().Length - 1), listID) Then
                            Log.Write("Error in BatchUnsubscribe.")
                            Return False
                        End If

                        Log.Write("Unsubscribed " & unsubIndex.ToString() & " addresses.")
                    End If

                Else
                    LogSyncResult("Error: The specified list does not exist in MailChimp.")
                    Return False
                End If

                Return True
            End Function

            ''' <summary>
            ''' Gets the MailChimp list ID for the list with a name matching the CRMLinkRow.UserName value
            ''' </summary>
            ''' <returns>a MailChimp-specific list ID</returns>
            ''' <remarks></remarks>
            Private Function GetImportListID() As String
                Dim returnID As String = Nothing

                Dim MCUri As New Uri("https://" & datacenter & ".api.mailchimp.com/" & apiVersion & "/lists?apikey=" & CRMLinkRow.SecurityToken1)
                Dim response As String = ServiceLibrary.MailChimp.MakeHttpWebRequestGet(MCUri.AbsoluteUri)
                Dim listsObject As ServiceLibrary.ListsObject.Lists = Newtonsoft.Json.JsonConvert.DeserializeObject(Of ServiceLibrary.ListsObject.Lists)(response)

                If listsObject IsNot Nothing AndAlso listsObject.TotalItems > 0 Then
                    For Each list As ServiceLibrary.ListsObject.List In listsObject.lists
                        If (Trim(list.name).ToLower() = Trim(CRMLinkRow.Username).ToLower()) Then
                            returnID = list.id
                            Exit For
                        End If
                    Next
                End If

                Return returnID
            End Function

            'Private Function GetMailChimpXML(ByVal PathAndQuery As String) As XmlDocument
            '    Dim MCUri As New Uri("https://" & datacenter & ".api.mailchimp.com/" & apiVersion & "/" & PathAndQuery)
            '    Return GetXML(MCUri)
            'End Function

            Private Function BatchSubscribe(ByVal postString As String) As Boolean
                Dim MCUri As New Uri("https://" & datacenter & ".api.mailchimp.com/1.3/?method=listBatchSubscribe")
                Dim postData As String = postString

                Return PostQueryString(Nothing, MCUri, postData) = HttpStatusCode.OK
            End Function

            Private Function BatchSubscribeUnsubscribe_v3(ByVal postString As String, ByVal listID As String) As Boolean
                Dim result As Boolean = False
                Dim MCUri As New Uri("https://" & datacenter & ".api.mailchimp.com/" & apiVersion & "/lists/" + listID)
                Dim bodyPost As String = postString

                Try
                    Dim encodedCredentials = DataUtils.GetEncodedCredentials("anystring", CRMLinkRow.SecurityToken1)
                    bodyPost = "{ ""members"": [" + bodyPost + "], ""update_existing"": true }"

                    Using response As HttpWebResponse = ServiceLibrary.MailChimp.MakeHTTPRequestPost(encodedCredentials, MCUri.AbsoluteUri, "POST", "application/json", bodyPost)
                        Dim responseReader As New StreamReader(response.GetResponseStream())
                        responseReader.Close()
                        response.Close()

                        If (response.StatusCode = 200 AndAlso response.StatusDescription.ToLower() = "ok") Then
                            result = True
                        End If
                    End Using
                Catch ex As Exception
                    Log.Write(ex.Message)
                    Log.Write(MCUri.AbsoluteUri)
                    Log.Write("body post: " + bodyPost)
                    result = False
                End Try

                Return result
            End Function

            Private Function BatchUnsubscribe(ByVal postString As String) As Boolean
                Dim MCUri As New Uri("https://" & datacenter & ".api.mailchimp.com/" & apiVersion & "/?method=listBatchUnsubscribe")
                Dim postData As String = postString

                Return PostQueryString(Nothing, MCUri, postData) = HttpStatusCode.OK
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
        End Class

    End Namespace
End Namespace