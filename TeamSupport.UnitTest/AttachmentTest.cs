using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamSupport.IDTree;
using TeamSupport.Data;
using TeamSupport.DataAPI;
using TeamSupport.ModelAPI;
using System.Configuration;

namespace TeamSupport.UnitTest
{
    [TestClass]
    public class AttachmentTest
    {
        public const string _userScot = "4787299|1078|0|51dab274-5d73-4f56-8df9-da97d20cdc5b|1";
        public static string _connectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString;

        [TestMethod]
        public void UpdateActionCopyingAttachment()
        {
            //LoginUser loginUser = new LoginUser(AttachmentTest._connectionString, 4787299, 1078, (IDataCache)null);    // Scot, TeamSupport
            //public TimeLineItem UpdateActionCopyingAttachment(LoginUser loginUser, ActionProxy proxy, int insertedKBTicketID)
            //TSWebServices.TicketPageService.CopyInsertedKBAttachments(int actionID, int insertedKBTicketID);

            string userData = _userScot;
            AuthenticationModel authentication = AuthenticationModel.AuthenticationModelTest(userData, _connectionString);
            using (ConnectionContext connection = new ConnectionContext(authentication))
            {
                int actionID = 58290070;
                int insertedKBTicketID = 19770659;
                AttachmentAPI.CopyInsertedKBAttachments(connection, actionID, insertedKBTicketID);
            }

        }

        [TestMethod]
        public void ActionAttachments()
        {
            string userData = _userScot;
            AuthenticationModel authentication = AuthenticationModel.AuthenticationModelTest(userData, _connectionString);
            using (ConnectionContext connection = new ConnectionContext(authentication))
            {
                // user ticket
                TicketProxy ticketProxy = IDTreeTest.EmptyTicket(); // from front end
                TicketModel ticketModel = (TicketModel)Data_API.Create(connection.User, ticketProxy);  // dbo.Tickets

                // ticket action
                ActionProxy actionProxy = IDTreeTest.EmptyAction(); // from front end
                ActionModel actionModel = (ActionModel)Data_API.Create(ticketModel, actionProxy);  // dbo.Actions

                // action attachment
                ActionAttachmentProxy attachmentProxy = (ActionAttachmentProxy)IDTreeTest.CreateAttachment(connection.OrganizationID, 
                    AttachmentProxy.References.Actions, actionModel.ActionID, actionModel.AttachmentPath);
                AttachmentModel attachmentModel = (AttachmentModel)Data_API.Create(actionModel, attachmentProxy);

                // read back attachment
                AttachmentProxy read = Data_API.ReadRefTypeProxy<AttachmentProxy>(connection, attachmentProxy.AttachmentID);
                switch (read)
                {
                    case ActionAttachmentProxy output:  // Action attachment
                        Assert.AreEqual(attachmentProxy, output);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }
    }
}
