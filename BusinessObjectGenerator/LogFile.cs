using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace BusinessObjectGenerator
{
  public class LogFile
  {
    private static bool _opened = false;
    private static string _fileName;
    private static StreamWriter _writer = null;

    private static string AppendToFileName(string fileName, string text)
    {
      string dir = Path.GetDirectoryName(fileName);
      string ext = Path.GetExtension(fileName);
      string name = Path.GetFileNameWithoutExtension(fileName) + text + ext;
      return Path.Combine(dir, name);
    }

    private static string GetNextFileName(string fileName)
    {
      string name = AppendToFileName(fileName, " " + DateTime.Today.ToString("yyyy-MM-dd"));
      string result = name;
      for (int i = 0; i < 1000; i++)
      {
        result = AppendToFileName(name, " (" + i.ToString() + ")");
        if (!File.Exists(result)) break;
      }

      return result;
    }

    public static void Open(string baseFileName, bool append)
    {
      lock (typeof(LogFile))
      {
        string dir = Path.GetDirectoryName(baseFileName);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        _fileName = append ? baseFileName : GetNextFileName(baseFileName);
        FileStream fs = new FileStream(_fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
        _writer = new StreamWriter(fs);
        _opened = true;
      }

    }

    public static void Close()
    {
      lock (typeof(LogFile))
      {
        _opened = false;
        if (_writer != null)
        {
          _writer.Close();
          _writer = null;
        }
      }
    }

    public static void AddLineBreak()
    {
      Add("", false);
    }

    public static void Add(string line)
    {
      Add(line, true);
    }

    public static void Add(string line, bool timeStamp)
    {
      lock (typeof(LogFile))
      {
        if (_writer == null || !_opened) return;
        if (timeStamp)
        {
          _writer.WriteLine("[" + DateTime.Now.ToString("hh:MM:ss") + "] " + line);
        }
        else
        {
          _writer.WriteLine(line);
        }
        _writer.Flush();
      }
    }

    public static void AddException(Exception e)
    {
      AddLineBreak();
      Add("<---Exception--->");
      Add("Message: " + e.Message);
      AddLineBreak();
      Add("Stack Trace: ");
      Add(e.StackTrace);
      AddLineBreak();
    }
  }
}



