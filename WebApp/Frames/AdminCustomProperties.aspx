<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
    CodeFile="AdminCustomProperties.aspx.cs" Inherits="Frames_AdminCustomProperties" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script src="/frontend/library/ajaxupload.js" type="text/javascript"></script>

    <style type="text/css">
        .types thead {
            background-color: #E2E2E2;
            border-bottom: solid 1px #73A3FE;
            text-align: left;
            white-space: nowrap;
        }

        .types td {
            border-top: solid 1px #A2B8CE;
            border-left: solid 1px #A2B8CE;
        }

        .types img {
            cursor: pointer;
        }

        .types td.headImage {
            border-left: none;
            width: 16px;
        }

        body {
            background: #fff;
            overflow: auto;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div style="height: 100%; overflow: auto;">
        <div style="padding: 10px 0 0 17px; width: 750px;">
            <div style="float: left; padding-right: 20px;">
                <div style="padding-bottom: 5px;">
                    System Property Type:
                </div>
                <div>
                    <telerik:RadComboBox ID="cmbTypes" runat="server" OnClientSelectedIndexChanged="cmbTypes_OnClientSelectedIndexChanged"
                        CollapseAnimation-Type="None" ExpandAnimation-Type="None">
                    </telerik:RadComboBox>
                </div>
            </div>
            <div id="divTicketType" style="float: left; padding-right: 20px; display: none;">
                <div style="padding-bottom: 5px;">
                    Ticket Type:
                </div>
                <div>
                    <telerik:RadComboBox ID="cmbTicketTypes" runat="server" OnClientSelectedIndexChanged="cmbTicketTypes_OnClientSelectedIndexChanged"
                        CollapseAnimation-Type="None" ExpandAnimation-Type="None">
                    </telerik:RadComboBox>
                </div>
            </div>
        </div>
        <div style="clear: both;">
        </div>
        <div style="padding: 10px 0 0 15px;">
            <div class="groupDiv groupLightBlue" style="width: 750px;">
                <div class="groupHeaderDiv">
                    <span class="groupHeaderSpan"></span><span class="groupCaptionSpan" id="spanCaption">Action Types</span> <span class="groupButtonSpanWrapper" id="spanNewType" runat="server">
                        <span class="groupButtonsSpan"><a class="groupButtonLink" href="#" onclick="editType(); return false;">
                            <span class="groupButtonSpan">
                                <img alt="" src="../images/icons/add.png" class="groupButtonImage" />
                                <span id="spanNew" class="groupButtonTextSpan">Add</span> </span></a></span>
                    </span>
                </div>
                <div class="groupBodyWrapperDiv">
                    <div class="groupBodyDiv">
                        <div id="divTypes" class="types">
                        </div>
                    </div>
                </div>
            </div>
            <p>* You may need to refresh your browser before your changes are visible.</p>
            <p style="color: Red; font-weight: bold;">** Important:  If you are making changes to your ticket statuses, please remember you need to visit the Workflow tab to define the workflow.  More specifically, what statuses will be visible as next possible selections based on the current status selected within each ticket.  You can also read more about workflow <a target="_blank" href="http://help.teamsupport.com/1/en/topic/workflow">here</a>.</p>
        </div>
    </div>
    <telerik:RadWindow ID="wndDeleteType" runat="server" Width="350px" Height="250px"
        VisibleStatusbar="False" Title="Delete Type" Behaviors="Close,Move" IconUrl="../images/icons/TeamSupportLogo16.png"
        Modal="True">
        <ContentTemplate>
            <div style="margin: 7px 20px;">
                <div style="color: Red; font-weight: bold; font-size: 18px; padding: 5px 0;">
                    WARNING
                </div>
                <div style="padding-bottom: 3px;">
                    Are you sure you would like to PERMANENTLY delete:
                </div>
                <div style="font-weight: bold; padding-bottom: 20px;" id="itemNameDiv">
                    x
                </div>
                <div id="divDeleteReplace">
                    <div style="padding-bottom: 3px;">
                        Replace existing items with:
                    </div>
                    <div style="padding-bottom: 20px;">
                        <telerik:RadComboBox ID="cmbReplaceTypes" runat="server" Width="200px" ExpandAnimation-Type="None"
                            CollapseAnimation-Type="None">
                        </telerik:RadComboBox>
                    </div>
                </div>
                <div style="float: right; margin-right: 15px;">
                    <asp:Button ID="btnOk" runat="server" Text="OK" OnClientClick="closeDeleteTypeWindow(true); return false;" />&nbsp
          <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="closeDeleteTypeWindow(false); return false;" />
                </div>
            </div>
        </ContentTemplate>
    </telerik:RadWindow>
    <telerik:RadWindow ID="wndEditType" runat="server" Width="450px" Height="350px" Top="100px"
        Left="100px" VisibleStatusbar="False" Title="Type Editor" Behaviors="Close,Move"
        IconUrl="../images/icons/TeamSupportLogo16.png" Modal="True">
        <ContentTemplate>
            <div style="margin: 7px 20px;">
                <div>
                    <div style="height: 265px;">
                        <div>
                            Name:
                        </div>
                        <div style="padding-bottom: 5px;">
                            <telerik:RadTextBox ID="textName" runat="server" Width="200px" Text="Name">
                            </telerik:RadTextBox>
                        </div>
                        <div>
                            Description:
                        </div>
                        <div style="padding-bottom: 5px;">
                            <telerik:RadTextBox ID="textDescription" runat="server" TextMode="MultiLine" Width="100%"
                                Height="50px">
                            </telerik:RadTextBox>
                        </div>
                        <div id="divActionType">
                            <asp:CheckBox ID="cbIsTimed" runat="server" Text="Is Timed" />
                        </div>
                        <div id="divTicketStatus">
                            <asp:CheckBox ID="cbIsClosed" runat="server" Text="Is Closed" />&nbsp&nbsp
              <asp:CheckBox ID="cbIsClosedEmail" runat="server" Text="Send Closed Email" />&nbsp&nbsp
              <asp:CheckBox ID="cbIsEmailResponse" runat="server" Text="Email Response" /><br />
                            <asp:CheckBox ID="cbPauseSla" runat="server" Text="Pause Sla" />
                        </div>
                        <div id="divProductVersionStatus">
                            <asp:CheckBox ID="cbIsShipping" runat="server" Text="Is Shipping" />&nbsp&nbsp<asp:CheckBox
                                ID="cbIsDiscontinued" runat="server" Text="Is Discontinued" />
                        </div>
                        <div id="divTicketTypeProductFamily">
                            <div style="padding-bottom: 3px;">
                                Product Line:
                            </div>
                            <div>
                                <div style="margin-bottom: 10px;">
                                    <telerik:RadComboBox ID="cmbTicketTypeProductFamilies" runat="server" Width="200px" ExpandAnimation-Type="None"
                                        CollapseAnimation-Type="None">
                                    </telerik:RadComboBox>
                                </div>
                            </div>
                        </div>
                        <div id="divTicketTypeIcon">
                            <div style="padding-bottom: 3px;">
                                Icon:
                            </div>
                            <div>
                                <div style="float: left;">
                                    <telerik:RadComboBox ID="cmbTicketTypeIcons" runat="server" Width="200px" ExpandAnimation-Type="None"
                                        CollapseAnimation-Type="None">
                                    </telerik:RadComboBox>
                                </div>
                                <div style="float: left; padding-left: 5px; line-height: 21px;">
                                    <span id="spanUploadIcon" style="cursor: pointer; text-decoration: underline; font-weight: bold;">Upload Icon</span>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 5px; font-size: 0.75em;">
                                * 16x16 PNG image is recommended.
                            </div>
                            <div style="height: 20px;">
                                <div id="divUploadResults" style="padding-top: 5px; font-weight: bold; display: none;">
                                </div>
                            </div>
                        </div>
                        <div id="divVisibleOnPortal">
                            <asp:CheckBox ID="cbIsVisibleOnPortal" runat="server" Text="Visible on Portal" />&nbsp&nbsp
                        </div>
                        <div id="divIsActive">
                            <asp:CheckBox ID="cbIsActive" runat="server" Text="Is Active" />&nbsp&nbsp
                        </div>
                        <div id="divExcludeFromCDI">
                            <asp:CheckBox ID="cbExcludeFromCDI" runat="server" Text="Exclude From CDI" />&nbsp&nbsp
                        </div>                        
                        
                    </div>
                    <div style="float: right; margin: 0px 0px 0 0;">
                        <asp:Button ID="btnOk1" runat="server" Text="OK" OnClientClick="closeEditTypeWindow(true); return false;" />&nbsp
            <asp:Button ID="btnCancel1" runat="server" Text="Cancel" OnClientClick="closeEditTypeWindow(false); return false;" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </telerik:RadWindow>
    <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

        <script type="text/javascript" language="javascript">
            var _type = '0';
            var _ticketType = '-1';
            var _editID = -1;
            var _useProductFamilies = false;

            function onShow() {
                parent.parent.Ts.Settings.User.read('SelectedCustomPropertyValue', 0, function (value) {
                    var cmbType = $find("<%=cmbTypes.ClientID %>");
                    var item = cmbType.findItemByValue(value);
                    if (item != null) item.select();
                    loadType(cmbType.get_selectedItem());
                    loadTicketTypeCombo();
                });
                }

                $(document).ready(function () {
                    var button = $('#spanUploadIcon'), interval;


                    new AjaxUpload(button, {
                        action: '/Upload/Images/TicketTypes',
                        autoSubmit: true,
                        responseType: false,
                        onSubmit: function (file, ext) {
                            button.text('Uploading Icon');
                            // Uploding -> Uploading. -> Uploading...
                            interval = window.setInterval(function () {
                                var text = button.text();
                                if (text.length < 13) {
                                    button.text(text + '.');
                                } else {
                                    button.text('Uploading Icon');
                                }
                            }, 200);
                        },
                        onComplete: function (file, response) {
                            button.text('Upload Icon');
                            window.clearInterval(interval);
                            loadTicketTypeImageCombo($find('<%= wndEditType.ContentContainer.FindControl("cmbTicketTypeIcons").ClientID %>').get_value());
                    if (response == '')
                        $('#divUploadResults').html('There was an error uploading your files.').show();
                    else
                        $('#divUploadResults').html('Uploaded: ' + response).show();

                    setTimeout("$('#divUploadResults').hide('slow')", 5000);
                }
            });
        });

        function pageLoad() {
            onShow();
            /*var cmbType = $find("<%=cmbTypes.ClientID %>");
    loadType(cmbType.get_selectedItem());
    loadTicketTypeCombo();*/
}

function cmbTypes_OnClientSelectedIndexChanged(sender, args) {
    loadType(args.get_item());
}

function loadType(comboBoxItem) {
    if (comboBoxItem == null) return;
    $('#spanCaption').text(comboBoxItem.get_text());
    _type = comboBoxItem.get_value();
    parent.parent.Ts.Settings.User.write('SelectedCustomPropertyValue', _type);
    if (_type == 4) {
        $('#divTicketType').show();
    }
    else {
        $('#divTicketType').hide();
    }
    loadTypes();
}

function cmbTicketTypes_OnClientSelectedIndexChanged(sender, args) {
    _ticketType = args.get_item().get_value();

    loadTypes();
}

function loadTypes() {
    PageMethods.GetTypesHtml2(_type, _ticketType, function(result) {
        $('#divTypes').html(result.Html);
        _useProductFamilies = result.UseProductFamilies;
    });
}

function loadTicketTypeCombo() {
    PageMethods.GetTicketTypesComboData(function(items) {
        var combo = $find("<%=cmbTicketTypes.ClientID %>");
              var selectedValue = combo.get_value();
              loadCombo(combo, items);
              var selectedItem = combo.findItemByValue(selectedValue);
              if (selectedItem) selectedItem.select();
              _ticketType = combo.get_value();
          });
          }

          function loadCombo(combo, items, oldValue) {
              combo.trackChanges();
              combo.clearItems();
              for (var i = 0; i < items.length; i++) {
                  var item = new Telerik.Web.UI.RadComboBoxItem();
                  item.set_text(items[i].Text);
                  item.set_value(items[i].Value);
                  if (items[i].ImageUrl != null) item.set_imageUrl(items[i].ImageUrl);
                  combo.get_items().add(item);
              }
              combo.commitChanges();
              combo.get_items().getItem(0).select();
        
              if (oldValue) {
                  var selectedItem = combo.findItemByValue(oldValue);
                  if (selectedItem) selectedItem.select();
              }
          }

          function editType(id) {
              $('#divActionType').hide();
              $('#divTicketStatus').hide();
              $('#divProductVersionStatus').hide();
              $('#divTicketTypeIcon').hide();
              $('#divVisibleOnPortal').hide();
              $('#divUploadResults').hide();
              $('#divIsActive').hide();
              $('#divExcludeFromCDI').hide();

              switch (_type) {
                  case '0': $('#divActionType').show(); break;
                  case '2': $('#divProductVersionStatus').show(); break;
                  case '3': $('#divVisibleOnPortal').show(); break;
                  case '4': $('#divTicketStatus').show(); break;
                  case '5': $('#divTicketTypeIcon').show();$('#divVisibleOnPortal').show();$('#divIsActive').show();$('#divExcludeFromCDI').show();  break;
              }

              if (_useProductFamilies && _type == 5) {
                  $('#divTicketTypeProductFamily').show();
              }
              else {
                  $('#divTicketTypeProductFamily').hide();
              }

              $find('<%= wndEditType.ContentContainer.FindControl("textName").ClientID %>').set_value('');
            $find('<%= wndEditType.ContentContainer.FindControl("textDescription").ClientID %>').set_value('');
            $get('<%= wndEditType.ContentContainer.FindControl("cbIsTimed").ClientID %>').checked = false;
            $get('<%= wndEditType.ContentContainer.FindControl("cbIsClosed").ClientID %>').checked = false;
            $get('<%= wndEditType.ContentContainer.FindControl("cbIsClosedEmail").ClientID %>').checked = false;
            $get('<%= wndEditType.ContentContainer.FindControl("cbIsEmailResponse").ClientID %>').checked = false;
            $get('<%= wndEditType.ContentContainer.FindControl("cbPauseSla").ClientID %>').checked = false;
            $get('<%= wndEditType.ContentContainer.FindControl("cbIsShipping").ClientID %>').checked = false;
            $get('<%= wndEditType.ContentContainer.FindControl("cbIsDiscontinued").ClientID %>').checked = false;
            $get('<%= wndEditType.ContentContainer.FindControl("cbIsVisibleOnPortal").ClientID %>').checked = false;
            $get('<%= wndEditType.ContentContainer.FindControl("cbIsActive").ClientID %>').checked = false; //vv
            $get('<%= wndEditType.ContentContainer.FindControl("cbExcludeFromCDI").ClientID %>').checked = false; //vv
              
            if (id != null) {
                PageMethods.GetTypeObject(_type, id, function(result) {
                    $find('<%= wndEditType.ContentContainer.FindControl("textName").ClientID %>').set_value(result.Name);
                  $find('<%= wndEditType.ContentContainer.FindControl("textDescription").ClientID %>').set_value(result.Description);
                  $get('<%= wndEditType.ContentContainer.FindControl("cbIsTimed").ClientID %>').checked = result.IsTimed;
                  $get('<%= wndEditType.ContentContainer.FindControl("cbIsClosed").ClientID %>').checked = result.IsClosed;
                  $get('<%= wndEditType.ContentContainer.FindControl("cbIsClosedEmail").ClientID %>').checked = result.IsClosedEmail;
                  $get('<%= wndEditType.ContentContainer.FindControl("cbIsEmailResponse").ClientID %>').checked = result.IsEmailResponse;
                  $get('<%= wndEditType.ContentContainer.FindControl("cbPauseSla").ClientID %>').checked = result.PauseSla;
                  $get('<%= wndEditType.ContentContainer.FindControl("cbIsShipping").ClientID %>').checked = result.IsShipping;
                  $get('<%= wndEditType.ContentContainer.FindControl("cbIsDiscontinued").ClientID %>').checked = result.IsDiscontinued;
                  $get('<%= wndEditType.ContentContainer.FindControl("cbIsVisibleOnPortal").ClientID %>').checked = result.IsVisibleOnPortal;
                  $get('<%= wndEditType.ContentContainer.FindControl("cbIsActive").ClientID %>').checked = result.IsActive; //vv
                  $get('<%= wndEditType.ContentContainer.FindControl("cbExcludeFromCDI").ClientID %>').checked = result.ExcludeFromCDI; //vv
                  if (_type == 5) {
                      loadTicketTypeImageCombo(result.IconUrl.toLowerCase());
                      loadTicketTypeProductFamilyCombo(result.ProductFamilyID);
                  }

                  showEditWindow(id);
              });
          }
          else {
              if (_type == 5) {
                  loadTicketTypeImageCombo();
                  loadTicketTypeProductFamilyCombo();
              }
              showEditWindow();
          }
      }

      function loadTicketTypeImageCombo(url) {
          PageMethods.GetTicketTypeImagesComboData(function(items) {
              var combo = $find('<%= wndEditType.ContentContainer.FindControl("cmbTicketTypeIcons").ClientID %>');
          loadCombo(combo, items, url);
      });
      }

      function loadTicketTypeProductFamilyCombo(productFamilyID) {
          PageMethods.GetTicketTypeProductFamilyComboData(function(items) {
              var combo = $find('<%= wndEditType.ContentContainer.FindControl("cmbTicketTypeProductFamilies").ClientID %>');
                loadCombo(combo, items, productFamilyID);
            });
            }

            function showEditWindow(id) {
                showWindow($find("<%=wndEditType.ClientID%>"), function(result) {
                if (!result) return;
                PageMethods.UpdateType(_type, _ticketType, (id == null) ? null : id,
                  $find('<%= wndEditType.ContentContainer.FindControl("textName").ClientID %>').get_value(),
            $find('<%= wndEditType.ContentContainer.FindControl("textDescription").ClientID %>').get_value(),
            $get('<%= wndEditType.ContentContainer.FindControl("cbIsTimed").ClientID %>').checked,
            $get('<%= wndEditType.ContentContainer.FindControl("cbIsClosed").ClientID %>').checked,
            $get('<%= wndEditType.ContentContainer.FindControl("cbIsClosedEmail").ClientID %>').checked,
            $get('<%= wndEditType.ContentContainer.FindControl("cbIsEmailResponse").ClientID %>').checked,
            $get('<%= wndEditType.ContentContainer.FindControl("cbPauseSla").ClientID %>').checked,
            $get('<%= wndEditType.ContentContainer.FindControl("cbIsShipping").ClientID %>').checked,
            $get('<%= wndEditType.ContentContainer.FindControl("cbIsDiscontinued").ClientID %>').checked,
            $find('<%= wndEditType.ContentContainer.FindControl("cmbTicketTypeProductFamilies").ClientID %>').get_value(),
            $find('<%= wndEditType.ContentContainer.FindControl("cmbTicketTypeIcons").ClientID %>').get_value(),
            $get('<%= wndEditType.ContentContainer.FindControl("cbIsVisibleOnPortal").ClientID %>').checked,
            $get('<%= wndEditType.ContentContainer.FindControl("cbIsActive").ClientID %>').checked, //vv
            $get('<%= wndEditType.ContentContainer.FindControl("cbExcludeFromCDI").ClientID %>').checked, //vv
            function (html) {
                $('#divTypes').html(html);
                if (_type == 5) loadTicketTypeCombo();
            });
            });
      }


      function closeEditTypeWindow(accepted) {
          if (accepted) {
              if ($find('<%= wndEditType.ContentContainer.FindControl("textName").ClientID %>').get_value().trim() == '') {
              alert("Please enter a name.");
              return;
          }
      }

      var wnd = $find("<%=wndEditType.ClientID%>");
      wnd.argument = accepted;
      wnd.close();
  }



  function deleteType(id, name) {
      PageMethods.CanDelete(id, _type, _ticketType, function(result) {
          if (!result) {
              PageMethods.GetReplaceTypeComboData(id, _type, _ticketType, function(items) {
                  var combo = $find('<%= wndDeleteType.ContentContainer.FindControl("cmbReplaceTypes").ClientID %>');
                      loadCombo(combo, items);
                      $('#itemNameDiv').html('"' + name + '"');
                      if (_type == 5) $('#divDeleteReplace').hide(); else $('#divDeleteReplace').show();
                      parent.parent.Ts.System.logAction('Admin Custom Properties - Type Deleted');

                      showWindow($find("<%=wndDeleteType.ClientID%>"), function(result) {
                    if (!result) return;
                    PageMethods.ReplaceType(_type, id, result, _ticketType, function(result) {
                        $('#divTypes').html(result);
                        if (_type == 5) loadTicketTypeCombo();
                    });
                });
                  });
        }
        else {
            alert(result);
        }
          });
}

function closeDeleteTypeWindow(accepted) {
    var wnd = $find("<%=wndDeleteType.ClientID%>");
    wnd.argument = accepted ? $find('<%= wndDeleteType.ContentContainer.FindControl("cmbReplaceTypes").ClientID %>').get_value() : null;
    wnd.close();
}



function showWindow(wnd, callback) {
    if (callback) {
        var fn = function(sender, args) { sender.remove_close(fn); callback(sender.argument); }
        wnd.add_close(fn);
    }
    wnd.show();
    parent.parent.Ts.System.logAction('Admin Custom Properties - Type Dialog Opened');
}

function moveUp(id) {
    PageMethods.MoveUp(_type, id, _ticketType, function(result) {
        $('#divTypes').html(result);
        if (_type == 5) loadTicketTypeCombo();
        parent.parent.Ts.System.logAction('Admin Custom Properties - Position Changed');
    });
}
function moveDown(id) {
    PageMethods.MoveDown(_type, id, _ticketType, function(result) {
        $('#divTypes').html(result);
        if (_type == 5) loadTicketTypeCombo();
        parent.parent.Ts.System.logAction('Admin Custom Properties - Position Changed');
    });
}

        </script>

    </telerik:RadScriptBlock>
</asp:Content>
