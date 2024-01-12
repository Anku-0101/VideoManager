namespace VideoApplicationServer.Logger
{
    public interface ILogger
    {
        void Log(LogMessage message);
        void SetNextLogger(ILogger nextLogger);
    }
}
