Imports OracleSalesParty
Imports System.ServiceModel
Imports System.IdentityModel
Imports System.Text
Imports TeamSupport.Data

Namespace TeamSupport
  Namespace CrmIntegration
    Public Class Oracle
      Inherits Integration

      Private Const MAXBATCHSIZE = 2
      Private _lastLinkDateTime As String
      Private _salesPartyServiceClient As SalesPartyServiceClient
      Private _connector As String

      Public Overrides Function PerformSync() As Boolean
        Dim Success As Boolean = True

        Success = SyncAccounts()

        If Success Then
            'Success = SendTicketsAsAccountComments()
        End If

        Return Success
      End Function

      Private Function SyncAccounts() As Boolean
        UpdateAccounts(GetRecordsToUpdate("OrganizationParty"))
        UpdateContacts(GetRecordsToUpdate("PersonParty"))
        Return Not SyncError
      End Function

      Private Function GetRecordsToUpdate(ByRef target As String) As List(Of SalesParty)
        Dim done As Boolean = False
        Dim aSalesParty As SalesParty() = Nothing
        Dim findCriteria As FindCriteria = GetFindCriteria(target)
        Dim findControl As FindControl = GetFindControl()
        Dim result As List(Of SalesParty) = New List(Of SalesParty)()
        Dim numberOfRecords As Integer = 0

        While Not done
            aSalesParty = _salesPartyServiceClient.findSalesParty(findCriteria, findControl)
            If aSalesParty IsNot Nothing Then
              For i As Integer = 0 To aSalesParty.Length - 1
                If target = "OrganizationParty" OrElse (Not String.IsNullOrEmpty(aSalesParty(i).PersonParty(0).EmailAddress) AndAlso aSalesParty(i).PersonParty(0).Relationship IsNot Nothing) Then
                  result.Add(aSalesParty(i))
                End If
                numberOfRecords += 1
              Next
                    
              If aSalesParty.Length = MAXBATCHSIZE Then
                findCriteria.fetchStart = numberOfRecords
              Else
                done = True            
              End If
            Else
              done = True
            End If
        End While
        Log.Write("Got " + result.Count.ToString() + " " + target + "s to update.")
        Return result
      End Function

      Private Function GetFindCriteria(ByRef target As String) As FindCriteria
        Dim result As FindCriteria = New FindCriteria()
        result.fetchSize = MAXBATCHSIZE
        result.findAttribute = New String() {target}
        result.filter = GetFilter("type", Replace(target, "Party", String.Empty))
        result.childFindCriteria = GetChildFindCriteria(target)
        Return result
      End Function

      Private Function GetFilter(ByRef clauseName As String, ByRef value As String) As ViewCriteria
        Dim row As ViewCriteriaRow = New ViewCriteriaRow()
        row.upperCaseCompare = True
        row.conjunction = Conjunction.And

        Dim clause As ViewCriteriaItem = New ViewCriteriaItem()
        clause.conjunction = Conjunction.And
        clause.upperCaseCompare = True
        Select Case clauseName
          Case "type"
            clause.attribute = "PartyType"
            clause.operator = "="
          Case "lastUpdateDate"
            clause.attribute = "LastUpdateDate"
            clause.operator = "ONORAFTER"
        End Select
        clause.Items = New string() {value} 
        row.item = New ViewCriteriaItem() {clause}       
        
        Dim result As ViewCriteria = New ViewCriteria()
        result.group = New ViewCriteriaRow() {row}

        Return result
      End Function

      Private Function GetChildFindCriteria(ByRef target As String) As ChildFindCriteria()
        Dim result As ChildFindCriteria = New ChildFindCriteria()
        result.childAttrName = target
        result.fetchStart = 0
        result.fetchSize = -1
        result.excludeAttribute = False
        result.filter = GetFilter("lastUpdateDate", _lastLinkDateTime)
        Return New ChildFindCriteria() {result}
      End Function

      Private Function GetFindControl() As FindControl
        Dim result As FindControl = New FindControl()
        result.retrieveAllTranslations = True
        Return result
      End Function

      Private Sub UpdateAccounts(ByRef accountsToUpdate As List(Of SalesParty))
        Dim thisCompany As CompanyData = Nothing
        For Each accountToUpdate As SalesParty In accountsToUpdate
          thisCompany = New CompanyData()
          With thisCompany
            .AccountID    = accountToUpdate.OrganizationParty(0).PartyId.ToString
            .AccountName  = accountToUpdate.OrganizationParty(0).PartyName
            .Street       = GetAddress(accountToUpdate.OrganizationParty(0))
            .City         = accountToUpdate.OrganizationParty(0).City
            .State        = accountToUpdate.OrganizationParty(0).State
            .Zip          = accountToUpdate.OrganizationParty(0).PostalCode
            .Country      = accountToUpdate.OrganizationParty(0).Country
            .Phone        = GetPhone(accountToUpdate.OrganizationParty(0), "Phone")
            .Fax          = GetPhone(accountToUpdate.OrganizationParty(0), "Fax")
          End With

          UpdateOrgInfo(thisCompany, CRMLinkRow.OrganizationID)
        Next
      End Sub

      Private Function GetAddress(ByRef account As OrganizationParty) As String
        Dim result As New StringBuilder()
        If account.Address1 IsNot Nothing Then
          result.Append(account.Address1)
        End If
        If account.Address2 IsNot Nothing Then
          result.Append(" " + account.Address2)
        End If
        If account.Address3 IsNot Nothing Then
          result.Append(" " + account.Address3)
        End If
        If account.Address4 IsNot Nothing Then
          result.Append(" " + account.Address4)
        End If
        Return result.ToString()
      End Function

      Private Function GetPhone(ByRef account As OrganizationParty, ByRef type As String) As String
        Dim result As New StringBuilder()
        If account.Phone IsNot Nothing
          For i As Integer = 0 To account.Phone.Length - 1
            If (type = "Phone" AndAlso account.Phone(i).PhoneLineType = "GEN") OrElse (type = "Fax" AndAlso account.Phone(0).PhoneLineType = "FAX") OrElse (type = "Mobile" AndAlso account.Phone(0).PhoneLineType = "MOBILE")Then
                result.Append(account.Phone(i).FormattedPhoneNumber)
                Exit For
            End If
          Next
        End If
        Return result.ToString()
      End Function

      Private Sub UpdateContacts(ByRef contactsToUpdate As List(Of SalesParty))
        Dim thisPerson As EmployeeData = Nothing
        For Each contactToUpdate As SalesParty In contactsToUpdate
          thisPerson = New EmployeeData()
          With thisPerson
            .Email      = contactToUpdate.PersonParty(0).EmailAddress
            .FirstName  = contactToUpdate.PersonParty(0).PersonFirstName
            .LastName   = contactToUpdate.PersonParty(0).PersonLastName
            .Phone      = GetPhone(contactToUpdate.PersonParty(0), "Phone")
            .Cell       = GetPhone(contactToUpdate.PersonParty(0), "Mobile")
            .Title      = contactToUpdate.PersonParty(0).Salutation
            .Fax        = GetPhone(contactToUpdate.PersonParty(0), "Fax")
          End With

          UpdateContactInfo(thisPerson, contactToUpdate.PersonParty(0).Relationship(0).ObjectId.ToString(), CRMLinkRow.OrganizationID)
        Next
      End Sub

      Private Function GetPhone(ByRef person As Person, ByRef type As String) As String
        Dim result As New StringBuilder()
        If person.Phone IsNot Nothing
          For i As Integer = 0 To person.Phone.Length - 1
            If (type = "Phone" AndAlso person.Phone(i).PhoneLineType = "GEN") OrElse (type = "Fax" AndAlso person.Phone(0).PhoneLineType = "FAX") Then
                result.Append(person.Phone(i).FormattedPhoneNumber)
                Exit For
            End If
          Next
        End If
        Return result.ToString()
      End Function

      Private Function SendTicketsAsAccountComments() As Boolean
        Dim Success As Boolean = True

        If CRMLinkRow.SendBackTicketData Then
          Success = SendTicketData(AddressOf CreateNote)
        End If

        Return Success
      End Function

      Private Function CreateNote(ByVal accountid As String, ByVal thisTicket As Ticket) As Boolean
        Dim Success As Boolean = True

        Dim Title = "Support Issue: " & thisTicket.Name
        Dim description As Action = Actions.GetTicketDescription(User, thisTicket.TicketID)
        Dim NoteBody As String = String.Format("A new support ticket has been created for this account entitled ""{0}"".{3}{2}{3}Click here to access the ticket information: https://app.teamsupport.com/Ticket.aspx?ticketid={1}", _
                                            thisTicket.Name, thisTicket.TicketID.ToString(), HtmlUtility.StripHTML(description.Description), Environment.NewLine)

        Try
          Dim note As OracleActivityAppointment.Note = New OracleActivityAppointment.Note()
          note.NoteTxt = Encoding.ASCII.GetBytes(Title + Environment.NewLine() + NoteBody)
          note.SourceObjectCode = "SALES_PARTY"
          note.SourceObjectId = accountid
          note.NoteTypeCode = "GENERAL"

          Dim binding As BasicHttpBinding = GetBinding()
          binding.MessageEncoding = WSMessageEncoding.Mtom
          Dim noteService As New OracleActivityAppointment.ActivityAppointmentServiceClient(binding, New EndpointAddress(CRMLinkRow.HostName + _connector + "appCmmnCompActivities/ActivityAppointmentService"))
          noteService.ClientCredentials.UserName.UserName = CRMLinkRow.Username
          noteService.ClientCredentials.UserName.Password = CRMLinkRow.Password
          Dim createdNote As OracleActivityAppointment.Note = noteService.createNotes(note)
        Catch ex As Exception
          'At this moment the response to the createNotes method is not being able to be parsed.
          'This is probably an error in the service WSDL that need to be addressed by Oracle.
          'All we can do is to submit the request and assume it was successfully created.
        End Try

        Return Success
      End Function

      Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor)
        MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, IntegrationType.SalesForce)
        _lastLinkDateTime = GetLastLinkDateTime()
        _connector = String.Empty
        If Right(CRMLinkRow.HostName, 1) <> "/" Then
          _connector = "/"
        End If
        _salesPartyServiceClient = GetSalesPartyServiceClient()
      End Sub

      Private Function GetLastLinkDateTime() As String
        Dim result As New DateTime(1900, 1, 1)

        If CRMLinkRow.LastLink IsNot Nothing Then
            result = CRMLinkRow.LastLink.Value.AddHours(-1) 'push last update time back 1 hour to make sure we catch every change
        End If

        return result.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'") 'format for SF query for time is 2011-01-26T16:57:00.000Z  ('yyyy'-'MM'-'dd'T'HH': 'mm': 'ss.fffffff'Z' )
      End Function

      Private Function GetSalesPartyServiceClient() As SalesPartyServiceClient
        Dim address As EndpointAddress = New EndpointAddress(CRMLinkRow.HostName + _connector + "crmCommonSalesParties/SalesPartyService")
        Dim binding As BasicHttpBinding = GetBinding()
        Dim result As SalesPartyServiceClient = New SalesPartyServiceClient(binding, address)
        result.ClientCredentials.UserName.UserName = CRMLinkRow.Username
        result.ClientCredentials.UserName.Password = CRMLinkRow.Password
        Return result
      End Function

      Private Function GetBinding() As BasicHttpBinding
        Dim result As New BasicHttpBinding()

        result.Security.Mode = SecurityMode.Transport
        result.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic

        result.TransferMode = TransferMode.Buffered

        result.MaxReceivedMessageSize              = 2147483647
        result.MaxBufferSize                       = 2147483647
        result.ReaderQuotas.MaxDepth               = 2147483647
        result.ReaderQuotas.MaxStringContentLength = 2147483647
        result.ReaderQuotas.MaxArrayLength         = 2147483647
        result.ReaderQuotas.MaxBytesPerRead        = 2147483647
        result.ReaderQuotas.MaxNameTableCharCount  = 2147483647
        Return result
      End Function

    End Class
  End Namespace
End Namespace