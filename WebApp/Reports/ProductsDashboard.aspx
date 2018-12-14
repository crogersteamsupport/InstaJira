<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ProductsDashboard.aspx.vb" Inherits="_Default" %>


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

 

    .style1
    {
        width: 100%;
    }

 

</style>
</head>
<body>
<div  text-align: center; margin:0 auto"> 
    <form id="form1" runat="server">
    <div class=strong>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        Tickets By Product</div>
    <div>
    </div>
    <div>
    </div>
</div>

    <div style="width: 100%; height:95%; overflow:auto; text-align: center; margin:0 auto;" >
    <div style="margin:0 auto;">
        <br />
        
    
     <iframe id="TicketsByProduct"  runat="server" frameborder="0" height="300" 
            width="400"></iframe>
    
        <br />
        <br />
    </div>    
    <div>
        <table align="center" class="style1">
            <tr>
                <td>
        
    
     <iframe id="IssuesByProduct"  runat="server" frameborder="0" height="300" name="I1" 
                        width="400"></iframe>
    
                </td>
                <td>
        
    
     <iframe id="TasksByProduct"  runat="server" frameborder="0" height="300" name="I2" width="400"></iframe>
    
                </td>
            </tr>
            <tr>
                <td>
        
    
     <iframe id="OpenBugs"  runat="server" frameborder="0" height="300" name="I3" width="400"></iframe>
    
                </td>
                <td>
        
    
     <iframe id="OpenFeatures"  runat="server" frameborder="0" height="300" name="I4" width="400"></iframe>
    
                </td>
            </tr>
        </table>
    
    </div>
    
    <asp:SqlDataSource ID="OpenTicketsByProduct" runat="server" 
        ConnectionString="<%$ ConnectionStrings:TSConnectionString %>" 
        SelectCommand="select p.name as ProductName, count(*) as NumTickets 
from tickets as t, products as p, ticketstatuses as ts, tickettypes as tt 
where t.productid = p.productid   
and t.organizationid = @OrganizationID   
and t.ticketstatusid = ts.ticketstatusid 
and ts.isclosed = 0  
and t.tickettypeid = tt.tickettypeid 

group by p.name">
        <SelectParameters>
            <asp:Parameter Name="OrganizationID" />
        </SelectParameters>
    </asp:SqlDataSource>
    </form>
    
    <asp:SqlDataSource ID="OpenBugsByProduct" runat="server" 
        ConnectionString="<%$ ConnectionStrings:TSConnectionString %>" 
        
            SelectCommand="select p.name as ProductName, count(*) as NumTickets from tickets as t, products as p, ticketstatuses as ts, tickettypes as tt where t.productid = p.productid   and t.organizationid = @OrganizationID   and t.ticketstatusid = ts.ticketstatusid and ts.isclosed = 0  and t.tickettypeid = tt.tickettypeid and tt.name = 'bugs' group by p.name">
        <SelectParameters>
            <asp:Parameter Name="OrganizationID" />
        </SelectParameters>
    </asp:SqlDataSource>
    
    <asp:SqlDataSource ID="OpenFeaturesByProduct" runat="server" 
        ConnectionString="<%$ ConnectionStrings:TSConnectionString %>" 
        SelectCommand="select p.name as ProductName, count(*) as NumTickets from tickets as t, products as p, ticketstatuses as ts, tickettypes as tt where t.productid = p.productid   and t.organizationid = @OrganizationID   and t.ticketstatusid = ts.ticketstatusid and ts.isclosed = 0  and t.tickettypeid = tt.tickettypeid 
and tt.name = 'features' group by p.name">
        <SelectParameters>
            <asp:Parameter Name="OrganizationID" />
        </SelectParameters>
    </asp:SqlDataSource>
    
    <asp:SqlDataSource ID="OpenTasksByProduct" runat="server" 
        ConnectionString="<%$ ConnectionStrings:TSConnectionString %>" 
        
            SelectCommand="select p.name as ProductName, count(*) as NumTickets from tickets as t, products as p, ticketstatuses as ts, tickettypes as tt where t.productid = p.productid   and t.organizationid = @OrganizationID   and t.ticketstatusid = ts.ticketstatusid and ts.isclosed = 0  and t.tickettypeid = tt.tickettypeid and tt.name = 'tasks' group by p.name">
        <SelectParameters>
            <asp:Parameter Name="OrganizationID" />
        </SelectParameters>
    </asp:SqlDataSource>
    
    <asp:SqlDataSource ID="OpenIssuesByProduct" runat="server" 
        ConnectionString="<%$ ConnectionStrings:TSConnectionString %>" 
        
            SelectCommand="select p.name as ProductName, count(*) as NumTickets  from tickets as t, products as p, ticketstatuses as ts, tickettypes as tt where t.productid = p.productid   and t.organizationid = @OrganizationID   and t.ticketstatusid = ts.ticketstatusid and ts.isclosed = 0  and t.tickettypeid = tt.tickettypeid and tt.name = 'issues' group by p.name">
        <SelectParameters>
            <asp:Parameter Name="OrganizationID" />
        </SelectParameters>
    </asp:SqlDataSource>
    </body>
</html>
