using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Configuration;

namespace TeamSupport.Data
{
    [Serializable]
    public class LoginUser
    {
        private static LoginUser _anonymous = null;

        public LoginUser(int userID, int organizationID)
        {
            _connectionString = GetConnectionString(userID);
            _userID = userID;
            _organizationID = organizationID;
            _dataCache = null;
            _timeZoneInfo = null;
        }

        public LoginUser(int userID, int organizationID, IDataCache dataCache)
        {
            _connectionString = GetConnectionString(userID);
            _userID = userID;
            _organizationID = organizationID;
            _dataCache = dataCache;

            _timeZoneInfo = null;
        }

        public LoginUser(string connectionString, int userID, int organizationID, IDataCache dataCache)
        {
            _connectionString = connectionString;
            _userID = userID;
            _organizationID = organizationID;
            _dataCache = dataCache;

            _timeZoneInfo = null;
        }

        public static LoginUser Anonymous
        {
            get
            {
                if (_anonymous == null)
                    _anonymous = new LoginUser(GetConnectionString(-1), -1, -1, null);
                return _anonymous;
            }
        }


        public static string GetConnectionString()
        {
            return GetConnectionString(-1);
        }

        public static string GetConnectionString(int userID)
        {
            ConnectionStringSettings cStrings = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["MainConnection"];
            if (cStrings != null)
            {
                return cStrings.ConnectionString;
            }
            
            return ConfigurationManager.AppSettings["ConnectionString"];
        }

        private string _connectionString;
        public string ConnectionString
        {
            get { return _connectionString; }
        }

        private int _userID;
        public int UserID
        {
            get { return _userID; }
        }

        private int _organizationID;
        public int OrganizationID
        {
            get { return _organizationID; }
        }

        private IDataCache _dataCache;
        public IDataCache DataCache
        {
            get { return _dataCache; }
        }

        private TimeZoneInfo _timeZoneInfo = null;
        public TimeZoneInfo TimeZoneInfo
        {
            get
            {
                if (_timeZoneInfo == null)
                {

                    try
                    {
                        User user = (User)Users.GetUser(this, _userID);
                        if (user != null && user.TimeZoneID != null && user.TimeZoneID != "")
                        {
                            _timeZoneInfo = System.TimeZoneInfo.FindSystemTimeZoneById(user.TimeZoneID);
                        }
                        else
                        {
                            Organization organization = (Organization)Organizations.GetOrganization(this, _organizationID);
                            if (organization != null && organization.TimeZoneID != null && organization.TimeZoneID != "")
                            {
                                _timeZoneInfo = System.TimeZoneInfo.FindSystemTimeZoneById(organization.TimeZoneID);
                            }
                        }

                    }
                    catch (Exception)
                    {
                        _timeZoneInfo = null;
                    }

                    if (_timeZoneInfo == null)
                    {
                        _timeZoneInfo = System.TimeZoneInfo.Local;
                    }

                }

                return _timeZoneInfo;
            }
            set { _timeZoneInfo = value; }
        }

        private TimeSpan _offset = new TimeSpan(0);

        public TimeSpan Offset
        {
            get
            {
                if (_offset.Ticks == 0)
                {
                    _offset = TimeZoneInfo.GetUtcOffset(DataUtils.DateToLocal(this, DateTime.UtcNow));
                }
                return _offset;
            }
        }


        private CultureInfo _cultureInfo = null;
        public CultureInfo CultureInfo
        {
            get
            {
                if (_cultureInfo == null)
                {

                    try
                    {
                        User user = Users.GetUser(this, _userID);
                        if (user != null && !string.IsNullOrEmpty(user.CultureName))
                        {
                            _cultureInfo = new CultureInfo(user.CultureName);
                        }
                        else
                        {
                            Organization organization = Organizations.GetOrganization(this, _organizationID);
                            if (organization != null)
                            {
                                _cultureInfo = new CultureInfo(organization.CultureName);
                            }
                        }

                        if (_cultureInfo.IsNeutralCulture)
                        {
                            _cultureInfo = new CultureInfo("en-US");
                        }
                    }
                    catch (Exception)
                    {
                        _cultureInfo = null;
                    }

                    if (_cultureInfo == null)
                    {
                        _cultureInfo = new CultureInfo("en-US");
                    }
                }


                return _cultureInfo;
            }
            set { _cultureInfo = value; }
        }

        private CultureInfo _organizationCulture = null;
        public CultureInfo OrganizationCulture
        {
            get
            {
                if (_organizationCulture == null)
                {

                    try
                    {
                        Organization organization = Organizations.GetOrganization(this, _organizationID);
                        if (organization != null)
                        {
                            _organizationCulture = new CultureInfo(organization.CultureName);
                        }

                        if (CultureInfo.IsNeutralCulture)
                        {
                            _cultureInfo = new CultureInfo("en-US");
                        }
                    }
                    catch (Exception)
                    {
                        _organizationCulture = null;
                    }

                    if (_organizationCulture == null)
                    {
                        _organizationCulture = new CultureInfo("en-US");
                    }
                }


                return _organizationCulture;
            }
            set { _organizationCulture = value; }
        }

        public string GetUserFullName()
        {
            return Users.GetUserFullName(this, _userID);
        }

        public User GetUser()
        {
            return Users.GetUser(this, this.UserID);
        }

				public Organization GetOrganization()
				{
					return Organizations.GetOrganization(this, this.OrganizationID);
				}
    }
}
