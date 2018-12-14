<%@ Page Title="Organization Product" Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true" CodeFile="OrganizationProduct.aspx.cs" Inherits="Dialogs_OrganizationProduct" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
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
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">

<table id="tblCustomControls" runat="server" width="775px" cellpadding="0" cellspacing="5" border="0">
  <tr>
    <td class="labelColTD">
      Customer:
    </td>
    <td class="inputColTD">
                    <telerik:RadComboBox ID="cmbCustomers" runat="server" Width="200px" AllowCustomText="False"
                      MarkFirstMatch="False" ShowToggleImage="false" EnableLoadOnDemand="True" LoadingMessage="Loading..."
                      OnClientItemsRequesting="OnClientItemsRequesting" OnClientDropDownClosed="OnClientDropDownClosed"
                      EmptyMessage="" ShowDropDownOnTextboxClick="False">
                      <WebServiceSettings Path="OrganizationProduct.aspx" Method="GetOrganization" />
                      <CollapseAnimation Duration="200" Type="OutQuint" />
                    </telerik:RadComboBox>
    </td>

    <td class="labelColTD">
      Product:
    </td>
    <td class="inputColTD">
      <telerik:RadComboBox ID="cmbProducts" runat="server" Width="200px" AutoPostBack="true" onselectedindexchanged="cmbProducts_SelectedIndexChanged"></telerik:RadComboBox>
    </td>
  </tr>

  <tr id="row2" runat="server">
   
    <td class="labelColTD">
      Version:
    </td>
    <td class="inputColTD">
      <telerik:RadComboBox ID="cmbVersions" runat="server" Width="200px"></telerik:RadComboBox>
    </td>
    <td class="labelColTD">
      Warranty Expiration:
    </td>
    <td class="inputColTD">
      <telerik:RadDatePicker ID="dtExpiration" runat="server" Width="200px"></telerik:RadDatePicker>
    </td>
  </tr>

</table>
</div>
</div>

</asp:Content>

