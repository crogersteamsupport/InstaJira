<%@ Page Title="Chat Properties" Language="C#" MasterPageFile="~/Dialogs/Dialog.master"
  AutoEventWireup="true" CodeFile="ChatProperties.aspx.cs" Inherits="Dialogs_ChatProperties" ValidateRequest="false" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

  <link href="../Css/jquery-ui-latest.custom.css" rel="stylesheet" type="text/css" />
  <script src="/frontend/library/jquery-1.4.2.min.js" type="text/javascript"></script>
  <script src="/frontend/library/jquery-ui-1.8.14.custom.min.js" type="text/javascript"></script>
  <script src="/frontend/library/ajaxupload.js" type="text/javascript"></script>
  <style type="text/css">
    .crmLabelDiv
    {
      padding-top: 10px;
    }
    .dialogContentDiv div
    {
      padding-bottom: 5px;
    }
    
    .label {}
    .image {}
    .button {}
    
  </style>
  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">
       <div class="image">
         <asp:Image ID="imgAvailable" runat="server" />
       </div>
      <div class="label">
        Available Image Upload</div>
      <div>
        <telerik:RadUpload ID="upAvailable" runat="server" MaxFileInputsCount="1" OverwriteExistingFiles="True"
          ControlObjectsVisibility="None">
        </telerik:RadUpload>
      </div>
       <div class="image">
         <asp:Image ID="imgUnavailable" runat="server" />
       </div>
      <div class="label">
        Unavailable Image Upload</div>
      <div>
        <telerik:RadUpload ID="upUnavailable" runat="server" MaxFileInputsCount="1" OverwriteExistingFiles="True"
          ControlObjectsVisibility="None">
        </telerik:RadUpload>
      </div>
       <div class="image">
         <asp:Image ID="imgLogo" runat="server" />
       </div>
      <div class="label">
        Company Logo Upload (Side Banner)</div>
        <div>Recomended Size (w:100 x h:250)</div>
      <div>
        <telerik:RadUpload ID="upLogo" runat="server" MaxFileInputsCount="1" OverwriteExistingFiles="True"
          ControlObjectsVisibility="None">
        </telerik:RadUpload>
      </div>
      <div class="label">
        Chat Intro Message</div>
      <div>
        <telerik:RadTextBox ID="textChatIntro" runat="server" Width="200px"></telerik:RadTextBox>
      </div>
        <div>
            <asp:CheckBox ID="cbChatAvatars" runat="server" Text="Enable Avatars"/>
        </div>
        <div>
            <asp:CheckBox ID="cbChatTOKScreenEnabled" runat="server" Text="Enable Screen Sharing"/>
        </div>
        <div>
            <asp:CheckBox ID="cbChatTicketDeflectionEnabled" runat="server" Text="Enable Ticket Deflection"/>
        </div>
        <div>
            <asp:CheckBox ID="cbChatTOKVoiceEnabled" runat="server" Text="Enable Voice Calls"/>
        </div>
        <div>
            <asp:CheckBox ID="cbChatTOKVideoEnabled" runat="server" Text="Enable Video Calls"/>
        </div>
    </div>
  </div>
  
  
</asp:Content>
