using System.Collections.Generic;

namespace ChatBotPOE
{
    /// <summary>
    /// Manages the cybersecurity quiz game with 12 questions covering phishing,
    /// password safety, safe browsing, social engineering, and other topics.
    /// Supports both Multiple Choice (MC) and True/False (TF) question types.
    /// </summary>
    public class QuizEngine
    {
        /// <summary>
        /// Represents a single quiz question with its options and correct answer.
        /// </summary>
        public class Question
        {
            public string Text { get; set; } = string.Empty;          // The question text
            public string Type { get; set; } = string.Empty;          // "MC" or "TF"
            public List<string> Options { get; set; } = new List<string>();  // Available choices
            public int CorrectIndex { get; set; }                    // Index of correct option (0-based)
            public string Explanation { get; set; } = string.Empty;  // Educational feedback
        }

        // Private list of all quiz questions
        private List<Question> _questions = new List<Question>();

        /// <summary>
        /// Public read-only property to access questions (prevents external modification).
        /// </summary>
        public List<Question> Questions => _questions;

        // Current position in the quiz
        private int currentIndex = 0;

        // User's score tracking
        public int Score { get; private set; } = 0;

        // Indicates whether the quiz has been completed
        public bool IsFinished { get; private set; } = false;

        /// <summary>
        /// Constructor initializes the question bank with 12 cybersecurity questions.
        /// Covers phishing, password safety, safe browsing, social engineering,
        /// two-factor authentication, malware, and firewalls.
        /// </summary>
        public QuizEngine()
        {
            _questions = new List<Question>
            {
                new Question
                {
                    Text = "What should you do if you receive an email asking for your password?",
                    Type = "MC",
                    Options = new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                    CorrectIndex = 2,
                    Explanation = "Reporting phishing emails helps prevent scams and protects others."
                },
                new Question
                {
                    Text = "Which of the following is a strong password?",
                    Type = "MC",
                    Options = new List<string> { "password123", "qwerty", "P@ssw0rd!2024", "abc" },
                    CorrectIndex = 2,
                    Explanation = "A strong password includes uppercase, lowercase, numbers, and special characters."
                },
                new Question
                {
                    Text = "True or False: It's safe to use the same password for multiple accounts.",
                    Type = "TF",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "Using the same password across accounts increases risk; if one is breached, all are vulnerable."
                },
                new Question
                {
                    Text = "What is a common sign of a phishing attempt?",
                    Type = "MC",
                    Options = new List<string> { "Personalized greeting", "Urgent language asking for action", "Known sender address", "Professional design" },
                    CorrectIndex = 1,
                    Explanation = "Phishing often creates urgency to trick you into acting without thinking."
                },
                new Question
                {
                    Text = "Which of these is a safe browsing practice?",
                    Type = "MC",
                    Options = new List<string> { "Clicking on pop-up ads", "Using public Wi-Fi without VPN", "Checking for HTTPS before entering personal info", "Downloading from untrusted sites" },
                    CorrectIndex = 2,
                    Explanation = "HTTPS encrypts data between you and the website, keeping it secure."
                },
                new Question
                {
                    Text = "True or False: Social engineering attacks always use technical hacking methods.",
                    Type = "TF",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "Social engineering manipulates people psychologically, not just through technical means."
                },
                new Question
                {
                    Text = "What is the best way to store passwords?",
                    Type = "MC",
                    Options = new List<string> { "In a plain text file", "In a password manager", "Written on a sticky note", "In your email drafts" },
                    CorrectIndex = 1,
                    Explanation = "Password managers securely encrypt and store your passwords."
                },
                new Question
                {
                    Text = "True or False: Two-factor authentication (2FA) adds an extra layer of security.",
                    Type = "TF",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 0,
                    Explanation = "2FA requires a second verification step, making unauthorized access harder."
                },
                new Question
                {
                    Text = "What should you do if you suspect your computer has malware?",
                    Type = "MC",
                    Options = new List<string> { "Ignore it", "Run a full antivirus scan", "Delete system files", "Disconnect from the internet immediately and run a scan" },
                    CorrectIndex = 3,
                    Explanation = "Disconnecting limits damage, then scanning removes the threat."
                },
                new Question
                {
                    Text = "Which of these is a phishing red flag in an email?",
                    Type = "MC",
                    Options = new List<string> { "Correct spelling", "Generic greeting (e.g., Dear Customer)", "Known sender", "Clear subject line" },
                    CorrectIndex = 1,
                    Explanation = "Phishing often uses generic greetings to target many people at once."
                },
                new Question
                {
                    Text = "True or False: It is safe to click on links in emails from unknown senders if they look interesting.",
                    Type = "TF",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "Links in unknown emails can lead to phishing sites or malware downloads."
                },
                new Question
                {
                    Text = "What is the primary purpose of a firewall?",
                    Type = "MC",
                    Options = new List<string> { "To speed up the internet", "To monitor and control incoming/outgoing network traffic", "To clean up disk space", "To install updates" },
                    CorrectIndex = 1,
                    Explanation = "Firewalls block unauthorized access while permitting authorized communications."
                }
            };
        }

        /// <summary>
        /// Retrieves the current question being displayed.
        /// Returns null if the quiz has ended.
        /// </summary>
        public Question? GetCurrentQuestion()
        {
            if (currentIndex < _questions.Count)
                return _questions[currentIndex];
            return null;
        }

        /// <summary>
        /// Processes the user's selected answer and updates the score.
        /// Returns true if the answer was correct, false otherwise.
        /// Automatically advances to the next question.
        /// </summary>
        public bool AnswerCurrent(int selectedIndex)
        {
            if (IsFinished || currentIndex >= _questions.Count) return false;
            var q = _questions[currentIndex];
            bool correct = (selectedIndex == q.CorrectIndex);
            if (correct) Score++;
            currentIndex++;
            if (currentIndex >= _questions.Count) IsFinished = true;
            return correct;
        }

        /// <summary>
        /// Generates encouraging feedback based on the user's final score.
        /// High scores (80%+) get "cybersecurity pro" praise.
        /// Low scores encourage further learning.
        /// </summary>
        public string GetFinalFeedback()
        {
            double percent = (double)Score / _questions.Count * 100;
            if (percent >= 80) return "Great job! You're a cybersecurity pro!";
            else if (percent >= 50) return "Good effort! Keep learning to stay safe online!";
            else return "Keep learning to stay safe online! Review the basics.";
        }

        /// <summary>
        /// Resets the quiz to its initial state for a new game.
        /// Clears score and sets current question back to the first one.
        /// </summary>
        public void Reset()
        {
            currentIndex = 0;
            Score = 0;
            IsFinished = false;
        }
    }
}