using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamSupport.WebUtils
{
  [Serializable]
  public class TicketFilters
  {
    public class Values
    {
      public const int All = -1;
      public const int Unassigned = -2;
      public const int Opened = -3;
      public const int Closed = -4;
    }

    private int _ticketTypeID = Values.All;
    private int _ticketStatusID = Values.All;
    private int _ticketSeverityID = Values.All;
    private int _userID = Values.All;
    private int _groupID = Values.All;
    private int _productID = Values.All;
    private int _reportedVersionID = Values.All;
    private int _resolvedVersionID = Values.All;
    private int _customerID = Values.All;
    private bool? _isPortal = null;
    private bool? _isKnowledgeBase = null;
    private DateTime? _dateCreateBegin = null;
    private DateTime? _dateCreateEnd = null;
    private DateTime? _dateModifiedBegin = null;
    private DateTime? _dateModifiedEnd = null;
    private string _searchText = "";
    private string _tags = "";

    public TicketFilters()
    {
    }

    public int TicketTypeID
    {
      get { return _ticketTypeID; }
      set { _ticketTypeID = value; }
    }

    public int TicketStatusID
    {
      get { return _ticketStatusID; }
      set { _ticketStatusID = value; }
    }

    public int TicketSeverityID
    {
      get { return _ticketSeverityID; }
      set { _ticketSeverityID = value; }
    }

    public int UserID
    {
      get { return _userID; }
      set { _userID = value; }
    }

    public int GroupID
    {
      get { return _groupID; }
      set { _groupID = value; }
    }

    public int ProductID
    {
      get { return _productID; }
      set { _productID = value; }
    }

    public int ReportedVersionID
    {
      get { return _reportedVersionID; }
      set { _reportedVersionID = value; }
    }

    public int ResolvedVersionID
    {
      get { return _resolvedVersionID; }
      set { _resolvedVersionID = value; }
    }
    
    public int CustomerID
    {
      get { return _customerID; }
      set { _customerID = value; }
    }

    public bool? IsPortal
    {
      get { return _isPortal; }
      set { _isPortal = value; }
    }

    public bool? IsKnowledgeBase
    {
      get { return _isKnowledgeBase; }
      set { _isKnowledgeBase = value; }
    }

    public DateTime? DateCreateBegin
    {
      get { return _dateCreateBegin; }
      set { _dateCreateBegin = value; }
    }

    public DateTime? DateCreateEnd
    {
      get { return _dateCreateEnd; }
      set { _dateCreateEnd = value; }
    }

    public DateTime? DateModifiedBegin
    {
      get { return _dateModifiedBegin; }
      set { _dateModifiedBegin = value; }
    }

    public DateTime? DateModifiedEnd
    {
      get { return _dateModifiedEnd; }
      set { _dateModifiedEnd = value; }
    }

    public string SearchText
    {
      get { return _searchText; }
      set { _searchText = value; }
    }

    public string Tags
    {
      get { return _tags; }
      set { _tags = value; }
    }
  }
}
