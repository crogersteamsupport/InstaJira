using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using System.Runtime.Serialization;
using System.Globalization;
using System.Linq;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ProductService : System.Web.Services.WebService
    {

        public ProductService()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        [WebMethod]
        public ProductVersionsViewItemProxy GetVersion(int versionID)
        {
            ProductVersionsViewItem version = ProductVersionsView.GetProductVersionsViewItemByUserRights(TSAuthentication.GetLoginUser(), versionID);
            if (version == null || version.OrganizationID != TSAuthentication.OrganizationID) return null;
            return version.GetProxy();
        }

        [WebMethod]
        public ProductVersionsViewItemProxy[] GetVersions(int productID)
        {
            Product product = Products.GetProduct(TSAuthentication.GetLoginUser(), productID);
            if (product.OrganizationID != TSAuthentication.OrganizationID) return null;

            ProductVersionsView versions = new ProductVersionsView(product.Collection.LoginUser);
            versions.LoadByProductID(productID);
            return versions.GetProductVersionsViewItemProxies();
        }

        [WebMethod]
        public BasicProduct[] GetProducts()
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Products products = new Products(loginUser);
            products.LoadByOrganizationIDAndUserRights(TSAuthentication.OrganizationID, loginUser.UserID);

            List<BasicProduct> result = new List<BasicProduct>();
            foreach (Product product in products)
            {
                ProductVersions versions = new ProductVersions(products.LoginUser);
                versions.LoadByProductID(product.ProductID);

                List<BasicVersion> basicVersions = new List<BasicVersion>();
                foreach (ProductVersion version in versions)
                {
                    BasicVersion basicVersion = new BasicVersion();
                    basicVersion.ProductVersionID = version.ProductVersionID;
                    basicVersion.VersionNumber = version.VersionNumber;
                    basicVersions.Add(basicVersion);
                }

                BasicProduct basicProduct = new BasicProduct();
                basicProduct.ProductID = product.ProductID;
                basicProduct.Name = product.Name;
                basicProduct.Versions = basicVersions.ToArray();
                if (product.ProductFamilyID != null)
                {
                    basicProduct.ProductFamilyID = product.ProductFamilyID;
                }

                result.Add(basicProduct);
            }

            return result.ToArray();
        }

        [WebMethod]
        public ProductProxy GetProduct(int productID)
        {
            Product product = Products.GetProductByUserRights(TSAuthentication.GetLoginUser(), productID);
            if (product == null || product.OrganizationID != TSAuthentication.OrganizationID) return null;
            return product.GetProxy();
        }

        [WebMethod]
        public ProductFamilyProxy GetProductFamily(int productFamilyID)
        {
            ProductFamily productFamily = ProductFamilies.GetProductFamily(TSAuthentication.GetLoginUser(), productFamilyID);
            if (productFamily == null || productFamily.OrganizationID != TSAuthentication.OrganizationID) return null;
            return productFamily.GetProxy();
        }

        [WebMethod]
        public ProductFamilyProxy[] GetProductFamilies()
        {
            ProductFamilyProxy[] result = null;

            LoginUser loginUser =TSAuthentication.GetLoginUser();

            ProductFamilies productFamilies = new ProductFamilies(loginUser);
            productFamilies.LoadByOrganizationID(loginUser.OrganizationID);

            if (productFamilies.Any())
            {
                result = productFamilies.GetProductFamilyProxies();
            }

            return result;
        }

        [WebMethod]
        public ProdProp GetProperties(int productID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Products products = new Products(loginUser);
            products.LoadByProductID(productID);
            ProductFamilies productFamilies = new ProductFamilies(loginUser);

            SlaLevels slaLevels = new SlaLevels(loginUser);
            slaLevels.LoadByOrganizationID(TSAuthentication.OrganizationID);

            ProdProp prodProp = new ProdProp();

            if (products.IsEmpty) return null;

            string productFamily = "Empty";
            if (products[0].ProductFamilyID != null)
            {
                productFamilies.LoadByProductFamilyID((int)products[0].ProductFamilyID);
                productFamily = productFamilies.IsEmpty ? "" : productFamilies[0].Name;
            }

            string slaAssigned = "Empty";
            if (slaLevels.Any() && products[0].SlaLevelID != null)
            {
                var sla = slaLevels.Where(m => m.SlaLevelID == products[0].SlaLevelID).First();
                if (sla != null)
                {
                    slaAssigned = sla.Name;
                }
            }

            prodProp.ProductFamily = productFamily;
            prodProp.SlaAssigned = slaAssigned;
            prodProp.prodproxy = products[0].GetProxy();
            prodProp.TFSProjectName = products[0].TFSProjectName;
            prodProp.JiraProjectKey = products[0].JiraProjectKey;
            prodProp.JiraInstance = "None";
            JiraInstanceProducts jiraInstanceProducts = new JiraInstanceProducts(loginUser);
            jiraInstanceProducts.LoadByProductAndOrganization(productID, products[0].OrganizationID, "Jira");
            JiraInstanceProduct jiraInstanceProduct = jiraInstanceProducts.FirstOrDefault();

            if (jiraInstanceProduct != null && jiraInstanceProduct.CrmLinkId > 0)
            {
                CRMLinkTable links = new CRMLinkTable(loginUser);
                links.LoadByCRMLinkID(jiraInstanceProduct.CrmLinkId);
                prodProp.JiraInstance = links.Where(p => p.CRMLinkID == jiraInstanceProduct.CrmLinkId).Select(p => p.InstanceName).FirstOrDefault();
                prodProp.CrmLinkId = jiraInstanceProduct.CrmLinkId;
            }

            return prodProp;
        }

        [WebMethod]
        public AttachmentProxy[] GetAttachments(int versionID)
        {
            return TeamSupport.Data.Quarantine.WebAppQ.GetAttachmentProxies4(versionID, TSAuthentication.GetLoginUser());
        }


        [WebMethod]
        public void DeleteAttachment(int attachmentID)
        {
            TeamSupport.ModelAPI.AttachmentAPI.DeleteAttachment(attachmentID, AttachmentProxy.References.ProductVersions);
        }

        [WebMethod]
        public string GetShortNameFromID(int productID)
        {
            Products products = new Products(TSAuthentication.GetLoginUser());
            products.LoadByProductID(productID);

            if (products.IsEmpty) return "N/A";

            if (products[0].Name.Length > 10)
                return products[0].Name.Substring(0, 10).ToString() + "...";
            else
                return products[0].Name.ToString();
        }

        [WebMethod]
        public string GetVersionShortNameFromID(int productVersionID)
        {
            ProductVersions productVersions = new ProductVersions(TSAuthentication.GetLoginUser());
            productVersions.LoadByProductVersionID(productVersionID);

            if (productVersions.IsEmpty) return "N/A";

            if (productVersions[0].VersionNumber.Length > 10)
                return productVersions[0].VersionNumber.Substring(0, 10).ToString() + "...";
            else
                return productVersions[0].VersionNumber.ToString();
        }

        [WebMethod]
        public string GetFamilyShortNameFromID(int productFamilyID)
        {
            ProductFamilies productFamilies = new ProductFamilies(TSAuthentication.GetLoginUser());
            productFamilies.LoadByProductFamilyID(productFamilyID);

            if (productFamilies.IsEmpty) return "N/A";

            if (productFamilies[0].Name.Length > 10)
                return productFamilies[0].Name.Substring(0, 10).ToString() + "...";
            else
                return productFamilies[0].Name.ToString();
        }

        [WebMethod]
        public string UpdateRecentlyViewed(string viewid)
        {
            int refType, refID;

            refID = Convert.ToInt32(viewid.Substring(1));

            LoginUser loginUser = TSAuthentication.GetLoginUser();

            if (viewid.StartsWith("v"))
            {
                refType = (int)ReferenceType.ProductVersions;
                ProductVersions pv = new ProductVersions(loginUser);
                pv.LoadByProductVersionID(refID);
                if (pv.IsEmpty)
                {
                    return GetRecentlyViewed();
                }
            }
            else if (viewid.StartsWith("p"))
            {
                refType = (int)ReferenceType.Products;
                Products p = new Products(loginUser);
                p.LoadByProductID(refID);
                if (p.IsEmpty)
                {
                    return GetRecentlyViewed();
                }
            }
            else
            {
                refType = (int)ReferenceType.ProductFamilies;
                ProductFamilies pf = new ProductFamilies(loginUser);
                pf.LoadByProductFamilyID(refID);
                if (pf.IsEmpty)
                {
                    return GetRecentlyViewed();
                }
            }

            RecentlyViewedItem recent = (new RecentlyViewedItems(loginUser).AddNewRecentlyViewedItem());


            recent.RefID = refID;
            recent.RefType = refType;
            recent.DateViewed = DateTime.UtcNow;
            recent.UserID = TSAuthentication.GetLoginUser().UserID;
            recent.BaseCollection.Save();

            return GetRecentlyViewed();
        }

        [WebMethod]
        public string GetRecentlyViewed()
        {
            StringBuilder builder = new StringBuilder();
            RecentlyViewedItems recent = new RecentlyViewedItems(TSAuthentication.GetLoginUser());
            recent.LoadRecentForProductsPage(TSAuthentication.GetLoginUser().UserID);

            builder.Append(@"<ul class=""recent-list"">");
            foreach (RecentlyViewedItem item in recent)
            {
                builder.Append(CreateRecentlyViewed(item));
            }
            builder.Append("</ul>");
            return builder.ToString();
        }

        public string CreateRecentlyViewed(RecentlyViewedItem recent)
        {
            string recentHTML;
            if (recent.RefType == (int)ReferenceType.ProductVersions)
            {
                ProductVersions pv = new ProductVersions(TSAuthentication.GetLoginUser());
                pv.LoadByProductVersionID(recent.RefID);
                recentHTML = @" 
          <li>
            <div class=""recent-info"">
              <h4><a class=""productversionlink"" data-productversionid=""{0}"" href=""""><i class=""fa fa-clock-o color-orange""></i>{1}</a></h4>
            </div>
          </li>";
                return string.Format(recentHTML, pv[0].ProductVersionID, pv[0].VersionNumber);
            }
            else if (recent.RefType == (int)ReferenceType.Products)
            {
                Products p = new Products(TSAuthentication.GetLoginUser());
                p.LoadByProductID(recent.RefID);
                recentHTML = @" 
          <li>
            <div class=""recent-info"">
              <h4><a class=""productlink"" data-productid=""{0}"" href=""""><i class=""fa fa-barcode color-green""></i>{1}</a></h4>
            </div>
          </li>";

                return string.Format(recentHTML, p[0].ProductID, p[0].Name);
            }
            else
            {
                ProductFamilies pf = new ProductFamilies(TSAuthentication.GetLoginUser());
                pf.LoadByProductFamilyID(recent.RefID);
                recentHTML = @" 
          <li>
            <div class=""recent-info"">
              <h4><a class=""productfamilylink"" data-productfamilyid=""{0}"" href=""""><i class=""fa fa-umbrella color-magenta""></i>{1}</a></h4>
            </div>
          </li>";

                return string.Format(recentHTML, pf[0].ProductFamilyID, pf[0].Name);
            }
        }

        [WebMethod]
        public int SaveProduct(string data)
        {
            NewProductSave info;
            try
            {
                info = Newtonsoft.Json.JsonConvert.DeserializeObject<NewProductSave>(data);
            }
            catch (Exception e)
            {
                return -1;
            }

            Product product;
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Products products = new Products(loginUser);

            product = products.AddNewProduct();
            product.OrganizationID = loginUser.OrganizationID;
            product.Name = info.Name;
            product.Description = info.Description;
            product.ProductFamilyID = info.ProductFamilyID;
            product.JiraProjectKey = info.JiraProjectKey;
            product.Collection.Save();

            string description = String.Format("{0} created {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, product.Name);
            ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Products, product.ProductID, description);

            foreach (CustomFieldSaveInfo field in info.Fields)
            {
                CustomValue customValue = CustomValues.GetValue(loginUser, field.CustomFieldID, product.ProductID);
                if (field.Value == null)
                {
                    customValue.Value = "";
                }
                else
                {
                    if (customValue.FieldType == CustomFieldType.DateTime)
                    {
                        customValue.Value = ((DateTime)field.Value).ToString();
                    }
                    else
                    {
                        customValue.Value = field.Value.ToString();
                    }

                }

                customValue.Collection.Save();
            }

            return product.ProductID;

        }

        [WebMethod]
        public int SaveProductVersion(string data)
        {
            NewProductVersionSave info;
            try
            {
                info = Newtonsoft.Json.JsonConvert.DeserializeObject<NewProductVersionSave>(data);
            }
            catch (Exception e)
            {
                return -1;
            }

            ProductVersion productVersion;
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            ProductVersions productVersions = new ProductVersions(loginUser);

            productVersion = productVersions.AddNewProductVersion();
            productVersion.ProductID = info.ProductID;
            productVersion.ProductVersionStatusID = info.ProductVersionStatusID;
            productVersion.VersionNumber = info.VersionNumber;
            if (info.ReleaseDate != null)
                productVersion.ReleaseDate = (DateTime)info.ReleaseDate;
            productVersion.IsReleased = info.IsReleased;
            productVersion.Description = info.Description;
            productVersion.JiraProjectKey = info.JiraProjectKey;
            productVersion.Collection.Save();

            string description = String.Format("{0} created {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, productVersion.VersionNumber);
            ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.ProductVersions, productVersion.ProductVersionID, description);

            foreach (CustomFieldSaveInfo field in info.Fields)
            {
                CustomValue customValue = CustomValues.GetValue(loginUser, field.CustomFieldID, productVersion.ProductVersionID);
                if (field.Value == null)
                {
                    customValue.Value = "";
                }
                else
                {
                    if (customValue.FieldType == CustomFieldType.DateTime)
                    {
                        customValue.Value = ((DateTime)field.Value).ToString();
                    }
                    else
                    {
                        customValue.Value = field.Value.ToString();
                    }

                }

                customValue.Collection.Save();
            }

            return productVersion.ProductVersionID;

        }

        [WebMethod]
        public int SaveProductFamily(string data)
        {
            NewProductFamilySave info;
            try
            {
                info = Newtonsoft.Json.JsonConvert.DeserializeObject<NewProductFamilySave>(data);
            }
            catch (Exception e)
            {
                return -1;
            }

            ProductFamily productFamily;
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            ProductFamilies productfamilies = new ProductFamilies(loginUser);

            productFamily = productfamilies.AddNewProductFamily();
            productFamily.OrganizationID = loginUser.OrganizationID;
            productFamily.Name = info.Name;
            productFamily.Description = info.Description;
            productFamily.Collection.Save();

            string description = String.Format("{0} created {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, productFamily.Name);
            ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.ProductFamilies, productFamily.ProductFamilyID, description);

            foreach (CustomFieldSaveInfo field in info.Fields)
            {
                CustomValue customValue = CustomValues.GetValue(loginUser, field.CustomFieldID, productFamily.ProductFamilyID);
                if (field.Value == null)
                {
                    customValue.Value = "";
                }
                else
                {
                    if (customValue.FieldType == CustomFieldType.DateTime)
                    {
                        customValue.Value = ((DateTime)field.Value).ToString();
                        //DateTime dt;
                        //if (DateTime.TryParse(((string)field.Value), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
                        //{
                        //    customValue.Value = dt.ToUniversalTime().ToString();
                        //}
                    }
                    else
                    {
                        customValue.Value = field.Value.ToString();
                    }

                }

                customValue.Collection.Save();
            }

            return productFamily.ProductFamilyID;

        }

        [WebMethod]
        public BasicVersionStatus[] GetProductVersionStatuses()
        {
            ProductVersionStatuses productVersionStatuses = new ProductVersionStatuses(TSAuthentication.GetLoginUser());
            productVersionStatuses.LoadByOrganizationID(TSAuthentication.OrganizationID);

            List<BasicVersionStatus> result = new List<BasicVersionStatus>();
            foreach (ProductVersionStatus productVersionStatus in productVersionStatuses)
            {
                BasicVersionStatus basicVersionStatus = new BasicVersionStatus();
                basicVersionStatus.ProductVersionStatusID = productVersionStatus.ProductVersionStatusID;
                basicVersionStatus.Name = productVersionStatus.Name;

                result.Add(basicVersionStatus);
            }

            return result.ToArray();
        }

        [WebMethod]
        public int GetProductOpenTicketCount(int productID)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

            TicketTypes ticketTypes = new TicketTypes(TSAuthentication.GetLoginUser());
            ticketTypes.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID, organizations[0].ProductType);

            int count = 0;
            foreach (TicketType ticketType in ticketTypes)
            {
                count += Tickets.GetProductOpenTicketCount(TSAuthentication.GetLoginUser(), productID, ticketType.TicketTypeID);
            }

            return count;
        }

        [WebMethod]
        public int GetProductVersionOpenTicketCount(int versionID)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

            TicketTypes ticketTypes = new TicketTypes(TSAuthentication.GetLoginUser());
            ticketTypes.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID, organizations[0].ProductType);

            int count = 0;
            foreach (TicketType ticketType in ticketTypes)
            {
                count += Tickets.GetProductVersionOpenTicketCount(TSAuthentication.GetLoginUser(), versionID, ticketType.TicketTypeID, TSAuthentication.OrganizationID);
            }

            return count;
        }

        [WebMethod]
        public string LoadChartData(int productID, bool open)
        {

            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

            TicketTypes ticketTypes = new TicketTypes(TSAuthentication.GetLoginUser());
            ticketTypes.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID, organizations[0].ProductType);

            int total = 0;
            StringBuilder chartString = new StringBuilder("");

            foreach (TicketType ticketType in ticketTypes)
            {
                int count;
                if (open)
                    count = Tickets.GetProductOpenTicketCount(TSAuthentication.GetLoginUser(), productID, ticketType.TicketTypeID);
                else
                    count = Tickets.GetProductClosedTicketCount(TSAuthentication.GetLoginUser(), productID, ticketType.TicketTypeID);
                total += count;

                if (count > 0)
                    chartString.AppendFormat("{0},{1},", ticketType.Name.Replace(",", ""), count.ToString().Replace(",", ""));
                //chartString.AppendFormat("['{0}',{1}],",ticketType.Name, count.ToString());
            }
            if (chartString.ToString().EndsWith(","))
            {
                chartString.Remove(chartString.Length - 1, 1);
            }

            return chartString.ToString();
        }

        [WebMethod]
        public string LoadVersionChartData(int productVersionID, bool open)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Organizations organizations = new Organizations(loginUser);
            organizations.LoadByOrganizationID(loginUser.OrganizationID);

            TicketTypes ticketTypes = new TicketTypes(loginUser);
            ticketTypes.LoadByOrganizationID(loginUser.OrganizationID, organizations[0].ProductType);

            int total = 0;
            StringBuilder chartString = new StringBuilder("");

            foreach (TicketType ticketType in ticketTypes)
            {
                int count;
                if (open)
                    count = Tickets.GetProductVersionOpenTicketCount(loginUser, productVersionID, ticketType.TicketTypeID, TSAuthentication.OrganizationID);
                else
                    count = Tickets.GetProductVersionClosedTicketCount(loginUser, productVersionID, ticketType.TicketTypeID, TSAuthentication.OrganizationID);
                total += count;

                if (count > 0)
                    chartString.AppendFormat("{0},{1},", ticketType.Name.Replace(",", ""), count.ToString().Replace(",", ""));
                //chartString.AppendFormat("['{0}',{1}],",ticketType.Name, count.ToString());
            }
            if (chartString.ToString().EndsWith(","))
            {
                chartString.Remove(chartString.Length - 1, 1);
            }

            return chartString.ToString();
        }

        [WebMethod]
        public string LoadFamilyChartData(int productFamilyID, bool open)
        {

            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

            TicketTypes ticketTypes = new TicketTypes(TSAuthentication.GetLoginUser());
            ticketTypes.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID, organizations[0].ProductType);

            int total = 0;
            StringBuilder chartString = new StringBuilder("");

            foreach (TicketType ticketType in ticketTypes)
            {
                int count;
                if (open)
                    count = Tickets.GetProductFamilyOpenTicketCount(TSAuthentication.GetLoginUser(), productFamilyID, ticketType.TicketTypeID);
                else
                    count = Tickets.GetProductFamilyClosedTicketCount(TSAuthentication.GetLoginUser(), productFamilyID, ticketType.TicketTypeID);
                total += count;

                if (count > 0)
                    chartString.AppendFormat("{0},{1},", ticketType.Name.Replace(",", ""), count.ToString().Replace(",", ""));
                //chartString.AppendFormat("['{0}',{1}],",ticketType.Name, count.ToString());
            }
            if (chartString.ToString().EndsWith(","))
            {
                chartString.Remove(chartString.Length - 1, 1);
            }

            return chartString.ToString();
        }

        [WebMethod]
        public string GetProductTickets(int productID, int closed)
        {
            TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());

            return tickets.GetProductTicketCount(productID, closed).ToString();
        }

        [WebMethod]
        public string GetProductVersionTickets(int productVersionID, int closed)
        {
            Tickets tickets = new Tickets(TSAuthentication.GetLoginUser());

            return tickets.GetProductVersionTicketCount(productVersionID, closed, TSAuthentication.OrganizationID).ToString();
        }

        [WebMethod]
        public string GetProductFamilyTickets(int productFamilyID, int closed)
        {
            TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());

            return tickets.GetProductFamilyTicketCount(productFamilyID, closed).ToString();
        }

        [WebMethod]
        public ActionLogProxy[] LoadHistory(int productID, int start)
        {
            ActionLogs actionLogs = new ActionLogs(TSAuthentication.GetLoginUser());
            actionLogs.LoadByProductIDLimit(productID, start);

            return actionLogs.GetActionLogProxies();
        }

        [WebMethod]
        public ActionLogProxy[] LoadVersionHistory(int productVersionID, int start)
        {
            ActionLogs actionLogs = new ActionLogs(TSAuthentication.GetLoginUser());
            actionLogs.LoadByProductVersionIDLimit(productVersionID, start);

            return actionLogs.GetActionLogProxies();
        }

        [WebMethod]
        public ActionLogProxy[] LoadFamilyHistory(int productFamilyID, int start)
        {
            ActionLogs actionLogs = new ActionLogs(TSAuthentication.GetLoginUser());
            actionLogs.LoadByProductFamilyIDLimit(productFamilyID, start);

            return actionLogs.GetActionLogProxies();
        }

        [WebMethod]
        public string SetName(int productID, string value)
        {
            Product p = Products.GetProduct(TSAuthentication.GetLoginUser(), productID);
            p.Name = value;
            p.Collection.Save();
            string description = String.Format("{0} set product name to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Products, productID, description);
            return value != "" ? value : "Empty";
        }

        [WebMethod]
        public string SetVersionNumber(int productVersionID, string value)
        {
            ProductVersion pv = ProductVersions.GetProductVersion(TSAuthentication.GetLoginUser(), productVersionID);
            pv.VersionNumber = value;
            pv.Collection.Save();
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            string description = String.Format("{0} set product version number to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, value);
            ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.ProductVersions, productVersionID, description);
            return value != "" ? value : "Empty";
        }

        [WebMethod]
        public string SetFamilyName(int productFamilyID, string value)
        {
            ProductFamily pf = ProductFamilies.GetProductFamily(TSAuthentication.GetLoginUser(), productFamilyID);
            pf.Name = value;
            pf.Collection.Save();
            string description = String.Format("{0} set product family name to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.ProductFamilies, productFamilyID, description);
            return value != "" ? value : "Empty";
        }

        [WebMethod]
        public string SetDescription(int productID, string value)
        {
            Product p = Products.GetProduct(TSAuthentication.GetLoginUser(), productID);
            p.Description = value;
            p.Collection.Save();
            string description = String.Format("{0} set product description to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Products, productID, description);
            return value != "" ? value : "Empty";
        }

        [WebMethod]
        public string SetVersionDescription(int productVersionID, string value)
        {
            ProductVersion pv = ProductVersions.GetProductVersion(TSAuthentication.GetLoginUser(), productVersionID);
            pv.Description = value;
            pv.Collection.Save();
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            string description = String.Format("{0} set product version description to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, value);
            ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.ProductVersions, productVersionID, description);
            return value != "" ? value : "Empty";
        }

        [WebMethod]
        public string SetFamilyDescription(int productFamilyID, string value)
        {
            ProductFamily pf = ProductFamilies.GetProductFamily(TSAuthentication.GetLoginUser(), productFamilyID);
            pf.Description = value;
            pf.Collection.Save();
            string description = String.Format("{0} set product description to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.ProductFamilies, productFamilyID, description);
            return value != "" ? value : "Empty";
        }

        [WebMethod]
        public int SetProductFamily(int productID, int value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Product p = Products.GetProduct(loginUser, productID);
            if (value == -1)
            {
                p.ProductFamilyID = null;
            }
            else
            {
                p.ProductFamilyID = value;
            }
            p.Collection.Save();
            ProductFamilies pf = new ProductFamilies(loginUser);
            pf.LoadByProductFamilyID(value);
            string description = String.Format("{0} set product family as {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, pf.IsEmpty ? "Unassigned" : pf[0].Name);
            ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Products, productID, description);
            return value;
        }

        [WebMethod]
        public int SetSlaLevel(int productID, int value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Product p = Products.GetProduct(loginUser, productID);
            if (value == -1)
            {
                p.SlaLevelID = null;
            }
            else
            {
                p.SlaLevelID = value;
            }
            p.Collection.Save();
            SlaLevels slaLevels = new SlaLevels(loginUser);
            slaLevels.LoadBySlaLevelID(value);

            string description = String.Format("{0} set product sla as {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, slaLevels.IsEmpty ? "Unassigned" : slaLevels[0].Name);
            ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Products, productID, description);
            return value;
        }

        [WebMethod]
        public string SetProductJiraProjectKey(int id, string value, bool isForProductVersion)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            string description = string.Empty;

            if (isForProductVersion)
            {
                ProductVersion productVersion = ProductVersions.GetProductVersion(loginUser, id);
                productVersion.JiraProjectKey = string.IsNullOrEmpty(value) ? null : value;
                productVersion.Collection.Save();
                description = String.Format("{0} set product version jira project key as {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, string.IsNullOrEmpty(value) ? "NULL" : value);
            }
            else
            {
                Product product = Products.GetProduct(loginUser, id);
                product.JiraProjectKey = string.IsNullOrEmpty(value) ? null : value;
                product.Collection.Save();
                description = String.Format("{0} set product jira project key as {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, string.IsNullOrEmpty(value) ? "NULL" : value);
            }

            ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Products, id, description);

            return value;
        }

        [WebMethod]
        public string SetProductTFSProjectName(int id, string value, bool isForProductVersion)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            string description = string.Empty;

            if (isForProductVersion)
            {
                ProductVersion productVersion = ProductVersions.GetProductVersion(loginUser, id);
                productVersion.TFSProjectName = string.IsNullOrEmpty(value) ? null : value;
                productVersion.Collection.Save();
                description = String.Format("{0} set product version tfs project name as {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, string.IsNullOrEmpty(value) ? "NULL" : value);
            }
            else
            {
                Product product = Products.GetProduct(loginUser, id);
                product.TFSProjectName = string.IsNullOrEmpty(value) ? null : value;
                product.Collection.Save();
                description = String.Format("{0} set product tfs project name as {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, string.IsNullOrEmpty(value) ? "NULL" : value);
            }

            ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Products, id, description);

            return value;
        }

        [WebMethod]
        public string SetEmailReplyToAddress(int id, string value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            string description = string.Empty;

            Product product = Products.GetProduct(loginUser, id);
            product.EmailReplyToAddress = string.IsNullOrEmpty(value) ? null : value;
            product.Collection.Save();
            description = String.Format("{0} set Email Reply To Address as {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, string.IsNullOrEmpty(value) ? "NULL" : value);

            ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Products, id, description);

            return value;
        }

        [WebMethod]
        public string LoadVersions(int productID, int start)
        {
            StringBuilder htmlresults = new StringBuilder("");
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            ProductVersionsView productVersions = new ProductVersionsView(loginUser);
            productVersions.LoadByProductIDLimit(productID, start);

            foreach (ProductVersionsViewItem productVersion in productVersions)
            {
                htmlresults.AppendFormat(@"<div class='list-group-item'>
                            <span class='pull-right'>{0}</span>
                            <a href='#' id='{1}' class='productversionlink'>
                              <h4 class='list-group-item-heading'>{2}</h4>
                            </a>
                            <div class='row'>
                                <div class='col-xs-6'>
                                    <p class='list-group-item-text'>{3}</p>
                                    {4}
                                </div>
                                <div class='col-xs-6'>
                                    <p class='list-group-item-text'>{5} Open Tickets</p>
                                    <p class='list-group-item-text'>{6} Closed Tickets</p>                            
                                </div>
                            </div>
                            </div>"

                    , productVersion.IsReleased ? "Released" : "Not released"
                    , productVersion.ProductVersionID
                    , productVersion.VersionNumber
                    , productVersion.VersionStatus
                    , (productVersion.IsReleased && productVersion.ReleaseDate != null) ? "Released on " + DataUtils.DateToLocal(loginUser, (((DateTime)productVersion.ReleaseDateUtc))).ToString(GetDateFormatNormal()) : ""
                    , GetProductVersionTickets(productVersion.ProductVersionID, 0)
                    , GetProductVersionTickets(productVersion.ProductVersionID, 1));
            }

            return htmlresults.ToString();
        }

        [WebMethod]
        public string LoadFamilyProducts(int productFamilyID, int start)
        {
            StringBuilder htmlresults = new StringBuilder("");
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Products products = new Products(loginUser);
            products.LoadByProductFamilyIDLimit(productFamilyID, start);

            foreach (Product product in products)
            {
                //htmlresults.AppendFormat(@"<div class='list-group-item'>
                //                <a href='#' id='{0}' class='productlink'>
                //                  <h4 class='list-group-item-heading'>{1}</h4>
                //                </a>
                //                <div class='row'>
                //                    <div class='col-xs-6'>
                //                        <p class='list-group-item-text'>{2} Open Tickets</p>
                //                        <p class='list-group-item-text'>{3} Closed Tickets</p>                            
                //                    </div>
                //                </div>
                //                </div>"

                //    , product.ProductID
                //    , product.Name
                //    , GetProductTickets(product.ProductID, 0)
                //    , GetProductTickets(product.ProductID, 1));
                htmlresults.AppendFormat(@"<div class='list-group-item'>
                            <a href='#' id='{0}' class='productlink'>
                              <h4 class='list-group-item-heading'>{1}</h4>
                            </a>
                            </div>"

                    , product.ProductID
                    , product.Name);
            }

            return htmlresults.ToString();
        }

        [WebMethod]
        public FamilyProduct LoadFamilyProducts2(int productFamilyID, int start)
        {
            StringBuilder htmlresults = new StringBuilder("");
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Products products = new Products(loginUser);
            products.LoadByProductFamilyIDLimit(productFamilyID, start);

            foreach (Product product in products)
            {
                //htmlresults.AppendFormat(@"<div class='list-group-item'>
                //                <a href='#' id='{0}' class='productlink'>
                //                  <h4 class='list-group-item-heading'>{1}</h4>
                //                </a>
                //                <div class='row'>
                //                    <div class='col-xs-6'>
                //                        <p class='list-group-item-text'>{2} Open Tickets</p>
                //                        <p class='list-group-item-text'>{3} Closed Tickets</p>                            
                //                    </div>
                //                </div>
                //                </div>"

                //    , product.ProductID
                //    , product.Name
                //    , GetProductTickets(product.ProductID, 0)
                //    , GetProductTickets(product.ProductID, 1));
                htmlresults.AppendFormat(@"<div class='list-group-item'>
                            <a href='#' id='{0}' class='productlink'>
                              <h4 class='list-group-item-heading'>{1}</h4>
                            </a>
                            </div>"

                    , product.ProductID
                    , product.Name);
            }

            FamilyProduct result = new FamilyProduct();
            if (products.Count == 10)
            {
                result.MoreAvailable = true;
            }
            result.HTML = htmlresults.ToString();
            return result;
        }

        [WebMethod]
        public AutocompleteItem[] SearchProductFamily(string searchTerm)
        {
            ProductFamilies productFamilies = new ProductFamilies(TSAuthentication.GetLoginUser());
            productFamilies.LoadByLikeProductFamilyName(TSAuthentication.OrganizationID, searchTerm);

            List<AutocompleteItem> list = new List<AutocompleteItem>();
            foreach (ProductFamily productFamily in productFamilies)
            {
                list.Add(new AutocompleteItem(productFamily.Name, productFamily.ProductFamilyID.ToString()));
            }

            return list.ToArray();
        }

        [WebMethod]
        public ProductCustomOrganization[] LoadCustomers(int productID, int start, string sortColumn, string sortDirection)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            OrganizationProductsView organizationProducts = new OrganizationProductsView(loginUser);
            organizationProducts.LoadByProductIDLimit(productID, start, GetSortColumnTableName(sortColumn), sortDirection);
            List<ProductCustomOrganization> list = new List<ProductCustomOrganization>();
            CustomFields fields = new CustomFields(loginUser);
            fields.LoadByReferenceType(loginUser.OrganizationID, ReferenceType.OrganizationProducts);


            foreach (DataRow row in organizationProducts.Table.Rows)
            {
                ProductCustomOrganization test = new ProductCustomOrganization();
                test.Customer = row["OrganizationName"].ToString();
                test.VersionNumber = row["VersionNumber"].ToString();
                test.SupportExpiration = row["SupportExpiration"].ToString() != "" ? DataUtils.DateToLocal(loginUser, (((DateTime)row["SupportExpiration"]))).ToString(GetDateFormatNormal()) : "";
                test.VersionStatus = row["VersionStatus"].ToString();
                test.IsReleased = row["IsReleased"].ToString();
                test.ReleaseDate = row["ReleaseDate"].ToString() != "" ? ((DateTime)row["ReleaseDate"]).ToString(GetDateFormatNormal()) : "";
                test.DateCreated = ((DateTime)row["DateCreated"]).ToString(GetDateFormatNormal()); ;
                test.OrganizationProductID = (int)row["OrganizationProductID"];
                test.CustomFields = new List<string>();
                foreach (CustomField field in fields)
                {
                    CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, test.OrganizationProductID);
                    DateTime temp;
                    if (customValue.FieldType == CustomFieldType.Date)
                    {
                        DateTime.TryParse(customValue.Value, out temp);
                        test.CustomFields.Add(temp.ToString(GetDateFormatNormal()));
                    }
                    else
                        test.CustomFields.Add(customValue.Value);
                }


                list.Add(test);
            }


            return list.ToArray();
        }

        private string GetSortColumnTableName(string interfaceName)
        {
            string result = "OrganizationProductID";
            switch (interfaceName.ToLower())
            {
                case "customer":
                    result = "OrganizationName";
                    break;
                case "version":
                    result = "VersionNumber";
                    break;
                case "support expiration":
                    result = "SupportExpiration";
                    break;
                case "status":
                    result = "VersionStatus";
                    break;
                case "released":
                    result = "IsReleased";
                    break;
                case "released date":
                    result = "ReleaseDate";
                    break;
                case "date created":
                    result = "OrganizationProductID";
                    break;
            }
            return result;
        }
        [WebMethod]
        public ProductCustomOrganization[] LoadVersionCustomers(int productVersionID, int start, string sortColumn, string sortDirection)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            OrganizationProductsView organizationProducts = new OrganizationProductsView(loginUser);
            organizationProducts.LoadByProductVersionIDLimit(productVersionID, start, GetSortColumnTableName(sortColumn), sortDirection);
            List<ProductCustomOrganization> list = new List<ProductCustomOrganization>();
            CustomFields fields = new CustomFields(loginUser);
            fields.LoadByReferenceType(loginUser.OrganizationID, ReferenceType.OrganizationProducts);


            foreach (DataRow row in organizationProducts.Table.Rows)
            {
                ProductCustomOrganization test = new ProductCustomOrganization();
                test.Customer = row["OrganizationName"].ToString();
                test.VersionNumber = row["VersionNumber"].ToString();
                test.SupportExpiration = row["SupportExpiration"].ToString() != "" ? DataUtils.DateToLocal(loginUser, (((DateTime)row["SupportExpiration"]))).ToString(GetDateFormatNormal()) : "";
                test.VersionStatus = row["VersionStatus"].ToString();
                test.IsReleased = row["IsReleased"].ToString();
                test.ReleaseDate = row["ReleaseDate"].ToString() != "" ? ((DateTime)row["ReleaseDate"]).ToString(GetDateFormatNormal()) : "";
                test.DateCreated = row["DateCreated"].ToString() != "" ? ((DateTime)row["DateCreated"]).ToString(GetDateFormatNormal()) : "";
                test.OrganizationProductID = (int)row["OrganizationProductID"];
                test.CustomFields = new List<string>();
                foreach (CustomField field in fields)
                {
                    CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, test.OrganizationProductID);
                    test.CustomFields.Add(customValue.Value);
                }


                list.Add(test);
            }


            return list.ToArray();
        }

        [WebMethod]
        public ProductCustomOrganization LoadCustomer(int organizationProductID)
        {
            OrganizationProductsViewItem organizationProduct = (OrganizationProductsViewItem)OrganizationProductsView.GetOrganizationProductsViewItem(TSAuthentication.GetLoginUser(), organizationProductID);
            ProductCustomOrganization productCustomer = new ProductCustomOrganization();

            productCustomer.Customer = organizationProduct.OrganizationName;
            productCustomer.VersionNumber = organizationProduct.ProductVersionID.HasValue ? organizationProduct.ProductVersionID.ToString() : "-1";

            if (organizationProduct.SupportExpiration.HasValue)
                productCustomer.SupportExpiration = ((DateTime)organizationProduct.SupportExpiration).ToString(GetDateFormatNormal());
            else
                productCustomer.SupportExpiration = "";

            productCustomer.VersionStatus = "";
            productCustomer.IsReleased = "";
            productCustomer.ReleaseDate = null;
            productCustomer.OrganizationProductID = organizationProduct.OrganizationProductID;
            productCustomer.OrganizationID = organizationProduct.OrganizationID;

            return productCustomer;
        }

        public string GetDateFormatNormal()
        {
            CultureInfo us = new CultureInfo(TSAuthentication.GetLoginUser().CultureInfo.ToString());
            return us.DateTimeFormat.ShortDatePattern;
        }

        [WebMethod]
        public string LoadAssets(int productID, int start)
        {
            StringBuilder htmlresults = new StringBuilder("");
            AssetsView assets = new AssetsView(TSAuthentication.GetLoginUser());
            assets.LoadByProductIDLimit(productID, start);

            StringBuilder productVersionNumberDisplayName;
            StringBuilder serialNumberDisplayValue;
            StringBuilder warrantyExpirationDisplayValue;

            foreach (AssetsViewItem asset in assets)
            {
                productVersionNumberDisplayName = new StringBuilder();
                serialNumberDisplayValue = new StringBuilder();
                warrantyExpirationDisplayValue = new StringBuilder();

                if (!string.IsNullOrEmpty(asset.ProductVersionNumber))
                {
                    productVersionNumberDisplayName.Append(" - " + asset.ProductVersionNumber);
                }

                if (string.IsNullOrEmpty(asset.SerialNumber))
                {
                    serialNumberDisplayValue.Append("Empty");
                }
                else
                {
                    serialNumberDisplayValue.Append(asset.SerialNumber);
                }

                if (asset.WarrantyExpiration == null)
                {
                    warrantyExpirationDisplayValue.Append("Empty");
                }
                else
                {
                    warrantyExpirationDisplayValue.Append(((DateTime)asset.WarrantyExpiration).ToString(GetDateFormatNormal()));
                }

                htmlresults.AppendFormat(@"<div class='list-group-item'>
                            <a href='#' id='{0}' class='assetLink'><h4 class='list-group-item-heading'>{1}</h4></a>
                            <div class='row'>
                                <div class='col-xs-8'>
                                    <p class='list-group-item-text'>{2}{3}</p>
                                </div>
                            </div>
                            <div class='row'>
                                <div class='col-xs-8'>
                                    <p class='list-group-item-text'>SN: {4} - Warr. Exp.: {5}</p>
                                </div>
                            </div>
                            </div>
                            </div>"

                    , asset.AssetID
                    , asset.DisplayName
                    , asset.ProductName
                    , productVersionNumberDisplayName
                    , serialNumberDisplayValue
                    , warrantyExpirationDisplayValue);
            }

            return htmlresults.ToString();
        }

        [WebMethod]
        public string LoadVersionAssets(int productVersionID, int start)
        {
            StringBuilder htmlresults = new StringBuilder("");
            AssetsView assets = new AssetsView(TSAuthentication.GetLoginUser());
            assets.LoadByProductVersionIDLimit(productVersionID, start);

            StringBuilder productVersionNumberDisplayName;
            StringBuilder serialNumberDisplayValue;
            StringBuilder warrantyExpirationDisplayValue;

            foreach (AssetsViewItem asset in assets)
            {
                productVersionNumberDisplayName = new StringBuilder();
                serialNumberDisplayValue = new StringBuilder();
                warrantyExpirationDisplayValue = new StringBuilder();

                if (!string.IsNullOrEmpty(asset.ProductVersionNumber))
                {
                    productVersionNumberDisplayName.Append(" - " + asset.ProductVersionNumber);
                }

                if (string.IsNullOrEmpty(asset.SerialNumber))
                {
                    serialNumberDisplayValue.Append("Empty");
                }
                else
                {
                    serialNumberDisplayValue.Append(asset.SerialNumber);
                }

                if (asset.WarrantyExpiration == null)
                {
                    warrantyExpirationDisplayValue.Append("Empty");
                }
                else
                {
                    warrantyExpirationDisplayValue.Append(((DateTime)asset.WarrantyExpiration).ToString(GetDateFormatNormal()));
                }

                htmlresults.AppendFormat(@"<div class='list-group-item'>
                            <a href='#' id='{0}' class='assetLink'><h4 class='list-group-item-heading'>{1}</h4></a>
                            <div class='row'>
                                <div class='col-xs-8'>
                                    <p class='list-group-item-text'>{2}{3}</p>
                                </div>
                            </div>
                            <div class='row'>
                                <div class='col-xs-8'>
                                    <p class='list-group-item-text'>SN: {4} - Warr. Exp.: {5}</p>
                                </div>
                            </div>
                            </div>
                            </div>"

                    , asset.AssetID
                    , asset.DisplayName
                    , asset.ProductName
                    , productVersionNumberDisplayName
                    , serialNumberDisplayValue
                    , warrantyExpirationDisplayValue);
            }

            return htmlresults.ToString();
        }

        [WebMethod]
        public ProductVersionStatusProxy[] LoadVersionStatuses()
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            ProductVersionStatuses statuses = new ProductVersionStatuses(loginUser);
            statuses.LoadByOrganizationID(loginUser.OrganizationID);
            return statuses.GetProductVersionStatusProxies();
        }

        [WebMethod]
        public int SetProductVersionStatus(int productVersionID, int productVersionStatusID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            ProductVersion pv = ProductVersions.GetProductVersion(loginUser, productVersionID);
            pv.ProductVersionStatusID = productVersionStatusID;
            pv.Collection.Save();
            ProductVersionStatus pvs = ProductVersionStatuses.GetProductVersionStatus(loginUser, productVersionStatusID);
            string description = String.Format("{0} set company name to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, pvs == null ? "Unassigned" : pvs.Name);
            ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.ProductVersions, productVersionID, description);
            return productVersionStatusID;
        }

        [WebMethod]
        public bool SetProductVersionReleased(int productVersionID, bool value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            ProductVersion pv = ProductVersions.GetProductVersion(loginUser, productVersionID);
            pv.IsReleased = value;
            pv.Collection.Save();
            string description = String.Format("{0} set product version released to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.ProductVersions, productVersionID, description);
            return value;
        }

        [WebMethod]
        public string SetReleaseDate(int productVersionID, object value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            ProductVersion pv = ProductVersions.GetProductVersion(loginUser, productVersionID);
            StringBuilder description = new StringBuilder();
            if (pv.ReleaseDate == null)
            {
                description.Append(String.Format("Changed Released Date from \"{0}\" to \"{1}\".", "Unassigned", ((DateTime)value).ToString(GetDateFormatNormal())));
            }
            else
            {
                description.Append(String.Format("Changed Released Date from \"{0}\" to \"{1}\".", ((DateTime)pv.ReleaseDate).ToString(GetDateFormatNormal()), ((DateTime)value).ToString(GetDateFormatNormal())));
            }
            pv.ReleaseDate = (DateTime)value;
            pv.Collection.Save();
            ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.ProductVersions, productVersionID, description.ToString());
            return value.ToString() != "" ? value.ToString() : null;
        }

        [WebMethod]
        public bool SetJiraInstance(int productId, int crmLinkId, string instance)
        {
            bool result = false;
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            JiraInstanceProducts jiraInstanceProducts = new JiraInstanceProducts(loginUser);
            JiraInstanceProduct jiraInstanceProduct = null;
            string description = string.Empty;

            jiraInstanceProducts.LoadByProductAndOrganization(productId, loginUser.OrganizationID, "Jira");

            foreach (JiraInstanceProduct existing in jiraInstanceProducts.Where(p => p.CrmLinkId != crmLinkId).ToList())
            {
                jiraInstanceProducts.DeleteFromDB(existing.JiraInstanceProductsId);
            }

            jiraInstanceProduct = jiraInstanceProducts.Where(p => p.CrmLinkId == crmLinkId).FirstOrDefault();

            if (crmLinkId > 0)
            {
                if (jiraInstanceProduct == null || jiraInstanceProduct.JiraInstanceProductsId == 0)
                {
                    jiraInstanceProducts = new JiraInstanceProducts(loginUser);
                    JiraInstanceProduct newJiraInstanceProduct = jiraInstanceProducts.AddNewJiraInstanceProduct();
                    newJiraInstanceProduct.CrmLinkId = crmLinkId;
                    newJiraInstanceProduct.ProductId = productId;
                    newJiraInstanceProduct.Collection.Save();

                    if (newJiraInstanceProduct.CrmLinkId > 0 && newJiraInstanceProduct.CrmLinkId == crmLinkId)
                    {
                        result = true;
                        description = String.Format("{0} associated product to jira instance {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, instance);
                        ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Products, productId, description);
                    }
                }
                else
                {
                    result = jiraInstanceProduct.CrmLinkId > 0 && jiraInstanceProduct.CrmLinkId == crmLinkId;
                }
            }
            else
            {
                result = true;
                description = String.Format("{0} removed jira instance {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, instance);
                ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Products, productId, description);
            }

            return result;
        }
    }

    [DataContract]
    public class BasicProduct
    {
        [DataMember] public int ProductID { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public BasicVersion[] Versions { get; set; }
        [DataMember] public int? ProductFamilyID { get; set; }
    }

    [DataContract]
    public class BasicVersion
    {
        [DataMember] public int ProductVersionID { get; set; }
        [DataMember] public string VersionNumber { get; set; }
    }

    [DataContract]
    public class BasicVersionStatus
    {
        [DataMember]
        public int ProductVersionStatusID { get; set; }
        [DataMember]
        public string Name { get; set; }
    }

    public class NewProductSave
    {
        public NewProductSave() { }
        [DataMember] public string Name { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] public int? ProductFamilyID { get; set; }
        [DataMember] public string JiraProjectKey { get; set; }
        [DataMember] public List<CustomFieldSaveInfo> Fields { get; set; }
    }

    public class NewProductVersionSave
    {
        public NewProductVersionSave() { }
        [DataMember] public int ProductID { get; set; }
        [DataMember] public int ProductVersionStatusID { get; set; }
        [DataMember] public string VersionNumber { get; set; }
        [DataMember] public DateTime? ReleaseDate { get; set; }
        [DataMember] public bool IsReleased { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] public string JiraProjectKey { get; set; }
        [DataMember] public List<CustomFieldSaveInfo> Fields { get; set; }
    }

    public class ProductCustomOrganization
    {
        [DataMember]
        public string Customer { get; set; }
        [DataMember]
        public string VersionNumber { get; set; }
        [DataMember]
        public string SupportExpiration { get; set; }
        [DataMember]
        public string VersionStatus { get; set; }
        [DataMember]
        public string IsReleased { get; set; }
        [DataMember]
        public string ReleaseDate { get; set; }
        [DataMember]
        public string DateCreated { get; set; }
        [DataMember]
        public int ProductID { get; set; }
        [DataMember]
        public int OrganizationID { get; set; }
        [DataMember]
        public int OrganizationProductID { get; set; }
        [DataMember]
        public List<string> CustomFields { get; set; }

    }

    public class NewProductFamilySave
    {
        public NewProductFamilySave() { }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public List<CustomFieldSaveInfo> Fields { get; set; }
    }

    public class ProdProp
    {
        [DataMember]
        public ProductProxy prodproxy { get; set; }
        [DataMember]
        public string ProductFamily { get; set; }
        [DataMember]
        public string JiraProjectKey { get; set; }
        [DataMember]
        public string JiraInstance { get; set; }
        [DataMember]
        public int CrmLinkId { get; set; }
        [DataMember]
        public string SlaAssigned { get; set; }
        [DataMember]
        public string TFSProjectName { get; set; }
    }

    [DataContract]
    public class FamilyProduct
    {
        [DataMember] public bool MoreAvailable { get; set; }
        [DataMember] public string HTML { get; set; }
    }
}
