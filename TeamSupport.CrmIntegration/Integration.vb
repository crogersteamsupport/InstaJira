Imports TeamSupport.Data
Imports System.Xml
Imports System.Xml.Serialization
Imports System.IO
Imports System.Net
Imports System.Text
Imports Newtonsoft.Json
Imports PusherServer

Namespace TeamSupport
    Namespace CrmIntegration
        Public MustInherit Class Integration
            Protected CRMLinkRow As CRMLinkTableItem
            Protected Log As SyncLog
            Protected User As LoginUser
            Protected Processor As CrmProcessor
            Protected ReadOnly Type As IntegrationType
            Protected Const Client As String = "Muroc Client"
            Protected _exception As IntegrationException = Nothing

            'tracks global errors so we can not update the sync date if there's a problem
            Public Property SyncError As Boolean
            Public Property ErrorCode As IntegrationError = IntegrationError.None 'should be deprecated in favor of IntegrationException
            Public ReadOnly Property Exception As IntegrationException
                Get
                    Return _exception
                End Get
            End Property


            Protected Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor, ByVal thisType As IntegrationType)
                CRMLinkRow = crmLinkOrg
                Log = crmLog
                User = thisUser
                Type = thisType
                Processor = thisProcessor
            End Sub

            Public MustOverride Function PerformSync() As Boolean

            Protected Delegate Function GetCompanyXML() As XmlDocument
            Protected Delegate Function ParseCompanyXML(ByVal CompaniesToSync As XmlDocument) As List(Of CompanyData)
            Protected Delegate Function GetPeopleXML() As XmlDocument
            Protected Delegate Function ParsePeopleXML(ByVal PeopleToSync As XmlDocument) As List(Of EmployeeData)
            Protected Delegate Function GetProductsXML(ByVal AccountID As String) As XmlDocument
            Protected Delegate Function ParseProductsXML(ByVal PeopleToSync As XmlDocument, ByVal AccountID As String) As List(Of ProductData)

            Protected Delegate Sub GetCustomFields(ByVal objType As String, ByVal AccountIDToUpdate As String)

            ''' <summary>
            ''' Sync company and customer data from an outside CRM into TeamSupport
            ''' </summary>
            ''' <param name="GetCompanyXML">A function returning the XML containing company data from the CRM</param>
            ''' <param name="ParseCompanyXML">A function that will parse the XML company data into a list of companydata</param>
            ''' <param name="GetPeopleXML">A function returning the XML containing customer data from the CRM</param>
            ''' <param name="ParsePeopleXML">A function that will parse the XML customer data into a list of employeedata</param>
            ''' <returns>Whether or not the sync processed successfully</returns>
            ''' <remarks>This should be used for all new integrations and older integrations can be updated to use it as time permits</remarks>
            Protected Function NewSyncAccounts(
              ByVal GetCompanyXML As GetCompanyXML,
              ByVal ParseCompanyXML As ParseCompanyXML,
              ByVal GetPeopleXML As GetPeopleXML,
              ByVal ParsePeopleXML As ParsePeopleXML,
              ByVal GetProductsXML As GetProductsXML,
              ByVal ParseProductsXML As ParseProductsXML,
              ByVal GetCustomFields As GetCustomFields
            ) As Boolean
                Dim CompaniesToSync As XmlDocument
                Dim CompanySyncData As List(Of CompanyData) = Nothing

                'retrieve company data
                CompaniesToSync = GetCompanyXML()
                Log.Write("The GetCompanyXML method has been executed.")
                If CompaniesToSync IsNot Nothing Then
                    Dim companiesToSyncAsXElement As XElement = XElement.Load(New XmlNodeReader(CompaniesToSync))
                    If companiesToSyncAsXElement.Descendants("error").Count() > 0 Then
                        _exception = New IntegrationException(companiesToSyncAsXElement.Value)
                        SyncError = True
                    Else
                        'parse company data
                        Log.Write("CompaniesToSync Count: " + companiesToSyncAsXElement.Descendants("row").Count().ToString())
                        If companiesToSyncAsXElement.Descendants("row").Count() > 0 Then
                            CompanySyncData = ParseCompanyXML(CompaniesToSync)
                            Log.Write("ParseCompanyXML method executed.")
                            If CompanySyncData IsNot Nothing Then

                                Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
                                crmLinkErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "in", "company")
                                Dim crmLinkError As CRMLinkError = Nothing

                                Log.Write(String.Format("Processed {0} accounts.", CompanySyncData.Count))

                                'update info for organizations
                                For Each company As CompanyData In CompanySyncData
                                    crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(company.AccountID, String.Empty)
                                    Try
                                        UpdateOrgInfo(company, CRMLinkRow.OrganizationID)
                                        If crmLinkError IsNot Nothing Then
                                            crmLinkError.Delete()
                                            crmLinkErrors.Save()
                                        End If
                                    Catch ex As Exception
                                        If crmLinkError Is Nothing Then
                                            Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                                            crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                                            crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                                            crmLinkError.CRMType = CRMLinkRow.CRMType
                                            crmLinkError.Orientation = "in"
                                            crmLinkError.ObjectType = "company"
                                            crmLinkError.ObjectID = company.AccountID
                                            crmLinkError.ObjectData = JsonConvert.SerializeObject(company)
                                            crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                            crmLinkError.OperationType = "unknown"
                                            newCrmLinkError.Save()
                                        Else
                                            crmLinkError.ObjectData = JsonConvert.SerializeObject(company)
                                            crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                        End If
                                    End Try
                                Next
                                crmLinkErrors.Save()

                                Log.Write("Finished updating account information.")
                                GetCustomFields("Account", String.Empty)
                                Log.Write("Finished updating Accounts Custom Mappings")

                                If CRMLinkRow.PullCustomerProducts Then
                                    Log.Write("Updating products information...")

                                    crmLinkErrors = New CRMLinkErrors(Me.User)
                                    crmLinkErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "in", "product")

                                    For Each company As CompanyData In CompanySyncData
                                        'get products data for each company
                                        Dim ProductsToSync As XmlDocument = GetProductsXML(company.AccountID)

                                        If ProductsToSync IsNot Nothing Then
                                            Dim ProductsSyncData As List(Of ProductData) = ParseProductsXML(ProductsToSync, company.AccountID)

                                            If ProductsSyncData IsNot Nothing Then
                                                For Each product As ProductData In ProductsSyncData
                                                    Try
                                                        crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(company.AccountID + product.Name + product.ExpirationDate.ToString(), String.Empty)
                                                        'update info for products
                                                        UpdateProductInfo(product, company.AccountID, CRMLinkRow.OrganizationID)
                                                        If crmLinkError IsNot Nothing Then
                                                            crmLinkError.Delete()
                                                            crmLinkErrors.Save()
                                                        End If
                                                    Catch ex As Exception
                                                        If crmLinkError Is Nothing Then
                                                            Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                                                            crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                                                            crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                                                            crmLinkError.CRMType = CRMLinkRow.CRMType
                                                            crmLinkError.Orientation = "in"
                                                            crmLinkError.ObjectType = "product"
                                                            crmLinkError.ObjectID = company.AccountID + product.Name + product.ExpirationDate.ToString()
                                                            crmLinkError.ObjectData = JsonConvert.SerializeObject(product)
                                                            crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                                            crmLinkError.OperationType = "unknown"
                                                            newCrmLinkError.Save()
                                                        Else
                                                            crmLinkError.ObjectData = JsonConvert.SerializeObject(product)
                                                            crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                                        End If
                                                    End Try
                                                Next

                                                Log.Write("Updated product information for " & company.AccountName)
                                            End If
                                        End If
                                    Next
                                    crmLinkErrors.Save()
                                    Log.Write("Finished updating product information")
                                End If
                            End If
                        End If
                        Log.Write("Updating people information...")

                        Dim PeopleToSync As XmlDocument = GetPeopleXML()
                        Log.Write("The GetCompanyXML method has been executed.")
                        If PeopleToSync IsNot Nothing Then
                            Dim peopleToSyncAsXElement As XElement = XElement.Load(New XmlNodeReader(PeopleToSync))
                            If peopleToSyncAsXElement.Descendants("error").Count() > 0 Then
                                _exception = New IntegrationException(peopleToSyncAsXElement.Value)
                                SyncError = True
                            Else
                                Dim PeopleSyncData As List(Of EmployeeData) = Nothing
                                Log.Write("PeopleToSync Count: " + peopleToSyncAsXElement.Descendants("row").Count().ToString())
                                If peopleToSyncAsXElement.Descendants("row").Count() > 0 Then
                                    PeopleSyncData = ParsePeopleXML(PeopleToSync)
                                    Log.Write("ParsePeopleXML method executed.")

                                    If PeopleSyncData IsNot Nothing Then
                                        Log.Write(String.Format("Processed {0} contacts.", PeopleSyncData.Count))

                                        Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
                                        crmLinkErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "in", "contact")
                                        Dim crmLinkError As CRMLinkError = Nothing

                                        'update info for customers
                                        For Each person As EmployeeData In PeopleSyncData
                                            crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(person.ZohoID, String.Empty)
                                            Try
                                                UpdateContactInfo(person, person.ZohoID, CRMLinkRow.OrganizationID)
                                                If crmLinkError IsNot Nothing Then
                                                    crmLinkError.Delete()
                                                    crmLinkErrors.Save()
                                                End If
                                            Catch ex As Exception
                                                If crmLinkError Is Nothing Then
                                                    Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                                                    crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                                                    crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                                                    crmLinkError.CRMType = CRMLinkRow.CRMType
                                                    crmLinkError.Orientation = "in"
                                                    crmLinkError.ObjectType = "contact"
                                                    crmLinkError.ObjectID = person.ZohoID
                                                    crmLinkError.ObjectData = JsonConvert.SerializeObject(person)
                                                    crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                                    crmLinkError.OperationType = "unknown"
                                                    newCrmLinkError.Save()
                                                Else
                                                    crmLinkError.ObjectData = JsonConvert.SerializeObject(person)
                                                    crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                                End If
                                            End Try
                                        Next
                                        crmLinkErrors.Save()
                                        Log.Write("Finished updating people information")
                                        GetCustomFields("Contact", Nothing)
                                        Log.Write("Finished updating Contacts Custom Mappings")
                                    Else
                                        Log.Write("PeopleSyncData was nothing.")
                                    End If
                                End If
                            End If
                        Else
                            Log.Write("PeopleToSync was nothing.")
                            SyncError = True
                        End If
                    End If
                Else
                    Log.Write("CompaniesToSync was nothing.")
                    SyncError = True
                End If

                Return Not SyncError
            End Function

            Protected Delegate Function CreateCRMNote(ByVal accountid As String, ByVal thisTicket As Ticket) As Boolean

            ''' <summary>
            ''' Save newly created tickets into the notes (or equivalent) field in the CRM system
            ''' </summary>
            ''' <param name="CreateCRMNote">A function that handles the creating of a note for the specific integration</param>
            ''' <returns>Whether or not all notes were created successfully</returns>
            ''' <remarks></remarks>
            Protected Function SendTicketData(ByVal CreateCRMNote As CreateCRMNote) As Boolean
                Dim ParentOrgID As String = CRMLinkRow.OrganizationID

                Try
                    If CRMLinkRow.SendBackTicketData Then
                        'get tickets created after the last link date
                        Dim tickets As New Tickets(User)
                        tickets.LoadByCRMLinkItem(CRMLinkRow)

                        If tickets IsNot Nothing Then
                            Log.Write(String.Format("Found {0} tickets to sync.", tickets.Count.ToString()))

                            Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
                            crmLinkErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "out", "ticket")
                            Dim crmLinkError As CRMLinkError = Nothing

                            For Each thisTicket As Ticket In tickets
                                If Processor.IsStopped Then
                                    Return False
                                End If

                                Dim atLeastOneSucceded As Boolean = False

                                'get a list of customers associated to the ticket
                                Dim customers As New OrganizationsView(User)
                                customers.LoadByTicketID(thisTicket.TicketID)

                                crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(thisTicket.TicketID.ToString(), "note")

                                For Each customer As OrganizationsViewItem In customers
                                    If customer.CRMLinkID <> "" Then
                                        Log.Write("Creating a comment...")

                                        If CreateCRMNote(customer.CRMLinkID, thisTicket) Then
                                            Log.Write("Comment created successfully.")
                                            atLeastOneSucceded = True
                                        Else
                                            Log.Write("Error creating comment.")
                                        End If
                                    End If
                                Next

                                CRMLinkRow.LastTicketID = thisTicket.TicketID
                                CRMLinkRow.Collection.Save()

                                ActionLogs.AddActionLog(
                                    User,
                                    ActionLogType.Insert,
                                    ReferenceType.Tickets,
                                    thisTicket.TicketID,
                                    "Sent ticket data to " + Type.ToString() + ".")

                                If atLeastOneSucceded Then
                                    If crmLinkError IsNot Nothing Then
                                        crmLinkError.Delete()
                                        crmLinkErrors.Save()
                                    End If
                                ElseIf crmLinkError Is Nothing Then
                                    Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                                    crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                                    crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                                    crmLinkError.CRMType = CRMLinkRow.CRMType
                                    crmLinkError.Orientation = "out"
                                    crmLinkError.ObjectType = "ticket"
                                    crmLinkError.ObjectFieldName = "note"
                                    crmLinkError.ObjectID = thisTicket.TicketID.ToString()
                                    crmLinkError.Exception = "Error creating ticket as note."
                                    crmLinkError.OperationType = "create"
                                    newCrmLinkError.Save()
                                Else
                                    crmLinkError.Exception = "Error creating ticket as note."
                                End If
                            Next

                            crmLinkErrors.Save()
                        Else
                            Log.Write("No new tickets to sync.")
                        End If
                    Else
                        Log.Write("Ticket data not sent since SendBackTicketData is set to FALSE for this organization.")
                    End If

                Catch ex As Exception
                    Log.Write("Error in Send Ticket Data.  Message=" + ex.Message)
                End Try

                Return True

            End Function

            ''' <summary>
            ''' Updates information about a company (Organization) in TeamSupport
            ''' </summary>
            ''' <param name="company">information about the company</param>
            ''' <param name="ParentOrgID">the parent Organization</param>
            ''' <remarks></remarks>
            Protected Sub UpdateOrgInfo(ByVal company As CompanyData, ByVal ParentOrgID As String)
                If Processor.IsStopped Then
                    Return
                End If

                Dim findCompany As New Organizations(User)
                Dim thisCompany As Organization
                Dim companyInfoNeedsUpdate As Boolean = True

                'search for the crmlinkid = accountid in db to see if it already exists
                findCompany.LoadByCRMLinkID(company.AccountID, ParentOrgID)
                Dim logMessage As String = String.Empty

                If findCompany.Count > 0 Then
                    thisCompany = findCompany(0)
                    'it exists, so update the name on the account if it has changed.
                    companyInfoNeedsUpdate = thisCompany.Name <> company.AccountName
                    thisCompany.Name = company.AccountName
                    logMessage = "Found by accountId"
                Else
                    'look for parentid = parentorgid and name = accountname, and use that
                    findCompany.LoadByParentID(ParentOrgID, False)
                    If findCompany.FindByName(company.AccountName) IsNot Nothing AndAlso CRMLinkRow.MatchAccountsByName Then
                        thisCompany = findCompany.FindByName(company.AccountName)
                        companyInfoNeedsUpdate = thisCompany.CRMLinkID <> company.AccountID
                        'update accountid
                        thisCompany.CRMLinkID = company.AccountID
                        logMessage = "Found by Name"
                    ElseIf ParentOrgID <> "1078" Then
                        'if still not found, add new
                        Dim crmlinkOrg As New Organizations(User)
                        crmlinkOrg.LoadByOrganizationID(ParentOrgID)
                        Dim isAdvancedPortal As Boolean = crmlinkOrg(0).IsAdvancedPortal

                        thisCompany = (New Organizations(User)).AddNewOrganization()
                        thisCompany.ParentID = ParentOrgID
                        thisCompany.Name = company.AccountName
                        thisCompany.CRMLinkID = company.AccountID
                        thisCompany.HasPortalAccess = isAdvancedPortal AndAlso CRMLinkRow.AllowPortalAccess
                        thisCompany.IsActive = True
                        thisCompany.SlaLevelID = CRMLinkRow.DefaultSlaLevelID
                        logMessage = "Added a new account"
                    Else
                        Log.Write(String.Format("Company {0} {1} was not added in our 1078. ", company.AccountName, company.AccountID))
                        Return
                    End If
                End If

                If companyInfoNeedsUpdate Then
                    thisCompany.Collection.Save()
                    Log.Write(String.Format("{0}: {1} OrgId:{2} (AccountId: {3}). ", logMessage, company.AccountName, thisCompany.OrganizationID, company.AccountID))
                End If

                logMessage = String.Empty
                Dim findAddress As New Addresses(User)
                Dim thisAddress As Address
                Dim addressNeedsUpdate As Boolean = False
                findAddress.LoadByID(thisCompany.OrganizationID, ReferenceType.Organizations)

                If findAddress.Count > 0 Then
                    thisAddress = findAddress(0)
                Else
                    thisAddress = (New Addresses(User)).AddNewAddress()
                    thisAddress.RefID = thisCompany.OrganizationID
                    thisAddress.RefType = ReferenceType.Organizations
                    thisAddress.Collection.Save()
                    logMessage = "added"
                End If

                With thisAddress
                    If .Addr1 <> company.Street OrElse
                         .Addr2 <> company.Street2 OrElse
                         .City <> company.City OrElse
                         .State <> company.State OrElse
                         .Zip <> company.Zip OrElse
                         .Country <> company.Country Then
                        addressNeedsUpdate = True
                    End If

                    .Addr1 = company.Street
                    .Addr2 = company.Street2
                    .City = company.City
                    .State = company.State
                    .Zip = company.Zip
                    .Country = company.Country

                    If addressNeedsUpdate Then
                        .Collection.Save()
                        Log.Write(String.Format("Address information {0}.", If(String.IsNullOrEmpty(logMessage), "updated", logMessage)))
                    End If
                End With

                Dim phoneTypes As New PhoneTypes(User)
                phoneTypes.LoadAllPositions(ParentOrgID)

                Dim CRMPhoneType As PhoneType = Nothing
                'We use the corresponding CRM ("phone" or "work") phone type.
                Select Case Type
                    Case IntegrationType.Batchbook, IntegrationType.SalesForce, IntegrationType.ZohoCRM
                        CRMPhoneType = phoneTypes.FindByName("Phone")
                        If CRMPhoneType Is Nothing Then
                            CRMPhoneType = AddPhoneType("Phone", phoneTypes.Count, ParentOrgID)
                            phoneTypes.LoadAllPositions(ParentOrgID)
                        End If
                    Case IntegrationType.Highrise
                        CRMPhoneType = phoneTypes.FindByName("Work")
                        If CRMPhoneType Is Nothing Then
                            CRMPhoneType = AddPhoneType("Work", phoneTypes.Count, ParentOrgID)
                            phoneTypes.LoadAllPositions(ParentOrgID)
                        End If
                End Select

                Dim thisPhone As PhoneNumber = Nothing
                Dim findPhone As New PhoneNumbers(User)
                findPhone.LoadByID(thisCompany.OrganizationID, ReferenceType.Organizations)

                If findPhone.Count > 0 Then
                    For Each phone As PhoneNumber In findPhone
                        'The previous version did not added type and this version uses the CRMPhoneType
                        If phone.PhoneTypeID Is Nothing OrElse (CRMPhoneType IsNot Nothing AndAlso phone.PhoneTypeID = CRMPhoneType.PhoneTypeID) Then
                            thisPhone = phone
                            Exit For
                        End If
                    Next
                End If

                logMessage = String.Empty

                If company.Phone Is Nothing OrElse company.Phone = String.Empty Then
                    If thisPhone IsNot Nothing Then
                        thisPhone.Collection.DeleteFromDB(thisPhone.PhoneID)
                        Log.Write("Account phone number deleted.")
                    End If
                Else
                    logMessage = "updated"

                    If thisPhone Is Nothing Then
                        thisPhone = (New PhoneNumbers(User)).AddNewPhoneNumber()
                        logMessage = "added"
                    End If

                    Dim phoneNeedsUpdate As Boolean = False

                    With thisPhone
                        If .Number <> company.Phone OrElse
                            .RefType <> ReferenceType.Organizations OrElse
                            .RefID <> thisCompany.OrganizationID Then
                            phoneNeedsUpdate = True
                        End If

                        .Number = company.Phone
                        .RefType = ReferenceType.Organizations
                        .RefID = thisCompany.OrganizationID

                        If CRMPhoneType IsNot Nothing Then
                            If Not phoneNeedsUpdate Then
                                phoneNeedsUpdate = If(.PhoneTypeID Is Nothing OrElse .PhoneTypeID <> CRMPhoneType.PhoneTypeID, True, False)
                            End If

                            .PhoneTypeID = CRMPhoneType.PhoneTypeID
                        End If

                        If phoneNeedsUpdate Then
                            .Collection.Save()
                            Log.Write(String.Format("Account phone number {0}.", logMessage))
                        End If
                    End With
                End If

                logMessage = String.Empty
                Dim faxType As PhoneType = phoneTypes.FindByName("Fax")

                If faxType Is Nothing Then
                    faxType = AddPhoneType("Fax", phoneTypes.Count, ParentOrgID)
                    phoneTypes.LoadAllPositions(ParentOrgID)
                End If

                Dim thisFax As PhoneNumber = findPhone.FindByPhoneTypeID(faxType.PhoneTypeID)
                Dim faxNeedsUpdate As Boolean = False

                If company.Fax Is Nothing OrElse company.Fax = String.Empty Then
                    If thisFax IsNot Nothing Then
                        thisFax.Collection.DeleteFromDB(thisFax.PhoneID)
                        Log.Write("Account Fax number deleted.")
                    End If
                Else
                    logMessage = "updated"
                    If thisFax Is Nothing Then
                        thisFax = (New PhoneNumbers(User)).AddNewPhoneNumber()
                        logMessage = "added"
                    End If

                    With thisFax
                        If .Number <> company.Fax OrElse
                          .RefType <> ReferenceType.Organizations OrElse
                          .RefID <> thisCompany.OrganizationID OrElse
                          .PhoneTypeID Is Nothing OrElse
                          .PhoneTypeID = faxType.PhoneTypeID Then
                            faxNeedsUpdate = True
                        End If

                        .Number = company.Fax
                        .RefType = ReferenceType.Organizations
                        .RefID = thisCompany.OrganizationID
                        .PhoneTypeID = faxType.PhoneTypeID

                        If faxNeedsUpdate Then
                            .Collection.Save()
                            Log.Write(String.Format("Account fax number {0}.", logMessage))
                        End If
                    End With
                End If
            End Sub

            Private Function AddPhoneType(ByVal typeName As String, ByVal position As Integer, ByVal parentOrgId As String) As PhoneType
                Dim phoneTypes As PhoneTypes = New PhoneTypes(User)
                Dim result As PhoneType = phoneTypes.AddNewPhoneType()

                result.Name = typeName
                result.Description = typeName
                result.Position = position
                result.OrganizationID = parentOrgId

                phoneTypes.Save()
                Log.Write(String.Format("PhoneType {0} added", typeName))

                Return result
            End Function

            ''' <summary>
            ''' Updates information about a customer (User) in TeamSupport
            ''' </summary>
            ''' <param name="person">information about the customer to update</param>
            ''' <param name="companyID">the CRM-specific ID of the company to which the customer belongs</param>
            ''' <param name="ParentOrgID">the parent Organization</param>
            ''' <remarks></remarks>
            Protected Sub UpdateContactInfo(ByVal person As EmployeeData, ByVal companyID As String, ByVal ParentOrgID As String)
                ParentOrgID = CRMLinkRow.OrganizationID

                If Processor.IsStopped Then
                    Log.Write("Processor is stopped")
                    Return
                End If

                If person.Email = "" Then
                    Dim noEmailMessage As String = String.Format("Contact {0} {1} does not have email address therefore is not added.", person.FirstName, person.LastName)
                    Log.Write(noEmailMessage)
                    Throw New Exception(noEmailMessage)
                End If

                Dim wasUpdated As Boolean = False
                Log.Write(String.Format("Adding/updating contact information for {0} ({1},{2}).", person.Email, person.LastName, person.FirstName))

                Dim findCompany As New Organizations(User)
                'make sure the company already exists
                findCompany.LoadByCRMLinkID(companyID, ParentOrgID)

                If findCompany.Count > 0 Then
                    Dim thisCompany As Organization = findCompany(0)
                    Dim findUser As New Users(User)
                    Dim thisUser As User
                    Dim logMessage As String = "updated"

                    findUser.LoadByOrganizationID(thisCompany.OrganizationID, False)
                    'First Condition uses SalesforceID, prevents duplicate contacts being created when the email address is updated in SalesForce
                    If person.SalesForceID IsNot Nothing And findUser.FindBySalesForceID(person.SalesForceID) IsNot Nothing Then
                        thisUser = findUser.FindBySalesForceID(person.SalesForceID)
                    ElseIf findUser.FindByEmail(person.Email) IsNot Nothing Then
                        thisUser = findUser.FindByEmail(person.Email)
                    Else
                        Dim pw = DataUtils.GenerateRandomPassword()
                        Dim crmlinkOrg As New Organizations(User)
                        crmlinkOrg.LoadByOrganizationID(ParentOrgID)
                        Dim isAdvancedPortal As Boolean = crmlinkOrg(0).IsAdvancedPortal

                        'add the contact
                        thisUser = (New Users(User)).AddNewUser()
                        thisUser.OrganizationID = thisCompany.OrganizationID
                        thisUser.IsActive = True
                        thisUser.IsPasswordExpired = True
                        thisUser.IsPortalUser = isAdvancedPortal AndAlso CRMLinkRow.AllowPortalAccess
                        thisUser.CryptedPassword = Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(pw, "MD5")
                        thisUser.Collection.Save()
                        Log.Write("Contact was added.")

                        If isAdvancedPortal AndAlso CRMLinkRow.AllowPortalAccess AndAlso CRMLinkRow.SendWelcomeEmail Then
                            EmailPosts.SendWelcomePortalUser(User, thisUser.UserID, pw)
                            Log.Write("EmailPost added for SendWelcomePortalUser")
                        End If
                    End If

                    Dim contactNeedsUpdate As Boolean = False

                    With thisUser
                        If .Email <> person.Email OrElse
                            .FirstName <> If(person.FirstName IsNot Nothing, person.FirstName, "") OrElse
                            .LastName <> If(person.LastName IsNot Nothing, person.LastName, "") OrElse
                            .Title <> If(person.Title IsNot Nothing, person.Title, "") OrElse
                            .MarkDeleted OrElse
                            .SalesForceID <> person.SalesForceID Then
                            contactNeedsUpdate = True
                        End If

                        .Email = person.Email
                        .FirstName = If(person.FirstName IsNot Nothing, person.FirstName, "")
                        .LastName = person.LastName
                        .Title = person.Title
                        .MarkDeleted = False
                        .SalesForceID = person.SalesForceID

                        If contactNeedsUpdate Then
                            .Collection.Save()
                            Log.Write(String.Format("Contact was {0}.", logMessage))
                            wasUpdated = True
                        End If
                    End With

                    Dim thesePhoneTypes As New PhoneTypes(User)
                    thesePhoneTypes.LoadAllPositions(ParentOrgID)
                    Dim CRMPhoneType As PhoneType = Nothing

                    'We'll save the phone number using the corresponding CRM ("phone" or "work") phone type.
                    Select Case Type
                        Case IntegrationType.Batchbook, IntegrationType.SalesForce, IntegrationType.ZohoCRM
                            CRMPhoneType = thesePhoneTypes.FindByName("Phone")

                            If CRMPhoneType Is Nothing Then
                                CRMPhoneType = AddPhoneType("Phone", thesePhoneTypes.Count, ParentOrgID)
                                thesePhoneTypes.LoadAllPositions(ParentOrgID)
                            End If
                        Case IntegrationType.Highrise
                            CRMPhoneType = thesePhoneTypes.FindByName("Work")

                            If CRMPhoneType Is Nothing Then
                                CRMPhoneType = AddPhoneType("Work", thesePhoneTypes.Count, ParentOrgID)
                                thesePhoneTypes.LoadAllPositions(ParentOrgID)
                            End If
                    End Select

                    'The worktype is used regardless of the CRM phone type to be able to update the numbers processed by the previous version of this class.
                    Dim workType As PhoneType = thesePhoneTypes.FindByName("Work")

                    If workType Is Nothing Then
                        workType = AddPhoneType("Work", thesePhoneTypes.Count, ParentOrgID)
                        thesePhoneTypes.LoadAllPositions(ParentOrgID)
                    End If

                    'All the CRMs uses Mobile for this phone type.
                    Dim mobileType As PhoneType = thesePhoneTypes.FindByName("Mobile")

                    If mobileType Is Nothing Then
                        mobileType = AddPhoneType("Mobile", thesePhoneTypes.Count, ParentOrgID)
                        thesePhoneTypes.LoadAllPositions(ParentOrgID)
                    End If

                    'All the CRMs uses Fax for this phone type.
                    Dim faxType As PhoneType = thesePhoneTypes.FindByName("Fax")

                    If faxType Is Nothing Then
                        faxType = AddPhoneType("Fax", thesePhoneTypes.Count, ParentOrgID)
                        thesePhoneTypes.LoadAllPositions(ParentOrgID)
                    End If

                    '2. Preparation. Get existing numbers, if any, to update instead of add new.
                    Dim phone As PhoneNumber = Nothing
                    Dim mobilePhone As PhoneNumber = Nothing
                    Dim faxPhone As PhoneNumber = Nothing

                    'We'll proceed to find an existing number to update instead of incorrectly adding a new number everytime the contact get sync.
                    'If more than one phone number exist with the type we are looking for, we might end up updating the incorrect number.
                    'Unfortunately there is not an easy way to prevent this undesirable effect.
                    'An alternative is to wipe all numbers and add the ones comming from the CRM. This has been reviewed and rejected by RJ.
                    'Error chances are less if we update the first existing number with the type being updated than deleting existing numbers.
                    'Specially because we are bringing only the first number from the CRM.
                    Dim findPhone As New PhoneNumbers(User)
                    findPhone.LoadByID(thisUser.UserID, ReferenceType.Users)

                    If findPhone.Count > 0 Then
                        'The previous version assigned phone to the work type when the work type existed.
                        'Because chances are low that the Work Type was deleted by a user chances are big that this is the number we need to update.
                        phone = findPhone.FindByPhoneTypeID(workType.PhoneTypeID)

                        'When no work number exist, there is a small chance that the work type was deleted.
                        'In this case, for a long time we did not add the phone, recently we updated the code to add the number without type.
                        'To handle this very low chance we look for a number without type.
                        If phone Is Nothing Then
                            For Each existingNumber As PhoneNumber In findPhone
                                If existingNumber.PhoneTypeID Is Nothing Then
                                    phone = existingNumber
                                    Exit For
                                End If
                            Next
                        End If

                        'If no number have been found so far, maybe the current version already updated this contact.
                        'Therefore we look for a number with the CRM phone type.
                        If phone Is Nothing AndAlso CRMPhoneType IsNot Nothing Then
                            phone = findPhone.FindByPhoneTypeID(CRMPhoneType.PhoneTypeID)
                        End If

                        If mobileType IsNot Nothing Then
                            mobilePhone = findPhone.FindByPhoneTypeID(mobileType.PhoneTypeID)
                        End If

                        If faxType IsNot Nothing Then
                            faxPhone = findPhone.FindByPhoneTypeID(faxType.PhoneTypeID)
                        End If
                    End If

                    '3. Action. Add/Update.
                    If person.Phone Is Nothing OrElse person.Phone = String.Empty Then
                        If phone IsNot Nothing Then
                            phone.Collection.DeleteFromDB(phone.PhoneID)
                            Log.Write("Contact Phone was deleted.")
                        End If
                    Else
                        logMessage = "updated"

                        If phone Is Nothing Then
                            phone = (New PhoneNumbers(User).AddNewPhoneNumber())
                            logMessage = "added"
                        End If

                        Dim phoneNeedsUpdate As Boolean = False

                        With phone
                            If .Number <> person.Phone OrElse
                                .RefType <> ReferenceType.Users OrElse
                                .RefID <> thisUser.UserID Then
                                phoneNeedsUpdate = True
                            End If

                            .Number = person.Phone
                            .RefType = ReferenceType.Users
                            .RefID = thisUser.UserID

                            If CRMPhoneType IsNot Nothing Then
                                If Not phoneNeedsUpdate Then
                                    phoneNeedsUpdate = If(.PhoneTypeID Is Nothing OrElse .PhoneTypeID <> CRMPhoneType.PhoneTypeID, True, False)
                                End If

                                .PhoneTypeID = CRMPhoneType.PhoneTypeID
                            End If

                            'Custom mapping for Tenmast.
                            If Type = IntegrationType.ZohoCRM Then
                                If Not phoneNeedsUpdate Then
                                    phoneNeedsUpdate = .Extension <> person.Extension
                                End If

                                .Extension = person.Extension
                            End If

                            If phoneNeedsUpdate Then
                                .Collection.Save()
                                Log.Write(String.Format("Contact phone was {0}", logMessage))
                                wasUpdated = True
                            End If
                        End With
                    End If

                    If person.Cell Is Nothing OrElse person.Cell = String.Empty Then
                        If mobilePhone IsNot Nothing Then
                            mobilePhone.Collection.DeleteFromDB(mobilePhone.PhoneID)
                            Log.Write("Contact mobile was deleted.")
                        End If
                    Else
                        logMessage = "updated"

                        If mobilePhone Is Nothing Then
                            mobilePhone = (New PhoneNumbers(User).AddNewPhoneNumber())
                            logMessage = "added"
                        End If

                        Dim mobileNeedsUpdate As Boolean = False

                        With mobilePhone
                            If .Number <> person.Cell OrElse
                                .RefType <> ReferenceType.Users OrElse
                                .RefID <> thisUser.UserID OrElse
                                .PhoneTypeID Is Nothing OrElse
                                .PhoneTypeID <> mobileType.PhoneTypeID Then
                                mobileNeedsUpdate = True
                            End If

                            .Number = person.Cell
                            .RefType = ReferenceType.Users
                            .RefID = thisUser.UserID
                            .PhoneTypeID = mobileType.PhoneTypeID

                            If mobileNeedsUpdate Then
                                .Collection.Save()
                                Log.Write(String.Format("Contact mobile was {0}", logMessage))
                                wasUpdated = True
                            End If
                        End With
                    End If

                    If person.Fax Is Nothing OrElse person.Fax = String.Empty Then
                        If faxPhone IsNot Nothing Then
                            faxPhone.Collection.DeleteFromDB(faxPhone.PhoneID)
                            Log.Write("Contact fax was deleted.")
                        End If
                    Else
                        logMessage = "updated"

                        If faxPhone Is Nothing Then
                            faxPhone = (New PhoneNumbers(User).AddNewPhoneNumber())
                            logMessage = "added"
                        End If

                        Dim faxNeedsUpdate As Boolean = False

                        With faxPhone
                            If .Number <> person.Fax OrElse
                                .RefType <> ReferenceType.Users OrElse
                                .RefID <> thisUser.UserID OrElse
                                .PhoneTypeID Is Nothing OrElse
                                .PhoneTypeID <> faxType.PhoneTypeID Then
                                faxNeedsUpdate = True
                            End If

                            .Number = person.Fax
                            .RefType = ReferenceType.Users
                            .RefID = thisUser.UserID
                            .PhoneTypeID = faxType.PhoneTypeID

                            If faxNeedsUpdate Then
                                .Collection.Save()
                                Log.Write(String.Format("Contact fax was {0}", logMessage))
                                wasUpdated = True
                            End If
                        End With
                    End If
                End If

                If (Not wasUpdated) Then
                    Log.Write("Nothing needed to be updated.")
                End If
            End Sub

            ''' <summary>
            ''' Updates information about a OrganizationProduct in TeamSupport
            ''' </summary>
            ''' <param name="person">information about the customer to update</param>
            ''' <param name="companyID">the CRM-specific ID of the company to which the customer belongs</param>
            ''' <param name="ParentOrgID">the parent Organization</param>
            ''' <remarks></remarks>
            Protected Sub UpdateProductInfo(ByVal product As ProductData, ByVal companyID As String, ByVal ParentOrgID As String)
                If Processor.IsStopped Then
                    Return
                End If

                If product.Name = "" Then
                    'we don't add products with no name
                    Return
                End If

                Log.Write(String.Format("Adding product information for {0}.", product.Name))

                Dim findCompany As New Organizations(User)

                'make sure the company already exists
                findCompany.LoadByCRMLinkID(companyID, ParentOrgID)
                If findCompany.Count > 0 Then
                    Dim thisCompany As Organization = findCompany(0)

                    Dim products As Products = New Products(User)
                    products.LoadByProductName(ParentOrgID, product.Name)
                    Dim existingProduct As Product = Nothing
                    If products.Count > 0 Then
                        existingProduct = products(0)
                    Else
                        products = New Products(User)
                        products.AddNewProduct()
                        products(0).Name = product.Name
                        products(0).OrganizationID = ParentOrgID
                        products.Save()
                        existingProduct = products(0)
                        Log.Write(String.Format("Added product {0} in organization.", product.Name))
                    End If

                    Dim organizationProducts As New OrganizationProducts(User)
                    organizationProducts.LoadByOrganizationAndProductID(thisCompany.OrganizationID, existingProduct.ProductID)
                    If organizationProducts.Count = 0 Then
                        organizationProducts = New OrganizationProducts(User)
                        organizationProducts.AddNewOrganizationProduct()
                        organizationProducts(0).OrganizationID = thisCompany.OrganizationID
                        organizationProducts(0).ProductID = existingProduct.ProductID
                        organizationProducts.Save()
                        Log.Write(String.Format("Added product {0} in customer.", product.Name))
                    End If
                End If
            End Sub

            'helper method, should be moved to WebHelpers.vb
            Protected Function GetXML(ByVal Address As Uri) As XmlDocument
                Return GetXML(Address, Nothing)
            End Function

            'helper method, should be moved to WebHelpers.vb
            Protected Function GetXML(ByVal Address As Uri, ByVal Key As NetworkCredential) As XmlDocument
                Dim returnXML As XmlDocument = Nothing

                If Address IsNot Nothing Then
                    Dim request As HttpWebRequest = WebRequest.Create(Address)
                    request.Credentials = Key
                    request.Method = "GET"
                    request.KeepAlive = False
                    request.UserAgent = Client
                    request.Timeout = 7000

                    Try
                        Using response As HttpWebResponse = request.GetResponse()

                            If request.HaveResponse AndAlso response IsNot Nothing Then
                                Using reader As New StreamReader(response.GetResponseStream())

                                    returnXML = New XmlDocument()
                                    returnXML.XmlResolver = Nothing
                                    returnXML.LoadXml(reader.ReadToEnd())
                                End Using
                            End If

                        End Using
                    Catch ex As WebException
                        Log.Write("Error contacting " & Address.ToString() & ": " & ex.Message)
                        SyncError = True
                    End Try

                End If
                Return returnXML
            End Function

            'helper method, should be moved to WebHelpers.vb
            Protected Function GetHTTPData(ByVal key As NetworkCredential, ByVal address As Uri) As String
                If address IsNot Nothing Then
                    Dim request As HttpWebRequest = WebRequest.Create(address)
                    If key IsNot Nothing Then
                        request.Credentials = key
                    End If

                    request.Method = "POST"
                    request.KeepAlive = False
                    request.UserAgent = Client
                    request.Timeout = 7000
                    request.ContentLength = 0

                    Try
                        Using response As HttpWebResponse = request.GetResponse()
                            If request.HaveResponse AndAlso response IsNot Nothing Then
                                Using reader As New StreamReader(response.GetResponseStream())
                                    Return reader.ReadToEnd()
                                End Using
                            End If

                        End Using
                    Catch ex As WebException
                        Log.Write("Error contacting " & address.ToString() & ": " & ex.Message)
                    End Try

                End If

                Return Nothing
            End Function

            'helper method, should be moved to WebHelpers.vb
            Protected Function PostXML(ByVal Key As NetworkCredential, ByVal Address As Uri, ByVal Content As String) As HttpStatusCode
                Dim returnStatus As HttpStatusCode = Nothing

                If Address IsNot Nothing And Content <> "" Then
                    Dim byteData = UTF8Encoding.UTF8.GetBytes(Content)

                    Dim request As HttpWebRequest = WebRequest.Create(Address)
                    request.Credentials = Key
                    request.Method = "POST"
                    request.ContentType = "application/xml"
                    request.UserAgent = Client
                    request.ContentLength = byteData.Length

                    Using postStream As Stream = request.GetRequestStream()
                        postStream.Write(byteData, 0, byteData.Length)
                    End Using

                    Using response As HttpWebResponse = request.GetResponse()
                        If request.HaveResponse AndAlso response IsNot Nothing Then
                            returnStatus = response.StatusCode
                        End If
                    End Using

                End If

                Return returnStatus
            End Function

            'helper method, should be moved to WebHelpers.vb
            Protected Function PostQueryString(ByVal key As NetworkCredential, ByVal Address As Uri, ByVal Content As String) As HttpStatusCode
                If Processor.IsStopped Then
                    Return Nothing
                End If

                Dim returnStatus As HttpStatusCode = Nothing

                If Address IsNot Nothing And Content <> "" Then
                    Dim byteData = UTF8Encoding.UTF8.GetBytes(Content)

                    Dim request As HttpWebRequest = WebRequest.Create(Address)

                    If key IsNot Nothing Then
                        request.Credentials = key
                    End If

                    request.Method = "POST"
                    request.ContentType = "application/x-www-form-urlencoded"
                    request.UserAgent = Client
                    request.ContentLength = byteData.Length
                    request.ReadWriteTimeout = 60 * 1000 'mailchimp sync may take awhile, give it a minute

                    Try
                        Using postStream As Stream = request.GetRequestStream()
                            postStream.Write(byteData, 0, byteData.Length)
                            postStream.Flush()
                        End Using

                        Using response As HttpWebResponse = request.GetResponse()
                            If request.HaveResponse AndAlso response IsNot Nothing Then
                                returnStatus = response.StatusCode

                                If returnStatus <> HttpStatusCode.OK Then
                                    Log.Write("Error posting query string: " & response.StatusDescription)
                                End If

                            End If
                        End Using
                    Catch ex As WebException
                        Log.Write("Error contacting " & Address.ToString() & ": " & ex.Message)
                        Log.Write(New StreamReader(ex.Response.GetResponseStream()).ReadToEnd())
                        Return Nothing
                    End Try

                End If

                Return returnStatus
            End Function

            ''' <summary>
            ''' Logs a result to the CRMLinkResults table (which can be viewed by the end user)
            ''' </summary>
            ''' <param name="ResultText">the message to log</param>
            ''' <remarks></remarks>
            Public Sub LogSyncResult(ByVal ResultText As String)
                LogSyncResult(ResultText, CRMLinkRow.OrganizationID, User)
            End Sub

            ''' <summary>
            ''' Logs a result to the CRMLinkResults table (which can be viewed by the end user)
            ''' </summary>
            ''' <param name="ResultText">the message to log</param>
            ''' <param name="OrgID">the Organization ID</param>
            ''' <param name="User">the login user to log the message as</param>
            ''' <remarks></remarks>
            Public Shared Sub LogSyncResult(ByVal ResultText As String, ByVal OrgID As String, ByVal User As LoginUser)
                Dim result As CRMLinkResult
                result = (New CRMLinkResults(User)).AddNewCRMLinkResult()
                result.AttemptResult = ResultText
                result.OrganizationID = OrgID
                result.AttemptDateTime = Now.ToUniversalTime()
                result.Collection.Save()
            End Sub

            Public Shared Sub LogSynchedOrganization(ByVal CRMLinkTableID As Integer, ByVal OrganizationCRMID As String, ByVal User As LoginUser)
                Dim result As CRMLinkSynchedOrganization
                result = (New CRMLinkSynchedOrganizations(User)).AddNewCRMLinkSynchedOrganization()
                result.CRMLinkTableID = CRMLinkTableID
                result.OrganizationCRMID = OrganizationCRMID
                result.Collection.Save()
            End Sub

			''' <summary>
			''' updates the value of a custom field in TeamSupport
			''' </summary>
			''' <param name="customFieldID">the ID of the field to update</param>
			''' <param name="RefID">the ID of the object the field is linked to</param>
			''' <param name="Value">the value to set</param>
			''' <remarks></remarks>
			Protected Sub UpdateCustomValue(ByVal customFieldID As Integer, ByVal RefID As Integer, ByVal Value As String)
				Dim findCustom As New CustomValues(User)
				Dim thisCustom As CustomValue

				findCustom.LoadByFieldID(customFieldID, RefID)
				If findCustom.Count > 0 Then
					thisCustom = findCustom(0)

				Else
					thisCustom = (New CustomValues(User)).AddNewCustomValue()
					thisCustom.CustomFieldID = customFieldID
					thisCustom.RefID = RefID
				End If

				If thisCustom IsNot Nothing AndAlso thisCustom.Value <> Value Then
					thisCustom.Value = Value
					thisCustom.Collection.Save()
				End If
			End Sub

			Protected Sub SendPusherMessage(ByVal channelName As String, ByVal eventName As String, ByVal message As Object)
				Try
					Dim options As PusherOptions = New PusherOptions()
					options.Encrypted = True
					Dim pusherKey As String = SystemSettings.GetPusherKey()
					Dim pusherAppId As String = SystemSettings.GetPusherAppId()
					Dim pusherSecret As String = SystemSettings.GetPusherSecret()
					Dim Pusher As Pusher = New Pusher(pusherAppId, pusherKey, pusherSecret, options)
					Dim result = Pusher.Trigger(channelName, eventName, message)
				Catch ex As Exception
					Log.Write(String.Format("SendPusherMessage. {0}", ex.Message))
				End Try
			End Sub
		End Class

        'to be deprecated in favor of integrationException (below)
        Public Enum IntegrationError
            None
            Unknown
            InvalidLogin
        End Enum

        ''' <summary>
        ''' Contains information about an exception that occurred while syncing
        ''' </summary>
        ''' <remarks>use this object to be able to log meaningful error messages for the end user when an error occurs</remarks>
        Public Class IntegrationException
            Inherits Exception

            Public Sub New()
                MyBase.New()
            End Sub

            Public Sub New(ByVal message As String)
                MyBase.New(message)
            End Sub

            Public Sub New(ByVal message As String, ByVal inner As Exception)
                MyBase.New(message, inner)
            End Sub
        End Class

        <Serializable()>
        Public Class CompanyData
            Private _city As String
            Private _country As String
            Private _state As String
            Private _street As String
            Private _street2 As String
            Private _zip As String
            Private _phone As String
            Private _fax As String
            Private _accountId As String
            Private _accountName As String
            Private _billingCity As String
            Private _billingCountry As String
            Private _billingState As String
            Private _billingStreet As String
            Private _billingZip As String

            <XmlElement("ShippingCity")>
            Property City As String
                Get
                    Return _city
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 1024 Then
                            _city = value.Substring(0, 1024)
                        Else
                            _city = value
                        End If
                    End If
                End Set
            End Property

            <XmlElement("ShippingCountry")>
            Property Country As String
                Get
                    Return _country
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 250 Then
                            _country = value.Substring(0, 250)
                        Else
                            _country = value
                        End If
                    End If
                End Set
            End Property

            <XmlElement("ShippingState")>
            Property State As String
                Get
                    Return _state
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 50 Then
                            _state = value.Substring(0, 50)
                        Else
                            _state = value
                        End If
                    End If
                End Set
            End Property

            <XmlElement("ShippingStreet")>
            Property Street As String
                Get
                    Return _street
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 1024 Then
                            _street = value.Substring(0, 1024)
                        Else
                            _street = value
                        End If

                    End If
                End Set
            End Property

            Property Street2 As String
                Get
                    Return _street2
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 1024 Then
                            _street2 = value.Substring(0, 1024)
                        Else
                            _street2 = value
                        End If
                    End If
                End Set
            End Property

            <XmlElement("ShippingPostalCode")>
            Property Zip As String
                Get
                    Return _zip
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 30 Then
                            _zip = value.Substring(0, 30)
                        Else
                            _zip = value
                        End If
                    End If
                End Set
            End Property

            <XmlElement("Phone")>
            Property Phone As String
                Get
                    Return _phone
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 50 Then
                            _phone = value.Substring(0, 50)
                        Else
                            _phone = value
                        End If
                    End If
                End Set
            End Property

            <XmlElement("Fax")>
            Property Fax As String
                Get
                    Return _fax
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 50 Then
                            _fax = value.Substring(0, 50)
                        Else
                            _fax = value
                        End If
                    End If
                End Set
            End Property

            <XmlElement("Id")>
            Property AccountID As String
                Get
                    Return _accountId
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 100 Then
                            _accountId = value.Substring(0, 100)
                        Else
                            _accountId = value
                        End If
                    End If
                End Set
            End Property

            <XmlElement("Name")>
            Property AccountName As String
                Get
                    Return _accountName
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 255 Then
                            _accountName = value.Substring(0, 255)
                        Else
                            _accountName = value
                        End If
                    End If
                End Set
            End Property

            <XmlElement("BillingCity")>
            Property BillingCity As String
                Get
                    Return _billingCity
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 1024 Then
                            _city = value.Substring(0, 1024)
                        Else
                            _city = value
                        End If
                    End If
                End Set
            End Property

            <XmlElement("BillingCountry")>
            Property BillingCountry As String
                Get
                    Return _billingCountry
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 250 Then
                            _billingCountry = value.Substring(0, 250)
                        Else
                            _billingCountry = value
                        End If
                    End If
                End Set
            End Property

            <XmlElement("BillingState")>
            Property BillingState As String
                Get
                    Return _billingState
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 50 Then
                            _billingState = value.Substring(0, 50)
                        Else
                            _billingState = value
                        End If
                    End If
                End Set
            End Property

            <XmlElement("BillingStreet")>
            Property BillingStreet As String
                Get
                    Return _billingStreet
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 1024 Then
                            _billingStreet = value.Substring(0, 1024)
                        Else
                            _billingStreet = value
                        End If

                    End If
                End Set
            End Property

            <XmlElement("BillingPostalCode")>
            Property BillingZip As String
                Get
                    Return _billingZip
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 30 Then
                            _billingZip = value.Substring(0, 30)
                        Else
                            _billingZip = value
                        End If
                    End If
                End Set
            End Property

            Public Overrides Function Equals(ByVal obj As Object) As Boolean
                If Not TypeOf (obj) Is CompanyData Then
                    Return False
                End If

                Dim compareObj As CompanyData = CType(obj, CompanyData)
                If compareObj.City = Me.City And compareObj.Country = Me.Country And compareObj.State = Me.State And compareObj.Street = Me.Street And compareObj.Zip = Me.Zip _
                        And compareObj.Phone = Me.Phone And compareObj.AccountID = Me.AccountID And compareObj.AccountName = Me.AccountName Then
                    Return True
                Else
                    Return False
                End If
            End Function
        End Class

        Public Class EmployeeData
            Private _firstName As String
            Private _lastName As String
            Private _title As String
            Private _email As String
            Private _phone As String
            Private _extension As String
            Private _cell As String
            Private _fax As String
            Private _salesForceID As String
            Private _zohoId As String
            Private _highriseID As String
            Private _hubSpotvid As String
            Private _hubSpotId As String


            Property FirstName As String
                Get
                    Return _firstName
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 100 Then
                            _firstName = value.Substring(0, 100)
                        Else
                            _firstName = value
                        End If
                    End If
                End Set
            End Property

            Property LastName As String
                Get
                    Return _lastName
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 100 Then
                            _lastName = value.Substring(0, 100)
                        Else
                            _lastName = value
                        End If
                    End If
                End Set
            End Property

            Property Title As String
                Get
                    Return _title
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 100 Then
                            _title = value.Substring(0, 100)
                        Else
                            _title = value
                        End If
                    End If
                End Set
            End Property

            Property Email As String
                Get
                    Return _email
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 1024 Then
                            _email = value.Substring(0, 1024)
                        Else
                            _email = value
                        End If
                    End If
                End Set
            End Property

            Property Phone As String
                Get
                    Return _phone
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 50 Then
                            _phone = value.Substring(0, 50)
                        Else
                            _phone = value
                        End If
                    End If
                End Set
            End Property

            Property Extension As String
                Get
                    Return _extension
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 50 Then
                            _extension = value.Substring(0, 50)
                        Else
                            _extension = value
                        End If
                    End If
                End Set
            End Property

            Property Cell As String
                Get
                    Return _cell
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 50 Then
                            _cell = value.Substring(0, 50)
                        Else
                            _cell = value
                        End If
                    End If
                End Set
            End Property

            Property Fax As String
                Get
                    Return _fax
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 50 Then
                            _fax = value.Substring(0, 50)
                        Else
                            _fax = value
                        End If
                    End If
                End Set
            End Property

            Property SalesForceID As String
                Get
                    Return _salesForceID
                End Get
                Set(ByVal value As String)
                    _salesForceID = value
                End Set
            End Property

            Property ZohoID As String
                Get
                    Return _zohoId
                End Get
                Set(ByVal value As String)
                    _zohoId = value
                End Set
            End Property

            Property HighriseID As String
                Get
                    Return _highriseID
                End Get
                Set(ByVal value As String)
                    _highriseID = value
                End Set
            End Property

            Property HubSpotVid As String
                Get
                    Return _hubSpotvid
                End Get
                Set(ByVal value As String)
                    _hubSpotvid = value
                End Set
            End Property

            Property HubSpotId As String
                Get
                    Return _hubSpotId
                End Get
                Set(ByVal value As String)
                    _hubSpotId = value
                End Set
            End Property
        End Class

        Public Class ProductData
            Private _name As String
            Private _expirationDate As DateTime
            Private _createdTime As DateTime

            Property Name As String
                Get
                    Return _name
                End Get
                Set(ByVal value As String)
                    If value IsNot Nothing Then
                        If value.Length > 255 Then
                            _name = value.Substring(0, 255)
                        Else
                            _name = value
                        End If
                    End If
                End Set
            End Property

            Property ExpirationDate As DateTime
                Get
                    Return _expirationDate
                End Get
                Set(ByVal value As DateTime)
                    _expirationDate = value
                End Set
            End Property

            Property CreatedTime As DateTime
                Get
                    Return _createdTime
                End Get
                Set(ByVal value As DateTime)
                    _createdTime = value
                End Set
            End Property
        End Class

        ''' <summary>
        ''' Contains logging functionality for the integrations
        ''' </summary>
        ''' <remarks>This log is not available to the end user and can be used to log debugging messages as necessary</remarks>
        Public Class SyncLog
            Private LogPath As String
            Private FileName As String

            Public Sub New(ByVal Path As String, ByVal thisType As IntegrationType)
                LogPath = Path
                FileName = thisType.ToString() & " Debug File - " & Today.Month.ToString() & Today.Day.ToString() & Today.Year.ToString() & ".txt"

                If Not Directory.Exists(LogPath) Then
                    Directory.CreateDirectory(LogPath)
                End If
            End Sub

			Public Sub Write(ByVal Text As String)
				' the very first time we write to this file (once each day), prune old files
				If Not File.Exists(LogPath & "\" & FileName) Then
					For Each oldFileName As String In Directory.GetFiles(LogPath)
						If File.GetLastWriteTime(oldFileName).AddDays(7) < Today Then
							File.Delete(oldFileName)
						End If
					Next
				End If

				Try
					File.AppendAllText(LogPath & "\" & FileName, Now.ToLongTimeString() + ": " & Text & Environment.NewLine)
				Catch ex As IOException
					'unfortunately if the file cannot be accessed we'll just keep going. Need to revisit this.
				End Try

			End Sub

			Public ReadOnly Property Path() As String
				Get
					Return LogPath
				End Get
			End Property

			Public ReadOnly Property FileNamePath() As String
				Get
					Return FileName
				End Get
			End Property

		End Class
    End Namespace
End Namespace