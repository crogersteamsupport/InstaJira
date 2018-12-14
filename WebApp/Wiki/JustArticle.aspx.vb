Imports System.Data.SqlClient

Partial Class JustArticle
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim OrgID As String

        Try
            OrgID = Request.QueryString("OrganizationID")
        Catch ex As Exception
            OrgID = ""
        End Try




        Dim ArticleID As String
        Try
            ArticleID = Request.QueryString("ArticleID")
        Catch ex As Exception
            ArticleID = "default"
        End Try

        If OrgID <> "" And ArticleID <> "" Then
            Literal1.Text = DisplayArticle(ArticleID, OrgID)
        Else
            Literal1.Text = "No article!"

        End If

        'Security thoughts...We don't really care about the org ID here, do we?
        '  Well...Actually yes, since it makes trolling for articles a lot harder. If we just had the article ID then someone could
        '  quickly scan to get all articles.  If we add the Org ID then it's more difficult



    End Sub

    Public Function DisplayArticle(ByVal ArticleID As String, ByVal OrganizationID As String) As String

        Dim sqlStatement As String = "select Body, ModifiedDate, PublicView from WikiArticles where ArticleID = @ArticleID and OrganizationID = @OrganizationID and IsDeleted = 0"

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@ArticleID", ArticleID))

        command.Connection.Open()
        'Try



        Dim reader As SqlDataReader = command.ExecuteReader


        'return the string
        Try


            Try

                If reader.Read Then
                    If reader(2).ToString = "True" Then
                        Return ProcessWikiBody(reader(0).ToString)
                    Else
                        Return "This article is not publicly viewable."
                    End If

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
        'Catch ex As Exception
        'Return ""
        'End Try

    End Function

    Function GetColonToBracket(ByVal StrToParse As String, ByVal StartLocation As Integer) As String
        'Pass the start location (from an instr, for example) and this will find the text between the colon and the end bracket

        Dim NewString As String = ""
        Dim ColonLocation As Integer

        'scan to find colon
        For x As Integer = StartLocation To StrToParse.Length
            If StrToParse(x) = ":" Then
                ColonLocation = x
                Exit For
            End If
        Next


        If Not (ColonLocation >= StrToParse.Length) Then 'if the colon location's not at the end of the string...

            'Now we should have colon location
            Dim y As Integer = ColonLocation + 1
            Try
                'scan to find closing bracket
                While StrToParse(y) <> "]"
                    NewString = NewString + StrToParse(y)
                    y = y + 1
                End While

                Return NewString.Trim 'trim takes care of leading and trailing spaces

            Catch ex As Exception
                Return "" 'if we don't get a trailing ] then we should error out and return nothing
            End Try
        Else
            Return "" 'looks like a problem - Return nothing
        End If

    End Function

    Function GetTextAfterSpace(ByVal StrToParse As String, ByVal StartLocation As Integer) As String
        'Pass the start location (from an instr, for example) and this will find the text between the colon and the first space

        Dim NewString As String = ""
        Dim SpaceLocation As Integer

        'scan to find space
        For x As Integer = StartLocation To StrToParse.Length
            If StrToParse(x) = " " Then
                SpaceLocation = x
                Exit For
            End If
        Next


        If Not (SpaceLocation >= StrToParse.Length) Then 'if the colon location's not at the end of the string...

            'Now we should have colon location
            Dim y As Integer = SpaceLocation + 1
            Try
                'scan to find closing bracket
                While StrToParse(y) <> "]"
                    NewString = NewString + StrToParse(y)
                    y = y + 1
                End While

                Return NewString.Trim 'trim takes care of leading and trailing spaces

            Catch ex As Exception
                Return "" 'if we don't get a trailing ] then we should error out and return nothing
            End Try
        Else
            Return "" 'looks like a problem - Return nothing
        End If

    End Function

    Function GetFinalBracketLocation(ByVal StrToParse As String, ByVal StartLocation As Integer) As Integer
        'Pass the start location (from an instr, for example) and this will find the text between the colon and the end bracket

        Dim NewString As String = ""
        Dim BracketLocation As Integer

        'scan to find colon
        For x As Integer = StartLocation To StrToParse.Length
            If StrToParse(x) = "]" Then
                BracketLocation = x
                Exit For
            End If
        Next


        If Not (BracketLocation >= StrToParse.Length) Then
            Return BracketLocation
        Else
            Return 0
        End If

    End Function

    Public Function ChangePageLinks(ByRef body As String) As Boolean
        'returns TRUE is we have changed something 

        Dim OrgID As String

        Try
            OrgID = Request.QueryString("OrganizationID")
        Catch ex As Exception
            OrgID = ""
        End Try

        Dim TempBody As String = body

        Dim PageLink As Integer = InStr(body.ToUpper, "[ARTICLE:")

        If PageLink > 0 Then
            'if we have found the text in the string...
            'Return GetColonToBracket(DescriptionText, VersionLocation)
            Dim LinkLocation As String = GetColonToBracket(TempBody, PageLink) 'this should be the articleid
            Dim FinalBracket As Integer = GetFinalBracketLocation(TempBody, PageLink)

            Dim TextforLink As String = GetTextAfterSpace(TempBody, PageLink)
            If TextforLink = "" Then TextforLink = LinkLocation



            'Now we can cut from PageLink to FinalBracket
            TempBody = TempBody.Remove(PageLink - 1, FinalBracket - PageLink + 2)
            TempBody = TempBody.Insert(PageLink - 1, "<a href=""./viewpage.aspx?Organizationid=" + OrgID + "&articleid=" + LinkLocation + """>" + TextforLink + "</a>")
            body = TempBody
            Return True
        Else
            Return False 'return false if we didn't find anything
        End If


    End Function

    Public Function ProcessWikiBody(ByVal Body As String) As String
        'This routine will process the text in a Wiki article and replace certain text with the correct html

        'for now just return the same text


        While ChangePageLinks(Body)
            'should get all of the page links and replace them with <a> tags to the correct page
        End While

        'lets replace all instances of the viewpage call with viewarticle (so we just display in this pane)
        
        Body = Strings.Replace(Body, "viewpage", "justarticle", 1, -1, CompareMethod.Text)



        Return Body



    End Function


End Class
