using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
  public class SyncLog
  {
    private string _logPath;
    private string _fileName;

    public SyncLog(string path, IntegrationType integrationType)
    {
      _logPath = path;
      _fileName = string.Format("{0} Debug File - {1}{2}{3}.txt", integrationType.ToString(), DateTime.UtcNow.Month.ToString(), DateTime.UtcNow.Day.ToString(), DateTime.UtcNow.Year.ToString());

      if (!Directory.Exists(_logPath))
      {
        Directory.CreateDirectory(_logPath);
      }
    }

    public void Write(string Text)
    {
      //the very first time we write to this file (once each day), purge old files
      if (!File.Exists(string.Format("{0}\\{1}", _logPath, _fileName)))
      {
        foreach (string oldFileName in Directory.GetFiles(_logPath))
        {
          if (File.GetLastWriteTime(oldFileName).AddDays(7) < DateTime.UtcNow)
          {
            File.Delete(oldFileName);
          }
        }
      }

      File.AppendAllText(string.Format("{0}\\{1}", _logPath, _fileName), string.Format("{0}: {1}{2}", DateTime.Now.ToLongTimeString(), Text, Environment.NewLine));
    }
  }
}
