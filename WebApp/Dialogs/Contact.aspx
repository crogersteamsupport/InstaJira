<%@ Page Title="" Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true"
  CodeFile="Contact.aspx.cs" Inherits="Dialogs_Contact" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <script src="/frontend/library/jquery-latest.min.js" type="text/javascript"></script>
  <script src="/frontend/library/jquery.maskedinput.min.js" type="text/javascript"></script>
  <script language="javascript" type="text/javascript">
    function OnClientItemsRequesting(sender, eventArgs) {
      var context = eventArgs.get_context();
      context["FilterString"] = eventArgs.get_text();
    }

    function OnClientDropDownClosed(sender, args) {
      sender.clearItems();
    }


  </script>



</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">
    <table id="tblCustomControls" runat="server" width="775px" cellpadding="0" cellspacing="5"
      border="0">
      <tr>
        <td class="labelColTD">
          First Name:
        </td>
        <td class="inputColTD">
          <telerik:RadTextBox ID="textFirstName" runat="server" Width="200px"></telerik:RadTextBox>
        </td>
        <td class="labelColTD">
          Middle Name:
        </td>
        <td class="inputColTD">
          <telerik:RadTextBox ID="textMiddleName" runat="server" Width="200px"></telerik:RadTextBox>
        </td>
      </tr>
      <tr>
        <td class="labelColTD">
          Last Name:
        </td>
        <td class="inputColTD">
          <telerik:RadTextBox ID="textLastName" runat="server" Width="200px"></telerik:RadTextBox>
        </td>
        <td class="labelColTD">
          Title
        </td>
        <td class="inputColTD">
          <telerik:RadTextBox ID="textTitle" runat="server" Width="200px"></telerik:RadTextBox>
        </td>
      </tr>
      <tr>
        <td class="labelColTD">
          Email:
        </td>
        <td class="inputColTD">
          <telerik:RadTextBox ID="textEmail" runat="server" Width="200px"></telerik:RadTextBox>
        </td>
        <td class="labelColTD">
          Active:
        </td>
        <td class="inputColTD">
          <asp:CheckBox ID="cbActive" runat="server" Text="" CssClass="" />
        </td>
      </tr>
      <tr runat="server">
        <td class="labelColTD">
          <span id="spanIsPortalUser" runat="server" >Portal User:</span>
        </td>
        <td class="inputColTD">
          <asp:CheckBox ID="cbIsPortalUser" runat="server" Text="" CssClass=""
            AutoPostBack="True" oncheckedchanged="cbIsPortalUser_CheckedChanged" />
        </td>
        <td class="labelColTD">
          Company:
        </td>
        <td class="inputColTD">
                    <telerik:RadComboBox ID="cmbCustomer" runat="server" Width="200px" AllowCustomText="False"
                      MarkFirstMatch="False" ShowToggleImage="false" EnableLoadOnDemand="True" LoadingMessage="Loading..."
                      OnClientItemsRequesting="OnClientItemsRequesting" OnClientDropDownClosed="OnClientDropDownClosed"
                      EmptyMessage="" ShowDropDownOnTextboxClick="False">
                      <WebServiceSettings Path="Contact.aspx" Method="GetOrganization" />
                      <CollapseAnimation Duration="200" Type="OutQuint" />
                    </telerik:RadComboBox>
        </td>
      </tr>
      <tr>
        <td class="labelColTD">
          Block all emails from sender:
        </td>
        <td class="inputColTD">
          <asp:CheckBox ID="cbBlockEmail" runat="server" Text="" CssClass="" />
        </td>
      </tr>


      <tr id="trAdmin" runat="server">
        <td class="labelColTD">
          System Admin:
        </td>
        <td class="inputColTD">
          <asp:CheckBox ID="cbIsSystemAdmin" runat="server" Text="" CssClass="" />
        </td>
        <td class="labelColTD">
          Financial Admin:
        </td>
        <td class="inputColTD">
          <asp:CheckBox ID="cbIsFinanceAdmin" runat="server" Text="" CssClass="" />
        </td>
      </tr>
    </table>
    <br />
    <asp:Button ID="btnReset" runat="server" Text="Send Contact a New Portal Password" OnClick="btnReset_Click" Visible="false"/> &nbsp
    <asp:Button ID="btnWelcome" runat="server" Text="Send User a Weclcome to TS Message"  Visible="false" onclick="btnWelcome_Click"/>
    <div style="padding: 0px 0 0 8px;">
      <asp:CheckBox ID="cbEmail" runat="server" Text="Email contact a password to the portal."
        Checked="False" Visible="False" />
    </div>
  </div>
  </div>
</asp:Content>
