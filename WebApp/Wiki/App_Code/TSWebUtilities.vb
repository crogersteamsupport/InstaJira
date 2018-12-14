Imports Microsoft.VisualBasic
Imports System.Data.SqlClient

Imports TeamSupportUtilities
Imports TeamSupport.Data
Imports System.Web.UI
Imports System.IO
Imports System
Imports System.Web
Imports System.Data
Imports Telerik.Web.UI
Imports System.Globalization
Imports System.Threading






Public Class TSWebUtilities
    Inherits System.Web.UI.Page


    'This lets us remove the reference to the configuration manager so I can use this in a VB app
    Public ConnectionString As String

    Public Sub ChangePassword(ByRef NewPassword As String, ByRef UserID As String)
        'This will change a users password.  UserID should be unique.

        Dim cryptedpassword As String = FormsAuthentication.HashPasswordForStoringInConfigFile(NewPassword, "MD5")

        Dim sqlStatement As String = "update users set CryptedPassword = @CryptedPassword, IsPasswordExpired=0 where UserID = @UserID"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@CryptedPassword", cryptedpassword))
        command.Parameters.Add(New SqlParameter("@UserID", UserID))

        command.Connection.Open()

        command.ExecuteNonQuery()

        command.Connection.Close()
        connection.Close()

    End Sub

    Public Sub ResetPassword(ByRef NewPassword As String, ByRef UserID As String)
        'This will change a users password.  UserID should be unique.

        Dim cryptedpassword As String = FormsAuthentication.HashPasswordForStoringInConfigFile(NewPassword, "MD5")

        'set expired as true so user has to change their password at next login
        Dim sqlStatement As String = "update users set CryptedPassword = @CryptedPassword, IsPasswordExpired=1 where UserID = @UserID"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@CryptedPassword", cryptedpassword))
        command.Parameters.Add(New SqlParameter("@UserID", UserID))

        command.Connection.Open()

        command.ExecuteNonQuery()

        command.Connection.Close()
        connection.Close()


    End Sub

    Public Sub UserMsgBox(ByVal F As Object, ByVal sMsg As String)
        ' more info here: http://www.daniweb.com/forums/thread134559.html
        Dim sb As New StringBuilder()
        Dim oFormObject As System.Web.UI.Control = Nothing
        Try
            sMsg = sMsg.Replace("'", "\'")
            sMsg = sMsg.Replace(Chr(34), "\" & Chr(34))
            sMsg = sMsg.Replace(vbCrLf, "\n")
            sMsg = "<script language='javascript'>alert('" & sMsg & "');</script>"
            sb = New StringBuilder()
            sb.Append(sMsg)
            For Each oFormObject In F.Controls
                If TypeOf oFormObject Is HtmlForm Then
                    Exit For
                End If
            Next
            oFormObject.Controls.AddAt(oFormObject.Controls.Count, New LiteralControl(sb.ToString()))
        Catch ex As Exception
        End Try
    End Sub

    Public Function GetHTMLHeader(ByRef OrganizationID As String) As String
        'This will return the HTML header which is stored in the Organization Table

        Dim sqlStatement As String = "select PortalHTMLHeader from PortalOptions where organizationid = @orgid"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrgID", OrganizationID))

        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader

        'return the HTML
        Try


            Try

                If reader.Read Then
                    Return reader(0)
                Else
                    Return "" 'return blank
                End If
            Catch ex As Exception
                Return ""
            End Try

        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try

        command.Connection.Close()

    End Function

    Public Function GetHTMLFooter(ByRef OrganizationID As String) As String
        'This will return the HTML header which is stored in the Organization Table

        Dim sqlStatement As String = "select PortalHTMLFooter from PortalOptions where organizationid = @orgid"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrgID", OrganizationID))

        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader

        'return the HTML
        Try
            Try


                If reader.Read Then
                    Return reader(0)
                Else
                    Return "" 'return blank
                End If
            Catch ex As Exception
                Return ""
            End Try

        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try
    End Function


    Public Sub SaveAttachments(ByVal actionID As Integer, ByVal userid As Integer, ByVal organizationid As Integer, ByVal UploadControl As Telerik.Web.UI.RadUpload, ByVal connectionstring As String)
        'Dim ConnectionString As String = ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString
        'Dim UserID As Integer = Profile.UserID.ToString
        'Dim OrganizationID As Integer = Profile.ParentID.ToString


        'You need to create the login user first, this lets me store the connection string, UserID, and OrgID
        'Params - ConnectionString, UserID, OrganizationID
        Dim loginUser As LoginUser = New LoginUser(connectionstring, userid, organizationid, Nothing)

        'Object to the attachments table
        Dim attachments As Attachments = New Attachments(loginUser)

        'For each file in the RadUpload control, save the attachments

        For Each file As UploadedFile In UploadControl.UploadedFiles
            'Get the path for attachments
            Dim attachmentPath As String = attachments.GetAttachmentPath(loginUser, ReferenceType.Actions, actionID)

            'Create a record in the Attachments Table
            Dim attachment As Attachment = attachments.AddNewAttachment()

            'Set the properties
            attachment.RefType = ReferenceType.Actions
            attachment.RefID = actionID
            attachment.OrganizationID = loginUser.OrganizationID
            attachment.FileName = file.GetName()
            attachment.Path = Path.Combine(attachmentPath, attachment.FileName)
            attachment.FileType = file.ContentType
            attachment.FileSize = file.ContentLength

            'Make sure the dir is there
            Directory.CreateDirectory(attachmentPath)

            'Save the file
            file.SaveAs(attachment.Path, True)

            'Save the record in the DB
            attachments.Save()

        Next file

    End Sub

    Public Function IsTicketOpen(ByVal OrganizationID As Integer, ByVal TicketNumber As Integer) As Boolean

        Dim sqlStatement As String = "select ticketstatuses.isclosed from tickets, ticketstatuses where tickets.ticketstatusid = ticketstatuses.ticketstatusid and tickets.organizationid = @organizationid and tickets.ticketnumber = @ticketnumber"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@TicketNumber", TicketNumber))

        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader
        Try
            'return the HTML
            Try
                If reader.Read Then
                    Return reader(0) = 0 'true if ticket is open
                Else
                    Return False 'return blank
                End If
            Catch ex As Exception
                Return False
            End Try


        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try
    End Function

    Public Function GetTicketType(ByVal OrganizationID As Integer, ByVal TicketNumber As Integer) As String
        'returns the ticket type name (Bugs, Issues, Features, Tasks)
        Dim sqlStatement As String = "select tt.name from tickets as t, tickettypes as tt where t.tickettypeid = tt.tickettypeid and t.ticketnumber = @TicketNumber and t.organizationid = @OrganizationID"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@TicketNumber", TicketNumber))

        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader
        Try
            'return the HTML
            Try
                If reader.Read Then
                    Return reader(0)
                Else
                    Return "" 'return blank
                End If
            Catch ex As Exception
                Return False
            End Try

        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try
    End Function


    Public Function GetAttachmentFileName(ByVal AttachmentID As Integer) As String
        'Grab the full filename of the attachment
        Using myConnection As New SqlConnection(ConnectionString)

            Const SQL As String = "select path from attachments where AttachmentID = @AttachmentID "
            Dim myCommand As New SqlCommand(SQL, myConnection)
            myCommand.Parameters.AddWithValue("@AttachmentID", AttachmentID)


            myConnection.Open()
            Dim myReader As SqlDataReader = myCommand.ExecuteReader


            If myReader.Read Then
                Return myReader(0)
            Else
                Return ""
            End If

            myReader.Close()
            myConnection.Close()
        End Using




    End Function

    Public Function GetAttachmentFileType(ByVal AttachmentID As Integer) As String
        'Grab the full filename of the attachment
        Using myConnection As New SqlConnection(ConnectionString)

            Const SQL As String = "select filetype from attachments where AttachmentID = @AttachmentID "
            Dim myCommand As New SqlCommand(SQL, myConnection)
            myCommand.Parameters.AddWithValue("@AttachmentID", AttachmentID)


            myConnection.Open()
            Dim myReader As SqlDataReader = myCommand.ExecuteReader


            If myReader.Read Then
                Return myReader(0)
            Else
                Return ""
            End If

            myReader.Close()
            myConnection.Close()
        End Using

    End Function

    Public Function GetAttachmentFileSize(ByVal AttachmentID As Integer) As String
        'Grab the full filename of the attachment
        Using myConnection As New SqlConnection(ConnectionString)

            Const SQL As String = "select filesize from attachments where AttachmentID = @AttachmentID "
            Dim myCommand As New SqlCommand(SQL, myConnection)
            myCommand.Parameters.AddWithValue("@AttachmentID", AttachmentID)


            myConnection.Open()
            Dim myReader As SqlDataReader = myCommand.ExecuteReader


            If myReader.Read Then
                Return myReader(0)
            Else
                Return ""
            End If

            myReader.Close()
            myConnection.Close()
        End Using

    End Function

    Public Function GetAttachmentOrgID(ByVal AttachmentID As Integer) As String
        'Grab the Org ID of the attachment (used for security - match to parentid

        Using myConnection As New SqlConnection(ConnectionString)

            Const SQL As String = "select OrganizationID from attachments where AttachmentID = @AttachmentID "
            Dim myCommand As New SqlCommand(SQL, myConnection)
            myCommand.Parameters.AddWithValue("@AttachmentID", AttachmentID)


            myConnection.Open()
            Dim myReader As SqlDataReader = myCommand.ExecuteReader


            If myReader.Read Then
                Return myReader(0)
            Else
                Return ""
            End If

            myReader.Close()
            myConnection.Close()
        End Using

    End Function


    Public Function AdvancedPortal(ByVal OrganizationID As Integer) As Boolean
        'Returns True if the organization is licensed for the AdvancedPortal

        Dim sqlStatement As String = "select IsAdvancedPortal from Organizations where OrganizationID = @OrganizationID"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))


        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader

        Try 'return the HTML
            Try
                If reader.Read Then
                    Return reader(0)  'true if Org has Advanced Portal
                Else
                    Return False 'Org is not licensed for advanced portal
                End If
            Catch ex As Exception
                Return False
            End Try

        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try
    End Function

    Public Function BasicPortal(ByVal OrganizationID As Integer) As Boolean
        'Returns True if the organization is licensed for the Basic Portal

        Dim sqlStatement As String = "select IsBasicPortal from Organizations where OrganizationID = @OrganizationID"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))


        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader
        Try
            'return the HTML
            Try
                If reader.Read Then
                    Return reader(0) 'true if Org has Basic Portal
                Else
                    Return False 'Org is not licensed for Basic portal
                End If
            Catch ex As Exception
                Return False
            End Try

        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try
    End Function



    Protected Sub AddLineFeed(ByVal PanelName As Panel)
        PanelName.Controls.Add(New LiteralControl("<BR />"))
    End Sub

    Protected Sub AddCustomTextBox(ByVal position As Integer, ByVal PanelName As Panel, ByVal Tooltip As String)
        Dim CustomTextBox As New TextBox


        CustomTextBox.ID = "CustomField" + position.ToString
        CustomTextBox.ToolTip = Tooltip
        'CustomTextBox.Width = PanelNum.Width
        PanelName.Controls.Add(CustomTextBox)

        CustomTextBox.Width = 300




        AddLineFeed(PanelName)


    End Sub

    Public Sub AddComboBox(ByVal Position As Integer, ByVal PanelNum As Panel, ByVal Picklistitems As String(), ByVal Tooltip As String)


        Dim TableTextBox As New Telerik.Web.UI.RadComboBox

        'TableTextBox.AllowCustomText = True
        'TableTextBox.Text = "ComboBox" + num.ToString
        TableTextBox.ID = "CustomField" + Position.ToString
        'TableTextBox.Width = PanelNum.Width


        Dim item As New Telerik.Web.UI.RadComboBoxItem()
        Dim j As Integer


        For j = 0 To UBound(Picklistitems)

            item.Text = Picklistitems(j)
            item.Value = Picklistitems(j)
            TableTextBox.Items.Add(New Telerik.Web.UI.RadComboBoxItem(Picklistitems(j), Picklistitems(j)))


        Next j

        TableTextBox.ToolTip = Tooltip

        TableTextBox.Skin = "Web20"



        TableTextBox.Font.Size = FontUnit.Small


        TableTextBox.MaxHeight = 200

        TableTextBox.Width = 300



        PanelNum.Controls.Add(TableTextBox)

        AddLineFeed(PanelNum)

    End Sub

    Public Sub AddCalendarField(ByVal Position As Integer, ByVal PanelNum As Panel, ByVal Tooltip As String, ByVal OrgID As String)


        Dim CalendarField As New Telerik.Web.UI.RadDateTimePicker

        'CalendarField.AllowCustomText = True
        'CalendarField.Text = "CustomTextBox" + num.ToString
        CalendarField.ID = "CustomField" + Position.ToString
        CalendarField.ToolTip = Tooltip



        '**PROBLEM HERE FOR SOME REASON 3-3-10

        'If Not UseEuropeDate(OrgID) Then
        'CalendarField.Culture = New System.Globalization.CultureInfo("en-US", True)

        'Else
        'CalendarField.Culture = New System.Globalization.CultureInfo("en-GB", True)

        'End If


        CalendarField.MinDate = "01/01/1900"

        'CalendarField.Width = PanelNum.Width



        CalendarField.Width = 300

        PanelNum.Controls.Add(CalendarField)

        AddLineFeed(PanelNum)
    End Sub

    Public Sub AddTimeField(ByVal Position As Integer, ByVal PanelNum As Panel, ByVal Tooltip As String)


        Dim TimeField As New Telerik.Web.UI.RadTimePicker


        'CalendarField.AllowCustomText = True
        'CalendarField.Text = "CustomTextBox" + num.ToString
        TimeField.ID = "CustomField" + Position.ToString
        TimeField.ToolTip = Tooltip

        'TimeField.Width = PanelNum.Width

        TimeField.Width = 300


        PanelNum.Controls.Add(TimeField)

        AddLineFeed(PanelNum)
    End Sub

    Protected Sub AddCustomLabel(ByVal Text As String, ByVal Description As String, ByVal PanelName As Panel, ByVal position As Integer)
        Dim CustomLabel As New Label


        CustomLabel.ID = "CustomFieldLabel" + position.ToString
        'CustomTextBox.ToolTip = Description
        'CustomTextBox.Width = PanelNum.Width
        CustomLabel.Text = Text + ":"


        'CustomLabel.Width = 300



        PanelName.Controls.Add(CustomLabel)

        AddLineFeed(PanelName)


    End Sub

    Public Sub DisplayCustomData(ByVal LabelPanel As Panel, ByVal DataPanel As Panel, ByVal OrganizationID As String, ByVal TitleCSS As String, ByVal DataCSS As String)
        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)
        Dim command As New SqlCommand("select cf.CustomFieldID, cf.Name, cf.FieldType, cf.ListValues, cf.Description, cf.position from customfields as cf, tickettypes as tt where reftype = 17  and isvisibleonportal = 1 and cf.auxid = tt.tickettypeid and tt.name = 'Issues' and cf.organizationid=@OrganizationID order by cf.position", connection)
        Dim adapter As New SqlDataAdapter(command)
        Dim table As New DataTable

        Dim PickListItems() As String
        Dim BooleanList As String() = {"True", "False"}

        Dim Picklist As String

        ' Dim item1 As New Telerik.Web.UI.RadComboBoxItem()

        Try



            connection.Open()
            command.Parameters.AddWithValue("@OrganizationID", OrganizationID)
            adapter.Fill(table)

            If table.Rows.Count > 0 Then

                'LabelPanel.Controls.Add(New LiteralControl("<Table>"))
                'DataPanel.Controls.Add(New LiteralControl("<Table>"))

                For i = 0 To table.Rows.Count - 1
                    Try
                        '    Text = 0,
                        ' DateTime = 1,
                        'Boolean = 2,
                        'Number = 3,
                        'PickList = 4



                        LabelPanel.Controls.Add(New LiteralControl("<tr>")) 'add a new row



                        'DataPanel.Controls.Add(New LiteralControl("<tr><td>"))

                        If table.Rows(i)(2).ToString = "1" Then 'datetime
                            LabelPanel.Controls.Add(New LiteralControl("<td class=""" + TitleCSS + """ width=""200px"">")) 'add a row and data cell to table
                            AddCustomLabel(table.Rows(i)(1).ToString, table.Rows(i)(1).ToString, LabelPanel, table.Rows(i)(5).ToString)
                            LabelPanel.Controls.Add(New LiteralControl("</td>"))

                            LabelPanel.Controls.Add(New LiteralControl("<td class=""" + DataCSS + """>"))
                            AddCalendarField(table.Rows(i)(5).ToString, LabelPanel, table.Rows(i)(4).ToString, OrganizationID)
                            LabelPanel.Controls.Add(New LiteralControl("</td>"))

                        ElseIf table.Rows(i)(2).ToString = "2" Then 'boolean
                            LabelPanel.Controls.Add(New LiteralControl("<td class=""" + TitleCSS + """ width=""200px"">")) 'add a row and data cell to table
                            AddCustomLabel(table.Rows(i)(1).ToString, table.Rows(i)(1).ToString, LabelPanel, table.Rows(i)(5).ToString)
                            LabelPanel.Controls.Add(New LiteralControl("</td>"))

                            LabelPanel.Controls.Add(New LiteralControl("<td class=""" + DataCSS + """>"))
                            AddComboBox(table.Rows(i)(5).ToString, LabelPanel, BooleanList, table.Rows(i)(4).ToString)
                            LabelPanel.Controls.Add(New LiteralControl("</td>"))

                        ElseIf table.Rows(i)(2).ToString = "3" Then 'number
                            'Text for now - Need to change
                            '**
                            LabelPanel.Controls.Add(New LiteralControl("<td class=""" + TitleCSS + """ width=""200px"">")) 'add a row and data cell to table
                            AddCustomLabel(table.Rows(i)(1).ToString, table.Rows(i)(1).ToString, LabelPanel, table.Rows(i)(5).ToString)
                            LabelPanel.Controls.Add(New LiteralControl("</td>"))

                            LabelPanel.Controls.Add(New LiteralControl("<td class=""" + DataCSS + """>"))
                            AddCustomTextBox(table.Rows(i)(5).ToString, LabelPanel, table.Rows(i)(4).ToString)
                            LabelPanel.Controls.Add(New LiteralControl("</td>"))

                        ElseIf table.Rows(i)(2).ToString = "4" Then 'picklist
                            Picklist = table.Rows(i)(3).ToString
                            PickListItems = Picklist.Split("|")

                            LabelPanel.Controls.Add(New LiteralControl("<td class=""" + TitleCSS + """ width=""200px"">")) 'add a row and data cell to table
                            AddCustomLabel(table.Rows(i)(1).ToString, table.Rows(i)(1).ToString, LabelPanel, table.Rows(i)(5).ToString)
                            LabelPanel.Controls.Add(New LiteralControl("</td>"))

                            LabelPanel.Controls.Add(New LiteralControl("<td class=""" + DataCSS + """>"))
                            AddComboBox(table.Rows(i)(5).ToString, LabelPanel, PickListItems, table.Rows(i)(4).ToString)
                            LabelPanel.Controls.Add(New LiteralControl("</td>"))



                        Else 'probbaly text box
                            LabelPanel.Controls.Add(New LiteralControl("<td class=""" + TitleCSS + """ width=""200px"">")) 'add a row and data cell to table
                            AddCustomLabel(table.Rows(i)(1).ToString, table.Rows(i)(1).ToString, LabelPanel, table.Rows(i)(5).ToString)
                            LabelPanel.Controls.Add(New LiteralControl("</td>"))

                            LabelPanel.Controls.Add(New LiteralControl("<td class=""" + DataCSS + """>"))
                            AddCustomTextBox(table.Rows(i)(5).ToString, LabelPanel, table.Rows(i)(4).ToString)
                            LabelPanel.Controls.Add(New LiteralControl("</td>"))


                        End If

                        LabelPanel.Controls.Add(New LiteralControl("</tr>")) 'end of row and end of cell
                        'DataPanel.Controls.Add(New LiteralControl("</tr></td>"))

                    Catch ex As Exception
                        'if we have two of the same field names, this should trap it.
                    End Try



                    'CustomLabel(i + 1).Visible = True
                    'CustomLabel(i + 1).Text = table.Rows(i)(0).ToString
                    'GetPanel(i + 1).Visible = True

                Next

                'LabelPanel.Controls.Add(New LiteralControl("</Table>")) 'end of table
                'DataPanel.Controls.Add(New LiteralControl("</Table>")) 'end of table
            End If

        Finally

            command.Connection.Close()
            connection.Close()

        End Try



    End Sub



    Protected Sub SaveCustomField(ByVal TicketID As String, ByVal DataToStore As String, ByVal CustomFieldID As String, Optional OrganizationID As Integer = -1)
        'This will store the custom data for a given field
        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)
        '*** Change to type Issues since that's the only ticket type you can create in the portal
        Dim command As New SqlCommand("Insert into CustomValues (CustomFieldID, RefID, CustomValue, DateCreated, DateModified, CreatorID, ModifierID, OrganizationID) Values(@CustomFieldID, @RefID, @CustomValue, GetUTCDate(), GetUTCDate(), -1, -1, @OrganizationID)", connection)

        connection.Open()
        command.Parameters.AddWithValue("@CustomFieldID", CustomFieldID)
        command.Parameters.AddWithValue("@RefID", TicketID)
        command.Parameters.AddWithValue("@CustomValue", DataToStore)
		command.Parameters.AddWithValue("@OrganizationID", OrganizationID)

        command.ExecuteNonQuery()
        command.Connection.Close()


    End Sub

    Public Sub StoreCustomData(ByVal TicketID As String, ByVal OrganizationID As String, ByVal DataPanel As Panel)
        'This routine will get the custom data from the form and store it to the database

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)
        '*** Change to type Issues since that's the only ticket type you can create in the portal
        Dim command As New SqlCommand("select cf.CustomFieldID, cf.Name, cf.FieldType, cf.ListValues, cf.Description, cf.position from customfields as cf, tickettypes as tt where reftype = 17  and isvisibleonportal = 1 and cf.auxid = tt.tickettypeid and tt.name = 'Issues' and cf.organizationid=@OrganizationID ", connection)
        Dim adapter As New SqlDataAdapter(command)
        Dim table As New DataTable

        Dim TempText As String

        Try



            connection.Open()
            command.Parameters.AddWithValue("@OrganizationID", OrganizationID)
            adapter.Fill(table)

            If table.Rows.Count > 0 Then

                For i = 0 To table.Rows.Count - 1
                    Try




                        '    Text = 0,
                        ' DateTime = 1,
                        'Boolean = 2,
                        'Number = 3,
                        'PickList = 4

                        If table.Rows(i)(2).ToString = "1" Then 'datetime

                            TempText = CType(DataPanel.FindControl("CustomField" + table.Rows(i)(5).ToString), Telerik.Web.UI.RadDatePicker).SelectedDate.ToString

                        ElseIf table.Rows(i)(2).ToString = "2" Then 'boolean

                            TempText = CType(DataPanel.FindControl("CustomField" + table.Rows(i)(5).ToString), Telerik.Web.UI.RadComboBox).SelectedItem.Text



                        ElseIf table.Rows(i)(2).ToString = "3" Then 'number
                            'Text for now - Need to change
                            '**

                            TempText = CType(DataPanel.FindControl("CustomField" + table.Rows(i)(5).ToString), TextBox).Text



                        ElseIf table.Rows(i)(2).ToString = "4" Then 'picklist
                            TempText = CType(DataPanel.FindControl("CustomField" + table.Rows(i)(5).ToString), Telerik.Web.UI.RadComboBox).SelectedItem.Text


                        Else 'probbaly text box
                            TempText = CType(DataPanel.FindControl("CustomField" + table.Rows(i)(5).ToString), TextBox).Text

                        End If


						'OK, we now have the data - Let's store it!
						SaveCustomField(TicketID, TempText, table.Rows(i)(0).ToString, OrganizationID)


                    Catch ex As Exception
                        'if we have two of the same field names, this should trap it.
                    End Try




                Next

            End If

        Finally

            command.Connection.Close()
            connection.Close()

        End Try



    End Sub

    Public Function GetFirstClosedStatus(ByRef ParentID As String, ByVal TicketType As String) As String

        'This will search the statuses for the parent company and get the first status that is CLOSED.  Used when closing a ticket.

        Dim sqlStatement As String = "select top 1 ticketstatusid from tickettypes, ticketstatuses where tickettypes.tickettypeid = ticketstatuses.tickettypeid and  ticketstatuses.isclosed = 1 and tickettypes.organizationid = @OrganizationID and tickettypes.name = @TicketType order by tickettypes.position, ticketstatuses.position "
        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)

        command.Connection.Open()

        command.Parameters.AddWithValue("@OrganizationID", ParentID)
        command.Parameters.AddWithValue("@TicketType", TicketType)



        Dim reader As SqlDataReader = command.ExecuteReader
        Try
            'return the ticketid
            If reader.Read Then
                Return reader(0)
            Else
                Return ""
            End If


        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try
    End Function

    Public Function GetFirstSeverity(ByRef ParentID As String) As String

        'This will return the first Severity type - Used for all new tickets on portal

        Dim sqlStatement As String = "select top 1 TicketSeverityID from ticketseverities where organizationid=@OrganizationID order by position"

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)

        command.Connection.Open()

        command.Parameters.AddWithValue("@OrganizationID", ParentID)
        'command.Parameters.AddWithValue("@TicketType", TicketType)



        Dim reader As SqlDataReader = command.ExecuteReader
        Try
            'return the ticketid
            If reader.Read Then
                Return reader(0)
            Else
                Return ""
            End If


        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try

    End Function


    Public Sub ChangeTicketStatus(ByVal TicketID As String, ByVal TicketStatusID As String)
        Dim sqlStatement As String = "Update tickets set TicketStatusID = @TicketStatusID where TicketID = @TicketID"
        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)

        command.Connection.Open()

        command.Parameters.AddWithValue("@TicketID", TicketID)
        command.Parameters.AddWithValue("@TicketStatusID", TicketStatusID)

        command.ExecuteNonQuery()

        command.Connection.Close()
        connection.Close()


    End Sub

    Public Sub CloseTicket(ByVal TicketID As String, ByVal UserID As String, ByVal OrgID As String, ByVal TicketType As String)
        'First, let's change the status to closed
        Dim ClosedStatusID = GetFirstClosedStatus(OrgID, TicketType)
        ChangeTicketStatus(TicketID, ClosedStatusID)

        'OK, ticket is now closed.  Let's update the User and closed date/time

        Dim sqlStatement As String = "Update tickets set DateClosed = GetUTCDate(), CloserID = @CloserID, DateModified = GetUTCDate(), ModifierID = @CloserID where TicketID = @TicketID"
        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)

        command.Connection.Open()

        command.Parameters.AddWithValue("@TicketID", TicketID)
        command.Parameters.AddWithValue("@CloserID", UserID)

        command.ExecuteNonQuery()

        command.Connection.Close()
        connection.Close()

    End Sub

    Public Sub AddHistory(ByVal UserID As String, ByVal RefType As String, ByVal OrganizationID As String, ByVal TicketID As String, ByVal HistoryText As String)
        'This will write a line to the history table
        '**Should put this in TSUtilities so it's available to every app


        Dim sqlStatement As String = "Insert into ActionLogs (OrganizationID, RefType, RefID, ActionLogType, Description, DateCreated, DateModified, CreatorID, ModifierID) values(@OrganizationID, @RefType, @RefID, 0, @Description, GetUTCDate(), GetUTCDate(), @UserID, @UserID)"
        'Update tickets set DateClosed = GetUTCDate(), CloserID = @CloserID, DateModified = GetUTCDate(), ModifierID = @CloserID where TicketID = @TicketID"
        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)

        command.Connection.Open()

        command.Parameters.AddWithValue("@OrganizationID", OrganizationID)
        command.Parameters.AddWithValue("@RefType", RefType)
        command.Parameters.AddWithValue("@RefID", TicketID)
        command.Parameters.AddWithValue("@Description", HistoryText)
        command.Parameters.AddWithValue("@UserID", UserID)



        command.ExecuteNonQuery()

        command.Connection.Close()
        connection.Close()

    End Sub


    Public Function DoesProductIDExist(ByRef OrganizationID As String, ByVal ProductID As String) As Boolean

        'This will see if the product ID actually exists and is associated with the company

        Dim sqlStatement As String = "select * from products where productid = @ProductID and organizationid = @OrganizationID"
        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)

        command.Connection.Open()

        command.Parameters.AddWithValue("@OrganizationID", OrganizationID)
        command.Parameters.AddWithValue("@ProductID", ProductID)



        Dim reader As SqlDataReader = command.ExecuteReader
        Try
            'return true if the ProductID exists for ths company
            If reader.Read Then
                Return True
            Else
                Return False
            End If


        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try
    End Function

    Public Function GetOrgName(ByRef OrgID As String) As String
        Dim sqlStatement As String = "select name from organizations where organizationid = " + OrgID
        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)

        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader
        Try
            'Eventually put some more stuff in here about not being active, not having portal access, password expired, etc
            If reader.Read Then
                Return reader(0)
            Else
                Return ""
            End If

        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try
    End Function

    Public Function IsTicketPublic(ByVal OrganizationID As Integer, ByVal TicketNumber As Integer) As Boolean

        'Returns True if the ticket should be visible on the portal

        Dim sqlStatement As String = "select isvisibleonportal from tickets where organizationid = @organizationid and ticketnumber = @ticketnumber"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@TicketNumber", TicketNumber))

        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader
        Try
            'return the HTML
            Try
                If reader.Read Then
                    Return reader(0) 'true if ticket is visible
                Else
                    Return False 'return blank
                End If
            Catch ex As Exception
                Return False
            End Try


        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try
    End Function

    Public Function UseRecaptcha(ByRef OrganizationID As String) As Boolean
        'This will return the HTML header which is stored in the Organization Table

        Dim sqlStatement As String = "select UseRecaptcha from PortalOptions where organizationid = @orgid"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrgID", OrganizationID))

        command.Connection.Open()
        Try



            Dim reader As SqlDataReader = command.ExecuteReader


            'return the HTML
            Try


                Try

                    If reader.Read Then
                        Return reader(0) 'should be 1=1 which is True, or 0=1 which is false
                    Else
                        Return True 'default to use recaptcha
                    End If
                Catch ex As Exception
                    Return ""
                End Try

            Finally
                command.Connection.Close()
                connection.Close()
                reader.Close()
            End Try

            command.Connection.Close()
        Catch ex As Exception
            Return True 'use recaptcha if there's an error
        End Try
    End Function


    Public Sub UpdateLastLogin(ByVal UserID As String)
        'This will write a line to the history table
        '**Should put this in TSUtilities so it's available to every app


        Dim sqlStatement As String = "Update Users set LastLogin = GetUTCDate() where UserID = @UserID"

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)

        command.Connection.Open()

        command.Parameters.AddWithValue("@UserID", UserID)



        command.ExecuteNonQuery()

        command.Connection.Close()
        connection.Close()

    End Sub


    Public Function GetFontFamily(ByRef OrganizationID As String) As String
        'This will return the Font Family stored in Portal Options

        Dim sqlStatement As String = "select FontFamily from PortalOptions where organizationid = @orgid"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrgID", OrganizationID))

        command.Connection.Open()
        Try



            Dim reader As SqlDataReader = command.ExecuteReader


            'return the string
            Try


                Try

                    If reader.Read Then
                        Return reader(0)
                    Else
                        Return ""
                    End If
                Catch ex As Exception
                    Return ""
                End Try

            Finally
                command.Connection.Close()
                connection.Close()
                reader.Close()
            End Try

            command.Connection.Close()
        Catch ex As Exception
            Return True 'use recaptcha if there's an error
        End Try
    End Function

    Public Function GetPageBackgroundColor(ByRef OrganizationID As String) As String
        'This will return the Font Family stored in Portal Options

        Dim sqlStatement As String = "select PageBackgroundColor from PortalOptions where organizationid = @orgid"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrgID", OrganizationID))

        command.Connection.Open()
        Try



            Dim reader As SqlDataReader = command.ExecuteReader


            'return the string
            Try


                Try

                    If reader.Read Then
                        Return reader(0)
                    Else
                        Return ""
                    End If
                Catch ex As Exception
                    Return ""
                End Try

            Finally
                command.Connection.Close()
                connection.Close()
                reader.Close()
            End Try

            command.Connection.Close()
        Catch ex As Exception
            Return True 'use recaptcha if there's an error
        End Try
    End Function

    Public Function GetFontColor(ByRef OrganizationID As String) As String
        'This will return the Font Family stored in Portal Options

        Dim sqlStatement As String = "select FontColor from PortalOptions where organizationid = @orgid"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrgID", OrganizationID))

        command.Connection.Open()
        Try



            Dim reader As SqlDataReader = command.ExecuteReader


            'return the string
            Try


                Try

                    If reader.Read Then
                        Return reader(0)
                    Else
                        Return ""
                    End If
                Catch ex As Exception
                    Return ""
                End Try

            Finally
                command.Connection.Close()
                connection.Close()
                reader.Close()
            End Try

            command.Connection.Close()
        Catch ex As Exception
            Return True 'use recaptcha if there's an error
        End Try
    End Function


    Public Sub SetPageDisplayStyle(ByVal PageName As Page, ByVal OrgID As String)

        Try


            Dim FontFamily As String = GetFontFamily(OrgID)
            If FontFamily <> "" Then
                PageName.Form.Style.Add("font-family", FontFamily)
            End If

            Dim FontColor As String = GetFontColor(OrgID)
            If FontFamily <> "" Then
                PageName.Form.Style.Add("color", FontColor)
            End If


            Dim PageColor As String = GetPageBackgroundColor(OrgID)
            If PageColor <> "" Then
                PageName.Form.Style.Add("background-color", PageColor)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Public Sub UpdateTicketViewCount(ByVal TicketID As String)
        'insert into ticketratings (ticketid, views) values(9146,1)

        Try

            Dim sqlStatement As String = "insert into ticketratings (ticketid, views) values(@TicketID,1)"
            Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)
            Try
                command.Connection.Open()

                command.Parameters.AddWithValue("@TicketID", TicketID)


                command.ExecuteNonQuery()
            Finally

                command.Connection.Close()
                connection.Close()
            End Try

        Catch ex As Exception
            'The insert failed which means we need to update instead of insert
            Dim sqlStatement As String = "update ticketratings set views = isnull(views,0)+1 where ticketid =  @TicketID"
            Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)

            Try


                command.Connection.Open()

                command.Parameters.AddWithValue("@TicketID", TicketID)


                command.ExecuteNonQuery()
            Finally
                command.Connection.Close()
                connection.Close()
            End Try

        End Try

    End Sub


    Public Function ReturnSearchString(ByVal SearchString As String, ByVal UseOr As Boolean) As String

        'This will format a list of words into a search string applicable for SQL
        'Takes a list of search terms, breaks them apart at the spaces, adds a wildcard and either AND or OR.

        Dim SearchArray() As String
        Dim SearchText As String = ""

        SearchArray = SearchString.Split(" ")

        For x As Integer = 0 To UBound(SearchArray)

            If (SearchArray(x).ToUpper <> "OR") And (SearchArray(x).ToUpper <> "AND") Then 'don't need the comparators
                If SearchText = "" Then
                    SearchText = Chr(34) + SearchArray(x) + "*" + Chr(34)
                Else
                    If UseOr Then
                        SearchText = SearchText + " or " + Chr(34) + SearchArray(x) + "*" + Chr(34)
                    Else
                        SearchText = SearchText + " and " + Chr(34) + SearchArray(x) + "*" + Chr(34)
                    End If

                End If
            End If

        Next

        Return SearchText


    End Function


    Public Sub UpdateKBStats(ByVal TicketID As String, ByVal ParentOrgID As String, ByVal SearchTerm As String, ByVal UsersIP As String)
        'Updates the KBViewStatistics table so we can track how many people are using the external KB

        Try

            Dim sqlStatement As String = "insert into KBStats (OrganizationID, KBArticleID, ViewIP, ViewDateTime, SearchTerm) values(@OrganizationID, @KBArticleID, @ViewIP, GetUTCDate(), @SearchTerm)"
            Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)
            Try
                command.Connection.Open()

                command.Parameters.AddWithValue("@OrganizationID", ParentOrgID)
                command.Parameters.AddWithValue("@KBArticleID", TicketID)
                command.Parameters.AddWithValue("@ViewIP", UsersIP)
                command.Parameters.AddWithValue("@SearchTerm", SearchTerm)

                command.ExecuteNonQuery()
            Finally

                command.Connection.Close()
                connection.Close()
            End Try

        Catch ex As Exception


        End Try

    End Sub

    Public Function GetProductNameFromID(ByRef OrganizationID As String, ByVal ProductID As String) As String

        'This will return the product name when an ID is passed

        Dim sqlStatement As String = "select name from products where productid = @ProductID and organizationid = @OrganizationID"
        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)

        command.Connection.Open()

        command.Parameters.AddWithValue("@OrganizationID", OrganizationID)
        command.Parameters.AddWithValue("@ProductID", ProductID)



        Dim reader As SqlDataReader = command.ExecuteReader
        Try
            'return true if the ProductID exists for ths company
            If reader.Read Then
                Return reader(0)
            Else
                Return ""
            End If


        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try
    End Function



    Public Function GetProductIDFromTicketID(ByRef TicketID As String) As String

        'This will return the productID assocaited with a ticket ID

        Dim sqlStatement As String = "select productid from tickets where ticketid = @ticketid"
        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)

        command.Connection.Open()

        command.Parameters.AddWithValue("@TicketID", TicketID)




        Dim reader As SqlDataReader = command.ExecuteReader
        Try
            'return true if the ProductID exists for ths company
            If reader.Read Then
                Return reader(0).ToString
            Else
                Return ""
            End If


        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try
    End Function



    Public Sub SetNavigationMenu(ByVal SelectedItem As String, ByVal ReferencedMasterPage As MasterPage)

        'This routine is called by the inner page and sets the selected navigation page as well as the mouseover graphics for the other pages

        If SelectedItem = "NewTicket" Then
            CType(ReferencedMasterPage.FindControl("btn_NewTicket"), ImageButton).ImageUrl = "~/Protected/images/nav_new_tix_on.jpg"
        Else
            CType(ReferencedMasterPage.FindControl("btn_NewTicket"), ImageButton).ImageUrl = "~/Protected/images/nav_new_tix.jpg"
            CType(ReferencedMasterPage.FindControl("btn_NewTicket"), ImageButton).Attributes.Add("onmouseover", "this.src='images/nav_new_tix_on.jpg'")
            CType(ReferencedMasterPage.FindControl("btn_NewTicket"), ImageButton).Attributes.Add("onmouseout", "this.src='images/nav_new_tix.jpg'")
        End If

        If SelectedItem = "MyTickets" Then
            CType(ReferencedMasterPage.FindControl("btn_MyTickets"), ImageButton).ImageUrl = "~/Protected/images/nav_my_tix_on.jpg"
        Else
            CType(ReferencedMasterPage.FindControl("btn_MyTickets"), ImageButton).ImageUrl = "~/Protected/images/nav_my_tix.jpg"
            CType(ReferencedMasterPage.FindControl("btn_MyTickets"), ImageButton).Attributes.Add("onmouseover", "this.src='images/nav_my_tix_on.jpg'")
            CType(ReferencedMasterPage.FindControl("btn_MyTickets"), ImageButton).Attributes.Add("onmouseout", "this.src='images/nav_my_tix.jpg'")
        End If

        If SelectedItem = "MyOrgsTickets" Then
            CType(ReferencedMasterPage.FindControl("btn_OrgTickets"), ImageButton).ImageUrl = "~/Protected/images/nav_myorg_tix_on.jpg"
        Else
            CType(ReferencedMasterPage.FindControl("btn_OrgTickets"), ImageButton).ImageUrl = "~/Protected/images/nav_myorg_tix.jpg"
            CType(ReferencedMasterPage.FindControl("btn_OrgTickets"), ImageButton).Attributes.Add("onmouseover", "this.src='images/nav_myorg_tix_on.jpg'")
            CType(ReferencedMasterPage.FindControl("btn_OrgTickets"), ImageButton).Attributes.Add("onmouseout", "this.src='images/nav_myorg_tix.jpg'")
        End If

        If SelectedItem = "ClosedTickets" Then
            CType(ReferencedMasterPage.FindControl("btn_ClosedTickets"), ImageButton).ImageUrl = "~/Protected/images/nav_closed_tix_on.jpg"
        Else
            CType(ReferencedMasterPage.FindControl("btn_ClosedTickets"), ImageButton).ImageUrl = "~/Protected/images/nav_closed_tix.jpg"
            CType(ReferencedMasterPage.FindControl("btn_ClosedTickets"), ImageButton).Attributes.Add("onmouseover", "this.src='images/nav_closed_tix_on.jpg'")
            CType(ReferencedMasterPage.FindControl("btn_ClosedTickets"), ImageButton).Attributes.Add("onmouseout", "this.src='images/nav_closed_tix.jpg'")
        End If

        If SelectedItem = "KnowledgeBase" Then
            CType(ReferencedMasterPage.FindControl("btn_KnowledgeBase"), ImageButton).ImageUrl = "~/Protected/images/nav_knowledge_base_on.jpg"

        Else
            CType(ReferencedMasterPage.FindControl("btn_KnowledgeBase"), ImageButton).ImageUrl = "~/Protected/images/nav_knowledge_base.jpg"
            CType(ReferencedMasterPage.FindControl("btn_KnowledgeBase"), ImageButton).Attributes.Add("onmouseover", "this.src='images/nav_knowledge_base_on.jpg'")
            CType(ReferencedMasterPage.FindControl("btn_KnowledgeBase"), ImageButton).Attributes.Add("onmouseout", "this.src='images/nav_knowledge_base.jpg'")
        End If


        If SelectedItem = "Search" Then
            CType(ReferencedMasterPage.FindControl("btn_Search"), ImageButton).ImageUrl = "~/Protected/images/nav_search_on.jpg"
        Else
            CType(ReferencedMasterPage.FindControl("btn_Search"), ImageButton).ImageUrl = "~/Protected/images/nav_search.jpg"
            CType(ReferencedMasterPage.FindControl("btn_Search"), ImageButton).Attributes.Add("onmouseover", "this.src='images/nav_search_on.jpg'")
            CType(ReferencedMasterPage.FindControl("btn_Search"), ImageButton).Attributes.Add("onmouseout", "this.src='images/nav_search.jpg'")
        End If

        If SelectedItem = "Products" Then
            CType(ReferencedMasterPage.FindControl("btn_Products"), ImageButton).ImageUrl = "~/Protected/images/nav_products_on.jpg"
        Else
            CType(ReferencedMasterPage.FindControl("btn_products"), ImageButton).ImageUrl = "~/Protected/images/nav_products.jpg"
            CType(ReferencedMasterPage.FindControl("btn_products"), ImageButton).Attributes.Add("onmouseover", "this.src='images/nav_products_on.jpg'")
            CType(ReferencedMasterPage.FindControl("btn_products"), ImageButton).Attributes.Add("onmouseout", "this.src='images/nav_products.jpg'")
        End If







    End Sub

    Public Sub ExportToExcel(ByVal RadGrid As Telerik.Web.UI.RadGrid)

        'This will export the data in a grid to excel
        RadGrid.ExportSettings.ExportOnlyData = True
        RadGrid.ExportSettings.IgnorePaging = True
        RadGrid.ExportSettings.OpenInNewWindow = True
        RadGrid.ExportSettings.Excel.Format = GridExcelExportFormat.ExcelML 'this fixes grid line issue (1717)




        RadGrid.ExportSettings.FileName = "Tickets"

        'per help file, this should solve potential SSL issue
        RadGrid.Page.Response.ClearHeaders()
        RadGrid.Page.Response.Cache.SetCacheability(HttpCacheability.[Private])

        RadGrid.MasterTableView.ExportToExcel()

    End Sub

    Public Function UseEuropeDate(ByVal OrgID As String) As Boolean
        Dim sqlStatement As String = "select UseEuropeDate from Organizations where organizationid = @orgid"


        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrgID", OrgID))

        command.Connection.Open()
        Try



            Dim reader As SqlDataReader = command.ExecuteReader


            'return the string
            Try


                Try

                    If reader.Read Then
                        Return reader(0)
                    Else
                        Return False
                    End If
                Catch ex As Exception
                    Return False
                End Try

            Finally
                command.Connection.Close()
                connection.Close()
                reader.Close()
            End Try

            command.Connection.Close()
        Catch ex As Exception
            Return False
        End Try
    End Function


    Public Sub ConvertGridTimeToLocal(ByRef e As GridItemEventArgs, ByVal TimeZone As String, ByVal OrganizationID As String)
        'This will take the grid information and convert the UTC time to local time based on the Organization's TZ settings

        'TEST CODE




        Dim TZ As System.TimeZoneInfo

        Try
            TZ = TimeZoneInfo.FindSystemTimeZoneById(TimeZone)
        Catch ex As Exception
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")
        End Try




        If TypeOf e.Item Is GridDataItem Then


            Dim item As GridDataItem = TryCast(e.Item, GridDataItem)





            For Each column As GridColumn In DirectCast((item), GridItem).OwnerTableView.Columns



                If column.DataTypeName = "System.DateTime" Then



                    Dim s As String = item(column).Text

                    If s <> "" Then




                        Try


                            Dim dt As DateTime = DateTime.Parse(s)


                            item(column).Text = TimeZoneInfo.ConvertTimeFromUtc(dt, TZ).ToString()
                        Catch generatedExceptionName As Exception




                        End Try

                    End If

                End If

            Next

        End If
    End Sub

    Public Function ConvertStringTimeToLocal(ByVal StrTime As String, ByVal TimeZone As String) As DateTime
        'TEST CODE
        'Thread.CurrentThread.CurrentCulture = New CultureInfo("en-gb")

        Dim TZ As System.TimeZoneInfo

        Try
            TZ = TimeZoneInfo.FindSystemTimeZoneById(TimeZone)
        Catch ex As Exception
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")
        End Try

        Return TimeZoneInfo.ConvertTimeFromUtc(StrTime, TZ)

    End Function


    Public Function GetTimeZone(ByRef OrganizationID As String) As String
        'This will return the Font Family stored in Portal Options

        Dim sqlStatement As String = "select TimeZoneID from Organizations where organizationid = @orgid"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrgID", OrganizationID))

        command.Connection.Open()
        Try



            Dim reader As SqlDataReader = command.ExecuteReader


            'return the string
            Try


                Try

                    If reader.Read Then
                        Return reader(0)
                    Else
                        Return "Central Standard Time"
                    End If
                Catch ex As Exception
                    Return "Central Standard Time"
                End Try

            Finally
                command.Connection.Close()
                connection.Close()
                reader.Close()
            End Try

            command.Connection.Close()
        Catch ex As Exception
            Return True 'use recaptcha if there's an error
        End Try
    End Function

    Public Sub GetPersonalInfo(ByRef Firstname As String, ByRef Lastname As String, ByRef Title As String, ByVal UserID As String)
        'This will pass the user's infor back

        Dim sqlStatement As String = "select Firstname, lastname, title from Users where UserID = @UserID"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@UserID", UserID))

        command.Connection.Open()
        Try



            Dim reader As SqlDataReader = command.ExecuteReader


            'return the string
            Try


                Try

                    If reader.Read Then
                        Firstname = reader(0).ToString
                        Lastname = reader(1).ToString
                        Title = reader(2).ToString
                    Else
                        Firstname = ""
                        Lastname = ""
                        Title = ""
                    End If
                Catch ex As Exception
                    Firstname = ""
                    Lastname = ""
                    Title = ""
                End Try

            Finally
                command.Connection.Close()
                connection.Close()
                reader.Close()
            End Try

            command.Connection.Close()
        Catch ex As Exception
            Firstname = ""
            Lastname = ""
            Title = ""
        End Try
    End Sub


    Public Sub UpdatePersonalInfo(ByRef Firstname As String, ByRef Lastname As String, ByRef Title As String, ByVal UserID As String)
        'This will pass the user's infor back

        Dim sqlStatement As String = "Update Users set Firstname=@Firstname, lastname=@lastname, title=@title where UserID = @UserID"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@UserID", UserID))
        command.Parameters.Add(New SqlParameter("@firstname", Firstname))
        command.Parameters.Add(New SqlParameter("@lastname", Lastname))
        command.Parameters.Add(New SqlParameter("@title", Title))


        command.Connection.Open()
        Try

            command.ExecuteNonQuery()

            command.Connection.Close()
        Catch ex As Exception

        End Try
    End Sub


    Public Function ConvertReportTimes(ByVal ReportText As String, ByVal tz As String) As String
        'This will go through the XML text of a report and replace the times with local times

        Dim StartPosition As Integer = 1
        Dim Location As Integer
        Dim TimeString As String

        Dim TempReportText As String = ReportText


        While InStr(StartPosition, TempReportText, "Type=""DateTime"">", CompareMethod.Text) > 0

            Location = InStr(StartPosition, TempReportText, "Type=""DateTime"">", CompareMethod.Text)
            TimeString = TempReportText.Substring(Location + 15, 23)

            TimeString = ConvertStringTimeToLocal(TimeString, tz).ToString("s") + ".000" 'converts the new time to the correct time for the XML file and appends the milliseconds


            TempReportText = TempReportText.Remove(Location + 15, 23) 'remove the old time
            TempReportText = TempReportText.Insert(Location + 15, TimeString) 'insert new timestring

            StartPosition = Location + 1 'start farther down the line
        End While

        Return TempReportText


    End Function


    Public Function GetUserCultureInfo(ByVal UserID As String) As String


        Dim sqlStatement As String = "select CultureInfo from Users where UserID = @UserID"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@UserID", UserID))

        command.Connection.Open()
        Try



            Dim reader As SqlDataReader = command.ExecuteReader


            'return the string
            Try


                Try

                    If reader.Read Then
                        Return reader(0).ToString
                    Else
                        Return ""
                    End If
                Catch ex As Exception
                    Return ""
                End Try

            Finally
                command.Connection.Close()
                connection.Close()
                reader.Close()
            End Try

            command.Connection.Close()
        Catch ex As Exception
            Return ""
        End Try
    End Function


    Public Function GetOrganizationCultureInfo(ByVal OrgID As String) As String


        Dim sqlStatement As String = "select CultureInfo from Organizations where OrganizationID = @OrgID"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrgID", OrgID))

        command.Connection.Open()
        Try



            Dim reader As SqlDataReader = command.ExecuteReader


            'return the string
            Try


                Try

                    If reader.Read Then
                        Return reader(0).ToString
                    Else
                        Return ""
                    End If
                Catch ex As Exception
                    Return ""
                End Try

            Finally
                command.Connection.Close()
                connection.Close()
                reader.Close()
            End Try

            command.Connection.Close()
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function GetCultureInfo(ByVal OrgID As String, ByVal UserID As String) As String
        'Checks the Users's culture value then the org's culture value and defaults to US if none

        Dim CultureInfo As String = "en-US"

        CultureInfo = GetUserCultureInfo(UserID)
        If CultureInfo = "" Then
            CultureInfo = GetOrganizationCultureInfo(OrgID)
            If CultureInfo = "" Then
                CultureInfo = "en-US"
            End If
        End If

        Return CultureInfo


    End Function

End Class
