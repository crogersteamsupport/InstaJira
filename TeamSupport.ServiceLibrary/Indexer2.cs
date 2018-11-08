using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dtSearch.Engine;
using TeamSupport.Data;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;

namespace TeamSupport.ServiceLibrary
{
    [Serializable]
    public class Indexer2 : ServiceThreadPoolProcess
    {
        private bool _isVerbose = false;

        

        public override void Run()
        {

            while (!IsStopped)
            {
                try
                {
                    UpdateHealth();

                    _isVerbose = Settings.ReadBool("VerboseLogging", false);

                    ProcessIndex(ReferenceType.Tickets);
                    /*
                    ProcessIndex(ReferenceType.Wikis);
                    ProcessIndex(ReferenceType.Notes);
                    ProcessIndex(ReferenceType.ProductVersions);
                    ProcessIndex(ReferenceType.WaterCooler);
                    ProcessIndex(ReferenceType.Organizations);
                    ProcessIndex(ReferenceType.Contacts);
                    ProcessIndex(ReferenceType.Assets);
                    ProcessIndex(ReferenceType.Products);
                    ProcessIndex(ReferenceType.Tasks);
                    */
                }
                catch (Exception ex)
                {
                    Logs.WriteEvent("Error processing indexes");
                    Logs.WriteException(ex);
                    ExceptionLogs.LogException(LoginUser, ex, "Indexer2", "Error processing indexes");
                }
            }

        }

        private void ProcessIndex(ReferenceType referenceType)
        {
            if (IsStopped) return;
            string indexPath = string.Empty;
            string storedFields = string.Empty;
            string tableName = string.Empty;
            string primaryKeyName = string.Empty;

            IndexDataSource2 indexDataSource = null;

            switch (referenceType)
            {
                case ReferenceType.Tickets:
                    indexPath = "\\Tickets";
                    storedFields = "TicketID OrganizationID TicketNumber Name IsKnowledgeBase Status Severity DateModified DateCreated DateClosed SlaViolationDate SlaWarningDate";
                    tableName = "Tickets";
                    primaryKeyName = "TicketID";
                    //indexDataSource = new TicketIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                    /*
                case ReferenceType.Wikis:
                    indexPath = "\\Wikis";
                    storedFields = "OrganizationID Creator Modifier";
                    tableName = "WikiArticles";
                    primaryKeyName = "ArticleID";
                    indexDataSource = new WikiIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                case ReferenceType.Notes:
                    indexPath = "\\Notes";
                    tableName = "Notes";
                    primaryKeyName = "NoteID";
                    indexDataSource = new NoteIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                case ReferenceType.ProductVersions:
                    indexPath = "\\ProductVersions";
                    tableName = "ProductVersions";
                    primaryKeyName = "ProductVersionID";
                    indexDataSource = new ProductVersionIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                case ReferenceType.WaterCooler:
                    indexPath = "\\WaterCooler";
                    tableName = "WatercoolerMsg";
                    primaryKeyName = "MessageID";
                    indexDataSource = new WaterCoolerIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                case ReferenceType.Organizations:
                    indexPath = "\\Customers";
                    storedFields = "Name JSON";
                    tableName = "Organizations";
                    primaryKeyName = "OrganizationID";
                    indexDataSource = new CustomerIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                case ReferenceType.Contacts:
                    indexPath = "\\Contacts";
                    storedFields = "Name JSON";
                    tableName = "Users";
                    primaryKeyName = "UserID";
                    indexDataSource = new ContactIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                case ReferenceType.Assets:
                    indexPath = "\\Assets";
                    storedFields = "Name JSON";
                    tableName = "Assets";
                    primaryKeyName = "AssetID";
                    indexDataSource = new AssetIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                case ReferenceType.Products:
                    indexPath = "\\Products";
                    storedFields = "Name JSON";
                    tableName = "Products";
                    primaryKeyName = "ProductID";
                    indexDataSource = new ProductIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                case ReferenceType.Tasks:
                    indexPath = "\\Tasks";
                    storedFields = "Name JSON";
                    tableName = "Tasks";
                    primaryKeyName = "TaskID";
                    indexDataSource = new TaskIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                    */
                default:
                    throw new System.ArgumentException("ReferenceType " + referenceType.ToString() + " is not supported by indexer.");
            }

            /*
            string root = Settings.ReadString("Tickets Index Path", "c:\\Indexes");
            string mainIndexPath = Path.Combine(root, organization.OrganizationID.ToString() + indexPath);
            string path = Path.Combine(Settings.ReadString("Tickets Index Path", "c:\\Indexes"), organization.OrganizationID.ToString() + indexPath);
            LogVerbose("Path: " + path);

            bool isNew = !System.IO.Directory.Exists(path);

            if (isNew)
            {
                Directory.CreateDirectory(path);
                LogVerbose("Creating path: " + path);
            }

            try
            {
                RemoveOldIndexItems(LoginUser, path, organization, referenceType, deletedIndexItemsFileName);
            }
            catch (Exception ex)
            {
                Logs.WriteException(ex);
                ExceptionLogs.LogException(LoginUser, ex, "Indexer.RemoveOldIndexItems - " + referenceType.ToString() + " - " + organization.OrganizationID.ToString());
            }

            string noiseFile = Path.Combine(root, "noise.dat");
            if (!File.Exists(noiseFile))
            {
                File.Create(noiseFile).Dispose();
            }

            Options options = new Options();
            options.TextFlags = TextFlags.dtsoTfRecognizeDates;
            options.NoiseWordFile = noiseFile;
            options.Save();
            LogVerbose("Processing " + tableName);
            using (IndexJob job = new IndexJob())
            {
                job.DataSourceToIndex = indexDataSource;

                job.IndexPath = path;
                job.ActionCreate = isNew;
                job.ActionAdd = true;
                job.CreateRelativePaths = false;
                job.StoredFields = Server.Tokenize(storedFields);
                job.IndexingFlags = IndexingFlags.dtsAlwaysAdd;
                bool doCompress = false;
                if (_threadPosition % 2 == 0 && (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday))
                {
                    IndexInfo info = new IndexInfo();
                    info = IndexJob.GetIndexInfo(path);
                    LogVerbose("Info - Doc Count:" + info.DocCount.ToString());
                    LogVerbose("Info - Obsolete:" + info.ObsoleteCount.ToString());

                    doCompress = (info.ObsoleteCount / info.DocCount) > 0.2;
                    if (doCompress)
                    {
                        job.ActionCompress = true;
                        job.ActionVerify = true;
                        LogVerbose("Compressing");
                    }
                }


                try
                {
                    job.ExecuteInThread();

                    // Monitor the job execution thread as it progresses
                    IndexProgressInfo status = new IndexProgressInfo();
                    while (job.IsThreadDone(1000, status) == false)
                    {
                        if (IsStopped)
                        {
                            job.AbortThread();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(LoginUser, ex, "Index Job Processor - " + referenceType.ToString() + " - " + organization.OrganizationID.ToString());
                    Logs.WriteException(ex);
                    throw;
                }

                if (doCompress)
                {
                    IndexInfo info = new IndexInfo();
                    info = IndexJob.GetIndexInfo(path);
                    LogVerbose("Compressed");
                    LogVerbose("Info - Doc Count:" + info.DocCount.ToString());
                    LogVerbose("Info - Obsolete:" + info.ObsoleteCount.ToString());
                }


                if (!IsStopped)
                {
                    Organization tempOrg = Organizations.GetOrganization(_loginUser, organization.OrganizationID);
                    UpdateItems(indexDataSource, tableName, primaryKeyName);
                }

            
            }
            */
        }
                
       
/*
        private void RemoveOldIndexItems(LoginUser loginUser, string indexPath, Organization organization, ReferenceType referenceType, string deletedIndexItemsFileName)
        {
            LogVerbose("Removing deleted items:  " + referenceType.ToString());
            if (!Directory.Exists(indexPath))
            {
                Logs.WriteEvent("Path does not exist:  " + indexPath);
                return;
            }
            DeletedIndexItems items = new DeletedIndexItems(loginUser);
            LogVerbose(string.Format("Retrieving deleted items:  RefType: {0}, OrgID: {1}", referenceType.ToString(), organization.OrganizationID.ToString()));
            items.LoadByReferenceType(referenceType, organization.OrganizationID);
            if (items.IsEmpty)
            {
                LogVerbose("No Items to delete");
                return;
            }


            StringBuilder builder = new StringBuilder();
            foreach (DeletedIndexItem item in items)
            {
                builder.AppendLine(item.RefID.ToString());
            }

            string fileName = Path.Combine(indexPath, deletedIndexItemsFileName);
            if (File.Exists(fileName)) File.Delete(fileName);
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                LogVerbose("Adding IDs to delete file: " + builder.ToString());
                writer.Write(builder.ToString());
            }


            LogVerbose("Deleting Items");
            using (IndexJob job = new IndexJob())
            {
                job.IndexPath = indexPath;
                job.ActionCreate = false;
                job.ActionAdd = false;
                job.ActionRemoveListed = true;
                job.ToRemoveListName = fileName;
                job.CreateRelativePaths = false;
                job.Execute();
            }

            LogVerbose("Items deleted");
            UpdateHealth();
            items.DeleteAll();
            items.Save();
            LogVerbose("Finished Removing Old Indexes - OrgID = " + organization.OrganizationID + " - " + referenceType.ToString());
        }

    */


        private void LogVerbose(string message)
        {
            if (_isVerbose) Logs.WriteEvent(message);
        }

        public override void ReleaseAllLocks()
        {
            
        }
    }




}
