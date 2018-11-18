using System;
using System.Collections.Generic;
using dtSearch.Engine;
using TeamSupport.Data;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

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

                    _isVerbose = true;// Settings.ReadBool("VerboseLogging", false);

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
            IndexProperties props = new IndexProperties(referenceType, Settings);
			int maxMessages = Settings.ReadInt("Max Records", 1000);
            ServiceMessages messages = ServiceBrokerUtils.ReadMessage(_loginUser.ConnectionString, props.QueueName, props.MessageKeyFieldName, maxMessages);

            if (messages.Updates.Any())
            {
                foreach (KeyValuePair<int, List<int>> orgTicketsList in orgTickets)
                {
                    indexDataSource2 = new TicketIndexDataSource2(LoginUser, orgTicketsList.Key, tableName, orgTicketsList.Value.ToArray(), Logs);
                    indexDataSources.Add(indexDataSource2);
                }
            }


            

            foreach (IndexDataSource2 indexDataSource in indexDataSources)
			{
                UpdateIndex(indexDataSource, tableName, storedFields, indexPath, referenceType);
			}*/
        }


        private void LogVerbose(string message)
        {
            if (_isVerbose) Logs.WriteEvent(message);
        }

        public override void ReleaseAllLocks()
        {

        }

		private void UpdateIndex(IndexDataSource2 indexDataSource, string tableName, string storedFields, string indexPath, ReferenceType referenceType)
		{
			int organizationID = indexDataSource.OrganizationID;

			string root = Settings.ReadString("Tickets Index Path", "c:\\Indexes");
			string mainIndexPath = Path.Combine(root, organizationID.ToString() + indexPath);
			string path = Path.Combine(Settings.ReadString("Tickets Index Path", "c:\\Indexes"), organizationID.ToString() + indexPath);
			LogVerbose("Path: " + path);

            // wait for lock
			while (true)
			{
				if (IndexLocks.AquireLock(path)) break; else System.Threading.Thread.Sleep(1000);
			}

			try
			{
				bool isNew = !System.IO.Directory.Exists(path);
				if (isNew) Directory.CreateDirectory(path);
				string noiseFile = Path.Combine(root, "noise.dat");
				if (!File.Exists(noiseFile)) File.Create(noiseFile).Dispose();

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
					bool doCompress = IsCompress(path);

					if (doCompress)
					{
						job.ActionCompress = true;
						job.ActionVerify = true;
						LogVerbose("Compressing");
					}

					try
					{
						job.Execute();
					}
					catch (Exception ex)
					{
						ExceptionLogs.LogException(LoginUser, ex, "Index Job Processor - " + referenceType.ToString() + " - " + organizationID.ToString());
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
				}
			}
			finally
			{
				IndexLocks.ReleaseLock(path);
			}
		}

		private bool IsCompress(string path)
		{
			bool doCompress = false;

			if (_threadPosition % 2 == 0 && (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday))
			{
				IndexInfo info = new IndexInfo();
				info = IndexJob.GetIndexInfo(path);
				LogVerbose("Info - Doc Count:" + info.DocCount.ToString());
				LogVerbose("Info - Obsolete:" + info.ObsoleteCount.ToString());

				doCompress = (info.ObsoleteCount / info.DocCount) > 0.2;
			}

			return doCompress;
		}


        private class IndexProperties
        {
            public string IndexPath { get; set; }
            public string StoredFields { get; set; }
            public string TableName { get; set; }
            public string PrimaryKeyName { get; set; }
            public string QueueName { get; set; }
            public string MessageKeyFieldName { get; set; }

            public IndexProperties(ReferenceType refType, Settings settings)
            {
                switch (refType)
                {
                    case ReferenceType.Tickets:
                        IndexPath = "\\Tickets";
                        StoredFields = "TicketID OrganizationID TicketNumber Name IsKnowledgeBase Status Severity DateModified DateCreated DateClosed SlaViolationDate SlaWarningDate";
                        TableName = "Tickets";
                        PrimaryKeyName = "TicketID";
                        QueueName = settings.ReadString("TicketsBrokerQueue", "TicketIndexReceiverQ");
                        MessageKeyFieldName = "TicketID";
                        break;

                    case ReferenceType.Wikis:
                        IndexPath = "\\Wikis";
                        StoredFields = "OrganizationID Creator Modifier";
                        TableName = "WikiArticles";
                        PrimaryKeyName = "ArticleID";
                        QueueName = settings.ReadString("WikiIndexReceiverQ", "WikiIndexReceiverQ");
                        MessageKeyFieldName = "ArticleID";
                        break;
                    case ReferenceType.Notes:
                        IndexPath = "\\Notes";
                        StoredFields = "";
                        TableName = "Notes";
                        PrimaryKeyName = "NoteID";
                        QueueName = settings.ReadString("NoteIndexReceiverQ", "NoteIndexReceiverQ");
                        MessageKeyFieldName = "NoteID";
                        break;
                    case ReferenceType.ProductVersions:
                        IndexPath = "\\ProductVersions";
                        StoredFields = "";
                        TableName = "ProductVersions";
                        PrimaryKeyName = "ProductVersionID";
                        QueueName = settings.ReadString("ProductVersionIndexReceiverQ", "ProductVersionIndexReceiverQ");
                        MessageKeyFieldName = "ProductVersionID";
                        break;
                    case ReferenceType.WaterCooler:
                        IndexPath = "\\WaterCooler";
                        StoredFields = "";
                        TableName = "WatercoolerMsg";
                        PrimaryKeyName = "MessageID";
                        QueueName = settings.ReadString("WaterCoolerIndexReceiverQ", "WaterCoolerIndexReceiverQ");
                        MessageKeyFieldName = "MessageID";
                        break;
                    case ReferenceType.Organizations:
                        IndexPath = "\\Customers";
                        StoredFields = "Name JSON";
                        TableName = "Organizations";
                        PrimaryKeyName = "OrganizationID";
                        QueueName = settings.ReadString("CustomerIndexReceiverQ", "CustomerIndexReceiverQ");
                        MessageKeyFieldName = "CustomerID";
                        break;
                    case ReferenceType.Contacts:
                        IndexPath = "\\Contacts";
                        StoredFields = "Name JSON";
                        TableName = "Users";
                        PrimaryKeyName = "UserID";
                        QueueName = settings.ReadString("ContactIndexReceiverQ", "ContactIndexReceiverQ");
                        MessageKeyFieldName = "UserID";
                        break;
                    case ReferenceType.Assets:
                        IndexPath = "\\Assets";
                        StoredFields = "Name JSON";
                        TableName = "Assets";
                        PrimaryKeyName = "AssetID";
                        QueueName = settings.ReadString("AssetIndexReceiverQ", "AssetIndexReceiverQ");
                        MessageKeyFieldName = "AssetID";
                        break;
                    case ReferenceType.Products:
                        IndexPath = "\\Products";
                        StoredFields = "Name JSON";
                        TableName = "Products";
                        PrimaryKeyName = "ProductID";
                        QueueName = settings.ReadString("ProductIndexReceiverQ", "ProductIndexReceiverQ");
                        MessageKeyFieldName = "ProductID";
                        break;
                    case ReferenceType.Tasks:
                        IndexPath = "\\Tasks";
                        StoredFields = "Name JSON";
                        TableName = "Tasks";
                        PrimaryKeyName = "TaskID";
                        QueueName = settings.ReadString("TaskIndexReceiverQ", "TaskIndexReceiverQ");
                        MessageKeyFieldName = "TaskID";
                        break;

                    default:
                        throw new System.ArgumentException("ReferenceType " + refType.ToString() + " is not supported by indexer.");
                }
            }
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




                }








            }
