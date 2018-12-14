<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs" Inherits="Frames_Dashboard" EnableViewState="false" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title></title>
  <link href="../vcr/1_9_0/Css/frame.css" rel="stylesheet" type="text/css" />
  <link href="../vcr/1_9_0/Css/ts.ui.css" rel="stylesheet" type="text/css" />
  <!--[if IE 7]><link href="../vcr/1_9_0/Css/ts.ui.ie7.css" rel="stylesheet" type="text/css" /><![endif]--><!--[if IE 8]><link href="../vcr/1_9_0/Css/ts.ui.ie8.css" rel="stylesheet" type="text/css" /><![endif]-->
  <link href="../Css/jquery-ui-latest.custom.css" rel="stylesheet" type="text/css" />
  <script src="/frontend/library/jquery-1.4.2.min.js" type="text/javascript"></script>
  <script src="/frontend/library/jquery-ui-1.8.14.custom.min.js" type="text/javascript"></script>
  <style type="text/css">
    body { background: #fff; }
    .header { float: right; padding: 7px 20px 0 0; }
    .header a { font-weight: bold; }
    .slaWarning { background: #FFFB00; }
    .slaViolation { background: #E86868; }
    .viewMore { padding: 3px 0 3px 7px; background: #E0EEFF; border-top: solid 1px #A6BED6; }
    .externalReport { padding: 0 0 2px 0; }
    .portlet-container-wrapper { height: 100%; overflow-y: auto; overflow-x: hidden; position:relative; border:none;}
    .portlet-container { padding: 10px 0 0 10px; overflow: hidden; }
    .portlet-column { padding-bottom: 10px; min-height: 20px; overflow: hidden; }
    #column0 { width: 300px; float: left; margin-left: -100%; }
    #column1Wrapper { float: left; width: 100%; }
    #column1 { margin-left: 300px; }
    .noRecords { padding: 10px 10px; }
    .portlet { margin: 0 15px 15px 0; overflow: hidden; position: relative; }
    .portlet-caption { width: 100px; display: block; float: left; }
    .portlet-header { height: 21px; line-height: 21px; overflow: hidden; padding-left: 10px; background: transparent url(../images/vmenu_selected.gif) repeat-x; border: solid 1px #A6BED6; }
    .portlet-body { border: solid 1px #A6BED6; border-top: none; position: relative; overflow: hidden; background: #fff; }
    .portlet-content { height: 100%; width: 100%; position: relative; overflow: auto; }
    .portlet-content table { width: 100%; }
    .portlet-content th { background: #E0EEFF; border-bottom: solid 1px #A6BED6; text-align: left; white-space: nowrap; height: 15px; font-size: .9em; }
    .portlet-content .icon { width: 20px; text-align: center; }
    .portlet-content .icon img { cursor: pointer; }
    /*.portlet-content td { text-align: left; border-bottom: solid 1px #A6BED6; border-right: solid 1px #A6BED6;}*/.portlet-content td { text-align: left; font-size: 0.9em; }
    .portlet-content tr { line-height: 0.9em; white-space: nowrap; }
    .portlet-content tr.oddRow { background: #EFF6FF; }
    .portlet-placeholder { border: 2px dashed #A5BDD4; visibility: visible !important; margin: 0 15px 15px 0; }
    .portlet-placeholder * { visibility: hidden; }
    .portlet-header .portlet-icon { float: right; top: 3px; margin-right: 7px; width: 16px; position: relative; }
  </style>
  <!--[if lte IE 7]>
<style>
    .portlet-header .portlet-icon { margin-top: -20px;}
</style>
<![endif]-->

  <script type="text/javascript" language="javascript">
    var _portlets = null;

    $(document).ready(function() {
      loadPortlets();
    });

    function onPortletResized(event, ui) {
      PageMethods.UpdatePortletHeight($(this).parent()[0].id, $(this).height(), function(result) { loadPortlets(); });
    }

    function onPortletPositionChanged(event, ui) {
      var result = new Array();
      for (var i = 0; i < 5; i++) {
        result[i] = getColumnPortlets(i);
      }
      PageMethods.UpdatePortletPositions(result);
    }

    function getColumnPortlets(index) {
      var result = new Array();
      $('#column' + index).find('.portlet').each(function(index, element) {
        result[result.length] = element.id;
      });
      return result;
    }

    function getPortletByID(id) {
      for (var i = 0; i < _portlets.length; i++) {
        if (_portlets[i].ID == id) { return _portlets[i]; }
      }
      return null;
    }


    function loadPortlets() {
      $('.page-loading').show().next().hide();
      PageMethods.GetColumnCount(function (colCount) {
        PageMethods.GetPortlets(function (portlets) {
          _portlets = portlets;


          $('.portlet-column').html('');
          for (var i = 0; i < portlets.length; i++) {
            $('#column' + portlets[i].X).append(portlets[i].Html);
            $('#' + portlets[i].ID).find('.portlet-body').height(portlets[i].Height);
          }
          $('.portlet-header .ts-icon-triangle-w').parent().next().toggle();
          $('.portlet table tr:odd').addClass('oddRow');
          $('.portlet-body').resizable({ handles: 's', stop: onPortletResized, grid: 20, containment: 'document', ghost: true });
          $('.portlet-header .portlet-state').click(function () {
            var isOpen = $(this).hasClass('ts-icon-triangle-s');
            if (isOpen) {
              $(this).addClass("ts-icon-triangle-w").removeClass("ts-icon-triangle-s");
              $(this).parent().addClass('ui-corner-all');
            }
            else {
              $(this).addClass("ts-icon-triangle-s").removeClass("ts-icon-triangle-w");
              $(this).parent().removeClass('ui-corner-all').addClass('ui-corner-top');
            }
            $(this).parent().next().toggle();
            PageMethods.UpdatePortletVisibility($(this).parent().parent()[0].id, !isOpen);
          });
          $('.portlet-header .portlet-close').click(function () {
            if (confirm('Are you sure you would like to remove this report from the dashboard?')) {
              PageMethods.DeletePortlet($(this).parent().parent()[0].id, function () { loadPortlets(); });
              top.Ts.System.logAction('Dashboard - Report Deleted');
            }
          });
          $('.portlet-column').sortable({ connectWith: '.portlet-column', cancel: '.portlet-body', items: '.portlet', placeholder: 'portlet-placeholder', forcePlaceholderSize: true, stop: onPortletPositionChanged, opacity: 0.6, tolerance: 'pointer' });
          $(".portlet-header").disableSelection();

          $('.page-loading').hide().next().show();

        }, function () { $('.page-loading').hide().next().show(); });
      });
    }

    function refresh() {
      loadPortlets();
    }

    function onShow() {
      loadPortlets();
    }

    function openTicket(ticketNumber) {
      top.Ts.MainPage.openTicket(ticketNumber, true);
      //PageMethods.GetTicketID(ticketNumber, function (result) { top.AddTicketTab(result, true); });
    }

    function openCustomer(name) {
      top.Ts.MainPage.openCustomerByName(name);
    }

    function showReportDialog() {
      var wnd = $find('wndSelectReportDialog');
      showDialog(wnd, true, null, 'Add a Report');
    }

    function showDialog(wnd, isModal, callback, title) {
      if (title && title != '') wnd.set_title(title);
      wnd.set_modal(isModal);
      if (callback) {
        var fn = function(sender, args) { sender.remove_close(fn); callback(sender.argument); }
        wnd.add_close(fn);
      }
      wnd.show();
    }    

  
  </script>

</head>
<body>
  <form id="form1" runat="server">
  <telerik:RadScriptManager ID="RadScriptManager1" runat="server" EnablePageMethods="true">
  </telerik:RadScriptManager>
  <div class="ts-loading page-loading"></div>
  <div class="portlet-container-wrapper ui-widget-content ui-widget">
    <div class="header">
      <a class="ts-link" href="#" onclick="showReportDialog(); return false;">Add Report</a> |
      <a class="ts-link" href="#" onclick="refresh(); top.Ts.System.logAction('Dashboard - Refreshed'); return false;">Refresh</a></div>
    <div style="clear: both; height: 1px;">
      &nbsp</div>
    <div class="portlet-container">
      <div id="column1Wrapper">
        <div class="portlet-column" id="column1">
        </div>
      </div>
      <div class="portlet-column" id="column0">
      </div>
    </div>
  </div>
  <telerik:RadWindow ID="wndSelectReportDialog" runat="server" Width="300px" Height="150px"
    Animation="None" KeepInScreenBounds="True" VisibleStatusbar="False" VisibleTitlebar="True"
    OnClientPageLoad="" Title="Select a User" Behaviors="Close,Move" IconUrl="../images/icons/TeamSupportLogo16.png"
    VisibleOnPageLoad="false" ShowContentDuringLoad="False" Modal="False" DestroyOnClose="True">
    <ContentTemplate>
      <div style="padding: 7px 20px;">
        <div style="font-weight: bold;">
          Report:</div>
        <div style="padding: 7px 0;">
          <telerik:RadComboBox ID="cmbReport" runat="server" Width="100%" AllowCustomText="False" ShowToggleImage="True"
            EnableLoadOnDemand="True" LoadingMessage="Loading reports..." 
            OnClientDropDownClosed="function(sender,args){sender.clearItems();}" EmptyMessage="Select a report..."
            ShowDropDownOnTextboxClick="False" CausesValidation="False" ChangeTextOnKeyBoardNavigation="False"
            ShowMoreResultsBox="False">
            <ExpandAnimation Type="None" />
            <WebServiceSettings Path="Dashboard.aspx" Method="GetReports" />
            <CollapseAnimation Duration="200" Type="None" />
          </telerik:RadComboBox>
          
          <div style="padding-top: 5px;">
          <span>Column: </span>
            <asp:RadioButton ID="rbLeft" runat="server" Text="Left" GroupName="newcolumns"/>
            <asp:RadioButton ID="rbRight" runat="server" Text="Right" GroupName="newcolumns" Checked="true"/>
          </div>
        </div>
        <div style="float: right;">
          <asp:Button ID="btnOk" runat="server" Text="OK" OnClientClick="CloseSelectReportDialog(true); return false;" />&nbsp
          <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="CloseSelectReportDialog(false); return false;" />
        </div>
      </div>
      <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript" language="javascript">
          function CloseSelectReportDialog(accepted) {
            var wnd = $find("<%=wndSelectReportDialog.ClientID%>");
            if (accepted) {
              var reportID = $find('<%= wndSelectReportDialog.ContentContainer.FindControl("cmbReport").ClientID %>').get_value();
              var isLeft = $get('<%= wndSelectReportDialog.ContentContainer.FindControl("rbLeft").ClientID %>').checked;
              if (reportID != null && reportID != '') {
                PageMethods.AddPortlet(reportID, isLeft ? 0 : 1, function () { loadPortlets(); });
                top.Ts.System.logAction('Dashboard - Report Added');
              }
            
            }
          
            wnd.close();
          }
        </script>

      </telerik:RadCodeBlock>
    </ContentTemplate>
  </telerik:RadWindow>
  </form>
</body>
</html>
