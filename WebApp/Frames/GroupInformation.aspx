<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
  CodeFile="GroupInformation.aspx.cs" Inherits="Frames_GroupInformation" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadWindowManager ID="RadWindowManager1" runat="server">
  </telerik:RadWindowManager>

  <telerik:RadSplitter ID="RadSplitter1" runat="server" BorderSize="0" Height="100%"
    Width="100%" VisibleDuringInit="false">
    <telerik:RadPane ID="RadPane1" runat="server" Scrolling="Y" Height="100%" Width="100%"
      BackColor="White">
  <div style="margin: 0 auto; padding: 0 40px 20px 20px;">

          <div class="groupDiv groupLightBlue" style="padding-top: 10px;">
            <div class="groupHeaderDiv">
              <span class="groupHeaderSpan"></span>
              <span class="groupCaptionSpan">Users</span>
            <span class="groupButtonSpanWrapper">
                      <span class="groupButtonsSpan">
                        <asp:LinkButton ID="btnAddUser" runat="server" CssClass="groupButtonLink">
                        <span class="groupButtonSpan">
                          <i class="fa fa-plus groupButtonImage" /></i>
                          <img alt="" src="../images/icons/add.png" class="groupButtonImage" />
                          <span class="groupButtonTextSpan">Add User</span> </span></asp:LinkButton>
                      </span>
                    </span>
            </div>
            <div class="groupBodyWrapperDiv">
              <div class="groupBodyDiv">
                            <div id="pnlUsers" runat="server" class="adminDiv" style="padding: 10px 0;">
              <asp:Repeater ID="rptUsers" runat="server" OnItemDataBound="rptUsers_ItemDataBound">
                <ItemTemplate>
                  <div style="margin: 0px 5px 5px 15px;">
                    <div class="repeaterItem" style="display:inline-block" runat="server" id="trash">
                      <img class="editImg" src="../images/icons/Trash.png" alt="Trash" onclick="radconfirm('Are you sure you would like to remove this user?', function(arg){if(arg){RemoveUser(<%# DataBinder.Eval(Container.DataItem, "UserID")%>, <%# DataBinder.Eval(Container.DataItem, "GroupID")%>);}}, 250, 125, null, 'Remove User'); return false;" />

                    </div>
                      <span style="display:inline-block">
                        <%# DataBinder.Eval(Container.DataItem, "Name")%>
                      </span>
                  </div>
                </ItemTemplate>
              </asp:Repeater>
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
        RefreshPage();
      }

      function ShowDialog(wnd) {
        wnd.add_close(DialogClosed);
        wnd.show();
        parent.Ts.System.logAction('Group Info - User(s) Added');
      }

      function RefreshPage() {
        __doPostBack();
      }

      function RemoveUser(userID, groupID) {
        parent.parent.privateServices.DeleteGroupUser(groupID, userID, RefreshPage);
        parent.Ts.System.logAction('Group Info - User Removed');

      }


    </script>

  </telerik:RadScriptBlock>
</asp:Content>
