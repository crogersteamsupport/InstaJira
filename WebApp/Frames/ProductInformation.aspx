<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="ProductInformation.aspx.cs" Inherits="Frames_ProductInformation" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Charting" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<telerik:RadSplitter ID="splMain" runat="server" Width="100%" Height="100%" BorderSize="0" BackColor="#ffffff" VisibleDuringInit="false">
  <telerik:RadPane ID="paneMain" runat="server" Height="100%" Width="100%" Scrolling="Y">
    <div style="width:95%; margin:0 auto; padding-bottom: 20px;">
      <div id="pnlContent" runat="server">
        <table border="0" cellpadding="0" cellspacing="0" width="100%">
          <tr>
            <td style="width: 20px; height: 100%;" valign="top">
              <div style="margin-top: 12px; width: 19px;">
                <!-- charts -->
              </div>
            </td>
            <td valign="top">
              <div class="groupDiv groupLightBlue" style="padding-top: 10px;">
                <div class="groupHeaderDiv">
                  <span class="groupHeaderSpan"></span>
                  <span class="groupCaptionSpan">Product Properties</span>
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
            </td>
          </tr>
        </table>
      </div>
    </div>
  </telerik:RadPane>
</telerik:RadSplitter>
</asp:Content>

