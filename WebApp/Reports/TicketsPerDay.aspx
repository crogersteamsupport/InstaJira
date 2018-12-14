<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TicketsPerDay.aspx.vb" Inherits="_Default" %>


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
<div  text-align: center; margin:0 auto"> 
    <form id="form1" runat="server">
    <div class=strong>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        Tickets by Day</div>
    <div>
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
    <div>
    </div>
</div>

    <div style="width: 100%; height:80%; overflow:auto; text-align: center; margin:0 auto;" >
        <br />
        
    
        <telerik:RadChart ID="RadChart1" runat="server" DataSourceID="TicketsCreatedPerDay" 
            DefaultType="Line" Height="370px" Width="485px" Skin="Gradient">
            <Series>
                <cc1:ChartSeries DataYColumn="TicketsCreated" 
                    Name="Series 1" Type="Line">
                    <Appearance>
                        <FillStyle MainColor="199, 243, 178" SecondColor="17, 147, 7">
                        </FillStyle>
                        <TextAppearance TextProperties-Color="Black">
                        </TextAppearance>
                        <Border Color="Black" />
                    </Appearance>
                </cc1:ChartSeries>
            </Series>
            <PlotArea>
                <XAxis AutoScale="False" DataLabelsColumn="Date" LabelStep="5" MaxValue="11" 
                    MinValue="1" Step="5">
                    <Appearance MajorTick-Color="Black">
                        <MajorGridLines Color="DimGray" />
                        <TextAppearance TextProperties-Color="Black">
                        </TextAppearance>
                    </Appearance>
                    <AxisLabel>
                        <TextBlock>
                            <Appearance TextProperties-Font="Verdana, 9.75pt, style=Bold">
                            </Appearance>
                        </TextBlock>
                    </AxisLabel>
                    <Items>
                        <cc1:ChartAxisItem Value="1">
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="6" Visible="False">
                            <Appearance Visible="False">
                            </Appearance>
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="11" Visible="False">
                            <Appearance Visible="False">
                            </Appearance>
                        </cc1:ChartAxisItem>
                    </Items>
                </XAxis>
                <YAxis>
                    <Appearance>
                        <MajorGridLines Color="Black" />
                    </Appearance>
                    <AxisLabel>
                        <TextBlock>
                            <Appearance TextProperties-Font="Verdana, 9.75pt, style=Bold">
                            </Appearance>
                        </TextBlock>
                    </AxisLabel>
                </YAxis>
                <YAxis2>
                    <AxisLabel>
                        <TextBlock>
                            <Appearance TextProperties-Font="Verdana, 9.75pt, style=Bold">
                            </Appearance>
                        </TextBlock>
                    </AxisLabel>
                </YAxis2>
                <Appearance Corners="Round, Round, Round, Round, 6" 
                    Dimensions-Margins="18%, 22%, 12%, 10%">
                    <FillStyle MainColor="65, 201, 254" SecondColor="0, 107, 186">
                    </FillStyle>
                    <Border Color="94, 94, 93" />
                </Appearance>
            </PlotArea>
            <Appearance>
                <FillStyle MainColor="244, 244, 234" FillType="Gradient" 
                    SecondColor="167, 172, 137">
                </FillStyle>
                <Border Color="117, 117, 117" />
            </Appearance>
            <ChartTitle>
                <Appearance Corners="Round, Round, Round, Round, 3" 
                    Dimensions-Margins="4%, 10px, 14px, 0%" Position-AlignedPosition="Top">
                    <FillStyle MainColor="177, 183, 144">
                    </FillStyle>
                    <Border Color="64, 64, 64" />
                </Appearance>
                <TextBlock Text="Tickets Created Per Day">
                    <Appearance TextProperties-Color="White" 
                        TextProperties-Font="Verdana, 14.25pt, style=Bold">
                    </Appearance>
                </TextBlock>
            </ChartTitle>
            <Legend Visible="False">
                <Appearance Visible="False" Dimensions-Margins="18%, 1%, 1px, 1px" 
                    Corners="Round, Round, Round, Round, 3">
                    <FillStyle MainColor="177, 183, 144">
                    </FillStyle>
                    <Border Color="64, 64, 64" />
                </Appearance>
            </Legend>
        </telerik:RadChart>
    
        <br />
        <br />
        
    
        <telerik:RadChart ID="RadChart2" runat="server" DataSourceID="TicketsClosedByDay" 
            DefaultType="Line" Height="370px" Width="485px" Skin="Gradient">
            <Series>
                <cc1:ChartSeries 
                    Name="Series 1" Type="Line">
                    <Appearance>
                        <FillStyle MainColor="199, 243, 178" SecondColor="17, 147, 7">
                        </FillStyle>
                        <TextAppearance TextProperties-Color="Black">
                        </TextAppearance>
                        <Border Color="Black" />
                    </Appearance>
                </cc1:ChartSeries>
            </Series>
            <PlotArea>
                <XAxis DataLabelsColumn="Date" LabelStep="5">
                    <Appearance MajorTick-Color="Black">
                        <MajorGridLines Color="DimGray" />
                        <TextAppearance TextProperties-Color="Black">
                        </TextAppearance>
                    </Appearance>
                    <AxisLabel>
                        <TextBlock>
                            <Appearance TextProperties-Font="Verdana, 9.75pt, style=Bold">
                            </Appearance>
                        </TextBlock>
                    </AxisLabel>
                    <Items>
                        <cc1:ChartAxisItem>
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="1" Visible="False">
                            <Appearance Visible="False">
                            </Appearance>
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="2" Visible="False">
                            <Appearance Visible="False">
                            </Appearance>
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="3" Visible="False">
                            <Appearance Visible="False">
                            </Appearance>
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="4" Visible="False">
                            <Appearance Visible="False">
                            </Appearance>
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="5">
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="6" Visible="False">
                            <Appearance Visible="False">
                            </Appearance>
                        </cc1:ChartAxisItem>
                        <cc1:ChartAxisItem Value="7" Visible="False">
                            <Appearance Visible="False">
                            </Appearance>
                        </cc1:ChartAxisItem>
                    </Items>
                </XAxis>
                <YAxis>
                    <Appearance>
                        <MajorGridLines Color="Black" />
                    </Appearance>
                    <AxisLabel>
                        <TextBlock>
                            <Appearance TextProperties-Font="Verdana, 9.75pt, style=Bold">
                            </Appearance>
                        </TextBlock>
                    </AxisLabel>
                </YAxis>
                <YAxis2>
                    <AxisLabel>
                        <TextBlock>
                            <Appearance TextProperties-Font="Verdana, 9.75pt, style=Bold">
                            </Appearance>
                        </TextBlock>
                    </AxisLabel>
                </YAxis2>
                <Appearance Corners="Round, Round, Round, Round, 6" 
                    Dimensions-Margins="18%, 22%, 12%, 10%">
                    <FillStyle MainColor="65, 201, 254" SecondColor="0, 107, 186">
                    </FillStyle>
                    <Border Color="94, 94, 93" />
                </Appearance>
            </PlotArea>
            <Appearance>
                <FillStyle MainColor="244, 244, 234" FillType="Gradient" 
                    SecondColor="167, 172, 137">
                </FillStyle>
                <Border Color="117, 117, 117" />
            </Appearance>
            <ChartTitle>
                <Appearance Corners="Round, Round, Round, Round, 3" 
                    Dimensions-Margins="4%, 10px, 14px, 0%" Position-AlignedPosition="Top">
                    <FillStyle MainColor="177, 183, 144">
                    </FillStyle>
                    <Border Color="64, 64, 64" />
                </Appearance>
                <TextBlock Text="Tickets Closed Per Day">
                    <Appearance TextProperties-Color="White" 
                        TextProperties-Font="Verdana, 14.25pt, style=Bold">
                    </Appearance>
                </TextBlock>
            </ChartTitle>
            <Legend Visible="False">
                <Appearance Visible="False" Dimensions-Margins="18%, 1%, 1px, 1px" 
                    Corners="Round, Round, Round, Round, 3">
                    <FillStyle MainColor="177, 183, 144">
                    </FillStyle>
                    <Border Color="64, 64, 64" />
                </Appearance>
            </Legend>
        </telerik:RadChart>
    
    </div>
    
    <asp:SqlDataSource ID="TicketsCreatedPerDay" runat="server" 
        ConnectionString="<%$ ConnectionStrings:TSConnectionString %>" 
        SelectCommand="Select CONVERT(VARCHAR,dt.dtime,1) AS 'Date',Count(t.datecreated) as 'TicketsCreated'
FROM dbo.udfDateTimes(@StartDate,@EndDate, 1, 'day') AS dt 
LEFT JOIN Tickets t ON dt.dtime = CAST(FLOOR(CAST(t.DateCreated AS FLOAT)) AS DATETIME) and  t.organizationid = @organizationid
 group by dt.dtime">
        <SelectParameters>
            <asp:Parameter Name="organizationid" />
            <asp:Parameter Name="StartDate" />
            <asp:Parameter Name="EndDate" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="TicketsClosedByDay" runat="server" 
        ConnectionString="<%$ ConnectionStrings:TSConnectionString %>" 
        SelectCommand="Select CONVERT(VARCHAR,dt.dtime,1) AS 'Date',Count(t.dateclosed) as 'TicketsClosed'
FROM dbo.udfDateTimes(@StartDate,@EndDate, 1, 'day') AS dt 
LEFT JOIN Tickets t ON dt.dtime = CAST(FLOOR(CAST(t.DateClosed AS FLOAT)) AS DATETIME) and  t.organizationid = @organizationid
 group by dt.dtime">
        <SelectParameters>
            <asp:Parameter Name="organizationid" />
            <asp:Parameter Name="StartDate" />
            <asp:Parameter Name="EndDate" />
        </SelectParameters>
    </asp:SqlDataSource>
    </form>
</body>
</html>
