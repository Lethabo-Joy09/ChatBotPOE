using System.Collections.Generic;

namespace ChatBotPOE
{
    /// <summary>
    /// MemoryManager stores user information such as names and preferences.
    /// It acts as the chatbot's short-term memory to recall user details.
    /// Uses a static dictionary so memory persists across the entire application session.
    /// </summary>
    public static class MemoryManager
    {
        // Dictionary to store key-value pairs (e.g., "name" -> "Alice")
        private static Dictionary<string, object> memory = new Dictionary<string, object>();

        /// <summary>
        /// Stores a piece of information with a specific key.
        /// Example: Remember("name", "Alice") allows the bot to recall the user's name later.
        /// </summary>
        public static void Remember(string key, object value)
        {
            memory[key] = value;
        }

        /// <summary>
        /// Retrieves stored information by its key.
        /// Returns null if the key doesn't exist, allowing safe checking.
        /// Changed return type to object? to indicate possible null (fixes CS8603).
        /// Example: string userName = (string)MemoryManager.Recall("name");
        /// </summary>
        public static object? Recall(string key)
        {
            return memory.ContainsKey(key) ? memory[key] : null;
        }

        /// <summary>
        /// Generates a human-readable summary of everything the bot remembers.
        /// Used when the user asks "What do you remember?" or "Summary".
        /// </summary>
        public static string GetSummary()
        {
            if (memory.Count == 0)
                return "I don't remember anything yet.";

            string summary = "I remember: ";
            foreach (var kvp in memory)
            {
                summary += $"{kvp.Key}: {kvp.Value}; ";
            }
            return summary;
        }
    }
}