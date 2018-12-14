using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class AttachmentDownloads
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("AttachmentDownloadID", "AttachmentDownloadID", false, false, false);
      _fieldMap.AddMap("AttachmentOrganizationID", "AttachmentOrganizationID", false, false, false);
      _fieldMap.AddMap("AttachmentID", "AttachmentID", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("DateDownloaded", "DateDownloaded", false, false, false);
            
    }
  }
  
}
