Imports TeamSupport.Data
Imports System.Net
Imports System.Web
Imports System.Xml
Imports System.Text
Imports System.IO
Imports System.Data.SqlClient

Namespace TeamSupport
    Namespace CrmIntegration

        Public MustInherit Class Zoho
            Inherits Integration

            Protected APITicket As String

            Protected Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor, ByVal thisType As IntegrationType)
                MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, thisType)
            End Sub

            Public MustOverride Overrides Function PerformSync() As Boolean

            ''' <summary>
            ''' Logs out after a sync is performed.
            ''' </summary>
            ''' <remarks></remarks>
            Protected Sub Logout()
                Log.Write("Logging out revision 1077.")
                Dim ZohoUri As Uri = Nothing
                ZohoUri = New Uri("https://accounts.zoho.com/logout?FROM_AGENT=true&authtoken=" & CRMLinkRow.SecurityToken1 & "&scope=crmapi")
        
                Try
                  GetHTTPData(Nothing, ZohoUri)
                Catch ex As Exception
                  Log.Write("The following exception occurred attempting to logout using URI: " & ZohoUri.ToString() & ". Ex: " & ex.Message)
                End Try
            End Sub
        End Class


        Public Class ZohoCRM
            Inherits Zoho

			Private Const MaxBatchSize = 200

			Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor)
                MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, IntegrationType.ZohoCRM)
            End Sub

            Public Overrides Function PerformSync() As Boolean
                Dim Success As Boolean = False
				Dim zohoCRMv2 As ServiceLibrary.ZohoCRM.ZohoCRMv2 = New ServiceLibrary.ZohoCRM.ZohoCRMv2(CRMLinkRow)
				Success = zohoCRMv2.PerformSync()

                Return Success
            End Function

            Private Function GetZohoCRMXML(ByVal PathAndQuery As String) As XmlDocument
                Dim ZohoUri As Uri = Nothing
                ZohoUri = New Uri("https://crm.zoho.com/crm/private/xml/" & PathAndQuery & "&authtoken=" & CRMLinkRow.SecurityToken1 & "&scope=crmapi")

                Return GetXML(ZohoUri)
            End Function

            Private Function ParseZohoCompanyXML(ByVal CompaniesToSync As XmlDocument) As List(Of CompanyData)
                Dim CompanySyncData As List(Of CompanyData) = Nothing
                Dim tagsToMatch As String() = Array.ConvertAll(CRMLinkRow.TypeFieldMatch.ToLower().Split(","), Function(p As String) p.Trim())

                If tagsToMatch.Contains(String.Empty) Then
                    Log.Write("Missing Account Type to Link To TeamSupport (TypeFieldMatch).")
                    SyncError = True
                Else
                    Dim needToGetMore As Boolean = True
                    Dim returnedRecords As Integer = 0
                    Dim totalRecords As Integer = 1

                    While needToGetMore
                        Dim allaccounts As XElement = XElement.Load(New XmlNodeReader(CompaniesToSync))

                        For Each company As XElement In allaccounts.Descendants("row")
                            If Processor.IsStopped Then
                                Return Nothing
                            End If

                            If CompanySyncData Is Nothing Then
                                CompanySyncData = New List(Of CompanyData)()
                            End If

                            Dim thisCustomer As New CompanyData()
                            Dim add As Boolean = False
                            Dim hasAccountType As Boolean = False

                            With thisCustomer
                                'Zoho's XML is really ugly so we have to parse it like this
                                For Each dataitem As XElement In company.Descendants("FL")
                                    Select Case dataitem.Attribute("val").Value
                                        Case "ACCOUNTID"
                                            .AccountID = dataitem.Value
                                        Case "Account Name"
                                            .AccountName = dataitem.Value
                                        Case "Shipping City"
                                            .City = dataitem.Value
                                        Case "Shipping Country"
                                            .Country = dataitem.Value
                                        Case "Shipping State"
                                            .State = dataitem.Value
                                        Case "Shipping Street"
                                            .Street = dataitem.Value
                                        Case "Shipping Code"
                                            .Zip = dataitem.Value
                                        Case "Phone"
                                            .Phone = dataitem.Value
                                        Case "Fax"
                                            .Fax = dataitem.Value
                                        Case "Account Type"
                                            add = tagsToMatch.Contains(dataitem.Value.ToLower())
                                            hasAccountType = True
                                    End Select
                                Next
                            End With

                            If tagsToMatch.Contains("none") AndAlso Not hasAccountType Then
                                add = True
                            End If

                            If add Then
                                CompanySyncData.Add(thisCustomer)
                            End If

                            returnedRecords += 1
                            totalRecords += 1
                        Next

                        If returnedRecords = MaxBatchSize Then
                            Log.Write("getting next set of records..." & totalRecords.ToString())

                            Try
                                CompaniesToSync = GetZohoCompanyXML(totalRecords, totalRecords + MaxBatchSize - 1)
                            Catch ex As Exception
                                Log.Write("Error getting next records..." & ex.Message)
                            End Try

                            If XElement.Load(New XmlNodeReader(CompaniesToSync)).Descendants("row").Count > 0 Then
                                returnedRecords = 0
                            Else
                                needToGetMore = False
                            End If
                        Else
                            needToGetMore = False
                        End If
                    End While
                End If

                Return CompanySyncData
            End Function

            Private Function ParseZohoPeopleXML(ByVal PeopleToSync As XmlDocument) As List(Of EmployeeData)
                Dim EmployeeSyncData As List(Of EmployeeData) = Nothing

                Dim allpeople As XElement = XElement.Load(New XmlNodeReader(PeopleToSync))

                If allpeople.Descendants("row").Count > 0 Then
                    EmployeeSyncData = New List(Of EmployeeData)()
                End If

                Dim needToGetMore As Boolean = True
                Dim returnedRecords As Integer = 0
                Dim totalRecords As Integer = 1

                While needToGetMore
                    For Each person As XElement In allpeople.Descendants("row")
                        If Processor.IsStopped Then
                            Return Nothing
                        End If

                        Dim thisPerson As New EmployeeData()

                        With thisPerson
                            'see above re: xml formatting/processing (it's done this way because this is how zoho formats it)
                            For Each dataitem As XElement In person.Descendants("FL")
                                Select Case dataitem.Attribute("val").Value
                                    Case "First Name"
                                        .FirstName = dataitem.Value
                                    Case "Last Name"
                                        .LastName = dataitem.Value
                                    Case "Title"
                                        .Title = dataitem.Value
                                    Case "Email"
                                        .Email = dataitem.Value
                                    Case "Phone"
                                        .Phone = dataitem.Value
                                    Case "Mobile"
                                        .Cell = dataitem.Value
                                    Case "Fax"
                                        .Fax = dataitem.Value
                                    Case "Phone Ext"
                                        .Extension = dataitem.Value
                                    Case "ACCOUNTID"
                                        .ZohoID = dataitem.Value
                                End Select
                            Next

                        End With

                        If Not String.IsNullOrEmpty(thisPerson.ZohoID) Then
                          EmployeeSyncData.Add(thisPerson)
                        End If
                        returnedRecords += 1
                        totalRecords += 1
                    Next

                    If returnedRecords = MaxBatchSize Then
                        Log.Write("getting next set of records..." & totalRecords.ToString())

                        Try
                            PeopleToSync = GetZohoPeopleXML(totalRecords, totalRecords + MaxBatchSize - 1)
                            allpeople = XElement.Load(New XmlNodeReader(PeopleToSync))
                        Catch ex As Exception
                            Log.Write("Error getting next records..." & ex.Message)
                        End Try

                        If allpeople.Descendants("row").Count > 0 Then
                            returnedRecords = 0
                        Else
                            needToGetMore = False
                        End If
                    Else
                        needToGetMore = False
                    End If
                End While

                Return EmployeeSyncData
            End Function

            Private Function ParseZohoProductsXML(ByVal ProductsToSync As XmlDocument, ByVal AccountID As String) As List(Of ProductData)
                Dim ProductSyncData As List(Of ProductData) = Nothing

                Dim allproducts As XElement = XElement.Load(New XmlNodeReader(ProductsToSync))

                If allproducts.Descendants("row").Count > 0 Then
                    ProductSyncData = New List(Of ProductData)()
                End If

                Dim needToGetMore As Boolean = True
                Dim returnedRecords As Integer = 0
                Dim totalRecords As Integer = 1

                While needToGetMore
                  For Each product As XElement In allproducts.Descendants("row")
                      If Processor.IsStopped Then
                          Return Nothing
                      End If

                      Dim thisProduct As New ProductData()

                      With thisProduct
                          'see above re: xml formatting/processing (it's done this way because this is how zoho formats it)
                          For Each dataitem As XElement In product.Descendants("FL")
                              Select Case dataitem.Attribute("val").Value
                                  Case "Product Name"
                                      .Name = dataitem.Value
                                  Case "Created Time"
                                      .CreatedTime = dataitem.Value
                              End Select
                          Next

                      End With

                      ProductSyncData.Add(thisProduct)
                      returnedRecords += 1
                      totalRecords += 1
                  Next

                  If returnedRecords = MaxBatchSize Then
                      Log.Write("getting next set of records..." & totalRecords.ToString())

                      Try
                          ProductsToSync = GetZohoProductsXML(AccountID, totalRecords, totalRecords + MaxBatchSize - 1)
                          allproducts = XElement.Load(New XmlNodeReader(ProductsToSync))
                      Catch ex As Exception
                          Log.Write("Error getting next records..." & ex.Message)
                      End Try

                      If allproducts.Descendants("row").Count > 0 Then
                          returnedRecords = 0
                      Else
                          needToGetMore = False
                      End If
                  Else
                      needToGetMore = False
                  End If

                End While

                Return ProductSyncData
            End Function

            Private Sub GetZohoCustomFields(ByVal objType As String, ByVal accountIDToUpdate As String)
              Dim customMappings As New CRMLinkFields(User)
              customMappings.LoadByObjectType(objType, CRMLinkRow.CRMLinkID)

              If (customMappings.Count > 0) Then
                Dim tagsToMatch As String() = Array.ConvertAll(CRMLinkRow.TypeFieldMatch.ToLower().Split(","), Function(p As String) p.Trim())
                Dim accountZohoType As String = String.Empty
                Dim accountZohoID As String = String.Empty

                Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
                
                Select Case objType
                  Case "Account"
                    crmLinkErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "in", "company")
                  Case "Contact"
                    crmLinkErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "in", "contact")
                End Select
                
                Dim crmLinkError As CRMLinkError = Nothing

                For Each recordToUpdate As XElement In GetCustomMappingValues(objType, customMappings, accountIDToUpdate)
                  accountZohoType = GetAccountZohoType(recordToUpdate, accountZohoID)
                  Dim companyToUpdate As New Organizations(User)
                  Select Case objType
                    Case "Account"
                  companyToUpdate.LoadByCRMLinkID(accountZohoID, CRMLinkRow.OrganizationID)
                  If companyToUpdate.Count > 0 Then
                        If tagsToMatch.Contains("none") OrElse tagsToMatch.Contains(accountZohoType) Then
                          For Each field As XElement In recordToUpdate.Descendants("FL")
                            Dim fieldMapping As CRMLinkField = customMappings.FindByCRMFieldName(field.Attribute("val").Value)
                            If fieldMapping IsNot Nothing Then
                              Try
                                If fieldMapping.CustomFieldID IsNot Nothing Then
                                  crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(accountZohoID, fieldMapping.CustomFieldID.ToString())
                                  UpdateCustomValue(fieldMapping.CustomFieldID, companyToUpdate(0).OrganizationID, field.Value)
                                ElseIf fieldMapping.TSFieldName IsNot Nothing Then
                                  crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(accountZohoID, fieldMapping.TSFieldName)
                                  companyToUpdate(0).Row(fieldMapping.TSFieldName) = field.Value
                                  companyToUpdate(0).BaseCollection.Save()
                                End If
                                If crmLinkError IsNot Nothing then
                                  crmLinkError.Delete()
                                  crmLinkErrors.Save()
                                End If

                              Catch ex As Exception
                                If crmLinkError Is Nothing then
                                  Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                                  crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                                  crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                                  crmLinkError.CRMType        = CRMLinkRow.CRMType
                                  crmLinkError.Orientation    = "in"
                                  crmLinkError.ObjectType     = "company"
                                  crmLinkError.ObjectID       = accountZohoID
                                  If fieldMapping.CustomFieldID IsNot Nothing Then
                                    crmLinkError.ObjectFieldName = fieldMapping.CustomFieldID.ToString()
                                  ElseIf fieldMapping.TSFieldName IsNot Nothing Then
                                    crmLinkError.ObjectFieldName = fieldMapping.TSFieldName
                                  End If
                                  crmLinkError.ObjectData     = field.Value
                                  crmLinkError.Exception      = ex.ToString() + ex.StackTrace
                                  crmLinkError.OperationType  = "update"
                                  newCrmLinkError.Save()
                                Else
                                  crmLinkError.ObjectData     = field.Value
                                  crmLinkError.Exception      = ex.ToString() + ex.StackTrace                                               
                                End If                                              
                              End Try
                            End If
                          Next
                        End If
                      End If
                      Case "Contact"
                      companyToUpdate.LoadByCRMLinkID(accountZohoID, CRMLinkRow.OrganizationID)
                      If companyToUpdate.Count > 0 Then
                        Dim userToUpdate As New Users(User)
                        userToUpdate.LoadByEmail(GetContactZohoEmail(recordToUpdate), companyToUpdate(0).OrganizationID)
                        If userToUpdate.Count > 0 Then
                          For Each field As XElement In recordToUpdate.Descendants("FL")
                            Dim fieldMapping As CRMLinkField = customMappings.FindByCRMFieldName(field.Attribute("val").Value)
                            If fieldMapping IsNot Nothing Then
                              Try
                                If fieldMapping.CustomFieldID IsNot Nothing Then
                                  crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(accountZohoID, fieldMapping.CustomFieldID.ToString())
                                  UpdateCustomValue(fieldMapping.CustomFieldID, userToUpdate(0).UserID, field.Value)
                                ElseIf fieldMapping.TSFieldName IsNot Nothing Then
                                  crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(accountZohoID, fieldMapping.TSFieldName)
                                  userToUpdate(0).Row(fieldMapping.TSFieldName) = field.Value
                                  userToUpdate(0).BaseCollection.Save()
                                End If
                                If crmLinkError IsNot Nothing then
                                  crmLinkError.Delete()
                                  crmLinkErrors.Save()
                                End If

                              Catch ex As Exception
                                If crmLinkError Is Nothing then
                                  Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                                  crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                                  crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                                  crmLinkError.CRMType        = CRMLinkRow.CRMType
                                  crmLinkError.Orientation    = "in"
                                  crmLinkError.ObjectType     = "contact"
                                  crmLinkError.ObjectID       = accountZohoID
                                  If fieldMapping.CustomFieldID IsNot Nothing Then
                                    crmLinkError.ObjectFieldName = fieldMapping.CustomFieldID.ToString()
                                  ElseIf fieldMapping.TSFieldName IsNot Nothing Then
                                    crmLinkError.ObjectFieldName = fieldMapping.TSFieldName
                                  End If
                                  crmLinkError.ObjectData     = field.Value
                                  crmLinkError.Exception      = ex.ToString() + ex.StackTrace
                                  crmLinkError.OperationType  = "update"
                                  newCrmLinkError.Save()
                                Else
                                  crmLinkError.ObjectData     = field.Value
                                  crmLinkError.Exception      = ex.ToString() + ex.StackTrace                                               
                                End If                                              
                              End Try
                            End If
                          Next
                        End If
                      End If
                    End Select
                Next
                crmLinkErrors.Save()
              End If
            End Sub

              Private Function GetCustomMappingValues(
                ByVal objType As String, 
                ByVal customMappings As CRMLinkFields, 
                ByVal accountIDToUpdate As String) As List(Of XElement)

                Dim result As List(Of XElement) = New List(Of XElement)
                Dim needToGetMore As Boolean = True
                Dim returnedRecords As Integer = 0
                Dim totalRecords As Integer = 1
                Dim query As String = String.Empty

                While needToGetMore
                  For Each record As XElement In XElement.Load(New XmlNodeReader(GetCustomMappingValuesBatch(
                    objType, 
                    totalRecords, 
                    totalRecords + MaxBatchSize - 1, 
                    customMappings, 
                    accountIDToUpdate, 
                    query))).Descendants("row")

                    result.Add(record)
                    returnedRecords += 1
                    totalRecords += 1

                  Next

                  If returnedRecords = MaxBatchSize Then
                    Log.Write("getting next set of records..." & totalRecords.ToString())
                    returnedRecords = 0
                  Else
                    needToGetMore = False
                  End If
                End While

                Return result
              End Function

                Private Function GetCustomMappingValuesBatch(
                  ByVal objType As String, 
                  ByVal fromIndex As Integer, 
                  ByVal toIndex As Integer, 
                  ByVal customMappings As CRMLinkFields,
                  ByVal accountIDToUpdate As String,
                  Optional ByRef query As String = "") As XmlDocument

                  If (query = String.Empty) Then
                    query = GetCustomMappingValuesQuery(objType, customMappings, accountIDToUpdate)
                  End If

                  Dim batchQuery As String = query & "&fromIndex=" & fromIndex.ToString() & "&toIndex=" & toIndex.ToString()
                  Log.Write("querying " & batchQuery)

                  Return GetZohoCRMXML(batchQuery)
                End Function

                  Private Function GetCustomMappingValuesQuery(
                    ByVal objType As String, 
                    ByVal customMappings As CRMLinkFields,
                    ByVal accountIDToUpdate As String) As String

                    Dim result As StringBuilder = New StringBuilder()
                    Select Case objType
                      Case "Account"
                        result.Append("Accounts/getRecords?newFormat=1&selectColumns=Accounts(")
                        result.Append(GetListOfFields(customMappings))
                        result.Append(")")
                        If CRMLinkRow.LastLink IsNot Nothing Then
                            '''Zoho's lastModifiedTime is in the user's local time. 
                            '''To include any possible timezone we use the smallest one (UTC-12:00) International Date Line West. (720)
                            '''Then we go back 30 more minutes as it was coded before.
                            result.Append("&lastModifiedTime=" & CRMLinkRow.LastLinkUtc.Value.AddMinutes(-750).ToString("s").Replace("T", " "))
                        End If
                      Case "Contact"
                        result.Append("Contacts/getRecords?newFormat=1")
                        'By implementing selectColumns the account id cannot be added in the contacts table. therefore we'll need to bring all the columns
                        'result.Append("&selectColumns=Contacts(")
                        'result.Append(GetListOfFields(customMappings))
                        'result.Append(")")
                        If CRMLinkRow.LastLink IsNot Nothing Then
                            result.Append("&lastModifiedTime=" & CRMLinkRow.LastLinkUtc.Value.AddMinutes(-750).ToString("s").Replace("T", " "))
                        End If
                    End Select

                    Return result.ToString()
                  End Function

                    Private Function GetListOfFields(ByVal customMappings As CRMLinkFields) As String
                      Dim listOfFields As List(Of String) = New List(Of String)()
                      For Each customMapping As CRMLinkField In customMappings
                        If Not listOfFields.Contains(customMapping.CRMFieldName) Then
                          listOfFields.Add(customMapping.CRMFieldName)
                        End If
                      Next

                      listOfFields.Add("Account Type")
                      listOfFields.Add("Email")

                      Return String.Join(",", listOfFields.ToArray())
                    End Function

              Private Function GetAccountZohoType(ByVal recordToUpdate As XElement, ByRef accountZohoID As String) As String
                Dim result As String = String.Empty
                For Each record As XElement In recordToUpdate.Descendants("FL")
                  Select Case record.Attribute("val").Value
                    Case "ACCOUNTID"
                      accountZohoID = record.Value
                    Case "Account Type"
                      result = record.Value
                  End Select
                Next
                Return result.ToLower()
              End Function

              Private Function GetContactZohoEmail(ByVal recordToUpdate As XElement) As String
                Dim result As String = String.Empty
                For Each record As XElement In recordToUpdate.Descendants("FL")
                  If record.Attribute("val").Value = "Email" Then
                    result = record.Value
                  End If
                Next
                Return result
              End Function

            Private Function GetZohoCompanyXML() As XmlDocument
                Return GetZohoCompanyXML(1, MaxBatchSize)
            End Function

            ''' <summary>
            ''' Gets an XML document from Zoho containing data about Companies
            ''' </summary>
            ''' <param name="from">the starting index</param>
            ''' <param name="_to">the ending index</param>
            ''' <returns></returns>
            ''' <remarks>The maximum allowed difference between start and end is 200 (Zoho will only return 200 records at a time)</remarks>
            Private Function GetZohoCompanyXML(ByVal from As Short, ByVal _to As Short) As XmlDocument
                Dim ZohoPath As String = "Accounts/getRecords?newFormat=1&fromIndex=" & from.ToString() & "&toIndex=" & _to.ToString()

                If CRMLinkRow.LastLink IsNot Nothing Then
                    '''Zoho's lastModifiedTime is in the user's local time. 
                    '''To include any possible timezone we use the smallest one (UTC-12:00) International Date Line West. (720)
                    '''Then we go back 30 more minutes as it was coded before.
                    ZohoPath &= "&lastModifiedTime=" & CRMLinkRow.LastLinkUtc.Value.AddMinutes(-750).ToString("s").Replace("T", " ")
                End If

                Log.Write("querying " & ZohoPath)
                Log.Write("LoginUser TimeZone: " & CRMLinkRow.BaseCollection.LoginUser.TimeZoneInfo.ToString())
                Return GetZohoCRMXML(ZohoPath)
            End Function

            Private Function GetZohoPeopleXML() As XmlDocument
                Return GetZohoPeopleXML(1, MaxBatchSize)
            End Function

            Private Function GetZohoPeopleXML(ByVal fromIndex As Short, ByVal toIndex As Short) As XmlDocument
                Dim zohoPath As String = "Contacts/getRecords?newFormat=1&fromIndex=" & fromIndex.ToString() & "&toIndex=" & toIndex.ToString()
                If CRMLinkRow.LastLink IsNot Nothing Then
                    ZohoPath &= "&lastModifiedTime=" & CRMLinkRow.LastLinkUtc.Value.AddMinutes(-750).ToString("s").Replace("T", " ")
                End If
                Return GetZohoCRMXML(zohoPath)
            End Function

            Private Function GetZohoProductsXML(ByVal AccountID As String) As XmlDocument
                Return GetZohoProductsXML(AccountID, 1, MaxBatchSize)
            End Function

            Private Function GetZohoProductsXML(ByVal AccountID As String, ByVal fromIndex As Short, ByVal toIndex As Short) As XmlDocument
                Return GetZohoCRMXML("Products/getRelatedRecords?newFormat=1&parentModule=Accounts&id=" & AccountID & "&fromIndex=" & fromIndex.ToString() & "&toIndex=" & toIndex.ToString())
            End Function

            Private Function CreateNote(ByVal accountid As String, ByVal thisTicket As Ticket) As Boolean
                Dim description As Action = Actions.GetTicketDescription(User, thisTicket.TicketID)
                Dim NoteBody As String = String.Format("A ticket has been created for this organization entitled ""{0}"".{3}{2}{3}Click here to access the ticket information: https://app.teamsupport.com/Ticket.aspx?ticketid={1}", _
                                                                             thisTicket.Name, thisTicket.TicketID.ToString(), HtmlUtility.StripHTML(description.Description), Environment.NewLine)

                Dim ZohoUri As Uri = Nothing
                ZohoUri = New Uri("https://crm.zoho.com/crm/private/xml/Notes/insertRecords?newFormat=1&authtoken=" & CRMLinkRow.SecurityToken1 & "&scope=crmapi")

                Dim postData As String = String.Format("<Notes><row no=""1"">" & _
                                        "<FL val=""entityId"">{0}</FL>" & _
                                        "<FL val=""Note Title"">Support Issue: {1}</FL>" & _
                                         "<FL val=""Note Content""><![CDATA[{2}]]></FL></row></Notes>", _
                                         accountid, thisTicket.Name, NoteBody)

                postData = "&xmlData=" & HttpUtility.UrlEncode(postData)

                Return PostQueryString(Nothing, ZohoUri, postData) = HttpStatusCode.OK
            End Function
        End Class


        Public Class ZohoReports
            Inherits Zoho

            Private databaseName As String = "TeamSupport"
            Private reportsToSend As Dictionary(Of String, String)

            Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor)
                MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, IntegrationType.ZohoReports)

                'this is perhaps not the best way to do it, but each dictionary entry contains the report name|primary key, date field (for checking if there is new data), and then the sql query.
                reportsToSend = New Dictionary(Of String, String)()
                reportsToSend.Add("KnowledgeBaseTraffic|ViewingIP,DateAndTime", "select k.viewdatetime as 'DateAndTime', t.name as 'ArticleTitle',  searchterm as 'SearchTermUsed', viewip as 'ViewingIP' from KBStats as k, organizations as o, tickets as t where k.organizationid = @OrganizationID and k.organizationid = o.organizationid and k.kbarticleid = t.ticketid AND k.viewdatetime > @LastModified")
                reportsToSend.Add("ChatRequests|ChatRequestor,DateAndTime", "select cr.datecreated as 'DateAndTime', cc.lastname+', '+cc.firstname as 'ChatRequestor', cc.email as 'RequestorsEmail',cr.message as 'Question', cr.isaccepted as 'ChatAccepted' from chatrequests as cr, organizations as o, chatclients as cc where cr.organizationid = o.organizationid and cc.chatclientid = cr.requestorid and o.organizationid = @OrganizationID AND cr.datecreated > @LastModified")
                reportsToSend.Add("TicketStatusHistory|User_Who_Changed,Time_Status_Changed", "select t.ticketnumber as Ticket_Number, t.name as Ticket_Name,  ts_old.name as Old_Status, ts_new.name as New_Status, StatusChangeTime as Time_Status_Changed, datediff(mi,'1900-01-01', sh.timeinoldstatus) as Time_In_Old_Status, u.lastname+', '+u.firstname as User_Who_Changed from statushistory as sh left outer join ticketstatuses as ts_old on sh.oldstatus = ts_old.ticketstatusid left outer join ticketstatuses as ts_new on sh.newstatus = ts_new.ticketstatusid,tickets as t, users as u where sh.ticketid = t.ticketid and sh.modifierid = u.userid and sh.organizationid = @OrganizationID AND StatusChangeTime > @LastModified")
                reportsToSend.Add("PortalLoginHistory|Username,LoginDateTime", "select Username, Success, LoginDateTime, IPAddress from portalloginhistory where OrganizationID = @OrganizationID AND LoginDateTime > @LastModified")

                'if they are not useing the default database name (TeamSupport), they can specify the name in CrmLinkRow.TypeFieldMatch
                If CRMLinkRow.TypeFieldMatch IsNot Nothing AndAlso CRMLinkRow.TypeFieldMatch <> "" Then
                    databaseName = CRMLinkRow.TypeFieldMatch
                End If
            End Sub

            Public Overrides Function PerformSync() As Boolean
                Dim Success As Boolean = False

                'check to make sure we have all the data we need
                If CRMLinkRow.SecurityToken1 Is Nothing OrElse CRMLinkRow.SecurityToken1 = "" Then
                  _exception = New IntegrationException("Authentication Token not specified.")
                ElseIf CRMLinkRow.Password Is Nothing OrElse CRMLinkRow.Username Is Nothing OrElse CRMLinkRow.Password = "" OrElse CRMLinkRow.Username = "" Then
                  _exception = New IntegrationException("Username or password not specified.")
                End If

                If Exception IsNot Nothing Then
                    Return False
                End If

                Log.Write("Sending report data...")

                Try
                    CheckGrid()
                    Success = SendReportData()
                Catch ex As Exception
                    Log.Write("exception: " & ex.Message & ": " & ex.StackTrace)
                    Success = False
                Finally
                    'Logout()
                End Try

                Return Success
            End Function

            ''' <summary>
            ''' Check if the hostname exists, if not then set it to default to the US grid: reportsapi.zoho.com
            ''' This way we ensure that all existing records without this entry (prior to the update to handle european grid) will be set to the US one. If it is already entered then nothing is done.
            ''' </summary>
            ''' <returns></returns>
            Private Function CheckGrid()
                If (CRMLinkRow.HostName Is Nothing OrElse String.IsNullOrEmpty(CRMLinkRow.HostName)) Then
                    CRMLinkRow.HostName = "reportsapi.zoho.com"
                    Log.Write("Hostname was null or empty, it has been set to: reportsapi.zoho.com")
                End If
            End Function

            ''' <summary>
            ''' Sends report data over to Zoho Reports
            ''' </summary>
            ''' <returns>Whether or not data was sent successfully</returns>
            ''' <remarks></remarks>
            Private Function SendReportData() As Boolean
                Dim thisSettings As New ServiceLibrary.Settings(User, Processor.ServiceName)

                'load data from ticketsview into a csv
                Dim tix As New TicketsView(User)
                tix.LoadForZoho(CRMLinkRow.OrganizationID, CRMLinkRow.LastLink)

                Dim ticketsViewBatchSize As Integer = thisSettings.ReadInt("ZohoReportsBatchSize", 50000)

                Dim batches As List(Of String) = GetCSVBatches(tix.Table, ticketsViewBatchSize)

                If batches IsNot Nothing Then
                    Log.Write("Found " & batches.Count & " ticketsView batches to send to Zoho.")

                    'now that we have data, send it to zoho
                    For Each batch As String In batches
                        If Processor.IsStopped Then
                            Return False
                        End If

                        Dim byteData As Byte() = UTF8Encoding.UTF8.GetBytes(batch)

                        If Not ImportZohoCSV("Tickets", "TicketNumber", byteData) Then
                            Log.Write("Error sending Tickets to Zoho.")
                            Return False
                        End If

                        'we have to delete the dummy row we insert with each import
                        Log.Write("Deleting datatype row...")
                        If DeleteZohoTableRow("Tickets", "TicketNumber", "-1") Then
                            Log.Write("row -1 deleted successfully")
                        Else
                            Log.Write("Error deleting dummy row.")
                            Return False
                        End If
                    Next

                    Log.Write("TicketsView batches sent successfully.")
                Else
                    Log.Write("no TicketsView data to send.")
                End If

                'get batches for other reports
                Log.Write(reportsToSend.Count & " other reports to send to Zoho...")

                For Each reportQuery As KeyValuePair(Of String, String) In reportsToSend
                    If Processor.IsStopped Then
                        Return False
                    End If

                    Dim tableName As String = reportQuery.Key.Split("|")(0)
                    Dim tableKey As String = reportQuery.Key.Split("|")(1)

                    Dim thisCommand As New SqlCommand(reportQuery.Value)

                    thisCommand.Parameters.AddWithValue("@OrganizationID", CRMLinkRow.OrganizationID)
                    thisCommand.Parameters.AddWithValue("@LastModified", If(CRMLinkRow.LastLink.HasValue, CRMLinkRow.LastLink.Value.AddMinutes(-15), New DateTime(1900, 1, 1)))

                    Dim thisTable As DataTable = SqlExecutor.ExecuteQuery(User, thisCommand)
                    Dim batches2 As List(Of String) = GetCSVBatches(thisTable, ticketsViewBatchSize)

                    If batches2 IsNot Nothing Then
                        Log.Write("Found " & batches2.Count & " " & tableName & " batches to send to Zoho...")

                        For Each batch As String In batches2
                            If Processor.IsStopped Then
                                Return False
                            End If

                            Dim byteData As Byte() = UTF8Encoding.UTF8.GetBytes(batch)

                            If Not ImportZohoCSV(tableName, tableKey, byteData) Then
                                Log.Write("Error sending " & tableName & " to Zoho.")
                                Return False
                            End If

                            Log.Write("Deleting datatype row...")
                            If DeleteZohoTableRow(tableName, tableKey.Split(",")(0), "'String'") Then
                                Log.Write("Row deleted successfully.")
                            Else
                                Log.Write("Error deleting datatype row.")
                                Return False
                            End If
                        Next
                    Else
                        Log.Write("No " & tableName & " data to send.")
                    End If

                Next

                Return True
            End Function

            Private Function GetCSVBatches(ByVal thisTable As DataTable, ByVal batchSize As Integer) As List(Of String)
                Dim csvBatches As New List(Of String)()
                Dim csvContent As StringBuilder = Nothing
                Dim csvHeader As New StringBuilder()
                Dim rowIndex As Integer = 0
				Dim ticketUrlHeader As String = "ticketurl"

                'column names in first row
                For i As Integer = 0 To thisTable.Columns.Count - 1
                    csvHeader.Append(thisTable.Columns(i).ColumnName)
                    csvHeader.Append(If(i < thisTable.Columns.Count - 1, ",", Environment.NewLine))
                Next

                'append a dummy row so zoho knows what data type each column is--in case this is the first import
                For Each thisCol As DataColumn In thisTable.Columns
                    If thisCol.ColumnName.ToLower() = ticketUrlHeader Then
                        csvHeader.Append("""http://www.teamsupport.com""")
                    ElseIf thisCol.DataType Is GetType(String) Then
                        csvHeader.Append("""String""")
                    ElseIf thisCol.DataType Is GetType(DateTime) Then
                        csvHeader.Append("""1/1/1900 1:00:00""")
                    ElseIf thisCol.DataType Is GetType(Boolean) Then
                        csvHeader.Append("true")
                    ElseIf thisCol.DataType Is GetType(Decimal) Then
                        csvHeader.Append("0.0")
                    Else
                        csvHeader.Append("-1")
                    End If
                    csvHeader.Append(If(thisCol.Ordinal < thisTable.Columns.Count - 1, ",", Environment.NewLine))
                Next

                Dim organization As Organizations = New Organizations(User)
                organization.LoadByOrganizationID(CRMLinkRow.OrganizationID)

                For Each thisRow As DataRow In thisTable.Rows
                    If Processor.IsStopped Then
                        Return Nothing
                    End If

                    If csvContent Is Nothing Then
                        csvContent = New StringBuilder(csvHeader.ToString())
                    End If

                    For i As Integer = 0 To thisTable.Columns.Count - 1
                        If thisTable.Columns(i).DataType Is GetType(String) Then
							csvContent.Append("""" & thisRow(i).ToString().Replace("""", "'") & """")
                        ElseIf thisTable.Columns(i).DataType Is GetType(DateTime) AndAlso Not IsDBNull(thisRow(i)) Then 'translate dates to org's local timezone
                            csvContent.Append("""" & TimeZoneInfo.ConvertTimeFromUtc(CType(thisRow(i).ToString(), DateTime), TimeZoneInfo.FindSystemTimeZoneById(organization(0).TimeZoneID)).ToString("M/d/yyyy H:mm:ss") & """")
                        ElseIf thisTable.Columns(i).ColumnName.ToLower() = ticketUrlHeader Then
							csvContent.Append("""" & SystemSettings.ReadString(User, "AppDomain", "https://app.teamsupport.com") & "?ticketid=" & thisRow(i).ToString() & """")
						Else
                            csvContent.Append(thisRow(i).ToString())
                        End If
                        csvContent.Append(If(i < thisTable.Columns.Count - 1, ",", Environment.NewLine))
                    Next
                    rowIndex += 1

                    If rowIndex = batchSize Then
                        csvBatches.Add(csvContent.ToString())

                        rowIndex = 0
                        csvContent = Nothing
                    End If

                Next

                If csvContent IsNot Nothing Then
                    csvBatches.Add(csvContent.ToString())
                End If

                Return csvBatches
            End Function

            Private Function ImportZohoCSV(ByVal tableName As String, ByVal keyName As String, ByVal byteData As Byte()) As Boolean
                Dim success As Boolean = True

                Dim zohoUri As Uri = Nothing

                zohoUri = New Uri(String.Format("https://{4}/api/{0}/{2}/{3}?ZOHO_ACTION=IMPORT&ZOHO_OUTPUT_FORMAT=XML&ZOHO_ERROR_FORMAT=XML&authtoken={1}&ZOHO_API_VERSION=1.0",
                                  CRMLinkRow.Username, CRMLinkRow.SecurityToken1, databaseName, tableName, CRMLinkRow.HostName.Replace("https://", "").Replace("http://", "")))

                Dim postParameters As New Dictionary(Of String, Object)()

                postParameters.Add("ZOHO_IMPORT_TYPE", "UPDATEADD")
                postParameters.Add("ZOHO_MATCHING_COLUMNS", keyName)
                postParameters.Add("ZOHO_AUTO_IDENTIFY", "false")
                postParameters.Add("ZOHO_ON_IMPORT_ERROR", "ABORT")
                postParameters.Add("ZOHO_DELIMITER", "0")
                postParameters.Add("ZOHO_QUOTED", "2")
                postParameters.Add("ZOHO_CREATE_TABLE", "true")
                postParameters.Add("ZOHO_DATE_FORMAT", "MM/dd/yyyy HH:mm:ss")
                postParameters.Add("ZOHO_FILE", byteData)

                '       File.WriteAllBytes(Path.Combine(thisSettings.ReadString("Log File Path", "C:\CrmLogs\"), CRMLinkRow.OrganizationID.ToString() & "\reports.csv"), byteData)

                Try
                    Using response As HttpWebResponse = WebHelpers.MultipartFormDataPost(zohoUri, Client, postParameters)

                        If response.StatusCode <> HttpStatusCode.OK Then
                            Log.Write("Error posting query string: " & response.StatusDescription)
                        Else
                            Log.Write(response.StatusDescription)
                        End If

                    End Using
                Catch ex As WebException
                    Log.Write("Error contacting " & zohoUri.ToString() & ": " & ex.Message)

                    If ex.Response IsNot Nothing Then
                        Log.Write(New StreamReader(ex.Response.GetResponseStream()).ReadToEnd())
                    End If

                    success = False
                End Try

                Return success
            End Function

            Private Function DeleteZohoTableRow(ByVal tableName As String, ByVal keyName As String, ByVal rowKey As String) As Boolean
                Dim DeletePath As String = String.Format("{0}/{1}/{2}?ZOHO_ACTION=DELETE&ZOHO_OUTPUT_FORMAT=XML&ZOHO_ERROR_FORMAT=XML&authtoken={3}&ZOHO_API_VERSION=1.0", CRMLinkRow.Username, databaseName, tableName, CRMLinkRow.SecurityToken1)                

                Dim DeleteParameters As String = "&ZOHO_CRITERIA=(""" & keyName & """ = " & rowKey & ")"

                Log.Write(DeleteParameters)
                Return PostZohoReports(DeletePath, DeleteParameters) = HttpStatusCode.OK
            End Function

            Private Function PostZohoReports(ByVal PathAndQuery As String, ByVal PostParameters As String) As HttpStatusCode
                Dim ZohoUri As Uri = New Uri("https://" + CRMLinkRow.HostName.Replace("https://", "").Replace("http://", "") + "/api/" & PathAndQuery)
                Return PostQueryString(Nothing, ZohoUri, PostParameters)
            End Function

        End Class
    End Namespace
End Namespace