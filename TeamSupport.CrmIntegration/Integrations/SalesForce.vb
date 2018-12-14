'changelog info at teamsupport.beanstalkapp.com/main

Imports System.Configuration
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Web.Services.Protocols
Imports sForce
Imports TeamSupport.Data
Imports Newtonsoft.Json

Namespace TeamSupport
    Namespace CrmIntegration

        Public Class SalesForce
            Inherits Integration

            Private Binding As SforceService
            Private AccountTypeString As String
            Private SiteAccountTypeString As String
            Private LastUpdateSFFormat As String

            Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor)
                MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, IntegrationType.SalesForce)

                Log.Write("Binding to SF object")
                Binding = New SforceService()

                If (Not Binding.Url Is Nothing) Then
                    Log.Write(String.Format("Url:{0}", Binding.Url.ToString()))
                End If

                'GridPoint-Sandbox	614460
                'gridpointtest		  614521
                'Axcient            674464
                'CiRBA              741865
                If CRMLinkRow.UseSandBoxServer Then
                    Binding.Url = "https://test.salesforce.com/services/Soap/u/16.0"
                End If

            End Sub

            Public Overrides Function PerformSync() As Boolean
                Dim Success As Boolean = True

                Success = SyncAccounts()

                If Success Then
                    Success = SendTicketsAsAccountComments()
                End If

                If Success Then
                    Success = PushTicketsAndPullCases()
                End If

                Return Success
            End Function

            '//vv DO NOT COMMIT this function
            Private Function Tests()
                Dim LoginReturn As String = login(Trim("hkriel@gwos.com"), Trim("dVuBXWt2bBCh"), Trim("x3ChyUtJxrMzWOF6HclTu9yIR"))
                Dim sfObjectName As String = "Account" 'Account,Contact,Case,Note,CaseComment
                Dim objDescription = Binding.describeSObject(sfObjectName)
                Dim query As String = "SELECT Id, CommentBody, CreatedById, CreatedDate, SystemModstamp, LastModifiedDate, LastModifiedById, IsDeleted FROM CaseComment WHERE ParentId = '5003800000ifiyyAAA'"
                Dim result As List(Of QueryResult) = Nothing
                Dim test As Integer
                result = GetQueriesResults(query, test)

                For i As Integer = 0 To result(0).records.Count - 1
                    For y As Integer = 0 To result(0).records(i).Any.Count - 1
                        Console.WriteLine(String.Format("name: {0}, value: {1}", result(0).records(i).Any(y).LocalName, result(0).records(i).Any(y).InnerText))
                    Next
                Next

                For Each apiField As Field In objDescription.fields
                    Console.WriteLine(String.Format("name: {0}, nameField: {1}, createable: {2}, updateable: {3}, type: {4}", apiField.name, apiField.nameField, apiField.createable, apiField.updateable, apiField.type))
                Next
            End Function

            Private Function SyncAccounts() As Boolean
                Dim SecurityToken As String = CRMLinkRow.SecurityToken1
                Dim CompanyName As String = CRMLinkRow.Username
                Dim Password As String = CRMLinkRow.Password
                Dim ParentOrgID As String = CRMLinkRow.OrganizationID
                Dim tagsToMatch As String = CRMLinkRow.TypeFieldMatch
                Dim selectFields As List(Of String) = New List(Of String)
                Dim fieldsList As List(Of String) = New List(Of String)

                Log.Write("Attempting to log in")

                Dim LoginReturn As String = login(Trim(CompanyName), Trim(Password), Trim(SecurityToken))

                If LoginReturn = "OK" Then
                    Log.Write("Logged in OK")

                    'The results will be placed in a list of objects named queryResults
                    'This list has a limit (BatchSize: default value 500 can be set up to 2,000 top) 
                    'in case than more companies than the limit are included in the results
                    'we use a list of lists of objects (list of queryResults) 
                    'and a GetQueriesResults() method that will make as many calls to SalesForce as necessary to populate it.
                    Dim queriesResults As List(Of QueryResult) = Nothing
                    Dim numberOfCompaniesInQueriesResults As Integer = 0
                    Dim TempTime As New DateTime(1900, 1, 1)
                    Binding.QueryOptionsValue = New QueryOptions()
                    Binding.QueryOptionsValue.batchSize = 2000
                    Binding.QueryOptionsValue.batchSizeSpecified = True

                    If CRMLinkRow.LastLink IsNot Nothing Then
                        'push last update time back 1 hour to make sure we catch every change
                        TempTime = CRMLinkRow.LastLinkUtc.Value.AddHours(-1)
                    End If

                    'format for SF query for time is 2011-01-26T16:57:00.000Z  ('yyyy'-'MM'-'dd'T'HH': 'mm': 'ss.fffffff'Z' )
                    LastUpdateSFFormat = TempTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'")

                    'SalesForce returns the LastModifed time in the Local Timezone of the organization.  Therefore, we need to store the LastUpdate
                    ' time as UTC and convert it to the local organization's TZ so they both match.
                    '**If the TS org's TZ does not match SF's TZ then we're in a world of hurt.
                    Log.Write("LastUpdate (last time CRM Sync Ran, in the organization's local timezone) = " + CRMLinkRow.LastLink.ToString())

                    Try
                        Dim TypeString As String = ""
                        AccountTypeString = String.Empty
                        SiteAccountTypeString = String.Empty
                        tagsToMatch = SanitizeTagsToMach(tagsToMatch)

                        If String.IsNullOrEmpty(tagsToMatch) Then
                            Log.Write("Missing Account Type to Link To TeamSupport (TypeFieldMatch).")
                            SyncError = True
                        Else
                            Dim MatchArray As String() = Array.ConvertAll(tagsToMatch.Split(","), Function(p As String) p.Trim())

                            For z As Integer = 0 To MatchArray.Length - 1
                                If Not String.IsNullOrEmpty(MatchArray(z)) Then
                                    If z > 0 Then
                                        TypeString = TypeString + " or "
                                        AccountTypeString = AccountTypeString + " or "
                                        SiteAccountTypeString = SiteAccountTypeString + " or "
                                    End If

                                    Dim tagToMatch As String = MatchArray(z)

                                    If tagToMatch.ToLower() = "none" Then
                                        tagToMatch = String.Empty
                                    ElseIf tagToMatch.ToLower() = "all" Then
                                        TypeString = "all"
                                        AccountTypeString = "all"
                                        SiteAccountTypeString = "all"
                                        Exit For
                                    End If

                                    TypeString = TypeString + " type = '" + tagToMatch + "'"
                                    AccountTypeString = AccountTypeString + " Account.Type = '" + tagToMatch + "'"
                                    SiteAccountTypeString = SiteAccountTypeString + " Account__r.Type = '" + tagToMatch + "'"
                                End If
                            Next

                            Log.Write("TypeString = " + TypeString)

                            Dim hasAddress As Boolean = True
                            Dim hasFax As Boolean = False
                            Dim queriedShippingAddress As Boolean = False
                            Dim bindingRanOK As Boolean
                            Dim accountDescribeSObjectResult As DescribeSObjectResult = New DescribeSObjectResult()

                            Try
                                fieldsList = New List(Of String)(New String() {"Id", "Name", "type", "ShippingStreet", "ShippingCity", "ShippingState", "ShippingPostalCode", "ShippingCountry", "Phone", "LastModifiedDate", "SystemModStamp", "Fax", "BillingStreet", "BillingCity", "BillingState", "BillingPostalCode", "BillingCountry"})
                                selectFields = GetAvailableFieldsForQuerySelect(fieldsList, "Account", accountDescribeSObjectResult)
                                queriesResults = GetSalesForceQueryResults(selectFields, hasAddress, hasFax, queriedShippingAddress, numberOfCompaniesInQueriesResults, bindingRanOK, TypeString)
#Region "Old. Moved to its own function"
                                'Try
                                '	If (selectFields.Count > 0) Then
                                '		SFQuery = String.Format("select {0} from Account where SystemModStamp >= {1} and ({2})", String.Join(",", selectFields.ToArray()) , LastUpdateSFFormat, If(TypeString = "all", String.Empty, TypeString))
                                '		hasAddress = SFQuery.ToLower().Contains("shipping")
                                '		hasFax = SFQuery.ToLower().Contains("fax")
                                '		Log.Write("SF Query String = " + SFQuery)
                                '		queriesResults = GetQueriesResults(SFQuery, numberOfCompaniesInQueriesResults)
                                '	Else
                                '		'These try catch blocks are implemented because some companies do not have all the fields been queried.
                                '		'I am adding trying with and without Fax by not knowing if it is included or not in all companies.
                                '		'1st attempt: with Shipping address and fax.
                                '		SFQuery = "select ID, Name, type, ShippingStreet, ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry, Phone, LastModifiedDate, SystemModStamp, Fax from Account where SystemModStamp >= " + LastUpdateSFFormat + IIf(TypeString = "all", String.Empty, " and (" + TypeString + ")")
                                '		Log.Write("SF Query String = " + SFQuery)
                                '		queriesResults = GetQueriesResults(SFQuery, numberOfCompaniesInQueriesResults)
                                '		hasFax = True
                                '	End If

                                '	bindingRanOK = True											
                                '	queriedShippingAddress = True
                                'Catch ex As Exception
                                '	Try
                                '		'2nd attempt: with Shipping address and without fax.
                                '		SFQuery = "select ID, Name, type, ShippingStreet, ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry, Phone, LastModifiedDate, SystemModStamp from Account where SystemModStamp >= " + LastUpdateSFFormat + IIf(TypeString = "all", String.Empty, " and (" + TypeString + ")")
                                '		Log.Write("SF Query String = " + SFQuery)

                                '		queriesResults = GetQueriesResults(SFQuery, numberOfCompaniesInQueriesResults)
                                '		bindingRanOK = True
                                '		hasFax = False
                                '		queriedShippingAddress = True
                                '	Catch ex1 As Exception
                                '		Try
                                '			'3rd attempt: With Billing address and fax
                                '			Log.Write("Shipping Address information not found - Attempting to find Billing Address information instead.")
                                '			SFQuery = "select ID, Name, type, BillingStreet, BillingCity, BillingState, BillingPostalCode, BillingCountry, Phone, LastModifiedDate, SystemModStamp, Fax from Account where SystemModStamp >= " + LastUpdateSFFormat + IIf(TypeString = "all", String.Empty, " and (" + TypeString + ")") ' and " + TypeString
                                '			Log.Write("SF Query String = " + SFQuery)

                                '			queriesResults = GetQueriesResults(SFQuery, numberOfCompaniesInQueriesResults)
                                '			bindingRanOK = True
                                '			hasFax = True
                                '		Catch ex2 As Exception
                                '			Try
                                '				'4th attempt: With Billing address and without fax
                                '				Log.Write("Shipping Address information not found - Attempting to find Billing Address information instead.")
                                '				SFQuery = "select ID, Name, type, BillingStreet, BillingCity, BillingState, BillingPostalCode, BillingCountry, Phone, LastModifiedDate, SystemModStamp from Account where SystemModStamp >= " + LastUpdateSFFormat + IIf(TypeString = "all", String.Empty, " and (" + TypeString + ")") ' and " + TypeString
                                '				Log.Write("SF Query String = " + SFQuery)

                                '				queriesResults = GetQueriesResults(SFQuery, numberOfCompaniesInQueriesResults)
                                '				bindingRanOK = True
                                '				hasFax = False
                                '			Catch ex3 As Exception
                                '				Try
                                '					'5th attempt: Without address and with Fax
                                '					Log.Write("Billing Address information not found - Attempting to process company with no address info at all.")
                                '					SFQuery = "select ID, Name, type,Phone, LastModifiedDate, SystemModStamp, Fax from Account where SystemModStamp >= " + LastUpdateSFFormat + IIf(TypeString = "all", String.Empty, " and (" + TypeString + ")") ' and " + TypeString
                                '					Log.Write("SF Query String = " + SFQuery)

                                '					queriesResults = GetQueriesResults(SFQuery, numberOfCompaniesInQueriesResults)
                                '					hasAddress = False 'Set this to false so we can set local variables correctly
                                '					bindingRanOK = True
                                '					hasFax = True
                                '				Catch ex4 As Exception
                                '					Log.Write("Billing Address information not found - Attempting to process company with no address info at all.")
                                '					SFQuery = "select ID, Name, type,Phone, LastModifiedDate, SystemModStamp from Account where SystemModStamp >= " + LastUpdateSFFormat + IIf(TypeString = "all", String.Empty, " and (" + TypeString + ")") ' and " + TypeString
                                '					Log.Write("SF Query String = " + SFQuery)

                                '					queriesResults = GetQueriesResults(SFQuery, numberOfCompaniesInQueriesResults)
                                '					hasAddress = False 'Set this to false so we can set local variables correctly
                                '					bindingRanOK = True
                                '					hasFax = False
                                '				End Try
                                '			End Try
                                '		End Try
                                '	End Try
                                'End Try
#End Region
                            Catch ex As Exception
                                bindingRanOK = False
                                Log.Write("Error when attempting to bind the Query: " + ex.Message)
                                SyncError = True
                            End Try

                            Log.Write("Number of Companies in queries results = " + Convert.ToString(numberOfCompaniesInQueriesResults))

                            'Get the Contact describe object here so we don't do it for each contact we want to update in the GetContactInformation function
                            Dim contactDescribeSObjectResult As DescribeSObjectResult = New DescribeSObjectResult()
                            fieldsList = New List(Of String)(New String() {"email", "FirstName", "LastName", "Phone", "MobilePhone", "Title", "IsDeleted", "SystemModstamp", "ID", "Fax"})
                            selectFields = New List(Of String)
                            selectFields = GetAvailableFieldsForQuerySelect(fieldsList, "Contact", contactDescribeSObjectResult)

                            If (bindingRanOK) And (numberOfCompaniesInQueriesResults > 0) Then
                                Dim synchedOrganizations As New CRMLinkSynchedOrganizations(Me.User)
                                synchedOrganizations.LoadByCRMLinkTableID(CRMLinkRow.CRMLinkID)
                                'We now have a list of all accounts that have been modified in the last 3 hours and that match our account types. Let's update.

                                Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
                                crmLinkErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "in", "company")
                                Dim crmLinkError As CRMLinkError = Nothing

                                For Each qr As QueryResult In queriesResults
                                    Log.Write(String.Format("Begining For Each Loop to get companies. qr.records.length = {0}{1}Updating company information, qr.size = {2}",
                                                            qr.records.Length.ToString,
                                                            vbTab,
                                                            qr.size.ToString))

                                    For i As Integer = 0 To qr.records.Length - 1
                                        If Not synchedOrganizations.CheckIfExists(qr.records(i).Any(0).InnerText) Then
                                            Dim thisCompany As New CompanyData()
                                            Dim LastModifiedDateTime As DateTime
                                            Dim records As sObject() = qr.records
                                            Log.Write("In for loop iteration " + i.ToString)

                                            Try
                                                thisCompany = SerializeXmlToCompanyDataObject(records(i).Any, LastModifiedDateTime)

                                                If (String.IsNullOrEmpty(thisCompany.Street) AndAlso queriedShippingAddress) Then
                                                    'We are now pulling billing address from all accounts query
                                                    'Dim billingAddressWithFax As Boolean = False
                                                    'Dim billingAddress As sObject() = GetBillingAddress(thisCompany.AccountID, billingAddressWithFax, accountDescribeSObjectResult)

                                                    'If billingAddress IsNot Nothing Then
                                                    '    thisCompany.Street = billingAddress(0).Any(0).InnerText
                                                    '    thisCompany.City = billingAddress(0).Any(1).InnerText
                                                    '    thisCompany.State = billingAddress(0).Any(2).InnerText
                                                    '    thisCompany.Zip = billingAddress(0).Any(3).InnerText
                                                    '    thisCompany.Country = billingAddress(0).Any(4).InnerText
                                                    '    thisCompany.Phone = billingAddress(0).Any(5).InnerText

                                                    '    If billingAddressWithFax Then
                                                    '        thisCompany.Fax = billingAddress(0).Any(6).InnerText
                                                    '    End If
                                                    'End If
                                                    If (Not String.IsNullOrEmpty(thisCompany.BillingStreet)) Then
                                                        thisCompany.Street = thisCompany.BillingStreet
                                                        thisCompany.City = thisCompany.BillingCity
                                                        thisCompany.State = thisCompany.BillingState
                                                        thisCompany.Zip = thisCompany.BillingZip
                                                        thisCompany.Country = thisCompany.BillingCountry
                                                    End If
                                                End If
                                            Catch ex As Exception
#Region "Old. Hoping to not fall into this catch"
                                                thisCompany = New CompanyData()

                                                With thisCompany
                                                    .AccountID = records(i).Any(0).InnerText
                                                    .AccountName = records(i).Any(1).InnerText

                                                    If hasAddress Then
                                                        If records(i).Any(3).InnerText = String.Empty AndAlso queriedShippingAddress Then
                                                            Dim billingAddressWithFax As Boolean = False
                                                            Dim billingAddress As sObject() = GetBillingAddress(.AccountID, billingAddressWithFax, accountDescribeSObjectResult)
                                                            If billingAddress IsNot Nothing Then
                                                                .Street = billingAddress(0).Any(0).InnerText
                                                                .City = billingAddress(0).Any(1).InnerText
                                                                .State = billingAddress(0).Any(2).InnerText
                                                                .Zip = billingAddress(0).Any(3).InnerText
                                                                .Country = billingAddress(0).Any(4).InnerText
                                                                .Phone = billingAddress(0).Any(5).InnerText
                                                                If billingAddressWithFax Then
                                                                    .Fax = billingAddress(0).Any(6).InnerText
                                                                End If
                                                            End If
                                                        Else
                                                            .Street = records(i).Any(3).InnerText
                                                            .City = records(i).Any(4).InnerText
                                                            .State = records(i).Any(5).InnerText
                                                            .Zip = records(i).Any(6).InnerText
                                                            .Country = records(i).Any(7).InnerText
                                                            .Phone = records(i).Any(8).InnerText
                                                            If hasFax Then
                                                                .Fax = records(i).Any(11).InnerText
                                                            End If
                                                        End If

                                                        LastModifiedDateTime = Date.Parse(records(i).Any(9).InnerText)
                                                    Else
                                                        .Phone = records(i).Any(3).InnerText
                                                        If hasFax Then
                                                            .Fax = records(i).Any(6).InnerText
                                                        End If
                                                        LastModifiedDateTime = Date.Parse(records(i).Any(4).InnerText)

                                                    End If
                                                End With
#End Region
                                            End Try

                                            Log.Write("Company " & thisCompany.AccountName & " last modified on " & LastModifiedDateTime.ToString())
                                            crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(thisCompany.AccountID, String.Empty)

                                            Try
                                                UpdateOrgInfo(thisCompany, ParentOrgID)
                                                Log.Write("Completed adding/updating account information for company " + thisCompany.AccountName)

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
                                                    crmLinkError.ObjectID = thisCompany.AccountID
                                                    crmLinkError.ObjectData = JsonConvert.SerializeObject(thisCompany)
                                                    crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                                    crmLinkError.OperationType = "unknown"
                                                    newCrmLinkError.Save()
                                                Else
                                                    crmLinkError.ObjectData = JsonConvert.SerializeObject(thisCompany)
                                                    crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                                End If
                                            End Try

                                            'Let's force an update of contact information for this company
                                            'This is not redundant with the same function call below. See ticket 18202
                                            'I've verified the call to this method after the account iteration is pulling recently modified contacts regardless of the account date modified.
                                            'If a similar issue to 18202 happens again a fix must be found within that call instead of reinstating this call as it produces an excesive amount 
                                            'of SalesForce API calls.
                                            'GetContactInformation(selectFields, ParentOrgID, LastUpdateSFFormat, AccountTypeString, thisCompany.AccountID, True, contactDescribeSObjectResult)
                                            'Log.Write("Completed force update contact info for " + thisCompany.AccountName)
                                            LogSynchedOrganization(CRMLinkRow.CRMLinkID, thisCompany.AccountID, Me.User)
                                        Else
                                            Log.Write("Company (iteration " + i.ToString() + ") " + qr.records(i).Any(1).InnerText + " skipped because it was synched in previous run.")
                                        End If
                                    Next
                                Next

                                'check for existence of custom fields to sync
                                GetCustomFields("Account", TypeString, LastUpdateSFFormat, crmLinkErrors)
                                crmLinkErrors.Save()
                                Log.Write("All done updating company information.")
                            Else
                                Log.Write("**No matching record found!!")
                            End If

                            If bindingRanOK Then
                                'Breaking this out separately since they were not running if there were no companies that had changed.
                                'We updated all of the ACCOUNT information above
                                'Now let's update all of the contact information
                                GetContactInformation(selectFields, ParentOrgID, LastUpdateSFFormat, AccountTypeString, "0", False, contactDescribeSObjectResult)

                                'Code for Axceler to update their product and license information
                                If ParentOrgID = 305383 Then
                                    GetProductAndLicenseInfo(LastUpdateSFFormat)
                                End If
                            End If
                        End If
                    Catch ex As Exception
                        ErrorCode = IntegrationError.Unknown
                        Log.Write("**Error:  " + ex.Message)
                        Return False
                    End Try

                    'Added to see if it solved problem with 2+ SalesForce accounts being active.
                    Binding.logout()
                    Binding.logoutAsync()
                Else
                    If LoginReturn.ToLower() = "password expired" Or LoginReturn.ToLower().Contains("invalid_login") Then
                        ErrorCode = IntegrationError.InvalidLogin
                    End If

                    Log.Write("Login failed: " & LoginReturn)
                    SyncError = True
                End If

                Return Not SyncError
            End Function

            ''' <summary>
            ''' Sanitize the "Account Type to Link to TeamSupport" entry
            ''' </summary>
            ''' <param name="tagsToMatch">The value of "Account Type to Link to TeamSupport"</param>
            ''' <returns>Sanitized value. No double commas, no leading or trailing commas.</returns>
            Private Function SanitizeTagsToMach(ByVal tagsToMatch As String) As String
                tagsToMatch = tagsToMatch.Trim()

                Try
                    'Remove duplicated consecutive commas
                    While (tagsToMatch.IndexOf(",,") > 0)
                        tagsToMatch = tagsToMatch.Replace(",,", ",")
                    End While

                    'Check and remove if there is a comma by itsef at the beginning end of the string
                    If Not String.IsNullOrEmpty(tagsToMatch) AndAlso tagsToMatch.Substring(0, 1) = "," Then
                        tagsToMatch = tagsToMatch.Substring(1, tagsToMatch.Length - 1)
                    End If

                    If Not String.IsNullOrEmpty(tagsToMatch) AndAlso tagsToMatch.Substring(tagsToMatch.Length - 1) = "," Then
                        tagsToMatch = tagsToMatch.Substring(0, tagsToMatch.Length - 1)
                    End If
                Catch ex As Exception
                    Log.Write(String.Format("Error when attempting to sanitize: {0}{1}{2}", tagsToMatch.ToString(), Environment.NewLine, ex.Message))
                    tagsToMatch = CRMLinkRow.TypeFieldMatch.Trim()
                    Log.Write(String.Format("Using original entry trimmed: {0}", tagsToMatch.ToString()))
                End Try

                Return tagsToMatch
            End Function

            Private Function GetSalesForceQueryResults(ByRef selectFields As List(Of String),
                                                ByRef hasAddress As Boolean,
                                                ByRef hasFax As Boolean,
                                                ByRef queriedShippingAddress As Boolean,
                                                ByRef numberOfCompaniesInQueriesResults As Integer,
                                                ByRef bindingRanOK As Boolean,
                                                ByRef typeString As String) As List(Of QueryResult)
                Dim queriesResults As List(Of QueryResult) = Nothing
                Dim SFQuery As String = String.Empty

                Try
                    If (selectFields.Count > 0) Then
                        SFQuery = String.Format("select {0} from Account where SystemModStamp >= {1} and ({2})", String.Join(",", selectFields.ToArray()), LastUpdateSFFormat, If(typeString = "all", String.Empty, typeString))
                        hasAddress = SFQuery.ToLower().Contains("shipping")
                        hasFax = SFQuery.ToLower().Contains("fax")
                        queriesResults = GetQueriesResults(SFQuery, numberOfCompaniesInQueriesResults)
                    Else
                        'These try catch blocks are implemented because some companies do not have all the fields been queried.
                        'I am adding trying with and without Fax by not knowing if it is included or not in all companies.
                        '1st attempt: with Shipping address and fax.
                        SFQuery = "select ID, Name, type, ShippingStreet, ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry, Phone, LastModifiedDate, SystemModStamp, Fax from Account where SystemModStamp >= " + LastUpdateSFFormat + IIf(typeString = "all", String.Empty, " and (" + typeString + ")")
                        queriesResults = GetQueriesResults(SFQuery, numberOfCompaniesInQueriesResults)
                        hasFax = True
                    End If

                    bindingRanOK = True
                    queriedShippingAddress = True
                Catch ex As Exception
                    Try
                        '2nd attempt: with Shipping address and without fax.
                        SFQuery = "select ID, Name, type, ShippingStreet, ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry, Phone, LastModifiedDate, SystemModStamp from Account where SystemModStamp >= " + LastUpdateSFFormat + IIf(typeString = "all", String.Empty, " and (" + typeString + ")")
                        queriesResults = GetQueriesResults(SFQuery, numberOfCompaniesInQueriesResults)
                        bindingRanOK = True
                        hasFax = False
                        queriedShippingAddress = True
                    Catch ex1 As Exception
                        Try
                            '3rd attempt: With Billing address and fax
                            Log.Write("Shipping Address information not found - Attempting to find Billing Address information instead.")
                            SFQuery = "select ID, Name, type, BillingStreet, BillingCity, BillingState, BillingPostalCode, BillingCountry, Phone, LastModifiedDate, SystemModStamp, Fax from Account where SystemModStamp >= " + LastUpdateSFFormat + IIf(typeString = "all", String.Empty, " and (" + typeString + ")") ' and " + TypeString
                            queriesResults = GetQueriesResults(SFQuery, numberOfCompaniesInQueriesResults)
                            bindingRanOK = True
                            hasFax = True
                        Catch ex2 As Exception
                            Try
                                '4th attempt: With Billing address and without fax
                                Log.Write("Shipping Address information not found - Attempting to find Billing Address information instead.")
                                SFQuery = "select ID, Name, type, BillingStreet, BillingCity, BillingState, BillingPostalCode, BillingCountry, Phone, LastModifiedDate, SystemModStamp from Account where SystemModStamp >= " + LastUpdateSFFormat + IIf(typeString = "all", String.Empty, " and (" + typeString + ")") ' and " + TypeString
                                queriesResults = GetQueriesResults(SFQuery, numberOfCompaniesInQueriesResults)
                                bindingRanOK = True
                                hasFax = False
                            Catch ex3 As Exception
                                Try
                                    '5th attempt: Without address and with Fax
                                    Log.Write("Billing Address information not found - Attempting to process company with no address info at all.")
                                    SFQuery = "select ID, Name, type,Phone, LastModifiedDate, SystemModStamp, Fax from Account where SystemModStamp >= " + LastUpdateSFFormat + IIf(typeString = "all", String.Empty, " and (" + typeString + ")") ' and " + TypeString
                                    queriesResults = GetQueriesResults(SFQuery, numberOfCompaniesInQueriesResults)
                                    hasAddress = False 'Set this to false so we can set local variables correctly
                                    bindingRanOK = True
                                    hasFax = True
                                Catch ex4 As Exception
                                    Log.Write("Billing Address information not found - Attempting to process company with no address info at all.")
                                    SFQuery = "select ID, Name, type,Phone, LastModifiedDate, SystemModStamp from Account where SystemModStamp >= " + LastUpdateSFFormat + IIf(typeString = "all", String.Empty, " and (" + typeString + ")") ' and " + TypeString
                                    queriesResults = GetQueriesResults(SFQuery, numberOfCompaniesInQueriesResults)
                                    hasAddress = False 'Set this to false so we can set local variables correctly
                                    bindingRanOK = True
                                    hasFax = False
                                End Try
                            End Try
                        End Try
                    End Try
                Finally
                    Log.Write("SF Query String = " + SFQuery)
                End Try

                Return queriesResults
            End Function

            ''' <summary>
            ''' This funcion takes the desired fields of the sql for SalesForce and returns the list of available fields from it for the specified object.
            ''' Helps to avoid querying SF with non-existing fields and getting exceptions back.
            ''' </summary>
            ''' <param name="fieldsList">String list of fields that would like to get from SalesForce</param>
            ''' <param name="sfObjectType">String SalesForce object type: Account,Contact,Case,Note,CaseComment </param>
            ''' <returns>List of available fields of the specific SalesForce object for the fieldList received.</returns>
            Private Function GetAvailableFieldsForQuerySelect(ByVal fieldsList As List(Of String), ByVal sfObjectType As String, ByRef objDescription As DescribeSObjectResult) As List(Of String)
                Dim selectFields As List(Of String) = New List(Of String)
                Dim selectFieldsNotFound As List(Of String) = New List(Of String)

                Try
                    'objDescription = Binding.describeSObject(sfObjectType)
                    objDescription = DescribeObject(sfObjectType)
                    For Each field As String In fieldsList
                        Dim found As Boolean = objDescription.fields.Where(Function(x) x.name.ToLower() = field.ToLower()).Any()

                        If (found) Then
                            selectFields.Add(field)
                        Else
                            selectFieldsNotFound.Add(field)
                        End If
                    Next

                    If (selectFieldsNotFound.Count > 0) Then
                        Log.Write(String.Format("These fields were not found in the {0} sf object: {1}", sfObjectType, String.Join(",", selectFieldsNotFound.ToArray())))
                    End If

                    'For Each apiField As Field In objDescription.fields
                    '	'//vv Console.WriteLine(String.Format("name: {0}, nameField: {1}, createable: {2}, updateable: {3}, type: {4}", apiField.name, apiField.nameField, apiField.createable, apiField.updateable, apiField.type))
                    '	Dim fieldFound As String = fieldsList.Where(Function(x) x.ToLower() = apiField.name.ToLower()).FirstOrDefault()
                    '	If (Not fieldFound Is Nothing AndAlso Not String.IsNullOrEmpty(fieldFound)) Then
                    '		selectFields.Add(fieldFound)
                    '	End If
                    'Next
                Catch ex As Exception
                    Log.Write(String.Format("Error on Binding.describeSObject('Account'), using the old hardcoded list then. Exception: {0}", ex.Message))
                    selectFields.Clear()
                End Try

                Return selectFields
            End Function

            Private Function DescribeObject(ByRef objectType As String) As DescribeSObjectResult
                Dim result As DescribeSObjectResult

                Try
                    result = Binding.describeSObject(objectType)
                Catch ex As Exception
                    If (ex.Message.Contains("INVALID_SESSION_ID")) Then
                        Dim loginResult = login(Trim(CRMLinkRow.Username), Trim(CRMLinkRow.Password), Trim(CRMLinkRow.SecurityToken1))
                        result = Binding.describeSObject(objectType)
                    Else
                        Throw ex
                    End If
                End Try

                Return result
            End Function

            ''' <summary>
            ''' Serializes the SalesForce xml returned into the CompanyData object (Integration.vb)
            ''' </summary>
            ''' <param name="sfXmlElement">SalesForce xml element of the sObject</param>
            ''' <param name="LastModifiedDateTime">Reference to this field needed for logging purposes</param>
            ''' <returns>A CompanyData object</returns>
            Private Function SerializeXmlToCompanyDataObject(ByVal sfXmlElement As XmlElement(), ByRef LastModifiedDateTime As Date) As CompanyData
                Dim companyData As New CompanyData()
                Dim xml As XmlDocument = New XmlDocument()
                Dim xmlElementObject As XmlElement = xml.CreateElement("CompanyData")
                xml.AppendChild(xmlElementObject)

                For Each xmlElement As XmlElement In sfXmlElement
                    Dim xmlNode As XmlNode = xml.CreateNode(XmlNodeType.Element, xmlElement.Name, "")
                    xmlNode.InnerText = xmlElement.InnerText
                    xmlElementObject.AppendChild(xmlNode)

                    If (xmlElement.LocalName.ToLower() = "lastmodifieddate") Then
                        LastModifiedDateTime = Date.Parse(xmlElement.InnerText)
                    End If
                Next

                Dim reader As XmlReader = XmlReader.Create(New StringReader(xml.InnerXml))
                Dim xmlSerializer As New XmlSerializer(companyData.GetType)
                companyData = xmlSerializer.Deserialize(reader)

                Return companyData
            End Function

            Private Function GetQueriesResults(ByRef SFQuery As String, ByRef numberOfCompaniesInQueriesResults As Integer) As List(Of QueryResult)
                Dim result As List(Of QueryResult) = New List(Of QueryResult)

                Dim done As Boolean = False
                Dim itsFirstIteration As Boolean = True
                While Not done
                    Dim queryResult As QueryResult = Nothing

                    If itsFirstIteration Then
                        'queryResult = Binding.query(SFQuery)
                        queryResult = GetQueryResults(SFQuery)
                        itsFirstIteration = False
                    Else
                        'queryResult = Binding.queryMore(result.Item(result.Count - 1).queryLocator)
                        queryResult = GetQueryMoreResults(result.Item(result.Count - 1).queryLocator)
                    End If

                    done = queryResult.done
                    If Not queryResult.records Is Nothing Then
                        numberOfCompaniesInQueriesResults += queryResult.records.Length
                    End If

                    result.Add(queryResult)
                End While

                Return result
            End Function

            Private Function GetQueryResults(ByRef sfQuery As String) As QueryResult
                Dim result As QueryResult

                Try
                    result = Binding.query(sfQuery)
                Catch ex As Exception
                    If (ex.Message.Contains("INVALID_SESSION_ID")) Then
                        Dim loginResult = login(Trim(CRMLinkRow.Username), Trim(CRMLinkRow.Password), Trim(CRMLinkRow.SecurityToken1))
                        result = Binding.query(sfQuery)
                    Else
                        Throw ex
                    End If
                End Try

                Return result
            End Function

            Private Function GetQueryMoreResults(ByRef sfQueryLocator As String) As QueryResult
                Dim result As QueryResult

                Try
                    result = Binding.queryMore(sfQueryLocator)
                Catch ex As Exception
                    If (ex.Message.Contains("INVALID_SESSION_ID")) Then
                        Dim loginResult = login(Trim(CRMLinkRow.Username), Trim(CRMLinkRow.Password), Trim(CRMLinkRow.SecurityToken1))
                        result = Binding.queryMore(sfQueryLocator)
                    Else
                        Throw ex
                    End If
                End Try

                Return result
            End Function

            Private Function GetBillingAddress(ByVal organizationId As String, ByRef hasFax As Boolean, ByRef objDescription As DescribeSObjectResult) As sObject()
                Dim SFQuery As String = Nothing
                Dim qr As QueryResult = Nothing
                hasFax = objDescription.fields.Where(Function(x) x.name.ToLower() = "fax").Any()

                Try
                    Log.Write(String.Format("Attempting to find Billing Address {0} fax of OrganizationId: {1}", If(hasFax, "with", "without"), organizationId.ToString()))
                    SFQuery = String.Format("select BillingStreet, BillingCity, BillingState, BillingPostalCode, BillingCountry, Phone{0} from Account where ID = '{1}'", If(hasFax, ", Fax", ""), organizationId.ToString())
                    Log.Write("SF Query String = " + SFQuery)
                    'qr = Binding.query(SFQuery)
                    qr = GetQueryResults(SFQuery)
                    Log.Write("qr.size = " + qr.size.ToString)
                Catch ex As Exception
                    Log.Write("Error when attempting to bind the Query: " + ex.Message)
                End Try

                'Try
                '    'First try with Fax
                '    Log.Write("Attempting to find Billing Address with fax of OrganizationId: " & organizationId.ToString() & ".")
                '    SFQuery = "select BillingStreet, BillingCity, BillingState, BillingPostalCode, BillingCountry, Phone, Fax from Account where ID = '" + organizationId.ToString() + "'"
                '    Log.Write("SF Query String = " + SFQuery)

                '    qr = Binding.query(SFQuery)
                '    Log.Write("qr.size = " + qr.size.ToString)
                '    hasFax = True
                'Catch ex1 As Exception
                '    Try
                '        'Try without fax
                '        Log.Write("Attempting to find Billing Address without fax of OrganizationId: " & organizationId.ToString() & ".")
                '        SFQuery = "select BillingStreet, BillingCity, BillingState, BillingPostalCode, BillingCountry, Phone from Account where ID = '" + organizationId.ToString() + "'"
                '        Log.Write("SF Query String = " + SFQuery)

                '        qr = Binding.query(SFQuery)
                '        Log.Write("qr.size = " + qr.size.ToString)
                '        hasFax = False
                '    Catch ex2 As Exception
                '        Log.Write("Error when attempting to bind the Query: " + ex2.Message)                        
                '    End Try
                'End Try

                Dim result As sObject() = Nothing
                If qr IsNot Nothing AndAlso qr.size > 0 Then
                    result = qr.records
                End If

                Return result
            End Function

            Private Function SendTicketsAsAccountComments() As Boolean
                Dim Success As Boolean = True

                If login(Trim(CRMLinkRow.Username), Trim(CRMLinkRow.Password), Trim(CRMLinkRow.SecurityToken1)) = "OK" Then
                    Success = SendTicketData(AddressOf CreateNote)

                    Binding.logout()
                    Binding.logoutAsync()
                End If

                Return Success
            End Function

            Private Function login(ByVal username As String, ByVal password As String, ByVal securitytoken As String) As String
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 Or System.Net.SecurityProtocolType.Tls11 Or System.Net.SecurityProtocolType.Tls

                'Set the partner WSDL

                Dim co As New CallOptions()
                co.client = "Muroc/TSCom/" '**this is our unique TS address
                Binding.CallOptionsValue = co

                ' Timeout after a minute
                Binding.Timeout = 60000

                ' Try logging in
                Dim lr As LoginResult
                Try
                    lr = Binding.login(username, password + securitytoken)
                Catch e As SoapException
                    Return e.Message

                Catch e As Exception
                    Return e.Message
                End Try

                ' Check if the password has expired
                If lr.passwordExpired Then
                    Return "Password expired"
                End If

                '* Once the client application has logged in successfully, it will use
                '             * the results of the login call to reset the endpoint of the service
                '             * to the virtual server instance that is servicing your organization
                '             

                Binding.Url = lr.serverUrl

                '* The sample client application now has an instance of the SforceService
                '             * that is pointing to the correct endpoint. Next, the sample client
                '             * application sets a persistent SOAP header (to be included on all
                '             * subsequent calls that are made with SforceService) that contains the
                '             * valid sessionId for our login credentials. To do this, the sample
                '             * client application creates a new SessionHeader object and persist it to
                '             * the SforceService. Add the session ID returned from the login to the
                '             * session header
                '             

                Binding.SessionHeaderValue = New SessionHeader()
                Binding.SessionHeaderValue.sessionId = lr.sessionId

                ' Return true to indicate that we are logged in, pointed
                ' at the right URL and have our security token in place.
                Return "OK"
            End Function

            Private Sub GetContactInformation(ByVal selectFields As List(Of String), ByVal ParentOrgID As String, ByVal LastUpdate As String, ByVal TypeString As String, ByVal AccountIDToUpdate As String, ByVal ForceUpdate As Boolean, ByRef objDescription As DescribeSObjectResult)
                'This will get the contact information from SalesForce for a given account ID
                'If ForceUpdate is set them we will change the query so that we grab all contact information from the AccountID company (otherwise AccountID is not used)
                Log.Write(String.Format("Getting Contact Information... ForceUpdate={0}", ForceUpdate.ToString()))

                Dim qr As QueryResult = Nothing
                Dim LastModifiedDateTime As DateTime
                Binding.QueryOptionsValue = New QueryOptions()
                Binding.QueryOptionsValue.batchSize = 2000
                Binding.QueryOptionsValue.batchSizeSpecified = True

                Try
                    Dim hasFax As Boolean = objDescription.fields.Where(Function(x) x.name.ToLower() = "fax").Any()
                    Dim done As Boolean = False
                    Dim query As String = String.Empty

                    Try
                        If Not ForceUpdate Then
                            'this the normal query
                            'This should give us a list of all the contact that in our account group that have been modified since the last time (- a few hours) we ran this

                            If (selectFields.Count > 0) Then
                                query = String.Format("select {0} from Contact where SystemModStamp >= {1} {2}", String.Join(",", selectFields.ToArray()), LastUpdate, If(TypeString = "all", String.Empty, " and (" + TypeString + ")"))
                                query = query.Replace("SystemModstamp,", "SystemModstamp,Account.ID,")
                            Else
                                query = String.Format("select email, FirstName, LastName, Phone, MobilePhone, Title, IsDeleted, ccount.ID, ID{0} from Contact where SystemModStamp >= {1} {2}", If(hasFax, ", Fax", ""), LastUpdate, If(TypeString = "all", String.Empty, " and (" + TypeString + ")"))
                            End If
                        Else
                            If (selectFields.Count > 0) Then
                                query = String.Format("select {0} from Contact where Account.ID = '{1}'", String.Join(",", selectFields.ToArray()), AccountIDToUpdate)
                                query = query.Replace("SystemModstamp,", "SystemModstamp,Account.ID,")
                            Else
                                query = String.Format("select email, FirstName, LastName, Phone, MobilePhone, Title, IsDeleted, SystemModstamp, Account.ID, ID{0} from Contact where Account.ID = '{1}'", If(hasFax, ", Fax", ""), AccountIDToUpdate)
                            End If
                        End If

                        'qr = Binding.query(query)
                        qr = GetQueryResults(query)
                        done = False
                        Log.Write("SF Query String = " + query)
                        Log.Write("Found " + qr.size.ToString + " contact records.")
                    Catch ex As Exception
                        Log.Write(ex.Message & " " & ex.StackTrace)
#Region "Old. Hoping to not fall into this catch"
                        'Uh Oh - Error.  Probably no fax number, so we'll try without the fax number
                        Log.Write("Error getting contact - Trying without fax number.")

                        'SystemModstamp or LastModifiedDate
                        'Note - Added second phone just so we have something in that space..
                        If Not ForceUpdate Then
                            'qr = Binding.query("select email, FirstName, LastName, Phone, MobilePhone, Title, IsDeleted, SystemModstamp, Account.ID, ID from Contact where SystemModStamp >= " + LastUpdate + IIf(TypeString = "all", String.Empty, " and (" + TypeString + ")"))
                            qr = GetQueryResults("select email, FirstName, LastName, Phone, MobilePhone, Title, IsDeleted, SystemModstamp, Account.ID, ID from Contact where SystemModStamp >= " + LastUpdate + IIf(TypeString = "all", String.Empty, " and (" + TypeString + ")"))
                        Else
                            'qr = Binding.query("select email, FirstName, LastName, Phone, MobilePhone, Title, IsDeleted, SystemModstamp, Account.ID, ID from Contact where Account.ID = '" + AccountIDToUpdate + "'")
                            qr = GetQueryResults("select email, FirstName, LastName, Phone, MobilePhone, Title, IsDeleted, SystemModstamp, Account.ID, ID from Contact where Account.ID = '" + AccountIDToUpdate + "'")
                        End If

                        done = False

                        Log.Write("Found " + qr.size.ToString + " contact records (no fax).")
                        hasFax = False
#End Region
                    End Try

                    If qr.size > 0 Then
                        Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
                        crmLinkErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "in", "contact")
                        Dim crmLinkError As CRMLinkError = Nothing

                        While Not done
                            For i As Integer = 0 To qr.records.Length - 1
                                Dim thisPerson As New EmployeeData()
                                Dim AccountID As String

                                Dim records As sObject() = qr.records

                                With thisPerson
                                    .Email = records(i).Any(0).InnerText
                                    .FirstName = records(i).Any(1).InnerText
                                    .LastName = records(i).Any(2).InnerText
                                    .Phone = records(i).Any(3).InnerText
                                    .Cell = records(i).Any(4).InnerText
                                    .Title = records(i).Any(5).InnerText

                                    AccountID = records(i).Any(8).InnerText
                                    If AccountID.Length > 18 Then
                                        'I have no idea why, but SF returns the ID here as something like "Account0018000000eM3oOAAS0018000000eM3oOAAS" instead of the standard 18 character account id
                                        'This will return just the final 18 characters which should work.
                                        AccountID = AccountID.Substring(AccountID.Length - 18)
                                    End If

                                    .SalesForceID = records(i).Any(9).InnerText

                                    If hasFax Then
                                        .Fax = records(i).Any(10).InnerText
                                    End If
                                End With

                                LastModifiedDateTime = Date.Parse(records(i).Any(7).InnerText)
                                crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(thisPerson.SalesForceID, String.Empty)

                                Try
                                    UpdateContactInfo(thisPerson, AccountID, ParentOrgID)

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
                                        crmLinkError.ObjectID = thisPerson.SalesForceID
                                        crmLinkError.ObjectData = JsonConvert.SerializeObject(thisPerson)
                                        crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                        crmLinkError.OperationType = "unknown"
                                        newCrmLinkError.Save()
                                    Else
                                        crmLinkError.ObjectData = JsonConvert.SerializeObject(thisPerson)
                                        crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                    End If
                                End Try
                            Next

                            If qr.done Then
                                done = True
                            Else
                                'qr = Binding.queryMore(qr.queryLocator)
                                qr = GetQueryMoreResults(qr.queryLocator)
                            End If
                        End While

                        'check for existence of custom fields to sync
                        GetCustomFields("Contact", TypeString, LastUpdate, crmLinkErrors, AccountIDToUpdate)
                        crmLinkErrors.Save()
                    Else
                        Log.Write("No records found.")
                    End If

                    Log.Write("All done with contact records!")
                Catch ex As Exception
                    Log.Write("Failed to execute query successfully, error message was: " & ex.Message)
                    ErrorCode = IntegrationError.Unknown
                    SyncError = True
                End Try
            End Sub

            Private Function CreateNote(ByVal accountid As String, ByVal thisTicket As Ticket) As Boolean
                Dim Success As Boolean = True

                Dim Title = "Support Issue: " & thisTicket.Name
                Dim description As Action = Actions.GetTicketDescription(User, thisTicket.TicketID)
                Dim NoteBody As String = String.Format("A new support ticket has been created for this account entitled ""{0}"".{3}{2}{3}Click here to access the ticket information: https://app.teamsupport.com/Ticket.aspx?ticketid={1}",
                                                    thisTicket.Name, thisTicket.TicketID.ToString(), HtmlUtility.StripHTML(description.Description), Environment.NewLine)

                Try

                    'Attach a note, which will get re-parented
                    Dim note As sForce.sObject = New sObject()
                    note.type = "Note"
                    note.Any = New System.Xml.XmlElement() {GetNewXmlElement("ParentId", accountid),
                        GetNewXmlElement("Body", NoteBody),
                        GetNewXmlElement("Title", Title)}

                    'Dim noteSave As SaveResult = Binding.create(New sObject() {note})(0)
                    Dim noteSave As SaveResult = CreateObject(New sObject() {note})(0)

                    Success = noteSave.success
                Catch ex As Exception
                    Log.Write("Error in CreateNote: " & ex.Message)
                    ErrorCode = IntegrationError.Unknown

                    Success = False
                End Try

                Return Success
            End Function

            Private Function CreateObject(ByRef objectToCreate As sObject()) As SaveResult()
                Dim result As SaveResult()

                Try
                    result = Binding.create(objectToCreate)
                Catch ex As Exception
                    If (ex.Message.Contains("INVALID_SESSION_ID")) Then
                        Dim loginResult = login(Trim(CRMLinkRow.Username), Trim(CRMLinkRow.Password), Trim(CRMLinkRow.SecurityToken1))
                        result = Binding.create(objectToCreate)
                    Else
                        Throw ex
                    End If
                End Try

                Return result
            End Function

            Private Function GetNewXmlElement(ByVal Name As String, ByVal nodeValue As String) As System.Xml.XmlElement
                Dim doc As System.Xml.XmlDocument = New System.Xml.XmlDocument
                Dim xmlel As System.Xml.XmlElement = doc.CreateElement(Name)
                xmlel.InnerText = DataUtils.StripInvalidXmlCharacters(nodeValue)
                Return xmlel
            End Function

            Private Sub GetProductAndLicenseInfo(ByVal SFLastUpdateTime As String)
                'This is *** CUSTOM CODE *** for Axceler to see if we can get their license and product information


                Log.Write("In GetProductAndLicenseInfo routine.")

                'The results will be placed in qr
                Dim qr As QueryResult = Nothing

                'We are going to increase our return batch size to 250 items
                'Setting is a recommendation only, different batch sizes may
                'be returned depending on data, to keep performance optimized.
                Binding.QueryOptionsValue = New QueryOptions()
                Binding.QueryOptionsValue.batchSize = 250
                Binding.QueryOptionsValue.batchSizeSpecified = True

                Try
                    Dim done As Boolean = False

                    Dim thisSettings As New ServiceLibrary.Settings(User, Processor.ServiceName)

                    Try
                        Dim queryString As String
                        If thisSettings.ReadBool("ResyncAllAxcelerLicenses", True) Then
                            queryString = "select Product__c, Expiration_Date__c, License_type__c, Status__c, Account__c from License__c ORDER BY Product__c"
                            Log.Write("Resyncing all products.")
                        Else
                            queryString = "select Product__c, Expiration_Date__c, License_type__c, Status__c, Account__c from License__c where SystemModStamp >= " + SFLastUpdateTime + " ORDER BY Product__c" 'added order by 3/25/11
                        End If

                        'qr = Binding.query(queryString)
                        qr = GetQueryResults(queryString)

                        done = False

                        Log.Write("Found " + qr.size.ToString + " license records.")
                    Catch ex As Exception
                        'Problem - Not sure what.
                        done = True

                        Log.Write("Error in GetProductAndLicenseInfo. " + ex.Message)
                        SyncError = True
                    End Try

                    If qr.size > 0 Then
                        While Not done

                            For i As Integer = 0 To qr.records.Length - 1

                                Try
                                    Dim ProductName, LicenseType, LicenseStatus, AccountID As String
                                    Dim ExpirationDate As Date? = Nothing
                                    Dim tempExpiration As Date

                                    Log.Write("In GetProductAndLicenseInfo routine - i=" + i.ToString)

                                    Dim records As sObject() = qr.records
                                    Dim Products As sObject = records(i)

                                    ProductName = records(i).Any(0).InnerText

                                    If Date.TryParse(records(i).Any(1).InnerText, tempExpiration) Then
                                        ExpirationDate = tempExpiration
                                    Else
                                        ExpirationDate = Nothing
                                        Log.Write("failed to parse expiration date value: " & records(i).Any(1).InnerText)
                                    End If

                                    LicenseType = records(i).Any(2).InnerText
                                    LicenseStatus = records(i).Any(3).InnerText
                                    AccountID = records(i).Any(4).InnerText

                                    ' 1) See if we can match a product in TS with the product name
                                    Dim thisProduct As Product
                                    Dim findProduct As New Products(User)
                                    findProduct.LoadByOrganizationID(CRMLinkRow.OrganizationID)

                                    thisProduct = findProduct.FindByName(ProductName)

                                    If thisProduct IsNot Nothing Then
                                        Log.Write(String.Format("ProductName = {0}, ExpirationDate = {1}, ProductID = {2}, AccountID = {3}",
                                                                ProductName, IIf(ExpirationDate IsNot Nothing, ExpirationDate.ToString(), ""), thisProduct.ProductID.ToString(), AccountID))

                                        ' 2) If we can, lets see if the product ID is assigned to this customer
                                        Dim findCompany As New Organizations(User)
                                        Dim thisCompany As Organization

                                        'make sure the company already exists
                                        findCompany.LoadByCRMLinkID(AccountID, CRMLinkRow.OrganizationID)

                                        If findCompany.Count > 0 Then
                                            thisCompany = findCompany(0)
                                            Dim findOrgProd As New OrganizationProducts(User)
                                            Dim thisOrgProd As OrganizationProduct

                                            Log.Write("Found product for " & thisCompany.Name)

                                            findOrgProd.LoadByOrganizationAndProductID(thisCompany.OrganizationID, thisProduct.ProductID)

                                            If findOrgProd.Count > 0 Then
                                                thisOrgProd = findOrgProd(0)
                                                'The company already has the product associated with them.
                                                ' We now just need to update the waranty expiration date 
                                                '  Note that the waranty expiration date is a custom field just for this customer...
                                                '      3) Update the waranty expiration date (this should be a custom field on product)
                                                '         Update CustomValue set CustomValue={ExpirationDate} where CustomFieldID = 3761 and RefID={OrganizationProductID}

                                                If ExpirationDate IsNot Nothing AndAlso (thisOrgProd.SupportExpiration Is Nothing OrElse ExpirationDate > thisOrgProd.SupportExpiration) Then 'test to see if we are using the most up to date expiration date (only use product/expiration date that is the most recent)
                                                    thisOrgProd.SupportExpiration = ExpirationDate
                                                    thisOrgProd.Collection.Save()

                                                    'License Type - CustomFieldID is 3770 (test value 101)
                                                    UpdateCustomValue(3770, thisOrgProd.OrganizationProductID, LicenseType)

                                                    'License Status - CustomFieldID is 3771 (test value 102)
                                                    UpdateCustomValue(3771, thisOrgProd.OrganizationProductID, LicenseStatus)

                                                    Log.Write("Product updated.")
                                                Else
                                                    Log.Write("Date information not updated since there is a later expiration date.")
                                                End If

                                            Else
                                                'We need to add the product to this company
                                                thisOrgProd = (New OrganizationProducts(User)).AddNewOrganizationProduct()
                                                thisOrgProd.OrganizationID = thisCompany.OrganizationID
                                                thisOrgProd.ProductID = thisProduct.ProductID

                                                'Product added, lets add/update the expiration date
                                                thisOrgProd.SupportExpiration = ExpirationDate
                                                thisOrgProd.Collection.Save()

                                                'License Type - CustomFieldID is 3770 (test value 101)
                                                UpdateCustomValue(3770, thisOrgProd.OrganizationProductID, LicenseType)

                                                'License Status - CustomFieldID is 3771 (test value 102)
                                                UpdateCustomValue(3771, thisOrgProd.OrganizationProductID, LicenseStatus)
                                                Log.Write("Product added.")
                                            End If

                                        Else
                                            Log.Write("Product not updated because company does not exist.")
                                        End If

                                    End If

                                Catch ex As Exception
                                    Log.Write("Error updating product " & i.ToString() & ": " & ex.Message & " " & ex.StackTrace)
                                    SyncError = True
                                End Try

                            Next

                            done = qr.done

                            If Not done Then
                                'qr = Binding.queryMore(qr.queryLocator)
                                qr = GetQueryMoreResults(qr.queryLocator)
                            End If

                        End While
                    Else
                        Log.Write("No records found.")
                    End If


                Catch ex As Exception
                    Log.Write("Error in GetProductAndLicenseInfo : Failed to execute succesfully, error message was: " & ex.Message & " " & ex.StackTrace)
                    SyncError = True
                End Try
            End Sub

            Private Shadows Sub GetCustomFields(ByVal objType As String, ByVal TypeString As String, ByVal LastUpdate As String, ByRef crmLinkErrors As CRMLinkErrors, Optional ByVal AccountIDToUpdate As String = "")
                Try
                    Dim theseFields As New CRMLinkFields(User)
                    theseFields.LoadByObjectType(objType, CRMLinkRow.CRMLinkID)
                    Dim customFieldList As String = GetCustomFieldList(theseFields, objType)

                    If customFieldList IsNot Nothing AndAlso Not String.IsNullOrEmpty(customFieldList) Then
                        Dim whereCompanyIDClause As String = String.Empty

                        If Not String.IsNullOrEmpty(AccountIDToUpdate) And AccountIDToUpdate <> "0" Then
                            whereCompanyIDClause = " and (Account.ID = '" + AccountIDToUpdate + "')"
                        End If

                        Dim whereNoEmptyEmailClause As String = String.Empty

                        If objType = "Contact" Then
                            whereNoEmptyEmailClause = " and (email <> '')"
                        End If

                        Dim customQuery As String =
                            "select Account.ID, " & If(objType = "Account", "", "email, ") & customFieldList &
                            " from " & objType &
                            " where SystemModStamp >= " + LastUpdate +
                            If(TypeString = "all", String.Empty, " and (" + TypeString + ")") +
                            whereCompanyIDClause +
                            whereNoEmptyEmailClause

                        Log.Write("SF Query String = " + customQuery)

                        Dim queriesResults As List(Of QueryResult) = Nothing
                        Dim numberOfRecordsInQueriesResults As Integer = 0

                        queriesResults = GetQueriesResults(customQuery, numberOfRecordsInQueriesResults)
                        Log.Write(numberOfRecordsInQueriesResults & " records with custom fields to sync.")

                        For Each qr As QueryResult In queriesResults
                            If qr.size > 0 Then
                                Dim crmLinkError As CRMLinkError = Nothing

                                For Each record As sObject In qr.records
                                    Dim accountID As String

                                    'find the object in OUR system
                                    If objType = "Account" Then
                                        accountID = record.Id
                                    ElseIf objType = "Contact" Then
                                        Try
                                            accountID = Array.Find(record.Any, Function(x As Xml.XmlElement) x.LocalName = "Account")("sf:Id").InnerText
                                        Catch ex As Exception
                                            Log.Write(ex.Message)
                                            Continue For
                                        End Try
                                    Else
                                        Return
                                    End If

                                    Dim findAccount As New Organizations(User)
                                    findAccount.LoadByCRMLinkID(accountID, CRMLinkRow.OrganizationID)

                                    If findAccount.Count > 0 Then
                                        Dim thisAccount As Organization = findAccount(0)

                                        'update fields
                                        If objType = "Account" Then
                                            For Each thisField As Xml.XmlElement In record.Any
                                                Dim thisMapping As CRMLinkField = theseFields.FindByCRMFieldName(thisField.LocalName)

                                                If thisMapping IsNot Nothing Then
                                                    Dim value As String = thisField.InnerText

                                                    If thisField.ChildNodes.Count > 1 Then
                                                        value = thisField.LastChild.InnerText
                                                    End If

                                                    Try
                                                        If thisMapping.CustomFieldID IsNot Nothing Then
                                                            crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(accountID, thisMapping.CustomFieldID.ToString())
                                                            Dim translatedFieldValue As String = TranslateFieldValue(thisMapping.CustomFieldID, thisAccount.OrganizationID, value)
                                                            UpdateCustomValue(thisMapping.CustomFieldID, thisAccount.OrganizationID, translatedFieldValue)

                                                            If crmLinkError IsNot Nothing Then
                                                                crmLinkError.Delete()
                                                                crmLinkErrors.Save()
                                                            End If
                                                        ElseIf thisMapping.TSFieldName IsNot Nothing Then
                                                            crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(accountID, thisMapping.TSFieldName)
                                                            Dim tableColumnName As String = GetTableColumnName(thisMapping.TSFieldName)

                                                            If tableColumnName = thisMapping.TSFieldName Then
                                                                thisAccount.Row(thisMapping.TSFieldName) = TranslateFieldValue(value, thisAccount.Row(thisMapping.TSFieldName).GetType().Name)
                                                            Else
                                                                Dim columnValue As Integer = GetTableColumnValue(value, thisMapping.TSFieldName, thisAccount.OrganizationID, thisAccount.ParentID)

                                                                If columnValue <> 0 Then
                                                                    thisAccount.Row(tableColumnName) = columnValue
                                                                End If
                                                            End If

                                                            thisAccount.BaseCollection.Save()

                                                            If crmLinkError IsNot Nothing Then
                                                                crmLinkError.Delete()
                                                                crmLinkErrors.Save()
                                                            End If
                                                        End If
                                                    Catch mappingException As Exception
                                                        Log.Write("The following exception was caught mapping the account field """ &
                                                            thisField.LocalName &
                                                            """ with """ &
                                                            thisMapping.TSFieldName &
                                                            """: " &
                                                            mappingException.Message)

                                                        If crmLinkError Is Nothing Then
                                                            Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                                                            crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                                                            crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                                                            crmLinkError.CRMType = CRMLinkRow.CRMType
                                                            crmLinkError.Orientation = "in"
                                                            crmLinkError.ObjectType = "company"
                                                            crmLinkError.ObjectID = accountID

                                                            If thisMapping.CustomFieldID IsNot Nothing Then
                                                                crmLinkError.ObjectFieldName = thisMapping.CustomFieldID.ToString()
                                                            ElseIf thisMapping.TSFieldName IsNot Nothing Then
                                                                crmLinkError.ObjectFieldName = thisMapping.TSFieldName
                                                            End If

                                                            crmLinkError.ObjectData = value
                                                            crmLinkError.Exception = mappingException.ToString() + mappingException.StackTrace
                                                            crmLinkError.OperationType = "update"
                                                            newCrmLinkError.Save()
                                                        Else
                                                            crmLinkError.ObjectData = value
                                                            crmLinkError.Exception = mappingException.ToString() + mappingException.StackTrace
                                                        End If
                                                    End Try
                                                End If
                                            Next
                                        Else
                                            'if it's not an account, it's a contact (otherwise we would have returned above)
                                            Dim email As String = Array.Find(record.Any, Function(x As Xml.XmlElement) x.LocalName.ToLower() = "email").InnerText
                                            Dim findContact As New Users(User)
                                            Dim thisContact As User = Nothing

                                            findContact.LoadByOrganizationID(thisAccount.OrganizationID, False)
                                            thisContact = findContact.FindByEmail(email)

                                            If thisContact IsNot Nothing Then
                                                For Each thisField As Xml.XmlElement In record.Any
                                                    Dim thisMapping As CRMLinkField = theseFields.FindByCRMFieldName(thisField.LocalName)

                                                    If thisMapping IsNot Nothing Then
                                                        Dim value As String = thisField.InnerText

                                                        If thisField.ChildNodes.Count > 1 Then
                                                            value = thisField.LastChild.InnerText
                                                        End If

                                                        Try
                                                            If thisMapping.CustomFieldID IsNot Nothing Then
                                                                crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(accountID, thisMapping.CustomFieldID.ToString())
                                                                Dim translatedFieldValue As String = TranslateFieldValue(thisMapping.CustomFieldID, thisContact.UserID, value)
                                                                UpdateCustomValue(thisMapping.CustomFieldID, thisContact.UserID, translatedFieldValue)
                                                                Log.Write("Updated """ + thisField.LocalName + """ with """ + thisField.InnerText + """ for """ + email + """ using UpdateCustomValue.")

                                                                If crmLinkError IsNot Nothing Then
                                                                    crmLinkError.Delete()
                                                                    crmLinkErrors.Save()
                                                                End If
                                                            ElseIf thisMapping.TSFieldName IsNot Nothing Then
                                                                crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(accountID, thisMapping.TSFieldName)
                                                                thisContact.Row(thisMapping.TSFieldName) = TranslateFieldValue(value, thisContact.Row(thisMapping.TSFieldName).GetType().Name)
                                                                thisContact.BaseCollection.Save()
                                                                Log.Write("Updated """ + thisField.LocalName + """ with """ + thisField.InnerText + """ for """ + email + """ using Save.")

                                                                If crmLinkError IsNot Nothing Then
                                                                    crmLinkError.Delete()
                                                                    crmLinkErrors.Save()
                                                                End If
                                                            End If
                                                        Catch mappingException As Exception
                                                            Log.Write("The following was exception caught mapping the contact field """ &
                                                                        thisField.LocalName &
                                                                        """ with """ &
                                                                        thisMapping.TSFieldName &
                                                                        """: " &
                                                                        mappingException.Message)

                                                            If crmLinkError Is Nothing Then
                                                                Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                                                                crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                                                                crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                                                                crmLinkError.CRMType = CRMLinkRow.CRMType
                                                                crmLinkError.Orientation = "in"
                                                                crmLinkError.ObjectType = "contact"
                                                                crmLinkError.ObjectID = accountID

                                                                If thisMapping.CustomFieldID IsNot Nothing Then
                                                                    crmLinkError.ObjectFieldName = thisMapping.CustomFieldID.ToString()
                                                                ElseIf thisMapping.TSFieldName IsNot Nothing Then
                                                                    crmLinkError.ObjectFieldName = thisMapping.TSFieldName
                                                                End If

                                                                crmLinkError.ObjectData = value
                                                                crmLinkError.Exception = mappingException.ToString() + mappingException.StackTrace
                                                                crmLinkError.OperationType = "update"
                                                                newCrmLinkError.Save()
                                                            Else
                                                                crmLinkError.ObjectData = value
                                                                crmLinkError.Exception = mappingException.ToString() + mappingException.StackTrace
                                                            End If
                                                        End Try
                                                    End If
                                                Next
                                            Else
                                                Log.Write("Custom fields of """ + email + """ not updated cause user not found in Org:""" + thisAccount.Name + """.")
                                            End If
                                        End If
                                    Else
                                        Log.Write("Custom fields not updated cause CRMLinkID: """ + accountID + """ was not found.")
                                    End If
                                Next
                            End If
                        Next
                    End If
                Catch ex As Exception
                    Log.Write("Exception caught in GetCustomFields: " & ex.Message)
                    Log.Write(ex.StackTrace)
                    SyncError = True
                End Try
            End Sub

            Private Function GetCustomFieldList(ByRef theseFields As CRMLinkFields, ByVal objType As String) As String
                Dim customFieldList As String = Nothing

                If theseFields.Count > 0 Then
                    'Dim objDescription = Binding.describeSObject(objType)
                    Dim objDescription = DescribeObject(objType)

                    For Each field As CRMLinkField In theseFields
                        If (
                            objType = "Account" AndAlso field.CRMFieldName.Trim().ToLower() <> "id"
                            ) OrElse (
                            objType = "Contact" AndAlso field.CRMFieldName.Trim().ToLower() <> "email"
                            ) Then
                            For Each apiField As Field In objDescription.fields
                                Dim fieldName As String = Nothing

                                If apiField.name.Trim().ToLower() = field.CRMFieldName.Trim().ToLower() Then
                                    fieldName = apiField.name
                                ElseIf apiField.type = fieldType.reference AndAlso apiField.relationshipName.Trim().ToLower() = field.CRMFieldName.Trim().ToLower() Then
                                    If TestRelationshipField(apiField.relationshipName, objType) Then
                                        fieldName = apiField.relationshipName + ".Name"
                                    End If
                                End If

                                If fieldName IsNot Nothing Then
                                    AddCustomFieldToList(fieldName, customFieldList)
                                    'No need to continue after the field has been added.
                                    Exit For
                                End If
                            Next
                        End If
                    Next
                End If

                Return customFieldList
            End Function

            Private Sub AddCustomFieldToList(ByRef fieldName As String, ByRef list As String)
                If list Is Nothing Then
                    list = fieldName
                Else
                    'Any duplicate will raise an exception.
                    Dim MatchArray As String() = Array.ConvertAll(list.Split(","), Function(p As String) p.Trim())
                    Dim isFoundInList = False

                    For Each field As String In MatchArray
                        If field = fieldName Then
                            isFoundInList = True
                            Exit For
                        End If
                    Next

                    If Not isFoundInList Then
                        list &= ", " & fieldName
                    Else
                        Log.Write("Custom field " & fieldName & " is mapped more than one time. Included only once in the SF query.")
                    End If
                End If
            End Sub

            Private Function TestRelationshipField(ByVal fieldName As String, ByVal tableName As String) As Boolean
                Dim result As Boolean = True

                Try
                    Dim numberOfCompaniesInQueriesResults As Integer = 0
                    Dim queryresults As List(Of QueryResult) = GetQueriesResults("SELECT " + fieldName + ".Name FROM " + tableName + " LIMIT 1", numberOfCompaniesInQueriesResults)
                Catch ex As Exception
                    result = False
                End Try

                Return result

            End Function

            Private Overloads Function TranslateFieldValue(ByVal salesForceValue As String, ByVal teamSupportTypeName As String) As String
                Dim teamSupportValue As String

                Select Case teamSupportTypeName.ToLower()
                    Case "boolean"
                        teamSupportValue = TranslateBooleanFieldValue(salesForceValue)
                    Case "datetime", "date", "time"
                        teamSupportValue = TranslateDateTimeFieldValue(salesForceValue)
                    Case Else
                        teamSupportValue = salesForceValue
                End Select

                Return teamSupportValue
            End Function

            Private Overloads Function TranslateFieldValue(ByVal customFieldID As Integer, ByVal RefID As Integer, ByVal salesForceValue As String) As String
                Dim result As String

                Dim findCustom As New CustomValues(User)
                findCustom.LoadByFieldID(customFieldID, RefID)
                If findCustom.Count > 0 Then
                    result = TranslateFieldValue(salesForceValue, findCustom(0).FieldType.ToString())
                Else
                    result = salesForceValue
                End If

                Return result
            End Function

            Private Function TranslateBooleanFieldValue(ByVal salesForceValue As String) As String
                Dim teamSupportValue As String

                Dim salesForceComparableValue As String = salesForceValue.Trim().ToLower()

                If salesForceComparableValue = "yes" OrElse salesForceComparableValue = "1" Then
                    teamSupportValue = "true"
                ElseIf salesForceComparableValue = "no" OrElse salesForceComparableValue = "0" Then
                    teamSupportValue = "false"
                Else
                    teamSupportValue = salesForceValue
                End If

                Return teamSupportValue
            End Function

            Private Function TranslateDateTimeFieldValue(ByVal salesForceValue As String) As String
                Dim result As String

                Try
                    Dim organization As New Organizations(User)
                    organization.LoadByOrganizationID(CRMLinkRow.OrganizationID)

                    If (organization.Count > 0) Then
                        Dim organizationTimeZone As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(organization(0).TimeZoneID)
                        Dim convertedToUtc As Date = TimeZoneInfo.ConvertTimeToUtc(Convert.ToDateTime(salesForceValue), organizationTimeZone)
                        result = convertedToUtc.ToString()
                    Else
                        result = Convert.ToDateTime(salesForceValue).ToString()
                    End If
                Catch ex As Exception
                    result = salesForceValue
                End Try

                Return result
            End Function

            Private Function GetTableColumnName(ByRef viewColumnName) As String
                Dim tableColumnName As String

                Select Case viewColumnName.ToLower()
                    Case "primarycontact"
                        tableColumnName = "primaryuserid"
                    Case "slaname"
                        tableColumnName = "slalevelid"
                    Case "defaultsupportgroup"
                        tableColumnName = "defaultsupportgroupid"
                    Case "defaultsupportuser"
                        tableColumnName = "defaultsupportuserid"
                    Case Else
                        tableColumnName = viewColumnName
                End Select

                Return tableColumnName
            End Function

            Private Function GetTableColumnValue(ByRef viewValue As String, ByRef viewColumnName As String, ByVal organizationID As Integer, ByVal organizationParentId As Integer) As String
                Dim tableColumnValue As String

                Select Case viewColumnName.ToLower()
                    Case "primarycontact"
                        Dim primaryContact As Users = New Users(User)
                        primaryContact.LoadByFirstAndLastName(viewValue, organizationID)
                        If primaryContact.Count > 0 Then
                            tableColumnValue = primaryContact(0).UserID.ToString()
                        End If
                    Case "slaname"
                        Dim serviceLevelAgreement As SlaLevels = New SlaLevels(User)
                        serviceLevelAgreement.LoadByName(CRMLinkRow.OrganizationID, viewValue)
                        If serviceLevelAgreement.Count > 0 Then
                            tableColumnValue = serviceLevelAgreement(0).SlaLevelID.ToString()
                        End If
                    Case "defaultsupportgroup"
                        Dim groups As Groups = New Groups(User)
                        groups.LoadByGroupName(organizationParentId, viewValue)
                        If (groups.Count > 0) Then
                            tableColumnValue = groups(0).GroupID.ToString()
                        End If
                    Case "defaultsupportuser"
                        Dim userObject As Users = New Users(User)
                        userObject.LoadByFirstAndLastName(viewValue, organizationParentId)
                        If userObject.Count > 0 Then
                            tableColumnValue = userObject(0).UserID.ToString()
                        End If
                    Case Else
                        tableColumnValue = viewColumnName
                End Select

                Return tableColumnValue
            End Function

            Protected Overrides Sub Finalize()
                MyBase.Finalize()
            End Sub

            Private Function PushTicketsAndPullCases()
                Dim result As Boolean = True

                Dim ticketsToPushAsCases As TicketsView = Nothing
                Dim actionsToPushAsCasesComments As Actions = Nothing
                Dim casesToPullAsTickets As List(Of QueryResult) = Nothing
                Dim casesCommentsToPullAsTickets As List(Of QueryResult) = Nothing

                Dim numberOfCasesToPullAsTickets As Integer = 0
                Dim numberOfCaseCommentsToPullAsTickets As Integer = 0

                Try
                    'To prevent circular updates we get tickets and cases before updating them in SF and TS.
                    If CRMLinkRow.PushTicketsAsCases Then
                        ticketsToPushAsCases = GetTicketsToPushAsCases()
                        actionsToPushAsCasesComments = GetActionsToPushAsCasesComments()
                    End If

                    If CRMLinkRow.PullCasesAsTickets Then
                        Dim loginToPullCasesAndComments As String = login(Trim(CRMLinkRow.Username), Trim(CRMLinkRow.Password), Trim(CRMLinkRow.SecurityToken1))
                        If loginToPullCasesAndComments = "OK" Then
                            casesToPullAsTickets = GetCasesToPullAsTickets(numberOfCasesToPullAsTickets)
                            casesCommentsToPullAsTickets = GetCasesCommentsToPullAsTicketsActions(numberOfCaseCommentsToPullAsTickets)
                            Binding.logout()
                            Binding.logoutAsync()
                        Else
                            If loginToPullCasesAndComments.ToLower() = "password expired" Or loginToPullCasesAndComments.ToLower().Contains("invalid_login") Then
                                ErrorCode = IntegrationError.InvalidLogin
                            End If

                            Log.Write("Login to bring cases and comments failed: " & loginToPullCasesAndComments)
                            Throw New Exception(loginToPullCasesAndComments)
                        End If
                    End If

                    ''Once the tickets and cases have been obtained we proceed to update.
                    If ticketsToPushAsCases IsNot Nothing OrElse actionsToPushAsCasesComments IsNot Nothing Then
                        Dim loginToPushTicketsAndActions As String = login(Trim(CRMLinkRow.Username), Trim(CRMLinkRow.Password), Trim(CRMLinkRow.SecurityToken1))
                        If loginToPushTicketsAndActions = "OK" Then
                            If ticketsToPushAsCases IsNot Nothing AndAlso ticketsToPushAsCases.Count > 0 Then
                                PushTicketsAsCases(ticketsToPushAsCases, casesToPullAsTickets, numberOfCasesToPullAsTickets)
                            End If
                            If actionsToPushAsCasesComments IsNot Nothing AndAlso actionsToPushAsCasesComments.Count > 0 Then
                                PushTicketsActionsAsCasesComments(actionsToPushAsCasesComments)
                            End If
                            Binding.logout()
                            Binding.logoutAsync()
                        Else
                            If loginToPushTicketsAndActions.ToLower() = "password expired" Or loginToPushTicketsAndActions.ToLower().Contains("invalid_login") Then
                                ErrorCode = IntegrationError.InvalidLogin
                            End If

                            Log.Write("Login to push tickets and actions failed: " & loginToPushTicketsAndActions)
                            Throw New Exception(loginToPushTicketsAndActions)
                        End If
                    End If

                    If numberOfCasesToPullAsTickets > 0 Then
                        PullCasesAsTickets(casesToPullAsTickets, ticketsToPushAsCases)
                    End If

                    If numberOfCaseCommentsToPullAsTickets > 0 Then
                        PullCasesCommentsAsTicketsActions(casesCommentsToPullAsTickets)
                    End If
                Catch ex As Exception
                    result = False
                    Log.Write("Error in PushTicketsAndPullCases.  Message = " + ex.Message)
                    Log.Write(ex.StackTrace)
                    Try
                        Binding.logout()
                        Binding.logoutAsync()
                    Catch
                    End Try
                End Try

                Return result
            End Function

            Private Function GetTicketsToPushAsCases() As TicketsView
                Dim result As New TicketsView(User)
                result.LoadModifiedByCRMLinkItem(CRMLinkRow)
                Log.Write("Got " + result.Count.ToString() + " Tickets To Send As Cases.")
                Return result
            End Function

            Private Function GetActionsToPushAsCasesComments()
                Dim result As New Actions(User)
                result.LoadModifiedByCRMLinkItem(CRMLinkRow)
                Log.Write("Got " + result.Count.ToString() + " TicketActions To Send As CaseComments.")
                Return result
            End Function

            Private Function GetCasesToPullAsTickets(ByRef numberOfCasesToBringAsTickets As Integer) As List(Of QueryResult)
                Dim result As List(Of QueryResult) = Nothing
                Dim fieldsList As String = GetFieldsListToGetCasesToBringAsTickets()
                Dim TempTime As New DateTime(1900, 1, 1)

                If CRMLinkRow.LastLink IsNot Nothing Then
                    TempTime = CRMLinkRow.LastLinkUtc.Value.AddHours(-1) 'push last update time back 1 hour to make sure we catch every change
                End If

                Dim lastLinkValueToQuery As String = TempTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'")

                If fieldsList IsNot Nothing Then
                    Dim query As String = "SELECT " + fieldsList + " FROM Case WHERE SystemModStamp >= " + lastLinkValueToQuery + IIf(AccountTypeString = "all", String.Empty, " and (" + AccountTypeString + ")")
                    result = GetQueriesResults(query, numberOfCasesToBringAsTickets)
                    Log.Write(query)
                    Log.Write("numberOfCasesToBringAsTickets: " + numberOfCasesToBringAsTickets.ToString())
                End If
                Return result
            End Function

            Private Function GetCasesCommentsToPullAsTicketsActions(ByRef numberOfCaseCommentsToBringAsTicketActions As Integer)
                Dim result As List(Of QueryResult) = Nothing
                Dim fieldsList As String = GetFieldsListToGetCasesCommentsToBringAsTickets()
                Dim TempTime As New DateTime(1900, 1, 1)

                If CRMLinkRow.LastLink IsNot Nothing Then
                    TempTime = CRMLinkRow.LastLinkUtc.Value.AddHours(-1) 'push last update time back 1 hour to make sure we catch every change
                End If

                Dim lastLinkValueToQuery As String = TempTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'")
                If fieldsList IsNot Nothing Then
                    'Problem: The CaseComment object does not include the Account relationship.
                    'Solution: Figure out a way to query either using AccountType or the list of cases
                    Dim query As String = "SELECT " + fieldsList + " FROM CaseComment WHERE SystemModStamp >= " + lastLinkValueToQuery + IIf(AccountTypeString = "all", String.Empty, " and (" + AccountTypeString.Replace("Account", "Parent.Account") + ")")
                    result = GetQueriesResults(query, numberOfCaseCommentsToBringAsTicketActions)
                    Log.Write(query)
                    Log.Write("numberOfCaseCommentsToBringAsTicketActions: " + numberOfCaseCommentsToBringAsTicketActions.ToString())
                End If
                Return result
            End Function

            Private Function GetFieldsListToGetCasesToBringAsTickets() As String
                Dim result As String = Nothing

                Dim customFields As New CRMLinkFields(User)
                customFields.LoadByObjectType("Ticket", CRMLinkRow.CRMLinkID)

                'For Each apiField As Field In Binding.describeSObject("Case").fields
                For Each apiField As Field In DescribeObject("Case").fields
                    Select Case apiField.name.Trim().ToLower()
                        Case "id", "accountid", "type", "status", "subject", "priority", "description", "closedate", "systemmodstamp"
                            AddCustomFieldToList(apiField.name, result)
                        Case "contactid", "ownerid", "createdbyid", "lastmodifiedbyid"
                            AddCustomFieldToList(apiField.name, result)
                            AddCustomFieldToList(apiField.relationshipName + ".Email", result)
                        Case Else
                            Dim cRMLinkField As CRMLinkField = customFields.FindByCRMFieldName(apiField.name)
                            If cRMLinkField IsNot Nothing Then
                                AddCustomFieldToList(apiField.name, result)
                            ElseIf apiField.relationshipName IsNot Nothing Then
                                cRMLinkField = customFields.FindByCRMFieldName(apiField.relationshipName)
                                If cRMLinkField IsNot Nothing Then
                                    AddCustomFieldToList(apiField.relationshipName + ".Name", result)
                                End If
                            End If
                    End Select
                Next

                Return result
            End Function

            Private Function GetFieldsListToGetCasesCommentsToBringAsTickets() As String
                Dim result As String = Nothing

                'For Each apiField As Field In Binding.describeSObject("CaseComment").fields
                For Each apiField As Field In DescribeObject("CaseComment").fields
                    Select Case apiField.name.Trim().ToLower()
                        Case "id", "parentid", "commentbody", "systemmodstamp"
                            AddCustomFieldToList(apiField.name, result)
                        Case "createdbyid", "lastmodifiedbyid"
                            AddCustomFieldToList(apiField.name, result)
                            AddCustomFieldToList(apiField.relationshipName + ".Email", result)
                    End Select
                Next

                Return result

            End Function

            Private Sub PushTicketsAsCases(
              ByVal ticketsToPushAsCases As TicketsView,
              ByVal casesToPullAsTickets As List(Of QueryResult),
              ByVal numberOfCasesToPullAsTickets As Integer)

                Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
                crmLinkErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "out", "ticket")
                Dim crmLinkError As CRMLinkError = Nothing

                For Each ticket As TicketsViewItem In ticketsToPushAsCases
                    'SalesForce Case-Customer relationship is one to one.
                    'Therefore we are assigning the first customer created in TeamSupport.
                    Dim salesForceCustomer As SalesForceCustomer = New SalesForceCustomer(User)
                    salesForceCustomer.LoadByTicketID(ticket.TicketID, Binding)
                    If salesForceCustomer.AccountID IsNot Nothing Then
                        Dim salesForceCase As sForce.sObject = New sObject()
                        salesForceCase.type = "Case"
                        salesForceCase.Id = ticket.SalesForceID
                        Dim isNewCase As Boolean = False
                        If ticket.SalesForceID Is Nothing Then
                            isNewCase = True
                        End If

                        Dim isNotCollidingWithACaseToPull = False

                        Dim impersonation = True

                        salesForceCase.Any = GetSalesForceCaseData(
                                                ticket,
                                                isNewCase,
                                                salesForceCustomer,
                                                casesToPullAsTickets,
                                                isNotCollidingWithACaseToPull,
                                                numberOfCasesToPullAsTickets,
                                                impersonation)

                        If isNotCollidingWithACaseToPull Then
                            Dim updateTicket As Tickets = New Tickets(User)
                            updateTicket.LoadByTicketID(ticket.TicketID)

                            crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID.ToString(), "case")

                            Dim pushSucceeded As Boolean = True

                            If isNewCase Then
                                Try
                                    'Dim result As SaveResult = Binding.create(New sObject() {salesForceCase})(0)
                                    Dim result As SaveResult = CreateObject(New sObject() {salesForceCase})(0)
                                    If result.errors Is Nothing Then
                                        updateTicket(0).SalesForceID = result.id
                                        Dim caseNumber As String = GetCaseNumber(result.id)
                                        Dim actionLogDescription As String = If(String.IsNullOrEmpty(caseNumber), String.Format("Sent Ticket to SalesForce as new Case with ID: '{0}'.", result.id), String.Format("Sent Ticket to SalesForce as new Case: '{0}'.", caseNumber))
                                        ActionLogs.AddActionLog(User, ActionLogType.Insert, ReferenceType.Tickets, ticket.TicketID, actionLogDescription)
                                        Dim logDescription As String = String.Format("Sent Ticket to SalesForce as new Case {0}, ID {1}.", caseNumber, result.id)
                                        Log.Write(logDescription)

                                        If crmLinkError IsNot Nothing Then
                                            crmLinkError.Delete()
                                            crmLinkErrors.Save()
                                        End If
                                    Else
                                        Log.Write("Creating case for ticketID: " + ticket.TicketID.ToString() + ", the following exception ocurred: " + result.errors(0).message)
                                        Log.Write("Attempting without impersonation...")
                                        impersonation = False
                                        salesForceCase.Any = GetSalesForceCaseData(
                                                                ticket,
                                                                isNewCase,
                                                                salesForceCustomer,
                                                                casesToPullAsTickets,
                                                                isNotCollidingWithACaseToPull,
                                                                numberOfCasesToPullAsTickets,
                                                                impersonation)
                                        'result = Binding.create(New sObject() {salesForceCase})(0)
                                        result = CreateObject(New sObject() {salesForceCase})(0)
                                        If result.errors Is Nothing Then
                                            updateTicket(0).SalesForceID = result.id
                                            Dim actionLogDescription As String = "Sent Ticket to SalesForce as new Case with ID: '" + result.id + "'."
                                            ActionLogs.AddActionLog(User, ActionLogType.Insert, ReferenceType.Tickets, ticket.TicketID, actionLogDescription)

                                            If crmLinkError IsNot Nothing Then
                                                crmLinkError.Delete()
                                                crmLinkErrors.Save()
                                            End If
                                        Else
                                            Throw (New Exception(result.errors(0).message))
                                        End If
                                    End If
                                Catch ex As Exception
                                    pushSucceeded = False
                                    Log.Write("Creating case for ticketID: " + ticket.TicketID.ToString() + ", the following exception ocurred: " + ex.Message)
                                    Log.Write("sObject.Any property value: " + salesForceCase.Any.ToString())
                                    Log.Write(ex.StackTrace)
                                    'Throw ex
                                    If crmLinkError Is Nothing Then
                                        Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                                        crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                                        crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                                        crmLinkError.CRMType = CRMLinkRow.CRMType
                                        crmLinkError.Orientation = "out"
                                        crmLinkError.ObjectType = "ticket"
                                        crmLinkError.ObjectFieldName = "case"
                                        crmLinkError.ObjectID = ticket.TicketID.ToString()
                                        crmLinkError.ObjectData = JsonConvert.SerializeObject(salesForceCase)
                                        crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                        crmLinkError.OperationType = "create"
                                        newCrmLinkError.Save()
                                    Else
                                        crmLinkError.ObjectData = JsonConvert.SerializeObject(salesForceCase)
                                        crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                    End If
                                End Try
                            Else
                                Try
                                    'Dim result As SaveResult = Binding.update(New sObject() {salesForceCase})(0)
                                    Dim result As SaveResult = UpdateObject(New sObject() {salesForceCase})(0)
                                    If result.errors Is Nothing Then
                                        Dim caseNumber As String = GetCaseNumber(ticket.SalesForceID)
                                        Dim actionLogDescription As String = If(String.IsNullOrEmpty(caseNumber), String.Format("Updated SalesForce Case ID: '{0}' with ticket changes.", ticket.SalesForceID), String.Format("Updated SalesForce Case Number: '{0}' with ticket changes.", caseNumber))
                                        Dim lastLog As ActionLogs = New ActionLogs(User)
                                        lastLog.LoadLastByTypeAndID(ReferenceType.Tickets, ticket.TicketID)

                                        If (lastLog.IsEmpty OrElse lastLog(0).Description <> actionLogDescription) Then
                                            ActionLogs.AddActionLog(User, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, actionLogDescription)
                                            Dim logDescription As String = String.Format("Updated SalesForce Case ID: '{0}', Case Number: {1} with ticket changes.", ticket.SalesForceID, caseNumber)
                                            Log.Write(logDescription)
                                        End If
                                        If crmLinkError IsNot Nothing Then
                                            crmLinkError.Delete()
                                            crmLinkErrors.Save()
                                        End If
                                    ElseIf result.errors(0).message.ToLower() = "entity is deleted" Then
                                        Dim actionLogDescription As String = "SalesForce Case ID: '" + ticket.SalesForceID + "' was not found. No update applied. Error: " + result.errors(0).message
                                        ActionLogs.AddActionLog(User, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, actionLogDescription)
                                        If crmLinkError IsNot Nothing Then
                                            crmLinkError.Delete()
                                            crmLinkErrors.Save()
                                        End If
                                    Else
                                        Log.Write("Updating case for ticketID: " + ticket.TicketID.ToString() + ", the following exception ocurred: " + result.errors(0).message)
                                        Log.Write("Attempting without impersonation...")
                                        impersonation = False
                                        salesForceCase.Any = GetSalesForceCaseData(
                                                                ticket,
                                                                isNewCase,
                                                                salesForceCustomer,
                                                                casesToPullAsTickets,
                                                                isNotCollidingWithACaseToPull,
                                                                numberOfCasesToPullAsTickets,
                                                                impersonation)
                                        'result = Binding.update(New sObject() {salesForceCase})(0)
                                        result = UpdateObject(New sObject() {salesForceCase})(0)
                                        If result.errors Is Nothing Then
                                            Dim caseNumber As String = GetCaseNumber(ticket.SalesForceID)
                                            Dim actionLogDescription As String = If(String.IsNullOrEmpty(caseNumber), String.Format("Updated SalesForce Case ID: '{0}' with ticket changes.", ticket.SalesForceID), String.Format("Updated SalesForce Case Number: '{0}' with ticket changes.", caseNumber))
                                            Dim lastLog As ActionLogs = New ActionLogs(User)
                                            lastLog.LoadLastByTypeAndID(ReferenceType.Tickets, ticket.TicketID)

                                            If (lastLog.IsEmpty OrElse lastLog(0).Description <> actionLogDescription) Then
                                                ActionLogs.AddActionLog(User, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, actionLogDescription)
                                                Dim logDescription As String = String.Format("Updated SalesForce Case ID: '{0}', Case Number: {1} with ticket changes.", ticket.SalesForceID, caseNumber)
                                                Log.Write(logDescription)
                                            End If
                                            If crmLinkError IsNot Nothing Then
                                                crmLinkError.Delete()
                                                crmLinkErrors.Save()
                                            End If
                                        Else
                                            Throw (New Exception(result.errors(0).message))
                                        End If
                                    End If
                                Catch ex As Exception
                                    pushSucceeded = False
                                    Log.Write("Updating case for ticketID: " + ticket.TicketID.ToString() + ", the following exception ocurred: " + ex.Message)
                                    Log.Write("sObject.Any property value: " + salesForceCase.Any.ToString())
                                    Log.Write(ex.StackTrace)
                                    'Throw ex
                                    If crmLinkError Is Nothing Then
                                        Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                                        crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                                        crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                                        crmLinkError.CRMType = CRMLinkRow.CRMType
                                        crmLinkError.Orientation = "out"
                                        crmLinkError.ObjectType = "ticket"
                                        crmLinkError.ObjectFieldName = "case"
                                        crmLinkError.ObjectID = ticket.TicketID.ToString()
                                        crmLinkError.ObjectData = JsonConvert.SerializeObject(salesForceCase)
                                        crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                        crmLinkError.OperationType = "update"
                                        newCrmLinkError.Save()
                                    Else
                                        crmLinkError.ObjectData = JsonConvert.SerializeObject(salesForceCase)
                                        crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                    End If
                                End Try
                            End If

                            If pushSucceeded Then
                                Dim userIds As List(Of Integer) = New List(Of Integer)()
                                Dim readTickets As TicketsView = New TicketsView(User)

                                readTickets.LoadUserTicketsByTicketId(ticket.TicketID, CRMLinkRow.OrganizationID)

                                For Each readTicket As TicketsViewItem In readTickets
                                    If readTicket.IsRead Then
                                        userIds.Add(Integer.Parse(readTicket.ViewerID))
                                    End If
                                Next

                                If salesForceCustomer.ContactID IsNot Nothing Then
                                    updateTicket.SetUserAsSentToSalesForce(salesForceCustomer.TeamSupportUserID, ticket.TicketID)
                                Else
                                    updateTicket.SetOrganizationAsSentToSalesForce(salesForceCustomer.TeamSupportOrganizationID, ticket.TicketID)
                                End If

                                updateTicket(0).DateModifiedBySalesForceSync = DateTime.UtcNow
                                updateTicket(0).UpdateSalesForceData()

                                Dim userTicketStatuses As UserTicketStatuses = New UserTicketStatuses(User)
                                userTicketStatuses.ResetToUnreadOnSalesForceUpdate(ticket.TicketID, userIds)
                            End If
                        Else
                            Log.Write("TicketID: " + ticket.TicketID.ToString() + ", was not pushed because it is colliding with a case being pulled from SalesForce (If DatesModified missing above SF object did not include it).")
                        End If
                    Else
                        Log.Write("TicketID: " + ticket.TicketID.ToString() + ", was not pushed because first customer is not in SalesForce.")
                    End If
                Next
                crmLinkErrors.Save()
            End Sub

            Private Function UpdateObject(ByRef objectToUpdate As sObject()) As SaveResult()
                Dim result As SaveResult()

                Try
                    result = Binding.update(objectToUpdate)
                Catch ex As Exception
                    If (ex.Message.Contains("INVALID_SESSION_ID")) Then
                        Dim loginResult = login(Trim(CRMLinkRow.Username), Trim(CRMLinkRow.Password), Trim(CRMLinkRow.SecurityToken1))
                        result = Binding.update(objectToUpdate)
                    Else
                        Throw ex
                    End If
                End Try

                Return result
            End Function

            Private Function GetSalesForceCaseData(
              ByVal ticket As TicketsViewItem,
              ByVal isNewCase As Boolean,
              ByVal salesForceCustomer As SalesForceCustomer,
              ByVal casesToPullAsTickets As List(Of QueryResult),
              ByRef isNotCollidingWithACaseToPull As Boolean,
              ByVal numberOfCasesToPullAsTickets As Integer,
              ByVal impersonation As Boolean) As XmlElement()
                Dim result As New List(Of XmlElement)

                Dim customFields As New CRMLinkFields(User)
                customFields.LoadByObjectType("Ticket", CRMLinkRow.CRMLinkID)

                If isNewCase OrElse numberOfCasesToPullAsTickets = 0 Then
                    isNotCollidingWithACaseToPull = True
                End If

                'Dim caseObjectDescription = Binding.describeSObject("Case")
                Dim caseObjectDescription = DescribeObject("Case")
                For Each field As Field In caseObjectDescription.fields
                    If Not isNotCollidingWithACaseToPull AndAlso field.name.Trim().ToLower() = "lastmodifieddate" Then
                        Dim dateModifiedOfCollidingCaseToPull As DateTime? = GetDateModifiedOfCollidingCaseToPull(casesToPullAsTickets, ticket.SalesForceID)
                        If dateModifiedOfCollidingCaseToPull IsNot Nothing AndAlso dateModifiedOfCollidingCaseToPull > ticket.DateModifiedUtc Then
                            Log.Write("A case to pull was found with a DateModified (" + dateModifiedOfCollidingCaseToPull.ToString() + ") greater than the ticket to push. (" + ticket.DateModifiedUtc.ToString() + ")")
                            Exit For
                        Else
                            isNotCollidingWithACaseToPull = True
                        End If
                    End If

                    Dim cRMLinkField As CRMLinkField = customFields.FindByCRMFieldName(field.name)
                    If cRMLinkField IsNot Nothing Then
                        If cRMLinkField.CustomFieldID IsNot Nothing Then
                            Dim findCustom As New CustomValues(User)
                            findCustom.LoadByFieldID(cRMLinkField.CustomFieldID, ticket.TicketID)
                            If findCustom.Count > 0 AndAlso ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                Dim value As String = findCustom(0).Value
                                If field.type = fieldType.date Then
                                    Dim dateValue As Date
                                    If Date.TryParse(value, dateValue) Then
                                        value = dateValue.ToString("yyyy'-'MM'-'dd")
                                    Else
                                        Dim message As StringBuilder = New StringBuilder("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because is mapped to a date field and the value is not a valid date.")
                                        Log.Write(message.ToString())
                                        Continue For
                                    End If
                                ElseIf field.type = fieldType.datetime Then
                                    Dim dateValue As DateTime
                                    If DateTime.TryParse(value, dateValue) Then
                                        value = dateValue.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'")
                                    Else
                                        Dim message As StringBuilder = New StringBuilder("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because is mapped to a datetime field and the value is not a valid datetime.")
                                        Log.Write(message.ToString())
                                        Continue For
                                    End If
                                ElseIf field.type = fieldType.boolean AndAlso value = String.Empty Then
                                    value = "0"
                                End If
                                result.Add(GetNewXmlElement(field.name, value))
                            Else
                                Dim message As StringBuilder = New StringBuilder("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because ")
                                If findCustom.Count = 0 Then
                                    message.Append("it was null")
                                End If
                                If findCustom.Count = 0 AndAlso Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                    message.Append(" and ")
                                End If
                                If Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                    message.Append("the field is not updatable.")
                                End If
                                Log.Write(message.ToString())
                            End If
                        ElseIf cRMLinkField.TSFieldName IsNot Nothing Then
                            If ticket.Row(cRMLinkField.TSFieldName) IsNot Nothing AndAlso ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                Dim value As String = If(IsDBNull(ticket.Row(cRMLinkField.TSFieldName)), String.Empty, ticket.Row(cRMLinkField.TSFieldName))
                                If field.type = fieldType.date Then
                                    Dim dateValue As Date
                                    If Date.TryParse(value, dateValue) Then
                                        value = dateValue.ToString("yyyy'-'MM'-'dd")
                                    Else
                                        Dim message As StringBuilder = New StringBuilder("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because is mapped to a date field and the value is not a valid date.")
                                        Log.Write(message.ToString())
                                        Continue For
                                    End If
                                ElseIf field.type = fieldType.datetime Then
                                    Dim dateValue As DateTime
                                    If DateTime.TryParse(value, dateValue) Then
                                        value = dateValue.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'")
                                    Else
                                        Dim message As StringBuilder = New StringBuilder("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because is mapped to a datetime field and the value is not a valid datetime.")
                                        Log.Write(message.ToString())
                                        Continue For
                                    End If
                                End If
                                result.Add(GetNewXmlElement(field.name, value))
                            Else
                                Dim message As StringBuilder = New StringBuilder("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because ")
                                If ticket.Row(cRMLinkField.TSFieldName) Is Nothing Then
                                    message.Append("it was null")
                                End If
                                If ticket.Row(cRMLinkField.TSFieldName) Is Nothing AndAlso Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                    message.Append(" and ")
                                End If
                                If Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                    message.Append("the field is not updatable.")
                                End If
                                Log.Write(message.ToString())
                            End If
                        Else
                            Log.Write(
                              "TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because custom field " +
                              cRMLinkField.CRMFieldID.ToString() + " CustomFieldID and TSFieldName are null.")
                        End If
                        ' Push of Lookup fields mappings is not supported at this time. 
                        'Else If apiField.relationshipName IsNot Nothing Then
                        '  cRMLinkField = customFields.FindByCRMFieldName(apiField.relationshipName)
                        '  If cRMLinkField IsNot Nothing Then
                        '    AddCustomFieldToList(apiField.relationshipName + ".Name", result)
                        '  End If
                    Else
                        Select Case field.name.Trim().ToLower()
                            Case "contactid"
                                If salesForceCustomer.ContactID IsNot Nothing AndAlso ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                    result.Add(GetNewXmlElement(field.name, salesForceCustomer.ContactID))
                                Else
                                    Dim message As StringBuilder = New StringBuilder("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because ")
                                    If salesForceCustomer.ContactID Is Nothing Then
                                        message.Append("it was null")
                                    End If
                                    If salesForceCustomer.ContactID Is Nothing AndAlso Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                        message.Append(" and ")
                                    End If
                                    If Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                        message.Append("the field is not updatable.")
                                    End If
                                    Log.Write(message.ToString())
                                End If
                            Case "accountid"
                                If salesForceCustomer.AccountID IsNot Nothing AndAlso ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                    result.Add(GetNewXmlElement(field.name, salesForceCustomer.AccountID))
                                Else
                                    Dim message As StringBuilder = New StringBuilder("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because ")
                                    If salesForceCustomer.AccountID Is Nothing Then
                                        message.Append("it was null")
                                    End If
                                    If salesForceCustomer.AccountID Is Nothing AndAlso Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                        message.Append(" and ")
                                    End If
                                    If Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                        message.Append("the field is not updatable.")
                                    End If
                                    Log.Write(message.ToString())
                                End If
                            Case "type"
                                'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                If ticket.TicketTypeName IsNot Nothing AndAlso ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                    result.Add(GetNewXmlElement(field.name, ticket.TicketTypeName))
                                Else
                                    Dim message As StringBuilder = New StringBuilder("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because ")
                                    If ticket.TicketTypeName Is Nothing Then
                                        message.Append("it was null")
                                    End If
                                    If ticket.TicketTypeName Is Nothing AndAlso Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                        message.Append(" and ")
                                    End If
                                    If Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                        message.Append("the field is not updatable.")
                                    End If
                                    Log.Write(message.ToString())
                                End If
                            Case "status"
                                'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                If ticket.Status IsNot Nothing AndAlso ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                    result.Add(GetNewXmlElement(field.name, ticket.Status))
                                Else
                                    Dim message As StringBuilder = New StringBuilder("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because ")
                                    If ticket.Status Is Nothing Then
                                        message.Append("it was null")
                                    End If
                                    If ticket.Status Is Nothing AndAlso Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                        message.Append(" and ")
                                    End If
                                    If Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                        message.Append("the field is not updatable.")
                                    End If
                                    Log.Write(message.ToString())
                                End If
                            Case "subject"
                                'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                If ticket.Name IsNot Nothing AndAlso ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                    result.Add(GetNewXmlElement(field.name, ticket.Name))
                                Else
                                    Dim message As StringBuilder = New StringBuilder("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because ")
                                    If ticket.Name Is Nothing Then
                                        message.Append("it was null")
                                    End If
                                    If ticket.Name Is Nothing AndAlso Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                        message.Append(" and ")
                                    End If
                                    If Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                        message.Append("the field is not updatable.")
                                    End If
                                    Log.Write(message.ToString())
                                End If
                            Case "priority"
                                'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                If ticket.Severity IsNot Nothing AndAlso ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                    result.Add(GetNewXmlElement(field.name, ticket.Severity))
                                Else
                                    Dim message As StringBuilder = New StringBuilder("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because ")
                                    If ticket.Severity Is Nothing Then
                                        message.Append("it was null")
                                    End If
                                    If ticket.Severity Is Nothing AndAlso Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                        message.Append(" and ")
                                    End If
                                    If Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                        message.Append("the field is not updatable.")
                                    End If
                                    Log.Write(message.ToString())
                                End If
                            Case "description"
                                'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                Dim action As Action = Actions.GetTicketDescription(User, ticket.TicketID)
                                Dim description As String = Nothing
                                Dim logError As Boolean = False
                                If action IsNot Nothing Then
                                    description = Actions.GetTicketDescription(User, ticket.TicketID).Description
                                    If description IsNot Nothing AndAlso ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                        result.Add(GetNewXmlElement(field.name, TruncateCaseCommentBody(HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(description)))))
                                    Else
                                        logError = True
                                    End If
                                Else
                                    logError = True
                                End If

                                If logError Then
                                    Dim message As StringBuilder = New StringBuilder("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because ")
                                    If description Is Nothing Then
                                        message.Append("it was null")
                                    End If
                                    If description Is Nothing AndAlso Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                        message.Append(" and ")
                                    End If
                                    If Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                        message.Append("the field is not updatable.")
                                    End If
                                    Log.Write(message.ToString())
                                End If
                    'See ticket 13807. Createdate was not passed as is not createable or updateable. Because SalesForce denies creating a case with a closedate greater than createdate i'm taking it out.
                    'Case "closeddate"
                    '  'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                    '  If ticket.DateClosedUtc IsNot Nothing AndAlso ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                    '    result.Add(GetNewXmlElement(field.name, CType(ticket.DateClosedUtc, DateTime).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'")))
                    '  Else
                    '    Dim message As StringBuilder = New StringBuilder("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because ")
                    '    If ticket.DateClosed Is Nothing Then
                    '      message.Append("it was null")
                    '    End If
                    '    If ticket.DateClosed Is Nothing AndAlso Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                    '      message.Append(" and ")
                    '    End If
                    '    If Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                    '      message.Append("the field is not updatable.")
                    '    End If
                    '    Log.Write(message.ToString())
                    '  End If
                            Case "ownerid"
                                If impersonation Then
                                    'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                    If ticket.UserID IsNot Nothing Then
                                        Dim owner As User = Users.GetUser(User, ticket.UserID)
                                        If owner IsNot Nothing Then
                                            Dim salesForceID As String = owner.SalesForceID
                                            If salesForceID Is Nothing Then
                                                salesForceID = GetSalesForceUserID(owner.Email)
                                                owner.SalesForceID = salesForceID
                                                owner.Collection.Save()
                                            End If
                                            If salesForceID IsNot Nothing AndAlso ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                                result.Add(GetNewXmlElement(field.name, salesForceID))
                                            Else
                                                Dim message As StringBuilder = New StringBuilder("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because ")
                                                If salesForceID Is Nothing Then
                                                    message.Append("it was null")
                                                End If
                                                If salesForceID Is Nothing AndAlso Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                                    message.Append(" and ")
                                                End If
                                                If Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                                    message.Append("the field is not updatable.")
                                                End If
                                                Log.Write(message.ToString())
                                            End If
                                        Else
                                            Log.Write("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because there is no owner.")
                                        End If
                                    Else
                                        Log.Write("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because there is no owner.")
                                    End If
                                Else
                                    Log.Write("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because impersonation flag is false.")
                                End If
                            Case "createdbyid"
                                If impersonation Then
                                    'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                    Dim creator As User = Users.GetUser(User, ticket.CreatorID)
                                    'Ticket 14381. SalesForce does not allow contact to create cases. Therefore we only try to add the creator if is not a contact.
                                    If creator IsNot Nothing AndAlso creator.OrganizationID = ticket.OrganizationID Then
                                        Dim salesForceID As String = creator.SalesForceID
                                        If salesForceID Is Nothing Then
                                            salesForceID = GetSalesForceUserID(creator.Email)
                                            creator.SalesForceID = salesForceID
                                            creator.Collection.Save()
                                        End If
                                        If salesForceID IsNot Nothing AndAlso ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                            result.Add(GetNewXmlElement(field.name, salesForceID))
                                        Else
                                            Dim message As StringBuilder = New StringBuilder("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because ")
                                            If salesForceID Is Nothing Then
                                                message.Append("it was null")
                                            End If
                                            If salesForceID Is Nothing AndAlso Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                                message.Append(" and ")
                                            End If
                                            If Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                                message.Append("the field is not updatable.")
                                            End If
                                            Log.Write(message.ToString())
                                        End If
                                    Else
                                        Log.Write("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because there is no creator.")
                                    End If
                                Else
                                    Log.Write("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because impersonation flag is false.")
                                End If
                            Case "lastmodifiedbyid"
                                If impersonation Then
                                    'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                    Dim modifier As User = Users.GetUser(User, ticket.ModifierID)
                                    'Ticket 14381. SalesForce does not allow contacts to modify cases. Therefore we only try to add the modifier if is not a contact.
                                    If modifier IsNot Nothing AndAlso modifier.OrganizationID = ticket.OrganizationID Then
                                        Dim salesForceID As String = modifier.SalesForceID
                                        If salesForceID Is Nothing Then
                                            salesForceID = GetSalesForceUserID(modifier.Email)
                                            modifier.SalesForceID = salesForceID
                                            modifier.Collection.Save()
                                        End If
                                        If salesForceID IsNot Nothing AndAlso ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                            result.Add(GetNewXmlElement(field.name, salesForceID))
                                        Else
                                            Dim message As StringBuilder = New StringBuilder("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because ")
                                            If salesForceID Is Nothing Then
                                                message.Append("it was null")
                                            End If
                                            If salesForceID Is Nothing AndAlso Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                                message.Append(" and ")
                                            End If
                                            If Not ((isNewCase AndAlso field.createable) OrElse field.updateable) Then
                                                message.Append("the field is not updatable.")
                                            End If
                                            Log.Write(message.ToString())
                                        End If
                                    Else
                                        Log.Write("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because there is no modifier.")
                                    End If
                                Else
                                    Log.Write("TicketID " + ticket.TicketID.ToString() + "'s field '" + field.name + "' was not included because impersonation flag is false.")
                                End If
                        End Select
                    End If
                Next

                Return result.ToArray()
            End Function

            Private Function GetSalesForceUserID(ByVal email As String) As String
                Dim query As String = "SELECT ID FROM User WHERE email = '" + email + "' "
                Dim qr As QueryResult = Nothing
                qr = Binding.query(query)

                Dim result As String = Nothing
                If qr.size > 0 Then
                    Dim records As sObject() = qr.records
                    result = records(0).Id
                End If

                Return result
            End Function

            Private Function GetDateModifiedOfCollidingCaseToPull(
              ByVal casesToPullAsTickets As List(Of QueryResult),
              ByVal ticketSalesForceID As String) As DateTime?

                Dim result As DateTime?

                For Each casesBatch As QueryResult In casesToPullAsTickets
                    If result Is Nothing Then
                        For Each caseToBring As sObject In casesBatch.records
                            If ticketSalesForceID = caseToBring.Id Then
                                result = GetCaseDateModified(caseToBring.Any)
                                Exit For
                            End If
                        Next
                    Else
                        Exit For
                    End If
                Next

                Return result
            End Function

            Private Function GetCaseDateModified(ByVal caseFields As XmlElement()) As DateTime
                Dim result As DateTime = New DateTime()

                For Each field As XmlElement In caseFields
                    If field.LocalName.Trim().ToLower() = "systemmodstamp" Then
                        result = DateTime.Parse(field.InnerText, Nothing, DateTimeStyles.AdjustToUniversal)
                    End If
                Next

                Return result
            End Function

            Private Sub PushTicketsActionsAsCasesComments(ByVal ticketsActionsToSendAsCasesComments As Actions)

                Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
                crmLinkErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "out", "action")
                Dim crmLinkError As CRMLinkError = Nothing
                Dim pushSucceeded As Boolean = True

                For Each action As Action In ticketsActionsToSendAsCasesComments
                    Dim salesForceCaseComment As sForce.sObject = New sObject()
                    salesForceCaseComment.type = "CaseComment"
                    salesForceCaseComment.Id = action.SalesForceID
                    Dim isNewCaseComment As Boolean = False
                    If action.SalesForceID Is Nothing Then
                        isNewCaseComment = True
                    End If
                    Dim hasParentID As Boolean = False
                    Dim impersonation As Boolean = True
                    salesForceCaseComment.Any = GetSalesForceCaseCommentData(action, isNewCaseComment, hasParentID, impersonation)

                    If hasParentID Then
                        crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(action.ActionID.ToString(), String.Empty)
                        pushSucceeded = True
                        If isNewCaseComment Then
                            Try
                                'Dim result As SaveResult = Binding.create(New sObject() {salesForceCaseComment})(0)
                                Dim result As SaveResult = CreateObject(New sObject() {salesForceCaseComment})(0)
                                If result.errors Is Nothing Then
                                    action.SalesForceID = result.id
                                    Dim actionLogDescription As String = "Sent Action to SalesForce as new CaseComment with ID: '" + result.id + "'."
                                    ActionLogs.AddActionLog(User, ActionLogType.Insert, ReferenceType.Tickets, action.TicketID, actionLogDescription)

                                    If crmLinkError IsNot Nothing Then
                                        crmLinkError.Delete()
                                        crmLinkErrors.Save()
                                    End If
                                ElseIf result.errors(0).message.ToLower() = "entity is deleted" Then
                                    Dim actionLogDescription As String = "Unable to send Action to SalesForce as parent Case was not found. Received error: " + result.errors(0).message
                                    ActionLogs.AddActionLog(User, ActionLogType.Insert, ReferenceType.Tickets, action.TicketID, actionLogDescription)

                                    If crmLinkError IsNot Nothing Then
                                        crmLinkError.Delete()
                                        crmLinkErrors.Save()
                                    End If
                                Else
                                    Log.Write("Creating CaseComment for actionID: " + action.ActionID.ToString() + ", the following exception ocurred: " + result.errors(0).message)
                                    Log.Write("Attempting without impersonation...")
                                    impersonation = False
                                    salesForceCaseComment.Any = GetSalesForceCaseCommentData(action, isNewCaseComment, hasParentID, impersonation)
                                    'result = Binding.create(New sObject() {salesForceCaseComment})(0)
                                    result = CreateObject(New sObject() {salesForceCaseComment})(0)
                                    If result.errors Is Nothing Then
                                        action.SalesForceID = result.id
                                        Dim actionLogDescription As String = "Sent Action to SalesForce as new CaseComment with ID: '" + result.id + "'."
                                        ActionLogs.AddActionLog(User, ActionLogType.Insert, ReferenceType.Tickets, action.TicketID, actionLogDescription)

                                        If crmLinkError IsNot Nothing Then
                                            crmLinkError.Delete()
                                            crmLinkErrors.Save()
                                        End If
                                    ElseIf result.errors(0).message.ToLower() = "entity is deleted" Then
                                        Dim actionLogDescription As String = "Unable to send Action to SalesForce as parent Case was not found. Received error: " + result.errors(0).message
                                        ActionLogs.AddActionLog(User, ActionLogType.Insert, ReferenceType.Tickets, action.TicketID, actionLogDescription)

                                        If crmLinkError IsNot Nothing Then
                                            crmLinkError.Delete()
                                            crmLinkErrors.Save()
                                        End If
                                    Else
                                        Throw (New Exception(result.errors(0).message))
                                    End If
                                End If
                            Catch ex As Exception
                                pushSucceeded = False
                                Log.Write("Creating CaseComment for actionID: " + action.ActionID.ToString() + ", the following exception ocurred: " + ex.Message)
                                Log.Write("sObject.Any property value: " + salesForceCaseComment.Any.ToString())
                                Log.Write(ex.StackTrace)
                                'Throw ex
                                If crmLinkError Is Nothing Then
                                    Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                                    crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                                    crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                                    crmLinkError.CRMType = CRMLinkRow.CRMType
                                    crmLinkError.Orientation = "out"
                                    crmLinkError.ObjectType = "action"
                                    crmLinkError.ObjectID = action.ActionID.ToString()
                                    crmLinkError.ObjectData = JsonConvert.SerializeObject(salesForceCaseComment)
                                    crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                    crmLinkError.OperationType = "create"
                                    newCrmLinkError.Save()
                                Else
                                    crmLinkError.ObjectData = JsonConvert.SerializeObject(salesForceCaseComment)
                                    crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                End If
                            End Try
                        Else
                            Try
                                'Dim result As SaveResult = Binding.update(New sObject() {salesForceCaseComment})(0)
                                Dim result As SaveResult = UpdateObject(New sObject() {salesForceCaseComment})(0)
                                If result.errors Is Nothing Then
                                    Dim actionLogDescription As String = "Updated SalesForce CaseComment ID: '" + action.SalesForceID + "' with action changes."
                                    ActionLogs.AddActionLog(User, ActionLogType.Insert, ReferenceType.Tickets, action.TicketID, actionLogDescription)

                                    If crmLinkError IsNot Nothing Then
                                        crmLinkError.Delete()
                                        crmLinkErrors.Save()
                                    End If
                                ElseIf result.errors(0).message.ToLower() = "entity is deleted" Then
                                    Dim actionLogDescription As String = "Unable to send Action to SalesForce as parent Case was not found. Received error: " + result.errors(0).message
                                    ActionLogs.AddActionLog(User, ActionLogType.Insert, ReferenceType.Tickets, action.TicketID, actionLogDescription)

                                    If crmLinkError IsNot Nothing Then
                                        crmLinkError.Delete()
                                        crmLinkErrors.Save()
                                    End If
                                Else
                                    Log.Write("Updating CaseComment for actionID: " + action.ActionID.ToString() + ", the following exception ocurred: " + result.errors(0).message)
                                    Log.Write("Attempting without impersonation...")
                                    impersonation = False
                                    salesForceCaseComment.Any = GetSalesForceCaseCommentData(action, isNewCaseComment, hasParentID, impersonation)
                                    'result = Binding.update(New sObject() {salesForceCaseComment})(0)
                                    result = UpdateObject(New sObject() {salesForceCaseComment})(0)
                                    If result.errors Is Nothing Then
                                        action.SalesForceID = result.id
                                        Dim actionLogDescription As String = "Updated SalesForce CaseComment ID: '" + action.SalesForceID + "' with action changes."
                                        ActionLogs.AddActionLog(User, ActionLogType.Insert, ReferenceType.Tickets, action.TicketID, actionLogDescription)

                                        If crmLinkError IsNot Nothing Then
                                            crmLinkError.Delete()
                                            crmLinkErrors.Save()
                                        End If
                                    ElseIf result.errors(0).message.ToLower() = "entity is deleted" Then
                                        Dim actionLogDescription As String = "Unable to send Action to SalesForce as parent Case was not found. Received error: " + result.errors(0).message
                                        ActionLogs.AddActionLog(User, ActionLogType.Insert, ReferenceType.Tickets, action.TicketID, actionLogDescription)

                                        If crmLinkError IsNot Nothing Then
                                            crmLinkError.Delete()
                                            crmLinkErrors.Save()
                                        End If
                                    Else
                                        Throw (New Exception(result.errors(0).message))
                                    End If
                                End If
                            Catch ex As Exception
                                pushSucceeded = False
                                Log.Write("Updating CaseComment for actionID: " + action.ActionID.ToString() + ", the following exception ocurred: " + ex.Message)
                                Log.Write("sObject.Any property value: " + salesForceCaseComment.Any.ToString())
                                Log.Write(ex.StackTrace)
                                'Throw ex
                                If crmLinkError Is Nothing Then
                                    Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                                    crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                                    crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                                    crmLinkError.CRMType = CRMLinkRow.CRMType
                                    crmLinkError.Orientation = "out"
                                    crmLinkError.ObjectType = "action"
                                    crmLinkError.ObjectID = action.ActionID.ToString()
                                    crmLinkError.ObjectData = JsonConvert.SerializeObject(salesForceCaseComment)
                                    crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                    crmLinkError.OperationType = "update"
                                    newCrmLinkError.Save()
                                Else
                                    crmLinkError.ObjectData = JsonConvert.SerializeObject(salesForceCaseComment)
                                    crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                End If
                            End Try
                        End If

                        If pushSucceeded Then
                            Dim tickets As Tickets = New Tickets(User)
                            tickets.LoadByTicketID(action.TicketID)

                            Dim userIds As List(Of Integer) = New List(Of Integer)()
                            Dim readTickets As TicketsView = New TicketsView(User)

                            readTickets.LoadUserTicketsByTicketId(action.TicketID, CRMLinkRow.OrganizationID)

                            For Each readTicket As TicketsViewItem In readTickets
                                If readTicket.IsRead Then
                                    userIds.Add(Integer.Parse(readTicket.ViewerID))
                                End If
                            Next

                            action.Collection.UpdateSalesForceSync(action.SalesForceID, DateTime.UtcNow, action.ActionID)

                            tickets(0).DateModifiedBySalesForceSync = DateTime.UtcNow
                            tickets(0).UpdateSalesForceData()

                            Dim userTicketStatuses As UserTicketStatuses = New UserTicketStatuses(User)
                            userTicketStatuses.ResetToUnreadOnSalesForceUpdate(action.TicketID, userIds)
                        End If
                    Else
                        Log.Write("Action with ID: " + action.ActionID.ToString() + " was not pushed because it has no ParentID.")
                    End If
                Next
                crmLinkErrors.Save()
            End Sub

            Private Function GetSalesForceCaseCommentData(
              ByVal action As Action,
              ByVal isNewCaseComment As Boolean,
              ByRef hasParentID As Boolean,
              ByVal impersonation As Boolean) As XmlElement()
                Dim result As New List(Of XmlElement)

                'Dim caseCommentObjectDescription = Binding.describeSObject("CaseComment")
                Dim caseCommentObjectDescription = DescribeObject("CaseComment")
                For Each field As Field In caseCommentObjectDescription.fields
                    Select Case field.name.Trim().ToLower()
                        Case "parentid"
                            Dim caseID As String = Tickets.GetTicket(User, action.TicketID).SalesForceID
                            If caseID IsNot Nothing AndAlso ((isNewCaseComment AndAlso field.createable) OrElse field.updateable) Then
                                result.Add(GetNewXmlElement(field.name, caseID))
                            Else
                                If caseID Is Nothing Then
                                    Exit For
                                Else
                                    Dim message As StringBuilder = New StringBuilder("TicketID " + action.ActionID.ToString() + "'s field '" + field.name + "' was not included because ")
                                    If caseID Is Nothing Then
                                        message.Append("it was null")
                                    End If
                                    If caseID Is Nothing AndAlso Not ((isNewCaseComment AndAlso field.createable) OrElse field.updateable) Then
                                        message.Append(" and ")
                                    End If
                                    If Not ((isNewCaseComment AndAlso field.createable) OrElse field.updateable) Then
                                        message.Append("the field is not updatable.")
                                    End If
                                    Log.Write(message.ToString())
                                End If
                            End If
                            hasParentID = True
                        Case "commentbody"
                            If action.Description IsNot Nothing AndAlso ((isNewCaseComment AndAlso field.createable) OrElse field.updateable) Then
                                result.Add(GetNewXmlElement(field.name, TruncateCaseCommentBody(HtmlUtility.StripHTML(action.Description))))
                            Else
                                Dim message As StringBuilder = New StringBuilder("TicketID " + action.ActionID.ToString() + "'s field '" + field.name + "' was not included because ")
                                If action.Description Is Nothing Then
                                    message.Append("it was null")
                                End If
                                If action.Description Is Nothing AndAlso Not ((isNewCaseComment AndAlso field.createable) OrElse field.updateable) Then
                                    message.Append(" and ")
                                End If
                                If Not ((isNewCaseComment AndAlso field.createable) OrElse field.updateable) Then
                                    message.Append("the field is not updatable.")
                                End If
                                Log.Write(message.ToString())
                            End If
                        Case "createdbyid"
                            If impersonation Then
                                'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                Dim creator As User = Users.GetUser(User, action.CreatorID)
                                If creator IsNot Nothing AndAlso creator.OrganizationID = CRMLinkRow.OrganizationID Then
                                    Dim salesForceID As String = creator.SalesForceID
                                    If salesForceID IsNot Nothing AndAlso ((isNewCaseComment AndAlso field.createable) OrElse field.updateable) Then
                                        result.Add(GetNewXmlElement(field.name, salesForceID))
                                    Else
                                        Dim message As StringBuilder = New StringBuilder("TicketID " + action.ActionID.ToString() + "'s field '" + field.name + "' was not included because ")
                                        If salesForceID Is Nothing Then
                                            message.Append("it was null")
                                        End If
                                        If salesForceID Is Nothing AndAlso Not ((isNewCaseComment AndAlso field.createable) OrElse field.updateable) Then
                                            message.Append(" and ")
                                        End If
                                        If Not ((isNewCaseComment AndAlso field.createable) OrElse field.updateable) Then
                                            message.Append("the field is not updatable.")
                                        End If
                                        Log.Write(message.ToString())
                                    End If
                                Else
                                    Log.Write("TicketID " + action.ActionID.ToString() + "'s field '" + field.name + "' was not included because there is no creator.")
                                End If
                            Else
                                Log.Write("TicketID " + action.ActionID.ToString() + "'s field '" + field.name + "' was not included because the impersonation flag is false.")
                            End If
                        Case "lastmodifiedbyid"
                            If impersonation Then
                                'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                Dim modifier As User = Users.GetUser(User, action.ModifierID)
                                If modifier IsNot Nothing AndAlso modifier.OrganizationID = CRMLinkRow.OrganizationID Then
                                    Dim salesForceID As String = Users.GetUser(User, action.ModifierID).SalesForceID
                                    If salesForceID IsNot Nothing AndAlso ((isNewCaseComment AndAlso field.createable) OrElse field.updateable) Then
                                        result.Add(GetNewXmlElement(field.name, salesForceID))
                                    Else
                                        Dim message As StringBuilder = New StringBuilder("TicketID " + action.ActionID.ToString() + "'s field '" + field.name + "' was not included because ")
                                        If salesForceID Is Nothing Then
                                            message.Append("it was null")
                                        End If
                                        If salesForceID Is Nothing AndAlso Not ((isNewCaseComment AndAlso field.createable) OrElse field.updateable) Then
                                            message.Append(" and ")
                                        End If
                                        If Not ((isNewCaseComment AndAlso field.createable) OrElse field.updateable) Then
                                            message.Append("the field is not updatable.")
                                        End If
                                        Log.Write(message.ToString())
                                    End If
                                Else
                                    Log.Write("TicketID " + action.ActionID.ToString() + "'s field '" + field.name + "' was not included because there is no modifier.")
                                End If
                            Else
                                Log.Write("TicketID " + action.ActionID.ToString() + "'s field '" + field.name + "' was not included because the impersonation flag is false.")
                            End If
                    End Select
                Next

                Return result.ToArray()
            End Function

            Private Function TruncateCaseCommentBody(ByVal input As String) As String
                'The official CaseComment limit is 4000 bytes but while testing a 3994 character count exceeded the limit. So I set it to 3900.
                If input IsNot Nothing AndAlso input.Length > 3600 Then
                    Log.Write("Truncated to 3450 too large case comment.")
                    Return input.Substring(0, 3450) + "... (This comment has been truncated because it exceeds SalesForce max length of 4000 characters. The full comment is available in TeamSupport.)"
                Else
                    Return input
                End If
            End Function

            Private Sub PullCasesAsTickets(ByVal casesToPullAsTickets As List(Of QueryResult), ByVal ticketsToPushAsCases As TicketsView)

                Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
                crmLinkErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "in", "case")
                Dim crmLinkError As CRMLinkError = Nothing

                Dim isUpdate As Boolean = False
                Dim allTicketCustomFields As New CustomFields(User)
                allTicketCustomFields.LoadByReferenceType(CRMLinkRow.OrganizationID, ReferenceType.Tickets)

                For Each casesBatch As QueryResult In casesToPullAsTickets
                    For Each caseToBring As sObject In casesBatch.records

                        crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(caseToBring.Id, String.Empty)

                        Try
                            Dim tickets As New Tickets(User)
                            tickets.LoadBySalesForceID(caseToBring.Id, CRMLinkRow.OrganizationID)

                            Dim ticket As Ticket = Nothing
                            If tickets.Count > 0 Then
                                ticket = tickets(0)
                                Log.Write("Updating ticketID: " + ticket.TicketID.ToString() + ", with caseID: " + caseToBring.Id)
                                isUpdate = True
                            Else
                                tickets = New Tickets(User)
                                ticket = tickets.AddNewTicket()
                                ticket.OrganizationID = CRMLinkRow.OrganizationID

                                Dim types As TicketTypes = New TicketTypes(User)
                                types.LoadAllPositions(CRMLinkRow.OrganizationID)
                                ticket.TicketTypeID = types(0).TicketTypeID

                                Dim statuses As TicketStatuses = New TicketStatuses(User)
                                statuses.LoadByTicketTypeID(ticket.TicketTypeID)
                                ticket.TicketStatusID = statuses(0).TicketStatusID

                                Dim severities As TicketSeverities = New TicketSeverities(User)
                                severities.LoadAllPositions(CRMLinkRow.OrganizationID)
                                ticket.TicketSeverityID = severities(0).TicketSeverityID

                                ticket.SalesForceID = caseToBring.Id
                                ticket.TicketSource = "SalesForce"
                                Dim creator As LoginUser = GetCreator(caseToBring)
                                If creator IsNot Nothing Then
                                    tickets.LoginUser = creator
                                End If

                                Dim caseNumber As String = GetCaseNumber(caseToBring.Id)
                                tickets.ActionLogInstantMessage = If(String.IsNullOrEmpty(caseNumber), String.Format("SalesForce Case ID: {0} Created In TeamSupport With Ticket Number ", caseToBring.Id), String.Format("SalesForce Case Number: '{0}' Created In TeamSupport With Ticket Number ", caseNumber))
                                tickets.Save()
                                Log.Write("Creating ticketID: " + ticket.TicketID.ToString() + ", with caseID: " + caseToBring.Id + ", case number: " + caseNumber)
                            End If

                            Dim ticketValuesChanged As Boolean = False
                            Dim isNotCollidingWithATicketToPush As Boolean = False

                            AssignCaseValuesToTicket(ticket, caseToBring, ticketValuesChanged, isUpdate, ticketsToPushAsCases, isNotCollidingWithATicketToPush, allTicketCustomFields)

                            If ticketValuesChanged AndAlso isNotCollidingWithATicketToPush Then
                                ticket.DateModifiedBySalesForceSync = DateTime.UtcNow
                                ticket.Collection.Save()
                                If isUpdate Then
                                    Dim caseNumber As String = GetCaseNumber(ticket.SalesForceID)
                                    Dim actionLogDescription As String = If(String.IsNullOrEmpty(caseNumber), String.Format("Updated Ticket with SalesForce Case ID: '{0}' changes.", ticket.SalesForceID), String.Format("Updated Ticket with SalesForce Case Number: '{0}' changes.", caseNumber))
                                    ActionLogs.AddActionLog(User, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, actionLogDescription)
                                    Log.Write("Updated Ticket with SalesForce Case ID: '" + ticket.SalesForceID + "', case number: '" + caseNumber + "' changes.")
                                End If
                            Else
                                Dim errorMessageSuffix As New StringBuilder()

                                If Not ticketValuesChanged Then
                                    errorMessageSuffix.Append("have not changed")
                                End If

                                If Not isNotCollidingWithATicketToPush Then
                                    If errorMessageSuffix.Length > 0 Then
                                        errorMessageSuffix.Append(" and ")
                                    End If
                                    errorMessageSuffix.Append("it is colliding with a ticket being pushed to SalesForce (If DatesModified missing above SF object did not include it)")
                                End If

                                errorMessageSuffix.Append(".")

                                Log.Write("ticketID: " + ticket.TicketID.ToString() + " values were not updated because " + errorMessageSuffix.ToString())
                            End If

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
                                crmLinkError.ObjectType = "case"
                                crmLinkError.ObjectID = caseToBring.Id
                                crmLinkError.ObjectData = JsonConvert.SerializeObject(caseToBring)
                                crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                If isUpdate Then
                                    crmLinkError.OperationType = "update"
                                Else
                                    crmLinkError.OperationType = "create"
                                End If
                                newCrmLinkError.Save()
                            Else
                                crmLinkError.ObjectData = JsonConvert.SerializeObject(caseToBring)
                                crmLinkError.Exception = ex.ToString() + ex.StackTrace
                            End If

                        End Try
                    Next
                Next
                crmLinkErrors.Save()
            End Sub

            Private Function GetCreator(ByRef caseToBring As sObject) As LoginUser
                Dim result As LoginUser = Nothing

                Try
                    Dim salesForceCreator As SalesForceCustomer = New SalesForceCustomer(User)

                    For Each caseField As Xml.XmlElement In caseToBring.Any
                        Dim value As String = caseField.InnerText
                        If caseField.ChildNodes.Count > 1 Then
                            value = caseField.LastChild.InnerText
                        End If

                        Select Case caseField.LocalName.Trim().ToLower()
                            Case "createdbyid"
                                salesForceCreator.ContactID = value
                            Case "createdby"
                                salesForceCreator.ContactEmail = value
                        End Select
                    Next

                    Dim newUser As Users = New Users(User)
                    newUser.LoadBySalesForceID(salesForceCreator.ContactID, CRMLinkRow.OrganizationID)
                    If newUser.Count = 0 Then
                        newUser.LoadByEmailIncludingDeleted(salesForceCreator.ContactEmail, CRMLinkRow.OrganizationID)
                        If newUser.Count > 0 Then
                            newUser(0).SalesForceID = salesForceCreator.ContactID
                            newUser(0).MarkDeleted = False
                            newUser.Save()
                        End If
                    End If

                    If newUser.Count > 0 Then
                        result = New LoginUser(User.ConnectionString, newUser(0).UserID, newUser(0).OrganizationID, Nothing)
                    End If

                Catch ex As Exception
                    Log.Write("The following exception was thrown getting the creator.")
                    Log.Write(ex.ToString())
                    Log.Write(ex.StackTrace)
                End Try

                Return result
            End Function

            Private Sub AssignCaseValuesToTicket(
              ByRef ticket As Ticket,
              ByRef caseToBring As sObject,
              ByRef ticketValuesChanged As Boolean,
              ByVal isUpdate As Boolean,
              ByVal ticketsToPushAsCases As TicketsView,
              ByRef isNotCollidingWithATicketToPush As Boolean,
              ByRef allTicketCustomFields As CustomFields)

                Dim customFields As New CRMLinkFields(User)
                customFields.LoadByObjectType("Ticket", CRMLinkRow.CRMLinkID)

                Dim salesForceCustomer As SalesForceCustomer = New SalesForceCustomer(User)
                Dim salesForceOwner As SalesForceCustomer = New SalesForceCustomer(User)
                Dim salesForceCreator As SalesForceCustomer = New SalesForceCustomer(User)
                Dim salesForceModifier As SalesForceCustomer = New SalesForceCustomer(User)

                If Not isUpdate OrElse ticketsToPushAsCases Is Nothing OrElse ticketsToPushAsCases.Count = 0 Then
                    isNotCollidingWithATicketToPush = True
                End If

                For Each caseField As Xml.XmlElement In caseToBring.Any
                    Dim value As String = caseField.InnerText
                    If caseField.ChildNodes.Count > 1 Then
                        value = caseField.LastChild.InnerText
                    End If

                    If Not isNotCollidingWithATicketToPush AndAlso caseField.LocalName.Trim().ToLower() = "systemmodstamp" Then
                        Dim caseToBringDateModified As DateTime = DateTime.Parse(value, Nothing, DateTimeStyles.AdjustToUniversal)
                        Dim collidingTicketToPush As TicketsViewItem = ticketsToPushAsCases.FindBySalesForceID(caseToBring.Id)
                        If collidingTicketToPush IsNot Nothing AndAlso collidingTicketToPush.DateModifiedUtc > caseToBringDateModified Then
                            Log.Write("A ticket to push was found with a DateModified (" + collidingTicketToPush.DateModifiedUtc.ToString() + ") greater than the case to pull. (" + caseToBringDateModified.ToString() + ")")
                            Exit For
                        Else
                            isNotCollidingWithATicketToPush = True
                        End If
                    End If

                    Dim cRMLinkField As CRMLinkField = customFields.FindByCRMFieldName(caseField.LocalName)
                    If cRMLinkField IsNot Nothing Then
                        Try
                            If cRMLinkField.CustomFieldID IsNot Nothing Then
                                'If the mapped custom field ticket type does not match the ticket update the cRMLinkField with the matching type mapping if such exists.
                                Dim customField As CustomField = allTicketCustomFields.FindByCustomFieldID(cRMLinkField.CustomFieldID)
                                If customField IsNot Nothing AndAlso customField.AuxID <> ticket.TicketTypeID Then
                                    Dim rightTypeCRMLinkField As CRMLinkField = GetRightTicketTypeMapping(CRMLinkRow.CRMLinkID, caseField.LocalName, ticket.TicketTypeID)
                                    If rightTypeCRMLinkField IsNot Nothing Then
                                        cRMLinkField = rightTypeCRMLinkField
                                    End If
                                End If
                                Dim translatedFieldValue As String = TranslateFieldValue(cRMLinkField.CustomFieldID, ticket.TicketID, value)
                                Dim findCustom As New CustomValues(User)
                                Dim thisCustom As CustomValue

                                findCustom.LoadByFieldID(cRMLinkField.CustomFieldID, ticket.TicketID)
                                If findCustom.Count > 0 Then
                                    thisCustom = findCustom(0)

                                Else
                                    thisCustom = (New CustomValues(User)).AddNewCustomValue()
                                    thisCustom.CustomFieldID = cRMLinkField.CustomFieldID
                                    thisCustom.RefID = ticket.TicketID
                                End If

                                If thisCustom.Value <> translatedFieldValue Then
                                    thisCustom.Value = translatedFieldValue
                                    thisCustom.Collection.Save()
                                    ticketValuesChanged = True
                                End If
                            ElseIf cRMLinkField.TSFieldName IsNot Nothing Then
                                Try
                                    'TicketNumber should not be updatable, if it is mapped (a project to remove non updatable fields off the mappings coming soon) then skip it.
                                    If cRMLinkField.TSFieldName.Trim().ToLower() = "ticketnumber" Then
                                        Throw New Exception
                                    End If

                                    If ticket.Row(cRMLinkField.TSFieldName) <> TranslateFieldValue(value, ticket.Row(cRMLinkField.TSFieldName).GetType().Name) Then
                                        ticket.Row(cRMLinkField.TSFieldName) = TranslateFieldValue(value, ticket.Row(cRMLinkField.TSFieldName).GetType().Name)
                                        ticketValuesChanged = True
                                    End If
                                Catch ex As Exception
                                    'An exception here is probably caused by a ticketview field mapped that does not exists in the tickets table.
                                    Select Case cRMLinkField.TSFieldName.Trim().ToLower()
                                        Case "productname"
                                            Dim products As Products = New Products(User)
                                            products.LoadByOrganizationID(CRMLinkRow.OrganizationID)
                                            'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                            Dim product As Product = products.FindByName(value)
                                            If product IsNot Nothing Then
                                                If ticket.ProductID <> product.ProductID Then
                                                    ticket.ProductID = product.ProductID
                                                    ticketValuesChanged = True
                                                End If
                                            Else
                                                Log.Write(caseField.LocalName + " not updated because value: '" + value + "' was not found.")
                                            End If
                                        Case "reportedversion"
                                            Dim productVersions As ProductVersions = New ProductVersions(User)
                                            productVersions.LoadByProductID(ticket.ProductID)
                                            'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                            Dim productVersion As ProductVersion = productVersions.FindByVersionNumber(value, ticket.ProductID)
                                            If productVersion IsNot Nothing Then
                                                If ticket.ReportedVersionID <> productVersion.ProductVersionID Then
                                                    ticket.ReportedVersionID = productVersion.ProductVersionID
                                                    ticketValuesChanged = True
                                                End If
                                            Else
                                                Log.Write(caseField.LocalName + " not updated because value: '" + value + "' was not found.")
                                            End If
                                        Case "solvedversion"
                                            Dim productVersions As ProductVersions = New ProductVersions(User)
                                            productVersions.LoadByProductID(ticket.ProductID)
                                            'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                            Dim productVersion As ProductVersion = productVersions.FindByVersionNumber(value, ticket.ProductID)
                                            If productVersion IsNot Nothing Then
                                                If ticket.SolvedVersionID <> productVersion.ProductVersionID Then
                                                    ticket.SolvedVersionID = productVersion.ProductVersionID
                                                    ticketValuesChanged = True
                                                End If
                                            Else
                                                Log.Write(caseField.LocalName + " not updated because value: '" + value + "' was not found.")
                                            End If
                                        Case "groupname"
                                            Dim groups As Groups = New Groups(User)
                                            groups.LoadByOrganizationID(CRMLinkRow.OrganizationID)
                                            'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                            Dim group As Group = groups.FindByName(value)
                                            If group IsNot Nothing Then
                                                If ticket.GroupID <> group.GroupID Then
                                                    ticket.GroupID = group.GroupID
                                                    ticketValuesChanged = True
                                                End If
                                            Else
                                                Log.Write(caseField.LocalName + " not updated because value: '" + value + "' was not found.")
                                            End If
                                        Case "tickettypename"
                                            Dim types As TicketTypes = New TicketTypes(User)
                                            types.LoadAllPositions(CRMLinkRow.OrganizationID)
                                            'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                            Dim type As TicketType = types.FindByName(value)
                                            If type IsNot Nothing Then
                                                If ticket.TicketTypeID <> type.TicketTypeID Then
                                                    ticket.TicketTypeID = type.TicketTypeID
                                                    ticketValuesChanged = True
                                                End If
                                            Else
                                                Log.Write(caseField.LocalName + " not updated because value: '" + value + "' was not found.")
                                            End If
                                        Case "status"
                                            Dim statuses As TicketStatuses = New TicketStatuses(User)
                                            statuses.LoadAllPositions(ticket.TicketTypeID)
                                            'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                            Dim status As TicketStatus = statuses.FindByName(value, ticket.TicketTypeID)
                                            If status IsNot Nothing Then
                                                If ticket.TicketStatusID <> status.TicketStatusID Then
                                                    ticket.TicketStatusID = status.TicketStatusID
                                                    ticketValuesChanged = True
                                                End If
                                            Else
                                                Log.Write(caseField.LocalName + " not updated because value: '" + value + "' was not found.")
                                            End If
                                        Case "severity"
                                            Dim severities As TicketSeverities = New TicketSeverities(User)
                                            severities.LoadAllPositions(CRMLinkRow.OrganizationID)
                                            'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                            Dim severity As TicketSeverity = severities.FindByName(value)
                                            If severity IsNot Nothing Then
                                                If ticket.TicketSeverityID <> severity.TicketSeverityID Then
                                                    ticket.TicketSeverityID = severity.TicketSeverityID
                                                    ticketValuesChanged = True
                                                End If
                                            Else
                                                Log.Write(caseField.LocalName + " not updated because value: '" + value + "' was not found.")
                                            End If
                          'Any change made for the UserName will be overriden by the AssignCustomerToTicket call made ahead.
                          'If ever needed a bigger change will be required to get it working.
                          'Case "username"
                          'Tags, Contacts and Customers require also a bigger change. Therefore I'll implement if ever needed.
                          'Case "tags"
                          'Case "contacts"
                          'Case "customers"
                                        Case "statusposition", "severityposition", "isclosed", "daysclosed", "daysopened", "closername", "creatorname", "modifiername", "hoursspent", "slawarninghours", "slaviolationhours", "minssincecreated", "dayssincecreated", "minssincemodified", "dayssincemodified", "slaviolationdate", "slawarningdate", "ticketnumber"
                                            Throw (New Exception("This is a read only field"))
                                        Case Else
                                            Throw (New Exception("This must be a new field in the ReportTableFields. Add support for it."))
                                    End Select
                                End Try
                            End If
                        Catch mappingException As Exception
                            Log.Write(
                              "The following exception was caught mapping the ticket field """ &
                              caseField.LocalName &
                              """ with """ &
                              cRMLinkField.TSFieldName &
                              """: " &
                              mappingException.Message &
                              " " & mappingException.StackTrace)
                        End Try
                    Else
                        Select Case caseField.LocalName.Trim().ToLower()
                            Case "contactid"
                                salesForceCustomer.ContactID = value
                            Case "contact"
                                salesForceCustomer.ContactEmail = value
                            Case "accountid"
                                salesForceCustomer.AccountID = value
                            Case "type"
                                Dim types As TicketTypes = New TicketTypes(User)
                                types.LoadAllPositions(CRMLinkRow.OrganizationID)
                                'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                Dim type As TicketType = types.FindByName(value)
                                If type IsNot Nothing Then
                                    If ticket.TicketTypeID <> type.TicketTypeID Then
                                        ticket.TicketTypeID = type.TicketTypeID
                                        ticketValuesChanged = True
                                    End If
                                Else
                                    Log.Write(caseField.LocalName + " not updated because value: '" + value + "' was not found.")
                                End If
                            Case "status"
                                'In order to process the status the type needs to be processed first.
                                'Assumming that type will always be processed first, we'll only process the status if the type exists
                                'Lets check in debugging what is the default value of the TicketTypeID
                                'If ticket.TicketTypeID IsNot Nothing Then
                                Dim statuses As TicketStatuses = New TicketStatuses(User)
                                statuses.LoadAllPositions(ticket.TicketTypeID)
                                'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                Dim status As TicketStatus = statuses.FindByName(value, ticket.TicketTypeID)
                                If status IsNot Nothing Then
                                    If ticket.TicketStatusID <> status.TicketStatusID Then
                                        ticket.TicketStatusID = status.TicketStatusID
                                        ticketValuesChanged = True
                                    End If
                                Else
                                    Log.Write(caseField.LocalName + " not updated because value: '" + value + "' was not found.")
                                End If
                      'End If
                            Case "subject"
                                'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                If ticket.Name <> value Then
                                    ticket.Name = value
                                    ticketValuesChanged = True
                                End If
                            Case "priority"
                                Dim severities As TicketSeverities = New TicketSeverities(User)
                                severities.LoadAllPositions(CRMLinkRow.OrganizationID)
                                'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                Dim severity As TicketSeverity = severities.FindByName(value)
                                If severity IsNot Nothing Then
                                    If ticket.TicketSeverityID <> severity.TicketSeverityID Then
                                        ticket.TicketSeverityID = severity.TicketSeverityID
                                        ticketValuesChanged = True
                                    End If
                                Else
                                    Log.Write(caseField.LocalName + " not updated because value: '" + value + "' was not found.")
                                End If
                            Case "description"
                                Dim action As Action = Actions.GetTicketDescription(User, ticket.TicketID)
                                If action IsNot Nothing Then
                                    'In order to prevent loosing the teamsupport action style no update is being made to actions.
                                    'If action.Description <> value Then
                                    '  action.Description = value
                                    '  ticketValuesChanged = True
                                    '  action.Collection.Save()
                                    'End If
                                Else
                                    action = New Actions(ticket.Collection.LoginUser).AddNewAction()
                                    action.ActionTypeID = Nothing
                                    action.Name = "Description"
                                    action.SystemActionTypeID = SystemActionType.Description
                                    action.ActionSource = ticket.TicketSource
                                    action.Description = value
                                    action.TicketID = ticket.TicketID
                                    ticketValuesChanged = True
                                    action.Collection.Save()
                                End If
                            Case "closeddate"
                                Dim closedDate As Date? = Nothing
                                'Dim equivalentTypeValueInSalesForce As String = GetEquivalentValueInSalesForce(field.name, ticket.TicketTypeID)
                                If Date.TryParse(value, closedDate) Then
                                    If ticket.DateClosed <> closedDate Then
                                        ticket.DateClosed = closedDate
                                        ticketValuesChanged = True
                                    End If
                                Else
                                    Log.Write(caseField.LocalName + " not updated because value: '" + value + "' did not parse as date.")
                                End If
                            Case "ownerid"
                                salesForceOwner.ContactID = value
                            Case "owner"
                                salesForceOwner.ContactEmail = value
                            Case "createdbyid"
                                salesForceCreator.ContactID = value
                            Case "createdby"
                                salesForceCreator.ContactEmail = value
                            Case "lastmodifiedbyid"
                                salesForceModifier.ContactID = value
                            Case "lastmodifiedby"
                                salesForceModifier.ContactEmail = value
                        End Select
                    End If
                Next

                If isNotCollidingWithATicketToPush Then
                    Try
                        AssignCustomerToTicket(salesForceCustomer, ticket, ticketValuesChanged)
                    Catch ex As Exception
                        Log.Write("Exception in AssignCustomerToTicket.")
                        Log.Write(ex.StackTrace)
                        Throw ex
                    End Try

                    'Pulled out to prevent case described in ticket 10781
                    'Try
                    '  AssignUserToTicket(salesForceOwner, ticket, "UserID", ticketValuesChanged)
                    'Catch ex As Exception
                    '  Log.Write("Exception in AssignOwnerToTicket.")
                    '  Log.Write(ex.StackTrace)
                    '  Throw ex
                    'End Try

                    'Pulled out to prevent case described in ticket 10781
                    'Try
                    '  AssignUserToTicket(salesForceCreator, ticket, "CreatorID", ticketValuesChanged)
                    'Catch ex As Exception
                    '  Log.Write("Exception in AssignCreatorToTicket.")
                    '  Log.Write(ex.StackTrace)
                    '  Throw ex
                    'End Try

                    Try
                        AssignUserToTicket(salesForceModifier, ticket, "ModifierID", ticketValuesChanged)
                    Catch ex As Exception
                        Log.Write("Exception in AssignModifierToTicket.")
                        Log.Write(ex.StackTrace)
                        Throw ex
                    End Try
                End If
            End Sub

            Private Function GetRightTicketTypeMapping(ByRef cRMLinkID As Integer, ByRef cRMFieldName As String, ByRef ticketTypeID As Integer) As CRMLinkField
                Dim result As CRMLinkField = Nothing
                Dim rightTicketTypeMapping As CRMLinkFields = New CRMLinkFields(User)
                rightTicketTypeMapping.LoadByCRMFieldNameAndTicketTypeID(cRMLinkID, cRMFieldName, ticketTypeID)
                If rightTicketTypeMapping.Count > 0 Then
                    result = rightTicketTypeMapping(0)
                End If
                Return result
            End Function

            Private Sub AssignCustomerToTicket(ByRef customer As SalesForceCustomer, ByRef ticket As Ticket, ByRef ticketValuesChanged As Boolean)
				If Not String.IsNullOrEmpty(customer.ContactID) AndAlso Not String.IsNullOrEmpty(customer.AccountID) Then
					Dim contact As ContactsViewItem = Nothing
					Dim teamSupportUser As User = Nothing
					If CheckIfCustomerExistsInTicket(customer, ticket, contact) Then
						Dim existingContact As ContactsView = New ContactsView(User)
						existingContact.LoadSentToSalesForce(ticket.TicketID)
						If existingContact.Count > 0 Then
							If existingContact(0).UserID <> contact.UserID Then
								ticket.Collection.SetUserAsSentToSalesForce(contact.UserID, ticket.TicketID)
								Log.Write("Set existing in ticket userID: " + contact.UserID.ToString() + " as SentToSalesForce.")
								ticketValuesChanged = True
							End If
						Else
							ticket.Collection.SetUserAsSentToSalesForce(contact.UserID, ticket.TicketID)
							Log.Write("Set existing in ticket userID: " + contact.UserID.ToString() + " as SentToSalesForce.")
							ticketValuesChanged = True
						End If
					Else
						Dim organization As Organization = Nothing
						If CheckIfUserExistsInOrganization(customer, teamSupportUser, organization) Then
							ticket.Collection.AddContact(teamSupportUser.UserID, ticket.TicketID)
							ticket.Collection.SetUserAsSentToSalesForce(teamSupportUser.UserID, ticket.TicketID)
							Log.Write("Added in ticket userID: " + teamSupportUser.UserID.ToString() + " as SentToSalesForce.")
							ticketValuesChanged = True
						Else
							Log.Write("Contact was not set as customer in ticket because it does not exists in TeamSupport.")
						End If
					End If
				ElseIf customer.AccountID IsNot Nothing Then
					Dim organization As Organization = Nothing
                    If CheckIfOrganizationExistsInTicket(customer, ticket, organization) Then
                        Dim existingOrganization As Organizations = New Organizations(User)
                        existingOrganization.LoadSentToSalesForce(ticket.TicketID)
                        If existingOrganization.Count > 0 Then
                            If existingOrganization(0).OrganizationID <> organization.OrganizationID Then
                                ticket.Collection.SetOrganizationAsSentToSalesForce(organization.OrganizationID, ticket.TicketID)
                                Log.Write("Set existing in ticket organizationID: " + organization.OrganizationID.ToString() + " as SentToSalesForce.")
                                ticketValuesChanged = True
                            End If
                        Else
                            ticket.Collection.SetOrganizationAsSentToSalesForce(organization.OrganizationID, ticket.TicketID)
                            Log.Write("Set existing in ticket organizationID: " + organization.OrganizationID.ToString() + " as SentToSalesForce.")
                            ticketValuesChanged = True
                        End If
                    ElseIf CheckIfOrganizationExistsInParentOrganization(customer, ticket, organization) Then
                        ticket.Collection.AddOrganization(organization.OrganizationID, ticket.TicketID)
                        ticket.Collection.SetOrganizationAsSentToSalesForce(organization.OrganizationID, ticket.TicketID)
                        Log.Write("Added in ticket organizationID: " + organization.OrganizationID.ToString() + " as SentToSalesForce.")
                        ticketValuesChanged = True
                    Else
                        Log.Write("Contact organization was not set as customer in ticket because it does not exists in TeamSupport.")
                    End If
                End If
            End Sub

            Private Function CheckIfCustomerExistsInTicket(ByRef customer As SalesForceCustomer, ByRef ticket As Ticket, ByRef contact As ContactsViewItem) As Boolean
                Dim result As Boolean = False

                Dim contacts As ContactsView = New ContactsView(User)
                contacts.LoadByTicketID(ticket.TicketID)

                If contacts.Count > 0 Then
                    contact = contacts.FindBySalesForceID(customer.ContactID, customer.AccountID)
                    If contact Is Nothing Then
                        contact = contacts.FindByEmail(customer.ContactEmail, customer.AccountID)
                        If contact IsNot Nothing Then
                            result = True
                        End If
                    Else
                        result = True
                    End If
                End If

                Return result
            End Function

            Private Function CheckIfOrganizationExistsInTicket(ByRef customer As SalesForceCustomer, ByRef ticket As Ticket, ByRef organization As Organization) As Boolean
                Dim result As Boolean = False

                Dim organizations As Organizations = New Organizations(User)
                organizations.LoadByTicketID(ticket.TicketID)

                If organizations.Count > 0 Then
                    organization = organizations.FindByCRMLinkID(customer.AccountID)
                    If organization IsNot Nothing Then
                        result = True
                    End If
                End If

                Return result
            End Function

            Private Function CheckIfUserExistsInOrganization(ByRef customer As SalesForceCustomer, ByRef teamSupportUser As User, ByRef organization As Organization) As Boolean
                Dim result As Boolean = False

                Dim organizations As Organizations = New Organizations(User)
                organizations.LoadByCRMLinkID(customer.AccountID, CRMLinkRow.OrganizationID)
                If organizations.Count > 0 Then
                    organization = organizations(0)
                    Dim teamSupportUserCollection As Users = New Users(User)
                    teamSupportUserCollection.LoadBySalesForceID(customer.ContactID, organization.OrganizationID)
                    If teamSupportUserCollection.Count > 0 Then
                        teamSupportUser = teamSupportUserCollection(0)
                        result = True
                    Else
                        teamSupportUserCollection.LoadByEmailIncludingDeleted(customer.ContactEmail, organization.OrganizationID)
                        If teamSupportUserCollection.Count > 0 Then
                            teamSupportUser = teamSupportUserCollection(0)
                            teamSupportUser.SalesForceID = customer.ContactID
                            teamSupportUserCollection.Save()
                            result = True
                        End If
                    End If
                End If

                Return result
            End Function

            Private Sub AddOrganization(ByVal salesForceAccountID As String, ByRef organization As Organization)
                'Dim query As String = "SELECT ID FROM Contact WHERE email = '" + email + "' "
                'If accountID IsNot Nothing Then
                '  query = query + "AND Account.ID = '" + accountID + "'"
                'End If
                'Dim qr As QueryResult = Nothing
                'qr = Binding.query(query)

                'Dim result As String = Nothing
                'If qr.size > 0 Then
                '  Dim records As sObject() = qr.records
                '  result = records(0).Id
                'End If

                'Return result
            End Sub

            Private Sub GetUserPhoneNumbers(ByVal userID As Integer, ByRef phoneNumber As PhoneNumber, ByRef mobileNumber As PhoneNumber, ByRef faxNumber As PhoneNumber)
                Dim phoneNumbers As PhoneNumbers = New PhoneNumbers(User)
                phoneNumbers.LoadByID(userID, ReferenceType.Users)

                Dim phoneTypes As PhoneTypes = New PhoneTypes(User)
                phoneTypes.LoadAllPositions(CRMLinkRow.OrganizationID)

                Dim phoneType As PhoneType = phoneTypes.FindByName("Phone")
                If phoneType IsNot Nothing Then
                    phoneNumber = phoneNumbers.FindByPhoneTypeID(phoneType.PhoneTypeID)
                End If
                If phoneNumber Is Nothing Then
                    Dim workType As PhoneType = phoneTypes.FindByName("Work")
                    If workType IsNot Nothing Then
                        phoneNumber = phoneNumbers.FindByPhoneTypeID(workType.PhoneTypeID)
                    End If
                End If

                Dim mobileType As PhoneType = phoneTypes.FindByName("Mobile")
                If mobileType IsNot Nothing Then
                    mobileNumber = phoneNumbers.FindByPhoneTypeID(mobileType.PhoneTypeID)
                End If

                Dim faxType As PhoneType = phoneTypes.FindByName("Fax")
                If faxType IsNot Nothing Then
                    faxNumber = phoneNumbers.FindByPhoneTypeID(faxType.PhoneTypeID)
                End If
            End Sub

            Private Function CheckIfOrganizationExistsInParentOrganization(ByRef customer As SalesForceCustomer, ByRef ticket As Ticket, ByRef organization As Organization) As Boolean
                Dim result As Boolean = False

                Dim organizations As Organizations = New Organizations(User)
                organizations.LoadByCRMLinkID(customer.AccountID, CRMLinkRow.OrganizationID)

                If organizations.Count > 0 Then
                    organization = organizations(0)
                    result = True
                End If

                Return result
            End Function

            Private Sub AddCustomer()
                'Pending implementation
            End Sub

            Private Sub AssignUserToTicket(ByRef customer As SalesForceCustomer, ByRef ticket As Ticket, ByRef field As String, ByRef ticketValuesChanged As Boolean)
                Dim currentUser As Users = New Users(User)
                Select Case field
                    Case "UserID"
                        If ticket.UserID IsNot Nothing Then
                            currentUser.LoadByUserID(ticket.UserID)
                        End If
                    Case "CreatorID"
                        currentUser.LoadByUserID(ticket.CreatorID)
                    Case "ModifierID"
                        currentUser.LoadByUserID(ticket.ModifierID)
                End Select

                If currentUser.Count = 0 OrElse currentUser(0).SalesForceID <> customer.ContactID Then
                    Dim newUser As Users = New Users(User)
                    newUser.LoadBySalesForceID(customer.ContactID, CRMLinkRow.OrganizationID)
                    If newUser.Count = 0 Then
                        newUser.LoadByEmailIncludingDeleted(customer.ContactEmail, CRMLinkRow.OrganizationID)
                        If newUser.Count > 0 Then
                            newUser(0).SalesForceID = customer.ContactID
                            newUser(0).MarkDeleted = False
                            newUser.Save()
                        End If
                    End If

                    If newUser.Count > 0 Then
                        Select Case field
                            Case "UserID"
                                ticket.UserID = newUser(0).UserID
                            Case "CreatorID"
                                ticket.CreatorID = newUser(0).UserID
                            Case "ModifierID"
                                ticket.Collection.LoginUser = New LoginUser(User.ConnectionString, newUser(0).UserID, newUser(0).OrganizationID, Nothing)
                        End Select
                        Log.Write(field + " changed to " + newUser(0).UserID.ToString())
                        ticketValuesChanged = True
                    Else
                        Log.Write(field + " wasn't changed.")
                    End If

                End If
            End Sub

            Private Sub PullCasesCommentsAsTicketsActions(ByVal casesCommentsToPullAsTickets As List(Of QueryResult))
                Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
                crmLinkErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "in", "caseComment")
                Dim crmLinkError As CRMLinkError = Nothing

                For Each casesCommentsBatch As QueryResult In casesCommentsToPullAsTickets
                    For Each caseCommentToBring As sObject In casesCommentsBatch.records
                        crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(caseCommentToBring.Id, String.Empty)

                        Try
                            Dim actions As New Actions(User)
                            actions.LoadBySalesForceID(caseCommentToBring.Id, CRMLinkRow.OrganizationID)
                            Dim action As Action = Nothing

                            If actions.Count > 0 Then
                                'action = actions(0)
                                'Log.Write("Updating actionID: " + action.ActionID.ToString() + ", with caseCommentID: " + caseCommentToBring.Id)
                                'To prevent loosing formatting of existing actions in TeamSupport we do not update SalesForce CaseComments changes.
                                Continue For
                            Else
                                actions = New Actions(User)
                                action = actions.AddNewAction()
                                action.SalesForceID = caseCommentToBring.Id
                                action.SystemActionTypeID = SystemActionType.Custom
                                Dim actionTypes As ActionTypes = New ActionTypes(User)
                                actionTypes.LoadAllPositions(CRMLinkRow.OrganizationID)
                                If actionTypes.Count > 0 Then
                                    Dim actionType As ActionType = actionTypes.FindByName("Comment")
                                    If actionType IsNot Nothing Then
                                        action.ActionTypeID = actionType.ActionTypeID
                                    Else
                                        action.ActionTypeID = actionTypes(0).ActionTypeID
                                    End If
                                End If
                                Log.Write("Creating new action with caseCommentID: " + caseCommentToBring.Id)
                            End If

                            Dim ticket As Tickets = New Tickets(User)
                            AssignCaseCommentsValuesToTicketAction(action, caseCommentToBring, ticket)

                            action.Collection.ActionLogInstantMessage = "SalesForce CaseComment ID: " + caseCommentToBring.Id + " Created In TeamSupport Action "

                            If ticket.Count > 0 Then
                                ticket(0).DateModifiedBySalesForceSync = DateTime.UtcNow
                            End If

                            action.DateModifiedBySalesForceSync = DateTime.UtcNow
                            action.Collection.Save()

                            If ticket.Count > 0 Then
                                ticket(0).UpdateSalesForceData()
                            End If

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
                                crmLinkError.ObjectType = "caseComment"
                                crmLinkError.ObjectID = caseCommentToBring.Id
                                crmLinkError.ObjectData = JsonConvert.SerializeObject(caseCommentToBring)
                                crmLinkError.Exception = ex.ToString() + ex.StackTrace
                                crmLinkError.OperationType = "create"
                                newCrmLinkError.Save()
                            Else
                                crmLinkError.ObjectData = JsonConvert.SerializeObject(caseCommentToBring)
                                crmLinkError.Exception = ex.ToString() + ex.StackTrace
                            End If

                        End Try

                    Next
                Next
                crmLinkErrors.Save()

            End Sub

            Private Sub AssignCaseCommentsValuesToTicketAction(ByRef action As Action, ByRef caseCommentToBring As sObject, ByRef ticket As Tickets)
                Dim salesForceCreator As SalesForceCustomer = New SalesForceCustomer(User)
                Dim salesForceModifier As SalesForceCustomer = New SalesForceCustomer(User)

                For Each caseField As Xml.XmlElement In caseCommentToBring.Any
                    Dim value As String = caseField.InnerText
                    If caseField.ChildNodes.Count > 1 Then
                        value = caseField.LastChild.InnerText
                    End If

                    Select Case caseField.LocalName.Trim().ToLower()
                        Case "parentid"
                            ticket.LoadBySalesForceID(value, CRMLinkRow.OrganizationID)
                            If ticket.Count > 0 Then
                                action.TicketID = ticket(0).TicketID
                            Else
                                Log.Write(caseField.LocalName + " not updated because no ticket with SalesForceID: '" + value + "' was found.")
                            End If
                        Case "commentbody"
                            action.Description = value
                        Case "createdbyid"
                            salesForceCreator.ContactID = value
                        Case "createdby"
                            salesForceCreator.ContactEmail = value
                        Case "lastmodifiedbyid"
                            salesForceModifier.ContactID = value
                        Case "lastmodifiedby"
                            salesForceModifier.ContactEmail = value
                    End Select
                Next

                Try
                    AssignUserToTicketAction(salesForceCreator, action, "CreatorID")
                Catch ex As Exception
                    Log.Write("Exception in AssignCreatorToTicketAction.")
                    Log.Write(ex.StackTrace)
                    Throw ex
                End Try

                Try
                    AssignUserToTicketAction(salesForceModifier, action, "ModifierID")
                Catch ex As Exception
                    Log.Write("Exception in AssignModifierToTicketAction.")
                    Log.Write(ex.StackTrace)
                    Throw ex
                End Try

            End Sub

            Private Sub AssignUserToTicketAction(ByRef customer As SalesForceCustomer, ByRef action As Action, ByRef field As String)
                Dim currentUser As Users = New Users(User)
                Select Case field
                    Case "CreatorID"
                        currentUser.LoadByUserID(action.CreatorID)
                    Case "ModifierID"
                        currentUser.LoadByUserID(action.ModifierID)
                End Select

                If currentUser.Count = 0 OrElse currentUser(0).SalesForceID <> customer.ContactID Then
                    Dim newUser As Users = New Users(User)
                    newUser.LoadBySalesForceID(customer.ContactID, CRMLinkRow.OrganizationID)
                    If newUser.Count = 0 Then
                        newUser.LoadByEmailIncludingDeleted(customer.ContactEmail, CRMLinkRow.OrganizationID)
                        If newUser.Count > 0 Then
                            newUser(0).SalesForceID = customer.ContactID
                            newUser(0).MarkDeleted = False
                            newUser.Save()
                        End If
                    End If

                    If newUser.Count > 0 Then
                        Select Case field
                            Case "CreatorID"
                                action.CreatorID = newUser(0).UserID
                                action.Collection.LoginUser = New LoginUser(User.ConnectionString, newUser(0).UserID, newUser(0).OrganizationID, Nothing)
                            Case "ModifierID"
                                action.Collection.LoginUser = New LoginUser(User.ConnectionString, newUser(0).UserID, newUser(0).OrganizationID, Nothing)
                        End Select
                        Log.Write(field + " changed to " + newUser(0).UserID.ToString())
                    Else
                        Log.Write(field + " wasn't changed.")
                    End If

                End If
            End Sub

            Private Function GetCaseNumber(ByVal caseId As String) As String
                Dim caseNumber As String = String.Empty
                Dim result As List(Of QueryResult) = Nothing
                Dim query As String = String.Format("SELECT CaseNumber FROM Case WHERE id = '{0}'", caseId)
                Dim count As Integer

                Try
                    login(Trim(CRMLinkRow.Username), Trim(CRMLinkRow.Password), Trim(CRMLinkRow.SecurityToken1))
                    result = GetQueriesResults(query, count)

                    If result IsNot Nothing AndAlso count > 0 Then
                        caseNumber = result(0).records(0).Any(0).InnerText
                    End If
                Catch ex As Exception
                    Log.Write(String.Format("Error pulling case number from SalesForce. {0}", query))
                    Log.Write(ex.Message)
                    Log.Write(ex.StackTrace)
                    caseNumber = String.Empty
                End Try

                Return caseNumber
            End Function
        End Class

        Friend Class SalesForceCustomer
            Private _contactID As String
            Private _contactEmail As String
            Private _accountID As String
            Private _user As LoginUser
            Private _teamSupportUserID As Integer
            Private _teamSupportOrganizationID As Integer

            Public Sub New(ByVal user As LoginUser)
                _user = user
                _teamSupportUserID = -1
                _teamSupportOrganizationID = -1
            End Sub

            Public Sub LoadByTicketID(ByVal ticketId As Integer, ByRef Binding As SforceService)

                Dim organization As Organizations = New Organizations(_user)
                Dim contacts As ContactsView = New ContactsView(_user)
                contacts.LoadSentToSalesForce(ticketId)
                If contacts.Count = 0 Then
                    contacts.LoadByTicketIDOrderedByDateCreated(ticketId)
                    'If contacts.Count > 0 Then
                    '  Dim ticket As Ticket = Tickets.GetTicket(_user, ticketID)
                    '  ticket.Collection.SetUserAsSentToSalesForce(contacts(0).UserID, ticketID)
                    'End If
                End If

                If contacts.Count > 0 Then
                    Dim firstSalesForceContactIndex As Integer = -1
                    For i As Integer = 0 To contacts.Count - 1
                        If contacts(i).SalesForceID IsNot Nothing Then
                            firstSalesForceContactIndex = i
                            Exit For
                        End If
                    Next

                    If firstSalesForceContactIndex >= 0 Then
                        Dim account As Organizations = New Organizations(_user)
                        account.LoadByOrganizationID(contacts(firstSalesForceContactIndex).OrganizationID)
                        If account.Count > 0 Then
                            _accountID = account(0).CRMLinkID
                            _teamSupportOrganizationID = account(0).OrganizationID
                            _contactID = contacts(firstSalesForceContactIndex).SalesForceID
                            _teamSupportUserID = contacts(firstSalesForceContactIndex).UserID
                        End If
                    Else
                        Try
                            For i As Integer = 0 To contacts.Count - 1
                                Dim account As Organizations = New Organizations(_user)
                                account.LoadByOrganizationID(contacts(i).OrganizationID)
                                If account.Count > 0 Then
                                    _accountID = account(0).CRMLinkID
                                    _teamSupportOrganizationID = account(0).OrganizationID
                                    _contactID = GetSalesForceCustomerContactID(contacts(i).Email, _accountID, Binding)
                                    If _contactID IsNot Nothing Then
                                        Dim updateUser As Users = New Users(_user)
                                        updateUser.LoadByUserID(contacts(i).UserID)
                                        updateUser(0).SalesForceID = _contactID
                                        updateUser.Save()
                                        _teamSupportUserID = contacts(i).UserID
                                        Exit For
                                    Else
                                        _accountID = Nothing
                                        _teamSupportOrganizationID = -1
                                    End If
                                End If
                            Next
                        Catch ex As Exception
                            _accountID = Nothing
                            _teamSupportOrganizationID = -1
                        End Try

                    End If
                Else
                    organization.LoadSentToSalesForce(ticketId)
                    If organization.Count = 0 Then
                        organization.LoadByTicketIDOrderedByDateCreated(ticketId)
                        'If organization.Count > 0 Then
                        '  Dim ticket As Ticket = Tickets.GetTicket(_user, ticketID)
                        '  ticket.Collection.SetOrganizationAsSentToSalesForce(organization(0).OrganizationID, ticketID)
                        'End If
                    End If
                    If organization.Count > 0 Then
                        _accountID = organization(0).CRMLinkID
                        _teamSupportOrganizationID = organization(0).OrganizationID
                    End If
                End If
            End Sub

            Public Function GetSalesForceCustomerContactID(ByVal email As String, ByVal accountID As String, ByRef Binding As SforceService) As String
                Dim query As String = "SELECT ID FROM Contact WHERE email = '" + email + "' "
                If accountID IsNot Nothing Then
                    query = query + "AND Account.ID = '" + accountID + "'"
                End If
                Dim qr As QueryResult = Nothing
                qr = Binding.query(query)

                Dim result As String = Nothing
                If qr.size > 0 Then
                    Dim records As sObject() = qr.records
                    result = records(0).Id
                End If

                Return result
            End Function

            Public Property ContactID() As String
                Get
                    Return _contactID
                End Get
                Set(value As String)
                    _contactID = value
                End Set
            End Property

            Public Property AccountID() As String
                Get
                    Return _accountID
                End Get
                Set(value As String)
                    _accountID = value
                End Set
            End Property

            Public Property ContactEmail() As String
                Get
                    Return _contactEmail
                End Get
                Set(value As String)
                    _contactEmail = value
                End Set
            End Property

            Public Property TeamSupportUserID() As Integer
                Get
                    Return _teamSupportUserID
                End Get
                Set(value As Integer)
                    _teamSupportUserID = value
                End Set
            End Property

            Public Property TeamSupportOrganizationID() As Integer
                Get
                    Return _teamSupportOrganizationID
                End Get
                Set(value As Integer)
                    _teamSupportOrganizationID = value
                End Set
            End Property

        End Class

    End Namespace
End Namespace