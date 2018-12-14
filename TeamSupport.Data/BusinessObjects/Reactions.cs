using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{

    public partial class Reactions
    {

        public void GetReactions (int ticketID, int actionID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT COUNT(*) AS tally FROM Reactions WHERE ReactionValue = 1 AND ReferenceID = @ReferenceID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ReferenceID", actionID);
                // Fill(command);
            }
        }

        public void ListReactions (int ticketID, int actionID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText  = "SELECT Reactions.*, Users.FirstName, Users.LastName FROM Reactions ";
                command.CommandText += "INNER JOIN Users ON Users.UserID = Reactions.UserID ";
                command.CommandText += "WHERE Reactions.ReferenceID = @ReferenceID AND Reactions.ReactionValue = 1";
                command.CommandType  = CommandType.Text;
                command.Parameters.AddWithValue("@ReferenceID", actionID);
                Fill(command);
            }
        }

        public void UpdateReactions (int UserID, int ReceiverID, int ReferenceID, int ReactionValue)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText  = "BEGIN TRAN ";
                command.CommandText += "IF EXISTS (SELECT * FROM Reactions WHERE ReferenceID = @ AND UserID = @ AND ReceiverID = @) ";
                command.CommandText += "BEGIN UPDATE Reactions SET ReactionValue = @ReactionValue, DateTimeChanged = @DateTimeChanged WHERE UserID = @UserID AND ReceiverID = @ReceiverID AND ReferenceID = @ReferenceID END ";
                command.CommandText += "ELSE INSERT Reactions (UserID,ReceiverID,ReferenceID,ReactionValue,DateTimeCreated) VALUES (@UserID,@ReceiverID,@ReferenceID,@ReactionValue,@DateTimeCreated) END ";
                command.CommandText += "COMMIT TRAN";
                command.CommandType  = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", UserID);
                command.Parameters.AddWithValue("@ReceiverID", ReceiverID);
                command.Parameters.AddWithValue("@ReferenceID", ReferenceID);
                command.Parameters.AddWithValue("@ReactionValue", ReactionValue);
                Fill(command);
            }
        }


    }

}