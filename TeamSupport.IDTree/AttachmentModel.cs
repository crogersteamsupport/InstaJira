﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.IO;
using System.Web;
using System.Diagnostics;
using TeamSupport.Proxy;
using TeamSupport.Data;
using System.Web.Security;

namespace TeamSupport.IDTree
{
    // interface to model class that supports attachments 
    public interface IAttachmentDestination
    {
        string AttachmentPath { get; }
        //IDNode AsIDNode { get; }    // back door to map class to IDNode at compile time
    }

    /// <summary>
    /// Base class for Attachments
    /// </summary>
    public class AttachmentModel : IDNode
    {
        // hard coded index into FilePaths table ???
        public const int AttachmentPathIndex = 3;

        public IAttachmentDestination AttachedTo { get; protected set; }  // what are we attached to?
        public int AttachmentID { get; protected set; }
        public AttachmentFile File { get; protected set; }

        public AttachmentModel(IAttachmentDestination attachedTo, int id) : base((attachedTo as IDNode).Connection)
        {
            AttachmentID = id;
            AttachedTo = attachedTo;
        }

        public override void Verify()
        {
            // also check if AttachedTo is valid?
            Verify($"SELECT AttachmentID FROM Attachments WITH (NOLOCK) WHERE AttachmentID={AttachmentID} AND OrganizationID={Connection.OrganizationID}");
        }

        public AttachmentModel(ConnectionContext connection, int id) : base(connection)
        {
        }

    }

}

