using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TeamSupport.IDTree
{
    public interface INoteDestination
    {

    }

    public class NoteModel : IDNode, IAttachmentDestination
    {
        public INoteDestination NoteDestination { get; private set; }
        public int NoteID { get; private set; }
        int IAttachmentDestination.RefID => NoteID;

        //public NoteModel(IAttachmentDestination attachedTo, int id) : base((attachedTo as IDNode).Connection)
        //{
        //    NoteID = id;
        //    //AttachedTo = attachedTo;
        //}

        public NoteModel(INoteDestination noteDestination, int noteID) : base(noteDestination as IDNode)
        {
            NoteDestination = noteDestination;
            NoteID = noteID;
        }

        //public NoteModel(ConnectionContext connection, int noteID) : base(connection)
        //{
        //    NoteID = noteID;

        //    //int id = ExecuteQuery<int>($"SELECT RefID FROM Notes WITH (NOLOCK) WHERE NoteID={noteID}").Min();
        //    //int refType = ExecuteQuery<int>($"SELECT RefType FROM Notes WITH (NOLOCK) WHERE NoteID={noteID}").Min();
        //    //Ticket = new TicketModel(Connection, ticketID);
        //    Verify();
        //}

        public override void Verify()
        {
            // also check if AttachedTo is valid?
            Verify($"SELECT NoteID FROM Notes WITH (NOLOCK) WHERE NoteID={NoteID}");
        }

        public string AttachmentPath
        {
            get
            {
                string path = Connection.Organization.AttachmentPath;
                if (NoteDestination is UserModel)
                    path = Path.Combine(path, "ContactActivityAttachments");    // C:\TSData\Organizations\1078\CustomerActivityAttachments\135627\ActionToAnalyze.txt
                else if (NoteDestination is OrganizationModel)
                    path = Path.Combine(path, "CustomerActivityAttachments");
                else
                {
                    if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
                    throw new Exception("Unknown INoteDestination");
                }
                path = Path.Combine(path, NoteID.ToString());
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }
    }

}
