using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.IO;
using System.Web;
using System.Diagnostics;
using TeamSupport.Data;
using System.Web.Security;

namespace TeamSupport.IDTree
{
    /// <summary> interface to model class that supports attachments </summary>
    public interface IAttachmentDestination
    {
        string AttachmentPath { get; }
        int RefID { get; }
    }


    /// <summary> Model to verify AttachmetID </summary>
    public class AttachmentModel : IDNode
    {
        // hard coded index into FilePaths table
        public const int AttachmentPathIndex = 3;

        public IAttachmentDestination AttachedTo { get; protected set; }  // what are we attached to?
        public int AttachmentID { get; protected set; }
        public AttachmentFile File { get; protected set; }

        public AttachmentModel(IAttachmentDestination attachedTo, int id) : base((attachedTo as IDNode).Connection)
        {
            AttachmentID = id;
            AttachedTo = attachedTo;
            Verify();
        }

        public AttachmentModel(ConnectionContext connection, AttachmentProxy attachment) : base(connection)
        {
            AttachmentID = attachment.AttachmentID;
            switch(attachment)
            {
                case UserPhotoAttachmentProxy proxy:
                    AttachedTo = new UserModel(connection, proxy.RefID);
                    break;
            }
        }

        public override void Verify()
        {
            // also check if AttachedTo is valid?
            Verify($"SELECT AttachmentID FROM Attachments WITH (NOLOCK) WHERE AttachmentID={AttachmentID} AND RefID={AttachedTo.RefID}");
        }
    }

}

