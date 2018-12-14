using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using TeamSupport.EFData;
using TeamSupport.EFData.Models;
using TeamSupport.JIRA;

namespace TeamSupport.UnitTest
{

    /// <summary>
    /// Only mock dependencies!
    /// </summary>
    [TestFixture]
    [Category("Jira Service Unit")]
    public class JiraServiceUnitTests
    {
        Mock<IJiraTicketsService> jiraTicketsService;
        Mock<TicketLinkToJira> mockedTicket;

        [SetUp]
        public void Init()
        {
            jiraTicketsService = new Mock<IJiraTicketsService>();
            mockedTicket = new Mock<TicketLinkToJira>();
        }

        #region Get Tests 
        [Test]
        public void JiraTicketsService_ShouldGetAllTickets()
        {
            jiraTicketsService.Setup(a => a.GetTicketsToPushAsIssues()).Returns(It.IsAny<List<TicketLinkToJira>>());
        }

        [Test]
        public void JiraTicketsService_ShouldGetAllTicksAsynchronously()
        {
            Assert.IsNotNull(jiraTicketsService.Object.GetTicketsToPushAsIssuesAsync().Result);
        }

        [Test]
        public void JiraTicketsService_ShouldGetAllTicksAsynchronously_AndPerformLambdaOperationOnCollection()
        {
            Assert.IsNotNull(jiraTicketsService.Object.GetTicketsToPushAsIssuesAsync().Result.Where(a => a.SyncWithJira == true));
        }

        [Test]
        public void JiraTicketsService_ShouldGetSyncedAllTicksAsynchronously()
        {
            Assert.IsNotNull(jiraTicketsService.Object.GetTicketsToPushAsIssuesAsyncByExpression());
        }

        [Test]
        public void JiraTicketService_ShouldGetSingleRecordById_WithValidId()
        {
            //Arrange
            var context = jiraTicketsService.Object.GetTicketsToPushAsIssues().ToList();
            var singleTicket = context.FirstOrDefault();

            //Act
            var result = jiraTicketsService.Object.GetTicketsToPushAsIssuesById(singleTicket.id);

            //Assert
            Assert.AreEqual(singleTicket.id, result.id);
        }

        [Test]
        public void JiraTicketService_GetSingleJiraTicketAsyncShouldGetSingleRecordById_WithValidId()
        {
            //Arrange
            var context = jiraTicketsService.Object.GetTicketsToPushAsIssues().ToList();
            var singleTicket = context.FirstOrDefault();

            //Act
            var result = jiraTicketsService.Object.GetSingleJiraTicketAsync(singleTicket.id).Result;

            //Assert
            Assert.AreEqual(singleTicket.id, result.id);
        }

        [Test]
        public void JiraTicketService_ShouldReturnEmptyRecord_WhenQueryingForNonExistentRecord()
        {
            //Arrange
            var testId = -999;
            var expected = 0;
            var result = jiraTicketsService.Object.GetTicketsToPushAsIssuesById(testId);

            //Assert
            Assert.AreEqual(expected, result.id);

        }
        #endregion

        #region Save Tests 
        [Test]
        public void JiraTicketsService_SaveShouldSaveSuccessfully_WithValidEntity()
        {
            //Arrange
            var jiraTicket = new TicketLinkToJira() { id = -1 };

            //Assert
            Assert.DoesNotThrow(() => jiraTicketsService.Object.SaveJiraTickets(jiraTicket));
        }



        [Test]
        public void JiraTicketsService_SaveAsyncShouldSaveSuccessfully_WithValidEntityAsync()
        {
            //Arrange
            var jiraTickets = new TicketLinkToJira() { id = -1 };

            //Assert
            Assert.DoesNotThrow(() => jiraTicketsService.Object.SaveJiraTicketsAsync(jiraTickets));
        }

        [Test]
        public void JiraTicketsService_SaveShouldThrowArgumentNullException_WithInvalidEntity()
        {
            jiraTicketsService.Setup(a => a.SaveJiraTickets(null)).Throws<ArgumentNullException>();
        }

        [Test]
        public void JiraTicketsService_SaveAsyncShouldThrowArgumentNullException_WithInvalidEntity()
        {
            
            try
            {
                //Act
                jiraTicketsService.Object.SaveJiraTicketsAsync(null);
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
            //Act
            jiraTicketsService.Setup(a => a.DeleteJiraTicket(It.IsAny<TicketLinkToJira>()));            
            jiraTicketsService.Object.DeleteJiraTicket(It.IsAny<TicketLinkToJira>());
            //Assert
            jiraTicketsService.Verify(a => a.DeleteJiraTicket(It.IsAny<TicketLinkToJira>()), Times.AtLeastOnce());
        }

        [Test]
        public void JiraTicketsService_DeleteShouldWork_WithValidEntity()
        {
            //Arrange
            var value = new List<TicketLinkToJira>();
            jiraTicketsService.Setup(a => a.GetTicketsToPushAsIssues()).Returns(value);
          
            //Assert
            Assert.DoesNotThrow(() => jiraTicketsService.Object.DeleteJiraTicket(value.FirstOrDefault()));
        }

        [Test]
        public void JiraTicketsService_DeleteAsyncShouldThrowInvalidOperationException_WitInvalidEntityObjectState()
        {
            jiraTicketsService.Setup(a => a.DeleteJiraTicketAsync(It.IsAny<TicketLinkToJira>()));
            Assert.Throws<NullReferenceException>(()=>jiraTicketsService.Object.DeleteJiraTicketAsync(null));
        }

        #endregion

        #region Edit Tests
        [Test]
        public void JiraTicketService_EditShouldThrowArgumentNullException_WithInvalidEntity()
        {
            //Arrange
            var singleTicket = new TicketLinkToJira();
            singleTicket.JiraStatus = "Test edit status";

          //  jiraTicketsService.Setup(a => a.UpdateTicket(singleTicket)).Returns(singleTicket);
            ////Assert
            Assert.Throws<ArgumentNullException>(() => jiraTicketsService.Object.UpdateTicket(singleTicket));
        }

        [Test]
        public void JiraTicketService_EditAsyncShouldThrowArgumentNullException_WithInvalidEntity()
        {
         
            try
            {
                //Act
                jiraTicketsService.Object.UpdateTicketAsync(null);
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
