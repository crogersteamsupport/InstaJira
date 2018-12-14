<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CustomerTickets.aspx.vb" Inherits="_Default" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
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
<div  text-align: center; margin:0 auto"> 
    <form id="form1" runat="server">
    <div class=strong>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        Tickets by Customer</div>
        <br />
    <div>
    
        <table align="center" class="style2">
            <tr>
                <td class="style3">
                    Customer:<telerik:RadComboBox ID="cb_CustomerList" Runat="server" 
                        DataSourceID="CustomerList" DataTextField="name" 
                        DataValueField="organizationid" Width="300px" Height="250px">
<CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                    </telerik:RadComboBox>
                </td>
                <td>
                    Start Date:<telerik:RadDatePicker ID="dp_StartDate" Runat="server">
<Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x"></Calendar>

<DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
                    </telerik:RadDatePicker>
                </td>
                <td>
                    End Date:<telerik:RadDatePicker 
                        ID="dp_enddate" Runat="server">
<Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x"></Calendar>

<DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
                    </telerik:RadDatePicker>
                </td>
            </tr>
        </table>
    
    </div>
    <div style="text-align: center">
        <asp:Button ID="Button1" runat="server" Text="Button" />
    </div>
</div>

    <div style="width: 100%; height:95%; overflow:auto; text-align: center; margin:0 auto;" >
    <div style="margin:0 auto;">
        <br />
        
    
     <iframe id="TicketsByCustomer"  runat="server" frameborder="0" height="300" 
            width="400"></iframe>
    
        <br />
        <br />
    </div>    
    <div>
        <table align="center" class="style1">
            <tr>
                <td>
        
    
                    &nbsp;</td>
                <td>
        
    
                    &nbsp;</td>
            </tr>
            <tr>
                <td>
        
    
                    &nbsp;</td>
                <td>
        
    
                    &nbsp;</td>
            </tr>
        </table>
    
    </div>
    
    <asp:SqlDataSource ID="CustomerList" runat="server" 
        ConnectionString="<%$ ConnectionStrings:TSConnectionString %>" 
        SelectCommand="select name, organizationid 
from organizations
where parentid = @OrganizationID
" ProviderName="<%$ ConnectionStrings:TSConnectionString.ProviderName %>">
        <SelectParameters>
            <asp:Parameter Name="OrganizationID" />
        </SelectParameters>
    </asp:SqlDataSource>
    </form>
    
    <asp:SqlDataSource ID="sql_TicketsByCustomer" runat="server" 
        ConnectionString="<%$ ConnectionStrings:TSConnectionString %>" 
        
            SelectCommand="select tt.name,count(tt.name) as NumTickets from tickets as t, tickettypes as tt
where 
t.tickettypeid = tt.tickettypeid 
and t.datecreated &gt;= '1/1/10'
and t.datecreated &lt;= '5/1/10'
and t.organizationid = 1078
group by tt.name">
        <SelectParameters>
            <asp:Parameter Name="OrganizationID" />
        </SelectParameters>
    </asp:SqlDataSource>
    
    </body>
</html>
