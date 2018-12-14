using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using TeamSupport.Data;
using Newtonsoft.Json;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  class ProductIndexDataSource : IndexDataSource
  {
    protected ProductIndexDataSource() { }
    public ProductIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, string table, bool isRebuilding, Logs logs)
      : base(loginUser, maxCount, organizationID, table, isRebuilding, logs)
    {
    }

    override protected void GetNextRecord()
    {
      Product product = Products.GetProduct(_loginUser, _itemIDList[_rowIndex]);
      _lastItemID = product.ProductID;
      UpdatedItems.Add((int)_lastItemID);

      StringBuilder builder = new StringBuilder();
      builder.AppendLine(product.Description
      + " " + product.Name
      + " " + product.DateCreated
      + " " + product.DateModified);

      DocText = builder.ToString();

      _docFields.Clear();
      AddDocField("ProductID", product.ProductID);
      AddDocField("Name", product.Name);
        if (product.ProductFamilyID != null)
        {
            AddDocField("ProductFamilyID", (int)product.ProductFamilyID);
        }
      DocDisplayName = product.Name;

      ProductSearch productItem = new ProductSearch(product);
      Tickets tickets = new Tickets(_loginUser);
      productItem.openTicketCount = tickets.GetProductTicketCount(product.ProductID, 0);
      AddDocField("**JSON", JsonConvert.SerializeObject(productItem));

      CustomValues customValues = new CustomValues(_loginUser);
      customValues.LoadByReferenceType(_organizationID, ReferenceType.Products, product.ProductID);

      foreach (CustomValue value in customValues)
      {
        object o = value.Row["CustomValue"];
        string s = o == null || o == DBNull.Value ? "" : o.ToString();
        AddDocField(value.Row["Name"].ToString(), s);
      }
      DocFields = _docFields.ToString();
      DocIsFile = false;
      DocName = product.ProductID.ToString();
      try
      {
        DocCreatedDate = (DateTime)product.Row["DateCreated"];
        DocModifiedDate = (DateTime)product.Row["DateModified"];
      }
      catch (Exception)
      {

      }
    }

    override protected void LoadData()
    {
      Products products = new Products(_loginUser);
      products.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
      foreach (Product product in products)
      {
        _itemIDList.Add(product.ProductID);
      }
    }
  }
}
