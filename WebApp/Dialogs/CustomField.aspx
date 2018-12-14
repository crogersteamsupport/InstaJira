<%@ Page Title="Custom Field" Language="C#" MasterPageFile="~/Dialogs/Dialog.master"
  AutoEventWireup="true" CodeFile="CustomField.aspx.cs" Inherits="Dialogs_CustomField" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <style type="text/css">
    .inputDiv
    {
      padding-bottom: 10px;
      padding-top: 2px;
    }
  </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">
      <div id="divMain" runat="server">
        <div>
          Name:</div>
        <div class="inputDiv">
          <telerik:RadTextBox ID="textName" runat="server" MaxLength="50" Width="340px"></telerik:RadTextBox></div>
        <div>
          API Field Name:</div>
        <div class="inputDiv">
          <telerik:RadTextBox ID="textApiFieldName" runat="server" MaxLength="50" Width="340px"></telerik:RadTextBox></div>
        <div id="parentFields" runat="server">
          <div id="parentPickList" runat="server">
            <div id="labelParentField">
              Parent Field:</div>
            <div class="inputDiv">
              <telerik:RadComboBox ID="comboParentField" runat="server" AutoPostBack="true" Width="340px" OnSelectedIndexChanged="comboParentField_SelectedIndexChanged"></telerik:RadComboBox>
            </div>
            <div id="parentValue" runat="server" visible="false">
              <div id="labelParentValue">
                Parent Value:</div>
              <div class="inputDiv">
                <telerik:RadComboBox ID="comboParentValue" runat="server" AutoPostBack="false" Width="340px"></telerik:RadComboBox>
              </div>
            </div>
          </div>
          <div id="parentProduct" runat="server">
            <div id="labelParentProduct">
              Parent Product:</div>
            <div class="inputDiv">
              <telerik:RadComboBox ID="comboParentProduct" runat="server" AutoPostBack="false" Width="340px"></telerik:RadComboBox>
            </div>
          </div>
        </div>
        <div>
          Field Type:</div>
        <div class="inputDiv">
          <telerik:RadComboBox ID="comboFieldType" runat="server" AutoPostBack="true" Width="340px">
          </telerik:RadComboBox>
        </div>
        <div id="pnlPickList" runat="server">
          <div>
            Pick List Values:</div>
          <div class="inputDiv">
            <asp:TextBox ID="textList" runat="server" Height="50px" Width="330px" Wrap="True"
              TextMode="MultiLine" MaxLength="8000" Style="overflow: auto;" Rows="400" Columns="30"></asp:TextBox>
          </div>
          <div><asp:CheckBox ID="cbFirstSelect" runat="server" Text="The first value is not a valid selection for a required field." /></div>
        </div>
          <div><asp:CheckBox ID="cbIsRequired" runat="server" Text="A value is required" /></div>
          <div><asp:CheckBox ID="cbIsRequiredToClose" runat="server" Text="A value is required prior to closing ticket" /></div>
        <br />
        <div>
          Description:</div>
        <div class="inputDiv">
          <telerik:RadTextBox ID="textDescription" runat="server" MaxLength="250" Width="340px">
          </telerik:RadTextBox></div>
          <div>
            <asp:CheckBox ID="cbIsVisibleOnPortal" runat="server" Text="Visible on Portal" />
          
          </div>
        <div ID="maskDiv" runat="server" style="margin-top: 15px;">
          <div>
            Mask: -leave blank for no mask (What's an edit mask? <a href="http://www.teamsupport.com/documentation/1/en/topic/input-masks-custom-fields" target="_blank">Click here</a>)
          </div>
          <div class="inputDiv">
            <telerik:RadTextBox ID="textMask" runat="server" MaxLength="250" Width="340px">
            </telerik:RadTextBox>
          </div>
        </div>
      </div>
    </div>
  </div>
</asp:Content>
