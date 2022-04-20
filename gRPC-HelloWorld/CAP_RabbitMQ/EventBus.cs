using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAP_RabbitMQ
{
    public class EventBus : IEventBus
    {
        private ICapPublisher _publisher;

        public EventBus(ICapPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Publish<TMessage>(TMessage message, string key = null, string callbackName = null)
        {
            var msgName = key ?? message.GetType().FullName;
            try
            {
                //logger
                _publisher.Publish(msgName, message, callbackName);
                
            }
            catch (Exception ex)
            {
                //logger
                throw ex;
            }
        }

        public async Task PublishAsync<TMessage>(TMessage message, string key = null, string callbackName = null, CancellationToken cancellationToken = default)
        {
            var msgName = key ?? message.GetType().FullName;
            try
            {
                //logger
                await _publisher.PublishAsync(msgName, message, callbackName, cancellationToken);

            }
            catch (Exception ex)
            {
                //logger
                throw ex;
            }
        }
    }
}
