using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TeamSupport.Data;

namespace APITester
{
    public partial class Form1 : Form
    {
        //i am getting unauthorized running it from 104.
        //lets try without datalayer
        private Logs _logs = new Logs(DateTime.Now.ToString("HH-mm"));
        private string _url = ConfigurationManager.AppSettings.Get("URL");
        private int _organizationID = Int32.Parse(ConfigurationManager.AppSettings.Get("OrganizationID"));
        private string _authenticationToken = ConfigurationManager.AppSettings.Get("AuthenticationToken");
        private string _encodedCredentials;

        private int _ticketID = Int32.Parse(ConfigurationManager.AppSettings.Get("TicketID"));
        private int _actionID = Int32.Parse(ConfigurationManager.AppSettings.Get("ActionID"));
        private int _assetID = Int32.Parse(ConfigurationManager.AppSettings.Get("AssetID"));

        public Form1()
        {
            InitializeComponent();
            DisplayAndLog(GetInitialText());
            Byte[] credentialsByteArray = UTF8Encoding.UTF8.GetBytes(string.Format("{0}:{1}", _organizationID, _authenticationToken));
            _encodedCredentials = Convert.ToBase64String(credentialsByteArray);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DisplayAndLog("POST Ticket attachment response: " + PostAttachment(_url + "/api/xml/tickets/" + _ticketID.ToString() + "/actions/" + _actionID.ToString() + "/attachments"));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DisplayAndLog("POST Asset attachment response: " + PostAttachment(_url + "/api/xml/assets/" + _assetID.ToString() + "/attachments"));
        }

        private void DisplayAndLog(string message)
        {
            string messageWithDateTime = DateTime.Now.ToString("G") + ": " + message;
            textBox1.AppendText(messageWithDateTime + Environment.NewLine);
            _logs.WriteEvent(messageWithDateTime);
        }

        private string GetInitialText()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("URL: " + _url);
            result.AppendLine("OrganizationID: " + _organizationID.ToString());
            result.AppendLine("Authentication Token: " + _authenticationToken);
            result.AppendLine("TicketID: " + _ticketID);
            result.AppendLine("ActionID: " + _actionID);
            result.AppendLine("AssetID: " + _assetID);
            return result.ToString();
        }

        private string PostAttachment(string URI)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            Stream postDataStream = GetPostStream(@"C:\Users\leona\Documents\Attachment.png", boundary);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URI);
            request.Headers.Add("Authorization", "Basic " + _encodedCredentials);
            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.ContentLength = postDataStream.Length;
            Stream reqStream = request.GetRequestStream();
            postDataStream.Position = 0;
            byte[] buffer = new byte[1024];
            int bytesRead = 0;
            while ((bytesRead = postDataStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                reqStream.Write(buffer, 0, bytesRead);
            }
            postDataStream.Close();
            reqStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(response.CharacterSet));
            return response.StatusDescription;
        }
        private static Stream GetPostStream(string filePath, string boundary)
        {
            Stream postDataStream = new System.IO.MemoryStream();
            FileInfo fileInfo = new FileInfo(filePath);
            string fileHeaderTemplate = "--" + boundary + Environment.NewLine +
            "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" +
            Environment.NewLine + "Content-Type: */*" + Environment.NewLine + Environment.NewLine;
            byte[] fileHeaderBytes = System.Text.Encoding.UTF8.GetBytes(string.Format(fileHeaderTemplate, "UploadFile", fileInfo.FullName));
            postDataStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);
            FileStream fileStream = fileInfo.OpenRead();
            byte[] buffer = new byte[1024];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                postDataStream.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();
            byte[] endBoundaryBytes = System.Text.Encoding.UTF8.GetBytes(Environment.NewLine + "--" + boundary);
            postDataStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            return postDataStream;
        }
    }
}
