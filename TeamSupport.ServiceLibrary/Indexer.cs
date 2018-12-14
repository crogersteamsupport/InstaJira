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
    public class Indexer : ServiceThreadPoolProcess
    {

        private static object _staticLock = new object();

        private static Organization GetNextOrganization(string connectionString, int lockID, bool isRebuilder, int? daysSinceLastRebuild = null, int? minutesSinceLastActive = null)
        {
            Organization result = null;
            lock (_staticLock)
            {
                LoginUser loginUser = new LoginUser(connectionString, -1, -1, null);
                Organizations orgs = new Organizations(loginUser);
                if (!isRebuilder)
                {
                    orgs.LoadByNeedsIndexing();
                    result = orgs.IsEmpty ? null : orgs[0];
                    if (result != null)
                    {
                        SqlCommand command = new SqlCommand();
                        command.CommandText = "UPDATE Organizations SET IsIndexLocked = 1, DateLastIndexed = GETUTCDATE() WHERE OrganizationID = @OrganizationID";
                        command.Parameters.AddWithValue("OrganizationID", result.OrganizationID);
                        SqlExecutor.ExecuteNonQuery(loginUser, command);

                    }
                }
                else
                {
                    int days = daysSinceLastRebuild ?? 14;
                    if (DateTime.Now.Hour > 6)
                    {
                        days = 100;
                    }

                    orgs.LoadByNeedsIndexRebuilt(minutesSinceLastActive ?? 30, days);
                    result = orgs.IsEmpty ? null : orgs[0];
                    if (result != null)
                    {
                        SqlCommand command = new SqlCommand();
                        command.CommandText = "UPDATE Organizations SET IsRebuildingIndex = 1 WHERE OrganizationID = @OrganizationID";
                        command.Parameters.AddWithValue("OrganizationID", result.OrganizationID);
                        SqlExecutor.ExecuteNonQuery(loginUser, command);

                    }
                }
            }

            return result;
        }

        private bool GetIsRebuilderMode()
        {
            return Settings.ReadInt("RebuilderMode", 0) == 1;
        }

        private bool _isVerbose = false;
        private int _idToLog = 0;

        private void UnlockIndex(int organizationID)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "UPDATE Organizations SET IsIndexLocked = 0 WHERE OrganizationID = @OrganizationID";
            command.Parameters.AddWithValue("OrganizationID", organizationID);
            int attempts = 0;
            while (attempts < 3)
            {
                try
                {
                    SqlExecutor.ExecuteNonQuery(LoginUser, command);
                    LogVerbose("Unlocked index for " + organizationID.ToString());
                    break;
                }
                catch (Exception e)
                {
                    attempts++;
                    if (attempts == 3)
                    {
                        throw;
                    }
                }
            }
        }

        private void UnmarkIndexRebuild(int organizationID)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "UPDATE Organizations SET IsRebuildingIndex = 0 WHERE OrganizationID = @OrganizationID";
            command.Parameters.AddWithValue("OrganizationID", organizationID);
            SqlExecutor.ExecuteNonQuery(LoginUser, command);
            LogVerbose("Unmarked index rebuild for " + organizationID.ToString());
        }

        private bool ObtainLock(int organizationID)
        {
            bool result = false;
            lock (_staticLock)
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = "UPDATE Organizations SET IsIndexLocked = 1 WHERE IsIndexLocked = 0 AND OrganizationID = @OrganizationID";
                command.Parameters.AddWithValue("OrganizationID", organizationID);
                result = SqlExecutor.ExecuteNonQuery(LoginUser, command) > 0;
                LogVerbose("Locked index for " + organizationID.ToString());
            }
            return result;
        }

        public override void ReleaseAllLocks()
        {
            if (GetIsRebuilderMode())
            {
                SqlExecutor.ExecuteNonQuery(LoginUser, "UPDATE Organizations SET IsRebuildingIndex = 0 WHERE IsRebuildingIndex=1");
            }
            else
            {
                SqlExecutor.ExecuteNonQuery(LoginUser, "UPDATE Organizations SET IsIndexLocked = 0 WHERE IsIndexLocked=1");
            }
        }

        public override void Run()
        {
            bool isRebuilder = GetIsRebuilderMode();
            int daysSinceLastRebuild = Settings.ReadInt("DaysSinceLastRebuild", 14);
            int minutesSinceLastActive = Settings.ReadInt("MinutesSinceLastActive", 30);

            while (!IsStopped)
            {
                try
                {
                    UpdateHealth();
                    Organization organization = GetNextOrganization(LoginUser.ConnectionString, (int)_threadPosition, isRebuilder, daysSinceLastRebuild, minutesSinceLastActive);
                    if (organization == null)
                    {
                        System.Threading.Thread.Sleep(10000);
                        continue;
                    }

                    try
                    {
                        _isVerbose = Settings.ReadBool("VerboseLogging", false);
                        int idToLog = Settings.ReadInt("VerboseLoggingOrg", 0);
                        if (_isVerbose && idToLog > 0)
                        {
                            _isVerbose = organization.OrganizationID == idToLog;
                        }

                        ProcessOrganization(organization, isRebuilder);
                    }
                    finally
                    {
                        if (isRebuilder)
                        {
                            UnmarkIndexRebuild(organization.OrganizationID);
                        }
                        else
                        {
                            UnlockIndex(organization.OrganizationID);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logs.WriteEvent("Error processing organization");
                    Logs.WriteException(ex);
                    ExceptionLogs.LogException(LoginUser, ex, "Indexer", "Error processing organization");
                }
            }

        }

        private void ProcessOrganization(Organization org, bool isRebuilder)
        {

            try
            {
                LogVerbose("");
                LogVerbose("");
                if (isRebuilder)
                {
                    LogVerbose(string.Format("Started rebuilding index for org: {0}", org.OrganizationID.ToString()));
                }
                else
                {
                    LogVerbose(string.Format("Started Indexing for org: {0}", org.OrganizationID.ToString()));
                }

                ProcessIndex(org, ReferenceType.Tickets, isRebuilder);
                ProcessIndex(org, ReferenceType.Wikis, isRebuilder);
                ProcessIndex(org, ReferenceType.Notes, isRebuilder);
                ProcessIndex(org, ReferenceType.ProductVersions, isRebuilder);
                ProcessIndex(org, ReferenceType.WaterCooler, isRebuilder);
                ProcessIndex(org, ReferenceType.Organizations, isRebuilder);
                ProcessIndex(org, ReferenceType.Contacts, isRebuilder);
                ProcessIndex(org, ReferenceType.Assets, isRebuilder);
                ProcessIndex(org, ReferenceType.Products, isRebuilder);
                ProcessIndex(org, ReferenceType.Tasks, isRebuilder);
                LogVerbose("Finished Processing");

                if (isRebuilder && !IsStopped)
                {
                    org.LastIndexRebuilt = DateTime.UtcNow;
                    LogVerbose("LastIndexRebuilt Updated");
                    org.Collection.Save();
                }
            }
            catch (Exception ex)
            {
                Logs.WriteException(ex);
                ExceptionLogs.LogException(LoginUser, ex, "Indexer");
            }
        }

        private void ProcessIndex(Organization organization, ReferenceType referenceType, bool isRebuilder)
        {
            if (IsStopped) return;
            string indexPath = string.Empty;
            string deletedIndexItemsFileName = string.Empty;
            string storedFields = string.Empty;
            string tableName = string.Empty;
            string primaryKeyName = string.Empty;

            IndexDataSource indexDataSource = null;
            int maxRecords = Settings.ReadInt("Max Records", 1000);

            switch (referenceType)
            {
                case ReferenceType.Tickets:
                    indexPath = "\\Tickets";
                    deletedIndexItemsFileName = "DeletedTickets.txt";
                    storedFields = "TicketID OrganizationID TicketNumber Name IsKnowledgeBase Status Severity DateModified DateCreated DateClosed SlaViolationDate SlaWarningDate";
                    tableName = "Tickets";
                    primaryKeyName = "TicketID";
                    indexDataSource = new TicketIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                case ReferenceType.Wikis:
                    indexPath = "\\Wikis";
                    deletedIndexItemsFileName = "DeletedWikis.txt";
                    storedFields = "OrganizationID Creator Modifier";
                    tableName = "WikiArticles";
                    primaryKeyName = "ArticleID";
                    indexDataSource = new WikiIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                case ReferenceType.Notes:
                    indexPath = "\\Notes";
                    deletedIndexItemsFileName = "DeletedNotes.txt";
                    tableName = "Notes";
                    primaryKeyName = "NoteID";
                    indexDataSource = new NoteIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                case ReferenceType.ProductVersions:
                    indexPath = "\\ProductVersions";
                    deletedIndexItemsFileName = "DeletedProductVersions.txt";
                    tableName = "ProductVersions";
                    primaryKeyName = "ProductVersionID";
                    indexDataSource = new ProductVersionIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                case ReferenceType.WaterCooler:
                    indexPath = "\\WaterCooler";
                    deletedIndexItemsFileName = "DeletedWaterCoolerMessages.txt";
                    tableName = "WatercoolerMsg";
                    primaryKeyName = "MessageID";
                    indexDataSource = new WaterCoolerIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                case ReferenceType.Organizations:
                    indexPath = "\\Customers";
                    deletedIndexItemsFileName = "DeletedCustomers.txt";
                    storedFields = "Name JSON";
                    tableName = "Organizations";
                    primaryKeyName = "OrganizationID";
                    indexDataSource = new CustomerIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                case ReferenceType.Contacts:
                    indexPath = "\\Contacts";
                    deletedIndexItemsFileName = "DeletedContacts.txt";
                    storedFields = "Name JSON";
                    tableName = "Users";
                    primaryKeyName = "UserID";
                    indexDataSource = new ContactIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                case ReferenceType.Assets:
                    indexPath = "\\Assets";
                    deletedIndexItemsFileName = "DeletedAssets.txt";
                    storedFields = "Name JSON";
                    tableName = "Assets";
                    primaryKeyName = "AssetID";
                    indexDataSource = new AssetIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                case ReferenceType.Products:
                    indexPath = "\\Products";
                    deletedIndexItemsFileName = "DeletedProducts.txt";
                    storedFields = "Name JSON";
                    tableName = "Products";
                    primaryKeyName = "ProductID";
                    indexDataSource = new ProductIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                case ReferenceType.Tasks:
                    indexPath = "\\Tasks";
                    deletedIndexItemsFileName = "DeletedTasks.txt";
                    storedFields = "Name JSON";
                    tableName = "Tasks";
                    primaryKeyName = "TaskID";
                    indexDataSource = new TaskIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, tableName, isRebuilder, Logs);
                    break;
                default:
                    throw new System.ArgumentException("ReferenceType " + referenceType.ToString() + " is not supported by indexer.");
            }
            string root = Settings.ReadString("Tickets Index Path", "c:\\Indexes");
            string mainIndexPath = Path.Combine(root, organization.OrganizationID.ToString() + indexPath);
            if (isRebuilder) indexPath = "\\Rebuild" + indexPath;
            string path = Path.Combine(Settings.ReadString("Tickets Index Path", "c:\\Indexes"), organization.OrganizationID.ToString() + indexPath);
            LogVerbose("Path: " + path);

            bool isNew = !System.IO.Directory.Exists(path);

            if (isNew)
            {
                Directory.CreateDirectory(path);
                LogVerbose("Creating path: " + path);
            }

            if (isRebuilder) DeleteIndex(path);

            try
            {
                if (!isRebuilder && !organization.IsRebuildingIndex) RemoveOldIndexItems(LoginUser, path, organization, referenceType, deletedIndexItemsFileName);
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
                job.ActionCreate = isNew || isRebuilder;
                job.ActionAdd = true;
                job.CreateRelativePaths = false;
                job.StoredFields = Server.Tokenize(storedFields);
                job.IndexingFlags = IndexingFlags.dtsAlwaysAdd;
                //string tempPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "TempIndexFiles" + indexPath);
                //if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
                //job.TempFileDir = tempPath;
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
                    if (!isRebuilder)
                    {
                        Organization tempOrg = Organizations.GetOrganization(_loginUser, organization.OrganizationID);
                        if (!tempOrg.IsRebuildingIndex) UpdateItems(indexDataSource, tableName, primaryKeyName);
                    }
                    else
                    {
                        MoveRebuiltIndex(organization.OrganizationID, mainIndexPath, path);
                    }
                }


            }
        }

        private void DeleteIndex(string path)
        {
            LogVerbose("Deleting Index Files: " + path);

            while (true)
            {
                int count = 0;
                try
                {
                    count++;
                    DirectoryInfo dir = new DirectoryInfo(path);
                    if (!dir.Exists) return;

                    foreach (FileInfo file in dir.GetFiles())
                    {
                        file.Delete();
                    }

                    break;
                }
                catch (Exception ex)
                {
                    Logs.WriteException(ex);
                    if (count > 3)
                    {
                        Logs.WriteEvent("Too many exceptions deleting old index. exiting...");
                        throw;
                    }
                }

            }

        }

        private void MoveRebuiltIndex(int organizationID, string indexPath, string rebuiltPath)
        {
            int count = 0;
            while (!ObtainLock(organizationID))
            {
                Logs.WriteEvent("Index Locked, trying again...");
                count++;
                System.Threading.Thread.Sleep(10000);
                if (count > 100)
                {
                    Exception ex = new Exception("Could not obain index lock to move rebuilt index");
                    ExceptionLogs.LogException(LoginUser, ex, "Index Rebuilder");
                    Logs.WriteException(ex);
                    return;
                }
            }
            try
            {
                try
                {
                    DeleteIndex(indexPath);

                    if (Directory.Exists(indexPath))
                    {
                        LogVerbose("Deleting: " + indexPath);
                        Directory.Delete(indexPath, true);
                    }

                    LogVerbose(string.Format("Moving files from {0} to {1}", rebuiltPath, indexPath));
                    Directory.Move(rebuiltPath, indexPath);
                }
                finally
                {
                    UnlockIndex(organizationID);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser, ex, "Index Rebuilder");
                Logs.WriteException(ex);
                return;
            }
        }

        private void UpdateItems(IndexDataSource dataSource, string tableName, string primaryKeyName)
        {
            UpdateHealth();

            if (dataSource.UpdatedItems.Count < 1) return;

            string updateSql = "UPDATE " + tableName + " SET NeedsIndexing = 0 WHERE " + primaryKeyName + " IN (" + DataUtils.IntArrayToCommaString(dataSource.UpdatedItems.ToArray()) + ")";
            LogVerbose(updateSql);
            SqlCommand command = new SqlCommand();
            command.CommandText = updateSql;
            command.CommandType = System.Data.CommandType.Text;

            SqlExecutor.ExecuteNonQuery(LoginUser, command);
            LogVerbose(tableName + " Indexes Statuses UPdated");
        }

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
        private void LogVerbose(string message)
        {
            if (_isVerbose) Logs.WriteEvent(message);
        }
    }




}
