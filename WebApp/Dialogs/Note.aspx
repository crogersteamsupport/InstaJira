<%@ Page Title="Note" Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true"
  CodeFile="Note.aspx.cs" Inherits="Dialogs_Note" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">
  
        <div>
      Title</div>
    <div>
      <telerik:RadTextBox ID="textTitle" runat="server" Width="760px"></telerik:RadTextBox></div>
      <br />
      <telerik:RadEditor ID="editorDescription" runat="server" EditModes="Design" ToolsFile="~/Editor/StandardTools.xml" OnClientLoad="EditorLoad" StripFormattingOnPaste="MSWord"
        BorderWidth="0px" Height="330px" Width="765px">
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
