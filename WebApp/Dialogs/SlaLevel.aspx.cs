using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using TeamSupport.WebUtils;
using TeamSupport.Data;
using Telerik.Web.UI;


public partial class Dialogs_SlaLevel : BaseDialogPage
{

  private int _slaLevelID = -1;
  private bool _isCloning = false;

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    if (!UserSession.CurrentUser.IsSystemAdmin)
    {
      Response.Write("Invalid Request");
      Response.End();
      return;
    }


    if (Request["SlaLevelID"] != null) _slaLevelID = int.Parse(Request["SlaLevelID"]);

    if (Request["IsCloning"] != null) _isCloning = bool.Parse(Request["IsCloning"]);

    if (_isCloning)
    {
        this.Title = "Clone Service Level Agreement";
    }

    if (!IsPostBack)
    {
      LoadLevel(_slaLevelID);
    }
  }

  private void LoadLevel(int slaLevelID)
  {
    SlaLevel level = SlaLevels.GetSlaLevel(UserSession.LoginUser, slaLevelID);
    if (level == null) return;
    if (level.OrganizationID != UserSession.LoginUser.OrganizationID)
    {
      Response.Write("Invalid Request");
      Response.End();
      return;
    }

    textName.Text = level.Name + (_isCloning ? " (Clone)" : "");
  }

  public override bool Save()
  {
    if (IsValid())
    {
        if (_isCloning)
        {
            //Clone SLA here
            LoginUser loginUser = TSAuthentication.GetLoginUser();
			int clonedSlaLevelId = 0;

			try
			{
				SlaLevel originalSlaLevel = SlaLevels.GetSlaLevel(loginUser, _slaLevelID);
                SlaLevel clonedSlaLevel = originalSlaLevel.Clone(textName.Text.Trim());
				clonedSlaLevelId = clonedSlaLevel.SlaLevelID;
            }
			catch (Exception ex)
			{
				ExceptionLogs.LogException(loginUser, ex, "Cloning Sla", "SlaLevel.aspx.cs.Save");
			}
        }
        else
        {
            SlaLevel level;
            SlaLevels levels = new SlaLevels(UserSession.LoginUser);

            if (_slaLevelID < 0)
            {
                level = levels.AddNewSlaLevel();
                level.OrganizationID = UserSession.LoginUser.OrganizationID;
            }
            else
            {
                level = SlaLevels.GetSlaLevel(UserSession.LoginUser, _slaLevelID);
                if (level == null) return false;
            }

            level.Name = textName.Text;
            level.Collection.Save();
            DialogResult = level.SlaLevelID.ToString();
        }
    }
    else
    {
        return false;
    } 
    
    return true;
  }

  private bool DoesNameExist(string name)
  {
    name = name.Trim().ToLower();
    SlaLevels levels = new SlaLevels(UserSession.LoginUser);
    levels.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
    if (_slaLevelID > -1)
    {
      SlaLevel current = SlaLevels.GetSlaLevel(UserSession.LoginUser, _slaLevelID);
      if (current.Name.Trim().ToLower() == name) return false;
    }
    


    foreach (SlaLevel level in levels)
    {
      if (level.Name.Trim().ToLower() == name)
      {
        return true;
      }
    }

    return false;
  
  }

    private bool IsValid()
    {
        bool isValid = true;

        if (textName.Text.Trim() == "")
        {
            _manager.Alert("Please enter a name.");
            isValid = false;
        }

        if (isValid && DoesNameExist(textName.Text))
        {
            _manager.Alert("That name is already being used.  Please choose another one.");
            isValid = false;
        }

        return isValid;
    }
}

