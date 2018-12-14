using Moq;
using NUnit.Framework;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using TeamSupport.EFData;
using TeamSupport.EFData.Models;
using TeamSupport.JIRA;

namespace TeamSupport.UnitTest
{
    [TestFixture]
    [Category("Jira Ticket Service Integration")]
    public class JiraTicketServiceIntegrationTests
    {
        IJiraTicketsService jiraTicketsService;

        [SetUp]
        public void Init()
        {
            jiraTicketsService = new JiraTicketsService(new JiraRepository(new GenericRepository<TicketLinkToJira>()));
        }

        #region Get Tests (Integration)
        [Test]
        public void JiraTicketsService_ShouldGetAllTickets()
        {
            Assert.IsNotNull(jiraTicketsService.GetTicketsToPushAsIssues().ToList());
        }

        [Test]
        public void JiraTicketsService_ShouldGetAllTicksAsynchronously()
        {
            Assert.IsNotNull(jiraTicketsService.GetTicketsToPushAsIssuesAsync().Result);
        }

        [Test]
        public void JiraTicketsService_ShouldGetAllTicksAsynchronously_AndPerformLambdaOperationOnCollection()
        {
            Assert.IsNotNull(jiraTicketsService.GetTicketsToPushAsIssuesAsync().Result.Where(a => a.SyncWithJira == true));
        }

        [Test]
        public void JiraTicketsService_ShouldGetSyncedAllTicksAsynchronously()
        {
            Assert.IsNotNull(jiraTicketsService.GetTicketsToPushAsIssuesAsyncByExpression());
        }

        [Test]
        public void JiraTicketService_ShouldGetSingleRecordById_WithValidId()
        {
            //Arrange
            var context = jiraTicketsService.GetTicketsToPushAsIssues().ToList();
            var singleTicket = context.FirstOrDefault();

            //Act
            var result = jiraTicketsService.GetTicketsToPushAsIssuesById(singleTicket.id);

            //Assert
            Assert.AreEqual(singleTicket.id, result.id);
        }

        [Test]
        public void JiraTicketService_GetSingleJiraTicketAsyncShouldGetSingleRecordById_WithValidId()
        {
            //Arrange
            var context = jiraTicketsService.GetTicketsToPushAsIssues().ToList();
            var singleTicket = context.FirstOrDefault();

            //Act
            var result = jiraTicketsService.GetSingleJiraTicketAsync(singleTicket.id).Result;

            //Assert
            Assert.AreEqual(singleTicket.id, result.id);
        }

        [Test]
        public void JiraTicketService_ShouldReturnEmptyRecord_WhenQueryingForNonExistentRecord()
        {
            //Arrange
            var testId = -999;
            var expected = 0;
            var result = jiraTicketsService.GetTicketsToPushAsIssuesById(testId);

            //Assert
            Assert.AreEqual(expected, result.id);

        }
        #endregion

        #region Save Tests (Unit)
        [Test]
        public void JiraTicketsService_SaveShouldSaveSuccessfully_WithValidEntity()
        {
            //Arrange
            var jiraTicket = new TicketLinkToJira() { id = -1 };

            //Assert
            Assert.DoesNotThrow(() => jiraTicketsService.SaveJiraTickets(jiraTicket));
        }
        

        [Test]
        public void JiraTicketsService_SaveAsyncShouldSaveSuccessfully_WithValidEntityAsync()
        {
            //Arrange
            var jiraTickets = new TicketLinkToJira() { id = -1 };

            //Assert
            Assert.DoesNotThrow(() => jiraTicketsService.SaveJiraTicketsAsync(jiraTickets));
        }

        [Test]
        public void JiraTicketsService_SaveShouldThrowArgumentNullException_WithInvalidEntity()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => jiraTicketsService.SaveJiraTickets(null));
        }

        [Test]
        public void JiraTicketsService_SaveAsyncShouldThrowArgumentNullException_WithInvalidEntity()
        {

            try
            {
                //Act
                jiraTicketsService.SaveJiraTicketsAsync(null);
            }
            catch (Exception ex)
            {
                var expected = "Value cannot be null.\r\nParameter name: entity";
                var result = ex.InnerException.Message.ToString();
                //Assert
                Assert.AreEqual(expected, result);
            }
        }
        #endregion

        #region Delete Tests
        [Test]
        public void JiraTicketsService_DeleteShouldThrowDbUpdateConcurrencyException_WithInvalidEntityObjectState()
        {
            //Arrange
            var jiraTickets = new TicketLinkToJira() { id = -1 };

            //Assert
            Assert.Throws<DbUpdateConcurrencyException>(() => jiraTicketsService.DeleteJiraTicket(jiraTickets));
        }

        [Test]
        public void JiraTicketsService_DeleteShouldWork_WithValidEntity()
        {
            //Arrange
            var context = jiraTicketsService.GetTicketsToPushAsIssues().ToList();
            var singleTicket = context.FirstOrDefault();

            //Assert
            Assert.DoesNotThrow(() => jiraTicketsService.DeleteJiraTicket(singleTicket));
        }

        [Test]
        public void JiraTicketsService_DeleteAsyncShouldThrowInvalidOperationException_WitInvalidEntityObjectState()
        {
            //Arrange
            var context = jiraTicketsService.GetTicketsToPushAsIssues().ToList();
            var singleTicket = context.FirstOrDefault();
            var testId = singleTicket.id;
            var resultTicket = jiraTicketsService.GetTicketsToPushAsIssuesById(testId);

            try
            {
                //Act
                jiraTicketsService.DeleteJiraTicketAsync(resultTicket);
            }
            catch (Exception ex)
            {
                var expected = "Value cannot be null.\r\nParameter name: predicate";
                var result = ex.InnerException.Message.ToString();
                //Assert
                Assert.AreEqual(expected, result);
            }
        }

        #endregion

        #region Edit Tests
        [Test]
        public void JiraTicketService_EditShouldEditRecordFromDatabase_WithValidEntity()
        {
            //Arrange
            var context = jiraTicketsService.GetTicketsToPushAsIssues().ToList();
            var singleTicket = context.FirstOrDefault();
            singleTicket.JiraStatus = "Test edit status";

            //Assert
            Assert.DoesNotThrow(() => jiraTicketsService.UpdateTicket(singleTicket));
        }

        [Test]
        public void JiraTicketService_EditShouldThrowArgumentNullException_WithInvalidEntity()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => jiraTicketsService.UpdateTicket(null));
        }

        [Test]
        public void JiraTicketService_EditAsyncShouldEditRecordFromDatabase_WithValidEntity()
        {
            //Arrange
            var context = jiraTicketsService.GetTicketsToPushAsIssues().ToList();
            var singleTicket = context.FirstOrDefault();
            singleTicket.JiraStatus = "Test edit status1";

            //Assert
            Assert.DoesNotThrow(() => jiraTicketsService.UpdateTicketAsync(singleTicket));
        }

        [Test]
        public void JiraTicketService_EditAsyncShouldThrowArgumentNullException_WithInvalidEntity()
        {

            try
            {
                //Act
                jiraTicketsService.UpdateTicketAsync(null);
            }
            catch (Exception ex)
            {
                var expected = "Value cannot be null.\r\nParameter name: entity";
                var result = ex.InnerException.Message.ToString();
                //Assert
                Assert.AreEqual(expected, result);
            }
        }
        #endregion


    }
}
