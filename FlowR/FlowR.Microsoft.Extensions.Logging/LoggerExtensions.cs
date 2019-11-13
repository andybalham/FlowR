using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FlowR.Microsoft.Extensions.Logging
{
    static class LoggerExtensions
    {
        #region Member declarations

        private const string FlowContextTemplate = "{FlowRootInstanceId} {FlowName}:{FlowStepName} - ";

        #endregion

        #region Public methods

        public static void LogDebug(this ILogger logger, FlowContext flowContext, string messageTemplate, params object[] args)
        {
            if (logger?.IsEnabled(LogLevel.Debug) != true) return;

            var argList = GetBaseArgList(flowContext, args);

            logger.LogDebug(FlowContextTemplate + messageTemplate, argList.ToArray());
        }

        public static void LogInformation(this ILogger logger, FlowContext flowContext, string messageTemplate, params object[] args)
        {
            if (logger?.IsEnabled(LogLevel.Information) != true) return;

            var argList = GetBaseArgList(flowContext, args);

            logger.LogInformation(FlowContextTemplate + messageTemplate, argList.ToArray());
        }

        public static void LogError(this ILogger logger, FlowContext flowContext, string messageTemplate, params object[] args)
        {
            if (logger?.IsEnabled(LogLevel.Error) != true) return;

            var argList = GetBaseArgList(flowContext, args);

            logger.LogError(FlowContextTemplate + messageTemplate, argList.ToArray());
        }

        public static void LogError(this ILogger logger, FlowContext flowContext, Exception exception, string messageTemplate, params object[] args)
        {
            if (logger?.IsEnabled(LogLevel.Error) != true) return;

            var argList = GetBaseArgList(flowContext, args);

            logger.LogError(exception, FlowContextTemplate + messageTemplate, argList.ToArray());
        }

        public static object EvalIf(this ILogger logger, LogLevel logLevel, Func<string> evalFunc)
        {
            return logger?.IsEnabled(logLevel) == true ? evalFunc() : default;
        }

        public static object EvalIfDebug(this ILogger logger, Func<string> evalFunc)
        {
            return logger?.EvalIf(LogLevel.Debug, evalFunc);
        }

        public static object EvalIfInformation(this ILogger logger, Func<string> evalFunc)
        {
            return logger?.EvalIf(LogLevel.Information, evalFunc);
        }

        public static object EvalIfError(this ILogger logger, Func<string> evalFunc)
        {
            return logger?.EvalIf(LogLevel.Error, evalFunc);
        }

        #endregion

        #region Private methods

        private static List<object> GetBaseArgList(FlowContext flowContext, object[] args)
        {
            var argList = new List<object>(new[]
            {
                flowContext.FlowRootInstanceId, 
                flowContext.FlowName, 
                flowContext.FlowStepName ?? "GOTO"
            });

            argList.AddRange(args);
            
            return argList;
        }

        #endregion
    }
}
