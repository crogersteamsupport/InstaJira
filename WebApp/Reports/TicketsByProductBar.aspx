<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TicketsByProductBar.aspx.vb" Inherits="_Default" %>


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
        Tickets by Product</div>
    <div>
    </div>
    <div>
    </div>
</div>

    <div style="width: 100%; height:95%; overflow:auto; text-align: center; margin:0 auto;" >
        <br />
        
    
        <telerik:RadChart ID="RadChart1" runat="server" 
            DataSourceID="TicketsByProduct" Height="500px" Width="600px" 
            Skin="WebBlue" DefaultType="StackedBar" SeriesOrientation="Horizontal">
            <Series>
                <cc1:ChartSeries DataYColumn="NumIssues" Name="NumIssues" Type="StackedBar">
                    <Appearance>
                        <FillStyle FillType="ComplexGradient" MainColor="94, 117, 142">
                            <FillSettings>
                                <ComplexGradient>
                                    <cc1:GradientElement Color="94, 117, 142" />
                                    <cc1:GradientElement Color="116, 138, 162" Position="0.5" />
                                    <cc1:GradientElement Color="139, 160, 183" Position="1" />
                                </ComplexGradient>
                            </FillSettings>
                        </FillStyle>
                        <TextAppearance TextProperties-Color="140, 140, 140">
                        </TextAppearance>
                        <Border Color="73, 86, 101" />
                    </Appearance>
                </cc1:ChartSeries>
                <cc1:ChartSeries DataYColumn="NumTasks" Name="NumTasks" Type="StackedBar">
                    <Appearance>
                        <FillStyle FillType="ComplexGradient" MainColor="164, 175, 187">
                            <FillSettings>
                                <ComplexGradient>
                                    <cc1:GradientElement Color="164, 175, 187" />
                                    <cc1:GradientElement Color="196, 203, 212" Position="0.5" />
                                    <cc1:GradientElement Color="221, 226, 233" Position="1" />
                                </ComplexGradient>
                            </FillSettings>
                        </FillStyle>
                        <TextAppearance TextProperties-Color="140, 140, 140">
                        </TextAppearance>
                        <Border Color="144, 150, 159" />
                    </Appearance>
                </cc1:ChartSeries>
                <cc1:ChartSeries DataYColumn="NumBugs" Name="NumBugs" Type="StackedBar">
                    <Appearance>
                        <FillStyle FillType="ComplexGradient" MainColor="230, 234, 237">
                            <FillSettings>
                                <ComplexGradient>
                                    <cc1:GradientElement Color="230, 234, 237" />
                                    <cc1:GradientElement Color="242, 244, 246" Position="0.5" />
                                    <cc1:GradientElement Color="White" Position="1" />
                                </ComplexGradient>
                            </FillSettings>
                        </FillStyle>
                        <TextAppearance TextProperties-Color="140, 140, 140">
                        </TextAppearance>
                        <Border Color="214, 214, 214" />
                    </Appearance>
                </cc1:ChartSeries>
                <cc1:ChartSeries DataYColumn="NumFeatures" Name="NumFeatures" Type="StackedBar">
                </cc1:ChartSeries>
            </Series>
            <PlotArea>
                <DataTable>
                    <Appearance Position-AlignedPosition="Center">
                    </Appearance>
                </DataTable>
                <XAxis AutoScale="False" DataLabelsColumn="Name" MaxValue="7" 
                    MinValue="1" Step="1" LayoutMode="Normal">
                    <Appearance Color="160, 160, 160">
                        <MajorGridLines Color="227, 227, 227" Width="0" />
                        <TextAppearance TextProperties-Color="140, 140, 140">
                        </TextAppearance>
                    </Appearance>
                    <AxisLabel>
                        <Appearance RotationAngle="45">
                        </Appearance>
                        <TextBlock>
                            <Appearance TextProperties-Color="140, 140, 140">
                            </Appearance>
                        </TextBlock>
                    </AxisLabel>
                    <Items>
                        <cc1:ChartAxisItem Value="1">
                            <TextBlock Text="abc">
                            </TextBlock>
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="2">
                            <TextBlock Text="abc">
                            </TextBlock>
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="3">
                            <TextBlock Text="abc">
                            </TextBlock>
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="4">
                            <TextBlock Text="abc">
                            </TextBlock>
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="5">
                            <TextBlock Text="abc">
                            </TextBlock>
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="6">
                            <TextBlock Text="abc">
                            </TextBlock>
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="7">
                            <TextBlock Text="abc">
                            </TextBlock>
                        </cc1:ChartAxisItem>
                    </Items>
                </XAxis>
                <YAxis>
                    <Appearance Color="160, 160, 160">
                        <MajorGridLines Color="227, 227, 227" />
                        <MinorGridLines Color="227, 227, 227" />
                    </Appearance>
                    <AxisLabel>
                        <Appearance RotationAngle="0">
                        </Appearance>
                        <TextBlock>
                            <Appearance TextProperties-Color="140, 140, 140">
                            </Appearance>
                        </TextBlock>
                    </AxisLabel>
                </YAxis>
                <YAxis2>
                    <AxisLabel>
                        <Appearance RotationAngle="0">
                        </Appearance>
                    </AxisLabel>
                </YAxis2>
                <Appearance Dimensions-Margins="18%, 24%, 12%, 20%" 
                    Position-AlignedPosition="Center" Position-Auto="False" Position-X="120" 
                    Position-Y="90">
                    <FillStyle FillType="Solid" MainColor="">
                    </FillStyle>
                </Appearance>
            </PlotArea>
            <Appearance>
                <FillStyle MainColor="249, 250, 251">
                </FillStyle>
                <Border Color="160, 170, 182" />
            </Appearance>
            <ChartTitle>
                <Appearance>
                    <FillStyle MainColor="">
                    </FillStyle>
                </Appearance>
                <TextBlock Text="Ticket Types by Product">
                    <Appearance TextProperties-Color="102, 102, 102" 
                        TextProperties-Font="Arial, 14pt">
                    </Appearance>
                </TextBlock>
            </ChartTitle>
            <Legend>
                <Appearance Dimensions-Margins="17.6%, 3%, 1px, 1px" 
                    Dimensions-Paddings="2px, 8px, 6px, 3px" 
                    Position-AlignedPosition="TopRight">
                    <ItemTextAppearance TextProperties-Color="102, 102, 102">
                    </ItemTextAppearance>
                    <FillStyle MainColor="216, 222, 227">
                    </FillStyle>
                    <Border Color="160, 170, 182" />
                </Appearance>
            </Legend>
        </telerik:RadChart>
    
        <br />
        <br />
        
    
   
</div>    
    
    <asp:SqlDataSource ID="TicketsByProduct" runat="server" 
        ConnectionString="<%$ ConnectionStrings:TSConnectionString %>" 
        SelectCommand="select p.name as ProductName, 
 (select count(*) from tickets as t, products as p2, ticketstatuses as ts, tickettypes as tt
  where t.productid = p2.productid
  and t.organizationid = @OrganizationID
  and t.ticketstatusid = ts.ticketstatusid 
  and ts.isclosed = 0  
  and t.tickettypeid = tt.tickettypeid
  and tt.name = 'issues'
  and p.productid = p2.productid) as NumIssues,
 (select count(*) from tickets as t, products as p2, ticketstatuses as ts, tickettypes as tt
  where t.productid = p2.productid
  and t.organizationid = @OrganizationID
  and t.ticketstatusid = ts.ticketstatusid 
  and ts.isclosed = 0  
  and t.tickettypeid = tt.tickettypeid
  and tt.name = 'tasks'
  and p.productid = p2.productid) as NumTasks,
 (select count(*) from tickets as t, products as p2, ticketstatuses as ts, tickettypes as tt
  where t.productid = p2.productid
  and t.organizationid = @OrganizationID
  and t.ticketstatusid = ts.ticketstatusid 
  and ts.isclosed = 0  
  and t.tickettypeid = tt.tickettypeid
  and tt.name = 'bugs'
  and p.productid = p2.productid) as NumBugs,
 (select count(*) from tickets as t, products as p2, ticketstatuses as ts, tickettypes as tt
  where t.productid = p2.productid
  and t.organizationid = @OrganizationID
  and t.ticketstatusid = ts.ticketstatusid 
  and ts.isclosed = 0  
  and t.tickettypeid = tt.tickettypeid
  and tt.name = 'features'
  and p.productid = p2.productid) as NumFeatures
From Products as p
where p.organizationid = @OrganizationID
order by p.name desc">
        <SelectParameters>
            <asp:Parameter Name="OrganizationID" />
        </SelectParameters>
    </asp:SqlDataSource>
    </form>
</body>
</html>
