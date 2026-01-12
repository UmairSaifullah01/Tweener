using UnityEngine;

namespace THEBADDEST.Tweening2.Core
{
    /// <summary>
    /// Centralized logging utility for THEBADDEST.Tweening2 with [Tween] prefix
    /// </summary>
    internal static class TweenLog
    {
        private const string PREFIX = "[Tween]";

        /// <summary>
        /// If false, all logging will be skipped. Set to true to enable logging (default: true)
        /// </summary>
        public static bool enabled = true;

        /// <summary>
        /// Logs an informational message
        /// </summary>
        public static void Log(object message)
        {
            if (!enabled) return;
            Debug.Log($"{PREFIX} {message}");
        }

        /// <summary>
        /// Logs an informational message with context
        /// </summary>
        public static void Log(object message, Object context)
        {
            if (!enabled) return;
            Debug.Log($"{PREFIX} {message}", context);
        }

        /// <summary>
        /// Logs a warning message
        /// </summary>
        public static void LogWarning(object message)
        {
            if (!enabled) return;
            Debug.LogWarning($"{PREFIX} {message}");
        }

        /// <summary>
        /// Logs a warning message with context
        /// </summary>
        public static void LogWarning(object message, Object context)
        {
            if (!enabled) return;
            Debug.LogWarning($"{PREFIX} {message}", context);
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        public static void LogError(object message)
        {
            if (!enabled) return;
            Debug.LogError($"{PREFIX} {message}");
        }

        /// <summary>
        /// Logs an error message with context
        /// </summary>
        public static void LogError(object message, Object context)
        {
            if (!enabled) return;
            Debug.LogError($"{PREFIX} {message}", context);
        }

        /// <summary>
        /// Logs an exception
        /// </summary>
        public static void LogException(System.Exception exception)
        {
            if (!enabled) return;
            Debug.LogException(exception);
        }

        /// <summary>
        /// Logs an exception with context
        /// </summary>
        public static void LogException(System.Exception exception, Object context)
        {
            if (!enabled) return;
            Debug.LogException(exception, context);
        }
    }
}
