using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoApplicationServer.Logger
{
    public class LoggerChainBuilder
    {
        public static ILogger BuildLoggerChain()
        {
            var errorLogger = new ErrorLogger();
            var fileLogger = new FileLogger();
            var consoleLogger = new ConsoleLogger();

            errorLogger.SetNextLogger(fileLogger);
            fileLogger.SetNextLogger(consoleLogger);

            return errorLogger;
        }
    }
}