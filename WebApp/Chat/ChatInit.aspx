<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChatInit.aspx.cs" Inherits="Chat_ChatInit" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title></title>
    <link href="/frontend/library/fontawesome-5.3.1/css/all.min.css?1540232401" rel="stylesheet" />
    <link href="/frontend/css/core/flexbox.css?1540232401" rel="stylesheet" />
    <link href="/frontend/css/features/customerchat-init.css?1540232401" rel="stylesheet" />
    <script src="https://js.pusher.com/3.1/pusher.min.js"></script>
    <script src="/frontend/library/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="/frontend/library/jquery.placeholder.js" type="text/javascript"></script>
    <script src="../vcr/1_9_0/Js/Ts/ts.utils.js"></script>

    <!-- HANDLEBARS. -->
    <script src="/frontend/library/handlebars/handlebars.runtime-v4.0.12.js"></script>
    <script src="/frontend/handlebars/chatinit.js?1540232401" type="text/javascript"></script>

    <script src="/frontend/javascript/features/customerchat-init.js?1540232401"></script>
</head>

<body>

    <div style="max-width:100%;width:100%;">

        <div class="flexbox column">
            <div cla="flex" style="padding:5px 10px;background-color:#2e3f52;color:white;">
                <div>Welcome to our live chat!</div>
            </div>
            <div cla="flex">
                <div class="alert alert-info chatOfflineWarning" role="alert" style="display:none;">Our live chat is not available at this time. Please submit a ticket request in the form below, and a member of our team will follow up with you as
                    soon as possible. Thank you for your patience.</div>
            </div>
        </div>

        <form id="newChatForm" class="container" runat="server">

            <asp:ScriptManager ID="ScriptManager1" runat="server" ScriptMode="Release" EnableScriptGlobalization="True">
                <services>
                    <asp:ServiceReference Path="~/Services/DeflectorService.asmx" />
                </services>
            </asp:ScriptManager>

            <div class="flexbox" style="max-width:100%;width:100%;">
                <div class="flex push">
                    <div class="flexbox column" style="max-width:100%;width:100%;">
                        <div class="flex">
                            <div style="padding:10px;">
                                <input type="text" id="userFirstName" placeholder="First Name" style="width:100%;">
                            </div>
                        </div>
                        <div class="flex">
                            <div style="padding:10px;">
                                <input type="text" id="userLastName" placeholder="Last Name" style="width:100%;">
                            </div>
                        </div>
                        <div class="flex">
                            <div style="padding:10px;">
                                <input type="text" id="userEmail" placeholder="Email Address" style="width:100%;">
                            </div>
                        </div>
                    </div>
                </div>
                <div class="flex basis120">
                    <div class="chat-logo pull-right"></div>
                </div>
            </div>

            <div class="flexbox column" style="width:100%;">
                <div class="flex">
                    <div style="padding:10px;">
                        <textarea id="userIssue" rows="5" placeholder="How can we help you?" style="width:100%;"></textarea>
                    </div>
                </div>
            </div>

            <div id="deflection-box" style="width:100%;">
                <div class="flexbox align" style="margin:0px 10px 2px;">
                    <div class="flex pull" style="padding:0px 5px 5px;vertical-align:top;"><i class="fas fa-exclamation-triangle" style="font-size:12px;"></i></div>
                    <div class="flex push" style="font-size:12px;">Would one of these articles help you?</div>
                </div>
                <div>
                    <div id="deflection-results"></div>
                </div>
            </div>

            <div class="flexbox column" style="max-width:100%;">
                <div class="flex">
                    <div style="text-align:right;padding:15px;">
                        <button type="submit">SUBMIT</button>
                    </div>
                </div>
            </div>

        </form>

    </div>

</body>

</html>
