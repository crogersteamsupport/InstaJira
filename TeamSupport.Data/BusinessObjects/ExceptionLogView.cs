using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ExceptionLogViewItem 
  {
  }

  public partial class ExceptionLogView 
  {
    public void LoadAll()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT ExceptionLogID, DateCreated, Name, FirstName, LastName, ExceptionName, Message, StackTrace, URL, PageInfo FROM ExceptionLogView ORDER BY DateCreated DESC";
        command.CommandType = CommandType.Text;
        Fill(command);
      }

    }
    public void LoadTop(int max)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP " + max.ToString() + " * FROM ExceptionLogView ORDER BY DateCreated DESC";
        command.CommandType = CommandType.Text;
        Fill(command);
      }

    }
  }

}
