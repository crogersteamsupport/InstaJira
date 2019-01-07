using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamSupport.IDTree;
using TeamSupport.UnitTest.Helpers;
using System.Linq;
using System.Diagnostics;

namespace TeamSupport.UnitTest
{
    class TicketText
    {
        public int TicketID { get; private set; }
        //public string Name;
        public string Description { get; private set; }
    }

    [TestClass]
    public class TicketSortTest
    {
        /*CREATE FUNCTION[dbo].[GetTicketActions]
        (
 @TicketID int
)
RETURNS NVARCHAR(max)
AS
BEGIN
    DECLARE @Actions nvarchar(max);

        SELECT
            @Actions = COALESCE(@Actions + ' ', ' ') + a.Description
    FROM
        Tickets t
        join Actions A on a.ticketid = t.TicketID
    WHERE t.TicketID = @TicketID


    RETURN ISNULL(@Actions, ' ');
        END


        GO*/

        [TestMethod]
        public void TicketWordCount()
        {
            string userData = AttachmentTest._userScot;
            AuthenticationModel authentication = AuthenticationModel.AuthenticationModelTest(userData, AttachmentTest._connectionString);
            using (ConnectionContext connection = new ConnectionContext(authentication))
            {
                string query = @"SELECT TOP 5000 t.TicketID
	                                ,dbo.GetTicketActions(t.TicketID) AS Description
                                FROM Tickets t
                                JOIN TicketStatuses ts on t.TicketStatusID = ts.TicketStatusID
                                WHERE t.OrganizationID = 1078 AND ts.IsClosed = 0 AND YEAR(t.DateCreated)=2018
                                GROUP BY TicketID
                                ORDER BY TicketID DESC";

                int i = 0;
                TicketCombination data = null;
                try
                {
                    data = new TicketCombination();
                    var enumerators = connection._db.ExecuteQuery<TicketText>(query);
                    foreach (var enumerator in enumerators)
                        data.Add(enumerator);
                    data.Reduce(100);
                    data.CountTriples();
                    data.Dump();
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                }
            }
        }


    }
}
