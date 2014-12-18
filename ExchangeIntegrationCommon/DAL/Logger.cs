using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExchangeIntegrationCommon.DAL
{
    public class Logger : ILogger
    {
        private static NLog.Logger _logger = null;

        public void Log(string message, LoggerLevel level)
        {
            Log(message, level, null);
        }

        public void Log(string message, LoggerLevel level, object parameters)
        {
            if(_logger == null)
            {
                _logger = LogManager.GetCurrentClassLogger();
            }
            switch(level)
            {
                case LoggerLevel.Trace:
                    {
                        _logger.Trace(message, parameters);
                    };break;
                case LoggerLevel.Debug:
                    {
                        _logger.Debug(message, parameters);
                    };break;
                case LoggerLevel.Info:
                    {
                        _logger.Info(message, parameters);
                    };break;
                case LoggerLevel.Warn:
                    {
                        _logger.Warn(message, parameters);
                    };break;
                case LoggerLevel.Error:
                    {
                        _logger.Error(message, parameters);
                    };break;
                case LoggerLevel.Fatal:
                    {
                        _logger.Fatal(message, parameters);
                    };break;
                default:
                    {

                    };break;
            }
        }
    }
}
