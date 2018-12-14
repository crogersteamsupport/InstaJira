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

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (Request["UserID"] != null)
        {
            _userID = int.Parse(Request["UserID"]);
        }

        _organizationID = UserSession.LoginUser.OrganizationID;

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
            Page.Title = "Image Paste";
        }

    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        Boolean FileOK = false;
        Boolean FileSaved = false;

        //String path = HttpContext.Current.Request.PhysicalApplicationPath + "images\\tempupload\\";
        //string path = AttachmentPath.GetPath(UserSession.LoginUser, UserSession.LoginUser.OrganizationID, AttachmentPath.Folder.ProfileImages);

        //if (Upload.HasFile)
        //{
        //    Session["WorkingImage"] = "tmpavatar" + Upload.FileName.Replace(" ",string.Empty);
        //    String FileExtension = Path.GetExtension(Session["WorkingImage"].ToString()).ToLower();
        //    String[] allowedExtensions = { ".png", ".jpeg", ".jpg" };
        //    for (int i = 0; i < allowedExtensions.Length; i++)
        //    {
        //        if (FileExtension == allowedExtensions[i])
        //        {
        //            FileOK = true;
        //        }
        //    }
        //}

        //if (FileOK)
        //{
        //    try
        //    {
        //        Upload.PostedFile.SaveAs(Server.MapPath("~/Images/") + Session["WorkingImage"]);
        //        FileSaved = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        lblError.Text = "File could not be uploaded." + ex.Message.ToString();
        //        lblError.Visible = true;
        //        FileSaved = false;
        //    }
        //}
        //else
        //{
        //    lblError.Text = "Cannot accept files of this type.";
        //    lblError.Visible = true;
        //}

        if (FileSaved)
        {
            //pnlUpload.Visible = false;
            //pnlCrop.Visible = true;
            //imgCrop.ImageUrl = "../images/tempupload/" + Session["WorkingImage"].ToString() + ".ashx?height=300";
            //imgCrop.ImageUrl = "../Images/" +  Session["WorkingImage"].ToString() + ".ashx?height=300";
            //croppanel.Visible = true;
        }
    }

    public override bool Save()
    {
        String temppath = HttpContext.Current.Request.PhysicalApplicationPath + "images\\";
        string path = TeamSupport.Data.Quarantine.WebAppQ.GetAttachmentPath6(UserSession.LoginUser);

        if (img1.Value != "")
        {
            string filename = Guid.NewGuid().ToString();
            using (WebClient Client = new WebClient())
            {
                img1.Value = img1.Value.Replace(".ashx", "");
                Client.DownloadFile(img1.Value, temppath + "temp_" + filename + ".png");
            }
            ImageBuilder.Current.Build(temppath + "temp_" + filename + ".png", path + '\\' + filename + ".png", new ResizeSettings(img1.Value));
            File.Delete(temppath + "temp_" + filename + ".png");
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
        catch {
            return false;
        }
         
    }

    protected void resizeButton_Click1(object sender, EventArgs e)
    {

    }
}