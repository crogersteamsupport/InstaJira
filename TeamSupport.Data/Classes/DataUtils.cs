using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Globalization;
using System.Dynamic;
using Newtonsoft.Json;

namespace TeamSupport.Data
{
    public class DataUtils
    {

        private static string[] WordBreakString(string text)
        {
            List<string> list = new List<string>();
            text = text.Trim();
            char? qoute = null;
            StringBuilder builder = new StringBuilder();

            foreach (char c in text)
            {

                if (qoute == null)
                {
                    if (c == ',' || c == ';') continue;

                    if (c == ')' || c == '(')
                    {
                        if (builder.Length > 0)
                        {
                            list.Add(builder.ToString().Trim());
                            builder.Remove(0, builder.Length);
                        }
                        list.Add(c.ToString());
                    }
                    else if (c == '"' || c == '\'')
                    {
                        qoute = c;
                        if (builder.Length > 0)
                        {
                            list.Add(builder.ToString().Trim());
                            builder.Remove(0, builder.Length);
                        }
                    }
                    else if (c == ' ')
                    {
                        if (builder.Length > 0)
                        {
                            list.Add(builder.ToString().Trim());
                            builder.Remove(0, builder.Length);
                        }
                    }
                    else
                    {
                        builder.Append(c);
                    }
                }
                else
                {
                    if (c == qoute)
                    {
                        list.Add('"' + builder.ToString().Trim() + '"');
                        builder.Remove(0, builder.Length);
                        qoute = null;
                    }
                    else
                    {
                        builder.Append(c);
                    }
                }
            }

            if (builder.Length > 0) list.Add(builder.ToString().Trim());

            return list.ToArray();
        }

        public static string BuildSearchString(string text, bool matchAll)
        {
            return BuildSearchString(text, matchAll, true);
        }

        public static string BuildSearchString(string text, bool matchAll, bool appendWildcard)
        {
            text = text.Replace("%20", " ");
            text = text.Replace("'", "\"");
            text = text.Replace(")", " ").Replace("(", " ");
            bool hasParenth = text.IndexOf("(") > -1;

            int quoteCount = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '"') quoteCount++;
            }

            if (quoteCount % 2 == 1) text = text + "\"";



            List<string> list = new List<string>();
            list.AddRange(WordBreakString(text));
            string con;

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == "(" || list[i] == ")")
                {
                    builder.Append(list[i]);
                    i++;
                    if (i >= list.Count) return builder.ToString();
                }

                con = matchAll ? "AND" : "OR"; ;
                string conTest = list[i].ToUpper();

                if (conTest == "OR" || conTest == "AND")
                {
                    con = conTest;
                    i++;

                    if (con == "AND" && list[i].ToUpper() == "NOT")
                    {
                        con = "AND NOT";
                        i++;
                    }
                }

                string term = list[i];
                if (appendWildcard && term.IndexOf('%') < 0 && term.IndexOf('"') < 0) term = term + '*';
                if (i != 0)
                {
                    string parenthTest = builder.ToString();
                    if ((parenthTest.Length > 0 && parenthTest[parenthTest.Length - 1] != '('))
                    {
                        builder.Append(" " + con + " ");
                    }

                    builder.Append(QuoteSearchTerm(term));
                }
                else
                {
                    builder.Append(QuoteSearchTerm(term));
                }
            }

            return builder.ToString();
        }

        private static string QuoteSearchTerm(string text)
        {
            if (text.Length > 0 && text[0] != '"')
            {
                // if (text.IndexOf('*') > -1) return "\"" + text + "\"";
            }
            return text;
        }

        public static string EllipseString(string text, int maxChars, string last = "...")
        {
            return text.Length <= maxChars ? text : text.Substring(0, maxChars + last.Length) + last;
        }

        public static string ProductTypeString(ProductType type)
        {
            string result = "";
            switch (type)
            {
                case ProductType.Express:
                    result = "Express";
                    break;
                case ProductType.HelpDesk:
                    result = "Support Desk";
                    break;
                case ProductType.Enterprise:
                    result = "Enterprise";
                    break;
                case ProductType.BugTracking:
                    result = "Bug Tracking";
                    break;
                default:
                    break;
            }
            return result;
        }

        public static string CustomFieldTypeString(CustomFieldType type)
        {
            string result = "";
            switch (type)
            {
                case CustomFieldType.Text: result = "Text"; break;
                case CustomFieldType.DateTime: result = "Date Time"; break;
                case CustomFieldType.Boolean: result = "True or False"; break;
                case CustomFieldType.Number: result = "Number"; break;
                case CustomFieldType.PickList: result = "Pick List"; break;
                case CustomFieldType.Date: result = "Date"; break;
                case CustomFieldType.Time: result = "Time"; break;
                default: break;
            }
            return result;
        }

        public static string GetCustomFieldColumns(LoginUser loginUser, ReferenceType refType, int? auxID, int organizationID, string refIDFieldName, int max, bool allowSpaces)
        {
            CustomFields fields = new CustomFields(loginUser);
            fields.LoadByReferenceType(organizationID, refType, auxID);
            int count = 0;
            StringBuilder builder = new StringBuilder();

            foreach (CustomField field in fields)
            {

                if (count >= max) break;
                builder.Append(", ");
                builder.Append(GetCustomFieldColumn(loginUser, field, refIDFieldName, allowSpaces));
                //,(SELECT CustomValue FROM CustomValues WHERE CustomFieldID = 246 AND RefID = op.OrganizationProductID) AS [Serial Number],

                /*builder.Append(", (SELECT CustomValue FROM CustomValues WHERE (CustomFieldID = ");
                builder.Append(field.CustomFieldID.ToString());
                builder.Append(") AND (RefID = ");
                builder.Append(refIDFieldName);
                builder.Append(")) AS [");
                if (allowSpaces)
                  builder.Append(field.Name);
                else
                  builder.Append(field.Name.Replace(' ', '_'));
                builder.Append("]");*/

                count++;
            }

            return builder.ToString();
        }

        public static string GetCustomFieldColumns(LoginUser loginUser, ReferenceType refType, int organizationID, string refIDFieldName, int max, bool allowSpaces)
        {
            return GetCustomFieldColumns(loginUser, refType, -1, organizationID, refIDFieldName, max, allowSpaces);
        }

        public static string GetCustomFieldColumn(LoginUser loginUser, CustomField field, string refIDFieldName, bool allowSpaces, bool writeAlias)
        {
            StringBuilder builder = new StringBuilder();
            //builder.Append("(SELECT CAST(NULLIF(RTRIM(CustomValue), '') AS varchar(8000)");
            builder.Append("(SELECT ");

            //MT:  Removed 12/10/15  Was causing issues loading reports with custom fields in the select that have bad values in the database
            //switch (field.FieldType)
            //{
            //  case CustomFieldType.Date:
            //  case CustomFieldType.Time:
            //  case CustomFieldType.DateTime:
            //    builder.Append("datetime");
            //    break;
            //  case CustomFieldType.Boolean:
            //    builder.Append("bit");
            //    break;
            //  case CustomFieldType.Number:
            //    builder.Append("decimal");
            //    break;
            //  default:
            //    builder.Append("varchar(8000)");
            //    break;
            //}

            //MT: Replaced above by adding in NULLIF checks on the datatype to ensure SQL queries dont crash from changed CF types. 
            switch (field.FieldType)
            {
                case CustomFieldType.Date:
                case CustomFieldType.Time:
                case CustomFieldType.DateTime:
                    builder.Append("(CASE WHEN ISDATE (CustomValue) = 1 THEN (SELECT CAST(RTRIM(CustomValue) AS Datetime)) END"); //good
                    break;
                case CustomFieldType.Boolean:
                    builder.Append("CAST(ISNULL(NULLIF(RTRIM(CustomValue), ''), 'False') AS BIT");
                    break;
                case CustomFieldType.Number:
                    builder.Append("TRY_CAST(NULLIF(RTRIM(CustomValue), '') AS decimal");
                    break;
                default:
                    builder.Append("CAST(NULLIF(RTRIM(CustomValue), '') AS varchar(1000)");
                    break;
            }

            builder.Append(") FROM CustomValues WHERE (CustomFieldID = ");
            builder.Append(field.CustomFieldID.ToString());
            builder.Append(") AND (RefID = ");
            builder.Append(refIDFieldName);
            builder.Append(")) ");


            //"(SELECT CustomValue FROM CustomValues WHERE (CustomFieldID = 654) AND (RefID = TicketsView.TicketID)) AS [Approved By Manager]"
            //MT:  Removed on 7/13/15 because it is causing issues in tickets with select custom fields since its showing the first item in the list as the default. 
            //if (field.FieldType == CustomFieldType.PickList)
            //{
            //string[] items = field.ListValues.Split('|');
            //if (items.Length > 0)
            //{
            //  builder.Insert(0, "ISNULL(");
            //  builder.Append(", '" + items[0].Replace("'", "''") + "')");
            //}
            //}

            if (writeAlias)
            {
                builder.Append("AS [");

                StringBuilder alias = new StringBuilder(field.Name);

                if (field.AuxID > 0 && field.RefType == ReferenceType.Tickets)
                {
                    TicketType ticketType = TicketTypes.GetTicketType(loginUser, field.AuxID);
                    if (ticketType != null && ticketType.OrganizationID == field.OrganizationID)
                    {
                        alias.Append(" (" + ticketType.Name + ")");
                    }

                }

                if (allowSpaces)
                    builder.Append(alias.ToString());
                else
                    builder.Append(alias.ToString().Replace(' ', '_'));
                builder.Append("]");
            }
            return builder.ToString();
        }

        public static string GetCustomFieldColumn(LoginUser loginUser, CustomField field, string refIDFieldName, bool allowSpaces)
        {
            return GetCustomFieldColumn(loginUser, field, refIDFieldName, allowSpaces, true);
        }

        public static string GetReportPrimaryKeyFieldName(ReferenceType refType)
        {
            string result = "";
            switch (refType)
            {
                case ReferenceType.Organizations:
                    result = "OrganizationsView.OrganizationID";
                    break;
                case ReferenceType.Products:
                    result = "Products.ProductID";
                    break;
                case ReferenceType.ProductVersions:
                    result = "ProductVersionsView.ProductVersionID";
                    break;
                case ReferenceType.Users:
                    result = "UsersView.UserID";
                    break;
                case ReferenceType.Contacts:
                    result = "ContactsView.UserID";
                    break;
                case ReferenceType.Tickets:
                    result = "ReportTicketsView.TicketID";
                    break;
                case ReferenceType.OrganizationProducts:
                    result = "OrganizationProductsView.OrganizationProductID";
                    break;
                case ReferenceType.Assets:
                    result = "AssetsView.AssetID";
                    break;
                case ReferenceType.ProductFamilies:
                    result = "ProductFamilies.ProductFamilyID";
                    break;
                default:
                    result = "";
                    break;
            }
            return result;

        }

        public static string FixStringFormat(string text)
        {
            return text.Replace("{", "{{").Replace("}", "}}");
        }

        public static DateTime DateToLocal(LoginUser loginUser, DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, loginUser.TimeZoneInfo);
        }

        public static DateTime? DateToLocal(LoginUser loginUser, DateTime? dateTime)
        {
            if (dateTime == null)
                return null;
            else
            {
                return DateToLocal(loginUser, (DateTime)dateTime);
            }
        }

        public static DateTime DateToUtc(LoginUser loginUser, DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, loginUser.TimeZoneInfo);
        }

        public static DateTime? DateToUtc(LoginUser loginUser, DateTime? dateTime)
        {
            if (dateTime == null)
                return null;
            else
            {
                return DateToUtc(loginUser, (DateTime)dateTime);
            }
        }

        public static int GetQuarter(DateTime dateTime)
        {
            return (dateTime.Month - 1) / 3 + 1;
        }

        public static ReferenceType TableNameToReferenceType(string tableName)
        {
            ReferenceType result = ReferenceType.None;
            switch (tableName.ToLower().Trim())
            {
                case "actionlogs": result = ReferenceType.ActionLogs; break;
                case "actions": result = ReferenceType.Actions; break;
                case "actiontypes": result = ReferenceType.ActionTypes; break;
                case "addresses": result = ReferenceType.Addresses; break;
                case "attachments": result = ReferenceType.Attachments; break;
                case "billinginfo": result = ReferenceType.BillingInfo; break;
                case "creditcards": result = ReferenceType.CreditCards; break;
                case "crmlinkresults": result = ReferenceType.None; break;
                case "crmlinktable": result = ReferenceType.None; break;
                case "customfields": result = ReferenceType.CustomFields; break;
                case "customvalues": result = ReferenceType.CustomValues; break;
                case "emailactiontable": result = ReferenceType.None; break;
                case "emailposts": result = ReferenceType.None; break;
                case "exceptionlogs": result = ReferenceType.ExceptionLogs; break;
                case "groups": result = ReferenceType.Groups; break;
                case "groupusers": result = ReferenceType.GroupUsers; break;
                case "invoices": result = ReferenceType.Invoices; break;
                case "loginhistory": result = ReferenceType.None; break;
                case "notes": result = ReferenceType.None; break;
                case "organizationproducts": result = ReferenceType.OrganizationProducts; break;
                case "organizations": result = ReferenceType.Organizations; break;
                case "organizationsettings": result = ReferenceType.None; break;
                case "organizationtickets": result = ReferenceType.OrganizationTickets; break;
                case "phonenumbers": result = ReferenceType.PhoneNumbers; break;
                case "phonetypes": result = ReferenceType.PhoneTypes; break;
                case "portalloginhistory": result = ReferenceType.None; break;
                case "portaloptions": result = ReferenceType.None; break;
                case "products": result = ReferenceType.Products; break;
                case "productversions": result = ReferenceType.ProductVersions; break;
                case "productversionstatuses": result = ReferenceType.ProductVersionStatuses; break;
                case "reportdata": result = ReferenceType.None; break;
                case "reportfields": result = ReferenceType.None; break;
                case "reports": result = ReferenceType.None; break;
                case "reportsubcategories": result = ReferenceType.None; break;
                case "reporttablefields": result = ReferenceType.None; break;
                case "reporttables": result = ReferenceType.None; break;
                case "sourcecommitlog": result = ReferenceType.None; break;
                case "subscriptions": result = ReferenceType.Subscriptions; break;
                case "systemsettings": result = ReferenceType.SystemSettings; break;
                case "techdocs": result = ReferenceType.TechDocs; break;
                case "ticketnextstatuses": result = ReferenceType.TicketNextStatuses; break;
                case "ticketqueue": result = ReferenceType.TicketQueue; break;
                case "tickets": result = ReferenceType.Tickets; break;
                case "ticketseverities": result = ReferenceType.TicketSeverities; break;
                case "ticketstatuses": result = ReferenceType.TicketStatuses; break;
                case "tickettypes": result = ReferenceType.TicketTypes; break;
                case "tsemailignorelist": result = ReferenceType.None; break;
                case "users": result = ReferenceType.Users; break;
                case "usersettings": result = ReferenceType.UserSettings; break;
                case "actionsview": result = ReferenceType.Actions; break;
                case "contactsview": result = ReferenceType.Contacts; break;
                case "exceptionlogview": result = ReferenceType.ExceptionLogs; break;
                case "notesview": result = ReferenceType.None; break;
                case "organizationsview": result = ReferenceType.Organizations; break;
                case "phonenumbersview": result = ReferenceType.PhoneNumbers; break;
                case "productversionsview": result = ReferenceType.ProductVersions; break;
                case "ticketgridview": result = ReferenceType.Tickets; break;
                case "ticketsview": result = ReferenceType.Tickets; break;
                case "usersview": result = ReferenceType.Users; break;
                case "assetsview":
                case "assets":
                    result = ReferenceType.Assets;
                    break;

                default:
                    result = ReferenceType.None;
                    break;
            }
            return result;
        }

        public static int GetIDFromXml(string data, string fieldName)
        {
            StringReader reader = new StringReader(data);
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(reader);
            return (int)dataSet.Tables[0].Rows[0][fieldName];
        }

        public static bool IsReferenceValid(LoginUser loginUser, ReferenceType refType, int refID)
        {
            bool result = false;

            if (refType == ReferenceType.Organizations)
            {
                result = Organizations.GetOrganization(loginUser, refID).ParentID == loginUser.OrganizationID;
            }
            else if (refType == ReferenceType.Contacts || refType == ReferenceType.Users)
            {
                result = Users.GetUser(loginUser, refID).OrganizationID == loginUser.OrganizationID;
                if (!result) result = Organizations.GetOrganization(loginUser, Users.GetUser(loginUser, refID).OrganizationID).ParentID == loginUser.OrganizationID;
            }
            else if (refType == ReferenceType.Tickets)
            {
                result = Users.GetUser(loginUser, refID).OrganizationID == loginUser.OrganizationID;
                if (!result) result = Organizations.GetOrganization(loginUser, Users.GetUser(loginUser, refID).OrganizationID).ParentID == loginUser.OrganizationID;
            }
            return result;
        }

        public static string StripHtml(string text)
        {
            return text;
            /*
            string pattern = @"</?(?i:script|embed|object|frameset|frame|iframe|meta|link|style|body|html|div|style)(.|\n)*?>";//  @"<(.|\n)*?>";
            string result = text;

            result = Regex.Replace(result, pattern, string.Empty);
            //result = result.Replace("<", "&lt;").Replace(">", "&gt;");
            //result = result.Replace("&nbsp;", " ").Replace("&nbsp", " ");
            //result = HttpUtility.HtmlEncode(result);
            return result;*/
        }

        public static string GetBrowserName(string userAgent)
        {
            string browser = "Unknown";
            userAgent = userAgent.ToLower();

            if (userAgent.IndexOf("opera") > -1)
                browser = "Opera";
            else if (userAgent.IndexOf("konqueror") > -1)
                browser = "Konqueror";
            else if (userAgent.IndexOf("firefox") > -1)
                browser = "Firefox";
            else if (userAgent.IndexOf("netscape") > -1)
                browser = "Netscape";
            else if (userAgent.IndexOf("msie") > -1)
                browser = "Internet Explorer";
            else if (userAgent.IndexOf("chrome") > -1)
                browser = "Chrome";
            else if (userAgent.IndexOf("safari") > -1)
                browser = "Safari";

            return browser;
        }

        public static string GetCommandTextSql(SqlCommand command)
        {
            StringBuilder builder = new StringBuilder();
            foreach (SqlParameter param in command.Parameters)
            {
                try
                {
                    string format = "";
                    switch (param.SqlDbType)
                    {
                        case SqlDbType.BigInt:
                        case SqlDbType.Binary:
                        case SqlDbType.Bit:
                        case SqlDbType.Decimal:
                        case SqlDbType.Float:
                        case SqlDbType.Int:
                        case SqlDbType.Real:
                        case SqlDbType.SmallInt:
                        case SqlDbType.TinyInt:
                            format = "DECLARE @{0} {1}; SET @{0} = {2};";
                            break;
                        default:
                            format = "DECLARE @{0} {1}; SET @{0} = '{2}';";
                            break;
                    }
                    builder.AppendLine(string.Format(format, param.ParameterName.Replace("@", ""), param.SqlDbType.ToString(), param.Value.ToString()));
                }
                catch (Exception)
                {
                    builder.AppendLine("Error setting param");
                }
            }
            builder.AppendLine(command.CommandText);

            return builder.ToString();
        }

        public static ExpandoObject[] DataTableToExpandoObject(DataTable table)
        {
            List<ExpandoObject> result = new List<ExpandoObject>();

            foreach (DataRow row in table.Rows)
            {
                var expando = new ExpandoObject();
                foreach (DataColumn column in table.Columns)
                {
                    var dict = expando as IDictionary<String, object>;
                    dict.Add(column.ColumnName, row[column] == DBNull.Value ? null : row[column]);
                }
                result.Add(expando);
            }

            return result.ToArray();
        }

        public static ExpandoObject DataRowToExpandoObject(DataRow row)
        {
            var expando = new ExpandoObject();
            foreach (DataColumn column in row.Table.Columns)
            {
                var dict = expando as IDictionary<String, object>;
                dict.Add(column.ColumnName, row[column] == DBNull.Value ? null : row[column]);
            }
            return expando;
        }

        public static string DataTableToJson(DataTable table)
        {
            return JsonConvert.SerializeObject(DataTableToExpandoObject(table));
        }

        /// <summary>
        /// Converts a DataContract Object to Json
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>

        public static string ObjectToJson(object o)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(o.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, o);
            string json = Encoding.Default.GetString(stream.ToArray());
            stream.Dispose();
            return json;
        }

        /// <summary>
        /// THIS IS NOT TESTED
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(string s)
        {
            T result = Activator.CreateInstance<T>();
            MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(s));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(result.GetType());
            result = (T)serializer.ReadObject(stream);
            stream.Close();
            stream.Dispose();
            return result;
        }


        public static string ObjectToString(object o)
        {
            LosFormatter formatter = new LosFormatter();
            StringWriter writer = new StringWriter();
            formatter.Serialize(writer, o);
            return writer.ToString();
        }

        public static object StringToObject(string value)
        {
            if (value == string.Empty) return null;

            try
            {
                LosFormatter formatter = new LosFormatter();
                return formatter.Deserialize(value);
            }
            catch
            {
                return null;
            }
        }

        public static string VerifyUniqueFileName(string directory, string fileName)
        {
            string path = Path.Combine(directory, fileName);
            string result = fileName;
            int i = 0;
            while (File.Exists(path))
            {
                i++;
                if (i > 20) break;
                string name = Path.GetFileNameWithoutExtension(fileName);
                string ext = Path.GetExtension(fileName);
                result = name + " (" + i.ToString() + ")" + ext;
                path = Path.Combine(directory, result);
            }

            return result;
        }

        public static string VerifyUniqueUrlFileName(string directory, string fileName)
        {
            string path = Path.Combine(directory, fileName);
            string result = fileName;
            int i = 0;
            while (File.Exists(path))
            {
                i++;
                if (i > 20) break;
                string name = Path.GetFileNameWithoutExtension(fileName);
                string ext = Path.GetExtension(fileName);
                result = name + "_" + i.ToString() + ext;
                path = Path.Combine(directory, result);
            }

            return result;
        }

        public static string GenerateRandomPassword()
        {
            Random random = new Random();
            return GenerateRandomPassword(random);
        }

        public static string GenerateRandomPassword(Random random)
        {
            string[] chars = { "abcdefgijkmnopqrstwxyz", "ABCDEFGHJKLMNPQRSTWXYZ", "123456789", "*$-+?_&=!%{}/" };
            StringBuilder builder = new StringBuilder();
            int length = random.Next(8, 12);

            for (int i = 0; i < length; i++)
            {
                string s = chars[random.Next(0, 3)];
                char c = s[random.Next(0, s.Length)];
                builder.Append(c);
            }

            return builder.ToString();
        }

        public static string GenerateRandomKey(int length)
        {
            RNGCryptoServiceProvider crytpo = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[length / 2];
            crytpo.GetBytes(buffer);
            StringBuilder builder = new StringBuilder(length);
            for (int i = 0; i < buffer.Length; i++)
            {
                builder.Append(string.Format("{0:X2}", buffer[i]));
            }
            return builder.ToString();
        }

        public static CreditCardType GetCreditCardType(string cardNumber)
        {
            CreditCardType ccType = CreditCardType.Invalid;
            if (cardNumber.Trim().Length < 1) return ccType;

            byte[] number = new byte[16]; // number to validate

            // Remove non-digits
            int len = 0;
            for (int i = 0; i < cardNumber.Length; i++)
            {
                if (!char.IsDigit(cardNumber, i))
                {
                    return ccType;
                }
                else
                {
                    if (len == 16) return ccType; // number has too many digits
                    number[len++] = byte.Parse(cardNumber[i].ToString());
                }
            }

            // Use Luhn Algorithm to validate
            int sum = 0;
            for (int i = len - 1; i >= 0; i--)
            {
                if (i % 2 == len % 2)
                {
                    int n = number[i] * 2;
                    sum += (n / 10) + (n % 10);
                }
                else
                    sum += number[i];
            }

            if (sum % 10 == 0)
            {
                switch (cardNumber.Substring(0, 1))
                {
                    case "3": ccType = CreditCardType.AmericanExpress; break;
                    case "4": ccType = CreditCardType.Visa; break;
                    case "5": ccType = CreditCardType.MasterCard; break;
                    case "6": ccType = CreditCardType.Discover; break;
                    default: ccType = CreditCardType.Unknown; break;
                }
            }

            return ccType;
        }

        public static string CreditCardTypeString(CreditCardType type)
        {
            string result = "";
            switch (type)
            {
                case CreditCardType.Unknown:
                    result = "Unknown";
                    break;
                case CreditCardType.Invalid:
                    result = "Invalid";
                    break;
                case CreditCardType.Visa:
                    result = "Visa";
                    break;
                case CreditCardType.MasterCard:
                    result = "Master Card";
                    break;
                case CreditCardType.Discover:
                    result = "Discover";
                    break;
                case CreditCardType.AmericanExpress:
                    result = "American Express";
                    break;
                case CreditCardType.BetaTest:
                    result = "Beta Test";
                    break;
                default:
                    result = "ERROR";
                    break;
            }
            return result;
        }

        public static string MinutesToDisplayTime(int minutes, string format)
        {
            bool isNeg = minutes < 0;
            string result = "0 Minutes";

            minutes = Math.Abs(minutes);

            if (minutes < 60)
            {
                result = minutes.ToString() + " Minutes";
            }
            else if (minutes < 24 * 60)
            {
                if (format == "0")
                {
                    int i = minutes / 60;
                    int j = minutes % 60;
                    result = string.Format("{0} hours {1} minutes", i, j);
                }
                else
                {
                    double d = minutes / (60.0);
                    result = d.ToString(format) + " Hours";
                }
            }
            else
            {
                if (format == "0")
                {
                    int m = minutes % 60;
                    int h = (minutes / 60) % 24;
                    int d = (minutes / 60 / 24);
                    result = string.Format("{0} days {1} hours {2} minutes", d, h, m);
                }
                else
                {
                    double d = minutes / (60.0 * 24.0);
                    result = d.ToString(format) + " Days";
                }
            }

            if (isNeg) result = "- " + result;
            return result;
        }

        public static string MinutesToDisplayTime(int minutes)
        {
            return MinutesToDisplayTime(minutes, "0.00");
        }

        public static void LogException(LoginUser loginUser, Exception exception)
        {
            try
            {
                ExceptionLogs logs = new ExceptionLogs(loginUser);
                ExceptionLog log = logs.AddNewExceptionLog();


                log.Browser = HttpContext.Current.Request.Browser.Browser + HttpContext.Current.Request.Browser.Version;
                log.ExceptionName = exception.ToString();
                log.Message = exception.Message;
                log.PageInfo = "";
                log.StackTrace = exception.StackTrace;
                log.URL = "";
                logs.Save();

            }
            catch (Exception)
            {

            }
        }

        public static string DataRowToString(DataRow row)
        {
            if (row == null) return "[Row is NULL]";
            StringBuilder builder = new StringBuilder();
            string format = "{0}: \"{1}\"";
            foreach (DataColumn column in row.Table.Columns)
            {
                if (builder.Length > 0) builder.Append(", ");
                builder.Append(string.Format(format, column.ColumnName, row[column].ToString()));
            }
            return string.Format("[{0}] - {1}", row.Table.TableName, builder.ToString());
        }

        public static string RemoveHTMLTag(string[] tags, string text)
        {
            foreach (string tag in tags)
            {
                string theTag = tag.ToLower();
                int start = text.ToLower().IndexOf("<" + theTag);

                while (start > -1)
                {
                    int end = text.ToLower().IndexOf("</" + theTag + ">");
                    if (end < 0)
                        end = text.Length;
                    else
                        end = end + theTag.Length + 3;

                    end = end - start;

                    text = text.Remove(start, end);
                    start = text.ToLower().IndexOf("<" + theTag);
                }
            }
            return text;
        }

        public static string StripHTMLTag(string[] tags, string text)
        {
            foreach (string tag in tags)
            {
                string theTag = tag.ToLower();
                int start = text.ToLower().IndexOf("<" + theTag);

                while (start > -1)
                {
                    int end = text.ToLower().IndexOf("</" + theTag + ">");
                    if (end < 0)
                        end = text.Length;
                    else
                        end = end + theTag.Length + 3;

                    end = end - start;

                    text = text.Remove(start, end);
                    start = text.ToLower().IndexOf("<" + theTag);
                }
            }
            return text;
        }

        public static string GetMailLink(LoginUser loginUser, int userID, int ticketID)
        {
            string href = GetMailLinkHRef(loginUser, userID, ticketID);
            if (href == "") return "";

            return "<a class=\"mailLink\" href=\"" + href + "\">&nbsp</a>";
        }

        public static string GetMailLinkHRef(LoginUser loginUser, int userID, int ticketID)
        {
            User user = Users.GetUser(loginUser, userID);
            Organization organization = Organizations.GetOrganization(loginUser, loginUser.OrganizationID);
            Ticket ticket = Tickets.GetTicket(loginUser, ticketID);
            if (user == null || organization == null || ticket == null) return "";

            StringBuilder builder = new StringBuilder();
            builder.Append("mailto:" + user.Email);
            builder.Append("?cc=" + organization.GetReplyToAddress());
            builder.Append("&subject=" + HttpUtility.UrlPathEncode(ticket.Name).Replace("#", "%23") + " - [" + ticket.TicketNumber + "]");
            return builder.ToString();
        }

        public static bool ResetPassword(LoginUser loginUser, User user, bool isPortalUser, bool isCustomerHub = false, int? productFamilyID = null)
        {
            string password = GenerateRandomPassword();
            if (isCustomerHub)
                user.IsPasswordExpired = false;
            else
                user.IsPasswordExpired = true;
            user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
            user.PasswordCreatedUtc = DateTime.UtcNow;
            try
            {
                if (isPortalUser)
                    if (isCustomerHub == true)
                    {
                        EmailPosts.SendResetCustomerHubPassword(loginUser, user.UserID, password, productFamilyID);
                    }
                    else
                    {
                        user.IsPasswordExpired = true;
                        EmailPosts.SendResetPortalPassword(loginUser, user.UserID, password);
                    }
                else
                    EmailPosts.SendResetTSPassword(loginUser, user.UserID, password);
                user.Collection.Save();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static string ReplaceTicketLinks(string text)
        {
            int id = -1;
            while (true)
            {
                id = FindTicketID(text, SystemSettings.GetAppUrl() + "/ticket.aspx?ticketid=");
                text = RemoveTicketTarget(text, SystemSettings.GetAppUrl() + "/ticket.aspx?ticketid=");
                if (id < 0) break;
                string s = id.ToString();
                text = Regex.Replace(text, Regex.Escape(SystemSettings.GetAppUrl() + "/ticket.aspx?ticketid=" + s), "javascript:top.Ts.MainPage.openTicket(" + s + ",true);", RegexOptions.IgnoreCase);
            }

            while (true)
            {
                id = FindTicketID(text, "../?ticketid");
                text = RemoveTicketTarget(text, "../?ticketid");
                if (id < 0) break;
                string s = id.ToString();
                text = Regex.Replace(text, Regex.Escape("../?ticketid" + s), "javascript:top.Ts.MainPage.openTicket(" + s + ",true);", RegexOptions.IgnoreCase);
            }
            return text;
        }

        private static int FindTicketID(string text, string url)
        {
            text = text.ToLower();
            int i = text.IndexOf(url);
            if (i < 0) return -1;
            i += url.Length;
            int j = text.IndexOf('"', i);
            if (j < i) return -1;
            string s = text.Substring(i, j - i);
            try
            {
                return int.Parse(s);
            }
            catch
            {
                return -1;
            }
        }

        private static string RemoveTicketTarget(string text, string url)
        {
            string low = text.ToLower();
            int i = low.IndexOf(url);
            if (i - 20 < 0) return text;
            int j = low.IndexOf("_blank", i - 20);
            return text.Remove(j, 6);


        }

        public static string TableToCsv(LoginUser loginUser, DataTable table, bool replaceNewLineWithHtml = false)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (i > 0) builder.Append(",");
                builder.Append(table.Columns[i].ColumnName);
            }
            foreach (DataRow row in table.Rows)
            {
                builder.AppendLine();
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (i > 0) builder.Append(",");
                    string value = row[i].ToString();
                    if (value.Length > 8000) value = value.Substring(0, 8000);

                    value = "\"" + value.Replace("\"", "\"\"") + "\"";
                    if (replaceNewLineWithHtml) value = value.Replace(Environment.NewLine, "<br />");
                    Encoding utf8 = Encoding.GetEncoding("utf-8", new EncoderReplacementFallback("*"), new DecoderReplacementFallback("*"));
                    builder.Append(utf8.GetString(utf8.GetBytes(value)));
                }
            }
            return builder.ToString();
        }

        public static string CommandToCsv(LoginUser loginUser, SqlCommand command, bool replaceNewLineWithHtml)
        {
            DateTime dateValue;
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
            {
                connection.Open();
                command.Connection = connection;
                StringBuilder builder = new StringBuilder();
                SqlDataReader reader = command.ExecuteReader();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (i > 0) builder.Append(",");
                    builder.Append(reader.GetName(i));
                }

                while (reader.Read())
                {
                    builder.AppendLine();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (i > 0) builder.Append(",");
                        string value = reader[i].ToString();

                        try
                        {
                            if (value.Length > 8000) value = value.Substring(0, 8000);

                            try
                            {
                                if (DateTime.TryParse(value, out dateValue))
                                {
                                    value = TimeZoneInfo.ConvertTimeFromUtc(dateValue, loginUser.TimeZoneInfo).ToString(loginUser.CultureInfo);
                                }
                            }
                            catch (Exception)
                            {

                            }

                            value = "\"" + value.Replace("\"", "\"\"") + "\"";

                            if (replaceNewLineWithHtml) value = value.Replace(Environment.NewLine, "<br />");

                        }
                        catch (Exception ex)
                        {
                            ex.Data.Add("value", value);
                            ExceptionLogs.LogException(loginUser, ex, "CommandToCsv");
                            value = "\"\"";
                        }

						Encoding utf8 = Encoding.GetEncoding("utf-8", new EncoderReplacementFallback("*"), new DecoderReplacementFallback("*"));
						builder.Append(utf8.GetString(utf8.GetBytes(value)));
                    }
                }
                reader.Close();
                return builder.ToString();
            }
        }

        public static DataTable GetTable(LoginUser loginUser, SqlCommand command)
        {
            BaseCollection.FixCommandParameters(command);
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);

                command.Connection = connection;
                command.Transaction = transaction;
                try
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionLogs.LogException(loginUser, ex, "DataUtils GetTable", GetCommandTextSql(command));
                    throw;
                }
                connection.Close();
            }

            return table;

        }

        public static string ReplaceEx(string original, string pattern, string replacement)
        {
            int count, position0, position1;
            count = position0 = position1 = 0;
            string upperString = original.ToUpper();
            string upperPattern = pattern.ToUpper();
            int inc = (original.Length / pattern.Length) *
                      (replacement.Length - pattern.Length);
            char[] chars = new char[original.Length + Math.Max(0, inc)];
            while ((position1 = upperString.IndexOf(upperPattern,
                                              position0)) != -1)
            {
                for (int i = position0; i < position1; ++i)
                    chars[count++] = original[i];
                for (int i = 0; i < replacement.Length; ++i)
                    chars[count++] = replacement[i];
                position0 = position1 + pattern.Length;
            }
            if (position0 == 0) return original;
            for (int i = position0; i < original.Length; ++i)
                chars[count++] = original[i];
            return new string(chars, 0, count);
        }

        public static string CreateLinks(string text, string caption, string href, string cssClass, string target)
        {
            try
            {
                string result = text;
                MatchCollection matches = Regex.Matches(result, "\\b" + caption + "\\b", RegexOptions.IgnoreCase);
                string link = "<a href=\"{0}\" class=\"{1}\" target=\"{2}\">{3}</a>";
                for (int i = matches.Count - 1; i >= 0; i--)
                {
                    Match match = matches[i];

                    string tag = GetParentHtmlTag(result, match.Index);
                    if (tag != null && (tag == "top" || tag == "p" || tag == "div" || tag == "span"))
                    {
                        result = result.Remove(match.Index, match.Length).Insert(match.Index, string.Format(link, href, cssClass, target, match.Value));
                    }
                }
                return result;
            }
            catch (Exception)
            {
                return text;
            }
        }

        public static string GetParentHtmlTag(string html, int startIndex)
        {
            bool closed = false;
            /*<a href="Ticket">Ticket</a><p>Ticket</p>*/
            for (int i = startIndex; i >= 0; i--)
            {
                char c = html[i];
                if (c == '<')
                {
                    if (closed)
                    {
                        int j = i + 1;
                        StringBuilder result = new StringBuilder();

                        while (html[j] == ' ') j++;

                        while (j < startIndex)
                        {
                            if (html[j] == ' ') break;
                            result.Append(html[j]);
                            j++;
                        }

                        return result.ToString().ToLower();

                    }
                    else
                    {
                        return null;
                    }
                }
                else if (c == '>')
                {
                    closed = true;
                }

            }
            return "top";
        }

        public static string MimeTypeFromFileName(string fileName)
        {
            string result = "";
            switch (Path.GetExtension(fileName).ToLower())
            {
                case ".3dm": result = "x-world/x-3dmf"; break;
                case ".3dmf": result = "x-world/x-3dmf"; break;
                case ".a": result = "application/octet-stream"; break;
                case ".aab": result = "application/x-authorware-bin"; break;
                case ".aam": result = "application/x-authorware-map"; break;
                case ".aas": result = "application/x-authorware-seg"; break;
                case ".abc": result = "text/vnd.abc"; break;
                case ".acgi": result = "text/html"; break;
                case ".afl": result = "video/animaflex"; break;
                case ".ai": result = "application/postscript"; break;
                case ".aif": result = "audio/aiff"; break;
                case ".aifc": result = "audio/aiff"; break;
                case ".aiff": result = "audio/aiff"; break;
                case ".aim": result = "application/x-aim"; break;
                case ".aip": result = "text/x-audiosoft-intra"; break;
                case ".ani": result = "application/x-navi-animation"; break;
                case ".aos": result = "application/x-nokia-9000-communicator-add-on-software"; break;
                case ".aps": result = "application/mime"; break;
                case ".arc": result = "application/octet-stream"; break;
                case ".arj": result = "application/arj"; break;
                case ".art": result = "image/x-jg"; break;
                case ".asf": result = "video/x-ms-asf"; break;
                case ".asm": result = "text/x-asm"; break;
                case ".asp": result = "text/asp"; break;
                case ".asx": result = "video/x-ms-asf"; break;
                case ".au": result = "audio/basic"; break;
                case ".avi": result = "video/avi"; break;
                case ".avs": result = "video/avs-video"; break;
                case ".bcpio": result = "application/x-bcpio"; break;
                case ".bin": result = "application/octet-stream"; break;
                case ".bm": result = "image/bmp"; break;
                case ".bmp": result = "image/bmp"; break;
                case ".boo": result = "application/book"; break;
                case ".book": result = "application/book"; break;
                case ".boz": result = "application/x-bzip2"; break;
                case ".bsh": result = "application/x-bsh"; break;
                case ".bz": result = "application/x-bzip"; break;
                case ".bz2": result = "application/x-bzip2"; break;
                case ".c": result = "text/plain"; break;
                case ".c++": result = "text/plain"; break;
                case ".cat": result = "application/vnd.ms-pki.seccat"; break;
                case ".cc": result = "text/plain"; break;
                case ".ccad": result = "application/clariscad"; break;
                case ".cco": result = "application/x-cocoa"; break;
                case ".cdf": result = "application/cdf"; break;
                case ".cer": result = "application/pkix-cert"; break;
                case ".cha": result = "application/x-chat"; break;
                case ".chat": result = "application/x-chat"; break;
                case ".class": result = "application/java"; break;
                case ".com": result = "application/octet-stream"; break;
                case ".conf": result = "text/plain"; break;
                case ".cpio": result = "application/x-cpio"; break;
                case ".cpp": result = "text/x-c"; break;
                case ".cpt": result = "application/x-cpt"; break;
                case ".crl": result = "application/pkcs-crl"; break;
                case ".crt": result = "application/pkix-cert"; break;
                case ".csh": result = "application/x-csh"; break;
                case ".css": result = "text/css"; break;
                case ".cxx": result = "text/plain"; break;
                case ".dcr": result = "application/x-director"; break;
                case ".deepv": result = "application/x-deepv"; break;
                case ".def": result = "text/plain"; break;
                case ".der": result = "application/x-x509-ca-cert"; break;
                case ".dif": result = "video/x-dv"; break;
                case ".dir": result = "application/x-director"; break;
                case ".dl": result = "video/dl"; break;
                case ".doc": result = "application/msword"; break;
                case ".dot": result = "application/msword"; break;
                case ".dp": result = "application/commonground"; break;
                case ".drw": result = "application/drafting"; break;
                case ".dump": result = "application/octet-stream"; break;
                case ".dv": result = "video/x-dv"; break;
                case ".dvi": result = "application/x-dvi"; break;
                case ".dwf": result = "model/vnd.dwf"; break;
                case ".dwg": result = "image/vnd.dwg"; break;
                case ".dxf": result = "image/vnd.dwg"; break;
                case ".dxr": result = "application/x-director"; break;
                case ".el": result = "text/x-script.elisp"; break;
                case ".elc": result = "application/x-elc"; break;
                case ".env": result = "application/x-envoy"; break;
                case ".eps": result = "application/postscript"; break;
                case ".es": result = "application/x-esrehber"; break;
                case ".etx": result = "text/x-setext"; break;
                case ".evy": result = "application/envoy"; break;
                case ".exe": result = "application/octet-stream"; break;
                case ".f": result = "text/plain"; break;
                case ".f77": result = "text/x-fortran"; break;
                case ".f90": result = "text/plain"; break;
                case ".fdf": result = "application/vnd.fdf"; break;
                case ".fif": result = "image/fif"; break;
                case ".fli": result = "video/fli"; break;
                case ".flo": result = "image/florian"; break;
                case ".flx": result = "text/vnd.fmi.flexstor"; break;
                case ".fmf": result = "video/x-atomic3d-feature"; break;
                case ".for": result = "text/x-fortran"; break;
                case ".fpx": result = "image/vnd.fpx"; break;
                case ".frl": result = "application/freeloader"; break;
                case ".funk": result = "audio/make"; break;
                case ".g": result = "text/plain"; break;
                case ".g3": result = "image/g3fax"; break;
                case ".gif": result = "image/gif"; break;
                case ".gl": result = "video/gl"; break;
                case ".gsd": result = "audio/x-gsm"; break;
                case ".gsm": result = "audio/x-gsm"; break;
                case ".gsp": result = "application/x-gsp"; break;
                case ".gss": result = "application/x-gss"; break;
                case ".gtar": result = "application/x-gtar"; break;
                case ".gz": result = "application/x-gzip"; break;
                case ".gzip": result = "application/x-gzip"; break;
                case ".h": result = "text/plain"; break;
                case ".hdf": result = "application/x-hdf"; break;
                case ".help": result = "application/x-helpfile"; break;
                case ".hgl": result = "application/vnd.hp-hpgl"; break;
                case ".hh": result = "text/plain"; break;
                case ".hlb": result = "text/x-script"; break;
                case ".hlp": result = "application/hlp"; break;
                case ".hpg": result = "application/vnd.hp-hpgl"; break;
                case ".hpgl": result = "application/vnd.hp-hpgl"; break;
                case ".hqx": result = "application/binhex"; break;
                case ".hta": result = "application/hta"; break;
                case ".htc": result = "text/x-component"; break;
                case ".htm": result = "text/html"; break;
                case ".html": result = "text/html"; break;
                case ".htmls": result = "text/html"; break;
                case ".htt": result = "text/webviewhtml"; break;
                case ".htx": result = "text/html"; break;
                case ".ice": result = "x-conference/x-cooltalk"; break;
                case ".ico": result = "image/x-icon"; break;
                case ".idc": result = "text/plain"; break;
                case ".ief": result = "image/ief"; break;
                case ".iefs": result = "image/ief"; break;
                case ".iges": result = "application/iges"; break;
                case ".igs": result = "application/iges"; break;
                case ".ima": result = "application/x-ima"; break;
                case ".imap": result = "application/x-httpd-imap"; break;
                case ".inf": result = "application/inf"; break;
                case ".ins": result = "application/x-internett-signup"; break;
                case ".ip": result = "application/x-ip2"; break;
                case ".isu": result = "video/x-isvideo"; break;
                case ".it": result = "audio/it"; break;
                case ".iv": result = "application/x-inventor"; break;
                case ".ivr": result = "i-world/i-vrml"; break;
                case ".ivy": result = "application/x-livescreen"; break;
                case ".jam": result = "audio/x-jam"; break;
                case ".jav": result = "text/plain"; break;
                case ".java": result = "text/plain"; break;
                case ".jcm": result = "application/x-java-commerce"; break;
                case ".jfif": result = "image/jpeg"; break;
                case ".jfif-tbnl": result = "image/jpeg"; break;
                case ".jpe": result = "image/jpeg"; break;
                case ".jpeg": result = "image/jpeg"; break;
                case ".jpg": result = "image/jpeg"; break;
                case ".jps": result = "image/x-jps"; break;
                case ".js": result = "application/x-javascript"; break;
                case ".jut": result = "image/jutvision"; break;
                case ".kar": result = "audio/midi"; break;
                case ".ksh": result = "application/x-ksh"; break;
                case ".la": result = "audio/nspaudio"; break;
                case ".lam": result = "audio/x-liveaudio"; break;
                case ".latex": result = "application/x-latex"; break;
                case ".lha": result = "application/octet-stream"; break;
                case ".lhx": result = "application/octet-stream"; break;
                case ".list": result = "text/plain"; break;
                case ".lma": result = "audio/nspaudio"; break;
                case ".log": result = "text/plain"; break;
                case ".lsp": result = "application/x-lisp"; break;
                case ".lst": result = "text/plain"; break;
                case ".lsx": result = "text/x-la-asf"; break;
                case ".ltx": result = "application/x-latex"; break;
                case ".lzh": result = "application/octet-stream"; break;
                case ".lzx": result = "application/octet-stream"; break;
                case ".m": result = "text/plain"; break;
                case ".m1v": result = "video/mpeg"; break;
                case ".m2a": result = "audio/mpeg"; break;
                case ".m2v": result = "video/mpeg"; break;
                case ".m3u": result = "audio/x-mpequrl"; break;
                case ".man": result = "application/x-troff-man"; break;
                case ".map": result = "application/x-navimap"; break;
                case ".mar": result = "text/plain"; break;
                case ".mbd": result = "application/mbedlet"; break;
                case ".mc$": result = "application/x-magic-cap-package-1.0"; break;
                case ".mcd": result = "application/mcad"; break;
                case ".mcf": result = "text/mcf"; break;
                case ".mcp": result = "application/netmc"; break;
                case ".me": result = "application/x-troff-me"; break;
                case ".mht": result = "message/rfc822"; break;
                case ".mhtml": result = "message/rfc822"; break;
                case ".mid": result = "audio/midi"; break;
                case ".midi": result = "audio/midi"; break;
                case ".mif": result = "application/x-mif"; break;
                case ".mime": result = "message/rfc822"; break;
                case ".mjf": result = "audio/x-vnd.audioexplosion.mjuicemediafile"; break;
                case ".mjpg": result = "video/x-motion-jpeg"; break;
                case ".mm": result = "application/base64"; break;
                case ".mme": result = "application/base64"; break;
                case ".mod": result = "audio/mod"; break;
                case ".moov": result = "video/quicktime"; break;
                case ".mov": result = "video/quicktime"; break;
                case ".movie": result = "video/x-sgi-movie"; break;
                case ".mp2": result = "audio/mpeg"; break;
                case ".mp3": result = "audio/mpeg"; break;
                case ".mpa": result = "audio/mpeg"; break;
                case ".mpc": result = "application/x-project"; break;
                case ".mpe": result = "video/mpeg"; break;
                case ".mpeg": result = "video/mpeg"; break;
                case ".mpg": result = "video/mpeg"; break;
                case ".mpga": result = "audio/mpeg"; break;
                case ".mpp": result = "application/vnd.ms-project"; break;
                case ".mpt": result = "application/vnd.ms-project"; break;
                case ".mpv": result = "application/vnd.ms-project"; break;
                case ".mpx": result = "application/vnd.ms-project"; break;
                case ".mrc": result = "application/marc"; break;
                case ".ms": result = "application/x-troff-ms"; break;
                case ".mv": result = "video/x-sgi-movie"; break;
                case ".my": result = "audio/make"; break;
                case ".mzz": result = "application/x-vnd.audioexplosion.mzz"; break;
                case ".nap": result = "image/naplps"; break;
                case ".naplps": result = "image/naplps"; break;
                case ".nc": result = "application/x-netcdf"; break;
                case ".ncm": result = "application/vnd.nokia.configuration-message"; break;
                case ".nif": result = "image/x-niff"; break;
                case ".niff": result = "image/x-niff"; break;
                case ".nix": result = "application/x-mix-transfer"; break;
                case ".nsc": result = "application/x-conference"; break;
                case ".nvd": result = "application/x-navidoc"; break;
                case ".o": result = "application/octet-stream"; break;
                case ".oda": result = "application/oda"; break;
                case ".omc": result = "application/x-omc"; break;
                case ".omcd": result = "application/x-omcdatamaker"; break;
                case ".omcr": result = "application/x-omcregerator"; break;
                case ".p": result = "text/x-pascal"; break;
                case ".p10": result = "application/pkcs10"; break;
                case ".p12": result = "application/pkcs-12"; break;
                case ".p7a": result = "application/x-pkcs7-signature"; break;
                case ".p7c": result = "application/pkcs7-mime"; break;
                case ".p7m": result = "application/pkcs7-mime"; break;
                case ".p7r": result = "application/x-pkcs7-certreqresp"; break;
                case ".p7s": result = "application/pkcs7-signature"; break;
                case ".part": result = "application/pro_eng"; break;
                case ".pas": result = "text/pascal"; break;
                case ".pbm": result = "image/x-portable-bitmap"; break;
                case ".pcl": result = "application/vnd.hp-pcl"; break;
                case ".pct": result = "image/x-pict"; break;
                case ".pcx": result = "image/x-pcx"; break;
                case ".pdb": result = "chemical/x-pdb"; break;
                case ".pdf": result = "application/pdf"; break;
                case ".pfunk": result = "audio/make"; break;
                case ".pgm": result = "image/x-portable-greymap"; break;
                case ".pic": result = "image/pict"; break;
                case ".pict": result = "image/pict"; break;
                case ".pkg": result = "application/x-newton-compatible-pkg"; break;
                case ".pko": result = "application/vnd.ms-pki.pko"; break;
                case ".pl": result = "text/plain"; break;
                case ".plx": result = "application/x-pixclscript"; break;
                case ".pm": result = "image/x-xpixmap"; break;
                case ".pm4": result = "application/x-pagemaker"; break;
                case ".pm5": result = "application/x-pagemaker"; break;
                case ".png": result = "image/png"; break;
                case ".pnm": result = "application/x-portable-anymap"; break;
                case ".pot": result = "application/vnd.ms-powerpoint"; break;
                case ".pov": result = "model/x-pov"; break;
                case ".ppa": result = "application/vnd.ms-powerpoint"; break;
                case ".ppm": result = "image/x-portable-pixmap"; break;
                case ".pps": result = "application/vnd.ms-powerpoint"; break;
                case ".ppt": result = "application/vnd.ms-powerpoint"; break;
                case ".ppz": result = "application/vnd.ms-powerpoint"; break;
                case ".pre": result = "application/x-freelance"; break;
                case ".prt": result = "application/pro_eng"; break;
                case ".ps": result = "application/postscript"; break;
                case ".psd": result = "application/octet-stream"; break;
                case ".pvu": result = "paleovu/x-pv"; break;
                case ".pwz": result = "application/vnd.ms-powerpoint"; break;
                case ".py": result = "text/x-script.phyton"; break;
                case ".pyc": result = "applicaiton/x-bytecode.python"; break;
                case ".qcp": result = "audio/vnd.qcelp"; break;
                case ".qd3": result = "x-world/x-3dmf"; break;
                case ".qd3d": result = "x-world/x-3dmf"; break;
                case ".qif": result = "image/x-quicktime"; break;
                case ".qt": result = "video/quicktime"; break;
                case ".qtc": result = "video/x-qtc"; break;
                case ".qti": result = "image/x-quicktime"; break;
                case ".qtif": result = "image/x-quicktime"; break;
                case ".ra": result = "audio/x-pn-realaudio"; break;
                case ".ram": result = "audio/x-pn-realaudio"; break;
                case ".ras": result = "application/x-cmu-raster"; break;
                case ".rast": result = "image/cmu-raster"; break;
                case ".rexx": result = "text/x-script.rexx"; break;
                case ".rf": result = "image/vnd.rn-realflash"; break;
                case ".rgb": result = "image/x-rgb"; break;
                case ".rm": result = "application/vnd.rn-realmedia"; break;
                case ".rmi": result = "audio/mid"; break;
                case ".rmm": result = "audio/x-pn-realaudio"; break;
                case ".rmp": result = "audio/x-pn-realaudio"; break;
                case ".rng": result = "application/ringing-tones"; break;
                case ".rnx": result = "application/vnd.rn-realplayer"; break;
                case ".roff": result = "application/x-troff"; break;
                case ".rp": result = "image/vnd.rn-realpix"; break;
                case ".rpm": result = "audio/x-pn-realaudio-plugin"; break;
                case ".rt": result = "text/richtext"; break;
                case ".rtf": result = "text/richtext"; break;
                case ".rtx": result = "text/richtext"; break;
                case ".rv": result = "video/vnd.rn-realvideo"; break;
                case ".s": result = "text/x-asm"; break;
                case ".s3m": result = "audio/s3m"; break;
                case ".saveme": result = "application/octet-stream"; break;
                case ".sbk": result = "application/x-tbook"; break;
                case ".scm": result = "application/x-lotusscreencam"; break;
                case ".sdml": result = "text/plain"; break;
                case ".sdp": result = "application/sdp"; break;
                case ".sdr": result = "application/sounder"; break;
                case ".sea": result = "application/sea"; break;
                case ".set": result = "application/set"; break;
                case ".sgm": result = "text/sgml"; break;
                case ".sgml": result = "text/sgml"; break;
                case ".sh": result = "application/x-sh"; break;
                case ".shar": result = "application/x-shar"; break;
                case ".shtml": result = "text/html"; break;
                case ".sid": result = "audio/x-psid"; break;
                case ".sit": result = "application/x-sit"; break;
                case ".skd": result = "application/x-koan"; break;
                case ".skm": result = "application/x-koan"; break;
                case ".skp": result = "application/x-koan"; break;
                case ".skt": result = "application/x-koan"; break;
                case ".sl": result = "application/x-seelogo"; break;
                case ".smi": result = "application/smil"; break;
                case ".smil": result = "application/smil"; break;
                case ".snd": result = "audio/basic"; break;
                case ".sol": result = "application/solids"; break;
                case ".spc": result = "text/x-speech"; break;
                case ".spl": result = "application/futuresplash"; break;
                case ".spr": result = "application/x-sprite"; break;
                case ".sprite": result = "application/x-sprite"; break;
                case ".src": result = "application/x-wais-source"; break;
                case ".ssi": result = "text/x-server-parsed-html"; break;
                case ".ssm": result = "application/streamingmedia"; break;
                case ".sst": result = "application/vnd.ms-pki.certstore"; break;
                case ".step": result = "application/step"; break;
                case ".stl": result = "application/sla"; break;
                case ".stp": result = "application/step"; break;
                case ".sv4cpio": result = "application/x-sv4cpio"; break;
                case ".sv4crc": result = "application/x-sv4crc"; break;
                case ".svf": result = "image/vnd.dwg"; break;
                case ".svr": result = "application/x-world"; break;
                case ".swf": result = "application/x-shockwave-flash"; break;
                case ".t": result = "application/x-troff"; break;
                case ".talk": result = "text/x-speech"; break;
                case ".tar": result = "application/x-tar"; break;
                case ".tbk": result = "application/toolbook"; break;
                case ".tcl": result = "application/x-tcl"; break;
                case ".tcsh": result = "text/x-script.tcsh"; break;
                case ".tex": result = "application/x-tex"; break;
                case ".texi": result = "application/x-texinfo"; break;
                case ".texinfo": result = "application/x-texinfo"; break;
                case ".text": result = "text/plain"; break;
                case ".tgz": result = "application/x-compressed"; break;
                case ".tif": result = "image/tiff"; break;
                case ".tiff": result = "image/tiff"; break;
                case ".tr": result = "application/x-troff"; break;
                case ".tsi": result = "audio/tsp-audio"; break;
                case ".tsp": result = "application/dsptype"; break;
                case ".tsv": result = "text/tab-separated-values"; break;
                case ".turbot": result = "image/florian"; break;
                case ".txt": result = "text/plain"; break;
                case ".uil": result = "text/x-uil"; break;
                case ".uni": result = "text/uri-list"; break;
                case ".unis": result = "text/uri-list"; break;
                case ".unv": result = "application/i-deas"; break;
                case ".uri": result = "text/uri-list"; break;
                case ".uris": result = "text/uri-list"; break;
                case ".ustar": result = "application/x-ustar"; break;
                case ".uu": result = "application/octet-stream"; break;
                case ".uue": result = "text/x-uuencode"; break;
                case ".vcd": result = "application/x-cdlink"; break;
                case ".vcs": result = "text/x-vcalendar"; break;
                case ".vda": result = "application/vda"; break;
                case ".vdo": result = "video/vdo"; break;
                case ".vew": result = "application/groupwise"; break;
                case ".viv": result = "video/vivo"; break;
                case ".vivo": result = "video/vivo"; break;
                case ".vmd": result = "application/vocaltec-media-desc"; break;
                case ".vmf": result = "application/vocaltec-media-file"; break;
                case ".voc": result = "audio/voc"; break;
                case ".vos": result = "video/vosaic"; break;
                case ".vox": result = "audio/voxware"; break;
                case ".vqe": result = "audio/x-twinvq-plugin"; break;
                case ".vqf": result = "audio/x-twinvq"; break;
                case ".vql": result = "audio/x-twinvq-plugin"; break;
                case ".vrml": result = "application/x-vrml"; break;
                case ".vrt": result = "x-world/x-vrt"; break;
                case ".vsd": result = "application/x-visio"; break;
                case ".vst": result = "application/x-visio"; break;
                case ".vsw": result = "application/x-visio"; break;
                case ".w60": result = "application/wordperfect6.0"; break;
                case ".w61": result = "application/wordperfect6.1"; break;
                case ".w6w": result = "application/msword"; break;
                case ".wav": result = "audio/wav"; break;
                case ".wb1": result = "application/x-qpro"; break;
                case ".wbmp": result = "image/vnd.wap.wbmp"; break;
                case ".web": result = "application/vnd.xara"; break;
                case ".wiz": result = "application/msword"; break;
                case ".wk1": result = "application/x-123"; break;
                case ".wmf": result = "windows/metafile"; break;
                case ".wml": result = "text/vnd.wap.wml"; break;
                case ".wmlc": result = "application/vnd.wap.wmlc"; break;
                case ".wmls": result = "text/vnd.wap.wmlscript"; break;
                case ".wmlsc": result = "application/vnd.wap.wmlscriptc"; break;
                case ".word": result = "application/msword"; break;
                case ".wp": result = "application/wordperfect"; break;
                case ".wp5": result = "application/wordperfect"; break;
                case ".wp6": result = "application/wordperfect"; break;
                case ".wpd": result = "application/wordperfect"; break;
                case ".wq1": result = "application/x-lotus"; break;
                case ".wri": result = "application/mswrite"; break;
                case ".wrl": result = "application/x-world"; break;
                case ".wrz": result = "x-world/x-vrml"; break;
                case ".wsc": result = "text/scriplet"; break;
                case ".wsrc": result = "application/x-wais-source"; break;
                case ".wtk": result = "application/x-wintalk"; break;
                case ".xbm": result = "image/x-xbitmap"; break;
                case ".xdr": result = "video/x-amt-demorun"; break;
                case ".xgz": result = "xgl/drawing"; break;
                case ".xif": result = "image/vnd.xiff"; break;
                case ".xl": result = "application/excel"; break;
                case ".xla": result = "application/vnd.ms-excel"; break;
                case ".xlb": result = "application/vnd.ms-excel"; break;
                case ".xlc": result = "application/vnd.ms-excel"; break;
                case ".xld": result = "application/vnd.ms-excel"; break;
                case ".xlk": result = "application/vnd.ms-excel"; break;
                case ".xll": result = "application/vnd.ms-excel"; break;
                case ".xlm": result = "application/vnd.ms-excel"; break;
                case ".xls": result = "application/vnd.ms-excel"; break;
                case ".xlt": result = "application/vnd.ms-excel"; break;
                case ".xlv": result = "application/vnd.ms-excel"; break;
                case ".xlw": result = "application/vnd.ms-excel"; break;
                case ".xm": result = "audio/xm"; break;
                case ".xml": result = "application/xml"; break;
                case ".xmz": result = "xgl/movie"; break;
                case ".xpix": result = "application/x-vnd.ls-xpix"; break;
                case ".xpm": result = "image/xpm"; break;
                case ".x-png": result = "image/png"; break;
                case ".xsr": result = "video/x-amt-showrun"; break;
                case ".xwd": result = "image/x-xwd"; break;
                case ".xyz": result = "chemical/x-pdb"; break;
                case ".z": result = "application/x-compressed"; break;
                case ".zip": result = "application/zip"; break;
                case ".zoo": result = "application/octet-stream"; break;
                case ".zsh": result = "text/x-script.zsh"; break;
                default: result = "application/octet-stream"; break;
            }
            return result;
        }

        public static string IntArrayToCommaString(int[] list)
        {
            return string.Join(",", list.Select(x => x.ToString()).ToArray());
        }

        public static int[] StringArrayToIntArray(string[] list)
        {
            return list.Select(x => int.Parse(x)).ToArray();
        }

        public static string GetIndexPath(LoginUser loginUser)
        {
            string path = System.Web.Configuration.WebConfigurationManager.AppSettings["IndexerPath"];
            path = string.IsNullOrWhiteSpace(path) ? SystemSettings.ReadString(loginUser, "IndexerPathTickets", "c:\\TSIndexes\\") : path;
            return Path.Combine(path, loginUser.OrganizationID.ToString());
        }

        public static string GetTicketIndexPath(LoginUser loginUser)
        {
            return GetIndexPath(loginUser) + "\\Tickets";
        }

        public static string GetWikiIndexPath(LoginUser loginUser)
        {
            return GetIndexPath(loginUser) + "\\Wikis";
        }

        public static string GetNotesIndexPath(LoginUser loginUser)
        {
            return GetIndexPath(loginUser) + "\\Notes";
        }

        public static string GetProductVersionsIndexPath(LoginUser loginUser)
        {
            return GetIndexPath(loginUser) + "\\ProductVersions";
        }

        public static string GetTasksIndexPath(LoginUser loginUser)
        {
            return GetIndexPath(loginUser) + "\\Tasks";
        }

        public static string GetWaterCoolerIndexPath(LoginUser loginUser)
        {
            return GetIndexPath(loginUser) + "\\WaterCooler";
        }

        public static string GetCompaniesIndexPath(LoginUser loginUser)
        {
            return GetIndexPath(loginUser) + "\\Customers";
        }

        public static string GetAssetsIndexPath(LoginUser loginUser)
        {
            return GetIndexPath(loginUser) + "\\Assets";
        }

        public static string GetProductsIndexPath(LoginUser loginUser)
        {
            return GetIndexPath(loginUser) + "\\Products";
        }

        public static string GetContactsIndexPath(LoginUser loginUser)
        {
            return GetIndexPath(loginUser) + "\\Contacts";
        }

        public static string GetPortalTicketsIndexPath(LoginUser loginUser, int parentOrganizationID)
        {
            return GetPortalIndexPath(loginUser, parentOrganizationID) + "\\Tickets";
        }

        public static string GetPortalWikiIndexPath(LoginUser loginUser, int parentOrganizationID)
        {
            return GetPortalIndexPath(loginUser, parentOrganizationID) + "\\Wikis";
        }

        public static string GetPortalIndexPath(LoginUser loginUser, int parentOrganizationID)
        {
            string path = System.Web.Configuration.WebConfigurationManager.AppSettings["IndexerPath"];
            path = string.IsNullOrWhiteSpace(path) ? SystemSettings.ReadString(loginUser, "IndexerPathTickets", "c:\\TSIndexes\\") : path;
            return Path.Combine(path, parentOrganizationID.ToString());
        }

        public static bool GetIsColumnInBaseCollection(BaseCollection baseCollection, string columnName)
        {
            bool result = false;
            foreach (DataColumn column in baseCollection.Table.Columns)
            {
                if (String.Equals(column.ColumnName, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public static string GetWikiEquivalentFieldName(string ticketFieldName)
        {
            string result = string.Empty;

            switch (ticketFieldName)
            {
                case "Name":
                    result = "ArticleName";
                    break;
                case "CreatorID":
                    result = "CreatedBy";
                    break;
                case "CreatorName":
                    result = "Creator";
                    break;
                case "DateCreated":
                    result = "CreatedDate";
                    break;
                case "ModifierID":
                    result = "ModifiedBy";
                    break;
                case "ModifierName":
                    result = "Modifier";
                    break;
                case "DateModified":
                    result = "ModifiedDate";
                    break;
                default:
                    result = ticketFieldName;
                    break;
            }

            return result;
        }

        public static string GetNotesEquivalentFieldName(string ticketFieldName)
        {
            string result = string.Empty;

            switch (ticketFieldName)
            {
                case "Name":
                    result = "Title";
                    break;
                default:
                    result = ticketFieldName;
                    break;
            }

            return result;
        }

        public static string GetProductVersionsEquivalentFieldName(string ticketFieldName)
        {
            string result = string.Empty;

            switch (ticketFieldName)
            {
                default:
                    result = ticketFieldName;
                    break;
            }

            return result;
        }

        public static string GetTasksEquivalentFieldName(string ticketFieldName)
        {
            string result = string.Empty;

            switch (ticketFieldName)
            {
                default:
                    result = ticketFieldName;
                    break;
            }

            return result;
        }

        public static string GetWaterCoolerEquivalentFieldName(string ticketFieldName)
        {
            string result = string.Empty;

            switch (ticketFieldName)
            {
                case "DateCreated":
                    result = "TimeStamp";
                    break;
                case "DateModified":
                    result = "LastModified";
                    break;
                default:
                    result = ticketFieldName;
                    break;
            }

            return result;
        }

        public static string StripInvalidXmlCharacters(string input)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                if (XmlConvert.IsXmlChar(input[i]))
                {
                    result.Append(input[i]);
                }
            }

            return result.ToString();
        }

        public static string GetEncodedCredentials(string username, string password)
        {
            string mergedCredentials = string.Format("{0}:{1}", username, password);
            byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }

        public static string GetJsonCompatibleString(string input)
        {
            if (input == null) return null;

            StringBuilder result = new StringBuilder();
            char ch;

            // I implemented this method after finding control characters after the StripHTML method was applied.
            // I noticed that running the StripHTML method twice end up getting the correct string format.
            // Not sure if is needed to run this method but decided to keep it in case it is.
            for (int i = 0; i < input.Length; i++)
            {
                ch = input[i];

                if (!char.IsControl(ch))
                {
                    result.Append(ch);
                }
                else if (ch == '\n')
                {
                    result.Append("\\n");
                }
                else if (ch == '\r')
                {
                    result.Append("\\r");
                }
            }

            //The single quote is supported in json and do not need to be scaped.
            result.
              Replace(@"\\", @"\").
              Replace(@"""""", @"""").
              //Replace(@"''", @"'").
              Replace(@"\""", @"""").
              //Replace(@"\'", @"'").
              Replace(@"\", @"\\").
              Replace(@"\\n", @"\n").
              Replace(@"\\r", @"\r").
              Replace(@"""", @"\""");
            //Replace(@"'", @"\'");

            return result.ToString();
        }

        public static object GetValueFromObject(
          LoginUser loginUser,
          FieldMap fieldMap,
          DataSet input,
          string objectIDColumnName,
          string objectNameColumnName,
          Func<LoginUser, string, int?, int?> GetIDWithName,
          bool isChild,
          int? parentID,
          bool overrideFieldMap = false)
        {
            object result = null;
            if (!isChild || parentID != null)
            {
                // If the field exists in the fieldMap and is able to do insert.
                FieldMapItem fieldMapItem = fieldMap.Items.Find(x => x.PrivateName == objectIDColumnName);
                if ((fieldMapItem != null && fieldMapItem.Insert) || overrideFieldMap)
                {
                    // If the fieldID is provided use it.
                    DataRow row = input.Tables[0].Rows[0];
                    if (input.Tables[0].Columns[objectIDColumnName] != null)
                    {
                        DataColumn column = input.Tables[0].Columns[objectIDColumnName];
                        if (row[column] != DBNull.Value && row[column].ToString().Trim() != string.Empty)
                        {
                            result = row[column].ToString().Trim();
                        }
                    }
                    // If no fieldID provided but fieldName is provided then use the name to get the id.
                    else if (objectNameColumnName != null && input.Tables[0].Columns[objectNameColumnName] != null)
                    {
                        DataColumn column = input.Tables[0].Columns[objectNameColumnName];
                        if (row[column] != DBNull.Value && row[column].ToString().Trim() != string.Empty)
                        {
                            result = GetIDWithName(loginUser, row[column].ToString().Trim(), parentID);
                        }
                    }
                }
            }
            return result;
        }

        public static DataTable DecodeDataTable(DataTable table)
        {
            if (table.Columns.Contains("Action Description"))
            {
                foreach (DataRow objRow in table.Rows)
                {
                    objRow.BeginEdit();

                    objRow["Action Description"] = HttpUtility.HtmlDecode(objRow["Action Description"].ToString());

                    objRow.EndEdit();
                }
            }

            return table;
        }

        public static DataTable StripHtmlDataTable(DataTable table)
        {
            
            char[] chars = new char[] { '<', '\\' };
            foreach (DataRow objRow in table.Rows)
            {
                for (int i = 0; i < objRow.ItemArray.Count(); i++)
                {
                    //Check first to see whether there's potentially any HTML to strip out, because Strip() is slow
                    if (objRow[i].GetType().Name == "String" && (objRow[i].ToString().IndexOfAny(chars, 0) >= 0))
                    {
                        objRow.BeginEdit();
                        objRow[i] = HtmlUtility.StripHTML(objRow[i].ToString());
                        objRow.EndEdit();
                    }
                }
            }

            return table;
        }

		public static string CleanValueScript(string originalValue)
		{
			string cleanValue = originalValue;

			List<string> eventsList = Chat.GetHTMLGlobalEventAttributes();
			bool containsProhibitedText = false;
			int i = 0;

			while (i < eventsList.Count && !containsProhibitedText)
			{
				containsProhibitedText = originalValue.Trim().IndexOf("<script ") >= 0 || originalValue.Trim().IndexOf("<script>") >= 0 || originalValue.Trim().Contains(string.Format(" {0}=", eventsList[i]));
				i++;
			}

			if (containsProhibitedText)
			{
				cleanValue = Microsoft.Security.Application.Encoder.HtmlEncode(originalValue);
			}

			return cleanValue;
		}

        #region "API Common methods"

        /// <summary>
        /// Add paging to GET API requests
        /// </summary>
        /// <param name="sql">Generated sql in the API project for the request</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="pageNumber">Page Number</param>
        /// <param name="command">sql command</param>
        /// <returns>The sql statement with the paging statements added to it. (For SQL 2008 and above)</returns>
        public static string AddPaging(string sql, int? pageSize, int? pageNumber, SqlCommand command)
        {
            if (pageSize != null || pageNumber != null)
            {
                //These are the default values if any of them is null
                pageSize = pageSize ?? 50;
                pageNumber = pageNumber ?? 1;

                if (!string.IsNullOrEmpty(sql))
                {
                    sql = string.Format("{0} OFFSET ((@PageNumber - 1) * @PageSize) ROWS FETCH NEXT @PageSize ROWS ONLY;", sql);
                    sql = sql.Insert(sql.IndexOf("SELECT") + "SELECT ".Length, "TotalRecords = COUNT(1) OVER(), ");

                    command.Parameters.AddWithValue("@PageNumber", (int)pageNumber);
                    command.Parameters.AddWithValue("@PageSize", (int)pageSize);
                }
            }

            return sql;
        }

        public static string BuildWhereClausesFromFilters(LoginUser loginUser,
                                                        BaseCollection baseCollection,
                                                        int organizationId,
                                                        System.Collections.Specialized.NameValueCollection filters,
                                                        ReferenceType refType,
                                                        string tableIdColName,
                                                        int? ticketTypeId,
                                                        ref SqlParameterCollection filterParameters)
        {
            StringBuilder result = new StringBuilder();
            CustomFields customFields = new CustomFields(loginUser);

            if (ticketTypeId == null)
            {
                customFields.LoadByReferenceType(organizationId, refType);
            }
            else
            {
                customFields.LoadByReferenceType(organizationId, refType, ticketTypeId);
            }

            StringBuilder filterFieldName;
            StringBuilder filterOperator;
            List<string> filterValues;
            CustomField customField = null;

            DataTable tableSchema = baseCollection.Table;
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                tableSchema = connection.GetSchema("Columns", new string[] { null, null, baseCollection.TableName });
                connection.Close();
            }

            foreach (string key in filters)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    filterFieldName = new StringBuilder();
                    filterOperator = new StringBuilder();
                    filterValues = new List<string>();

                    filterFieldName = GetFilterFieldName(baseCollection, key, filters.GetValues(key), customFields, ref filterOperator, ref filterValues, ref customField);

                    if (filterFieldName.Length > 0)
                    {
                        result.Append(" AND ");

                        if (customField == null)
                        {
                            int size = 0;
                            SqlDbType type = SqlDbType.BigInt;
                            bool hasSize = false;

                            if (tableSchema != null && tableSchema.Rows != null && tableSchema.Rows.Count > 0)
                            {
                                //ref: https://msdn.microsoft.com/en-us/library/ms254969.aspx
                                string schemaCollectionColumnName = "column_name";
                                string schemaCollectionColumnSize = "character_maximum_length";
                                string schemaCollectionColumnDataType = "data_type";

                                foreach (DataRow row in tableSchema.Rows)
                                {
                                    if (row.Table.Columns.Contains(schemaCollectionColumnName)
                                        && row.Table.Columns.Contains(schemaCollectionColumnSize)
                                        && row.Table.Columns.Contains(schemaCollectionColumnDataType)
                                        && row[schemaCollectionColumnName].ToString().ToLower() == filterFieldName.ToString().ToLower())
                                    {
                                        try
                                        {
                                            string dataType = row[schemaCollectionColumnDataType].ToString();
                                            type = (SqlDbType)Enum.Parse(typeof(SqlDbType), dataType, true);

                                            switch (type)
                                            {
                                                case SqlDbType.Char:
                                                case SqlDbType.NChar:
                                                case SqlDbType.NVarChar:
                                                case SqlDbType.VarChar:
                                                    hasSize = int.TryParse(row[schemaCollectionColumnSize].ToString(), out size);
                                                    break;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            hasSize = false;
                                        }

                                        break;
                                    }
                                }
                            }

                            if (filterValues.Count > 1)
                                result.Append("(");

                            for (int j = 0; j < filterValues.Count; j++)
                            {
								bool isBetween = filterOperator.ToString().Trim().ToUpper() == "BETWEEN";

								//If is a BETWEEN filter statement we must have only two values, if not then add a 1=1 (due to the already appended 'AND' of a few lines above)
								if (isBetween && filterValues.Count != 2)
								{
									result.Append(" 1=1 ");
									break;
								}

								if (j > 0 && !isBetween)
								{
                                    result.Append(" OR ");
								}
								else if (j > 0 && isBetween)
								{
									result.Append(" AND ");
								}

                                if (filterValues[j] == null)
                                {
                                    string notEmptyOperator = filterOperator.ToString().ToLower() == "is not" ? "<>" : "=";
                                    result.Append("(");
                                    result.Append(filterFieldName + " " + filterOperator + " NULL");
                                    result.Append(" OR ");
                                    result.Append(filterFieldName + " " + notEmptyOperator + " ''");
                                    result.Append(")");
                                }
                                else
                                {
                                    string parameterIndex = "";

                                    if (j > 0)
                                        parameterIndex = j.ToString();

									if (!isBetween || (isBetween && j == 0))
									{
										result.Append(filterFieldName + " " + filterOperator + " @" + filterFieldName + parameterIndex);
									}
									else if (isBetween && j >0)
									{
										result.Append(" @" + filterFieldName + parameterIndex);
									}

                                    if (hasSize)
                                    {
                                        filterParameters.Add("@" + filterFieldName + parameterIndex, type, size);
                                        filterParameters["@" + filterFieldName + parameterIndex].Value = filterValues[j];
                                    }
                                    else
                                    {
                                        filterParameters.AddWithValue("@" + filterFieldName + parameterIndex, filterValues[j]);
                                    }
								}
							}

                            if (filterValues.Count > 1)
                                result.Append(")");
                        }
                        else
                        {
                            result.Append(tableIdColName + " IN (SELECT RefID FROM CustomValues WHERE CustomFieldID = ");
                            result.Append(customField.CustomFieldID.ToString());
                            result.Append(" AND ");
                            if (filterValues.Count > 1) result.Append("(");
                            result.Append("CustomValue " + filterOperator + " @" + filterFieldName);
                            filterParameters.AddWithValue("@" + filterFieldName, filterValues[0]);

                            if (filterValues.Count > 1)
                            {
                                for (int j = 1; j < filterValues.Count; j++)
                                {
                                    result.Append(" OR ");
                                    result.Append("CustomValue " + filterOperator + " @" + filterFieldName + j.ToString());
                                    filterParameters.AddWithValue("@" + filterFieldName + j.ToString(), filterValues[j]);
                                }

                                result.Append(")");
                            }

                            result.Append(")");
                        }
                    }
                }
            }

            return result.ToString();
        }

        private static StringBuilder GetFilterFieldName(BaseCollection baseCollection, string rawFieldName, string[] rawValues, CustomFields customFields, ref StringBuilder filterOperator, ref List<string> filterValues, ref CustomField customField)
        {
            StringBuilder result = new StringBuilder();
            string rawOperator = "=";

            if (rawFieldName.Contains('['))
            {
                int index = rawFieldName.IndexOf('[');
                result.Append(rawFieldName.Substring(0, index));
                rawOperator = Regex.Match(rawFieldName, @"\[([^]]*)\]").Groups[1].Value;
            }
            else
            {
                result.Append(rawFieldName);
            }

            Type filterFieldDataType = GetFilterFieldDataType(result.ToString(), customFields, baseCollection, ref customField);

            if (filterFieldDataType != null)
            {
                filterOperator.Append(GetSqlOperator(filterFieldDataType, rawOperator, rawValues, ref filterValues));

                if (filterOperator.Length == 0)
                {
                    result.Clear();
                }
            }
            else
            {
                result.Clear();
            }

            return result;
        }

        private static Type GetFilterFieldDataType(string fieldName, CustomFields customFields, BaseCollection baseCollection, ref CustomField customField)
        {
            Type fieldDataType = null;
            FieldMap fieldMap = baseCollection.FieldMap;
            DataTable table = baseCollection.Table;
            string field = fieldMap.GetPrivateField(fieldName);

            if (!string.IsNullOrEmpty(field))
            {
                if (table.Rows == null || table.Rows.Count == 0)
                {
                    DataRow row = table.NewRow();
                    table.Rows.Add(row);
                }

                BaseItem baseItem = new BaseItem(table.Rows[0], baseCollection);
                object fieldObject = baseItem.Row[field];

                if (fieldObject != null)
                {
                    customField = null;
                    fieldDataType = baseItem.Row.Table.Columns[field].DataType;
                }
            }
            else
            {
                customField = customFields.FindByApiFieldName(fieldName);

                if (customField != null)
                {
                    switch (customField.FieldType)
                    {
                        case CustomFieldType.Boolean:
                            Boolean boolean = true;
                            fieldDataType = boolean.GetType();
                            break;
                        case CustomFieldType.Date:
                        case CustomFieldType.DateTime:
                        case CustomFieldType.Time:
                            DateTime dateTime = DateTime.Now;
                            fieldDataType = dateTime.GetType();
                            break;
                        case CustomFieldType.Number:
                            int integer = 0;
                            fieldDataType = integer.GetType();
                            break;
                        case CustomFieldType.PickList:
                        case CustomFieldType.Text:
                            string text = string.Empty;
                            fieldDataType = text.GetType();
                            break;
                        default:
                            fieldDataType = null;
                            break;
                    }
                }
            }

            return fieldDataType;
        }

        private static string GetSqlOperator(Type filterFieldDataType, string rawOperator, string[] rawValues, ref List<string> filterValues)
        {
            rawOperator = rawOperator.ToLower();
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < rawValues.Length; i++)
            {
                if (rawValues[i].ToLower() == "[null]")
                {
                    if (i == 0)
                    {
                        if (rawOperator == "not")
                        {
                            result.Append("IS NOT");
                        }
                        else
                        {
                            result.Append("IS");
                        }
                    }

                    filterValues.Add(null);
                }
                else
                {
                    if (filterFieldDataType == typeof(System.DateTime))
                    {
                        //format needs to be: yyyymmddhhmmss
                        if (rawValues[i].Length == "yyyymmddhhmmss".Length)
                        {
                            StringBuilder filterValue = new StringBuilder();
                            //sql default datetime format "yyyy-mm-dd hh:mm:ss"
                            //yyyy
                            filterValue.Append(rawValues[i].Substring(0, 4));
                            filterValue.Append("-");
                            //mm
                            filterValue.Append(rawValues[i].Substring(4, 2));
                            filterValue.Append("-");
                            //dd
                            filterValue.Append(rawValues[i].Substring(6, 2));
                            filterValue.Append(" ");
                            //hh
                            filterValue.Append(rawValues[i].Substring(8, 2));
                            filterValue.Append(":");
                            //mm
                            filterValue.Append(rawValues[i].Substring(10, 2));
                            filterValue.Append(":");
                            //ss
                            filterValue.Append(rawValues[i].Substring(12, 2));

                            filterValues.Add(filterValue.ToString());

							if (i > 0)
								filterValues.Sort();

                            if (i == 0)
                            {
                                if (rawOperator == "lt")
                                {
                                    result.Append("<");
                                }
								else if (rawOperator == "bt")
								{
									result.Append("BETWEEN");
								}
                                else
                                {
                                    result.Append(">");
                                }
                            }
                        }
                    }
                    else if (filterFieldDataType == typeof(System.Boolean))
                    {
                        if (rawValues[i].ToLower().IndexOf("t") > -1 || rawValues[i].ToLower().IndexOf("1") > -1 || rawValues[i].ToLower().IndexOf("y") > -1)
                        {
                            filterValues.Add("true");
                        }
                        else if (rawValues[i].ToLower().IndexOf("f") > -1 || rawValues[i].ToLower().IndexOf("0") > -1 || rawValues[i].ToLower().IndexOf("n") > -1)
                        {
                            filterValues.Add("false");
                        }
                        if (i == 0)
                        {
                            result.Append("=");
                        }
                    }
                    else if (filterFieldDataType == typeof(System.Double))
                    {
                        double d = double.Parse(rawValues[i]);
                        filterValues.Add(d.ToString());

                        if (i == 0)
                        {
                            switch (rawOperator)
                            {
                                case "lt": result.Append("<"); break;
                                case "lte": result.Append("<="); break;
                                case "gt": result.Append(">"); break;
                                case "gte": result.Append(">="); break;
                                case "not": result.Append("<>"); break;
                                default: result.Append("="); break;
                            }
                        }
                    }
                    else if (filterFieldDataType == typeof(System.Int32))
                    {
                        try
                        {
                            int j = int.Parse(rawValues[i]);
                            filterValues.Add(j.ToString());
                        }
                        catch (OverflowException oEx)
                        {
                            long l = long.Parse(rawValues[i]);
                            filterValues.Add(l.ToString());
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        if (i == 0)
                        {
                            switch (rawOperator)
                            {
                                case "lt": result.Append("<"); break;
                                case "lte": result.Append("<="); break;
                                case "gt": result.Append(">"); break;
                                case "gte": result.Append(">="); break;
                                case "not": result.Append("<>"); break;
                                default: result.Append("="); break;
                            }
                        }
                    }
                    else
                    {
                        switch (rawOperator)
                        {
                            case "contains":
                                if (i == 0) result.Append("LIKE");
                                filterValues.Add("%" + rawValues[i] + "%");
                                break;
                            case "not":
                                if (i == 0) result.Append("<>");
                                filterValues.Add(rawValues[i]);
                                break;
                            case "doesnotcontain":
                                if (i == 0) result.Append("NOT LIKE");
                                filterValues.Add("%" + rawValues[i] + "%");
                                break;
                            default:
                                if (i == 0) result.Append("=");
                                filterValues.Add(rawValues[i]);
                                break;
                        }
                    }
                }
            }

            return result.ToString();
        }

        #endregion
    }
}
