using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class AssetTicketModel : IDNode
    {
        public AssetModel Asset { get; private set; }
        public TicketModel Ticket { get; private set; }

        public AssetTicketModel(AssetModel asset, TicketModel ticket) : this(asset, ticket, true)
        {
        }

        private AssetTicketModel(AssetModel asset, TicketModel ticket, bool verify) : base(ticket)
        {
            Asset = asset;
            Ticket = ticket;
            if (verify)
                Verify();
        }

        public static AssetTicketModel[] GetAssetTickets(TicketModel ticket)
        {
            OrganizationModel organization = ticket.Organization;
            //int[] ids = IDReader.Read(TicketChild.Asset, ticket);   // $"SELECT AssetID From AssetTickets WITH (NOLOCK) WHERE TicketID = {ticket.TicketID}"
            string query = $"SELECT AssetID From AssetTickets WITH (NOLOCK) WHERE TicketID = {ticket.TicketID}";
            int[] ids = ticket.ExecuteQuery<int>(query).ToArray();
            AssetTicketModel[] models = new AssetTicketModel[ids.Length];
            for (int i = 0; i < ids.Length; ++i)
                models[i] = new AssetTicketModel(new AssetModel(organization, ids[i]), ticket, false);
            return models;
        }

        public override void Verify()
        {
            Verify($"SELECT AssetID FROM AssetTickets WITH (NOLOCK) WHERE AssetID={Asset.AssetID} AND TicketID={Ticket.TicketID}");
        }
    }
}
