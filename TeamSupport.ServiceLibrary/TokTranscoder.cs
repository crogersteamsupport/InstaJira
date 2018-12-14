using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using Microsoft.Win32;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;
using Ionic.Zip;
using System.Diagnostics;
using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Specialized;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web;

namespace TeamSupport.ServiceLibrary
{
    [Serializable]
    public class TokTranscoder : ServiceThreadPoolProcess
    {
        private static object _staticLock = new object();
        //value for the current bucket to use
        string _bucketName = "";
        //path of the file that is being referrenced
        string _s3Path = "";
        //value for the filename to use when uploading
        string _uploadS3Path = "";
        //path of the unzipped files
        string _outputFileLocation = "";
        //path to download the zip files
        string _downloadLocation = "";
        string _ffmpegPath = "";
        string _archiveID;

        static List<string> _webmFiles = new List<string>();
        static IAmazonS3 _client;

        public override void ReleaseAllLocks()
        {
            //You need to lock this, if you plan on multiple threads
        }

        public override void Run()
        {
            UpdateHealth();

            //Amazon.Util.ProfileManager.RegisterProfile("TeamsupportAWS", SystemSettings.ReadString("AWS-Key", ""), SystemSettings.ReadString("AWS-Password", ""));
            Amazon.Util.ProfileManager.RegisterProfile("TeamsupportAWS", "AKIAJHKO22YGQ6N7MAEA", "aEZUTAv6yp2aQdNN4yxv+PJbE/H++bvt33E9IJvd");

            AmazonSQSConfig amazonSQSConfig = new AmazonSQSConfig();
            amazonSQSConfig.ServiceURL = "https://sqs.us-east-1.amazonaws.com/";
            var sqs = new AmazonSQSClient(amazonSQSConfig);

            try
            {
                var receiveMessageRequest = new ReceiveMessageRequest { QueueUrl = "https://sqs.us-east-1.amazonaws.com/168534988511/tsTranscoding-dev" };
                var receiveMessageResponse = sqs.ReceiveMessage(receiveMessageRequest);
                if (receiveMessageResponse.Messages != null)
                {

                    foreach (var message in receiveMessageResponse.Messages)
                    {
                        if (!string.IsNullOrEmpty(message.Body))
                        {
                            JObject o = JObject.Parse(message.Body);
                            string key = HttpUtility.UrlDecode((string)o["Records"][0]["s3"]["object"]["key"]);
                            string bucket = (string)o["Records"][0]["s3"]["bucket"]["name"];
                            string path = String.Format("https://s3.amazonaws.com/{0}/{1}", bucket, key);

                            Logs.WriteEvent(path);
                            InitSettings(path);
                            using (_client = new AmazonS3Client())
                            {
                                DownLoadZip();
                                ExtractZip($@"{_downloadLocation}/{_s3Path}");
                                MergeVideoFiles();
                                UploadHighResVideo();
                                CleanUpFiles();
                            }
                            var messageRecieptHandle = receiveMessageResponse.Messages[0].ReceiptHandle;
                            //Deleting a message
                            Console.WriteLine("Deleting the message.\n");
                            var deleteRequest = new DeleteMessageRequest { QueueUrl = "https://sqs.us-east-1.amazonaws.com/168534988511/tsTranscoding-dev", ReceiptHandle = messageRecieptHandle };
                            sqs.DeleteMessage(deleteRequest);

                            UpdateHealth();
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        void InitSettings(string amazonPath)
        {
            Logs.WriteEvent("----- Building Configuration Settings");
            string[] path = amazonPath.Split('/');
            _bucketName = path[3];
            _archiveID = path[5];
            _s3Path = ($@"{path[4]}/{path[5]}/archive.zip");
            _uploadS3Path = ($@"{path[4]}/{path[5]}/archive.mp4");
            _outputFileLocation = Settings.ReadString("outputFilePath");
            _downloadLocation = Settings.ReadString("downloadFilePath");
            _ffmpegPath = Settings.ReadString("ffmpegPath");

        }

        // High Res Video Process
        // Download zip of webm files
        // Extract files
        // Merge webm video files
        // Upload merged file to amazon s3
        // Delete all related files


        void DownLoadZip()
        {
            Logs.WriteEvent("----- Downloading Zip ...");
            try
            {
                Logs.WriteEvent($@"Key: {_s3Path}");
                Logs.WriteEvent($@"Bucket: {_bucketName}");
                GetObjectRequest request = new GetObjectRequest()
                {
                    BucketName = _bucketName,
                    Key = _s3Path
                };

                using (GetObjectResponse response = _client.GetObject(request))
                {
                    string dest = Path.Combine(_downloadLocation, _s3Path);
                    if (!File.Exists(dest))
                    {
                        response.WriteResponseStreamToFile(dest);
                    }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Logs.WriteEvent("Please check the provided AWS Credentials.");
                    Logs.WriteEvent("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    Logs.WriteEvent(string.Format("An error occurred with the message '{0}' when reading an object", amazonS3Exception.Message));
                }
                Logs.WriteException(amazonS3Exception);
            }
        }

        void ExtractZip(string path)
        {
            Logs.WriteEvent("----- Extracting Zip ..." + path);
            string activeDirectory = Path.GetDirectoryName(path);

            using (ZipFile zip1 = ZipFile.Read(path))
            {
                foreach (ZipEntry e in zip1.OrderBy(e => e.UncompressedSize))
                {
                    if (Path.GetExtension(e.FileName) == ".webm")
                    {
                        e.Extract(Path.GetDirectoryName(path), ExtractExistingFileAction.OverwriteSilently);
                        Logs.WriteEvent($@"Extracting File : {Path.Combine(activeDirectory, e.FileName)}");
                        _webmFiles.Add(Path.Combine(activeDirectory, e.FileName));
                    }
                }
            }
        }

        void MergeVideoFiles()
        {
            Logs.WriteEvent("----- Merging webm files ...");
            _outputFileLocation = Path.Combine(Path.GetDirectoryName(_webmFiles[0]), "archive.mp4");
            Process proc1 = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Path.Combine(_ffmpegPath, "ffprobe.exe"),
                    Arguments = $@"-v error -show_frames -of default=noprint_wrappers=1 {_webmFiles[1]}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    WindowStyle = ProcessWindowStyle.Hidden

                }
            };
            proc1.EnableRaisingEvents = false;

            Logs.WriteEvent("----- Running  ffprobe to get video resolution ...");
            if (!proc1.Start())
            {
                Logs.WriteEvent("Error starting");
                return;
            }
            StreamReader rw = proc1.StandardOutput;
            var height = "";
            var width = "";
            string line;
            while ((line = rw.ReadLine()) != null)
            {
                if (line.Contains("width"))
                {
                    width = line.Split('=')[1];
                }
                if (line.Contains("height"))
                {
                    height = line.Split('=')[1];
                    break;
                }
            }
            try
            {
                proc1.Close();
                proc1.Kill();
            }
            catch (Exception ex)
            {
                Logs.WriteEvent("----- Error killing or closing  ffmpeg : " + ex.Message);
            }
            Logs.WriteEvent("----- Running  ffprobe complete ...");


            Process proc = new Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = Path.Combine(_ffmpegPath, "ffmpeg.exe");
            proc.StartInfo.Arguments = $@"-i {_webmFiles[0]} -i {_webmFiles[1]} -map 0:0 -map 1:1 -codec:a aac -ab 128k -codec:v libx264 -vf scale={width}:{height} -aspect 16:9 -r 30 {_outputFileLocation}";
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.ErrorDialog = false;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.UseShellExecute = false;

            Logs.WriteEvent("----- Running ffmpeg to convert video files ...");
            if (!proc.Start())
            {
                Logs.WriteEvent("Error starting");
                return;
            }
            StreamReader reader = proc.StandardError;

            while ((line = reader.ReadLine()) != null)
            {
                Logs.WriteEvent(line);
            }

            try
            {
                proc.Close();
                proc.Kill();

            }
            catch (Exception ex)
            {
                Logs.WriteEvent("----- Error killing or closing  ffmpeg : " + ex.Message);
            }
            Logs.WriteEvent("----- Running ffmpeg complete ...");
        }

        void UploadHighResVideo()
        {
            Logs.WriteEvent("----- Uploading Converted Video ...");
            try
            {
                PutObjectRequest request = new PutObjectRequest()
                {
                    FilePath = _outputFileLocation,
                    BucketName = _bucketName,
                    Key = _uploadS3Path
                };

                PutObjectResponse response = _client.PutObject(request);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Logs.WriteEvent("Please check the provided AWS Credentials.");
                    Logs.WriteEvent("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    Logs.WriteEvent(string.Format("An error occurred with the message '{0}' when writing an object", amazonS3Exception.Message));
                }
            }
        }

        void CleanUpFiles()
        {
            TokStorage dbItem = new TokStorage(LoginUser);
            dbItem.LoadByArchiveID(_archiveID);
            dbItem[0].Transcoded = true;
            dbItem[0].Collection.Save();

            Logs.WriteEvent("----- Cleaning up files and marking as processed");
            string dest = Path.GetDirectoryName(_webmFiles[0]);
            System.IO.DirectoryInfo di = new DirectoryInfo(dest);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            di.Delete();
            _webmFiles.Clear();

        }

    }
}
