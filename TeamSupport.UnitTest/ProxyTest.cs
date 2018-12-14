using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamSupport.IDTree;
using TeamSupport.Data;
using TeamSupport.DataAPI;
using TeamSupport.ModelAPI;

namespace TeamSupport.UnitTest
{
    [TestClass]
    public class ProxyTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string userData = "4787299|1078|0|51dab274-5d73-4f56-8df9-da97d20cdc5b|1";
            string connectionString = "Application Name=App;Data Source=dev-sql.corp.teamsupport.com; Initial Catalog=TeamSupportNightly;Persist Security Info=True;User ID=Dev-Sql-WebApp;Password=TeamSupportDev;Connect Timeout=500;";
            AuthenticationModel authentication = AuthenticationModel.AuthenticationModelTest(userData, connectionString);

            using (ConnectionContext connection = new ConnectionContext(authentication))
            {
            }
        }
    }
}
    