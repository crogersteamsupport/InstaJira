<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
  CodeFile="Chat.aspx.cs" Inherits="Frames_Chat" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <link href="../Css/jquery-ui-latest.custom.css" rel="stylesheet" type="text/css" />
  <link href="../Css/ui.css" rel="stylesheet" type="text/css" />
  <script src="../vcr/1_9_0/Js/Ts/ts.pendo.js" type="text/javascript"></script>
    <script src="/frontend/javascript/vendors/growthscore.js?1515087127" type="text/javascript"></script>

  <style type="text/css">
    body { background-color: #fff !important; }
    .reToolbar.Office2007 .InsertTicketLink { background-image: url(../images/icons/add.png); }
    .reButton_text { text-indent: 0px !important;}
    .requests table { font-size: 12px; }
    .requests td { vertical-align: top; }
    .requests div.accept { padding: 5px 0; text-align: center;  }
    .requests td.col1 { font-weight: bold; }
    .requests div.content {margin-bottom: 7px;}
    .chat { padding: 5px 5px; cursor: pointer; }
    .chat.ui-state-hover {font-weight:normal; }
    .chat.ui-state-hover.ui-state-default {font-weight:bold;}
    .chat-state-normal {border: solid 1px #ffffff;}
    #divActiveChats{ padding: 5px 5px;}
    #divChatRequests { padding: 5px 5px; }
    #divChatMessages {font-size:12px;}
    #divChatMessagesWrapper { position: relative; width: 100%; height: 100%; overflow:auto;}
    #divTyping{ width: 100%; height: 20px; text-align:center;}
  </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadSplitter ID="splMain" runat="server" Height="100%" Width="100%"
    BorderSize="0" Orientation="Vertical">
    <telerik:RadPane ID="paneLeft" runat="server" Width="250px" BackColor="White" Scrolling="None">
      <telerik:RadSplitter ID="RadSplitter1" runat="server" Height="100%" Width="100%" BorderSize="0" Orientation="Horizontal">
        <telerik:RadPane ID="paneRequests" runat="server" Height="300px" Scrolling="Y">
        <div class="ts-list-section">
          Chat &amp; Transfers Requests </div>
        <div id="divChatRequests" class="ts-panellist ui-widget ui-helper-reset requests">
        </div>
        </telerik:RadPane>
        <telerik:RadSplitBar ID="RadSplitBar2" runat="server" />
        <telerik:RadPane ID="paneChats" runat="server" Scrolling="Y">
        <div class="ts-list-section">
          Active Chats</div>
        <div id="divActiveChats" class="ui-widget ui-helper-reset">
        </div>
        </telerik:RadPane>
      </telerik:RadSplitter>
    </telerik:RadPane>
    <telerik:RadSplitBar ID="RadSplitBar1" runat="server" />
    <telerik:RadPane ID="paneDialog" runat="server" Scrolling="None">
      <telerik:RadSplitter ID="splMessages" runat="server" Height="100%" Width="100%"
        BorderSize="0" Orientation="Horizontal">
        <telerik:RadPane ID="paneTools" runat="server" Scrolling="None" Height="32px">
          <telerik:RadToolBar ID="tbChat" runat="server" CssClass="NoRoundedCornerEnds" OnClientButtonClicked="tbChat_OnClientButtonClicked">
            <Items>
              <telerik:RadToolBarButton runat="server" Text="I am available" Value="available" ImageUrl="../images/icons/chat.png" Visible="false">
              </telerik:RadToolBarButton>
              <telerik:RadToolBarButton runat="server" Text="Leave Chat" Value="leave" ImageUrl="../images/icons/close_2.png" Enabled="false">
              </telerik:RadToolBarButton>
              <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Button 4">
              </telerik:RadToolBarButton>
              <telerik:RadToolBarButton runat="server" Text="Transfer" Value="transfer" ImageUrl="../images/icons/chattransfer.png" Visible="false" Enabled="false">
              </telerik:RadToolBarButton>
              <telerik:RadToolBarButton runat="server" Text="Invite" Value="invite" ImageUrl="../images/icons/chatinvite.png" Enabled="false">
              </telerik:RadToolBarButton>
              <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Button 5">
              </telerik:RadToolBarButton>
              <telerik:RadToolBarButton runat="server" Text="Open Customer" Value="opencustomer" ImageUrl="../images/icons/open.png" Enabled="false">
              </telerik:RadToolBarButton>
              <telerik:RadToolBarButton runat="server" Text="Create Ticket" Value="createticket" ImageUrl="../images/icons/new.png" Enabled="false">
              </telerik:RadToolBarButton>
              <telerik:RadToolBarButton runat="server" Text="Add to Ticket" Value="addticket" ImageUrl="../images/icons/add.png" Enabled="false">
              </telerik:RadToolBarButton>
              <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Button 5">
              </telerik:RadToolBarButton>
              <telerik:RadToolBarButton runat="server" Text="Open Ticket" Value="openticket" ImageUrl="../images/icons/open.png" Enabled="false">
              </telerik:RadToolBarButton>
            </Items>
          </telerik:RadToolBar>
        </telerik:RadPane>
        <telerik:RadPane ID="paneMessages" runat="server" BackColor="White" Scrolling="None">
          <div id="divChatMessagesWrapper">
            <div id="divChatMessages">
            </div>
          </div>
        </telerik:RadPane>
        <telerik:RadPane ID="paneInput" runat="server" Height="100px" BackColor="White" Scrolling="None">
          <table border="0" cellpadding="0" cellspacing="5" width="100%">
            <tr>
              <td colspan="2">
                <div id="divTyping">
                </div>
              </td>
            </tr>
            <tr>
              <td>
                <textarea id="textSend" rows="3" style="width:100%"></textarea>
              </td>
              <td style="width: 55px; text-align: right;">
                <asp:Button ID="btnSend" runat="server" Text="Send" Height="50px" Width="50px" OnClientClick="SendMessage(); return false;" />
              </td>
            </tr>
          </table>
        </telerik:RadPane>
      </telerik:RadSplitter>
    </telerik:RadPane>
    <telerik:RadPane ID="paneCanned" runat="server" Width="250px" Scrolling="None" Visible="false">
        <div class="ts-list-section">
          Canned Text</div>
    </telerik:RadPane>
  </telerik:RadSplitter>


  <script type="text/javascript" language="javascript">
    var _activeChatID = -1;
    var _lastMessageID = -1;
    var _actionID = null;
    var _contactID = null;
    var _intervalUpdateChatRequests = null;
    var _intervalUpdateActiveChats = null;
    var _intervalUpdateMessages = null;
    var _userInfo = null;

    //$(document).ready(function() {  });
    function pageLoad() {
      PageMethods.GetUserInfo(function(result) {
        _userInfo = result;
        onShow();
      });
    }

    function onShow() {
      if (_intervalUpdateChatRequests != null) return;
      UpdateChatRequests(true);
      UpdateActiveChats(true);
      _intervalUpdateChatRequests = setInterval('UpdateChatRequests(false);', 7500);
      _intervalUpdateActiveChats = setInterval('UpdateActiveChats(false);', 5200);
      _intervalUpdateMessages = setInterval('UpdateMessages();', 1250);
    }

    function onHide() {
      PageMethods.SetCurrentChatID(-1);
      clearInterval(_intervalUpdateChatRequests);
      clearInterval(_intervalUpdateActiveChats);
      clearInterval(_intervalUpdateMessages);
      _intervalUpdateChatRequests = null;
    }

    function GetUserInfo() { PageMethods.GetUserInfo(function(result) { _userInfo = result;  }); }

    function UpdateChatRequests(forceUpdate) {
      if (_userInfo == null) { GetUserInfo(); return; }
      PageMethods.GetChatRequestHtml(_userInfo, forceUpdate, function(result) {
        if (result == '') return;
        $('#divChatRequests').html(result);
        $('#divChatRequests .ts-panellist-header').next().hide();
        $('a.ts-link-button').hover(function() { $(this).addClass('ui-state-hover'); }, function() { $(this).removeClass('ui-state-hover'); });

        $('#divChatRequests .ts-panellist-header').click(function() {
          $('#divChatRequests .ts-panellist-header').next().hide();
          $('#divChatRequests .ui-icon-triangle-1-se').addClass('ui-icon-triangle-1-e').removeClass('ui-icon-triangle-1-se');
          $(this).next().show();
          $(this).find('.ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-se').removeClass('ui-icon-triangle-1-e');
          return false;
        });

        $('#divChatRequests .ts-panellist-header:first').click();
      });
    }

    function UpdateActiveChats(forceUpdate) {
      if (_userInfo == null) { GetUserInfo(); return; }
      PageMethods.GetActiveChatsHtml(_userInfo, forceUpdate, _activeChatID, function(result) {
        if (!result) return;
        if (result[1] != _activeChatID) return;
        $('#divActiveChats').html(result[0]);
        $('.chat').click(function() {
          LoadChat(GetChatID(this));
          return false;
        }).hover(function() { $(this).addClass('ui-state-hover'); }, function() { $(this).removeClass('ui-state-hover'); });
        if (_activeChatID < 0) $('.chat:first').click();
      });
      RefreshActionID();
      SetToolbar();
    }

    function RefreshActionID() {
      PageMethods.GetActionID(_activeChatID, function(result) {
        if (!result) return;
        if (result[0] == _activeChatID) _actionID = result[1];
        if (result[0] == _activeChatID) _contactID = result[2];
      });
    }

    function UpdateMessages() {
      if (_userInfo == null) {
        GetUserInfo();
        return;
      }

      if (!_activeChatID || _activeChatID < 0) {
        $('#divChatMessages').html('');
        return;
      }

      PageMethods.GetChatHtml(_userInfo, _activeChatID, _lastMessageID, function(result) {
        if (!result || _activeChatID != result[0]) return;
        _lastMessageID = result[1];
        if (result[2] != '') {
          var atBottom = 10 > $('#divChatMessagesWrapper')[0].scrollHeight - $('#divChatMessagesWrapper')[0].offsetHeight - $('#divChatMessagesWrapper').scrollTop();

          $('#divChatMessages').html(result[2].replace(/(\b(https?|ftp|file):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/igm, '<a href="$1" target="_blank">$1</a>'));
          if (atBottom) $('#divChatMessagesWrapper').scrollTop($('#divChatMessagesWrapper')[0].scrollHeight);
        }
        $('#divTyping').text(result[3])
      });
    }


    function AcceptRequest(chatRequestID) {
      parent.Ts.System.logAction('Chat - Chat Request Accepted');
      PageMethods.AcceptRequest(chatRequestID, function(result) {
        if (result < 0) {
          alert('Request was already accepted.');
          return;
        }
        LoadChat(result);
        UpdateChatRequests(true);
      });
    }

    function GetChatID(element) {
      return element.id.substring(7);
    }

    function LoadChat(chatID) {
      if (!chatID || chatID < 0) {
        _activeChatID = -1;
        return;
      }
      _activeChatID = chatID;
      _lastMessageID = -1;
      _actionID = null;
      _contactID = null;
      $('#divChatMessages').html('');
      UpdateActiveChats(true);
      UpdateMessages();
      RefreshActionID();
      parent.privateServices.UpdateUserActivityTime();
    }



    function SendMessage() {
      if (_activeChatID < 0) return;
      parent.Ts.System.logAction('Chat - Message Sent');
      var textbox = $('#textSend');
      var message = textbox.val().trim();
      if (message == '') return;
      PageMethods.PostMessage(message, _activeChatID, function () { UpdateMessages(); });
      textbox.val('');
    }

    $('#textSend').keydown(function (e) {
      if (e.which == 13) { SendMessage(); } else { PageMethods.SetTyping(_activeChatID); }
    });



    function SetToolbar() {
      var toolbar = $find("<%=tbChat.ClientID %>");
      if (!_activeChatID || _activeChatID < 0) {
        toolbar.findItemByValue('leave').set_enabled(false);
        toolbar.findItemByValue('invite').set_enabled(false);
        toolbar.findItemByValue('createticket').set_enabled(false);
        toolbar.findItemByValue('addticket').set_enabled(false);
        toolbar.findItemByValue('openticket').set_enabled(false);
      }
      else {
        toolbar.findItemByValue('leave').set_enabled(true);
        toolbar.findItemByValue('invite').set_enabled(true);

        var btnOpenCustomer = toolbar.findItemByValue('opencustomer');
        if (btnOpenCustomer != null)
        {
          if (_contactID == null || _contactID < 0) {
            btnOpenCustomer.set_enabled(false);
          }
          else {
            btnOpenCustomer.set_enabled(true);
          }
        }


        if (_actionID == null) {
          toolbar.findItemByValue('createticket').set_enabled(false);
          toolbar.findItemByValue('addticket').set_enabled(false);
          toolbar.findItemByValue('openticket').set_enabled(false);
        }
        else if (_actionID < 0) {
          toolbar.findItemByValue('createticket').set_enabled(true);
          toolbar.findItemByValue('addticket').set_enabled(true);
          toolbar.findItemByValue('openticket').set_enabled(false);
        }
        else {
          toolbar.findItemByValue('createticket').set_enabled(false);
          toolbar.findItemByValue('addticket').set_enabled(false);
          toolbar.findItemByValue('openticket').set_enabled(true);
        }
      }

    }

    function tbChat_OnClientButtonClicked(sender, args) {
      var button = args.get_item();
      var value = button.get_value();
      if (value == 'leave') { CloseChat(); }
      else if (value == 'invite') { InviteChat(_activeChatID); }
      else if (value == 'createticket') { CreateTicket(_activeChatID); }
      else if (value == 'addticket') { AddTicket(_activeChatID); }
      else if (value == 'openticket') { OpenTicket(_activeChatID); }
      else if (value == 'available') { ToggleAvailable(); }
      else if (value == 'opencustomer') { parent.Ts.MainPage.openContact(_contactID); }


    }

    function CloseChat() {
      if (confirm("Are you sure you would like to leave this chat?")) {
        parent.Ts.System.logAction('Chat - Chat Closed');
        PageMethods.CloseChat(_activeChatID, function(result) {
          _activeChatID = -1;
          $('#divChatMessages').html('Closed');
          UpdateActiveChats(true);
          UpdateMessages();
        });
      }
    }

    function TransferChat(chatID) {
      parent.ShowUserDialog('OtherChatUsers', true, function (arg) {
        parent.Ts.System.logAction('Chat - Chat Transfered');
        if (arg) PageMethods.RequestTransfer(chatID, arg);
      }, 'Select a Chat User');
    }

    function InviteChat(chatID) {
      parent.ShowUserDialog('OtherChatUsers', true, function (arg) {
        parent.Ts.System.logAction('Chat - Invitation Sent');
        if (arg) PageMethods.RequestInvite(chatID, arg);
      }, 'Select a Chat User');
    }

    function CreateTicket(chatID) {
      parent.Ts.MainPage.newTicket('?ChatID=' + chatID);
      parent.Ts.System.logAction('Chat - Ticket Created');
    }

    function AddTicket(chatID) {
      parent.ShowTicketDialog(true, function(ticketID) {
        PageMethods.AddTicket(chatID, ticketID, function(ticketID) {
          parent.Ts.MainPage.openTicketByID(ticketID);
          parent.Ts.System.logAction('Chat - Ticket Added');
        });
      });
    }
    function OpenTicket(chatID) {
      PageMethods.GetTicketID(chatID, function (ticketID) {
        parent.Ts.MainPage.openTicketByID(ticketID);
        parent.Ts.System.logAction('Chat - Ticket Opened');
      });
    }

    function ToggleAvailable() {
      PageMethods.ToggleAvailable(function (isAvailable) {
        parent.Ts.System.logAction('Chat - Availablity Set');
        var button = $find("<%=tbChat.ClientID %>").findItemByValue('available');
        if (isAvailable) {
          button.set_text('I am available');
          button.set_imageUrl('../images/icons/chat.png');
        }
        else {
          button.set_text('I am not available');
          button.set_imageUrl('../images/icons/chat_d.png');
        }

      });
    }

  </script>



</asp:Content>
