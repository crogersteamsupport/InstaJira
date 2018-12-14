Imports TeamSupport.Data
Imports TeamSupport.ServiceLibrary

Namespace TeamSupport
  Namespace CrmIntegration
    <Serializable()> Public Class CrmPool
      Inherits ServiceThread

      Private Shared _crmPoolStopped As Boolean = False
      Public Shared Property CrmPoolStopped() As Boolean
        Get
          SyncLock GetType(CrmPool)
            Return _crmPoolStopped
          End SyncLock
        End Get
        Set(ByVal value As Boolean)
          SyncLock GetType(CrmPool)
            _crmPoolStopped = value
          End SyncLock
        End Set
      End Property

      Private _threads As New List(Of CrmProcessor)()

      Public Sub New()
        RunHandlesStop = True
      End Sub

	  Public Sub New(ByVal serviceUser As SystemUser) 
	  : MyBase.New(serviceUser)
        RunHandlesStop = True
      End Sub

      Public Overrides Sub [Stop]()
        CrmPoolStopped = True
        MyBase.[Stop]()
      End Sub

      Public Overrides Sub Start()
        Dim service As Service = Services.GetService(LoginUser, "CrmProcessor")
        service.RunCount = 0
        service.RunTimeAvg = 0
        service.RunTimeMax = 0
        service.ErrorCount = 0
        service.LastError = ""
        service.LastResult = ""
        service.Collection.Save()
        MyBase.Start()
      End Sub


      Public Overrides Sub Run()
        Logs.WriteEvent("CrmPool is running...", true)
        Dim maxThreads As Integer = Settings.ReadInt("Max Worker Processes", 1)
		Logs.WriteEventFormat("maxThreads: {0}, IsStopped: {1}, Threads count: {2}", maxThreads.ToString(), IsStopped.ToString(), _threads.Count.ToString())

		If _threads.Count > 0 Then
          For index = _threads.Count - 1 To 0 Step -1
            Logs.WriteEvent("Checking thread index: " + index.ToString(), true)

			If IsStopped Then
			  _threads(index).Stop()
			  Logs.WriteEvent("Thread index: " + index.ToString() + " stopped.", True)
			  _threads.RemoveAt(index)
			  Logs.WriteEvent("Thread index: " + index.ToString() + " removed.", True)
			ElseIf (_threads(index).IsStopped) Then
			  Logs.WriteEvent("Thread index: " + index.ToString() + " is stopped.", True)
			  _threads.RemoveAt(index)
			  Logs.WriteEvent("Thread index: " + index.ToString() + " removed.", True)
			End If
          Next
        End If

        If IsStopped Then
		  Logs.WriteEvent("CrmPool finishing as is stopped", True)
		  Return
        End If

        'See if we have reached our max thread count
        If _threads.Count >= maxThreads Then
		  Logs.WriteEvent("CrmPool finishing as its threads count exceeds maxThreads", True)
		  Return
        End If

        'Load the waiting links to process
        Dim links As New CRMLinkTable(_loginUser)
        links.LoadActive(Settings.ReadInt("CRMLinkTable Process Interval", 15))

		Logs.WriteEvent("Active links count: " + links.Count().ToString(), True)
		Dim isAlreadyProcessingValue As Boolean = False
        For Each link As CRMLinkTableItem In links
		  Logs.WriteEvent(String.Format("Checking link type: {0} of OrgID: {1}{2}", link.CRMType, link.OrganizationID.ToString(), If(link.CRMType.ToLower() = "jira", String.Format(" ({0})", link.InstanceName), "")), True)
		  LogJiraInstanceProducts(link)

          If _threads.Count >= maxThreads Then
			Logs.WriteEvent("CrmPool finishing as its threads count exceeds maxThreads", True)
			Return
          End If

          isAlreadyProcessingValue = IsAlreadyProcessing(link.CRMLinkID)
		  Logs.WriteEvent("isAlreadyProcessing: " + isAlreadyProcessingValue.ToString(), True)

		  If Not isAlreadyProcessingValue Then
			Dim crmProcessor As New CrmProcessor(link.CRMLinkID)
			Logs.WriteEvent("link CrmProcessor instantiated.", True)
			crmProcessor.Start()
			Logs.WriteEvent("link CrmProcessor started.", True)
			_threads.Add(crmProcessor)
			Logs.WriteEvent("link CrmProcessor thread added.", True)
		  End If
		Next
      End Sub

		Private Function IsAlreadyProcessing(ByVal crmLinkID As Integer) As Boolean
		  For Each thread As CrmProcessor In _threads
			 If thread.CrmLinkID = crmLinkID Then
				Return True
			 End If
		  Next
		  Return False
		End Function

		Private Sub LogJiraInstanceProducts(ByVal link As CRMLinkTableItem)
			If link.CRMType.ToLower() = "jira" Then
				Dim jiraInstances As JiraInstanceProducts = New JiraInstanceProducts(_loginUser)
				jiraInstances.LoadByOrganizationAndLinkId(link.OrganizationID, link.CRMLinkID, link.CRMType)
				Dim productsListString As String = String.Empty

					For Each jiraInstance As JiraInstanceProduct In jiraInstances
						If (Not String.IsNullOrEmpty(productsListString)) Then
							productsListString = productsListString + ","
						End If

						Dim product As Product = Products.GetProduct(_loginUser, jiraInstance.ProductId)
						productsListString = productsListString + String.Format("{0} ({1})", product.Name, product.ProductID)
					Next

					Logs.WriteEvent(String.Format("Instance is associated with Products: {0}", If(Not String.IsNullOrEmpty(productsListString), productsListString, "None")))
				End If
		End Sub
    End Class

  End Namespace
End Namespace


