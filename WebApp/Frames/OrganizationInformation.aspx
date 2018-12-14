<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
  CodeFile="OrganizationInformation.aspx.cs" Inherits="Frames_OrganizationInformation" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Charting" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <style type="text/css">
  .customfield-cat {margin: 10px 5px 5px 10px; padding: 0; line-height:16px; padding-bottom: 1em; cursor:pointer;}
  .customfield-cat .caption { font-weight: bold; font-size: 1.2em; float:left;}
  .customfield-cat .ui-icon { float:left; margin-right: .25em;}
  .ui-widget-content.ts-separator { height: 1px; border-bottom: none; border-left:none; border-right:none; clear:both; margin: 7px 5px 10px 15px;  }
  </style>

  <script language="javascript" type="text/javascript">
    $(document).ready(function () {
      $('.customfield-cat').click(function (e) {
        $(this).next().toggle().next().toggle();
        var expanded = $(this).next().is(':visible');
        if (expanded) {
          $(this).find('.ui-icon').addClass('ui-icon-triangle-1-s').removeClass('ui-icon-triangle-1-e');
        }
        else {
          $(this).find('.ui-icon').addClass('ui-icon-triangle-1-e').removeClass('ui-icon-triangle-1-s');
        }
        top.privateServices.SetUserSetting('Custom Cat Expanded (Customer) - ' + $(this).find('.caption').text(), expanded);
      });



    });
  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadSplitter ID="splMain" runat="server" Width="100%" Height="100%" BorderSize="0"
    BackColor="White" VisibleDuringInit="false">
    <telerik:RadPane ID="paneMain" runat="server" Height="100%" Width="100%" Scrolling="Y">
      <div style="width: 95%; margin: 0 auto; padding-bottom: 20px;">
        <div id="pnlContent" runat="server">
          <table border="0" cellpadding="0" cellspacing="0" width="100%">
            <tr>
              <td style="width: 200px; height: 100%;" valign="top">
                <div style="margin-top: 12px; width: 190px;">
                  <telerik:RadChart ID="chartOpenTickets" runat="server" DefaultType="Pie" Height="120px"
                    Width="190px" AutoLayout="True" OnItemDataBound="chartTickets_ItemDataBound" 
                    Skin="Office2007">
                    <Series>
                      <cc1:ChartSeries Name="Series 1">
                        <Appearance>
                          <FillStyle FillType="Solid" MainColor="69, 115, 167"></FillStyle>
                          <TextAppearance TextProperties-Color="Black"></TextAppearance>
                          <Border Color="69, 115, 167" />
                        </Appearance>
                      </cc1:ChartSeries>
                      <cc1:ChartSeries Name="Series 2">
                        <Appearance>
                          <FillStyle FillType="Solid" MainColor="107, 70, 68"></FillStyle>
                          <TextAppearance TextProperties-Color="Black"></TextAppearance>
                          <Border Color="107, 70, 68" />
                        </Appearance>
                      </cc1:ChartSeries>
                    </Series>
                    <PlotArea>
                      <EmptySeriesMessage Visible="True">
                        <TextBlock Visible="False"></TextBlock>
                      </EmptySeriesMessage>
                      <XAxis>
                        <Appearance Color="134, 134, 134" MajorTick-Color="134, 134, 134">
                          <MajorGridLines Color="134, 134, 134" Width="0" />
                          <TextAppearance TextProperties-Color="Red"></TextAppearance>
                        </Appearance>
                        <AxisLabel>
                          <TextBlock>
                            <Appearance TextProperties-Color="Black"></Appearance>
                          </TextBlock>
                        </AxisLabel>
                      </XAxis>
                      <YAxis>
                        <Appearance Color="134, 134, 134" MajorTick-Color="134, 134, 134" MinorTick-Color="134, 134, 134">
                          <MajorGridLines Color="134, 134, 134" />
                          <MinorGridLines Color="134, 134, 134" />
                          <TextAppearance TextProperties-Color="Black"></TextAppearance>
                        </Appearance>
                        <AxisLabel>
                          <TextBlock>
                            <Appearance TextProperties-Color="Black"></Appearance>
                          </TextBlock>
                        </AxisLabel>
                      </YAxis>
                      <Appearance Dimensions-Margins="1px, 1px, 1px, 1px" Dimensions-Paddings="20px, 1px, 1px, 1px">
                        <FillStyle FillType="Solid" MainColor=""></FillStyle>
                        <Border Width="0" />
                      </Appearance>
                    </PlotArea>
                    <Appearance TextQuality="ClearTypeGridFit">
                      <Border Color="134, 134, 134" Width="0" />
                    </Appearance>
                    <ChartTitle>
                      <Appearance>
                        <FillStyle MainColor=""></FillStyle>
                      </Appearance>
                      <TextBlock Text="My Ticket Summary">
                        <Appearance TextProperties-Color="Black" TextProperties-Font="Arial, 14px, style=Bold">
                        </Appearance>
                      </TextBlock>
                    </ChartTitle>
                    <Legend Visible="False">
                      <Appearance Dimensions-Margins="1%, 1%, 1px, 1px" Dimensions-Paddings="0px, 0px, 0px, 0px"
                        Position-AlignedPosition="TopRight" Visible="False">
                        <ItemTextAppearance TextProperties-Color="Black"></ItemTextAppearance>
                        <ItemMarkerAppearance Figure="Square"></ItemMarkerAppearance>
                      </Appearance>
                    </Legend></telerik:RadChart>
                  <br />
                  <br />
                  <br />
                  <telerik:RadChart ID="chartClosedTickets" runat="server" DefaultType="Pie" Height="120px"
                    Width="190px" AutoLayout="True" OnItemDataBound="chartTickets_ItemDataBound" 
                    Skin="Office2007">
                    <Series>
                      <cc1:ChartSeries Name="Series 1">
                        <Appearance>
                          <FillStyle FillType="Solid" MainColor="69, 115, 167"></FillStyle>
                          <TextAppearance TextProperties-Color="Black"></TextAppearance>
                          <Border Color="69, 115, 167" />
                        </Appearance>
                      </cc1:ChartSeries>
                      <cc1:ChartSeries Name="Series 2">
                        <Appearance>
                          <FillStyle FillType="Solid" MainColor="107, 70, 68"></FillStyle>
                          <TextAppearance TextProperties-Color="Black"></TextAppearance>
                          <Border Color="107, 70, 68" />
                        </Appearance>
                      </cc1:ChartSeries>
                    </Series>
                    <PlotArea>
                      <EmptySeriesMessage>
                        <TextBlock Visible="False"></TextBlock>
                      </EmptySeriesMessage>
                      <XAxis>
                        <Appearance Color="134, 134, 134" MajorTick-Color="134, 134, 134">
                          <MajorGridLines Color="134, 134, 134" Width="0" />
                          <TextAppearance TextProperties-Color="Red"></TextAppearance>
                        </Appearance>
                        <AxisLabel>
                          <TextBlock>
                            <Appearance TextProperties-Color="Black"></Appearance>
                          </TextBlock>
                        </AxisLabel>
                      </XAxis>
                      <YAxis>
                        <Appearance Color="134, 134, 134" MajorTick-Color="134, 134, 134" MinorTick-Color="134, 134, 134">
                          <MajorGridLines Color="134, 134, 134" />
                          <MinorGridLines Color="134, 134, 134" />
                          <TextAppearance TextProperties-Color="Black"></TextAppearance>
                        </Appearance>
                        <AxisLabel>
                          <TextBlock>
                            <Appearance TextProperties-Color="Black"></Appearance>
                          </TextBlock>
                        </AxisLabel>
                      </YAxis>
                      <Appearance Dimensions-Margins="1px, 1px, 1px, 1px" Dimensions-Paddings="20px, 1px, 1px, 1px">
                        <FillStyle FillType="Solid" MainColor=""></FillStyle>
                        <Border Width="0" />
                      </Appearance>
                    </PlotArea>
                    <Appearance TextQuality="ClearTypeGridFit">
                      <Border Color="134, 134, 134" Width="0" />
                    </Appearance>
                    <ChartTitle>
                      <Appearance>
                        <FillStyle MainColor=""></FillStyle>
                      </Appearance>
                      <TextBlock Text="My Ticket Summary">
                        <Appearance TextProperties-Color="Black" TextProperties-Font="Arial, 14px, style=Bold">
                        </Appearance>
                      </TextBlock>
                    </ChartTitle>
                    <Legend Visible="False">
                      <Appearance Dimensions-Margins="1%, 1%, 1px, 1px" Dimensions-Paddings="0px, 0px, 0px, 0px"
                        Position-AlignedPosition="TopRight" Visible="False">
                        <ItemTextAppearance TextProperties-Color="Black"></ItemTextAppearance>
                        <ItemMarkerAppearance Figure="Square"></ItemMarkerAppearance>
                      </Appearance>
                    </Legend></telerik:RadChart>
                </div>
              </td>
              <td valign="top">
                <div class="groupDiv groupLightBlue" style="padding-top: 10px;">
                  <div class="groupHeaderDiv">
                    <span class="groupHeaderSpan"></span>
                    <span class="groupCaptionSpan">Company Properties</span>
                    <span class="groupButtonSpanWrapper">
                      <span class="groupButtonsSpan">
                        <asp:LinkButton ID="btnEditProperties" runat="server" CssClass="groupButtonLink">
                        <span class="groupButtonSpan">
                          <img alt="" src="../images/icons/edit.png" class="groupButtonImage" />
                          <span class="groupButtonTextSpan">Edit Organization</span> </span></asp:LinkButton>
                      </span>
                    </span>
                  </div>
                  <div class="groupBodyWrapperDiv">
                    <div class="groupBodyDiv">
                      <div id="pnlProperties" runat="server" class="adminDiv" style="padding: 5px 5px 5px 5px;">
                        <asp:Label ID="lblProperties" runat="server" Text="There are no properties to display."></asp:Label>
                        <asp:Repeater ID="rptProperties" runat="server">
                          <ItemTemplate>
                            <div style="margin: 5px 5px 5px 15px; line-height: 20px;">
                              <span style="font-weight: bold;">
                                <%# DataBinder.Eval(Container.DataItem, "Name")%></span>
                              <span>
                                <%# DataBinder.Eval(Container.DataItem, "Value")%>
                                <br />
                              </span>
                            </div>
                          </ItemTemplate>
                        </asp:Repeater>
                        <asp:Literal ID="litProperties" runat="server"></asp:Literal>
                      </div>
                    </div>
                  </div>
                </div>
                <div class="groupDiv groupLightBlue" style="padding-top: 10px;">
                  <div class="groupHeaderDiv">
                    <span class="groupHeaderSpan"></span>
                    <span class="groupCaptionSpan">Company Phone Numbers</span>
                    <span class="groupButtonSpanWrapper">
                      <span class="groupButtonsSpan">
                        <asp:LinkButton ID="btnNewPhone" runat="server" CssClass="groupButtonLink">
                  <span class="groupButtonSpan">
                    <img alt="" src="../images/icons/add.png" class="groupButtonImage" />
                    <span class="groupButtonTextSpan">Add Phone Number</span> </span></asp:LinkButton>
                      </span>
                    </span>
                  </div>
                  <div class="groupBodyWrapperDiv">
                    <div class="groupBodyDiv"">
                      <div id="pnlPhone" runat="server" class="adminDiv" style="padding: 5px 5px 5px 5px;">
                        <asp:Label ID="lblPhone" runat="server" Text="There are no phone numbers to display."></asp:Label>
                        <asp:Repeater ID="rptPhone" runat="server">
                          <ItemTemplate>
                            <div style="margin: 5px 5px 5px 15px;">
                              <div class="repeaterItem">
                                <img src="../images/icons/Edit.png" alt="Edit" onclick="ShowDialog(top.GetPhoneDialog(<%# DataBinder.Eval(Container.DataItem, "PhoneID")%>));" />
                                <img src="../images/icons/Trash.png" alt="Trash" onclick="if (confirm('Are you sure you would like to remove this phone number?')){top.privateServices.DeletePhone(<%# DataBinder.Eval(Container.DataItem, "PhoneID")%>); RefreshContent();}" />
                                <span style="font-weight: bold;">
                                  <%# DataBinder.Eval(Container.DataItem, "Type")%>: </span>
                                <span>
                                  <%# DataBinder.Eval(Container.DataItem, "Number")%></span>
                                <span>
                                  <%# DataBinder.Eval(Container.DataItem, "Ext")%></span>
                              </div>
                            </div>
                          </ItemTemplate>
                        </asp:Repeater>
                      </div>
                    </div>
                  </div>
                </div>
                <div class="groupDiv groupLightBlue" style="padding-top: 10px;">
                  <div class="groupHeaderDiv">
                    <span class="groupHeaderSpan"></span>
                    <span class="groupCaptionSpan">Company Addresses</span>
                    <span class="groupButtonSpanWrapper">
                      <span class="groupButtonsSpan">
                        <asp:LinkButton ID="btnNewAddress" runat="server" CssClass="groupButtonLink">
                  <span class="groupButtonSpan">
                    <img alt="" src="../images/icons/add.png" class="groupButtonImage" />
                    <span class="groupButtonTextSpan">Add Address</span> </span></asp:LinkButton>
                      </span>
                    </span>
                  </div>
                  <div class="groupBodyWrapperDiv">
                    <div class="groupBodyDiv">
                      <div id="pnlAddress" runat="server" class="adminDiv" style="padding: 5px 5px 5px 5px;">
                        <asp:Label ID="lblAddresses" runat="server" Text="There are no addresses to display."></asp:Label>
                        <asp:Repeater ID="rptAddresses" runat="server">
                          <ItemTemplate>
                            <div style="margin: 5px 5px 15px 15px;">
                              <div class="repeaterItem" style="font-weight: bold;">
                                <img src="../images/icons/Edit.png" alt="Edit" onclick="ShowDialog(top.GetAddressDialog(<%# DataBinder.Eval(Container.DataItem, "AddressID")%>));" />
                                <img src="../images/icons/Trash.png" alt="Trash" onclick="if (confirm('Are you sure you would like to remove this address?')){top.privateServices.DeleteAddress(<%# DataBinder.Eval(Container.DataItem, "AddressID")%>); RefreshContent();}" />
                                <span>
                                  <%# DataBinder.Eval(Container.DataItem, "Description")%></span>
                              </div>
                              <div>
                                <%# DataBinder.Eval(Container.DataItem, "Addr1")%></div>
                              <div>
                                <%# DataBinder.Eval(Container.DataItem, "Addr2")%></div>
                              <div>
                                <%# DataBinder.Eval(Container.DataItem, "Addr3")%></div>
                              <div>
                                <%# DataBinder.Eval(Container.DataItem, "City")%>,
                                <%# DataBinder.Eval(Container.DataItem, "State")%>
                                <%# DataBinder.Eval(Container.DataItem, "Zip")%></div>
                              <div><%# DataBinder.Eval(Container.DataItem, "Country")%></div>
                              <div><%# DataBinder.Eval(Container.DataItem, "MapLink")%></div>
                            </div>
                          </ItemTemplate>
                        </asp:Repeater>
                      </div>
                    </div>
                  </div>
                </div>
              </td>
            </tr>
          </table>
        </div>
      </div>
    </telerik:RadPane>
  </telerik:RadSplitter>
  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

    <script type="text/javascript" language="javascript">
      function DialogClosed(sender, args) {
        sender.remove_close(DialogClosed);
        RefreshContent();
      }

      function ShowDialog(wnd) {
        wnd.add_close(DialogClosed);
        wnd.show();
      }

      function RefreshContent() {
        __doPostBack();
      }
    
    
    </script>

  </telerik:RadScriptBlock>
</asp:Content>
