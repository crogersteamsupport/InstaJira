using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TeamSupport.JIRA.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class JiraTest
    {
        public JiraTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void OneTest()
        {  
            JiraClient client = new JiraClient("https://teamsupport.atlassian.net", "michael", "Welcome1");
            IssueRef issueRef = new IssueRef();
            issueRef.id = "CON-42";
            issueRef.key = "CON-42";
            var issue = client.LoadIssue(issueRef);
            var remotlink = client.GetRemoteLinks(issueRef);
            Assert.AreEqual("CON-42", issue.key);           
        }

        [TestMethod]
        public void CreateNewIssue()
        {
            JiraClient client = new JiraClient("https://teamsupport.atlassian.net", "michael", "Welcome1");
            IssueFields fields = new IssueFields();
            fields.description = "This is a test description";
            Comment comment = new Comment();
            comment.body = "This is a test comment";
            fields.comments.Add(comment);
            fields.summary = "This is a test summary";
            var issue = client.CreateIssue("CON", "New Feature", fields);

            Assert.AreEqual("This is a test description", issue.fields.description);
        }
    }

}
