using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  
  public partial class TaskAssociationsViewItem : BaseItem
  {
    public TaskAssociationsViewItemProxy GetProxy()
    {
      TaskAssociationsViewItemProxy result = new TaskAssociationsViewItemProxy();
      result.Contact = this.Contact;
      result.Product = this.Product;
      result.Group = this.Group;
      result.Company = this.Company;
      result.User = this.User;
      result.TicketName = this.TicketName;
      result.TicketNumber = this.TicketNumber;
      result.RefType = this.RefType;
      result.RefID = this.RefID;
      result.TaskID = this.TaskID;

      if(result.RefType == (int)ReferenceType.CompanyActivity || result.RefType == (int)ReferenceType.ContactActivity)       
      {
            Notes notes = new Notes(BaseCollection.LoginUser);
            notes.LoadByNoteID(result.RefID);
            //check if note even exist
            if (notes.Count > 0)
            {
                var notesProxy = notes[0].GetProxy();
                result.Activity = notesProxy.Title;
                result.ActivityID = notesProxy.NoteID;
                result.ActivityRefID = notesProxy.RefID;
            }
            else
                return null;
      } 
       
      return result;
    }	
  }
}
