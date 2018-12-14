<%@ Page Title="SLA Trigger" Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true" CodeFile="SlaTrigger.aspx.cs" Inherits="Dialogs_SlaTrigger" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <style type="text/css">
    .col1{ text-align:right; padding-right: 5px;}

    .HiddenPicker td
    {
	    padding:0 !important;
    }

    .HiddenTextBox
    {
	    width:1px !important;
	    border:0 !important;
	    margin:0 !important;
	    background:none transparent !important;
	    visibility:hidden !important;
        display: none;
    }

    .SlaStartEnd
    {
        width: 40% !important;
    }

    .daysToPause
    {
        vertical-align: top !important;
    }

    .holidaysTip
    {
        font-style: italic;
        font-size: smaller;
    }

    .RadPicker .rcCalPopup
    {
        margin-left: 5px;
    }

    .deleteDates
    {
        vertical-align: bottom;
    }
  </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">
      <table cellpadding="0" cellspacing="5" border="0">
        <tr>
          <td class="col1">SLA Name:</td><td><asp:Label ID="lblSla" runat="server" Text="Label" Font-Bold="true"></asp:Label></td>
        </tr>
        <tr><td class="col1">Ticket Type:</td><td><asp:Label ID="lblTicketType" runat="server" Text="Label" Font-Bold="true"></asp:Label></td></tr>
        <tr><td class="col1">Ticket Severity:</td><td colspan="3"><telerik:RadComboBox ID="cmbSeverities" runat="server"></telerik:RadComboBox></td></tr>
        <tr><td class="col1">Warning Time Before Violation:</td><td>
          <telerik:RadNumericTextBox ID="numWarning" Width="50px" runat="server" 
            DataType="System.Int32" MinValue="0" Value="0">
            <NumberFormat DecimalDigits="0" />
          </telerik:RadNumericTextBox>
            <asp:RadioButton ID="rbWarningMin" runat="server" Text="Minutes" GroupName="WarningTime" />
            <asp:RadioButton ID="rbWarningHour" runat="server" Text="Hours" GroupName="WarningTime" Checked="true"/>
            <asp:RadioButton ID="rbWarningDay" runat="server" Text="Days" GroupName="WarningTime"/></td></tr>

        <tr><td colspan="4"><div style="border-bottom: solid 1px; font-size: 16px; margin-bottom: 7px; padding-top: 6px;">Pause</div></td></tr>    
        <tr>
            <td class="col1">Pause on Company Holidays<br /><span class="holidaysTip">(Holidays are defined in the Calendar)</span></td>
            <td><asp:CheckBox ID="cbPauseOnOrganizationHolidays" runat="server" /></td>
        </tr>
        <tr>
            <td class="col1" style="vertical-align: top;">Pause on Specific Dates</td>
        </tr>
        <tr>
            <td class="col1">
                <asp:ListBox ID="daysToPauseList" runat="server" Rows="6" Width="100px" SelectionMode="Multiple" CssClass="daysToPause"></asp:ListBox>
                <input type="hidden" runat="server" id="DaysToPauseHidden" name="DaysToPauseHidden" />
                <telerik:RadDatePicker ID="PauseOnDates" runat="server" Width="0" CssClass="HiddenPicker">
                    <ClientEvents OnDateSelected="AddToList" />
                    <DateInput runat="server" CssClass="HiddenTextBox" />
                </telerik:RadDatePicker>
            </td>
            <td class="deleteDates">
                <img src="../images/icons/Trash.png" title="Delete Selected Dates" alt="Delete Selected Dates" onclick="RemoveDates();" />
            </td>
        </tr>

        <tr><td colspan="4"><div style="border-bottom: solid 1px; font-size: 16px; margin-bottom: 7px; padding-top: 6px;">Business Hours</div></td></tr>    
          <tr>
            <td class="col1">No Business Hours:</td>
            <td><asp:RadioButton ID="rbNoBusinessHours" runat="server" Text="" GroupName="BusinessHours" OnClick="DisableSlaDaysAndHours()" /></td>
        </tr>
        <tr>
            <td class="col1">Use Account Business Hours:</td>
            <td><asp:RadioButton ID="rbBusinessHours" runat="server" Text="" GroupName="BusinessHours" OnClick="DisableSlaDaysAndHours()" /></td>
        </tr>
        <tr>
            <td class="col1">Use Custom Business Hours:</td>
            <td><asp:RadioButton ID="rbCustomBusinessHours" runat="server" Text="" GroupName="BusinessHours" OnClick="DisableSlaDaysAndHours()" /></td>
        </tr>
        <tr>
            <td class="col1">SLA Start:</td>
            <td>
                <telerik:RadTimePicker ID="timeSLAStart" runat="server" Width="90px">
                </telerik:RadTimePicker>
            </td>
        </tr>
        <tr>
            <td class="col1">SLA End:</td>
            <td>
                <telerik:RadTimePicker ID="timeSLAEnd" runat="server" Width="90px">
                </telerik:RadTimePicker>
            </td>
        </tr>
        <tr>
            <td class="col1">Time Zone:</td>
            <td>
                <telerik:RadComboBox ID="cbTimeZones" runat="server" Width="200px"></telerik:RadComboBox>
            </td>
        </tr>
            
        <tr>
            <td class="col1">SLA Days:</td>
            <td colspan="4">
                <asp:CheckBox ID="cbSLAMonday" runat="server" Text="Monday" />
                <asp:CheckBox ID="cbSLATuesday" runat="server" Text="Tuesday" />
                <asp:CheckBox ID="cbSLAWednesday" runat="server" Text="Wednesday" />
                <asp:CheckBox ID="cbSLAThursday" runat="server" Text="Thursday" />
            </td>
        </tr>
        <tr>
            <td class="col1"></td>
            <td>
                <asp:CheckBox ID="cbSLAFriday" runat="server" Text="Friday" />
                <asp:CheckBox ID="cbSLASaturday" runat="server" Text="Saturday" />
                <asp:CheckBox ID="cbSLASunday" runat="server" Text="Sunday" />
            </td>
        </tr>

        <tr><td colspan="4"><div style="border-bottom: solid 1px; font-size: 16px; margin-bottom: 7px; padding-top: 6px;">Violation Times</div></td></tr>    
        
        <tr><td class="col1">Time Since Initial Response:</td><td>
          <telerik:RadNumericTextBox ID="numInitial" Width="50px" runat="server" 
            DataType="System.Int32" MinValue="0" Value="0">
            <NumberFormat DecimalDigits="0" />
          </telerik:RadNumericTextBox>
            <asp:RadioButton ID="rbInitialMin" runat="server" Text="Minutes" GroupName="IntialTime" />
            <asp:RadioButton ID="rbInitialHour" runat="server" Text="Hours" GroupName="IntialTime" Checked="true"/>
            <asp:RadioButton ID="rbInitialDay" runat="server" Text="Days" GroupName="IntialTime"/></td></tr>
        <tr><td class="col1">Time Since Last Action:</td><td>
          <telerik:RadNumericTextBox ID="numAction" Width="50px" runat="server" 
            DataType="System.Int32" MinValue="0" Value="0">
            <NumberFormat DecimalDigits="0" />
          </telerik:RadNumericTextBox>
            <asp:RadioButton ID="rbActionMin" runat="server" Text="Minutes" GroupName="LastAction" />
            <asp:RadioButton ID="rbActionHour" runat="server" Text="Hours" GroupName="LastAction" Checked="true"/>
            <asp:RadioButton ID="rbActionDay" runat="server" Text="Days" GroupName="LastAction"/></td></tr>
        <tr><td class="col1">Time Until Closed:</td><td>
          <telerik:RadNumericTextBox ID="numClosed" Width="50px" runat="server" 
            DataType="System.Int32" MinValue="0" Value="0">
            <NumberFormat DecimalDigits="0" />
          </telerik:RadNumericTextBox>
            <asp:RadioButton ID="rbClosedMin" runat="server" Text="Minutes" GroupName="TimeClosed" />
            <asp:RadioButton ID="rbClosedHour" runat="server" Text="Hours" GroupName="TimeClosed" Checked="true"/>
            <asp:RadioButton ID="rbClosedDay" runat="server" Text="Days" GroupName="TimeClosed"/></td></tr>
            
        <tr><td colspan="4"><div style="border-bottom: solid 1px; font-size: 16px; margin-bottom: 7px; padding-top: 6px;">Notification Options</div></td></tr>    
            
      </table>
      
      <table cellpadding="0" cellspacing="5" border="0" width="500px">
      <tr>
      <td><asp:CheckBox ID="cbGroupWarnings" runat="server" Text="Notify Group of Warnings" /></td>
      <td><asp:CheckBox ID="cbUserWarnings" runat="server" Text="Notify User of Warnings" /></td>
      </tr>
      <tr>
      <td><asp:CheckBox ID="cbGroupViolations" runat="server" Text="Notify Group of Violations" /></td>
      <td><asp:CheckBox ID="cbUserViolations" runat="server" Text="Notify User of Violations" /></td>
      </tr>
      </table>
      
    </div>
  </div>

    <script type="text/javascript">
        var _reset = false;

        function AddToList()
        {
            if (!_reset) {
                var datePicker = $find("<%=PauseOnDates.ClientID %>");
                var dateText = datePicker.get_dateInput().get_selectedDate().localeFormat(window.parent.Ts.Utils.getDatePattern());
                var dateValue = datePicker.get_dateInput().get_selectedDate().toLocaleDateString();
                var htmlSelect = document.getElementById('<%=daysToPauseList.ClientID%>');
                var option = document.createElement("OPTION");
                option.innerHTML = dateText;
                option.value = dateValue;
                htmlSelect.appendChild(option);
                AddToHidden(datePicker.get_dateInput().get_selectedDate().localeFormat("MM/dd/yyyy"));
            }

            _reset = false;            
        }

        function AddToHidden(dateText) {
            var daysToPauseHidden = document.getElementById('<%=DaysToPauseHidden.ClientID%>');

            if (daysToPauseHidden.value == null || daysToPauseHidden.value == '') {
                daysToPauseHidden.value = dateText;
            } else {
                var daysToPauseHiddenTemp = daysToPauseHidden.value;
                daysToPauseHiddenTemp = daysToPauseHiddenTemp + "," + dateText;
                daysToPauseHidden.value = daysToPauseHiddenTemp;
            }
        }

        function DeleteSelectedItems(event) {
            if (event.keyCode == 46 || event.key == "Delete") {
                RemoveDates();
            }
        }

        function RemoveDates() {
            var htmlSelect = document.getElementById('<%=daysToPauseList.ClientID%>');

            for (var i = (htmlSelect.options.length - 1) ; i >= 0; i--) {
                if (htmlSelect.options[i].selected) {
                    RemoveOfHidden(htmlSelect.options[i].value);
                    var deletedDate = new Date(htmlSelect.options[i].value);
                    htmlSelect.options[i] = null;

                    try {
                        var datePicker = $find("<%=PauseOnDates.ClientID %>");
                        var dateText = datePicker.get_dateInput().get_selectedDate();

                        if (deletedDate.getTime() === dateText.getTime()) {
                            _reset = true;
                            var month = dateText.getMonth();
                            var day = dateText.getDate();
                            day = (day > 1) ? 1 : 2;
                            var year = dateText.getFullYear();
                            var newDate = new Date(year, month, day);
                            datePicker.set_selectedDate(newDate);
                        }
                    } catch (err) {
                    }
                }
            }
        }

        function RemoveOfHidden(dateText) {
            var daysToPauseHidden = document.getElementById('<%=DaysToPauseHidden.ClientID%>');

            if (daysToPauseHidden.value != null && daysToPauseHidden.value !== '') {
                daysToPauseHidden.value = daysToPauseHidden.value.replace(dateText, "");
                daysToPauseHidden.value = daysToPauseHidden.value.replace(",,", ",");
            }
        }

        function DisableSlaDaysAndHours() {
            var noBusinessHours = document.getElementById('<%= rbNoBusinessHours.ClientID %>');
            var cbSLASunday = document.getElementById('<%= cbSLASunday.ClientID %>');
            var cbSLAMonday = document.getElementById('<%= cbSLAMonday.ClientID %>');
            var cbSLATuesday = document.getElementById('<%= cbSLATuesday.ClientID %>');
            var cbSLAWednesday = document.getElementById('<%= cbSLAWednesday.ClientID %>');
            var cbSLAThursday = document.getElementById('<%= cbSLAThursday.ClientID %>');
            var cbSLAFriday = document.getElementById('<%= cbSLAFriday.ClientID %>');
            var cbSLASaturday = document.getElementById('<%= cbSLASaturday.ClientID %>');
            var useOrgBusinessHours = document.getElementById('<%= rbBusinessHours.ClientID %>');
            var timeSLAStart = $find("<%= timeSLAStart.ClientID %>");
            var timeSLAEnd = $find("<%= timeSLAEnd.ClientID %>");
            var timeZones = $find("<%= cbTimeZones.ClientID %>")

            if (noBusinessHours.checked) {
                timeSLAStart.set_enabled(false);
                timeSLAEnd.set_enabled(false);
                timeZones.disable();
                cbSLASunday.disabled = true;
                cbSLAMonday.disabled = true;
                cbSLATuesday.disabled = true;
                cbSLAWednesday.disabled = true;
                cbSLAThursday.disabled = true
                cbSLAFriday.disabled = true;
                cbSLASaturday.disabled = true;
            } else {                
                timeSLAStart.set_enabled(!useOrgBusinessHours.checked);
                timeSLAEnd.set_enabled(!useOrgBusinessHours.checked);

                if (useOrgBusinessHours.checked) {
                    timeZones.disable();
                } else {
                    timeZones.enable();
                }

                cbSLASunday.disabled = useOrgBusinessHours.checked;

                if (cbSLASunday.parentElement.tagName == 'SPAN' && cbSLASunday.parentElement.disabled == true) {
                    cbSLASunday.parentElement.disabled = false;
                }

                cbSLAMonday.disabled = useOrgBusinessHours.checked;

                if (cbSLAMonday.parentElement.tagName == 'SPAN' && cbSLAMonday.parentElement.disabled == true) {
                    cbSLAMonday.parentElement.disabled = false;
                }

                cbSLATuesday.disabled = useOrgBusinessHours.checked;

                if (cbSLATuesday.parentElement.tagName == 'SPAN' && cbSLATuesday.parentElement.disabled == true) {
                    cbSLATuesday.parentElement.disabled = false;
                }

                cbSLAWednesday.disabled = useOrgBusinessHours.checked;

                if (cbSLAWednesday.parentElement.tagName == 'SPAN' && cbSLAWednesday.parentElement.disabled == true) {
                    cbSLAWednesday.parentElement.disabled = false;
                }

                cbSLAThursday.disabled = useOrgBusinessHours.checked;

                if (cbSLAThursday.parentElement.tagName == 'SPAN' && cbSLAThursday.parentElement.disabled == true) {
                    cbSLAThursday.parentElement.disabled = false;
                }

                cbSLAFriday.disabled = useOrgBusinessHours.checked;

                if (cbSLAFriday.parentElement.tagName == 'SPAN' && cbSLAFriday.parentElement.disabled == true) {
                    cbSLAFriday.parentElement.disabled = false;
                }

                cbSLASaturday.disabled = useOrgBusinessHours.checked;

                if (cbSLASaturday.parentElement.tagName == 'SPAN' && cbSLASaturday.parentElement.disabled == true) {
                    cbSLASaturday.parentElement.disabled = false;
                }
            }
        }
    </script>
</asp:Content>