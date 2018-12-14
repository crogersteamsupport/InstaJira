using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TeamSupport.IDTree
{
    public class ChatModel : IDNode, IAttachmentDestination
    {
        ChatModel chat;
        public int ChatID { get; private set; }
        int IAttachmentDestination.RefID => ChatID;

        public ChatModel(ChatModel action, int chatID) : base(action)
        {
            chat = action;
            ChatID = chatID;
            Verify();
        }

        public ChatModel(ConnectionContext connection, int chatID) : base(connection)
        {
            //var ChatID = Connection._db.ExecuteQuery<int>($"SELECT ChatID FROM Chats WITH (NOLOCK) WHERE ChatID = {chatID}").Min();
            //chat = new ChatModel(connection, chatID);
            ChatID = chatID;
            Verify();
        }

        public override void Verify()
        {
            Verify($"SELECT ChatID FROM Chats WITH (NOLOCK) WHERE ChatID={ChatID}");
        }

        public string AttachmentPath
        {
            get
            {
                string path = Connection.Organization.AttachmentPath;
                path = Path.Combine(path, "ChatAttachments");   // see static AttachmentAPI()
                path = Path.Combine(path, ChatID.ToString());
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }
    }

}
