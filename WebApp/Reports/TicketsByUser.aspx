<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TicketsByUser.aspx.vb" Inherits="_Default" %>


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
<div style="text-align: center; margin:0 auto"> 
    
    <div class="strong">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        Tickets Open By User</div>
    <div>
    </div>
    <div>
    </div>
</div>

    <div style="width: 100%; height:95%; overflow:auto; text-align: center; margin:0 auto;" >
        <br />
        
    
        <telerik:RadChart ID="RadChart1" runat="server" 
            DataSourceID="TicketsOpenByUser" Height="500px" SeriesOrientation="Horizontal" 
            Skin="Web20" style="text-align: center" Width="700px">
            <Series>
                <cc1:ChartSeries DataYColumn="Tickets" Name="Tickets">
                    <Appearance>
                        <FillStyle FillType="ComplexGradient">
                            <FillSettings>
                                <ComplexGradient>
                                    <cc1:GradientElement Color="213, 247, 255" />
                                    <cc1:GradientElement Color="193, 239, 252" Position="0.5" />
                                    <cc1:GradientElement Color="157, 217, 238" Position="1" />
                                </ComplexGradient>
                            </FillSettings>
                        </FillStyle>
                        <TextAppearance TextProperties-Color="103, 136, 190">
                        </TextAppearance>
                    </Appearance>
                </cc1:ChartSeries>
            </Series>
            <PlotArea>
                <XAxis AutoScale="False" DataLabelsColumn="Name" MaxValue="5" MinValue="1" 
                    Step="1">
                    <Appearance Color="149, 184, 206" MajorTick-Color="149, 184, 206" 
                        ValueFormat="General">
                        <MajorGridLines Color="209, 221, 238" Width="0" />
                    </Appearance>
                    <AxisLabel>
                        <Appearance Position-AlignedPosition="BottomRight" RotationAngle="270">
                        </Appearance>
                    </AxisLabel>
                    <Items>
                        <cc1:ChartAxisItem Value="1">
                            <TextBlock Text="Harrington, Eric">
                            </TextBlock>
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="2">
                            <TextBlock Text="Johnson, Robert">
                            </TextBlock>
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="3">
                            <TextBlock Text="Jones, Kevin">
                            </TextBlock>
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="4">
                            <TextBlock Text="Krant, Steve">
                            </TextBlock>
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="5">
                            <TextBlock Text="Person, Test">
                            </TextBlock>
                        </cc1:ChartAxisItem>
                    </Items>
                </XAxis>
                <YAxis>
                    <Appearance Color="149, 184, 206" MajorTick-Color="149, 184, 206" 
                        MinorTick-Color="149, 184, 206">
                        <MajorGridLines Color="209, 221, 238" />
                        <MinorGridLines Color="209, 221, 238" />
                    </Appearance>
                    <AxisLabel>
                        <Appearance RotationAngle="0">
                        </Appearance>
                    </AxisLabel>
                </YAxis>
                <YAxis2>
                    <AxisLabel>
                        <Appearance RotationAngle="0">
                        </Appearance>
                    </AxisLabel>
                </YAxis2>
                <Appearance Dimensions-Margins="18%, 10%, 12%, 25%" 
                    Position-AlignedPosition="Top">
                    <FillStyle FillType="Solid" MainColor="249, 250, 251">
                    </FillStyle>
                    <Border Color="149, 184, 206" />
                </Appearance>
            </PlotArea>
            <Appearance>
                <Border Color="103, 136, 190" />
            </Appearance>
            <ChartTitle>
                <Appearance>
                    <FillStyle MainColor="">
                    </FillStyle>
                </Appearance>
                <TextBlock Text="Open Tickets by User">
                    <Appearance TextProperties-Color="0, 0, 79">
                    </Appearance>
                </TextBlock>
            </ChartTitle>
            <Legend Visible="False">
                <Appearance Dimensions-Margins="17.6%, 3%, 1px, 1px" 
                    Dimensions-Paddings="2px, 8px, 6px, 3px" Position-AlignedPosition="TopRight" 
                    Visible="False">
                    <Border Color="165, 190, 223" />
                </Appearance>
            </Legend>
        </telerik:RadChart>
    
        <br />
        <br />
        
    
   
</div>    
    
    <asp:SqlDataSource ID="TicketsOpenByUser" runat="server" 
        ConnectionString="<%$ ConnectionStrings:TSConnectionString %>" 
        SelectCommand="select u.lastname+', '+u.firstname as Name, 
(select count(*) from tickets as t, ticketstatuses as ts, tickettypes as tt where t.ticketstatusid = ts.ticketstatusid and t.userid = u.userid and  t.tickettypeid = tt.tickettypeid and ts.isclosed = 0) as Tickets
from users  as u 
where u.organizationid = @OrganizationID
and isnull(u.isactive,1)=1 and isnull(u.markdeleted,0)&lt;&gt;1
order by u.lastname ">
        <SelectParameters>
            <asp:Parameter Name="OrganizationID" />
        </SelectParameters>
    </asp:SqlDataSource>
    </form>
</body>
</html>
