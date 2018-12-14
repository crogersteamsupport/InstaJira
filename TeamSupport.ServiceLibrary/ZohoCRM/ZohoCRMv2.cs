using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary.ZohoCRM
{
	public class ZohoCRMv2
	{
		private const string _zohoCrmAPIv2 = "https://www.zohoapis.com/crm/v2/";
		private const int _maxBatchSize = 200;
		private const string RATELIMIT = "X-RATELIMIT-LIMIT";
		private const string RATELIMITREMAINING = "X-RATELIMIT-REMAINING";
		private const string RATELIMITRESET = "X-RATELIMIT-RESET";

		protected CRMLinkTableItem _crmLinkRow;
		protected CRMLinkTableItem CrmLinkRow
		{
			get { return _crmLinkRow; }
		}

		protected LoginUser _loginUser;
		protected LoginUser LoginUser
		{
			get { return _loginUser; }
		}

		protected Logs _logs;
		protected Logs Logs
		{
			get { return _logs; }
		}

		public ZohoCRMv2()
		{
		}

		public ZohoCRMv2(CRMLinkTableItem crmLinkTableItem)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
			_loginUser = crmLinkTableItem.Collection.LoginUser;
			_crmLinkRow = crmLinkTableItem;
			_logs = new Logs(IntegrationType.ZohoCRM, CrmLinkRow.OrganizationID);
		}

		public bool PerformSync()
		{
			bool isSuccess = false;

			if (IsValid() && SyncAccounts())
			{
				SendTicketData();
				isSuccess = true;
			}

			return isSuccess;
		}

		private bool IsValid()
		{
			bool isValid = !string.IsNullOrEmpty(CrmLinkRow.SecurityToken1);

			return isValid = true;
		}

		private bool SyncAccounts()
		{
			bool isSuccess = false;

			try
			{
				//We need the FullData for the regular fields sync and also later down for the custom mappings sync.
				List<ZohoAccountData.Account> accountFullData = GetZohoAccountData();

				if (accountFullData.Any())
				{
					CRMLinkErrors crmLinkErrors = new CRMLinkErrors(LoginUser);
					crmLinkErrors.LoadByOperation(CrmLinkRow.OrganizationID, CrmLinkRow.CRMType, Enums.GetDescription(IntegrationOrientation.IntoTeamSupport), "company");
					CRMLinkError crmLinkError = null;
					int totalCount = accountFullData.Sum(p => p.info.count);
					totalCount = accountFullData.Sum(p => p.data.Count());
					Logs.WriteEvent($"Processing {totalCount} accounts.");

					//Update info for organizations
					List<ZohoAccountData.Data> zohoAccountData = accountFullData.SelectMany(p => p.data).ToList();

					foreach(ZohoAccountData.Data account in zohoAccountData)
					{
						crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(account.id, string.Empty);

						try
						{
							UpdateCompanyInfo(account, CrmLinkRow.OrganizationID);

							if (crmLinkError != null)
							{
								crmLinkError.Delete();
								crmLinkErrors.Save();
							}
						}
						catch (Exception ex)
						{
							isSuccess = false;

							if (crmLinkError == null)
							{
								CRMLinkErrors newCrmLinkError = new CRMLinkErrors(LoginUser);
								crmLinkError = newCrmLinkError.AddNewCRMLinkError();
								crmLinkError.OrganizationID = CrmLinkRow.OrganizationID;
								crmLinkError.CRMType = CrmLinkRow.CRMType;
								crmLinkError.Orientation = Enums.GetDescription(IntegrationOrientation.IntoTeamSupport);
								crmLinkError.ObjectType = Enums.GetDescription(ObjectType.company);
								crmLinkError.ObjectID = account.id;
								crmLinkError.ObjectData = JsonConvert.SerializeObject(account);
								crmLinkError.Exception = ex.ToString() + ex.StackTrace;
								crmLinkError.OperationType = "unknown";
								newCrmLinkError.Save();
							}
							else
							{
								crmLinkError.ObjectData = JsonConvert.SerializeObject(account);
								crmLinkError.Exception = ex.ToString() + ex.StackTrace;
							}
						}
					}

					crmLinkErrors.Save();
					Logs.WriteEvent("Finished updating account information.");

					UpdateCompanyCustomMappings(accountFullData);
					Logs.WriteEvent("Finished updating Accounts Custom Mappings");

					SyncAccountProducts(zohoAccountData);
					SyncContacts(zohoAccountData);
					isSuccess = true;
				}
				else
				{
					Logs.WriteEvent("No recently updated accounts to sync.");
					isSuccess = true;
				}
			}
			catch (Exception ex)
			{
				isSuccess = false;
				Logs.WriteException(ex);
			}

			return isSuccess;
		}

		private void SyncAccountProducts(List<ZohoAccountData.Data> accountDataList)
		{
			if (CrmLinkRow.PullCustomerProducts ?? false)
			{
				Logs.WriteEvent("Updating products information...");
				CRMLinkErrors crmLinkErrors = new CRMLinkErrors(LoginUser);
				crmLinkErrors.LoadByOperation(CrmLinkRow.OrganizationID, CrmLinkRow.CRMType, Enums.GetDescription(IntegrationOrientation.IntoTeamSupport), "product");
				CRMLinkError crmLinkError = null;
				List<string> accounts = accountDataList.Select(p => p.id).ToList();

				foreach(string accountId in accounts)
				{
					List<ZohoAccountProducts.Product> productsDataList = GetZohoProductData(accountId);

					if (productsDataList.Any())
					{
						List<ZohoAccountProducts.Data> products = productsDataList.SelectMany(p => p.data).ToList();

						foreach(ZohoAccountProducts.Data product in products)
						{
							try
							{
								crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(accountId + product.Product_Name + product.Support_Expiry_Date.ToString(), string.Empty);
								
								//update info for products
								UpdateProductInfo(product, accountId, CrmLinkRow.OrganizationID);

								if (crmLinkError != null)
								{
									crmLinkError.Delete();
									crmLinkErrors.Save();
								}
							}
							catch (Exception ex)
							{
								if (crmLinkError == null)
								{
									CRMLinkErrors newCrmLinkError = new CRMLinkErrors(LoginUser);
									crmLinkError = newCrmLinkError.AddNewCRMLinkError();
									crmLinkError.OrganizationID = CrmLinkRow.OrganizationID;
									crmLinkError.CRMType = CrmLinkRow.CRMType;
									crmLinkError.Orientation = Enums.GetDescription(IntegrationOrientation.IntoTeamSupport);
									crmLinkError.ObjectType = Enums.GetDescription(ObjectType.product);
									crmLinkError.ObjectID = accountId + product.Product_Name + product.Support_Expiry_Date.ToString();
									crmLinkError.ObjectData = JsonConvert.SerializeObject(product);
									crmLinkError.Exception = ex.ToString() + ex.StackTrace;
									crmLinkError.OperationType = "unknown";
									newCrmLinkError.Save();
								}
								else
								{
									crmLinkError.ObjectData = JsonConvert.SerializeObject(product);
									crmLinkError.Exception = ex.ToString() + ex.StackTrace;
								}
							}
						}

						Logs.WriteEvent($"Updated product information for {accountDataList.Where(p => p.id == accountId).Select(p => p.Account_Name).FirstOrDefault()}");
					}
				}

				crmLinkErrors.Save();
				Logs.WriteEvent("Finished updating product information");
			}
		}

		private void SyncContacts(List<ZohoAccountData.Data> accountDataList)
		{
			List<string> accounts = accountDataList.Select(p => p.id).ToList();
			foreach (string accountId in accounts)
			{
				string accountName = accountDataList.Where(p => p.id == accountId).Select(p => p.Account_Name).FirstOrDefault();
				List<ZohoContactData.Contact> contactDataList = GetZohoContactData(accountId);
				Logs.WriteEvent("Updating people information...");

				if (contactDataList.Any())
				{
					Logs.WriteEvent($"PeopleToSync Count for account {accountName}: {contactDataList.Sum(p => p.info.count)}");
					CRMLinkErrors crmLinkErrors = new CRMLinkErrors(LoginUser);
					crmLinkErrors.LoadByOperation(CrmLinkRow.OrganizationID, CrmLinkRow.CRMType, Enums.GetDescription(IntegrationOrientation.IntoTeamSupport), Enums.GetDescription(ObjectType.contact));
					CRMLinkError crmLinkError = null;
					List<ZohoContactData.Data> contacts = contactDataList.SelectMany(p => p.data).ToList();

					//update info for customers
					foreach (ZohoContactData.Data contact in contacts)
					{
						crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(contact.id, string.Empty);

						try
						{
							UpdateContactInfo(contact, accountId, CrmLinkRow.OrganizationID);

							if (crmLinkError != null)
							{
								crmLinkError.Delete();
								crmLinkErrors.Save();
							}
						}
						catch (Exception ex)
						{
							if (crmLinkError == null)
							{
								CRMLinkErrors newCrmLinkError = new CRMLinkErrors(LoginUser);
								crmLinkError = newCrmLinkError.AddNewCRMLinkError();
								crmLinkError.OrganizationID = CrmLinkRow.OrganizationID;
								crmLinkError.CRMType = CrmLinkRow.CRMType;
								crmLinkError.Orientation = Enums.GetDescription(IntegrationOrientation.IntoTeamSupport);
								crmLinkError.ObjectType = Enums.GetDescription(ObjectType.contact);
								crmLinkError.ObjectID = contact.id;
								crmLinkError.ObjectData = JsonConvert.SerializeObject(contact);
								crmLinkError.Exception = ex.ToString() + ex.StackTrace;
								crmLinkError.OperationType = "unknown";
								newCrmLinkError.Save();
							}
							else
							{
								crmLinkError.ObjectData = JsonConvert.SerializeObject(contact);
								crmLinkError.Exception = ex.ToString() + ex.StackTrace;
							}
						}
					}

					crmLinkErrors.Save();
					Logs.WriteEvent("Finished updating people information");
					UpdateContactCustomMappings(contactDataList);
					Logs.WriteEvent("Finished updating Contacts Custom Mappings");
				}
				else
				{
					Logs.WriteEvent($"No recently updated contacts to sync for ZohoCrm accountId {accountName}.");
				}
			}
		}

		private List<ZohoAccountData.Account> GetZohoAccountData()
		{
			List<ZohoAccountData.Account> accountList = new List<ZohoAccountData.Account>();
			bool getMore = false;
			int page = 1;
			int pageSize = _maxBatchSize;

			do
			{
				getMore = false;
				string accountDataJson = GetAccountData(page, pageSize);
				ZohoAccountData.Account zohoAccountData = JsonConvert.DeserializeObject<ZohoAccountData.Account>(accountDataJson);
				Logs.WriteEvent("companyDataJson deserialized to ZohoAccountData.Account.");

				if (!string.IsNullOrEmpty(accountDataJson) && zohoAccountData != null)
				{
					//Store the original Json string to use it later for the custom field mapping sync
					zohoAccountData.info.JsonString = accountDataJson;

					accountList.Add(zohoAccountData);

					if (zohoAccountData.info.more_records)
					{
						getMore = true;
						page++;
					}
				}
			} while (getMore);
			

			Logs.WriteEvent("The GetZohoAccountData method has been executed.");

			return accountList;
		}

		private string GetAccountData(int page, int pageSize)
		{
			List<string> tagsToMatch = CrmLinkRow.TypeFieldMatch.ToLower().Split(',').Select(p => p.ToString().Trim()).ToList();
			string criteria = string.Empty;
			bool isFirst = true;

			foreach (string tag in tagsToMatch)
			{
				//If "none" then we are synching ALL types
				if (tag == "none")
				{
					criteria = string.Empty;
					break;
				}

				criteria += (isFirst ? "" : "OR") + $"(Account_Type:equals:{tag})";
				isFirst = false;
			}

			string apiUrl = $"Accounts?page={page}&per_page={pageSize}";
			
			if (!string.IsNullOrEmpty(criteria))
			{
				//If we need to match specific types then we can use a filter 'criteria' in the API call
				apiUrl = $"Accounts/search?page={page}&per_page={pageSize}&criteria=({criteria})";
			}

			Logs.WriteEvent($"querying {apiUrl}");
			Logs.WriteEvent($"LoginUser TimeZone: {CrmLinkRow.BaseCollection.LoginUser.TimeZoneInfo.ToString()}");

			Uri zohoUri = new Uri(_zohoCrmAPIv2 + apiUrl);
			string responseText = MakeHttpRequest(zohoUri);

			return responseText;
		}

		private List<ZohoAccountProducts.Product> GetZohoProductData(string accountId)
		{
			List<ZohoAccountProducts.Product> products = new List<ZohoAccountProducts.Product>();
			bool getMore = false;
			int page = 1;
			int pageSize = _maxBatchSize;

			do
			{
				getMore = false;
				string productDataJson = GetProductData(accountId, page, pageSize);
				ZohoAccountProducts.Product zohoProductData = JsonConvert.DeserializeObject<ZohoAccountProducts.Product>(productDataJson);
				Logs.WriteEvent("GetZohoProductData deserialized to ZohoAccountProducts.Product.");

				if (!string.IsNullOrEmpty(productDataJson) && zohoProductData != null)
				{
					products.Add(zohoProductData);

					if (zohoProductData.info.more_records)
					{
						getMore = true;
						page++;
					}
				}
			} while (getMore);

			return products;
		}

		private string GetProductData(string accountId, int page, int pageSize)
		{
			string apiUrl = $"Accounts/{accountId}/Products?page={page}&per_page={pageSize}";
			Logs.WriteEvent($"querying {apiUrl}");
			Logs.WriteEvent($"LoginUser TimeZone: {CrmLinkRow.BaseCollection.LoginUser.TimeZoneInfo.ToString()}");

			Uri zohoUri = new Uri(_zohoCrmAPIv2 + apiUrl);
			string responseText = MakeHttpRequest(zohoUri);

			Logs.WriteEvent("The GetProductData method has been executed.");

			return responseText;
		}

		private List<ZohoContactData.Contact> GetZohoContactData(string accountId)
		{
			List<ZohoContactData.Contact> accountList = new List<ZohoContactData.Contact>();
			bool getMore = false;
			int page = 1;
			int pageSize = _maxBatchSize;

			do
			{
				getMore = false;
				string contactDataJson = GetContactData(accountId, page, pageSize);
				ZohoContactData.Contact zohoContactData = JsonConvert.DeserializeObject<ZohoContactData.Contact>(contactDataJson);
				Logs.WriteEvent("contactDataJson deserialized to ZohoContactData.Contact.");

				if (!string.IsNullOrEmpty(contactDataJson) && zohoContactData != null)
				{
					//Store the original Json string to use it later for the custom field mapping sync
					zohoContactData.info.JsonString = contactDataJson;

					accountList.Add(zohoContactData);

					if (zohoContactData.info.more_records)
					{
						getMore = true;
						page++;
					}
				}
			} while (getMore);


			Logs.WriteEvent("The GetZohoContactData method has been executed.");

			return accountList;
		}

		private string GetContactData(string accountId, int page, int pageSize)
		{
			string apiUrl = $"Accounts/{accountId}/Contacts?page={page}&per_page={pageSize}";
			Logs.WriteEvent($"querying {apiUrl}");
			Logs.WriteEvent($"LoginUser TimeZone: {CrmLinkRow.BaseCollection.LoginUser.TimeZoneInfo.ToString()}");

			Uri zohoUri = new Uri(_zohoCrmAPIv2 + apiUrl);
			string responseText = MakeHttpRequest(zohoUri);

			return responseText;
		}

		protected void UpdateCompanyInfo(ZohoAccountData.Data company, int parentOrgId)
		{
			Organizations findCompany = new Organizations(LoginUser);
			Organization thisCompany;
			bool companyInfoNeedsUpdate = true;

			// search for the crmlinkid = accountid in db to see if it already exists
			findCompany.LoadByCRMLinkID(company.id, parentOrgId);
			string logMessage = string.Empty;

			if (findCompany.Count > 0)
			{
				thisCompany = findCompany[0];
				// it exists, so update the name on the account if it has changed.
				companyInfoNeedsUpdate = thisCompany.Name != company.Account_Name;
				thisCompany.Name = company.Account_Name;
				logMessage = "Found by accountId";
			}
			else
			{
				// look for parentid = parentorgid and name = accountname, and use that
				findCompany.LoadByParentID(parentOrgId, false);

				if (findCompany.FindByName(company.Account_Name) != null && CrmLinkRow.MatchAccountsByName)
				{
					thisCompany = findCompany.FindByName(company.Account_Name);
					companyInfoNeedsUpdate = thisCompany.CRMLinkID != company.id;
					// update accountid
					thisCompany.CRMLinkID = company.id;
					logMessage = "Found by Name";
				}
				else if (parentOrgId != 1078)
				{
					// if still not found, add new
					Organizations crmlinkOrg = new Organizations(LoginUser);
					crmlinkOrg.LoadByOrganizationID(parentOrgId);
					bool isAdvancedPortal = crmlinkOrg[0].IsAdvancedPortal;

					thisCompany = (new Organizations(LoginUser)).AddNewOrganization();
					thisCompany.ParentID = parentOrgId;
					thisCompany.Name = company.Account_Name;
					thisCompany.CRMLinkID = company.id;
					thisCompany.HasPortalAccess = isAdvancedPortal && CrmLinkRow.AllowPortalAccess;
					thisCompany.IsActive = true;
					thisCompany.SlaLevelID = CrmLinkRow.DefaultSlaLevelID;
					logMessage = "Added a new account";
				}
				else
				{
					Logs.WriteEvent($"Company {company.Account_Name} {company.id} was not added in our 1078.");
					return;
				}
			}

			if (companyInfoNeedsUpdate)
			{
				thisCompany.Collection.Save();
				Logs.WriteEvent($"{logMessage}: {company.Account_Name} OrgId: {thisCompany.OrganizationID} (AccountId: {company.id}).");
			}

			logMessage = string.Empty;
			Addresses findAddress = new Addresses(LoginUser);
			Address thisAddress;
			findAddress.LoadByID(thisCompany.OrganizationID, ReferenceType.Organizations);

			if (findAddress.Count > 0)
				thisAddress = findAddress[0];
			else
			{
				thisAddress = (new Addresses(LoginUser)).AddNewAddress();
				thisAddress.RefID = thisCompany.OrganizationID;
				thisAddress.RefType = ReferenceType.Organizations;
				thisAddress.Collection.Save();
				logMessage = "added";
			}

			bool addressNeedsUpdate = thisAddress.Addr1 != company.Billing_Street || thisAddress.City != company.Billing_City || thisAddress.State != company.Billing_State || thisAddress.Zip != company.Billing_Code || thisAddress.Country != company.Billing_Country;

			thisAddress.Addr1 = company.Billing_Street;
			thisAddress.City = company.Billing_City;
			thisAddress.State = company.Billing_State;
			thisAddress.Zip = company.Billing_Code;
			thisAddress.Country = company.Billing_Country;

			if (addressNeedsUpdate)
			{
				thisAddress.Collection.Save();
				Logs.WriteEvent(string.Format("Address information {0}.", string.IsNullOrEmpty(logMessage) ? "updated" : logMessage));
			}

			PhoneTypes phoneTypes = new PhoneTypes(LoginUser);
			phoneTypes.LoadAllPositions(parentOrgId);

			PhoneType CRMPhoneType = null;

			CRMPhoneType = phoneTypes.FindByName("Phone");
			if (CRMPhoneType == null)
			{
				CRMPhoneType = AddPhoneType("Phone", phoneTypes.Count, parentOrgId);
				phoneTypes.LoadAllPositions(parentOrgId);
			}

			PhoneNumber thisPhone = null;
			PhoneNumbers findPhone = new PhoneNumbers(LoginUser);
			findPhone.LoadByID(thisCompany.OrganizationID, ReferenceType.Organizations);

			if (findPhone.Count > 0)
			{
				foreach (PhoneNumber phone in findPhone)
				{
					if (phone.PhoneTypeID == null || (CRMPhoneType != null && phone.PhoneTypeID == CRMPhoneType.PhoneTypeID))
					{
						thisPhone = phone;
						break;
					}
				}
			}

			logMessage = string.Empty;

			if (company.Phone == null || company.Phone == string.Empty)
			{
				if (thisPhone != null)
				{
					thisPhone.Collection.DeleteFromDB(thisPhone.PhoneID);
					Logs.WriteEvent("Account phone number deleted.");
				}
			}
			else
			{
				logMessage = "updated";

				if (thisPhone == null)
				{
					thisPhone = (new PhoneNumbers(LoginUser)).AddNewPhoneNumber();
					logMessage = "added";
				}

				bool phoneNeedsUpdate = thisPhone.Number != company.Phone || thisPhone.RefType != ReferenceType.Organizations || thisPhone.RefID != thisCompany.OrganizationID;

				thisPhone.Number = company.Phone ?? "";
				thisPhone.RefType = ReferenceType.Organizations;
				thisPhone.RefID = thisCompany.OrganizationID;

				if (CRMPhoneType != null)
				{
					if (!phoneNeedsUpdate)
						phoneNeedsUpdate = thisPhone.PhoneTypeID == null || thisPhone.PhoneTypeID != CRMPhoneType.PhoneTypeID ? true : false;

					thisPhone.PhoneTypeID = CRMPhoneType.PhoneTypeID;
				}

				if (phoneNeedsUpdate)
				{
					thisPhone.Collection.Save();
					Logs.WriteEvent($"Account phone number {logMessage}.");
				}
				
			}

			logMessage = string.Empty;
			PhoneType faxType = phoneTypes.FindByName("Fax");

			if (faxType == null)
			{
				faxType = AddPhoneType("Fax", phoneTypes.Count, parentOrgId);
				phoneTypes.LoadAllPositions(parentOrgId);
			}

			PhoneNumber thisFax = findPhone.FindByPhoneTypeID(faxType.PhoneTypeID);

			if ((company.Fax == null || company.Fax == string.Empty) && thisFax != null)
			{
				thisFax.Collection.DeleteFromDB(thisFax.PhoneID);
				Logs.WriteEvent("Account Fax number deleted.");
			}
			else
			{
				logMessage = "updated";

				if (thisFax == null)
				{
					thisFax = (new PhoneNumbers(LoginUser)).AddNewPhoneNumber();
					logMessage = "added";
				}

				
				bool faxNeedsUpdate = thisFax.Number != (company.Fax ?? "") || thisFax.RefType != ReferenceType.Organizations || thisFax.RefID != thisCompany.OrganizationID || thisFax.PhoneTypeID == null || thisFax.PhoneTypeID == faxType.PhoneTypeID;

				thisFax.Number = company.Fax ?? "";
				thisFax.RefType = ReferenceType.Organizations;
				thisFax.RefID = thisCompany.OrganizationID;
				thisFax.PhoneTypeID = faxType.PhoneTypeID;

				if (faxNeedsUpdate)
				{
					thisFax.Collection.Save();
					Logs.WriteEvent($"Account fax number {logMessage}.");
				}
			}
		}

		private PhoneType AddPhoneType(string typeName, int position, int parentOrgId)
		{
			PhoneTypes phoneTypes = new PhoneTypes(LoginUser);
			PhoneType newPhoneType = phoneTypes.AddNewPhoneType();

			newPhoneType.Name = typeName;
			newPhoneType.Description = typeName;
			newPhoneType.Position = position;
			newPhoneType.OrganizationID = parentOrgId;

			phoneTypes.Save();

			Logs.WriteEvent($"PhoneType {typeName} added");

			return newPhoneType;
		}

		private void UpdateProductInfo(ZohoAccountProducts.Data product, string accountId, int organizationId)
		{
			if (string.IsNullOrEmpty(product.Product_Name))
			{
				Logs.WriteEvent($"The Product with id {product.id} does not have name, we don't add products with no name");
			}
			else
			{
				Logs.WriteEvent($"Adding product information for {product.Product_Name}.");

				Organizations organizations = new Organizations(LoginUser);
				//make sure the company already exists
				organizations.LoadByCRMLinkID(accountId, organizationId);

				if (organizations.Any())
				{
					Organization organization = organizations[0];
					Products products = new Products(LoginUser);
					products.LoadByProductName(organizationId, product.Product_Name);
					Product existingProduct = null;

					if (products.Any())
					{
						existingProduct = products[0];
					}
					else
					{
						products = new Products(LoginUser);
						products.AddNewProduct();
						products[0].Name = product.Product_Name;
						products[0].OrganizationID = organizationId;
						products.Save();
						existingProduct = products[0];
						Logs.WriteEvent($"Added product {product.Product_Name} in organization.");
					}

					OrganizationProducts organizationProducts = new OrganizationProducts(LoginUser);
					organizationProducts.LoadByOrganizationAndProductID(organization.OrganizationID, existingProduct.ProductID);

					if (!organizationProducts.Any())
					{
						organizationProducts = new OrganizationProducts(LoginUser);
						organizationProducts.AddNewOrganizationProduct();
						organizationProducts[0].OrganizationID = organizationId;
						organizationProducts[0].ProductID = existingProduct.ProductID;
						organizationProducts.Save();
						Logs.WriteEvent($"Added product {product.Product_Name} in customer.");
					}
				}
			}
		}

		private void UpdateContactInfo(ZohoContactData.Data contact, string accountId, int organizationId)
		{
			if (string.IsNullOrEmpty(contact.Email))
			{
				Logs.WriteEvent($"Contact {contact.First_Name} {contact.Last_Name} does not have email address therefore is not added.");
			}
			else
			{
				bool wasUpdated = false;
				Logs.WriteEvent($"Adding/updating contact information for {contact.Email} ({contact.Last_Name},{contact.First_Name}).");
				Organizations companies = new Organizations(LoginUser);
				companies.LoadByCRMLinkID(accountId, organizationId);

				if (companies.Any())
				{
					Organization company = companies[0];
					Users contacts = new Users(LoginUser);
					User user = null;

					contacts.LoadByOrganizationID(company.OrganizationID, false);

					//First Condition uses SalesforceID, prevents duplicate contacts being created when the email address is updated in SalesForce
					//vv This looks seriously wrong!, it looks like the sync of ZohoCRM contacts has never been working, and SalesForceId is the only column in the users table to keep 'track' of the contacts created by the integration
					if (contact.id != null && contacts.FindBySalesForceID(contact.id) != null)
					{
						user = contacts.FindBySalesForceID(contact.id);
					}
					else if (contacts.FindByEmail(contact.Email) != null)
					{
						user = contacts.FindByEmail(contact.Email);
					}
					else
					{
						string pwd = DataUtils.GenerateRandomPassword();
						Organizations organization = new Organizations(LoginUser);
						organization.LoadByOrganizationID(organizationId);
						bool isAdvancedPortal = organization[0].IsAdvancedPortal;

						//add the contact
						user = (new Users(LoginUser)).AddNewUser();
						user.OrganizationID = company.OrganizationID;
						user.IsActive = true;
						user.IsPasswordExpired = true;
						user.IsPortalUser = isAdvancedPortal && CrmLinkRow.AllowPortalAccess;
						user.CryptedPassword = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(pwd, "MD5");
						user.Email = contact.Email;
						user.LastName = contact.First_Name ?? "";
						user.FirstName = contact.First_Name ?? "";
						user.Title = contact.Title;
						user.MarkDeleted = false;
						user.SalesForceID = contact.id; //vv !!
						user.Collection.Save();

						Logs.WriteEvent("Contact was added.");

						if (isAdvancedPortal && CrmLinkRow.AllowPortalAccess && CrmLinkRow.SendWelcomeEmail)
						{
							EmailPosts.SendWelcomePortalUser(LoginUser, user.UserID, pwd);
						}
					}

					if (user.Email != contact.Email
						|| user.FirstName != (contact.First_Name ?? "")
						|| user.LastName != (contact.Last_Name ?? "")
						|| user.Title != (contact.Title ?? "")
						|| user.MarkDeleted
						|| user.SalesForceID != contact.id)
					{
						user.Email = contact.Email;
						user.LastName = contact.Last_Name ?? "";
						user.FirstName = contact.First_Name ?? "";
						user.Title = contact.Title;
						user.MarkDeleted = false;
						user.SalesForceID = contact.id; //vv !!
						user.Collection.Save();
						Logs.WriteEvent("Contact was updated.");
					}

					PhoneTypes phoneTypes = new PhoneTypes(LoginUser);
					phoneTypes.LoadAllPositions(organizationId);
					PhoneType crmPhoneType = null;

					//We'll save the phone number using the corresponding CRM ("phone" or "work") phone type.
					crmPhoneType = phoneTypes.FindByName("Phone");

					if (crmPhoneType == null)
					{
						crmPhoneType = AddPhoneType("Phone", phoneTypes.Count, organizationId);
						phoneTypes.LoadAllPositions(organizationId);
					}

					//The worktype is used regardless of the CRM phone type to be able to update the numbers processed by the previous version of this class.
					PhoneType workType = phoneTypes.FindByName("Work");

					if (workType == null)
					{
						workType = AddPhoneType("Work", phoneTypes.Count, organizationId);
						phoneTypes.LoadAllPositions(organizationId);
					}

					//All the CRMs uses Mobile for this phone type.
					PhoneType mobileType = phoneTypes.FindByName("Mobile");

					if (mobileType == null)
					{
						mobileType = AddPhoneType("Mobile", phoneTypes.Count, organizationId);
						phoneTypes.LoadAllPositions(organizationId);
					}

					//All the CRMs uses Fax for this phone type.
					PhoneType faxType = phoneTypes.FindByName("Fax");

					if (faxType == null)
					{
						faxType = AddPhoneType("Fax", phoneTypes.Count, organizationId);
						phoneTypes.LoadAllPositions(organizationId);
					}

					//2. Preparation. Get existing numbers, if any, to update instead of add new.
					PhoneNumber phone = null;
					PhoneNumber mobilePhone = null;
					PhoneNumber faxPhone = null;

					/*
					We'll proceed to find an existing number to update instead of incorrectly adding a new number everytime the contact get sync.
                    If more than one phone number exist with the type we are looking for, we might end up updating the incorrect number.
                    Unfortunately there is not an easy way to prevent this undesirable effect.
                    An alternative is to wipe all numbers and add the ones comming from the CRM. This has been reviewed and rejected by RJ.
                    Error chances are less if we update the first existing number with the type being updated than deleting existing numbers.
                    Specially because we are bringing only the first number from the CRM.
					*/
					PhoneNumbers findPhone = new PhoneNumbers(LoginUser);
					findPhone.LoadByID(user.UserID, ReferenceType.Users);

					if (findPhone.Any())
					{
						//The previous version assigned phone to the work type when the work type existed.
						//Because chances are low that the Work Type was deleted by a user chances are big that this is the number we need to update.
						phone = findPhone.FindByPhoneTypeID(workType.PhoneTypeID);

						//When no work number exist, there is a small chance that the work type was deleted.
						//In this case, for a long time we did not add the phone, recently we updated the code to add the number without type.
						//To handle this very low chance we look for a number without type.
						if (phone == null)
						{
							foreach (PhoneNumber existingNumber in findPhone)
							{
								if (existingNumber.PhoneTypeID == null)
								{
									phone = existingNumber;
									break;
								}
							}
						}

						//If no number have been found so far, maybe the current version already updated this contact.
						//Therefore we look for a number with the CRM phone type.
						if (phone == null && crmPhoneType != null)
						{
							phone = findPhone.FindByPhoneTypeID(crmPhoneType.PhoneTypeID);
						}

						if (mobilePhone != null)
						{
							mobilePhone = findPhone.FindByPhoneTypeID(mobileType.PhoneTypeID);
						}

						if (faxPhone != null)
						{
							faxPhone = findPhone.FindByPhoneTypeID(faxType.PhoneTypeID);
						}
					}

					string logMessage = "updated";

					//3. Action. Add/Update.
					if (string.IsNullOrEmpty(contact.Phone))
					{
						if (phone != null)
						{
							phone.Collection.DeleteFromDB(phone.PhoneID);
							Logs.WriteEvent("Contact Phone was deleted.");
						}
					}
					else
					{
						logMessage = "updated";

						if (phone == null)
						{
							phone = (new PhoneNumbers(LoginUser).AddNewPhoneNumber());
							logMessage = "added";
						}

						if (phone.Number != contact.Phone
							|| phone.RefType != ReferenceType.Users
							|| phone.RefID != user.UserID
							|| phone.PhoneTypeID == null
							|| phone.PhoneTypeID != crmPhoneType.PhoneTypeID
							|| phone.Extension != contact.TenmastExtension)
						{
							phone.Number = contact.Phone;
							phone.RefType = ReferenceType.Users;
							phone.RefID = user.UserID;
							phone.PhoneTypeID = crmPhoneType.PhoneTypeID;
							phone.Extension = contact.TenmastExtension;
							phone.Collection.Save();
							Logs.WriteEvent($"Contact phone was {logMessage}");
							wasUpdated = true;
						}
					}

					if (string.IsNullOrEmpty(contact.Mobile))
					{
						if (mobilePhone != null)
						{
							mobilePhone.Collection.DeleteFromDB(mobilePhone.PhoneID);
							Logs.WriteEvent("Contact Mobile was deleted.");
						}
					}
					else
					{
						logMessage = "updated";

						if (mobilePhone == null)
						{
							mobilePhone = (new PhoneNumbers(LoginUser).AddNewPhoneNumber());
							logMessage = "added";
						}

						if (mobilePhone.Number != contact.Mobile
							|| mobilePhone.RefType != ReferenceType.Users
							|| mobilePhone.RefID != user.UserID
							|| mobilePhone.PhoneTypeID == null
							|| mobilePhone.PhoneTypeID != mobileType.PhoneTypeID)
						{
							mobilePhone.Number = contact.Mobile;
							mobilePhone.RefType = ReferenceType.Users;
							mobilePhone.RefID = user.UserID;
							mobilePhone.PhoneTypeID = mobileType.PhoneTypeID;
							mobilePhone.Collection.Save();
							Logs.WriteEvent($"Contact mobile was {logMessage}");
							wasUpdated = true;
						}
					}

					if (string.IsNullOrEmpty(contact.Fax))
					{
						if (faxPhone != null)
						{
							faxPhone.Collection.DeleteFromDB(faxPhone.PhoneID);
							Logs.WriteEvent("Contact Fax was deleted.");
						}
					}
					else
					{
						logMessage = "updated";

						if (faxPhone == null)
						{
							faxPhone = (new PhoneNumbers(LoginUser).AddNewPhoneNumber());
							logMessage = "added";
						}

						if (faxPhone.Number != contact.Fax
							|| faxPhone.RefType != ReferenceType.Users
							|| faxPhone.RefID != user.UserID
							|| faxPhone.PhoneTypeID == null
							|| faxPhone.PhoneTypeID != faxType.PhoneTypeID)
						{
							faxPhone.Number = contact.Fax;
							faxPhone.RefType = ReferenceType.Users;
							faxPhone.RefID = user.UserID;
							faxPhone.PhoneTypeID = faxType.PhoneTypeID;
							faxPhone.Collection.Save();
							Logs.WriteEvent($"Contact fax was {logMessage}");
							wasUpdated = true;
						}
					}
					
				}

				if (!wasUpdated)
				{
					Logs.WriteEvent("Nothing needed to be updated.");
				}
			}
		}

		private void UpdateCompanyCustomMappings(List<ZohoAccountData.Account> accountFullData)
		{
			ObjectType objectType = ObjectType.company;
			CRMLinkFields customMappings = new CRMLinkFields(LoginUser);
			customMappings.LoadByObjectType("Account", CrmLinkRow.CRMLinkID);

			if (customMappings.Any())
			{
				CRMLinkErrors crmLinkErrors = new CRMLinkErrors(LoginUser);
				crmLinkErrors.LoadByOperation(CrmLinkRow.OrganizationID, CrmLinkRow.CRMType, Enums.GetDescription(IntegrationOrientation.IntoTeamSupport), Enums.GetDescription(objectType));
				CRMLinkError crmLinkError = null;

				foreach(ZohoAccountData.Account fullZohoObject in accountFullData)
				{
					try
					{
						JObject zohoAccountsData = JObject.Parse(fullZohoObject.info.JsonString);
						JToken zohoAccountsDataJToken = zohoAccountsData["data"];
						JArray zohoAccountsDataArray = JArray.Parse(zohoAccountsDataJToken.ToString());

						foreach (ZohoAccountData.Data zohoDataObject in fullZohoObject.data)
						{
							Organizations companyToUpdate = new Organizations(LoginUser);
							companyToUpdate.LoadByCRMLinkID(zohoDataObject.id, CrmLinkRow.OrganizationID);

							if (companyToUpdate.Any())
							{
								JObject zohoItem = zohoAccountsDataArray.Children<JObject>().FirstOrDefault(p => p["id"] != null && p["id"].ToString() == zohoDataObject.id);

								//Do the Custom Mapping updates here:
								foreach (CRMLinkField crmLinkField in customMappings)
								{
									string crmLinkFieldValue = zohoItem.GetValue(crmLinkField.CRMFieldName, StringComparison.OrdinalIgnoreCase)?.Value<string>();

									if (crmLinkField.CustomFieldID != null)
									{
										crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(zohoDataObject.id, crmLinkField.CustomFieldID.ToString());
										UpdateCustomValue((int)crmLinkField.CustomFieldID, companyToUpdate[0].OrganizationID, crmLinkFieldValue);
									}
									else if (crmLinkField.TSFieldName != null)
									{
										crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(zohoDataObject.id, crmLinkField.TSFieldName);
										companyToUpdate[0].Row[crmLinkField.TSFieldName] = crmLinkFieldValue;
										companyToUpdate[0].BaseCollection.Save();
									}

									if (crmLinkError != null)
									{
										crmLinkError.Delete();
										crmLinkErrors.Save();
									}
								}
							}
						}
					}
					catch (Exception ex)
					{
						Logs.WriteException(ex);
					}
				}
			}
		}

		private void UpdateContactCustomMappings(List<ZohoContactData.Contact> contactFullData)
		{
			ObjectType objectType = ObjectType.contact;
			CRMLinkFields customMappings = new CRMLinkFields(LoginUser);
			customMappings.LoadByObjectType(Enums.GetDescription(objectType), CrmLinkRow.CRMLinkID);

			if (customMappings.Any())
			{
				CRMLinkErrors crmLinkErrors = new CRMLinkErrors(LoginUser);
				crmLinkErrors.LoadByOperation(CrmLinkRow.OrganizationID, CrmLinkRow.CRMType, Enums.GetDescription(IntegrationOrientation.IntoTeamSupport), Enums.GetDescription(objectType));
				CRMLinkError crmLinkError = null;

				foreach (ZohoContactData.Contact fullZohoObject in contactFullData)
				{
					JObject zohoContactsData = JObject.Parse(fullZohoObject.info.JsonString);
					JToken zohoContactsDataJToken = zohoContactsData["data"];
					JArray zohoContactsDataArray = JArray.Parse(zohoContactsDataJToken.ToString());

					foreach (ZohoContactData.Data zohoDataObject in fullZohoObject.data)
					{
						Organizations companyOfContacts = new Organizations(LoginUser);
						companyOfContacts.LoadByCRMLinkID(zohoDataObject.id, CrmLinkRow.OrganizationID);

						if (companyOfContacts.Any())
						{
							Users userToUpdate = new Users(LoginUser);
							userToUpdate.LoadByEmail(zohoDataObject.Email, companyOfContacts[0].OrganizationID);

							if (userToUpdate.Any())
							{
								JObject zohoItem = zohoContactsDataArray.Children<JObject>().FirstOrDefault(p => p["id"] != null && p["id"].ToString() == zohoDataObject.id);

								//Do the Custom Mapping updates here:
								foreach (CRMLinkField crmLinkField in customMappings)
								{
									string crmLinkFieldValue = zohoItem.GetValue(crmLinkField.CRMFieldName, StringComparison.OrdinalIgnoreCase)?.Value<string>();

									if (crmLinkField.CustomFieldID != null)
									{
										crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(zohoDataObject.id, crmLinkField.CustomFieldID.ToString());
										UpdateCustomValue((int)crmLinkField.CustomFieldID, userToUpdate[0].UserID, crmLinkFieldValue);
									}
									else if (crmLinkField.TSFieldName != null)
									{
										crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(zohoDataObject.id, crmLinkField.TSFieldName);
										userToUpdate[0].Row[crmLinkField.TSFieldName] = crmLinkFieldValue;
										userToUpdate[0].BaseCollection.Save();
									}

									if (crmLinkError != null)
									{
										crmLinkError.Delete();
										crmLinkErrors.Save();
									}
								}
							}
						}
					}
				}
			}
		}

		private void UpdateCustomValue(int customFieldId, int recordId, string value)
		{
			CustomValues customValues = new CustomValues(LoginUser);
			CustomValue customValue = null;

			customValues.LoadByFieldID(customFieldId, recordId);

			if (customValues.Any())
			{
				customValue = customValues[0];
			}
			else
			{
				customValue = (new CustomValues(LoginUser)).AddNewCustomValue();
				customValue.CustomFieldID = customFieldId;
				customValue.RefID = recordId;
			}

			if (customValue != null && customValue.Value != value)
			{
				customValue.Value = value;
				customValue.Collection.Save();
			}
		}

		private void SendTicketData()
		{
			try
			{
				if (CrmLinkRow.SendBackTicketData)
				{
					//get tickets created after the last link date
					Tickets tickets = new Tickets(LoginUser);
					tickets.LoadByCRMLinkItem(CrmLinkRow);

					if (tickets.Any())
					{
						Logs.WriteEvent($"Found {tickets.Count.ToString()} tickets to sync.");
						CRMLinkErrors crmLinkErrors = new CRMLinkErrors(LoginUser);
						crmLinkErrors.LoadByOperation(CrmLinkRow.OrganizationID, CrmLinkRow.CRMType, "out", "ticket");
						CRMLinkError crmLinkError = null;

						foreach (Ticket ticket in tickets)
						{
							bool atLeastOneSucceded = false;
							//get a list of customers associated to the ticket
							OrganizationsView customers = new OrganizationsView(LoginUser);
							customers.LoadByTicketID(ticket.TicketID);
							crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID.ToString(), "note");

							foreach (OrganizationsViewItem customer in customers)
							{
								if (!string.IsNullOrEmpty(customer.CRMLinkID))
								{
									Logs.WriteEvent("Creating a comment...");

									if (CreateNote(customer.CRMLinkID, ticket))
									{
										Logs.WriteEvent("Comment created successfully.");
										atLeastOneSucceded = true;
									}
									else
									{
										Logs.WriteEvent("Error creating Note in ZohoCrm.");
									}
								}
							}

							CrmLinkRow.LastTicketID = ticket.TicketID;
							CrmLinkRow.Collection.Save();

							ActionLogs.AddActionLog(
									LoginUser,
									ActionLogType.Insert,
									ReferenceType.Tickets,
									ticket.TicketID,
									"Sent ticket data to ZohoCrm.");

							if (atLeastOneSucceded)
							{
								if (crmLinkError != null)
								{
									crmLinkError.Delete();
									crmLinkErrors.Save();
								}
							}
							else if (crmLinkError == null)
							{
								CRMLinkErrors newCrmLinkError = new CRMLinkErrors(LoginUser);
								crmLinkError = newCrmLinkError.AddNewCRMLinkError();
								crmLinkError.OrganizationID = CrmLinkRow.OrganizationID;
								crmLinkError.CRMType = CrmLinkRow.CRMType;
								crmLinkError.Orientation = "out";
								crmLinkError.ObjectType = "ticket";
								crmLinkError.ObjectFieldName = "note";
								crmLinkError.ObjectID = ticket.TicketID.ToString();
								crmLinkError.Exception = "Error creating ticket as note.";
								crmLinkError.OperationType = "create";
								newCrmLinkError.Save();
							}
							else
							{
								crmLinkError.Exception = "Error creating ticket as note.";
							}
						}

						crmLinkErrors.Save();
					}
					else
					{
						Logs.WriteEvent("No new tickets to sync.");
					}
				}
				else
				{
					Logs.WriteEvent("Ticket data not sent since SendBackTicketData is set to FALSE for this organization.");
				}
			}
			catch (Exception ex)
			{
				Logs.WriteException(ex);
			}
		}

		private bool CreateNote(string accountId, Ticket ticket)
		{
			Data.Action description = Actions.GetTicketDescription(LoginUser, ticket.TicketID);
			string appDomain = SystemSettings.ReadString(LoginUser, "AppDomain", "https://app.teamsupport.com");
			string noteBody = $"A ticket has been created for this organization entitled '{ticket.Name}'.{Environment.NewLine}{HtmlUtility.StripHTML(description.Description)}{Environment.NewLine}Click here to access the ticket information: {appDomain}/Ticket.aspx?ticketid={ticket.TicketID.ToString()}";
			dynamic dynamicObject = new JObject();
			dynamicObject.Parent_Id = accountId;
			dynamicObject.Note_Title = $"Support Issue: {ticket.Name}";
			dynamicObject.Note_Content = noteBody;
			dynamicObject.se_module = "Accounts";

			JObject jsonObject = new JObject();
			jsonObject["data"] = JToken.FromObject(new List<object> { dynamicObject });
			string postData = jsonObject.ToString();
			string apiUrl = "Notes";

			Uri zohoUri = new Uri(_zohoCrmAPIv2 + apiUrl);
			string response = MakeHttpRequest(zohoUri, "POST", postData);

			return !string.IsNullOrEmpty(response);
		}

		private string MakeHttpRequest(Uri uri, string method = "GET", string postData = "")
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
			request.Method = method;
			request.KeepAlive = false;
			request.UserAgent = "Muroc Client";
			request.Timeout = 10000;
			request.ContentType = "application/json";
			request.Headers.Add("Authorization", CrmLinkRow.SecurityToken1);

			if (CrmLinkRow.LastLink != null && method == "GET")
			{
				/// Zoho's lastModifiedTime is in the user's local time. 
				/// To include any possible timezone we use the smallest one (UTC-12:00) International Date Line West. (720)
				/// Then we go back 30 more minutes as it was coded before.
				/// This is coming from original v1 code.
				request.IfModifiedSince = DateTime.Parse(CrmLinkRow.LastLinkUtc.Value.AddMinutes(-750).ToString("s"));
			}

			string responseText = string.Empty;

			try
			{
				if (!string.IsNullOrEmpty(postData))
				{
					var writer = new StreamWriter(request.GetRequestStream());
					writer.Write(postData);
					writer.Close();
				}

				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				{
					if (request.HaveResponse && response != null)
					{
						CheckAPIRequestLimitAndWait(response);

						using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8))
						{
							responseText = reader.ReadToEnd();

							if (method == "POST" && response.StatusCode != HttpStatusCode.Created)
							{
								Logs.WriteEvent(responseText);
								Logs.WriteEvent($"Post data: {postData}");
								responseText = string.Empty;
							}
						}
					}
				}
			}
			catch (WebException webEx)
			{
				if (webEx.Status == WebExceptionStatus.ProtocolError)
				{
					HttpWebResponse response = webEx.Response as HttpWebResponse;

					if (response != null)
					{
						//We'll use status code numbers instead of the enum because the 429 returned by ZohoCRM does not exist in the HttpStatusCode enum. :(
						switch ((int)response.StatusCode)
						{
							case 304: // HttpStatusCode.NotModified
				//This is the error returned by ZohoCrm when there are no records to return for that IfModifiedSince value: The remote server returned an error: (304) Not Modified
								Logs.WriteEvent($"There were no records modified since last sync: {request.IfModifiedSince}. ZohoCRM error: {webEx.Message}");
								break;
							case 429: // Too many requests
									  //This is the error returned by ZohoCrm when the API Calls per minute has been reached.
								Logs.WriteEvent($"The maximum API calls per minute to ZohoCRM has been reached, we'll need to wait. ZohoCRM error: {webEx.Message}");
								CheckAPIRequestLimitAndWait(response);
								//Try again
								MakeHttpRequest(uri, method, postData);
								break;
							default:
								Logs.WriteException(webEx);
								break;
						}
				}
				else
				{
					Logs.WriteException(webEx);
					}
				}

					if (method == "POST")
					{
						Logs.WriteEvent($"Body: {postData}");
					}
				}
			catch (Exception ex)
			{
				Logs.WriteException(ex);
			}

			return responseText;
		}

		private void CheckAPIRequestLimitAndWait(HttpWebResponse response)
		{
			// Get the headers associated with the response.
			WebHeaderCollection headerCollection = response.Headers;
			String[] remaining = headerCollection.GetValues(RATELIMITREMAINING);
			String[] reset = headerCollection.GetValues(RATELIMITRESET);
			String[] limit = headerCollection.GetValues(RATELIMIT);

			if (remaining.Any())
			{
				int remainingCount = int.Parse(remaining[0]);

				if (remainingCount == 1)
				{
					Logs.WriteEventFormat("Remaining Count: {0}", remainingCount);
					Logs.WriteEventFormat("Reset: {0}", reset[0]);
					Logs.WriteEventFormat("Limit: {0}", limit[0]);

					//70 secs just in case the remaining header is not there.
					int resetSeconds = 70;

					if (reset.Any())
					{
						DateTime resetTime = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(long.Parse(reset[0]));
						TimeSpan wait = resetTime - DateTime.UtcNow;
						resetSeconds = (int)wait.TotalSeconds;
					}

					Logs.WriteEvent(string.Format($"Rate Limit of {limit[0]} reached. Sleeping {resetSeconds} seconds until it resets."));
					System.Threading.Thread.Sleep(resetSeconds * 1000);
					Logs.WriteEvent("Continuing...");
				}
			}
		}

		private enum ObjectType : byte
		{
			company= 1,
			contact = 2,
			product = 3
		}
	}
}
