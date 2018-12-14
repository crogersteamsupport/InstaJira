using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using TeamSupport.Messaging;
using Newtonsoft.Json;
using System.Configuration;

namespace TSWebServices
{
	[ScriptService]
	[WebService(Namespace = "http://teamsupport.com/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class LoginService : System.Web.Services.WebService
	{
		private const int MAXLOGINATTEMPTS = 10;
		private const int MINUTESTOEXPIREVERIFICATIONCODE = 5;
		private static bool _skipVerification = false;

		public LoginService()
		{

			//Uncomment the following line if using designed components
			//InitializeComponent();
		}

		[WebMethod]
		[ScriptMethod(ResponseFormat=ResponseFormat.Json)]
		public string NewSignUpSignIn(int userID)
		{
            User user = Users.GetUser(LoginUser.Anonymous, userID);
            return DoSignIn(user.Email, "", user.OrganizationID, false, false);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SignIn(string email, string password, int organizationId, bool verificationRequired, bool rememberMe)
        {
            return DoSignIn(email, password, organizationId, verificationRequired, rememberMe);
        }

        private string DoSignIn(string email, string password, int organizationId, bool verificationRequired, bool rememberMe)
        {
            try
            {
                SignInResult result = new SignInResult();
                LoginUser loginUser = LoginUser.Anonymous;
                User user = null;
                Organization organization = null;
                result = IsValid(loginUser, email, password, organizationId, ref user, ref organization);

                if (result.Result == LoginResult.Success) {

                    // DEVICE VERIFICATION // TRUE ALLOWS BYPASS.
                    UserDevices devices = new UserDevices(loginUser);
                    devices.LoadByUserIDAndDeviceID(user.UserID, GetDeviceID());
                    // if (organization.RequireTwoFactor) {
                    //    _skipVerification = false;
                    if (devices.IsEmpty) {
                        _skipVerification = false;
                    } else if (devices[0].IsActivated) {
                        _skipVerification = true;
                    } else {
                        _skipVerification = false;
                    }

                    if (organization.TwoStepVerificationEnabled && verificationRequired && !_skipVerification) {
                        string userVerificationPhoneNumber = user.verificationPhoneNumber;

                        if (!string.IsNullOrEmpty(userVerificationPhoneNumber))
                        {
                            int verificationCode = 0;
                            bool isNewVerificationCode = false;

                            if (!string.IsNullOrEmpty(user.verificationCode) && user.verificationCodeExpirationUtc > DateTime.UtcNow)
                            {
                                bool isNumeric = int.TryParse(user.verificationCode, out verificationCode);

                                if (!isNumeric)
                                {
                                    verificationCode = SendAndGetVerificationCode(userVerificationPhoneNumber);
                                    isNewVerificationCode = true;
                                }
                            }
                            else
                            {
                                verificationCode = SendAndGetVerificationCode(userVerificationPhoneNumber);
                                isNewVerificationCode = true;
                            }

                            if (verificationCode > 0)
                            {
                                if (isNewVerificationCode)
                                {
                                    user.verificationCode = verificationCode.ToString();
                                    user.verificationCodeExpiration = DateTime.UtcNow.AddMinutes(MINUTESTOEXPIREVERIFICATIONCODE);
                                    user.Collection.Save();
                                }

                                result.Result = LoginResult.VerificationNeeded;
                            }
                            else
                            {
                                result.Error = "Verification Code failed to be generated or sent.";
                                result.Result = LoginResult.Fail;
                            }
                        }
                        else
                        {
                            result.Error = "Organization requires two step verification and user does not have a verification phone number setup.";
                            result.Result = LoginResult.VerificationSetupNeeded;
                        }
                    }
                    else
                    {
                        string authenticateResult = AuthenticateUser(user.UserID, user.OrganizationID, rememberMe, false);
                    }
                }
                else if (result.Result == LoginResult.PasswordExpired)
                {
                    string authenticateResult = AuthenticateUser(user.UserID, user.OrganizationID, rememberMe, false);
                    result.RedirectURL = string.Format("LoginNewPassword.html?UserID={0}&Token={1}", user.UserID, user.CryptedPassword);
                }

                return JsonConvert.SerializeObject(result);

            }
            catch (Exception ex)
            {
                SignInResult errorResult = new SignInResult();
                errorResult.Error = "There was a error signing you in. Please verify your email and password and try again.";
                errorResult.Result = LoginResult.Fail;
                return JsonConvert.SerializeObject(ex);
            }
        }

        [WebMethod]
		[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
		public string SupportSignIn(string token)
		{
			BackdoorLogins logins = new BackdoorLogins(LoginUser.Anonymous);
			logins.LoadByToken(token);

			if (logins.Count > 0 && logins[0].DateIssuedUtc.AddMinutes(10) > DateTime.UtcNow)
			{
				User user = Users.GetUser(LoginUser.Anonymous, logins[0].ContactID);
                Organization org = Organizations.GetOrganization(LoginUser.Anonymous, user.OrganizationID);
                if (org.DisableSupportLogin == false)
                {
                    AuthenticateUser(user.UserID, user.OrganizationID, false, true);
                }
			}
			return JsonConvert.SerializeObject("");


		}

        [WebMethod]
        public string GetMobileURL(int userID)
        {
            return JsonConvert.SerializeObject(SystemSettings.GetMobileURL());
        }


        [WebMethod]
		public string CodeVerification(int userId, string codeEntered)
		{
			SignInResult result = new SignInResult();
			LoginUser loginUser = LoginUser.Anonymous;

			try
			{
				Users users = new Users(loginUser);
				users.LoadByUserID(userId);
				result.UserId = userId;

				if (users.Count > 0)
				{
					result.OrganizationId = users[0].OrganizationID;

					string codeSent = users[0].verificationCode;
					codeEntered = codeEntered.Trim();
					codeEntered = codeEntered.Replace(" ", string.Empty);

					if (codeSent == codeEntered)
					{
						if (users[0].verificationCodeExpirationUtc > DateTime.UtcNow)
						{
							users[0].verificationCode = null;
							users[0].verificationCodeExpiration = null;
							users.Save();

							UserDevices devices = new UserDevices(loginUser);
							UserDevice device = devices.AddNewUserDevice();
							device.DateActivated = DateTime.UtcNow;
							device.IsActivated = true;
							device.DeviceID = Guid.NewGuid().ToString();
							device.UserID = users[0].UserID;
							devices.Save();

							EmailPosts.SendNewDevice(loginUser, users[0].UserID);

                            HttpCookie deviceCookie = new HttpCookie("di", device.DeviceID);
                			deviceCookie.Expires = DateTime.Now.AddYears(14);
                			HttpContext.Current.Response.Cookies.Add(deviceCookie);

							result.Result = LoginResult.Success;
							string authenticateResult = AuthenticateUser(users[0].UserID, users[0].OrganizationID, false, false);
						}
						else
						{
							result.Error = "Verification Code has expired.";
							result.Result = LoginResult.Fail;
						}
					}
					else
					{
						result.Error = "Invalid Verification Code.";
						result.Result = LoginResult.Fail;
					}
				}
				else
				{
					result.Error = "User not found.";
					result.Result = LoginResult.Fail;
				}
			}
			catch (Exception ex)
			{
				result.Error = ex.Message;
				result.Result = LoginResult.Fail;
			}

			return JsonConvert.SerializeObject(result);
		}

		[WebMethod]
		public string RegenerateCodeVerification(int userId)
		{
			SignInResult result = new SignInResult();
			LoginUser loginUser = LoginUser.Anonymous;

			try
			{
				Users users = new Users(loginUser);
				users.LoadByUserID(userId);
				result.UserId = userId;

				if (users.Count > 0)
				{
					result.OrganizationId = users[0].OrganizationID;
					int verificationCode = SendAndGetVerificationCode(users[0].verificationPhoneNumber);

					if (verificationCode > 0)
					{
						users[0].verificationCode = verificationCode.ToString();
						users[0].verificationCodeExpiration = DateTime.UtcNow.AddMinutes(MINUTESTOEXPIREVERIFICATIONCODE);
						users.Save();
						result.Result = LoginResult.Success;
					}
					else
					{
						result.Error = "Verification Code failed to be generated or sent.";
						result.Result = LoginResult.Fail;
					}
				}
				else
				{
					result.Error = "User not found.";
					result.Result = LoginResult.Fail;
				}
			}
			catch (Exception ex)
			{
				result.Error = ex.Message;
				result.Result = LoginResult.Fail;
			}

			return JsonConvert.SerializeObject(result);
		}

		[WebMethod]
		public string SetupVerificationPhoneNumber(int userId, string phoneNumber, bool sendMessage)
		{
			SignInResult result = new SignInResult();
			LoginUser loginUser = LoginUser.Anonymous;
			result.UserId = userId;

			try
			{
				Users users = new Users(loginUser);
				users.LoadByUserID(userId);

				if (users.Count > 0)
				{
					result.OrganizationId = users[0].OrganizationID;
					users[0].verificationPhoneNumber = phoneNumber;
					users.Save();

					if (sendMessage)
					{
						int verificationCode = SendAndGetVerificationCode(phoneNumber);

						if (verificationCode > 0)
						{
							users[0].verificationCode = verificationCode.ToString();
							users[0].verificationCodeExpiration = DateTime.UtcNow.AddMinutes(MINUTESTOEXPIREVERIFICATIONCODE);
							users.Save();
							result.Result = LoginResult.Success;
						}
						else
						{
							result.Error = "Verification Phone Number updated but the Verification Code failed to be generated or sent.";
							result.Result = LoginResult.Fail;
						}
					}
				}
				else
				{
					result.Error = "User not found.";
					result.Result = LoginResult.Fail;
				}
			}
			catch (Exception ex)
			{
				result.Error = ex.Message;
				result.Result = LoginResult.Fail;
			}

			return JsonConvert.SerializeObject(result);
		}

		[WebMethod]
		[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
		public string GetCompanies(string email)
		{
			Organizations organizations = new Organizations(LoginUser.Anonymous);
			organizations.LoadByEmailExcludeInActive(email);
			List<ComboBoxItem> items = new List<ComboBoxItem>();
			foreach (Organization organization in organizations)
			{
				items.Add(new ComboBoxItem(organization.Name, organization.OrganizationID));
			}

			return JsonConvert.SerializeObject(items);
		}

		[WebMethod]
		public string GetUsersPhoneNumber(int userID)
		{
			User user = Users.GetUser(LoginUser.Anonymous, userID);
			return JsonConvert.SerializeObject(user.verificationPhoneNumber);
		}

		[WebMethod]
		[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
		public string GetRemoteLoginUsers(string email, string passPhrase)
		{
			string passKey = ConfigurationManager.AppSettings["RemoteLoginKey"];
			if (passKey == passPhrase)
			{
				List<LoginScanResult> scanResult = new List<LoginScanResult>();

				Organizations organizations = new Organizations(LoginUser.Anonymous);
				organizations.LoadByEmail(email);

				foreach (Organization organization in organizations)
				{
					Users users = new Users(LoginUser.Anonymous);
					users.LoadByEmail(email, organization.OrganizationID);

					foreach (User user in users)
					{
						LoginScanResult result = new LoginScanResult
						{
							OrganizationID = user.OrganizationID,
							OrganizationName = organization.Name,
							UserID = user.UserID,
							UserFullName = user.DisplayName
						};
						scanResult.Add(result);
					}
				}

				return JsonConvert.SerializeObject(scanResult);
			}
			else return null;
		}

		public class LoginScanResult
		{
			public int UserID { get; set; }
			public string UserFullName { get; set; }
			public string UserAvatarURL { get; set; }
			public int OrganizationID { get; set; }
			public string OrganizationName { get; set; }
			public string RouteURL { get; set; }
		}

		[WebMethod]
		 public string GetEmail(int userID)
		 {
			User user = Users.GetUser(LoginUser.Anonymous, userID);
			return user.Email;
		 }


		[WebMethod]
		 public string[] SavePassword(int userID, string token, string pw1, string pw2)
		 {
			 List<string> result = new List<string>();

			 if (pw1 != pw2) result.Add("Passwords do not match.");
			 if (!pw1.Any(char.IsUpper)) result.Add("At least one uppercase letter is required.");
			 if (!pw1.Any(char.IsLower)) result.Add("At least one lowercase letter is required.");
			 if (!pw1.Any(char.IsDigit)) result.Add("At least one number is required.");
			 if (pw1.Length < 8) result.Add("Use at least 8 characters.");
			 if (pw1.Length > 20) result.Add("Use less than 20 characters.");

			 if (result.Count < 1)
			 {
				 User user = null;


				 if (TSAuthentication.Ticket != null)
				 {
					 user = Users.GetUser(TSAuthentication.GetLoginUser(), TSAuthentication.UserID);
				 }
				 else
				 {
					 user = Users.GetUser(LoginUser.Anonymous, userID);
					 if (user.CryptedPassword != token && user.CryptedPassword != FormsAuthentication.HashPasswordForStoringInConfigFile(token, "MD5"))
					 {
						 user = null;
					 }
				 }

				 if (user != null)
				 {
					 user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(pw1, "MD5");
					 user.IsPasswordExpired = false;
					 user.PasswordCreatedUtc = DateTime.UtcNow;
					 user.Collection.Save();
					 EmailPosts.SendChangedTSPassword(LoginUser.Anonymous, user.UserID);
				 }
				 else
				 {
					 result.Add("There was an issue saving your password.  Please try resetting your password again.");
				 }


			 }

			 return result.ToArray();

		 }

		private static SignInResult IsValid(LoginUser loginUser, string email, string password, int organizationId, ref User user, ref Organization organization)
		{
			SignInResult validation = new SignInResult();
			organization = Organizations.GetOrganization(loginUser, organizationId);
			bool isNewSignUp = DateTime.UtcNow.Subtract(organization.DateCreatedUtc).TotalMinutes < 10;


            if (!organization.IsActive)
            {
                if (string.IsNullOrEmpty(organization.InActiveReason))
                {
                    validation.Error = "Your account is no longer active.  Please contact TeamSupport.com.";
                    validation.Result = LoginResult.Fail;
                }
                else
                {
                    validation.Error = "Your company account is no longer active.<br />" + organization.InActiveReason;
                    validation.Result = LoginResult.Fail;
                }
                TSEventLog.WriteEvent(TSEventLogEventType.FailedLoginAttempt, HttpContext.Current.Request, null, organization, new string[] { "Email: " + email });
                return validation;
            }


            Users users = new Users(loginUser);
			users.LoadByEmail(1, email);

			if (users.Count == 1)
			{
				user = users[0];
			}
			else
			{
				foreach (User u in users)
				{
					if (u.OrganizationID == organizationId)
					{
						user = u;
						break;
					}
				}
			}

			int attempts = LoginAttempts.GetAttemptCount(loginUser, user.UserID, 15);
			validation.LoginFailedAttempts = attempts;

			if (user != null && attempts <= MAXLOGINATTEMPTS)
			{
				validation.UserId = user.UserID;
				validation.OrganizationId = user.OrganizationID;

				if (IsSupportImpersonation(password))
				{
					_skipVerification = true;
					validation.Result = LoginResult.Success;
					validation.Error = string.Empty;
					//vv Log this information!
				}
				else
				{
					if (user.CryptedPassword != EncryptPassword(password) && user.CryptedPassword != password && !isNewSignUp)
					{
						validation.Error = "Invalid email or password.";
						validation.Result = LoginResult.Fail;
					}

					if (!organization.IsActive)
					{
						if (string.IsNullOrEmpty(organization.InActiveReason))
						{
							validation.Error = "Your account is no longer active.  Please contact TeamSupport.com.";
							validation.Result = LoginResult.Fail;
						}
						else
						{
							validation.Error = "Your company account is no longer active.<br />" + organization.InActiveReason;
							validation.Result = LoginResult.Fail;
						}
					}

					if (!user.IsActive)
					{
						validation.Error = "Your account is no longer active.&nbsp&nbsp Please contact your administrator.";
						validation.Result = LoginResult.Fail;
					}

                    DateTime passwordCreatedDate = user.PasswordCreatedUtc != null ? (DateTime) user.PasswordCreatedUtc : user.DateCreated;

                    if (validation.Result != LoginResult.Fail && user.IsPasswordExpired || (organization.DaysBeforePasswordExpire > 0 && DateTime.UtcNow > passwordCreatedDate.AddDays(organization.DaysBeforePasswordExpire)))
					{
						validation.Error = "Your password has expired.";
						validation.Result = LoginResult.PasswordExpired;
					}
				}
			}
			else if (user == null)
			{
				validation.Error = "Invalid email or password.";
				validation.Result = LoginResult.Fail;
			}
			else
			{
				validation.Error = string.Format("Your account is temporarily locked, because of too many failed login attempts.{0}Try again in 15 minutes or use the forgot password link above to reset your password. ", Environment.NewLine);
				validation.Result = LoginResult.Fail;
                if (attempts == MAXLOGINATTEMPTS + 1)
                {
                    TSEventLog.WriteEvent(TSEventLogEventType.AccountLocked, HttpContext.Current.Request, user, organization);
                    EmailPosts.SendTooManyAttempts(loginUser, user.UserID);
                }

			}

			if (validation.Result != LoginResult.Success && validation.Result != LoginResult.Unknown && !string.IsNullOrEmpty(validation.Error))
			{
                TSEventLog.WriteEvent(TSEventLogEventType.FailedLoginAttempt, HttpContext.Current.Request, user, organization, new string[] { "Attempted Email: " + email });
				LoginAttempts.AddAttempt(loginUser, user.UserID, false, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser, HttpContext.Current.Request.UserAgent, GetDeviceID());
			}
			else
			{
                TSEventLog.WriteEvent(TSEventLogEventType.LoginSuccess, HttpContext.Current.Request, user, organization);
				validation.Result = LoginResult.Success;
			}

			return validation;
		}

		/// <summary>
		/// Check if password entered is a code to impersonate other account's user.
		/// </summary>
		/// <returns>true/false</returns>
		private static bool IsSupportImpersonation(string password)
		{
			bool isImpersonation = false;

			return isImpersonation;
		}

		private static int SendAndGetVerificationCode(string userVerificationPhoneNumber)
		{
			int verificationCode = GenerateRandomVerificationCode();
			SMS smsVerification = new SMS();
			smsVerification.Send(string.Format("Your TeamSupport verification code is: {0}", verificationCode.ToString("### ## ###")), userVerificationPhoneNumber);

			if (!smsVerification.IsSuccessful)
			{
				verificationCode = 0;
			}

			return verificationCode;
		}

		private static int GenerateRandomVerificationCode()
		{
			Random rand = new Random((int)System.DateTime.Now.Ticks);
			return rand.Next(10000000, 99999999);
		}

		private static string EncryptPassword(string password)
		{
			return FormsAuthentication.HashPasswordForStoringInConfigFile(password.Trim(), "MD5");
		}

		private static string AuthenticateUser(int userId, int organizationId, bool storeInfo, bool isBackDoor = false)
		{
			string result = string.Empty;
			LoginUser loginUser = new LoginUser(UserSession.ConnectionString, userId, organizationId, null);
			User user = Users.GetUser(loginUser, userId);
			string deviceID = GetDeviceID();

			TSAuthentication.Authenticate(user, isBackDoor, deviceID);
			if (!isBackDoor)
			{
				LoginAttempts.AddAttempt(loginUser, userId, true, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser, HttpContext.Current.Request.UserAgent, deviceID);
				System.Web.HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
				ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Users, userId, "Logged in (" + browser.Browser + " " + browser.Version + ")");

				ConfirmBaseData(loginUser);

				if (storeInfo)
				{
					HttpContext.Current.Response.Cookies["rm"]["a"] = user.Email;
					HttpContext.Current.Response.Cookies["rm"]["b"] = user.OrganizationID.ToString();
					HttpContext.Current.Response.Cookies["rm"].Expires = DateTime.UtcNow.AddDays(7);
				}
				else
				{
					HttpContext.Current.Response.Cookies["rm"].Value = "";
				}
			}

			if (user.IsPasswordExpired && !isBackDoor)
				result = string.Format("vcr/1/LoginNewPassword.html?UserID={0}&Token={1}", user.UserID, user.CryptedPassword);
			else
			{
				string rawQueryString = null;

				try
				{
					rawQueryString = HttpContext.Current.Request.UrlReferrer.Query;
				}
				catch (Exception)
				{
					//vv
				}

				if (!string.IsNullOrEmpty(rawQueryString))
				{
					string urlRedirect = GetQueryStringValue(rawQueryString, "ReturnUrl");

					if (!string.IsNullOrEmpty(urlRedirect) && urlRedirect.Trim().Length > 0)
						result = urlRedirect;
					else
						result = ".";
				}
				else
				{
					result = ".";
				}
			}

			return result;
		}

		private static string GetDeviceID()
		{
			if (HttpContext.Current.Request.Cookies["di"] != null && HttpContext.Current.Request.Cookies["di"].Value != "")
			{
				return HttpContext.Current.Request.Cookies["di"].Value.Trim();
			}
			else
			{
				return "";
			}
		}

		private static string GetQueryStringValue(string queryStr, string key)
		{
			string returnValue = null;
			NameValueCollection queryStrPairs = HttpUtility.ParseQueryString(queryStr);

			if (queryStrPairs != null && queryStrPairs.AllKeys.Contains(key))
			{
				returnValue = queryStrPairs[key];
			}

			return returnValue;
		}

		private static void ConfirmBaseData(LoginUser loginUser)
		{
			Organization organization = (Organization)Organizations.GetOrganization(loginUser, loginUser.OrganizationID);
			TicketTypes types = new TicketTypes(loginUser);
			types.LoadAllPositions(loginUser.OrganizationID);

			if (types.IsEmpty)
			{
				Organizations.CreateStandardData(loginUser, organization, true, true);
			}
		}



		public class SignInResult
		{
			public int UserId { get; set; }
			public int OrganizationId { get; set; }
			public string RedirectURL { get; set; }
			public LoginResult Result { get; set; }
			public string ResultValue
			{
				get
				{
					return Result.ToString();
				}
			}
			public string Error { get; set; }
			public int LoginFailedAttempts { get; set; }
		}

		public enum LoginResult : int
		{
			Unknown = 0,
			Success = 1,
			Fail = 2,
			VerificationNeeded = 3,
			VerificationSetupNeeded = 4,
			PasswordExpired = 5
		}

		[Serializable]
		public class ComboBoxItem
		{
			public ComboBoxItem(string label, int id)
			{
				Label = label;
				ID = id;
			}
			public string Label { get; set; }
			public int ID { get; set; }
		}
	}
}
