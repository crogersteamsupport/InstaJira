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
                //if (HttpContext.Current.Session["LoginUser"] == null)
                //{
                //    HttpContext.Current.Session.Remove("PostAuthToken");
                //    if (HttpContext.Current.User.Identity is FormsIdentity)
                //    {
                //        FormsAuthenticationTicket ticket = (HttpContext.Current.User.Identity as FormsIdentity).Ticket;
                //        string[] userData = ticket.UserData.Split('|');
                //        if (userData.Length > 1)
                //        {
                //            int userID = int.Parse(userData[0]);
                //            int organizationID = int.Parse(userData[1]);
                //            //LoginUser loginUser = new LoginUser(ConnectionString, userID, organizationID, new WebDataCache());
                //            //HttpContext.Current.Session["LoginUser"] = loginUser;
                //            return TSAuthentication.GetLoginUser();
                //        }
                //    }
                //}
                //else
                //{
                //    return (LoginUser)HttpContext.Current.Session["LoginUser"];
                //}

                //return null;
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
            //HttpContext.Current.Session.Remove("CurrentUserInfo");
        }

        public static void RefreshPostAuthToken()
        {
            //HttpContext.Current.Session.Remove("PostAuthToken");
        }

        public static void RefreshLoginUser()
        {
            //HttpContext.Current.Session.Remove("LoginUser");
        }

        private static bool IsSessionValid()
        {
            /*

            if ((HttpContext.Current.Session != null && HttpContext.Current.Session.IsNewSession))
            {
              string cookie = HttpContext.Current.Request.Headers["Cookie"];
              if (cookie != null && cookie.IndexOf("ASP.NET_SessionId") >= 0)
              {
                HttpContext.Current.Session.Abandon();
                return false;
              }
            }

            if (HttpContext.Current.Session["LoginUser"] == null)
            {
              HttpContext.Current.Session.Abandon();
              return false;
            }
            */
            return true;
        }

        public static string PostAuthenticationToken
        {
            get
            {
                //if (HttpContext.Current.Session["PostAuthToken"] == null)
                //{
                //    HttpContext.Current.Session["PostAuthToken"] = Guid.NewGuid().ToString();
                //}
                //return (string)HttpContext.Current.Session["PostAuthToken"];
                return "";
            }
        }

        public static int GetID(string key)
        {
            return -1;
            //return HttpContext.Current.Session[key] == null ? -1 : (int)HttpContext.Current.Session[key];
        }

        public static void SetID(string key, int id)
        {
            //HttpContext.Current.Session[key] = id;
        }

        private static object GetValue(string key)
        {
            return null;
            //if (HttpContext.Current.Session != null && HttpContext.Current.Session[key] != null)
            //{
            //    return HttpContext.Current.Session[key];
            //}
            //else
            //{
            //    //HttpContext.Current.Response.Redirect("~/Login.aspx");
            //    return null;

            //}
        }

    }
}
