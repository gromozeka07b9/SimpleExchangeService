using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExchangeIntegrationCommon.DAL
{
    public interface ILogger
    {
        void Log(string message, LoggerLevel level, object parameters);
        void Log(string message, LoggerLevel level);
    }
}
