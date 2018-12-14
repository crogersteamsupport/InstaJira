


var windowFocus = true;
var username;
var originalTitle;
var blinkOrder = 0;

var chatboxFocus = new Array();
var newMessages = new Array();
var newMessagesWin = new Array();
var chatBoxes = new Array();

$(document).ready(function(){
	originalTitle = document.title;

//	$([window, document]).blur(function(){
//		windowFocus = false;
//	}).focus(function(){
//		windowFocus = true;
//		document.title = originalTitle;
//	});
});

function restructureChatBoxes() {
    align = 0;

    for (x=0;x<chatBoxes.length;x++)
    {
		chatboxtitle = chatBoxes[x];

		if ($("#chatbox_"+chatboxtitle).css('display') != 'none') {
			if (align == 0) {
				$("#chatbox_"+chatboxtitle).css('right', '20px');
			} else {
				width = (align)*(225+7)+20;
				$("#chatbox_"+chatboxtitle).css('right', width+'px');
			}
			align++;
		}
	}
}

function chatWith(name, chatuserID) {
    createChatBox(chatuserID, name);
    //$("#chatbox_" + chatuserID + " .boxsizingBorder").focus();
    maximizeChatBox(chatuserID);
    restructureChatBoxes();
}

function chatAddMsg(chatboxtitle, message, chatname) {
    //var windowid = chatboxtitle.replace(/ /g, "");

    message = message.replace(/\n/g, "<br>");

    if (chatname == "system")
        $("#chatbox_" + chatboxtitle + " .chatboxcontent").append('<div class="chatboxmessage"><span class="chatboxmessagecontent systemMsg">' + message + '</span></div>');
    else
        $("#chatbox_" + chatboxtitle + " .chatboxcontent").append('<div class="chatboxmessage"><span class="chatboxmessageto">' + chatname + '</span>:<span class="chatboxmessagecontent">' + message + '</span></div>');

    $("#chatbox_" + chatboxtitle + " .chatboxcontent").scrollTop($("#chatbox_" + chatboxtitle + " .chatboxcontent")[0].scrollHeight);

}

function createChatBox(chatboxtitle,name) {
	if ($("#chatbox_"+chatboxtitle).length > 0) {
		if ($("#chatbox_"+chatboxtitle).css('display') == 'none') {
			$("#chatbox_"+chatboxtitle).css('display','block');
			restructureChatBoxes();
		}
        //$("#chatbox_" + chatboxtitle + " .boxsizingBorder").focus();
		return;
	}

	$(" <div />" ).attr("id","chatbox_"+chatboxtitle)
	.addClass("chatbox")
	.html('<div class="chatboxhead"><div class="chatboxtitle">' + name + '</div><div class="chatboxoptions"><a href="#" onclick="toggleChatBoxGrowth(\'' + chatboxtitle + '\')">-</a> <a href="#" onclick="closeChatBox(\'' + chatboxtitle + '\')">X</a></div><br clear="all"/></div><div class="chatboxcontent"></div><div class="chatboxinput"><textarea class="boxsizingBorder" onkeydown="return checkChatBoxInputKey(event,this,\'' + chatboxtitle + '\');"></textarea></div>')
    .data('Chatid', chatboxtitle)
    .data('Name', name)
	.appendTo($( "body" ));
			   
	$("#chatbox_"+chatboxtitle).css('bottom', '0px');
	
	var chatBoxeslength = 0;


	    for (x=0;x<chatBoxes.length;x++){
	        var test = $("#chatbox_" + chatBoxes[x]).css('display');
	        if (test != 'none') {
	            chatBoxeslength++;
	        }
	    }

	if (chatBoxeslength == 0) {
		$("#chatbox_"+chatboxtitle).css('right', '20px');
	} else {
		width = (chatBoxeslength)*(225+7)+20;
		$("#chatbox_"+chatboxtitle).css('right', width+'px');
	}
	
	chatBoxes.push(chatboxtitle);

//	if (minimizeChatBox == 1) {
//		minimizedChatBoxes = new Array();

//		if ($.cookie('chatbox_minimized')) {
//			minimizedChatBoxes = $.cookie('chatbox_minimized').split(/\|/);
//		}
//		minimize = 0;
//		for (j=0;j<minimizedChatBoxes.length;j++) {
//			if (minimizedChatBoxes[j] == chatboxtitle) {
//				minimize = 1;
//			}
//		}

//		if (minimize == 1) {
//			$('#chatbox_'+chatboxtitle+' .chatboxcontent').css('display','none');
//			$('#chatbox_'+chatboxtitle+' .chatboxinput').css('display','none');
//		}
//	}

	chatboxFocus[chatboxtitle] = false;

	$("#chatbox_" + chatboxtitle + " .boxsizingBorder").blur(function () {
		chatboxFocus[chatboxtitle] = false;
		$("#chatbox_" + chatboxtitle + " .boxsizingBorder").removeClass('chatboxtextareaselected');
	}).focus(function(){
		chatboxFocus[chatboxtitle] = true;
		newMessages[chatboxtitle] = false;
		$('#chatbox_'+chatboxtitle+' .chatboxhead').removeClass('chatboxblink');
		$("#chatbox_" + chatboxtitle + " .boxsizingBorder").addClass('chatboxtextareaselected');
	});

	$("#chatbox_"+chatboxtitle).click(function() {
		if ($('#chatbox_'+chatboxtitle+' .chatboxcontent').css('display') != 'none') {
		    $("#chatbox_" + chatboxtitle + " .boxsizingBorder").focus();
		}
	});

	$("#chatbox_"+chatboxtitle).show();
}

function closeChatBox(chatboxtitle) {
	$('#chatbox_'+chatboxtitle).css('display','none');
	restructureChatBoxes();
}

function maximizeChatBox(chatboxtitle) {
    if ($('#chatbox_' + chatboxtitle + ' .chatboxcontent').css('display') == 'none') {

        var minimizedChatBoxes = new Array();

        $('#chatbox_' + chatboxtitle + ' .chatboxcontent').css('display', 'block');
        $('#chatbox_' + chatboxtitle + ' .chatboxinput').css('display', 'block');
        $("#chatbox_" + chatboxtitle + " .chatboxcontent").scrollTop($("#chatbox_" + chatboxtitle + " .chatboxcontent")[0].scrollHeight);
    }
}

function toggleChatBoxGrowth(chatboxtitle) {
	if ($('#chatbox_'+chatboxtitle+' .chatboxcontent').css('display') == 'none') {  
		
		var minimizedChatBoxes = new Array();
		
		$('#chatbox_'+chatboxtitle+' .chatboxcontent').css('display','block');
		$('#chatbox_'+chatboxtitle+' .chatboxinput').css('display','block');
		$("#chatbox_"+chatboxtitle+" .chatboxcontent").scrollTop($("#chatbox_"+chatboxtitle+" .chatboxcontent")[0].scrollHeight);
	} else {
		
		$('#chatbox_'+chatboxtitle+' .chatboxcontent').css('display','none');
		$('#chatbox_'+chatboxtitle+' .chatboxinput').css('display','none');
	}
	
}

function checkChatBoxInputKey(event,chatboxtextarea,chatboxtitle) {
	 
	if(event.keyCode == 13 && event.shiftKey == 0)  {
		message = $(chatboxtextarea).val();
		message = message.replace(/^\s+|\s+$/g,"");


		if (message != '') {
		    mainFrame.Ts.Services.Dispatch.SendChat(message, chatboxtitle, top.Ts.System.User.FirstName + ' ' + top.Ts.System.User.LastName);
		    message = message.replace(/\n/g, "<br>");
		    message = message.replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/\"/g, "&quot;");
		    $("#chatbox_" + chatboxtitle + " .chatboxcontent").append('<div class="chatboxmessage"><span class="chatboxmessageto">' + top.Ts.System.User.FirstName + ' ' + top.Ts.System.User.LastName + '</span>:<span class="chatboxmessagecontent">' + message + '</span></div>');
		    $("#chatbox_" + chatboxtitle + " .chatboxcontent").scrollTop($("#chatbox_" + chatboxtitle + " .chatboxcontent")[0].scrollHeight);
		}
		$(chatboxtextarea).val('');
		$(chatboxtextarea).focus();
		$(chatboxtextarea).css('height', '60px');
		return false;
	}

	var adjustedHeight = chatboxtextarea.clientHeight;
	var maxHeight = 94;

	if (maxHeight > adjustedHeight) {
		adjustedHeight = Math.max(chatboxtextarea.scrollHeight, adjustedHeight);
		if (maxHeight)
			adjustedHeight = Math.min(maxHeight, adjustedHeight);
		if (adjustedHeight > chatboxtextarea.clientHeight)
			$(chatboxtextarea).css('height',adjustedHeight+8 +'px');
	} else {
		$(chatboxtextarea).css('overflow','auto');
	}
	 
}

