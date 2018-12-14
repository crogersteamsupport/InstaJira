/*function TicketLoadFilter() { 
  this.UnassignedValue = -1
  this.TicketTypeID = null;
  this.ProductID = null;
  this.ResolvedID = null;
  this.ReportedID = null;
  this.IsClosed = null;
  this.TicketStatusID = null;
  this.TicketSeverityID = null;
  this.UserID = null;
  this.GroupID = null;
  this.CustomerID = null;
  this.IsVisibleOnPortal = null;
  this.IsKnowledgeBase = null;
  this.DateCreatedBegin = null;
  this.DateCreatedEnd = null;
  this.DateModifiedBegin = null;
  this.DateModifiedEnd = null;
  this.Tags = new Array();
  this.SearchText = ''; 
}

/*
function TicketLoadFilter() { 
  this._TicketTypeID = null;
  this._ProductID = null;
  this._ResolvedID = null;
  this._ReportedID = null;
  this._IsClosed = null;
  this._TicketStatusID = null;
  this._TicketSeverityID = null;
  this._UserID = null;
  this._GroupID = null;
  this._CustomerID = null;
  this._IsVisibleOnPortal = null;
  this._IsKnowledgeBase = null;
  this._DateCreatedBegin = null;
  this._DateCreatedEnd = null;
  this._DateModifiedBegin = null;
  this._DateModifiedEnd = null;
  this._Tags = new Array();
  this._SearchText = ''; 
}


TicketLoadFilter.prototype =
{
  constructor: TicketLoadFilter,

  getTicketTypeID: function() { return this._TicketTypeID; },
  getProductID: function() { return this._ProductID; },
  getResolvedID: function() { return this._ResolvedID; },
  getReportedID: function() { return this._ReportedID; },
  getTicketStatusID: function() { return this._TicketStatusID; },
  getTicketSeverityID: function() { return this._TicketSeverityID; },
  getUserID: function() { return this._UserID; },
  getGroupID: function() { return this._GroupID; },
  getCustomerID: function() { return this._CustomerID; },
  getIsClosed: function() { return this._IsClosed; },
  getIsVisibleOnPortal: function() { return this._IsVisibleOnPortal; },
  getIsKnowledgeBase: function() { return this._IsKnowledgeBase; },
  getDateCreatedBegin: function() { return this._DateCreatedBegin; },
  getDateCreatedEnd: function() { return this._DateCreatedEnd; },
  getDateModifiedBegin: function() { return this._DateModifiedBegin; },
  getDateModifiedEnd: function() { return this._DateModifiedEnd; },
  getTags: function() { return this._Tags; },
  getSearchText: function() { return this._SearchText; },
  setTicketTypeID: function(value) { this._TicketTypeID = value; },
  setProductID: function(value) { this._ProductID = value; },
  setResolvedID: function(value) { this._ResolvedID = value; },
  setReportedID: function(value) { this._ReportedID = value; },
  setTicketStatusID: function(value) { this._TicketStatusID = value; },
  setTicketSeverityID: function(value) { this._TicketSeverityID = value; },
  setUserID: function(value) { this._UserID = value; },
  setGroupID: function(value) { this._GroupID = value; },
  setCustomerID: function(value) { this._CustomerID = value; },
  setIsClosed: function(value) { this._IsClosed = value; },
  setIsVisibleOnPortal: function(value) { this._IsVisibleOnPortal = value; },
  setIsKnowledgeBase: function(value) { this._IsKnowledgeBase = value; },
  setDateCreatedBegin: function(value) { this._DateCreatedBegin = value; },
  setDateCreatedEnd: function(value) { this._DateCreatedEnd = value; },
  setDateModifiedBegin: function(value) { this._DateModifiedBegin = value; },
  setDateModifiedEnd: function(value) { this._DateModifiedEnd = value; },
  setTags: function(value) { this._Tags = value; },
  setSearchText: function(value) { this._SearchText = value; }
}



/*
"TicketTypeID":null,
"ProductID":null,
"ResolvedID":null,
"ReportedID":null,
"IsClosed":null,
"TicketStatusID":null,
"TicketSeverityID":null,
"UserID":null,
"GroupID":null,
"CustomerID":null,
"IsVisibleOnPortal":null,
"IsKnowledgeBase":null,
"DateCreatedBegin":null,
"DateCreatedEnd":null,
"DateModifiedBegin":null,
"DateModifiedEnd":null,
"SearchText":null,
"Tags":[]*/