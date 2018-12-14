using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
    //There are at this point AT LEAST two places with the following Log class duplicated (HubSpotSources/SyncLog.cs) and other more with something similar. We need to centralize this at some point!
    public class Log
    {
        private string _logPath;
        private string _fileName;

        public Log(string path, string logType, int? threadNumber = null)
        {
            _logPath = path;
            _fileName = string.Format("{0} Debug File {1} - {2}{3}{4}.txt", logType, (threadNumber == null ? "" : "[" + threadNumber.ToString() + "]"), DateTime.UtcNow.Month.ToString(), DateTime.UtcNow.Day.ToString(), DateTime.UtcNow.Year.ToString());

            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
        }

        public void Write(string text)
        {
            //the very first time we write to this file (once each day), purge old files
            if (!File.Exists(string.Format("{0}\\{1}", _logPath, _fileName)))
            {
                foreach (string oldFileName in Directory.GetFiles(_logPath))
                {
                    if (File.GetLastWriteTime(oldFileName).AddDays(7) < DateTime.UtcNow)
                    {
                        File.Delete(oldFileName);
                    }
                }
            }

            File.AppendAllText(string.Format("{0}\\{1}", _logPath, _fileName), string.Format("{0}: {1}{2}", DateTime.Now.ToLongTimeString(), text, Environment.NewLine));
        }

        public void WriteData(DataRow row)
        {
            Write("Data Row:");
            Write(DataUtils.DataRowToString(row));
        }

        public void WriteToCrmErrorReport(LoginUser loginUser,
                                        string errorMessage,
                                        int organizationId,
                                        string objectId,
                                        string objectType,
                                        string objectFieldName,
                                        string objectData,
                                        string operation,
                                        IntegrationType integration,
                                        string orientation,
                                        CRMLinkErrors crmErrors,
                                        bool logText = false)
        {
            CRMLinkError error = crmErrors.FindUnclearedByObjectIDAndFieldName(objectId, objectFieldName);

            if (error == null)
            {
                CRMLinkErrors newCrmLinkError = new CRMLinkErrors(loginUser);
                error = newCrmLinkError.AddNewCRMLinkError();
                error.OrganizationID = organizationId;
                error.CRMType = Enums.GetDescription(integration);
                error.Orientation = orientation;
                error.ObjectType = objectType;
                error.ObjectID = objectId.ToString();
                error.ObjectFieldName = objectFieldName;
                error.ObjectData = objectData;
                error.Exception = errorMessage;
                error.OperationType = operation;
                error.ErrorCount = 1;
                error.IsCleared = false;
                error.ErrorMessage = errorMessage;
                newCrmLinkError.Save();
            }
            else
            {
                error.ErrorCount = error.ErrorCount + 1;
                error.IsCleared = false;
                error.ObjectData = objectData;
                error.Exception = errorMessage;
                error.Collection.Save();
            }

            if (logText)
            {
                Write(errorMessage);
            }
        }

        public void ClearCrmLinkError(string objectId, string objectFieldName, CRMLinkErrors crmErrors)
        {
            CRMLinkError error = crmErrors.FindUnclearedByObjectIDAndFieldName(objectId, objectFieldName);

            if (error != null)
            {
                error.IsCleared = true;
                error.DateModified = DateTime.UtcNow;
                error.Collection.Save();
            }
        }
    }

}
