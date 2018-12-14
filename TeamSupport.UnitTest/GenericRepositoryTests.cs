using NUnit.Framework;
using System;
using System.Linq;
using TeamSupport.EFData;
using TeamSupport.EFData.Models;

namespace TeamSupport.UnitTest
{
    [TestFixture, Category("Generic Repository Unit and Integration")]
    public class GenericRepositoryTests
    {
        IGenericRepository<JiraRepository> jiraRepo;

        [SetUp]
        public void InitRepo()
        {
            jiraRepo = new GenericRepository<JiraRepository>();
        }

        [Test]
        public void JiraContext_TicketLinkToJiraShouldUseAutoProperties()
        {
            var context = new JiraContext();
            context.TicketLinkToJira.Add(new TicketLinkToJira() { id = 1 });
            Assert.IsNotNull(context.TicketLinkToJira);
        }

        [Test]
        public void GenericRepository_CtroShouldBeInstantiated()
        {
            var sut = new GenericRepository<JiraRepository>();

            Assert.IsNotNull(sut);
        }

        [Test]
        public void GenericRepository_ShouldBeInstatiated_WhenCalled()
        {
            //Assert
            Assert.DoesNotThrow(() => new GenericRepository<JiraRepository>());
        }

        [Test]
        public void GenericRepository_AddShouldThrowArugmentNullException_WithNullObject()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(()=> jiraRepo.Add(null));
        }

        [Test]
        public void GenericRepository_AddAsyncShouldThrowArugmentNullException_WithNullObject()
        {
            try
            {
                var tmp = jiraRepo.AddAsync(null).Result;
            }
            catch (Exception ex)
            {
                var expected = "Value cannot be null.\r\nParameter name: entity";
                var result = ex.InnerException.Message.ToString();
                //Assert
                Assert.AreEqual(expected, result);
            }
        }

        [Test]
        public void GenericRepository_DeleteShouldThrowArgumentNullException_WithNullObject()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => jiraRepo.Delete(null));
        }

        [Test]
        public void GenericRepository_DeleteAsyncShouldThrowArgumentNullException_WithNullObject()
        {
            try
            {
                var tmp = jiraRepo.DeleteAsync(null).Result;
            }
            catch (Exception ex)
            {
                var expected = "Value cannot be null.\r\nParameter name: entity";
                var result = ex.InnerException.Message.ToString();
                //Assert
                Assert.AreEqual(expected, result);
            }
        }

        [Test]
        public void GenericRepository_EditShouldThrowArgumentNullException_WithNullObject()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => jiraRepo.Edit(null));
        }

        [Test]
        public void GenericRepository_EditAsyncShouldThrowArgumentNullException_WithNullObject()
        {
            try
            {
                var tmp = jiraRepo.EditAsync(null).Result;
            }
            catch (Exception ex)
            {
                var expected = "Value cannot be null.\r\nParameter name: entity";
                var result = ex.InnerException.Message.ToString();
                //Assert
                Assert.AreEqual(expected, result);
            }
        }

        //FindBy
        [Test]
        public void GenericRepository_FindByShouldThrowArgumentNullException_WithNullObject()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => jiraRepo.FindBy(null));
        }

        [Test]
        public void GenericRepository_FindByAsyncShouldThrowArgumentNullException_WithNullObject()
        {
            try
            {
                var tmp = jiraRepo.FindByAsync(null).Result;
            }
            catch (Exception ex)
            {
                var expected = "Value cannot be null.\r\nParameter name: predicate";
                var result = ex.InnerException.Message.ToString();
                //Assert
                Assert.AreEqual(expected, result);
            }
        }

        [Test]
        public void GenericReporistory_GetAllShouldThrowInvalidOperationException_WithInvalidEntity()
        {
            Assert.Throws<InvalidOperationException>(()=> jiraRepo.GetAll().ToList());
        }
   
        [Test]
        public void GenericRepository_GetAllAsyncShouldThrowArgumentNullException_WithNullObject()
        {
            try
            {
                var tmp = jiraRepo.GetAllAsync(null).Result;
            }
            catch (Exception ex)
            {
                var expected = "Value cannot be null.\r\nParameter name: predicate";
                var result = ex.InnerException.Message.ToString();
                //Assert
                Assert.AreEqual(expected, result);
            }
        }
        
    }
}
