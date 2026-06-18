using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatBotPOE
{
    /// <summary>
    /// ActivityLogger tracks all significant actions performed by the chatbot.
    /// Uses a singleton pattern so only one instance exists throughout the application.
    /// Actions include tasks added, reminders set, quiz activity, and NLP commands.
    /// </summary>
    public class ActivityLogger
    {
        // Singleton instance (lazy initialization)
        private static ActivityLogger? instance = null;

        // List of log entries with timestamps
        private List<string> logEntries = new List<string>();

        // Private constructor prevents external instantiation
        private ActivityLogger() { }

        /// <summary>
        /// Provides access to the single instance of the logger.
        /// Creates it if it doesn't exist yet.
        /// </summary>
        public static ActivityLogger Instance => instance ?? (instance = new ActivityLogger());

        /// <summary>
        /// Adds a new entry to the activity log with a timestamp.
        /// Format: [YYYY-MM-DD HH:MM:SS] Action description
        /// Keeps only the last 100 entries to prevent memory issues.
        /// </summary>
        public void Log(string action)
        {
            string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {action}";
            logEntries.Add(entry);
            if (logEntries.Count > 100) logEntries.RemoveAt(0);
        }

        /// <summary>
        /// Retrieves the most recent log entries (default: last 10).
        /// Used when the user types "Show activity log".
        /// </summary>
        public List<string> GetRecentLogs(int count = 10)
        {
            return logEntries.TakeLast(count).ToList();
        }

        /// <summary>
        /// Returns a copy of all log entries.
        /// Useful for the "Show More" button to display full history.
        /// </summary>
        public List<string> GetAllLogs() => new List<string>(logEntries);
    }
}