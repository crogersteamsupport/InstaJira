using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TeamSupport.Data.Quarantine
{
    public class HubAttachmentsQ
    {
        public static string CreateAttachmentDirectory(LoginUser loginUser, int parentOrganizationID, AttachmentProxy.References refType, int refID)
        {
            // hub save only saves two types of attachments on this code path
            AttachmentPath.Folder folder = AttachmentPath.Folder.CustomerHubLogo;
            if (refType == AttachmentProxy.References.Actions)
                folder = AttachmentPath.Folder.Actions;

            string path = AttachmentPath.GetPath(loginUser, parentOrganizationID, folder);
            path = Path.Combine(path, refID.ToString());
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }

        public static AttachmentProxy LoadByReference(LoginUser loginUser, int refID, ReferenceType refType)
        {
            Attachments attachements = new Attachments(loginUser);
            attachements.LoadByReference(ReferenceType.CustomerHubLogo, refID);

            if (attachements.Any())
            {
                //Order by Descending so that we get the newest logo uploaded since more than one can exist. 
                return attachements.OrderByDescending(a => a.DateCreated).First().GetProxy();
            }
            else return null;
        }

        public static string GetAttachmentPath(LoginUser loginUser, int parentOrganizationID)
        {
            Attachments attachements = new Attachments(loginUser);
            attachements.LoadByReference(ReferenceType.CustomerHubLogo, parentOrganizationID);

            //Order by Descending so that we get the newest logo uploaded since more than one can exist. 
            if (attachements.Any())
                return attachements.OrderByDescending(a => a.DateCreated).First().Path;
            return String.Empty;
        }

        public static List<AttachmentProxy> GetAttachments(LoginUser loginUser, ReferenceType refType, int refID)
        {
            Attachments attachments = new Attachments(loginUser);
            attachments.LoadByReference(refType, refID);

            List<AttachmentProxy> results = new List<AttachmentProxy>();
            if (attachments.Any())
            {
                for (int a = 0; a < attachments.Count(); a++)
                {
                    results.Add(attachments[a].GetProxy());
                }
            }
            return results;
        }

        public static AttachmentProxy[] GetAttachmentsByAction(LoginUser loginUser, int actionID)
        {
            Data.Attachments attachments = new Data.Attachments(loginUser);
            attachments.LoadByActionID(actionID);

            return attachments.GetAttachmentProxies();
        }
    }
}
