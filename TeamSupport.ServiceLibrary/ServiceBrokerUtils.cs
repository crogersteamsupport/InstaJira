using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading;
using System.Data;
using System.Transactions;


namespace TeamSupport.ServiceLibrary
{
    public class ServiceBrokerUtils
    {

        internal static List<byte[]> GetMessage(string queueName, SqlConnection con, TimeSpan timeout, int maxMessages = 1)
        {
			List<byte[]> messages = new List<byte[]>();

			using (SqlDataReader r = GetMessageBatch(queueName, con, timeout, maxMessages))
            {
                if (r == null || !r.HasRows)
                    return null;

				while (r.Read())
				{
					Guid conversation_handle = r.GetGuid(r.GetOrdinal("conversation_handle"));
					string messageType = r.GetString(r.GetOrdinal("message_type_name"));

					if (messageType == "http://schemas.microsoft.com/SQL/ServiceBroker/EndDialog")
					{
						EndConversation(conversation_handle, con);
						return null;
					}

					var body = r.GetSqlBinary(r.GetOrdinal("message_body"));
					messages.Add((byte[])body);
				}
            }

			return messages;
		}

        public static List<string> ReadMessage(string connectionString, string queueName, int maxMessages = 1)
        {
            List<string> messages = new List<string>();
            TransactionOptions to = new TransactionOptions();
            to.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            to.Timeout = TimeSpan.MaxValue;

            CommittableTransaction tran = new CommittableTransaction(to);

            try
            {

                using (var con = new SqlConnection(connectionString))
                {
                    con.Open();
                    con.EnlistTransaction(tran);
					List<byte[]> messagesBytes = ServiceBrokerUtils.GetMessage(queueName, con, TimeSpan.FromSeconds(10), maxMessages);

					if (messagesBytes == null) //no messages available
                    {
                        tran.Commit();
                        con.Close();
                        return null;
                    }
					
					foreach(byte[] messageBytes in messagesBytes)
					{
						string messageText = messageBytes != null ? Encoding.Unicode.GetString(messageBytes) : null;
						messages.Add(messageText);
					}

                    tran.Commit(); // the message processing succeeded or the FailedMessageProcessor ran so commit the RECEIVE
                    con.Close();
                }
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Trace.Write("Error processing message from " + queueName + ": " + ex.Message);
                tran.Rollback();
                tran.Dispose();
                Thread.Sleep(1000);
            }
            ///catch any other non-fatal exceptions that should not stop the listener loop.
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Unexpected Exception in Thread Proc for " + queueName + ".  Thread Proc is exiting: " + ex.Message);
                tran.Rollback();
                tran.Dispose();
                return null;
            }

            return messages;
        }

        internal static void EndConversation(Guid conversationHandle, SqlConnection con)
        {
            try
            {
                string SQL = "END CONVERSATION @ConversationHandle;";

                using (SqlCommand cmd = new SqlCommand(SQL, con))
                {
                    SqlParameter pConversation = cmd.Parameters.Add("@ConversationHandle", SqlDbType.UniqueIdentifier);
                    pConversation.Value = conversationHandle;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw;
            }
        }
        
        /// <summary>
        /// This is the method that actually receives Service Broker messages.
        /// </summary>
        /// <param name="timeout">Maximum time to wait for a message.  This is passed to the RECEIVE command, not used as a SqlCommand.CommandTimeout</param>
        /// <returns></returns>
        static SqlDataReader GetMessageBatch(string queueName, SqlConnection con, TimeSpan timeout, int maxMessages)
        {
            string SQL = string.Format(@"
            waitfor( 
                RECEIVE top (@count) conversation_handle,service_name,message_type_name,message_body,message_sequence_number 
                FROM [{0}] 
                    ), timeout @timeout", queueName);
            SqlCommand cmd = new SqlCommand(SQL, con);

            SqlParameter pCount = cmd.Parameters.Add("@count", SqlDbType.Int);
            pCount.Value = maxMessages;

            SqlParameter pTimeout = cmd.Parameters.Add("@timeout", SqlDbType.Int);

            if (timeout == TimeSpan.MaxValue)
            {
                pTimeout.Value = -1;
            }
            else
            {
                pTimeout.Value = (int)timeout.TotalMilliseconds;
            }

            cmd.CommandTimeout = 0; //honor the RECEIVE timeout, whatever it is.


            return cmd.ExecuteReader();
        }


    }
}
