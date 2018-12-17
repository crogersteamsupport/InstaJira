using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
    [Serializable]
    class WaterCoolerIndexDataSource2 : IndexDataSource2
    {
        protected WaterCoolerIndexDataSource2() { }

        public WaterCoolerIndexDataSource2(LoginUser loginUser, int organizationID, string table, int[] idList, Logs logs)
            : base(loginUser, organizationID, table, idList, logs)
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

    }
}
