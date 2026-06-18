using System.Collections.Generic;
using System.Linq;

namespace ChatBotPOE
{
    /// <summary>
    /// SentimentAnalyzer detects the emotional tone of user messages.
    /// Uses a simple keyword-based approach: positive words like "good", "great"
    /// and negative words like "bad", "terrible".
    /// Returns "positive", "negative", or "neutral".
    /// </summary>
    public static class SentimentAnalyzer
    {
        // List of words that indicate positive sentiment
        private static HashSet<string> positiveWords = new HashSet<string> { "good", "great", "awesome", "nice", "happy", "thanks", "love" };

        // List of words that indicate negative sentiment
        private static HashSet<string> negativeWords = new HashSet<string> { "bad", "terrible", "awful", "hate", "annoying", "stupid" };

        /// <summary>
        /// Analyzes the user's input and returns the detected sentiment.
        /// Counts positive and negative words, then compares the counts.
        /// </summary>
        public static string Analyze(string input)
        {
            var words = input.ToLower().Split(' ');
            int pos = words.Count(w => positiveWords.Contains(w));
            int neg = words.Count(w => negativeWords.Contains(w));
            if (pos > neg) return "positive";
            else if (neg > pos) return "negative";
            else return "neutral";
        }
    }
}
