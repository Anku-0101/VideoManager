using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoApplicationServer.Logger
{
    public abstract class BaseLogger : ILogger
    {
        private ILogger nextLogger;

        public void SetNextLogger(ILogger nextLogger)
        {
            this.nextLogger = nextLogger;
        }

        public void Log(LogMessage message)
        {
            if (IsLoggable(message))
            {
                WriteLog(message);
            }

            // Pass the message to the next logger in the chain
            if (nextLogger != null)
            {
                nextLogger.Log(message);
            }
        }

        // Each logger subclass should implement this method
        protected abstract void WriteLog(LogMessage message);

        // Each logger subclass should implement this method to determine if it should log the message
        protected abstract bool IsLoggable(LogMessage message);
    }
}