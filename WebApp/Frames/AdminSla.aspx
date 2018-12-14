<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
  CodeFile="AdminSla.aspx.cs" Inherits="Frames_AdminSla" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

  <style type="text/css">
    .tbMainAdmin
    {
        overflow: hidden;
    }
    .divTriggers thead
    {
      background-color: #E2E2E2;
      border-bottom: solid 1px #73A3FE;
      text-align:left;
    }
    .divTriggers td
    {
      border-top: solid 1px #A2B8CE;
    }
    .divTriggers img
    {
      cursor: pointer;
    }
    .hiddenSpan
    {
      display: none;
    }
  </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadSplitter ID="RadSplitter1" runat="server" Height="100%" Width="100%"
    BorderSize="0" Orientation="Horizontal">
    <telerik:RadPane ID="paneToolbar" runat="server" Height="32px">
      <telerik:RadToolBar ID="tbMain" runat="server" Width="100%" OnClientButtonClicked="OnToolBarButtonClicked"
        CssClass="NoRoundedCornerEnds tbMainAdmin">
        <Items>
          <telerik:RadToolBarButton runat="server" Text="New SLA" Value="NewSLA" ImageUrl="~/images/icons/new.png">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Edit SLA" Value="EditSLA" ImageUrl="~/images/icons/edit.png">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Delete SLA" Value="DeleteSLA" ImageUrl="~/images/icons/trash.png">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Clone SLA" Value="CloneSLA" ImageUrl="~/images/icons/RequestUpdate.png">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Button 3">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Add Trigger" Value="AddTrigger" ImageUrl="~/images/icons/add.png">
          </telerik:RadToolBarButton>
        </Items>
      </telerik:RadToolBar>
    </telerik:RadPane>
    <telerik:RadPane ID="RadPane2" runat="server" Scrolling="Y" BackColor="#ffffff">
      <div id="pnlMain" runat="server" style="">
        <div style="padding: 10px 0 0 17px; width: 750px;">
          <div style="float: left; padding-right: 20px;">
            <div style="padding-bottom: 5px;">
              Service Level Agreement:</div>
            <div>
              <telerik:RadComboBox ID="cmbLevels" runat="server" OnClientSelectedIndexChanged="OnLevelComboChanges">
              </telerik:RadComboBox>
            </div>
          </div>
          <div id="pnlTicket" runat="server" style="float: left; padding-right: 20px;">
            <div style="padding-bottom: 5px;">
              Ticket Type:</div>
            <div>
              <telerik:RadComboBox ID="cmbTicketTypes" runat="server" OnClientSelectedIndexChanged="OnTicketTypeChanged">
              </telerik:RadComboBox>
            </div>
          </div>
           
        </div>
        <div style="clear: both;">
        </div>
        <div style="width: 100%; height: 90%; padding: 10px 0 0 15px;">
          <div class="groupDiv groupLightBlue" style="width: 100%;">
            <div class="groupHeaderDiv">
              <span class="groupHeaderSpan"></span><span class="groupCaptionSpan">Triggers</span>
              <span class="groupButtonSpanWrapper hiddenSpan addTriggerSpan"><span class="groupButtonsSpan">
                <asp:LinkButton ID="lnkAddTrigger" runat="server" CssClass="groupButtonLink" OnClientClick="AddTrigger(); return false;">
              <span class="groupButtonSpan">
                <img alt="" src="../images/icons/add.png" class="groupButtonImage" />
                <span class="groupButtonTextSpan">Add Trigger</span> </span></asp:LinkButton>
              </span></span>
            </div>
            <div class="groupBodyWrapperDiv">
              <div class="groupBodyDiv">
                <div id="divTriggers" class="divTriggers" runat="server">
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </telerik:RadPane>
  </telerik:RadSplitter>
  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

    <script type="text/javascript" language="javascript">

      var _slaID = '';
      function pageLoad() {
        var comboLevels = $find("<%=cmbLevels.ClientID %>");
        SetEnablement(comboLevels.get_items().get_count() > 0);

      }

      function SetEnablement(value) {
        var toolBar = $find("<%= tbMain.ClientID %>");
        var cmbTicketTypes = $find("<%=cmbTicketTypes.ClientID %>");
        var comboLevels = $find("<%=cmbLevels.ClientID %>");
        if (value) {
          $('.addTriggerSpan').removeClass('hiddenSpan');
          toolBar.findItemByValue('EditSLA').enable();
          toolBar.findItemByValue('DeleteSLA').enable();
          toolBar.findItemByValue('AddTrigger').enable();
          toolBar.findItemByValue('CloneSLA').enable();
          comboLevels.enable();
          cmbTicketTypes.enable();
        }
        else {
          $('.addTriggerSpan').addClass('hiddenSpan');
          toolBar.findItemByValue('EditSLA').disable();
          toolBar.findItemByValue('DeleteSLA').disable();
          toolBar.findItemByValue('AddTrigger').disable();
          toolBar.findItemByValue('CloneSLA').disable();
          comboLevels.disable();
          cmbTicketTypes.disable();
        }

      }


      function TriggerDialogClosed(sender, args) {
        sender.remove_close(TriggerDialogClosed);
        RefreshTriggers();
      }

      function ShowTriggerDialog(wnd) { wnd.add_close(TriggerDialogClosed); wnd.show(); }

      function RefreshTriggers() {
        var id = GetSelectedLevelID();
        if (id == '') id = -1;
        PageMethods.GetTriggersHtml(id, GetSelectedTicketTypeID(), OnRefreshTriggers);
      }
      function OnRefreshTriggers(result) {
        $('.divTriggers').html(result);
      }

      function LevelDialogClosed(sender, args) {
        sender.remove_close(LevelDialogClosed);
        _slaID = GetSelectedLevelID();
        if (args != null && args.get_argument() != null) _slaID = args.get_argument();
        PageMethods.GetComboLevels(OnLevelsLoad);
      }

      function ShowLevelDialog(wnd) { wnd.add_close(LevelDialogClosed); wnd.show(); }

      function OnLevelsLoad(result) {
        try {
          SetEnablement(result.length > 0);

          var comboLevels = $find("<%=cmbLevels.ClientID %>");
          var selectedLevel = comboLevels.get_value();
          if (_slaID != '') selectedLevel = _slaID;

          comboLevels.trackChanges();
          comboLevels.clearItems();

          for (var i = 0; i < result.length; i++) {
            var item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text(result[i].Text);
            var value = result[i].Value;
            item.set_value(value);
            comboLevels.get_items().add(item);
          }

          //if (comboLevels.get_value() != selectedLevel && comboLevels.get_items().get_count() > 0) comboLevels.get_items().getItem(0).select();
          comboLevels.set_value(selectedLevel);
          comboLevels.commitChanges();
          var selectedItem = comboLevels.findItemByValue(selectedLevel);
          if (selectedItem)
            selectedItem.select();
          else if (comboLevels.get_items().get_count() > 0)
            comboLevels.get_items().getItem(0).select();
          else {
            $find("<%=cmbLevels.ClientID %>").set_text('');
            RefreshTriggers();
          }


        }
        catch (err) { }
      }

      function OnLevelComboChanges(sender, args) {
        parent.parent.privateServices.SetSessionSetting('SelectedAdminSlaLevelID', GetSelectedLevelID());
        RefreshTriggers();
      }

      function OnTicketTypeChanged(sender, args) {
        parent.parent.privateServices.SetSessionSetting('SelectedSlaAdminTicketTypeID', GetSelectedTicketTypeID());
        RefreshTriggers();
      }

      function GetSelectedLevelID() { return $find("<%=cmbLevels.ClientID %>").get_value(); }
      function GetSelectedTicketTypeID() { return $find("<%=cmbTicketTypes.ClientID %>").get_value(); }
      function GetSelectedLevelName() { return $find("<%=cmbLevels.ClientID %>").get_text(); }
      function DeleteSlaLevel() {
        if (!confirm("Are you sure you would like to delete '" + GetSelectedLevelName() + "'?")) return;
        _slaID = ''
        PageMethods.DeleteLevel(GetSelectedLevelID(), function () { PageMethods.GetComboLevels(OnLevelsLoad); });
        parent.parent.Ts.System.logAction('Admin SLA - SLA Level Deleted');
      }

      function AddTrigger() {
        ShowTriggerDialog(parent.parent.GetSlaTriggerDialog(true, GetSelectedLevelID(), GetSelectedTicketTypeID()));
        parent.parent.Ts.System.logAction('Admin SLA - Add Trigger Dialog Opened');
      }
      function EditTrigger(id) {
        ShowTriggerDialog(parent.parent.GetSlaTriggerDialog(false, id));
      }
      function DeleteTrigger(id) {
        if (!confirm("Are you sure you would like to delete this trigger?")) return;
        PageMethods.DeleteTrigger(id, function () {
          RefreshTriggers();
          parent.parent.Ts.System.logAction('Admin SLA - Trigger Deleted');
         });

      }

      function OnToolBarButtonClicked(sender, args) {
        var value = args.get_item().get_value();
        if (value == 'NewSLA') { ShowLevelDialog(parent.parent.GetSlaLevelDialog(true)); parent.parent.Ts.System.logAction('Admin SLA - New SLA Level Dialog Opened'); }
        else if (value == 'EditSLA') { ShowLevelDialog(parent.parent.GetSlaLevelDialog(false, GetSelectedLevelID())); parent.parent.Ts.System.logAction('Admin SLA - Edit SLA Level Dialog Opened'); }
        else if (value == 'DeleteSLA') { DeleteSlaLevel(); }
        else if (value == 'AddTrigger') { AddTrigger(); }
        else if (value == 'CloneSLA') { ShowLevelDialog(parent.parent.GetSlaLevelDialog(true, GetSelectedLevelID())); parent.parent.Ts.System.logAction('Admin SLA - Clone SLA Level Dialog Opened'); }
      }
    
    </script>

  </telerik:RadScriptBlock>
</asp:Content>
