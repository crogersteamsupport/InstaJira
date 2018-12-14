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
using TeamSupport.WebUtils;
using TeamSupport.Data;
using Telerik.Web.UI;
using System.IO;

public partial class Dialogs_AttachFile : BaseDialogPage
{
  private int _refID = -1;
  private AttachmentProxy.References _refType;

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    _manager.AjaxSettings.Clear();

    _refID = int.Parse(Request["RefID"]);
    _refType = (AttachmentProxy.References)int.Parse(Request["RefType"]);

  }

  public override bool Save()
  {
    int used = Organizations.GetStorageUsed(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
    int allowed = Organizations.GetTotalStorageAllowed(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
    /*
    if (used > allowed)
    {
      if (UserSession.CurrentUser.IsFinanceAdmin)
      {
        _manager.Alert("You have exceeded your allocated storage capacity.  If you would like to add additional storage, please contact our sales team at 800.596.2820 x806, or send an email to sales@teamsupport.com");
      }
      else
      {
        Users users = new Users(UserSession.LoginUser);
        users.LoadFinanceAdmins(UserSession.LoginUser.OrganizationID);
        if (users.IsEmpty)
        {
          _manager.Alert("Please ask your billing administrator to purchase additional storage to add additional attachments.");
        }
        else
        {
          _manager.Alert("Please ask your billing administrator (" + users[0].FirstLastName + ") to purchase additional storage to add additional attachments.");
        }
      }


      return false;
    }
    */


    string folderName = "Unknown";

    switch (_refType)
    {
      case AttachmentProxy.References.Organizations:
        folderName = "OrganizationAttachments";
        break;
      default:
        folderName = "Unknown";
        break;
    }

    foreach (UploadedFile file in ulFile.UploadedFiles)
        {
            string root = TeamSupport.ModelAPI.AttachmentAPI.GetOrganizationAttachmentPath();
            string directory = Path.Combine(Path.Combine(root, folderName), _refID.ToString()) + "\\";
            //string directory = TSUtils.GetAttachmentPath(folderName, _refID, 3);

            string fileName = file.GetName();
            fileName = Path.GetFileName(fileName);
            fileName = DataUtils.VerifyUniqueFileName(directory, fileName);

            string path = TeamSupport.Data.Quarantine.WebAppQ.SaveAttachment(UserSession.LoginUser, file.ContentType, file.ContentLength, directory, fileName, _refType, _refID, textDescription.Text);
            file.SaveAs(path, true);
        }

        return true;
  }

}
