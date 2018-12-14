using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  class NoteIndexDataSource : IndexDataSource
  {
    protected NoteIndexDataSource() { }

    public NoteIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, string table, bool isRebuilding, Logs logs) : base(loginUser, maxCount, organizationID, table, isRebuilding, logs) { }

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

    override protected void LoadData()
    {
      NotesView notes = new NotesView(_loginUser);
      notes.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
      foreach (NotesViewItem note in notes)
      {
        _itemIDList.Add(note.NoteID);
      }
    }
  }
}
