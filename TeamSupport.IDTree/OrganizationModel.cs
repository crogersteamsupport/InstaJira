using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO;
using System.Diagnostics;

namespace TeamSupport.IDTree
{
    /// <summary> Wrapper for valid OrganizationID </summary>
    public class OrganizationModel : IDNode, IAttachmentDestination, ITaskAssociation, INoteDestination
    {
        public int OrganizationID { get; private set; }
        int IAttachmentDestination.RefID => OrganizationID;

        /// <summary> OrganizationID and UserID come from ConnectionContext.Authentication </summary>
        public OrganizationModel(ConnectionContext connection) : this(connection, connection.Authentication.OrganizationID, false)
        {
        }

        public OrganizationModel(ConnectionContext connection, int organizationID) : this(connection, organizationID, true)
        {
        }

        private OrganizationModel(ConnectionContext connection, int organizationID, bool verify) : base(connection)
        {
            OrganizationID = organizationID;
            if(verify)
                Verify();
        }

        public OrganizationModel Parent()
        {
            int? parentID = ExecuteQuery<int?>($"SELECT ParentID FROM Organizations WITH (NOLOCK) WHERE OrganizationID={OrganizationID}").FirstOrDefault();
            if (!parentID.HasValue)
                return null;
            return new OrganizationModel(Connection, parentID.Value);
        }

        public string AttachmentPath
        {
            get { return AttachmentPathx(AttachmentModel.AttachmentPathIndex); }
        }

        private string AttachmentPathx(int id)
        {
            string path = Connection.AttachmentPath(id);
            path = Path.Combine(Path.Combine(path, "Organizations"), OrganizationID.ToString());
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public override void Verify()
        {
            Verify($"SELECT OrganizationID FROM Organizations WITH (NOLOCK) WHERE OrganizationID={OrganizationID}");
        }

    }
}
