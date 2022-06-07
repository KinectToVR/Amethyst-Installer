using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace amethyst_installer_gui
{
    /// <summary>
    /// A logging class which mimics GLOG's trace style. While not highly configurable, this is
    /// designed to be a simple drag and drop logger which relies solely on built-in C# APIs
    /// </summary>
    public static class Logger
    {

        // This is used instead of Thread.CurrentThread.ManagedThreadId since it returns the OS thread rather than the managed thread
        // Consider using ManagedThreadId instead of this if you have to run this on non-Windows platforms
        // https://stackoverflow.com/a/1679270
        [DllImport("Kernel32", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        private static extern int GetCurrentWin32ThreadId();

        public static string LogFilePath;

        #region Log Functions

        public static void Info(string text, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
        {
            LogInternal(FormatToLogMessage(text, "I", lineNumber, filePath, memberName));
        }

        public static void Info(object obj, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
        {
            LogInternal(FormatToLogMessage(obj.ToString(), "I", lineNumber, filePath, memberName));
        }

        public static void Warn(string text, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
        {
            LogInternal(FormatToLogMessage(text, "W", lineNumber, filePath, memberName), ConsoleColor.Yellow);
        }

        public static void Warn(object obj, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
        {
            LogInternal(FormatToLogMessage(obj.ToString(), "W", lineNumber, filePath, memberName), ConsoleColor.Yellow);
        }

        public static void Error(string text, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
        {
            LogInternal(FormatToLogMessage(text, "E", lineNumber, filePath, memberName), ConsoleColor.Red);
        }

        public static void Error(object obj, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
        {
            LogInternal(FormatToLogMessage(obj.ToString(), "E", lineNumber, filePath, memberName), ConsoleColor.Red);
        }

        public static void Fatal(string text, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
        {
            LogInternal(FormatToLogMessage(text, "F", lineNumber, filePath, memberName), ConsoleColor.DarkRed);
        }

        public static void Fatal(object obj, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
        {
            LogInternal(FormatToLogMessage(obj.ToString(), "F", lineNumber, filePath, memberName), ConsoleColor.DarkRed);
        }

        #endregion

        #region Logger Internals

        /// <summary>
        /// Initializes the logger
        /// </summary>
        public static void Init(string filePath = "")
        {
            if (filePath == "")
                LogFilePath = $"{Assembly.GetCallingAssembly().GetName()}_{DateTime.Now.ToString("yyyyMMdd-hhmmss.ffffff")}.log";
            else
                LogFilePath = filePath;

            LogFilePath = Path.GetFullPath(filePath);
            string dir = Path.GetFullPath(Path.GetDirectoryName(LogFilePath));

            bool loggingPathDidntExist = false;
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
                loggingPathDidntExist = true;
            }

            // Create the file, rest of the methods will append
            File.Create(LogFilePath).Close();

            if (loggingPathDidntExist)
                LogInternal($"Created logging directory at \"{dir}\"");
            LogInternal($"Log file created at: {DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")}");
            LogInternal($"Running on machine: {Environment.MachineName}");
            LogInternal("Running duration (h:mm:ss): 0:00:00");
            LogInternal("Log line format: [IWEF]yyyyMMdd hh:mm:ss.ffffff threadid file::member:line] msg");
        }

        private static string FormatToLogMessage(string message, string level, int lineNumber, string filePath, string memberName)
        {
            return $"{level}{DateTime.Now.ToString("yyyyMMdd hh:mm:ss.ffffff")} {GetCurrentWin32ThreadId()} {Path.GetFileName(filePath)}::{memberName}:{lineNumber}] {message}";
        }

        private static void LogInternal(string message)
        {
            if (LogFilePath == null)
                throw new InvalidOperationException("Tried logging something without calling Logger.Init()! Aborting...");
            Console.ResetColor();
            Console.WriteLine(message);
            File.AppendAllLines(LogFilePath, new[] { message });
        }

        private static void LogInternal(string message, ConsoleColor color)
        {
            if (LogFilePath == null)
                throw new InvalidOperationException("Tried logging something without calling Logger.Init()! Aborting...");
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            File.AppendAllLines(LogFilePath, new[] { message });
        }
        
        #endregion
    }
}