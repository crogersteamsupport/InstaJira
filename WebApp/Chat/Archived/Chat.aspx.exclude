<%@ Page Title="" Language="C#" MasterPageFile="~/Chat/Archived/Chat.master" AutoEventWireup="true"
  CodeFile="Chat.aspx.cs" Inherits="Chat_Chat" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <style type="text/css">
    #divChatMessagesWrapper
    {
      background: #fff;
      margin: 5px 0 0 5px;
      border: solid 1px #b8b8b8;
      position: relative;
      width: 100%;
      height: 100%;
      float: left;
      overflow:auto;
    }
  </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadSplitter ID="RadSplitter1" runat="server" Orientation="Horizontal" Height="100%"
    Width="100%" BorderSize="0">
    <telerik:RadPane ID="paneMessages" runat="server" Scrolling="None" Height="100%"
      OnClientResizing="function(sender,args){AdjustMessageDiv();}" OnClientResized="function(sender,args){AdjustMessageDiv();}">
      <div id="divChatMessagesWrapper">
        <div id="divChatMessages" style="padding: 5px 5px;">
          Waiting for a chat operator...</div>
      </div>
      <div style="float: right; margin: 5px 5px 0 0;">
        <div class="chat-logo" style="margin: 0 auto;">
        </div>
      </div>
    </telerik:RadPane>
    <telerik:RadPane ID="paneInput" runat="server" Scrolling="None" Height="65px">
      <table border="0" cellpadding="5" cellspacing="0" width="100%">
        <tr>
          <td valign="middle">
            <div>
              <telerik:RadTextBox ID="textSend" runat="server" Width="100%" Height="52px" TextMode="MultiLine"
                ClientEvents-OnKeyPress="textSend_OnKeyPress" Skin="Default">
              </telerik:RadTextBox>
            </div>
          </td>
          <td valign="middle" style="width: 105px; vertical-align: top; text-align: right;">
            <a class="chat-button chat-button-send" href="#" onclick="SendMessage();"></a>
          </td>
        </tr>
      </table>
    </telerik:RadPane>
    <telerik:RadPane ID="paneFooter" runat="server" Scrolling="None" Height="20px" >
      <div style="padding-left: 5px;">
        <table border="0" cellpadding="0" cellspacing="0">
          <tr style="height: 20px;">
            <td valign="bottom">
              <img alt="" src="../images/icons/lock.png" />
            </td>
            <td valign="middle">
              <span id="typers" style="padding-left: 5px;"></span>
            </td>
          </tr>
        </table>
      </div>
    </telerik:RadPane>
  </telerik:RadSplitter>
  <asp:HiddenField ID="fieldChatID" runat="server" />
  <asp:HiddenField ID="fieldRequestID" runat="server" />
  
  <script type="text/javascript" language="javascript">
    var _lastMessageID = -1;
    var _closeCount = 0;
    var _isAccepted = false;
    var _startTime = new Date();

    $(document).ready(function() { });

    function pageLoad() {
      AdjustMessageDiv();
      setInterval('GetChatStatus();', 1250);
    }

    function GetChatID() { return "<%=fieldChatID.Value %>"; }
    function GetRequestID() { return "<%=fieldRequestID.Value %>"; }

    function GetChatStatus() {
      PageMethods.GetChatStatus(GetChatID(), _lastMessageID, GetRequestID(), function (status) {
        if (_isAccepted) {
          $('#typers').html(status.Typers)
          _lastMessageID = status.LastMessageID;
          if (status.ChatHtml != null) {
            var atBottom = 10 > $('#divChatMessagesWrapper')[0].scrollHeight - $('#divChatMessagesWrapper')[0].offsetHeight - $('#divChatMessagesWrapper').scrollTop();
            $('#divChatMessages').html(status.ChatHtml.replace(/(\b(https?|ftp|file):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/igm, '<a href="$1" target="_blank">$1</a>'));

            if (atBottom) $('#divChatMessagesWrapper').scrollTop($('#divChatMessagesWrapper')[0].scrollHeight);
          }
          if (status.ParticipantCount < 2) {
            _closeCount++;
            if (_closeCount > 10) {
              window.location = 'ThankYou.aspx';
              return;
            }
          }
        } else {
          var now = new Date();
          var diff = (now - _startTime) / 1000;
          if (diff > 120) {
            window.location = 'ChatMissed.aspx' + window.location.search;
          }
          _isAccepted = status.IsAccepted;

        }
      });
    }
    
    function SendMessage() {
      if (!_isAccepted) return;
      var textbox = $find("<%=textSend.ClientID %>");
      var message = textbox.get_value().trim();
      if (message == '') return;
      PageMethods.PostMessage(GetChatID(), textbox.get_value(), function() { GetChatStatus(); });
      textbox.clear();
      textbox.selectAllText();
      textbox.set_caretPosition(0);
    }

    function textSend_OnKeyPress(sender, args) {
      PageMethods.SetTyping(GetChatID());
      if (args.get_keyCode() == 13) { SendMessage(); }
    }

    function AdjustMessageDiv() {
      var pane = $find("<%=paneMessages.ClientID %>");
      $('#divChatMessagesWrapper').width(pane.get_width() - $('.chat-logo').width() - 20);
      $('#divChatMessagesWrapper').height(pane.get_height() - 10);
    }
    
  </script>

</asp:Content>
