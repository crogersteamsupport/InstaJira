using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class FilePath
  {
  }
  
  public partial class FilePaths
  {
        public void LoadThemAll()
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM FilePaths";
                command.CommandType = CommandType.Text;
                Fill(command);
            }
        }
    }

}
