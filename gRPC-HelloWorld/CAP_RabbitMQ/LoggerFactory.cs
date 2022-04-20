using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAP_RabbitMQ
{
    public class CAPLoggerFactory
    {
        public static ILoggerFactory capLoggerFactory = new LoggerFactory(new List<ILoggerProvider>() { new NLogLoggerProvider() });
    }
}
