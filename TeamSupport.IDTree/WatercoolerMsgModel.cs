using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TeamSupport.IDTree
{
    public class WatercoolerMsgModel : IDNode, IAttachmentDestination
    {
        OrganizationModel Organization;
        public int MessageID { get; private set; }
        int IAttachmentDestination.RefID => MessageID;

        public WatercoolerMsgModel(OrganizationModel organization, int messageID) : base(organization)
        {
            Organization = organization;
            MessageID = messageID;
            Verify();
        }

        public WatercoolerMsgModel(ConnectionContext connection, int messageID) : base(connection)
        {
            Organization = connection.Organization;
            MessageID = messageID;
            Verify();
        }

        public override void Verify()
        {
            Verify($"SELECT MessageID FROM WatercoolerMsg WITH (NOLOCK) WHERE MessageID={MessageID}");
        }

        // C:\TSData\Organizations\1078\Tasks\57269\file.txt
        public string AttachmentPath
        {
            get
            {
                string path = Organization.AttachmentPath;
                path = Path.Combine(path, "WaterCooler");   // see static AttachmentAPI()
                path = Path.Combine(path, MessageID.ToString());
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }
    }
}
