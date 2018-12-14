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
    [KnownType(typeof(ActionProxy))]
    [Table(Name = "Actions")]
    public class ActionProxy
    {
        public ActionProxy() { }
        [DataMember, Column(DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)] public int ActionID { get; set; }
        [DataMember, Column] public int? ActionTypeID { get; set; }
        [DataMember, Column] public SystemActionType SystemActionTypeID { get; set; }
        [DataMember, Column] public string Name { get; set; }
        [DataMember, Column] public int? TimeSpent { get; set; }
        [DataMember, Column] public DateTime? DateStarted { get; set; }
        [DataMember, Column] public bool IsVisibleOnPortal { get; set; }
        [DataMember, Column] public bool IsKnowledgeBase { get; set; }
        [DataMember, Column] public string ImportID { get; set; }
        [DataMember, Column] public DateTime? DateCreated { get; set; }
        [DataMember, Column] public DateTime? DateModified { get; set; }
        [DataMember, Column] public int? CreatorID { get; set; }
        [DataMember, Column] public int? ModifierID { get; set; }
        [DataMember, Column] public int TicketID { get; set; }
        [DataMember, Column] public string Description { get; set; }
        [DataMember] public string DisplayName { get; set; }
        [DataMember, Column] public string SalesForceID { get; set; }
        [DataMember, Column] public DateTime? DateModifiedBySalesForceSync { get; set; }
        [DataMember, Column] public bool Pinned { get; set; }
        [DataMember, Column] public int? ImportFileID { get; set; }
        public string ActionSource { get; set; }

        //public ActionProxy(DataRow row)
        //{
        //    ActionID = (int)row["ActionID"];
        //    ActionTypeID = (int?)row["ActionTypeID"];
        //    SystemActionTypeID = (SystemActionType)row["SystemActionTypeID"];
        //    Name = (string)row["Name"];
        //    TimeSpent = (int?)row["TimeSpent"];
        //    DateStarted = (DateTime)row["DateStarted"];
        //    IsVisibleOnPortal = (bool)row["IsVisibleOnPortal"];
        //    IsKnowledgeBase = (bool)row["IsKnowledgeBase"];
        //    ImportID = row["ImportID"] != DBNull.Value ? row["ImportID"].ToString().Trim().ToLower() : "";
        //    DateCreated = (DateTime?)row["DateCreated"];
        //    DateModified = (DateTime?)row["DateModified"];
        //    CreatorID = (int?)row["CreatorID"];
        //    ModifierID = (int?)row["ModifierID"];
        //    TicketID = (int)row["TicketID"];
        //    Description = (string)row["Description"];
        //    //DisplayName;
        //    SalesForceID = row["SalesForceID"] != DBNull.Value ? row["SalesForceID"].ToString().Trim().ToLower() : "";
        //    DateModifiedBySalesForceSync = row["DateModifiedBySalesForceSync"] != DBNull.Value ? (DateTime?)row["DateModifiedBySalesForceSync"] : null;
        //    Pinned = (bool)row["Pinned"];
        //    ImportFileID = row["ImportFileID"] != DBNull.Value ? (int?)row["ImportFileID"] : null; ;
        //}

        //string ToSql<T>(T value) { return value == null ? "NULL" : value.ToString(); }
        //string ToSql(int? value) { return value.HasValue ? value.Value.ToString() : "NULL"; }
        //string ToSql(DateTime? value) { return value.HasValue ? ToSql(value.Value) : "NULL"; }
        //string ToSql(DateTime value) { return "'" + value.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'"; }
        //string ToSql(bool value) { return value ? "1" : "0"; }
        //string ToSql(string value) { return $"'{value}'"; }

        //public string InsertCommandText()
        //{
        //    string command = "INSERT INTO Actions(ActionTypeID, SystemActionTypeID, Name, TimeSpent, DateStarted, IsVisibleOnPortal, IsKnowledgeBase, ImportID, DateCreated, DateModified, CreatorID, ModifierID, TicketID, Description, SalesForceID, DateModifiedBySalesForceSync, Pinned, ImportFileID)" +
        //    $"VALUES({ToSql(ActionTypeID)}, '{(int)SystemActionTypeID}', {ToSql(Name)}, {ToSql(TimeSpent)}, {ToSql(DateStarted)}, {ToSql(IsVisibleOnPortal)}, {ToSql(IsKnowledgeBase)}, {ToSql(ImportID)}, {ToSql(DateCreated)}, {ToSql(DateModified)}, {ToSql(CreatorID)}, {ToSql(ModifierID)}, {ToSql(TicketID)}, {ToSql(Description)}, {ToSql(SalesForceID)}, {ToSql(DateModifiedBySalesForceSync)}, {ToSql(Pinned)}, {ToSql(ImportFileID)})" +
        //    "SELECT SCOPE_IDENTITY()";
        //    return command;
        //}
    }

}