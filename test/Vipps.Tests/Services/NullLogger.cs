using EPiServer.Logging;
using System;


namespace Vipps.Test.Services
{
    public class NullLogger : ILogger
    {
        public bool IsEnabled(Level level)
        {
            return false;
        }

        public void Log<TState, TException>(Level level, TState state, TException exception, Func<TState, TException, string> messageFormatter, Type boundaryType) where TException : Exception
        {
            
        }
    }
}
