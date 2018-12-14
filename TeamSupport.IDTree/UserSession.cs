using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics;

namespace TeamSupport.IDTree
{
    /// <summary>
    /// Wrapper for valid UserID
    /// </summary>
    public class UserSession
    {
        public OrganizationNode Customer { get; private set; }
        public UserNode User { get; private set; }
        Proxy.AuthenticationModel _authentication;

        public int UserID { get { return _authentication.UserID; } }

        /// <summary> OrganizationID and UserID come from ConnectionContext.Authentication </summary>
        public UserSession(OrganizationNode organization)
        {
            Customer = organization;
            ..User = new UserNode(UserID);

            _authentication = Customer.Connection.Authentication;
            //DBReader.VerifyUser(_db, Organization.OrganizationID, UserID);     // connection already verified
        }

        /// <summary> Trace creator/modifier by having user own tickets... </summary>
        //public TicketNode Ticket(int ticketID)
        //{
        //    return new TicketNode(this, ticketID);
        //}

        public bool AllowUserToEditAnyAction() { return IDReader.UserAllowUserToEditAnyAction(Customer.Connection._db, UserID); }
        public bool CanEdit() { return _authentication.IsSystemAdmin || AllowUserToEditAnyAction(); }

    }
}
