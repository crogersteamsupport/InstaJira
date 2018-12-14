<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Sla.aspx.cs" Inherits="Tips_Sla" EnableViewState="false"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <title></title>
    </head>

    <body>
        <div>
            <div>
                <span id="tipSla" runat="server" style="float:left;line-height:16px;margin-right:3px;"></span>
                <a id="tipNumber" runat="server" href="#" style="float:left; line-height:16px;"></a>
            </div>
            <div class="ui-helper-clearfix"></div>
            <p id="tipName" runat="server" style="padding-top:.5em;"></p>
            <div class="ui-widget-content ts-separator"></div>

            <table cellspacing="0" cellpadding="0" border="0" class="tip-sla-props" width="100%">
              <thead>
                <tr class="ts-color-bg-accent">
                  <th>SLA Name</th>
                  <th colspan="2" id="wslaName" runat="server"></th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>SLA Type</td>
                  <td>Warning</td>
                  <td>Violation</td>
                </tr>
                <tr>
                  <td>Last Action</td>
                  <td id="wLast" runat="server"></td>
                  <td id="vLast" runat="server"></td>
                </tr>
                <tr>
                  <td>Time To Close</td>
                  <td id="wClose" runat="server"></td>
                  <td id="vClose" runat="server"></td>
                </tr>
                <tr>
                  <td>Initial Response</td>
                  <td id="wInit" runat="server"></td>
                  <td id="vInit" runat="server"></td>
                </tr>
              </tbody>
              <tfoot style="font-weight:bold;">
                <tr>
                  <td>Soonest Date</td>
                  <td id="wNext" runat="server"></td>
                  <td id="vNext" runat="server"></td>
                </tr>
              </tfoot>
            </table>
            <dl id="tipProps" runat="server"></dl>
            <div class="ui-helper-clearfix"></div>
            </div>
    </body>
</html>
