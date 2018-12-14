
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="User.aspx.cs" Inherits="Tips_User" EnableViewState="false"  %>

 <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

 <html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <title></title>
        <style>
            #tipCompany,
            #tipName,
            #tipTitle {
                font-size:14px;
            }
            .tip-customer-expired {
                color: Red;
            }
            .ui-cluetip-content dd {
                width: 60%;
            }
            .ui-cluetip-content dt {
                width: 35%;
            }
            .title {
                line-height: 1.5em;
                display: block;
                font-weight: bold;
            }
            .ticket-tip-number {
                display: inline-block;
                width: 50px;
            }
            .ticket-tip-status {
                display: inline-block;
                width: 100px;
            }
            .ticket-tip-name {
                display: inline;
                width: 100px;
            }
            .red {
                color: red;
            }
        </style>
    </head>

    <body>
        <div>
            <div><a id="tipName" runat="server" href="#"></a></div>
            <div><span id="tipTitle" runat="server"></span><a id="tipCompany" runat="server" href="#"></a></div>
            <div class="ui-widget-content ts-separator"></div>
            <dl id="tipProps" runat="server">
                <dt>Email</dt>
                <dd><a href="mailto:kevin@kevinjones.ws">kevin@kevinjones.ws</a></dd>
                <dt>Mobile</dt>
                <dd>682-583-8509</dd>
                <dt>Office</dt>
                <dd>817-886-0797</dd>
            </dl>
            <div class="ui-widget-content ts-separator"></div>
            <span class="title">Recent Ticket History</span>
            <div id="tipRecent" runat="server">
                <div><span class="ticket-item">#1234</span><span class="ticket-item">Closed</span><span class="ticket-item">Ticket name blah blah</span></div>
                <div><span class="ticket-item">#1234</span><span class="ticket-item">Closed</span><span class="ticket-item">Ticket name blah blah</span></div>
                <div><span class="ticket-item">#1234</span><span class="ticket-item">Closed</span><span class="ticket-item">Ticket name blah blah</span></div>
                <div><span class="ticket-item">#1234</span><span class="ticket-item">Closed</span><span class="ticket-item">Ticket name blah blah</span></div>
                <div><span class="ticket-item">#1234</span><span class="ticket-item">Closed</span><span class="ticket-item">Ticket name blah blah</span></div>
            </div>
            <div class="ui-widget-content ts-separator"></div>
            <span class="title">Notes</span>
            <div id="tipNotes" runat="server">
            </div>
            <div class="ui-helper-clearfix"></div>
        </div>
    </body>

</html>
