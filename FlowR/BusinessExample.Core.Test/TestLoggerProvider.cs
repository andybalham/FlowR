using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace BusinessExample.Core.Test
{
    public class TestLoggerProvider : ILoggerProvider
    {
        private readonly StringBuilder _loggingOutputBuilder;

        public TestLoggerProvider(StringBuilder loggingOutputBuilder)
        {
            _loggingOutputBuilder = loggingOutputBuilder;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger(_loggingOutputBuilder);
        }

        public void Dispose()
        {
        }
    }

    public class TestLogger : ILogger
    {
        private readonly StringBuilder _loggingOutputBuilder;

        public TestLogger(StringBuilder loggingOutputBuilder)
        {
            _loggingOutputBuilder = loggingOutputBuilder;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new LogScope();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _loggingOutputBuilder.AppendLine(formatter(state, exception));
        }

        private class LogScope : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
