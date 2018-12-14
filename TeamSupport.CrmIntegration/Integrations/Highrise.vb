'changelog info at teamsupport.beanstalkapp.com/main

Imports System.Net
Imports System.IO
Imports System.Xml
Imports TeamSupport.Data
Imports Newtonsoft.Json

Namespace TeamSupport
    Namespace CrmIntegration
        Public Class Highrise
            Inherits Integration

            Dim UseSSL As Boolean = True

            Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor)
                MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, IntegrationType.Highrise)
            End Sub

            Public Overrides Function PerformSync() As Boolean
                Dim Success As Boolean = True

                Success = SyncAccounts()

                If Success Then
                    Success = SendTicketData(AddressOf CreateNote)
                End If

                Return Success
            End Function

            Private Function SyncAccounts() As Boolean
                Dim TagsToMatch As String = CRMLinkRow.TypeFieldMatch

                Dim MatchArray As String() = Array.ConvertAll(TagsToMatch.Split(","), Function(p As String) p.Trim())
                If MatchArray.Contains(String.Empty) Then
                    Log.Write("Missing Account Type to Link To TeamSupport (TypeFieldMatch).")
                    SyncError = True
                Else
                    Dim MyXML As XmlDocument
          Dim Key As String = CRMLinkRow.SecurityToken1
                    Dim CompanyName As String = CRMLinkRow.Username
                    Dim ParentOrgID As String = CRMLinkRow.OrganizationID

                    SyncError = False
                    UseSSL = True

                    'strip .highrisehq.com from username if present
                    CompanyName = CompanyName.Replace(".highrisehq.com", "")

                    Try
                        Dim TagIDs As New List(Of String)()

                        For Each TagToMatch As String In MatchArray
                            If Processor.IsStopped Then
                                Return False
                            End If

                            Dim TagID As String = GetTagID(Key, CompanyName, TagToMatch.Trim(), ParentOrgID)

                            If TagID IsNot Nothing Then
                                TagIDs.Add(TagID)
                            End If
                        Next

                        If TagIDs.Count > 0 Then

                            Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
                            crmLinkErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "in", "company")
                            Dim crmLinkError As CRMLinkError = Nothing

                            Dim crmLinkContactErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
                            crmLinkContactErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "in", "contact")

                            For Each TagID As String In TagIDs
                                If Processor.IsStopped Then
                                    Return False
                                End If

                                Dim CompanySyncData As New List(Of CompanyData)()
                                Dim NeedtoGetMore As Boolean = True
                                Dim XMLLoopCount As Integer = 0
                                Dim TotalLoopCount As Integer = 0

                                Log.Write("Starting GetXML")
                                MyXML = GetHighriseXML(Key, CompanyName, "companies.xml?tag_id=" + TagID)  'note that capitalization DOES matter
                                Log.Write("Finished GetXML")

                                If MyXML IsNot Nothing Then

                                    While NeedtoGetMore
                                        Dim allcust As XElement = XElement.Load(New XmlNodeReader(MyXML))

                                        For Each company As XElement In allcust.Descendants("company")
                                            If Processor.IsStopped Then
                                                Return False
                                            End If

                                            If CRMLinkRow.LastLink Is Nothing Or Date.Parse(company.Element("updated-at").Value).AddMinutes(30) > CRMLinkRow.LastLink Then
                                                Dim thisCustomer As New CompanyData()
                                                Dim address As XElement = company.Element("contact-data").Element("addresses").Element("address")
                                                Dim phone As XElement = Nothing
                                                Dim fax As XElement = Nothing
                                                If company.Element("contact-data").Element("phone-numbers") IsNot Nothing Then
                                                    For Each phoneNumber As XElement In company.Element("contact-data").Element("phone-numbers").Descendants("phone-number")
                                                        If phoneNumber.Element("location").Value = "Work" AndAlso phone Is Nothing Then
                                                            phone = phoneNumber
                                                        ElseIf phoneNumber.Element("location").Value = "Fax" AndAlso fax Is Nothing Then
                                                            fax = phoneNumber
                                                        End If
                                                        If phone IsNot Nothing AndAlso fax IsNot Nothing Then
                                                            Exit For
                                                        End If
                                                    Next
                                                End If

                                                With thisCustomer
                                                    .AccountID = company.Element("id").Value
                                                    .AccountName = company.Element("name").Value

                                                    If address IsNot Nothing Then
                                                        .City = address.Element("city").Value
                                                        .Country = address.Element("country").Value
                                                        .Street = address.Element("street").Value
                                                        .State = address.Element("state").Value
                                                        .Zip = address.Element("zip").Value
                                                    End If
                                                    If phone IsNot Nothing Then
                                                        .Phone = phone.Element("number").Value
                                                    End If
                                                    If fax IsNot Nothing Then
                                                        .Fax = fax.Element("number").Value
                                                    End If
                                                End With

                                                CompanySyncData.Add(thisCustomer)
                                            End If

                                            XMLLoopCount += 1
                                            TotalLoopCount += 1
                                        Next

                                        If XMLLoopCount = 500 Then
                                            Log.Write("Getting next set of records..." & TotalLoopCount.ToString())
                                            Try
                                                MyXML = GetHighriseXML(Key, CompanyName, "companies.xml?tag_id=" + TagID + "&n=" + TotalLoopCount.ToString())  'note that capitalization DOES matter
                                            Catch e As Exception
                                                Log.Write("Error getting next records: " & e.Message)

                                            End Try

                                            If MyXML IsNot Nothing Then
                                                XMLLoopCount = 0
                                            Else
                                                NeedtoGetMore = False
                                            End If
                                        Else
                                            NeedtoGetMore = False
                                        End If
                                    End While

                                    Log.Write("Processed " + CompanySyncData.Count.ToString() + " accounts.")
                                    Log.Write("Updating account information...")

                                    For Each company As CompanyData In CompanySyncData

                                        crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(company.AccountID, String.Empty)
                                        
                                        'Go through all accounts we just processed and add to the TS database
                                        Try
                                          UpdateOrgInfo(company, ParentOrgID)
                                          Log.Write("Updated w/ Address: " & company.AccountName)
                                          If crmLinkError IsNot Nothing then
                                            crmLinkError.Delete()
                                            crmLinkErrors.Save()
                                          End If

                                        Catch ex As Exception

                                          If crmLinkError Is Nothing then
                                            Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                                            crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                                            crmLinkError.OrganizationID   = CRMLinkRow.OrganizationID
                                            crmLinkError.CRMType          = CRMLinkRow.CRMType
                                            crmLinkError.Orientation      = "in"
                                            crmLinkError.ObjectType       = "company"
                                            crmLinkError.ObjectID         = company.AccountID
                                            crmLinkError.ObjectData       = JsonConvert.SerializeObject(company)
                                            crmLinkError.Exception        = ex.ToString() + ex.StackTrace
                                            crmLinkError.OperationType    = "unknown"
                                            newCrmLinkError.Save()
                                          Else
                                            crmLinkError.ObjectData       = JsonConvert.SerializeObject(company)
                                            crmLinkError.Exception        = ex.ToString() + ex.StackTrace                                               
                                          End If                                              

                                        End Try
                                    Next

                                    Log.Write("Finished updating account information.")
                                    Log.Write("Updating people information...")

                                    For Each company As CompanyData In CompanySyncData
                                        Try
                                            GetPeople(Key, CompanyName, company.AccountID, ParentOrgID, crmLinkContactErrors)
                                            Log.Write("Updated people information for " & company.AccountName)
                                        Catch ex As Exception
                                            Log.Write("Error in Updating People loop:" + ex.Message)

                                        End Try
                                    Next

                                    Log.Write("Finished updating people information")
                                End If

                            Next
                            crmLinkErrors.Save()
                            crmLinkContactErrors.Save()
                        End If

                    Catch ex As Exception
                        SyncError = True

                        ErrorCode = IntegrationError.Unknown
                        Log.Write("Error in Perform Highrise Sync: " + ex.Message)
                    End Try
                End If

                Return Not SyncError

            End Function

            'TODO: this should be rewritten to take advantage of the GetXML function
            Private Function GetHighriseXML(ByVal Token As String, ByVal CompanyName As String, ByVal URL As String) As XmlDocument
                Dim request As HttpWebRequest
                Dim response As HttpWebResponse = Nothing
                Dim reader As StreamReader

                Dim WebString As String

                If UseSSL Then
                    WebString = "https://" + CompanyName + ".highrisehq.com/" + URL
                Else
                    'No s.
                    WebString = "http://" + CompanyName + ".highrisehq.com/" + URL
                End If
                Log.Write("Request: " + WebString)

                If CompanyName <> "" Then
                    Try
                        System.Threading.Thread.Sleep(100) '1 ms sleep to see if there are issues with the API and throttling

                        ' Create the web request  
                        request = DirectCast(WebRequest.Create(WebString), HttpWebRequest)
                        ' Add authentication to request  
                        request.Credentials = New NetworkCredential(Token, "x")
                        request.Timeout = 40000 'was 5, changed to 7 11/18/09, changed to 40 on 4/16/2012
                        request.Method = "GET" 'not sure on this one?
                        request.KeepAlive = False
                        request.UserAgent = Client

                        ' Get response  
                        response = DirectCast(request.GetResponse(), HttpWebResponse)
                        Log.Write("Request have response: " + request.HaveResponse.ToString)
                        If request.HaveResponse Then
                          Log.Write("Response Content Length: " + response.ContentLength.ToString)
                        End If

                        If request.HaveResponse AndAlso Not (response Is Nothing) Then
                            ' Get the response stream into a reader  
                            reader = New StreamReader(response.GetResponseStream())

                            'Load the stream into an xml doc
                            Dim myxml As XmlDocument = New XmlDocument
                            myxml.XmlResolver = Nothing

                            myxml.LoadXml(reader.ReadToEnd)

                            reader.Close()

                            Return myxml
                        End If

                        response.Close()

                    Catch ex As Exception
                        Log.Write("Requesting: " + WebString + ". The following error in GetHighriseXML occurred: " + ex.ToString)
                        'If UseSSL Then
                        '    'If UseSSL is true then it means this is probably the first time we've called this.  Set to false then try again
                        '    UseSSL = False

                        'Else
                        '    'Don't need to raise the rror flag the first time through.
                            ErrorCode = IntegrationError.Unknown
                        '    Log.Write("Error in GetHighriseXML: " + ex.ToString)
                            SyncError = True

                        'End If

                        Return Nothing
                    End Try
                End If

                Return Nothing
            End Function

            'TODO: rewrite to use PostXML function
            Private Function CreateNote(ByVal AccountID As String, ByVal thisTicket As Ticket) As Boolean
                Dim authorName As String = Nothing
                Using findAuthor As New Users(User)
                    findAuthor.LoadByUserID(thisTicket.CreatorID)

                    If findAuthor.Count > 0 Then
                        Dim author As User
                        author = findAuthor(0)

                        If author IsNot Nothing Then
                            authorName = author.FirstLastName
                        End If
                    End If
                End Using

                Dim description As Action = Actions.GetTicketDescription(User, thisTicket.TicketID)
                Dim descriptionString = String.Empty
                If description IsNot Nothing Then
                  descriptionString = HtmlUtility.StripHTML(description.Description)
                End If
                Dim NoteBody As String = String.Format("A ticket has been created for this organization entitled ""{0}"".{3}{2}{3}Click here to access the ticket information: https://app.teamsupport.com/Ticket.aspx?ticketid={1}{3}{4}", _
                                                                    thisTicket.Name, thisTicket.TicketID, descriptionString, Environment.NewLine, IIf(authorName IsNot Nothing, "Created by " & authorName, ""))

                Try
                    Dim urlPost As String

                    If UseSSL Then
                        urlPost = "https://" + CRMLinkRow.Username + ".highrisehq.com/companies/" + AccountID + "/notes.xml"
                    Else
                        urlPost = "http://" + CRMLinkRow.Username + ".highrisehq.com/companies/" + AccountID + "/notes.xml"
                    End If

                    Dim postData As String = "<note><body><![CDATA[" + NoteBody + "]]></body></note>"

                    Dim PostStream As Stream = Nothing


                    Dim Reader As StreamReader
                    Dim Request As HttpWebRequest



                    Dim ByteData = Text.UTF8Encoding.UTF8.GetBytes(postData)

                    Request = WebRequest.Create(urlPost)
                    Request.Method = "POST"
                    Request.ContentType = "application/xml"
                    Request.ContentLength = ByteData.Length
                    Request.PreAuthenticate = True

          Request.Credentials = New NetworkCredential(CRMLinkRow.SecurityToken1, "xx")


                    'OK, let's write the data
                    Try
                        PostStream = Request.GetRequestStream
                        PostStream.Write(ByteData, 0, ByteData.Length)
                    Catch ex As Exception
                        If Not PostStream Is Nothing Then PostStream.Close()

                        Log.Write("Create Note: " + ex.Message.ToString)
                    End Try

                    Try

                        Log.Write("Create Note URI: " + urlPost)
                        Log.Write("Create Note Post Data: " + postData)

                        Dim response As HttpWebResponse
                        response = Request.GetResponse

                        Reader = New StreamReader(response.GetResponseStream())


                        response.Close()

                    Catch wex As WebException
                        Log.Write("Error in create Note 2: " + wex.Message.ToString)
                        ErrorCode = IntegrationError.Unknown

                    End Try

                    PostStream.Close()

                Catch ex As Exception
                    Log.Write("Error in Create Note (Main routine).  Error = " + ex.Message)
                    Return False
                End Try

                Return True

            End Function

            ''' <summary>
            ''' We use Tags in Highrise to identify the companies we want synced from HR to TS.
            ''' This tag shows up as a string in HR, and we need to figure out what the ID of that tag is.
            ''' This routine takes a text tag name and returns the (integer) id.
            ''' </summary>
            ''' <param name="token"></param>
            ''' <param name="companyname"></param>
            ''' <param name="TagString"></param>
            ''' <param name="parentorgid"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Private Function GetTagID(ByVal token As String, ByVal companyname As String, ByVal TagString As String, ByVal parentorgid As String) As String
                Dim MyXML As XmlDocument
                Dim TagID As String = Nothing
                Dim TagName As String

                MyXML = GetHighriseXML(token, companyname, "tags.xml")  'note that capitalization DOES matter

                If MyXML IsNot Nothing Then
                    Dim XMLreader As XmlNodeReader = New XmlNodeReader(MyXML)
                    Dim FoundTag As Boolean = False

                    While XMLreader.Read And (Not FoundTag)
                        If (XMLreader.NodeType = XmlNodeType.Element) Then
                            If XMLreader.Name = "id" Then
                                TagID = XMLreader.ReadString
                            ElseIf XMLreader.Name = "name" Then
                                TagName = XMLreader.ReadString

                                If TagName.ToLower() = TagString.ToLower() Then
                                    FoundTag = True 'exit if we've found the tag
                                End If
                            End If
                        End If

                    End While

                    If FoundTag Then
                        Return TagID
                    End If
                End If

                Return Nothing
            End Function

            Private Sub GetPeople(ByVal token As String, ByVal CompanyName As String, ByVal AccountID As String, ByVal ParentOrgID As String, ByRef crmLinkContactErrors As CRMLinkErrors)
                Dim MyXML As XmlDocument

                Dim PeopleSyncData As List(Of EmployeeData) = Nothing

                MyXML = GetHighriseXML(token, CompanyName, "companies/" + AccountID + "/people.xml")  'List of people in this company

                If MyXML IsNot Nothing Then
                    Dim allpeople As XElement = XElement.Load(New XmlNodeReader(MyXML))

                    If allpeople.Descendants("person").Count > 0 Then
                        PeopleSyncData = New List(Of EmployeeData)()
                    End If

                    Dim crmLinkError As CRMLinkError = Nothing

                    For Each person As XElement In allpeople.Descendants("person")
                        Dim thisPerson As New EmployeeData()
                        Dim phones As XElement = person.Element("contact-data").Element("phone-numbers")

                        With thisPerson
                            .HighriseID = person.Element("id").Value
                            .FirstName = person.Element("first-name").Value
                            .LastName = person.Element("last-name").Value
                            .Title = person.Element("title").Value

                            If phones.HasElements Then
                                For Each phone As XElement In phones.Descendants("phone-number")
                                    Select Case phone.Element("location").Value
                                        Case "Work"
                                            .Phone = phone.Element("number").Value
                                        Case "Mobile"
                                            .Cell = phone.Element("number").Value
                                        Case "Fax"
                                            .Fax = phone.Element("number").Value
                                    End Select
                                Next
                            End If

                            If person.Element("contact-data").Element("email-addresses").HasElements Then
                                .Email = person.Element("contact-data").Element("email-addresses").Element("email-address").Element("address").Value
                            End If
                        End With

                        PeopleSyncData.Add(thisPerson)
                    Next

                    If PeopleSyncData IsNot Nothing AndAlso PeopleSyncData.Count > 0 Then
                        Log.Write(String.Format("{0} people found. Updating...", PeopleSyncData.Count.ToString()))

                        For Each person As EmployeeData In PeopleSyncData
                            crmLinkError = crmLinkContactErrors.FindByObjectIDAndFieldName(person.HighriseID, string.Empty)
                            Try
                              UpdateContactInfo(person, AccountID, ParentOrgID)                             
                              If crmLinkError IsNot Nothing then
                                crmLinkError.Delete()
                                crmLinkContactErrors.Save()
                              End If

                            Catch ex As Exception

                              If crmLinkError Is Nothing then
                                Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                                crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                                crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                                crmLinkError.CRMType        = CRMLinkRow.CRMType
                                crmLinkError.Orientation    = "in"
                                crmLinkError.ObjectType     = "contact"
                                crmLinkError.ObjectID       = person.HighriseID
                                crmLinkError.ObjectData     = JsonConvert.SerializeObject(person)
                                crmLinkError.Exception      = ex.ToString() + ex.StackTrace
                                crmLinkError.OperationType  = "unknown"
                                newCrmLinkError.Save()
                              Else
                                crmLinkError.ObjectData     = JsonConvert.SerializeObject(person)
                                crmLinkError.Exception      = ex.ToString() + ex.StackTrace                                               
                              End If                                              

                            End Try
                        Next
                    Else
                        Log.Write("No people to update.")
                    End If


                End If
            End Sub

        End Class
    End Namespace
End Namespace