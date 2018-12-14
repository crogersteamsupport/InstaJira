using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Data.Linq;
using TeamSupport.Data.BusinessObjects;

namespace TeamSupport.Data {

    public partial class WatsonScores {

        public static string PullSummary (LoginUser loginUser, int ticketID) {
            try {
                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString)) {
                    using (SqlCommand command = new SqlCommand()) {
                        command.Connection   = connection;
                        command.CommandType  = CommandType.Text;
                        command.CommandText  = "SELECT ActionSentimentScores.SentimentID, COUNT(ActionSentimentScores.ActionSentimentScoreID) AS count, AVG(ActionSentimentScores.SentimentScore) AS average FROM dbo.ActionSentimentScores ";
                        command.CommandText += "INNER JOIN dbo.ActionSentiments ON ActionSentiments.ActionSentimentID = ActionSentimentScores.ActionSentimentID ";
                        command.CommandText += "WHERE ActionSentiments.TicketID = @TicketID AND ActionSentiments.IsAgent = 0 AND ActionSentimentScores.SentimentID > 0 ";
                        command.CommandText += "GROUP BY ActionSentimentScores.SentimentID ";
                        command.CommandText += "ORDER BY count DESC ";
                        command.CommandText += "FOR JSON PATH, ROOT('summary') ";
                        command.Parameters.AddWithValue("@TicketID", ticketID);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        return (reader.HasRows && reader.Read()) ? reader.GetValue(0).ToString() : "nothing";
                    }
                }
            }
            catch (SqlException e) {
                return "fault";
            }
            catch (Exception e) {
                return "fault";
            }
        }

        public static string PullTicket (LoginUser loginUser, int ticketID) {
            try {
                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString)) {
                    connection.Open();
                    DataContext db = new DataContext(connection);
                    Table<TicketSentiment> ticketScoresTable = db.GetTable<TicketSentiment>();
                    TicketSentiment ticketSentimentScore = (from u in ticketScoresTable where u.TicketID == ticketID && !u.IsAgent select u).FirstOrDefault();
                    return (ticketSentimentScore == null) ? "negative" : JsonConvert.SerializeObject(ticketSentimentScore);
                }
            }
            catch (SqlException e) {
                return "negative";
            }
            catch (Exception e) {
                return "negative";
            }
        }

        public static string PullAction (LoginUser loginUser, int ticketID, int actionID) {
            try {
                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString)) {
                    using (SqlCommand command = new SqlCommand()) {
                        command.Connection   = connection;
                        command.CommandType  = CommandType.Text;
                        command.CommandText  = "SELECT ActionSentiments.*, ActionSentimentScores.SentimentID, ActionSentimentScores.SentimentScore FROM dbo.ActionSentiments ";
                        command.CommandText += "INNER JOIN dbo.ActionSentimentScores ON ActionSentiments.ActionSentimentID = ActionSentimentScores.ActionSentimentID ";
                        command.CommandText += "WHERE ActionSentiments.TicketID = @TicketID AND ActionSentiments.ActionID = @ActionID AND ActionSentiments.IsAgent = 0";
                        command.CommandText += "FOR JSON PATH, ROOT('watson')";
                        command.Parameters.AddWithValue("@TicketID", ticketID);
                        command.Parameters.AddWithValue("@ActionID", actionID);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        return (reader.HasRows && reader.Read()) ? reader.GetValue(0).ToString() : "nothing";
                    }
                }
            } catch (SqlException e) {
                return "negative";
            } catch (Exception e) {
                return "negative";
            }
        }

    }
}
