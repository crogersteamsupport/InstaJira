<%@ Page Title="Version" Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true" CodeFile="Version.aspx.cs" Inherits="Dialogs_Version" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <script src="/frontend/library/jquery-latest.min.js" type="text/javascript"></script>
  <script src="/frontend/library/jquery.maskedinput.min.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">
<div>Version Number:</div>
<div>
  <telerik:RadTextBox ID="textVersionNumber" runat="server" Width="760px">
  </telerik:RadTextBox>
</div>


<table id="tblCustomControls" runat="server" width="775px" cellpadding="0" cellspacing="5" border="0">
  <tr>
    <td class="labelColTD">
      Product:
    </td>
    <td class="inputColTD">
      <telerik:RadComboBox ID="cmbProduct" runat="server" Width="200PX"></telerik:RadComboBox>
    </td>
    <td class="labelColTD">
      Status:
    </td>
    <td class="inputColTD">
      <telerik:RadComboBox ID="cmbStatus" runat="server" Width="200PX"></telerik:RadComboBox>
    </td>
  </tr>
  <tr>
    <td class="labelColTD">
      Released:
    </td>
    <td class="inputColTD">
      <asp:CheckBox ID="cbReleased" runat="server"  Text="" AutoPostBack="true"/>
    </td>
    <td class="labelColTD">
      <asp:Label ID="lblRelease" runat="server" Text="Released Date:"></asp:Label>
    </td>
    <td class="inputColTD">
      <telerik:RadDatePicker ID="dpRelease" runat="server"></telerik:RadDatePicker>
    </td>
  </tr>
</table>



<div style="padding-bottom: 3px;">Version Description:</div>
<div>
      <telerik:RadEditor ID="editorDescription" runat="server" EditModes="Design" ToolsFile="~/Editor/StandardTools.xml" OnClientLoad="EditorLoad" StripFormattingOnPaste="MSWord"
        BorderWidth="0px" Height="260px" Width="765px">
        <Content></Content>
      </telerik:RadEditor>
</div>
</div>
</div>
    <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
    <script type="text/javascript" language="javascript">


      function EditorLoad(sender, args) {
        sender.get_contentArea().style.overflow = "auto";
      }
    
      Telerik.Web.UI.Editor.CommandList["InsertTicketLink"] = function(commandName, editor, args) {
        var elem = editor.getSelectedElement(); //returns the selected element.

        if (elem.tagName == "A") {
          editor.selectElement(elem);
          argument = elem;
        }
        else {
          var content = editor.getSelectionHtml();
          var link = editor.get_document().createElement("A");
          link.innerHTML = content;
          argument = link;
        }

        var myCallbackFunction = function(sender, args) {
          editor.pasteHtml(String.format("<a href={0} target='{1}' class='{2}'>{3}</a> ", args.href, args.target, args.className, args.name))
        }

        editor.showExternalDialog(
            '../Editor/SelectTicket.aspx',
            argument,
            290,
            200,
            myCallbackFunction,
            null,
            'Insert Ticket Link',
            true,
            Telerik.Web.UI.WindowBehaviors.Close + Telerik.Web.UI.WindowBehaviors.Move,
            false,
            false);
      };
      
    </script>

  </telerik:RadScriptBlock>
</asp:Content>

