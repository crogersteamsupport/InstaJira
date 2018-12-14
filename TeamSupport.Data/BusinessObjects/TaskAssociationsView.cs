using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class TaskAssociationsViewItem
    {
        public string GetText()
        {
            StringBuilder result = new StringBuilder();
            switch (this.RefType)
            {
                case 6:
                    result.Append("Group ");
                    result.Append(this.Group);
                    break;
                case 9:
                    result.Append("Company ");
                    result.Append(this.Company);
                    break;
                case 13:
                    result.Append("Product ");
                    result.Append(this.Product);
                    break;
                case 17:
                    result.Append("Ticket ");
                    result.Append(this.TicketNumber + ": ");
                    result.Append(this.TicketName);
                    break;
                case 22:
                    result.Append("User ");
                    result.Append(this.User);
                    break;
                default:
                    result.Append("Unknown association");
                    break;
            }

            return result.ToString();
        }
    }

    public partial class TaskAssociationsView
    {
        public void LoadByTaskIDOnly(int taskID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                SELECT 
                    * 
                FROM 
                    TaskAssociationsView
                WHERE 
                    TaskID = @TaskID
                ORDER BY RefType";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TaskID", taskID);
                Fill(command);
            }
        }

        public string GetText()
        {
            if (this.Count > 0)
            {
                StringBuilder result = new StringBuilder();
                for (int i = 0; i < this.Count; i++)
                {
                    result.Append(this[i].GetText());
                    if (i < this.Count - 1)
                    {
                        result.Append(", ");
                    }
                    else
                    {
                        result.Append(".");
                    }
                }
                return result.ToString();
            }
            else
            {
                return "[None]";
            }
        }
    }
}