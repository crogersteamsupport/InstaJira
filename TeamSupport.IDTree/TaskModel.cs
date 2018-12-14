using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TeamSupport.IDTree
{

    public class TaskModel : IDNode, IAttachmentDestination
    {
        OrganizationModel Organization;
        public int TaskID { get; private set; }
        int IAttachmentDestination.RefID => TaskID;

        public TaskModel(OrganizationModel organization, int taskID) : base(organization)
        {
            Organization = organization;
            TaskID = taskID;
            Verify();
        }

        public TaskModel(ConnectionContext connection, int taskID) : base(connection)
        {
            Organization = connection.Organization;
            TaskID = taskID;
            Verify();
        }

        public override void Verify()
        {
            Verify($"SELECT TaskID FROM Tasks WITH (NOLOCK) WHERE TaskID={TaskID} AND OrganizationID={Organization.OrganizationID}");
        }

        // C:\TSData\Organizations\1078\Tasks\57269\file.txt
        public string AttachmentPath
        {
            get
            {
                string path = Organization.AttachmentPath;
                path = Path.Combine(path, "Tasks");   // see static AttachmentAPI()
                path = Path.Combine(path, TaskID.ToString());
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }
    }

}
