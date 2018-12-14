Imports System.Net
Imports System.IO
Imports System.Text

'the code contained herein is adapted from  http://stackoverflow.com/questions/219827/multipart-forms-from-c-sharp-client


<Serializable()> Public Module WebHelpers

  Private encoding As Encoding = encoding.UTF8

  ''' <summary>
  ''' Post a group of multipart parameters to a url.
  ''' </summary>
  ''' <param name="postUrl">the url to post to</param>
  ''' <param name="userAgent">the useragent to use for the post</param>
  ''' <param name="postParameters">the parameters to post</param>
  ''' <returns>the HTTP response code from the post</returns>
  ''' <remarks></remarks>
  Public Function MultipartFormDataPost(ByVal postUrl As Uri, ByVal userAgent As String, ByVal postParameters As Dictionary(Of String, Object)) As HttpWebResponse
    Dim formDataBoundary As String = Guid.NewGuid.ToString()
    Dim contentType As String = "multipart/form-data; boundary=" & formDataBoundary

    Dim formData As Byte() = GetMultipartFormData(postParameters, formDataBoundary)


    Return PostForm(postUrl, userAgent, contentType, formData)
  End Function

  ''' <summary>
  ''' Post a group of multipart parameters to a url mimicking the way it is done by a html form
  ''' </summary>
  ''' <param name="postUrl"></param>
  ''' <param name="userAgent"></param>
  ''' <param name="contentType"></param>
  ''' <param name="formData"></param>
  ''' <returns>the HTTP response code from the post</returns>
  ''' <remarks>used by MultipartFormDataPost</remarks>
  Private Function PostForm(ByVal postUrl As Uri, ByVal userAgent As String, ByVal contentType As String, ByVal formData As Byte()) As HttpWebResponse
    Dim request As HttpWebRequest = WebRequest.Create(postUrl)

    If request Is Nothing Then
      Throw New NullReferenceException("Request is not a HTTP request")
    End If

    request.Method = "POST"
    request.ContentType = contentType
    request.UserAgent = userAgent
    request.ContentLength = formData.Length
    request.Timeout = 10 * 60000 'give request 10 minutes

    Using postStream As Stream = request.GetRequestStream()
      postStream.Write(formData, 0, formData.Length)
      postStream.Close()
    End Using

    Return request.GetResponse()
  End Function

  ''' <summary>
  ''' Converts a list of parameters to a byte array for posting
  ''' </summary>
  ''' <param name="postParameters">the parameters</param>
  ''' <param name="boundary">a multipart form boundary</param>
  ''' <returns>a byte array containing the parameter data</returns>
  ''' <remarks></remarks>
  Private Function GetMultipartFormData(ByVal postParameters As Dictionary(Of String, Object), ByVal boundary As String) As Byte()
    Dim formDataStream As Stream = New MemoryStream()

    For Each param In postParameters
      If TypeOf (param.Value) Is Byte() Then
        Dim fileData As Byte() = param.Value

        Dim header As String = String.Format("--{0}{3}Content-Disposition: form-data; name=""{1}""; filename=""{2}"";{3}Content-Type: text/csv{3}{3}", boundary, param.Key, param.Key & ".csv", Environment.NewLine)
        formDataStream.Write(encoding.GetBytes(header), 0, header.Length)

        formDataStream.Write(fileData, 0, fileData.Length)
      Else
        Dim postData As String = String.Format("--{0}{3}Content-Disposition: form-data; name=""{1}""{3}{3}{2}{3}", boundary, param.Key, param.Value, Environment.NewLine)
        formDataStream.Write(encoding.GetBytes(postData), 0, postData.Length)
      End If
    Next

    'add the end of the request
    Dim footer As String = Environment.NewLine & "--" & boundary & "--" & Environment.NewLine
    formDataStream.Write(encoding.GetBytes(footer), 0, footer.Length)

    'dump the stream into a byte()
    formDataStream.Position = 0
    Dim formData(formDataStream.Length) As Byte
    formDataStream.Read(formData, 0, formData.Length)
    formDataStream.Close()

    Return formData
  End Function


End Module
