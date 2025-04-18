using BepInEx.Logging;
using ShinyShoe.Logging;

namespace Stoker.Base
{
    public class ConsoleLogger(ManualLogSource logger) : ILogProvider
    {
        private readonly ManualLogSource logger = logger;

        public void CloseLog() {}

        public void Debug(string log, LogOptions options) {
            logger.LogDebug(log);
        }

        public void Debug(string log) {
            logger.LogDebug(log);
        }

        public void Error(string log, LogOptions options) {
            logger.LogError(log);
        }

        public void Error(string log) {
            logger.LogError(log);
        }

        public void Info(string log, LogOptions options) {
            logger.LogInfo(log);
        }

        public void Info(string log) {
            logger.LogInfo(log);
        }

        public void Log(string message)
        {
            logger.LogInfo(message);
        }

        public void Verbose(string log, LogOptions options)
        {
            logger.LogDebug(log);
        }

        public void Verbose(string log) {
            logger.LogDebug(log);
        }

        public void Warning(string log, LogOptions options)
        {
            logger.LogWarning(log);
        }

        public void Warning(string log) {
            logger.LogWarning(log);
        }
    }
}
