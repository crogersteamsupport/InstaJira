using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ActionLinkToTFSItem
  {
  }
  
  public partial class ActionLinkToTFS
  {
        //Changes to this method needs to be applied to Actions.LoadToPushToTFS also.
        public void LoadToPushToTFS(CRMLinkTableItem item, int ticketID)
        {
            string actionTypeFilter = "1 = 1";

            if (item.ActionTypeIDToPush != null)
            {
                actionTypeFilter = "a.ActionTypeID = @actionTypeID";
            }

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText =
                @"
          SELECT 
            tfs.* 
          FROM 
            Actions a
            JOIN ActionLinkToTFS tfs
              ON a.ActionID = tfs.ActionID
          WHERE 
            a.SystemActionTypeID <> 1 
            AND a.TicketID = @ticketID
            AND " + actionTypeFilter + @"
            AND 
            (
              tfs.DateModifiedByTFSSync IS NULL
              OR a.DateModified > DATEADD(s, 2, tfs.DateModifiedByTFSSync)
            )
          ORDER BY 
            a.DateCreated ASC
        ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ticketID", ticketID);
                command.Parameters.AddWithValue("@DateModified", item.LastLink == null ? new DateTime(1753, 1, 1) : item.LastLinkUtc.Value.AddHours(-1));
                command.Parameters.AddWithValue("@actionTypeID", item.ActionTypeIDToPush == null ? -1 : item.ActionTypeIDToPush);

                Fill(command, "ActionLinkToTFS");
            }
        }

        public ActionLinkToTFSItem FindByActionID(int actionID)
        {
            foreach (ActionLinkToTFSItem item in this)
            {
                if (item.ActionID == actionID)
                {
                    return item;
                }
            }
            return null;
        }

        public void LoadByTicketID(int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText =
                @"
          SELECT 
            tfs.* 
          FROM 
            Actions a
            JOIN ActionLinkToTFS tfs
              ON a.ActionID = tfs.ActionID
          WHERE 
            a.SystemActionTypeID <> 1 
            AND a.TicketID = @ticketID
        ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ticketID", ticketID);

                Fill(command, "ActionLinkToTFS");
            }
        }

        public ActionLinkToTFSItem FindByTFSID(int? tfsID)
        {
            foreach (ActionLinkToTFSItem item in this)
            {
                if (item.TFSID == tfsID)
                {
                    return item;
                }
            }
            return null;
        }

        public virtual ActionLinkToTFSItem AddNewActionLinkToTFSItem(int actionID)
        {
            if (Table.Columns.Count < 1) LoadColumns("ActionLinkToTFS");
            DataRow row = Table.NewRow();
            row["ActionID"] = actionID;
            Table.Rows.Add(row);
            return new ActionLinkToTFSItem(row, this);
        }

    }

}
