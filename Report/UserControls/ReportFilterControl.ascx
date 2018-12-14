<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReportFilterControl.ascx.cs"
  Inherits="UserControls_ReportFilterControl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<div style="line-height: 25px;">
  <asp:Literal ID="litCaption" runat="server"></asp:Literal>
</div>
<div style="padding: 2px 0 7px 10px; line-height: 20px;">
  <asp:Literal ID="litDisplayText" runat="server"></asp:Literal>
</div>
<div>
  <telerik:RadComboBox ID="cmbFields" runat="server" Width="275px" AutoPostBack="true"
    OnSelectedIndexChanged="cmbFields_SelectedIndexChanged"></telerik:RadComboBox>
  &nbsp
  <telerik:RadComboBox ID="cmbOperator" runat="server" Width="150px" AutoPostBack="true"
    OnSelectedIndexChanged="cmbOperator_SelectedIndexChanged"></telerik:RadComboBox>
</div>
<div style="padding-top: 5px;">
  Value:
  <telerik:RadTextBox ID="textValue" runat="server" Visible="false"></telerik:RadTextBox>
  <telerik:RadComboBox ID="cmbBool" runat="server" Width="100px" Visible="False">
    <Items>
      <telerik:RadComboBoxItem runat="server" Text="True" Value="True" />
      <telerik:RadComboBoxItem runat="server" Text="False" Value="False" />
    </Items>
  </telerik:RadComboBox>
  &nbsp
  <telerik:RadDatePicker ID="dateValue1" runat="server" Visible="false">
    <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x">
    </Calendar>
    <DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
  </telerik:RadDatePicker>
  <telerik:RadNumericTextBox ID="numValue1" runat="server" Visible="false" DataType="System.Int32">
    <NumberFormat DecimalDigits="0" />
  </telerik:RadNumericTextBox>
  <asp:Label ID="lblAnd" runat="server" Text="and" Visible="false"></asp:Label>
  <telerik:RadDatePicker ID="dateValue2" runat="server" Visible="false">
    <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x">
    </Calendar>
    <DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
  </telerik:RadDatePicker>
  <telerik:RadNumericTextBox ID="numValue2" runat="server" Visible="false" DataType="System.Int32">
    <NumberFormat DecimalDigits="0" />
  </telerik:RadNumericTextBox>
  &nbsp
  <asp:Button ID="btnAdd" runat="server" Text="Add Condition" OnClientClick="SendAjaxRequest('AddCondition'); return false;" />
</div>
<asp:HiddenField ID="fieldSubcategoryID" runat="server" Visible="false" />
<asp:HiddenField ID="fieldFilterObject" runat="server" Visible="false" />
<asp:HiddenField ID="fieldChanged" runat="server" Value="false"/>
<asp:HiddenField ID="fieldReportID" runat="server" Value="-1"/>
<telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

  <script type="text/javascript" language="javascript">
    function SendAjaxRequest(args) {
      $get("<%=fieldChanged.ClientID %>").value = 'true';
      var ajaxManager = $find("<%=RadAjaxManager.GetCurrent(Page).ClientID  %>");
      ajaxManager.ajaxRequest(args);
      return false;
    }

  </script>

</telerik:RadScriptBlock>
