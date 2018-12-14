<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TicketsByProduct.aspx.cs" Inherits="Charts_TicketsByProduct" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
  <title></title>
  <style type="text/css">
    body { background: #fff; }
  </style>

  <script type="text/javascript" src="https://www.google.com/jsapi"></script>

</head>
<body>
  <form id="form1" runat="server">
  <telerik:RadScriptManager ID="RadScriptManager1" runat="server" EnablePageMethods="true">
  </telerik:RadScriptManager>
  <div>
    <div id="chart_div">
    </div>
  </div>
  </form>
</body>

  <script type="text/javascript">
      if (google != null) {
          google.load("visualization", "1", { packages: ["corechart"] });
          google.setOnLoadCallback(drawChart);
      }

      function drawChart() {
          PageMethods.GetData(function (result) {
              var data = new google.visualization.DataTable();
              data.addColumn('string', 'Product');
              data.addColumn('number', 'Issues');
              data.addColumn('number', 'Tasks');
              data.addColumn('number', 'Bugs');
              data.addColumn('number', 'Features');
              data.addRows(result.length);
              for (var i = 0; i < result.length; i++) {
                  data.setValue(i, 0, result[i].Product);
                  data.setValue(i, 1, result[i].Issues);
                  data.setValue(i, 2, result[i].Tasks);
                  data.setValue(i, 3, result[i].Bugs);
                  data.setValue(i, 4, result[i].Features);
              }

              var chart = new google.visualization.BarChart(document.getElementById('chart_div'));
              chart.draw(data, { <%=Request.UrlReferrer.PathAndQuery.Contains("Dashboard") ? "" : "height:500, "%>isStacked: true, fontSize:10 });
          });
      }
    
  </script>


</html>
