using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Data.Linq.Mapping;

namespace TeamSupport.Data
{
    [DataContract(Namespace = "http://teamsupport.com/")]
    [KnownType(typeof(TicketProxy))]
    [Table(Name = "Tickets")]
    public class TicketProxy
    {
        public TicketProxy() { }
        [DataMember, Column(DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)] public int TicketID { get; set; }
        [DataMember, Column] public int? ReportedVersionID { get; set; }
        [DataMember, Column] public int? SolvedVersionID { get; set; }
        [DataMember, Column] public int? ProductID { get; set; }
        [DataMember, Column] public int? GroupID { get; set; }
        [DataMember, Column] public int? UserID { get; set; }
        [DataMember, Column] public int TicketStatusID { get; set; }
        [DataMember, Column] public int TicketTypeID { get; set; }
        [DataMember, Column] public int TicketSeverityID { get; set; }
        [DataMember, Column] public int OrganizationID { get; set; }
        [DataMember, Column] public string Name { get; set; }
        [DataMember, Column] public int? ParentID { get; set; }
        [DataMember, Column] public int TicketNumber { get; set; }
        [DataMember, Column] public bool IsVisibleOnPortal { get; set; }
        [DataMember, Column] public bool IsKnowledgeBase { get; set; }
        [DataMember, Column] public DateTime? DateClosed { get; set; }
        [DataMember, Column] public int? CloserID { get; set; }
        [DataMember, Column] public string ImportID { get; set; }
        [DataMember, Column] public DateTime? LastViolationTime { get; set; }
        [DataMember, Column] public DateTime? LastWarningTime { get; set; }
        [DataMember, Column] public string TicketSource { get; set; }
        [DataMember, Column] public string PortalEmail { get; set; }
        [DataMember, Column] public DateTime? SlaViolationTimeClosed { get; set; }
        [DataMember, Column] public DateTime? SlaViolationLastAction { get; set; }
        [DataMember, Column] public DateTime? SlaViolationInitialResponse { get; set; }
        [DataMember, Column] public DateTime? SlaWarningTimeClosed { get; set; }
        [DataMember, Column] public DateTime? SlaWarningLastAction { get; set; }
        [DataMember, Column] public DateTime? SlaWarningInitialResponse { get; set; }
        [DataMember, Column] public bool NeedsIndexing { get; set; }
        [DataMember, Column] public int? DocID { get; set; }
        [DataMember, Column] public DateTime DateCreated { get; set; }
        [DataMember, Column] public DateTime DateModified { get; set; }
        [DataMember, Column] public int CreatorID { get; set; }
        [DataMember, Column] public int ModifierID { get; set; }
        [DataMember, Column] public int? KnowledgeBaseCategoryID { get; set; }
        [DataMember, Column] public string SalesForceID { get; set; }
        [DataMember, Column] public DateTime? DateModifiedBySalesForceSync { get; set; }
        [DataMember, Column] public int? ImportFileID { get; set; }

        //string ToSql<T>(T value) { return value == null ? "NULL" : value.ToString(); }
        //string ToSql(int? value) { return value.HasValue ? value.Value.ToString() : "NULL"; }
        //string ToSql(DateTime? value) { return value.HasValue ? ToSql(value.Value) : "NULL"; }
        //string ToSql(DateTime value) { return "'" + value.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'"; }
        //string ToSql(bool value) { return value ? "1" : "0"; }
        //string ToSql(string value) { return $"'{value}'"; }

        //public string InsertCommandText(int ticketNumber)
        //{
        //    return $"INSERT INTO Tickets (" +
        //        $"ReportedVersionID, SolvedVersionID, ProductID, GroupID, UserID, TicketStatusID, TicketTypeID, " +
        //        $"TicketSeverityID, OrganizationID, Name, ParentID, TicketNumber, IsVisibleOnPortal, IsKnowledgeBase, " +
        //        $"DateClosed, CloserID, ImportID, LastViolationTime, LastWarningTime, TicketSource, PortalEmail, " +
        //        $"SlaViolationTimeClosed, SlaViolationLastAction, SlaViolationInitialResponse, SlaWarningTimeClosed, " +
        //        $"SlaWarningLastAction, SlaWarningInitialResponse, NeedsIndexing, DocID, DateCreated, DateModified, " +
        //        $"CreatorID, ModifierID, KnowledgeBaseCategoryID, DateModifiedBySalesForceSync, SalesForceID, ImportFileID" +
        //        $") VALUES(" +
        //        $"{ToSql(ReportedVersionID)}, {ToSql(SolvedVersionID)}, {ToSql(ProductID)}, {ToSql(GroupID)}, {ToSql(UserID)}, {ToSql(TicketStatusID)}, {ToSql(TicketTypeID)}, " +
        //        $"{ToSql(TicketSeverityID)}, {ToSql(OrganizationID)}, {ToSql(Name)}, {ToSql(ParentID)}, {ticketNumber}, {ToSql(IsVisibleOnPortal)}, {ToSql(IsKnowledgeBase)}, " +
        //        $"{ToSql(DateClosed)}, {ToSql(CloserID)}, {ToSql(ImportID)}, {ToSql(LastViolationTime)}, {ToSql(LastWarningTime)}, {ToSql(TicketSource)}, {ToSql(PortalEmail)}, " +
        //        $"{ToSql(SlaViolationTimeClosed)}, {ToSql(SlaViolationLastAction)}, {ToSql(SlaViolationInitialResponse)}, {ToSql(SlaWarningTimeClosed)}, " +
        //        $"{ToSql(SlaWarningLastAction)}, {ToSql(SlaWarningInitialResponse)}, {ToSql(NeedsIndexing)}, {ToSql(DocID)}, {ToSql(DateCreated)}, {ToSql(DateModified)}, " +
        //        $"{ToSql(CreatorID)}, {ToSql(ModifierID)}, {ToSql(KnowledgeBaseCategoryID)}, {ToSql(DateModifiedBySalesForceSync)}, {ToSql(SalesForceID)}, {ToSql(ImportFileID)}" +
        //        $") SELECT SCOPE_IDENTITY()";
        //}
    }

}