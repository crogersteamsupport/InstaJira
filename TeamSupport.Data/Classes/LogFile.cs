using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using System.Data;
using System.IO;


namespace TeamSupport.Data
{
  public class ImportLog: IDisposable
  {
    public enum LogWarningType
    {
      NoParent,
      Duplicate,
    }

    private StringBuilder _log;
    private string _fileName;
    private TextWriter _writer;

    public ImportLog(string fileName)
    {
      _log = new StringBuilder();
      _fileName = fileName;
      _writer = new StreamWriter(fileName);
    }

    public void AppendMessage(string message)
    {
      _writer.WriteLine(message);
      _writer.Flush();
    }

    public void Append(LogWarningType type, DataRow row)
    {
      switch (type)
      {
        case LogWarningType.NoParent:
          AppendMessage("Record could not be linked with foreign key [" + RowToString(row) + "]");
          break;
        case LogWarningType.Duplicate:
          AppendMessage("Record already exists [" + RowToString(row) + "]");
          break;
        default:
          break;
      }

    }

    public void AppendError(DataRow row, string error)
    {

      AppendMessage(error + " [" + RowToString(row) + "]");

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
