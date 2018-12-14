using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using System.IO;
using System.Data.SqlClient;
using System.Net.Mail;
using LumenWorks.Framework.IO.Csv;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.Security;

namespace TeamSupport.ServiceLibrary
{
    [Serializable]
    public class ImportProcessor : ServiceThread
    {
        const int BULK_LIMIT = 1000;

        private int _organizationID;
        private LoginUser _importUser;
        private CsvReader _csv;
        private ImportFieldsView _map;
        private string[] _headers;
        private ImportLog _importLog;

        public override void Run()
        {
            Imports imports = new Imports(LoginUser);
            imports.LoadWaiting();
            System.Threading.Thread healthThread = new System.Threading.Thread(new System.Threading.ThreadStart(KeepHealthUpdated));

            try
            {
                if (!imports.IsEmpty)
                {
                    _importUser = new Data.LoginUser(LoginUser.ConnectionString, -5, imports[0].OrganizationID, null);
                    string path = TeamSupport.Data.Quarantine.ServiceQ.GetAttachmentPath14(_importUser, imports[0].OrganizationID);
                    //string logPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Logs");
                    //logPath = Path.Combine(logPath, imports[0].OrganizationID.ToString());
                    _importLog = new ImportLog(path, imports[0].ImportID);
                    _importLog.Write("Importing importID: " + imports[0].ImportID.ToString());
                    _importLog.Write("Revision 3868");

                    healthThread.Start();
                    ProcessImport(imports[0]);
                }
            }
            catch (Exception ex)
            {
                _importLog.Write(ex.Message + " " + ex.StackTrace);
                Logs.WriteException(ex);
                ExceptionLogs.LogException(LoginUser, ex, "ImportProcessor");
            }
            finally
            {
                ClearEmails();
                healthThread.Abort();
            }
        }

        private void UpdateImportCount(Import import, int count)
        {
            import.CompletedRows = count;
            import.Collection.Save();
        }

        private void ProcessImport(Import import)
        {
            Logs.WriteLine();
            Logs.WriteEvent("***********************************************************************************");
            Logs.WriteEvent("Processing Import  ImportID: " + import.ImportID.ToString());
            Logs.WriteData(import.Row);

            //string csvFile = "U:\\Development\\Imports\\TestFiles\test.csv"; // Path.Combine(path, import.FileName);
            string csvFile = TeamSupport.Data.Quarantine.ServiceQ.GetAttachmentPath15(_importUser, import.OrganizationID, import.FilePathID, import.FileName);
            _organizationID = import.OrganizationID;

            try
            {
                import.TotalRows = GetTotalRows(csvFile);
            }
            catch (Exception ex)
            {
                _importLog.Write(ex.Message + " " + ex.StackTrace);
            }
            import.CompletedRows = 0;
            import.IsRunning = true;
            import.DateStarted = DateTime.UtcNow;
            import.Collection.Save();

            _map = new ImportFieldsView(_importUser);
            _map.LoadByImportID(import.ImportID);

            using (_csv = new CsvReader(new StreamReader(csvFile), true))
            {
                _headers = _csv.GetFieldHeaders();
                switch (import.RefType)
                {
                    case ReferenceType.Actions:
                        ImportActions(import);
                        break;
                    case ReferenceType.Assets:
                        ImportAssets(import);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportCustomFields(import.RefType, import.ImportID);
                        break;
                    case ReferenceType.Organizations:
                        ImportCompanies(import);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportCustomFields(import.RefType, import.ImportID);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportAddresses(import, ReferenceType.Organizations, false);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportPhoneNumbers(import, ReferenceType.Organizations, false);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportParentCompanies(import);
                        break;
                    case ReferenceType.CompanyAddresses:
                        ImportAddresses(import, ReferenceType.Organizations, true);
                        break;
                    case ReferenceType.CompanyPhoneNumbers:
                        ImportPhoneNumbers(import, ReferenceType.Organizations, true);
                        break;
                    case ReferenceType.Contacts:
                        //ImportCompanies(import);
                        //_csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportContacts(import);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportCustomFields(import.RefType, import.ImportID);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportAddresses(import, ReferenceType.Users, false);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportPhoneNumbers(import, ReferenceType.Contacts, false);
                        break;
                    case ReferenceType.ContactAddresses:
                        ImportAddresses(import, ReferenceType.Users, true);
                        break;
                    case ReferenceType.ContactPhoneNumbers:
                        ImportPhoneNumbers(import, ReferenceType.Contacts, true);
                        break;
                    case ReferenceType.Tickets:
                        ImportTickets(import);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportCustomFields(import.RefType, import.ImportID);
                        //_csv = new CsvReader(new StreamReader(csvFile), true);
                        //ImportActions(import);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportOrganizationTickets(import);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportContactTickets(import);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportAssetTickets(import);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportTicketRelationships(import);
                        break;
                    case ReferenceType.OrganizationTickets:
                        ImportOrganizationTickets(import);
                        break;
                    case ReferenceType.ContactTickets:
                        ImportContactTickets(import);
                        break;
                    case ReferenceType.AssetTickets:
                        ImportAssetTickets(import);
                        break;
                    case ReferenceType.TicketRelationships:
                        ImportTicketRelationships(import);
                        break;
                    //case ReferenceType.CustomFieldPickList:
                    //  ImportCustomFieldPickList(import);
                    //  break;
                    case ReferenceType.Products:
                        ImportProducts(import);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportCustomFields(import.RefType, import.ImportID);
                        break;
                    case ReferenceType.ProductVersions:
                        ImportProductVersions(import);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportCustomFields(import.RefType, import.ImportID);
                        break;
                    case ReferenceType.Users:
                        ImportUsers(import);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportCustomFields(import.RefType, import.ImportID);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportAddresses(import, ReferenceType.Users, false, true);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportPhoneNumbers(import, ReferenceType.Users, false);
                        break;
                    case ReferenceType.OrganizationProducts:
                        ImportOrganizationProducts(import);
                        _csv = new CsvReader(new StreamReader(csvFile), true);
                        ImportCustomFields(import.RefType, import.ImportID);
                        break;
                    case ReferenceType.PrimaryContacts:
                        ImportPrimaryContacts(import);
                        break;
                    case ReferenceType.Notes:
                        ImportCustomerNotes(import);
                        break;
                    case ReferenceType.ContactNotes:
                        ImportContactNotes(import);
                        break;
                    case ReferenceType.ProductFamilies:
                        ImportProductFamilies(import);
                        break;
                    default:
                        Logs.WriteEvent("ERROR: Unknown Reference Type");
                        break;
                }
            }

            import.IsDone = true;
            import.DateEnded = DateTime.UtcNow;
            import.Collection.Save();

            Logs.WriteEvent("Finished Processing Import  ImportID: " + import.ImportID.ToString());
            Logs.WriteEvent("***********************************************************************************");
        }

        private int GetTotalRows(string csvFile)
        {
            using (CsvReader csv = new CsvReader(new StreamReader(csvFile), true))
            {
                while (csv.ReadNextRecord()) ;
                return (int)csv.CurrentRecordIndex + 1;
            }
        }

        private void ImportActions(Import import)
        {
            SortedList<string, int> userList = GetUserAndContactWithoutCompanyNameList();
            SortedList<string, int> ticketList = GetTicketList();
            SortedList<string, int> usersAndContactsFirstAndLastNameList = new SortedList<string, int>();

            ActionTypes actionTypes = new ActionTypes(_importUser);
            actionTypes.LoadAllPositions(_organizationID);

            Actions actions = new Actions(_importUser);
            int count = 0;

            while (_csv.ReadNextRecord())
            {

                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";
                int ticketID = 0;
                Ticket ticket = null;

                ticketID = ReadInt("TicketID");
                if (ticketID != 0 && !ticketList.ContainsValue(ticketID) && ticket == null)
                {
                    _importLog.Write(messagePrefix + "Skipped. No TicketID " + ticketID.ToString() + ".");
                    continue;
                }

                string ticketImportID = ReadString("TicketImportID", string.Empty);
                if (ticketID == 0 && !string.IsNullOrEmpty(ticketImportID))
                {
                    Tickets tickets = new Tickets(_importUser);
                    tickets.LoadByImportID(ticketImportID, _organizationID);
                    if (tickets.Count == 1)
                    {
                        ticket = tickets[0];
                        ticketID = ticket.TicketID;
                    }
                    else if (tickets.Count > 1)
                    {
                        _importLog.Write(messagePrefix + "Skipped. More than one ticket found matching TicketImportID " + ticketImportID + ".");
                        continue;
                    }
                    else
                    {
                        _importLog.Write(messagePrefix + "Skipped. No ticket found matching TicketImportID " + ticketImportID + ".");
                        continue;
                    }
                }
                else
                {
                    //_importLog.Write(messagePrefix + "Skipped. TicketImportID is required.");
                    //continue;
                }

                if (ticketID == 0)
                {
                    int? ticketNumber;
                    ticketNumber = ReadIntNull("TicketNumber", string.Empty);
                    if (ticketNumber != null)
                    {
                        Tickets tickets = new Tickets(_importUser);
                        tickets.LoadByTicketNumber(_organizationID, (int)ticketNumber);
                        if (tickets.Count > 1)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one ticket found matching TicketNumber " + ticketNumber.ToString() + ".");
                            continue;
                        }
                        else if (tickets.Count == 1)
                        {
                            ticket = tickets[0];
                            ticketID = ticket.TicketID;
                        }
                        else
                        {
                            _importLog.Write(messagePrefix + "Skipped. No ticket found matching TicketNumber " + ticketNumber.ToString() + ".");
                            continue;
                        }
                    }
                }

                if (ticketID == 0)
                {
                    _importLog.Write(messagePrefix + "Skipped. No ticket found.");
                    continue;
                }

                //Actions existingAction = new Actions(_importUser);
                TeamSupport.Data.Action action = null;
                //bool isUpdate = false;

                //int actionID = ReadInt("ActionID");
                //if (actionID != 0)
                //{
                //  existingAction.LoadByActionID(actionID);
                //  if (existingAction.Count > 0)
                //  {
                //    action = existingAction[0];
                //    isUpdate = true;
                //  }
                //}

                // Actions are not parent objects therefore do not need importID
                ////if (action == null)
                ////{
                //  string importID = ReadString("ActionImportID", string.Empty);
                //  if (importID != string.Empty)
                //  {
                //    //existingAction = new Actions(_importUser);
                //    //existingAction.LoadByImportID(importID, _organizationID);
                //    //if (existingAction.Count == 1)
                //    //{
                //    //  action = existingAction[0];
                //    //  isUpdate = true;
                //    //}
                //    //else if (existingAction.Count > 1)
                //    //{
                //    //  _importLog.Write("More than one action matching the importID was found");
                //    //  continue;
                //    //}
                //  }
                //  else
                //  {
                //    _importLog.Write(messagePrefix + "Skipped. ActionImportID is required.");
                //    continue;
                //  }

                ////}

                string actionType = ReadString("Type", string.Empty);
                //if (action == null)
                //{
                //if (actionType.Trim().ToLower() == "description")
                //{
                //  existingAction = new Actions(_importUser);
                //  existingAction.LoadOldestTicketDescription(ticketID);
                //  if (existingAction.Count == 1)
                //  {
                //    action = existingAction[0];
                //    isUpdate = true;
                //  }
                //}
                //}

                //if (action == null)
                //{
                action = actions.AddNewAction();
                //}

                action.SystemActionTypeID = GetSystemActionTypeID(actionType);
                action.ActionTypeID = GetActionTypeID(actionTypes, actionType);

                int creatorID = -5;
                if (Int32.TryParse(ReadString("CreatorID", action.CreatorID.ToString()), out creatorID))
                {
                    if (!userList.ContainsValue(creatorID))
                    {
                        creatorID = -5;
                    }
                }

                if (creatorID == -5)
                {
                    string creatorEmail = ReadString("CreatorEmail", string.Empty);
                    if (creatorEmail != string.Empty)
                    {
                        if (userList.TryGetValue(creatorEmail.ToUpper(), out creatorID))
                        {
                        }
                        else
                        {
                            _importLog.Write("No CreatorEmail " + creatorEmail + " found.");
                        }
                    }
                    else
                    {
                        string creatorName = ReadString("CreatorName", string.Empty);
                        {
                            if (creatorName != string.Empty)
                            {
                                if (usersAndContactsFirstAndLastNameList.Count == 0)
                                {
                                    usersAndContactsFirstAndLastNameList = GetUserAndContactNameAndLastNameList();
                                }

                                if (usersAndContactsFirstAndLastNameList.TryGetValue(creatorName.ToUpper(), out creatorID))
                                {
                                }
                                else
                                {
                                    _importLog.Write("No CreatorName " + creatorName + " found.");
                                }
                            }
                        }
                    }
                }

                action.CreatorID = creatorID;

                string desc = ConvertHtmlLineBreaks(ReadString("Description", string.Empty));
                action.Description = desc;

                DateTime? dateCreated = ReadDateNull("DateCreated", action.DateCreated.ToString());
                if (dateCreated != null)
                {
                    action.DateCreated = (DateTime)dateCreated;
                }
                action.DateModified = DateTime.UtcNow;
                action.DateStarted = ReadDateNull("DateStarted", action.DateStarted.ToString());
                action.IsKnowledgeBase = ReadBool("Knowledgebase", action.IsKnowledgeBase.ToString());
                action.ActionSource = ReadString("ActionSource", "Import");
                action.IsVisibleOnPortal = ReadBool("Visible", action.IsVisibleOnPortal.ToString());
                action.ModifierID = -5;
                action.Name = "";
                action.TicketID = ticketID;
                action.ImportID = ReadString("ActionImportID", string.Empty);
                action.TimeSpent = ReadIntNull("MinutesSpent", action.TimeSpent.ToString());

                action.Pinned = ReadBool("IsPinned", action.Pinned.ToString());
                action.ImportFileID = import.ImportID;

                _importLog.Write(messagePrefix + "Action added to bulk insert.");
                count++;

                if (count % BULK_LIMIT == 0)
                {
                    actions.BulkSave();
                    actions = new Actions(_importUser);
                    UpdateImportCount(import, count);
                }
            }
            actions.BulkSave();
            UpdateImportCount(import, count);
            _importLog.Write(count.ToString() + " actions imported.");
        }

        private void ImportAssets(Import import)
        {
            Products products = new Products(_importUser);
            products.LoadByOrganizationID(_organizationID);
            //IdList productIDs = GetIdList(products);

            ProductVersions productVersions = new ProductVersions(_importUser);
            productVersions.LoadByParentOrganizationID(_organizationID);

            Assets assets = new Assets(_importUser);
            //int orgCount = 0;
            //int prodCount = 0;

            SortedList<string, int> userList = GetUserAndContactList();
            Users contacts = new Users(_importUser);

            int count = 0;
            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";
                //_currentRow = row;
                //string organizationID = row["AssignedTo"].ToString().Trim().ToLower();

                Product product = null;
                int productID = ReadInt("ProductID");
                if (productID != 0)
                {
                    product = products.FindByProductID(productID);
                    if (product == null)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No product ID: " + productID.ToString() + " found.");
                        continue;
                    }
                }

                string productImportID = ReadString("ProductImportID", string.Empty);
                if (product == null && productImportID != string.Empty)
                {
                    product = products.FindByImportID(productImportID);
                    if (product == null)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No product import ID " + productImportID + " found.");
                        continue;
                    }
                }

                string productName = ReadString("ProductName", string.Empty);
                if (product == null && productName != string.Empty)
                {
                    product = products.FindByName(productName);
                    if (product == null)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No product " + productName + " found.");
                        continue;
                    }
                }

                if (product == null)
                {
                    _importLog.Write(messagePrefix + "Skipped. Product required.");
                    continue;
                }

                //Assets existingAsset = new Assets(_importUser);
                Asset asset = null;
                //bool isUpdate = false;

                //int assetID = ReadInt("AssetID");
                //if (assetID != 0)
                //{
                //  existingAsset.LoadByAssetID(assetID);
                //  if (existingAsset.Count > 0)
                //  {
                //    asset = existingAsset[0];
                //    isUpdate = true;
                //  }
                //}

                //if (asset == null)
                //{
                string importID = ReadString("AssetImportID", string.Empty);
                if (importID != string.Empty)
                {
                    //existingAsset = new Assets(_importUser);
                    //existingAsset.LoadByImportID(importID, _organizationID);
                    //if (existingAsset.Count == 1)
                    //{
                    //  asset = existingAsset[0];
                    //  isUpdate = true;
                    //}
                    //else if (existingAsset.Count > 1)
                    //{
                    //  _importLog.Write("More than one asset already exists in the database with given importID");
                    //  continue;
                    //}
                }
                else
                {
                    _importLog.Write(messagePrefix + "Skipped. AssetImportID is required.");
                    continue;
                }
                //}

                string location = "2";
                switch (ReadString("Location", location).ToLower().Trim())
                {
                    case "1":
                    case "assigned": location = "1"; break;
                    case "2":
                    case "warehouse": location = "2"; break;
                    case "3":
                    case "junkyard": location = "3"; break;
                    default:
                        break;
                }

                Assets newAssignedAsset = new Assets(_importUser);
                //if (asset == null)
                //{
                if (location == "1")
                {
                    asset = newAssignedAsset.AddNewAsset();
                }
                else
                {
                    asset = assets.AddNewAsset();
                }
                //}

                asset.OrganizationID = _organizationID;
                asset.SerialNumber = ReadString("SerialNumber", string.Empty);
                asset.Name = ReadString("Name", asset.Name);
                asset.Location = location;
                asset.Notes = ReadString("Notes", asset.Notes);
                asset.ProductID = product.ProductID;

                DateTime? warrantyExipration = ReadDateNull("WarrantyExpiration", asset.WarrantyExpiration.ToString());
                if (warrantyExipration != null)
                {
                    asset.WarrantyExpiration = (DateTime)warrantyExipration;
                }
                DateTime? dateCreated = ReadDateNull("DateCreated", asset.WarrantyExpiration.ToString());
                if (dateCreated != null)
                {
                    asset.DateCreated = (DateTime)dateCreated;
                }
                asset.DateModified = DateTime.UtcNow;

                int creatorID = -5;
                if (Int32.TryParse(ReadString("CreatorID", "-5"), out creatorID))
                {
                    if (!userList.ContainsValue(creatorID))
                    {
                        creatorID = -5;
                    }
                }
                asset.CreatorID = creatorID;
                asset.ModifierID = -5;
                asset.SubPartOf = null;
                //asset.Status = this is a deprecated field
                asset.ImportID = importID;
                asset.ImportFileID = import.ImportID;

                ProductVersion productVersion = null;
                int productVersionID = ReadInt("ProductVersionID");
                if (productVersionID != 0)
                {
                    productVersion = productVersions.FindByProductVersionID(productVersionID);
                    if (productVersion == null)
                    {
                        _importLog.Write(messagePrefix + "No product version ID: " + productVersionID.ToString() + " found.");
                    }
                }

                string productVersionImportID = ReadString("ProductVersionImportID", string.Empty);
                if (productVersion == null && productVersionImportID != string.Empty)
                {
                    productVersion = productVersions.FindByImportID(productVersionImportID);
                    if (productVersion == null)
                    {
                        _importLog.Write(messagePrefix + "No product version import ID " + productVersionImportID + " found.");
                    }
                }

                string productVersionNumber = ReadString("ProductVersion", string.Empty);
                if (productVersion == null && productVersionNumber != string.Empty)
                {
                    productVersion = productVersions.FindByVersionNumber(productVersionNumber, product.ProductID);
                    if (productVersion == null)
                    {
                        _importLog.Write(messagePrefix + "No product version " + productVersionNumber + " found.");
                    }
                }

                if (productVersion != null)
                {
                    asset.ProductVersionID = productVersion.ProductVersionID;
                }

                if (asset.Location == "1")
                {
                    User contactAssignedTo = null;
                    bool contactMatchedByID = false;
                    Organizations companyAssignedTo = new Organizations(_importUser);

                    string importIDOfCompanyAssignedTo = ReadString("ImportIDOfCompanyAssignedTo", string.Empty);
                    if (!string.IsNullOrEmpty(importIDOfCompanyAssignedTo))
                    {
                        companyAssignedTo.LoadByImportID(importIDOfCompanyAssignedTo, _organizationID);
                        if (companyAssignedTo.Count == 1)
                        {
                            string importIDOfContactAssignedTo = ReadString("ImportIDOfContactAssignedTo", string.Empty);
                            string emailOfContactAssignedTo = ReadString("EmailOfContactAssignedTo", string.Empty);
                            int idOfContactAssigedTo = ReadInt("IDOfContactAssigedTo");
                            if (idOfContactAssigedTo != 0)
                            {
                                if (contacts.Count == 0)
                                {
                                    contacts.LoadContacts(_organizationID, true);
                                }

                                contactAssignedTo = contacts.FindByUserID(idOfContactAssigedTo);

                                if (contactAssignedTo == null)
                                {
                                    _importLog.Write(messagePrefix + "Contact found by IDOfContactAssignedTo: " + idOfContactAssigedTo.ToString() + " does not exists in account.");
                                }
                            }
                            else if (!string.IsNullOrEmpty(importIDOfContactAssignedTo))
                            {
                                if (contacts.Count == 0)
                                {
                                    contacts.LoadContacts(_organizationID, true);
                                }

                                contactAssignedTo = contacts.FindByImportID(importIDOfContactAssignedTo);
                                contactMatchedByID = true;
                            }
                            else if (!string.IsNullOrEmpty(emailOfContactAssignedTo))
                            {
                                if (contacts.Count == 0)
                                {
                                    contacts.LoadContacts(_organizationID, true);
                                }

                                contactAssignedTo = contacts.FindByEmail(emailOfContactAssignedTo);
                            }
                            else
                            {
                                DateTime? dateShipped = ReadDateNull("DateShipped", string.Empty);
                                string shippingMethod = ReadString("ShippingMethod", string.Empty);
                                if (dateShipped == null || shippingMethod == string.Empty)
                                {
                                    _importLog.Write(messagePrefix + "Skipped. DateShipped and shippingMethod are required for assigned assets.");
                                    continue;
                                }

                                DateTime validDateShipped = (DateTime)dateShipped;

                                asset.AssignedTo = companyAssignedTo[0].OrganizationID;
                                newAssignedAsset.Save();

                                AssetHistory assetHistory = new AssetHistory(_importUser);
                                AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();

                                DateTime now = DateTime.UtcNow;

                                assetHistoryItem.AssetID = asset.AssetID;
                                assetHistoryItem.OrganizationID = _organizationID;
                                assetHistoryItem.ActionTime = now;
                                assetHistoryItem.ActionDescription = "Asset Shipped on " + validDateShipped.Month.ToString() + "/" + validDateShipped.Day.ToString() + "/" + validDateShipped.Year.ToString();
                                assetHistoryItem.ShippedFrom = _organizationID;
                                assetHistoryItem.ShippedFromRefType = (int)ReferenceType.Organizations;
                                assetHistoryItem.ShippedTo = companyAssignedTo[0].OrganizationID;
                                assetHistoryItem.TrackingNumber = ReadString("TrackingNumber", assetHistoryItem.TrackingNumber);
                                assetHistoryItem.ShippingMethod = shippingMethod;
                                assetHistoryItem.ReferenceNum = ReadString("ReferenceNumber", assetHistoryItem.ReferenceNum);
                                assetHistoryItem.Comments = ReadString("Comments", assetHistoryItem.Comments);

                                assetHistoryItem.DateCreated = now;
                                assetHistoryItem.Actor = -5;
                                assetHistoryItem.RefType = (int)ReferenceType.Organizations;
                                assetHistoryItem.DateModified = now;
                                assetHistoryItem.ModifierID = -5;
                                assetHistoryItem.ImportFileID = import.ImportID;

                                assetHistory.Save();

                                AssetAssignments assetAssignments = new AssetAssignments(_importUser);
                                AssetAssignment assetAssignment = assetAssignments.AddNewAssetAssignment();

                                assetAssignment.HistoryID = assetHistoryItem.HistoryID;
                                assetAssignment.ImportFileID = import.ImportID;

                                assetAssignments.Save();

                                string description = String.Format("Assigned asset to {0}.", companyAssignedTo[0].Name);
                                ActionLogs.AddActionLog(_importUser, ActionLogType.Update, ReferenceType.Assets, asset.AssetID, description);
                            }
                        }
                        else if (companyAssignedTo.Count > 1)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one ImportID matching company found.");
                            continue;
                        }
                        else
                        {
                            _importLog.Write(messagePrefix + "Skipped. No ImportID matching company found.");
                            continue;
                        }
                    }
                    else
                    {
                        string nameOfCompanyAssignedTo = ReadString("NameOfCompanyAssignedTo", string.Empty);
                        if (!string.IsNullOrEmpty(nameOfCompanyAssignedTo))
                        {
                            companyAssignedTo.LoadByOrganizationNameActive(nameOfCompanyAssignedTo, _organizationID);
                            if (companyAssignedTo.Count == 1)
                            {
                                string importIDOfContactAssignedTo = ReadString("ImportIDOfContactAssignedTo", string.Empty);
                                string emailOfContactAssignedTo = ReadString("EmailOfContactAssignedTo", string.Empty);
                                int idOfContactAssigedTo = ReadInt("IDOfContactAssigedTo");
                                if (idOfContactAssigedTo != 0)
                                {
                                    if (contacts.Count == 0)
                                    {
                                        contacts.LoadContacts(_organizationID, true);
                                    }

                                    contactAssignedTo = contacts.FindByUserID(idOfContactAssigedTo);

                                    if (contactAssignedTo == null)
                                    {
                                        _importLog.Write(messagePrefix + "Contact found by IDOfContactAssignedTo: " + idOfContactAssigedTo.ToString() + " does not exists in account.");
                                    }
                                }
                                else if (!string.IsNullOrEmpty(importIDOfContactAssignedTo))
                                {
                                    if (contacts.Count == 0)
                                    {
                                        contacts.LoadContacts(_organizationID, true);
                                    }

                                    contactAssignedTo = contacts.FindByImportID(importIDOfContactAssignedTo);
                                    contactMatchedByID = true;
                                }
                                else if (!string.IsNullOrEmpty(emailOfContactAssignedTo))
                                {
                                    if (contacts.Count == 0)
                                    {
                                        contacts.LoadContacts(_organizationID, true);
                                    }

                                    contactAssignedTo = contacts.FindByEmail(emailOfContactAssignedTo);
                                }
                                else
                                {
                                    DateTime? dateShipped = ReadDateNull("DateShipped", string.Empty);
                                    string shippingMethod = ReadString("ShippingMethod", string.Empty);
                                    if (dateShipped == null || shippingMethod == string.Empty)
                                    {
                                        _importLog.Write(messagePrefix + "Skipped. DateShipped and shippingMethod are required for assigned assets.");
                                        continue;
                                    }

                                    DateTime validDateShipped = (DateTime)dateShipped;

                                    asset.AssignedTo = companyAssignedTo[0].OrganizationID;
                                    newAssignedAsset.Save();

                                    AssetHistory assetHistory = new AssetHistory(_importUser);
                                    AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();

                                    DateTime now = DateTime.UtcNow;

                                    assetHistoryItem.AssetID = asset.AssetID;
                                    assetHistoryItem.OrganizationID = _organizationID;
                                    assetHistoryItem.ActionTime = now;
                                    assetHistoryItem.ActionDescription = "Asset Shipped on " + validDateShipped.Month.ToString() + "/" + validDateShipped.Day.ToString() + "/" + validDateShipped.Year.ToString();
                                    assetHistoryItem.ShippedFrom = _organizationID;
                                    assetHistoryItem.ShippedFromRefType = (int)ReferenceType.Organizations;
                                    assetHistoryItem.ShippedTo = companyAssignedTo[0].OrganizationID;
                                    assetHistoryItem.TrackingNumber = ReadString("TrackingNumber", assetHistoryItem.TrackingNumber);
                                    assetHistoryItem.ShippingMethod = shippingMethod;
                                    assetHistoryItem.ReferenceNum = ReadString("ReferenceNumber", assetHistoryItem.ReferenceNum);
                                    assetHistoryItem.Comments = ReadString("Comments", assetHistoryItem.Comments);

                                    assetHistoryItem.DateCreated = now;
                                    assetHistoryItem.Actor = -5;
                                    assetHistoryItem.RefType = (int)ReferenceType.Organizations;
                                    assetHistoryItem.DateModified = now;
                                    assetHistoryItem.ModifierID = -5;
                                    assetHistoryItem.ImportFileID = import.ImportID;

                                    assetHistory.Save();

                                    AssetAssignments assetAssignments = new AssetAssignments(_importUser);
                                    AssetAssignment assetAssignment = assetAssignments.AddNewAssetAssignment();

                                    assetAssignment.HistoryID = assetHistoryItem.HistoryID;
                                    assetAssignment.ImportFileID = import.ImportID;

                                    assetAssignments.Save();

                                    string description = String.Format("Assigned asset to {0}.", companyAssignedTo[0].Name);
                                    ActionLogs.AddActionLog(_importUser, ActionLogType.Update, ReferenceType.Assets, asset.AssetID, description);
                                }
                            }
                            else if (companyAssignedTo.Count > 1)
                            {
                                _importLog.Write(messagePrefix + "Skipped. More than one name matching company found.");
                                continue;
                            }
                            else
                            {
                                _importLog.Write(messagePrefix + "Skipped. No name matching company found.");
                                continue;
                            }
                        }
                        else
                        {
                            int IDOfCompanyAssignedTo = ReadInt("IDOfCompanyAssignedTo");
                            if (IDOfCompanyAssignedTo != 0)
                            {
                                companyAssignedTo.LoadByOrganizationID(IDOfCompanyAssignedTo);
                                if (companyAssignedTo.Count == 1 && companyAssignedTo[0].ParentID == _organizationID)
                                {
                                    string importIDOfContactAssignedTo = ReadString("ImportIDOfContactAssignedTo", string.Empty);
                                    string emailOfContactAssignedTo = ReadString("EmailOfContactAssignedTo", string.Empty);
                                    int idOfContactAssigedTo = ReadInt("IDOfContactAssigedTo");
                                    if (idOfContactAssigedTo != 0)
                                    {
                                        if (contacts.Count == 0)
                                        {
                                            contacts.LoadContacts(_organizationID, true);
                                        }

                                        contactAssignedTo = contacts.FindByUserID(idOfContactAssigedTo);

                                        if (contactAssignedTo == null)
                                        {
                                            _importLog.Write(messagePrefix + "Contact found by IDOfContactAssignedTo: " + idOfContactAssigedTo.ToString() + " does not exists in account.");
                                        }
                                    }
                                    else if (!string.IsNullOrEmpty(importIDOfContactAssignedTo))
                                    {
                                        if (contacts.Count == 0)
                                        {
                                            contacts.LoadContacts(_organizationID, true);
                                        }

                                        contactAssignedTo = contacts.FindByImportID(importIDOfContactAssignedTo);
                                        contactMatchedByID = true;
                                    }
                                    else if (!string.IsNullOrEmpty(emailOfContactAssignedTo))
                                    {
                                        if (contacts.Count == 0)
                                        {
                                            contacts.LoadContacts(_organizationID, true);
                                        }

                                        contactAssignedTo = contacts.FindByEmail(emailOfContactAssignedTo);
                                    }
                                    else
                                    {
                                        DateTime? dateShipped = ReadDateNull("DateShipped", string.Empty);
                                        string shippingMethod = ReadString("ShippingMethod", string.Empty);
                                        if (dateShipped == null || shippingMethod == string.Empty)
                                        {
                                            _importLog.Write(messagePrefix + "Skipped. DateShipped and shippingMethod are required for assigned assets.");
                                            continue;
                                        }

                                        DateTime validDateShipped = (DateTime)dateShipped;

                                        asset.AssignedTo = companyAssignedTo[0].OrganizationID;
                                        newAssignedAsset.Save();

                                        AssetHistory assetHistory = new AssetHistory(_importUser);
                                        AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();

                                        DateTime now = DateTime.UtcNow;

                                        assetHistoryItem.AssetID = asset.AssetID;
                                        assetHistoryItem.OrganizationID = _organizationID;
                                        assetHistoryItem.ActionTime = now;
                                        assetHistoryItem.ActionDescription = "Asset Shipped on " + validDateShipped.Month.ToString() + "/" + validDateShipped.Day.ToString() + "/" + validDateShipped.Year.ToString();
                                        assetHistoryItem.ShippedFrom = _organizationID;
                                        assetHistoryItem.ShippedFromRefType = (int)ReferenceType.Organizations;
                                        assetHistoryItem.ShippedTo = companyAssignedTo[0].OrganizationID;
                                        assetHistoryItem.TrackingNumber = ReadString("TrackingNumber", assetHistoryItem.TrackingNumber);
                                        assetHistoryItem.ShippingMethod = shippingMethod;
                                        assetHistoryItem.ReferenceNum = ReadString("ReferenceNumber", assetHistoryItem.ReferenceNum);
                                        assetHistoryItem.Comments = ReadString("Comments", assetHistoryItem.Comments);

                                        assetHistoryItem.DateCreated = now;
                                        assetHistoryItem.Actor = -5;
                                        assetHistoryItem.RefType = (int)ReferenceType.Organizations;
                                        assetHistoryItem.DateModified = now;
                                        assetHistoryItem.ModifierID = -5;
                                        assetHistoryItem.ImportFileID = import.ImportID;

                                        assetHistory.Save();

                                        AssetAssignments assetAssignments = new AssetAssignments(_importUser);
                                        AssetAssignment assetAssignment = assetAssignments.AddNewAssetAssignment();

                                        assetAssignment.HistoryID = assetHistoryItem.HistoryID;
                                        assetAssignment.ImportFileID = import.ImportID;

                                        assetAssignments.Save();

                                        string description = String.Format("Assigned asset to {0}.", companyAssignedTo[0].Name);
                                        ActionLogs.AddActionLog(_importUser, ActionLogType.Update, ReferenceType.Assets, asset.AssetID, description);
                                    }
                                }
                                else
                                {
                                    _importLog.Write(messagePrefix + "Skipped. No company matching IDOfCompanyAssignedTo " + IDOfCompanyAssignedTo.ToString() + " was found for customfields.");
                                    continue;
                                }
                            }
                            else
                            {
                                string importIDOfContactAssignedTo = ReadString("ImportIDOfContactAssignedTo", string.Empty);
                                string emailOfContactAssignedTo = ReadString("EmailOfContactAssignedTo", string.Empty);
                                int idOfContactAssigedTo = ReadInt("IDOfContactAssignedTo");
                                if (idOfContactAssigedTo != 0)
                                {
                                    if (contacts.Count == 0)
                                    {
                                        contacts.LoadContacts(_organizationID, true);
                                    }

                                    contactAssignedTo = contacts.FindByUserID(idOfContactAssigedTo);

                                    if (contactAssignedTo == null)
                                    {
                                        _importLog.Write(messagePrefix + "Contact found by IDOfContactAssignedTo: " + idOfContactAssigedTo.ToString() + " does not exists in account.");
                                    }
                                }
                                else if (!string.IsNullOrEmpty(importIDOfContactAssignedTo))
                                {
                                    if (contacts.Count == 0)
                                    {
                                        contacts.LoadContacts(_organizationID, true);
                                    }

                                    contactAssignedTo = contacts.FindByImportID(importIDOfContactAssignedTo);
                                    contactMatchedByID = true;
                                }
                                else if (!string.IsNullOrEmpty(emailOfContactAssignedTo))
                                {
                                    if (contacts.Count == 0)
                                    {
                                        contacts.LoadContacts(_organizationID, true);
                                    }

                                    contactAssignedTo = contacts.FindByEmail(emailOfContactAssignedTo);
                                }
                                else
                                {
                                    _importLog.Write(messagePrefix + "Skipped. No company or contact info to assign to.");
                                    continue;
                                }
                            }
                        }
                    }

                    if (contactAssignedTo != null)
                    {
                        DateTime? dateShipped = ReadDateNull("DateShipped", string.Empty);
                        string shippingMethod = ReadString("ShippingMethod", string.Empty);
                        if (dateShipped == null || shippingMethod == string.Empty)
                        {
                            _importLog.Write(messagePrefix + "Skipped. DateShipped and shippingMethod are required for assigned assets.");
                            continue;
                        }

                        DateTime validDateShipped = (DateTime)dateShipped;

                        asset.AssignedTo = contactAssignedTo.UserID;
                        newAssignedAsset.Save();

                        AssetHistory assetHistory = new AssetHistory(_importUser);
                        AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();

                        DateTime now = DateTime.UtcNow;

                        assetHistoryItem.AssetID = asset.AssetID;
                        assetHistoryItem.OrganizationID = _organizationID;
                        assetHistoryItem.ActionTime = now;
                        assetHistoryItem.ActionDescription = "Asset Shipped on " + validDateShipped.Month.ToString() + "/" + validDateShipped.Day.ToString() + "/" + validDateShipped.Year.ToString();
                        assetHistoryItem.ShippedFrom = _organizationID;
                        assetHistoryItem.ShippedFromRefType = (int)ReferenceType.Organizations;
                        assetHistoryItem.ShippedTo = contactAssignedTo.UserID;
                        assetHistoryItem.TrackingNumber = ReadString("TrackingNumber", assetHistoryItem.TrackingNumber);
                        assetHistoryItem.ShippingMethod = shippingMethod;
                        assetHistoryItem.ReferenceNum = ReadString("ReferenceNumber", assetHistoryItem.ReferenceNum);
                        assetHistoryItem.Comments = ReadString("Comments", assetHistoryItem.Comments);

                        assetHistoryItem.DateCreated = now;
                        assetHistoryItem.Actor = -5;
                        assetHistoryItem.RefType = (int)ReferenceType.Contacts;
                        assetHistoryItem.DateModified = now;
                        assetHistoryItem.ModifierID = -5;
                        assetHistoryItem.ImportFileID = import.ImportID;

                        assetHistory.Save();

                        AssetAssignments assetAssignments = new AssetAssignments(_importUser);
                        AssetAssignment assetAssignment = assetAssignments.AddNewAssetAssignment();

                        assetAssignment.HistoryID = assetHistoryItem.HistoryID;
                        assetAssignment.ImportFileID = import.ImportID;

                        assetAssignments.Save();

                        string description = String.Format("Assigned asset to {0}.", contactAssignedTo.FirstLastName);
                        ActionLogs.AddActionLog(_importUser, ActionLogType.Update, ReferenceType.Assets, asset.AssetID, description);
                    }
                    else if (companyAssignedTo.Count == 0)
                    {
                        if (contactMatchedByID)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No ImportID matching assigned contact found.");
                        }
                        else
                        {
                            _importLog.Write(messagePrefix + "Skipped. No email matching assigned contact found.");
                        }
                        continue;
                    }
                }

                _importLog.Write(messagePrefix + "Asset " + importID + " added to bulk insert.");
                count++;

                if (count % BULK_LIMIT == 0)
                {
                    assets.BulkSave();
                    assets = new Assets(_importUser);
                    UpdateImportCount(import, count);
                }
            }
            assets.BulkSave();
            UpdateImportCount(import, count);
            _importLog.Write(count.ToString() + " assets imported.");
        }

        private void ImportCustomFields(ReferenceType refType, int importFileID)
        {
            SortedList<string, int> assetList = null;
            //SortedList<string, int> contactList = null;
            SortedList<string, int> ticketList = null;

            Users contacts = new Users(_importUser);
            Organizations companies = new Organizations(_importUser);
            Products allProducts = new Products(_importUser);
            ProductVersions productVersions = new ProductVersions(_importUser);

            switch (refType)
            {
                case ReferenceType.Assets:
                    assetList = GetAssetList();
                    break;
                case ReferenceType.Contacts:
                    //contactList = GetContactList();
                    contacts.LoadContacts(_organizationID, false);
                    break;
                case ReferenceType.Tickets:
                    ticketList = GetTicketList();
                    break;
                case ReferenceType.OrganizationProducts:
                    companies.LoadByParentID(_organizationID, false);
                    allProducts.LoadByOrganizationID(_organizationID);
                    productVersions.LoadByParentOrganizationID(_organizationID);
                    break;
            }

            SortedList<string, int> userList = GetUserAndContactList();
            CustomValues customValues = new CustomValues(_importUser);
            int count = 0;
            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";
                int refID = 0;
                string errorMessage = string.Empty;
                switch (refType)
                {
                    case ReferenceType.Users:
                        string userImportID = ReadString("UserImportID", string.Empty);
                        if (userImportID != string.Empty)
                        {
                            Users existingUser = new Users(_importUser);
                            existingUser.LoadByImportID(userImportID, _organizationID);
                            if (existingUser.Count == 1)
                            {
                                refID = existingUser[0].UserID;
                            }
                            else if (existingUser.Count > 1)
                            {
                                _importLog.Write(messagePrefix + "Skipped. More than one user matching UserImportID " + userImportID.ToString() + " was found for customfields.");
                                continue;
                            }
                            else
                            {
                                _importLog.Write(messagePrefix + "Skipped. No user matching UserImportID " + userImportID.ToString() + " was found for customfields.");
                                continue;
                            }
                        }
                        else
                        {
                            errorMessage = "Custom fields in row index " + _csv.CurrentRecordIndex + "could not be imported as UserImportID was not provided.";
                        }
                        break;
                    case ReferenceType.Assets:
                        string assetName = ReadString("Name", string.Empty);
                        string assetSerialNumber = ReadString("SerialNumber", string.Empty);
                        string location = ReadString("Location", string.Empty);
                        switch (location.Trim().ToLower())
                        {
                            case "assigned":
                                location = "1";
                                break;
                            case "warehouse":
                                location = "2";
                                break;
                            case "junkyard":
                                location = "3";
                                break;
                            default:
                                location = "2";
                                break;
                        }
                        string key = assetSerialNumber + assetName + location;
                        if (!assetList.TryGetValue(key.ToUpper().Replace(" ", string.Empty), out refID))
                        {
                            errorMessage = "Custom fields in row index " + _csv.CurrentRecordIndex + "could not be imported as asset " + assetName + " does not exists.";
                        }
                        break;
                    case ReferenceType.Organizations:
                        int companyID = ReadInt("CompanyID");
                        if (companyID != 0)
                        {
                            Organizations existingCompany = new Organizations(_importUser);
                            existingCompany.LoadByOrganizationID(companyID);
                            if (existingCompany.Count == 1 && existingCompany[0].ParentID == _organizationID)
                            {
                                refID = existingCompany[0].OrganizationID;
                            }
                            else
                            {
                                _importLog.Write(messagePrefix + "Skipped. No company matching CompanyID " + companyID.ToString() + " was found for customfields.");
                                continue;
                            }
                        }

                        string importID = ReadString("CompanyImportID", string.Empty);
                        if (refID == 0)
                        {
                            if (importID != string.Empty)
                            {
                                Organizations existingCompany = new Organizations(_importUser);
                                existingCompany.LoadByImportID(importID, _organizationID);
                                if (existingCompany.Count == 1)
                                {
                                    refID = existingCompany[0].OrganizationID;
                                }
                                else if (existingCompany.Count > 1)
                                {
                                    _importLog.Write(messagePrefix + "Skipped. More than one company matching the importID " + importID + " was found.");
                                    continue;
                                }
                            }
                        }

                        string companyName = ReadString("CompanyName", string.Empty);
                        if (refID == 0)
                        {
                            Organizations companiesMatchingName = new Organizations(_importUser);
                            companiesMatchingName.LoadByName(companyName, _organizationID);
                            if (companiesMatchingName.Count > 0)
                            {
                                refID = companiesMatchingName[0].OrganizationID;
                            }
                            else
                            {
                                errorMessage = "Custom fields in row index " + _csv.CurrentRecordIndex + "could not be imported as company " + companyName + " does not exists.";
                            }
                        }
                        break;
                    case ReferenceType.Contacts:
                        string contactImportID = ReadString("ContactImportID", string.Empty);
                        if (contactImportID != string.Empty)
                        {
                            //existingUser.LoadByImportID(importID, _organizationID);
                            //if (existingUser.Count == 1)
                            //{
                            //  user = existingUser[0];
                            //  oldOrganizationID = user.OrganizationID;
                            //  isUpdate = true;
                            //}
                            //else if (existingUser.Count > 1)
                            //{
                            //  _importLog.Write(messagePrefix + "Skipped. More than one user matching importID was found");
                            //  continue;
                            //}
                            User existingContact = contacts.FindByImportID(contactImportID);
                            if (existingContact != null)
                            {
                                refID = existingContact.UserID;
                            }
                            else
                            {
                                _importLog.Write(messagePrefix + "Skipped. No contact found matching contactImportID in custom fields.");
                                continue;
                            }
                        }
                        else
                        {
                            _importLog.Write(messagePrefix + "Skipped. ContactImportID is required in custom fields.");
                            continue;
                        }
                        break;
                    case ReferenceType.Tickets:
                        int ticketID = ReadInt("TicketID");
                        if (ticketID != 0)
                        {
                            if (ticketList.ContainsValue(ticketID))
                            {
                                refID = ticketID;
                            }
                            else
                            {
                                _importLog.Write(messagePrefix + "Skipped. No ticket matching TicketID: " + ticketID.ToString() + " was found processing custom fields.");
                                continue;
                            }
                        }

                        if (refID == 0)
                        {
                            importID = ReadString("TicketImportID", string.Empty);
                            if (importID != string.Empty)
                            {
                                Tickets tickets = new Tickets(_importUser);
                                tickets.LoadByImportID(importID, _organizationID);
                                if (tickets.Count > 1)
                                {
                                    _importLog.Write(messagePrefix + "Skipped. More than one ticket matching the TicketImportID " + importID + " was found.");
                                    continue;
                                }
                                else if (tickets.Count == 1)
                                {
                                    refID = tickets[0].TicketID;
                                }
                            }
                        }

                        if (refID == 0)
                        {
                            string ticketNumber = ReadString("TicketNumber", string.Empty);
                            if (!ticketList.TryGetValue(ticketNumber, out refID))
                            {
                                errorMessage = "Custom fields in row index " + _csv.CurrentRecordIndex + "could not be imported as ticket# " + ticketNumber + " does not exists.";
                            }
                        }
                        break;
                    case ReferenceType.Products:
                        string productImportID = ReadString("ProductImportID", string.Empty);
                        if (productImportID != string.Empty)
                        {
                            Products products = new Products(_importUser);
                            products.LoadByImportID(productImportID, _organizationID);
                            if (products.Count > 1)
                            {
                                errorMessage = "More than one product matching the ProductImportID " + productImportID + " was found.";
                            }
                            else if (products.Count == 1)
                            {
                                refID = products[0].ProductID;
                            }
                            else
                            {
                                errorMessage = "No product matching the ProductImportID was found.";
                            }
                        }

                        if (refID == 0)
                        {
                            string productName = ReadString("Name", string.Empty);
                            if (!string.IsNullOrEmpty(productName))
                            {
                                Products products = new Products(_importUser);
                                products.LoadByProductName(_organizationID, productName);
                                if (products.Count > 1)
                                {
                                    errorMessage = "More than one product matching the ProductName " + productName + " was found.";
                                }
                                else if (products.Count == 1)
                                {
                                    refID = products[0].ProductID;
                                }
                                else
                                {
                                    errorMessage = "No product matching the ProductName " + productName + " was found.";
                                }
                            }
                            else
                            {
                                errorMessage = "No product found matching either ProductName or ProductImportID.";
                            }
                        }
                        break;
                    case ReferenceType.ProductVersions:
                        string name = ReadString("VersionNumber", string.Empty);
                        if (name == string.Empty)
                        {
                            errorMessage = "Product version custom fields skipped due to missing VersionNumber";
                        }

                        int productID = 0;
                        productImportID = string.Empty;
                        productImportID = ReadString("ProductImportID", string.Empty);
                        if (productImportID != string.Empty)
                        {
                            Products products = new Products(_importUser);
                            products.LoadByImportID(productImportID, _organizationID);
                            if (products.Count > 1)
                            {
                                errorMessage = "More than one product matching the ProductImportID " + productImportID + " was found.";
                            }
                            else if (products.Count == 1)
                            {
                                productID = products[0].ProductID;
                            }
                            else
                            {
                                errorMessage = "No product matching the ProductImportID " + productImportID + " was found.";
                            }
                        }

                        if (productID == 0)
                        {
                            string productName = ReadString("Name", string.Empty);
                            if (!string.IsNullOrEmpty(productName))
                            {
                                Products products = new Products(_importUser);
                                products.LoadByProductName(_organizationID, productName);
                                if (products.Count > 1)
                                {
                                    errorMessage = "More than one product matching the ProductName " + productName + " was found.";
                                }
                                else if (products.Count == 1)
                                {
                                    productID = products[0].ProductID;
                                }
                            }
                        }

                        if (productID == 0)
                        {
                            productID = ReadInt("ProductID");
                            if (productID != 0)
                            {
                                Products products = new Products(_importUser);
                                products.LoadByProductID(productID);
                                if (products.Count != 1 || products[0].OrganizationID != _organizationID)
                                {
                                    errorMessage = "No product found with productID " + productID.ToString() + " provided.";
                                }
                            }
                            else
                            {
                                errorMessage = "Product is required and is missing.";
                            }
                        }

                        ProductVersions existingProductVersion = new ProductVersions(_importUser);

                        string productVersionImportID = ReadString("ProductVersionImportID", string.Empty);
                        if (productVersionImportID != string.Empty)
                        {
                            existingProductVersion.LoadByImportID(productVersionImportID, _organizationID);
                            if (existingProductVersion.Count == 1)
                            {
                                refID = existingProductVersion[0].ProductVersionID;
                            }
                            else if (existingProductVersion.Count > 1)
                            {
                                errorMessage = "More than one product version matching the ProductVerionImportID " + productVersionImportID + " was found when pulling custom fields.";
                            }
                            else
                            {
                                errorMessage = "No product version matching the ProductVerionImportID " + productVersionImportID + " was found when pulling custom fields.";
                            }
                        }

                        if (refID == 0)
                        {
                            existingProductVersion = new ProductVersions(_importUser);
                            existingProductVersion.LoadByProductIDAndVersionNumber(productID, name);
                            if (existingProductVersion.Count > 0)
                            {
                                refID = existingProductVersion[0].ProductVersionID;
                            }
                            else
                            {
                                errorMessage = "Product version is required and is missing.";
                            }
                        }
                        break;
                    case ReferenceType.OrganizationProducts:
                        int organizationProductCompanyID = ReadInt("CompanyID");
                        if (organizationProductCompanyID != 0)
                        {
                            Organization company = companies.FindByOrganizationID(organizationProductCompanyID);
                            if (company == null)
                            {
                                _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyID " + organizationProductCompanyID.ToString() + " was found.");
                                continue;
                            }
                        }

                        if (organizationProductCompanyID == 0)
                        {
                            string companyImportID = ReadString("CompanyImportID", string.Empty);
                            if (companyImportID != string.Empty)
                            {
                                Organizations company = new Organizations(_importUser);
                                company.LoadByImportID(companyImportID, _organizationID);
                                if (company.Count == 1)
                                {
                                    organizationProductCompanyID = company[0].OrganizationID;
                                }
                                else if (company.Count > 1)
                                {
                                    _importLog.Write(messagePrefix + "Skipped. More than one company matching the CompanyImportID " + companyImportID + " was found in custom fields.");
                                    continue;
                                }
                                else
                                {
                                    _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyImportID " + companyImportID + " was found in custom fields.");
                                    continue;
                                }
                            }
                        }

                        if (organizationProductCompanyID == 0)
                        {
                            string organizationProductCompanyName = ReadString("CompanyName", string.Empty);
                            if (organizationProductCompanyName != string.Empty)
                            {
                                Organization company = companies.FindByName(organizationProductCompanyName);
                                if (company == null)
                                {
                                    _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyName " + organizationProductCompanyName + " was found in customfields.");
                                    continue;
                                }
                                else
                                {
                                    organizationProductCompanyID = company.OrganizationID;
                                }
                            }
                        }

                        if (organizationProductCompanyID == 0)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No company matching either the CompanyID, the CompanyImportID or the CompanyName was found in customfields.");
                            continue;
                        }

                        int organizationProductProductID = ReadInt("ProductID");
                        if (organizationProductProductID != 0)
                        {
                            Product product = allProducts.FindByProductID(organizationProductProductID);
                            if (product == null)
                            {
                                _importLog.Write(messagePrefix + "Skipped. No product matching ProductID: " + organizationProductProductID.ToString() + " was found in custom fields.");
                                continue;
                            }
                        }

                        if (organizationProductProductID == 0)
                        {
                            string organizationProductProductImportID = ReadString("ProductImportID", string.Empty);
                            if (organizationProductProductImportID != string.Empty)
                            {
                                Product product = allProducts.FindByImportID(organizationProductProductImportID);
                                if (product == null)
                                {
                                    _importLog.Write(messagePrefix + "Skipped. No product matching the ProductImportID " + organizationProductProductImportID + " was found.");
                                    continue;
                                }
                                else
                                {
                                    organizationProductProductID = product.ProductID;
                                }
                            }
                        }

                        if (organizationProductProductID == 0)
                        {
                            string productName = ReadString("ProductName", string.Empty);
                            if (productName != string.Empty)
                            {
                                Product product = allProducts.FindByName(productName);
                                if (product == null)
                                {
                                    _importLog.Write(messagePrefix + "Skipped. No product matching ProductName " + productName + " was found.");
                                    continue;
                                }
                                else
                                {
                                    organizationProductProductID = product.ProductID;
                                }
                            }
                        }

                        if (organizationProductProductID == 0)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No product matching either the ProductID, ProductImportID or the ProductName was found.");
                            continue;
                        }

                        int productVersionID = ReadInt("ProductVersionID");
                        if (productVersionID != 0)
                        {
                            ProductVersion productVersion = productVersions.FindByProductVersionID(productVersionID);
                            if (productVersion == null)
                            {
                                _importLog.Write(messagePrefix + ". No product version matching ProductVersionID: " + productVersionID.ToString() + " was found.");
                            }
                        }

                        if (productVersionID == 0)
                        {
                            string organizationProductProductVersionImportID = ReadString("ProductVersionImportID", string.Empty);
                            if (organizationProductProductVersionImportID != string.Empty)
                            {
                                ProductVersion productVersion = productVersions.FindByImportID(organizationProductProductVersionImportID);
                                if (productVersion == null)
                                {
                                    _importLog.Write(messagePrefix + ". No product version matching the ProductVersionImportID " + organizationProductProductVersionImportID + " was found.");
                                }
                                else
                                {
                                    productVersionID = productVersion.ProductVersionID;
                                }
                            }
                        }

                        if (productVersionID == 0)
                        {
                            string productVersionNumber = ReadString("ProductVersionNumber", string.Empty);
                            if (productVersionNumber != string.Empty)
                            {
                                ProductVersion productVersion = productVersions.FindByVersionNumber(productVersionNumber, organizationProductProductID);
                                if (productVersion == null)
                                {
                                    _importLog.Write(messagePrefix + ". No product version matching ProductVersionNumber " + productVersionNumber + " was found.");
                                }
                                else
                                {
                                    productVersionID = productVersion.ProductVersionID;
                                }
                            }
                        }

                        OrganizationProducts organizationProduct = new OrganizationProducts(_importUser);
                        if (productVersionID == 0)
                        {
                            organizationProduct.LoadByOrganizationAndProductID(organizationProductCompanyID, organizationProductProductID);
                        }
                        else
                        {
                            organizationProduct.LoadByOrganizationProductIDAndVersionID(organizationProductCompanyID, organizationProductProductID, productVersionID);
                        }

                        if (organizationProduct.Count == 1)
                        {
                            refID = organizationProduct[0].OrganizationProductID;
                        }
                        else
                        {
                            _importLog.Write(messagePrefix + "Skipped. No organization product found for companyID " + organizationProductCompanyID.ToString() + ", productID " + organizationProductProductID.ToString() + " and productVersionID " + productVersionID.ToString() + " was found.");
                            continue;
                        }
                        break;
                }

                if (errorMessage != string.Empty)
                {
                    _importLog.Write(messagePrefix + errorMessage);
                    continue;
                }

                int creatorID = -5;
                if (Int32.TryParse(ReadString("CreatorID", creatorID.ToString()), out creatorID))
                {
                    if (!userList.ContainsValue(creatorID))
                    {
                        creatorID = -5;
                    }
                }
                DateTime? dateCreated = ReadDateNull("DateCreated", string.Empty);

                SimpleImportFieldsView fields = new SimpleImportFieldsView(_importUser);
                fields.LoadByRefType((int)refType);
                _importLog.Write(messagePrefix + "Processing custom fields for refID: " + refID.ToString());
                foreach (SimpleImportFieldsViewItem field in fields)
                {
                    if (Convert.ToBoolean(field.IsCustom))
                    {
                        string value = ReadString(field.FieldName, string.Empty);
                        if (!string.IsNullOrEmpty(value.Trim()))
                        {
                            //CustomValues existingCustomValue = new CustomValues(_importUser);
                            //existingCustomValue.LoadByFieldID(field.ImportFieldID, refID);
                            //if (existingCustomValue.Count > 0)
                            //{
                            //  existingCustomValue[0].Value = value;
                            //  existingCustomValue[0].ModifierID = -5;
                            //  existingCustomValue.Save();
                            //  _importLog.Write(messagePrefix + "Updated custom value of field: " + field.FieldName);
                            //}
                            //else
                            //{
                            CustomValues newCustomValues = new CustomValues(_importUser);
                            CustomValue customValue = newCustomValues.AddNewCustomValue();
                            customValue.RefID = refID;
                            customValue.Value = value;
                            customValue.CustomFieldID = field.ImportFieldID;
                            if (dateCreated != null)
                            {
                                customValue.DateCreated = (DateTime)dateCreated;
                            }
                            customValue.CreatorID = -5;
                            customValue.ModifierID = -5;
                            customValue.ImportFileID = importFileID;

                            try
                            {
                                newCustomValues.Save();
                                _importLog.Write(messagePrefix + "Added custom value of field: " + field.FieldName + ".");
                            }
                            catch (Exception e)
                            {
                                _importLog.Write(messagePrefix + "The following exception ocurred attemting to add new value for field: " + field.FieldName + " with value: " + customValue.Value + " for refID: " + refID.ToString());
                            }
                            //count++;

                            //if (count % BULK_LIMIT == 0)
                            //{
                            //  customValues.BulkSave();
                            //  count = 0;
                            //  customValues = new CustomValues(_importUser);
                            //}
                            //}
                        }
                    }
                }
            }
            //customValues.BulkSave();
        }

        private void ImportCompanies(Import import)
        {
            SortedList<string, int> userList = GetUserAndContactList();

            Organizations companies = new Organizations(_importUser);

            int count = 0;
            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";

                string name = ReadString("CompanyName", string.Empty);
                if (name == string.Empty)
                {
                    _importLog.Write(messagePrefix + "Skipped. CompanyName is required.");
                    continue;
                }

                //Organizations existingCompany = new Organizations(_importUser);
                Organization company = null;
                //bool isUpdate = false;

                //int companyID = ReadInt("CompanyID");
                //if (companyID != 0)
                //{
                //  existingCompany.LoadByOrganizationID(companyID);
                //  if (existingCompany.Count == 1 && existingCompany[0].ParentID == _organizationID)
                //  {
                //    company = existingCompany[0];
                //    isUpdate = true;
                //  }
                //  else
                //  {
                //    _importLog.Write(messagePrefix + "Skipped. No company matching CompanyID was found");
                //    continue;
                //  }
                //}

                string importID = ReadString("CompanyImportID", string.Empty);
                //if (company == null)
                //{
                if (importID != string.Empty)
                {
                    //existingCompany.LoadByImportID(importID, _organizationID);
                    //if (existingCompany.Count == 1)
                    //{
                    //  company = existingCompany[0];
                    //  isUpdate = true;
                    //}
                    //else if (existingCompany.Count > 1)
                    //{
                    //  _importLog.Write(messagePrefix + "Skipped. More than one company matching the importID was found.");
                    //  continue;
                    //}
                }
                else
                {
                    _importLog.Write(messagePrefix + "Skipped. CompanyImportID is required.");
                    continue;
                }
                //}

                //if (company == null && _organizationID != 887356)
                //{
                //  existingCompany = new Organizations(_importUser);
                //  existingCompany.LoadByName(name, _organizationID);
                //  if (existingCompany.Count > 0)
                //  {
                //    company = existingCompany[0];
                //    isUpdate = true;
                //  }
                //}

                //if (company == null)
                //{
                company = companies.AddNewOrganization();
                //}

                company.Name = name;
                company.Description = ReadString("Description", company.Description);
                company.Website = ReadString("Website", company.Website);
                company.CompanyDomains = ReadString("Domains", company.CompanyDomains);

                string emailOfDefaultSupportUser = ReadString("EmailOfDefaultSupportUser", string.Empty);
                if (!string.IsNullOrEmpty(emailOfDefaultSupportUser))
                {
                    Users defaultSupportUser = new Users(_importUser);
                    defaultSupportUser.LoadByEmail(emailOfDefaultSupportUser, _organizationID);
                    if (defaultSupportUser.Count == 1)
                    {
                        company.DefaultSupportUserID = defaultSupportUser[0].UserID;
                    }
                    else if (defaultSupportUser.Count > 1)
                    {
                        _importLog.Write(messagePrefix + "More than one user found matching email of default support user.");
                    }
                    else
                    {
                        _importLog.Write(messagePrefix + "No user found matching email of default support user.");
                    }
                }

                string defaultSupportGroupName = ReadString("DefaultSupportGroup", string.Empty);
                if (!string.IsNullOrEmpty(defaultSupportGroupName))
                {
                    Groups defaultSupportGroup = new Groups(_importUser);
                    defaultSupportGroup.LoadByGroupName(_organizationID, defaultSupportGroupName);
                    if (defaultSupportGroup.Count == 1)
                    {
                        company.DefaultSupportGroupID = defaultSupportGroup[0].GroupID;
                    }
                    else if (defaultSupportGroup.Count > 1)
                    {
                        _importLog.Write(messagePrefix + "More than one group found matching name of default support group.");
                    }
                    else
                    {
                        _importLog.Write(messagePrefix + "No group found matching name of default support group.");
                    }
                }

                //company.TimeZoneID = $("#ddlTz").val(); Not available in webapp
                company.SAExpirationDate = ReadDateNull("ServiceAgreementExpiration", company.SAExpirationDate.ToString());

                string slaName = ReadString("ServiceLevelAgreement", string.Empty);
                if (!string.IsNullOrEmpty(slaName))
                {
                    SlaLevels sla = new SlaLevels(_importUser);
                    sla.LoadByName(_organizationID, slaName);
                    if (sla.Count == 1)
                    {
                        company.SlaLevelID = sla[0].SlaLevelID;
                    }
                    else if (sla.Count > 1)
                    {
                        _importLog.Write(messagePrefix + "More than one service level agreement found matching name.");
                    }
                    else
                    {
                        _importLog.Write(messagePrefix + "No service level agreement found matching name.");
                    }
                }

                company.SupportHoursMonth = ReadInt("SupportHoursPerMonth", company.SupportHoursMonth);
                company.IsActive = ReadBool("Active", "1");
                company.HasPortalAccess = ReadBool("PortalAccess", company.HasPortalAccess.ToString());
                company.IsApiEnabled = ReadBool("APIEnabled", company.IsApiEnabled.ToString());
                company.IsApiActive = ReadBool("APIEnabled", company.IsApiActive.ToString());
                company.InActiveReason = ReadString("InactiveReason", company.InActiveReason);

                company.ExtraStorageUnits = 0;
                company.ImportID = importID;
                company.IsCustomerFree = false;
                company.ParentID = _organizationID;
                company.PortalSeats = 0;
                company.PrimaryUserID = null;
                company.ProductType = ProductType.Express;
                company.UserSeats = 0;
                company.NeedsIndexing = true;
                //if (!isUpdate)
                //{
                company.SystemEmailID = Guid.NewGuid();
                company.WebServiceID = Guid.NewGuid();
                //}
                DateTime? dateCreated = ReadDateNull("DateCreated", string.Empty);
                if (dateCreated != null)
                {
                    company.DateCreated = (DateTime)dateCreated;
                }

                int creatorID = -5;
                if (Int32.TryParse(ReadString("CreatorID", creatorID.ToString()), out creatorID))
                {
                    if (!userList.ContainsValue(creatorID))
                    {
                        creatorID = -5;
                    }
                }
                company.CreatorID = creatorID;
                company.ModifierID = -5;
                company.ImportFileID = import.ImportID;

                //if (isUpdate)
                //{
                //  existingCompany.Save();
                //  _importLog.Write(messagePrefix + "CompanyID " + company.OrganizationID.ToString() + " was updated.");
                //}
                //else
                //{
                _importLog.Write(messagePrefix + "Company " + importID + " added to bulk insert.");
                //}
                count++;

                if (count % BULK_LIMIT == 0)
                {
                    companies.BulkSave();
                    companies = new Organizations(_importUser);
                    UpdateImportCount(import, count);
                    _importLog.Write("Import set with " + count.ToString() + " companies inserted in database.");
                }
            }
            companies.BulkSave();
            UpdateImportCount(import, count);
            _importLog.Write("Import set with " + count.ToString() + " companies inserted in database.");
            //Per ticket 45431 Indexer does not process recently imported companies so we use index rebuild.
            Organizations.SetRebuildIndexes(_importUser, _importUser.OrganizationID);
        }

        private void ImportContacts(Import import)
        {
            SortedList<string, int> usersAndContactsFirstAndLastNameList = new SortedList<string, int>();
            SortedList<string, int> userList = GetUserAndContactList();

            Organization unknownCompany = Organizations.GetUnknownCompany(_loginUser, _organizationID);

            Users users = new Users(_importUser);

            int count = 0;
            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";

                string firstName = ReadString("FirstName", string.Empty);
                if (string.IsNullOrEmpty(firstName))
                {
                    _importLog.Write(messagePrefix + "Skipped. Contact skipped due to missing first name");
                    continue;
                }

                string lastName = ReadString("LastName", string.Empty);
                if (string.IsNullOrEmpty(lastName))
                {
                    _importLog.Write(messagePrefix + "Skipped. Contact skipped due to missing last name");
                    continue;
                }

                Users existingUser = new Users(_importUser);
                User user = null;
                bool isUpdate = false;
                int oldOrganizationID = 0;

                string importID = ReadString("ContactImportID", string.Empty);
                if (importID != string.Empty)
                {
                    //existingUser.LoadByImportID(importID, _organizationID);
                    //if (existingUser.Count == 1)
                    //{
                    //  user = existingUser[0];
                    //  oldOrganizationID = user.OrganizationID;
                    //  isUpdate = true;
                    //}
                    //else if (existingUser.Count > 1)
                    //{
                    //  _importLog.Write(messagePrefix + "Skipped. More than one user matching importID was found");
                    //  continue;
                    //}
                }
                else
                {
                    _importLog.Write(messagePrefix + "Skipped. ContactImportID is required.");
                    continue;
                }

                string email = ReadString("ContactEmail", string.Empty);
                //if (email != string.Empty)
                //{
                //  existingUser = new Users(_importUser);
                //  existingUser.LoadByEmail(_organizationID, email);
                //  if (existingUser.Count > 0)
                //  {
                //    user = existingUser[0];
                //    oldOrganizationID = user.OrganizationID;
                //    isUpdate = true;
                //  }
                //}

                //if (user == null)
                //{
                user = users.AddNewUser();
                //}

                DateTime? dateCreated = ReadDateNull("DateCreated", user.DateCreated.ToString());
                if (dateCreated != null)
                {
                    user.DateCreated = (DateTime)dateCreated;
                }

                Organizations companies = new Organizations(_importUser);
                Organization company = null;

                int? companyID = ReadIntNull("CompanyID", string.Empty);
                if (companyID != null)
                {
                    companies = new Organizations(_importUser);
                    companies.LoadByOrganizationID((int)companyID);
                    if (companies.Count == 1 && companies[0].ParentID == _organizationID)
                    {
                        company = companies[0];
                        user.OrganizationID = companies[0].OrganizationID;
                    }
                    else if (companies[0].ParentID != _organizationID)
                    {
                        _importLog.Write(messagePrefix + "Skipped. Invalid companyID provided.");
                        continue;
                    }
                    else if (companies.Count > 1)
                    {
                        _importLog.Write(messagePrefix + "Skipped. More than one company matching companyID found.");
                        continue;
                    }
                    else
                    {
                        _importLog.Write(messagePrefix + "Skipped. No company matching companyID found.");
                        continue;
                    }
                }

                if (company == null)
                {
                    string companyImportID = ReadString("CompanyImportID", string.Empty);
                    if (!string.IsNullOrEmpty(companyImportID))
                    {
                        companies.LoadByImportID(companyImportID, _organizationID);
                        if (companies.Count == 1)
                        {
                            company = companies[0];
                            user.OrganizationID = company.OrganizationID;
                        }
                        else if (companies.Count > 1)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one company matching companyImportID found.");
                            continue;
                        }
                        else
                        {
                            _importLog.Write(messagePrefix + "Skipped. No company matching companyImportID found.");
                            continue;
                        }
                    }
                }

                if (company == null)
                {
                    string companyName = ReadString("CompanyName", string.Empty);
                    if (!string.IsNullOrEmpty(companyName))
                    {
                        companies = new Organizations(_importUser);
                        companies.LoadByOrganizationName(companyName, _organizationID);
                        if (companies.Count == 1)
                        {
                            company = companies[0];
                            user.OrganizationID = companies[0].OrganizationID;
                        }
                        else if (companies.Count > 1)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one company matching CompanyName found.");
                            continue;
                        }
                        else
                        {
                            Organizations newCompanies = new Organizations(_importUser);
                            Organization newCompany = newCompanies.AddNewOrganization();
                            newCompany.Name = companyName;
                            newCompany.ParentID = _organizationID;
                            newCompany.IsActive = true;
                            newCompany.ExtraStorageUnits = 0;
                            newCompany.IsCustomerFree = false;
                            newCompany.PortalSeats = 0;
                            newCompany.PrimaryUserID = null;
                            newCompany.ProductType = ProductType.Express;
                            newCompany.UserSeats = 0;
                            newCompany.NeedsIndexing = true;
                            newCompany.SystemEmailID = Guid.NewGuid();
                            newCompany.WebServiceID = Guid.NewGuid();
                            if (dateCreated != null)
                            {
                                newCompany.DateCreated = (DateTime)dateCreated;
                            }
                            newCompany.CreatorID = -5;
                            newCompany.ModifierID = -5;
                            newCompany.ImportFileID = import.ImportID;

                            newCompanies.Save();
                            company = newCompany;
                            user.OrganizationID = company.OrganizationID;
                            _importLog.Write(messagePrefix + "No company matching CompanyName found. Created company " + companyName);
                        }
                    }
                }

                if (company == null)
                {
                    user.OrganizationID = unknownCompany.OrganizationID;
                }

                user.ImportID = importID;
                user.FirstName = firstName;
                user.MiddleName = ReadString("MiddleName", user.MiddleName);
                user.LastName = lastName;
                user.Title = ReadString("Title", user.Title);
                //user.Email = ReadString("ContactEmail", user.Email);
                user.Email = email;
                user.BlockInboundEmail = ReadBool("PreventEmailFromCreatingAndUpdatingTickets", user.BlockInboundEmail.ToString());
                user.BlockEmailFromCreatingOnly = ReadBool("PreventEmailFromCreatingButAllowUpdatingTickets", user.BlockEmailFromCreatingOnly.ToString());
                user.IsPortalUser = ReadBool("PortalUser", user.IsPortalUser.ToString());
                user.PortalLimitOrgTickets = ReadBool("DisableOrganizationTicketsViewonPortal", user.PortalLimitOrgTickets.ToString());

                string isActive = ReadString("IsActive", string.Empty);
                if (!string.IsNullOrEmpty(isActive))
                {
                    user.IsActive = ReadBool("IsActive", "1");
                }
                else
                {
                    user.IsActive = true;
                }

                user.ActivatedOn = DateTime.UtcNow;
                DateTime? activatedOn = ReadDateNull("ActivatedOn", user.ActivatedOn.ToString());
                if (activatedOn != null)
                {
                    user.ActivatedOn = (DateTime)activatedOn;
                }

                user.CryptedPassword = "";

                user.DeactivatedOn = null;
                DateTime? deactivatedOn = ReadDateNull("DeactivatedOn", user.DeactivatedOn.ToString());
                if (deactivatedOn != null)
                {
                    user.DeactivatedOn = (DateTime)deactivatedOn;
                }

                user.InOffice = false;
                user.InOfficeComment = "";
                user.IsFinanceAdmin = false;
                user.IsPasswordExpired = true;
                user.IsSystemAdmin = false;
                user.LastActivity = DateTime.UtcNow;
                user.LastLogin = DateTime.UtcNow;
                user.NeedsIndexing = true;
                user.PrimaryGroupID = null;

                int creatorID = -5;
                if (Int32.TryParse(ReadString("CreatorID", creatorID.ToString()), out creatorID))
                {
                    if (!userList.ContainsValue(creatorID))
                    {
                        creatorID = -5;
                    }
                }

                string creatorName = ReadString("CreatorName", string.Empty);
                {
                    if (creatorName != string.Empty)
                    {
                        if (usersAndContactsFirstAndLastNameList.Count == 0)
                        {
                            usersAndContactsFirstAndLastNameList = GetUserAndContactNameAndLastNameList();
                        }

                        if (usersAndContactsFirstAndLastNameList.TryGetValue(creatorName.ToUpper(), out creatorID))
                        {
                        }
                        else
                        {
                            _importLog.Write("No CreatorName " + creatorName + " found.");
                        }
                    }
                }

                user.CreatorID = creatorID;
                user.ModifierID = -5;
                user.ImportFileID = import.ImportID;

                //if (isUpdate)
                //{
                //  if (oldOrganizationID != user.OrganizationID)
                //  {
                //    Tickets t = new Tickets(_importUser);
                //    t.LoadByContact(user.UserID);

                //    foreach (Ticket tix in t)
                //    {
                //      tix.Collection.RemoveContact(user.UserID, tix.TicketID);
                //    }

                //    existingUser.Save();

                //    foreach (Ticket tix in t)
                //    {
                //      tix.Collection.AddContact(user.UserID, tix.TicketID);

                //    }

                //    EmailPosts ep = new EmailPosts(_importUser);
                //    ep.LoadByRecentUserID(user.UserID);
                //    ep.DeleteAll();
                //    ep.Save();

                //    //Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), value);
                //    //string description = String.Format("{0} set contact company to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, organization.Name);
                //    //ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, userID, description);

                //  }
                //  else
                //  {
                //    existingUser.Save();
                //    _importLog.Write(messagePrefix + "UserID " + user.UserID.ToString() + " was updated.");
                //  }
                //  // Add updated rows column as completed rows will reflect only adds
                //}
                _importLog.Write(messagePrefix + "Contact " + importID + " added to bulk insert.");
                count++;

                if (count % BULK_LIMIT == 0)
                {
                    users.BulkSave();
                    users = new Users(_importUser);
                    UpdateImportCount(import, count);
                }
            }
            users.BulkSave();
            UpdateImportCount(import, count);
            _importLog.Write(count.ToString() + " contacts imported.");
        }

        private void ImportAddresses(Import import, ReferenceType addressReferenceType, bool updateStatus, bool isUser = false)
        {
            SortedList<string, int> userList = GetUserList();
            SortedList<string, int> contactList = null;
            if (addressReferenceType == ReferenceType.Users)
            {
                contactList = GetContactList();
            }

            Organization unknownCompany = Organizations.GetUnknownCompany(_loginUser, _organizationID);

            Addresses addresses = new Addresses(_importUser);
            int count = 0;
            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";

                DateTime now = DateTime.UtcNow;
                Addresses newAddresses = new Addresses(_importUser);
                Address newAddress = newAddresses.AddNewAddress();
                newAddress.RefType = addressReferenceType;

                DateTime? dateCreated = ReadDateNull("DateCreated", newAddress.DateCreated.ToString());
                if (dateCreated != null)
                {
                    newAddress.DateCreated = (DateTime)dateCreated;
                }

                int creatorID = -5;
                if (Int32.TryParse(ReadString("CreatorID", creatorID.ToString()), out creatorID))
                {
                    if (!userList.ContainsValue(creatorID))
                    {
                        creatorID = -5;
                    }
                }
                newAddress.CreatorID = -5;
                newAddress.ModifierID = -5;
                newAddress.ImportFileID = import.ImportID;

                int orgID = 0;
                string companyName = string.Empty;
                int companyID = ReadInt("CompanyID");
                if (companyID != 0)
                {
                    Organizations existingCompany = new Organizations(_importUser);
                    existingCompany.LoadByOrganizationID(companyID);
                    if (existingCompany.Count == 1 && existingCompany[0].ParentID == _organizationID)
                    {
                        orgID = existingCompany[0].OrganizationID;
                        companyName = existingCompany[0].Name;
                    }
                    else
                    {
                        _importLog.Write(messagePrefix + "Skipped. No company matching CompanyID was found for addresses.");
                        continue;
                    }
                }

                if (orgID == 0)
                {
                    string importID = ReadString("CompanyImportID", string.Empty);
                    if (importID != string.Empty)
                    {
                        Organizations existingCompany = new Organizations(_importUser);
                        existingCompany.LoadByImportID(importID, _organizationID);
                        if (existingCompany.Count == 1)
                        {
                            orgID = existingCompany[0].OrganizationID;
                            companyName = existingCompany[0].Name;
                        }
                        else if (existingCompany.Count > 1)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one company matching the importID was found.");
                            continue;
                        }
                    }
                }

                companyName = ReadString("CompanyName", string.Empty);
                if (orgID == 0)
                {
                    if (companyName != string.Empty && companyName.Trim().ToLower() != "_unknown company" && !isUser)
                    {
                        Organizations organizationsMatchingName = new Organizations(_importUser);
                        organizationsMatchingName.LoadByName(companyName, _organizationID);
                        if (organizationsMatchingName.Count == 0)
                        {
                            Organizations newCompanies = new Organizations(_importUser);
                            Organization newCompany = newCompanies.AddNewOrganization();
                            newCompany.Name = companyName;
                            newCompany.ParentID = _organizationID;
                            newCompany.IsActive = true;
                            newCompany.ExtraStorageUnits = 0;
                            newCompany.IsCustomerFree = false;
                            newCompany.PortalSeats = 0;
                            newCompany.PrimaryUserID = null;
                            newCompany.ProductType = ProductType.Express;
                            newCompany.UserSeats = 0;
                            newCompany.NeedsIndexing = true;
                            newCompany.SystemEmailID = Guid.NewGuid();
                            newCompany.WebServiceID = Guid.NewGuid();
                            if (dateCreated != null)
                            {
                                newCompany.DateCreated = (DateTime)dateCreated;
                            }
                            newCompany.CreatorID = -5;
                            newCompany.ModifierID = -5;
                            newCompany.ImportFileID = import.ImportID;

                            newCompanies.Save();
                            orgID = newCompany.OrganizationID;
                        }
                        else
                        {
                            orgID = organizationsMatchingName[0].OrganizationID;
                        }
                    }
                    else
                    {
                        companyName = "_unknown company";
                        orgID = unknownCompany.OrganizationID;
                    }
                }

                switch (addressReferenceType)
                {
                    case ReferenceType.Organizations:
                        newAddress.RefID = orgID;
                        break;
                    case ReferenceType.Users:
                        if (!isUser)
                        {
                            int contactID = ReadInt("ContactID");
                            if (contactID != 0)
                            {
                                if (!contactList.ContainsValue(contactID))
                                {
                                    _importLog.Write(messagePrefix + "Skipped. No contact matching ContactID " + contactID.ToString() + " was found for addresses.");
                                    continue;
                                }
                            }

                            if (contactID == 0)
                            {
                                string contactImportID = ReadString("ContactImportID", string.Empty);
                                if (contactImportID != string.Empty)
                                {
                                    Users existingContact = new Users(_importUser);
                                    existingContact.LoadByImportID(_organizationID, contactImportID);
                                    if (existingContact.Count == 1)
                                    {
                                        contactID = existingContact[0].UserID;
                                    }
                                    else if (existingContact.Count > 1)
                                    {
                                        _importLog.Write(messagePrefix + "Skipped. More than one contact matching the ContactImportID " + contactImportID + " was found.");
                                        continue;
                                    }
                                    else
                                    {
                                        _importLog.Write(messagePrefix + "Skipped. No contact matching the ContactImportID " + contactImportID + " was found.");
                                        continue;
                                    }
                                }
                            }

                            string contactEmail = ReadString("ContactEmail", string.Empty);
                            string searchTerm = contactEmail.Replace(" ", string.Empty) + "(" + companyName.Replace(" ", string.Empty) + ")";
                            if (contactID == 0 && !contactList.TryGetValue(searchTerm.ToUpper(), out contactID))
                            {
                                string firstName = ReadString("FirstName", string.Empty);
                                string lastName = ReadString("LastName", string.Empty);
                                if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                                {
                                    Users newContacts = new Users(_importUser);
                                    User newContact = newContacts.AddNewUser();
                                    newContact.FirstName = firstName;
                                    newContact.LastName = lastName;
                                    newContact.Email = contactEmail;
                                    newContact.OrganizationID = orgID;
                                    newContact.ActivatedOn = now;
                                    newContact.CryptedPassword = "";
                                    newContact.DeactivatedOn = null;
                                    newContact.InOffice = false;
                                    newContact.InOfficeComment = "";
                                    newContact.IsFinanceAdmin = false;
                                    newContact.IsPasswordExpired = true;
                                    newContact.IsSystemAdmin = false;
                                    newContact.LastActivity = now;
                                    newContact.LastLogin = now;
                                    newContact.NeedsIndexing = true;
                                    newContact.PrimaryGroupID = null;
                                    if (dateCreated != null)
                                    {
                                        newContact.DateCreated = (DateTime)dateCreated;
                                    }
                                    newContact.CreatorID = -5;
                                    newContact.ModifierID = -5;
                                    newContact.ImportFileID = import.ImportID;

                                    newContacts.Save();
                                    contactID = newContact.UserID;
                                    contactList.Add(searchTerm, contactID);
                                }
                                else
                                {
                                    _importLog.Write(messagePrefix + "Skipped. Address could not be added as contact does not exists and either first or last name are missing.");
                                    continue;
                                }
                            }
                            newAddress.RefID = contactID;
                        }
                        else
                        {
                            string userImportID = ReadString("UserImportID", string.Empty);
                            if (userImportID != string.Empty)
                            {
                                Users existingUser = new Users(_importUser);
                                existingUser.LoadByImportID(userImportID, _organizationID);
                                if (existingUser.Count == 1)
                                {
                                    newAddress.RefID = existingUser[0].UserID;
                                }
                                else if (existingUser.Count > 1)
                                {
                                    _importLog.Write(messagePrefix + "Skipped. More than one user matching the UserImportID " + userImportID + " was found.");
                                    continue;
                                }
                                else
                                {
                                    _importLog.Write(messagePrefix + "Skipped. No user matching the UserImportID " + userImportID + " was found.");
                                    continue;
                                }
                            }
                            else
                            {
                                _importLog.Write(messagePrefix + "Skipped. UserImportID was missing.");
                                continue;
                            }
                        }
                        break;
                }

                newAddress.Description = ReadString("AddressDescription", newAddress.Description);
                newAddress.Addr1 = ReadString("Addr1", newAddress.Addr1);
                newAddress.Addr2 = ReadString("Addr2", newAddress.Addr2);
                newAddress.Addr3 = ReadString("Addr3", newAddress.Addr3);
                newAddress.City = ReadString("City", newAddress.City);
                newAddress.State = ReadString("State", newAddress.State);
                newAddress.Zip = ReadString("Zip", newAddress.Zip);
                newAddress.Country = ReadString("Country", newAddress.Country);
                newAddress.Comment = ReadString("AddressComment", newAddress.Comment);

                //Addresses existingAddresses = new Addresses(_importUser);
                //existingAddresses.LoadByID(newAddress.RefID, addressReferenceType);
                //bool alreadyExists = false;
                //foreach (Address existingAddress in existingAddresses)
                //{
                //  if (
                //    newAddress.Description.Replace(" ", string.Empty).ToLower() == existingAddress.Description.Replace(" ", string.Empty).ToLower() 
                //    && newAddress.Addr1.Replace(" ", string.Empty).ToLower() == existingAddress.Addr1.Replace(" ", string.Empty).ToLower()
                //    && newAddress.Addr2.Replace(" ", string.Empty).ToLower() == existingAddress.Addr2.Replace(" ", string.Empty).ToLower()
                //    && newAddress.Addr3.Replace(" ", string.Empty).ToLower() == existingAddress.Addr3.Replace(" ", string.Empty).ToLower()
                //    && newAddress.City.Replace(" ", string.Empty).ToLower() == existingAddress.City.Replace(" ", string.Empty).ToLower()
                //    && newAddress.State.Replace(" ", string.Empty).ToLower() == existingAddress.State.Replace(" ", string.Empty).ToLower()
                //    && newAddress.Zip.Replace(" ", string.Empty).ToLower() == existingAddress.Zip.Replace(" ", string.Empty).ToLower()
                //    && newAddress.Country.Replace(" ", string.Empty).ToLower() == existingAddress.Country.Replace(" ", string.Empty).ToLower()
                //    && newAddress.Comment.Replace(" ", string.Empty).ToLower() == existingAddress.Comment.Replace(" ", string.Empty).ToLower())
                //  {
                //    alreadyExists = true;
                //    break;
                //  }
                //}

                //if (!alreadyExists)
                //{
                Address address = addresses.AddNewAddress();
                address.RefType = newAddress.RefType;
                address.DateCreated = newAddress.DateCreated;
                address.CreatorID = -5;
                address.ModifierID = -5;
                address.RefID = newAddress.RefID;
                address.Description = newAddress.Description;
                address.Addr1 = newAddress.Addr1;
                address.Addr2 = newAddress.Addr2;
                address.Addr3 = newAddress.Addr3;
                address.City = newAddress.City;
                address.State = newAddress.State;
                address.Zip = newAddress.Zip;
                address.Country = newAddress.Country;
                address.Comment = newAddress.Comment;
                address.ImportFileID = import.ImportID;

                _importLog.Write(messagePrefix + "Address added to bulk insert.");
                //}
                //else
                //{
                //  _importLog.Write(messagePrefix + "Address in row " + index.ToString() + " already exists and was not added to addresses set.");
                //}
                count++;

                if (count % BULK_LIMIT == 0)
                {
                    addresses.BulkSave();
                    addresses = new Addresses(_importUser);
                    if (updateStatus)
                    {
                        UpdateImportCount(import, count);
                    }
                    _importLog.Write("Import set with " + count.ToString() + " addresses inserted in database.");
                }
            }
            addresses.BulkSave();
            if (updateStatus)
            {
                UpdateImportCount(import, count);
            }
            _importLog.Write("Import set with " + count.ToString() + " addresses inserted in database.");
        }

        private void ImportPhoneNumbers(Import import, ReferenceType phoneNumberReferenceType, bool updateStatus)
        {
            SortedList<string, int> usersAndContactsFirstAndLastNameList = new SortedList<string, int>();
            SortedList<string, int> userList = GetUserList();
            SortedList<string, int> userAndContactList = GetUserAndContactList();
            SortedList<string, int> contactList = null;
            if (phoneNumberReferenceType == ReferenceType.Contacts)
            {
                contactList = GetContactList();
            }

            Organization unknownCompany = Organizations.GetUnknownCompany(_loginUser, _organizationID);

            PhoneTypes phoneTypes = new PhoneTypes(_importUser);
            phoneTypes.LoadByOrganizationID(_organizationID);

            PhoneNumbers phoneNumbers = new PhoneNumbers(_importUser);
            int count = 0;
            int bulkCount = 0;
            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";

                DateTime now = DateTime.UtcNow;
                PhoneNumbers newPhoneNumbers = new PhoneNumbers(_importUser);
                PhoneNumber newPhoneNumber = newPhoneNumbers.AddNewPhoneNumber();
                PhoneNumber newPhoneNumber2 = newPhoneNumbers.AddNewPhoneNumber();
                PhoneNumber newPhoneNumber3 = newPhoneNumbers.AddNewPhoneNumber();
                newPhoneNumber.RefType = phoneNumberReferenceType;
                newPhoneNumber2.RefType = phoneNumberReferenceType;
                newPhoneNumber3.RefType = phoneNumberReferenceType;

                DateTime? dateCreated = ReadDateNull("DateCreated", string.Empty);
                if (dateCreated != null)
                {
                    newPhoneNumber.DateCreated = (DateTime)dateCreated;
                    newPhoneNumber2.DateCreated = (DateTime)dateCreated;
                    newPhoneNumber3.DateCreated = (DateTime)dateCreated;
                }

                int creatorID = -5;
                if (Int32.TryParse(ReadString("CreatorID", creatorID.ToString()), out creatorID))
                {
                    if (!userAndContactList.ContainsValue(creatorID))
                    {
                        creatorID = -5;
                    }
                }

                string creatorEmail = ReadString("CreatorEmail", string.Empty);
                if (!string.IsNullOrEmpty(creatorEmail))
                {
                    if (userAndContactList.TryGetValue(creatorEmail.ToUpper(), out creatorID))
                    {
                    }
                    else
                    {
                        _importLog.Write("No CreatorEmail " + creatorEmail + " found.");
                    }
                }

                string creatorName = ReadString("CreatorName", string.Empty);
                {
                    if (creatorName != string.Empty)
                    {
                        if (usersAndContactsFirstAndLastNameList.Count == 0)
                        {
                            usersAndContactsFirstAndLastNameList = GetUserAndContactNameAndLastNameList();
                        }

                        if (usersAndContactsFirstAndLastNameList.TryGetValue(creatorName.ToUpper(), out creatorID))
                        {
                        }
                        else
                        {
                            _importLog.Write("No CreatorName " + creatorName + " found.");
                        }
                    }
                }

                newPhoneNumber.CreatorID = creatorID;
                newPhoneNumber2.CreatorID = creatorID;
                newPhoneNumber3.CreatorID = creatorID;

                newPhoneNumber.ModifierID = -5;
                newPhoneNumber2.ModifierID = -5;
                newPhoneNumber3.ModifierID = -5;

                int orgID = 0;
                string companyName = string.Empty;
                int companyID = ReadInt("CompanyID");
                if (companyID != 0)
                {
                    Organizations existingCompany = new Organizations(_importUser);
                    existingCompany.LoadByOrganizationID(companyID);
                    if (existingCompany.Count == 1 && existingCompany[0].ParentID == _organizationID)
                    {
                        orgID = existingCompany[0].OrganizationID;
                        companyName = existingCompany[0].Name;
                    }
                    else
                    {
                        _importLog.Write(messagePrefix + "Skipped. No company matching CompanyID was found for phone numbers.");
                        continue;
                    }
                }

                if (orgID == 0)
                {
                    string importID = ReadString("CompanyImportID", string.Empty);
                    if (importID != string.Empty)
                    {
                        Organizations existingCompany = new Organizations(_importUser);
                        existingCompany.LoadByImportID(importID, _organizationID);
                        if (existingCompany.Count == 1)
                        {
                            orgID = existingCompany[0].OrganizationID;
                            companyName = existingCompany[0].Name;
                        }
                        else if (existingCompany.Count > 1)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one company matching the importID was found.");
                            continue;
                        }
                    }
                }

                if (orgID == 0)
                {
                    companyName = ReadString("CompanyName", string.Empty);
                    if (companyName != string.Empty && companyName.Trim().ToLower() != "_unknown company")
                    {
                        Organizations organizationsMatchingName = new Organizations(_importUser);
                        organizationsMatchingName.LoadByName(companyName, _organizationID);
                        if (organizationsMatchingName.Count == 0)
                        {
                            Organizations newCompanies = new Organizations(_importUser);
                            Organization newCompany = newCompanies.AddNewOrganization();
                            newCompany.Name = companyName;
                            newCompany.ParentID = _organizationID;
                            newCompany.IsActive = true;
                            newCompany.ExtraStorageUnits = 0;
                            newCompany.IsCustomerFree = false;
                            newCompany.PortalSeats = 0;
                            newCompany.PrimaryUserID = null;
                            newCompany.ProductType = ProductType.Express;
                            newCompany.UserSeats = 0;
                            newCompany.NeedsIndexing = true;
                            newCompany.SystemEmailID = Guid.NewGuid();
                            newCompany.WebServiceID = Guid.NewGuid();
                            if (dateCreated != null)
                            {
                                newCompany.DateCreated = (DateTime)dateCreated;
                            }
                            newCompany.CreatorID = newPhoneNumber.CreatorID;
                            newCompany.ImportFileID = import.ImportID;

                            newCompany.ModifierID = -5;
                            newCompanies.Save();
                            orgID = newCompany.OrganizationID;
                        }
                        else
                        {
                            orgID = organizationsMatchingName[0].OrganizationID;
                        }
                    }
                    else
                    {
                        orgID = unknownCompany.OrganizationID;
                        companyName = "_unknown company";
                    }
                }

                switch (phoneNumberReferenceType)
                {
                    case ReferenceType.Organizations:
                        newPhoneNumber.RefID = orgID;
                        newPhoneNumber2.RefID = orgID;
                        newPhoneNumber3.RefID = orgID;
                        break;
                    case ReferenceType.Contacts:
                        newPhoneNumber.RefType = ReferenceType.Users;
                        newPhoneNumber2.RefType = ReferenceType.Users;
                        newPhoneNumber3.RefType = ReferenceType.Users;
                        int contactID = ReadInt("ContactID");
                        if (contactID != 0)
                        {
                            if (!contactList.ContainsValue(contactID))
                            {
                                _importLog.Write(messagePrefix + "Skipped. No contact matching ContactID " + contactID.ToString() + " was found for phone numbers.");
                                continue;
                            }
                        }

                        if (contactID == 0)
                        {
                            string contactImportID = ReadString("ContactImportID", string.Empty);
                            if (contactImportID != string.Empty)
                            {
                                Users existingContact = new Users(_importUser);
                                existingContact.LoadByImportID(_organizationID, contactImportID);
                                if (existingContact.Count == 1)
                                {
                                    contactID = existingContact[0].UserID;
                                }
                                else if (existingContact.Count > 1)
                                {
                                    _importLog.Write(messagePrefix + "Skipped. More than one contact matching the ContactImportID " + contactImportID + " was found for phone numbers.");
                                    continue;
                                }
                                else
                                {
                                    _importLog.Write(messagePrefix + "Skipped. No contact matching the ContactImportID " + contactImportID + " was found for phone numbers.");
                                    continue;
                                }
                            }
                        }

                        if (contactID == 0)
                        {
                            string contactEmail = ReadString("ContactEmail", string.Empty);
                            string searchTerm = contactEmail.Replace(" ", string.Empty) + "(" + companyName.Replace(" ", string.Empty) + ")";
                            if (!contactList.TryGetValue(searchTerm.ToUpper(), out contactID))
                            {
                                string firstName = ReadString("FirstName", string.Empty);
                                string lastName = ReadString("LastName", string.Empty);
                                if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                                {
                                    Users newContacts = new Users(_importUser);
                                    User newContact = newContacts.AddNewUser();
                                    newContact.FirstName = firstName;
                                    newContact.LastName = lastName;
                                    newContact.Email = contactEmail;
                                    newContact.OrganizationID = orgID;
                                    newContact.ActivatedOn = now;
                                    newContact.CryptedPassword = "";
                                    newContact.DeactivatedOn = null;
                                    newContact.InOffice = false;
                                    newContact.InOfficeComment = "";
                                    newContact.IsFinanceAdmin = false;
                                    newContact.IsPasswordExpired = true;
                                    newContact.IsSystemAdmin = false;
                                    newContact.LastActivity = now;
                                    newContact.LastLogin = now;
                                    newContact.NeedsIndexing = true;
                                    newContact.PrimaryGroupID = null;
                                    if (dateCreated != null)
                                    {
                                        newContact.DateCreated = (DateTime)dateCreated;
                                    }
                                    newContact.CreatorID = newPhoneNumber.CreatorID;
                                    newContact.ModifierID = -5;
                                    newContact.ImportFileID = import.ImportID;

                                    newContacts.Save();
                                    contactID = newContact.UserID;
                                    contactList.Add(searchTerm, contactID);
                                }
                                else
                                {
                                    _importLog.Write(messagePrefix + "Skipped. Phone number could not be added as contact does not exists and either first (" + firstName + ") or last (" + lastName + ") name are missing.");
                                    continue;
                                }
                            }
                        }
                        newPhoneNumber.RefID = contactID;
                        newPhoneNumber2.RefID = contactID;
                        newPhoneNumber3.RefID = contactID;
                        break;
                    case ReferenceType.Users:
                        int userID = ReadInt("UserID");
                        if (userID != 0)
                        {
                            if (!userList.ContainsValue(userID))
                            {
                                _importLog.Write(messagePrefix + "Skipped. No user matching UserID " + userID.ToString() + " was found for phone numbers.");
                                continue;
                            }
                        }

                        if (userID == 0)
                        {
                            string userImportID = ReadString("UserImportID", string.Empty);
                            if (userImportID != string.Empty)
                            {
                                Users existingUser = new Users(_importUser);
                                existingUser.LoadByImportID(userImportID, _organizationID);
                                if (existingUser.Count == 1)
                                {
                                    userID = existingUser[0].UserID;
                                }
                                else if (existingUser.Count > 1)
                                {
                                    _importLog.Write(messagePrefix + "Skipped. More than one user matching the userID " + userID + " was found for phone numbers.");
                                    continue;
                                }
                            }
                        }

                        if (userID == 0)
                        {
                            string userEmail = ReadString("UserEmail", string.Empty);
                            if (userEmail != string.Empty)
                            {
                                Users existingUser = new Users(_importUser);
                                existingUser.LoadByEmail(userEmail, _organizationID);
                                if (existingUser.Count == 1)
                                {
                                    userID = existingUser[0].UserID;
                                }
                                else if (existingUser.Count > 1)
                                {
                                    _importLog.Write(messagePrefix + "Skipped. More than one user matching the userEmail " + userEmail + " was found for phone numbers.");
                                    continue;
                                }
                                else
                                {
                                    _importLog.Write(messagePrefix + "Skipped. No user matching the userEmail " + userEmail + " was found for phone numbers.");
                                    continue;
                                }
                            }
                        }
                        newPhoneNumber.RefID = userID;
                        newPhoneNumber2.RefID = userID;
                        newPhoneNumber3.RefID = userID;
                        break;
                }

                string phoneTypeName = ReadString("PhoneType", string.Empty);
                string phoneTypeName2 = ReadString("PhoneType2", string.Empty);
                string phoneTypeName3 = ReadString("PhoneType3", string.Empty);
                PhoneType phoneType = phoneTypes.FindByName(phoneTypeName);
                int phoneTypeID = 0;
                if (phoneType == null)
                {
                    PhoneTypes newPhoneTypes = new PhoneTypes(_importUser);
                    PhoneType newPhoneType = newPhoneTypes.AddNewPhoneType();
                    newPhoneType.Name = phoneTypeName;
                    newPhoneType.OrganizationID = _organizationID;
                    if (dateCreated != null)
                    {
                        newPhoneType.DateCreated = (DateTime)dateCreated;
                    }
                    newPhoneType.CreatorID = newPhoneNumber.CreatorID;
                    newPhoneTypes.Save();
                    phoneTypeID = newPhoneType.PhoneTypeID;

                    phoneTypes = new PhoneTypes(_importUser);
                    phoneTypes.LoadByOrganizationID(_organizationID);
                }
                else
                {
                    phoneTypeID = phoneType.PhoneTypeID;
                }
                newPhoneNumber.PhoneTypeID = phoneTypeID;

                if (!string.IsNullOrEmpty(phoneTypeName2))
                {
                    PhoneType phoneType2 = phoneTypes.FindByName(phoneTypeName2);
                    if (phoneType2 == null)
                    {
                        PhoneTypes newPhoneTypes = new PhoneTypes(_importUser);
                        PhoneType newPhoneType = newPhoneTypes.AddNewPhoneType();
                        newPhoneType.Name = phoneTypeName;
                        newPhoneType.OrganizationID = _organizationID;
                        if (dateCreated != null)
                        {
                            newPhoneType.DateCreated = (DateTime)dateCreated;
                        }
                        newPhoneType.CreatorID = newPhoneNumber.CreatorID;
                        newPhoneTypes.Save();
                        phoneTypeID = newPhoneType.PhoneTypeID;

                        phoneTypes = new PhoneTypes(_importUser);
                        phoneTypes.LoadByOrganizationID(_organizationID);
                    }
                    else
                    {
                        phoneTypeID = phoneType2.PhoneTypeID;
                    }
                    newPhoneNumber2.PhoneTypeID = phoneTypeID;
                }

                if (!string.IsNullOrEmpty(phoneTypeName3))
                {
                    PhoneType phoneType3 = phoneTypes.FindByName(phoneTypeName3);
                    if (phoneType3 == null)
                    {
                        PhoneTypes newPhoneTypes = new PhoneTypes(_importUser);
                        PhoneType newPhoneType = newPhoneTypes.AddNewPhoneType();
                        newPhoneType.Name = phoneTypeName;
                        newPhoneType.OrganizationID = _organizationID;
                        if (dateCreated != null)
                        {
                            newPhoneType.DateCreated = (DateTime)dateCreated;
                        }
                        newPhoneType.CreatorID = newPhoneNumber.CreatorID;
                        newPhoneTypes.Save();
                        phoneTypeID = newPhoneType.PhoneTypeID;

                        phoneTypes = new PhoneTypes(_importUser);
                        phoneTypes.LoadByOrganizationID(_organizationID);
                    }
                    else
                    {
                        phoneTypeID = phoneType3.PhoneTypeID;
                    }
                    newPhoneNumber3.PhoneTypeID = phoneTypeID;
                }

                newPhoneNumber.Number = ReadString("Number", newPhoneNumber.Number);
                newPhoneNumber2.Number = ReadString("Number2", newPhoneNumber2.Number);
                newPhoneNumber3.Number = ReadString("Number3", newPhoneNumber3.Number);

                newPhoneNumber.Extension = ReadString("Extension", newPhoneNumber.Extension);
                newPhoneNumber2.Extension = ReadString("Extension2", newPhoneNumber2.Extension);
                newPhoneNumber3.Extension = ReadString("Extension3", newPhoneNumber3.Extension);

                //PhoneNumbers existingPhoneNumbers = new PhoneNumbers(_importUser);
                //existingPhoneNumbers.LoadByID(newPhoneNumber.RefID, phoneNumberReferenceType);
                //bool alreadyExists = false;
                //bool alreadyExists2 = false;
                //bool alreadyExists3 = false;
                bool phoneAdded = false;

                //foreach (PhoneNumber existingPhoneNumber in existingPhoneNumbers)
                //{
                //  if (
                //    newPhoneNumber.Number.Replace(" ", string.Empty).ToLower() == existingPhoneNumber.Number.Replace(" ", string.Empty).ToLower()
                //    && newPhoneNumber.Extension.Replace(" ", string.Empty).ToLower() == existingPhoneNumber.Extension.Replace(" ", string.Empty).ToLower()
                //    && newPhoneNumber.PhoneTypeID == existingPhoneNumber.PhoneTypeID)
                //  {
                //    alreadyExists = true;
                //    break;
                //  }
                //}

                //if (!alreadyExists && (newPhoneNumber.Number.Trim() != string.Empty || newPhoneNumber.Extension.Trim() != string.Empty))
                if ((newPhoneNumber.Number != null && newPhoneNumber.Number.Trim() != string.Empty) || (newPhoneNumber.Extension != null && newPhoneNumber.Extension.Trim() != string.Empty))
                {
                    phoneAdded = true;
                    bulkCount++;
                    PhoneNumber phoneNumber = phoneNumbers.AddNewPhoneNumber();
                    phoneNumber.RefType = newPhoneNumber.RefType;
                    phoneNumber.DateCreated = newPhoneNumber.DateCreated;
                    phoneNumber.CreatorID = newPhoneNumber.CreatorID;
                    phoneNumber.ModifierID = -5;
                    phoneNumber.RefID = newPhoneNumber.RefID;
                    phoneNumber.Number = newPhoneNumber.Number;
                    phoneNumber.Extension = newPhoneNumber.Extension;
                    phoneNumber.PhoneTypeID = newPhoneNumber.PhoneTypeID;
                    phoneNumber.ImportFileID = import.ImportID;

                    _importLog.Write(messagePrefix + "Phone Number added to bulk insert.");
                }
                //else
                //{
                //  _importLog.Write(messagePrefix + "Phone Number in row " + index.ToString() + " already exists and was not added to phone numbers set.");
                //}

                if (!string.IsNullOrEmpty(newPhoneNumber2.Number))
                {
                    //foreach (PhoneNumber existingPhoneNumber in existingPhoneNumbers)
                    //{
                    //  if (
                    //    newPhoneNumber2.Number.Replace(" ", string.Empty).ToLower() == existingPhoneNumber.Number.Replace(" ", string.Empty).ToLower()
                    //    && newPhoneNumber2.Extension.Replace(" ", string.Empty).ToLower() == existingPhoneNumber.Extension.Replace(" ", string.Empty).ToLower()
                    //    && newPhoneNumber2.PhoneTypeID == existingPhoneNumber.PhoneTypeID)
                    //  {
                    //    alreadyExists2 = true;
                    //    break;
                    //  }
                    //}

                    if (newPhoneNumber2.Number.Trim() != string.Empty || newPhoneNumber2.Extension.Trim() != string.Empty)
                    {
                        phoneAdded = true;
                        bulkCount++;
                        PhoneNumber phoneNumber = phoneNumbers.AddNewPhoneNumber();
                        phoneNumber.RefType = newPhoneNumber2.RefType;
                        phoneNumber.DateCreated = newPhoneNumber2.DateCreated;
                        phoneNumber.CreatorID = newPhoneNumber2.CreatorID;
                        phoneNumber.ModifierID = -5;
                        phoneNumber.RefID = newPhoneNumber2.RefID;
                        phoneNumber.Number = newPhoneNumber2.Number;
                        phoneNumber.Extension = newPhoneNumber2.Extension;
                        phoneNumber.PhoneTypeID = newPhoneNumber2.PhoneTypeID;
                        phoneNumber.ImportFileID = import.ImportID;

                        _importLog.Write(messagePrefix + "Phone Number 2 was added to bulk insert.");
                    }
                    //else
                    //{
                    //  _importLog.Write(messagePrefix + "Phone Number 2 in row " + index.ToString() + " already exists and was not added to phone numbers set.");
                    //}
                }

                if (!string.IsNullOrEmpty(newPhoneNumber3.Number))
                {
                    //foreach (PhoneNumber existingPhoneNumber in existingPhoneNumbers)
                    //{
                    //  if (
                    //    newPhoneNumber3.Number.Replace(" ", string.Empty).ToLower() == existingPhoneNumber.Number.Replace(" ", string.Empty).ToLower()
                    //    && newPhoneNumber3.Extension.Replace(" ", string.Empty).ToLower() == existingPhoneNumber.Extension.Replace(" ", string.Empty).ToLower()
                    //    && newPhoneNumber3.PhoneTypeID == existingPhoneNumber.PhoneTypeID)
                    //  {
                    //    alreadyExists3 = true;
                    //    break;
                    //  }
                    //}

                    if (newPhoneNumber3.Number.Trim() != string.Empty || newPhoneNumber3.Extension.Trim() != string.Empty)
                    {
                        phoneAdded = true;
                        bulkCount++;
                        PhoneNumber phoneNumber = phoneNumbers.AddNewPhoneNumber();
                        phoneNumber.RefType = newPhoneNumber3.RefType;
                        phoneNumber.DateCreated = newPhoneNumber3.DateCreated;
                        phoneNumber.CreatorID = newPhoneNumber3.CreatorID;
                        phoneNumber.ModifierID = -5;
                        phoneNumber.RefID = newPhoneNumber3.RefID;
                        phoneNumber.Number = newPhoneNumber3.Number;
                        phoneNumber.Extension = newPhoneNumber3.Extension;
                        phoneNumber.PhoneTypeID = newPhoneNumber3.PhoneTypeID;
                        phoneNumber.ImportFileID = import.ImportID;

                        _importLog.Write(messagePrefix + "Phone Number 3 was added to bulk insert.");
                    }
                    //else
                    //{
                    //  _importLog.Write(messagePrefix + "Phone Number 3 in row " + index.ToString() + " already exists and was not added to phone numbers set.");
                    //}
                }
                if (phoneAdded)
                {
                    count++;
                }

                if (bulkCount % BULK_LIMIT == 0)
                {
                    phoneNumbers.BulkSave();
                    phoneNumbers = new PhoneNumbers(_importUser);
                    if (updateStatus)
                    {
                        UpdateImportCount(import, count);
                    }
                    _importLog.Write("Import set with " + count.ToString() + " phone numbers inserted in database.");
                }
            }
            phoneNumbers.BulkSave();
            if (updateStatus)
            {
                UpdateImportCount(import, count);
            }
            _importLog.Write("Import set with " + count.ToString() + " phone numbers inserted in database.");
        }

        private void ImportParentCompanies(Import import)
        {
            SortedList<string, int> userList = GetUserList();
            Organization unknownCompany = Organizations.GetUnknownCompany(_loginUser, _organizationID);
            CustomerRelationships customerRelationships = new CustomerRelationships(_importUser);
            int count = 0;
            int bulkCount = 0;
            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";

                DateTime now = DateTime.UtcNow;
                CustomerRelationships newCustomerRelationships = new CustomerRelationships(_importUser);

                DateTime? dateCreated = ReadDateNull("DateCreated", string.Empty);

                int creatorID = -5;
                if (Int32.TryParse(ReadString("CreatorID", creatorID.ToString()), out creatorID))
                {
                    if (!userList.ContainsValue(creatorID))
                    {
                        creatorID = -5;
                    }
                }

                int orgID = 0;
                string companyName = string.Empty;
                int companyID = ReadInt("CompanyID");
                if (companyID != 0)
                {
                    Organizations existingCompany = new Organizations(_importUser);
                    existingCompany.LoadByOrganizationID(companyID);
                    if (existingCompany.Count == 1 && existingCompany[0].ParentID == _organizationID)
                    {
                        orgID = existingCompany[0].OrganizationID;
                        companyName = existingCompany[0].Name;
                    }
                    else
                    {
                        _importLog.Write(messagePrefix + "Skipped. No company matching CompanyID was found to set company parent.");
                        continue;
                    }
                }

                if (orgID == 0)
                {
                    string importID = ReadString("CompanyImportID", string.Empty);
                    if (importID != string.Empty)
                    {
                        Organizations existingCompany = new Organizations(_importUser);
                        existingCompany.LoadByImportID(importID, _organizationID);
                        if (existingCompany.Count == 1)
                        {
                            orgID = existingCompany[0].OrganizationID;
                            companyName = existingCompany[0].Name;
                        }
                        else if (existingCompany.Count > 1)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one company matching the importID was found.");
                            continue;
                        }
                    }
                }

                if (orgID == 0)
                {
                    companyName = ReadString("CompanyName", string.Empty);
                    if (companyName != string.Empty && companyName.Trim().ToLower() != "_unknown company")
                    {
                        Organizations organizationsMatchingName = new Organizations(_importUser);
                        organizationsMatchingName.LoadByName(companyName, _organizationID);
                        if (organizationsMatchingName.Count == 0)
                        {
                            Organizations newCompanies = new Organizations(_importUser);
                            Organization newCompany = newCompanies.AddNewOrganization();
                            newCompany.Name = companyName;
                            newCompany.ParentID = _organizationID;
                            newCompany.IsActive = true;
                            newCompany.ExtraStorageUnits = 0;
                            newCompany.IsCustomerFree = false;
                            newCompany.PortalSeats = 0;
                            newCompany.PrimaryUserID = null;
                            newCompany.ProductType = ProductType.Express;
                            newCompany.UserSeats = 0;
                            newCompany.NeedsIndexing = true;
                            newCompany.SystemEmailID = Guid.NewGuid();
                            newCompany.WebServiceID = Guid.NewGuid();
                            if (dateCreated != null)
                            {
                                newCompany.DateCreated = (DateTime)dateCreated;
                            }
                            newCompany.CreatorID = -5;
                            newCompany.ImportFileID = import.ImportID;

                            newCompany.ModifierID = -5;
                            newCompanies.Save();
                            orgID = newCompany.OrganizationID;
                        }
                        else
                        {
                            orgID = organizationsMatchingName[0].OrganizationID;
                        }
                    }
                    else
                    {
                        orgID = unknownCompany.OrganizationID;
                        companyName = "_unknown company";
                    }
                }

                Organizations parentCompany = new Organizations(_importUser);
                int parentCompanyID = ReadInt("ParentCompanyID");
                if (parentCompanyID != 0)
                {
                    parentCompany.LoadByOrganizationID(parentCompanyID);
                    if (parentCompany.Count == 1 && parentCompany[0].ParentID == _organizationID)
                    {
                        CustomerRelationship newCustomerRelationship = newCustomerRelationships.AddNewCustomerRelationship();
                        newCustomerRelationship.CustomerID = orgID;
                        newCustomerRelationship.RelatedCustomerID = parentCompany[0].OrganizationID;
                        newCustomerRelationship.CreatorID = -5;
                        if (dateCreated != null)
                        {
                            newCustomerRelationship.DateCreated = (DateTime)dateCreated;
                        }
                    }
                    else
                    {
                        _importLog.Write(messagePrefix + ". No company matching ParentCompanyID " + parentCompanyID.ToString() + " was found for parent companies.");
                    }
                }

                if (newCustomerRelationships.Count == 0)
                {
                    string parentImportID = ReadString("ParentCompanyImportID", string.Empty);
                    if (parentImportID != string.Empty)
                    {
                        parentCompany = new Organizations(_importUser);
                        parentCompany.LoadByImportID(parentImportID, _organizationID);
                        if (parentCompany.Count == 1)
                        {
                            CustomerRelationship newCustomerRelationship = newCustomerRelationships.AddNewCustomerRelationship();
                            newCustomerRelationship.CustomerID = orgID;
                            newCustomerRelationship.RelatedCustomerID = parentCompany[0].OrganizationID;
                            newCustomerRelationship.CreatorID = -5;
                            if (dateCreated != null)
                            {
                                newCustomerRelationship.DateCreated = (DateTime)dateCreated;
                            }
                        }
                        else
                        {
                            _importLog.Write(messagePrefix + ". No company matching ParentCompanyImportID " + parentImportID + " was found for parent companies.");
                        }
                    }
                }

                if (newCustomerRelationships.Count == 0)
                {
                    string parentCompanyName = ReadString("ParentCompanyName", string.Empty);
                    if (parentCompanyName != string.Empty)
                    {
                        parentCompany = new Organizations(_importUser);
                        parentCompany.LoadByName(parentCompanyName, _organizationID);
                        if (parentCompany.Count == 1)
                        {
                            CustomerRelationship newCustomerRelationship = newCustomerRelationships.AddNewCustomerRelationship();
                            newCustomerRelationship.CustomerID = orgID;
                            newCustomerRelationship.RelatedCustomerID = parentCompany[0].OrganizationID;
                            newCustomerRelationship.CreatorID = -5;
                            if (dateCreated != null)
                            {
                                newCustomerRelationship.DateCreated = (DateTime)dateCreated;
                            }
                        }
                        else
                        {
                            _importLog.Write(messagePrefix + ". No company matching ParentCompanyName " + parentCompanyName + " was found for parent companies.");
                        }
                    }
                }

                bool parentCompany2Found = false;
                int parentCompanyID2 = ReadInt("ParentCompanyID2");
                if (parentCompanyID2 != 0)
                {
                    parentCompany = new Organizations(_importUser);
                    parentCompany.LoadByOrganizationID(parentCompanyID2);
                    if (parentCompany.Count == 1 && parentCompany[0].ParentID == _organizationID)
                    {
                        parentCompany2Found = true;
                        CustomerRelationship newCustomerRelationship = newCustomerRelationships.AddNewCustomerRelationship();
                        newCustomerRelationship.CustomerID = orgID;
                        newCustomerRelationship.RelatedCustomerID = parentCompany[0].OrganizationID;
                        newCustomerRelationship.CreatorID = -5;
                        if (dateCreated != null)
                        {
                            newCustomerRelationship.DateCreated = (DateTime)dateCreated;
                        }
                    }
                    else
                    {
                        _importLog.Write(messagePrefix + ". No company matching ParentCompanyID2 " + parentCompanyID2.ToString() + " was found for parent companies.");
                    }
                }

                if (!parentCompany2Found)
                {
                    string parentImportID2 = ReadString("ParentCompanyImportID2", string.Empty);
                    if (parentImportID2 != string.Empty)
                    {
                        parentCompany = new Organizations(_importUser);
                        parentCompany.LoadByImportID(parentImportID2, _organizationID);
                        if (parentCompany.Count == 1)
                        {
                            parentCompany2Found = true;
                            CustomerRelationship newCustomerRelationship2 = newCustomerRelationships.AddNewCustomerRelationship();
                            newCustomerRelationship2.CustomerID = orgID;
                            newCustomerRelationship2.RelatedCustomerID = parentCompany[0].OrganizationID;
                            newCustomerRelationship2.CreatorID = -5;
                            if (dateCreated != null)
                            {
                                newCustomerRelationship2.DateCreated = (DateTime)dateCreated;
                            }
                        }
                        else
                        {
                            _importLog.Write(messagePrefix + ". No company matching ParentCompanyImportID2 " + parentImportID2 + " was found for parent companies.");
                        }
                    }
                }

                if (!parentCompany2Found)
                {
                    string parentCompanyName2 = ReadString("ParentCompanyName2", string.Empty);
                    if (parentCompanyName2 != string.Empty)
                    {
                        parentCompany = new Organizations(_importUser);
                        parentCompany.LoadByName(parentCompanyName2, _organizationID);
                        if (parentCompany.Count == 1)
                        {
                            CustomerRelationship newCustomerRelationship = newCustomerRelationships.AddNewCustomerRelationship();
                            newCustomerRelationship.CustomerID = orgID;
                            newCustomerRelationship.RelatedCustomerID = parentCompany[0].OrganizationID;
                            newCustomerRelationship.CreatorID = -5;
                            if (dateCreated != null)
                            {
                                newCustomerRelationship.DateCreated = (DateTime)dateCreated;
                            }
                        }
                        else
                        {
                            _importLog.Write(messagePrefix + ". No company matching ParentCompanyName " + parentCompanyName2 + " was found for parent companies.");
                        }
                    }
                }


                bool parentCompany3Found = false;
                int parentCompanyID3 = ReadInt("ParentCompanyID3");
                if (parentCompanyID3 != 0)
                {
                    parentCompany = new Organizations(_importUser);
                    parentCompany.LoadByOrganizationID(parentCompanyID3);
                    if (parentCompany.Count == 1 && parentCompany[0].ParentID == _organizationID)
                    {
                        parentCompany3Found = true;
                        CustomerRelationship newCustomerRelationship = newCustomerRelationships.AddNewCustomerRelationship();
                        newCustomerRelationship.CustomerID = orgID;
                        newCustomerRelationship.RelatedCustomerID = parentCompany[0].OrganizationID;
                        newCustomerRelationship.CreatorID = -5;
                        if (dateCreated != null)
                        {
                            newCustomerRelationship.DateCreated = (DateTime)dateCreated;
                        }
                    }
                    else
                    {
                        _importLog.Write(messagePrefix + ". No company matching ParentCompanyID3 " + parentCompanyID3.ToString() + " was found for parent companies.");
                    }
                }

                if (!parentCompany3Found)
                {
                    string parentImportID3 = ReadString("ParentCompanyImportID3", string.Empty);
                    if (parentImportID3 != string.Empty)
                    {
                        parentCompany = new Organizations(_importUser);
                        parentCompany.LoadByImportID(parentImportID3, _organizationID);
                        if (parentCompany.Count == 1)
                        {
                            parentCompany3Found = true;
                            CustomerRelationship newCustomerRelationship3 = newCustomerRelationships.AddNewCustomerRelationship();
                            newCustomerRelationship3.CustomerID = orgID;
                            newCustomerRelationship3.RelatedCustomerID = parentCompany[0].OrganizationID;
                            newCustomerRelationship3.CreatorID = -5;
                            if (dateCreated != null)
                            {
                                newCustomerRelationship3.DateCreated = (DateTime)dateCreated;
                            }
                        }
                        else
                        {
                            _importLog.Write(messagePrefix + ". No company matching ParentCompanyImportID3 " + parentImportID3 + " was found for parent companies.");
                        }
                    }
                }

                if (!parentCompany3Found)
                {
                    string parentCompanyName3 = ReadString("ParentCompanyName3", string.Empty);
                    if (parentCompanyName3 != string.Empty)
                    {
                        parentCompany.LoadByName(parentCompanyName3, _organizationID);
                        if (parentCompany.Count == 1)
                        {
                            CustomerRelationship newCustomerRelationship = newCustomerRelationships.AddNewCustomerRelationship();
                            newCustomerRelationship.CustomerID = orgID;
                            newCustomerRelationship.RelatedCustomerID = parentCompany[0].OrganizationID;
                            newCustomerRelationship.CreatorID = -5;
                            if (dateCreated != null)
                            {
                                newCustomerRelationship.DateCreated = (DateTime)dateCreated;
                            }
                        }
                        else
                        {
                            _importLog.Write(messagePrefix + ". No company matching ParentCompanyName " + parentCompanyName3 + " was found for parent companies.");
                        }
                    }
                }

                bool customerRelationshipAdded = false;

                if (newCustomerRelationships.Count > 0)
                {
                    customerRelationshipAdded = true;
                    bulkCount++;
                    CustomerRelationship customerRelationship = customerRelationships.AddNewCustomerRelationship();
                    customerRelationship.CustomerID = newCustomerRelationships[0].CustomerID;
                    customerRelationship.RelatedCustomerID = newCustomerRelationships[0].RelatedCustomerID;
                    customerRelationship.DateCreated = newCustomerRelationships[0].DateCreated;
                    customerRelationship.CreatorID = newCustomerRelationships[0].CreatorID;

                    _importLog.Write(messagePrefix + "Parent Company added to bulk insert.");

                    if (newCustomerRelationships.Count > 1)
                    {
                        bulkCount++;
                        CustomerRelationship customerRelationship2 = customerRelationships.AddNewCustomerRelationship();
                        customerRelationship2.CustomerID = newCustomerRelationships[1].CustomerID;
                        customerRelationship2.RelatedCustomerID = newCustomerRelationships[1].RelatedCustomerID;
                        customerRelationship2.DateCreated = newCustomerRelationships[1].DateCreated;
                        customerRelationship2.CreatorID = newCustomerRelationships[1].CreatorID;

                        _importLog.Write(messagePrefix + "Parent Company 2 added to bulk insert.");


                        if (newCustomerRelationships.Count > 2)
                        {
                            bulkCount++;
                            CustomerRelationship customerRelationship3 = customerRelationships.AddNewCustomerRelationship();
                            customerRelationship3.CustomerID = newCustomerRelationships[2].CustomerID;
                            customerRelationship3.RelatedCustomerID = newCustomerRelationships[2].RelatedCustomerID;
                            customerRelationship3.DateCreated = newCustomerRelationships[2].DateCreated;
                            customerRelationship3.CreatorID = newCustomerRelationships[2].CreatorID;

                            _importLog.Write(messagePrefix + "Parent Company 3 added to bulk insert.");
                        }
                    }
                }
                if (customerRelationshipAdded)
                {
                    count++;
                }

                if (bulkCount % BULK_LIMIT == 0)
                {
                    customerRelationships.BulkSave();
                    customerRelationships = new CustomerRelationships(_importUser);
                    //UpdateImportCount(import, count);
                    _importLog.Write("Import set with " + count.ToString() + " children companies inserted in database.");
                }
            }

            customerRelationships.BulkSave();
            //UpdateImportCount(import, count);
            _importLog.Write("Import set with " + count.ToString() + " children companies inserted in database.");
        }

        private void ImportTickets(Import import)
        {
            SortedList<string, int> userList = GetUserAndContactList();
            SortedList<string, int> userOnlyList = GetUserList();
            SortedList<string, int> usersAndContactsFirstAndLastNameList = new SortedList<string, int>();
            SortedList<string, int> usersAndContactsImportIDList = new SortedList<string, int>();

            TicketTypes ticketTypes = new TicketTypes(_importUser);
            ticketTypes.LoadAllPositions(_organizationID);

            TicketStatuses ticketStatuses = new TicketStatuses(_importUser);
            ticketStatuses.LoadByOrganizationID(_organizationID);

            TicketSeverities ticketSeverities = new TicketSeverities(_importUser);
            ticketSeverities.LoadByOrganizationID(_organizationID);

            Groups groups = new Groups(_importUser);
            groups.LoadByOrganizationID(_organizationID);

            Organizations account = new Organizations(_importUser);
            account.LoadByOrganizationID(_organizationID);

            Products products = new Products(_importUser);
            products.LoadByOrganizationID(_organizationID);

            ProductVersions productVersions = new ProductVersions(_importUser);
            productVersions.LoadByParentOrganizationID(_organizationID);

            ProductVersionStatuses productVersionStatuses = new ProductVersionStatuses(_importUser);
            productVersionStatuses.LoadByOrganizationID(_organizationID);

            KnowledgeBaseCategories kbCats = new KnowledgeBaseCategories(_importUser);
            kbCats.LoadAllCategories(_organizationID);

            ForumCategories forumCategories = new ForumCategories(_importUser);
            forumCategories.LoadCategoriesAndSubcategories(_organizationID);

            Tags tags = new TeamSupport.Data.Tags(_importUser);

            Tickets tickets = new Tickets(_importUser);

            int count = 0;

            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";

                //Tickets existingTicket = new Tickets(_importUser);
                Ticket ticket = null;
                //bool isUpdate = false;

                //int ticketID = ReadInt("TicketID");
                //if (ticketID != 0)
                //{
                //  existingTicket = new Tickets(_importUser);
                //  existingTicket.LoadByTicketID(ticketID);
                //  if (existingTicket.Count == 1 && existingTicket[0].OrganizationID == _organizationID)
                //  {
                //    ticket = existingTicket[0];
                //    isUpdate = true;
                //  }
                //  else
                //  {
                //    _importLog.Write(messagePrefix + "Skipped. No ticket matching TicketID: " + ticketID.ToString() + " was found.");
                //    continue;
                //  }
                //}

                //if (ticket == null)
                //{
                string importID = ReadString("TicketImportID", string.Empty);
                if (importID != string.Empty)
                {
                    //existingTicket = new Tickets(_importUser);
                    //existingTicket.LoadByImportID(importID, _organizationID);
                    //if (existingTicket.Count == 1)
                    //{
                    //  ticket = existingTicket[0];
                    //  isUpdate = true;
                    //}
                    //else if (existingTicket.Count > 1)
                    //{
                    //  _importLog.Write(messagePrefix + "Skipped. More than one action matching the importID was found.");
                    //  continue;
                    //}
                }
                else
                {
                    _importLog.Write(messagePrefix + "Skipped. TicketImportID is required.");
                    continue;
                }
                //}

                int? ticketNumber;
                ticketNumber = ReadIntNull("TicketNumber", string.Empty);
                //if (ticket == null)
                //{
                //  if (ticketNumber != null)
                //  {
                //    existingTicket = new Tickets(_importUser);
                //    existingTicket.LoadByTicketNumber(_organizationID, (int)ticketNumber);
                //    if (existingTicket.Count == 1)
                //    {
                //      ticket = existingTicket[0];
                //      isUpdate = true;
                //      //maxTicketNumber = Math.Max((int)ticketNumber, maxTicketNumber);
                //    }
                //    else if (existingTicket.Count > 1)
                //    {
                //      _importLog.Write(messagePrefix + "Skipped. More than one action matching the TicketNumber was found.");
                //      continue;
                //    }
                //  }
                //  //else
                //  //{
                //  //  ticketNumber = maxTicketNumber + 1;
                //  //  maxTicketNumber++;
                //  //}
                //}
                ////else
                ////{
                ////  ticketNumber = ticket.TicketNumber;
                ////}

                //if (ticket == null)
                //{
                tickets = new Tickets(_importUser);
                int maxTicketNumber = tickets.GetMaxTicketNumber(_organizationID);
                //if (maxTicketNumber < 0) maxTicketNumber++;
                ticket = tickets.AddNewTicket();
                if (ticketNumber != null)
                {
                    ticket.TicketNumber = (int)ticketNumber;
                }
                else
                {
                    maxTicketNumber++;
                    ticket.TicketNumber = maxTicketNumber;
                }
                //}
                //ticket.TicketNumber = (int)ticketNumber;

                string name = ReadString("Name", string.Empty);
                if (string.IsNullOrEmpty(name))
                {
                    //if (!isUpdate)
                    //{
                    _importLog.Write(messagePrefix + "Skipped. Ticket Name is required.");
                    continue;
                    //}
                }
                else
                {
                    ticket.Name = name;
                }

                int creatorID = -5;
                if (Int32.TryParse(ReadString("CreatorID", creatorID.ToString()), out creatorID))
                {
                    if (!userList.ContainsValue(creatorID))
                    {
                        _importLog.Write("No CreatorID " + creatorID.ToString() + " found.");
                        creatorID = -5;
                    }
                }

                if (creatorID == -5)
                {
                    string creatorImportID = ReadString("CreatorImportID", string.Empty);
                    if (creatorImportID != string.Empty)
                    {
                        if (usersAndContactsImportIDList.Count == 0)
                        {
                            usersAndContactsImportIDList = GetUserAndContactImportIdList();
                        }

                        if (usersAndContactsImportIDList.TryGetValue(creatorImportID.ToUpper(), out creatorID))
                        {
                        }
                        else
                        {
                            _importLog.Write("No CreatorImportID " + creatorImportID + " found.");
                        }
                    }
                    else
                    {
                        string creatorEmail = ReadString("CreatorEmail", string.Empty);
                        if (creatorEmail != string.Empty)
                        {
                            if (userList.TryGetValue(creatorEmail.ToUpper(), out creatorID))
                            {
                            }
                            else
                            {
                                _importLog.Write("No CreatorEmail " + creatorEmail + " found.");
                            }
                        }
                        else
                        {
                            string creatorName = ReadString("CreatorName", string.Empty);
                            {
                                if (creatorName != string.Empty)
                                {
                                    if (usersAndContactsFirstAndLastNameList.Count == 0)
                                    {
                                        usersAndContactsFirstAndLastNameList = GetUserAndContactNameAndLastNameList();
                                    }

                                    if (usersAndContactsFirstAndLastNameList.TryGetValue(creatorName.ToUpper(), out creatorID))
                                    {
                                    }
                                    else
                                    {
                                        _importLog.Write("No CreatorName " + creatorName + " found.");
                                    }
                                }
                            }
                        }
                    }
                }

                DateTime now = DateTime.UtcNow;

                TicketType ticketType = null;
                string ticketTypeString = ReadString("Type", string.Empty);
                if (string.IsNullOrEmpty(ticketTypeString))
                {
                    //if (!isUpdate)
                    //{
                    _importLog.Write(messagePrefix + "Skipped. Ticket skipped due to missing type");
                    continue;
                    //}
                }
                else
                {
                    ticketType = ticketTypes.FindByName(ticketTypeString);
                    if (ticketType == null)
                    {
                        TicketTypes newTicketTypes = new TicketTypes(_importUser);
                        ticketType = newTicketTypes.AddNewTicketType();
                        ticketType.Name = ticketTypeString;
                        ticketType.Description = ticketTypeString;
                        ticketType.Position = newTicketTypes.GetMaxPosition(_organizationID) + 1;
                        ticketType.OrganizationID = _organizationID;
                        ticketType.CreatorID = -5;
                        ticketType.ModifierID = -5;
                        ticketType.DateCreated = now;
                        ticketType.DateModified = now;
                        newTicketTypes.Save();
                        newTicketTypes.ValidatePositions(_organizationID);
                        ticketTypes = new TicketTypes(_importUser);
                        ticketTypes.LoadAllPositions(_organizationID);

                        TicketStatuses newTicketStatuses = new TicketStatuses(_importUser);
                        TicketStatus newTicketStatus = newTicketStatuses.AddNewTicketStatus();
                        newTicketStatus.Name = "New";
                        newTicketStatus.Description = "New";
                        newTicketStatus.Position = 0;
                        newTicketStatus.OrganizationID = _organizationID;
                        newTicketStatus.TicketTypeID = ticketType.TicketTypeID;
                        newTicketStatus.IsClosed = false;
                        newTicketStatus.IsClosedEmail = false;
                        newTicketStatus.CreatorID = -5;
                        newTicketStatus.DateCreated = now;
                        newTicketStatus.ModifierID = -5;
                        newTicketStatus.DateModified = now;

                        newTicketStatus = newTicketStatuses.AddNewTicketStatus();
                        newTicketStatus.Name = "Closed";
                        newTicketStatus.Description = "Closed";
                        newTicketStatus.Position = 30;
                        newTicketStatus.OrganizationID = _organizationID;
                        newTicketStatus.TicketTypeID = ticketType.TicketTypeID;
                        newTicketStatus.IsClosed = true;
                        newTicketStatus.IsClosedEmail = false;
                        newTicketStatus.CreatorID = -5;
                        newTicketStatus.DateCreated = now;
                        newTicketStatus.ModifierID = -5;
                        newTicketStatus.DateModified = now;
                        newTicketStatus.Collection.Save();
                        newTicketStatus.Collection.ValidatePositions(_organizationID);

                        ticketStatuses = new TicketStatuses(_importUser);
                        ticketStatuses.LoadByOrganizationID(_organizationID);
                    }
                    ticket.TicketTypeID = ticketType.TicketTypeID;
                }

                TicketStatus ticketStatus = null;
                string ticketStatusString = ReadString("Status", string.Empty);
                if (string.IsNullOrEmpty(ticketStatusString))
                {
                    //if (!isUpdate)
                    //{
                    _importLog.Write(messagePrefix + "Skipped. Ticket Status is required.");
                    continue;
                    //}
                }
                else
                {
                    ticketStatus = ticketStatuses.FindByName(ticketStatusString, ticketType.TicketTypeID);
                    if (ticketStatus == null)
                    {
                        TicketStatuses newTicketStatuses = new TicketStatuses(_importUser);
                        ticketStatus = newTicketStatuses.AddNewTicketStatus();
                        ticketStatus.Name = ticketStatusString;
                        ticketStatus.Description = ticketStatusString;
                        ticketStatus.Position = newTicketStatuses.GetMaxPosition(ticketType.TicketTypeID) + 1;
                        ticketStatus.OrganizationID = _organizationID;
                        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                        ticketStatus.IsClosed = false;
                        ticketStatus.IsClosedEmail = false;
                        ticketStatus.CreatorID = -5;
                        ticketStatus.DateCreated = now;
                        ticketStatus.ModifierID = -5;
                        ticketStatus.DateModified = now;
                        newTicketStatuses.Save();
                        newTicketStatuses.ValidatePositions(_organizationID);

                        ticketStatuses = new TicketStatuses(_importUser);
                        ticketStatuses.LoadByOrganizationID(_organizationID);
                    }
                    ticket.TicketStatusID = ticketStatus.TicketStatusID;
                }

                TicketSeverity ticketSeverity = null;
                string ticketSeverityString = ReadString("Severity", string.Empty);
                if (string.IsNullOrEmpty(ticketSeverityString))
                {
                    //if (!isUpdate)
                    //{
                    _importLog.Write(messagePrefix + "Skipped. Ticket Severity is required.");
                    continue;
                    //}
                }
                else
                {
                    ticketSeverity = ticketSeverities.FindByName(ticketSeverityString);
                    if (ticketSeverity == null)
                    {
                        TicketSeverities newTicketSeverities = new TicketSeverities(_importUser);
                        ticketSeverity = newTicketSeverities.AddNewTicketSeverity();
                        ticketSeverity.Name = ticketSeverityString;
                        ticketSeverity.Description = ticketSeverityString;
                        ticketSeverity.Position = newTicketSeverities.GetMaxPosition(_organizationID) + 1;
                        ticketSeverity.OrganizationID = _organizationID;
                        ticketSeverity.CreatorID = -5;
                        ticketSeverity.DateCreated = now;
                        ticketSeverity.ModifierID = -5;
                        ticketSeverity.DateModified = now;
                        newTicketSeverities.Save();
                        newTicketSeverities.ValidatePositions(_organizationID);

                        ticketSeverities = new TicketSeverities(_importUser);
                        ticketSeverities.LoadByOrganizationID(_organizationID);
                    }
                    ticket.TicketSeverityID = ticketSeverity.TicketSeverityID;
                }

                string emailOfUserAssignedTo = ReadString("EmailOfUserAssignedTo", string.Empty);
                if (!string.IsNullOrEmpty(emailOfUserAssignedTo))
                {
                    int userID;
                    if (userOnlyList.TryGetValue(emailOfUserAssignedTo.ToUpper(), out userID))
                    {
                        ticket.UserID = userID;
                    }
                    //else if (usersFirstAndLastNameList.TryGetValue(emailOfUserAssignedTo.ToUpper(), out userID))
                    //{
                    //    ticket.UserID = userID;
                    //}
                }

                int closerID = ReadInt("CloserID", 0);
                if (closerID != 0)
                {
                    if (!userList.ContainsValue(closerID))
                    {
                        _importLog.Write("No CloserID " + closerID.ToString() + " found.");
                        closerID = 0;
                    }
                }
                else
                {
                    string emailOfCloser = ReadString("EmailOfCloser", string.Empty);
                    if (!string.IsNullOrEmpty(emailOfCloser))
                    {
                        if (userList.TryGetValue(emailOfCloser.ToUpper(), out closerID))
                        {
                        }
                        else
                        {
                            _importLog.Write("No EmailOfCloser " + emailOfCloser + " found.");
                        }
                    }
                }

                if (closerID != 0)
                {
                    ticket.CloserID = closerID;
                }

                string groupName = ReadString("Group", string.Empty);
                if (!string.IsNullOrEmpty(groupName))
                {
                    TeamSupport.Data.Group group = groups.FindByName(groupName);
                    if (group == null)
                    {
                        Groups newGroups = new Groups(_importUser);
                        group = newGroups.AddNewGroup();
                        group.Name = groupName;
                        group.Description = groupName;
                        group.OrganizationID = _organizationID;
                        group.CreatorID = -5;
                        group.ModifierID = -5;
                        group.DateCreated = now;
                        group.DateModified = now;
                        newGroups.Save();

                        groups = new Groups(_importUser);
                        groups.LoadByOrganizationID(_organizationID);
                    }
                    ticket.GroupID = group.GroupID;
                }

                ticket.DueDate = ReadDateNull("DueDate", string.Empty);

                Product product = null;
                int productID = ReadInt("ProductID");
                if (productID != 0)
                {
                    product = products.FindByProductID(productID);
                    if (product == null)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No product matching ProductID " + productID.ToString() + " was found.");
                        continue;
                    }
                }

                if (productID == 0)
                {
                    string productImportID = ReadString("ProductImportID", string.Empty);
                    if (productImportID != string.Empty)
                    {
                        product = products.FindByImportID(productImportID);
                        if (product == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No product matching ProductImportID " + productImportID + " was found.");
                            continue;
                        }
                        else
                        {
                            productID = product.ProductID;
                        }
                    }
                }

                if (productID == 0)
                {
                    string productName = ReadString("Product", string.Empty);
                    if (!string.IsNullOrEmpty(productName))
                    {
                        product = products.FindByName(productName);
                        if (product == null)
                        {
                            Products newProducts = new Products(_importUser);
                            product = newProducts.AddNewProduct();
                            product.Name = productName;
                            product.Description = productName;
                            product.OrganizationID = _organizationID;
                            product.CreatorID = -5;
                            product.ModifierID = -5;
                            product.DateCreated = now;
                            product.DateModified = now;
                            product.ImportFileID = import.ImportID;

                            newProducts.Save();
                            products = new Products(_importUser);
                            products.LoadByOrganizationID(_organizationID);
                        }
                        productID = product.ProductID;
                    }
                }


                if (productID == 0)
                {
                    if (account[0].ProductRequired)
                    {
                        _importLog.Write(messagePrefix + "Skipped. Product is required.");
                        continue;
                    }
                }
                else
                {
                    ticket.ProductID = productID;
                }

                ProductVersion reportedVersion;
                int reportedVersionID = ReadInt("ReportedVersionID");
                if (reportedVersionID != 0)
                {
                    reportedVersion = productVersions.FindByProductVersionID(reportedVersionID);
                    if (reportedVersion == null || reportedVersion.ProductID != ticket.ProductID)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No product version matching ReportedVersionID " + reportedVersion.ProductID.ToString() + " was found.");
                        continue;
                    }
                }

                if (reportedVersionID == 0)
                {
                    string reportedVersionImportID = ReadString("ReportedVersionImportID", string.Empty);
                    if (reportedVersionImportID != string.Empty)
                    {
                        reportedVersion = productVersions.FindByImportID(reportedVersionImportID, ticket.ProductID);
                        if (reportedVersion == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No product version matching ReportedVersionImportID " + reportedVersionImportID + " was found.");
                            continue;
                        }
                        else
                        {
                            reportedVersionID = reportedVersion.ProductVersionID;
                        }
                    }
                }

                string reportedVersionName = ReadString("ReportedVersion", string.Empty);
                if (reportedVersionID == 0 && !string.IsNullOrEmpty(reportedVersionName) && ticket.ProductID != null)
                {
                    reportedVersion = productVersions.FindByVersionNumber(reportedVersionName, (int)ticket.ProductID);
                    if (reportedVersion == null)
                    {
                        ProductVersions newProductVersions = new ProductVersions(_importUser);
                        reportedVersion = newProductVersions.AddNewProductVersion();
                        reportedVersion.VersionNumber = reportedVersionName;
                        reportedVersion.Description = reportedVersionName;
                        reportedVersion.ProductID = (int)ticket.ProductID;
                        reportedVersion.ProductVersionStatusID = productVersionStatuses[0].ProductVersionStatusID;
                        reportedVersion.IsReleased = true;
                        reportedVersion.CreatorID = -5;
                        reportedVersion.ModifierID = -5;
                        reportedVersion.DateCreated = now;
                        reportedVersion.DateModified = now;
                        reportedVersion.ImportFileID = import.ImportID;

                        newProductVersions.Save();

                        productVersions = new ProductVersions(_importUser);
                        productVersions.LoadByParentOrganizationID(_organizationID);
                    }
                    ticket.ReportedVersionID = reportedVersion.ProductVersionID;
                }
                else if (account[0].ProductVersionRequired)
                {
                    _importLog.Write(messagePrefix + "Skipped. Reported Version is required.");
                    continue;
                }

                ProductVersion resolvedVersion;
                int resolvedVersionID = ReadInt("ResolvedVersionID");
                if (resolvedVersionID != 0)
                {
                    resolvedVersion = productVersions.FindByProductVersionID(resolvedVersionID);
                    if (resolvedVersion == null || resolvedVersion.ProductID != ticket.ProductID)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No product version matching ResolvedVersionID " + resolvedVersionID.ToString() + " was found.");
                        continue;
                    }
                }

                if (resolvedVersionID == 0)
                {
                    string resolvedVersionImportID = ReadString("ResolvedVersionImportID", string.Empty);
                    if (resolvedVersionImportID != string.Empty)
                    {
                        resolvedVersion = productVersions.FindByImportID(resolvedVersionImportID, ticket.ProductID);
                        if (resolvedVersion == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No product version matching ResolvedVersionImportID " + resolvedVersionImportID + " was found.");
                            continue;
                        }
                        else
                        {
                            resolvedVersionID = resolvedVersion.ProductVersionID;
                        }
                    }
                }

                string resolvedVersionName = ReadString("ResolvedVersion", string.Empty);
                if (resolvedVersionID == 0 && !string.IsNullOrEmpty(resolvedVersionName) && ticket.ProductID != null)
                {
                    resolvedVersion = productVersions.FindByVersionNumber(resolvedVersionName, (int)ticket.ProductID);
                    if (resolvedVersion == null)
                    {
                        ProductVersions newProductVersions = new ProductVersions(_importUser);
                        resolvedVersion = newProductVersions.AddNewProductVersion();
                        resolvedVersion.VersionNumber = resolvedVersionName;
                        resolvedVersion.Description = resolvedVersionName;
                        resolvedVersion.ProductID = (int)ticket.ProductID;
                        resolvedVersion.ProductVersionStatusID = productVersionStatuses[0].ProductVersionStatusID;
                        resolvedVersion.IsReleased = true;
                        resolvedVersion.CreatorID = -5;
                        resolvedVersion.ModifierID = -5;
                        resolvedVersion.DateCreated = now;
                        resolvedVersion.DateModified = now;
                        resolvedVersion.ImportFileID = import.ImportID;
                        newProductVersions.Save();

                        productVersions = new ProductVersions(_importUser);
                        productVersions.LoadByParentOrganizationID(_organizationID);
                    }
                    ticket.SolvedVersionID = resolvedVersion.ProductVersionID;
                }

                ticket.IsKnowledgeBase = ReadBool("Knowledgebase", ticket.IsKnowledgeBase.ToString());
                string parentCatName = ReadString("KBParentCatName", string.Empty);
                string catName = ReadString("KBCatName", string.Empty);
                KnowledgeBaseCategory cat = null;
                if (!string.IsNullOrEmpty(catName))
                {
                    if (string.IsNullOrEmpty(parentCatName))
                    {
                        cat = kbCats.FindByName(catName, -1);
                        if (cat == null)
                        {
                            // craete cat
                            AddKnowledgeBaseCategory(null, catName);
                            kbCats = new KnowledgeBaseCategories(_loginUser);
                            kbCats.LoadAllCategories(_organizationID);
                            cat = kbCats.FindByName(catName, -1);
                        }
                    }
                    else
                    {
                        KnowledgeBaseCategory parent = kbCats.FindByName(parentCatName, -1);
                        if (parent != null)
                        {
                            cat = kbCats.FindByName(catName, parent.CategoryID);
                        }
                        else
                        {
                            // create parent 
                            KnowledgeBaseCategory parentCat = AddKnowledgeBaseCategory(null, parentCatName);
                            AddKnowledgeBaseCategory(parentCat.CategoryID, catName);
                            kbCats = new KnowledgeBaseCategories(_loginUser);
                            kbCats.LoadAllCategories(_organizationID);
                            cat = kbCats.FindByName(catName, parentCat.CategoryID);
                        }
                    }
                }

                if (cat != null)
                {
                    ticket.KnowledgeBaseCategoryID = cat.CategoryID;
                }

                ticket.IsVisibleOnPortal = ReadBool("VisibleToCustomers", ticket.IsVisibleOnPortal.ToString());
                ticket.OrganizationID = _organizationID;
                ticket.TicketSource = ReadString("Source", ticket.TicketSource);
                ticket.ImportID = importID;
                DateTime? dateCreated = ReadDateNull("DateCreated", ticket.DateCreated.ToString());
                if (dateCreated != null)
                {
                    ticket.DateCreated = (DateTime)dateCreated;
                }
                else
                {
                    ticket.DateCreated = now;
                }
                ticket.CreatorID = -5;
                ticket.DateModified = now;
                ticket.ModifierID = -5;
                ticket.ImportFileID = import.ImportID;

                DateTime? dateClosed = ReadDateNull("DateClosed", ticket.DateClosed.ToString());
                if (dateClosed != null)
                {
                    ticket.DateClosed = (DateTime)dateClosed;
                }

                //if (isUpdate)
                //{
                //  existingTicket.Save();
                //  _importLog.Write(messagePrefix + "Ticket: '" + ticket.Name + "' was updated.");
                //}
                //else
                //{
                tickets.Save();
                _importLog.Write(messagePrefix + "Ticket: '" + ticket.Name + "' was created with TicketID: " + ticket.TicketID.ToString());
                //}
                EmailPosts.DeleteImportEmailsWithRetryOnDeadLock(_importUser);
                count++;

                //if (count % BULK_LIMIT == 0)
                //{
                //  tickets.BulkSave();
                //  tickets = new Tickets(_importUser);
                //  UpdateImportCount(import, count);
                //  EmailPosts.DeleteImportEmails(_importUser);
                //}

                //if (isUpdate)
                //{
                //  Actions actions = new Actions(_importUser);
                //  actions.LoadOldestTicketDescription(ticket.TicketID);
                //  if (actions.Count == 1)
                //  {
                //    actions[0].Description = ReadString("Description");
                //    if (creatorID != -2 || creatorID != 0)
                //    {
                //      User user = Users.GetUser(_importUser, creatorID);
                //      if (user != null)
                //      {
                //        if (!string.IsNullOrWhiteSpace(user.Signature) && ticket.IsVisibleOnPortal)
                //        {
                //          actions[0].Description = actions[0].Description + "<br/><br/>" + user.Signature;
                //        }
                //      }
                //    }
                //    actions.Save();
                //  }
                //}
                //else
                //{
                Actions actions = new Actions(_importUser);
                TeamSupport.Data.Action action = actions.AddNewAction();
                action.ActionTypeID = null;
                action.Name = "Description";
                action.SystemActionTypeID = SystemActionType.Description;
                action.ActionSource = ticket.TicketSource;
                action.Description = ReadString("Description", string.Empty);

                if (creatorID != -5 || creatorID != 0)
                {
                    User user = Users.GetUser(_importUser, creatorID);
                    if (user != null)
                    {
                        if (!string.IsNullOrWhiteSpace(user.Signature) && ticket.IsVisibleOnPortal)
                        {
                            action.Description = action.Description + "<br/><br/>" + user.Signature;
                        }
                    }
                }

                action.IsVisibleOnPortal = ticket.IsVisibleOnPortal;
                action.IsKnowledgeBase = ticket.IsKnowledgeBase;
                action.TicketID = ticket.TicketID;
                action.ImportFileID = import.ImportID;
                //action.TimeSpent = info.TimeSpent;
                if (dateCreated != null)
                {
                    action.DateCreated = (DateTime)dateCreated;
                }
                else
                {
                    action.DateCreated = now;
                }
                actions.Save();
                //}
                EmailPosts.DeleteImportEmailsWithRetryOnDeadLock(_importUser);

                ForumCategory forumCategory = null;
                int categoryID = 0;
                string categoryName = string.Empty;

                categoryID = ReadInt("CategoryID");
                if (categoryID != 0)
                {
                    forumCategory = forumCategories.FindByCategoryID(categoryID);
                    if (forumCategory == null)
                    {
                        _importLog.Write("No category matching CategoryID " + categoryID.ToString() + " was found.");
                        //continue;
                    }
                    else
                    {
                        categoryName = forumCategory.CategoryName;
                    }
                }
                else
                {
                    categoryName = ReadString("CategoryName", string.Empty);
                    if (categoryName != string.Empty)
                    {
                        forumCategory = forumCategories.FindByName(categoryName);
                    }
                }

                if (forumCategory != null)
                {
                    if (forumCategory.TicketType == null || forumCategory.TicketType == ticket.TicketTypeID)
                    {
                        ForumTickets forumTickets = new ForumTickets(_loginUser);
                        ForumTicket forumTicket = forumTickets.AddNewForumTicket();
                        forumTicket.TicketID = ticket.TicketID;
                        forumTicket.ForumCategory = forumCategory.CategoryID;
                        forumTickets.Save();
                    }
                    else
                    {
                        _importLog.Write("Matching Category " + forumCategory.CategoryName.ToString() + " belongs to different ticket type.");
                    }
                }

                string tagsCSV = ReadString("Tags", string.Empty).Replace("\"", "");
                if (tagsCSV != string.Empty)
                {
                    TagLinks tagLinks = new TagLinks(_loginUser);

                    if (tags.Count == 0)
                    {
                        tags.LoadByOrganization(_organizationID);
                    }

                    string[] tagsToImport = tagsCSV.Split(',');
                    foreach (string tagToImport in tagsToImport)
                    {
                        Tag tag = tags.FindByValue(tagToImport);
                        if (tag == null)
                        {
                            Tags newTags = new Tags(_importUser);
                            tag = newTags.AddNewTag();
                            tag.OrganizationID = _organizationID;
                            tag.Value = tagToImport;
                            newTags.Save();
                        }

                        TagLink tagLink = tagLinks.AddNewTagLink();
                        tagLink.RefType = ReferenceType.Tickets;
                        tagLink.RefID = ticket.TicketID;
                        tagLink.TagID = tag.TagID;
                    }

                    if (tagLinks.Count > 0)
                    {
                        tagLinks.Save();
                    }
                }
            }
            //tickets.BulkSave();
            //UpdateImportCount(import, count);
            //EmailPosts.DeleteImportEmails(_importUser);
            _importLog.Write(count.ToString() + " tickets imported.");
        }

        private void ImportOrganizationTickets(Import import)
        {
            Organizations companies = new Organizations(_importUser);
            companies.LoadByParentID(_organizationID, false);

            Tickets tickets = new Tickets(_importUser);
            tickets.LoadByOrganizationID(_organizationID);

            Tickets newTickets = new Tickets(_importUser);
            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";

                int ticketID = ReadInt("TicketID");
                if (ticketID != 0)
                {
                    Ticket ticket = tickets.FindByTicketID(ticketID);
                    if (ticket == null)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No ticket matching TicketID: " + ticketID.ToString() + " was found.");
                        continue;
                    }
                }

                if (ticketID == 0)
                {
                    string importID = ReadString("TicketImportID", string.Empty);
                    if (importID != string.Empty)
                    {
                        Ticket ticket = tickets.FindByImportID(importID);
                        if (ticket == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one ticket matching the TicketImportID " + importID + " was found.");
                            continue;
                        }
                        else
                        {
                            ticketID = ticket.TicketID;
                        }
                    }
                }

                if (ticketID == 0)
                {
                    int? ticketNumber;
                    ticketNumber = ReadIntNull("TicketNumber", string.Empty);
                    if (ticketNumber != null)
                    {
                        Ticket ticket = tickets.FindByTicketNumber((int)ticketNumber);
                        if (ticket == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one ticket matching the TicketNumber " + ticketNumber.ToString() + " was found.");
                            continue;
                        }
                        else
                        {
                            ticketID = ticket.TicketID;
                        }
                    }
                }

                if (ticketID == 0)
                {
                    _importLog.Write(messagePrefix + "Skipped. No ticket matching either the TicketID, TicketImportID or the TicketNumber was found.");
                    continue;
                }

                int companyID = ReadInt("CompanyID");
                if (companyID != 0)
                {
                    Organization company = companies.FindByOrganizationID(companyID);
                    if (company == null)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyID " + companyID.ToString() + " was found.");
                        continue;
                    }
                }

                if (companyID == 0)
                {
                    string companyImportID = ReadString("CompanyImportID", string.Empty);
                    if (companyImportID != string.Empty)
                    {
                        Organization company = companies.FindOnlyByImportID(companyImportID);
                        if (company == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyImportID " + companyImportID + " was found.");
                            continue;
                        }
                        else
                        {
                            companyID = company.OrganizationID;
                        }
                    }
                }

                if (companyID == 0)
                {
                    string companyName = ReadString("CompanyName", string.Empty);
                    if (companyName != string.Empty)
                    {
                        Organization company = companies.FindByName(companyName);
                        if (company == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyName " + companyName + " was found.");
                            continue;
                        }
                        else
                        {
                            companyID = company.OrganizationID;
                        }
                    }
                }

                if (companyID == 0)
                {
                    _importLog.Write(messagePrefix + "No company matching either the CompanyID, the CompanyImportID or the CompanyName was found.");
                }
                else
                {
                    newTickets.AddOrganization(companyID, ticketID, import.ImportID);
                    _importLog.Write(messagePrefix + "CompanyID " + companyID.ToString() + " was added to TicketID " + ticketID.ToString() + ".");
                }

                int companyID2 = ReadInt("CompanyID2");
                if (companyID2 != 0)
                {
                    Organization company = companies.FindByOrganizationID(companyID2);
                    if (company == null)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyID2 " + companyID2.ToString() + " was found.");
                        continue;
                    }
                }

                if (companyID2 == 0)
                {
                    string companyImportID2 = ReadString("CompanyImportID2", string.Empty);
                    if (companyImportID2 != string.Empty)
                    {
                        Organization company = companies.FindOnlyByImportID(companyImportID2);
                        if (company == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyImportID2 " + companyImportID2 + " was found.");
                            continue;
                        }
                        else
                        {
                            companyID2 = company.OrganizationID;
                        }
                    }
                }

                if (companyID2 == 0)
                {
                    string companyName2 = ReadString("CompanyName2", string.Empty);
                    if (companyName2 != string.Empty)
                    {
                        Organization company = companies.FindByName(companyName2);
                        if (company == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyName2 " + companyName2 + " was found.");
                            continue;
                        }
                        else
                        {
                            companyID2 = company.OrganizationID;
                        }
                    }
                }

                if (companyID2 == 0)
                {
                    _importLog.Write(messagePrefix + "No company matching either the CompanyID2, the CompanyImportID2 or the CompanyName2 was found.");
                }
                else
                {
                    newTickets.AddOrganization(companyID2, ticketID, import.ImportID);
                    _importLog.Write(messagePrefix + "CompanyID2 " + companyID2.ToString() + " was added to TicketID " + ticketID.ToString() + ".");
                }

                int companyID3 = ReadInt("CompanyID3");
                if (companyID3 != 0)
                {
                    Organization company = companies.FindByOrganizationID(companyID3);
                    if (company == null)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyID3 " + companyID3.ToString() + " was found.");
                        continue;
                    }
                }

                if (companyID3 == 0)
                {
                    string companyImportID3 = ReadString("CompanyImportID3", string.Empty);
                    if (companyImportID3 != string.Empty)
                    {
                        Organization company = companies.FindOnlyByImportID(companyImportID3);
                        if (company == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyImportID3 " + companyImportID3 + " was found.");
                            continue;
                        }
                        else
                        {
                            companyID3 = company.OrganizationID;
                        }
                    }
                }

                if (companyID3 == 0)
                {
                    string companyName3 = ReadString("CompanyName3", string.Empty);
                    if (companyName3 != string.Empty)
                    {
                        Organization company = companies.FindByName(companyName3);
                        if (company == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyName3 " + companyName3 + " was found.");
                            continue;
                        }
                        else
                        {
                            companyID3 = company.OrganizationID;
                        }
                    }
                }

                if (companyID3 == 0)
                {
                    _importLog.Write(messagePrefix + "No company matching either the CompanyID3, the CompanyImportID3 or the CompanyName3 was found.");
                }
                else
                {
                    newTickets.AddOrganization(companyID3, ticketID, import.ImportID);
                    _importLog.Write(messagePrefix + "CompanyID3 " + companyID3.ToString() + " was added to TicketID " + ticketID.ToString() + ".");
                }
            }
        }

        private void ImportContactTickets(Import import)
        {
            Users contacts = new Users(_importUser);
            contacts.LoadContacts(_organizationID, false);

            Organizations companies = new Organizations(_importUser);
            companies.LoadByParentID(_organizationID, false);

            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";

                Tickets ticket = new Tickets(_importUser);

                int ticketID = ReadInt("TicketID");
                if (ticketID != 0)
                {
                    ticket.LoadByTicketID(ticketID);
                    if (ticket.Count == 0 || ticket[0].OrganizationID != _organizationID)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No ticket matching TicketID: " + ticketID.ToString() + " was found processing ticket contacts.");
                        continue;
                    }
                }

                if (ticket.Count == 0)
                {
                    string importID = ReadString("TicketImportID", string.Empty);
                    if (importID != string.Empty)
                    {
                        ticket = new Tickets(_importUser);
                        ticket.LoadByImportID(importID, _organizationID);
                        if (ticket.Count > 1)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one ticket matching the TicketImportID " + importID + " was found.");
                            continue;
                        }
                    }
                }

                if (ticket.Count == 0)
                {
                    int? ticketNumber;
                    ticketNumber = ReadIntNull("TicketNumber", string.Empty);
                    if (ticketNumber != null)
                    {
                        ticket = new Tickets(_importUser);
                        ticket.LoadByTicketNumber(_organizationID, (int)ticketNumber);
                        if (ticket.Count > 1)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one ticket matching the TicketNumber " + ticketNumber.ToString() + " was found.");
                            continue;
                        }
                    }
                }

                if (ticket.Count == 0)
                {
                    _importLog.Write(messagePrefix + "Skipped. No ticket matching either the TicketImportID or the TicketNumber was found.");
                    continue;
                }

                User contact = null;

                int contactID = ReadInt("ContactID");
                if (contactID != 0)
                {
                    contact = contacts.FindByUserID(contactID);
                    if (contact == null)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No contact matching the ContactID " + contactID.ToString() + " was found.");
                        continue;
                    }
                }

                string contactImportID = ReadString("ContactImportID", string.Empty);
                if (contactID == 0 && contactImportID != string.Empty)
                {
                    contact = contacts.FindByImportID(contactImportID);
                    if (contact == null)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No contact matching the ContactImportID " + contactImportID + " was found.");
                        continue;
                    }
                    else
                    {
                        contactID = contact.UserID;
                    }
                }

                if (contactID == 0)
                {
                    Organization company = null;

                    int companyID = ReadInt("CompanyID");
                    if (companyID != 0)
                    {
                        company = companies.FindByOrganizationID(companyID);
                        if (company == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyID " + companyID.ToString() + " was found.");
                            continue;
                        }
                    }

                    string companyImportID = ReadString("CompanyImportID", string.Empty);
                    if (companyID == 0)
                    {
                        if (companyImportID != string.Empty)
                        {
                            company = companies.FindOnlyByImportID(companyImportID);
                            if (company == null)
                            {
                                _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyImportID " + companyImportID + " was found.");
                                continue;
                            }
                            else
                            {
                                companyID = company.OrganizationID;
                            }
                        }
                    }

                    if (companyID == 0)
                    {
                        string companyName = ReadString("CompanyName", string.Empty);
                        if (companyName != string.Empty)
                        {
                            company = companies.FindByName(companyName);
                            if (company == null)
                            {
                                _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyName " + companyName + " was found.");
                                continue;
                            }
                            else
                            {
                                companyID = company.OrganizationID;
                            }
                        }
                    }

                    if (companyID == 0)
                    {
                        string contactEmail = ReadString("ContactEmail", string.Empty);
                        if (contactEmail != string.Empty)
                        {
                            contact = contacts.FindByEmail(contactEmail);
                            if (contact == null)
                            {
                                _importLog.Write(messagePrefix + "Skipped. No contact matching the ContactEmail " + contactEmail + " was found.");
                                continue;
                            }
                            else
                            {
                                contactID = contact.UserID;
                            }
                        }

                        if (contactID == 0)
                        {
                            string firstName = ReadString("FirstName", string.Empty);
                            string lastName = ReadString("LastName", string.Empty);
                            if (firstName != string.Empty || lastName != string.Empty)
                            {
                                contact = contacts.FindByName(firstName, lastName);
                                if (contact == null)
                                {
                                    _importLog.Write(messagePrefix + "Skipped. No contact matching " + firstName + " " + lastName + " was found.");
                                    continue;
                                }
                                else
                                {
                                    contactID = contact.UserID;
                                }
                            }
                        }
                    }
                    else
                    {
                        string contactEmail = ReadString("ContactEmail", string.Empty);
                        if (contactEmail != string.Empty)
                        {
                            contact = contacts.FindByEmailAndOrganization(contactEmail, companyID);
                            if (contact == null)
                            {
                                _importLog.Write(messagePrefix + "Skipped. No contact matching the ContactEmail " + contactEmail + " was found.");
                                continue;
                            }
                            else
                            {
                                contactID = contact.UserID;
                            }
                        }

                        if (contactID == 0)
                        {
                            string firstName = ReadString("FirstName", string.Empty);
                            string lastName = ReadString("LastName", string.Empty);
                            if (firstName != string.Empty || lastName != string.Empty)
                            {
                                contact = contacts.FindByNameAndOrganization(firstName, lastName, companyID);
                                if (contact == null)
                                {
                                    _importLog.Write(messagePrefix + "Skipped. No contact matching " + firstName + " " + lastName + " was found.");
                                    continue;
                                }
                                else
                                {
                                    contactID = contact.UserID;
                                }
                            }
                        }
                    }
                }

                if (contactID != 0)
                {
                    ticket.AddContact(contactID, ticket[0].TicketID, import.ImportID);
                    _importLog.Write(messagePrefix + "Contact " + contact.FirstLastName + " was added to ticket number " + ticket[0].TicketNumber.ToString() + ".");
                }
                else
                {
                    _importLog.Write(messagePrefix + "No contact found to add to ticket number " + ticket[0].TicketNumber.ToString() + ".");
                }

                User contact2 = null;
                int contact2ID = ReadInt("ContactID2");
                if (contact2ID != 0)
                {
                    contact2 = contacts.FindByUserID(contact2ID);
                    if (contact2 == null)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No contact matching the ContactID2 " + contact2ID.ToString() + " was found.");
                        continue;
                    }
                }

                string contactImportID2 = ReadString("ContactImportID2", string.Empty);
                if (contact2ID == 0 && contactImportID2 != string.Empty)
                {
                    contact2 = contacts.FindByImportID(contactImportID2);
                    if (contact2 == null)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No contact matching the ContactImportID2 " + contactImportID2 + " was found.");
                        continue;
                    }
                    else
                    {
                        contact2ID = contact2.UserID;
                    }
                }

                if (contact2ID == 0)
                {
                    Organization company2 = null;
                    int company2ID = ReadInt("CompanyID2");
                    if (company2ID != 0)
                    {
                        company2 = companies.FindByOrganizationID(company2ID);
                        if (company2 == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyID2 " + company2ID.ToString() + " was found.");
                            continue;
                        }
                    }

                    string companyImportID2 = ReadString("CompanyImportID2", string.Empty);
                    if (company2ID == 0 && companyImportID2 != string.Empty)
                    {
                        company2 = companies.FindOnlyByImportID(companyImportID2);
                        if (company2 == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyImportID2 " + companyImportID2 + " was found.");
                            continue;
                        }
                        else
                        {
                            company2ID = company2.OrganizationID;
                        }
                    }

                    if (company2ID == 0)
                    {
                        string companyName2 = ReadString("CompanyName2", string.Empty);
                        if (companyName2 != string.Empty)
                        {
                            company2 = companies.FindByName(companyName2);
                            if (company2 == null)
                            {
                                _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyName2 " + companyName2 + " was found.");
                                continue;
                            }
                            else
                            {
                                company2ID = company2.OrganizationID;
                            }
                        }
                    }

                    if (company2ID == 0)
                    {
                        string contactEmail2 = ReadString("ContactEmail2", string.Empty);
                        if (contactEmail2 != string.Empty)
                        {
                            contact2 = contacts.FindByEmail(contactEmail2);
                            if (contact2 == null)
                            {
                                _importLog.Write(messagePrefix + "Skipped. No contact matching the ContactEmail2 " + contactEmail2 + " was found.");
                                continue;
                            }
                            else
                            {
                                contact2ID = contact2.UserID;
                            }
                        }

                        if (contact2ID == 0)
                        {
                            string firstName2 = ReadString("FirstName2", string.Empty);
                            string lastName2 = ReadString("LastName2", string.Empty);
                            if (firstName2 != string.Empty || lastName2 != string.Empty)
                            {
                                contact2 = contacts.FindByName(firstName2, lastName2);
                                if (contact2 == null)
                                {
                                    _importLog.Write(messagePrefix + "Skipped. No contact matching the first and last name 2 (" + firstName2 + " " + lastName2 + ") was found.");
                                    continue;
                                }
                                else
                                {
                                    contact2ID = contact2.UserID;
                                }
                            }
                        }
                    }
                    else
                    {
                        string contactEmail2 = ReadString("ContactEmail2", string.Empty);
                        if (contactEmail2 != string.Empty)
                        {
                            contact2 = contacts.FindByEmailAndOrganization(contactEmail2, company2ID);
                            if (contact2 == null)
                            {
                                _importLog.Write(messagePrefix + "Skipped. No contact matching the ContactEmail2 " + contactEmail2 + " was found.");
                                continue;
                            }
                            else
                            {
                                contact2ID = contact2.UserID;
                            }
                        }

                        if (contact2ID == 0)
                        {
                            string firstName2 = ReadString("FirstName2", string.Empty);
                            string lastName2 = ReadString("LastName2", string.Empty);
                            if (firstName2 != string.Empty || lastName2 != string.Empty)
                            {
                                contact2 = contacts.FindByNameAndOrganization(firstName2, lastName2, company2ID);
                                if (contact2 == null)
                                {
                                    _importLog.Write(messagePrefix + "Skipped. No contact matching the first and last name 2 (" + firstName2 + " " + lastName2 + ") was found.");
                                    continue;
                                }
                                else
                                {
                                    contact2ID = contact2.UserID;
                                }
                            }
                        }
                    }
                }

                if (contact2ID != 0)
                {
                    ticket.AddContact(contact2ID, ticket[0].TicketID, import.ImportID);
                    _importLog.Write(messagePrefix + "Contact " + contact2.FirstLastName + " was added to ticket number " + ticket[0].TicketNumber.ToString() + ".");
                }
                else
                {
                    _importLog.Write(messagePrefix + "No contact2 found to add to ticket number " + ticket[0].TicketNumber.ToString() + ".");
                }

                User contact3 = null;
                int contact3ID = ReadInt("ContactID3");
                if (contact3ID != 0)
                {
                    contact3 = contacts.FindByUserID(contact3ID);
                    if (contact3 == null)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No contact matching the ContactID3 " + contact3ID.ToString() + " was found.");
                        continue;
                    }
                }

                string contactImportID3 = ReadString("ContactImportID3", string.Empty);
                if (contact3ID == 0 && contactImportID3 != string.Empty)
                {
                    contact3 = contacts.FindByImportID(contactImportID3);
                    if (contact3 == null)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No contact matching the ContactImportID3 " + contactImportID3 + " was found.");
                        continue;
                    }
                    else
                    {
                        contact3ID = contact3.UserID;
                    }
                }

                if (contact3ID == 0)
                {
                    Organization company3 = null;
                    int company3ID = ReadInt("CompanyID3");
                    if (company3ID != 0)
                    {
                        company3 = companies.FindByOrganizationID(company3ID);
                        if (company3 == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyID3 " + company3ID.ToString() + " was found.");
                            continue;
                        }
                    }

                    string companyImportID3 = ReadString("CompanyImportID3", string.Empty);
                    if (company3ID == 0 && companyImportID3 != string.Empty)
                    {
                        company3 = companies.FindOnlyByImportID(companyImportID3);
                        if (company3 == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyImportID3 " + companyImportID3 + " was found.");
                            continue;
                        }
                        else
                        {
                            company3ID = company3.OrganizationID;
                        }
                    }

                    if (company3ID == 0)
                    {
                        string companyName3 = ReadString("CompanyName3", string.Empty);
                        if (companyName3 != string.Empty)
                        {
                            company3 = companies.FindByName(companyName3);
                            if (company3 == null)
                            {
                                _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyName3 " + companyName3 + " was found.");
                                continue;
                            }
                            else
                            {
                                company3ID = company3.OrganizationID;
                            }
                        }
                    }

                    if (company3ID == 0)
                    {
                        string contactEmail3 = ReadString("ContactEmail3", string.Empty);
                        if (contactEmail3 != string.Empty)
                        {
                            contact3 = contacts.FindByEmail(contactEmail3);
                            if (contact3 == null)
                            {
                                _importLog.Write(messagePrefix + "Skipped. No contact matching the ContactEmail3 " + contactEmail3 + " was found.");
                                continue;
                            }
                            else
                            {
                                contact3ID = contact3.UserID;
                            }
                        }

                        if (contact3ID == 0)
                        {
                            string firstName3 = ReadString("FirstName3", string.Empty);
                            string lastName3 = ReadString("LastName3", string.Empty);
                            if (firstName3 != string.Empty || lastName3 != string.Empty)
                            {
                                contact3 = contacts.FindByName(firstName3, lastName3);
                                if (contact3 == null)
                                {
                                    _importLog.Write(messagePrefix + "Skipped. No contact matching the first and last name 3 (" + firstName3 + " " + lastName3 + ") was found.");
                                    continue;
                                }
                                else
                                {
                                    contact3ID = contact3.UserID;
                                }
                            }
                        }
                    }
                    else
                    {
                        string contactEmail3 = ReadString("ContactEmail3", string.Empty);
                        if (contactEmail3 != string.Empty)
                        {
                            contact3 = contacts.FindByEmailAndOrganization(contactEmail3, company3ID);
                            if (contact3 == null)
                            {
                                _importLog.Write(messagePrefix + "Skipped. No contact matching the ContactEmail3 " + contactEmail3 + " was found.");
                                continue;
                            }
                            else
                            {
                                contact3ID = contact3.UserID;
                            }
                        }

                        if (contact3ID == 0)
                        {
                            string firstName3 = ReadString("FirstName3", string.Empty);
                            string lastName3 = ReadString("LastName3", string.Empty);
                            if (firstName3 != string.Empty || lastName3 != string.Empty)
                            {
                                contact3 = contacts.FindByNameAndOrganization(firstName3, lastName3, company3ID);
                                if (contact3 == null)
                                {
                                    _importLog.Write(messagePrefix + "Skipped. No contact matching the first and last name3 (" + firstName3 + " " + lastName3 + ") was found.");
                                    continue;
                                }
                                else
                                {
                                    contact3ID = contact3.UserID;
                                }
                            }
                        }
                    }
                }

                if (contact3ID != 0)
                {
                    ticket.AddContact(contact3ID, ticket[0].TicketID, import.ImportID);
                    _importLog.Write(messagePrefix + "Contact " + contact3.FirstLastName + " was added to ticket number " + ticket[0].TicketNumber.ToString() + ".");
                }
                else
                {
                    _importLog.Write(messagePrefix + "No contact3 found to add to ticket number " + ticket[0].TicketNumber.ToString() + ".");
                }
            }
        }

        private void ImportAssetTickets(Import import)
        {
            SortedList<string, int> assetList = GetAssetList();

            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";

                Tickets ticket = new Tickets(_importUser);

                int ticketID = ReadInt("TicketID");
                if (ticketID != 0)
                {
                    ticket.LoadByTicketID(ticketID);
                    if (ticket.Count == 0 || ticket[0].OrganizationID != _organizationID)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No ticket matching TicketID: " + ticketID.ToString() + " was found processing ticket assets.");
                        continue;
                    }
                }

                if (ticket.Count == 0)
                {
                    string importID = ReadString("TicketImportID", string.Empty);
                    if (importID != string.Empty)
                    {
                        ticket = new Tickets(_importUser);
                        ticket.LoadByImportID(importID, _organizationID);
                        if (ticket.Count > 1)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one ticket matching the TicketImportID " + importID + " was found processing ticket assets.");
                            continue;
                        }
                    }
                }

                if (ticket.Count == 0)
                {
                    int? ticketNumber;
                    ticketNumber = ReadIntNull("TicketNumber", string.Empty);
                    if (ticketNumber != null)
                    {
                        ticket = new Tickets(_importUser);
                        ticket.LoadByTicketNumber(_organizationID, (int)ticketNumber);
                        if (ticket.Count > 1)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one ticket matching the TicketNumber " + ticketNumber.ToString() + " was found.");
                            continue;
                        }
                    }
                }

                if (ticket.Count == 0)
                {
                    _importLog.Write(messagePrefix + "Skipped. No ticket matching either the TicketImportID or the TicketNumber was found.");
                    continue;
                }

                int assetID = 0;
                string assetName = string.Empty;

                assetID = ReadInt("AssetID");
                if (assetID != 0)
                {
                    if (!assetList.ContainsValue(assetID))
                    {
                        _importLog.Write(messagePrefix + "Skipped. No asset matching AssetID " + assetID.ToString() + " was found.");
                        continue;
                    }
                    else
                    {
                        assetName = assetList.Keys[assetList.IndexOfValue(assetID)];
                    }
                }

                string assetImportID = ReadString("AssetImportID", string.Empty);
                if (assetID == 0 && !string.IsNullOrEmpty(assetImportID))
                {
                    Assets asset = new Assets(_importUser);
                    asset.LoadByImportID(assetImportID, _organizationID);
                    if (asset.Count == 1)
                    {
                        assetID = asset[0].AssetID;
                        assetName = asset[0].Name;
                    }
                }

                if (assetID == 0)
                {
                    assetName = ReadString("AssetName", string.Empty);
                    string assetSerialNumber = ReadString("AssetSerialNumber", string.Empty);
                    string location = ReadString("AssetLocation", string.Empty);
                    switch (location.Trim().ToLower())
                    {
                        case "assigned":
                            location = "1";
                            break;
                        case "warehouse":
                            location = "2";
                            break;
                        case "junkyard":
                            location = "3";
                            break;
                        default:
                            location = "2";
                            break;
                    }
                    string key = assetSerialNumber + assetName + location;
                    if (!string.IsNullOrEmpty(assetSerialNumber) || !string.IsNullOrEmpty(assetName))
                    {
                        if (!assetList.TryGetValue(key.ToUpper().Replace(" ", string.Empty), out assetID))
                        {
                            _importLog.Write(messagePrefix + "Asset '" + assetName + "' does not exists.");
                        }
                    }
                }

                if (assetID != 0)
                {
                    if (Tickets.GetAssetCount(_importUser, assetID, ticket[0].TicketID) > 0)
                    {
                        _importLog.Write(messagePrefix + "Asset '" + assetName + "' already in ticket #" + ticket[0].TicketNumber.ToString() + ".");
                    }
                    else
                    {
                        ticket.AddAsset(assetID, ticket[0].TicketID, import.ImportID);
                        _importLog.Write(messagePrefix + "Asset '" + assetName + "' was added to ticket #" + ticket[0].TicketNumber.ToString() + ".");
                    }
                }

                int assetID2 = 0;
                string assetName2 = string.Empty;

                assetID2 = ReadInt("AssetID2");
                if (assetID2 != 0)
                {
                    if (!assetList.ContainsValue(assetID2))
                    {
                        _importLog.Write(messagePrefix + "Skipped. No asset matching AssetID2 " + assetID2.ToString() + " was found.");
                        continue;
                    }
                    else
                    {
                        assetName = assetList.Keys[assetList.IndexOfValue(assetID2)];
                    }
                }

                string assetImportID2 = ReadString("AssetImportID2", string.Empty);
                if (assetID2 == 0 && !string.IsNullOrEmpty(assetImportID2))
                {
                    Assets asset = new Assets(_importUser);
                    asset.LoadByImportID(assetImportID, _organizationID);
                    if (asset.Count == 1)
                    {
                        assetID2 = asset[0].AssetID;
                        assetName2 = asset[0].Name;
                    }
                }

                if (assetID2 == 0)
                {
                    assetName2 = ReadString("AssetName2", string.Empty);
                    string assetSerialNumber2 = ReadString("AssetSerialNumber2", string.Empty);
                    string location2 = ReadString("AssetLocation2", string.Empty);
                    switch (location2.Trim().ToLower())
                    {
                        case "assigned":
                            location2 = "1";
                            break;
                        case "warehouse":
                            location2 = "2";
                            break;
                        case "junkyard":
                            location2 = "3";
                            break;
                        default:
                            location2 = "2";
                            break;
                    }
                    string key2 = assetSerialNumber2 + assetName2 + location2;
                    if (!string.IsNullOrEmpty(assetSerialNumber2) || !string.IsNullOrEmpty(assetName2))
                    {
                        if (!assetList.TryGetValue(key2.ToUpper().Replace(" ", string.Empty), out assetID2))
                        {
                            _importLog.Write(messagePrefix + "Asset '" + assetName2 + "' does not exists.");
                        }
                    }
                }

                if (assetID2 != 0)
                {
                    if (Tickets.GetAssetCount(_importUser, assetID2, ticket[0].TicketID) > 0)
                    {
                        _importLog.Write(messagePrefix + "Asset '" + assetName2 + "' already in ticket #" + ticket[0].TicketNumber.ToString() + ".");
                    }
                    else
                    {
                        ticket.AddAsset(assetID2, ticket[0].TicketID, import.ImportID);
                        _importLog.Write(messagePrefix + "Asset '" + assetName2 + "' was added to ticket #" + ticket[0].TicketNumber.ToString() + ".");
                    }
                }

                int assetID3 = 0;
                string assetName3 = string.Empty;

                assetID3 = ReadInt("AssetID3");
                if (assetID3 != 0)
                {
                    if (!assetList.ContainsValue(assetID3))
                    {
                        _importLog.Write(messagePrefix + "Skipped. No asset matching AssetID3 " + assetID3.ToString() + " was found.");
                        continue;
                    }
                    else
                    {
                        assetName = assetList.Keys[assetList.IndexOfValue(assetID3)];
                    }
                }

                string assetImportID3 = ReadString("AssetImportID3", string.Empty);
                if (assetID3 == 0 && !string.IsNullOrEmpty(assetImportID3))
                {
                    Assets asset = new Assets(_importUser);
                    asset.LoadByImportID(assetImportID3, _organizationID);
                    if (asset.Count == 1)
                    {
                        assetID3 = asset[0].AssetID;
                        assetName3 = asset[0].Name;
                    }
                }

                if (assetID3 == 0)
                {
                    assetName3 = ReadString("AssetName3", string.Empty);
                    string assetSerialNumber3 = ReadString("AssetSerialNumber3", string.Empty);
                    string location3 = ReadString("AssetLocation3", string.Empty);
                    switch (location3.Trim().ToLower())
                    {
                        case "assigned":
                            location3 = "1";
                            break;
                        case "warehouse":
                            location3 = "2";
                            break;
                        case "junkyard":
                            location3 = "3";
                            break;
                        default:
                            location3 = "2";
                            break;
                    }
                    string key3 = assetSerialNumber3 + assetName3 + location3;
                    if (!string.IsNullOrEmpty(assetSerialNumber3) || !string.IsNullOrEmpty(assetName3))
                    {
                        if (!assetList.TryGetValue(key3.ToUpper().Replace(" ", string.Empty), out assetID3))
                        {
                            _importLog.Write(messagePrefix + "Asset '" + assetName3 + "' does not exists.");
                        }
                    }
                }

                if (assetID3 != 0)
                {
                    if (Tickets.GetAssetCount(_importUser, assetID3, ticket[0].TicketID) > 0)
                    {
                        _importLog.Write(messagePrefix + "Asset '" + assetName3 + "' already in ticket #" + ticket[0].TicketNumber.ToString() + ".");
                    }
                    else
                    {
                        ticket.AddAsset(assetID3, ticket[0].TicketID, import.ImportID);
                        _importLog.Write(messagePrefix + "Asset '" + assetName3 + "' was added to ticket #" + ticket[0].TicketNumber.ToString() + ".");
                    }
                }
            }
        }

        private void ImportTicketRelationships(Import import)
        {
            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";

                Tickets ticket = new Tickets(_importUser);

                int ticketID = ReadInt("TicketID");
                if (ticketID != 0)
                {
                    ticket.LoadByTicketID(ticketID);
                    if (ticket.Count == 0 || ticket[0].OrganizationID != _organizationID)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No ticket matching TicketID: " + ticketID.ToString() + " was found processing ticket relationships.");
                        continue;
                    }
                }

                if (ticket.Count == 0)
                {
                    string importID = ReadString("TicketImportID", string.Empty);
                    if (importID != string.Empty)
                    {
                        ticket = new Tickets(_importUser);
                        ticket.LoadByImportID(importID, _organizationID);
                        if (ticket.Count > 1)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one ticket matching the TicketImportID " + importID + " was found.");
                            continue;
                        }
                    }
                }

                if (ticket.Count == 0)
                {
                    int? ticketNumber;
                    ticketNumber = ReadIntNull("TicketNumber", string.Empty);
                    if (ticketNumber != null)
                    {
                        ticket = new Tickets(_importUser);
                        ticket.LoadByTicketNumber(_organizationID, (int)ticketNumber);
                        if (ticket.Count > 1)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one ticket matching the TicketNumber " + ticketNumber.ToString() + " was found.");
                            continue;
                        }
                    }
                }

                if (ticket.Count == 0)
                {
                    _importLog.Write(messagePrefix + "Skipped. No ticket matching either the TicketID, TicketImportID or the TicketNumber was found.");
                    continue;
                }

                Tickets associatedTicket = new Tickets(_importUser);

                int associatedTicketID = ReadInt("AssociatedTicketID");
                if (associatedTicketID != 0)
                {
                    associatedTicket.LoadByTicketID(ticketID);
                    if (associatedTicket.Count == 0 || associatedTicket[0].OrganizationID != _organizationID)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No ticket matching AssociatedTicketID: " + associatedTicketID.ToString() + " was found processing ticket relationships.");
                        continue;
                    }
                }

                if (associatedTicket.Count == 0)
                {
                    string associatedImportID = ReadString("AssociatedTicketImportID", string.Empty);
                    if (associatedImportID != string.Empty)
                    {
                        associatedTicket = new Tickets(_importUser);
                        associatedTicket.LoadByImportID(associatedImportID, _organizationID);
                        if (associatedTicket.Count > 1)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one ticket matching the AssociatedTicketImportID " + associatedImportID + " was found processing ticket relationships.");
                            continue;
                        }
                    }
                }

                if (associatedTicket.Count == 0)
                {
                    int? associatedTicketNumber;
                    associatedTicketNumber = ReadIntNull("AssociatedTicketNumber", string.Empty);
                    if (associatedTicketNumber != null)
                    {
                        associatedTicket = new Tickets(_importUser);
                        associatedTicket.LoadByTicketNumber(_organizationID, (int)associatedTicketNumber);
                        if (associatedTicket.Count > 1)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one ticket matching the AssociatedTicketNumber " + associatedTicketNumber.ToString() + " was found.");
                            continue;
                        }
                    }
                }

                if (associatedTicket.Count == 0)
                {
                    _importLog.Write(messagePrefix + "Skipped. No ticket matching either the AssociatedTicketImportID or the AssociatedTicketNumber was found.");
                    continue;
                }

                bool related = ReadBool("Related", string.Empty);
                bool parent = ReadBool("Parent", string.Empty);
                bool child = ReadBool("Child", string.Empty);
                if (!related && !parent && !child)
                {
                    related = true;
                }

                if (related) // just related
                {

                    if (IsTicketRelated(ticket[0], associatedTicket[0]))
                    {
                        _importLog.Write(messagePrefix + "Skipped. Ticket #" + ticket[0].TicketNumber.ToString() + " is already related to ticket #" + associatedTicket[0].TicketNumber.ToString() + ".");
                        continue;
                    }

                    TicketRelationship item = (new TicketRelationships(_importUser)).AddNewTicketRelationship();
                    item.OrganizationID = _organizationID;
                    item.Ticket1ID = ticket[0].TicketID;
                    item.Ticket2ID = associatedTicket[0].TicketID;
                    item.ImportFileID = import.ImportID;
                    item.Collection.Save();
                }
                else if (parent) // parent
                {
                    if (associatedTicket[0].ParentID != null)
                    {
                        if (ticket[0].ParentID == associatedTicket[0].TicketID)
                        {
                            _importLog.Write(messagePrefix + "Skipped. Ticket #" + ticket[0].TicketNumber.ToString() + " is the child of ticket #" + associatedTicket[0].TicketNumber.ToString() + " whos has a parent.");
                            continue;
                        }
                        else
                        {
                            _importLog.Write(messagePrefix + "Skipped. Ticket #" + associatedTicket[0].TicketNumber.ToString() + " is already the child of a different ticket.");
                            continue;
                        }
                    }

                    if (ticket[0].ParentID == associatedTicket[0].TicketID)
                    {
                        _importLog.Write(messagePrefix + "Skipped. Ticket #" + ticket[0].TicketNumber.ToString() + " is the child of ticket #" + associatedTicket[0].TicketNumber.ToString() + " whos doesn't has a parent.");
                        continue;
                    }

                    TicketRelationship item = TicketRelationships.GetTicketRelationship(_importUser, ticket[0].TicketID, associatedTicket[0].TicketID);
                    if (item != null)
                    {
                        item.Delete();
                        item.Collection.Save();
                    }

                    associatedTicket[0].ParentID = ticket[0].TicketID;
                    associatedTicket.Save();
                    _importLog.Write(messagePrefix + "Ticket #" + ticket[0].TicketNumber.ToString() + " has been set as parent of ticket #" + associatedTicket[0].TicketNumber.ToString() + ".");
                }
                else // child
                {
                    if (ticket[0].ParentID != null && ticket[0].ParentID == associatedTicket[0].TicketID)
                    {
                        _importLog.Write(messagePrefix + "Skipped. Ticket #" + ticket[0].TicketNumber.ToString() + " is already the child of ticket #" + associatedTicket[0].TicketNumber.ToString() + ".");
                        continue;
                    }
                    if (associatedTicket[0].ParentID == ticket[0].TicketID)
                    {
                        _importLog.Write(messagePrefix + "Skipped. Ticket #" + ticket[0].TicketNumber.ToString() + " is the parent of ticket #" + associatedTicket[0].TicketNumber.ToString() + ".");
                        continue;
                    }
                    TicketRelationship item = TicketRelationships.GetTicketRelationship(_importUser, ticket[0].TicketID, associatedTicket[0].TicketID);
                    if (item != null)
                    {
                        item.Delete();
                        item.Collection.Save();
                    }

                    ticket[0].ParentID = associatedTicket[0].TicketID;
                    ticket.Save();
                    _importLog.Write(messagePrefix + "Ticket #" + ticket[0].TicketNumber.ToString() + " has been set as the child of ticket #" + associatedTicket[0].TicketNumber.ToString() + ".");
                }

            }
        }

        private void ImportProducts(Import import)
        {
            SortedList<string, int> userList = GetUserAndContactList();

            Products products = new Products(_importUser);
            ProductFamilies productFamilies = new ProductFamilies(_importUser);

            int count = 0;
            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";

                string name = ReadString("Name", string.Empty);
                if (name == string.Empty)
                {
                    _importLog.Write(messagePrefix + "Skipped. Product Name is required.");
                    continue;
                }

                //Products existingProduct = new Products(_importUser);
                Product product = null;
                //bool isUpdate = false;

                string importID = ReadString("ProductImportID", string.Empty);
                if (importID != string.Empty)
                {
                    //existingProduct.LoadByImportID(importID, _organizationID);
                    //if (existingProduct.Count == 1)
                    //{
                    //  product = existingProduct[0];
                    //  isUpdate = true;
                    //}
                    //else if (existingProduct.Count > 1)
                    //{
                    //  _importLog.Write(messagePrefix + "Skipped. More than one product matching the importID " + importID + " was found.");
                    //  continue;
                    //}
                }
                else
                {
                    _importLog.Write(messagePrefix + "Skipped. ProductImportID is required.");
                    continue;
                }

                //if (product == null)
                //{
                //  existingProduct = new Products(_importUser);
                //  existingProduct.LoadByProductName(_organizationID, name);
                //  if (existingProduct.Count > 0)
                //  {
                //    product = existingProduct[0];
                //    isUpdate = true;
                //  }
                //}

                //if (product == null)
                //{
                product = products.AddNewProduct();
                //}

                product.Name = name;
                product.Description = ReadString("Description", product.Description);
                product.ImportID = importID;
                DateTime? dateCreated = ReadDateNull("DateCreated", product.DateCreated.ToString());
                if (dateCreated != null)
                {
                    product.DateCreated = (DateTime)dateCreated;
                }

                int creatorID = -5;
                if (Int32.TryParse(ReadString("CreatorID", creatorID.ToString()), out creatorID))
                {
                    if (!userList.ContainsValue(creatorID))
                    {
                        creatorID = -5;
                    }
                }
                product.CreatorID = -5;
                product.ModifierID = -5;
                product.OrganizationID = _organizationID;
                product.ImportFileID = import.ImportID;

                int productFamilyID = 0;
                string productFamilyImportID = ReadString("ProductLineImportID", string.Empty);
                if (productFamilyImportID != string.Empty)
                {
                    if (productFamilies.Count == 0)
                    {
                        productFamilies.LoadByOrganizationID(_organizationID);
                    }

                    ProductFamily productFamily = productFamilies.FindByImportID(productFamilyImportID);
                    if (productFamily != null)
                    {
                        productFamilyID = productFamily.ProductFamilyID;
                    }
                    else
                    {
                        _importLog.Write("No product line found with importID " + productFamilyImportID + ".");
                    }
                }
                else
                {
                    string productFamilyName = ReadString("ProductLineName", string.Empty);
                    if (productFamilyName != string.Empty)
                    {
                        if (productFamilies.Count == 0)
                        {
                            productFamilies.LoadByOrganizationID(_organizationID);
                        }

                        ProductFamily productFamily = productFamilies.FindByName(productFamilyName);
                        if (productFamily != null)
                        {
                            productFamilyID = productFamily.ProductFamilyID;
                        }
                        else
                        {
                            _importLog.Write("No product line found with name " + productFamilyName + ".");
                        }
                    }
                }

                if (productFamilyID != 0)
                {
                    product.ProductFamilyID = productFamilyID;
                }

                //if (isUpdate)
                //{
                //  existingProduct.Save();
                //  _importLog.Write(messagePrefix + "ProductID " + product.ProductID.ToString() + " was updated.");
                //}
                //else
                //{
                _importLog.Write(messagePrefix + "Product " + importID + " added to bulk insert.");
                //}
                count++;

                if (count % BULK_LIMIT == 0)
                {
                    products.BulkSave();
                    products = new Products(_importUser);
                    UpdateImportCount(import, count);
                    _importLog.Write("Import set with " + count.ToString() + " products inserted in database.");
                }
            }
            products.BulkSave();
            UpdateImportCount(import, count);
            _importLog.Write("Import set with " + count.ToString() + " products inserted in database.");
            //Per ticket 45431 Indexer does not process recently imported products so we use index rebuild.
            Organizations.SetRebuildIndexes(_importUser, _importUser.OrganizationID);
        }

        private void ImportProductVersions(Import import)
        {
            SortedList<string, int> userList = GetUserAndContactList();

            ProductVersionStatuses productVersionStatuses = new ProductVersionStatuses(_importUser);
            productVersionStatuses.LoadByOrganizationID(_organizationID);

            ProductVersions productVersions = new ProductVersions(_importUser);

            int count = 0;
            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";

                string name = ReadString("VersionNumber", string.Empty);
                if (name == string.Empty)
                {
                    _importLog.Write(messagePrefix + "Skipped. VersionNumber is required.");
                    continue;
                }

                int productID = 0;
                Product product = null;
                string productImportID = ReadString("ProductImportID", string.Empty);
                if (!string.IsNullOrEmpty(productImportID))
                {
                    Products products = new Products(_importUser);
                    products.LoadByImportID(productImportID, _organizationID);
                    if (products.Count == 1)
                    {
                        product = products[0];
                        productID = product.ProductID;
                    }
                    else if (products.Count > 1)
                    {
                        _importLog.Write(messagePrefix + "Skipped. More than one product found matching TicketImportID " + productImportID);
                        continue;
                    }
                }

                if (productID == 0)
                {
                    productID = ReadInt("ProductID");
                    if (productID != 0)
                    {
                        Products products = new Products(_importUser);
                        products.LoadByProductID(productID);
                        if (products.Count == 1 && products[0].OrganizationID == _organizationID)
                        {
                            product = products[0];
                        }
                        else
                        {
                            _importLog.Write(messagePrefix + "Skipped. No product found with productID " + productID.ToString() + " provided.");
                            continue;
                        }
                    }
                }

                if (product == null)
                {
                    string productName = ReadString("ProductName", string.Empty);
                    if (!string.IsNullOrEmpty(productName))
                    {
                        Products products = new Products(_importUser);
                        products.LoadByProductName(_organizationID, productName);
                        if (products.Count > 1)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one product matching the ProductName was found.");
                            continue;
                        }
                        else if (products.Count == 1)
                        {
                            product = products[0];
                            productID = product.ProductID;
                        }
                        else
                        {
                            _importLog.Write(messagePrefix + "Skipped. No product matching the ProductName was found.");
                            continue;
                        }
                    }
                    else
                    {
                        _importLog.Write(messagePrefix + "Skipped. Product is required.");
                        continue;
                    }
                }

                //ProductVersions existingProductVersion = new ProductVersions(_importUser);
                ProductVersion productVersion = null;
                //bool isUpdate = false;

                string importID = ReadString("ProductVersionImportID", string.Empty);
                if (importID != string.Empty)
                {
                    //existingProductVersion.LoadByImportID(importID, _organizationID);
                    //if (existingProductVersion.Count == 1)
                    //{
                    //  productVersion = existingProductVersion[0];
                    //  isUpdate = true;
                    //}
                    //else if (existingProductVersion.Count > 1)
                    //{
                    //  _importLog.Write(messagePrefix + "Skipped. More than one product version matching the importID " + importID + " was found.");
                    //  continue;
                    //}
                }
                else
                {
                    _importLog.Write(messagePrefix + "Skipped. ProductVersionImportID is required.");
                    continue;
                }


                //if (productVersion == null)
                //{
                //  existingProductVersion = new ProductVersions(_importUser);
                //  existingProductVersion.LoadByProductIDAndVersionNumber(productID, name);
                //  if (existingProductVersion.Count > 0)
                //  {
                //    productVersion = existingProductVersion[0];
                //    isUpdate = true;
                //  }
                //}

                //if (productVersion == null)
                //{
                productVersion = productVersions.AddNewProductVersion();
                //}

                productVersion.ProductID = productID;
                productVersion.VersionNumber = name;
                productVersion.Description = ReadString("Description", productVersion.Description);
                productVersion.ImportID = importID;
                productVersion.IsReleased = ReadBool("Released", productVersion.IsReleased.ToString());

                DateTime? releaseDate = ReadDateNull("ReleaseDate", productVersion.ReleaseDate.ToString());
                if (releaseDate != null)
                {
                    productVersion.ReleaseDate = (DateTime)releaseDate;
                }

                DateTime? dateCreated = ReadDateNull("DateCreated", productVersion.DateCreated.ToString());
                if (dateCreated != null)
                {
                    productVersion.DateCreated = (DateTime)dateCreated;
                }

                int creatorID = -5;
                if (Int32.TryParse(ReadString("CreatorID", creatorID.ToString()), out creatorID))
                {
                    if (!userList.ContainsValue(creatorID))
                    {
                        creatorID = -5;
                    }
                }
                productVersion.CreatorID = -5;
                productVersion.ModifierID = -5;
                productVersion.ImportFileID = import.ImportID;

                int productVersionStatusID = productVersionStatuses[0].ProductVersionStatusID;
                string status = ReadString("Status", string.Empty);
                if (!string.IsNullOrEmpty(status))
                {
                    ProductVersionStatus productVersionStatus = productVersionStatuses.FindByName(status);
                    if (productVersionStatus == null)
                    {
                        ProductVersionStatuses newProductVersionStatuses = new ProductVersionStatuses(_importUser);
                        productVersionStatus = newProductVersionStatuses.AddNewProductVersionStatus();
                        productVersionStatus.Name = status;
                        productVersionStatus.Description = status;
                        productVersionStatus.OrganizationID = _organizationID;
                        productVersionStatus.Position = newProductVersionStatuses.GetMaxPosition(_organizationID) + 1;
                        productVersionStatus.CreatorID = -5;
                        productVersionStatus.ModifierID = -5;
                        newProductVersionStatuses.Save();
                        newProductVersionStatuses.ValidatePositions(_organizationID);
                    }
                    productVersionStatusID = productVersionStatus.ProductVersionStatusID;
                }
                productVersion.ProductVersionStatusID = productVersionStatusID;

                //if (isUpdate)
                //{
                //  existingProductVersion.Save();
                //  _importLog.Write(messagePrefix + "ProductVersionID " + productVersion.ProductVersionID.ToString() + " was updated.");
                //}
                //else
                //{
                _importLog.Write(messagePrefix + "Product Version " + importID + " added to bulk insert.");
                //}
                count++;

                if (count % BULK_LIMIT == 0)
                {
                    productVersions.BulkSave();
                    productVersions = new ProductVersions(_importUser);
                    UpdateImportCount(import, count);
                    _importLog.Write("Import set with " + count.ToString() + " product versions inserted in database.");
                }
            }
            productVersions.BulkSave();
            UpdateImportCount(import, count);
            _importLog.Write("Import set with " + count.ToString() + " product versions inserted in database.");
        }

        private void ImportUsers(Import import)
        {
            SortedList<string, int> userList = GetUserList();

            Users users = new Users(_importUser);

            int count = 0;
            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";

                string firstName = ReadString("FirstName", string.Empty);
                if (string.IsNullOrEmpty(firstName))
                {
                    _importLog.Write(messagePrefix + "Skipped. User skipped due to missing first name");
                    continue;
                }

                string lastName = ReadString("LastName", string.Empty);
                if (string.IsNullOrEmpty(lastName))
                {
                    _importLog.Write(messagePrefix + "Skipped. User skipped due to missing last name");
                    continue;
                }

                //Users existingUser = new Users(_importUser);
                User user = null;
                //bool isUpdate = false;
                //int oldOrganizationID = 0;

                string importID = ReadString("UserImportID", string.Empty);
                if (importID != string.Empty)
                {
                    //existingUser.LoadByImportID(importID, _organizationID);
                    //if (existingUser.Count == 1)
                    //{
                    //  user = existingUser[0];
                    //  oldOrganizationID = user.OrganizationID;
                    //  isUpdate = true;
                    //}
                    //else if (existingUser.Count > 1)
                    //{
                    //  _importLog.Write(messagePrefix + "Skipped. More than one user matching importID was found");
                    //  continue;
                    //}
                }
                else
                {
                    _importLog.Write(messagePrefix + "Skipped. UserImportID is required.");
                    continue;
                }

                string email = ReadString("UserEmail", string.Empty);
                if (email != string.Empty)
                {
                    //  existingUser = new Users(_importUser);
                    //  existingUser.LoadByEmail(_organizationID, email);
                    //  if (existingUser.Count > 0)
                    //  {
                    //    user = existingUser[0];
                    //    oldOrganizationID = user.OrganizationID;
                    //    isUpdate = true;
                    //  }
                }
                else
                {
                    _importLog.Write(messagePrefix + "Skipped. UserEmail is required.");
                    continue;
                }


                //if (user == null)
                //{
                user = users.AddNewUser();
                //}

                DateTime? dateCreated = ReadDateNull("DateCreated", user.DateCreated.ToString());
                if (dateCreated != null)
                {
                    user.DateCreated = (DateTime)dateCreated;
                }
                int creatorID = -5;
                if (Int32.TryParse(ReadString("CreatorID", creatorID.ToString()), out creatorID))
                {
                    if (!userList.ContainsValue(creatorID))
                    {
                        creatorID = -5;
                    }
                }

                user.OrganizationID = _organizationID;
                user.ImportID = importID;
                user.FirstName = firstName;
                user.MiddleName = ReadString("MiddleName", user.MiddleName);
                user.LastName = lastName;
                user.Title = ReadString("Title", user.Title);
                user.Email = email;

                string isActive = ReadString("IsActive", string.Empty);
                if (!string.IsNullOrEmpty(isActive))
                {
                    user.IsActive = ReadBool("IsActive", user.IsActive.ToString());
                }
                else
                {
                    user.IsActive = true;
                }

                //string isFinanceAdmin = ReadString("IsFinanceAdmin", string.Empty);
                //if (!string.IsNullOrEmpty(isFinanceAdmin))
                //{
                //	user.IsFinanceAdmin = ReadBool("IsFinanceAdmin", user.IsFinanceAdmin.ToString());
                //}
                //else
                //{
                //	user.IsFinanceAdmin = false;
                //}

                string isSystemAdmin = ReadString("IsSystemAdmin", string.Empty);
                if (!string.IsNullOrEmpty(isSystemAdmin))
                {
                    user.IsSystemAdmin = ReadBool("IsSystemAdmin", user.IsSystemAdmin.ToString());
                }
                else
                {
                    user.IsSystemAdmin = false;
                }

                string isChatUser = ReadString("IsChatUser", string.Empty);
                if (!string.IsNullOrEmpty(isChatUser))
                {
                    user.IsChatUser = ReadBool("IsChatUser", user.IsChatUser.ToString());
                }
                else
                {
                    user.IsChatUser = false;
                }

                user.ActivatedOn = DateTime.UtcNow;
                DateTime? activatedOn = ReadDateNull("ActivatedOn", user.ActivatedOn.ToString());
                if (activatedOn != null)
                {
                    user.ActivatedOn = (DateTime)activatedOn;
                }

                user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(ReadString("Password", DataUtils.GenerateRandomPassword()).Trim(), "MD5");
                user.DeactivatedOn = null;
                user.InOffice = false;
                user.InOfficeComment = "";
                user.IsPasswordExpired = true;
                user.IsPortalUser = true;
                user.LastActivity = DateTime.UtcNow;
                user.LastLogin = DateTime.UtcNow;
                user.NeedsIndexing = true;
                user.PrimaryGroupID = null;
                user.SubscribeToNewTickets = false;
                user.ReceiveTicketNotifications = true;
                user.EnforceSingleSession = true;
                user.CreatorID = -5;
                user.ModifierID = -5;
                user.ImportFileID = import.ImportID;
                user.IsClassicView = true;

                _importLog.Write(messagePrefix + "User " + importID + " added to bulk insert.");
                count++;

                if (count % BULK_LIMIT == 0)
                {
                    users.BulkSave();
                    users = new Users(_importUser);
                    UpdateImportCount(import, count);
                }
            }
            users.BulkSave();
            UpdateImportCount(import, count);
            _importLog.Write(count.ToString() + " users imported.");
        }

        private void ImportOrganizationProducts(Import import)
        {
            Organizations companies = new Organizations(_importUser);
            companies.LoadByParentID(_organizationID, false);

            Products products = new Products(_importUser);
            products.LoadByOrganizationID(_organizationID);

            ProductVersions productVersions = new ProductVersions(_importUser);
            productVersions.LoadByParentOrganizationID(_organizationID);

            SortedList<string, int> userList = GetUserAndContactList();

            OrganizationProducts organizationProducts = new OrganizationProducts(_importUser);
            int count = 0;
            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";

                int companyID = ReadInt("CompanyID");
                if (companyID != 0)
                {
                    Organization company = companies.FindByOrganizationID(companyID);
                    if (company == null)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyID " + companyID.ToString() + " was found.");
                        continue;
                    }
                }

                if (companyID == 0)
                {
                    string companyImportID = ReadString("CompanyImportID", string.Empty);
                    if (companyImportID != string.Empty)
                    {
                        Organization company = companies.FindOnlyByImportID(companyImportID);
                        if (company == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyImportID " + companyImportID + " was found.");
                            continue;
                        }
                        else
                        {
                            companyID = company.OrganizationID;
                        }
                    }
                }

                if (companyID == 0)
                {
                    string companyName = ReadString("CompanyName", string.Empty);
                    if (companyName != string.Empty)
                    {
                        Organization company = companies.FindByName(companyName);
                        if (company == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyName " + companyName + " was found.");
                            continue;
                        }
                        else
                        {
                            companyID = company.OrganizationID;
                        }
                    }
                }

                if (companyID == 0)
                {
                    _importLog.Write(messagePrefix + "Skipped. No company matching either the CompanyID, the CompanyImportID or the CompanyName was found.");
                    continue;
                }

                int productID = ReadInt("ProductID");
                if (productID != 0)
                {
                    Product product = products.FindByProductID(productID);
                    if (product == null)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No product matching ProductID: " + productID.ToString() + " was found.");
                        continue;
                    }
                }

                if (productID == 0)
                {
                    string productImportID = ReadString("ProductImportID", string.Empty);
                    if (productImportID != string.Empty)
                    {
                        Product product = products.FindByImportID(productImportID);
                        if (product == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No product matching the ProductImportID " + productImportID + " was found.");
                            continue;
                        }
                        else
                        {
                            productID = product.ProductID;
                        }
                    }
                }

                if (productID == 0)
                {
                    string productName = ReadString("ProductName", string.Empty);
                    if (productName != string.Empty)
                    {
                        Product product = products.FindByName(productName);
                        if (product == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No product matching ProductName " + productName + " was found.");
                            continue;
                        }
                        else
                        {
                            productID = product.ProductID;
                        }
                    }
                }

                if (productID == 0)
                {
                    _importLog.Write(messagePrefix + "Skipped. No product matching either the ProductID, ProductImportID or the ProductName was found.");
                    continue;
                }

                int productVersionID = ReadInt("ProductVersionID");
                if (productVersionID != 0)
                {
                    ProductVersion productVersion = productVersions.FindByProductVersionID(productVersionID);
                    if (productVersion == null)
                    {
                        _importLog.Write(messagePrefix + ". No product version matching ProductVersionID: " + productVersionID.ToString() + " was found.");
                    }
                }

                if (productVersionID == 0)
                {
                    string productVersionImportID = ReadString("ProductVersionImportID", string.Empty);
                    if (productVersionImportID != string.Empty)
                    {
                        ProductVersion productVersion = productVersions.FindByImportID(productVersionImportID);
                        if (productVersion == null)
                        {
                            _importLog.Write(messagePrefix + ". No product version matching the ProductVersionImportID " + productVersionImportID + " was found.");
                        }
                        else
                        {
                            productVersionID = productVersion.ProductVersionID;
                        }
                    }
                }

                if (productVersionID == 0)
                {
                    string productVersionNumber = ReadString("ProductVersionNumber", string.Empty);
                    if (productVersionNumber != string.Empty)
                    {
                        ProductVersion productVersion = productVersions.FindByVersionNumber(productVersionNumber, productID);
                        if (productVersion == null)
                        {
                            _importLog.Write(messagePrefix + ". No product version matching ProductVersionNumber " + productVersionNumber + " was found.");
                        }
                        else
                        {
                            productVersionID = productVersion.ProductVersionID;
                        }
                    }
                }

                OrganizationProduct organizationProduct = organizationProducts.AddNewOrganizationProduct();
                organizationProduct.OrganizationID = companyID;
                organizationProduct.ProductID = productID;
                if (productVersionID != 0)
                {
                    organizationProduct.ProductVersionID = productVersionID;
                }

                DateTime? supportExpiration = ReadDateNull("SupportExpiration", organizationProduct.SupportExpiration.ToString());
                if (supportExpiration != null)
                {
                    organizationProduct.SupportExpiration = (DateTime)supportExpiration;
                }

                DateTime? dateCreated = ReadDateNull("DateCreated", organizationProduct.DateCreated.ToString());
                if (dateCreated != null)
                {
                    organizationProduct.DateCreated = (DateTime)dateCreated;
                }
                int creatorID = -5;
                if (Int32.TryParse(ReadString("CreatorID", creatorID.ToString()), out creatorID))
                {
                    if (!userList.ContainsValue(creatorID))
                    {
                        creatorID = -5;
                    }
                }
                organizationProduct.CreatorID = -5;
                organizationProduct.ModifierID = -5;
                organizationProduct.ImportFileID = import.ImportID;

                _importLog.Write(messagePrefix + "Organization " + companyID.ToString() + " - Product " + productID.ToString() + " added to bulk insert.");
                count++;

                if (count % BULK_LIMIT == 0)
                {
                    organizationProducts.BulkSave();
                    organizationProducts = new OrganizationProducts(_importUser);
                    UpdateImportCount(import, count);
                }
            }
            organizationProducts.BulkSave();
            UpdateImportCount(import, count);
            _importLog.Write(count.ToString() + " organization products imported.");

        }

        private void ImportPrimaryContacts(Import import)
        {
            Organizations companies = new Organizations(_importUser);
            companies.LoadByParentID(_organizationID, false);

            Users contacts = new Users(_loginUser);
            contacts.LoadContacts(_organizationID, false);

            int count = 0;
            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";

                string companyImportID = ReadString("CompanyImportID", string.Empty);
                if (companyImportID != string.Empty)
                {
                    Organization company = companies.FindOnlyByImportID(companyImportID);
                    if (company == null)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyImportID " + companyImportID + " was found.");
                        continue;
                    }
                    else
                    {
                        string contactImportID = ReadString("ContactImportID", string.Empty);
                        if (contactImportID != string.Empty)
                        {
                            User contact = contacts.FindByImportID(contactImportID);
                            if (contact == null)
                            {
                                _importLog.Write(messagePrefix + "Skipped. No contact matching the ContactImportID " + contactImportID + " was found.");
                                continue;
                            }
                            else
                            {
                                company.PrimaryUserID = contact.UserID;
                                _importLog.Write(messagePrefix + "Company with importID: " + companyImportID + " Primary Contact set as contact with importID: " + contactImportID + ".");
                                count++;
                            }
                        }
                        else
                        {
                            _importLog.Write(messagePrefix + "Skipped. ContactImportID is required.");
                            continue;
                        }
                    }
                }
                else
                {
                    _importLog.Write(messagePrefix + "Skipped. CompanyImportID is required.");
                    continue;
                }
            }
            companies.Save();
            UpdateImportCount(import, count);
            _importLog.Write(count.ToString() + " Primary Contacts imported.");

        }

        private void ImportCustomerNotes(Import import)
        {
            Organizations companies = new Organizations(_loginUser);
            companies.LoadByParentID(_organizationID, false);

            SortedList<string, int> userList = GetUserAndContactList();
            SortedList<string, int> usersAndContactsFirstAndLastNameList = new SortedList<string, int>();

            Notes notes = new Notes(_loginUser);
            int count = 0;
            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";

                int companyID = ReadInt("CompanyID");
                if (companyID != 0)
                {
                    Organization company = companies.FindByOrganizationID(companyID);
                    if (company == null)
                    {
                        _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyID " + companyID.ToString() + " was found.");
                        continue;
                    }
                }

                if (companyID == 0)
                {
                    string companyImportID = ReadString("CompanyImportID", string.Empty);
                    if (companyImportID != string.Empty)
                    {
                        Organization company = companies.FindOnlyByImportID(companyImportID);
                        if (company == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyImportID " + companyImportID + " was found.");
                            continue;
                        }
                        else
                        {
                            companyID = company.OrganizationID;
                        }
                    }
                }

                if (companyID == 0)
                {
                    string companyName = ReadString("CompanyName", string.Empty);
                    if (companyName != string.Empty)
                    {
                        Organization company = companies.FindByName(companyName);
                        if (company == null)
                        {
                            _importLog.Write(messagePrefix + "Skipped. No company matching the CompanyName " + companyName + " was found.");
                            continue;
                        }
                        else
                        {
                            companyID = company.OrganizationID;
                        }
                    }
                }

                if (companyID == 0)
                {
                    _importLog.Write(messagePrefix + "Skipped. No company matching either the CompanyID, the CompanyImportID or the CompanyName was found.");
                    continue;
                }

                Note note = notes.AddNewNote();
                DateTime? dateCreated = ReadDateNull("DateCreated", note.DateCreated.ToString());
                if (dateCreated != null)
                {
                    note.DateCreated = (DateTime)dateCreated;
                }
                int creatorID = -5;
                if (Int32.TryParse(ReadString("CreatorID", creatorID.ToString()), out creatorID))
                {
                    if (!userList.ContainsValue(creatorID))
                    {
                        creatorID = -5;
                    }
                }

                string emailOfAuthor = ReadString("EmailOfAuthor", string.Empty);
                if (!string.IsNullOrEmpty(emailOfAuthor))
                {
                    if (userList.TryGetValue(emailOfAuthor.ToUpper(), out creatorID))
                    {
                    }
                    else
                    {
                        _importLog.Write("No EmailOfAuthor " + emailOfAuthor + " found.");
                    }
                }

                string creatorName = ReadString("CreatorName", string.Empty);
                {
                    if (creatorName != string.Empty)
                    {
                        if (usersAndContactsFirstAndLastNameList.Count == 0)
                        {
                            usersAndContactsFirstAndLastNameList = GetUserAndContactNameAndLastNameList();
                        }

                        if (usersAndContactsFirstAndLastNameList.TryGetValue(creatorName.ToUpper(), out creatorID))
                        {
                        }
                        else
                        {
                            _importLog.Write("No CreatorName " + creatorName + " found.");
                        }
                    }
                }
                note.CreatorID = creatorID;
                note.Description = ConvertHtmlLineBreaks(ReadString("Description", string.Empty));
                note.RefID = companyID;
                note.RefType = ReferenceType.Organizations;
                note.Title = ReadString("Title", string.Empty).Trim();
                note.ModifierID = -5;
                note.ImportFileID = import.ImportID;

                _importLog.Write(messagePrefix + "Organization " + companyID.ToString() + " - Note added to bulk insert.");
                count++;

                if (count % BULK_LIMIT == 0)
                {
                    notes.BulkSave();
                    notes = new Notes(_importUser);
                    UpdateImportCount(import, count);
                }
            }
            notes.BulkSave();
            UpdateImportCount(import, count);
            _importLog.Write(count.ToString() + " organization notes imported.");
        }

        private void ImportContactNotes(Import import)
        {
            SortedList<string, int> userList = GetUserAndContactList();
            SortedList<string, int> usersAndContactsFirstAndLastNameList = new SortedList<string, int>();

            SortedList<string, int> contactList = null;
            contactList = GetContactList();

            Notes notes = new Notes(_loginUser);
            int count = 0;
            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";

                int contactID = ReadInt("ContactID");
                if (contactID != 0)
                {
                    if (!contactList.ContainsValue(contactID))
                    {
                        _importLog.Write(messagePrefix + "Skipped. No contact matching ContactID " + contactID.ToString() + " was found for notes.");
                        continue;
                    }
                }

                if (contactID == 0)
                {
                    string contactImportID = ReadString("ContactImportID", string.Empty);
                    if (contactImportID != string.Empty)
                    {
                        Users existingContact = new Users(_importUser);
                        existingContact.LoadByImportID(_organizationID, contactImportID);
                        if (existingContact.Count == 1)
                        {
                            contactID = existingContact[0].UserID;
                        }
                        else if (existingContact.Count > 1)
                        {
                            _importLog.Write(messagePrefix + "Skipped. More than one contact matching the ContactImportID " + contactImportID + " was found.");
                            continue;
                        }
                        else
                        {
                            _importLog.Write(messagePrefix + "Skipped. No contact matching the ContactImportID " + contactImportID + " was found.");
                            continue;
                        }
                    }
                }

                if (contactID == 0)
                {
                    _importLog.Write(messagePrefix + "Skipped. No contact matching either the ContactID or the ContactImportID was found.");
                    continue;
                }

                Note note = notes.AddNewNote();
                DateTime? dateCreated = ReadDateNull("DateCreated", note.DateCreated.ToString());
                if (dateCreated != null)
                {
                    note.DateCreated = (DateTime)dateCreated;
                }

                int creatorID = -5;
                if (Int32.TryParse(ReadString("CreatorID", creatorID.ToString()), out creatorID))
                {
                    if (!userList.ContainsValue(creatorID))
                    {
                        creatorID = -5;
                    }
                }

                string emailOfAuthor = ReadString("EmailOfAuthor", string.Empty);
                if (!string.IsNullOrEmpty(emailOfAuthor))
                {
                    if (userList.TryGetValue(emailOfAuthor.ToUpper(), out creatorID))
                    {
                    }
                    else
                    {
                        _importLog.Write("No EmailOfAuthor " + emailOfAuthor + " found.");
                    }
                }

                string creatorName = ReadString("CreatorName", string.Empty);
                {
                    if (creatorName != string.Empty)
                    {
                        if (usersAndContactsFirstAndLastNameList.Count == 0)
                        {
                            usersAndContactsFirstAndLastNameList = GetUserAndContactNameAndLastNameList();
                        }

                        if (usersAndContactsFirstAndLastNameList.TryGetValue(creatorName.ToUpper(), out creatorID))
                        {
                        }
                        else
                        {
                            _importLog.Write("No CreatorName " + creatorName + " found.");
                        }
                    }
                }

                note.CreatorID = creatorID;
                note.Description = ConvertHtmlLineBreaks(ReadString("Description", string.Empty));
                note.RefID = contactID;
                note.RefType = ReferenceType.Users;
                note.Title = ReadString("Title", string.Empty).Trim();
                if (note.Title == string.Empty)
                {
                    _importLog.Write(messagePrefix + "Skipped. Title is required.");
                    continue;
                }
                note.ModifierID = -5;
                note.ImportFileID = import.ImportID;

                _importLog.Write(messagePrefix + "Contact " + contactID.ToString() + " - Note added to bulk insert.");
                count++;

                if (count % BULK_LIMIT == 0)
                {
                    notes.BulkSave();
                    notes = new Notes(_importUser);
                    UpdateImportCount(import, count);
                }
            }
            notes.BulkSave();
            UpdateImportCount(import, count);
            _importLog.Write(count.ToString() + " contact notes imported.");
        }

        private void ImportProductFamilies(Import import)
        {
            SortedList<string, int> userList = GetUserList();
            SortedList<string, int> usersFirstAndLastNameList = new SortedList<string, int>();

            ProductFamilies productFamilies = new ProductFamilies(_importUser);
            int count = 0;

            while (_csv.ReadNextRecord())
            {
                long rowNumber = _csv.CurrentRecordIndex + 1;
                string messagePrefix = "Row " + rowNumber.ToString() + ": ";

                string productFamilyImportID = ReadString("ProductLineImportID", string.Empty);
                if (productFamilyImportID == string.Empty)
                {
                    _importLog.Write(messagePrefix + "Skipped. Product Line ImportID is required.");
                    continue;
                }

                string name = ReadString("Name", string.Empty);
                if (name == string.Empty)
                {
                    _importLog.Write(messagePrefix + "Skipped. Product Line Name is required.");
                    continue;
                }

                ProductFamily productFamily = productFamilies.AddNewProductFamily();
                productFamily.OrganizationID = _organizationID;
                productFamily.Name = name;
                productFamily.Description = ConvertHtmlLineBreaks(ReadString("Description", string.Empty));

                DateTime? dateCreated = ReadDateNull("DateCreated", productFamily.DateCreated.ToString());
                if (dateCreated != null)
                {
                    productFamily.DateCreated = (DateTime)dateCreated;
                }
                productFamily.DateModified = DateTime.UtcNow;

                int creatorID = -5;
                if (Int32.TryParse(ReadString("CreatorID", productFamily.CreatorID.ToString()), out creatorID))
                {
                    if (!userList.ContainsValue(creatorID))
                    {
                        creatorID = -5;
                    }
                }

                if (creatorID == -5)
                {
                    string creatorEmail = ReadString("CreatorEmail", string.Empty);
                    if (creatorEmail != string.Empty)
                    {
                        if (userList.TryGetValue(creatorEmail.ToUpper(), out creatorID))
                        {
                        }
                        else
                        {
                            _importLog.Write("No CreatorEmail " + creatorEmail + " found.");
                        }
                    }
                    else
                    {
                        string creatorName = ReadString("CreatorName", string.Empty);
                        {
                            if (creatorName != string.Empty)
                            {
                                if (usersFirstAndLastNameList.Count == 0)
                                {
                                    usersFirstAndLastNameList = GetUserNameAndLastNameList();
                                }

                                if (usersFirstAndLastNameList.TryGetValue(creatorName.ToUpper(), out creatorID))
                                {
                                }
                                else
                                {
                                    _importLog.Write("No CreatorName " + creatorName + " found.");
                                }
                            }
                        }
                    }
                }

                productFamily.CreatorID = creatorID;
                productFamily.ModifierID = -5;
                productFamily.NeedsIndexing = 1;
                productFamily.ImportID = productFamilyImportID;
                productFamily.ImportFileID = import.ImportID;

                _importLog.Write(messagePrefix + "Product Line added to bulk insert.");
                count++;

                if (count % BULK_LIMIT == 0)
                {
                    productFamilies.BulkSave();
                    productFamilies = new ProductFamilies(_importUser);
                    UpdateImportCount(import, count);
                }
            }
            productFamilies.BulkSave();
            UpdateImportCount(import, count);
            _importLog.Write(count.ToString() + " product lines imported.");
        }

        private bool IsTicketRelated(Ticket ticket1, Ticket ticket2)
        {
            if (ticket1.ParentID != null && ticket1.ParentID == ticket2.TicketID) return true;
            if (ticket2.ParentID != null && ticket2.ParentID == ticket1.TicketID) return true;
            TicketRelationship item = TicketRelationships.GetTicketRelationship(ticket1.Collection.LoginUser, ticket1.TicketID, ticket2.TicketID);
            return item != null;
        }

        //private void ImportCustomFieldPickList(Import import)
        //{
        //  int count = 0;
        //  var fields = new Dictionary<string, List<string>>();

        //  while (_csv.ReadNextRecord())
        //  {
        //    count++;
        //    string apiFieldName = ReadString("ApiFieldName");
        //    string listValue = ReadString("PickListValue");
        //    List<string> list;
        //    if (!fields.TryGetValue(apiFieldName.ToUpper(), out list))
        //    {
        //      list = new List<string>();
        //      fields.Add(apiFieldName, list);
        //    }

        //    list.Add(listValue);
        //  }

        //  foreach (KeyValuePair<string, List<string>> item in fields)
        //  {
        //    CustomField customField = CustomFields.GetCustomFieldByApi(_importUser, _organizationID, item.Key);
        //    if (customField != null)
        //    {
        //      customField.ListValues = string.Join("|", item.Value.ToArray());
        //      customField.Collection.Save();
        //    }
        //  }

        //  UpdateImportCount(import, count);
        //}

        private void ClearEmails()
        {
            try
            {
                EmailPosts.DeleteImportEmailsWithRetryOnDeadLock(_importUser);

            }
            catch (Exception)
            {

            }
        }

        private string GetMappedName(string field)
        {
            ImportFieldsViewItem map = _map.FindByFieldName(field);
            if (map != null && map.SourceName.Trim() != string.Empty)
            {
                return map.SourceName.Trim();
            }
            else
            {
                return field;
            }
        }

        private string GetMappedValue(string field, string existingValue)
        {
            string mappedField = GetMappedName(field);
            string result = existingValue;
            if (mappedField != "Skipped")
            {
                result = _headers.Contains(mappedField) ? _csv[mappedField] : existingValue;
            }
            return result;
        }

        private DateTime ReadDate(string field, DateTime defaultValue)
        {
            string value = GetMappedValue(field, defaultValue.ToString());
            DateTime result = defaultValue;
            DateTime.TryParse(value, out result);
            return TimeZoneInfo.ConvertTimeToUtc(result);
        }

        private DateTime? ReadDateNull(string field, string existingValue)
        {
            string value = GetMappedValue(field, existingValue);
            DateTime result;
            if (!DateTime.TryParse(value, out result))
            {
                return null;
            }

            // We use to do just the following:
            // return TimeZoneInfo.ConvertTimeToUtc(result);
            // But due to ticket #46668 exception ArgumentException	TimeZoneInfo.Local.IsInvalidDateTime(dateTime) returns true.
            // See https://blogs.msdn.microsoft.com/bclteam/2007/06/11/system-timezoneinfo-working-with-ambiguous-and-invalid-points-in-time-josh-free/
            // We are implementing the following logic
            DateTime? utcResult = null;
            try
            {
                utcResult = TimeZoneInfo.ConvertTimeToUtc(result);
            }
            catch (ArgumentException argEx)
            {
                if (result.Month == 3)
                {
                    result = result.AddHours(1);
                }
                else
                {
                    result = result.AddHours(-1);
                }
                try
                {
                    utcResult = TimeZoneInfo.ConvertTimeToUtc(result);
                }
                catch (Exception ex2)
                {
                    _importLog.Write("Reading DateNull field " + field + " with value " + value + " the following exception was thrown: " + ex2.Message);
                }
            }
            catch (Exception ex)
            {
                _importLog.Write("Reading DateNull field " + field + " with value " + value + " the following exception was thrown: " + ex.Message);
            }

            return utcResult;
        }

        private bool ReadBool(string field, string existingValue)
        {
            string value = GetMappedValue(field, existingValue);
            if (value != null)
            {
                value = value.ToLower();
                return value.IndexOf('t') > -1 || value.IndexOf('y') > -1 || value.IndexOf('1') > -1;
            }
            else
            {
                return false;
            }
        }

        private int ReadInt(string field, int defaultValue = 0)
        {
            string value = GetMappedValue(field, defaultValue.ToString());
            int result = defaultValue;
            int.TryParse(value, out result);
            return result;
        }

        private int? ReadIntNull(string field, string existingValue)
        {
            string value = GetMappedValue(field, existingValue);
            int result;
            if (!int.TryParse(value, out result))
            {
                return null;
            }
            return result;
        }

        private string ReadString(string field, string existingValue)
        {
            string value = GetMappedValue(field, existingValue);
            if (value != null)
            {
                return value.Trim();
            }
            else
            {
                return value;
            }
        }

        private SystemActionType GetSystemActionTypeID(string name)
        {
            SystemActionType result = SystemActionType.Custom;
            switch (name.ToLower().Trim())
            {
                case "description": result = SystemActionType.Description; break;
                case "resolution": result = SystemActionType.Resolution; break;
                case "updaterequest": result = SystemActionType.UpdateRequest; break;
                case "email": result = SystemActionType.Email; break;
                case "reminder": result = SystemActionType.Reminder; break;
                default:
                    break;
            }
            return result;
        }

        private int? GetActionTypeID(ActionTypes actionTypes, string name)
        {
            name = name.Trim();
            if (GetSystemActionTypeID(name) != SystemActionType.Custom) return null;
            if (string.IsNullOrWhiteSpace(name)) name = "Comment";
            ActionType actionType = actionTypes.FindByName(name);

            if (actionType == null)
            {
                ActionTypes ats = new ActionTypes(_importUser);
                actionType = ats.AddNewActionType();
                actionType.Name = name;
                actionType.Description = "";
                actionType.IsTimed = true;
                actionType.OrganizationID = _organizationID;
                actionType.Position = actionTypes.GetMaxPosition(_organizationID) + 1;
                ats.Save();
                int? result = actionType.ActionTypeID;
                actionTypes.LoadAllPositions(_organizationID);
                return result;
            }
            else
            {
                return actionType.ActionTypeID;
            }
        }

        private string ConvertHtmlLineBreaks(string text, string lineBreak = "<br />")
        {
            return Regex.Replace(text, @"\r\n?|\n", lineBreak);
        }

        private SortedList<string, int> GetTicketList()
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"SELECT TicketNumber, TicketID
                              FROM Tickets
                              WHERE (OrganizationID = @OrganizationID)";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", _organizationID);
            return GetList(command);
        }

        private SortedList<string, int> GetUserAndContactImportIdList()
        {
            SortedList<string, int> list = GetUserImportIdList();
            return GetContactImportIdList(list);
        }

        private SortedList<string, int> GetUserAndContactNameAndLastNameList()
        {
            SortedList<string, int> list = GetUserNameAndLastNameList();
            return GetContactFirstAndLastNameList(list);
        }

        private SortedList<string, int> GetUserAndContactList()
        {
            SortedList<string, int> list = GetUserList();
            return GetContactList(list);
        }

        private SortedList<string, int> GetUserAndContactWithoutCompanyNameList()
        {
            SortedList<string, int> list = GetUserList();
            return GetContactListWithoutCompany(list);
        }

        private SortedList<string, int> GetUserList()
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
SELECT DISTINCT REPLACE(u.Email, ' ', ''), MAX(u.UserID) AS UserID
FROM Users u 
WHERE (u.OrganizationID = @OrganizationID)
AND (u.MarkDeleted = 0)
GROUP BY REPLACE(u.Email, ' ', '')
";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", _organizationID);
            return GetList(command);
        }

        private SortedList<string, int> GetUserImportIdList()
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
        SELECT
	        DISTINCT u.ImportID,
            MAX(u.UserID) AS UserID
        FROM
            Users u 
        WHERE
            u.OrganizationID = @OrganizationID
            AND u.MarkDeleted = 0
        GROUP BY
	        u.ImportID";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", _organizationID);
            return GetList(command);
        }

        private SortedList<string, int> GetUserNameAndLastNameList()
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
        SELECT
	        DISTINCT u.FirstName + ' ' + u.LastName,
            MAX(u.UserID) AS UserID
        FROM
            Users u 
        WHERE
            u.OrganizationID = @OrganizationID
            AND u.MarkDeleted = 0
        GROUP BY
	        u.FirstName + ' ' + u.LastName";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", _organizationID);
            return GetList(command);
        }

        private SortedList<string, int> GetAssetList(SortedList<string, int> list = null)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
SELECT DISTINCT(REPLACE(a.SerialNumber + a.Name + a.Location, ' ', '')), MAX(a.AssetID)
FROM Assets a 
WHERE a.OrganizationID = @OrganizationID
GROUP BY REPLACE(a.SerialNumber + a.Name + a.Location, ' ', '')";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", _organizationID);
            return list == null ? GetList(command) : GetList(command, list);
        }

        private SortedList<string, int> GetContactImportIdList(SortedList<string, int> list = null)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
            SELECT
	            DISTINCT u.ImportID,
                MAX(u.UserID) AS UserID
            FROM
                Users u 
                LEFT JOIN Organizations o
                    ON o.OrganizationID = u.OrganizationID
            WHERE
                o.ParentID = @OrganizationID
                AND u.MarkDeleted = 0
            GROUP BY
                u.ImportID";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", _organizationID);
            return list == null ? GetList(command) : GetList(command, list);
        }

        private SortedList<string, int> GetContactFirstAndLastNameList(SortedList<string, int> list = null)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
            SELECT
	            DISTINCT u.FirstName + ' ' + u.LastName,
                MAX(u.UserID) AS UserID
            FROM
                Users u 
                LEFT JOIN Organizations o
                    ON o.OrganizationID = u.OrganizationID
            WHERE
                o.ParentID = @OrganizationID
                AND u.MarkDeleted = 0
            GROUP BY
                u.FirstName + ' ' + u.LastName";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", _organizationID);
            return list == null ? GetList(command) : GetList(command, list);
        }

        private SortedList<string, int> GetContactList(SortedList<string, int> list = null)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
SELECT DISTINCT(REPLACE(u.Email + '(' + o.Name + ')', ' ', '')), MAX(u.UserID)
FROM Users u 
LEFT JOIN Organizations o
ON o.OrganizationID = u.OrganizationID
WHERE (o.ParentID = @OrganizationID)
AND (u.MarkDeleted = 0)
GROUP BY REPLACE(u.Email + '(' + o.Name + ')', ' ', '')";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", _organizationID);
            return list == null ? GetList(command) : GetList(command, list);
        }

        private SortedList<string, int> GetContactListWithoutCompany(SortedList<string, int> list = null)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
SELECT DISTINCT(REPLACE(u.Email, ' ', '')), MAX(u.UserID)
FROM Users u 
LEFT JOIN Organizations o
ON o.OrganizationID = u.OrganizationID
WHERE (o.ParentID = @OrganizationID)
AND (u.MarkDeleted = 0)
GROUP BY REPLACE(u.Email, ' ', '')";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", _organizationID);
            return list == null ? GetList(command) : GetList(command, list);
        }

        private SortedList<string, int> GetCompanyList(SortedList<string, int> list = null)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
SELECT DISTINCT(REPLACE(o.Name, ' ', '')), MAX(o.OrganizationID)
FROM Organizations o
WHERE (o.ParentID = @OrganizationID)
GROUP BY o.Name";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", _organizationID);
            return list == null ? GetList(command) : GetList(command, list);
        }

        private SortedList<string, int> GetList(SqlCommand command)
        {
            return GetList(command, new SortedList<string, int>());
        }


        private SortedList<string, int> GetList(SqlCommand command, SortedList<string, int> list)
        {

            using (SqlConnection connection = new SqlConnection(_importUser.ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                command.Connection = connection;
                command.Transaction = transaction;
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        try
                        {
                            list.Add((reader[0].ToString()).Trim().ToUpper(), reader[1] as int? ?? -1);
                        }
                        catch (Exception e)
                        {
                            
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }

                transaction.Commit();
            }
            return list;

        }

        public KnowledgeBaseCategory AddKnowledgeBaseCategory(int? parentID, string name)
        {
            KnowledgeBaseCategory cat = (new KnowledgeBaseCategories(_loginUser)).AddNewKnowledgeBaseCategory();
            cat.OrganizationID = _organizationID;
            cat.CategoryName = name;
            cat.ParentID = parentID ?? -1;
            cat.Position = GetKnowledgeBaseCategoryMaxPosition(parentID) + 1;
            cat.VisibleOnPortal = true;
            cat.Collection.Save();
            return cat;
        }

        private int GetKnowledgeBaseCategoryMaxPosition(int? parentID)
        {
            parentID = parentID ?? -1;

            KnowledgeBaseCategories cats = new KnowledgeBaseCategories(_loginUser);
            if (parentID < 0) cats.LoadCategories(_organizationID);
            else cats.LoadSubcategories((int)parentID);

            int max = -1;

            foreach (KnowledgeBaseCategory cat in cats)
            {
                if (cat.Position != null && cat.Position > max) max = (int)cat.Position;
            }

            return max;
        }

        private void KeepHealthUpdated()
        {
            DateTime alertTimeLowerLimit = DateTime.Now;
            while (true)
            {
                if (DateTime.Now.Subtract(alertTimeLowerLimit).TotalMinutes > 9)
                {
                    UpdateHealth();
                    Logs.WriteEvent("Health update after " + DateTime.Now.Subtract(alertTimeLowerLimit).TotalMinutes.ToString() + " processing minutes.");
                    alertTimeLowerLimit = DateTime.Now;
                }
            }
        }
    }

    public class ImportLog
    {
        private string _logPath;
        private string _fileName;

        public ImportLog(string path, int importID)
        {
            _logPath = path;
            _fileName = importID.ToString() + ".txt";

            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
        }

        public void Write(string text)
        {
            if (!File.Exists(_logPath + @"\" + _fileName))
            {
                foreach (string oldFileName in Directory.GetFiles(_logPath))
                {
                    if (File.GetLastWriteTime(oldFileName).AddDays(30) < DateTime.Today)
                    {
                        File.Delete(oldFileName);
                    }
                }
            }

            File.AppendAllText(_logPath + @"\" + _fileName, DateTime.Now.ToLongTimeString() + ": " + text + Environment.NewLine);
        }
    }
}
