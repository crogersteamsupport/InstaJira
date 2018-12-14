using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class TaskEmailPosts
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("TaskEmailPostID", "TaskEmailPostID", false, false, false);
      _fieldMap.AddMap("TaskEmailPostType", "TaskEmailPostType", false, false, false);
      _fieldMap.AddMap("HoldTime", "HoldTime", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("TaskID", "TaskID", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("LockProcessID", "LockProcessID", false, false, false);
      _fieldMap.AddMap("OldUserID", "OldUserID", false, false, false);
            
    }
  }
  
}
