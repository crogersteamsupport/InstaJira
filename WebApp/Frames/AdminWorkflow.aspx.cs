using System;
using System.Web.Services;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;
using log4net;

public partial class Frames_AdminWorkflow : BaseFramePage
{
	private static readonly ILog _log = LogManager.GetLogger("RollingLogFileAppenderApp");
	private static int _nextStatusSelected = 0;
  public int SelectedTicketTypeIndex
  {
		get
		{
			int index = 0;
			int.TryParse(SelectedTicketTypeIndexHidden.Value, out index);
			return index;
		}
		set
		{
			SelectedTicketTypeIndexHidden.Value = value.ToString();
		}
	}

  public int SelectedTicketStatusIndex
  {
		get
		{
			int index = 0;
			int.TryParse(SelectedTicketStatusIndexHidden.Value, out index);
			return index;
		}
		set
		{
			SelectedTicketStatusIndexHidden.Value = value.ToString();
		}
  }

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);
	log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "logging.config"));
    LoadTicketTypes();
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    bool isAdmin = UserSession.CurrentUser.IsSystemAdmin;
    gridNext.Columns[0].Visible = isAdmin;
    gridNext.Columns[1].Visible = isAdmin;
    gridNext.Columns[2].Visible = isAdmin;
    lnkAddStatus.Visible = isAdmin;

	if (cmbTicketTypes.Items.Count > 0)
    {
      cmbTicketTypes.SelectedIndex = SelectedTicketTypeIndex;
      LoadStatuses(int.Parse(cmbTicketTypes.SelectedValue));
      if (cmbStatuses.Items.Count > 0)
      {
        cmbStatuses.SelectedIndex = SelectedTicketStatusIndex;
        LoadNextStatuses(int.Parse(cmbStatuses.SelectedValue));
      }
    }
    else
    {
      cmbStatuses.Items.Clear();
    }

    if (SelectedTicketTypeIndex != cmbTicketTypes.SelectedIndex)
    {
      SelectedTicketTypeIndex = cmbTicketTypes.SelectedIndex;
      LoadStatuses(GetSelectedTypeID());
      LoadNextStatuses(GetSelectedStatusID());
      SelectedTicketStatusIndex = cmbStatuses.SelectedIndex;
    }
    else if (SelectedTicketStatusIndex != cmbStatuses.SelectedIndex)
    {
      SelectedTicketStatusIndex = cmbStatuses.SelectedIndex;
      LoadNextStatuses(GetSelectedStatusID());
    }
  }

  private int GetSelectedTypeID()
  {
    if (cmbTicketTypes.SelectedIndex < 0) return -1;
    return int.Parse(cmbTicketTypes.SelectedValue);
  }

  private int GetSelectedStatusID()
  { 
    if (cmbStatuses.SelectedIndex < 0) return -1;
    return int.Parse(cmbStatuses.SelectedValue);
  }

  private void LoadTicketTypes()
  {
    cmbTicketTypes.Items.Clear();
    TicketTypes types = new TicketTypes(UserSession.LoginUser);
    types.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, UserSession.CurrentUser.ProductType);
    foreach (TicketType type in types)
    {
      cmbTicketTypes.Items.Add(new RadComboBoxItem(type.Name, type.TicketTypeID.ToString()));
    }
    if (cmbTicketTypes.Items.Count > 0) cmbTicketTypes.SelectedIndex = 0;
  }

  private void LoadStatuses(int ticketTypeID)
  {
    cmbStatuses.Items.Clear();
    
    TicketStatuses statuses = new TicketStatuses(UserSession.LoginUser);
    statuses.LoadByTicketTypeID(ticketTypeID);

    foreach (TicketStatus status in statuses)
    {
      cmbStatuses.Items.Add(new RadComboBoxItem(status.Name, status.TicketStatusID.ToString()));
    }
    if (cmbStatuses.Items.Count > 0) cmbStatuses.SelectedIndex = 0;
  }

  private void LoadAvailableStatuses(int statusID)
  {
    TicketStatuses statuses = new TicketStatuses(UserSession.LoginUser);
    statuses.LoadNotNextStatuses(statusID);

	string postBackControl = this.Request.Params["__EVENTTARGET"];

	if (!string.IsNullOrEmpty(postBackControl) && postBackControl.EndsWith("cmbNewStatus"))
	{
		int.TryParse(cmbNewStatus.SelectedValue, out _nextStatusSelected);
	}

    cmbNewStatus.Items.Clear();
    cmbNewStatus.Items.Add(new RadComboBoxItem("[Select a status]", "-1"));

    foreach (TicketStatus status in statuses)
    {
      cmbNewStatus.Items.Add(new RadComboBoxItem(status.Name, status.TicketStatusID.ToString()));
    }
  }

  private void LoadNextStatuses(int ticketStatusID)
  {
    TicketNextStatuses nextStatuses = new TicketNextStatuses(UserSession.LoginUser);
    nextStatuses.LoadNextStatuses(ticketStatusID);
    gridNext.DataSource = nextStatuses.Table;
    gridNext.DataBind();
    nextStatuses.ValidatePositions(ticketStatusID);
    LoadAvailableStatuses(ticketStatusID);
  }

	//This does not seem to be working. I could not find a way to have this method fired by the RadGrid control, except only for the RowClick which does not help.
	protected void gridNext_ItemCommand(object source, GridCommandEventArgs e)
  {
    if (e.CommandName == RadGrid.DeleteCommandName)
    {
      TicketNextStatuses statuses = new TicketNextStatuses(UserSession.LoginUser);
      statuses.DeleteFromDB((int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["TicketNextStatusID"]);
    }
    else if (e.CommandName == "MoveUp")
    {
      TicketNextStatuses statuses = new TicketNextStatuses(UserSession.LoginUser);
      statuses.MovePositionUp((int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["TicketNextStatusID"]);
    }
    else if (e.CommandName == "MoveDown")
    {
      TicketNextStatuses statuses = new TicketNextStatuses(UserSession.LoginUser);
      statuses.MovePositionDown((int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["TicketNextStatusID"]);
    }

    LoadNextStatuses(GetSelectedStatusID());
    
  }

  protected void cmbNewStatus_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
  {
	if (_nextStatusSelected > 0)
	{
		TicketNextStatuses ticketNextStatuses = new TicketNextStatuses(UserSession.LoginUser);
		TicketNextStatus ticketNextStatus = ticketNextStatuses.AddNewTicketNextStatus();
		ticketNextStatus.CurrentStatusID = GetSelectedStatusID();
		ticketNextStatus.NextStatusID = _nextStatusSelected;
		ticketNextStatus.Position = ticketNextStatuses.GetMaxPosition(ticketNextStatus.CurrentStatusID) + 1;
		ticketNextStatuses.Save();
		LoadNextStatuses(GetSelectedStatusID());
	}
  }

	protected void gridNext_ItemDataBound(object sender, GridItemEventArgs e)
	{
		if (e.Item is GridDataItem)
		{
			int nextStatusId = (int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["TicketNextStatusID"];

			GridDataItem dataItem = (GridDataItem)e.Item;
			ImageButton deleteButton = (ImageButton)dataItem["columnDelete"].Controls[0];
			deleteButton.Attributes.Add("onclick", string.Format("Delete('{0}');", nextStatusId));
			ImageButton upButton = (ImageButton)dataItem["columnMoveUp"].Controls[0];
			upButton.Attributes.Add("onclick", string.Format("MoveUp('{0}');", nextStatusId));
			ImageButton downButton = (ImageButton)dataItem["columnMoveDown"].Controls[0];
			downButton.Attributes.Add("onclick", string.Format("MoveDown('{0}');", nextStatusId));
		}
	}

	[WebMethod]
	public static bool MoveUp(int nextStatusId)
	{
		bool moved = false;

		if (!UserSession.CurrentUser.IsSystemAdmin) return moved;


		try
		{
			TicketNextStatuses statuses = new TicketNextStatuses(UserSession.LoginUser);
			statuses.MovePositionUp(nextStatusId);
			moved = true;
		}
		catch (Exception ex)
		{
			_log.ErrorFormat("AdminWorkflow.MoveUp: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);
			moved = false;
		}
		
		
		return moved;
	}

	[WebMethod]
	public static bool MoveDown(int nextStatusId)
	{
		bool moved = false;

		if (!UserSession.CurrentUser.IsSystemAdmin) return moved;

		try
		{
			TicketNextStatuses statuses = new TicketNextStatuses(UserSession.LoginUser);
			statuses.MovePositionDown(nextStatusId);
			moved = true;
		}
		catch (Exception ex)
		{
			_log.ErrorFormat("AdminWorkflow.MoveDown: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);
			moved = false;
		}

		return moved;
	}

	[WebMethod]
	public static bool Delete(int nextStatusId)
	{
		bool deleted = false;

		if (!UserSession.CurrentUser.IsSystemAdmin) return deleted;

		try
		{
			TicketNextStatuses statuses = new TicketNextStatuses(UserSession.LoginUser);
			statuses.DeleteFromDB(nextStatusId);
			deleted = true;
		}
		catch (Exception ex)
		{
			_log.ErrorFormat("AdminWorkflow.Delete: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);
			deleted = false;
		}

		return deleted;
	}
}
