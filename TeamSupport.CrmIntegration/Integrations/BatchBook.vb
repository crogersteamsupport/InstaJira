Imports System.Xml
Imports System.Net
Imports System.IO
Imports TeamSupport.Data

Namespace TeamSupport
    Namespace CrmIntegration

        Public Class BatchBook
            Inherits Integration

            Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor)
                MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, IntegrationType.Batchbook)
            End Sub

            Public Overrides Function PerformSync() As Boolean
                Dim Success As Boolean = True

                'Commented out per action #5 of Ticket 21795.
                'Success = SyncAccounts()

                If Success Then
                    'Success = SendTicketData(AddressOf CreateComment)
                End If

                Return Success
            End Function

            Private Function SyncAccounts() As Boolean
                Dim ParentOrgID As String = CRMLinkRow.OrganizationID
                Dim TagsToMatch As String = CRMLinkRow.TypeFieldMatch

                Dim CompaniesToSync As XmlDocument
                Dim CompanySyncData As List(Of CompanyData) = Nothing

                'first retrieve a list of company data from batchbook
                If TagsToMatch.Contains(",") Then
                    Dim tagsToMatchArray As String() = Array.ConvertAll(TagsToMatch.Split(","), Function(p As String) p.Trim())
                    If tagsToMatchArray.Contains(String.Empty) Then
                        Log.Write("Missing Account Type to Link To TeamSupport (TypeFieldMatch).")
                        SyncError = True
                    Else
                        'process multiple tags where present
                        For Each TagToMatch As String In tagsToMatchArray
                            If Processor.IsStopped Then
                                Return False
                            End If

                            CompaniesToSync = GetBatchBookXML("companies.xml?tag=" & TagToMatch)

                            If CompaniesToSync IsNot Nothing Then
                                If CompanySyncData Is Nothing Then
                                    CompanySyncData = New List(Of CompanyData)()
                                End If

                                Dim thisCompanyData As List(Of CompanyData) = ParseBBCompanyXML(CompaniesToSync)

                                'avoid duplicates by only adding companies that aren't already in the list
                                For Each newData As CompanyData In thisCompanyData
                                    Dim alreadyExists As Boolean = False
                                    For Each existingData As CompanyData In CompanySyncData
                                        If newData.Equals(existingData) Then
                                            alreadyExists = True
                                        End If
                                    Next

                                    If Not alreadyExists Then
                                        CompanySyncData.Add(newData)
                                    End If
                                Next
                            End If
                        Next
                    End If
                Else
                    If TagsToMatch = String.Empty Then
                        Log.Write("Missing Account Type to Link To TeamSupport (TypeFieldMatch).")
                        SyncError = True
                    Else
                        'only one tag, don't need to loop
                        CompaniesToSync = GetBatchBookXML("companies.xml?tag=" & TagsToMatch)

                        If CompaniesToSync IsNot Nothing Then
                            CompanySyncData = New List(Of CompanyData)()
                            CompanySyncData = ParseBBCompanyXML(CompaniesToSync)
                        End If
                    End If
                End If

                'then use company data to update teamsupport
                If CompanySyncData IsNot Nothing Then
                    Log.Write(String.Format("Processed {0} accounts.", CompanySyncData.Count))

                    For Each company As CompanyData In CompanySyncData
                        UpdateOrgInfo(company, ParentOrgID)
                    Next

                    Log.Write("Finished updating account information.")
                    Log.Write("Updating people information...")

                    For Each company As CompanyData In CompanySyncData
                        Dim PeopleToSync As XmlDocument = GetBatchBookXML("companies/" & company.AccountID & "/people.xml")
                        If PeopleToSync IsNot Nothing Then
                            Dim PeopleSyncData As List(Of EmployeeData) = ParseBBPeopleXML(PeopleToSync)

                            If PeopleSyncData IsNot Nothing Then
                                For Each person As EmployeeData In PeopleSyncData
                                    UpdateContactInfo(person, company.AccountID, ParentOrgID)
                                Next
                            End If
                        End If

                        Log.Write("Updated people information for " & company.AccountID)
                    Next

                    Log.Write("Finished updating people information")
                End If

                Return Not SyncError
            End Function

            Private Function ParseBBCompanyXML(ByRef CompaniesToSync As XmlDocument) As List(Of CompanyData)
                Dim CompanySyncData As List(Of CompanyData) = Nothing

                'parse the xml doc to get information about each customer
                Dim allcust As XElement = XElement.Load(New XmlNodeReader(CompaniesToSync))

                If allcust.Descendants("company").Count > 0 Then
                    CompanySyncData = New List(Of CompanyData)()
                End If

                For Each company As XElement In allcust.Descendants("company")
                    If CRMLinkRow.LastLink Is Nothing Or Date.ParseExact(company.Element("updated_at").Value, "ddd MMM dd HH:mm:ss 'UTC' yyyy", Nothing).AddMinutes(30) > CRMLinkRow.LastLink Then
                        Dim thisCustomer As New CompanyData()
                        Dim address As XElement = company.Element("locations").Element("location")

                        With thisCustomer
                            .AccountID = company.Element("id").Value
                            .AccountName = company.Element("name").Value

                            If address IsNot Nothing Then
                                .City = address.Element("city").Value
                                .Country = address.Element("country").Value
                                .State = address.Element("state").Value
                                .Street = address.Element("street_1").Value
                                .Zip = address.Element("postal_code").Value
                                .Phone = address.Element("phone").Value
                                If address.Element("fax") IsNot Nothing Then
                                    .Fax = address.Element("fax").Value
                                End If
                            End If
                        End With

                        CompanySyncData.Add(thisCustomer)

                    End If
                Next

                Return CompanySyncData
            End Function

            Private Function ParseBBPeopleXML(ByRef PeopleToSync As XmlDocument) As List(Of EmployeeData)
                Dim EmployeeSyncData As List(Of EmployeeData) = Nothing

                Dim allpeople As XElement = XElement.Load(New XmlNodeReader(PeopleToSync))

                If allpeople.Descendants("person").Count > 0 Then
                    EmployeeSyncData = New List(Of EmployeeData)()
                End If

                For Each person As XElement In allpeople.Descendants("person")
                    Dim thisPerson As New EmployeeData()
                    Dim address As XElement = person.Element("locations").Element("location")

                    With thisPerson
                        .FirstName = person.Element("first_name").Value
                        .LastName = person.Element("last_name").Value
                        .Title = person.Element("title").Value

                        If address IsNot Nothing Then
                            .Email = address.Element("email").Value
                            .Phone = address.Element("phone").Value
                            .Cell = address.Element("cell").Value
                            .Fax = address.Element("fax").Value
                        End If
                    End With

                    EmployeeSyncData.Add(thisPerson)
                Next

                Return EmployeeSyncData
            End Function

            Private Function GetBatchBookXML(ByVal PathAndQuery As String) As XmlDocument
                Dim BBUri As New Uri("https://" & CRMLinkRow.Username & ".batchbook.com/service/" & PathAndQuery)
                Dim returnXML As XmlDocument = Nothing

                If CRMLinkRow.Username <> "" Then
          returnXML = GetXML(BBUri, New NetworkCredential(CRMLinkRow.SecurityToken1, "X"))
                End If
                Return returnXML
            End Function

            'returns a boolean value to indicate whether or not comment was created successfully
            Private Function CreateComment(ByVal AccountID As String, ByVal thisTicket As Ticket) As Boolean

                Dim description As Action = Actions.GetTicketDescription(User, thisTicket.TicketID)
                Dim NoteBody As String = String.Format("A ticket has been created for this organization entitled ""{0}"".{3}{2}{3}Click here to access the ticket information: https://app.teamsupport.com/Ticket.aspx?ticketid={1}", _
                                                                             thisTicket.Name, thisTicket.TicketID.ToString(), HtmlUtility.StripHTML(description.Description), Environment.NewLine)

                Dim success As Boolean = False
                Dim statusCode As HttpStatusCode

                If CRMLinkRow.Username <> "" Then
                    Dim BBUri As New Uri("https://" & CRMLinkRow.Username & ".batchbook.com/service/companies/" & AccountID & "/comments.xml")
                    Dim postData As String = "<comment><comment><![CDATA[" & NoteBody & "]]></comment></comment>"

          statusCode = PostXML(New NetworkCredential(CRMLinkRow.SecurityToken1, "X"), BBUri, postData)
                End If

                success = statusCode = HttpStatusCode.Created

                Return success
            End Function

        End Class

    End Namespace
End Namespace