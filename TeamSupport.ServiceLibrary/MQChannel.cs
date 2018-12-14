using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WatsonToneAnalyzer
{
    /// <summary>Wrapper for model of communication channel</summary>
    class MQChannel : IDisposable
    {
        static string routingQueue = "watson_task";
        static bool durable = false;

        IModel _channel;
        EventingBasicConsumer _consumer;

        /// <summary>Expose the consumer.Received event handler</summary>
        public EventingBasicConsumer Consumer
        {
            get { return _consumer; }
        }

        /// <summary>Constructor declaring the message queue and a consumer</summary>
        /// <param name="channel"></param>
        public MQChannel(IModel channel)
        {
            _channel = channel;
            _channel.QueueDeclare(queue: routingQueue,
                                 durable: durable,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            _consumer = new EventingBasicConsumer(_channel);
            _channel.BasicConsume(queue: routingQueue, autoAck: false, consumer: _consumer);
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
        }
    }
}
*/