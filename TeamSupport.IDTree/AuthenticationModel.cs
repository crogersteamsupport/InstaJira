using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Configuration;
using System.Web.Configuration;
using System.Web;
using System.Security.Authentication;

namespace TeamSupport.IDTree
{
    public class AuthenticationModel
    {
        string[] _userData;

        public FormsAuthenticationTicket AuthenticationTicket { get; private set; }
        public int UserID { get { return int.Parse(_userData[0]); } }
        public int OrganizationID { get; private set; }
        public bool IsBackdoor { get { return (_userData[2] == "1"); } }
        public string SessionID { get { return _userData[3]; } }
        public bool IsSystemAdmin { get { return (_userData[4] == "1"); } }
        public string ConnectionString { get; private set; }

        public static bool AuthenticatedUser
        {
            get { return (HttpContext.Current.User != null) && (HttpContext.Current.User.Identity is FormsIdentity);  }
        }

        public AuthenticationModel()
        {
            // Authentication from HttpContext
            if (AuthenticatedUser)
            {
                // use HttpContext on thread
                AuthenticationTicket = (HttpContext.Current.User.Identity as FormsIdentity).Ticket;
                _userData = AuthenticationTicket.UserData.Split('|');
                OrganizationID = int.Parse(_userData[1]);
            }
            else
            {
                // support anonymous user from URL
                _userData = new string[] { "0", "0", "0", "0", "0" };
                string[] segments = HttpContext.Current.Request.Url.Segments;
                string organizationIDstring = segments[segments.Length - 2].ToLower().Trim().Replace("/", "");
                int organizationID = 0;
                if (int.TryParse(organizationIDstring, out organizationID))  // second to last in URL is OrganizationID
                    OrganizationID = organizationID;
            }

            // Connection string
            ConnectionStringSettings cStrings = WebConfigurationManager.ConnectionStrings["MainConnection"];
            ConnectionString = (cStrings != null) ? cStrings.ConnectionString : ConfigurationManager.AppSettings["ConnectionString"];
        }

        private AuthenticationModel(string userData, string connectionString)
        {
            _userData = userData.Split('|');
            ConnectionString = connectionString;
            OrganizationID = int.Parse(_userData[1]);
        }

        public static AuthenticationModel AuthenticationModelTest(string userData, string connectionString)
        {
            return new AuthenticationModel(userData, connectionString);
        }
    }
}
