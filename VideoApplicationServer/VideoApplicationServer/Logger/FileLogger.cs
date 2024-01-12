using System;
using System.IO;

namespace VideoApplicationServer.Logger
{
    public class FileLogger : BaseLogger
    {
        private string logFilePath = Utilities.Constants.LOGFILE;

        protected override void WriteLog(LogMessage message)
        {
            // Append the log message to the log file
            using (StreamWriter writer = File.AppendText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logFilePath)))
            {
                writer.WriteLine($"{DateTime.Now} [{message.LogLevel}] - {message.Message}");
            }
        }

        protected override bool IsLoggable(LogMessage message)
        {
            return message.LogLevel >= LogLevel.Warning; // Log warnings and errors to the file
        }
    }
}