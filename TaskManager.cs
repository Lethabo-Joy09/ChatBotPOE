using System;
using System.Collections.Generic;
using System.Data;

namespace ChatBotPOE
{
    /// <summary>
    /// TaskManager handles all business logic for cybersecurity tasks.
    /// It coordinates between the DatabaseHelper (data layer) and the UI.
    /// Manages adding tasks, marking them complete, deleting, and checking due reminders.
    /// </summary>
    public class TaskManager
    {
        private DatabaseHelper db = new DatabaseHelper();
        private ActivityLogger logger = ActivityLogger.Instance;

        /// <summary>
        /// Adds a new task with an optional reminder.
        /// Parses natural language reminder phrases like "in 3 days" or "tomorrow".
        /// The reminderText parameter is now nullable to fix CS8625.
        /// </summary>
        public bool AddTask(string title, string description, string? reminderText = null)
        {
            DateTime? reminderDate = null;
            if (!string.IsNullOrEmpty(reminderText))
            {
                reminderDate = ParseReminder(reminderText);
            }

            bool success = db.AddTask(title, description, reminderDate);
            if (success)
            {
                logger.Log($"Task added: '{title}'" + (reminderDate.HasValue ? $" with reminder on {reminderDate.Value}" : ""));
            }
            return success;
        }

        /// <summary>
        /// Parses natural language time expressions into DateTime objects.
        /// Supports: "tomorrow", "in 3 days", "in 2 weeks", "in 1 month".
        /// Returns null if no valid time expression is detected.
        /// </summary>
        private DateTime? ParseReminder(string input)
        {
            input = input.ToLower();
            if (input.Contains("tomorrow"))
                return DateTime.Today.AddDays(1);
            if (input.Contains("in "))
            {
                var parts = input.Split(' ');
                if (parts.Length >= 3 && int.TryParse(parts[1], out int num))
                {
                    if (input.Contains("day")) return DateTime.Today.AddDays(num);
                    if (input.Contains("week")) return DateTime.Today.AddDays(num * 7);
                    if (input.Contains("month")) return DateTime.Today.AddMonths(num);
                }
            }
            return null;
        }

        public DataTable GetAllTasks() => db.GetAllTasks();

        public bool CompleteTask(int id)
        {
            bool ok = db.CompleteTask(id);
            if (ok) logger.Log($"Task completed (ID {id})");
            return ok;
        }

        public bool DeleteTask(int id)
        {
            bool ok = db.DeleteTask(id);
            if (ok) logger.Log($"Task deleted (ID {id})");
            return ok;
        }

        public List<string> GetDueReminders()
        {
            var dt = db.GetAllTasks();
            var due = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                if (row["reminder_date"] != DBNull.Value && Convert.ToBoolean(row["is_completed"]) == false)
                {
                    DateTime rem = Convert.ToDateTime(row["reminder_date"]);
                    if (rem <= DateTime.Now)
                    {
                        due.Add($"Reminder: {row["title"]} - {row["description"]}");
                    }
                }
            }
            return due;
        }
    }
}