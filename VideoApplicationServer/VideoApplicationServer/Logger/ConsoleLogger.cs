﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoApplicationServer.Logger
{
    public class ConsoleLogger : BaseLogger
    {
        protected override void WriteLog(LogMessage message)
        {
            Console.WriteLine($"Console Log: {message.Message}");
        }

        protected override bool IsLoggable(LogMessage message)
        {
            return true; // Log all messages to the console
        }
    }
}