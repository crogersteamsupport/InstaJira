using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TeamSupport.IDTree
{
    public class AssetModel : IDNode, IAttachmentDestination
    {
        public OrganizationModel Organization { get; private set; }
        public int AssetID { get; private set; }
        int IAttachmentDestination.RefID => AssetID;

        public AssetModel(OrganizationModel organization, int assetID) : base(organization)
        {
            Organization = organization;
            AssetID = assetID;
            Verify();
        }

        public AssetModel(ConnectionContext connection, int assetID) : this(connection.Organization, assetID)
        {
        }

        public override void Verify()
        {
            Verify($"SELECT AssetID FROM Assets WITH (NOLOCK) WHERE AssetID={AssetID} AND OrganizationID={Organization.OrganizationID}");
        }

        public IAttachmentDestination ClassFactory(ConnectionContext connection, int id)
        {
            return new AssetModel(connection, id);
        }

        public string AttachmentPath
        {
            get
            {
                string path = Organization.AttachmentPath;
                path = Path.Combine(path, "AssetAttachments");   // see static AttachmentAPI()
                path = Path.Combine(path, AssetID.ToString());
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }
    }
}
