
' 3/31/10 - Freshbooks itegration class (copied from Highrise class to start)

Imports System.Net
Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.Http
Imports System.Text


Public Class FreshBooks
    Private Const Integration As IntegrationType = IntegrationType.FreshBooks

    Dim SyncError As Boolean = False 'tracks global errors so we can not update the sync date if there's a problem
    Dim UseSSL As Boolean = True

    Public Function GetXML(ByVal Token As String, ByVal CompanyName As String, ByVal Command As String, ByVal FormName As TextBox, ByVal ParentOrgID As String) As XmlDocument
        Dim request As HttpWebRequest
        Dim response As HttpWebResponse = Nothing
        Dim reader As StreamReader

        Dim ts As New TSCheckData


        Dim WebString As String
        WebString = "https://" + CompanyName + ".freshbooks.com/api/2.1/xml-in"


        'If CompanyName <> "" Then
        Try


            System.Threading.Thread.Sleep(100) '1 ms sleep to see if there are issues with the API and throttling

            ' Create the web request  
            request = DirectCast(WebRequest.Create(WebString), HttpWebRequest)
            ' Add authentication to request  
            request.Credentials = New NetworkCredential(Token, "x")
            request.Timeout = 7000 'was 5, changed to 7 11/18/09
            request.Method = "POST" 'not sure on this one?
            request.ContentLength = Command.Length
            request.ContentType = "application/x-www-form-urlencoded" 'I think this is right?


            request.KeepAlive = False

            request.UserAgent = "Muroc Client"

            'these two lines should send the command
            Dim mywriter As New StreamWriter(request.GetRequestStream)
            mywriter.Write(Command)

            ' Get response  
            response = DirectCast(request.GetResponse(), HttpWebResponse)

            If request.HaveResponse = True AndAlso Not (response Is Nothing) Then


                ' Get the response stream into a reader  
                reader = New StreamReader(response.GetResponseStream())

                'Load the stream into an xml doc
                Dim myxml As XmlDocument = New XmlDocument
                myxml.LoadXml(reader.ReadToEnd)

                reader.Close()

                Return myxml
            End If

            response.Close()


        Catch wex As WebException

            If UseSSL Then
                '' If UseSSL is true then it means this is probably the first time we've called this.  Set to false then
                ''  have the calling routine try again.
                UseSSL = False

            Else
                'Don't need to raise the rror flag the first time through.

                ts.PublicAddText("GetXML: " + wex.ToString, FormName)
                ts.WriteToCRMResults(ParentOrgID, "Error in GetXML: " + wex.ToString)

                SyncError = True

            End If

            Return Nothing
        End Try
        ' Else
        'Probably should throw exception here
        ' Return Nothing
        ' End If






    End Function

    Public Function GetDataFromFB(ByVal SecurityToken As String, ByVal CompanyName As String, ByVal Command As String, ByVal formname As TextBox, ByVal parentOrgid As String) As XmlDocument

        Dim urlPost As String
        Dim myxml As XmlDocument = New XmlDocument

        urlPost = "https://" + CompanyName + ".freshbooks.com/api/2.1/xml-in"

        'This will request a list of clients.  Appears that only primary contact is returned with this
        'Probably need to increase the page count to a large number so we can grab them all at one shot
        'Dim postData As String = "<request method=""client.list""></request>"
        Dim PostData As String = Command
        Dim encoding As New UTF8Encoding()

        Dim PostStream As Stream = Nothing

        Dim Reader As StreamReader

        Dim ts As New TSCheckData

        Dim Request As HttpWebRequest

        Dim ByteData = UTF8Encoding.UTF8.GetBytes(PostData)


        Request = WebRequest.Create(urlPost)

        Request.Method = "POST"

        Request.ContentType = "application/xml"

        Request.ContentLength = ByteData.Length

        Request.PreAuthenticate = True

        'just a token with no password
        Request.Credentials = New NetworkCredential(SecurityToken, "xx") ' cred.GetCredential(Request.RequestUri, "Basic") '




        'OK, let's write the data
        Try
            PostStream = Request.GetRequestStream
            PostStream.Write(ByteData, 0, ByteData.Length)
        Catch ex As Exception
            If Not PostStream Is Nothing Then PostStream.Close()

            'Removing sync error from creating notes
            'SyncError = True

            ts.PublicAddText("Error in GetDataFromFB", formname)

        End Try

        Try

            'let's get the response from the system

            Dim response As HttpWebResponse
            response = Request.GetResponse


            'Get the response data
            Reader = New StreamReader(response.GetResponseStream())

            'Reader should now have data...

            'Dim ResponseText As String
            'ResponseText = Reader.ReadToEnd



            'Load the stream into an xml doc

            myxml.LoadXml(Reader.ReadToEnd)

            Reader.Close()



            response.Close()




        Catch wex As WebException
            'removing Sync error from creating notes.
            'SyncError = True

            ts.PublicAddText("Error GetDataFromFB", formname)
            'ts.WriteToCRMResults(parentOrgid, "Error in GetDataFromFB: " + wex.ToString)
            Return Nothing
        End Try

        'Response.Close()
        PostStream.Close()

        Return myxml





    End Function

    Public Sub GetClientList(ByVal Company As String, ByVal Token As String, ByVal ParentID As String, ByVal formname As TextBox)
        Dim ClientListXML As XmlDocument
        Dim Teststuff As XmlDocument


        'This should populate ClientListXML with the list of client data
        ClientListXML = GetDataFromFB(Token, Company, "<request method=""client.list""></request>", formname, ParentID)

        Teststuff = ClientListXML


    End Sub

    Public Sub TestPostNote()
        Dim http As New HttpClient("https://murocsystems.highrisehq.com/")

        http.TransportSettings.Credentials = New NetworkCredential("3f4d842b0361f53ce3cb8c009b5a409a87edef8f", "xx")


        'Dim body As HttpContent
        'body = New HttpContent

        'Dim Resp As HttpResponseMessage = http.Post("companies/20502030/notes.xml", "<note><body>Hello world!</body><subject-id type=""integer"">4</subject-id><subject-type>Party</subject-type></note>")

        Dim form As New HttpUrlEncodedForm
        form.Add("body", "<note><body>Hello world!</body><subject-id type=""integer"">4</subject-id><subject-type>Party</subject-type></note>")
        http.Post("companies/20502030/notes.xml", form.CreateHttpContent())



        Dim req As WebRequest
        req = WebRequest.Create("xxx")

        req.Method = "POST"
        req.ContentType = "text/xml"

        Dim writer As StreamWriter = New StreamWriter(req.GetRequestStream)
        writer.WriteLine("xxxx")
        writer.Close()






    End Sub




    Public Function GetTagID(ByVal token As String, ByVal companyname As String, ByVal TagString As String, ByVal formname As TextBox, ByVal parentorgid As String) As String
        'We use Tags in Highrise to identify the companies we want synced from HR to TS.
        '  This tag shows up as a string in HR, and we need to figure out what the ID of that tag is.
        '  This routine takes a text tag name and returns the (integer) id.


        Dim MyXML As XmlDocument

        Dim TagID As String
        Dim TagName As String


        MyXML = GetXML(token, companyname, "tags.xml", formname, parentorgid)  'note that capitalization DOES matter


        If MyXML IsNot Nothing Then

            Dim XMLreader As XmlNodeReader = New XmlNodeReader(MyXML)

            Dim FoundTag As Boolean = False

            While XMLreader.Read And (Not FoundTag)
                If (XMLreader.NodeType = XmlNodeType.Element) Then
                    If XMLreader.Name = "id" Then
                        TagID = XMLreader.ReadString
                    ElseIf XMLreader.Name = "name" Then
                        TagName = XMLreader.ReadString

                        If TagName = TagString Then
                            FoundTag = True 'exit if we've found the tag
                        End If

                    End If
                End If


            End While


            If FoundTag Then
                Return TagID
            Else
                Return "-1"
            End If


        Else
            Return Nothing
        End If

    End Function

    Public Sub GetClients(ByVal token As String, ByVal CompanyName As String, ByVal AccountID As String, ByVal ParentOrgID As String, ByVal formname As TextBox)


        Dim MyXML As XmlDocument

        Dim TS As New TSCheckData


        Dim CountPeople As Integer = 0


        Dim FirstName() As String
        Dim LastName() As String
        Dim email() As String 'hmmm - Need to explore this
        Dim Title() As String
        Dim WorkPhone() As String
        Dim MobilePhone() As String
        Dim FaxPhone() As String

        Dim ContactEMail() As String

        Dim TestAddress As String

        Dim FoundEMail As Boolean = False

        Dim LastLocation As String = "nothing"



        MyXML = GetXML(token, CompanyName, "companies/" + AccountID + "/people.xml", formname, ParentOrgID)  'List of people in this company

        If MyXML IsNot Nothing Then

            Dim XMLreader As XmlNodeReader = New XmlNodeReader(MyXML)

            While XMLreader.Read
                If (XMLreader.NodeType = XmlNodeType.Element) Then
                    If XMLreader.Name = "person" Then
                        CountPeople = CountPeople + 1

                        FoundEMail = False 'reset found e-mail counter

                        'AccountID(CountAccounts) = XMLreader.ReadString
                    ElseIf XMLreader.Name = "first-name" Then
                        ReDim Preserve FirstName(CountPeople)
                        FirstName(CountPeople) = XMLreader.ReadString

                    ElseIf XMLreader.Name = "last-name" Then
                        ReDim Preserve LastName(CountPeople)
                        LastName(CountPeople) = XMLreader.ReadString
                    ElseIf XMLreader.Name = "title" Then
                        ReDim Preserve Title(CountPeople)
                        Title(CountPeople) = XMLreader.ReadString

                    ElseIf XMLreader.Name = "location" Then
                        LastLocation = XMLreader.ReadString 'this will store the last location field.  Used to identify the phone number

                    ElseIf XMLreader.Name = "number" Then
                        'we've found a number, hopefully a phone number.  Based on the last value of the Location field, we will now assign it to the correct phone type
                        If LastLocation = "Work" Then
                            ReDim Preserve WorkPhone(CountPeople)
                            WorkPhone(CountPeople) = XMLreader.ReadString
                        ElseIf LastLocation = "Mobile" Then
                            ReDim Preserve MobilePhone(CountPeople)
                            MobilePhone(CountPeople) = XMLreader.ReadString
                        ElseIf LastLocation = "Fax" Then
                            ReDim Preserve FaxPhone(CountPeople)
                            FaxPhone(CountPeople) = XMLreader.ReadString
                        End If


                    ElseIf XMLreader.Name = "address" Then
                        'do something
                        'Problem is that there are 2 address attributes, and both are at the same depth
                        'How about we search for an @??

                        'Found e-mail should make sure we only grab the first e-mail (hopefully work)

                        If Not FoundEMail Then
                            TestAddress = XMLreader.ReadString
                            If TestAddress.Contains("@") Then
                                ReDim Preserve email(CountPeople)
                                email(CountPeople) = TestAddress
                                FoundEMail = True


                            End If
                        End If

                    End If

                End If

            End While

            XMLreader.Close()

            Dim TempEMail, TempFirstName, TempLastName, TempWorkPhone, TempFaxPhone, TempMobilePhone, TempTitle As String


            For x As Integer = 1 To CountPeople

                'Since we are using dynamic arrays, it's possible the array values may not exist.  If this is the case, we will get an exception and set the temp value to nothing



                Try
                    TempFirstName = FirstName(x)
                Catch ex As Exception
                    TempFirstName = "None"
                End Try

                Try
                    TempLastName = LastName(x)
                Catch ex As Exception
                    TempLastName = "None"
                End Try


                Try
                    TempEMail = email(x)
                Catch ex As Exception
                    Try
                        TempEMail = LastName(x) + "." + FirstName(x) 'added 11/18/09
                    Catch ex2 As Exception
                        TempEMail = ""
                    End Try

                End Try

                Try
                    TempWorkPhone = WorkPhone(x)
                Catch ex As Exception
                    TempWorkPhone = "None"
                End Try


                Try
                    TempFaxPhone = FaxPhone(x)
                Catch ex As Exception
                    TempFaxPhone = "None"
                End Try


                Try
                    TempMobilePhone = MobilePhone(x)
                Catch ex As Exception
                    TempMobilePhone = "None"
                End Try


                Try
                    TempTitle = Title(x)
                Catch ex As Exception
                    TempTitle = "None"
                End Try

                TS.AddOrUpdateContactInformation(AccountID, TempEMail, TempFirstName, TempLastName, TempWorkPhone, TempFaxPhone, TempMobilePhone, TempTitle, 0, ParentOrgID)
                Application.DoEvents()
            Next
        End If


        Erase FirstName, LastName, email, Title, WorkPhone, MobilePhone, FaxPhone, ContactEMail


    End Sub

    Public Sub SendTicketData(ByVal SecurityToken As String, ByVal companyname As String, ByVal ParentOrgID As String, ByVal formname As TextBox)
        '8/8/09 - CHanged this so that we only make one call to our database then update tickets based on that one call.  

        Dim TS As New TSCheckData
        Dim tickets As DataTable

        'get tickets which were created after the last link date
        tickets = TSCheckData.GetLatestTickets(ParentOrgID, Integration)

        If tickets IsNot Nothing Then
            For Each thisTicket As DataRow In tickets.Rows

                'Add the new tickets to the company record
                Dim NoteBody As String = "A ticket has been created for this organization entitled """ + thisTicket(1).ToString + """.  Click here to access the ticket information: https://app.teamsupport.com/Ticket.aspx?ticketid=" + thisTicket(0).ToString

                TS.PublicAddText("Creating a note...", formname)
                'CreateNote(SecurityToken, companyname, thisTicket(3).ToString, NoteBody, formname, ParentOrgID)

                Application.DoEvents()
            Next
        End If

    End Sub


    Public Function PerformFBSync(ByVal token As String, ByVal CompanyName As String, ByVal ParentOrgID As String, ByVal TagToMatch As String, ByVal FormName As TextBox)

        Dim TS As New TSCheckData

        Dim MyXML As XmlDocument

        Dim CountAccounts As Integer = 0
        Dim AccountName() As String
        Dim AccountID() As String
        Dim LastUpdated() As Date

        SyncError = False
        UseSSL = True





        Dim City(20, 1), Country(20, 1), State(20, 1), Street(20, 1), Zip(20, 1) As String 'rightmost is the company
        'Note:  Redim can only adjust the right side, so we're making that the account # since it could be large.  The left side is only
        ' for multiple addresses, and it's unlikely we will have more than 10 addresses per company.

        'Interesting - Found one company with 15 addresses per company!  Changed left side from 10 to 20.  12/11/09

        Dim LastUpdate As DateTime





        Try


            'Dim TagID As String
            'TagID = GetTagID(token, CompanyName, TagToMatch, FormName, ParentOrgID) 'caps matter here too
            'If Not UseSSL Then
            ' UseSSL was true when we called this routine, so if it's false now we know we had an error
            'If we try to get tags the firsst time and get an error, the UseSSL flag will be set to false.
            '  Let's try to get the tag info again, but this time without SSL and see what happens.
            'TagID = GetTagID(token, CompanyName, TagToMatch, FormName, ParentOrgID) 'caps matter here too
            'End If

            'If TagID <> "-1" Then

            Dim AddressCount() As Integer
            'Dim Phone(3000) As Integer

            ReDim AccountName(0 To 1) 'just set it up

            Dim XMLReader As XmlNodeReader

            Dim XMLLoopCount As Integer = 0 'counts the number of records that we got in this loop
            Dim NeedToGetMore As Boolean = True

            TS.PublicAddText("Starting to get client.list", FormName)
            'MyXML = GetXML(token, CompanyName, "companies.xml?tag_id=" + TagID, FormName, ParentOrgID)  'note that capitalization DOES matter
            MyXML = GetDataFromFB(token, CompanyName, "<request method=""client.list""></request>", FormName, ParentOrgID)
            TS.PublicAddText("Finished getting client.list", FormName)


            If MyXML IsNot Nothing Then

                While NeedToGetMore


                    'repeat until not needtogetmore

                    XMLReader = New XmlNodeReader(MyXML)

                    XMLReader.MoveToContent()


                    TS.PublicAddText("Processing client information...", FormName)
                    While XMLReader.Read
                        If (XMLReader.NodeType = XmlNodeType.Element) Then
                            If (XMLReader.Name = "client_id") Then
                                CountAccounts = CountAccounts + 1
                                XMLLoopCount = XMLLoopCount + 1 'tracks the number we've gotten in this loop

                                ReDim Preserve AddressCount(0 To CountAccounts)
                                AddressCount(CountAccounts) = 0

                                ReDim Preserve AccountID(0 To CountAccounts)
                                AccountID(CountAccounts) = XMLReader.ReadString

                                'TS.PublicAddText("CountAccounts = " + CountAccounts.ToString, FormName)
                                'ReDim AccountName(0 To CountAccounts)



                            ElseIf XMLReader.Name = "organization" Then 'company name
                                ReDim Preserve AccountName(0 To CountAccounts)
                                AccountName(CountAccounts) = XMLReader.ReadString
                                'ElseIf (XMLReader.Name = "p_street1") And (XMLReader.Depth = 4) Then ' I think this is righ
                                '   ReDim Preserve AddressCount(0 To CountAccounts)
                                '  AddressCount(CountAccounts) = AddressCount(CountAccounts) + 1






                            ElseIf XMLReader.Name = "p_city" Then
                                ReDim Preserve City(20, CountAccounts)
                                City(AddressCount(CountAccounts), CountAccounts) = XMLReader.ReadString
                            ElseIf XMLReader.Name = "p_country" Then
                                ReDim Preserve Country(20, CountAccounts)
                                Country(AddressCount(CountAccounts), CountAccounts) = XMLReader.ReadString
                                'ElseIf XMLreader.Name = "location" Then 'hmm - There are multiple locations.
                                'Location(CountAccounts, AddressCount) = XMLreader.ReadString
                            ElseIf XMLReader.Name = "p_state" Then
                                ReDim Preserve State(20, CountAccounts)
                                State(AddressCount(CountAccounts), CountAccounts) = XMLReader.ReadString
                            ElseIf XMLReader.Name = "p_street1" Then
                                ReDim Preserve AddressCount(0 To CountAccounts)
                                AddressCount(CountAccounts) = AddressCount(CountAccounts) + 1


                                ReDim Preserve Street(20, CountAccounts)
                                Street(AddressCount(CountAccounts), CountAccounts) = XMLReader.ReadString
                            ElseIf XMLReader.Name = "p_code" Then
                                ReDim Preserve Zip(20, CountAccounts)
                                Zip(AddressCount(CountAccounts), CountAccounts) = XMLReader.ReadString
                                'ElseIf XMLReader.Name = "updated-at" Then
                                '   ReDim Preserve LastUpdated(0 To CountAccounts)
                                '  LastUpdated(CountAccounts) = Date.Parse(XMLReader.ReadString) 'gets date from XML  Should (?) remain in Zulu time

                                'TS.PublicAddText("Updated at = " + LastUpdated(CountAccounts).ToString + " for company: " + CountAccounts.ToString, FormName)
                                'TS.PublicAddText("Plus 5 = " + LastUpdated(CountAccounts).AddMinutes(5).ToString + " for company: " + CountAccounts.ToString, FormName)
                                'TS.PublicAddText("Plus 5(again) = " + LastUpdated(CountAccounts).AddMinutes(5).ToString + " for company: " + CountAccounts.ToString, FormName)

                                'ElseIf XMLreader.Name = "number" Then
                                '    If Phone(CountAccounts) = Nothing Then
                                '**right now we don't do anything with the phone numbers.
                                'Phone(CountAccounts) = XMLreader.ReadString

                                'End If

                            End If


                        End If


                        Application.DoEvents()

                    End While


                    XMLReader.Close()


                    'If XMLLoopCount = 500 Then

                    'get the next 500 records
                    'TS.PublicAddText("Getting next set of records..." + CountAccounts.ToString, FormName)
                    'MyXML = GetXML(token, CompanyName, "companies.xml?tag_id=" + TagID + "&n=" + CountAccounts.ToString, FormName, ParentOrgID)  'note that capitalization DOES matter

                    'If MyXML IsNot Nothing Then


                    'XMLReader = New XmlNodeReader(MyXML)

                    'XMLReader.MoveToContent()

                    'XMLLoopCount = 0
                    'NeedToGetMore = True
                    'End If
                    'Else
                    NeedToGetMore = False 'don't get any more!

                    'End If

                End While



                TS.PublicAddText("Processed " + CountAccounts.ToString + " accounts.", FormName)

                'TS.PublicAddText("TEST: " + AccountName(5).ToString + ", Lastupdated: " + LastUpdated(5).ToString, FormName)


                'Dim WorkAddressID As Integer

                'LastUpdate = TS.ConvertDateTimeToHR(TS.GetLastCRMUpdate(ParentOrgID)) 'returns last update time formatted correctly for HR
                LastUpdate = Date.Parse(TSCheckData.GetLastCRMUpdate(ParentOrgID, Integration)).ToLocalTime 'returns the last update, in local time


                TS.PublicAddText("Updating account information...", FormName)
                For accounts As Integer = 1 To CountAccounts

                    'Go through all accounts we just processed and add to the TS database

                    If AddressCount(accounts) > 0 Then

                        'Grab the first address as the one we use in TS
                        ' Note that FB really only has one address (we're using postal, not shipping)
                        'If LastUpdated(accounts).AddMinutes(30) > LastUpdate Then
                        'Only update if the data is new
                        TS.AddOrUpdateAccountInformation(AccountID(accounts), AccountName(accounts), Street(1, accounts), "", City(1, accounts), State(1, accounts), Zip(1, accounts), Country(1, accounts), "", ParentOrgID)
                        TS.PublicAddText("Updated w/ Address:  " + AccountName(accounts), FormName)
                        'Else
                        'TS.PublicAddText("No update", FormName)
                        'End If

                    Else
                        'TS.PublicAddText("Adding/Updating Account Information (n) (" + accounts.ToString + ")", FormName)
                        'If LastUpdated(accounts).AddMinutes(30) > LastUpdate Then
                        'Only update if the data is new
                        TS.AddOrUpdateAccountInformation(AccountID(accounts), AccountName(accounts), "", "", "", "", "", "", "", ParentOrgID)
                        TS.PublicAddText("Updated w/out Address:  " + AccountName(accounts), FormName)
                        'Else
                        'TS.PublicAddText("No update", FormName)
                    End If




                    Application.DoEvents()
                Next


                TS.PublicAddText("Finished updaing account information.", FormName)


                'TS.PublicAddText("Updating people information...", FormName)
                'For x As Integer = 1 To CountAccounts
                'Try


                'Now get the individuals from the company
                'TS.PublicAddText("Getting People, CountAccounts=" + x.ToString, FormName)
                'If LastUpdated(x).AddMinutes(30) > LastUpdate Then
                'Only update if the data is new
                'GetPeople(token, CompanyName, AccountID(x), ParentOrgID, FormName)
                'TS.PublicAddText("Updated people information for " + AccountID(x).ToString, FormName)
                'Else
                'TS.PublicAddText("Company: " + AccountName(x) + ", LastUpdatedHR: " + LastUpdated(x).AddMinutes(5).ToString + ", LastUpdate: " + LastUpdate.ToString, FormName)
                'End If



                'And let's put a note in the companies record if there have been any new tickets created since the last time we did a sync
                'TS.PublicAddText("Sending tickets, CountAccounts=" + x.ToString, FormName)
                'SendTicketData(token, CompanyName, AccountID(x), TS.GetOrgIDFromCRMLinkID(AccountID(x)), ParentOrgID, FormName)
                'Catch ex As Exception
                'SyncError = True
                'TS.PublicAddText("Error in Updating People loop:" + ex.Message, FormName)
                'End Try
                'Next
                'TS.PublicAddText("Finished updating people information", FormName)
            Else
                'XML is not empty
            End If '



            'See what tickets have been created - If any, add.
            'TS.PublicAddText("Sending ticket data...", FormName)
            'SendTicketData(token, CompanyName, ParentOrgID, FormName)



            'Clear out the dynamic arrays we've been using
            Erase AccountName, AccountID, LastUpdated, City, Country, State, Street, Zip


            If Not SyncError Then
                Return "Completed"
            Else
                Return "Error"
            End If


        Catch ex As Exception
            SyncError = True

            Return ex.Message
            TS.WriteToCRMResults(ParentOrgID, "Error in Perform FreshBooks Sync: " + ex.ToString)
        End Try



    End Function





End Class
