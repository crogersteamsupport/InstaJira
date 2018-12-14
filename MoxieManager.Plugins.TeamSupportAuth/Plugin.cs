using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.IO;
using System.Xml;
using MoxieManager.Core;
using MoxieManager.Core.Auth;
using MoxieManager.Core.Util;
using TeamSupport.WebUtils;
using TeamSupport.Data;

namespace MoxieManager.Plugins.TeamSupportAuth
{
    public class Plugin : IPlugin, IAuthenticator
    {
        public void Init()
        {
        }

        public bool Authenticate(MoxieManager.Core.Auth.User user)
        {
            if (!TSAuthentication.IsAuthenticated()) return false;
            var config = ManagerContext.Current.Config;
            //string root = SystemSettings.ReadString(TSAuthentication.GetLoginUser(), "FilePath", "C:\\TSData");
            Data.FilePaths filePaths = new FilePaths(TSAuthentication.GetLoginUser());
            filePaths.LoadByID(1);
            string root = filePaths[0].Value;
            root = Path.Combine(root, "WikiDocs\\" + TSAuthentication.OrganizationID.ToString());
            Directory.CreateDirectory(Path.Combine(root, "images"));
            Directory.CreateDirectory(Path.Combine(root, "documents"));
            config.Put("filesystem.rootpath", root);
            config.Put("filesystem.local.wwwroot", root);
            config.Put("filesystem.local.urlprefix", "{proto}://{host}/Wiki/WikiDocs/" + TSAuthentication.OrganizationID.ToString());
            return true;
        }

        public string Name
        {
            get { return "TeamSupportAuth"; }
        }
    }
}


