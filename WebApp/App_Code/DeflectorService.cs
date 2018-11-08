using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Runtime.Serialization;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Diagnostics;
using ImageResizer;
using System.Net;
using System.IO;
using System.Dynamic;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Threading.Tasks;
using System.Runtime;

namespace TSWebServices {

    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class DeflectorService : System.Web.Services.WebService {

        [WebMethod]
        public async Task<string> TestDeflectorAPI(string tag) {
            try
            {
                DeflectorAPI deflectorAPI = new DeflectorAPI();
                return await deflectorAPI.TestDeflectorAPIAsync(tag);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
                return null;
            }
        }

        [WebMethod]
        public async Task<string> FetchDeflections(int organization, string phrase, int? customerHubID)
        {
            try
            {
                List<DeflectorReturn> filteredList = new List<DeflectorReturn>();

                if (Convert.ToBoolean(OrganizationSettings.ReadString(LoginUser.Anonymous, organization, "ChatTicketDeflectionEnabled", "False"))) { 
                    //Customer Hub
                    if (customerHubID != null)
                    {
                        filteredList = Deflector.FetchHubDeflections(organization, phrase, (int)customerHubID);
                    }
                    //Portal
                }

                return JsonConvert.SerializeObject(filteredList);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
                return "[]";
            }
        }

        public void IndexDeflector(string deflectorIndex)
        {
            try
            {
                DeflectorAPI deflectorAPI = new DeflectorAPI();
                deflectorAPI.IndexDeflectorAsync(deflectorIndex);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
            }
        }

        public void IndexTicket(int ticketID)
        {
            Ticket Ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            Tags Tags = new Tags(TSAuthentication.GetLoginUser());
            Tags.LoadByReference(ReferenceType.Tickets, ticketID);
            DeflectorService Deflection = new DeflectorService();

            List<DeflectorIndex> deflectorIndexList = new List<DeflectorIndex>();

            foreach (var Tag in Tags)
            {
                deflectorIndexList.Add(new DeflectorIndex
                {
                    TicketID = Ticket.TicketID,
                    Name = Ticket.Name,
                    OrganizationID = Ticket.OrganizationID,
                    TagID = Tag.TagID,
                    Value = Tag.Value,
                    ProductID = Ticket.ProductID
                });
                
            }

            try
            {
                DeflectorAPI deflectorAPI = new DeflectorAPI();
                deflectorAPI.BulkIndexDeflectorAsync(Newtonsoft.Json.JsonConvert.SerializeObject(deflectorIndexList));
            }
            catch (Exception ex){
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
            }
        }

        [WebMethod]
        public void HydrateOrganization(int organizationID)
        {
            string response = TeamSupport.Data.Deflector.GetOrganizationIndeces(TSAuthentication.GetLoginUser(), organizationID);

            try
            {
                DeflectorAPI deflectorAPI = new DeflectorAPI();
                deflectorAPI.BulkIndexDeflectorAsync(response);
            }
            catch (Exception ex) {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
            }
        }

        [WebMethod]
        public void HydratePod()
        {
            List<String> indeceses = TeamSupport.Data.Deflector.GetPodIndeces(TSAuthentication.GetLoginUser());
            foreach (string index in indeceses)
            {
                try
                {
                    DeflectorAPI deflectorAPI = new DeflectorAPI();
                    deflectorAPI.BulkIndexDeflectorAsync(index);
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
                }
            }
        }

        public void DeleteTicket(int ticketID)
        {
            try
            {
                DeflectorAPI deflectorAPI = new DeflectorAPI();
                deflectorAPI.DeleteTicketAsync(ticketID);
            }
            catch (Exception ex) {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
            }
        }

        public void DeleteTag(int organizationID, string value)
        {

            try
            {
                DeflectorAPI deflectorAPI = new DeflectorAPI();
                deflectorAPI.DeleteTagAsync(organizationID, value);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
            }
        }

        public void RemoveTag(int organizationID, int ticketID, string value)
        {

            try
            {
                DeflectorAPI deflectorAPI = new DeflectorAPI();
                deflectorAPI.RemoveTagAsync(organizationID, ticketID, value);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
            }
        }
    }

}
