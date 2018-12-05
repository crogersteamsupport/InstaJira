using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using TeamSupport.Data;
using System.Web.Security;

namespace TeamSupport.WebUtils
{
    public class UserSession
    {

        private UserSession()
        {

        }

        public static string ConnectionString
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString;
            }
        }

        public static LoginUser LoginUser
        {
            get
            {
                return TSAuthentication.GetLoginUser();
            }
        }

        public static UserInfo CurrentUser
        {
            get
            {
                UserInfo result = null;
                User user = (User)Users.GetUser(LoginUser, LoginUser.UserID);
                if (user != null)
                {
                    result = new UserInfo(user);
                }
                else
                {
                    HttpContext.Current.Session.Clear();
                    HttpContext.Current.Response.Cookies.Clear();
                    FormsAuthentication.SignOut();
                    HttpContext.Current.Session.Abandon();
                    HttpContext.Current.Response.Redirect("~/login");

                }

                return result;
            }
        }

        public static bool IsAuthenticated()
        {
            if (HttpContext.Current.User.Identity is FormsIdentity)
            {
                FormsAuthenticationTicket ticket = (HttpContext.Current.User.Identity as FormsIdentity).Ticket;
                return !ticket.Expired;
            }
            return false;
        }


        public static void RefreshCurrentUserInfo()
        {

        }

        public static void RefreshPostAuthToken()
        {

        }

        public static void RefreshLoginUser()
        {

        }

        private static bool IsSessionValid()
        {
            return true;
        }

        public static string PostAuthenticationToken
        {
            get
            {
                return "";
            }
        }

        public static int GetID(string key)
        {
            return -1;
        }

        public static void SetID(string key, int id)
        {
        }

        private static object GetValue(string key)
        {
            return null;
        }

    }
}
