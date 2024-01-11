using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoApplicationServer.Logger
{
    public class FileLogger : ILogger
    {
        public LogLevel LogLevel { get; set; }
        public ILogger NextLogger { get; set; }

        public FileLogger(LogLevel logLevel)
        {
            LogLevel = logLevel;
        }

        public void Log(LogLevel level, string message)
        {
            if (level <= LogLevel)
            {
                string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

                // Write the log message to a log file in the debug folder
                File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}\n");
            }

            NextLogger?.Log(level, message);
        }
    }
}
