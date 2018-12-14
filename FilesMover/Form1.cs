using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TeamSupport.Data;

namespace FilesMover
{
    public partial class Form1 : Form
    {
        private LoginUser _loginUser = new LoginUser(ConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString, -5, -1, null);
        private Logs _logs = new Logs(DateTime.Now.ToString("HH-mm"));
        private int _batchSize = Int32.Parse(ConfigurationManager.AppSettings.Get("BatchSize"));
        private string _source = ConfigurationManager.AppSettings.Get("Source");
        private string _destination = ConfigurationManager.AppSettings.Get("Destination");
        private int _filesMoved = 0;
        private bool _stop = false;

        public Form1()
        {
            InitializeComponent();
            DisplayAndLog(GetInitialText());
        }

        private void run_Click(object sender, EventArgs e)
        {
            MoveAttachments();
            MoveImports();
            MoveScheduledReports();
        }

        private void MoveAttachments()
        {
            try
            {
                DisplayAndLog("Start MoveAttachments at " + DateTime.Now.ToString());
                DataTable attachmentsToMove = GetAttachmentsToMove(_batchSize);
                DisplayAndLog("Got batch of " + attachmentsToMove.Rows.Count.ToString() + " attachments.");
                while (attachmentsToMove.Rows.Count > 0 && !_stop)
                {
                    foreach (DataRow fileToMove in attachmentsToMove.Rows)
                    {
                        try
                        {
                            if (!_stop)
                            {
                                string pathWithoutSource = MoveAttachmentFile(fileToMove);
                                UpdateAttachment((int)fileToMove[0], pathWithoutSource);
                                _filesMoved++;
                                DisplayAndLog("Moved file with AttachmentID: " + fileToMove[0].ToString());
                            }
                        }
                        catch (Exception exception)
                        {
                            AddFailedAttachmentMove((int)fileToMove[0], exception);
                            DisplayAndLog("Moving file with AttachmentID: " + fileToMove[0].ToString() + " got exception: " + exception.Message);
                        }
                    }
                    attachmentsToMove = GetAttachmentsToMove(_batchSize);
                    DisplayAndLog("Got batch of " + attachmentsToMove.Rows.Count.ToString() + " attachments.");
                }
            }
            catch (Exception exception)
            {
                DisplayAndLog(exception.Message);
            }
            finally
            {
                DisplayAndLog("Finished moving attachments.");
            }
        }

        private void MoveImports()
        {
            try
            {
                DisplayAndLog("Start MoveImports at " + DateTime.Now.ToString());
                DataTable importsToMove = GetImportsToMove(_batchSize);
                DisplayAndLog("Got batch of " + importsToMove.Rows.Count.ToString() + " imports.");
                while (importsToMove.Rows.Count > 0 && !_stop)
                {
                    foreach (DataRow fileToMove in importsToMove.Rows)
                    {
                        try
                        {
                            if (!_stop)
                            {
                                string pathWithoutSource = MoveImportFile(fileToMove);
                                UpdateImport((int)fileToMove[0]);
                                _filesMoved++;
                                DisplayAndLog("Moved file with ImportID: " + fileToMove[0].ToString());
                            }
                        }
                        catch (Exception exception)
                        {
                            AddFailedImportMove((int)fileToMove[0], exception);
                            DisplayAndLog("Moving file with ImportID: " + fileToMove[0].ToString() + " got exception: " + exception.Message);
                        }
                    }
                    importsToMove = GetImportsToMove(_batchSize);
                    DisplayAndLog("Got batch of " + importsToMove.Rows.Count.ToString() + " imports.");
                }
            }
            catch (Exception exception)
            {
                DisplayAndLog(exception.Message);
            }
            finally
            {
                DisplayAndLog("Finished moving imports.");
            }
        }

        private void MoveScheduledReports()
        {
            try
            {
                DisplayAndLog("Start MoveScheduledReports at " + DateTime.Now.ToString());
                DataTable scheduledReportsToMove = GetScheduledReportsToMove(_batchSize);
                DisplayAndLog("Got batch of " + scheduledReportsToMove.Rows.Count.ToString() + " scheduled reports.");
                while (scheduledReportsToMove.Rows.Count > 0 && !_stop)
                {
                    foreach (DataRow fileToMove in scheduledReportsToMove.Rows)
                    {
                        try
                        {
                            if (!_stop)
                            {
                                string pathWithoutSource = MoveScheduledReportFile(fileToMove);
                                UpdateScheduledReport((int)fileToMove[0]);
                                _filesMoved++;
                                DisplayAndLog("Moved file with ScheduledReportID: " + fileToMove[0].ToString());
                            }
                        }
                        catch (Exception exception)
                        {
                            AddFailedScheduledReportMove((int)fileToMove[0], exception);
                            DisplayAndLog("Moving file with ScheduledReportID: " + fileToMove[0].ToString() + " got exception: " + exception.Message);
                        }
                    }
                    scheduledReportsToMove = GetScheduledReportsToMove(_batchSize);
                    DisplayAndLog("Got batch of " + scheduledReportsToMove.Rows.Count.ToString() + " scheduled reports.");
                }
            }
            catch (Exception exception)
            {
                DisplayAndLog(exception.Message);
            }
            finally
            {
                DisplayAndLog("Finished moving scheduled reports.");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void DisplayAndLog(string message)
        {
            string messageWithCount = "#: " + _filesMoved.ToString() + " " + message;
            textBox1.AppendText(messageWithCount + Environment.NewLine);
            _logs.WriteEvent(messageWithCount);
        }

        private string GetInitialText()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("ConnectionString: " + _loginUser.ConnectionString);
            result.AppendLine("BatchSize: " + _batchSize.ToString());
            result.AppendLine("Source: " + _source);
            result.AppendLine("Destination: " + _destination);
            return result.ToString();
        }

        private DataTable GetAttachmentsToMove(int batchSize)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "LoadMoveAttachmentsQuery";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@batchSize", batchSize);
            command.Parameters.AddWithValue("@source", _source);
            //command.Parameters.AddWithValue("@sourceLength", _source.Length);
            return SqlExecutor.ExecuteQuery(_loginUser, command);
        }

        private DataTable GetImportsToMove(int batchSize)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "LoadMoveImportsQuery";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@batchSize", batchSize);
            return SqlExecutor.ExecuteQuery(_loginUser, command);
        }

        private DataTable GetScheduledReportsToMove(int batchSize)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "LoadMoveScheduledReportsQuery";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@batchSize", batchSize);
            return SqlExecutor.ExecuteQuery(_loginUser, command);
        }

        private string MoveAttachmentFile(DataRow fileToMove)
        {
            string fileName = fileToMove[1].ToString();
            string path = fileToMove[2].ToString();
            string pathWithoutSource = path.Substring(_source.Length, path.Length - _source.Length);
            string pathWithoutSourceAndFile = pathWithoutSource.Replace(fileName, string.Empty);
            if (!Directory.Exists(_destination + pathWithoutSourceAndFile)) Directory.CreateDirectory(_destination + pathWithoutSourceAndFile);
            File.Move(path, _destination + pathWithoutSource);
            return pathWithoutSource;
        }

        private string MoveImportFile(DataRow fileToMove)
        {
            string fileName = fileToMove[0].ToString() + ".txt";
            string pathWithoutSourceAndFile = @"\TSData\Organizations\" + fileToMove[1].ToString() + @"\Imports\Logs\";
            if (!Directory.Exists(_destination + pathWithoutSourceAndFile)) Directory.CreateDirectory(_destination + pathWithoutSourceAndFile);
            string pathWithoutSource = pathWithoutSourceAndFile + fileName;
            File.Move(_source + pathWithoutSource, _destination + pathWithoutSource);
            return pathWithoutSource;
        }

        private string MoveScheduledReportFile(DataRow fileToMove)
        {
            string fileName = fileToMove[0].ToString() + ".txt";
            string pathWithoutSourceAndFile = @"\TSData\Organizations\" + fileToMove[1].ToString() + @"\ScheduledReports\Logs\";
            if (!Directory.Exists(_destination + pathWithoutSourceAndFile)) Directory.CreateDirectory(_destination + pathWithoutSourceAndFile);
            string pathWithoutSource = pathWithoutSourceAndFile + fileName;
            File.Move(_source + pathWithoutSource, _destination + pathWithoutSource);
            return pathWithoutSource;
        }

        private void UpdateAttachment(int attachmentID, string pathWithoutSource)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
                UPDATE
                    Attachments
                SET
                    Path = @newPath,
                    FilePathID = 3
                WHERE
                    AttachmentID = @attachmentID
            ";
            command.Parameters.AddWithValue("@newPath", _destination + pathWithoutSource);
            command.Parameters.AddWithValue("@attachmentID", attachmentID);
            SqlExecutor.ExecuteNonQuery(_loginUser, command);
        }

        private void UpdateImport(int importID)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
                UPDATE
                    Imports
                SET
                    FilePathID = 3
                WHERE
                    ImportID = @importID
            ";
            command.Parameters.AddWithValue("@importID", importID);
            SqlExecutor.ExecuteNonQuery(_loginUser, command);
        }

        private void UpdateScheduledReport(int id)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
                UPDATE
                    ScheduledReports
                SET
                    FilePathID = 3
                WHERE
                    Id = @id
            ";
            command.Parameters.AddWithValue("@id", id);
            SqlExecutor.ExecuteNonQuery(_loginUser, command);
        }

        private void AddFailedAttachmentMove(int attachmentID, Exception ex)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
                INSERT INTO
                    FailedToMoveAttachments
                VALUES
                (
                    @attachmentID,
                    @exceptionMessage,
                    @exceptionType
                )
            ";
            command.Parameters.AddWithValue("@attachmentID", attachmentID);
            command.Parameters.AddWithValue("@exceptionMessage", ex.Message);
            command.Parameters.AddWithValue("@exceptionType", ex.GetType().ToString());
            SqlExecutor.ExecuteNonQuery(_loginUser, command);
        }

        private void AddFailedImportMove(int importID, Exception ex)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
                INSERT INTO
                    FailedToMoveImports
                VALUES
                (
                    @importID,
                    @exceptionMessage,
                    @exceptionType
                )
            ";
            command.Parameters.AddWithValue("@importID", importID);
            command.Parameters.AddWithValue("@exceptionMessage", ex.Message);
            command.Parameters.AddWithValue("@exceptionType", ex.GetType().ToString());
            SqlExecutor.ExecuteNonQuery(_loginUser, command);
        }

        private void AddFailedScheduledReportMove(int id, Exception ex)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
                INSERT INTO
                    FailedToMoveScheduledReports
                VALUES
                (
                    @id,
                    @exceptionMessage,
                    @exceptionType
                )
            ";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@exceptionMessage", ex.Message);
            command.Parameters.AddWithValue("@exceptionType", ex.GetType().ToString());
            SqlExecutor.ExecuteNonQuery(_loginUser, command);
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void stepAttachment_Click(object sender, EventArgs e)
        {
            try
            {
                DisplayAndLog("Start at " + DateTime.Now.ToString());
                DataTable filesToMove = GetAttachmentsToMove(1);
                DisplayAndLog("Got batch of " + filesToMove.Rows.Count.ToString() + " files.");
                foreach (DataRow fileToMove in filesToMove.Rows)
                {
                    try
                    {
                        string pathWithoutSource = MoveAttachmentFile(fileToMove);
                        UpdateAttachment((int)fileToMove[0], pathWithoutSource);
                        _filesMoved++;
                        DisplayAndLog("Moved file with AttachmentID: " + fileToMove[0].ToString());
                    }
                    catch (Exception exception)
                    {
                        AddFailedAttachmentMove((int)fileToMove[0], exception);
                        DisplayAndLog("Moving file with AttachmentID: " + fileToMove[0].ToString() + " got exception: " + exception.Message);
                    }
                }
            }
            catch (Exception exception)
            {
                DisplayAndLog(exception.Message);
            }
            finally
            {
                DisplayAndLog("Finished step attachment.");
            }
        }

        private void stop_Click(object sender, EventArgs e)
        {
            _stop = true;
        }

        private void stepImport_Click(object sender, EventArgs e)
        {
            try
            {
                DisplayAndLog("Start at " + DateTime.Now.ToString());
                DataTable filesToMove = GetImportsToMove(1);
                DisplayAndLog("Got batch of " + filesToMove.Rows.Count.ToString() + " imports.");
                foreach (DataRow fileToMove in filesToMove.Rows)
                {
                    try
                    {
                        string pathWithoutSource = MoveImportFile(fileToMove);
                        UpdateImport((int)fileToMove[0]);
                        _filesMoved++;
                        DisplayAndLog("Moved file with ImportID: " + fileToMove[0].ToString());
                    }
                    catch (Exception exception)
                    {
                        AddFailedImportMove((int)fileToMove[0], exception);
                        DisplayAndLog("Moving file with ImportID: " + fileToMove[0].ToString() + " got exception: " + exception.Message);
                    }
                }
            }
            catch (Exception exception)
            {
                DisplayAndLog(exception.Message);
            }
            finally
            {
                DisplayAndLog("Finished step import.");
            }
        }

        private void stepScheduledReport_Click(object sender, EventArgs e)
        {
            try
            {
                DisplayAndLog("Start at " + DateTime.Now.ToString());
                DataTable filesToMove = GetScheduledReportsToMove(1);
                DisplayAndLog("Got batch of " + filesToMove.Rows.Count.ToString() + " scheduled reports.");
                foreach (DataRow fileToMove in filesToMove.Rows)
                {
                    try
                    {
                        string pathWithoutSource = MoveScheduledReportFile(fileToMove);
                        UpdateScheduledReport((int)fileToMove[0]);
                        _filesMoved++;
                        DisplayAndLog("Moved scheduled report with ID: " + fileToMove[0].ToString());
                    }
                    catch (Exception exception)
                    {
                        AddFailedScheduledReportMove((int)fileToMove[0], exception);
                        DisplayAndLog("Moving scheduled report with ID: " + fileToMove[0].ToString() + " got exception: " + exception.Message);
                    }
                }
            }
            catch (Exception exception)
            {
                DisplayAndLog(exception.Message);
            }
            finally
            {
                DisplayAndLog("Finished step scheduled report.");
            }
        }
    }
}