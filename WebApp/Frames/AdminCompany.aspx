<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
  CodeFile="AdminCompany.aspx.cs" Inherits="Frames_AdminCompany" ValidateRequest="false" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    <AjaxSettings>
      <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="pnlProperties" />
          <telerik:AjaxUpdatedControl ControlID="pnlPhone" />
          <telerik:AjaxUpdatedControl ControlID="pnlAddress" />
        </UpdatedControls>
      </telerik:AjaxSetting>
    </AjaxSettings>
  </telerik:RadAjaxManager>
  <div style="width: 100%; height: 100%; background-color: #ffffff; margin: 0 auto; overflow:auto; 
    text-align: center;">
    <div style="padding-bottom:20px;">
    <div style="margin: 0 auto; text-align: center;">
      <div class="groupDiv groupLightBlue" style="padding-top: 10px;">
        <div class="groupHeaderDiv">
          <span class="groupHeaderSpan"></span>
          <span class="groupCaptionSpan">Properties and Settings</span>
          <span class="groupButtonSpanWrapper">
            <span class="groupButtonsSpan">
              <asp:LinkButton ID="btnEditProperties" runat="server" CssClass="groupButtonLink">
            <span class="groupButtonSpan">
            <img alt="" src="../images/icons/edit.png" class="groupButtonImage" />
                <span class="groupButtonTextSpan">Edit Properties</span> </span></asp:LinkButton>
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
            </div>
          </div>
        </div>
      </div>
      
      <div id="divChat" runat="server">
      <div class="groupDiv groupLightBlue" style="padding-top: 10px;">
        <div class="groupHeaderDiv">
          <span class="groupHeaderSpan"></span>
          <span class="groupCaptionSpan">Chat</span>
          <span class="groupButtonSpanWrapper">
            <span class="groupButtonsSpan">
              <asp:LinkButton ID="btnChat" runat="server" CssClass="groupButtonLink">
            <span class="groupButtonSpan">
            <img alt="" src="../images/icons/edit.png" class="groupButtonImage" />
                <span class="groupButtonTextSpan">Edit Chat Properties</span> </span></asp:LinkButton>
            </span>
          </span>
        </div>
        <div class="groupBodyWrapperDiv">
          <div class="groupBodyDiv">
            <div class="adminDiv" style="padding: 5px 5px 5px 5px; margin: 5px 15px 5px 15px;">
            
               <div id="divChatProperties" runat="server">
              </div>
               <br />
              <div>
                <strong>Chat Link Code:</strong>
                <telerik:RadTextBox ID="textChatCode" runat="server" TextMode="MultiLine" Height="100px" Width="100%" ReadOnly="true">
                </telerik:RadTextBox>
              </div>
              <br />
              <div>
                <asp:Button ID="btnChatOffline" runat="server" Text="Mark all chat users as Unavailable" OnClientClick="PageMethods.MarkChatOffline(); alert('All chat users have been marked as Unavailable'); return false;" />
              </div>
              
            </div>
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
                      <img src="../images/icons/Edit.png" alt="Edit" onclick="ShowDialog(top.GetPhoneDialog(<%# DataBinder.Eval(Container.DataItem, "PhoneID")%>));RefreshContent();" />
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
                    <div>
                      <%# DataBinder.Eval(Container.DataItem, "Country")%></div>
                  </div>
                </ItemTemplate>
              </asp:Repeater>
            </div>
          </div>
        </div>
      </div>
    </div>
    </div>
  </div>
  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

    <script type="text/javascript" language="javascript">
      function DialogClosed(sender, args) {
        sender.remove_close(DialogClosed);
        window.location = window.location;
        //RefreshContent();
      }

      function ShowDialog(wnd) {
        wnd.add_close(DialogClosed);
        wnd.show();
      }

      function RefreshContent() {
        SendAjaxRequest();
      }

      function SendAjaxRequest(args) {
        var ajaxManager = $find("<%=RadAjaxManager.GetCurrent(Page).ClientID  %>");
        ajaxManager.ajaxRequest(args);
        return false;
      }

    
    </script>

  </telerik:RadScriptBlock>
</asp:Content>
