using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
    [DataContract(Namespace = "http://teamsupport.com/")]
    [KnownType(typeof(RecentlyViewedItemProxy))]
    public class RecentlyViewedItemProxy
    {
        public RecentlyViewedItemProxy() { }
        public RecentlyViewedItemProxy(ReferenceType referenceType) { }
        [DataMember] public int UserID { get; set; }
    [DataMember] public int RefType { get; set; }
        [DataMember] public int RefID { get; set; }
        [DataMember] public DateTime DateViewed { get; set; }

        public static RecentlyViewedItemProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Actions:
                    return new ActionsRecentlyViewedItemProxy();
                case References.ActionTypes:
                    return new ActionTypesRecentlyViewedItemProxy();
                case References.Products:
                    return new ProductsRecentlyViewedItemProxy();
                case References.ProductVersions:
                    return new ProductVersionsRecentlyViewedItemProxy();
                case References.Assets:
                    return new AssetsRecentlyViewedItemProxy();
                case References.ProductFamilies:
                    return new ProductFamiliesRecentlyViewedItemProxy();
                default: throw new Exception("Invalid PhoneNumber Reference Type");
            }
        }
        public enum References
        {
            Actions = ReferenceType.Actions,
            ActionTypes = ReferenceType.ActionTypes,
            Products = ReferenceType.Products,
            ProductVersions = ReferenceType.ProductVersions,
            Assets = ReferenceType.Assets,
            ProductFamilies = ReferenceType.ProductFamilies,
        }

    }

    internal class ProductFamiliesRecentlyViewedItemProxy : RecentlyViewedItemProxy
    {
        public ProductFamiliesRecentlyViewedItemProxy() : base(ReferenceType.ProductFamilies)
        {
        }
    }

    internal class AssetsRecentlyViewedItemProxy : RecentlyViewedItemProxy
    {
        public AssetsRecentlyViewedItemProxy() : base(ReferenceType.Assets)
        {
        }
    }

    internal class ProductVersionsRecentlyViewedItemProxy : RecentlyViewedItemProxy
    {
        public ProductVersionsRecentlyViewedItemProxy() : base(ReferenceType.ProductVersions)
        {
        }
    }

    internal class ProductsRecentlyViewedItemProxy : RecentlyViewedItemProxy
    {
        public ProductsRecentlyViewedItemProxy() : base(ReferenceType.Products)
        {
        }
    }

    internal class ActionTypesRecentlyViewedItemProxy : RecentlyViewedItemProxy
    {
        public ActionTypesRecentlyViewedItemProxy() : base(ReferenceType.ActionTypes)
        {
        }
    }

    internal class ActionsRecentlyViewedItemProxy : RecentlyViewedItemProxy
    {
        public ActionsRecentlyViewedItemProxy() : base(ReferenceType.Users)
        {
        }
    }

}
