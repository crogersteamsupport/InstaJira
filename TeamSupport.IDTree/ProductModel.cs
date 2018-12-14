using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class ProductModel : IDNode
    {
        public OrganizationModel Organization { get; private set; }
        public int ProductID { get; private set; }

        public ProductModel(OrganizationModel organization, int productID) : base(organization)
        {

        }

        public ProductModel(ConnectionContext connection, int productID) : base(connection)
        {
            int organizationID = ExecuteQuery<int>($"SELECT OrganizationID FROM Products WITH (NOLOCK) WHERE ProductID = {productID}").Min();
            Organization = new OrganizationModel(connection, organizationID);
            ProductID = productID;
        }

        public override void Verify()
        {
            throw new NotImplementedException();
        }
    }
}
