using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  class WaterCoolerIndexDataSource : IndexDataSource
  {
    protected WaterCoolerIndexDataSource() { }

    public WaterCoolerIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, string table, bool isRebuilding, Logs logs)
      : base(loginUser, maxCount, organizationID, table, isRebuilding, logs)
    {
    }

    override protected void GetNextRecord()
    {
      WaterCoolerViewItem waterCooler = WaterCoolerView.GetWaterCoolerViewItem(_loginUser, _itemIDList[_rowIndex]);
      _lastItemID = waterCooler.MessageID;
      UpdatedItems.Add((int)_lastItemID);

      DocText = HtmlToText.ConvertHtml(waterCooler.Message);

      _docFields.Clear();
      foreach (DataColumn column in waterCooler.Collection.Table.Columns)
      {
        object value = waterCooler.Row[column];
        string s = value == null || value == DBNull.Value ? "" : value.ToString();
        AddDocField(column.ColumnName, s);
      }

      DocFields = _docFields.ToString();
      DocIsFile = false;
      DocName = waterCooler.MessageID.ToString();
      DocCreatedDate = waterCooler.TimeStampUtc;
      DocModifiedDate = DateTime.UtcNow;
    }

    override protected void LoadData()
    {
      WaterCoolerView waterCoolerMessages = new WaterCoolerView(_loginUser);
      waterCoolerMessages.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
      foreach (WaterCoolerViewItem waterCoolerMessage in waterCoolerMessages)
      {
        _itemIDList.Add(waterCoolerMessage.MessageID);
      }
    }
  }
}
