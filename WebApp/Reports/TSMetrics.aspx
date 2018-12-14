<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TSMetrics.aspx.vb" Inherits="_Default" %>


<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Charting" tagprefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
      <style type="text/css">

    html, body, form

    {

        height: 100%;

        margin: 0px;

        padding: 0px;

        overflow: hidden;

    }

  </style>
<style type="text/css">

body

{

      font-family: "Lucida Grande",Helvetica,Arial,Verdana,sans-serif;

      font-size: 12px;

      color: #15428B;

      background-color: #DBE6F4;

}

 

strong

{

  color: #5779BD;

}

 

a, a:link, a:active, a:visited

{

  text-decoration:underline;

  color:#15428B;

  outline: 0;

  border:none;

}

 

a:focus

{

  outline: 0;

  border:none;

}

 

    .strong
    {
        text-align: center;
        font-size: x-large;
    }

 

</style>
</head>
<body>
    <form id="form1" runat="server">
    <div class=strong>TeamSupport Metrics</div>
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
    </div>
    
    <div style="text-align: center; background-color: #99CCFF">
        <br />
                Start Date:
        <telerik:RadDateTimePicker ID="StartDate" Runat="server" Skin="Outlook">
<TimePopupButton ImageUrl="" HoverImageUrl=""></TimePopupButton>

<TimeView CellSpacing="-1"></TimeView>

<Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" 
                ViewSelectorText="x" Skin="Outlook"></Calendar>

<DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
        </telerik:RadDateTimePicker>
        &nbsp;
        End Date:<telerik:RadDateTimePicker ID="EndDate" Runat="server" Skin="Outlook">
<TimePopupButton ImageUrl="" HoverImageUrl=""></TimePopupButton>

<TimeView CellSpacing="-1"></TimeView>

<Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" 
                ViewSelectorText="x" Skin="Outlook"></Calendar>

<DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
        </telerik:RadDateTimePicker>
        <br />
        <asp:Button ID="Button1" runat="server" Text="Generate Graphs" />
        <br />
        <br />
    </div>
    
    
    <div style="width: 100%; height:80%; overflow:auto; text-align: center; margin:0 auto;" >
    <div>
        
    
     <iframe id="frm_loginsperday"  runat="server" frameborder="0" height="400" 
            name="I1" width="700"></iframe>
    
    </div>
    <div></div>
    <div style="text-align: center">
    
    
     <iframe id="frm_ActionsPerDay"  runat="server" frameborder="0" height="400" 
            name="I2" width="700"></iframe>
    
    </div>
    <div>
    </div>
    <asp:SqlDataSource ID="LoginsPerDay" runat="server" 
        ConnectionString="<%$ ConnectionStrings:TSConnectionString %>" 
        
            SelectCommand="Select CONVERT(VARCHAR,dt.dtime,1) AS 'Date',Count(lh.userid) as 'NumLogins' FROM dbo.udfDateTimes(@StartDate,@enddate, 1, 'day') AS dt LEFT JOIN LoginHistory lh ON dt.dtime = CAST(FLOOR(CAST(lh.DateCreated AS FLOAT)) AS DATETIME)  group by dt.dtime">
        <SelectParameters>
            <asp:Parameter Name="StartDate" />
            <asp:Parameter Name="enddate" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="ActionsPerDay" runat="server" 
        ConnectionString="<%$ ConnectionStrings:TSConnectionString %>" 
        
            SelectCommand="Select CONVERT(VARCHAR,dt.dtime,1) AS 'Action Date',Count(a.datecreated) as 'Number of Actions' FROM dbo.udfDateTimes(@StartDate,@enddate, 1, 'day') AS dt LEFT JOIN Actions a ON dt.dtime = CAST(FLOOR(CAST(a.DateCreated AS FLOAT)) AS DATETIME)  group by dt.dtime">
        <SelectParameters>
            <asp:Parameter Name="StartDate" />
            <asp:Parameter Name="enddate" />
        </SelectParameters>
    </asp:SqlDataSource>
    </form>
</body>
</html>
