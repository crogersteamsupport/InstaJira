using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using TeamSupport.Data;


namespace TeamSupport.WebUtils
{
  public class TSAuthentication
  {
    public static FormsAuthenticationTicket Ticket
    {
      get 
      {
        if (HttpContext.Current.User == null) return null;
        if (HttpContext.Current.User.Identity is FormsIdentity)
        {
          return (HttpContext.Current.User.Identity as FormsIdentity).Ticket;
        }
        return null;
      }
    }

    public static int UserID
    {
      get
      {
        if (Ticket == null) return -1;
        return int.Parse(Ticket.UserData.Split('|')[0]);
      }
    }

    public static int OrganizationID
    {
      get
      {
        if (Ticket == null) return -1;
        return int.Parse(Ticket.UserData.Split('|')[1]);
      }
    }

    public static string UserData
    {
      get 
      {
        if (Ticket == null) return "";
        return Ticket.UserData; 
      }
    }

    public static bool IsBackdoor
    {
      get
      {
        if (Ticket == null) return false;
        string[] data = Ticket.UserData.Split('|');
        if (data.Length < 3) return false;
        return data[2] == "1";
      }
    }

    public static bool IsSystemAdmin
    {
      get
      {
        if (Ticket == null) return false;
        string[] data = Ticket.UserData.Split('|');
        return data[4] == "1";
      }
    }

    public static string SessionID
    {
      get
      {
        if (Ticket == null) return "";

        string[] data = Ticket.UserData.Split('|');
        if (data.Length < 4) return "";
        return data[3];
      }
    }

    public static bool IsTSSuperAdmin
    {
      get 
      {
        return TSAuthentication.IsSystemAdmin && (TSAuthentication.UserID == 34 || TSAuthentication.UserID == 43 || TSAuthentication.UserID == 257660) && !TSAuthentication.IsBackdoor;
      }
    }

    public static int TimeOut 
    {
      get 
      {
        return int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["SessionTimeOut"]);
      }
    }

    public static void SlideExpiration()
    {
      FormsAuthenticationTicket ticket = (HttpContext.Current.User.Identity as FormsIdentity).Ticket;
      if (ticket == null) return;
      ticket = new FormsAuthenticationTicket(1, ticket.Name, DateTime.UtcNow, DateTime.UtcNow.AddSeconds(TimeOut), false, ticket.UserData, FormsAuthentication.FormsCookiePath);
      HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
      cookie.Domain = FormsAuthentication.CookieDomain;
      cookie.HttpOnly = true;
      cookie.Expires = DateTime.UtcNow.AddYears(1);
      HttpContext.Current.Response.Cookies.Add(cookie);
    }

    public static void Authenticate(User user, bool isBackdoor, string deviceID)
    {
      if (IsAuthenticated(user, isBackdoor))
      {
        SlideExpiration();
      }
      else
      {
        Guid guid = Guid.NewGuid();
        string userData = GetUserDataString(user.UserID, user.OrganizationID, isBackdoor, guid.ToString(), user.IsSystemAdmin);
        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.DisplayName, DateTime.UtcNow, DateTime.UtcNow.AddSeconds(TimeOut), false, userData, FormsAuthentication.FormsCookiePath);
        string encTicket = FormsAuthentication.Encrypt(ticket);
        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
        cookie.HttpOnly = true;
        cookie.Domain = FormsAuthentication.CookieDomain;
        cookie.Expires = DateTime.UtcNow.AddYears(1);
        HttpContext.Current.Response.Cookies.Add(cookie);

        if (!isBackdoor){

          user.LastLogin = DateTime.UtcNow;
          user.SessionID = guid;
          user.LastActivity = DateTime.UtcNow;
          user.Collection.Save();
        }

        HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
        LoginHistoryItem history = (new LoginHistory(LoginUser.Anonymous)).AddNewLoginHistoryItem();
        history.UserID = user.UserID;
        history.Browser = DataUtils.GetBrowserName(HttpContext.Current.Request.UserAgent);
        history.Version = bc.Version;
        history.MajorVersion = bc.MajorVersion.ToString();
        history.UserAgent = HttpContext.Current.Request.UserAgent;
        history.Language = "";
        history.Platform = bc.Platform;
        history.CookiesEnabled = bc.Cookies;
        history.IPAddress = HttpContext.Current.Request.UserHostAddress;
        history.PixelDepth = bc.ScreenBitDepth.ToString();
        history.ScreenHeight = bc.ScreenPixelsHeight.ToString();
        history.ScreenWidth = bc.ScreenPixelsWidth.ToString();
        history.URL = (isBackdoor ? "BACKDOOR - " : "") + HttpContext.Current.Request.Url.OriginalString;
        history.DeviceID = deviceID;
        history.IsSupport = isBackdoor;

        history.Collection.Save();
        
      }
    }

    public static bool IsAuthenticated(User user, bool isBackdoor)
    {
      return IsAuthenticated(user) && isBackdoor == IsBackdoor;
    }

    public static bool IsAuthenticated()
    {
            return Ticket != null && !Ticket.Expired;
    }

        public static bool IsAuthenticated(User user)
    {
      int length = UserData.Split('|').Length;
      return Ticket != null && !Ticket.Expired && user.UserID == UserID && user.OrganizationID == OrganizationID && length == 5;
    }

    private static string GetUserDataString(int userID, int organizationID, bool isBackdoor, string sessionID, bool isAdmin)
    {
      return userID.ToString() + "|" + organizationID.ToString() + "|" + (isBackdoor ? "1" : "0") + "|" + sessionID + "|" + (isAdmin ? "1" : "0");
    }

    public static LoginUser GetLoginUser()
    {
      return new LoginUser(UserID, OrganizationID, null);
    }

    public static User GetUser(LoginUser loginUser)
    {
      return Users.GetUser(loginUser, UserID);
    }

	public static List<int> GetUserGroups()
    {
		List<int> groups = new List<int>();

		using (SqlConnection connection = new SqlConnection(LoginUser.Anonymous.ConnectionString))
		{
			connection.Open();
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.CommandText = "SELECT GroupID FROM GroupUsers WITH(NOLOCK) WHERE UserID = @UserID";
			command.Parameters.AddWithValue("UserID", TSAuthentication.UserID);
			SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
			DataTable groupUsersTable = new DataTable();
			groupUsersTable.Load(reader);

			for (int i = 0; i < groupUsersTable.Rows.Count; i++)
            {
				groups.Add((int)groupUsersTable.Rows[i]["GroupID"]);
            }

			reader.Close();
		}

			return groups;
    }

    public static Organization GetOrganization(LoginUser loginUser)
    {
      return Organizations.GetOrganization(loginUser, OrganizationID);
    }

  }
}
