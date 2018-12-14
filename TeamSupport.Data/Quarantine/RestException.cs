using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace TeamSupport.Api
{
  public class RestException : Exception
  {
    private HttpStatusCode _httpStatusCode;

    public HttpStatusCode HttpStatusCode
    {
      get { return _httpStatusCode; }
      set { _httpStatusCode = value; }
    }

    public RestException(HttpStatusCode httpStatusCode)
      : base("Error Code: " + httpStatusCode.ToString())
    {
      _httpStatusCode = httpStatusCode;
      
    }

    public RestException(HttpStatusCode httpStatusCode, string message)
      : base(message)
    {
      _httpStatusCode = httpStatusCode;
    }

    public RestException(HttpStatusCode httpStatusCode, string message, Exception innerException)
      : base(message, innerException)
    {
      _httpStatusCode = httpStatusCode;
    }
  }
}
