using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
    [Serializable]
    class NoteIndexDataSource2 : IndexDataSource2
    {
        protected NoteIndexDataSource2() { }

        public NoteIndexDataSource2(LoginUser loginUser, int organizationID, string table, int[] idList, Logs logs)
            : base(loginUser, organizationID, table, idList, logs)
        {
        }
        override protected void GetNextRecord()
        {
            NotesViewItem note = NotesView.GetNotesViewItem(_loginUser, _itemIDList[_rowIndex]);
            _lastItemID = note.NoteID;
            UpdatedItems.Add((int)_lastItemID);


            DocText = HtmlToText.ConvertHtml(note.Description);

            _docFields.Clear();
            foreach (DataColumn column in note.Collection.Table.Columns)
            {
                object value = note.Row[column];
                string s = value == null || value == DBNull.Value ? "" : value.ToString();
                AddDocField(column.ColumnName, s);
            }
            DocFields = _docFields.ToString();
            DocIsFile = false;
            DocName = note.NoteID.ToString();
            DocCreatedDate = note.DateCreatedUtc;
            DocModifiedDate = DateTime.UtcNow;
        }


    }
}
