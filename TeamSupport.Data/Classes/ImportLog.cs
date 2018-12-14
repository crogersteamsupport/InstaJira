using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using System.Data;
using System.IO;


namespace TeamSupport.Data
{
  public class LogFile: IDisposable
  {
    private StringBuilder _log;
    private string _fileName;
    private TextWriter _writer;

    public LogFile(string fileName)
    {
      _log = new StringBuilder();
      _fileName = fileName;
      _writer = new StreamWriter(fileName);
    }

    public void Append(string message)
    {
      _writer.WriteLine(message);
      _writer.Flush();
    }

    public void Append(DataRow row, string text)
    {
       Append(text + " [" + RowToString(row) + "]");
    }

    public void Append(Exception e, string text)
    {
      Append(e.Message + " [" + e.StackTrace + "]");
    }

    private string RowToString(DataRow row)
    {
      StringBuilder builder = new StringBuilder();
      foreach (DataColumn column in row.Table.Columns)
      {
        builder.Append(" " + column.ColumnName + "=" + row[column].ToString() + ";");
      }
      return builder.ToString();
    }


    #region IDisposable Members

    public void Dispose()
    {
      _writer.Close();
    }

    #endregion
  }



}
