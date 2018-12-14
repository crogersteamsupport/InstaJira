using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WatsonToneAnalyzer
{
    /// <summary>Connection to RabbitMQ message queue</summary>
    class MQConnection : IDisposable
    {
        static ConnectionFactory _factory;

        IConnection _connection;
        MQChannel _channel;

        /// <summary>static constructor for ConnectionFactory</summary>
        static MQConnection()
        {
            // configuration?
            _factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = ConnectionFactory.DefaultUser,
                Password = ConnectionFactory.DefaultPass,
                VirtualHost = ConnectionFactory.DefaultVHost,
                Port = AmqpTcpEndpoint.UseDefaultPort
            };
        }

        /// <summary>Constructor</summary>
        public MQConnection()
        {
            _connection = _factory.CreateConnection();
        }

        /// <summary>Create a channel on the connection</summary>
        /// <param name="handler">handler for receiving messages</param>
        public void CreateChannel(EventHandler<BasicDeliverEventArgs> handler)
        {
            _channel = new MQChannel(_connection.CreateModel());
            _channel.Consumer.Received += handler;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_channel != null)
                _channel.Dispose();

            if (_connection != null)
                _connection.Dispose();
        }
    }
}
*/