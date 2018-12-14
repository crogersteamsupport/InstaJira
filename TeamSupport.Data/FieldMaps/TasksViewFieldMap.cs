using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{

    public partial class TasksView
    {
        protected override void BuildFieldMap()
        {
            _fieldMap = new FieldMap();
            _fieldMap.AddMap("TaskID", "TaskID", false, false, false);
            _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
            _fieldMap.AddMap("Name", "Name", false, false, false);
            _fieldMap.AddMap("Description", "Description", false, false, false);
            _fieldMap.AddMap("DueDate", "DueDate", false, false, false);
            _fieldMap.AddMap("UserID", "UserID", false, false, false);
            _fieldMap.AddMap("IsComplete", "IsComplete", false, false, false);
            _fieldMap.AddMap("DateCompleted", "DateCompleted", false, false, false);
            _fieldMap.AddMap("CompletionComment", "CompletionComment", false, false, false);
            _fieldMap.AddMap("ParentID", "ParentID", false, false, false);
            _fieldMap.AddMap("IsDismissed", "IsDismissed", false, false, false);
            _fieldMap.AddMap("HasEmailSent", "HasEmailSent", false, false, false);
            _fieldMap.AddMap("ReminderDueDate", "ReminderDueDate", false, false, false);
            _fieldMap.AddMap("TaskParentName", "TaskParentName", false, false, false);
            _fieldMap.AddMap("UserName", "UserName", false, false, false);
            _fieldMap.AddMap("Creator", "Creator", false, false, false);
            _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
            _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
            _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
            _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
            _fieldMap.AddMap("NeedsIndexing", "NeedsIndexing", false, false, false);

        }
    }

}
