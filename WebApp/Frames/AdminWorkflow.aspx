<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="AdminWorkflow.aspx.cs" Inherits="Frames_AdminWorkflow" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <style type="text/css">
    body{background:#fff;}
  </style>
	<script language="javascript" type="text/javascript">
		function OnClientSelectedTicketTypeIndexChanged(sender, eventArgs) {
			document.getElementById('<%= SelectedTicketTypeIndexHidden.ClientID %>').value = sender._selectedIndex;
		}

		function OnClientSelectedTicketStatusIndexChanged(sender, eventArgs) {
			document.getElementById('<%= SelectedTicketStatusIndexHidden.ClientID %>').value = sender._selectedIndex;
		}

		function MoveUp(id) {
			PageMethods.MoveUp(id, function(result) {
				if (result) {
					__doPostBack('<%= SelectedTicketTypeIndexHidden.ClientID %>', '');
				}
			});
		}

		function MoveDown(id) {
			PageMethods.MoveDown(id, function(result) {
				if (result) {
					__doPostBack('<%= SelectedTicketTypeIndexHidden.ClientID %>', '');
				}
			});
		}

		function Delete(id) {
			if (confirm('Are you sure you would like to delete this next ticket status?')) {
				PageMethods.Delete(id, function(result) {
					if (result) {
						__doPostBack('<%= SelectedTicketTypeIndexHidden.ClientID %>', '');
					}
				});
			}
		}
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline">
    <AjaxSettings>
      <telerik:AjaxSetting AjaxControlID="pnlMain">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="pnlMain" />
        </UpdatedControls>
      </telerik:AjaxSetting>
    </AjaxSettings>
  </telerik:RadAjaxManager>
<telerik:RadAjaxLoadingPanel ID="lpType" runat="server" Height="15px" Width="75px" IsSticky="true">
  <img alt="Loading..." src='<%= RadAjaxLoadingPanel.GetWebResourceUrl(Page, "Telerik.Web.UI.Skins.Default.Ajax.loading3.gif") %>' style="border: 0px; margin-left: 10px;" />
</telerik:RadAjaxLoadingPanel>
<div id="pnlMain" runat="server" style="background-color:#ffffff; height:100%; overflow:auto;">
    
    
    
    
  <div style="padding: 10px 0 0 17px; width: 400px;">
    <div style="float: left; padding-right: 20px;">
      <div>
        Ticket Type:</div>
      <div>
        <telerik:RadComboBox ID="cmbTicketTypes" runat="server" AutoPostBack="True" onclientselectedindexchanged="OnClientSelectedTicketTypeIndexChanged">
          <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
        </telerik:RadComboBox>
      </div>
    </div>
    <div style="float: left;">
      <div>
        Ticket Status:</div>
      <div>
        <telerik:RadComboBox ID="cmbStatuses" runat="server" AutoPostBack="True" onclientselectedindexchanged="OnClientSelectedTicketStatusIndexChanged">
          <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
        </telerik:RadComboBox>
      </div>
    </div>
    
  <div style="clear: both; padding-bottom: 10px;"></div>
    <div class="groupDiv groupLightBlue">
      <div class="groupHeaderDiv">
        <span class="groupHeaderSpan"></span>
        <span class="groupCaptionSpan">Ticket Statuses</span><span class="groupButtonSpanWrapper"><span class="groupButtonsSpan">
          <asp:LinkButton ID="lnkAddStatus" runat="server" CssClass="groupButtonLink" OnClientClick="parent.parent.Ts.System.logAction('Admin Workflow - Add Status Dialog Opened');">
              <span class="groupButtonSpan">
                <img alt="" src="../images/icons/add.png" class="groupButtonImage" />
                <span class="groupButtonTextSpan">Add Status</span> </span></asp:LinkButton>
        </span>
        </span>
      </div>
      <div class="groupBodyWrapperDiv">
        <div class="groupBodyDiv">
          <telerik:RadGrid ID="gridNext" runat="server" Width="100%" Height="100%" AutoGenerateColumns="False" GridLines="None" OnItemCommand="gridNext_ItemCommand" OnItemDataBound="gridNext_ItemDataBound" BorderWidth="0px" ShowHeader="False">
            <MasterTableView DataKeyNames="TicketNextStatusID" ClientDataKeyNames="TicketNextStatusID">
              <RowIndicatorColumn>
                <HeaderStyle Width="20px"></HeaderStyle>
              </RowIndicatorColumn>
              <ExpandCollapseColumn>
                <HeaderStyle Width="20px"></HeaderStyle>
              </ExpandCollapseColumn>
              <Columns>
                <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="../images/icons/trash.png" UniqueName="columnDelete" CommandName="Delete" ConfirmText="Are you sure you would like to delete this next ticket status?" ConfirmDialogType="RadWindow">
                  <HeaderStyle BorderWidth="0px" />
                  <ItemStyle Width="16px" />
                </telerik:GridButtonColumn>
                <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="../images/icons/Arrow_Up.png" UniqueName="columnMoveUp" CommandName="MoveUp">
                  <HeaderStyle BorderWidth="0px" />
                  <ItemStyle Width="16px" />
                </telerik:GridButtonColumn>
                <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="../images/icons/Arrow_Down.png" UniqueName="columnMoveDown" CommandName="MoveDown">
                  <HeaderStyle BorderWidth="0px" />
                  <ItemStyle Width="16px" />
                </telerik:GridButtonColumn>
                <telerik:GridBoundColumn DataField="TicketNextStatusID" UniqueName="columnTicketNextStatusID" Visible="False"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Position" UniqueName="Position" Visible="False"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Name" HeaderText="Status" UniqueName="columnName"></telerik:GridBoundColumn>
              </Columns>
              <EditFormSettings EditFormType="WebUserControl" UserControlName="~/UserControls/Admin/NewNextStatus.ascx">
                <EditColumn UniqueName="EditCommandColumn1"></EditColumn>
              </EditFormSettings>
            </MasterTableView>
            <FilterMenu EnableTheming="True">
              <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
            </FilterMenu>
          </telerik:RadGrid>
        </div>
      </div>
    </div>
  </div>
  
  <telerik:RadToolTip ID="tipNewStatus" runat="server" ShowEvent="OnClick" TargetControlID="lnkAddStatus" Animation="Fade" AutoCloseDelay="60000" HideDelay="1000" OffsetY="-17" RelativeTo="Element" ShowCallout="False">
    <telerik:RadComboBox ID="cmbNewStatus" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbNewStatus_SelectedIndexChanged">
      <CollapseAnimation Duration="200" Type="OutQuint" />
    </telerik:RadComboBox>
  </telerik:RadToolTip>
</div>
	<asp:HiddenField ID="SelectedTicketTypeIndexHidden" runat="server" Value="0" />
	<asp:HiddenField ID="SelectedTicketStatusIndexHidden" runat="server" Value="0" />
</asp:Content>

