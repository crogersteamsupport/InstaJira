using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  class TicketIndexDataSource : IndexDataSource
  {
    protected TicketIndexDataSource() { }

    public TicketIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, string table, bool isRebuilding, Logs logs)
      : base(loginUser, maxCount, organizationID, table, isRebuilding, logs)
    {
    }


    override protected void GetNextRecord()
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(_loginUser, _itemIDList[_rowIndex]);
      _lastItemID = ticket.TicketID;
      UpdatedItems.Add((int)_lastItemID);

      StringBuilder actionsBuilder = new StringBuilder();
      Actions actions = new Actions(_loginUser);
      actions.LoadByTicketID(ticket.TicketID);
      foreach (TeamSupport.Data.Action action in actions)
      {
        string actionText = action.Description;

        if (!action.IsClean)
        {
          try
          {
            actionText = HtmlUtility.Sanitize(actionText);
            SqlCommand command = new SqlCommand();
            command.CommandText = "UPDATE Actions SET Description = @Description, IsClean=1 WHERE ActionID=@ActionID";
            command.Parameters.AddWithValue("ActionID", action.ActionID);
            command.Parameters.AddWithValue("Description", actionText);
            SqlExecutor.ExecuteNonQuery(_loginUser, command);
          }
          catch (Exception)
          {
            _logs.WriteEvent("Unable to sanitize action: " + action.ActionID);
          }
        }

        try
        {
          //actionText = HtmlToText.ConvertHtml(actionText);
          actionsBuilder.AppendLine(actionText);
        }
        catch (Exception ex)
        {
          _logs.WriteEvent("Unable to convert action html: " + action.ActionID);
          _logs.WriteException(ex);
        }
      }

      DocText = actionsBuilder.ToString();

      _docFields.Clear();
      foreach (DataColumn column in ticket.Collection.Table.Columns)
	    {
        object value = ticket.Row[column];
        string s = value == null || value == DBNull.Value ? "" : value.ToString();
        AddDocField(column.ColumnName, s);
      }

      CustomValues customValues = new CustomValues(_loginUser);
      customValues.LoadByReferenceType(_organizationID, ReferenceType.Tickets, ticket.TicketTypeID, ticket.TicketID);

      foreach (CustomValue value in customValues)
      {
        object o = value.Row["CustomValue"];
        string s = o == null || o == DBNull.Value ? "" : o.ToString();
        AddDocField(value.Row["Name"].ToString(), s);
      }
      DocFields = _docFields.ToString();

      DocIsFile       = false;
      DocName         = ticket.TicketID.ToString();
      DocDisplayName  = string.Format("{0}: {1}", ticket.TicketNumber.ToString(), ticket.Name);
      DocCreatedDate  = (DateTime)ticket.Row["DateCreated"];
      DocModifiedDate = (DateTime)ticket.Row["DateModified"];
    }



    override protected void LoadData()
    {
      TicketsView tickets = new TicketsView(_loginUser);
      tickets.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
      foreach (TicketsViewItem ticket in tickets)
      {
        _itemIDList.Add(ticket.TicketID);
      }
    }
  }
}