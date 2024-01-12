using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoApplicationServer.Logger
{
    public interface ILogger
    {
        void Log(LogMessage message);
        void SetNextLogger(ILogger nextLogger);
    }
}
