using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBotPOE
{
    /// <summary>
    /// NLPSimulator uses keyword detection and pattern matching to understand
    /// different ways users phrase their requests.
    /// Recognizes variations like "Add task", "Create a task", "Remind me", etc.
    /// Prevents the bot from frequently saying "I didn't understand that".
    /// </summary>
    public class NLPSimulator
    {
        // Maps user phrases to specific action categories
        // Each action type has multiple keywords/phrases that trigger it
        private Dictionary<string, List<string>> actionKeywords = new Dictionary<string, List<string>>
        {
            { "task", new List<string> { "add task", "create task", "new task", "task", "to-do" } },
            { "reminder", new List<string> { "remind me", "set reminder", "reminder", "remember" } },
            { "quiz", new List<string> { "quiz", "game", "test", "play quiz" } },
            { "password", new List<string> { "password", "passphrase", "credentials" } },
            { "phishing", new List<string> { "phishing", "scam", "fraud" } }
        };

        /// <summary>
        /// Analyzes user input and determines the most likely action intent.
        /// Returns the action category (task, reminder, quiz, etc.) or "unknown".
        /// </summary>
        public string DetectIntent(string userInput)
        {
            string lower = userInput.ToLower();
            foreach (var kvp in actionKeywords)
            {
                foreach (string keyword in kvp.Value)
                {
                    if (lower.Contains(keyword))
                        return kvp.Key;
                }
            }
            return "unknown";
        }

        /// <summary>
        /// Removes action words from the user's message to extract the actual task description.
        /// Example: "Add task to enable 2FA" becomes "enable 2FA".
        /// </summary>
        public string ExtractDescription(string userInput, string actionType)
        {
            string cleaned = userInput;
            string[] patterns = new string[]
            {
                @"add task", @"create task", @"new task", @"remind me to", @"set reminder", @"remind me", @"add reminder"
            };
            foreach (string pat in patterns)
            {
                cleaned = Regex.Replace(cleaned, pat, "", RegexOptions.IgnoreCase);
            }
            cleaned = Regex.Replace(cleaned, @"^\s*to\s+", ""); // Remove "to" at start
            return cleaned.Trim();
        }

        /// <summary>
        /// Detects time-related phrases like "in 3 days", "tomorrow", "next week".
        /// Returns the matched phrase or an empty string if none found.
        /// Used to set reminders based on natural language.
        /// </summary>
        public string ExtractReminderTime(string input)
        {
            // Regular expression matches common time expressions
            Match m = Regex.Match(input, @"(in\s+\d+\s+(day|days|week|weeks|month|months)|tomorrow|next\s+\w+)", RegexOptions.IgnoreCase);
            return m.Success ? m.Value : string.Empty;
        }
    }
}