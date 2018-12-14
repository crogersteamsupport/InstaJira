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
using System.Globalization;
using System.IO;
using System.Net;
using ImageResizer;

public partial class Dialogs_ProfileImage : BaseDialogPage
{
    private int _userID = -1;
    private int _organizationID = -1;
    private static string uploadedFileName = "";
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (Request["UserID"] != null)
        {
            _userID = int.Parse(Request["UserID"]);
        }

        if (Request["OrganizationID"] != null)
        {
            _organizationID = int.Parse(Request["OrganizationID"]);
        }

        if (!UserSession.CurrentUser.IsSystemAdmin && _userID != UserSession.LoginUser.UserID)
        {
            Response.Write("");
            Response.End();
            return;
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        Organization organization = (Organization)Organizations.GetOrganization(UserSession.LoginUser, _organizationID);

        if (organization.OrganizationID != UserSession.LoginUser.OrganizationID && organization.ParentID != UserSession.LoginUser.OrganizationID)
        {
            Response.Write("Invalid Request");
            Response.End();
            return;
        }

        if (!IsPostBack)
        {
            Page.Title = "Profile Image";
        }
    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        Boolean FileOK = false;
        Boolean FileSaved = false;

        string fileName = "tmpavatar" + Upload.FileName.Replace(" ", string.Empty);
        uploadedFileName = fileName;

        if (Upload.HasFile)
        {
            Session["WorkingImage"] = fileName;
            String FileExtension = Path.GetExtension(Session["WorkingImage"].ToString()).ToLower();
            String[] allowedExtensions = { ".png", ".jpeg", ".jpg" };
            for (int i = 0; i < allowedExtensions.Length; i++)
            {
                if (FileExtension == allowedExtensions[i])
                {
                    FileOK = true;
                }
            }
        }

        if (FileOK)
        {
            try
            {
                string path = TeamSupport.Data.Quarantine.WebAppQ.GetAttachmentPath7(UserSession.LoginUser, fileName);
                Upload.PostedFile.SaveAs(path);
                FileSaved = true;
            }
            catch (Exception ex)
            {
                lblError.Text = "File could not be uploaded." + ex.Message.ToString();
                lblError.Visible = true;
                FileSaved = false;
            }
        }
        else
        {
            lblError.Text = "Cannot accept files of this type.";
            lblError.Visible = true;
        }

        if (FileSaved)
        {
            imgCrop.ImageUrl = "dc/" + UserSession.LoginUser.OrganizationID + "/images/temp/" + fileName + "?height=300";
            croppanel.Visible = true;
        }
    }


    private void RemoveCachedImages(int organizationID, int userID)
    {
        string cachePath = TeamSupport.Data.Quarantine.WebAppQ.GetImageCachePath1(organizationID);
        if (Directory.Exists(cachePath))
        {
            string pattern = userID.ToString() + "-*.*";
            string[] files = Directory.GetFiles(cachePath, pattern, SearchOption.TopDirectoryOnly);
            foreach (String file in files)
            {
                File.Delete(file);
            }
        }
    }

    public override bool Save()
    {
        try
        {
            string path = TeamSupport.Data.Quarantine.WebAppQ.GetAttachmentPath8(UserSession.LoginUser);
            RemoveCachedImages(UserSession.LoginUser.OrganizationID, _userID);

            if (img1.Value != "")
            {
                img1.Value = img1.Value.Replace(".ashx", "");
                string source = TeamSupport.Data.Quarantine.WebAppQ.GetAttachmentPath9(UserSession.LoginUser, Session["WorkingImage"].ToString());
                string dest = path + '\\' + _userID + "avatar.jpg";
                try
                {
                    ImageBuilder.Current.Build(source, dest, new ResizeSettings(img1.Value));
                }
                catch (Exception ex2)
                {
                    ExceptionLogs.LogException(UserSession.LoginUser, ex2, "ImageBuilder", string.Format("source:{0},  dest:{1}", source, dest));
                    throw;
                }

                File.Delete(source);

                AttachmentProxy proxy = TeamSupport.ModelAPI.Model_API.ReadRefTypeProxyByRefID<UserPhotoAttachmentProxy>(_userID);
                if (proxy != null)
                {
                    proxy.FileName = _userID + "avatar.jpg";
                    proxy.Path = path + '\\' + _userID + "avatar.jpg";
                    proxy.FilePathID = 3;
                    TeamSupport.ModelAPI.Model_API.Update(proxy);
                }
                else
                {
                    proxy = AttachmentProxy.ClassFactory(AttachmentProxy.References.UserPhoto);
                    proxy.RefID = _userID;
                    proxy.OrganizationID = _organizationID;
                    proxy.FileName = _userID + "avatar.jpg";
                    proxy.Path = path + '\\' + _userID + "avatar.jpg";
                    proxy.FilePathID = 3;
                    proxy.FileType = "image/jpeg";
                    proxy.FileSize = -1;
                    TeamSupport.ModelAPI.Model_API.Create(proxy);
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionLogs.LogException(UserSession.LoginUser, ex, "Save Avatar");
            throw;
        }
        return true;
    }

    public override bool Close()
    {
        String temppath = HttpContext.Current.Request.PhysicalApplicationPath + "images\\";
        try
        {
            File.Delete(temppath + Session["WorkingImage"].ToString());
            return true;
        }
        catch
        {
            return false;
        }

    }

}