using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using System.Globalization;

namespace TeamSupport.WebUtils
{
  [Serializable]
  public class UserInfoProxy
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public string FirstLastName { get; set; }
    public string Email { get; set; }
    public int UserID { get; set; }
    public int OrganizationID { get; set; }
    public bool IsActive { get; set; }
    public bool IsChatUser { get; set; }
    public bool IsSystemAdmin { get; set; }
    public string LastVersion { get; set; }
    public string OrganizationName { get; set; }
    public int ProductType { get; set; }
  }

  [Serializable]
  public class UserInfo
  {
    public UserInfo(User user)
    {
      _firstName = user.FirstName;
      _lastName = user.LastName;
      _middleName = user.MiddleName;
      _email = user.Email;
      _userID = user.UserID;
      _organizationID = user.OrganizationID;
      _isActive = user.IsActive;
      _isChatUser = user.IsChatUser;
      _isSystemAdmin = user.IsSystemAdmin;
      _isFinanceAdmin = user.IsFinanceAdmin;
      _lastVersion = user.LastVersion;


      Organization organizaiton = (Organization)Organizations.GetOrganization(user.Collection.LoginUser, user.OrganizationID);
      OrganizationName = organizaiton.Name;
      _isTSUser = organizaiton.ParentID == null;
      _productType = organizaiton.ProductType;
      _hasPortalRights = organizaiton.HasPortalAccess;
      _hasChatRights = organizaiton.ChatSeats > 0;
      _isInventoryEnabled = organizaiton.IsInventoryEnabled;
      _isAdminOnlyCustomers = organizaiton.AdminOnlyCustomers;

      _proxy = new UserInfoProxy();
      _proxy.FirstName = this.FirstName;
      _proxy.FirstLastName = this.FirstLastName;
      _proxy.IsActive = this.IsActive;
      _proxy.IsChatUser = this.IsChatUser;
      _proxy.IsSystemAdmin = this.IsSystemAdmin;
      _proxy.LastName = this.LastName;
      _proxy.LastVersion = this.LastVersion;
      _proxy.MiddleName = this.MiddleName;
      _proxy.Email = this.Email;
      _proxy.OrganizationID = this.OrganizationID;
      _proxy.OrganizationName = this.OrganizationName;
      _proxy.ProductType = (int)this.ProductType;
      _proxy.UserID = this.UserID;
    }

    private bool _hasChatRights;

    public bool HasChatRights
    {
      get { return _hasChatRights; }
    }

    private UserInfoProxy _proxy;
    public UserInfoProxy Proxy
    {
      get { return _proxy; }
    }

    private string _firstName;
    public string FirstName
    {
      get { return _firstName; }
    }

    private string _lastName;
    public string LastName
    {
      get { return _lastName; }
    }

    private string _middleName;
    public string MiddleName
    {
      get { return _middleName; }
    }

    private string _email;
    public string Email
    {
      get { return _email; }
    }

    public string DisplayName
    {
      get { return _lastName + ", " + _firstName; }
    }

    public string FirstLastName
    {
      get { return _firstName + " " + _lastName; }
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

    private bool _isSystemAdmin = false;
    public bool IsSystemAdmin
    {
      get { return _isSystemAdmin; }
    }

    private bool _isFinanceAdmin = false;
    public bool IsFinanceAdmin
    {
      get { return _isFinanceAdmin; }
    }

    private bool _isChatUser = false;
    public bool IsChatUser
    {
      get { return _isChatUser; }
      set { _isChatUser = value; }
    }

    private bool _isActive = false;
    public bool IsActive
    {
      get { return _isActive; }
    }

    private string _lastVersion = "";
    public string LastVersion
    {
      get { return _lastVersion; }
    }

    private bool _isTSUser = false;
    public bool IsTSUser
    {
      get { return _isTSUser; }
    }

    private ProductType _productType = ProductType.Express;
    public ProductType ProductType
    {
      get { return _productType; }
    }

    private bool _hasPortalRights = false;
    public bool HasPortalRights
    {
      get { return _hasPortalRights; }
    }

    private string _organizationName;

    public string OrganizationName
    {
      get { return _organizationName; }
      set { _organizationName = value; }
    }

    private bool _isAdminOnlyCustomers = false;

    public bool IsAdminOnlyCustomers
    {
      get { return _isAdminOnlyCustomers; }
      set { _isAdminOnlyCustomers = value; }
    }

    private bool _isInventoryEnabled = false;

    public bool IsInventoryEnabled
    {
      get { return _isInventoryEnabled; }
      set { _isInventoryEnabled = value; }
    }

  }
}
