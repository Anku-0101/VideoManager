﻿using System;

namespace VideoApplicationServer.Logger
{
    public class ErrorLogger : BaseLogger
    {
        protected override void WriteLog(LogMessage message)
        {
            Console.WriteLine($"Error Log: {message.Message}");
        }

        protected override bool IsLoggable(LogMessage message)
        {
            return message.LogLevel == LogLevel.Error;
        }
    }
}