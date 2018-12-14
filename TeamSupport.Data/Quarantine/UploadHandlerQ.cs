using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace TeamSupport.Data.Quarantine
{
    public static class UploadHandlerQ
    {
        public static void ProcessRequest(LoginUser loginUser, int organizationID, HttpContext context, int? _id, string _ratingImage, List<string> segments)
        {
            int id;
            if (int.TryParse(segments[segments.Count - 1], out id))
            {
                _id = id;
                segments.RemoveAt(segments.Count - 1);
            }

            if (segments[segments.Count - 1] == "ratingpositive" || segments[segments.Count - 1] == "ratingneutral" || segments[segments.Count - 1] == "ratingnegative")
            {
                _ratingImage = segments[segments.Count - 1];
                segments.RemoveAt(segments.Count - 1);
            }


            AttachmentPath.Folder folder = GetFolder(context, segments.ToArray());
            if (folder == AttachmentPath.Folder.None) throw new Exception("Invalid path.");

            AttachmentProxy.References refType = AttachmentPath.GetFolderReferenceType(folder);
            if (refType == AttachmentProxy.References.None)
            {
                SaveFilesOld(loginUser, organizationID, _id, _ratingImage, context, folder);
            }
            else
            {
                SaveFiles(loginUser, context, folder, organizationID, _id);
            }

        }
        private static AttachmentPath.Folder GetFolder(HttpContext context, string[] segments)
        {
            StringBuilder path = new StringBuilder();
            for (int i = 0; i < segments.Length; i++)
            {
                path.Append("\\");
                path.Append(segments[i]);
            }
            return AttachmentPath.GetFolderByName(path.ToString());
        }

        private static void SaveFilesOld(LoginUser loginUser, int organizationID, int? _id, string _ratingImage, HttpContext context, AttachmentPath.Folder folder)
        {
            StringBuilder builder = new StringBuilder();
            string path = AttachmentPath.GetPath(loginUser, organizationID, folder);
            if (_id != null) path = Path.Combine(path, _id.ToString());
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            HttpFileCollection files = context.Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].ContentLength > 0)
                {
                    string fileName = TeamSupport.Handlers.UploadUtils.RemoveSpecialCharacters(DataUtils.VerifyUniqueUrlFileName(path, Path.GetFileName(files[i].FileName)));
                    if (builder.Length > 0) builder.Append(",");
                    builder.Append(fileName);

                    if (_ratingImage != "")
                    {
                        fileName = _ratingImage + ".png";
                        AgentRatingsOptions ratingoptions = new AgentRatingsOptions(loginUser);
                        ratingoptions.LoadByOrganizationID(organizationID);

                        if (ratingoptions.IsEmpty)
                        {
                            AgentRatingsOption opt = (new AgentRatingsOptions(loginUser)).AddNewAgentRatingsOption();
                            opt.OrganizationID = organizationID;
                            switch (_ratingImage)
                            {
                                case "ratingpositive":
                                    opt.PositiveImage = "/dc/" + organizationID + "/agentrating/ratingpositive";
                                    break;
                                case "ratingneutral":
                                    opt.NeutralImage = "/dc/" + organizationID + "/agentrating/ratingneutral";
                                    break;
                                case "ratingnegative":
                                    opt.NegativeImage = "/dc/" + organizationID + "/agentrating/ratingnegative";
                                    break;
                            }
                            opt.Collection.Save();
                        }
                        else
                        {
                            switch (_ratingImage)
                            {
                                case "ratingpositive":
                                    ratingoptions[0].PositiveImage = "/dc/" + organizationID + "/agentrating/ratingpositive";
                                    break;
                                case "ratingneutral":
                                    ratingoptions[0].NeutralImage = "/dc/" + organizationID + "/agentrating/ratingneutral";
                                    break;
                                case "ratingnegative":
                                    ratingoptions[0].NegativeImage = "/dc/" + organizationID + "/agentrating/ratingnegative";
                                    break;
                            }
                            ratingoptions[0].Collection.Save();
                        }



                    }

                    fileName = Path.Combine(path, fileName);
                    files[i].SaveAs(fileName);
                }
            }
            context.Response.Write(builder.ToString());
        }

        static void SaveFiles(LoginUser loginUser, HttpContext context, AttachmentPath.Folder folder, int organizationID, int? itemID)
        {
            List<TeamSupport.Handlers.UploadResult> result = new List<TeamSupport.Handlers.UploadResult>();

            AttachmentProxy.References refType = AttachmentPath.GetFolderReferenceType(folder);

            string path = AttachmentPath.GetPath(LoginUser.Anonymous, organizationID, folder, 3);
            if (itemID != null) path = Path.Combine(path, itemID.ToString());
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            HttpFileCollection files = context.Request.Files;

            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].ContentLength > 0)
                {
                    string fileName = TeamSupport.Handlers.UploadUtils.RemoveSpecialCharacters(DataUtils.VerifyUniqueUrlFileName(path, Path.GetFileName(files[i].FileName)));

                    files[i].SaveAs(Path.Combine(path, fileName));
                    if (refType != AttachmentProxy.References.None && itemID != null)
                    {
                        Attachment attachment = (new Attachments(loginUser)).AddNewAttachment();
                        attachment.RefType = refType;
                        attachment.RefID = (int)itemID;
                        attachment.OrganizationID = organizationID;
                        attachment.FileName = fileName;
                        attachment.Path = Path.Combine(path, fileName);
                        attachment.FileType = files[i].ContentType;
                        attachment.FileSize = files[i].ContentLength;
                        attachment.FilePathID = 3;
                        if (context.Request.Form["description"] != null)
                            attachment.Description = context.Request.Form["description"].Replace("\n", "<br />");
                        if (context.Request.Form["productFamilyID"] != null && context.Request.Form["productFamilyID"] != "-1")
                            attachment.ProductFamilyID = Int32.Parse(context.Request.Form["productFamilyID"]);

                        attachment.Collection.Save();
                        result.Add(new TeamSupport.Handlers.UploadResult(fileName, attachment.FileType, attachment.FileSize, attachment.AttachmentID));
                    }
                    else
                    {
                        switch (refType)
                        {
                            default:
                                break;
                        }
                    }
                }
            }
            context.Response.Clear();
            context.Response.ContentType = "text/plain";
            context.Response.Write(DataUtils.ObjectToJson(result.ToArray()));
        }

    }
}