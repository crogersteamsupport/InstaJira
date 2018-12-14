<%@ Page Title="Contact Information" Language="C#" MasterPageFile="~/Frames/Frame.master"
  AutoEventWireup="true" CodeFile="ContactInformation.aspx.cs" Inherits="Frames_ContactInformation" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <link href="../vcr/1_9_0/Css/jquery-ui-latest.custom.css" rel="stylesheet" type="text/css" />
  <link href="../vcr/1_9_0/Css/jquery-ui-enhanced.css" rel="stylesheet" type="text/css" />
  <link href="../vcr/1_9_0/Css/ts.ui.css" rel="stylesheet" type="text/css" />
  <link href="../vcr/1_9_0/Css/jquery.cluetip.css" rel="stylesheet" type="text/css" />
  <style>
    .tickets { padding: 0 1em 1em 1em;}
    .ticket { padding-top: 1em; lineheight: 16px;}
    .ticket .ts-icon { float:left; margin-right:0.5em;}
    .no-tickets { padding-top: 1em;}

  </style>
  <script src="/frontend/library/jquery-latest.min.js" type="text/javascript"></script>
  <script src="/frontend/library/jquery-ui-latest.custom.min.js" type="text/javascript"></script>

  <script src="/frontend/library/jquery.hoverIntent.min.js" type="text/javascript"></script>
  <script src="/frontend/library/jquery.cluetip.js" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadWindowManager ID="RadWindowManager1" runat="server"></telerik:RadWindowManager>
  <telerik:RadSplitter ID="RadSplitter1" runat="server" BorderSize="0" Height="100%"
    Width="100%" VisibleDuringInit="false">
    <telerik:RadPane ID="RadPane1" runat="server" Scrolling="Y" Height="100%" Width="100%"
      BackColor="White">
      <div id="pnlUserProperties" runat="server">
        <div style="margin: 0 auto; padding: 0 40px 20px 20px;" class="ui-widget">
          <div class="groupDiv groupLightBlue" style="padding-top: 10px;">
            <div class="groupHeaderDiv">
              <span class="groupHeaderSpan"></span>
              <span class="groupCaptionSpan">Properties</span>
                        <span class="groupButtonSpanWrapper">
                      <span class="groupButtonsSpan">
                        <asp:LinkButton ID="btnEditProperties" runat="server" CssClass="groupButtonLink">
                        <span class="groupButtonSpan">
                          <img alt="" src="../images/icons/edit.png" class="groupButtonImage" />
                          <span class="groupButtonTextSpan">Edit Contact</span> </span></asp:LinkButton>
                      </span>
                    </span>
            </div>
            <div class="groupBodyWrapperDiv">
              <div class="groupBodyDiv">
                <div id="pnlProperties" runat="server" class="adminDiv" style="padding: 5px 5px 5px 5px;">
                  <asp:Label ID="lblProperties" runat="server" Text="There are no properties to display."></asp:Label>
                  <asp:Repeater ID="rptProperties" runat="server">
                    <ItemTemplate>
                      <div style="margin: 1em 1em;">
                        <span style="font-weight: bold;">
                          <%# DataBinder.Eval(Container.DataItem, "Name")%></span>
                        <span>
                          <%# DataBinder.Eval(Container.DataItem, "Value")%>
                          <br />
                        </span>
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
              <span class="groupCaptionSpan">Phone Numbers</span>
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
                <div id="pnlPhone" runat="server" class="adminDiv" style="padding: 1em 1em;">
                  <asp:Label ID="lblPhone" runat="server" Text="There are no phone numbers to display."></asp:Label>
                  <asp:Repeater ID="rptPhone" runat="server">
                    <ItemTemplate>
                      <div style="margin: 5px 5px 5px 15px;">
                        <div class="repeaterItem">
                          <a href="#" onclick="ShowDialog(top.GetPhoneDialog(<%# DataBinder.Eval(Container.DataItem, "PhoneID")%>));">
                            <img src="../images/icons/Edit.png" alt="Edit" /></a>
                          <img src="../images/icons/Trash.png" alt="Trash" onclick="radconfirm('Are you sure you would like to remove this phone number?', function(arg){if(arg){top.privateServices.DeletePhone(<%# DataBinder.Eval(Container.DataItem, "PhoneID")%>, RefreshContent);}}, 250, 125, null, 'Delete Phone Number'); return false;" />
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
              <span class="groupCaptionSpan">Addresses</span>
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
                <div id="pnlAddress" runat="server" class="adminDiv" style="padding: 1em 1em;">
                  <asp:Label ID="lblAddresses" runat="server" Text="There are no addresses to display."></asp:Label>
                  <asp:Repeater ID="rptAddresses" runat="server">
                    <ItemTemplate>
                      <div style="margin: 5px 5px 15px 15px;">
                        <div class="repeaterItem" style="font-weight: bold;">
                          <a href="#" onclick="ShowDialog(top.GetAddressDialog(<%# DataBinder.Eval(Container.DataItem, "AddressID")%>));">
                            <img src="../images/icons/Edit.png" alt="Edit" /></a>
                          <img src="../images/icons/Trash.png" alt="Trash" onclick="radconfirm('Are you sure you would like to remove this address?', function(arg){if(arg){top.privateServices.DeleteAddress(<%# DataBinder.Eval(Container.DataItem, "AddressID")%>, RefreshContent);}}, 250, 125, null, 'Delete Address'); return false;" />
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
                        <div>
                          <%# DataBinder.Eval(Container.DataItem, "Country")%></div>
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
              <span class="groupCaptionSpan">Open Tickets</span>
              <span class="groupButtonSpanWrapper">
                <span class="groupButtonsSpan">
                </span>
              </span>
            </div>
            <div class="groupBodyWrapperDiv">
              <div class="groupBodyDiv">
                <div id="openTickets" class="tickets">

                </div>
              </div>
            </div>
          </div>

<div class="groupDiv groupLightBlue" style="padding-top: 10px;">
            <div class="groupHeaderDiv">
              <span class="groupHeaderSpan"></span>
              <span class="groupCaptionSpan">Closed Tickets</span>
              <span class="groupButtonSpanWrapper">
                <span class="groupButtonsSpan">
                </span>
              </span>
            </div>
            <div class="groupBodyWrapperDiv">
              <div class="groupBodyDiv">
                <div id="closedTickets" class="tickets">

                </div>
              </div>
            </div>
          </div>
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

      $(document).ready(function () {
        var userID = top.Ts.Utils.getQueryValue('userid', window);
        var tipTimer = null;
        var clueTipOptions = top.Ts.Utils.getClueTipOptions(tipTimer);

        $('body').delegate('.ts-icon-info', 'mouseout', function (e) {
          if (tipTimer != null) clearTimeout(tipTimer);
          tipTimer = setTimeout("$(document).trigger('hideCluetip');", 1000);
        });

        $('body').delegate('.cluetip', 'mouseover', function (e) {
          if (tipTimer != null) clearTimeout(tipTimer);
        });

        top.Ts.Services.Tickets.GetContactTickets(userID, function (tickets) {
          for (var i = 0; i < tickets.length; i++) {
            var div = $('<div>')
          .data('o', tickets[i])
          .addClass('ticket');

            $('<span>')
          .addClass('ts-icon ts-icon-info')
          .attr('rel', '../Tips/Ticket.aspx?TicketID=' + tickets[i].TicketID)
          .cluetip(clueTipOptions)
          .appendTo(div);

            var caption = $('<span>')
          .addClass('ticket-name')
          .appendTo(div);

            $('<a>')
          .addClass('ts-link ui-state-defaultx')
          .attr('href', '#')
          .text(tickets[i].TicketNumber + ': ' + tickets[i].Name)
          .appendTo(caption)
          .click(function (e) {

            top.Ts.MainPage.openTicket($(this).closest('.ticket').data('o').TicketNumber, true);
          });


            div.appendTo(tickets[i].IsClosed == false ? '#openTickets' : '#closedTickets');
          }

          if ($('#closedTickets .ticket').length < 1) {
            $('<div>')
            .addClass('no-tickets')
            .text('There are no tickets to display')
            .appendTo('#closedTickets');
          }

          if ($('#openTickets .ticket').length < 1) {
            $('<div>')
            .addClass('no-tickets')
            .text('There are no tickets to display')
            .appendTo('#openTickets');
          }
        });






      });


    </script>

  </telerik:RadScriptBlock>
</asp:Content>
