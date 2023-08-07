// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using RealityCollective.Definitions.Utilities;
using System;
using System.Text;
using UnityEngine;

namespace RealityCollective.Utilities
{
    /// <summary>
    /// Logging service to capture Log data and make it presentable, also includes helpers to upload logs to an external provider
    /// </summary>
    public static class StaticLogger
    {
        #region Events
        /// <summary>
        /// Public delegate to expose when logging occurs (Cannot use UnityEvent because Unity may not be initialised when logging starts).
        /// </summary>
        public delegate void SampleEventHandler(object sender);

        /// <summary>
        /// Public log event, fired when logging occurs (Cannot use UnityEvent because Unity may not be initialised when logging starts).
        /// </summary>
        public static event SampleEventHandler OnLogEvent;
        #endregion Events

        #region Private Properties
        private static StringBuilder sb = new StringBuilder();
        private static int logIndex = 0;
        private static FilterLogType currentLogFilter = FilterLogType.All;
        private static bool pauseLog = false;
        #endregion Private Properties

        #region Public Properties
        /// <summary>
        /// Current Log content.
        /// </summary>
        public static string CurrentLog => sb.ToString();

        /// <summary>
        /// Which type of Log data to filter for.
        /// </summary>
        public static FilterLogType CurrentLogFilter => currentLogFilter;

        /// <summary>
        /// Should logs be written to the Unity debugger.
        /// </summary>
        public static bool DebugMode { get; set; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>
        /// Function to change the current logging type, thread safe.
        /// </summary>
        public static void UpdateLogFilterType(FilterLogType logType)
        {
            currentLogFilter = logType;
        }

        /// <summary>
        /// Pass data to the Log Service to store in the runtime log.
        /// </summary>
        /// <param name="message">The log message to write.</param>
        /// <param name="logType">The type of log to report, message, warning or error.</param>
        /// <param name="appLog">Is the log event already in the Unity debugger, if so, skip recording it and just add it to the text log.</param>
        /// <param name="includeStackTrace">Should a stack trace also be generated when logging.</param>
        public static void Log(string message, LogType logType = LogType.Log, bool appLog = false, bool includeStackTrace = true)
        {
            if (!pauseLog)
            {
                if (!appLog)
                {
                    Console.WriteLine(message);
                    if (DebugMode)
                    {
                        Debug.LogFormat(logType, includeStackTrace ? LogOption.None : LogOption.NoStacktrace, null, message);
                    }
                    return;
                }

                sb.AppendLine($"{logIndex}-{message}");
                logIndex++;

                OnLogEvent?.Invoke(null);
            }
        }

        /// <summary>
        /// Log overload to log a message as a warning.
        /// </summary>
        /// <param name="message">The log message to write.</param>
        public static void LogWarning(string message)
        {
            Log(message, LogType.Warning);
        }

        /// <summary>
        /// Log overload to log a message as an error.
        /// </summary>
        /// <param name="message">The log message to write.</param>
        public static void LogError(string message)
        {
            Log(message, LogType.Error);
        }

        /// <summary>
        /// Clear the current state of the Log.
        /// </summary>
        public static void ClearLog()
        {
            sb.Clear();
            Log($"{currentLogFilter} - Log cleared");
        }

        /// <summary>
        /// Pause the collection and storage of logs.
        /// </summary>
        public static void PauseLog()
        {
            pauseLog = !pauseLog;
        }
        #endregion Public Methods
    }
}