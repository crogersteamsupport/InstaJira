using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.WebUtils;
using TeamSupport.Data;
using System.Web.Services;
using Telerik.Web.UI;

public partial class Frames_AdminEmails : System.Web.UI.Page
{
    [Serializable]
    public class Template
    {
        public int EmailTemplateID { get; set; }
        public string Subject { get; set; }
        public bool IsHtml { get; set; }
        public bool IsEmail { get; set; }
        public bool UseTemplate { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public string Body { get; set; }
        public string Description { get; set; }
    }


    [Serializable]
    public class PlaceHolders
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public PlaceHolder[] Items { get; set; }
    }

    [Serializable]
    public class PlaceHolder
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    [Serializable]
    public class AltEmail
    {
        public int? ProductID { get; set; }
        public int? GroupID { get; set; }
        public int? TicketTypeID { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string Product { get; set; }
        public string Group { get; set; }
        public string TicketType { get; set; }
        public string SendingEmailAddress { get; set; }

    }





    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Organization organization = Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
            if (organization.UseProductFamilies)
            {
                ProductFamilies productFamilies = new ProductFamilies(UserSession.LoginUser);
                productFamilies.LoadByOrganizationID(organization.OrganizationID);
                cmbProductFamily.Items.Add(new Telerik.Web.UI.RadComboBoxItem("<Without Product Line>", "-1"));
                foreach (ProductFamily productFamily in productFamilies)
                {
                    cmbProductFamily.Items.Add(new Telerik.Web.UI.RadComboBoxItem(productFamily.Name, productFamily.ProductFamilyID.ToString()));
                }
                cmbProductFamily.Visible = true;
            }

            LoadGroupTypes(organization.OrganizationID);
            cmbDefaultGroup.SelectedValue = organization.DefaultPortalGroupID.ToString();

            EmailTemplates templates = new EmailTemplates(UserSession.LoginUser);

            templates.LoadAll((UserSession.LoginUser.OrganizationID == 1078) && UserSession.CurrentUser.IsSystemAdmin, organization.ProductType);

            foreach (EmailTemplate template in templates)
            {
                cmbTemplate.Items.Add(new Telerik.Web.UI.RadComboBoxItem(template.Name, template.EmailTemplateID.ToString()));
            }

            LoadEAICombos();
        }
    }

    private void LoadGroupTypes(int organizationID)
    {
        cmbDefaultGroup.Items.Clear();

        Groups groups = new Groups(UserSession.LoginUser);
        groups.LoadByOrganizationID(organizationID);
        cmbDefaultGroup.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
        foreach (Group group in groups)
        {
            cmbDefaultGroup.Items.Add(new RadComboBoxItem(group.Name, group.GroupID.ToString()));
        }

        cmbDefaultGroup.SelectedIndex = 0;
    }


    private void LoadEAICombos()
    {
        RadComboBox combo = wndAltEmail.ContentContainer.FindControl("cmbEAIGroup") as RadComboBox;
        Groups groups = new Groups(UserSession.LoginUser);
        groups.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
        combo.Items.Add(new Telerik.Web.UI.RadComboBoxItem("Unassigned", "-1"));
        foreach (Group group in groups)
            combo.Items.Add(new Telerik.Web.UI.RadComboBoxItem(group.Name, group.GroupID.ToString()));
        combo.SelectedIndex = 0;

        combo = wndAltEmail.ContentContainer.FindControl("cmbEAITicket") as RadComboBox;
        TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);
        ticketTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);
        combo.Items.Add(new Telerik.Web.UI.RadComboBoxItem("Unassigned", "-1"));
        foreach (TicketType ticketType in ticketTypes)
            combo.Items.Add(new Telerik.Web.UI.RadComboBoxItem(ticketType.Name, ticketType.TicketTypeID.ToString()));
        combo.SelectedIndex = 0;

        combo = wndAltEmail.ContentContainer.FindControl("cmbEAIProduct") as RadComboBox;
        Products products = new Products(UserSession.LoginUser);
        products.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
        combo.Items.Add(new Telerik.Web.UI.RadComboBoxItem("Unassigned", "-1"));
        foreach (Product product in products)
            combo.Items.Add(new Telerik.Web.UI.RadComboBoxItem(product.Name, product.ProductID.ToString()));
        combo.SelectedIndex = 0;
    }

    [WebMethod]
    public static OrganizationProxy GetOrganization()
    {
        Organization organization = Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
        OrganizationProxy result = organization.GetProxy();
        return result;
    }

    [WebMethod]
    public static AltEmail[] GetAltEmails()
    {
        EMailAlternateInbound items = new EMailAlternateInbound(UserSession.LoginUser);
        items.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
        List<AltEmail> result = new List<AltEmail>();
        foreach (EMailAlternateInboundItem item in items)
        {
            AltEmail altEmail = new AltEmail();
            altEmail.Email = item.SystemEMailID.ToString();
            altEmail.Description = item.Description ?? "";
            altEmail.GroupID = item.GroupToAssign == null ? -1 : item.GroupToAssign;
            altEmail.TicketTypeID = item.DefaultTicketType == null ? -1 : item.DefaultTicketType;
            altEmail.ProductID = item.ProductID == null ? -1 : item.ProductID;
            altEmail.Product = item.Row["ProductName"].ToString();
            altEmail.Group = item.Row["GroupName"].ToString();
            altEmail.TicketType = item.Row["TicketTypeName"].ToString();
            altEmail.SendingEmailAddress = item.Row["SendingEMailAddress"].ToString();
            result.Add(altEmail);
        }
        return result.ToArray();
    }

    [WebMethod]
    public static AltEmail GetAltEmail(Guid id)
    {
        EMailAlternateInboundItem item = EMailAlternateInbound.GetItem(UserSession.LoginUser, id);
        AltEmail result = new AltEmail();
        result.Email = item.SystemEMailID.ToString();
        result.Description = item.Description ?? "";
        result.GroupID = item.GroupToAssign == null ? -1 : item.GroupToAssign;
        result.TicketTypeID = item.DefaultTicketType == null ? -1 : item.DefaultTicketType;
        result.ProductID = item.ProductID == null ? -1 : item.ProductID;
        result.Product = item.Row["ProductName"].ToString();
        result.Group = item.Row["GroupName"].ToString();
        result.TicketType = item.Row["TicketTypeName"].ToString();
        result.SendingEmailAddress = item.Row["SendingEMailAddress"].ToString();
        return result;
    }

    [WebMethod]
    public static void SaveAltEmail(string id, string description, int? groupID, int? ticketTypeID, int? productID, string sendingEmailAddress)
    {
        if (!UserSession.CurrentUser.IsSystemAdmin) return;
        EMailAlternateInboundItem item = id != null ? EMailAlternateInbound.GetItem(UserSession.LoginUser, new Guid(id)) : (new EMailAlternateInbound(UserSession.LoginUser)).AddNewEMailAlternateInboundItem();
        item.OrganizationID = UserSession.LoginUser.OrganizationID;
        item.Description = description;
        item.GroupToAssign = groupID < 0 ? null : groupID;
        item.DefaultTicketType = ticketTypeID < 0 ? null : ticketTypeID;
        item.ProductID = productID < 0 ? null : productID;
        item.SendingEMailAddress = sendingEmailAddress;
        item.Collection.Save();
    }

    [WebMethod]
    public static void DeleteAltEmail(string id)
    {
        if (!UserSession.CurrentUser.IsSystemAdmin) return;
        EMailAlternateInbound.DeleteFromDB(UserSession.LoginUser, new Guid(id));
    }

    [WebMethod]
    public static void SaveEmailSettings(string reply, bool reqNew, bool reqKnown, bool changeStatus, bool addContacts, bool matchSubject, bool markSpam, bool forceBccPrivate, bool needCustForTicketMatch, bool replyToAlternateEmailAddresses, bool addEmailViaTS, int? defaultPortalGroupID)
    {
        if (!UserSession.CurrentUser.IsSystemAdmin) return;

        Organization organization = Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
        organization.OrganizationReplyToAddress = reply.Trim();
        organization.RequireNewKeyword = reqNew;
        organization.RequireKnownUserForNewEmail = reqKnown;
        organization.ChangeStatusIfClosed = changeStatus;
        organization.AddAdditionalContacts = addContacts;
        organization.MatchEmailSubject = matchSubject;
        organization.MarkSpam = markSpam;
        organization.ForceBCCEmailsPrivate = forceBccPrivate;
        organization.NeedCustForTicketMatch = needCustForTicketMatch;
        organization.ReplyToAlternateEmailAddresses = replyToAlternateEmailAddresses;
        organization.AddEmailViaTS = addEmailViaTS;
        organization.DefaultPortalGroupID = defaultPortalGroupID < 0 ? null : defaultPortalGroupID;

        organization.Collection.Save();
    }

    [WebMethod]
    public static Template GetEmailTemplate(int emailTemplateID, int productFamilyID)
    {
        Template result = new Template();

        EmailTemplate template = EmailTemplates.GetEmailTemplate(UserSession.LoginUser, emailTemplateID);
        OrganizationEmails emails = new OrganizationEmails(UserSession.LoginUser);
        emails.LoadByTemplateAndProductFamily(UserSession.LoginUser.OrganizationID, emailTemplateID, productFamilyID);

        result.EmailTemplateID = template.EmailTemplateID;
        result.Description = template.Description;
        result.Body = template.Body;
        result.IsHtml = template.IsHtml;
        result.IsEmail = template.IsEmail;
        result.Header = template.Header;
        result.UseTemplate = template.UseGlobalTemplate && template.IsEmail;
        result.Footer = template.Footer;
        result.Subject = template.Subject;

        if (!emails.IsEmpty && UserSession.LoginUser.UserID != -34)
        {
            result.Body = emails[0].Body;
            result.IsHtml = emails[0].IsHtml;
            result.Subject = emails[0].Subject;
            result.Header = emails[0].Header;
            result.Footer = emails[0].Footer;
            result.UseTemplate = emails[0].UseGlobalTemplate && template.IsEmail;
        }

        return result;
    }

    [WebMethod]
    public static string GetBuiltBody(string header, string footer, string body)
    {
        EmailTemplate template = EmailTemplate.GetGlobalTemplate(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
        template.ReplaceParameter("Header", header).ReplaceParameter("Footer", footer).ReplaceParameter("Body", body);
        return template.Body;
    }


    [WebMethod]
    public static void DeleteTemplate(int emailTemplateID, int productFamilyID)
    {
        if (!UserSession.CurrentUser.IsSystemAdmin) return;
        OrganizationEmails emails = new OrganizationEmails(UserSession.LoginUser);
        emails.LoadByTemplateAndProductFamilyForDelete(UserSession.LoginUser.OrganizationID, emailTemplateID, productFamilyID);
        if (!emails.IsEmpty && emails[0].OrganizationID == UserSession.LoginUser.OrganizationID)
        {
            emails[0].Delete();
            emails.Save();
        }


    }

    [WebMethod]
    public static void SaveEmailTemplate(int emailTemplateID, string subject, string header, string footer, string body, bool isHtml, bool useTemplate, int productFamilyID)
    {
        if (!UserSession.CurrentUser.IsSystemAdmin) return;

        EmailTemplate template = EmailTemplates.GetEmailTemplate(UserSession.LoginUser, emailTemplateID);
        subject = template.IsEmail ? subject : "";

        if (UserSession.LoginUser.UserID == -34)
        {
            template.Body = body;
            template.EmailTemplateID = emailTemplateID;
            template.IsHtml = isHtml;
            template.Subject = subject;
            template.Header = header;
            template.Footer = footer;
            template.UseGlobalTemplate = useTemplate && template.IsEmail;
            template.Collection.Save();
        }
        else
        {
            OrganizationEmails emails = new OrganizationEmails(UserSession.LoginUser);
            emails.LoadByTemplateAndProductFamily(UserSession.LoginUser.OrganizationID, emailTemplateID, productFamilyID);
            OrganizationEmail email = emails.IsEmpty || (emails[0].ProductFamilyID == null && productFamilyID != -1) ? (new OrganizationEmails(UserSession.LoginUser)).AddNewOrganizationEmail() : emails[0];

            email.Body = body;
            email.EmailTemplateID = emailTemplateID;
            email.IsHtml = isHtml;
            email.OrganizationID = UserSession.LoginUser.OrganizationID;
            email.Subject = subject;
            email.Header = header;
            email.Footer = footer;
            email.UseGlobalTemplate = useTemplate && template.IsEmail;
            if (productFamilyID == -1)
            {
                email.ProductFamilyID = null;
            }
            else
            {
                email.ProductFamilyID = productFamilyID;
            }
            email.Collection.Save();
        }
    }

    [WebMethod]
    public static PlaceHolders[] GetPlaceHolders(int emailTemplateID)
    {
        EmailTemplate template = EmailTemplates.GetEmailTemplate(UserSession.LoginUser, emailTemplateID);
        List<PlaceHolders> result = new List<PlaceHolders>();


        EmailTemplateParameters paramaters = new EmailTemplateParameters(UserSession.LoginUser);
        paramaters.LoadByTemplate(emailTemplateID);

        List<PlaceHolder> list = new List<PlaceHolder>();
        PlaceHolders phs = new PlaceHolders();
        if (!paramaters.IsEmpty)
        {
            phs.Name = "Miscellaneous";
            phs.Description = "";
            foreach (EmailTemplateParameter parameter in paramaters)
            {
                PlaceHolder ph = new PlaceHolder();
                ph.Name = parameter.Name;
                ph.Description = parameter.Description ?? "";
                list.Add(ph);
            }


            PlaceHolder phTo = new PlaceHolder();
            phTo.Name = "ToEmailAddress";
            phTo.Description = "This adds the recipient's email address.";
            list.Add(phTo);

            phTo = new PlaceHolder();
            phTo.Name = "ToFirstName";
            phTo.Description = "This adds the recipient's first name.";
            list.Add(phTo);

            phTo = new PlaceHolder();
            phTo.Name = "ToLastName";
            phTo.Description = "This adds the recipient's last name.";
            list.Add(phTo);

            phs.Items = list.ToArray();
            result.Add(phs);
        }

        EmailTemplateTables tables = new EmailTemplateTables(UserSession.LoginUser);
        tables.LoadByTemplate(emailTemplateID);

        foreach (EmailTemplateTable table in tables)
        {
            phs = new PlaceHolders();
            phs.Name = table.Alias;
            phs.Description = table.Description ?? "";
            list.Clear();

            ReportTable reportTable = ReportTables.GetReportTable(UserSession.LoginUser, table.ReportTableID);

            ReportTableFields fields = new ReportTableFields(UserSession.LoginUser);
            fields.LoadByReportTableID(table.ReportTableID);

            foreach (ReportTableField field in fields)
            {
                PlaceHolder ph = new PlaceHolder();
                ph.Name = table.Alias + '.' + field.FieldName;
                ph.Description = field.Description ?? "";
                list.Add(ph);
            }

            //do custom fields here

            ReferenceType refType = reportTable.CustomFieldRefType;

            if (refType != ReferenceType.None)
            {
                //TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);
                //ticketTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);

                CustomFields customFields = new CustomFields(UserSession.LoginUser);
                customFields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, refType, null, "Name");

                foreach (CustomField customField in customFields)
                {
                    string customFieldName = table.Alias + "." + customField.Name;
                    bool flag = false;

                    foreach (PlaceHolder existingPH in list)
                    {
                        if (existingPH.Name == customFieldName)
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (flag) continue;

                    PlaceHolder ph = new PlaceHolder();
                    ph.Name = customFieldName;
                    ph.Description = "";
                    list.Add(ph);

                    /*
                    if (customField.RefType == ReferenceType.Tickets || customField.RefType == ReferenceType.Actions)
                    {
                        TicketType ticketType = ticketTypes.FindByTicketTypeID(customField.AuxID);
                        if (ticketType != null)
                        {
                            //
                        }
                    }
                    else
                    {
                    }*/

                }
            }

            phs.Items = list.ToArray();
            result.Add(phs);

        }

        phs = new PlaceHolders();
        phs.Name = "MyCompany";
        phs.Description = "Your company's information";
        list.Clear();

        ReportTableFields companyFields = new ReportTableFields(UserSession.LoginUser);
        companyFields.LoadByReportTableID(6);

        foreach (ReportTableField field in companyFields)
        {
            PlaceHolder ph = new PlaceHolder();
            ph.Name = "MyCompany." + field.FieldName;
            ph.Description = field.Description ?? "";
            list.Add(ph);
        }


        phs.Items = list.ToArray();
        result.Add(phs);

        return result.ToArray();
    }

}
