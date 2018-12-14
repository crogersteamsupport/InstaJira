using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Data;

namespace TeamSupport.Data.Quarantine
{
    public class ServiceQ
    {
        public static string UpdloadAttachments(LoginUser _loginUser, CRMLinkErrors _crmErrors, TeamSupport.ServiceLibrary.Log _log, Data.Action action, string encodedCredentials, string result, string appId, string hostName)
        {
            Attachments attachments = new Attachments(_loginUser);
            attachments.LoadForIntegration(action.ActionID, IntegrationType.ServiceNow);

            foreach (Attachment attachment in attachments)
            {
                UploadAttachment(_loginUser, _crmErrors, _log, appId, encodedCredentials, ref result, attachment, hostName);
            }

            return result;
        }

        private static HttpStatusCode UploadAttachment(LoginUser _loginUser, CRMLinkErrors _crmErrors, TeamSupport.ServiceLibrary.Log _log, string incidentId, string encodedCredentials, ref string responseString, Attachment attachment, string hostName)
        {
            HttpStatusCode resultStatusCode = HttpStatusCode.BadRequest;
            string error = string.Empty;

            if (File.Exists(attachment.Path))
            {
                FileStream attachmentFileStream = new FileStream(attachment.Path, FileMode.Open, FileAccess.Read);
                int fileSizeLimit = 50000000; //vv 50MB. can we get this from Snow?

                if (attachmentFileStream.Length <= fileSizeLimit)
                {
                    try
                    {
                        string url = GetFullUrl(hostName, string.Format("api/now/attachment/file?table_name=incident&table_sys_id={0}&file_name={1}", incidentId, System.Web.HttpUtility.UrlEncode(attachment.FileName)));
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        request.Method = "POST";
                        request.ContentType = "multipart/form-data";
                        request.Accept = "application/json";
                        request.Headers.Add("Authorization", "Basic " + encodedCredentials);

                        MemoryStream content = new MemoryStream();
                        StreamWriter writer = new StreamWriter(content);
                        byte[] data = new byte[attachmentFileStream.Length + 1];
                        attachmentFileStream.Read(data, 0, data.Length);
                        attachmentFileStream.Close();
                        content.Write(data, 0, data.Length);
                        writer.WriteLine();
                        writer.Flush();
                        content.Seek(0, SeekOrigin.Begin);
                        request.ContentLength = content.Length;

                        using (Stream requestStream = request.GetRequestStream())
                        {
                            content.WriteTo(requestStream);
                            requestStream.Close();
                        }

                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            _log.Write("Attachment \"" + attachment.FileName + "\" sent.");
                            Stream responseStream = response.GetResponseStream();

                            if (responseStream != null)
                            {
                                var streamReader = new StreamReader(responseStream);
                                responseString = streamReader.ReadToEnd();
                            }

                            resultStatusCode = response.StatusCode;
                            response.Close();
                        }

                        content.Flush();
                        content.Close();
                        attachment.SentToSnow = true;
                        attachment.Collection.Save();
                        _log.ClearCrmLinkError(attachment.AttachmentID.ToString(), string.Empty, _crmErrors);
                    }
                    catch (WebException webEx)
                    {
                        using (var response = (HttpWebResponse)webEx.Response)
                        {
                            Stream responseStream = response.GetResponseStream();

                            if (responseStream != null)
                            {
                                var reader = new StreamReader(responseStream);
                                string responseText = string.Format("Error code: {0} - {1}{2}{3}", response.StatusCode, response.StatusDescription, Environment.NewLine, reader.ReadToEnd());
                                throw new Exception(responseText);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex.InnerException);
                    }
                }
                else
                {
                    error = string.Format("Attachment was not sent as its file size ({0}) exceeded the file size limit of {1}", attachmentFileStream.Length.ToString(), fileSizeLimit.ToString());
                }
            }
            else
            {
                error = "Attachment was not sent as it was not found on server";
            }

            if (!string.IsNullOrEmpty(error))
            {
                _crmErrors.LoadByOperationAndObjectId(attachment.OrganizationID, Enums.GetDescription(IntegrationType.ServiceNow), Enums.GetDescription(IntegrationOrientation.OutToServiceNow), "attachment", attachment.AttachmentID.ToString());
                _log.WriteToCrmErrorReport(_loginUser, error, attachment.OrganizationID, attachment.AttachmentID.ToString(), "attachment", "file", attachment.FileName, "create", IntegrationType.ServiceNow, Enums.GetDescription(IntegrationOrientation.OutToServiceNow), _crmErrors, logText: true);
            }

            return resultStatusCode;
        }

        private static string GetFullUrl(string hostName, string path)
        {
            if (!hostName.EndsWith("/"))
            {
                hostName += "/";
            }

            return hostName + path;
        }
        public static string GetAttachmentPath10(LoginUser loginUser, int parentID)
        {
            return AttachmentPath.GetPath(loginUser, parentID, AttachmentPath.Folder.OrganizationsLogo);
        }

        public static string GetAttachmentPath11(LoginUser loginUser, string parentID)
        {
            return System.IO.Path.Combine(AttachmentPath.GetImageCachePath(loginUser), "CompanyLogo\\" + parentID);
        }

        public static string GetAttachmentPath12(LoginUser loginUser, int organizationParentId)
        {
            return AttachmentPath.GetPath(loginUser, organizationParentId, AttachmentPath.Folder.ContactImages);
        }

        public static string GetAttachmentPath13(LoginUser loginUser, int organizationParentId)
        {
            return System.IO.Path.Combine(AttachmentPath.GetImageCachePath(loginUser), "Avatars\\" + organizationParentId.ToString() + "\\Contacts\\");
        }

        public static string GetAttachmentPath14(LoginUser loginUser, int organizationID)
        {
            return AttachmentPath.GetPath(loginUser, organizationID, AttachmentPath.Folder.ImportLogs, 3);
        }

        public static string GetAttachmentPath15(LoginUser loginUser, int organizationID, int filePathID, string fileName)
        {
            return Path.Combine(AttachmentPath.GetPath(loginUser, organizationID, AttachmentPath.Folder.Imports, filePathID), fileName);
        }

        public static string GetAttachmentPath16(LoginUser loginUser, int organizationId, int filePathID)
        {
            return AttachmentPath.GetPath(loginUser, organizationId, AttachmentPath.Folder.ScheduledReportsLogs, filePathID);
        }

        public static string GetAttachmentPath17(LoginUser loginUser, int organizationId)
        {
            return AttachmentPath.GetPath(loginUser, organizationId, AttachmentPath.Folder.ScheduledReports, 3);
        }
        public static List<string> GetAtachmentFileNames(LoginUser loginUser, int actionID, Actions actions, TeamSupport.Data.Logs log)
        {
            List<string> fileNames = new List<string>();

            Attachments attachmentsHelper = new Attachments(loginUser);
            attachmentsHelper.LoadByActionID(actionID);

            //specific logic for including attachments a user has added via the send email lightbox
            foreach (Data.Attachment attachment in attachmentsHelper)
            {
                fileNames.Add(attachment.Path);
                log.WriteEventFormat("Adding Attachment   AttachmentID:{0}, ActionID:{1}, Path:{2}", attachment.AttachmentID.ToString(), actions[0].ActionID.ToString(), attachment.Path);
            }

            return fileNames;
        }

        public static void AddFileNames(Actions actions, List<string> fileNames, Data.Action action, Logs log)
        {
            Attachments attachments = action.GetAttachments();

            foreach (Data.Attachment attachment in attachments)
            {
                fileNames.Add(attachment.Path);
                log.WriteEventFormat("Adding Attachment   AttachmentID:{0}, ActionID:{1}, Path:{2}", attachment.AttachmentID.ToString(), actions[0].ActionID.ToString(), attachment.Path);
            }
        }
        public static void AddAttachmentFileNames1(List<string> fileNames, Actions actions, Logs log)
        {
            Attachments attachments = actions[0].GetAttachments();

            foreach (Data.Attachment attachment in attachments)
            {
                fileNames.Add(attachment.Path);
                log.WriteEvent(string.Format("Adding Attachment   AttachmentID:{0}, ActionID:{1}, Path:{2}", attachment.AttachmentID.ToString(), actions[0].ActionID.ToString(), attachment.Path));
            }
        }

        public static void AddAttachmentFileNames2(Actions actions, List<string> fileNames, Logs log)
        {
            Attachments attachments = actions[0].GetAttachments();
            foreach (Data.Attachment attachment in attachments)
            {
                fileNames.Add(attachment.Path);
                log.WriteEventFormat("Adding Attachment   AttachmentID:{0}, ActionID:{1}, Path:{2}", attachment.AttachmentID.ToString(), actions[0].ActionID.ToString(), attachment.Path);
            }
        }

        public static void AddAttachmentFileNames3(List<string> fileNames, Actions actions)
        {
            Attachments attachments = actions[0].GetAttachments();
            foreach (Data.Attachment attachment in attachments)
            {
                fileNames.Add(attachment.Path);
            }
        }

        public static AttachmentProxy GetAttachmentProxy(LoginUser user, int actionDescriptionId)
        {
            return Attachments.GetAttachment(user, actionDescriptionId).GetProxy();
        }

        public static void SetAttachmentSentToJira(LoginUser loginUser, int attachmentID)
        {
            Attachment attachment = Attachments.GetAttachment(loginUser, attachmentID);
            attachment.SentToJira = true;
            attachment.Collection.Save();
        }

        public static void SetAttachmentSentToTFS(LoginUser loginUser, int attachmentID)
        {
            Attachment attachment = Attachments.GetAttachment(loginUser, attachmentID);
            attachment.SentToTFS = true;
            attachment.Collection.Save();
        }

        public static AttachmentProxy[] LoadForJira(LoginUser user, int actionID)
        {
            Attachments attachments = new Attachments(user);
            attachments.LoadForJira(actionID);
            return attachments.GetAttachmentProxies();
        }

        public static AttachmentProxy[] LoadForTFS(LoginUser user, int actionID)
        {
            Attachments attachments = new Attachments(user);
            attachments.LoadForTFS(actionID);
            return attachments.GetAttachmentProxies();
        }


        public static void CreateAttachment(string savePath, int organizationId, AttachmentProxy.References refType, LoginUser user, HttpWebResponse httpWebResponse)
        {
            Attachments attachments = new Attachments(user);
            Attachment attachment = attachments.AddNewAttachment();
            attachment.RefType = refType;
            attachment.RefID = user.UserID;
            attachment.OrganizationID = organizationId;
            attachment.FileName = Path.GetFileName(savePath);
            attachment.Path = savePath;
            attachment.FileType = string.IsNullOrEmpty(httpWebResponse.ContentType) ? "application/octet-stream" : httpWebResponse.ContentType;
            attachment.FileSize = httpWebResponse.ContentLength;
            //attachment.FilePathID = 3;
            attachments.Save();
        }

        /*
TeamSupport.Data.Quarantine.ServiceQ.
    */
    }

}
