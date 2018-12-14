<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserOpenTicketSummary.aspx.cs"
  Inherits="Charts_UserOpenTicketSummary" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
      google.load("visualization", "1", { packages: ["piechart"] });
      google.setOnLoadCallback(drawChart);
    }
    function drawChart() {
      PageMethods.GetData(function(result) {
        var data = new google.visualization.DataTable();
        data.addColumn('string', 'TicketType');
        data.addColumn('number', 'OpenCount');
        data.addRows(result.length);
        for (var i = 0; i < result.length; i++) {
          data.setValue(i, 0, result[i].Name);
          data.setValue(i, 1, result[i].Count);
        }
        var chart = new google.visualization.PieChart(document.getElementById('chart_div'));
        chart.draw(data, { width: 260, height: 250, is3D: true, titleFontSize: 0, legend: 'bottom', legendFontSize: 10 });
      });
    }
    
  </script>


</html>
