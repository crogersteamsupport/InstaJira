using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TeamSupport.Data;
using Newtonsoft.Json;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  class ProductVersionIndexDataSource : IndexDataSource
  {
    protected ProductVersionIndexDataSource() { }

    public ProductVersionIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, string table, bool isRebuilding, Logs logs)
      : base(loginUser, maxCount, organizationID, table, isRebuilding, logs)
    {
    }

    override protected void GetNextRecord()
    {
      ProductVersionsViewItem productVersion = ProductVersionsView.GetProductVersionsViewItem(_loginUser, _itemIDList[_rowIndex]);
      _lastItemID = productVersion.ProductVersionID;
      UpdatedItems.Add((int)_lastItemID);

      DocText = HtmlToText.ConvertHtml(productVersion.Description == null ? string.Empty : productVersion.Description);

      _docFields.Clear();
      foreach (DataColumn column in productVersion.Collection.Table.Columns)
      {
        object value = productVersion.Row[column];
        string s = value == null || value == DBNull.Value ? "" : value.ToString();
        AddDocField(column.ColumnName, s);
      }

      ProductVersionsSearch productVersionsSearch = new ProductVersionsSearch(productVersion);
      Tickets tickets = new Tickets(_loginUser);
      productVersionsSearch.openTicketCount = tickets.GetProductVersionTicketCount(productVersion.ProductVersionID, 0, _organizationID);
      AddDocField("**JSON", JsonConvert.SerializeObject(productVersionsSearch));

      CustomValues customValues = new CustomValues(_loginUser);
      customValues.LoadByReferenceType(_organizationID, ReferenceType.ProductVersions, null, productVersion.ProductVersionID);

      foreach (CustomValue value in customValues)
      {
        object o = value.Row["CustomValue"];
        string s = o == null || o == DBNull.Value ? "" : o.ToString();
        AddDocField(value.Row["Name"].ToString(), s);
      }
      DocFields = _docFields.ToString();
      DocIsFile = false;
      DocName = productVersion.ProductVersionID.ToString();
      DocCreatedDate = productVersion.DateCreatedUtc;
      DocModifiedDate = DateTime.UtcNow;
    }

    override protected void LoadData()
    {
      ProductVersionsView productVersions = new ProductVersionsView(_loginUser);
      productVersions.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
      foreach (ProductVersionsViewItem productVersion in productVersions)
      {
        _itemIDList.Add(productVersion.ProductVersionID);
      }
    }
  }
}
