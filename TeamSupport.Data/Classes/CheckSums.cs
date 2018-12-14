using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Security;


namespace TeamSupport.Data
{
  public class CheckSums
  {

    public static int GetCheckSum(LoginUser loginUser, ReferenceType refType)
    {
      string sql = "SELECT CHECKSUM_AGG(CHECKSUM(*)) FROM {0} WHERE OrganizationID = " + loginUser.OrganizationID.ToString();
      switch (refType)
      {
        case ReferenceType.ActionTypes:
          sql = string.Format(sql, "ActionTypes");
          break;
        case ReferenceType.CustomFields:
          sql = string.Format(sql, "CustomFields");
          break;
        case ReferenceType.Groups:
          sql = string.Format(sql, "Groups");
          break;
        case ReferenceType.PhoneTypes:
          sql = string.Format(sql, "PhoneTypes");
          break;
        case ReferenceType.Products:
          sql = string.Format(sql, "Products");
          break;
        case ReferenceType.ProductVersions:
          sql = "SELECT CHECKSUM_AGG(CHECKSUM(p.Name, pv.VersionNumber)) FROM ProductVersions pv LEFT JOIN Products p ON p.ProductID = pv.ProductID WHERE p.OrganizationID = " + loginUser.OrganizationID.ToString();
          break;
        case ReferenceType.ProductVersionStatuses:
          sql = string.Format(sql, "ProductVersionStatuses");
          break;
        case ReferenceType.ProductFamilies:
          sql = string.Format(sql, "ProductFamilies");
          break;
        case ReferenceType.TicketSeverities:
          sql = string.Format(sql, "TicketSeverities");
          break;
        case ReferenceType.TicketStatuses:
          sql = string.Format(sql, "TicketStatuses");
          break;
        case ReferenceType.TicketTypes:
          sql = string.Format(sql, "TicketTypes");
          break;
        case ReferenceType.Users:
          //sql = "SELECT CHECKSUM_AGG(CHECKSUM(FirstName, LastName, Email, IsActive, MarkDeleted, TimeZoneID, CultureName, IsSystemAdmin, IsChatUser, InOffice, InOfficeComment)) FROM Users WHERE OrganizationID = " + loginUser.OrganizationID.ToString();
          sql = "SELECT CHECKSUM_AGG(CHECKSUM(FirstName, LastName, InOfficeComment)) FROM Users WHERE OrganizationID = " + loginUser.OrganizationID.ToString();
          break;
        case ReferenceType.TicketNextStatuses:
          sql = "SELECT CHECKSUM_AGG(CHECKSUM(*)) FROM TicketNextStatuses tns LEFT JOIN TicketStatuses ts ON ts.TicketStatusID = tns.CurrentStatusID WHERE ts.OrganizationID = " + loginUser.OrganizationID.ToString();
          break;
        case ReferenceType.ForumCategories:
          sql = string.Format(sql, "ForumCategories");
          break;
        default:
          return 0;
      }

      SqlCommand command = new SqlCommand();
      command.CommandText = sql;
      command.CommandType = CommandType.Text;
      object o = SqlExecutor.ExecuteScalar(loginUser, command);
      if (o == null || o == DBNull.Value) return 0;
      return (int)o;
    }

    
  }

}
