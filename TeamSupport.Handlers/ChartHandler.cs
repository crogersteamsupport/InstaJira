namespace TeamSupport.Handlers.Chart
{
  using System;
  using System.Web;

  public class ChartHandler : IHttpHandler
  {
    public bool IsReusable
    {
      get { return true; }
    }

    /// <summary>
    /// Processes HTTP web requests directed to this HttpHandler.
    /// </summary>
    /// <param name="context">An HttpContext object that provides references 
    /// to the intrinsic server objects (for example, Request, Response, 
    /// Session, and Server) used to service HTTP requests.</param>
    public void ProcessRequest(HttpContext context)
    {
      // Process the request to export chart.
      ExportChart.ProcessExportRequest(context);
    }
  }
}