using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace AmazonS3TransferUtility
{
  class Program
  {
    public const int FIVE_MINUTES = 5 * 60 * 1000;
    public static TransferUtility _transferUtility;
    public static string _downloadFolder;
    public static string _resultsFile;
    public static string _bucket;
    public static string _screenRFileList;
    public static string[] _screenRUrls;
    public static List<string> _failedFile;

    static void Main(string[] args)
    {
      Console.WriteLine("************************************************************");
      Console.WriteLine("This process will:");
      Console.WriteLine("1) Download all of the ScreenR videos as mp4s, reading the file specified in the 'ScreenRFileList' config key.");
      Console.WriteLine("2) Move (upload) the file to Amazon to the bucket specified in the 'Bucket' config key.");
      Console.WriteLine("It will be done one at a time.");
      Console.WriteLine("Press any key to start...");
      Console.ReadKey();

      LoadConfiguration();
      WriteLog(_resultsFile, "************************************************************");
      Process();
      Console.ReadKey();
    }

    private static void LoadConfiguration()
    {
      NameValueCollection appConfig = ConfigurationManager.AppSettings;

      if (string.IsNullOrEmpty(appConfig["AWSProfileName"]))
      {
        WriteLog(_resultsFile, "AWSProfileName is not set in the App.Config");
        return;
      }

      _transferUtility = new TransferUtility(RegionEndpoint.USEast1);

      string currentPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
      _downloadFolder = Path.Combine(currentPath, "Downloads");

      if (!Directory.Exists(_downloadFolder))
      {
        Directory.CreateDirectory(_downloadFolder);
      }

      _bucket = appConfig["Bucket"];

      _screenRFileList = appConfig["ScreenRFileList"];

      if (string.IsNullOrEmpty(_screenRFileList))
      {
        _screenRFileList = Path.Combine(currentPath, "ScreenRfiles.txt");
      }

      _resultsFile = Path.Combine(currentPath, string.Format("results_{0}.txt", DateTime.Now.ToString("MMddyyyy_hhmmss")));
      _failedFile = new List<string>();
    }

    private static void Process()
    {
      try
      {
        _screenRUrls = File.ReadAllLines(_screenRFileList);  

        foreach (string screenRFileUrl in _screenRUrls)
        {  
          try
          {
            Console.WriteLine(screenRFileUrl);
            WriteLog(_resultsFile, screenRFileUrl);

            //vv Download File
            string downloadFile = string.Empty;

            if (screenRFileUrl.Contains(".mp4"))
            {
              downloadFile = Path.Combine(_downloadFolder, Path.GetFileName(screenRFileUrl));

              using (var client = new WebClient())
              {
                client.DownloadFile(screenRFileUrl, downloadFile);
              }

              WriteLog(_resultsFile, "Completed file download.");

              //Upload to Amazon S3
              TransferUtilityUploadRequest request = new TransferUtilityUploadRequest()
              {
                BucketName = _bucket,
                FilePath = downloadFile,
                CannedACL = S3CannedACL.PublicRead
              };

              _transferUtility.Upload(request);

              WriteLog(_resultsFile, "Completed file upload to Amazon S3.");
            }
            else
            {
              _failedFile.Add(screenRFileUrl);
              WriteLog(_resultsFile, "This is not a mp4 file.");
            }
          }
          catch (Exception e)
          {
            _failedFile.Add(screenRFileUrl);
            WriteLog(_resultsFile, string.Format("{0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace));
          }
        }

        string processedFile = _screenRFileList.Replace(".", string.Format("_{0}.", DateTime.Now.ToString("MMddyyyy_hhmmss")));
        File.Move(_screenRFileList, processedFile);

        if (File.Exists(processedFile))
        {
          File.Delete(_screenRFileList);
        }

        if (_failedFile.Any() && _failedFile.Count > 0)
        {
          File.WriteAllLines(_screenRFileList, _failedFile.ToArray());
        }
      }
      catch (Exception e)
      {
        WriteLog(_resultsFile, string.Format("{0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace));
      }
      finally
      {
        Console.WriteLine("Done");
        WriteLog(_resultsFile, "Done");
        Console.ReadKey();
      }
    }

    private static void WriteLog(string fileName, string content)
    {
      content = string.Format("{0}: {1}", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"), content);

      bool doesFileExist = File.Exists(fileName);
      using (StreamWriter writer = !doesFileExist ? File.CreateText(fileName) : File.AppendText(fileName))
      {
        writer.WriteLine(content);
      }
    }
  }
}
