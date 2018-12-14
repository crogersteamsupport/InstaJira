using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;
using System.IO;

public partial class Dialogs_ChatProperties : BaseDialogPage
{
    private int _organizationID = -1;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (Request["OrganizationID"] != null) _organizationID = int.Parse(Request["OrganizationID"]);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (_organizationID != UserSession.LoginUser.OrganizationID)
        {
            Response.Write("Invalid paramater");
            Response.End();
            return;
        }

        if (!IsPostBack)
        {
            _manager.AjaxSettings.Clear();
            LoadData();
        }
    }

    private void LoadData()
    {
        ChatSetting setting = ChatSettings.GetSetting(UserSession.LoginUser, _organizationID);
        if (setting == null) return;

        imgAvailable.ImageUrl = String.Format("../dc/{0}/images/chat/chat_available", _organizationID.ToString());
        imgUnavailable.ImageUrl = String.Format("../dc/{0}/images/chat/chat_unavailable", _organizationID.ToString());
        imgLogo.ImageUrl = String.Format("../dc/{0}/images/chat/chat_logo", _organizationID.ToString());

        textChatIntro.Text = Settings.OrganizationDB.ReadString("ChatIntroMessage", "Welcome to our live chat!");

        if (UserSession.LoginUser.GetOrganization().ProductType != ProductType.HelpDesk)
        {
            cbChatTOKScreenEnabled.Checked = Settings.OrganizationDB.ReadBool("ChatTOKScreenEnabled", false);
            cbChatTOKVoiceEnabled.Checked = Settings.OrganizationDB.ReadBool("ChatTOKVoiceEnabled", false);
            cbChatTOKVideoEnabled.Checked = Settings.OrganizationDB.ReadBool("ChatTOKVideoEnabled", false);

        }
        else
        {
            cbChatTOKScreenEnabled.Enabled = false;
            cbChatTOKVoiceEnabled.Enabled = false;
            cbChatTOKVideoEnabled.Enabled = false;
        }

        cbChatTicketDeflectionEnabled.Checked = Settings.OrganizationDB.ReadBool("ChatTicketDeflectionEnabled", false);
        cbChatAvatars.Checked = Settings.OrganizationDB.ReadBool("ChatAvatarsEnabled", false);
    }


    public override bool Save()
    {
        string path = TeamSupport.Data.Quarantine.WebAppQ.GetAttachmentPath5(UserSession.LoginUser);
        if (upAvailable.UploadedFiles.Count > 0)
        {
            string fileName = Path.Combine(path, "chat_available.png");
            fileName = Path.ChangeExtension(fileName, Path.GetExtension(upAvailable.UploadedFiles[0].FileName));
            TeamSupport.Data.Quarantine.WebAppQ.DeleteAttachment(path, fileName);
            upAvailable.UploadedFiles[0].SaveAs(fileName, true);
        }

        if (upUnavailable.UploadedFiles.Count > 0)
        {
            string fileName = Path.Combine(path, "chat_unavailable.png");
            fileName = Path.ChangeExtension(fileName, Path.GetExtension(upUnavailable.UploadedFiles[0].FileName));
            TeamSupport.Data.Quarantine.WebAppQ.DeleteAttachment(path, fileName);
            upUnavailable.UploadedFiles[0].SaveAs(fileName, true);
        }

        if (upLogo.UploadedFiles.Count > 0)
        {
            string fileName = Path.Combine(path, "chat_logo.png");
            fileName = Path.ChangeExtension(fileName, Path.GetExtension(upLogo.UploadedFiles[0].FileName));
            TeamSupport.Data.Quarantine.WebAppQ.DeleteAttachment(path, fileName);
            upLogo.UploadedFiles[0].SaveAs(fileName, true);
        }

        Settings.OrganizationDB.WriteString("ChatIntroMessage", textChatIntro.Text);
        Settings.OrganizationDB.WriteBool("ChatTOKScreenEnabled", cbChatTOKScreenEnabled.Checked);
        Settings.OrganizationDB.WriteBool("ChatTOKVoiceEnabled", cbChatTOKVoiceEnabled.Checked);
        Settings.OrganizationDB.WriteBool("ChatTOKVideoEnabled", cbChatTOKVideoEnabled.Checked);
        Settings.OrganizationDB.WriteBool("ChatAvatarsEnabled", cbChatAvatars.Checked);
        Settings.OrganizationDB.WriteBool("ChatTicketDeflectionEnabled", cbChatTicketDeflectionEnabled.Checked);

        return true;
    }

}
