using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoApplicationServer.Logger
{
    public interface ILogger
    {
        LogLevel LogLevel { get; set; }
        ILogger NextLogger { get; set; }

        void Log(LogLevel level, string message);
    }
}
