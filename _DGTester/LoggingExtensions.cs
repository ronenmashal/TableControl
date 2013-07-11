using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using log4net;
using log4net.Core;

namespace MagicSoftware.Common.Utils
{
   /// <summary>
   /// log4net extensions
   /// </summary>
   public static class LoggingExtensions
   {
      static string separatorLine;

      static LoggingExtensions()
      {
         StringBuilder sepLineBuilder = new StringBuilder();
         sepLineBuilder.Append('-', 80);
         separatorLine = sepLineBuilder.ToString();

         log4net.GlobalContext.Properties["indent"] = "";
      }

      /// <summary>
      /// Logs a message at a certain level. The purpose of this method is to allow logging of the
      /// same message in different log levels, depending on the current state.
      /// You should normally use the default methods provided by ILog, such as 'log.Info', 'log.Debug', etc.
      /// </summary>
      /// <param name="log">The ILog instance on which logging will take action.</param>
      /// <param name="level">The log4net.Core.Level of the message.</param>
      /// <param name="message">The message to write to the log.</param>
      /// <param name="args">Message arguments.</param>
      public static void LogMessage(this ILog log, Level level, string message, params object[] args)
      {
         if (log.Logger.IsEnabledFor(level))
         {
            string formattedMessage;
            if (args != null && args.Length > 0)
               formattedMessage = String.Format(message, args);
            else
               formattedMessage = message;
            log.Logger.Log(typeof(LoggingExtensions), level, formattedMessage, null);
         }
      }

      /// <summary>
      /// Logs a result that can be either success (TRUE) or failure (FALSE). A successful operation
      /// will be logged at 'debug' level and a failure will be logged as an 'error'.
      /// </summary>
      /// <param name="log">The log which logs the operation.</param>
      /// <param name="result">The value of the operation's result.</param>
      /// <param name="operationDescription">A string describing the operation that was taken.</param>
      public static void LogBooleanResult(this ILog log, bool result, string operationDescription)
      {
         LogBooleanResult(log, result, operationDescription, new object[] { });
      }

      /// <summary>
      /// Logs a result that can be either success (TRUE) or failure (FALSE). A successful operation
      /// will be logged at 'debug' level and a failure will be logged as an 'error'.
      /// </summary>
      /// <param name="log">The log which logs the operation.</param>
      /// <param name="result">The value of the operation's result.</param>
      /// <param name="operationDescription">A string describing the operation that was taken.</param>
      /// <param name="args">Arguments of the operationDescription string.</param>
      public static void LogBooleanResult(this ILog log, bool result, string operationDescription, params object[] args)
      {
         if (result)
         {
            log.DebugFormat("Successfully executed: " + operationDescription, args);
         }
         else
         {
            log.ErrorFormat("Operation failed: " + operationDescription, args);
         }
      }

      /// <summary>
      /// Produce a log message containing the full stack trace, which begins from the
      /// method that invoked this method.
      /// </summary>
      /// <param name="log">The log object on which to log the stack trace.</param>
      /// <param name="level">The level threshold for printing the stack trace.</param>
      public static void LogStackTrace(this ILog log, Level level)
      {
         LogStackTrace(log, level, uint.MaxValue);
      }

      /// <summary>
      /// Produce a log message containing the stack trace, which begins from the
      /// method that invoked this method, and includes up to framesToPrint stack frames.
      /// </summary>
      /// <param name="log">The log object on which to log the stack trace.</param>
      /// <param name="level">The level threshold for printing the stack trace.</param>
      /// <param name="framesToPrint">The number of stack frames to include in the stack trace.</param>
      /// <param name="framesToSkip">The number of stack frames to skip. The default is 1, which means that this
      /// method (LogStackTrace) will not be part of the stack trace.</param>
      public static void LogStackTrace(this ILog log, Level level, uint framesToPrint, int framesToSkip = 1)
      {
         if (log.Logger.IsEnabledFor(level))
         {
            StringBuilder formattedMessage = new StringBuilder("Stack trace:");
            StackTrace stackTrace = new StackTrace(framesToSkip, true);
            StackFrame[] frames = stackTrace.GetFrames();
            foreach (StackFrame frame in frames)
            {
               framesToPrint--;
               if (framesToPrint == 0)
               {
                  formattedMessage.Append("\n\t...");
                  break;
               }
               MethodBase method = frame.GetMethod();
               formattedMessage.AppendFormat("\n\t{0} ({1}:{2})", (method.DeclaringType.Name + ". " + method.ToString()), frame.GetFileName(), frame.GetFileLineNumber());
            }
            log.Logger.Log(typeof(LoggingExtensions), level, formattedMessage, null);
         }
      }

      /// <summary>
      /// Print a separation line to mark the beginning of a process in the log.
      /// </summary>
      /// <param name="log"></param>
      /// <param name="level"></param>
      public static void LogSeparator(this ILog log, Level level)
      {
         LogMessage(log, level, separatorLine.ToString());
      }

      /// <summary>
      /// Prints a debug level message with the calling method information.
      /// </summary>
      /// <param name="log">The log object on which to perform the action.</param>
      /// <param name="methodArgValues">The values of the method's arguments (optional).</param>
      public static void DebugLogMethod(this ILog log, params object[] methodArgValues)
      {
         if (log.Logger.IsEnabledFor(Level.Debug))
         {
            StackTrace trace = new StackTrace(1, true);
            StackFrame frame = trace.GetFrame(0);
            MethodBase method = frame.GetMethod();
            LogMethod(log, Level.Debug, method, methodArgValues);
         }
      }

      /// <summary>
      /// Print the method name to the log and, optionally, its argument values.
      /// This method will add a log message which contains the method fully qualified name
      /// and its argument list.
      /// If the methodArgValues are also specified, and their count matches the method argument
      /// count, the values are also printed to the log.
      /// </summary>
      /// <param name="log">The log object on which to perform the action.</param>
      /// <param name="level">The level in which to log the message.</param>
      /// <param name="method">The information of the method to log.</param>
      /// <param name="methodArgValues">The values of the method's arguments.</param>
      public static void LogMethod(this ILog log, Level level, MethodBase method, params object[] methodArgValues)
      {
         if (log.Logger.IsEnabledFor(level))
         {
            StringBuilder methodDesc = new StringBuilder(">>>>> ");
            methodDesc.AppendFormat("{0}.{1}(", method.DeclaringType.FullName, method.Name);
            bool isFirst = true;
            ParameterInfo[] paramInfoList = method.GetParameters();
            bool bPrintValues = false;
            if (paramInfoList.Length == methodArgValues.Length)
               bPrintValues = true;
            int paramIndex = 0;
            foreach (ParameterInfo pi in paramInfoList)
            {
               if (isFirst)
                  isFirst = false;
               else
                  methodDesc.Append(", ");
               methodDesc.Append(pi.Name);
               if (bPrintValues)
               {
                  methodDesc.AppendFormat("={0}", methodArgValues[paramIndex]);
                  paramIndex++;
               }
            }
            methodDesc.Append(")");
            LogMessage(log, level, methodDesc.ToString());
         }
      }

      /// <summary>
      /// Increases the log's indetation property. The method returns a disposable
      /// object, which, when disposed, removes the indentation.
      /// </summary>
      /// <returns>An IDisposable object that can be used in a 'using' clause.</returns>
      /// <example>
      /// The following is an example of a layout configuration. Note the %P{indent} before the %message.
      /// <code lang="xml">
      /// <layout type="log4net.Layout.PatternLayout">
      /// <conversionPattern value="[%-5level] %date{HH:mm:ss.fff} %50ndc> %P{indent}%message%newline"/>
      /// </layout>
      /// </code>
      /// 
      /// Then in the code you'd write something like this:
      /// <code>
      /// using (LoggingExtensions.Indet())
      /// {
      ///   log.Debug("An indented message");
      /// }
      /// log.Debug("This message will not be indented.");
      /// </code>
      /// </example>
      public static IDisposable Indent()
      {
         return new Indentator();
      }

      class Indentator : IDisposable
      {
         static string INDENTATION = "   ";

         public Indentator()
         {
            log4net.GlobalContext.Properties["indent"] = ((string)log4net.GlobalContext.Properties["indent"]) + INDENTATION;
         }

         public void Dispose()
         {
            log4net.GlobalContext.Properties["indent"] = ((string)log4net.GlobalContext.Properties["indent"]).Substring(INDENTATION.Length);
         }
      }
   }
}
