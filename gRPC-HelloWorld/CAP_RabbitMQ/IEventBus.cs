using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAP_RabbitMQ
{
    public interface IEventBus
    {
        void Publish<TMessage>(TMessage message, string key = null, string callbackName = null);
        Task PublishAsync<TMessage>(TMessage message, string key = null, string callbackName = null, CancellationToken cancellationToken = default);
    }
}
