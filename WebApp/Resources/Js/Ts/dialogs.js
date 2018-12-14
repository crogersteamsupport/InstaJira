function GetRadWindow()
{
    if (window.radWindow) {
        return window.radWindow;
    }
    if (window.frameElement && window.frameElement.radWindow) {
        return window.frameElement.radWindow;
    }
    return null;
}  

function SizeToFit(newWidth)
{
    window.setTimeout(
    function()
    {
        var oWnd = GetRadWindow();
        var w = document.body.scrollWidth + 4;
        var h = document.body.scrollHeight + 70;
        if (h > 600) h = 600;
        oWnd.SetWidth(newWidth);
        oWnd.SetHeight(h);
        //window.setTimeout(function() {GetRadWindow().center();}, 500);
        //oWnd.center();
        
    }, 200);
}

function GetNewTicketDialog(ticketTypeID)
{
   var manager = GetRadWindowManager();
   var wnd = manager.getWindowByName('wndNewTicket');
   wnd.setUrl('Dialogs/NewTicket.aspx?TicketTypeID=' + ticketTypeID);
   wnd.setSize(800, 500);
   return wnd;
}

function ShowHelpDialog(topicID) {
  alert('Unable to locate help page.');
  return;
  var manager = GetRadWindowManager();
  var wnd = manager.getWindowByName('wndHelp');
  wnd.setUrl('http://www.teamsupport.com/help/topics/IDH_Topic'+topicID+'.htm');
  wnd.setSize(800, 500);
  wnd.show();
  return wnd;
}
function GetAccountDialog(organizationID) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndAccount');
    wnd.setUrl('Dialogs/Account.aspx?OrganizationID=' + organizationID);
    wnd.setSize(800, 500);
    return wnd;
}

function GetSelectOrganizationDialog(refType, refID1, refID2)
{
   var manager = GetRadWindowManager();
   var wnd = manager.getWindowByName('wndSelectOrganization');
   var url = 'Dialogs/SelectOrganization.aspx?RefType=' + refType + '&RefID1=' + refID1;
   if (refID2 != null || refID2 > -1) url = url + '&RefID2=' + refID2;
   wnd.setUrl(url);
   wnd.setSize(400, 300);
   return wnd;
}

function GetActionDialog(ticketID, actionID)
{
   var manager = GetRadWindowManager();
   var wnd = manager.getWindowByName('wndAction');
   var url = 'Dialogs/Action.aspx?TicketID=' + ticketID;
   if (actionID) url = url + '&ActionID=' + actionID;
   wnd.setUrl(url);
   wnd.setSize(800, 400);
   return wnd;
}


function GetReportEditorDialog(reportID) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndReportEditor');
    var url = 'ReportEditor/ReportEditor.aspx';
    if (reportID) url = url + '?ReportID=' + reportID;
    wnd.setUrl(url);
    wnd.setSize(500, 400);
    return wnd;
}

function GetHistoryDialog(refType, refID)
{
   var manager = GetRadWindowManager();
   var wnd = manager.getWindowByName('wndAction');
   var url = 'Dialogs/History.aspx?IsPrompt=0&RefType=' + refType + '&RefID=' + refID;
   wnd.setUrl(url);
   wnd.setSize(800, 500);
   return wnd;

}

function GetAddressDialog(id, refType) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndAddress');
    var url = 'Dialogs/Address.aspx?';
    if (refType == null)
    {
      url = url + 'AddressID=' + id;
    }
    else    
    {
      url = url + 'RefType=' + refType + '&RefID=' + id;
    }
    wnd.setSize(380, 360);
    
    wnd.setUrl(url);
    return wnd;
}

function GetPhoneDialog(id, refType) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndPhone');
    var url = 'Dialogs/PhoneNumber.aspx?';
    if (refType == null) {
        url = url + 'PhoneID=' + id;
    }
    else {
        url = url + 'RefType=' + refType + '&RefID=' + id;
    }
    wnd.setSize(320, 200);

    wnd.setUrl(url);
    return wnd;
}

function GetUserDialog(organizationID, userID) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndUser');
    var url = 'Dialogs/User.aspx?OrganizationID=' + organizationID;
    if (userID != null) {
        url = url + '&UserID=' + userID;
    }
    wnd.setSize(850, 400);

    wnd.setUrl(url);
    return wnd;
}

function GetContactDialog(organizationID, userID) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndContact');
    var url = 'Dialogs/Contact.aspx?OrganizationID=' + organizationID;
    if (userID != null) {
        url = url + '&UserID=' + userID;
    }
    wnd.setSize(800, 350);

    wnd.setUrl(url);
    return wnd;
}

function GetOrganizationDialog(organizationID) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndOrganization');
    var url = 'Dialogs/Organization.aspx';
    if (organizationID != null) {
        url = url + '?OrganizationID=' + organizationID;
    }
    wnd.setSize(800, 500);

    wnd.setUrl(url);
    return wnd;
}

function GetMyCompanyDialog(organizationID) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndMyCompany');
    var url = 'Dialogs/MyCompany.aspx';
    if (organizationID != null) {
        url = url + '?OrganizationID=' + organizationID;
    }
    wnd.setSize(820, 500);

    wnd.setUrl(url);
    return wnd;
}



function GetChatPropertiesDialog(organizationID) {
  var manager = GetRadWindowManager();
  var wnd = manager.getWindowByName('wndChatProperties');
  var url = 'Dialogs/ChatProperties.aspx';
  if (organizationID != null) {
    url = url + '?OrganizationID=' + organizationID;
  }
  wnd.setSize(400, 400);

  wnd.setUrl(url);
  return wnd;
}

function GetProductDialog(isNew, productID) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndProduct');
    var url = 'Dialogs/Product.aspx';
    if (!isNew) {
        url = url + '?ProductID=' + productID;
    }
    wnd.setSize(800, 350);

    wnd.setUrl(url);
    return wnd;
}

function GetSlaLevelDialog(isNew, slaLevelID) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndSlaLevel');
    var url = 'Dialogs/SlaLevel.aspx';
    var isCloning = isNew && slaLevelID != null && slaLevelID > 0;

    if (!isNew) {
        url = url + '?SlaLevelID=' + slaLevelID;
    }

    if (isCloning) {
        url = url + '?SlaLevelID=' + slaLevelID + '&IsCloning=true';
    }

    wnd.setSize(300, 200);

    wnd.setUrl(url);
    return wnd;
}

function GetSlaTriggerDialog(isNew, id, ticketTypeID) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndSlaTrigger');
    var url = 'Dialogs/SlaTrigger.aspx';
    if (isNew) {
        url = url + '?SlaLevelID=' + id + '&TicketTypeID=' + ticketTypeID;
    }
    else {
        url = url + '?SlaTriggerID=' + id;
    }
    wnd.setSize(590, 650);

    wnd.setUrl(url);
    return wnd;
}

function GetVersionDialog(isNew, id) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndVersion');
    var url = 'Dialogs/Version.aspx';
    if (isNew) {
        url = url + '?ProductID=' + id;
    }
    else {
        url = url + '?VersionID=' + id;
    }
    wnd.setSize(800, 500);

    wnd.setUrl(url);
    return wnd;
}

function GetOrganizationProductDialog(organizationProductID, organizationID, productID, versionID) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndOrganizationProduct');
    var url = 'Dialogs/OrganizationProduct.aspx';
    var seperator = '?';



    if (organizationProductID == null) {
        if (organizationID != null) {
            url = url + '?OrganizationID=' + organizationID;
            seperator = '&';
        }
        
        if (productID != null) {
            url = url + seperator + 'ProductID=' + productID;

            if (versionID != null) {
                url = url + '&VersionID=' + versionID;
            }
        }
    }
    else {
        url = url + '?OrganizationProductID=' + organizationProductID;
    }    
    
    wnd.setSize(800, 350);
    wnd.setUrl(url);
    return wnd;
}


function GetSelectGroupDialog(refID, refType) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndSelectGroup');
    var url = 'Dialogs/SelectGroup.aspx?RefType=' + refType + '&RefID=' + refID;
    wnd.setSize(500, 300);

    wnd.setUrl(url);
    return wnd;
}

function GetAttachFileDialog(refID, refType) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndAttachFile');
    var url = 'Dialogs/AttachFile.aspx?RefType=' + refType + '&RefID=' + refID;
    wnd.setSize(500, 300);

    wnd.setUrl(url);
    return wnd;
}


function GetNoteDialog(noteID, refID, refType) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndNote');
    var url = 'Dialogs/Note.aspx'
    if (noteID == null) {
      url = url + '?RefType=' + refType + '&RefID=' + refID;
    }
    else {
        url = url + '?NoteID=' + noteID;
    }
    
    wnd.setSize(800, 500);

    wnd.setUrl(url);
    return wnd;
}


function GetGroupDialog(groupID) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndGroup');
    var url = 'Dialogs/Group.aspx';
    if (groupID != null) {
        url = url + '?GroupID=' + groupID;
    }
    wnd.setSize(350, 350);

    wnd.setUrl(url);
    return wnd;
}

function GetSelectUserDialog(refID, refType) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndSelectUser');
    var url = 'Dialogs/SelectUser.aspx?RefType=' + refType + '&RefID=' + refID;
    wnd.setSize(500, 300);

    wnd.setUrl(url);
    return wnd;
}

function GetCustomFieldDialog(customFieldID, refType, auxID, catID) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndCustomField');
    var url = 'Dialogs/CustomField.aspx?'

    if (customFieldID != null) {
        url = url + 'CustomFieldID=' + customFieldID + '&';
    }
    
    if (auxID != null) {
      url = url + 'RefType=' + refType + '&AuxID=' + auxID;
    }
    else {
        url = url + 'RefType=' + refType;
      }

    if (catID != null) {
      url = url + '&CatID=' + catID;
    }

    wnd.setSize(400, 500);

    wnd.setUrl(url);
    return wnd;

}

function GetProfileImageDialog(organizationID, userID) {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndUser');
    var url = 'Dialogs/ProfileImage.aspx?OrganizationID=' + organizationID;
    if (userID != null) {
        url = url + '&UserID=' + userID;
    }
    wnd.setSize(500, 600);

    wnd.setUrl(url);
    return wnd;
}

function GetPasteImageDialog() {
    var manager = GetRadWindowManager();
    var wnd = manager.getWindowByName('wndImagePaste');
    var url = 'Dialogs/ImagePaste.aspx';

    wnd.setSize(800, 400);
    wnd.setUrl(url);
    return wnd;
}