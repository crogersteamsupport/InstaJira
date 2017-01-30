﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Chat/Chat.master" AutoEventWireup="true" CodeFile="ChatInit.aspx.cs" Inherits="Chat_ChatInit" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

  <script language="javascript" type="text/javascript">

    function Validate() {
      var message = '';
      
      if ($find("<%=textFirstName.ClientID %>").get_value().trim() == '' ||
          $find("<%=textLastName.ClientID %>").get_value().trim() == '') {

        message = '<div>Please fill in your name.</div>';
      }
      if (!IsValidEmail($find("<%=textEmail.ClientID %>").get_value().trim())) {
        message = message + '<div>Please enter a valid email address.</div>'
      }

      if (message != '') {
        $('#divValidate').html(message).show('slow');
        return false;
      }
      else {
        return true;
      }
    }

    function IsValidEmail(email) {
      var pattern = /^([a-zA-Z0-9_.-])+@([a-zA-Z0-9_.-])+\.([a-zA-Z])+([a-zA-Z])+/;
      return pattern.test(email);
    }
    
  
  
  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  
 
  <div class="chat-content">
  
  <div style="float:left; width: 250px;">
  <h2>How can we help?</h2>
  <p>Please submit your question to start a live chat.</p>
  <div id="divValidate" class="ui-helper-hidden ui-state-highlight ui-corner-all" style="margin: 10px 10px; padding: 5px 5px;" ></div>
  <div style="padding-bottom: 8px;">
  <div>First Name<em>*</em></div>
  <div><telerik:RadTextBox ID="textFirstName" runat="server" Width="200px" Skin="Default"></telerik:RadTextBox></div>
  </div>
  <div style="padding-bottom: 8px;">
  <div>Last Name<em>*</em></div>
  <div><telerik:RadTextBox ID="textLastName" runat="server" Width="200px" Skin="Default"></telerik:RadTextBox></div>
  </div>
  <div style="padding-bottom: 8px;">
  <div>Email<em>*</em></div>
  <div><telerik:RadTextBox ID="textEmail" runat="server" Width="200px" Skin="Default"></telerik:RadTextBox></div>
  </div>
  <div style="padding-bottom: 8px;">
  <em>* Required</em></div>
  </div>
  
  <div style="float: right; margin: 5px 0 0 0;">
    <div class="chat-logo" style="margin: 0 auto;">
    </div>
  </div>
  
  
  <div style="clear:both;"></div>
  
  <div>What is your question?</div>
  <telerik:RadTextBox ID="textMessage" runat="server"  TextMode="MultiLine" Width="100%" Height="100px" MaxLength="2500" Skin="Default"></telerik:RadTextBox>
        
  <div style="text-align:center; margin:0 auto; padding: 10px 0;"><asp:Button ID="btnSubmit" runat="server" Text="Submit" onclick="btnSubmit_Click" OnClientClick="return Validate();" /></div>
  
  </div>

  
  
</asp:Content>

